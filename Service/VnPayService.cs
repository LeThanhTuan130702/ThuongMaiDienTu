using System.Security.Policy;
using ThuongMaiDienTu.Helper;
using ThuongMaiDienTu.ViewModel;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ThuongMaiDienTu.Service
{
    public class VnPayService : IVnPayService
    {
        private IConfiguration _configuration;

        public VnPayService(IConfiguration configuration) {
            _configuration = configuration;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentResquesModel model)
        {
            //var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            //Số tiền thanh toán. Số tiền không 
            //mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
            //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
            //là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", model.CreateDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh Toán Cho Đơn Hàng:" + model.orderId);
            //vnpay.AddRequestData("Cus_OrderId",  model.orderId.ToString());

            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", model.orderId.ToString());
            //// Mã tham chiếu của giao dịch tại hệ 
            //thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không được
            //    trùng lặp trong ngày
            var PaymentUrl = vnpay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return PaymentUrl;
        }

        public VnPaymentResponseModel PaymentExcute(IQueryCollection collection)
        {
           var vnpay=new VnPayLibrary();
            foreach(var (key,value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                } 
                    
            }
            var vnp_orderId = Convert.ToUInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");;
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            bool CheckSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["Vnpay:HashSecret"]);
            if (!CheckSignature) 
            {
                return new VnPaymentResponseModel {
                 Success = false,
                };
            }
            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "Vnpay",
                OrderDescription = vnp_OrderInfo,
                OrderId = (int)vnp_orderId,
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,


            };
        }
    }
}
