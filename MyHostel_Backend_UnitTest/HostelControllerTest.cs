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
    public class HostelControllerTest
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private HostelController _Controller;

        public HostelControllerTest()
        {
            _context = new MyHostelContext();
            _Controller = new HostelController(_configuration, _context);
        }
        [Fact]
        public async void TestSearchHostel ()
        {
            var result1 = await _Controller.SearchHostel(null, null, null, null, null, 0, 0);
            var result2 = await _Controller.SearchHostel("001", null, null, null, null, 0, 0);
            var result3 = await _Controller.SearchHostel("001", "100k-3m", null, null, null, 0, 0);
            var result4 = await _Controller.SearchHostel("001", "100k-3m", "19 20", null, null, 0, 0);
            var result5 = await _Controller.SearchHostel("001", "100k-3m", "19 20", "1 2", null, 0, 0);
            var result6 = await _Controller.SearchHostel("001", "100k-3m", "19 20", "1 2", 2, 0, 0);
            
            Assert.IsType<NotFoundResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
        }
        [Fact]
        public async void TestSearchNearbyHostel()
        {
            var result1 = await _Controller.SearchNearbyHostel("01",null,null,0,0);
            var result2 = await _Controller.SearchNearbyHostel("01","123",null,0,0);
            var result3 = await _Controller.SearchNearbyHostel("01","123","123",0,0);
            var result4 = await _Controller.SearchNearbyHostel("01","123","123",999,0);

            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<BadRequestObjectResult>(result3);
            Assert.IsType<BadRequestObjectResult>(result4);

        }
        [Fact]
        public async void TestGetHostelById()
        {
            var result1 = await _Controller.GetHostel(1,0);//NF
            var result2 = await _Controller.GetHostel(11,0);// OK
            var result3 = await _Controller.GetHostel(0,0); // NF
            var result4 = await _Controller.GetHostel(11,1);// OK



            Assert.IsType<NotFoundResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<NotFoundResult>(result3);
            Assert.IsType<OkObjectResult>(result4);

        }
        [Fact]
        public async void TestGetHostelForLandlord()
        {
            var result1 = await _Controller.GetHostelForLandlord(1);//NF
            var result2 = await _Controller.GetHostelForLandlord(2);//NF
            var result3 = await _Controller.GetHostelForLandlord(3);//NF
            var result4 = await _Controller.GetHostelForLandlord(0);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);

        }
        [Fact]
        public async void TestGetHostelReview()
        {
            var result1 = await _Controller.GetHostelReviews(1);//NF
            var result2 = await _Controller.GetHostelReviews(2);//NF
            var result3 = await _Controller.GetHostelReviews(3);//NF
            var result4 = await _Controller.GetHostelReviews(0);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);

        }
        [Fact]
        public async void TestGetHostelRoom()
        {
            var result1 = await _Controller.GetHostelRooms(1);//NF
            var result2 = await _Controller.GetHostelRooms(2);//NF
            var result3 = await _Controller.GetHostelRooms(3);//NF
            var result4 = await _Controller.GetHostelRooms(0);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);

        }
        [Fact]
        public void TestConvertString()
        {
            string result1 = replaceString(100000);
            string result2 = replaceString(1000000);
            string result3 = replaceString(20000);
            string result4 = replaceString(20000000);
            Assert.Equal("100K", result1);
            Assert.Equal("1M", result2);
            Assert.Equal("20K", result3);
            Assert.Equal("20M", result4);
        }
        private string replaceString(decimal price)
        {
            double result = (double)price;
            string result1 = "";
            if (result >= 1000 && result < 1000000)
            {
                result = result / 1000;
                result1 = result.ToString() + "K";
            }
            else if (price >= 1000000)
            {
                result = result / 1000000;
                result1 = result.ToString() + "M";
            }
            return result1;
        }
    }
}
