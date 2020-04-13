namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    internal class GetCurrentRequestsHandler : IRequestHandler<GetCurrentRequestsQuery, IEnumerable<RequestDTO>>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public GetCurrentRequestsHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public Task<IEnumerable<RequestDTO>> Handle(GetCurrentRequestsQuery request, CancellationToken cancellationToken)
        {
            return this.serviceDeskClient.GetCurrentRequests(request.Username, cancellationToken);
        }
    }
}
