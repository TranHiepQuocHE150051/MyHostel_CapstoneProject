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
                        return Ok(new LoginResponse
                        {
                            Token = accessToken,
                            MemberId = acc.Id
                        });
                    }
                    else
                    {
                        Member member = new Member
                        {
                            FacebookId = account.id,
                            FirstName = account.FirstName,
                            LastName = account.LastName,
                            Avatar = account.AvatarURL,
                            CreatedAt = DateTime.Now,
                            RoleId = account.role,
                            FcmToken = "",
                            InviteCode = GenerateInviteCode()
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
                            return Ok(new LoginResponse
                            {
                                Token = accessToken,
                                MemberId = member.Id
                            });
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
                        new Claim("Id",  acc.Id.ToString()),
                        new Claim("GoogleId", acc.GoogleId.ToString()),
                        new Claim("Fname", acc.FirstName.ToString()),
                        new Claim("Lname", acc.LastName.ToString()),
                        new Claim(ClaimTypes.Role, acc.RoleId.ToString())

                    };
                        var accessToken = GenerateJSONWebToken(claims);
                        return Ok(new LoginResponse
                        {
                            Token = accessToken,
                            MemberId = acc.Id
                        });
                    }
                    else
                    {
                        Member member = new Member
                        {
                            GoogleId = account.id,
                            FirstName = account.FirstName,
                            LastName = account.LastName,
                            Avatar = account.AvatarURL,
                            CreatedAt = DateTime.Now,
                            RoleId = account.role,
                            FcmToken = "",
                            InviteCode = GenerateInviteCode()
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
                        new Claim("GoogleId", member.GoogleId.ToString()),
                        new Claim("Fname", member.FirstName.ToString()),
                        new Claim("Lname", member.LastName.ToString()),
                        new Claim(ClaimTypes.Role, member.RoleId.ToString())

                        };
                            var accessToken = GenerateJSONWebToken(claims);
                            return Ok(new LoginResponse
                            {
                                Token = accessToken,
                                MemberId = member.Id
                            });
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

        [HttpPost("logout/{memberId}")]
        public async Task<IActionResult> Logout(int memberId)
        {
            try
            {

                var member = await _context.Members.Where(m => m.Id == memberId).FirstOrDefaultAsync();
                member.FcmToken = "";
                _context.Members.Update(member);
                _context.SaveChanges();
                return Ok(new {message= "Logout"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    messsage= ex.Message
                });
            }

        }


        [HttpGet("member")]
        public async Task<IActionResult> Member([FromQuery] int id,
                                                 [FromQuery] string inviteCode)
        {
            try
            {
                if (id != 0)
                {
                    var member = await _context.Members.Where(m => m.Id == id).FirstOrDefaultAsync();
                    if (member != null)
                        return Ok(new
                        {
                            MemberId = member.Id,
                            Name = member.FirstName + " " + member.LastName,
                            AvatarUrl = member.Avatar,
                            InviteCode = member.InviteCode,
                            OauthSocial = member.GoogleId != null ? "google" : "facebook"
                        });
                }
                else
                {
                    var member = await _context.Members.Where(m => m.InviteCode.Trim().Equals(inviteCode)).FirstOrDefaultAsync();
                    if (member != null)
                        return Ok(new
                        {
                            MemberId = member.Id,
                            Name = member.FirstName + " " + member.LastName,
                            AvatarUrl = member.Avatar,
                            InviteCode = member.InviteCode,
                            OauthSocial = member.GoogleId != null ? "google" : "facebook"
                        });


                }
                return NotFound();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("fcm-token")]
        public async Task<IActionResult> UpdateFCMToken([FromBody] UpdateFcmTokenDTO fcmTokenDTO)
        {
            try
            {
                var member = _context.Members.FirstOrDefault(m => m.Id == fcmTokenDTO.MemberId);
                if (member != null)
                {
                    member.FcmToken = fcmTokenDTO.Token;
                    _context.Members.Update(member);
                    await _context.SaveChangesAsync();
                    return Ok("Success");
                }
                return BadRequest("Member not exist");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        private string GenerateInviteCode()
        {
            Guid g = Guid.NewGuid();
            string code = g.ToString();
            string[] codes = code.Split("-");
            return codes[0] + "-" + codes[1];
        }

        private string GenerateJSONWebToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(100),
                    signingCredentials: signIn
                    );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
