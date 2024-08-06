using System.Text;

namespace ThuongMaiDienTu.Helper
{
	public class Util
	{
		
		public static string UploadImage(IFormFile Hinh, string folder)
		{
			try
			{
				var fullpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
				using (var myfile = new FileStream(fullpath, FileMode.CreateNew))
				{
					Hinh.CopyTo(myfile);
				}
				return Hinh.FileName;
			}
			catch(Exception e) {
			return string.Empty;
			}
		}
		public static string GenerateRandomKey(int length = 5)
		{
			var pattern = "qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKOLP";
			var sb = new StringBuilder();
			var rd = new Random();
			for (int i = 0; i < length; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}
			return sb.ToString();
		}
	}
}
