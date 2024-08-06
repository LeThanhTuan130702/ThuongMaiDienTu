using Microsoft.AspNetCore.Mvc;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.ViewModel;
using X.PagedList;
using X.PagedList.Extensions;

namespace ThuongMaiDienTu.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context _context;
        private const int PageSize = 6;

        public HangHoaController(Hshop2023Context context) =>_context=context;
        public IActionResult Index(int? loai,int?page=1)
        {
            if(loai==0)
            {
                loai = null;
            }    
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var hh=_context.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hh = hh.Where(p => p.MaLoai == loai.Value).OrderBy(x=>x.TenHh);
            }
            var result = hh.Select(p => new HangHoaVM
            {
                Id = p.MaHh,
                Name = p.TenHh,
                Image = p.Hinh ,
                Discout=p.GiamGia,
                Price=p.DonGia ??0,
                CateID=loai??0

            }).ToPagedList(pageNumber,PageSize);
            return View(result);
        }
        public IActionResult Search(string? query, double? minPrice, double? maxPrice, string?OrderByName,string?OrderByPrice, int?page=1)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var hh = _context.HangHoas.AsQueryable();
            if (minPrice.HasValue)
            {
                hh = hh.Where(p => p.DonGia >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                hh = hh.Where(p => p.DonGia <= maxPrice.Value);
            }
            if(!string.IsNullOrEmpty(query))
            {
               hh= hh.Where(p => p.TenHh.Contains(query));
            }
            if (!string.IsNullOrEmpty(OrderByPrice))
            {
                hh = hh.OrderBy(x=>x.DonGia);
            }
            if (!string.IsNullOrEmpty(OrderByName))
            {
                hh = hh.OrderBy(x => x.TenHh);
            }
            var result = hh.Select(p => new HangHoaVM
            {
                Id = p.MaHh,
                Name = p.TenHh,
                Image = p.Hinh,
                Discout = p.GiamGia,
                Price = p.DonGia ?? 0,

            }).ToPagedList(pageNumber, PageSize);
            ViewBag.query = query;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;

            return View(result);
        }
        public IActionResult Detail(int id) {
            var data = _context.HangHoas.SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
               return NotFound();
            }
            var result = new HangHoaDetailVM
            {
                Id = data.MaHh,    
                Name = data.TenHh,
                Price=data.DonGia??0,
                Description=data.MoTa??string.Empty,
                ShortDescription = data.MoTaDonVi ?? string.Empty,
                Discout =data.GiamGia,
                Review=5,
                Image=data.Hinh??string.Empty,
                QuantityInStock=10

            };
            return View(result);
        }
    }
}
