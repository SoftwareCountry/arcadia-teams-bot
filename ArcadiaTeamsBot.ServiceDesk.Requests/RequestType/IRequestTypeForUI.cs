namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    using System.Collections.Generic;

    public interface IRequestTypeForUI
    {
        List<RequestTypeForUIField> RequestTypeForUIFields { get; }
    }
}
