namespace ArcadiaTeamsBot.CQRS.Abstractions.Commands
{
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    public class CreateNewServiceDeskRequestCommand : IRequest
    {
        public CreateNewServiceDeskRequestCommand(CreateRequestDTO requestForCreationDTO)
        {
            this.RequestForCreationDto = requestForCreationDTO;
        }

        public CreateRequestDTO RequestForCreationDto { get; }
    }
}
