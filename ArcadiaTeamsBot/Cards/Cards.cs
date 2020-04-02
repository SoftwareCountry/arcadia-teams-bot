namespace ArcadiaTeamsBot.Cards
{
    using System.Collections.Generic;

    using Microsoft.Bot.Schema;

    public class Cards
    {
        public static HeroCard GetChoiceCard()
        {
            var choiceCard = new HeroCard
            {
                Title = "What do you want to do?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "New request", value: "New request"),
                    new CardAction(ActionTypes.ImBack, "See current requests", value: "See current requests"),
                },
            };
            return choiceCard;
        }

        public static HeroCard GetInfoCard()
        {
            var infoCard = new HeroCard
            {
                Title = "In Development",
            };
            return infoCard;
        }
    }
}
