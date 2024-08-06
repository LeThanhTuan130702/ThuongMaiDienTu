using Microsoft.AspNetCore.Mvc;
using ThuongMaiDienTu.Data;

namespace ThuongMaiDienTu.ViewComponents
{
    public class Navbar:ViewComponent
    {
        private readonly Hshop2023Context db;

        public Navbar(Hshop2023Context context) => db = context;
       public IViewComponentResult Invoke()
        {
          return View(db.Loais.ToList()); 
        }
        
    }
}
