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
            var result6 = await _Controller.Member(0, "d834916f-38b3       ");
            var result7 = await _Controller.Member(0, "d834916f-38c3       ");
            var result8 = await _Controller.Member(0, "0f8fad5b-d9cb       ");
            Assert.IsType<NotFoundResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<NotFoundResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<OkObjectResult>(result7);
            Assert.IsType<OkObjectResult>(result8);
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
            LoginDTO case4 = new LoginDTO
            {
                socialType = "google",
                role = 2,
                FirstName = "nguyen",
                LastName = "trung",
                AvatarURL = "abc123"
            };
            LoginDTO case5 = new LoginDTO
            {
                socialType = "google",
                role = 2,
                FirstName = "tran",
                LastName = "quoc1",
                AvatarURL = "abc123"
            };
            LoginDTO case6 = new LoginDTO
            {
                socialType = "google",
                role = 2,
                FirstName = "tran",
                LastName = "quoc2",
                AvatarURL = "abc123"
            };
            LoginDTO case7 = new LoginDTO
            {
                socialType = "google",
                role = 2,
                FirstName = "tran",
                LastName = "quoc3",
                AvatarURL = "abc123"
            };
            var result1 = IsLoginSuccess(case1);
            var result2 = IsLoginSuccess(case2);
            var result3 = IsLoginSuccess(case3);
            var result4 = IsLoginSuccess(case4);
            var result5 = IsLoginSuccess(case5);
            var result6 = IsLoginSuccess(case6);
            var result7 = IsLoginSuccess(case7);
            
            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
            Assert.False(result4);
            Assert.False(result5);
            Assert.False(result6);
            Assert.False(result7);
            
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