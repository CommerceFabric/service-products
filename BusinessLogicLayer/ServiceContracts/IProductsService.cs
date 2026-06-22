using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> condition);

        /// <summary>
        /// Retrieves a single product that satisfies the specified condition.
        /// </summary>
        /// <param name="condition">A function that defines the condition to filter products.</param>
        /// <returns>A product that meets the specified condition, or null if no such product exists.</returns>
        Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> condition);

        /// <summary>
        /// Adds a new product to the data source based on the provided productAddRequest.
        /// </summary>
        /// <param name="productAddRequest">The request object containing the details of the product to be added.</param>
        /// <returns>The added product's response object, or null if the addition failed.</returns>
        Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest);

        /// <summary>
        /// Updates an existing product in the data source based on the provided productUpdateRequest.
        /// </summary>
        /// <param name="productUpdateRequest">The request object containing the details of the product to be updated.</param>
        /// <returns>The updated product's response object, or null if the update failed.</returns>
        Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest);

        /// <summary>
        /// Deletes a product from the data source based on the provided productID.
        /// </summary>
        /// <param name="productID">The unique identifier of the product to be deleted.</param>
        /// <returns>True if the product was successfully deleted, otherwise false.</returns>
        Task<bool> DeleteProduct(Guid productID);
    }
}
