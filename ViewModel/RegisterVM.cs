using System.ComponentModel.DataAnnotations;

namespace ThuongMaiDienTu.ViewModel
{
    public class RegisterVM
    {
        [Display(Name ="Username")]
        [Required(ErrorMessage ="*")]
        [MaxLength(20,ErrorMessage ="Max is 20")]
        public string MaKh { get; set; }
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string MatKhau { get; set; }
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Max is 50")]
        [Display(Name = "Name")]

        public string HoTen { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Sex")]


        
        public bool GioiTinh { get; set; }=true;
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]

        public DateTime? NgaySinh { get; set; }
        [Required(ErrorMessage = "*")]
        [MaxLength(60, ErrorMessage = "Max is 60")]
        [Display(Name = "Address")]


        public string DiaChi { get; set; }
        [Required(ErrorMessage = "*")]
        [MaxLength(24, ErrorMessage = "Max is 24")]
        [RegularExpression(@"0[98735]\d{8}",ErrorMessage ="number must begin by 0")]
        [Display(Name = "Mobile No")]

        public string DienThoai { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "E-mail")]

        [EmailAddress(ErrorMessage ="Wrong")]
        public string Email { get; set; }
        [Display(Name = "Image")]
        public string? Hinh { get; set; }
    }
}
