namespace ArcadiaTeamsBot
{
    using ArcadiaTeamsBot.CQRS.Handlers;
    using ArcadiaTeamsBot.Dialogs;
    using ArcadiaTeamsBot.Infrastructure;
    using ArcadiaTeamsBot.ServiceDesk;
    using ArcadiaTeamsBot.ServiceDesk.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Requests;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory;

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
        private readonly RequestTypesMappingConfiguration requestTypesMappingConfiguration;

        public Startup(IConfiguration configuration)
        {
            this.serviceDeskConfiguration = configuration.GetSection("ServiceDesk").Get<ServiceDeskConfiguration>();
            this.requestTypesMappingConfiguration = configuration.GetSection("ServiceDeskRequestsMapping").Get<RequestTypesMappingConfiguration>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMediatR(typeof(GetServiceDeskRequestTypesHandler).Assembly);

            services.AddHttpClient();

            services.AddScoped<IServiceDeskClient, ServiceDeskClient>();
            services.AddSingleton<IBotFrameworkHttpAdapter, BotAdapterWithErrorHandling>();
            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<ConversationState>();
            services.AddSingleton<MainDialog>();
            services.AddTransient<IBot, Bot<MainDialog>>();
            services.AddSingleton(this.serviceDeskConfiguration);
            services.AddSingleton(this.requestTypesMappingConfiguration);
            services.AddSingleton<IRequestTypeForUIFactory, RequestTypeForUIFactory>();
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
