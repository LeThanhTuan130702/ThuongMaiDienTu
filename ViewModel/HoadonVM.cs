using System.ComponentModel.DataAnnotations;

namespace ThuongMaiDienTu.ViewModel
{
    public class HoadonVM
    {
        [Key]
        public int MaHd { get; set; }

        public string MaKh { get; set; } = null!;

        public DateTime NgayDat { get; set; }
        public int MaTrangThai { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Ten {  get; set; }

    }
}
