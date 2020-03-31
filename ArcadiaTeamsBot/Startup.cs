namespace ArcadiaTeamsBot
{
    using MediatR;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk;
    using ArcadiaTeamsBot.Infrastructure;
    using ArcadiaTeamsBot.CQRS.Handlers;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMediatR(typeof(GetServiceDeskRequestTypesHandler).Assembly);

            services.AddHttpClient();

            services.AddScoped<IServiceDeskClient, ServiceDeskClient>();
            services.AddTransient<IBot, Bot>();
            services.AddSingleton<IBotFrameworkHttpAdapter, BotAdapterWithErrorHandling>();
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