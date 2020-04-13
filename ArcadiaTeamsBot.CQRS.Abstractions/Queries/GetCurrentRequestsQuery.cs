namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    public class GetCurrentRequestsQuery : IRequest<IEnumerable<RequestDTO>>
    {
        public GetCurrentRequestsQuery(string username)
        {
            this.Username = username;
        }

        public string Username { get; }
    }
}
