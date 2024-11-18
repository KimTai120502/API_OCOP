using Dapper;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<ProductImage>> GetProductImg(string productID)
        {
            DynamicParameters param = new DynamicParameters();
            var query = "SELECT * FROM dbo.[Product_Image] With(nolock) Where productID = @productID";

            param.Add("productID", productID);
            using (var connection = this.dapperContext.CreateConnection())
            {
                return (await connection.QueryAsync<ProductImage>(query, param)).ToList();
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

        public async Task AddProduct(Product product, List<ProductImage> imgList)
        {

            try
            {
                try
                {
                    using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {

                            await this.dbContext.Products.AddAsync(product);
                            await this.dbContext.SaveChangesAsync();

                            int sortOrder = 1;
                            //Save detail
                            if (imgList.Count > 0)
                            {
                                foreach (ProductImage row in imgList)
                                {
                                    row.ProductId = product.ProductId;
                                    row.ImageId = Guid.NewGuid().ToString();
                                }
                                await this.dbContext.ProductImages.AddRangeAsync(imgList);
                            }

                            await this.dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateProduct(Product product)
        {

            try
            {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        this.dbContext.Entry(product).State = EntityState.Modified;
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

        public async Task AddProductImg(ProductImage img)
        {

            try
            {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await this.dbContext.ProductImages.AddAsync(img);
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

        public async Task AddRating(Rating rating)
        {

            try
            {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await this.dbContext.Ratings.AddAsync(rating);
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

        public async Task<List<Rating>> getRatingListByProductID(string productID)
        {
            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder("WHERE productID = @productID");
            param.Add("productID", productID);

            string sqlQuery = @"SELECT COUNT(1) FROM dbo.[Rating] With(nolock) " + sqlWhere +
                                  @";SELECT * FROM dbo.[Rating] With(nolock) " + sqlWhere;
            using (var connection = this.dapperContext.CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                {
                    this.TotalRows = (await multi.ReadAsync<int>()).Single();
                    List<Rating> rating = (await multi.ReadAsync<Rating>()).ToList();
                    return rating;

                }
            }
        }
    }
}
