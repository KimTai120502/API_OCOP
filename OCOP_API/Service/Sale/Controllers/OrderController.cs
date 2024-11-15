using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<ActionResult> AddNewProduct([FromBody] Dictionary<string, object> dicData)
        {
            Guid productID = Guid.NewGuid();
            Guid productCategoryID = Guid.NewGuid();
            string productName = "Test SP 1";
            List<City> citys = await this.productRepository.SearchProduct("a");
            var data = new
            {
                Citys = citys,
                ProductID = productID,
                ProductCategoryID = productCategoryID,
            };
            return Ok(data);
        }
    }
}
