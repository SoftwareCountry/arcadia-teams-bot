namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using MediatR;
    using ServiceDesk.Abstractions.DTOs;
    using System.Collections.Generic;

    public class GetCurrentServiceDeskRequestsQuery : IRequest<IEnumerable<ServiceDeskRequestDTO>>
    {
        public GetCurrentServiceDeskRequestsQuery(string username)
        {
            this.Username = username;
        }

        public string Username { get; }
    }
}
