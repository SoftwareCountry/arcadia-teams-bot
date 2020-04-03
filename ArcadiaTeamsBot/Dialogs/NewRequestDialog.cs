﻿namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class NewRequestDialog : ComponentDialog
    {
        public NewRequestDialog() : base(nameof(NewRequestDialog))
        {
            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InfoStep,
            }));

            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> InfoStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachments = new[]
            {
                GetInfoCard().ToAttachment()
            };

            var reply = MessageFactory.Attachment(attachments);
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }


        public static HeroCard GetInfoCard()
        {
            var infoCard = new HeroCard
            {
                Title = "New In Development",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Back", value: "Back"),
                },
            };

            return infoCard;
        }
    }
}
