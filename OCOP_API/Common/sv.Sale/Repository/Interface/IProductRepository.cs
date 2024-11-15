using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public interface IProductRepository
    {
        public int TotalRow { get; set; }
        public Task<List<City>> SearchProduct(string searchString);

    }
}
