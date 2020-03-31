namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using MediatR;

    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;

    internal class GetCurrentServiceDeskRequestsHandler : IRequestHandler<GetCurrentServiceDeskRequestsQuery, IEnumerable<ServiceDeskRequestDTO>>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public GetCurrentServiceDeskRequestsHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public Task<IEnumerable<ServiceDeskRequestDTO>> Handle(GetCurrentServiceDeskRequestsQuery request, CancellationToken cancellationToken)
        {
            return this.serviceDeskClient.GetCurrentRequests(request.Username, cancellationToken); ;
        }
    }
}
