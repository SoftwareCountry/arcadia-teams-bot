namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class MainDialog : ComponentDialog
    {
        public MainDialog() : base(nameof(MainDialog))
        {
            AddDialog(new NewRequestDialog());
            AddDialog(new OpenedRequestsDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChoiceStep,
                RequestStep,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(new List<Attachment>());
            reply.Attachments.Add(GetChoiceCard().ToAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private static async Task<DialogTurnResult> RequestStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((string)stepContext.Result == "New request")
            {
                return await stepContext.BeginDialogAsync(nameof(NewRequestDialog), null, cancellationToken);
            }
            return await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);
        }

        public static HeroCard GetChoiceCard()
        {
            var choiceCard = new HeroCard
            {
                Title = "What do you want to do?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "New request", value: "New request"),
                    new CardAction(ActionTypes.ImBack, "See current requests", value: "See current requests"),
                },
            };
            return choiceCard;
        }
    }
}
