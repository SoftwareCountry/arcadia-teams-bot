namespace ArcadiaTeamsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AdaptiveCards;

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
        private const string Back = "Back";
        private const string Submit = "Submit";
        private const string Username = "ekaterina.kuznetsova@arcadia.spb.ru";
        private readonly IRequestTypeUIFactory requestTypeUiFactory;
        private readonly IMediator mediator;

        public NewRequestDialog(IMediator mediator, IRequestTypeUIFactory requestTypeUIFactory) : base(nameof(NewRequestDialog))
        {
            this.mediator = mediator; 
            this.requestTypeUiFactory = requestTypeUIFactory;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.InputStep,
                EndStep
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
            this.AddDialog(new TextPrompt("askForInput", this.AdaptiveCardVerify));
        }

        private  async Task<bool> AdaptiveCardVerify(PromptValidatorContext<string> propmptContext, CancellationToken cancellationToken)
        {
            if (propmptContext.Context.Activity.Value != null)
            {
                var data = (JObject)propmptContext.Context.Activity.Value;
                var dataForCreateRequest = new CreateRequestDTO
                {
                    Title = data["Title"].ToString(),
                    Description = data["Description"].ToString(),
                    Type = new CreateRequestTypeDTO { Id = (int)data["Type"] },
                    PriorityId = (int?)data["PriorityId"],
                    ExecutionDate = (DateTime?)data["ExecutionDate"],
                    Username = Username,
                    //FieldValues = (List<string>)data;
                };

                var sendRequestsQuery = new CreateNewServiceDeskRequestCommand(dataForCreateRequest); 
                await this.mediator.Send(sendRequestsQuery, cancellationToken);
            }

            return false;
        }

        private async Task<DialogTurnResult> InputStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestTypeDTO = (ServiceDeskRequestTypeDTO)stepContext.Options;
            var typeId = requestTypeDTO.Id;

            var additionalFields = this.requestTypeUiFactory.CreateRequestTypeUI(requestTypeDTO).RequestTypeUIFields;

            var body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock() { Text = "Input all data of request", Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Large },

                new AdaptiveTextInput { Id = "Type", Placeholder = "Type", Value = typeId.ToString()},

                new AdaptiveTextBlock("Enter title of request"),
                new AdaptiveTextInput { Id = "Title", Placeholder = "Title", Value = null},

                new AdaptiveTextBlock("Enter description of request"),
                new AdaptiveTextInput { Id = "Description", Placeholder = "Description", Value = null },

                new AdaptiveTextBlock("Enter execute date"),
                new AdaptiveDateInput { Id = "ExecuteDate", Value = null },

                new AdaptiveTextBlock("Choose a priority"),
                new AdaptiveChoiceSetInput
                {
                    Type = AdaptiveChoiceSetInput.TypeName,
                    Value = "2",
                    IsMultiSelect = false,
                    Id = "PriorityId",
                    Choices = new List<AdaptiveChoice>
                    {
                        new AdaptiveChoice { Title = "Low", Value = "1" },
                        new AdaptiveChoice { Title = "Default", Value = "2" },
                        new AdaptiveChoice { Title = "High", Value = "3" }
                    }
                }
            };

            foreach (var field in additionalFields)
            {
                AdaptiveTextBlock textBlock;
                AdaptiveInput input;

                switch (field.FieldType)
                {
                    case RequestTypeUIFieldType.Year:
                        textBlock = new AdaptiveTextBlock("Choose a year");
                        input = new AdaptiveDateInput { Id = field.FieldName, Placeholder = field.FieldName };
                        break;

                    case RequestTypeUIFieldType.Select:
                        textBlock = new AdaptiveTextBlock("Choose an item");
                        input = new AdaptiveChoiceSetInput
                        {
                            Type = AdaptiveChoiceSetInput.TypeName,
                            IsMultiSelect = false,
                            Id = "Items",
                            Choices = requestTypeDTO.RequestTypeFields
                                .First(rt => rt.FieldName == field.FieldName).Items
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

                body.Add(textBlock);
                body.Add(input);
            }

            var Actions = new List<AdaptiveAction>();
            var backAction = new AdaptiveSubmitAction
            {
                Title = Back,
                Data = Back,
            };

            var submitAction = new AdaptiveSubmitAction()
            {
                Title = Submit,
            };

            Actions.Add(submitAction);
            //Actions.Add(backAction);

            var attachment = InputCard(body, Actions);
            var opts = new PromptOptions()
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Attachments = new List<Attachment> { attachment }
                }
            };

            await stepContext.Context.SendActivityAsync(opts.Prompt, cancellationToken);
            opts.Prompt = new Activity(type: ActivityTypes.Typing);
            return await stepContext.PromptAsync("askForInput", opts, cancellationToken);
        }

        private static async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((string)stepContext.Result == Back)
            {
                return await stepContext.BeginDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);
            }
            return await stepContext.ContinueDialogAsync(cancellationToken);
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
    }
}
