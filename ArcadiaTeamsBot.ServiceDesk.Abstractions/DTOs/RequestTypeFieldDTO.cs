namespace ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs
{
    public class RequestTypeFieldDTO
    {
        public int Id { get; set; }

        public string FieldName { get; set; }

        public bool IsMandatory { get; set; }

        public string Items { get; set; }
    }
}
