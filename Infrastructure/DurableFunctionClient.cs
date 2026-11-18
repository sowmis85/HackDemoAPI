using global::HackDemoAPI.Application.DTO.Responses;
using global::HackDemoAPI.Application.Interfaces;

namespace HackDemoAPI.Infrastructure
    {
        public class DurableFunctionClient : IDurableFunctionClient
        {
            private readonly HttpClient _httpClient;
            private readonly IConfiguration _configuration;

            public DurableFunctionClient(HttpClient httpClient, IConfiguration configuration)
            {
                _httpClient = httpClient;
                _configuration = configuration;
            }

            public async Task<DurableInstanceStatusResponse> StartRegistrationOrchestratorAsync(
                string eventId,
                string userId,
                string registrationId,
                CancellationToken cancellationToken)
            
        
        {
                var durableFunctionUrl = _configuration["DurableFunctions:HttpStarterUrl"]
                    ?? throw new InvalidOperationException("DurableFunctions:HttpStarterUrl is not configured.");

                var orchestratorInput = new
                {
                    eventId,
                    userId,
                    registrationId
                };

                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(orchestratorInput),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    durableFunctionUrl,
                    content,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                var statusResponse = System.Text.Json.JsonSerializer.Deserialize<DurableInstanceStatusResponse>(
                    responseContent,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return statusResponse ?? new DurableInstanceStatusResponse();
            }
        }
    }

