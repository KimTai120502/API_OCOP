using sv.Sale.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale
{
    public interface ICartRepository
    {
        public int TotalRows { get; set; }

        public Task AddProductToCart(Cart cartDetail);
        public Task UpdateCart(List<Cart> carts, string userID);
        public Task DeleteProductFromCart(string cartID);
        public Task<List<Cart>> GetCartList(string userID);
    }
}
