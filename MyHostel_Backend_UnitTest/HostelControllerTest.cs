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
        public async void TestGetHostelReviewbyMember()
        {
            var result1 = await _Controller.GetHostelReview(11,1010);//NF
            var result2 = await _Controller.GetHostelReview(11,1009);//NF
            var result3 = await _Controller.GetHostelReview(11,1008);//NF
            var result4 = await _Controller.GetHostelReview(15,2010);//NF
            var result5 = await _Controller.GetHostelReview(15,2011);//NF
            var result6 = await _Controller.GetHostelReview(15,2012);//NF
            var result7 = await _Controller.GetHostelReview(11,2010);//NF
            var result8 = await _Controller.GetHostelReview(11,2011);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<StatusCodeResult>(result7);
            Assert.IsType<StatusCodeResult>(result8);

        }
        [Fact]
        public async void TestGetOtherCost()
        {
            var result1 = await _Controller.GetOtherCost(11);//NF
            var result2 = await _Controller.GetOtherCost(12);//NF
            var result3 = await _Controller.GetOtherCost(15);//NF
            var result4 = await _Controller.GetOtherCost(116);//NF
            var result5 = await _Controller.GetOtherCost(117);//NF
            var result6 = await _Controller.GetOtherCost(118);//NF
            var result7 = await _Controller.GetOtherCost(119);//NF
            var result8 = await _Controller.GetOtherCost(16);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<OkObjectResult>(result7);
            Assert.IsType<BadRequestObjectResult>(result8);

        }
        [Fact]
        public async void TestCheckHostelResident()
        {
            var result1 = await _Controller.CheckHostelResident(11,1008);//NF
            var result2 = await _Controller.CheckHostelResident(11,1009);//NF
            var result3 = await _Controller.CheckHostelResident(11,1010);//NF
            var result4 = await _Controller.CheckHostelResident(11,2010);//NF
            var result5 = await _Controller.CheckHostelResident(11,2011);//NF
            var result6 = await _Controller.CheckHostelResident(15,2010);//NF
            var result7 = await _Controller.CheckHostelResident(15,2011);//NF
            var result8 = await _Controller.CheckHostelResident(15,2012);//NF


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<OkObjectResult>(result5);
            Assert.IsType<OkObjectResult>(result6);
            Assert.IsType<OkObjectResult>(result7);
            Assert.IsType<OkObjectResult>(result8);

        }
        [Fact]
        public void TestCheckResidentChangedRoom()
        {
            bool result1 = CheckResidentChangedRoom(11,1009);
            bool result2 = CheckResidentChangedRoom(11,1008);
            bool result3 = CheckResidentChangedRoom(11,1010);
            bool result4 = CheckResidentChangedRoom(15,2010);
            bool result5 = CheckResidentChangedRoom(15,2011);
            bool result6 = CheckResidentChangedRoom(15,2012);
            bool result7 = CheckResidentChangedRoom(15,1009);
            Assert.False(result1);
            Assert.True(result2);
            Assert.False(result3);
            Assert.False(result4);
            Assert.False(result5);
            Assert.False(result6);
            Assert.False(result7);
        }
        [Fact]
        public void TestCountResidentInRoom()
        {
            var result1 = CountResidentInRoom(_context.Rooms.Where(r=>r.Id==61).SingleOrDefault());
            var result2 = CountResidentInRoom(_context.Rooms.Where(r => r.Id == 62).SingleOrDefault());
            var result3 = CountResidentInRoom(_context.Rooms.Where(r => r.Id == 63).SingleOrDefault());
            var result4 = CountResidentInRoom(_context.Rooms.Where(r => r.Id == 64).SingleOrDefault());
            var result5 = CountResidentInRoom(_context.Rooms.Where(r => r.Id == 73).SingleOrDefault());
            var result6 = CountResidentInRoom(_context.Rooms.Where(r => r.Id == 75).SingleOrDefault());
            var result7 = CountResidentInRoom(_context.Rooms.Where(r => r.Id == 74).SingleOrDefault());
            Assert.Equal(2,result1);
            Assert.Equal(1,result2);
            Assert.Equal(0,result3);
            Assert.Equal(0,result4);
            Assert.Equal(1,result5);
            Assert.Equal(2,result6);
            Assert.Equal(0,result7);
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
        private bool CheckResidentChangedRoom(int hostelId, int MemberId)
        {
            var residents = _context.Residents.Where(r => r.HostelId == hostelId && r.MemberId == MemberId).ToList();
            if (residents.Count > 1)
            {
                return true;
            }
            return false;

        }
        private int CountResidentInRoom(Room room)
        {
            var residents = _context.Residents.Where(r => r.RoomId == room.Id).ToList();
            residents = residents.Where(r => r.Status == 1).ToList();
            return residents.Count();
        }
    }


}
