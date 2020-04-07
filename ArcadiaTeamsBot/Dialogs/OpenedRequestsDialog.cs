namespace ArcadiaTeamsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

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

            var heroCardList = new List<Attachment>();
            for (var i = 0; i < serviceDeskRequests.Count(); i++)
            {
                var heroCard = OpenedRequestsCard(
                    serviceDeskRequests[i].RequestNumber,
                    serviceDeskRequests[i].Title,
                    serviceDeskRequests[i].Created,
                    serviceDeskRequests[i].StatusName,
                    serviceDeskRequests[i].ExecutorFullName);

                heroCardList.Add(heroCard.ToAttachment());
            }

            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Carousel(heroCardList),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((string)stepContext.Result == Back)
            {
                return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);
            }
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        public static HeroCard OpenedRequestsCard(string requestNumber, string title, DateTime created, string statusName, string executorFullName)
        {
            var heroCard = new HeroCard
            {
                Title = requestNumber + " " +
                        created.ToShortDateString(),
                Subtitle = executorFullName,
                Text = statusName + ": " + title,
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, Back, value: Back),
                },
            };

            return heroCard;
        }
    }
}
