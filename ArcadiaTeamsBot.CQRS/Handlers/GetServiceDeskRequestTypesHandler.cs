namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ServiceDesk.Abstractions;

    internal class GetServiceDeskRequestTypesHandler : IRequestHandler<GetServiceDeskRequestTypesQuery, IEnumerable<ServiceDeskRequestTypeDTO>>
    {
        public GetServiceDeskRequestTypesHandler(IServiceDeskClient serviceDeskClient)
        {
            ServiceDeskClient = serviceDeskClient;
        }

        public IServiceDeskClient ServiceDeskClient { get; }

        public async Task<IEnumerable<ServiceDeskRequestTypeDTO>> Handle(GetServiceDeskRequestTypesQuery request, CancellationToken cancellationToken)
        {
            return await ServiceDeskClient.GetRequestTypes(cancellationToken);
        }
    }
}
