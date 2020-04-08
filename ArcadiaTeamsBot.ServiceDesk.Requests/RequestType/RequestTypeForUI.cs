namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    using System.Collections.Generic;

    internal class RequestTypeForUI : IRequestTypeForUI
    {
        public RequestTypeForUI(List<RequestTypeForUIField> requestTypeForUIFields)
        {
            this.RequestTypeForUIFields = requestTypeForUIFields;
        }

        public List<RequestTypeForUIField> RequestTypeForUIFields { get; }
    }
}
