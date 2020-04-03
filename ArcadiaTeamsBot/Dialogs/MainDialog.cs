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

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachments = new []
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
                case "New request":
                    return await stepContext.BeginDialogAsync(nameof(NewRequestDialog), null, cancellationToken);
                case "See current requests":
                    return await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);
                default:
                    return await stepContext.NextAsync(cancellationToken: cancellationToken);
            }
        }

        public static HeroCard GetChoiceCard()
        {
            var choiceCard = new HeroCard()
            {
                Title = " You can create a new request or view opened requests.",
                Subtitle = "What do you want to do?",
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
