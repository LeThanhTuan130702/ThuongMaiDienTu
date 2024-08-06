using ThuongMaiDienTu.ViewModel;

namespace ThuongMaiDienTu.Service
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentResquesModel model);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collection);
    }
}
