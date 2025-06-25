using AiPlayground.Api.Services;
using AiPlayground.Api.ViewModels;
using AiPlayground.Core.Models.Interactions;
using Microsoft.AspNetCore.Mvc;

namespace AiPlayground.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class InteractionController(
    ILogger<InteractionController> logger,
    InteractionService interactionService,
    PlaygroundService playgroundService
) : ControllerBase
{
    private readonly ILogger<InteractionController> _logger = logger;
    private readonly InteractionService _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
    private readonly PlaygroundService _playgroundService = playgroundService ?? throw new ArgumentNullException(nameof(playgroundService));

    [HttpPost(Name = "Interact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CharacterViewModel>> Interact([FromBody] InteractInputModel model)
    {
        var character = await _interactionService.ProcessInteraction(model);
        await _playgroundService.UpdatePlaygroundIterationsAsync();

        return Ok(character);
    }
}
