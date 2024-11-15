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
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext dapperContext;
        private readonly SaleContext dbContext;
        public int TotalRow { get; set; }

        public ProductRepository(DapperContext dapperContext, SaleContext dbContext)
        {
            this.dapperContext = dapperContext;
            this.dbContext = dbContext;
        }

        public async Task<List<City>> SearchProduct(string searchString) 
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[City]";

            param.Add("AccessCode", searchString);
            using (var connection = this.dapperContext.CreateConnection())
            {
                List<City> products = (await connection.QueryAsync<City>(query, param)).ToList();
                return products;
            }
        }
    }
}
