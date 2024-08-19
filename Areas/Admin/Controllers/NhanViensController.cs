using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Helper;
using ThuongMaiDienTu.ViewModel;

namespace ThuongMaiDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/Account")]
    public class NhanViensController : Controller
    {
        private readonly Hshop2023Context _context;

        public NhanViensController(Hshop2023Context context)
        {
            _context = context;
        }

        // GET: Admin/NhanViens
        [HttpGet]
        [Route("Index")]
        public  IActionResult Index()
        {
            return View( _context.NhanViens.ToList());
        }

        // GET: Admin/NhanViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNv,HoTen,Email,MatKhau,GioiTinh,NgaySinh,DiaChi,DienThoai,Hinh,HieuLuc,VaiTro,RandomKey")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNv,HoTen,Email,MatKhau,GioiTinh,NgaySinh,DiaChi,DienThoai,Hinh,HieuLuc,VaiTro,RandomKey")] NhanVien nhanVien)
        {
            if (id != nhanVien.MaNv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.MaNv))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(string id)
        {
            return _context.NhanViens.Any(e => e.MaNv == id);
        }
        //[HttpGet]
        //[Route("register")]
        //public IActionResult Register()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult Register(RegisterVM model, IFormFile? Hinh)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var KH = _mapper.Map<KhachHang>(model);
        //            KH.RandomKey = Util.GenerateRandomKey();
        //            KH.MatKhau = model.MatKhau.ToMd5Hash(KH.RandomKey);
        //            KH.HieuLuc = true;//xu ly sau
        //            KH.VaiTro = 0;
        //            if (Hinh != null)
        //            {
        //                KH.Hinh = Util.UploadImage(Hinh, "KhachHang");
        //            }
        //            _context.KhachHangs.Add(KH);
        //            _context.SaveChanges();
        //            return RedirectToAction("Index", "HangHoa");
        //        }
        //        catch (Exception ex)
        //        {
        //            return NotFound();
        //        }

        //    }
        //    return View();
        //}
        [HttpGet]
        [Route("login")]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
            
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var NV = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == model.username);
                if (NV == null)
                {
                    ModelState.AddModelError("error", "wrong username or password");
                }
                else
                {
                    if (!NV.HieuLuc)
                    {
                        ModelState.AddModelError("error", "You cannot use this account anymore");
                    }
                    else
                    {
                        if (NV.MatKhau != model.password.ToMd5Hash(NV.RandomKey))
                        {
                            ModelState.AddModelError("error", "wrong username or password");

                        }
                        else
                        {
                            var claims = new List<Claim>()
                            {
                                new Claim(ClaimTypes.Email, NV.Email),
                                new Claim(ClaimTypes.Name, NV.HoTen),
                                new Claim(MySetting.CLAIMS_Admin, NV.MaNv),
								// động
								new Claim(ClaimTypes.Role, "Admin"),
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, "login");
                            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrinciple);
                            HttpContext.Session.SetString("Admin", NV.HoTen);
                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/admin");
                            }

                        }
                    }
                }
            }
            return View();

        }
        [Authorize(Roles = "Admin")]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Remove("Admin");


            return Redirect("/admin/account/login");
        }
    }
}
