namespace ArcadiaTeamsBot.ServiceDesk
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public class ServiceDeskClient : IServiceDeskClient
    {
        private const string requestTypesUrl = "intra/requestTypes";
        private const string currentRequestsUrl = "intra/requests?username=";
        private const string createNewRequestUrl = "intra/request";

        private readonly IHttpClientFactory clientFactory;

        private readonly IEnumerable<ServiceDeskRequestPriorityDTO> priorities = new[]
        {
            new ServiceDeskRequestPriorityDTO
            {
                Key = 1,
                Value = "Low"
            },
            new ServiceDeskRequestPriorityDTO
            {
                Key = 2,
                Value = "Default"
            },
            new ServiceDeskRequestPriorityDTO
            {
                Key = 3,
                Value = "High"
            }
        };

        private readonly ServiceDeskConfiguration serviceDeskConfiguration;

        public ServiceDeskClient(IHttpClientFactory clientFactory, ServiceDeskConfiguration serviceDeskConfiguration)
        {
            this.clientFactory = clientFactory;
            this.serviceDeskConfiguration = serviceDeskConfiguration;
        }

        public Task<IEnumerable<ServiceDeskRequestTypeDTO>> GetRequestTypes(CancellationToken cancellationToken)
        {
            return this.GetByUrl<IEnumerable<ServiceDeskRequestTypeDTO>>($"{this.serviceDeskConfiguration.ApiUrl}{requestTypesUrl}", cancellationToken);
        }

        public Task<IEnumerable<ServiceDeskRequestDTO>> GetCurrentRequests(string username, CancellationToken cancellationToken)
        {
            return this.GetByUrl<IEnumerable<ServiceDeskRequestDTO>>($"{this.serviceDeskConfiguration.ApiUrl}{currentRequestsUrl}{username}", cancellationToken);
        }

        public Task<IEnumerable<ServiceDeskRequestPriorityDTO>> GetPriorities(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.priorities);
        }

        public async Task CreateNewRequest(RequestForCreationDTO requestForCreationDTO, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{this.serviceDeskConfiguration.ApiUrl}{createNewRequestUrl}");

            httpRequest.Headers.Add("x-api-key", this.serviceDeskConfiguration.ApiKey);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            httpRequest.Content = new StringContent(JsonSerializer.Serialize(requestForCreationDTO, options));
            httpRequest.Content.Headers.Remove("Content-Type");
            httpRequest.Content.Headers.Add("Content-Type", "application/json");

            await this.clientFactory.CreateClient().SendAsync(httpRequest, cancellationToken);
        }

        private async Task<T> GetByUrl<T>(string url, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            httpRequest.Headers.Add("x-api-key", this.serviceDeskConfiguration.ApiKey);

            var client = this.clientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return await JsonSerializer.DeserializeAsync<T>(responseBody, options, cancellationToken);
        }
    }
}
