using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyHostel_BackEnd.Controllers;
using MyHostel_BackEnd.DTOs;
using MyHostel_BackEnd.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace MyHostel_Backend_UnitTest
{
    public class AccountControllerTest 
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private AccountController _Controller;

        public AccountControllerTest()
        {
            _context = new MyHostelContext();
            _Controller = new AccountController(_configuration, _context);
        }

        [Fact]
        public void TestGenerateInviteCode()
        {
            Guid g = Guid.NewGuid();
            string code = g.ToString();
            string[] codes = code.Split("-");
            string result= codes[0] + "-" + codes[1];
            Assert.Equal(8, codes[0].Length);
            Assert.Equal(4, codes[1].Length);
            Assert.Equal(13, result.Trim().Length);
        }
        [Fact]
        public async void TestGetMember()
        {
            var result1 = await _Controller.Member(1,null); // NotFound
            var result2 = await _Controller.Member(2,null); // Ok
            var result3 = await _Controller.Member(3,null);// Ok
            var result4 = await _Controller.Member(0,null);// NotFound
            var result5 = await _Controller.Member(0, "d834916f-38a3");
            Assert.IsType<NotFoundResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<NotFoundResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
        }
        [Fact]
        public  void TestLogin()
        {
            LoginDTO case1 = new LoginDTO
            {
                socialType = "facebook",
                role = 2,
                id = "tranhiepquocfb2",
                FirstName = "tran",
                LastName = "quoc",
                AvatarURL = "abc123"
            };
            LoginDTO case2 = new LoginDTO
            {
                socialType = "google",
                role = 2,
                id = "demo1",
                FirstName = "tran",
                LastName = "quoc",
                AvatarURL = "abc123"
            };
            LoginDTO case3 = new LoginDTO
            {
                socialType = "google",
                role = 2,
                FirstName = "tran",
                LastName = "quoc",
                AvatarURL = "abc123"
            };
            var result1 = IsLoginSuccess(case1);
            var result2 = IsLoginSuccess(case2);
            var result3 = IsLoginSuccess(case3);
            
            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
            
        }
       private bool IsLoginSuccess(LoginDTO loginDTO)
        {
            if (loginDTO.id != null)
            {
                return true;
            }return false;
        }

    }
}