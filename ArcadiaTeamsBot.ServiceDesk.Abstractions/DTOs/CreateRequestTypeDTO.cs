namespace ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs
{
    using System.Collections.Generic;

    public class CreateRequestTypeDTO
    {
        public int Id { get; set; }
        
        public IEnumerable<CreateRequestTypeFieldDTO> RequestTypeFields { get; set; }

    }
}
