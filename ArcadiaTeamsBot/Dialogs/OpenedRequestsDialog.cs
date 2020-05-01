namespace ArcadiaTeamsBot.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AdaptiveCards;

    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    using MediatR;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    internal class OpenedRequestsDialog : ComponentDialog
    {
        private const string Back = "Back";
        private const string Username = "ekaterina.kuznetsova@arcadia.spb.ru";
        private readonly IMediator mediator;

        public OpenedRequestsDialog(IMediator mediator) : base(nameof(OpenedRequestsDialog))
        {
            this.mediator = mediator;

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                this.InfoStep,
                this.EndStep
            }));

            this.AddDialog(new TextPrompt(nameof(TextPrompt)));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InfoStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var getOpenedRequestsQuery = new GetCurrentServiceDeskRequestsQuery(Username);
            var openedRequest = await this.mediator.Send(getOpenedRequestsQuery, cancellationToken);

            var actions = openedRequest.Select(GetRequestAction).ToList();
            actions.Add(new AdaptiveSubmitAction { Title = Back, Data = Back });

            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(GetOpenedRequestsCard(actions)), cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions(), cancellationToken);
        }

        private async Task<DialogTurnResult> EndStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            switch (stepContext.Result)
            {
                case Back:
                    await stepContext.EndDialogAsync(null, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(MainDialog), null, cancellationToken);

                default:
                    return await stepContext.ReplaceDialogAsync(nameof(OpenedRequestsDialog), null, cancellationToken);
            }
        }

        private static Attachment GetOpenedRequestsCard(List<AdaptiveAction> actions)
        {
            var card = new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = "All your opened requests",
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large
                    }
                },
                Actions = actions
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private static AdaptiveAction GetRequestAction(ServiceDeskRequestDTO request)
        {
            var executorFullName = string.IsNullOrEmpty(request.ExecutorFullName) ? "-" : request.ExecutorFullName;
            var action = new AdaptiveShowCardAction
            {
                Title = request.RequestNumber,
                
                Card = new AdaptiveCard
                {
                    Body = new List<AdaptiveElement>
                    {
                        new AdaptiveTextBlock { Text = $"{request.RequestNumber}: {request.Title}", Size = AdaptiveTextSize.Large },
                        new AdaptiveRichTextBlock
                        {
                            Inlines = new List<IAdaptiveInline>
                            {
                                new AdaptiveTextRun { Text = "Created: ", Weight = AdaptiveTextWeight.Bolder },
                                new AdaptiveTextRun { Text = request.Created.ToLongDateString() }
                            }
                        },
                        new AdaptiveRichTextBlock
                        {
                            Inlines = new List<IAdaptiveInline>
                            {
                                new AdaptiveTextRun { Text = "Status: ", Weight = AdaptiveTextWeight.Bolder },
                                new AdaptiveTextRun { Text = request.StatusName }
                            }
                        },
                        new AdaptiveRichTextBlock
                        {
                            Inlines = new List<IAdaptiveInline>
                            {
                                new AdaptiveTextRun { Text = "Executor: ", Weight = AdaptiveTextWeight.Bolder },
                                new AdaptiveTextRun { Text = executorFullName}
                            }
                        }
                    }
                }
            };
            return action;
        }
    }
}
