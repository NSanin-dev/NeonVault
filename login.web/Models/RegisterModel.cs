using System.ComponentModel.DataAnnotations;

namespace login.web.Models;

public class RegisterModel
{
    public string? ProfilePicture {get; set;} = null;

    [Required(ErrorMessage = "Username is required")]
    public string UserName {get; set;} = "";

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string PassWord { get; set; } = "";

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(PassWord), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";
}