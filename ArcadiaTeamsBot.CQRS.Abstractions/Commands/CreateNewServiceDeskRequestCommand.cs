namespace ArcadiaTeamsBot.CQRS.Abstractions.Commands
{
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    public class CreateNewServiceDeskRequestCommand : IRequest
    {
        public CreateNewServiceDeskRequestCommand(RequestForCreationDTO requestForCreationDTO)
        {
            this.RequestForCreationDto = requestForCreationDTO;
        }

        public RequestForCreationDTO RequestForCreationDto { get; }
    }
}
