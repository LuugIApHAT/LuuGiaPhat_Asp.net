using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Luugiaphat.Data;
using Luugiaphat.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Luugiaphat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Login loginUser)
        {
            if (loginUser == null || string.IsNullOrEmpty(loginUser.Username) || string.IsNullOrEmpty(loginUser.Password))
            {
                return BadRequest("Username or Password is missing");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username);
            if (user == null || user.Password != loginUser.Password)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        // POST: api/user/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] User registerUser)
        {
            if (registerUser == null || string.IsNullOrEmpty(registerUser.Username) || string.IsNullOrEmpty(registerUser.Password))
            {
                return BadRequest("Username or Password is missing");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerUser.Username);
            if (existingUser != null)
            {
                return Conflict("Username is already taken");
            }

            _context.Users.Add(registerUser);
            await _context.SaveChangesAsync();

            var token = GenerateToken(registerUser);
            return CreatedAtAction(nameof(Login), new { username = registerUser.Username }, new { token });
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Username, u.Email, u.Password })  // Trả về các trường cần thiết
                .ToListAsync();

            return Ok(users);
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy người dùng
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent(); // Trả về 204 nếu xóa thành công
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
