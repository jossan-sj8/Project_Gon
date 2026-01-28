namespace Project_Gon.Core.DTOs.Auth;

public class LoginDto
{
    public string EmailOrRut { get; set; } = string.Empty;  // Email O RUT
    public string Password { get; set; } = string.Empty;
}