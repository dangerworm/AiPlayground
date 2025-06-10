using AiPlayground.Api.Models.Interactions;
using AiPlayground.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiPlayground.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class InteractionController : ControllerBase
    {
        private readonly ILogger<InteractionController> _logger;

        private readonly PromptService _promptService;

        public InteractionController(
            ILogger<InteractionController> logger,
            PromptService promptService
        )
        {
            _logger = logger;
            _promptService = promptService ?? throw new ArgumentNullException(nameof(promptService));
        }

        [HttpGet(Name = "GetSystemPrompt")]
        public ActionResult<string> GetSystemPrompt()
        {
            var systemPrompt = _promptService.GetSystemPrompt();
            return Ok(systemPrompt);
        }

        [HttpGet(Name = "Interact")]
        public ActionResult<InteractResponseModel> Interact([FromBody] InteractInputModel model)
        {
            throw new NotImplementedException();  
        }
    }
}
