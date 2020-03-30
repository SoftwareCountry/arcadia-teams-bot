using MediatR;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;

namespace ArcadiaTeamsBot.CQRS.Queries
{
    public class GetServiceDeskRequestPrioritiesQuery : IRequest<IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
    }
}
