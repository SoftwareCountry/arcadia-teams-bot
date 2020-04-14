namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class MainDialog : ComponentDialog
    {
        private const string NewRequest = "New request";
        private const string OpenedRequests = "See current requests";

        public MainDialog(IMediator mediator, IRequestTypeUIFactory request) : base(nameof(MainDialog))
        {
            this.AddDialog(new RequestsTypeDialog(mediator, request));
            this.AddDialog(new OpenedRequestsDialog(mediator));

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChoiceStep,
                RequestStep,
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachments = new[]
            {
                GetChoiceCard().ToAttachment()
            };
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(attachments),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> RequestStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            switch ((string)stepContext.Result)
            {
                case NewRequest:
                    return await stepContext.BeginDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);
                case OpenedRequests:
                    return await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);
                default:
                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
            }
        }

        public static HeroCard GetChoiceCard()
        {
            var choiceCard = new HeroCard
            {
                Title = "You can create a new request or view opened requests.",
                Subtitle = "What do you want to do?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, NewRequest, value: NewRequest),
                    new CardAction(ActionTypes.ImBack, OpenedRequests, value: OpenedRequests),
                },
            };
            return choiceCard;
        }
    }
}
