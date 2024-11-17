using Dapper;
using sv.Sale.Context;
using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public class StoreRepository : IStoreRepository
    {
        private readonly DapperContext dapperContext;
        private readonly SaleContext dbContext;
        public int TotalRows { get; set; }

        public StoreRepository(DapperContext dapperContext, SaleContext dbContext)
        {
            this.dapperContext = dapperContext;
            this.dbContext = dbContext;
        }

        public async Task<List<Store>> GetStoreList()
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[Store] With(nolock) WHERE isBaned = 0";

            using (var connection = this.dapperContext.CreateConnection())
            {
                return (await connection.QueryAsync<Store>(query, param)).ToList();
            }
        }
        public async Task<Store> GetStoreByID(string storeID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[Product] With(nolock) Where storeID = @storeID";

            param.Add("storeID", storeID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Store>(query, param);
            }
        }
    }
}
