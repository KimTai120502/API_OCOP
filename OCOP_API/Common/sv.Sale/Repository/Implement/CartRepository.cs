using Dapper;
using Microsoft.EntityFrameworkCore;
using sv.Sale.Context;
using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public class CartRepository : ICartRepository
    {
        private readonly DapperContext dapperContext;
        private readonly SaleContext dbContext;
        public int TotalRows { get; set; }

        public CartRepository(DapperContext dapperContext, SaleContext dbContext)
        {
            this.dapperContext = dapperContext;
            this.dbContext = dbContext;
        }

        public async Task AddProductToCart(Cart cartDetail)
        {
            try
            {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        List<Cart> details =  this.dbContext.Carts.Where(x => x.UserId == cartDetail.UserId && x.ProductId == cartDetail.ProductId).ToList();
                        if (details.Any())
                        {
                            details[0].Quantity = cartDetail.Quantity;
                            this.dbContext.Entry(details[0]).State = EntityState.Modified;
                            await this.dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return;
                        }
                        else
                        {
                            await this.dbContext.Carts.AddAsync(cartDetail);
                            await this.dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }                       
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

        public async Task UpdateCart(List<Cart> carts,string userID)
        {
            try
            {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        this.dbContext.Carts.RemoveRange(this.dbContext.Carts.Where(x => x.UserId == userID));
                        await this.dbContext.SaveChangesAsync();

                        await this.dbContext.Carts.AddRangeAsync(carts);
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

        public async Task DeleteProductFromCart(string cartID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                string sqlQuery = "DELETE [dbo].[Cart] where cartID = @cartID";
                param.Add("cartID", cartID);
                using (var connection = this.dapperContext.CreateConnection())
                {
                    int textData = await connection.ExecuteAsync(sqlQuery, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Cart>> GetCartList(string userID)
        {
            DynamicParameters param = new DynamicParameters();
            var sqlWhere = "Where userID = @userID";
            param.Add("userID", userID);

            string sqlQuery = @"SELECT COUNT(1) FROM dbo.[Cart] With(nolock) " + sqlWhere +
                                  @";SELECT * FROM dbo.[Cart] With(nolock) " + sqlWhere;
            using (var connection = this.dapperContext.CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                {
                    this.TotalRows = (await multi.ReadAsync<int>()).Single();
                    List<Cart> carts = (await multi.ReadAsync<Cart>()).ToList();
                    return carts;

                }
            }

        }
    }
}
