namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestType
{
    using System.Collections.Generic;

    public interface IRequestTypeUI
    {
        IEnumerable<RequestTypeUIField> RequestTypeUIFields { get; }
    }
}
