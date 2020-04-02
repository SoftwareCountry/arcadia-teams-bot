namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class OpenedRequestsDialog : ComponentDialog
    {
        public OpenedRequestsDialog() : base(nameof(OpenedRequestsDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                TypeStep,
            }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> TypeStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(new List<Attachment>());
            reply.Attachments.Add(GetInfoCard().ToAttachment());

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Type anything to continue."), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        public static HeroCard GetInfoCard()
        {
            var infoCard = new HeroCard
            {
                Title = "In Development",
            };
            return infoCard;
        }
    }
}
