namespace ArcadiaTeamsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions.Commands;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    public class NewRequestDialog : ComponentDialog
    {
        private const string Back = "Back";
        private const string Send = "Send";
        private const string Submit = "Submit";
        const string Username = "ekaterina.kuznetsova@arcadia.spb.ru";
        private readonly IRequestTypeUIFactory requestTypeUiFactory;
        private readonly IMediator mediator;

        public NewRequestDialog(IRequestTypeUIFactory requestTypeUIFactory, IMediator mediator) : base(nameof(NewRequestDialog))
        {
            this.requestTypeUiFactory = requestTypeUIFactory;
            this.mediator = mediator;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                TitleStep,
                DescriptionStep,
                PriorityStep,
                ExecutionDateStep,
                //AdditionalStep,
                ConfirmStep,
                CreateRequestStep,
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            this.AddDialog(new DateTimePrompt(nameof(DateTimePrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> TitleStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestTypeDTO = (ServiceDeskRequestTypeDTO)stepContext.Options;
            stepContext.Values["Type"] = requestTypeDTO.Id;
            stepContext.Values["Fields"] = requestTypeDTO.RequestTypeFields;
            stepContext.Values["DTO"] = requestTypeDTO;
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Type request title")
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> DescriptionStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Title"] = stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Type request description")
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> PriorityStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Description"] = stepContext.Result;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Type request priority"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Low", "Default", "High" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> ExecutionDateStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["PriorityId"] = stepContext.Result;
            return await stepContext.PromptAsync(nameof(DateTimePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Type execution date of request"),
                }, cancellationToken);
        }

        /*private async Task<DialogTurnResult> AdditionalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ExecutionDate"] = stepContext.Result;
            var additionalFields = this.requestTypeUiFactory.CreateRequestTypeUI((ServiceDeskRequestTypeDTO)stepContext.Values["DTO"]).RequestTypeUIFields;
            if (additionalFields == null)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            foreach (var field in additionalFields)
            {
                switch (field.FieldType)
                {
                    case RequestTypeUIFieldType.Year:
                        return await stepContext.PromptAsync(nameof(TextPrompt),
                            new PromptOptions
                            {
                                Prompt = MessageFactory.Text("Chose a year"),
                            }, cancellationToken);

                    case RequestTypeUIFieldType.Select:
                        return await stepContext.PromptAsync(nameof(ChoicePrompt),
                            new PromptOptions
                            {
                                Prompt = MessageFactory.Text("Choose an item of" + field.FieldName),
                                Choices = ((ServiceDeskRequestTypeDTO)stepContext.Values["DTO"]).RequestTypeFields
                                    .First(rt => rt.FieldName == field.FieldName).Items
                                    .Split(';')
                                    .Select(item => new Choice { Value = item })
                                    .ToList()
                            }, cancellationToken);
                    case RequestTypeUIFieldType.Number:
                        return await stepContext.PromptAsync(nameof(TextPrompt),
                            new PromptOptions
                            {
                                Prompt = MessageFactory.Text("Enter a number of" + field.FieldName),
                            }, cancellationToken);

                    case RequestTypeUIFieldType.String:
                        return await stepContext.PromptAsync(nameof(TextPrompt),
                            new PromptOptions
                            {
                                Prompt = MessageFactory.Text("Enter a string of" + field.FieldName),
                            }, cancellationToken);

                    default:
                        return await stepContext.PromptAsync(nameof(TextPrompt),
                            new PromptOptions
                            {
                                Prompt = MessageFactory.Text("Enter a string of" + field.FieldName),
                            }, cancellationToken);
                }
            }
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }*/
        private static async Task<DialogTurnResult> ConfirmStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Send or Back?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { Send, Back }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> CreateRequestStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((string)stepContext.Values["PriorityId"] == "Low")
            {
                stepContext.Values["PriorityId"] = "1";
            }
            else if ((string)stepContext.Values["PriorityId"] == "Default")
            {
                stepContext.Values["PriorityId"] = "2";
            }
            else
            {
                stepContext.Values["PriorityId"] = "3";
            }
            stepContext.Values["Field"] = stepContext.Result;
            var data = new CreateRequestDTO()
            {
                Title = stepContext.Values["Title"].ToString(),
                Description = stepContext.Values["Description"].ToString(),
                Type = new CreateRequestTypeDTO
                {
                    Id = (int)stepContext.Values["Type"],
                },
                PriorityId = (int)stepContext.Values["PriorityId"],
                ExecutionDate = (DateTime?)stepContext.Values["ExecutionDate"],
                Username = Username,
            };

            if ((string)stepContext.Result == Send)
            {
                var sendRequestsQuery = new CreateNewServiceDeskRequestCommand(data);
                await this.mediator.Send(sendRequestsQuery, cancellationToken);
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            if ((string)stepContext.Result == Back)
            {
                return await stepContext.ReplaceDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);
            }

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }
    }
}
