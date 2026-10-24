namespace login.api.dto;

public class SuccesfulStringDto
{
    public required string Message {get; set;}
}

public class SuccesfulUserListDto
{
    public required List<ReadUserDto> UserList {get; set;}
}

public class SuccesfulStringListDto
{
    public required List<string> Games {get; set;}
}

public class SuccesfulGameListDto
{
    public required List<ReadGameDto> GameList {get; set;}
}