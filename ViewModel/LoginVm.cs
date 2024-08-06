using System.ComponentModel.DataAnnotations;

namespace ThuongMaiDienTu.ViewModel
{
	public class LoginVm
	{
		[Display(Name ="Username")]
		[Required]
		[MaxLength(20)]
		public string username { get; set; }
		[Display(Name = "Password")]
		[Required]
		[DataType(DataType.Password)]

		public string password { get; set; }
	}
}
