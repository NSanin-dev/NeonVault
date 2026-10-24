namespace login.api.data;

public class AppUser
{
    public int Id {get; set;}
    public string? ProfilePicture {get; set;}
    public required string UserName {get; set;}
    public required string PassWord {get; set;}
    public string Role {get; set;} = "Free";
    public List<Game> Games {get; set;} = new List<Game>();
}