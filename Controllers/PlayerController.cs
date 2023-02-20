using MongoDBDemo;
using LiveScoreAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LiveScoreAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class playerController : ControllerBase
{
    private static readonly IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .Build();
    readonly MongoCRUD _db = new MongoCRUD(configuration, "player");

    [HttpPost("addPlayer")]
    public async Task<IActionResult> AddPlayerAsync([FromBody] Player playerData)
    {
        var isInserted = await _db.InsertRecord(playerData);
        if (isInserted.Item1)
        {
            return Ok($"Player '{playerData.name}' {isInserted.Item2}");
        }
        else
        {
            return BadRequest(isInserted.Item2);
        }
    }

    [HttpGet("allPlayers")]
    public async Task<IActionResult> GetPlayers()
    {
        var getStatus = await _db.LoadRecords<Player>();
        if (getStatus.Item1)
        {
            return Ok(getStatus.Item3);
        }
        else
        {
            return BadRequest(getStatus.Item2);
        }
    }
}
