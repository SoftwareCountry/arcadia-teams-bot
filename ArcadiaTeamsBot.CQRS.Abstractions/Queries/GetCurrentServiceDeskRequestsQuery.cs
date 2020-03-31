namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using MediatR;

    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public class GetCurrentServiceDeskRequestsQuery : IRequest<IEnumerable<ServiceDeskRequestDTO>>
    {
        public GetCurrentServiceDeskRequestsQuery(string username)
        {
            this.Username = username;
        }

        public string Username { get; }
    }
}
