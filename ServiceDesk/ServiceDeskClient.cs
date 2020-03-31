namespace ServiceDesk
{
    using ServiceDesk.Abstractions;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Threading;

    public class ServiceDeskClient : IServiceDeskClient
    {
        private const string requestTypesUrl = "https://arcadia-servicedesk-dev31.azurewebsites.net/api/intra/requestTypes";

        private const string currentRequestsUri = "https://arcadia-servicedesk-dev31.azurewebsites.net/api/intra/requests?username=vyacheslav.lasukov@arcadia.spb.ru";

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
            ClientFactory = clientFactory;
        }

        private IHttpClientFactory ClientFactory { get; }

        public async Task<IEnumerable<ServiceDeskRequestTypeDTO>> GetRequestTypes(CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestTypesUrl);

            httpRequest.Headers.Add("x-api-key", "not-installed");

            var client = ClientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var serviceDeskRequestTypes = await JsonSerializer.DeserializeAsync<IEnumerable<ServiceDeskRequestTypeDTO>>(responseBody);

            return serviceDeskRequestTypes;
        }

        public async Task<IEnumerable<ServiceDeskRequestDTO>> GetCurrentRequests(string username, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{currentRequestsUri}{username}");

            httpRequest.Headers.Add("x-api-key", "not-installed");

            var client = ClientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var serviceDeskCurrentRequests = await JsonSerializer.DeserializeAsync<IEnumerable<ServiceDeskRequestDTO>>(responseBody);

            return serviceDeskCurrentRequests;
        }

        public async Task<IEnumerable<ServiceDeskRequestPriorityDTO>> GetPriorities(CancellationToken cancellationToken)
        {
            return this.priorities;
        }
    }
}
