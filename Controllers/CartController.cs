using Microsoft.AspNetCore.Mvc;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.ViewModel;
using ThuongMaiDienTu.Helper;
using Microsoft.AspNetCore.Authorization;
using ThuongMaiDienTu.Service;
using PayPalCheckoutSdk.Core;

namespace ThuongMaiDienTu.Controllers
{
    public class CartController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly PaypalClient _paypalClient;
        private readonly Hshop2023Context _contex;


        public CartController(Hshop2023Context context, PaypalClient paypalClient, IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
            _paypalClient = paypalClient;
            _contex = context;
        }
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        public IActionResult Index()
        {

            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = Cart;

            var item = cart.SingleOrDefault(x => x.Id == id);
            if (item == null)
            {
                var hh = _contex.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hh == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return NotFound();
                }

                item = new CartItem
                {
                    Id = hh.MaHh,
                    Image = hh.Hinh ?? string.Empty,
                    Quantity = quantity,
                    Name = hh.TenHh,
                    Price = hh.DonGia ?? 0,
                    Discout = hh.GiamGia
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveToCart(int id)
        {
            var cart = Cart;

            var item = cart.SingleOrDefault(x => x.Id == id);
            if (item == null)
            {
                var hh = _contex.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hh == null)
                {
                    return RedirectToAction("Index", "HomeController");
                }
                return RedirectToAction("Index", "HomeController");

            }
            else {
                cart.Remove(item);
            }

            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public IActionResult CheckOut()
        {


            if (Cart.Count() == 0)
            {
                return Redirect("/");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;

            return View(Cart);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CheckOut(CheckOutVM model, string payment = PaymentType.COD)
        {

            if (ModelState.IsValid)
            { if (!model.SameCustomer)
                {
                    if (model.Name == null || model.Phone == null || model.Address == null)
                    {
                        ModelState.AddModelError("error", "information is not empty");
                        return View(Cart);

                    }
                }
                var _paymentModel = new PaymentsModel
                {
                    CachThanhToan = "",
                    CachVanChuyen = "",
                    MaTrangThai = 0  //Unpaid
                };

                if (payment == "Place Order By VNPAY")
                {
                    var VnpayModel = new VnPaymentResquesModel
                    {
                        Amount = Cart.Sum(p => p.Total),
                        CreateDate = DateTime.Now,
                        Description = $"{model.Name} {model.Phone}",
                        fullname = model.Name,

                    };
                    _paymentModel.CachVanChuyen = "Local";
                    _paymentModel.CachThanhToan = "VNPAY";

                    var ad = AddOrder(model, _paymentModel);
                    if (ad != 0 && ad != -1)
                    {
                        VnpayModel.orderId = ad;
                        return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, VnpayModel));

                    }
                    return View("fail");
                }
                _paymentModel.CachVanChuyen = "Local";
                _paymentModel.CachThanhToan = "COD";
                var _add_order = AddOrder(model, _paymentModel);
                if (_add_order == -1)
                {
                    return View("Fail");
                }
                if (_add_order == 0)
                {
                    return View(Cart);

                }
                return View("Success");

            }
            return View(Cart);
        }
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View("fail");
        }
        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }
        [Authorize]

        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExcute(Request.Query);
            var HD = _contex.HoaDons.FirstOrDefault(p => p.MaHd == response.OrderId);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi Thanh Toán VNPAY: {response.VnPayResponseCode}";
                HD.MaTrangThai = -1;// lỗi thanh toán 
                _contex.SaveChanges();
                return RedirectToAction("PaymentFail");
            }
            HD.MaTrangThai = 1;
            _contex.SaveChanges();
            TempData["Message"] = $"Thanh Toán VNPAY Thành Công";
            return RedirectToAction("PaymentSuccess");

        }
        public int AddOrder(CheckOutVM model, PaymentsModel PModel)
        {
            var ghichu = model.Note;
            var CustomerId = HttpContext.User.Claims.SingleOrDefault(Cu => Cu.Type == MySetting.CLAIMS_CUSTOMERID).Value;
            var KH = new KhachHang();
            if(model.SameCustomer)
            {
                model = new CheckOutVM();
                KH = _contex.KhachHangs.SingleOrDefault(Kh => Kh.MaKh == CustomerId);

            }
            var HoaDon = new HoaDon()
            {
                MaKh = CustomerId ?? KH.MaKh,
                HoTen = model.Name ?? KH.HoTen,
                DiaChi = model.Address ?? KH.DiaChi,
                SoDienThoai = model.Phone ?? KH.DienThoai,
                NgayDat = DateTime.Now,
                CachThanhToan = PModel.CachThanhToan,
                CachVanChuyen = PModel.CachVanChuyen,
                MaTrangThai = PModel.MaTrangThai,
                GhiChu = ghichu


            };
            //_contex.HoaDons.Add(HoaDon);
            _contex.Database.BeginTransaction();
            try
            {

                _contex.Database.CommitTransaction();
                _contex.Add(HoaDon);
                _contex.SaveChanges();

                var Details = new List<ChiTietHd>();
                foreach (var item in Cart)
                {
                    if (item == null)
                    {
                        return -1;//error
                    }
                    Details.Add(new ChiTietHd
                    {
                        MaHd = HoaDon.MaHd,
                        SoLuong = item.Quantity,
                        DonGia = item.Price - (item.Price * item.Discout),
                        MaHh = item.Id,
                        GiamGia = item.Discout,

                    });
                }
                _contex.AddRange(Details);
                _contex.SaveChanges();
                HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                return HoaDon.MaHd;//success

            }
            catch (Exception ex)
            {
                _contex.Database.RollbackTransaction();
                return 0;

            }
        }
        #region PaypalPayment
        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var TotalPrice=Cart.Sum(x => x.Total).ToString();
            var currency = "USD";
            var reference = "DH" + DateTime.Now.Ticks.ToString();
            try
            {
                var response = await _paypalClient.CreateOrder(TotalPrice, currency, reference);
                return Ok(response);
            }
            catch(Exception e) {
             var error = new {e.GetBaseException().Message};
                return BadRequest(error);
            }
        }
        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePayPalOrder(string orderID,[FromBody]CheckOutVM model,CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);
                if(orderID=="0")
                {
                    return RedirectToAction("PaymentFail");
                }
                var _paymentModel = new PaymentsModel
                {
                    CachThanhToan="PayPal",
                    CachVanChuyen="local",
                    MaTrangThai=1
                };

                var add_db = AddOrder(model, _paymentModel);
                if (add_db != 0 && add_db != -1)
                {
                    TempData["Message"] = $"Thanh Toán PayPal Thành Công";
                    return Ok(response);
                    

                }
                TempData["Message"] = $"Lỗi Dữ liệu vui lòng liên hệ admin để được hỗ trợ";
                return RedirectToAction("PaymentFail");
                // add to db    
                //var add_db=AddOrder()

                //return Ok(response);
            }
            catch (Exception e)
            {
                var error = new { e.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion
    }
}
