namespace ArcadiaTeamsBot.ServiceDesk.Abstractions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public interface IServiceDeskClient
    {
        Task<IEnumerable<RequestTypeDTO>> GetRequestTypes(CancellationToken cancellationToken);

        Task<IEnumerable<RequestDTO>> GetCurrentRequests(string username, CancellationToken cancellationToken);

        Task<IEnumerable<RequestPriorityDTO>> GetPriorities(CancellationToken cancellationToken);
    }
}
