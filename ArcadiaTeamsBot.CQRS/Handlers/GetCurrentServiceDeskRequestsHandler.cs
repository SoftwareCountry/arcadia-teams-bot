using ArcadiaTeamsBot.CQRS.Queries;
using MediatR;
using Newtonsoft.Json;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ArcadiaTeamsBot.CQRS.Handlers
{
    class GetCurrentServiceDeskRequestsHandler : IRequestHandler<GetCurrentServiceDeskRequestsQuery, IEnumerable<ServiceDeskRequestDTO>>
    {
        private const string currentRequestsUri = "https://arcadia-servicedesk-dev31.azurewebsites.net/api/intra/requests?username=vyacheslav.lasukov@arcadia.spb.ru";

        public GetCurrentServiceDeskRequestsHandler(IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
        }

        public IHttpClientFactory ClientFactory { get; }

        public async Task<IEnumerable<ServiceDeskRequestDTO>> Handle(GetCurrentServiceDeskRequestsQuery request, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, currentRequestsUri);

            httpRequest.Headers.Add("x-api-key", "not-installed");

            var client = ClientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync();

            var serviceDeskCurrentRequests = JsonConvert.DeserializeObject<IEnumerable<ServiceDeskRequestDTO>>(responseBody);

            return serviceDeskCurrentRequests;
        }
    }
}
