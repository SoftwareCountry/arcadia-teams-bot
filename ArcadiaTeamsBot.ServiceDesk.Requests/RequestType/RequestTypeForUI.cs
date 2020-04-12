namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    using System.Collections.Generic;

    internal class RequestTypeForUI : IRequestTypeForUI
    {
        public RequestTypeForUI(IEnumerable<RequestTypeForUIField> requestTypeForUIFields)
        {
            this.RequestTypeForUIFields = requestTypeForUIFields;
        }

        public IEnumerable<RequestTypeForUIField> RequestTypeForUIFields { get; }
    }
}
