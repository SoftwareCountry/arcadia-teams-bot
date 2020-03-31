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
    class GetServiceDeskRequestTypesHandler : IRequestHandler<GetServiceDeskRequestTypesQuery, IEnumerable<ServiceDeskRequestTypeDTO>>
    {
        private const string requestTypesUrl = "https://arcadia-servicedesk-dev31.azurewebsites.net/api/intra/requestTypes";

        public GetServiceDeskRequestTypesHandler(IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
        }

        private IHttpClientFactory ClientFactory { get; }

        public async Task<IEnumerable<ServiceDeskRequestTypeDTO>> Handle(GetServiceDeskRequestTypesQuery request, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestTypesUrl);

            httpRequest.Headers.Add("x-api-key", "not-installed");

            var client = ClientFactory.CreateClient();

            var response = await client.SendAsync(httpRequest, cancellationToken);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var serviceDeskRequestTypes = await JsonSerializer.DeserializeAsync<IEnumerable<ServiceDeskRequestTypeDTO>>(responseBody);

            return serviceDeskRequestTypes;
        }
    }
}
