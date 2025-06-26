using AiPlayground.Api.Services;
using AiPlayground.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AiPlayground.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PlaygroundController(
    ILogger<PlaygroundController> logger,
    PlaygroundService playgroundService
) : ControllerBase
{
    private readonly ILogger<PlaygroundController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly PlaygroundService _playgroundService = playgroundService ?? throw new ArgumentNullException(nameof(playgroundService));

    [HttpGet(Name = "GetPlaygroundSetup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PlaygroundViewModel>> GetPlaygroundSetup()
    {
        var model = await _playgroundService.GetSetupAsync();
        return Ok(model);
    }


    [HttpPost(Name = "Iterate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PlaygroundViewModel>> Iterate()
    {
        var updatedState = await _playgroundService.Iterate();

        return Ok(updatedState);
    }
}
