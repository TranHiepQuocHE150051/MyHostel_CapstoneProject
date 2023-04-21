using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MyHostel_BackEnd.ChatHubController;
using MyHostel_BackEnd.Controllers;
using MyHostel_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHostel_Backend_UnitTest
{
    public class RoomControllerTest
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private RoomController _Controller;

        public RoomControllerTest()
        {
            _context = new MyHostelContext();
            _Controller = new RoomController(_configuration, _context);
        }
        [Fact]
        public async void TestGetTransactionForRoom()
        {
            var result1 = await _Controller.GetTransactionForRoom(61, 1, 2023);//NF
            var result2 = await _Controller.GetTransactionForRoom(62, 1, 2023);//NF
            var result3 = await _Controller.GetTransactionForRoom(63, 1, 2023);//NF
            var result4 = await _Controller.GetTransactionForRoom(64, 1, 2023);//NF
            var result5 = await _Controller.GetTransactionForRoom(65, 1, 2023);//NF
            var result6 = await _Controller.GetTransactionForRoom(73, 1, 2023);//NF
            var result7 = await _Controller.GetTransactionForRoom(75, 1, 2023);//NF


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
