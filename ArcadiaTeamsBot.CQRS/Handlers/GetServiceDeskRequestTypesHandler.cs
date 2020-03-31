namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ServiceDesk.Abstractions;

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
