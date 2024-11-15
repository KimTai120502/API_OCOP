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
    }
}
