using Arch.BaseApi;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsageApi.CQRS.Commnand;
using UsageApi.CQRS.Query;

namespace UsageApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsageController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsageController> _logger;

        public UsageController(ILogger<UsageController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetAllCachedUsageQuery() { Id=1});
            var result2 = await _mediator.Send(new GetAllCachedUsageQuery() { Id=2});
            return ApiOk(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUsageCommand command)
        {
            await _mediator.Send(command);
            return ApiOk();
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateUsageCommand command)
        {
            await _mediator.Send(command);
            return ApiOk();
        }
    }
}
