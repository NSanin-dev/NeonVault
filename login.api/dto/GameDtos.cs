namespace login.api.dto;

public class ReadGameDto
{
    public int Id {get; set;}
    public required string Title {get; set;}
    public string? CoverArt {get; set;}
    public List<string> UserNames {get; set;} = new List<string>();
}

public class CreateGameDto
{
    public required string Title {get; set;}
}
