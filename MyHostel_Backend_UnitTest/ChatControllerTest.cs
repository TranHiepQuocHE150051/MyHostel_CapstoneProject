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
    public class ChatControllerTest
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private ChatController _Controller;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatControllerTest()
        {
            _context = new MyHostelContext();
            _Controller = new ChatController(_configuration, _context, _hubContext);
        }
        [Fact]
        public async void TestGetChatInfo()
        {
            var result1 = await _Controller.GetChatInfo(2);//NF
            var result2 = await _Controller.GetChatInfo(3);//NF
            var result3 = await _Controller.GetChatInfo(4);//NF
            var result4 = await _Controller.GetChatInfo(5);//NF
            var result5 = await _Controller.GetChatInfo(6);//NF
            var result6 = await _Controller.GetChatInfo(7);//NF
            var result7 = await _Controller.GetChatInfo(8);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<OkObjectResult>(result7);

        }
        [Fact]
        public async void TestGetMessage()
        {
            var result1 = await _Controller.GetMessages(2,1,10);//NF
            var result2 = await _Controller.GetMessages(3,1,10);//NF
            var result3 = await _Controller.GetMessages(4,1,10);//NF
            var result4 = await _Controller.GetMessages(5,1,10);//NF
            var result5 = await _Controller.GetMessages(6,1,10);//NF
            var result6 = await _Controller.GetMessages(7,1,10);//NF
            var result7 = await _Controller.GetMessages(8,1,10);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<BadRequestObjectResult>(result6);
            Assert.IsType<BadRequestObjectResult>(result7);

        }
        [Fact]
        public async void TestSearchChat()
        {
            var result1 = await _Controller.SearchChat(1008, 1009);//NF
            var result2 = await _Controller.SearchChat(1008, 1010);//NF
            var result3 = await _Controller.SearchChat(1009, 1010);
            var result4 = await _Controller.SearchChat(1009, 2010);//NF
            var result5 = await _Controller.SearchChat(2010, 2012);//NF
            var result6 = await _Controller.SearchChat(2011, 2012);//NF
            var result7 = await _Controller.SearchChat(2012, 1008);//NF


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
