namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs.Choices;
    
    public class MainDialog : ComponentDialog
    {
        public const string WelcomeText = @" You can create a new request or view opened requests. Let's go?";
        public MainDialog() : base(nameof(MainDialog))
        {
            AddDialog(new NewRequestDialog());
            AddDialog(new OpenedRequestsDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                WelcomeStep,
                ChoiceStep,
                RequestStep,
            }));

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> WelcomeStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ConfirmPrompt),
                new PromptOptions { 
                    Prompt = MessageFactory.Text($"{WelcomeText}") 
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What would you like to do?"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "New", "Current" }),
                    }, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Bye.", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        private static async Task<DialogTurnResult> RequestStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["choice"] = ((FoundChoice)stepContext.Result).Value;
            if ((string)stepContext.Values["choice"] == "New")
            {
                return await stepContext.BeginDialogAsync(nameof(NewRequestDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);
            }
        }
    }
}
