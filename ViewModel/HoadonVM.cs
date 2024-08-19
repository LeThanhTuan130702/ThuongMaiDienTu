using System.ComponentModel.DataAnnotations;
using ThuongMaiDienTu.Data;

namespace ThuongMaiDienTu.ViewModel
{
    public class HoadonVM
    {
        [Key]
        public int MaHd { get; set; }

        public string MaKh { get; set; } = null!;

        public DateTime NgayDat { get; set; }
        public int MaTrangThai { get; set; }
        public string? DiaChi {  get; set; }
        public string? SoDienThoai { get; set; }
        public string? Ten {  get; set; }
        public double TongTien {  get; set; }

    }
    public class HoadonDetailVM
    {
        [Key]

        public string Ten { get; set; }

        public double DonGia { get; set; }

        public int SoLuong { get; set; }

        public double GiamGia { get; set; }
        public double TongTien { get;set; }


    }
}
