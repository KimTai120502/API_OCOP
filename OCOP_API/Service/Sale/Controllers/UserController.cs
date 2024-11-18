using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sv.Sale.DBModels;
using sv.Sale;
using System;
using Sale.Models.Response;
using Newtonsoft.Json;

namespace Sale.Controllers
{
    [Route("~/user/[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IProductRepository productRepository;
        private readonly IUserRepository userRepository;
        private readonly IUtilsRepository utilsRepository;

        public UserController(IProductRepository productRepository,IUserRepository userRepository, IUtilsRepository utilsRepository)
        {
            this.productRepository = productRepository;
            this.userRepository = userRepository;
            this.utilsRepository = utilsRepository;
        }

        [HttpPost]
        public async Task<ActionResult> SignUp([FromBody] Dictionary<string, object> dicData)
        {
            try
            {

                ResponseModel repData = await ResponseFail();
                if (dicData == null)
                {
                    repData.message = "Dữ liệu đầu vào null";
                    return Ok(repData);
                }

                string userName = dicData["userName"].ToString();
                string password = dicData["passWord"].ToString();
                string fullName = dicData["fullName"].ToString();
                string phone = dicData["phone"].ToString();
                string? email = dicData["email"].ToString();   

                User newUser = new User();
                newUser.UserName = userName;
                newUser.PassWord = password;
                newUser.FullName = fullName;
                newUser.Phone = phone;
                newUser.Email = email;

                bool phoneExisted = await this.userRepository.CheckUserExistsByPhone(newUser.Phone);
                if (phoneExisted) 
                { 
                    repData = await ResponseFail();
                    repData.message = "Số điện thoại đã tạo 1 tài khoản khác";
                    return Ok(repData);
                }

                bool userNameExisted = await this.userRepository.CheckUserExistsByUserName(newUser.UserName);
                if (userNameExisted)
                {
                    repData = await ResponseFail();
                    repData.message = "Tên đăng nhập đã tồn tại";
                    return Ok(repData);
                }

                newUser.UserId = Guid.NewGuid().ToString();
                newUser.Token = newUser.UserId;
                newUser.CreatedAt = DateTime.Now;
                newUser.IsBaned = false;
                newUser.ShipAddressId = Guid.Empty.ToString();
                newUser.PassWord = await this.utilsRepository.HashPassword(newUser.PassWord);

                bool result = await this.userRepository.AddNewUser(newUser);
                if (!result) 
                { 
                    repData = await ResponseFail();
                    repData.message = "Tạo tài khoản thất bại";
                    return Ok(repData);
                }


                repData = await ResponseSucceeded();
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                
                ResponseModel repData = await ResponseFail();
                if (dicData == null)
                {
                    repData.message = "Dữ liệu đầu vào null";
                    return Ok(repData);
                }

                string userName = dicData["userName"].ToString();
                string password = dicData["passWord"].ToString();

                User customer = await this.userRepository.GetUser(userName);
                if (customer is null)
                {
                    repData.message = "Tài khoản không tồn tại";
                    return Ok(repData);
                }
           
                bool verifyPassword = await this.utilsRepository.VerifyPassword(password, customer.PassWord);
                if (!verifyPassword)
                {
                    repData.message = "Mật khẩu không đúng";
                    return Ok(repData);
                }

                if (customer.IsBaned == true)
                {
                    repData.message = "Tài khoản đã bị khóa bởi Quản Trị Viên";
                    return Ok(repData);
                }

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

                repData = await ResponseSucceeded();
                repData.data = new
                {
                    token = customer.UserId,
                    fullName = customer.FullName
                };

                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }                   
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword([FromBody] Dictionary<string, object> dicData)
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

                Guid id = Guid.Parse(token);
                string oldPassword = dicData["oldPassWord"].ToString();
                string newPassword = dicData["newPassWord"].ToString();

                User user = await this.userRepository.GetUserByID(id);
                if (user is null)
                {
                    repData.message = "Tài khoản không tồn tại";
                    return Ok(repData);
                }

                //string password = await this.userRepository.CreateMD5(Password);
                if (user.PassWord != oldPassword)
                {
                    repData.message = "Mật khẩu cũ không đúng";
                    return Ok(repData);
                }
                user.PassWord = newPassword;
                bool result = await this.userRepository.UpdateUser(user);
                if (!result)
                {
                    repData.message = "Đổi mật khẩu không thành công";
                    return Ok(repData);
                }

                repData = await ResponseSucceeded();

                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateInfo([FromBody] Dictionary<string, object> dicData)
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

                Guid id = Guid.Parse(token);
               
                string fullName = dicData["fullName"].ToString();
                string phone = dicData["phone"].ToString();
                string email = dicData["email"].ToString();
                string shipAddressID = dicData["shipAddressID"].ToString();
                string shipAddress = dicData["shipAddress"].ToString();

                User user = await this.userRepository.GetUserByID(id);
                if (user is null)
                {
                    repData.message = "Tài khoản không tồn tại";
                    return Ok(repData);
                }

                user.FullName = fullName;
                user.Phone = phone;
                user.Email = email;
                user.ShipAddressId = shipAddressID;
                user.ShipAddress = shipAddress;
                    
                bool result = await this.userRepository.UpdateUser(user);
                if (!result)
                {
                    repData.message = "Cập nhật thông tin tài khoản không thành công";
                    return Ok(repData);
                }

                repData = await ResponseSucceeded();

                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNewShipAddress([FromBody] Dictionary<string, object> dicData)
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

                string shortAddress = dicData["shortAddress"].ToString();
                int cityID = Int32.Parse(dicData["cityID"].ToString());
                int districtID = Int32.Parse(dicData["districtID"].ToString());
                int wardID = Int32.Parse(dicData["wardID"].ToString());

                City city = await this.utilsRepository.GetCityByID(cityID);
                District district = await this.utilsRepository.GetDistrictByID(districtID);
                Ward ward = await this.utilsRepository.GetWardByID(wardID);

                string fullAddress = city.CityName + ", " + district.DistrictName + ", " + ward.WardName  + ", " + shortAddress;

                UserAddress address = new UserAddress();
                address.AddressId = Guid.NewGuid().ToString();
                address.ShortAddress = shortAddress;
                address.CityId = city.CityId;
                address.DistrictId = district.DistrictId;
                address.WardId = ward.WardId;
                address.FullAddress = fullAddress;
                address.UserId = id;
                
                bool result = await this.utilsRepository.AddNewAddress(address);

                if (!result)
                {
                    repData.message = "Thêm địa chỉ không thành công";
                    return Ok(repData);
                }

                repData = await ResponseSucceeded();

                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetAddress([FromBody] Dictionary<string, object> dicData)
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
                string AddressId = dicData["addressID"].ToString();
               
                UserAddress address = await this.utilsRepository.GetAddressByID(AddressId);

                repData = await ResponseSucceeded();
                repData.data = new { Address = address };
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAddress([FromBody] Dictionary<string, object> dicData)
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
                string AddressId = dicData["addressID"].ToString();

                await this.utilsRepository.DeleteAddress(AddressId);

                repData = await ResponseSucceeded();
                repData.data = new {};
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateShipAddress([FromBody] Dictionary<string, object> dicData)
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

                UserAddress address = JsonConvert.DeserializeObject<UserAddress>(dicData["UserAddress"].ToString());

                City city = await this.utilsRepository.GetCityByID(address.CityId);
                District district = await this.utilsRepository.GetDistrictByID(address.DistrictId);
                Ward ward = await this.utilsRepository.GetWardByID(address.WardId);

                address.FullAddress  = city.CityName + ", " + district.DistrictName + ", " + ward.WardName + ", " + address.ShortAddress;

                await this.utilsRepository.UpdateAddress(address);

                repData = await ResponseSucceeded();

                return Ok(repData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetUserAddressList([FromBody] Dictionary<string, object> dicData)
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
                string userId = token;
                //string AddressId = dicData["addressID"].ToString();

                List<UserAddress> addressList = await this.utilsRepository.GetUserAddressList(userId);

                repData = await ResponseSucceeded();
                repData.data = new { AddressList = addressList };
                return Ok(repData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SignUpStore([FromBody] Dictionary<string, object> dicData)
        {
            try
            {

                ResponseModel repData = await ResponseFail();
                if (dicData == null)
                {
                    repData.message = "Dữ liệu đầu vào null";
                    return Ok(repData);
                }

                string storeName = dicData["storeName"].ToString();
                string storeLogo = dicData["storeLogo"].ToString();
                string description = dicData["description"].ToString();
                string phone = dicData["phone"].ToString();
                string? email = dicData["email"].ToString();
                int cityID = Int32.Parse(dicData["cityID"].ToString());
                int districtID = Int32.Parse(dicData["districtID"].ToString());
                int wardID = Int32.Parse(dicData["wardID"].ToString());
                string shortAddress = dicData["shortAddress"].ToString();
                string accountName = dicData["accountName"].ToString();
                string password = dicData["password"].ToString();

                bool accountNameExisted = await this.userRepository.CheckStoreExistsByAccountName(accountName);
                if (accountNameExisted)
                {
                    repData = await ResponseFail();
                    repData.message = "Tên đăng nhập đã tồn tại";
                    return Ok(repData);
                }

                Store newStore = new Store();
                newStore.StoreId = Guid.NewGuid().ToString();
                newStore.StoreName = storeName;
                newStore.StoreLogo = storeLogo;
                newStore.Phone = phone;
                newStore.Email = email;
                newStore.CityId = cityID;
                newStore.DistrictId = districtID;
                newStore.WardId = wardID;
                newStore.ShortAddress = shortAddress;
                newStore.Password = password;
                newStore.AccountName = accountName;
                newStore.Wallet = 0;
                newStore.IsBaned = false;

                City city = await this.utilsRepository.GetCityByID(cityID);
                District district = await this.utilsRepository.GetDistrictByID(districtID);
                Ward ward = await this.utilsRepository.GetWardByID(wardID);

                newStore.FullAddress = city.CityName + ", " + district.DistrictName + ", " + ward.WardName + ", " + shortAddress;


                newStore.Password = await this.utilsRepository.HashPassword(newStore.Password);

                bool result = await this.userRepository.AddNewStore(newStore);
                if (!result)
                {
                    repData = await ResponseFail();
                    repData.message = "Tạo tài khoản thất bại";
                    return Ok(repData);
                }


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
