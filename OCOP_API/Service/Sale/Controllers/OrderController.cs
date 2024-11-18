using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly IOrderRepository orderRepository;
        private readonly IUserRepository userRepository;

        public OrderController(IProductRepository productRepository, IOrderRepository orderRepository, IUserRepository userRepository)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult> AddOrder([FromBody] Dictionary<string, object> dicData)
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

                /*string storeID = dicData["storeID"].ToString();
                string productCategoryID = dicData["productCategoryID"].ToString();
                string productName = dicData["productName"].ToString();
                string description = dicData["description"].ToString();
                string subDescription = dicData["subDescription"].ToString();
                string videoLink = dicData["videoLink"].ToString();
                string mainImage = dicData["mainImage"].ToString();
                int price = Int32.Parse(dicData["price"].ToString());*/
                string notes = dicData["notes"].ToString();
                string shipAddress = dicData["shipAddress"].ToString();
                Voucher voucher = JsonConvert.DeserializeObject<Voucher>(dicData["voucher"].ToString());
                Unit unit = JsonConvert.DeserializeObject<Unit>(dicData["unit"].ToString());
                List<Cart> carts = JsonConvert.DeserializeObject<List<Cart>>(dicData["cart"].ToString());

                List<OrderDetail> orderDetailList = new List<OrderDetail>();

                Order order = new Order();
                order.OrderId = Guid.NewGuid().ToString();

                int amount = 0;
                double discountAmount = 0;
                foreach (Cart cart in carts)
                {
                    Product product = await this.productRepository.GetProductByID(cart.ProductId);
                    product.QuantitySold += cart.Quantity.Value;
                    await this.productRepository.UpdateProduct(product);
                    double productDiscount;
                    if (product.IsDiscountPercent.GetValueOrDefault())
                    {
                        productDiscount = product.Price * (product.DiscountAmount.Value/100);
                    }
                    else
                    {
                        productDiscount = product.DiscountAmount == null? 0: product.DiscountAmount.Value;
                    }
                    discountAmount += productDiscount;
                    amount = amount + (cart.Quantity.Value * product.Price);

                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderDetailId = Guid.NewGuid().ToString();
                    orderDetail.ProductId = cart.ProductId;
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.Price = product.Price;
                    orderDetail.Quantity = cart.Quantity;
                    orderDetail.UnitId = unit.UnitId;
                    orderDetail.UnitName = unit.UnitName;
                    orderDetail.DiscountValue = productDiscount;
                    orderDetail.TotalAmount = (int)((orderDetail.Price * orderDetail.Quantity) - orderDetail.DiscountValue);

                    orderDetailList.Add(orderDetail);

                }

                User customer = await this.userRepository.GetUserByID(Guid.Parse(carts[0].UserId));

                //order.Status: 0:hủy, 1:mới tạo, 2 đã thanh toán,
                order.CreatedDate = DateTime.Now;
                order.Amount = amount;
                order.DiscountAmount = (int) discountAmount;
                order.TotalAmount = order.Amount - order.DiscountAmount;
                order.Status = 1;
                order.CustomerId = customer.UserId;
                order.CustomerPhone = customer.Phone;
                order.CustomerName = customer.FullName;
                order.Notes = notes;
                order.ShipAddress = shipAddress;
                order.VoucherId = voucher.VoucherId;
                order.VoucherType = voucher.VoucherType.ToString();
                order.VoucherValue = voucher.VoucherValue;
                order.ShippingStatus = 1;
                order.EstimatedShippingDate = DateTime.Now.AddDays(3);
                order.Shippingfee = 30000;

                await this.orderRepository.AddOrder(order, orderDetailList);

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
