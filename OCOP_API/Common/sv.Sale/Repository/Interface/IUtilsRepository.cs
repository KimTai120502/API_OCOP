using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public interface IUtilsRepository
    {
        public Task<City> GetCityByID(int cityID);
        public Task<District> GetDistrictByID(int districtID);
        public Task<Ward> GetWardByID(int wardID);
        public Task<bool> AddNewAddress(UserAddress address);
        public Task<UserAddress> GetAddressByID(string addressID);
        public Task DeleteAddress(string addressID);
        public Task UpdateAddress(UserAddress address);
        public Task<List<UserAddress>> GetUserAddressList(string userID);
    }
}
