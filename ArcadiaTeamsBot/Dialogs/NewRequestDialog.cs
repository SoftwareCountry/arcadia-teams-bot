namespace ArcadiaTeamsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private const string back = "Back";
        private const string send = "Send";
        private const string username = "ekaterina.kuznetsova@arcadia.spb.ru";
        private readonly IRequestTypeUIFactory requestTypeUIFactory;
        private readonly IMediator mediator;

        public NewRequestDialog(IRequestTypeUIFactory requestTypeUIFactory, IMediator mediator) : base(nameof(NewRequestDialog))
        {
            this.requestTypeUIFactory = requestTypeUIFactory;
            this.mediator = mediator;
            this.AddDialog(new AdditionalFieldsDialog());

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.TitleStep,
                DescriptionStep,
                PriorityStep,
                ExecutionDateStep,
                SaveDateStep,
                this.AdditionalFieldsStep,
                ConfirmStep,
                this.CreateRequestStep
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
                    Prompt = MessageFactory.Text("Enter title")
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> DescriptionStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Title"] = stepContext.Result;

            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Enter description")
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> PriorityStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Description"] = stepContext.Result;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Choose priority"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Low", "Default", "High" })
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> ExecutionDateStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["PriorityId"] = stepContext.Result switch
            {
                "Low" => "1",
                "High" => "3",
                _ => "2"
            };

            return await stepContext.PromptAsync(nameof(DateTimePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Enter execution date")
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> SaveDateStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var dateTime = ((IList<DateTimeResolution>)stepContext.Result).First();
            stepContext.Values["ExecutionDate"] = Convert.ToDateTime(dateTime.Value);

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        private async Task<DialogTurnResult> AdditionalFieldsStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var additionalFields = this.requestTypeUIFactory.CreateRequestTypeUI((ServiceDeskRequestTypeDTO)stepContext.Values["DTO"]).RequestTypeUIFields.ToList();

            if (!additionalFields.Any())
            {
                return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
            }
            var fields = new List<string>();

            for (var i = 0; i < additionalFields.Count; i++)
            {
                await stepContext.BeginDialogAsync(nameof(AdditionalFieldsDialog), additionalFields[i], cancellationToken);

                stepContext.Values[(7 + i).ToString()] = stepContext.Result;
                fields.Add(stepContext.Values[(7 + i).ToString()].ToString());
            }

            stepContext.Values["Fields"] = fields;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { }, cancellationToken);
        }

        private static async Task<DialogTurnResult> ConfirmStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Send or go back?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { send, back })
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> CreateRequestStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var data = new CreateRequestDTO
            {
                Title = stepContext.Values["Title"].ToString(),
                Description = stepContext.Values["Description"].ToString(),
                Type = new CreateRequestTypeDTO { Id = (int)stepContext.Values["Type"] },
                PriorityId = Convert.ToInt32(stepContext.Values["PriorityId"]),
                ExecutionDate = (DateTime?)stepContext.Values["ExecutionDate"],
                Username = username,
                FieldValues = (List<string>)stepContext.Values["Fields"]
            };

            //data.FieldValues = new List<string>{"bla","bla"};

            switch (((FoundChoice)stepContext.Result).Value)
            {
                case send:
                    var sendRequestsQuery = new CreateNewServiceDeskRequestCommand(data);
                    await this.mediator.Send(sendRequestsQuery, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);

                case back:
                    return await stepContext.ReplaceDialogAsync(nameof(RequestsTypeDialog), null, cancellationToken);

                default:
                    return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }
    }
}
