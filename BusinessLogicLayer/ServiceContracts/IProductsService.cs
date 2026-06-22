using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.ServiceContracts
{
    /// <summary>
    /// Interface for the ProductsService, defining the contract for product-related operations.
    /// </summary>
    public interface IProductsService
    {
        /// <summary>
        /// Retrieves a list of products from the data source.
        /// </summary>
        /// <returns></returns>
        Task<List<ProductResponse?>> GetProducts();

        /// <summary>
        /// Retrieves a list of products that satisfy the specified condition.
        /// </summary>
        /// <param name="condition">A function that defines the condition to filter products.</param>
        /// <returns>A list of products that meet the specified condition.</returns>
        Task<List<ProductResponse?>> GetProductsByCondition(Func<Product, bool> condition);

        /// <summary>
        /// Retrieves a single product that satisfies the specified condition.
        /// </summary>
        /// <param name="condition">A function that defines the condition to filter products.</param>
        /// <returns>A product that meets the specified condition, or null if no such product exists.</returns>
        Task<ProductResponse?> GetProductByCondition(Func<Product, bool> condition);
        Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest);
        Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest);
        Task<bool> DeleteProduct(Func<Product, bool> condition);
    }
}
