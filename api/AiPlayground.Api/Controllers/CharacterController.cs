using AiPlayground.Api.Services;
using AiPlayground.Api.ViewModels;
using AiPlayground.Core.Models.Playground;
using Microsoft.AspNetCore.Mvc;

namespace AiPlayground.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CharacterController(
    ILogger<CharacterController> logger,
    CharacterService characterService
) : ControllerBase
{
    private readonly ILogger<CharacterController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly CharacterService _characterService = characterService ?? throw new ArgumentNullException(nameof(characterService));

    [HttpPost(Name = "CreateCharacter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CharacterViewModel>> CreateCharacter([FromBody] CreateCharacterInputModel model)
    {
        if (model == null)
        {
            return BadRequest("Input model cannot be null.");
        }

        var character = await _characterService.CreateCharacterAsync(model);
        return Ok(character);
    }
}
