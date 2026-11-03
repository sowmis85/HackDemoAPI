using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.Interfaces.IService;
using System.Text;
using System.Text.Json;

namespace HackDemoAPI.Application.Services
{
    public class AzureOpenAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AzureOpenAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateEventAIDescriptionAsync(AIEventDescRequest request, CancellationToken cancellationToken)
        {
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var deployment = _configuration["AzureOpenAI:DeploymentName"];

            var prompt = $@"
                    Generate a professional event description.
                    Title: {request.Title}
                    Category: {request.Category}
                    Target Audience: {request.TargetAudience}
                    Keywords: {string.Join(", ", request.Keywords)}
                    Description:";

            var payload = new
            {
                messages = new[]
                {
                new
                {
                    role = "system",
                    content = "You are an event management assistant."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
                max_tokens = 300,
                temperature = 0.7
            };

            var url =
                $"{endpoint}/openai/deployments/{deployment}/chat/completions?api-version=2024-02-15-preview";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            requestMessage.Headers.Add("api-key", apiKey);

            requestMessage.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json =
                await response.Content.ReadAsStringAsync(cancellationToken);

            using var document = JsonDocument.Parse(json);

            return document
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }
    }
}
