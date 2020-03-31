namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;

    public class GetServiceDeskRequestTypesQuery : IRequest<IEnumerable<ServiceDeskRequestTypeDTO>>
    {
    }
}
