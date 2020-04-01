namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using System.Collections.Generic;

    using MediatR;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public class GetServiceDeskRequestPrioritiesQuery : IRequest<IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
    }
}
