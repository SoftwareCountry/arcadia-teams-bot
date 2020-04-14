namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class RequestsTypeDialog : ComponentDialog
    {
        private const string Back = "Back";
        const string Username = "vyacheslav.lasukov@arcadia.spb.ru";
        private readonly IMediator mediator;

        public RequestsTypeDialog(IMediator mediator) : base(nameof(RequestsTypeDialog))
        {
            this.mediator = mediator;
            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                TypeStep,
                EndStep,
            }));

            this.AddDialog(new NewRequestDialog());
            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> TypeStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var getTypesQuery = new GetServiceDeskRequestTypesQuery();
            var requestsTypes = await this.mediator.Send(getTypesQuery, cancellationToken);

            var Buttons = new List<CardAction>();
            foreach (var type in requestsTypes)
            {
                var button = new CardAction(ActionTypes.ImBack, type.Title, value: type.Title);
                Buttons.Add(button);
            }

            var backButton = new CardAction(ActionTypes.ImBack, Back, value: Back);
            Buttons.Add(backButton);

            var attachments = new[]
            {
                GetInfoCard(Buttons).ToAttachment()
            };
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(attachments),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var getTypesQuery = new GetServiceDeskRequestTypesQuery();
            var requestsTypes = await this.mediator.Send(getTypesQuery, cancellationToken);
            foreach (var type in requestsTypes)
            {
                if ((string)stepContext.Result == type.Title)
                {
                    return await stepContext.BeginDialogAsync(nameof(NewRequestDialog), null, cancellationToken);
                }
            }
            
            if ((string)stepContext.Result == Back)
            {
                return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
            }
            return await stepContext.ReplaceDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);
        }

        public static HeroCard GetInfoCard(List<CardAction> Buttons)
        {
            var infoCard = new HeroCard
            {
                Title = "Select the type of request which you want to create",
                Buttons = Buttons,
            };

            return infoCard;
        }
    }
}
