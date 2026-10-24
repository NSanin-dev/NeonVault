using System.ComponentModel.DataAnnotations;

namespace login.web.Models;

public class GameModelAdmin
{
    [Required(ErrorMessage = "Game title is required")]
    public string GameTitle {get; set;} = "";
}