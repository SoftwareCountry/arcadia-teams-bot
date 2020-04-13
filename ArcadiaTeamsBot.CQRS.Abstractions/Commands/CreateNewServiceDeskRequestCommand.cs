namespace ArcadiaTeamsBot.CQRS.Abstractions.Commands
{
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    public class CreateNewServiceDeskRequestCommand : IRequest
    {
        public CreateNewServiceDeskRequestCommand(CreateRequestDTO createRequestDTO)
        {
            this.CreateRequestDTO = createRequestDTO;
        }

        public CreateRequestDTO CreateRequestDTO { get; }
    }
}
