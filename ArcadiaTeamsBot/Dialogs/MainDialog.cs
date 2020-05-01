namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestType;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    internal class MainDialog : ComponentDialog
    {
        private const string NewRequest = "New request";
        private const string OpenedRequests = "Opened requests";

        public MainDialog(IMediator mediator, IRequestTypeUIFactory requestTypeUIFactory, IEnumerable<RequestTypeUIField> requestTypeUIFields) : base(nameof(MainDialog))
        {
            this.AddDialog(new RequestsTypeDialog(mediator, requestTypeUIFactory, requestTypeUIFields));
            this.AddDialog(new OpenedRequestsDialog(mediator));

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.ChoiceStep,
                this.RequestStep
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(ChoiceCard().ToAttachment())
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> RequestStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return stepContext.Result switch
            {
                NewRequest => await stepContext.BeginDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken),
                OpenedRequests => await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken),
                _ => await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken)
            };
        }

        private static HeroCard ChoiceCard()
        {
            return new HeroCard
            {
                Title = "You can create a new request or view opened requests.",
                Subtitle = "What do you want to do?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, NewRequest, value: NewRequest),
                    new CardAction(ActionTypes.ImBack, OpenedRequests, value: OpenedRequests)
                }
            };
        }
    }
}
