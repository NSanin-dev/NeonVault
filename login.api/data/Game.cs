namespace login.api.data;

public class Game
{
    public int Id {get; set;}
    public required string Title {get; set;}
    public string? CoverArt {get; set;}
    public List<AppUser> Users {get; set;} = new List<AppUser>();
}