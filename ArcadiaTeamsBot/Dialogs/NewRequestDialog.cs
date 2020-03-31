namespace ArcadiaTeamsBot.Dialogs
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class NewRequestDialog : ComponentDialog
    {
        public NewRequestDialog() : base(nameof(NewRequestDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                TypeStep,
                AnotherSteps,
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
                    Prompt = MessageFactory.Text("What request do you want to create?"),
                    //types from serviceDesk
                    Choices = ChoiceFactory.ToChoices(new List<string> { "transport", "ndfl" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> AnotherSteps(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Type"] = ((FoundChoice)stepContext.Result).Value;
            await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("another questions "),
                }, cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
