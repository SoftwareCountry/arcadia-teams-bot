namespace ArcadiaTeamsBot.ServiceDesk.Requests
{
    public class RequestTypeForUIField
    {
        public RequestTypeForUIField(string fieldName, RequestTypeForUIFieldType fieldType)
        {
            this.FieldName = fieldName;
            this.FieldType = fieldType;
        }

        public string FieldName { get; set; }

        public RequestTypeForUIFieldType FieldType { get; set; }
    }
}
