using AiPlayground.Api.Services;
using AiPlayground.Api.ViewModels;
using AiPlayground.Core.DataTransferObjects;
using AiPlayground.Core.Models.Playground;
using Microsoft.AspNetCore.Mvc;

namespace AiPlayground.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PlaygroundController(
    ILogger<InteractionController> logger,
    PlaygroundService playgroundService
) : ControllerBase
{
    private readonly ILogger<InteractionController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly PlaygroundService _playgroundService = playgroundService ?? throw new ArgumentNullException(nameof(playgroundService));

    [HttpPost(Name = "CreateCharacter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CharacterViewModel>> CreateCharacter([FromBody] CreateCharacterInputModel model)
    {
        if (model == null)
        {
            return BadRequest("Input model cannot be null.");
        }

        var character = await _playgroundService.CreateCharacterAsync(model);
        return Ok(character);
    }

    [HttpGet(Name = "GetPlaygroundSetup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PlaygroundSetupResponseViewModel>> GetPlaygroundSetup()
    {
        var model = await _playgroundService.GetSetupAsync();
        return Ok(model);
    }
}
