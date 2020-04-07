namespace ArcadiaTeamsBot
{
    using System.Collections.Generic;

    using ArcadiaTeamsBot.CQRS.Handlers;
    using ArcadiaTeamsBot.Infrastructure;
    using ArcadiaTeamsBot.ServiceDesk;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;

    using MediatR;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly ServiceDeskConfiguration serviceDeskConfiguration;
        private readonly Dictionary<string, string> serviceDeskMappingConfiguration;

        public Startup(IConfiguration configuration)
        {
            this.serviceDeskConfiguration = configuration.GetSection("ServiceDesk").Get<ServiceDeskConfiguration>();
            this.serviceDeskMappingConfiguration = configuration.GetSection("ServiceDeskRequestsMapping").Get<Dictionary<string, string>>();
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
            services.AddSingleton(this.serviceDeskMappingConfiguration);
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
