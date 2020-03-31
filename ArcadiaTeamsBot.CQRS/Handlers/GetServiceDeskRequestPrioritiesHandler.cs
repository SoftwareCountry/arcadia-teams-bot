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
        public GetServiceDeskRequestPrioritiesHandler(IServiceDeskClient serviceDeskClient)
        {
            this.ServiceDeskClient = serviceDeskClient;
        }

        private IServiceDeskClient ServiceDeskClient { get; }

        public async Task<IEnumerable<ServiceDeskRequestPriorityDTO>> Handle(GetServiceDeskRequestPrioritiesQuery request, CancellationToken cancellationToken)
        {
            return await this.ServiceDeskClient.GetPriorities(cancellationToken);
        }
    }
}
