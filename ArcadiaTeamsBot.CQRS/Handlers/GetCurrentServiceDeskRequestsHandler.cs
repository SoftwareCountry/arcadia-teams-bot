using MediatR;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using ArcadiaTeamsBot.CQRS.Abstractions;

namespace ArcadiaTeamsBot.CQRS.Handlers
{
    class GetCurrentServiceDeskRequestsHandler : IRequestHandler<GetCurrentServiceDeskRequestsQuery, IEnumerable<ServiceDeskRequestDTO>>
    {
        private const string currentRequestsUri = "https://arcadia-servicedesk-dev31.azurewebsites.net/api/intra/requests?username=vyacheslav.lasukov@arcadia.spb.ru";

        public GetCurrentServiceDeskRequestsHandler(IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
        }

        private IHttpClientFactory ClientFactory { get; }

        public async Task<IEnumerable<ServiceDeskRequestDTO>> Handle(GetCurrentServiceDeskRequestsQuery request, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, currentRequestsUri);

            httpRequest.Headers.Add("x-api-key", "not-installed");

            var client = ClientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var serviceDeskCurrentRequests = await JsonSerializer.DeserializeAsync<IEnumerable<ServiceDeskRequestDTO>>(responseBody);

            return serviceDeskCurrentRequests;
        }
    }
}
