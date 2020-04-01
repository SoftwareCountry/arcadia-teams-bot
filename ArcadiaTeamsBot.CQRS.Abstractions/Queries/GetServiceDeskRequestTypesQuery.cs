namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using System.Collections.Generic;

    using MediatR;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public class GetServiceDeskRequestTypesQuery : IRequest<IEnumerable<ServiceDeskRequestTypeDTO>>
    {
    }
}
