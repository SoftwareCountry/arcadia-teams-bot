namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions.Commands;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;

    using MediatR;

    internal class CreateNewServiceDeskRequestCommandHandler : IRequestHandler<CreateNewServiceDeskRequestCommand>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public CreateNewServiceDeskRequestCommandHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public async Task<Unit> Handle(CreateNewServiceDeskRequestCommand request, CancellationToken cancellationToken)
        {
            await this.serviceDeskClient.CreateNewRequest(request.CreateRequestDTO, cancellationToken);

            return Unit.Value;
        }
    }
}
