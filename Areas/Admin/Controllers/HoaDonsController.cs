using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.ViewModel;
using X.PagedList.Extensions;

namespace ThuongMaiDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/HoaDon")]
    [Authorize(Roles = "Admin")]

    public class HoaDonsController : Controller
    {
        private readonly Hshop2023Context _context;
        private const int pageSize =20;
        public HoaDonsController(Hshop2023Context context)
        {
            _context = context;
        }

        // GET: Admin/HoaDons
        [Route("")]
        public IActionResult Index(int? filter, int? page=1)
        {
            var data=_context.HoaDons.AsQueryable();
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
             data = data.Include(h => h.MaKhNavigation).Include(h => h.MaNvNavigation).Include(h => h.MaTrangThaiNavigation);
            if (filter.HasValue && filter != 100)
            {
                data = data.Where(x => x.MaTrangThai == filter);
            }
            var hd = data.Select(p => new HoadonVM
            {
                MaHd = p.MaHd,
                MaKh = p.MaKh,
                MaTrangThai = p.MaTrangThai,
                NgayDat = p.NgayDat,
                SoDienThoai = p.SoDienThoai,
                Ten=p.MaKhNavigation.HoTen??string.Empty,
                DiaChi=p.DiaChi,
                TongTien=_context.ChiTietHds.Where(x=>x.MaHd==p.MaHd).Sum(x=>(x.SoLuong*(x.DonGia-(x.DonGia*x.GiamGia))))           

            }).OrderByDescending(x => x.NgayDat).ToPagedList(pageNumber,pageSize);
            ViewBag.filter = filter;
            return View(hd);
        }
        [Route("FilterInvoices")]
        public IActionResult FilterInvoices(int? filter, int? page = 1)
        {
            var data = _context.HoaDons.AsQueryable();
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            data = data.Include(h => h.MaKhNavigation).Include(h => h.MaNvNavigation).Include(h => h.MaTrangThaiNavigation);
            if (filter.HasValue&&filter!=100)
            {
                data = data.Where(x => x.MaTrangThai == filter);
            }
            var hd = data.Select(p => new HoadonVM
            {
                MaHd = p.MaHd,
                MaKh = p.MaKh,
                MaTrangThai = p.MaTrangThai,
                NgayDat = p.NgayDat,
                SoDienThoai = p.SoDienThoai,
                Ten = p.MaKhNavigation.HoTen ?? string.Empty,
                DiaChi = p.DiaChi,
                TongTien = _context.ChiTietHds.Where(x => x.MaHd == p.MaHd).Sum(x => (x.SoLuong * (x.DonGia - (x.DonGia * x.GiamGia))))

            }).OrderByDescending(x => x.NgayDat).ToPagedList(pageNumber, pageSize);
            ViewBag.filter = filter;

            return PartialView("HoaDonViewFilter", hd);
        }

        // GET: Admin/HoaDons/Details/5
        [Route("Details")]
        public  IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
             var data = _context.ChiTietHds.Include(h => h.MaHhNavigation).Include(h => h.MaHdNavigation).Where(p=>p.MaHd==id);
            if (data == null)
            {
                return NotFound();
            }
            var hdct = data.Select(p => new HoadonDetailVM {
                DonGia = p.DonGia,
                GiamGia = p.GiamGia,
                SoLuong = p.SoLuong,
                Ten = p.MaHhNavigation.TenHh ?? string.Empty,
                TongTien = p.SoLuong * (p.DonGia-(p.DonGia*p.GiamGia))
            });
           

            return View(hdct.ToList());
        }

        [Route("Done")]

        [HttpGet]
        public  IActionResult Done(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hd =  _context.HoaDons.FirstOrDefault(x=>x.MaHd==id);
            if (hd == null)
            {
                return NotFound();
            }
            hd.MaTrangThai = 3;
            _context.Update(hd);
            _context.SaveChanges();
            TempData["Message"] = "Cập nhật thành công ";
            return RedirectToAction("index");
        }

    }
}
