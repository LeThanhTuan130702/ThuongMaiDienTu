using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Helper;
using ThuongMaiDienTu.ViewModel;

namespace ThuongMaiDienTu.Controllers
{
	public class KhachHangController : Controller
	{
		private readonly Hshop2023Context _context;
		private readonly IMapper _mapper;

		public KhachHangController(Hshop2023Context context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Register(RegisterVM model, IFormFile? Hinh)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var KH = _mapper.Map<KhachHang>(model);
					KH.RandomKey = Util.GenerateRandomKey();
					KH.MatKhau = model.MatKhau.ToMd5Hash(KH.RandomKey);
					KH.HieuLuc = true;//xu ly sau
					KH.VaiTro = 0;
					if (Hinh != null)
					{
						KH.Hinh = Util.UploadImage(Hinh, "KhachHang");
					}
					_context.KhachHangs.Add(KH);
					_context.SaveChanges();
					return RedirectToAction("Index", "HangHoa");
				}
				catch (Exception ex)
				{
					return NotFound();
				}

			}
			return View();
		}
		[HttpGet]
		public IActionResult Login(string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			return View();

		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVm model, string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			if (ModelState.IsValid)
			{ 
				var KH = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.username);
				if (KH == null)
				{
					ModelState.AddModelError("error", "wrong username or password");
				}
				else
				{
					if (!KH.HieuLuc)
					{
						ModelState.AddModelError("error", "You cannot use this account anymore");
					}
					else
					{
						if (KH.MatKhau != model.password.ToMd5Hash(KH.RandomKey))
						{
							ModelState.AddModelError("error", "wrong username or password");

						}
						else
						{
							var claims = new List<Claim>()
							{
								new Claim(ClaimTypes.Email, KH.Email),
								new Claim(ClaimTypes.Name, KH.HoTen),
								new Claim(MySetting.CLAIMS_CUSTOMERID, KH.MaKh),
								// động
								new Claim(ClaimTypes.Role, "Customer"),
							};
							var claimsIdentity=new ClaimsIdentity(claims,"login");
							var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);
							await HttpContext.SignInAsync(claimsPrinciple);
                            HttpContext.Session.SetString("UserName", KH.HoTen);
                            if (Url.IsLocalUrl(ReturnUrl))
							{
								 return Redirect(ReturnUrl);
							}
							else
							{ 
							return Redirect("/");
							}

						}
					}
				}
			}
			return View();

		}
		[Authorize]
		[HttpGet]
		public IActionResult Profile()
		{

			return View(); 
		}
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
			await HttpContext.SignOutAsync();
            HttpContext.Session.Remove("UserName");

            return Redirect("/");
        }

    }
}
