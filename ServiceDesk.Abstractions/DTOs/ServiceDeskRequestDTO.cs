namespace ServiceDesk.Abstractions.DTOs
{
    using System;

    public class ServiceDeskRequestDTO
    {
        public string RequestNumber { get; set; }

        public DateTime Created { get; set; }

        public string Title { get; set; }

        public string StatusName { get; set; }

        public string ExecutorFullName { get; set; }
    }
}
