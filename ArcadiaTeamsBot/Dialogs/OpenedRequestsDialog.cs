namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using AdaptiveCards;

    using ArcadiaTeamsBot.CQRS.Abstractions;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class OpenedRequestsDialog : ComponentDialog
    {
        private const string Back = "Back";
        private const string username = "ekaterina.kuznetsova@arcadia.spb.ru";
        private readonly IMediator mediator;

        public OpenedRequestsDialog(IMediator mediator) : base(nameof(OpenedRequestsDialog))
        {
            this.mediator = mediator;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.InfoStep,
                EndStep,
            }));
            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InfoStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetCurrentServiceDeskRequestsQuery(username);
            var openedRequest = await this.mediator.Send(getRequestsQuery, cancellationToken);

            var Actions = new List<AdaptiveAction>();
            foreach (var request in openedRequest)
            {
                if (request.ExecutorFullName == "")
                {
                    request.ExecutorFullName = "-";
                }

                var shownCard = new AdaptiveShowCardAction()
                {
                    Title = request.RequestNumber,
                    Card = new AdaptiveCard()
                    {
                        Body = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock() { Text = request.Title, Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Large },
                            new AdaptiveTextBlock() { Text = request.StatusName + ": " + request.Created.ToShortDateString(), Weight = AdaptiveTextWeight.Default, Size = AdaptiveTextSize.Medium },
                            new AdaptiveTextBlock() { Text = "Executor Name: " + request.ExecutorFullName, Weight = AdaptiveTextWeight.Normal, Size = AdaptiveTextSize.Default },
                        }
                    },
                };
                Actions.Add(shownCard);
            }

            var backAction = new AdaptiveSubmitAction
            {
                Title = Back,
                Data = Back,
            };
            Actions.Add(backAction);

            var attachment = OpenedRequestsCard(Actions);
            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(attachment), cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { }, cancellationToken);
        }

        private static async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((string)stepContext.Result == Back)
            {
                return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
            }
            return await stepContext.ReplaceDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);
        }

        public static Attachment OpenedRequestsCard(List<AdaptiveAction> actions)
        {
            var card = new AdaptiveCard()
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock() { Text = "All your opened requests", Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Large },
                },
                Actions = actions,
            };
            var attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }
    }
}
