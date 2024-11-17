using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Models.Response;

namespace Sale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        internal async Task<ResponseModel> ResponseFail()
        {
            return new ResponseModel() { status = -1 };
        }

        internal async Task<ResponseModel> ResponseSucceeded()
        {
            return new ResponseModel() { status = 1, message = "" };
        }

        private static string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string ReplaceUnicode(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
                return string.Empty;
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                {
                    strInput = strInput.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                }
            }
            strInput = strInput.Replace("_", "\\_").Replace("[", "\\[").Replace("%", "\\%").Replace("^", "\\^").Replace("-", "\\-").Replace("]", "\\]");
            return strInput.ToLower();
        }
    }
}
