using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Models.Response;
using sv.Sale.DBModels;

namespace Sale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> testCookie()
        {
            try
            {
                // Tạo một cookie với tên và giá trị
                var cookieName = "MyCookie";
                var cookieValue = "ThisIsMyCookieValue";

                // Thêm cookie vào response
                Response.Cookies.Append(cookieName, cookieValue, new CookieOptions
                {
                    HttpOnly = true, // Bảo mật, không thể truy cập từ JavaScript
                    Secure = true,   // Chỉ gửi cookie qua HTTPS
                    SameSite = SameSiteMode.Strict, // Cookie không gửi khi redirect
                    Expires = DateTimeOffset.UtcNow.AddDays(7) // Hết hạn sau 7 ngày
                });

                return Ok("abc");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
