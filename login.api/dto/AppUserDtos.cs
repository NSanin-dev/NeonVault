namespace login.api.dto;

public class ReadUserDto
{
    public int Id {get; set;}
    public string? ProfilePicture {get; set;}
    public required string UserName {get; set;}
    public required string Role {get; set;}
    public List<string> Games {get; set;} = new List<string>();
    public List<string?> GamesCoverArt {get; set;} = new List<string?>();
}


public class CreateUserDto
{
    public string? ProfilePicture {get; set;}
    public required string UserName {get; set;}
    public required string PassWord {get; set;}
}

public class LoginUserDto
{ 
    public required string UserName {get; set;}
    public required string PassWord {get; set;}
}