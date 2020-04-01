namespace ArcadiaTeamsBot.ServiceDesk
{
    using System.Collections.Generic;

    public class ServiceDeskConfiguration
    {
        public string ApiUrl { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
