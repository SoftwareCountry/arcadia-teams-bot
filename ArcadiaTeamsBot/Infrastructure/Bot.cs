using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ArcadiaTeamsBot.Infrastructure
{
    public class Bot : ActivityHandler
    {
        public const string WelcomeText = @"You can create a new request or view opened requests:";

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendWelcomeMessageAsync(turnContext, cancellationToken);
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var text = turnContext.Activity.Text.ToLowerInvariant();

            var responseText = ProcessInput(text);

            await turnContext.SendActivityAsync(responseText, cancellationToken: cancellationToken);

            await SendSuggestedActionsAsync(turnContext, cancellationToken);
        }
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to ArcadiaBot, {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                    await SendSuggestedActionsAsync(turnContext, cancellationToken);
                }
            }
        }

        private static string ProcessInput(string text)
        {
            switch (text)
            {
                case "open":
                    {
                        return $"Here should be opened «Open requests»";
                    }

                case "new":
                    {
                        return $" Here should be opened «New request»";
                    }

                default:
                    {
                        return "Please select a card";
                    }
            }
        }

        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("What would you like to do?");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "View opened requests", Type = ActionTypes.ImBack, Value = "open" },
                    new CardAction() { Title = "Create a new request", Type = ActionTypes.ImBack, Value = "new" },
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
