using MediatR;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;

namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    public class GetCurrentServiceDeskRequestsQuery : IRequest<IEnumerable<ServiceDeskRequestDTO>>
    {
    }
}
