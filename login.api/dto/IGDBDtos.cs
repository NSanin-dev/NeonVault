namespace login.api.dto;

public class TwitchTokenResponse
{
    public string access_token { get; set; } = string.Empty;
    public int expires_in { get; set; }
}

public class IgdbGameResponse
{
    public string name { get; set; } = string.Empty;
    public IgdbCover? cover { get; set; }
}

public class IgdbCover
{
    public string url { get; set; } = string.Empty;
}