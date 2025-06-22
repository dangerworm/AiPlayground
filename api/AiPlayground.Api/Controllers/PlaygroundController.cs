using AiPlayground.Api.Models.Playground;
using AiPlayground.Api.Services;
using AiPlayground.Core.DataTransferObjects;
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
    public ActionResult<CharacterDto> CreateCharacter([FromBody] CreateCharacterInputModel model)
    {
        if (model == null)
        {
            return BadRequest("Input model cannot be null.");
        }

        var response = _playgroundService.CreateCharacter(model);
        return Ok(response);
    }

    [HttpGet(Name = "GetPlaygroundSetup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<PlaygroundSetupResponseModel> GetPlaygroundSetup()
    {
        var model = _playgroundService.GetSetup();
        return Ok(model);
    }
}
