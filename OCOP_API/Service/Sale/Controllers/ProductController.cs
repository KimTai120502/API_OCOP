using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Models.Response;
using sv.Sale.DBModels;
using sv.Sale;
using Newtonsoft.Json;

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
                List<ProductImage> image = await this.productRepository.GetProductImg(productID);

                repData = await ResponseSucceeded();
                repData.data = new { Product = product , ProductImage = image };
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

        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] Dictionary<string, object> dicData)
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

                string storeID = dicData["storeID"].ToString();
                string productCategoryID = dicData["productCategoryID"].ToString();
                string productName = dicData["productName"].ToString();
                string description = dicData["description"].ToString();
                string subDescription = dicData["subDescription"].ToString();               
                string videoLink = dicData["videoLink"].ToString();
                string mainImage = dicData["mainImage"].ToString();
                int price = Int32.Parse(dicData["price"].ToString());

                List<ProductImage> imgList = JsonConvert.DeserializeObject<List<ProductImage>>(dicData["productImageList"].ToString());

                Product product = new Product();
                product.ProductId = Guid.NewGuid().ToString();
                product.ProductCategoryId = productCategoryID;
                product.ProductName = productName;
                product.Description = description;
                product.SubDescription = subDescription;
                product.VideoLink = videoLink;
                product.MainImage = mainImage;
                product.QuantitySold = 0;
                product.Price = price;
                product.Rating = 0;
                product.StoreId = storeID;
                product.SearchString = ReplaceUnicode(productName);

                await this.productRepository.AddProduct(product, imgList);
                /*foreach (ProductImage img in imgList)
                {
                    img.ProductId = product.ProductId;
                    img.ImageId = Guid.NewGuid().ToString();
                    await this.productRepository.AddProductImg(img);
                }     */          
                repData = await ResponseSucceeded();

                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddRating([FromBody] Dictionary<string, object> dicData)
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
                string customerID = dicData["customerID"].ToString();
                string comment = dicData["comment"].ToString();
                string mediaLink = dicData["mediaLink"].ToString();
                int star = Int32.Parse(dicData["star"].ToString());

                Rating rating = new Rating();
                rating.Star = star > 5 ? 5 : star;
                rating.ProductId = productID;
                rating.CustomerId = customerID;
                rating.Comment = comment;
                rating.MediaLink = mediaLink;
                rating.RatingId = Guid.NewGuid().ToString();

                await this.productRepository.AddRating(rating);

                repData = await ResponseSucceeded();

                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetRatingProduct([FromBody] Dictionary<string, object> dicData)
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



                List<Rating> data = await this.productRepository.getRatingListByProductID(productID);    
                repData = await ResponseSucceeded();
                repData.data = new { RatingList =  data, TotalRow = this.productRepository.TotalRows };
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
