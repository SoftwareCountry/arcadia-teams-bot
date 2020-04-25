namespace ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs
{
    public class CreateRequestTypeFieldDTO
    {
        public int Id { get; set; }

        public string FieldName { get; set; }

        public bool IsMandatory { get; set; }
    }
}
