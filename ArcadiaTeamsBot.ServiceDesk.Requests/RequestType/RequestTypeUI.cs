namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    using System.Collections.Generic;

    internal class RequestTypeUI : IRequestTypeUI
    {
        public RequestTypeUI(IEnumerable<RequestTypeUIField> requestTypeForUIFields)
        {
            this.RequestTypeUIFields = requestTypeForUIFields;
        }

        public IEnumerable<RequestTypeUIField> RequestTypeUIFields { get; }
    }
}
