namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using AdaptiveCards;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    public class NewRequestDialog : ComponentDialog
    {
        private const string Back = "Back";
        private readonly IRequestTypeUIFactory request;
        public NewRequestDialog(IRequestTypeUIFactory request) : base(nameof(NewRequestDialog))
        {
            this.request = request;
            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InputStep,
                EndStep,
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InputStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var additionalFields = this.request.CreateRequestTypeUI((ServiceDeskRequestTypeDTO)stepContext.Options).RequestTypeUIFields;
            var data = new CreateRequestDTO();
            var body = new List<AdaptiveElement>()
            {
                new AdaptiveTextBlock("Enter title of request"),
                new AdaptiveTextInput() { Id = "Title", Placeholder = "Title", Value = data.Title  },
                new AdaptiveTextBlock("Enter description of request"),
                new AdaptiveTextInput() { Id = "Description", Placeholder = "Description", Value = data.Description },
                new AdaptiveTextBlock("Enter execute date"),
                new AdaptiveTextInput() { Id = "ExecuteDate", Placeholder = "ExecuteDate", Value = data.ExecutionDate.ToString()},
                new AdaptiveTextBlock("Choose a priority"),
                new AdaptiveChoiceSetInput
                {
                    Type = AdaptiveChoiceSetInput.TypeName,
                    Value = data.PriorityId.ToString(),
                    IsMultiSelect = false,
                    Id = "PriorityId",
                    Choices = new List<AdaptiveChoice>
                    {
                        new AdaptiveChoice() { Title = "Low", Value = "1" },
                        new AdaptiveChoice() { Title = "Default", Value = "2" },
                        new AdaptiveChoice() { Title = "High", Value = "3" },
                    },
                }
            };

            foreach (var field in additionalFields)
            {
                var input = new AdaptiveTextInput() { Id = field.FieldName, Placeholder = field.FieldName };
                body.Add(input);
            }

            var attachments = InputCard(body);
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(attachments),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((string)stepContext.Result == Back)
            {
                return await stepContext.BeginDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);
            }
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        public static Attachment InputCard(List<AdaptiveElement> body)
        {
            var card = new AdaptiveCard()
            {
                Title = "Input all data",
                Body = body,
                //Actions = actions,
            };
            var attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            return attachment;
        }
    }
}