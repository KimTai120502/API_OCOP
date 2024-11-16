using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Models.Response;
using sv.Sale.DBModels;
using sv.Sale;

namespace Sale.Controllers
{
    [Route("~/sale/[controller]/[action]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IProductRepository productRepository;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpPost]
        public async Task<ActionResult> GetProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {

                ResponseModel repData = await ResponseFail();
                if (dicData == null)
                {
                    repData.message = "Dữ liệu đầu vào null";
                    return Ok(repData);
                }
                string token = Request.Headers["Token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    repData.message = "Token rỗng";
                    return Ok(repData);
                }
                string id = token;
                string productID = dicData["productID"].ToString();

                Product product = await this.productRepository.GetProductByID(productID);

                repData = await ResponseSucceeded();
                repData.data = new { Product = product };
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> QuickSearchProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {

                ResponseModel repData = await ResponseFail();
                if (dicData == null)
                {
                    repData.message = "Dữ liệu đầu vào null";
                    return Ok(repData);
                }
                string token = Request.Headers["Token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    repData.message = "Token rỗng";
                    return Ok(repData);
                }
                string id = token;
                string searchString = dicData["searchString"].ToString();
                string storeID = dicData["storeID"].ToString();
                string productCategoryID = dicData["productCategoryID"].ToString();

                List<Product> products = await this.productRepository.SearchProduct(searchString, storeID, productCategoryID);

                repData = await ResponseSucceeded();
                repData.data = new { ProductList = products };
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
