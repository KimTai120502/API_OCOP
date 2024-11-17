using sv.Sale.Context;
using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext dapperContext;
        private readonly SaleContext dbContext;
        public int TotalRows { get; set; }

        public OrderRepository(DapperContext dapperContext, SaleContext dbContext)
        {
            this.dapperContext = dapperContext;
            this.dbContext = dbContext;
        }

        public async Task AddOrder(Order oder, List<OrderDetail> details)
        {

            try
            {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {

                        await this.dbContext.Orders.AddAsync(oder);
                        await this.dbContext.SaveChangesAsync();

                        //Save detail
                        if (details.Count > 0)
                        {
                            foreach (OrderDetail row in details)
                            {
                                row.OrderId = oder.OrderId;
                                row.OrderDetailId = Guid.NewGuid().ToString();
                            }
                            await this.dbContext.OrderDetails.AddRangeAsync(details);
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
    }
}
