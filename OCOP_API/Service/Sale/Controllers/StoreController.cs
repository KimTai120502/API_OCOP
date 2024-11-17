using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sv.Sale;

namespace Sale.Controllers
{
    [Route("~/sale/[controller]/[action]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public StoreController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

    }
}
