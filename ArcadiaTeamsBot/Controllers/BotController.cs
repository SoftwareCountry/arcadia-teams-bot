namespace ArcadiaTeamsBot.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;

    [ApiController]
    [Route("api/messages")]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter botAdapter;

        private readonly IBot bot;

        public BotController(IBotFrameworkHttpAdapter botAdapter, IBot bot)
        {
            this.botAdapter = botAdapter;
            this.bot = bot;
        }

        [HttpPost]
        public async Task OnPostAsync(CancellationToken cancellationToken)
        {
            await this.botAdapter.ProcessAsync(this.Request, this.Response, this.bot, cancellationToken);
        }
    }
}