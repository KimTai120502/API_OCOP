using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sale.Models.Response;
using sv.Sale;
using sv.Sale.DBModels;

namespace Sale.Controllers
{
    [Route("~/cart/[controller]/[action]")]
    [ApiController]
    public class CartController : BaseController
    {
        private readonly ICartRepository cartRepository;
        private readonly IProductRepository productRepository;

        public CartController(ICartRepository cartRepository, IProductRepository productRepository)
        {
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
        }

        [HttpPost]
        public async Task<ActionResult> AddProductToCart([FromBody] Dictionary<string, object> dicData)
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

                string productId = dicData["productID"].ToString();
                int quantity = Int32.Parse(dicData["quantity"].ToString());

                Cart cart = new Cart();
                cart.CartId = Guid.NewGuid().ToString();
                cart.UserId = id;
                cart.ProductId = productId;
                cart.Quantity = quantity;

                List<Cart> carts = await this.cartRepository.GetCartList(id);

                await this.cartRepository.AddProductToCart(cart);

                carts = await this.cartRepository.GetCartList(id);
                repData = await ResponseSucceeded();
                repData.data = new {Cart = carts, TotalRow = this.cartRepository.TotalRows };
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetCartDetail([FromBody] Dictionary<string, object> dicData)
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
                List<Cart> carts = await this.cartRepository.GetCartList(id);
                List<object> CartProduct = new List<object>();
                foreach (Cart cart in carts) 
                { 
                    Product product = await this.productRepository.GetProductByID(cart.ProductId);
                    var productCart = new
                    {
                        productID = product.ProductId,
                        productName = product.ProductName,
                        price = product.Price,
                        isDiscountPercent = product.IsDiscountPercent,
                        discountAmount = product.DiscountAmount,
                        mainImage = product.MainImage,
                        quantity = cart.Quantity
                    };
                    CartProduct.Add(productCart);
                }

                repData = await ResponseSucceeded();
                repData.data = new { Cart = CartProduct, TotalRow = this.cartRepository.TotalRows };
                return Ok(repData);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCart([FromBody] Dictionary<string, object> dicData)
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

                List<Cart> carts = JsonConvert.DeserializeObject<List<Cart>>(dicData["Cart"].ToString());

                await this.cartRepository.UpdateCart(carts, id);
                
                repData = await ResponseSucceeded();
                return Ok(repData);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProductFromCart([FromBody] Dictionary<string, object> dicData)
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

                string cartID = dicData["cartID"].ToString();

                await this.cartRepository.DeleteProductFromCart(cartID);

                repData = await ResponseSucceeded();
                return Ok(repData);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
