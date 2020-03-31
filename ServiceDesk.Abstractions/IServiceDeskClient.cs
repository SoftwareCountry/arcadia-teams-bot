using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceDesk.Abstractions
{
    public interface IServiceDeskClient
    {
        Task<IEnumerable<ServiceDeskRequestTypeDTO>> GetRequestTypes(CancellationToken cancellationToken);

        Task<IEnumerable<ServiceDeskRequestDTO>> GetCurrentRequests(string username, CancellationToken cancellationToken);

        Task<IEnumerable<ServiceDeskRequestPriorityDTO>> GetPriorities(CancellationToken cancellationToken);
    }
}
