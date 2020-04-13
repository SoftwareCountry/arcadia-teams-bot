namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    internal class GetRequestPrioritiesHandler : IRequestHandler<GetRequestPrioritiesQuery, IEnumerable<RequestPriorityDTO>>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public GetRequestPrioritiesHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public Task<IEnumerable<RequestPriorityDTO>> Handle(GetRequestPrioritiesQuery request, CancellationToken cancellationToken)
        {
            return this.serviceDeskClient.GetPriorities(cancellationToken);
        }
    }
}
