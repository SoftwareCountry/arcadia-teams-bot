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
        private const string Button = "Button";
        private const string Title = "Title";
        private const string TypeId = "TypeId";
        private const string Priority = "Priority";
        private const string Description = "Description";
        private const string ExecutionDate = "ExecutionDate";
        private const string Username = "ekaterina.kuznetsova@arcadia.spb.ru";
        private readonly IRequestTypeUIFactory requestTypeUIFactory;
        private readonly IMediator mediator;

        public NewRequestDialog(IMediator mediator, IRequestTypeUIFactory requestTypeUIFactory) : base(nameof(NewRequestDialog))
        {
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
            this.AddDialog(new TextPrompt("askForInput", this.ValidateForm));
        }

        private async Task<bool> ValidateForm(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var formData = (JObject)promptContext.Context.Activity.Value;
            if (formData == null)
            {
                return false;
            }

            if (formData[Button].ToString() == Cancel)
            {
                return true;
            }

            var additionalFieldsValues = new List<string>();
            var additionalFieldsData = (formData).Properties()
                .Where(p => p.Value.Type == JTokenType.String).ToList();

            for (var i = 6; i < additionalFieldsData.Count; i++)
            {
                additionalFieldsValues.Add(additionalFieldsData[i].Last.ToString());
            };

            if (string.IsNullOrEmpty(formData[Title].ToString())
                || string.IsNullOrEmpty(formData[Description].ToString())
                || string.IsNullOrEmpty(formData[ExecutionDate].ToString())
                || additionalFieldsValues.Any(string.IsNullOrEmpty))
            {
                return false;
            }

            var createRequestTypeFields =
                (await this.mediator.Send(new GetServiceDeskRequestTypesQuery(), cancellationToken))
                .First(requestTypeDTO => requestTypeDTO.Id == Convert.ToInt32(formData[TypeId].ToString()))
                .RequestTypeFields
                .Select(requestTypeFieldDTO => new CreateRequestTypeFieldDTO
                {
                    Id = requestTypeFieldDTO.Id,
                    FieldName = requestTypeFieldDTO.FieldName,
                    IsMandatory = requestTypeFieldDTO.IsMandatory
                });

            var newRequest = new CreateRequestDTO
            {
                Title = formData[Title].ToString(),
                Description = formData[Description].ToString(),
                Priority = Convert.ToInt32(formData[Priority].ToString()),
                ExecutionDate = Convert.ToDateTime(formData[ExecutionDate].ToString()),
                Username = Username,
                FieldValues = additionalFieldsValues,
                Type = new CreateRequestTypeDTO
                {
                    Id = Convert.ToInt32(formData[TypeId].ToString()),
                    RequestTypeFields = createRequestTypeFields
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

            var cardBody = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock { Text = requestTypeDTO.Title, Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Large },

                new AdaptiveTextInput { Id = "TypeId", Value = requestTypeDTO.Id.ToString(), IsVisible = false },

                new AdaptiveTextBlock("Title"),
                new AdaptiveTextInput { Id = "Title", Placeholder = "Title" },

                new AdaptiveTextBlock("Description"),
                new AdaptiveTextInput { Id = "Description", Placeholder = "Description" },

                new AdaptiveTextBlock("Execution date"),
                new AdaptiveDateInput { Id = "ExecutionDate" },

                new AdaptiveTextBlock("Priority"),
                new AdaptiveChoiceSetInput
                {
                    Value = "2",
                    Id = "Priority",
                    Choices = new List<AdaptiveChoice>
                    {
                        new AdaptiveChoice { Title = "Low", Value = "1" },
                        new AdaptiveChoice { Title = "Default", Value = "2" },
                        new AdaptiveChoice { Title = "High", Value = "3" }
                    }
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
                        textBlock = new AdaptiveTextBlock("Enter a year");
                        input = new AdaptiveNumberInput { Id = field.FieldName, Placeholder = field.FieldName, Min = 1900 };
                        break;

                    case RequestTypeUIFieldType.Select:
                        textBlock = new AdaptiveTextBlock("Choose an item");

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
                        textBlock = new AdaptiveTextBlock("Enter a number");
                        input = new AdaptiveNumberInput { Id = field.FieldName, Placeholder = field.FieldName };
                        break;

                    case RequestTypeUIFieldType.String:
                        textBlock = new AdaptiveTextBlock("Enter a string");
                        input = new AdaptiveTextInput { Id = field.FieldName, Placeholder = field.FieldName };
                        break;

                    default:
                        textBlock = new AdaptiveTextBlock("Unknown field type. Enter a string");
                        input = new AdaptiveTextInput { Id = field.FieldName, Placeholder = field.FieldName };
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
                    Data = new { Button = Submit }
                },
                new AdaptiveSubmitAction
                {
                    Title = Cancel,
                    Data = new { Button = Cancel }
                }
            };

            var promptOptions = new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(InputFormCard(cardBody, Actions)),
                RetryPrompt = MessageFactory.Text("Not all fields are filled. Repeat")
            };

            await stepContext.Context.SendActivityAsync(promptOptions.Prompt, cancellationToken);
            promptOptions.Prompt = new Activity(ActivityTypes.Message);
            return await stepContext.PromptAsync("askForInput", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var formData = (JObject)stepContext.Context.Activity.Value;
            if (formData["Button"].ToString() == Cancel)
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

        private static Attachment InputFormCard(List<AdaptiveElement> body, List<AdaptiveAction> actions)
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
    }
}
