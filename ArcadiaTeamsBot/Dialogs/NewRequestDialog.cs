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

    public class NewRequestDialog : ComponentDialog
    {
        private const string Submit = "Submit";
        private const string Cancel = "Cancel";
        private const string NewRequest = "New request";
        private const string ViewOpened = "Opened requests";
        private const string Username = "vyacheslav.lasukov@arcadia.spb.ru";
        private readonly IRequestTypeUIFactory requestTypeUiFactory;
        private readonly IMediator mediator;

        public NewRequestDialog(IMediator mediator, IRequestTypeUIFactory requestTypeUIFactory) : base(nameof(NewRequestDialog))
        {
            this.mediator = mediator;
            this.requestTypeUiFactory = requestTypeUIFactory;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.InputStep,
                ChoiceStep,
                EndStep
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
            this.AddDialog(new TextPrompt("askForInput", this.AdaptiveCardVerify));
        }

        private async Task<bool> AdaptiveCardVerify(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var formData = (JObject)promptContext.Context.Activity.Value;

            if (formData == null)
            {
                return false;
            }

            if (formData["Button"].ToString() == Cancel)
            {
                return true;
            }

            var additionalFields = new List<string>();

            for (var i = 0; i < formData.Count; i++)
            {
                if (!formData.TryGetValue(i.ToString(), out var additionalField))
                {
                    continue;
                }

                additionalFields.Add(additionalField.ToString());
            }

            if (string.IsNullOrEmpty(formData["Title"].ToString())
                || string.IsNullOrEmpty(formData["Description"].ToString())
                || string.IsNullOrEmpty(formData["ExecutionDate"].ToString())
                || additionalFields.Any(field => field == null))
            {
                return false;
            }

            var createRequestTypeFields =
                (await this.mediator.Send(new GetServiceDeskRequestTypesQuery(), cancellationToken))
                .First(requestTypeDTO => requestTypeDTO.Id == Convert.ToInt32(formData["TypeId"].ToString()))
                .RequestTypeFields
                .Select(requestTypeFieldDTO => new CreateRequestTypeFieldDTO
                {
                    Id = requestTypeFieldDTO.Id,
                    FieldName = requestTypeFieldDTO.FieldName,
                    IsMandatory = requestTypeFieldDTO.IsMandatory
                });

            var newRequest = new CreateRequestDTO
            {
                Title = formData["Title"].ToString(),
                Description = formData["Description"].ToString(),
                Priority = Convert.ToInt32(formData["Priority"].ToString()),
                ExecutionDate = Convert.ToDateTime(formData["ExecutionDate"].ToString()),
                Username = Username,
                FieldValues = additionalFields,
                Type = new CreateRequestTypeDTO
                {
                    Id = Convert.ToInt32(formData["TypeId"].ToString()),
                    RequestTypeFields = createRequestTypeFields
                }
            };

            var sendRequestsQuery = new CreateNewServiceDeskRequestCommand(newRequest);
            await this.mediator.Send(sendRequestsQuery, cancellationToken);

            return true;
        }

        private async Task<DialogTurnResult> InputStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestTypeDTO = (ServiceDeskRequestTypeDTO)stepContext.Options;

            var additionalFields = this.requestTypeUiFactory.CreateRequestTypeUI(requestTypeDTO).RequestTypeUIFields;

            var body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock { Text = requestTypeDTO.Title, Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Large },

                new AdaptiveTextInput { Id = "TypeId", Placeholder = "Type", Value = requestTypeDTO.Id.ToString(), IsVisible = false },

                new AdaptiveTextBlock("Title"),
                new AdaptiveTextInput { Id = "Title", Placeholder = "Title", Value = null },

                new AdaptiveTextBlock("Description"),
                new AdaptiveTextInput { Id = "Description", Placeholder = "Description", Value = null },

                new AdaptiveTextBlock("Execution date"),
                new AdaptiveDateInput { Id = "ExecutionDate", Value = null },

                new AdaptiveTextBlock("Priority"),
                new AdaptiveChoiceSetInput
                {
                    Type = AdaptiveChoiceSetInput.TypeName,
                    IsMultiSelect = false,
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

            var requestTypeUIFields = additionalFields.ToList();

            for (var i = 0; i < requestTypeUIFields.Count; i++)
            {
                AdaptiveTextBlock textBlock;
                AdaptiveInput input;

                switch (requestTypeUIFields[i].FieldType)
                {
                    case RequestTypeUIFieldType.Year:
                        textBlock = new AdaptiveTextBlock("Choose a year");
                        input = new AdaptiveDateInput { Id = i.ToString(), Placeholder = requestTypeUIFields[i].FieldName };
                        break;

                    case RequestTypeUIFieldType.Select:
                        textBlock = new AdaptiveTextBlock("Choose an item");

                        input = new AdaptiveChoiceSetInput
                        {
                            Type = AdaptiveChoiceSetInput.TypeName,
                            IsMultiSelect = false,
                            Id = i.ToString(),
                            Choices = requestTypeDTO.RequestTypeFields
                                .First(requestTypeFieldDTO => requestTypeFieldDTO.FieldName == requestTypeUIFields[i].FieldName).Items
                                .Split(';')
                                .Select(item => new AdaptiveChoice { Title = item, Value = item })
                                .ToList()
                        };
                        break;

                    case RequestTypeUIFieldType.Number:
                        textBlock = new AdaptiveTextBlock("Enter a number");
                        input = new AdaptiveNumberInput { Id = i.ToString(), Placeholder = requestTypeUIFields[i].FieldName };
                        break;

                    case RequestTypeUIFieldType.String:
                        textBlock = new AdaptiveTextBlock("Enter a string");
                        input = new AdaptiveTextInput { Id = i.ToString(), Placeholder = requestTypeUIFields[i].FieldName };
                        break;

                    default:
                        textBlock = new AdaptiveTextBlock("Unknown field type. Enter a string");
                        input = new AdaptiveTextInput { Id = i.ToString(), Placeholder = requestTypeUIFields[i].FieldName };
                        break;
                }

                body.Add(textBlock);
                body.Add(input);
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

            var attachment = InputCard(body, Actions);

            var promptOptions = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Attachments = new List<Attachment> { attachment }
                },
                RetryPrompt = MessageFactory.Text("Not all fields are filled. Repeat")
            };

            await stepContext.Context.SendActivityAsync(promptOptions.Prompt, cancellationToken);
            promptOptions.Prompt = new Activity(ActivityTypes.Typing);

            return await stepContext.PromptAsync("askForInput", promptOptions, cancellationToken);
        }

        private static async Task<DialogTurnResult> ChoiceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actions = ActionCard();

            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(actions)
                },
                cancellationToken);
        }

        private static async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return (string)stepContext.Result switch
            {
                NewRequest => await stepContext.BeginDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken),
                ViewOpened => await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken),
                _ => await stepContext.ContinueDialogAsync(cancellationToken)
            };
        }

        public static Attachment InputCard(List<AdaptiveElement> body, List<AdaptiveAction> actions)
        {
            var card = new AdaptiveCard
            {
                Body = body,
                Actions = actions
            };

            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            return attachment;
        }

        public static Attachment ActionCard()
        {
            var infoCard = new HeroCard
            {
                Title = "You can create a new request or view opened requests.",
                Subtitle = "What do you want to do?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, NewRequest, value: NewRequest),
                    new CardAction(ActionTypes.ImBack, ViewOpened, value: ViewOpened)
                }
            };

            return infoCard.ToAttachment();
        }
    }
}
