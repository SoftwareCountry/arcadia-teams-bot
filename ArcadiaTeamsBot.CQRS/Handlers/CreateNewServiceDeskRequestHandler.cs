namespace ArcadiaTeamsBot.CQRS.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions.Commands;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;

    using MediatR;

    internal class CreateNewServiceDeskRequestHandler : IRequestHandler<CreateNewServiceDeskRequestCommand>
    {
        private readonly IServiceDeskClient serviceDeskClient;

        public CreateNewServiceDeskRequestHandler(IServiceDeskClient serviceDeskClient)
        {
            this.serviceDeskClient = serviceDeskClient;
        }

        public async Task<Unit> Handle(CreateNewServiceDeskRequestCommand request, CancellationToken cancellationToken)
        {
            await this.serviceDeskClient.CreateNewRequest(request.RequestForCreationDto, cancellationToken);

            return Unit.Value;
        }
    }
}
