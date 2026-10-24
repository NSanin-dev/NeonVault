namespace login.web.Models;

public class ProfileModel
{
    public string? ProfilePicture {get; set;}
    public string UserName {get; set;} = "";
    public string Role {get; set;} = "";
    public List<string> Games {get; set;} = new List<string>();
    public List<string?> GamesCoverArt {get; set;} = new List<string?>();
}