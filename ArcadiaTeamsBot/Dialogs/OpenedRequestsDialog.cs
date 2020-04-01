namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    public class OpenedRequestsDialog : ComponentDialog
    {
        public OpenedRequestsDialog() : base(nameof(OpenedRequestsDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                TypeStep,
                ResultStep,
            }));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> TypeStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What requests do you want to see?"),
                    //types from serviceDesk
                    Choices = ChoiceFactory.ToChoices(new List<string> { "transport", "ndfl" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> ResultStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Type"] = ((FoundChoice)stepContext.Result).Value;
            await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Requests: bla bla "),
                }, cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
