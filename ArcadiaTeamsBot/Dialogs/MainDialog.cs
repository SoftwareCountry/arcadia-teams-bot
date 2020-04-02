namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    using Cards;

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

            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(new List<Attachment>());
            reply.Attachments.Add(Cards.GetChoiceCard().ToAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private static async Task<DialogTurnResult> RequestStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = (string)stepContext.Result;
            if ((string)stepContext.Values["choice"] == "New request")
            {
                return await stepContext.BeginDialogAsync(nameof(NewRequestDialog), null, cancellationToken);
            }
            return await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);
        }
    }
}
