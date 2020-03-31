using MediatR;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using ArcadiaTeamsBot.CQRS.Abstractions;
using ServiceDesk.Abstractions;

namespace ArcadiaTeamsBot.CQRS.Handlers
{
    internal class GetServiceDeskRequestPrioritiesHandler : IRequestHandler<GetServiceDeskRequestPrioritiesQuery, IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
        public GetServiceDeskRequestPrioritiesHandler(IServiceDeskClient serviceDeskClient)
        {
            ServiceDeskClient = serviceDeskClient;
        }

        public IServiceDeskClient ServiceDeskClient { get; }

        public async Task<IEnumerable<ServiceDeskRequestPriorityDTO>> Handle(GetServiceDeskRequestPrioritiesQuery request, CancellationToken cancellationToken)
        {
            return await ServiceDeskClient.GetPriorities(cancellationToken);
        }
    }
}
