using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, IPositionRepository positionRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _positionRepository = positionRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        if (model == null || (string.IsNullOrEmpty(model.NumberPhone)) || string.IsNullOrEmpty(model.Password))
        {
            return BadRequest("Phải cung cấp số điện thoại hoặc email và mật khẩu.");
        }

        // Tìm người dùng bằng số điện thoại hoặc email
        User? user = null;
        if (!string.IsNullOrEmpty(model.NumberPhone))
        {
            user = await _userRepository.FindByNumberPhoneAsync(model.NumberPhone);
        }
        // else if (!string.IsNullOrEmpty(model.Mail))
        // {
        //     user = await _userRepository.FindByMailAsync(model.Mail);
        // }

        if (user == null || user.Password != model.Password)
        {
            return Unauthorized("Số điện thoại/email hoặc mật khẩu không chính xác.");
        }

        // Tạo JWT token
        var token = await GenerateJwtToken(user);
        return Ok(new { token });
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim("numberPhone", user.NumberPhone ?? string.Empty),
            // new Claim("mail", user.Mail ?? string.Empty),
            new Claim("role", user.Position ?? string.Empty), // Gán role dựa trên Position
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
