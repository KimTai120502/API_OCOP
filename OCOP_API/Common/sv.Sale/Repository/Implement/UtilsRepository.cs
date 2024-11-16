using Dapper;
using Microsoft.EntityFrameworkCore;
using sv.Sale.Context;
using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public class UtilsRepository : IUtilsRepository
    {
        private readonly DapperContext dapperContext;
        private readonly SaleContext dbContext;
        public int TotalRow { get; set; }

        public UtilsRepository(DapperContext dapperContext, SaleContext dbContext)
        {
            this.dapperContext = dapperContext;
            this.dbContext = dbContext;
        }

        public async Task<City> GetCityByID(int cityID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[City] With(nolock) Where cityID = @cityID";

            param.Add("cityID", cityID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                City user = await connection.QuerySingleOrDefaultAsync<City>(query, param);
                return user;
            }
        }
        public async Task<District> GetDistrictByID(int districtID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[District] With(nolock) Where districtID = @districtID";

            param.Add("districtID", districtID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                District user = await connection.QuerySingleOrDefaultAsync<District>(query, param);
                return user;
            }
        }
        public async Task<Ward> GetWardByID(int wardID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[Ward] With(nolock) Where wardID = @wardID";

            param.Add("wardID", wardID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                Ward user = await connection.QuerySingleOrDefaultAsync<Ward>(query, param);
                return user;
            }
        }

        public async Task<bool> AddNewAddress(UserAddress address)
        {
            try
            {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await this.dbContext.UserAddresses.AddAsync(address);
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

        public async Task<UserAddress> GetAddressByID(string addressID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[User_Address] With(nolock) Where addressID = @addressID";

            param.Add("addressID", addressID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                UserAddress address = await connection.QuerySingleOrDefaultAsync<UserAddress>(query, param);
                return address;
            }
        }

        public async Task DeleteAddress(string addressID)
        {
            try
            {
      
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        this.dbContext.UserAddresses.RemoveRange(this.dbContext.UserAddresses.Where(x => x.AddressId == addressID));
                        await this.dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();     
                    }
                    catch (Exception ex)
                    {
                        await transaction.DisposeAsync();
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateAddress(UserAddress address)
        {
            try
            {
                using (var transaction = this.dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        this.dbContext.Entry(address).State = EntityState.Modified;
                        await this.dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
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
                throw ex;
            }
        }

        public async Task<List<UserAddress>> GetUserAddressList(string userID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[User_Address] With(nolock) Where userID = @userID";

            param.Add("userID", userID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                return (await connection.QueryAsync<UserAddress>(query, param)).ToList();
            }
        }

    }
}
