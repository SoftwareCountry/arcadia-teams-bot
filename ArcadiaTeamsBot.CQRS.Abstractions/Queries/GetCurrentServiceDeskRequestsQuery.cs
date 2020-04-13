namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    public class GetCurrentServiceDeskRequestsQuery : IRequest<IEnumerable<ServiceDeskRequestDTO>>
    {
        public GetCurrentServiceDeskRequestsQuery(string username)
        {
            this.Username = username;
        }

        public string Username { get; }
    }
}
