namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;
    using Microsoft.Bot.Schema;

    public class OpenedRequestsDialog : ComponentDialog
    {
        private const string Back = "Back";
        const string username = "vyacheslav.lasukov@arcadia.spb.ru";
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

            this.AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        }

        private async Task<DialogTurnResult> InfoStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetCurrentServiceDeskRequestsQuery(username);
            var openedRequest = await this.mediator.Send(getRequestsQuery, cancellationToken);
            var serviceDeskRequests = openedRequest.ToList();

            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);

            var heroCardList = new List<HeroCard>();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            for (var i = 0; i < serviceDeskRequests.Count(); i++)
            {
                var heroCard = new HeroCard
                {
                    Title = serviceDeskRequests[i].RequestNumber + " " +
                            serviceDeskRequests[i].Created.ToShortDateString(),
                    Subtitle = serviceDeskRequests[i].ExecutorFullName,
                    Text = serviceDeskRequests[i].StatusName +  ": " + serviceDeskRequests[i].Title,
                };

                reply.Attachments.Add(heroCard.ToAttachment());
            }
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            var backCard = new[]
            {
                BackCard().ToAttachment()
            };
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(backCard),
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

        public static HeroCard BackCard()
        {
            var backCard = new HeroCard
            {
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, Back, value: Back),
                },
            };

            return backCard;
        }

        public async Task<List<HeroCard>> GetOpenedRequests(CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetCurrentServiceDeskRequestsQuery("vyacheslav.lasukov@arcadia.spb.ru");
            var openedRequest = await this.mediator.Send(getRequestsQuery, cancellationToken);
            var serviceDeskRequests = openedRequest.ToList();

            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);

            var heroCardList = new List<HeroCard>();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            for (var i = 0; i < serviceDeskRequests.Count(); i++)
            {
                var heroCard = new HeroCard
                {
                    Title = serviceDeskRequests[i].RequestNumber + " " +
                            serviceDeskRequests[i].Created.ToShortDateString(),
                    Subtitle = serviceDeskRequests[i].ExecutorFullName,
                    Text = serviceDeskRequests[i].StatusName + ": " + serviceDeskRequests[i].Title,

                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, Back, value: Back),
                    },
                };
                heroCardList.Add(heroCard);
            }
            return heroCardList;
        }
    }
}
