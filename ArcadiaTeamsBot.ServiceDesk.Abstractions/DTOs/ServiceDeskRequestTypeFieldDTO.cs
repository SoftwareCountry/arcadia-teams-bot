namespace ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs
{
    public class ServiceDeskRequestTypeFieldDTO
    {
        public int Id { get; set; }

        public string FieldName { get; set; }

        public bool IsMandatory { get; set; }
    }
}
