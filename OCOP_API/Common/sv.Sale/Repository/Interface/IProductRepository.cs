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
        public int TotalRows { get; set; }

        public Task<Product> GetProductByID(string productID);
        public Task<List<ProductImage>> GetProductImg(string productID);
        public Task<List<Product>> SearchProduct(string searchString, string storeID, string productCategoryID);
        public Task AddProductImg(ProductImage img);
        public Task AddProduct(Product product, List<ProductImage> imgList);
        public Task AddRating(Rating rating);
        public Task<List<Rating>> getRatingListByProductID(string productID);
    }
}
