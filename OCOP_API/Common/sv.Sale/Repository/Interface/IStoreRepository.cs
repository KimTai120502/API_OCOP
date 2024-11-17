using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public interface IStoreRepository
    {
        public int TotalRows { get; set; }

        public Task<List<Store>> GetStoreList();
        public Task<Store> GetStoreByID(string storeID);
    }
}
