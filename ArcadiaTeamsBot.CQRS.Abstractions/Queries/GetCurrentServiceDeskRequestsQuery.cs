namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;

    public class GetCurrentServiceDeskRequestsQuery : IRequest<IEnumerable<ServiceDeskRequestDTO>>
    {
    }
}
