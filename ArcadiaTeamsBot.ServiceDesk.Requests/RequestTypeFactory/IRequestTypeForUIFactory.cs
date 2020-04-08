namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory
{
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestType;

    public interface IRequestTypeForUIFactory
    {
        IRequestTypeForUI CreateRequestTypeForUI(ServiceDeskRequestTypeDTO requestTypeDTO);
    }
}
