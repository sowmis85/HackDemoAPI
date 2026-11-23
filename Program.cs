
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Application.Interfaces.Repositories;
using HackDemoAPI.Application.Interfaces.Services;
using HackDemoAPI.Application.Services;
using HackDemoAPI.Common;
using HackDemoAPI.Infrastructure;
using HackDemoAPI.Presentation.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

try
{
    var keyVaultUri = new Uri(builder.Configuration["KeyVault:VaultUri"]!);
    var credential = new DefaultAzureCredential();
    builder.Configuration.AddAzureKeyVault(keyVaultUri, credential);
}
catch (Azure.Identity.AuthenticationFailedException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
    throw;
}
catch (Exception ex)
{
    Console.WriteLine($"Key Vault connection failed: {ex.GetType().Name} - {ex.Message}");
    throw;
}
// 2. Retrieve the JWT Signing Key loaded from Key Vault
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("JWT Signing Key not found in Key Vault.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtSigningKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException(ValidationConstant.JWTConfigValidation);
}
// 1. Add JWT Authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey))
    };
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add Logging
builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.AddDebug();
    // Consider Serilog for production
});
// Add HttpClient for Durable Functions
builder.Services.AddHttpClient<IDurableFunctionClient, DurableFunctionClient>();

//Service registrations
builder.Services.AddHttpClient<IAIService, AzureOpenAIService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
// Register the Durable Function Client
builder.Services.AddScoped<IDurableFunctionClient, DurableFunctionClient>();

//Repository registrations
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
//Helper
builder.Services.AddSingleton<IJsonFileHelper, JsonFileHelper>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AngularPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

