using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public interface IUserRepository
    {
        public Task<User> GetUser(string userName);
        public Task<User> GetUserByID(Guid id);
        public Task<bool> AddNewUser(User newUser);
        public Task<bool> UpdateUser(User newUser);
        public Task<bool> CheckUserExistsByPhone(string phone);

        public Task<bool> CheckUserExistsByUserName(string userName);
        public Task<bool> AddNewStore(Store newStore);
        public Task<bool> CheckStoreExistsByAccountName(string accountName);

        #region cryptor
        public Task<string> DecryptString(string encrString);
        public Task<string> EnryptString(string strEncrypted);
        public Task<string> CreateGUIID(int siteId, string storeId, DateTime createdDate, string keyEncrypt);
        public Task<string> DecryptGUIID(string guiId);
        public Task<string> CreateMD5(string input);
        #endregion

    }
}
