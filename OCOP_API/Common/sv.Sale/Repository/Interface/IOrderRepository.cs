using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public interface IOrderRepository
    {
        public int TotalRows { get; set; }

        public Task AddOrder(Order oder, List<OrderDetail> details);
    }
}
