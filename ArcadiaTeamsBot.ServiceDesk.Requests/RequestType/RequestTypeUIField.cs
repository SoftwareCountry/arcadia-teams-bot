namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    public class RequestTypeUIField
    {
        public RequestTypeUIField(string fieldName, RequestTypeUIFieldType fieldType)
        {
            this.FieldName = fieldName;
            this.FieldType = fieldType;
        }

        public string FieldName { get; set; }

        public RequestTypeUIFieldType FieldType { get; set; }
    }
}
