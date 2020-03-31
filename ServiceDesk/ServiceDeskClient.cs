namespace ArcadiaTeamsBot.ServiceDesk
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Threading;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public class ServiceDeskClient : IServiceDeskClient
    {
        private const string requestTypesUrl = "https://arcadia-servicedesk-dev31.azurewebsites.net/api/intra/requestTypes";
        private const string currentRequestsUri = "https://arcadia-servicedesk-dev31.azurewebsites.net/api/intra/requests?username=";
        private readonly IHttpClientFactory clientFactory;
        private readonly ServiceDeskRequestPriorityDTO[] priorities = new[] {

                new ServiceDeskRequestPriorityDTO
                {
                    Key = 1,
                    Value="Low"
                },
                new ServiceDeskRequestPriorityDTO
                {
                    Key = 2,
                    Value="Default"
                },
                new ServiceDeskRequestPriorityDTO
                {
                    Key = 3,
                    Value="High"
                },
            };

        public ServiceDeskClient(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<IEnumerable<ServiceDeskRequestTypeDTO>> GetRequestTypes(CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestTypesUrl);

            httpRequest.Headers.Add("x-api-key", "not-installed");

            var client = this.clientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var serviceDeskRequestTypes = await JsonSerializer.DeserializeAsync<IEnumerable<ServiceDeskRequestTypeDTO>>(responseBody, options);

            return serviceDeskRequestTypes;
        }

        public async Task<IEnumerable<ServiceDeskRequestDTO>> GetCurrentRequests(string username, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{currentRequestsUri}{username}");

            httpRequest.Headers.Add("x-api-key", "not-installed");

            var client = this.clientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var serviceDeskCurrentRequests = await JsonSerializer.DeserializeAsync<IEnumerable<ServiceDeskRequestDTO>>(responseBody, options);

            return serviceDeskCurrentRequests;
        }

        public async Task<IEnumerable<ServiceDeskRequestPriorityDTO>> GetPriorities(CancellationToken cancellationToken)
        {
            return this.priorities;
        }
    }
}
