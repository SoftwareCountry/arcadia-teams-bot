namespace ArcadiaTeamsBot.Controllers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ArcadiaTeamsBot.CQRS.Abstractions;
    using ArcadiaTeamsBot.ServiceDesk.Requests;

    using MediatR;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly Dictionary<string, string> serviceDeskMappingConfiguration;

        public TestController(IMediator mediator, Dictionary<string, string> serviceDeskMappingConfiguration)
        {
            this.mediator = mediator;
            this.serviceDeskMappingConfiguration = serviceDeskMappingConfiguration;
        }

        [HttpGet]
        [Route("GetRequestTypes")]
        public async Task<IActionResult> GetRequestTypes(CancellationToken cancellationToken)
        {
            var query = new GetServiceDeskRequestTypesQuery();

            var requestTypes = await this.mediator.Send(query, cancellationToken);

            return this.Ok(requestTypes);
        }

        [HttpGet]
        [Route("GetCurrentRequest")]
        public async Task<IActionResult> GetCurrentRequest(CancellationToken cancellationToken)
        {
            const string username = "vyacheslav.lasukov@arcadia.spb.ru";

            var query = new GetCurrentServiceDeskRequestsQuery(username);

            var currentRequest = await this.mediator.Send(query, cancellationToken);

            return this.Ok(currentRequest);
        }

        [HttpGet]
        [Route("GetRequestPriorities")]
        public async Task<IActionResult> GetRequestPriorities(CancellationToken cancellationToken)
        {
            var query = new GetServiceDeskRequestPrioritiesQuery();

            var requestPriorities = await this.mediator.Send(query, cancellationToken);

            return this.Ok(requestPriorities);
        }

        [HttpGet]
        [Route("GetFields")]
        public async Task<IActionResult> GetFields(CancellationToken cancellationToken)
        {
            var requestTypesForUI = new Dictionary<string, RequestTypeForUI>();

            var query = new GetServiceDeskRequestTypesQuery();

            var requestTypes = await this.mediator.Send(query, cancellationToken);

            foreach (var requestType in requestTypes)
            {
                if (requestType.RequestTypeFields.Length != 0)
                {
                    var requestTypeForUI = new RequestTypeForUI(this.serviceDeskMappingConfiguration);
                    requestTypeForUI.LoadFieldsTypes(requestType);
                    requestTypesForUI.Add(requestType.Title, requestTypeForUI);
                }
            }

            return this.Ok(requestTypesForUI);
        }
    }
}
