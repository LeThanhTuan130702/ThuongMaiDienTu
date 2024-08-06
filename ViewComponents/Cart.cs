using Microsoft.AspNetCore.Mvc;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Helper;
using ThuongMaiDienTu.ViewModel;

namespace ThuongMaiDienTu.ViewComponents
{
    public class Cart : ViewComponent
    {
       
        public IViewComponentResult Invoke()
        {
            var cart= HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

            return View(new CartModel {
            Quantity=cart.Sum(x => x.Quantity),
            });
        }

    }
}
