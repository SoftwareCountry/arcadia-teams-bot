namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestType;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    public class AdditionalFieldsDialog : ComponentDialog
    {
        private IList<RequestTypeUIField> additionalFields;

        public AdditionalFieldsDialog()
        {
            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AdditionalFieldStep,
                EndStep
            }));
        }

        private async Task<DialogTurnResult> AdditionalFieldStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var currentField = (RequestTypeUIField)stepContext.Options;
            switch (currentField.FieldType)
            {
                case RequestTypeUIFieldType.Year:
                    return await stepContext.PromptAsync(nameof(DateTimePrompt),
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text("Choose a year")
                        }, cancellationToken);

                case RequestTypeUIFieldType.Select:
                    return await stepContext.PromptAsync(nameof(ChoicePrompt),
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text("Choose an item of" + currentField.FieldName),
                            Choices = ((ServiceDeskRequestTypeDTO)stepContext.Values["DTO"]).RequestTypeFields
                                .First(rt => rt.FieldName == currentField.FieldName).Items
                                .Split(';')
                                .Select(item => new Choice { Value = item })
                                .ToList()
                        }, cancellationToken);

                case RequestTypeUIFieldType.Number:
                    return await stepContext.PromptAsync(nameof(NumberPrompt<int>),
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text("Enter a number of " + currentField.FieldName)
                        }, cancellationToken);

                case RequestTypeUIFieldType.String:
                    stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 1;

                    return await stepContext.PromptAsync(nameof(TextPrompt),
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text("Enter a string of " + currentField.FieldName)
                        }, cancellationToken);

                default:
                    return await stepContext.PromptAsync(nameof(TextPrompt),
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text("Enter a string of " + currentField.FieldName)
                        }, cancellationToken);
            }
        }
       
        private async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
