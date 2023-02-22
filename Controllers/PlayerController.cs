using MongoDBDemo;
using MongoDB.Driver;
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
    readonly MongoCRUD _db = new(configuration, "player");

    [HttpPost("addPlayer")]
    public async Task<IActionResult> AddPlayerAsync([FromBody] Player playerData)
    {
        var isInserted = await _db.InsertRecord(playerData);
        if (!isInserted.Item1)
        {
            return BadRequest(isInserted.Item2);
        }
        return Ok($"Player '{playerData.name}' {isInserted.Item2}");
    }

    [HttpGet("allPlayers")]
    public async Task<IActionResult> GetPlayers()
    {
        var getStatus = await _db.LoadRecords<Player>();
        if (!getStatus.Item1)
        {
            return BadRequest(getStatus.Item2);
        }
        return Ok(getStatus.Item3);
    }

    [HttpGet("anyQuery")]
    public async Task<IActionResult> GetPlayersByQuery([FromQuery] int? id, [FromQuery] string? name, [FromQuery] bool? isHost)
    {
        #region FilterBuilder
        var filterBuilder = Builders<Player>.Filter;
        var filter = filterBuilder.Empty;
        var isEmptyQuery = true;
        if (id.HasValue)
        {
            filter &= filterBuilder.Eq(p => p.id, id.Value);
            isEmptyQuery = false;
        }
        if (!string.IsNullOrEmpty(name))
        {
            filter &= filterBuilder.Regex(p => p.name, name.ToString());
            isEmptyQuery = false;
        }
        if (isHost.HasValue)
        {
            filter &= filterBuilder.Eq(p => p.isHost, isHost.Value);
            isEmptyQuery = false;
        }
        #endregion

        if (!isEmptyQuery)
        {
            var getQueryStatus = await _db.LoadRecordsByQuery(filter);
            if (!getQueryStatus.Item1)
            {
                return BadRequest(getQueryStatus.Item2);
            }
            return Ok(getQueryStatus.Item3);
        }
        return BadRequest();
    }

    [HttpPut("updatePlayer")]
    public async Task<IActionResult> UpdatePlayer([FromBody] Player playerData)
    {
        var isUpdated = await _db.UpdateRecordById(playerData.id, playerData);
        if (!isUpdated.Item1)
        {
            return BadRequest(isUpdated.Item2);
        }
        return Ok($"Player '{playerData.name}' {isUpdated.Item2}");
    }

    [HttpDelete("deletePlayer")]
    public async Task<IActionResult> DeletePlayerByID([FromQuery] int? id)
    {
        if (id.HasValue) {
            var isDeleted = await _db.DeleteRecord<Player>(id);
            if (!isDeleted.Item1 || id == null)
            {
                return BadRequest(isDeleted.Item2);
            }
            return NoContent();
        }
        return BadRequest("Empty Query");
    }
}
