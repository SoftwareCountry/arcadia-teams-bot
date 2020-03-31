using MediatR;
using ServiceDesk.Abstractions.DTOs;
using System.Collections.Generic;

namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    public class GetServiceDeskRequestTypesQuery : IRequest<IEnumerable<ServiceDeskRequestTypeDTO>>
    {
    }
}
