namespace ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs
{
    using System.Collections.Generic;

    public class ServiceDeskRequestTypeDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsDeleted { get; set; }

        public string Description { get; set; }

        public IEnumerable<ServiceDeskRequestTypeFieldDTO> RequestTypeFields { get; set; }
    }
}
