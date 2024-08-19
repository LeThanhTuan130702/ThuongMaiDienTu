using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly Hshop2023Context _context;

        public NhanViensController(Hshop2023Context context,IMapper mapper)
        {
            _mapper=mapper;
            _context = context;
        }

        // GET: Admin/NhanViens
        [HttpGet]
        [Route("Index")]
        [Authorize(Roles = "Admin")]

        public IActionResult Index()
        {
            return View( _context.NhanViens.ToList());
        }

        // GET: Admin/NhanViens/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var nhanVien = await _context.NhanViens
        //        .FirstOrDefaultAsync(m => m.MaNv == id);
        //    if (nhanVien == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(nhanVien);
        //}

        // GET: Admin/NhanViens/Create
        [Route("Create")]
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(RegisterVM model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var NV = _mapper.Map<NhanVien>(model);
                    NV.MaNv = model.MaKh;
                    NV.RandomKey = Util.GenerateRandomKey();
                    NV.MatKhau = model.MatKhau.ToMd5Hash(NV.RandomKey);
                    NV.HieuLuc = true;//xu ly sau
                    NV.VaiTro = 0;
                    if (Hinh != null)
                    {
                        NV.Hinh = Util.UploadImage(Hinh, "NhanVien");
                    }
                    _context.NhanViens.Add(NV);
                    _context.SaveChanges();
                    TempData["Message"] = "Successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return NotFound();
                }

            }
            return View();
        }



        // GET: Admin/NhanViens/Edit/5
        [Route("Edit")]
        [Authorize(Roles = "Admin")]

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
            return View();
        }

        // POST: Admin/NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
    [Authorize(Roles = "Admin")]


        //public async Task<IActionResult> Edit(string id, RegisterVM model, IFormFile? Hinh)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(nhanVien);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!NhanVienExists(nhanVien.MaNv))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(nhanVien);
        //}

        // GET: Admin/NhanViens/Delete/5
        [Route("Delete")]
        [Authorize(Roles = "Admin")]

        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien =  _context.NhanViens
                .FirstOrDefault(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            nhanVien.HieuLuc = false;
            _context.Update(nhanVien);
            TempData["Message"] = "Successfully";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/NhanViens/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var nhanVien = await _context.NhanViens.FindAsync(id);
        //    if (nhanVien != null)
        //    {
        //        _context.NhanViens.Remove(nhanVien);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

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
        //            var NV = _mapper.Map<NVachHang>(model);
        //            NV.RandomKey = Util.GenerateRandomKey();
        //            NV.MatNVau = model.MatNVau.ToMd5Hash(NV.RandomKey);
        //            NV.HieuLuc = true;//xu ly sau
        //            NV.VaiTro = 0;
        //            if (Hinh != null)
        //            {
        //                NV.Hinh = Util.UploadImage(Hinh, "NVachHang");
        //            }
        //            _context.NVachHangs.Add(NV);
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
