using AiPlayground.Api.Models.Interactions;
using AiPlayground.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiPlayground.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class InteractionController(
    ILogger<InteractionController> logger,
    InteractionService interactionService
) : ControllerBase
{
    private readonly ILogger<InteractionController> _logger = logger;
    private readonly InteractionService _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));

    [HttpPost(Name = "Interact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<InteractResponseModel>> Interact([FromBody] InteractInputModel model)
    public async Task<ActionResult<string>> Interact([FromBody] InteractInputModel model)
    {
        var response = await _interactionService.ContactLlm(model);
        if (string.IsNullOrEmpty(response))
        {
            _logger.LogError("No response received from LLM for character ID '{CharacterId}'", model.CharacterId);
            return NotFound($"No response received for character ID '{model.CharacterId}'.");
        }

        _logger.LogInformation("Received response from LLM for character ID '{CharacterId}': {Response}", model.CharacterId, response);

        return Ok(response);
    }
}
