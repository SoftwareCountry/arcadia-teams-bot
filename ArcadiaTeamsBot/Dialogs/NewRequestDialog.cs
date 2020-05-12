namespace ArcadiaTeamsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AdaptiveCards;

    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.CQRS.Abstractions.Commands;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestType;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    using Newtonsoft.Json.Linq;

    internal class NewRequestDialog : ComponentDialog
    {
        private const string Submit = "Submit";
        private const string Cancel = "Cancel";
        private const string Yes = "Yes";
        private const string No = "No";
        private const string FormTitle = "Title";
        private const string FormTypeId = "TypeId";
        private const string InputValidation = "InputValidation";
        private const string FormPriority = "Priority";
        private const string FormDescription = "Description";
        private const string FormExecutionDate = "Execution Date";
        private const string Username = "ekaterina.kuznetsova@arcadia.spb.ru";

        private readonly IRequestTypeUIFactory requestTypeUIFactory;
        private readonly IMediator mediator;
        private readonly IServiceDeskClient serviceDeskClient;
        public interface IButtonData { string Button { get; set; } }

        public NewRequestDialog(IMediator mediator, IRequestTypeUIFactory requestTypeUIFactory, IServiceDeskClient serviceDeskClient) : base(nameof(NewRequestDialog))
        {
            this.serviceDeskClient = serviceDeskClient;
            this.mediator = mediator;
            this.requestTypeUIFactory = requestTypeUIFactory;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.InputStep,
                this.ChoiceStep,
                this.EndStep
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
            this.AddDialog(new TextPrompt(InputValidation, this.ValidateForm));
        }

        private async Task<bool> ValidateForm(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var formData = new FormData(promptContext.Context);
            formData.GetFormData();

            if (formData.IsCancelled)
            {
                return true;
            }

            if (string.IsNullOrEmpty(formData.Title)
                || string.IsNullOrEmpty(formData.Description)
                || string.IsNullOrEmpty(formData.ExecutionDate.ToString())
                || formData.AdditionalFields.Any(string.IsNullOrEmpty))
            {
                return false;
            }

            var newRequest = new CreateRequestDTO
            {
                Title = formData.Title,
                Description = formData.Description,
                Priority = formData.Priority,
                ExecutionDate = formData.ExecutionDate,
                Username = Username,
                FieldValues = (IList<string>)formData.AdditionalFields,
                Type = new CreateRequestTypeDTO
                {
                    Id = Convert.ToInt32(formData.TypeId),
                    RequestTypeFields = await this.GetRequestTypeField(promptContext, cancellationToken)
                }
            };

            var sendRequestsQuery = new CreateNewServiceDeskRequestCommand(newRequest);
            await this.mediator.Send(sendRequestsQuery, cancellationToken);

            await promptContext.Context.SendActivityAsync(MessageFactory.Text("New request created successfully"), cancellationToken);
            return true;
        }

        private async Task<DialogTurnResult> InputStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestTypeDTO = (ServiceDeskRequestTypeDTO)stepContext.Options;

            var priorities = await this.serviceDeskClient.GetPriorities(cancellationToken);
            var adaptiveChoiceList = priorities.Select(priority => new AdaptiveChoice { Title = priority.Value, Value = priority.Key.ToString() }).ToList();

            var cardBody = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock { Text = requestTypeDTO.Title, Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Large },

                new AdaptiveTextInput { Id = FormTypeId, Value = requestTypeDTO.Id.ToString(), IsVisible = false },

                new AdaptiveTextBlock("Title"),
                new AdaptiveTextInput { Id = FormTitle, Placeholder = "Title"},

                new AdaptiveTextBlock("Description"),
                new AdaptiveTextInput { Id = FormDescription, Placeholder = "Description" },

                new AdaptiveTextBlock("Execution Date"),
                new AdaptiveDateInput { Id = FormExecutionDate },

                new AdaptiveTextBlock("Priority"),
                new AdaptiveChoiceSetInput
                {
                    Value = "2",
                    Id = FormPriority,
                    Choices = adaptiveChoiceList
                }
            };

            var requestTypeUIFields = this.requestTypeUIFactory.CreateRequestTypeUI(requestTypeDTO).RequestTypeUIFields.ToList();

            foreach (var field in requestTypeUIFields)
            {
                AdaptiveTextBlock textBlock;
                AdaptiveInput input;

                switch (field.FieldType)
                {
                    case RequestTypeUIFieldType.Year:
                        textBlock = new AdaptiveTextBlock(field.FieldName);
                        input = new AdaptiveNumberInput { Id = field.FieldName, Placeholder = "Enter a year", Min = 1900 };
                        break;

                    case RequestTypeUIFieldType.Select:
                        textBlock = new AdaptiveTextBlock(field.FieldName);

                        input = new AdaptiveChoiceSetInput
                        {
                            Type = AdaptiveChoiceSetInput.TypeName,
                            IsMultiSelect = false,
                            Id = field.FieldName,
                            Choices = requestTypeDTO.RequestTypeFields
                                .First(requestTypeFieldDTO => requestTypeFieldDTO.FieldName == field.FieldName)
                                .Items
                                .Split(';')
                                .Select(item => new AdaptiveChoice { Title = item, Value = item })
                                .ToList()
                        };
                        break;

                    case RequestTypeUIFieldType.Number:
                        textBlock = new AdaptiveTextBlock(field.FieldName);
                        input = new AdaptiveNumberInput { Id = field.FieldName, Placeholder = "Enter a number" };
                        break;

                    case RequestTypeUIFieldType.String:
                        textBlock = new AdaptiveTextBlock(field.FieldName);
                        input = new AdaptiveTextInput { Id = field.FieldName, Placeholder = "Enter a string" };
                        break;

                    default:
                        textBlock = new AdaptiveTextBlock(field.FieldName);
                        input = new AdaptiveTextInput { Id = field.FieldName, Placeholder = "Enter a string" };
                        break;
                }

                cardBody.Add(textBlock);
                cardBody.Add(input);
            }

            var Actions = new List<AdaptiveAction>
            {
                new AdaptiveSubmitAction
                {
                    Title = Submit,
                    Data = new Dictionary<string, string> { {nameof(IButtonData.Button), Submit} }
                },
                new AdaptiveSubmitAction
                {
                    Title = Cancel,
                    Data = new Dictionary<string, string> { {nameof(IButtonData.Button), Cancel} }
                }
            };

            var promptOptions = new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(GetInputFormCard(cardBody, Actions)),
                RetryPrompt = MessageFactory.Text("Not all fields are filled. Repeat please.")
            };

            await stepContext.Context.SendActivityAsync(promptOptions.Prompt, cancellationToken);
            promptOptions.Prompt = new Activity(ActivityTypes.Message);
            return await stepContext.PromptAsync(InputValidation, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var formData = new FormData(stepContext.Context);
            formData.GetFormData();

            if (formData.IsCancelled)
            {
                return await stepContext.BeginDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);

            }
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(GetChoiceCard().ToAttachment())
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return stepContext.Result switch
            {
                Yes => await stepContext.BeginDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken),
                _ => await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken)
            };
        }

        private static Attachment GetInputFormCard(List<AdaptiveElement> body, List<AdaptiveAction> actions)
        {
            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = new AdaptiveCard
                {
                    Body = body,
                    Actions = actions
                }
            };
        }

        private static HeroCard GetChoiceCard()
        {
            return new HeroCard
            {
                Title = "Do you want to create one more request?",

                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, Yes, value: Yes),
                    new CardAction(ActionTypes.ImBack, No, value: No)
                }
            };
        }

        private async Task<IEnumerable<CreateRequestTypeFieldDTO>> GetRequestTypeField(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var formData = new FormData(promptContext.Context);
            formData.GetFormData();

            var createRequestTypeFields =
                (await this.mediator.Send(new GetServiceDeskRequestTypesQuery(), cancellationToken))
                .First(requestTypeDTO => requestTypeDTO.Id == Convert.ToInt32(formData.TypeId))
                .RequestTypeFields
                .Select(requestTypeFieldDTO => new CreateRequestTypeFieldDTO
                {
                    Id = requestTypeFieldDTO.Id,
                    FieldName = requestTypeFieldDTO.FieldName,
                    IsMandatory = requestTypeFieldDTO.IsMandatory
                });

            return createRequestTypeFields;
        }

        private class FormData
        {
            private const int FirstNumberOfAdditionalFields = 6;
            internal string Title { get; private set; }
            internal string Description { get; private set; }
            internal bool IsCancelled { get; private set; }
            internal int TypeId { get; private set; }
            internal int? Priority { get; private set; }
            internal DateTime? ExecutionDate { get; private set; }
            internal IEnumerable<string> AdditionalFields { get; private set; }

            private readonly JObject formData;

            public FormData(ITurnContext context)
            {
                this.formData = (JObject)context.Activity.Value;
            }

            public void GetFormData()
            {
                if (this.formData == null)
                {
                    return;
                }

                this.Title = this.GetStringField(FormTitle);
                this.Description = this.GetStringField(FormDescription); 
                this.Priority = Convert.ToInt32(this.GetStringField(FormPriority));
                this.TypeId = Convert.ToInt32(this.GetStringField(FormTypeId));

                if (this.GetStringField(nameof(IButtonData.Button)) == Cancel)
                {
                    this.IsCancelled = true;
                }

                if (DateTime.TryParse(this.GetStringField(FormExecutionDate), out var date))
                {
                    this.ExecutionDate = date;
                }

                var additionalFieldsData = (this.formData).Properties()
                    .Where(p => p.Value.Type == JTokenType.String).ToList();

                this.AdditionalFields = additionalFieldsData.Select((value, index) => new { Index = index, Value = value })
                    .Where(p => p.Index >= FirstNumberOfAdditionalFields)
                    .Select(p => p.Value.Last.ToString()).ToList();
            }

            private string GetStringField(string name)
            {
                return this.formData[name].ToString();
            }
        }
    }
}
