namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ServiceDesk.Abstractions;

    internal class GetServiceDeskRequestPrioritiesHandler : IRequestHandler<GetServiceDeskRequestPrioritiesQuery, IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public GetServiceDeskRequestPrioritiesHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public Task<IEnumerable<ServiceDeskRequestPriorityDTO>> Handle(GetServiceDeskRequestPrioritiesQuery request, CancellationToken cancellationToken)
        {
            return this.serviceDeskClient.GetPriorities(cancellationToken);
        }
    }
}
