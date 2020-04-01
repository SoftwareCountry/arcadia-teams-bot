namespace ArcadiaTeamsBot.ServiceDesk.Abstractions
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public interface IServiceDeskClient
    {
        Task<IEnumerable<ServiceDeskRequestTypeDTO>> GetRequestTypes(CancellationToken cancellationToken);

        Task<IEnumerable<ServiceDeskRequestDTO>> GetCurrentRequests(string username, CancellationToken cancellationToken);

        Task<IEnumerable<ServiceDeskRequestPriorityDTO>> GetPriorities();
    }
}
