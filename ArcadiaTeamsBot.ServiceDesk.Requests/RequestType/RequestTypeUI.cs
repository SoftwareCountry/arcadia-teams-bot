namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    using System.Collections.Generic;

    internal class RequestTypeUI : IRequestTypeUI
    {
        public RequestTypeUI(IEnumerable<RequestTypeUIField> requestTypeForUIFields)
        {
            this.RequestTypeForUIFields = requestTypeForUIFields;
        }

        public IEnumerable<RequestTypeUIField> RequestTypeForUIFields { get; }
    }
}
