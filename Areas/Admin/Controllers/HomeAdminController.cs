using System.Net.WebSockets;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Helper;
using ThuongMaiDienTu.ViewModel;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ThuongMaiDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    [Route("admin/homeAdmin")]

    public class HomeAdminController : Controller
    {
        private const int pageSize = 6;
        private readonly Hshop2023Context _context;
        private readonly IMapper _mapper;

        public HomeAdminController(Hshop2023Context context,IMapper mapper) {
            _context=context;
            _mapper = mapper;
        }
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("ProductList")]
        public IActionResult ProductList(int? loai,int? page)
        {
            if(loai==0)
            {
                loai = null;
            }    
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var hh = _context.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hh = hh.Where(p => p.MaLoai == loai.Value);
            }
            
            var result = hh.Select(p => new HangHoaVM
            {
                Id = p.MaHh,
                Name = p.TenHh,
                Image = p.Hinh,
                Discout = p.GiamGia,
                Price = p.DonGia ?? 0,
                CateID=loai??null

            }).ToPagedList(pageNumber,pageSize);
            return View(result);
        }
        [Route("Create")]
        [HttpGet]
        public IActionResult Create() {
            var loai = _context.Loais.ToList();
            ViewBag.Maloai = new SelectList(loai, "MaLoai", "TenLoai");
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HangHoaDetailVM model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var hh = new HangHoa
                    {
                        DonGia = model.Price,
                        GiamGia=model.Discout,
                        MaLoai=model.CateId??0,
                        MaNcc="AP",
                        NgaySx=DateTime.Now,
                        MoTa=model.Description,
                        MoTaDonVi=model.ShortDescription,
                        TenHh=model.Name,
                        SoLanXem=model.Review,
                        
                        
                        
                    };
                    if (Hinh != null)
                    {
                        hh.Hinh = Util.UploadImage(Hinh, "HangHoa");
                    }
                    TempData["Message"] = "Tạo Thành Công ";
                    _context.HangHoas.Add(hh);
                    _context.SaveChanges();

                    return RedirectToAction("ProductList");

                }
                catch
                {
                    TempData["Message"] = "Lỗi tạo sản phẩm ";
                    return RedirectToAction("ProductList");


                }

            }    
            return RedirectToAction("ProductList");

        }
        //[Authorize]
        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data =_context.HangHoas.Find(id);
            if (data == null)
            {
                TempData["Message"] = "Không Tìm Thấy Sản phẩm Sửa";
                return RedirectToAction("ProductList");

                return NotFound();
            }
           
            else
            {

                var hhvm = new HangHoaDetailVM
                {
                    Id = data.MaHh,
                    Name = data.TenHh,
                    Price = data.DonGia ?? 0,
                    Description = data.MoTa ?? string.Empty,
                    ShortDescription = data.MoTaDonVi ?? string.Empty,
                    Discout = data.GiamGia,
                    Review = data.SoLanXem,
                    Image = data.Hinh ?? string.Empty,
                    QuantityInStock = 10,
                    CateId = data.MaLoai

                };
                ViewBag.Maloai = new SelectList(_context.Loais.ToList(), "MaLoai", "TenLoai", hhvm.CateId);
                //ViewBag.MaNCC = new SelectList(_context.NhaCungCaps.ToList(), "MaNcc", "TenCongTy", hhvm.MaNcc);
                return View(hhvm);

            }
        }
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(HangHoaDetailVM model,IFormFile? Hinh)
        {
            if(ModelState.IsValid)
            {

                var hh = _context.HangHoas.Find(model.Id);
                if(hh == null)
                {
                    TempData["Message"] = "Không Tìm Thấy Sản phẩm xóa";
                    return RedirectToAction("ProductList");

                }
                hh.DonGia = model.Price;
                hh.GiamGia = model.Discout;
                hh.MaLoai = model.CateId ?? 0;
                hh.MaNcc = "AP";
                hh.NgaySx = DateTime.Now;
                hh.MoTa = model.Description;
                hh.MoTaDonVi = model.ShortDescription;
                hh.TenHh = model.Name;
                hh.SoLanXem = model.Review;
                if (Hinh != null)
                {
                    hh.Hinh = Util.UploadImage(Hinh, "HangHoa");
                }
                TempData["Message"] = "Sửa Thành Công ";

                _context.Update(hh);
                _context.SaveChanges();
                return RedirectToAction("ProductList");
            }
            return View();
        }
        [Route("Delete")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var hh = _context.HangHoas.FirstOrDefault(x=>x.MaHh==id);
                var Detail = _context.ChiTietHds.FirstOrDefault(p => p.MaHh == id);
                if (Detail != null)
                {
                    TempData["Message"] = "Không thể xóa do vướng ràng buộc ";
                    return RedirectToAction("ProductList");
                }
                if (hh == null)
                {
                    TempData["Message"] = "Không Tìm thấy sản phẩm ";
                    return RedirectToAction("ProductList");


                }
                _context.Remove(hh);
                _context.SaveChanges();
                TempData["Message"] = "Xóa thành Công";
                return RedirectToAction("ProductList");
            }
            catch (Exception ex) {
                TempData["Message"]=ex.Message;
                return RedirectToAction("ProductList");

            }

        }

    }
}
