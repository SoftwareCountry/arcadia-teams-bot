namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
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
        const string Username = "vyacheslav.lasukov@arcadia.spb.ru";
        private readonly IMediator mediator;

        public OpenedRequestsDialog(IMediator mediator) : base(nameof(OpenedRequestsDialog))
        {
            this.mediator = mediator;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStep,
                EndStep,
            }));
            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InfoStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetCurrentServiceDeskRequestsQuery(Username);
            var openedRequest = await this.mediator.Send(getRequestsQuery, cancellationToken);
            var serviceDeskRequests = openedRequest.ToList();

            var Actions = new List<AdaptiveAction>();
            for (var i = 0; i < serviceDeskRequests.Count(); i++)
            {
                if (serviceDeskRequests[i].ExecutorFullName == "")
                {
                    serviceDeskRequests[i].ExecutorFullName = "-";
                }

                var shownCard = new AdaptiveShowCardAction()
                {
                    Title = serviceDeskRequests[i].RequestNumber,
                    Card = new AdaptiveCard()
                    {
                        Body = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock() { Text = serviceDeskRequests[i].Title, Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Large },
                            new AdaptiveTextBlock() { Text = serviceDeskRequests[i].StatusName + ": " + serviceDeskRequests[i].Created.ToShortDateString(), Weight = AdaptiveTextWeight.Default, Size = AdaptiveTextSize.Medium },
                            new AdaptiveTextBlock() { Text = "Executor Name: " + serviceDeskRequests[i].ExecutorFullName, Weight = AdaptiveTextWeight.Normal, Size = AdaptiveTextSize.Default },
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
            var reply = MessageFactory.Attachment(attachment);

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { }, cancellationToken);
        }

        private static async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((string)stepContext.Result == Back)
            {
                return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
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
