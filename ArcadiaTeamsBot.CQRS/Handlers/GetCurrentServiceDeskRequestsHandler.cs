using MediatR;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcadiaTeamsBot.CQRS.Abstractions;
using ServiceDesk.Abstractions;

namespace ArcadiaTeamsBot.CQRS.Handlers
{
    internal class GetCurrentServiceDeskRequestsHandler : IRequestHandler<GetCurrentServiceDeskRequestsQuery, IEnumerable<ServiceDeskRequestDTO>>
    {
        public GetCurrentServiceDeskRequestsHandler(IServiceDeskClient serviceDeskClient)
        {
            ServiceDeskClient = serviceDeskClient;
        }

        public IServiceDeskClient ServiceDeskClient { get; }

        public async Task<IEnumerable<ServiceDeskRequestDTO>> Handle(GetCurrentServiceDeskRequestsQuery request, CancellationToken cancellationToken)
        {
            // Temporary
            var username = "vyacheslav.lasukov@arcadia.spb.ru"; 

            return await ServiceDeskClient.GetCurrentRequests(username, cancellationToken); ;
        }
    }
}
