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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO account)
        {
            if (account != null && account.id != null)
            {
                if (account.socialType.ToLower().Equals("facebook"))
                {
                    var acc = await _context.Members.FirstOrDefaultAsync(x => x.FacebookId == account.id);
                    if (acc != null)
                    {
                        var claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id",  acc.Id.ToString()),
                        new Claim("FacebookId", acc.FacebookId.ToString()),
                        new Claim("Fname", acc.FirstName.ToString()),
                        new Claim("Lname", acc.LastName.ToString()),
                        new Claim(ClaimTypes.Role, acc.RoleId.ToString())

                    };
                        var accessToken = GenerateJSONWebToken(claims);
                        return Ok(accessToken);
                    }
                    else
                    {
                        Member member = new Member
                        {
                            FacebookId = account.id,
                            FirstName = account.FirstName,
                            LastName = account.LastName,
                            Avatar = account.AvatarURL,
                            CreatedAt = BitConverter.GetBytes(DateTime.Now.Ticks),
                            RoleId = account.role
                        };
                        await _context.Members.AddAsync(member);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            var claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id",  member.Id.ToString()),
                        new Claim("FacebookId", member.FacebookId.ToString()),
                        new Claim("Fname", member.FirstName.ToString()),
                        new Claim("Lname", member.LastName.ToString()),
                        new Claim(ClaimTypes.Role, member.RoleId.ToString())

                        };
                            var accessToken = GenerateJSONWebToken(claims);
                            return Ok(accessToken);
                        }
                        return StatusCode(500);
                    }
                }
                else if (account.socialType.ToLower().Equals("google"))
                {
                    var acc = await _context.Members.FirstOrDefaultAsync(x => x.GoogleId == account.id);
                    if (acc != null)
                    {
                        var claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.Name,  acc.Id.ToString()),
                        new Claim("GoogleId", acc.GoogleId.ToString()),
                        new Claim("Fname", acc.FirstName.ToString()),
                        new Claim("Lname", acc.LastName.ToString()),
                        new Claim(ClaimTypes.Role, acc.RoleId.ToString())

                    };
                        var accessToken = GenerateJSONWebToken(claims);
                        return Ok(accessToken);
                    }
                    else
                    {
                        Member member = new Member
                        {
                            GoogleId = account.id,
                            FirstName = account.FirstName,
                            LastName = account.LastName,
                            Avatar = account.AvatarURL,
                            CreatedAt = BitConverter.GetBytes(DateTime.Now.Ticks),
                            RoleId = account.role
                        };
                        await _context.Members.AddAsync(member);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            var claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id",  member.Id.ToString()),
                        new Claim("FacebookId", member.GoogleId.ToString()),
                        new Claim("Fname", member.FirstName.ToString()),
                        new Claim("Lname", member.LastName.ToString()),
                        new Claim(ClaimTypes.Role, member.RoleId.ToString())

                        };
                            var accessToken = GenerateJSONWebToken(claims);
                            return Ok(accessToken);
                        }
                        return StatusCode(500);
                    }
                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }


            else
            {
                return BadRequest("Login failed");
            }
        }

        [HttpGet("member")]
        public async Task<IActionResult> Member([FromQuery] int id)
        {
            try
            {
                var member = await _context.Members.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (member != null)
                    return Ok(member);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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
