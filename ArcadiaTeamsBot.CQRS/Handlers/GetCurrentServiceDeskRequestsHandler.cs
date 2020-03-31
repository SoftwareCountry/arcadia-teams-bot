namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ServiceDesk.Abstractions;

    internal class GetCurrentServiceDeskRequestsHandler : IRequestHandler<GetCurrentServiceDeskRequestsQuery, IEnumerable<ServiceDeskRequestDTO>>
    {
        public GetCurrentServiceDeskRequestsHandler(IServiceDeskClient serviceDeskClient)
        {
            this.ServiceDeskClient = serviceDeskClient;
        }

        private IServiceDeskClient ServiceDeskClient { get; }

        public async Task<IEnumerable<ServiceDeskRequestDTO>> Handle(GetCurrentServiceDeskRequestsQuery request, CancellationToken cancellationToken)
        {
            return await this.ServiceDeskClient.GetCurrentRequests(request.Username, cancellationToken); ;
        }
    }
}
