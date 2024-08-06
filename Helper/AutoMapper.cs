using AutoMapper;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.ViewModel;

namespace ThuongMaiDienTu.Helper
{
	public class AutoMapperProfile:Profile
	{
		public AutoMapperProfile() {
			CreateMap<RegisterVM, KhachHang>().ReverseMap();
            CreateMap<HangHoaDetailVM, HangHoa>().ReverseMap();

        }
    }
}
