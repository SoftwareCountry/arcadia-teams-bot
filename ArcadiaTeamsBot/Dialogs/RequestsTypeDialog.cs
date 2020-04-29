namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class RequestsTypeDialog : ComponentDialog
    {
        private const string Back = "Back";
        private static IEnumerable<ServiceDeskRequestTypeDTO> requestTypes;
        private readonly IMediator mediator;

        public RequestsTypeDialog(IMediator mediator) : base(nameof(RequestsTypeDialog))
        {
            this.mediator = mediator;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.TypeStep,
                this.EndStep
            }));

            this.AddDialog(new NewRequestDialog());
            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> TypeStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var getRequestTypesQuery = new GetServiceDeskRequestTypesQuery();
            requestTypes = await this.mediator.Send(getRequestTypesQuery, cancellationToken);

            var buttons = requestTypes
                .Select(type => new CardAction(ActionTypes.ImBack, type.Title, value: type.Title))
                .ToList();

            buttons.Add(new CardAction(ActionTypes.ImBack, Back, value: Back));

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(GetInfoCard(buttons).ToAttachment())
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var type = requestTypes.FirstOrDefault(type => type.Title == (string)stepContext.Result);
            if (type != null)
            {
                return await stepContext.BeginDialogAsync(nameof(NewRequestDialog), type, cancellationToken);
            }

            switch (stepContext.Result)
            {
                case Back:
                    await stepContext.EndDialogAsync(null, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);

                default:
                    return await stepContext.ReplaceDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);
            }
        }

        private static HeroCard GetInfoCard(IList<CardAction> Buttons)
        {
            return new HeroCard
            {
                Title = "Select the type of request which you want to create",
                Buttons = Buttons
            };
        }
    }
}
