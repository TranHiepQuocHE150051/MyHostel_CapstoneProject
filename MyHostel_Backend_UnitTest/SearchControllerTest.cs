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
    public class SearchControllerTest
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        private SearchController _Controller;

        public SearchControllerTest()
        {
            _context = new MyHostelContext();
            _Controller = new SearchController(_configuration, _context);
        }
        [Fact]
        public async void TestSearchProvince()
        {
            var result1 = await _Controller.searchProvince("");
            var result2 = await _Controller.searchProvince(null);
            var result3 = await _Controller.searchProvince("Ha noi");
            var result4 = await _Controller.searchProvince("Nam dinh");


            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<OkObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);

        }
        [Fact]
        public async void TestSearchDW()
        {
            var result1 = await _Controller.searchDW("","");
            var result2 = await _Controller.searchDW(null,null);
            var result3 = await _Controller.searchDW("Ha","01");
            var result4 = await _Controller.searchDW("Ha", null);
            var result5 = await _Controller.searchDW(null, "01");


            Assert.IsType<NotFoundResult>(result1);
            Assert.IsType<BadRequestObjectResult>(result2);
            Assert.IsType<OkObjectResult>(result3);
            Assert.IsType<OkObjectResult>(result4);
            Assert.IsType<BadRequestObjectResult>(result5);

        }
    }
}
