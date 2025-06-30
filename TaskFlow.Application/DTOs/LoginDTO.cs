using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

public class LoginDTO
{
    [Display(Name = "Username")]
    public string userName { get; set; } = null!;
    [Display(Name = "Password")]
    public string password { get; set; } = null!;
}
