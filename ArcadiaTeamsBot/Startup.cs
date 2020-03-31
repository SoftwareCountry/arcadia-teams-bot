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
    using Microsoft.Extensions.Configuration;

    public class Startup
    {
        private readonly ServiceDeskConfiguration serviceDeskConfiguration;

        public Startup(IConfiguration configuration)
        {
            this.serviceDeskConfiguration = configuration.GetSection("ServiceDesk").Get<ServiceDeskConfiguration>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMediatR(typeof(GetServiceDeskRequestTypesHandler).Assembly);

            services.AddHttpClient();

            services.AddScoped<IServiceDeskClient, ServiceDeskClient>();
            services.AddTransient<IBot, Bot>();
            services.AddSingleton<IBotFrameworkHttpAdapter, BotAdapterWithErrorHandling>();
            services.AddSingleton(this.serviceDeskConfiguration);
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