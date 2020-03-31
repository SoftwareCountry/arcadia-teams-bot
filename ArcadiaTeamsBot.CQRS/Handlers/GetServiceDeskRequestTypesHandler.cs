namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using MediatR;

    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;

    public class GetServiceDeskRequestTypesHandler : IRequestHandler<GetServiceDeskRequestTypesQuery, IEnumerable<ServiceDeskRequestTypeDTO>>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public GetServiceDeskRequestTypesHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public Task<IEnumerable<ServiceDeskRequestTypeDTO>> Handle(GetServiceDeskRequestTypesQuery request, CancellationToken cancellationToken)
        {
            return this.serviceDeskClient.GetRequestTypes(cancellationToken);
        }
    }
}
