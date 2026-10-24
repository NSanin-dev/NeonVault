using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using login.api.data;
using login.api.dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace login.api.services;

public interface IAuthService
{
    Task<(ReadUserDto? user, string? error)> AppRegisterAsync(CreateUserDto request);
    Task<(string? token, string? error)> AppLoginAsync(LoginUserDto request);
}

public class ImplementedAuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    public ImplementedAuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }


    public async Task<(ReadUserDto? user, string? error)> AppRegisterAsync(CreateUserDto request)
    {
        var existingUser = await _db.AppUsers.FirstOrDefaultAsync(u => u.UserName == request.UserName);

        if(existingUser != null)
        {
            return (null, "A user with this user name already exists");
        }

        var newUser = new AppUser
        {
            UserName = request.UserName,
            PassWord = BCrypt.Net.BCrypt.HashPassword(request.PassWord),
            ProfilePicture = request.ProfilePicture
        };

        _db.AppUsers.Add(newUser);
        await _db.SaveChangesAsync();

        var safeResponse = new ReadUserDto
        {
            Id = newUser.Id,
            UserName = newUser.UserName,
            Role = newUser.Role,
            ProfilePicture = newUser.ProfilePicture
        };

        return (safeResponse, null);
    }


    public async Task<(string? token, string? error)> AppLoginAsync(LoginUserDto request)
    {
        var existingUser = await _db.AppUsers.FirstOrDefaultAsync(u => u.UserName == request.UserName);

        if(existingUser == null)
        {
            return (null, "User or password does not match our system");
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(request.PassWord, existingUser.PassWord);

        if (!isValid)
        {
            return (null, "User or password does not match our system");
        }

        //Create token handler
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!); //Get key bytes

        //List of claims
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim(ClaimTypes.Name, existingUser.UserName),
            new Claim(ClaimTypes.Role, existingUser.Role)
        };

        //Add the profile picture if it exists, if not frontend will check for null to display default
        if (!string.IsNullOrEmpty(existingUser.ProfilePicture))
        {
            claims.Add(new Claim("ProfilePicture", existingUser.ProfilePicture));
        }

        //Create token descriptor with claim types and other paramaters
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        //Mint the token by passing tokendescriptor to tokenhandler
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token); //Convert it to a string

        return (tokenString, null);
    }
}