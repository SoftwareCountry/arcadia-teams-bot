namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    using System.Collections.Generic;

    public interface IRequestTypeForUI
    {
        IEnumerable<RequestTypeForUIField> RequestTypeForUIFields { get; }
    }
}
