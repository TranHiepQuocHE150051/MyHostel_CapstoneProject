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
    public class TenantControllerTest
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private TenantController _Controller;

        public TenantControllerTest()
        {
            _context = new MyHostelContext();
            _Controller = new TenantController(_configuration, _context);
        }
        [Fact]
        public async void TestGetRoomInfo()
        {
            var result1 = await _Controller.GetRoomInfo( 1010);//NF
            var result2 = await _Controller.GetRoomInfo( 1009);//NF
            var result3 = await _Controller.GetRoomInfo( 1008);//NF
            var result4 = await _Controller.GetRoomInfo(2010);//NF
            var result5 = await _Controller.GetRoomInfo(2011);//NF
            var result6 = await _Controller.GetRoomInfo( 2012);//NF
            var result7 = await _Controller.GetRoomInfo( 2);//NF
            var result8 = await _Controller.GetRoomInfo( 3);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<BadRequestObjectResult>(result7);
            Assert.IsType<BadRequestObjectResult>(result8);

        }
        [Fact]
        public async void TestGetBill()
        {
            var result1 = await _Controller.GetUnpaidBill(1010,"a");//NF
            var result2 = await _Controller.GetUnpaidBill(1009, "a");//NF
            var result3 = await _Controller.GetUnpaidBill(1008, "a");//NF
            var result4 = await _Controller.GetUnpaidBill(2010, "a");//NF
            var result5 = await _Controller.GetUnpaidBill(2011, "a");//NF
            var result6 = await _Controller.GetUnpaidBill(2012, "a");//NF
            var result7 = await _Controller.GetUnpaidBill(2, "a");//NF
            var result8 = await _Controller.GetUnpaidBill(3, "a");//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<BadRequestObjectResult>(result7);
            Assert.IsType<BadRequestObjectResult>(result8);

        }
    }
}
