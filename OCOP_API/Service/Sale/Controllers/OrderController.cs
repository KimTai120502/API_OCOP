using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Models.Response;
using sv.Sale;
using sv.Sale.DBModels;

namespace Sale.Controllers
{
    [Route("~/sale/[controller]/[action]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IProductRepository productRepository;

        public OrderController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
      
    }
}
