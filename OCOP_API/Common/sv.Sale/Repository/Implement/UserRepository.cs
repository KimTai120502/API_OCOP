using Dapper;
using sv.Sale.Context;
using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;

namespace sv.Sale
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext dapperContext;
        private readonly SaleContext dbContext;
        public int TotalRow { get; set; }

        public UserRepository(DapperContext dapperContext, SaleContext dbContext)
        {
            this.dapperContext = dapperContext;
            this.dbContext = dbContext;
        }

       
        public async Task<User> GetUser(string userName)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[User] With(nolock) Where userName = @userName";

            param.Add("userName", userName, DbType.String);
            using (var connection = this.dapperContext.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, param);
                return user;
            }
        }

        public async Task<User> GetUserByID(Guid id)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[User] With(nolock) Where userID = @userID";

            param.Add("userID", id.ToString());
            using (var connection = this.dapperContext.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, param);
                return user;
            }
        }

        public async Task<bool> AddNewUser(User newUser)
        {

            try
            {
                using (var transaction = this.dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        await this.dbContext.Users.AddAsync(newUser);
                        await this.dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public async Task<bool> UpdateUser(User newUser)
        {

            try
            {
                using (var transaction = this.dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        this.dbContext.Entry(newUser).State = EntityState.Modified;
                        await this.dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public async Task<bool> CheckUserExistsByPhone(string phone)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT count(*) FROM dbo.[User] With(nolock) Where Phone = @phone";
            param.Add("phone", phone);

            using (var connection = this.dapperContext.CreateConnection())
            {
                int userExists = await connection.QueryFirstOrDefaultAsync<int>(query, param);
                return userExists <= 0 ? false : true;
            }
        }

        public async Task<bool> CheckUserExistsByUserName(string userName)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT count(*) FROM dbo.[User] With(nolock) Where userName = @userName";
            param.Add("userName", userName);

            using (var connection = this.dapperContext.CreateConnection())
            {
                int userExists = await connection.QueryFirstOrDefaultAsync<int>(query, param);
                return userExists <= 0 ? false : true;
            }
        }
        #region Cryptography Function
        /// <summary>
        /// Descrypt GUIID
        /// </summary>
        /// <param name="encrString"></param>
        /// <returns></returns>
        public async Task<string> DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }

        /// <summary>
        /// Generate GUIID 
        /// </summary>
        /// <param name="strEncrypted"></param>
        /// <returns></returns>
        public async Task<string> EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }

        public async Task<string> CreateGUIID(int siteId, string storeId, DateTime createdDate, string keyEncrypt)
        {
            string str = siteId + ";" + keyEncrypt + ";" + storeId + ";" + createdDate.Ticks;
            return await EnryptString(str);
        }

        public async Task<string> DecryptGUIID(string guiId)
        {
            return await DecryptString(guiId);
        }

        public async Task<string> CreateMD5(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                    return input;
                // Use input string to calculate MD5 hash
                MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
    }
}
