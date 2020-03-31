﻿namespace ArcadiaTeamsBot.CQRS.Abstractions
{
    using MediatR;

    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public class GetServiceDeskRequestPrioritiesQuery : IRequest<IEnumerable<ServiceDeskRequestPriorityDTO>>
    {
    }
}
