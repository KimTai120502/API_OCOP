using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public int TotalRows { get; set; }

        public ProductRepository(DapperContext dapperContext, SaleContext dbContext)
        {
            this.dapperContext = dapperContext;
            this.dbContext = dbContext;
        }

        public async Task<Product> GetProductByID(string productID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[Product] With(nolock) Where productID = @productID";

            param.Add("productID", productID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Product>(query, param);
            }
        }
        public async Task<List<Product>> SearchProduct(string searchString,string storeID,string productCategoryID)
        {
            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder("WHERE searchString LIKE @searchString");
            param.Add("searchString", "%" + searchString + "%");

            if (!string.IsNullOrEmpty(storeID))
            {
                sqlWhere.Append(" AND storeID = @storeID");
                param.Add("storeID", storeID);
            }
            if (!string.IsNullOrEmpty(productCategoryID))
            {
                sqlWhere.Append(" AND productCategoryID = @productCategoryID");
                param.Add("productCategoryID", productCategoryID);
            }

            string sqlQuery = @"SELECT COUNT(1) FROM dbo.[Product] With(nolock) " + sqlWhere +
                                  @";SELECT * FROM dbo.[Product] With(nolock) " + sqlWhere;
            using (var connection = this.dapperContext.CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                {
                    this.TotalRows = (await multi.ReadAsync<int>()).Single();
                    List<Product> products = (await multi.ReadAsync<Product>()).ToList();
                    return products;

                }
            }
        }
    }
}
