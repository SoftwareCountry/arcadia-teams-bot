﻿namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    public class GetServiceDeskRequestPrioritiesQuery : IRequest<IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
    }
}
