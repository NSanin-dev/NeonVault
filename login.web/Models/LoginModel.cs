using System.ComponentModel.DataAnnotations;

namespace login.web.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName {get; set;} = "";
    [Required (ErrorMessage = "Password is required")]
    public string PassWord {get; set;} = "";
}