using ArcadiaTeamsBot.CQRS.Queries;
using MediatR;
using Newtonsoft.Json;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ArcadiaTeamsBot.CQRS.Handlers
{
    class GetServiceDeskRequestPrioritiesHandler : IRequestHandler<GetServiceDeskRequestPrioritiesQuery, IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
        public async Task<IEnumerable<ServiceDeskRequestPriorityDTO>> Handle(GetServiceDeskRequestPrioritiesQuery request, CancellationToken cancellationToken)
        {
            var prioritiesJsonAsString = 
                @"[
                        {
                        'key': 1,
                        'value': 'Low'
                        },
                        {
                        'key': 2,
                        'value': 'Default'
                        },
                        {
                        'key': 3,
                        'value': 'High'
                        }
                ]";

            var serviceDeskRequestPriorities = JsonConvert.DeserializeObject<IEnumerable<ServiceDeskRequestPriorityDTO>>(prioritiesJsonAsString);

            return serviceDeskRequestPriorities;
        }
    }
}
