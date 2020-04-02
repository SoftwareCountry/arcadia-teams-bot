namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    using Cards;

    public class NewRequestDialog : ComponentDialog
    {
        public NewRequestDialog() : base(nameof(NewRequestDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStep,
            }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> InfoStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);
            reply.Attachments.Add(Cards.GetInfoCard().ToAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Type anything to continue."), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
