namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;

    public class GetServiceDeskRequestPrioritiesQuery : IRequest<IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
    }
}
