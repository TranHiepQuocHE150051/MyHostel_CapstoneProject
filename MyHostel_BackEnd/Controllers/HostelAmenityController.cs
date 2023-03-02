using Microsoft.AspNetCore.Mvc;
using MyHostel_BackEnd.Models;

namespace MyHostel_BackEnd.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HostelAmenityController : Controller
    {
        private IConfiguration _configuration;
        private MyHostelContext _context;
        public HostelAmenityController(IConfiguration configuration, MyHostelContext context)
        {
            _configuration = configuration;
            _context = context;
        }
    }
}
