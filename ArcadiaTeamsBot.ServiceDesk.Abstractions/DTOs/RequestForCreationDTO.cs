﻿namespace ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs
{
    using System;
    using System.Collections.Generic;

    public class RequestForCreationDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public RequestTypeForCreation Type { get; set; }

        public int? PriorityId { get; set; }

        public DateTime? ExecutionDate { get; set; }

        public IList<string> FieldValues { get; set; }

        public string Username { get; set; }
    }
}
