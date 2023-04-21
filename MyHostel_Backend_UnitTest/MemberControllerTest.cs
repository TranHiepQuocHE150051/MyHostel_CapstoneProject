using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyHostel_BackEnd.Controllers;
using MyHostel_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHostel_Backend_UnitTest
{
    public class MemberControllerTest
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private MemberController _Controller;

        public MemberControllerTest()
        {
            _context = new MyHostelContext();
            _Controller = new MemberController(_configuration, _context);
        }
        [Fact]
        public async void TestGetListChat()
        {
            var result1 = await _Controller.GetListChat(1008, 1, 10);//NF
            var result2 = await _Controller.GetListChat(1009, 1, 10);//NF
            var result3 = await _Controller.GetListChat(1010, 1, 10);//NF
            var result4 = await _Controller.GetListChat(2010, 1, 10);//NF
            var result5 = await _Controller.GetListChat(2011, 1, 10);//NF
            var result6 = await _Controller.GetListChat(2012, 1, 10);//NF
            var result7 = await _Controller.GetListChat(1007, 1, 10);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<OkObjectResult>(result7);

        }
    }
}
