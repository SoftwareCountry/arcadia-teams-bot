namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory
{
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestType;

    public interface IRequestTypeUIFactory
    {
        IRequestTypeUI CreateRequestTypeForUI(ServiceDeskRequestTypeDTO requestTypeDTO);
    }
}
