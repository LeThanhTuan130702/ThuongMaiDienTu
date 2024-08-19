using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Models;

namespace ThuongMaiDienTu.Controllers
{
    public class HomeController : Controller
    {
        private readonly Hshop2023Context _context;
        

        public HomeController(Hshop2023Context context)

        {
            _context = context;
            
        }

        public IActionResult Index()
        {
            var Loai=_context.Loais.Include(l=>l.HangHoas).ToList();
            return View(Loai);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
