namespace ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs
{
    public class RequestTypeDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsDeleted { get; set; }

        public string Description { get; set; }

        public RequestTypeFieldDTO[] RequestTypeFields { get; set; }
    }
}
