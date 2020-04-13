namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    public class GetRequestTypesHandler : IRequestHandler<GetRequestTypesQuery, IEnumerable<RequestTypeDTO>>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public GetRequestTypesHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public Task<IEnumerable<RequestTypeDTO>> Handle(GetRequestTypesQuery request, CancellationToken cancellationToken)
        {
            return this.serviceDeskClient.GetRequestTypes(cancellationToken);
        }
    }
}
