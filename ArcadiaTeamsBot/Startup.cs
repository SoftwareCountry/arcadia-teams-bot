namespace ArcadiaTeamsBot
{
    using ArcadiaTeamsBot.Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MediatR;
    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ServiceDesk.Abstractions;
    using ServiceDesk;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IBotFrameworkHttpAdapter, BotAdapterWithErrorHandling>();

            services.AddTransient<IBot, Bot>();

            services.AddMediatR(typeof(GetServiceDeskRequestTypesQuery).Assembly);

            services.AddHttpClient();

            services.AddScoped<IServiceDeskClient, ServiceDeskClient>();
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