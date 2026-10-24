namespace login.web.Models;

public class GameModel
{
    public string? Title {get; set;}
    public string? CoverArt {get; set;}
    public List<string> UserNames {get; set;} = new List<string>();
}