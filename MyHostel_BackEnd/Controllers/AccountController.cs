using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyHostel_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;

        public AccountController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("loginfacebook")]
        public async Task<IActionResult> Login([FromForm] FacebookLoginDTO account)
        {
            if (account != null && account.FacebookId != null)
            {
                var acc = await _context.Members.FirstOrDefaultAsync(x => x.FacebookId == account.FacebookId);
                if (acc != null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.Name,  acc.FacebookId.ToString()),
                        new Claim("FacebookId", acc.FacebookId.ToString()),
                        new Claim("Fname", acc.FirstName.ToString()),
                        new Claim("Lname", acc.LastName.ToString())
                       
                    };
                    var accessToken = GenerateJSONWebToken(claims);
                    return Ok(accessToken);
                }
                else
                {
                    Member member = new Member
                    {
                        FacebookId = account.FacebookId,
                        FirstName = account.FirstName,
                        LastName = account.LastName,
                        Avatar = account.Avatar,
                        CreatedAt = BitConverter.GetBytes(DateTime.Now.Ticks),
                        RoleId = 2
                    };
                    await _context.Members.AddAsync(member);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        return Ok("Register success");
                    }
                    return StatusCode(500);
                }
            }
            else
            {
                return BadRequest("Login failed");
            }
        }
        [HttpPost("logingoogle")]
        public async Task<IActionResult> LoginGoogle([FromForm] GoogleLoginDTO account)
        {
            if (account != null && account.GoogleId != null)
            {
                var acc = await _context.Members.FirstOrDefaultAsync(x => x.FacebookId == account.GoogleId);
                if (acc != null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.Name,  acc.GoogleId.ToString()),
                        new Claim("GoogleId", acc.GoogleId.ToString()),
                        new Claim("Fname", acc.FirstName.ToString()),
                        new Claim("Lname", acc.LastName.ToString()),
                        new Claim(ClaimTypes.Role, acc.Role.ToString())
                    };
                    var accessToken = GenerateJSONWebToken(claims);
                    return Ok(accessToken);
                }
                else
                {
                    Member member = new Member
                    {
                        FacebookId = account.GoogleId,
                        FirstName = account.FirstName,
                        LastName = account.LastName,
                        Avatar = account.Avatar,
                        CreatedAt = BitConverter.GetBytes(DateTime.Now.Ticks),
                        RoleId = 2
                    };
                    await _context.Members.AddAsync(member);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        return Ok("Register success");
                    }
                    
                        return StatusCode(500);
                    
                }
            }
            else
            {
                return BadRequest("Login failed");
            }
        }
        private string GenerateJSONWebToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddSeconds(3600),
                    signingCredentials: signIn
                    );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    

}
