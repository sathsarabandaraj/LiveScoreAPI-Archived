using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LiveScoreAPI.Models;

public class Player
{
    [BsonId]
    public int id { get; set; }
    [Required]
    public bool isHost { get; set; }
    [Required]
    public string? name { get; set; }
    public int age { get; set; }
    public string? position { get; set; }
    public int runs { get; set; }
    public int wickets { get; set; }

    public Player()
    {
        age = 0;
        position = "";
        runs = 0;
        wickets = 0;
    }
}