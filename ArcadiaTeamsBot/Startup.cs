namespace ArcadiaTeamsBot
{
    using ArcadiaTeamsBot.Dialogs;
    using ArcadiaTeamsBot.Infrastructure;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IBotFrameworkHttpAdapter, BotAdapterWithErrorHandling>();

            services.AddSingleton<IStorage, MemoryStorage>();

            services.AddSingleton<ConversationState>();

            services.AddSingleton<MainDialog>();

            services.AddTransient<IBot, Bot<MainDialog>>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}