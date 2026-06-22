using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DataAccessLayer.RepositoryContracts
{
    /// <summary>
    /// Interface for Products Repository, which defines the contract for data access operations related to products.
    /// </summary>
    public interface IProductsRepository
    {
        /// <summary>
        /// Asynchronously retrieves a list of products from the data source.
        /// </summary>
        /// <returns>A list of products from the data source.</returns>
        Task<IEnumerable<Product?>> GetProducts();

        /// <summary>
        /// Asynchronously retrieves a list of products that satisfy the specified condition from the data source.
        /// </summary>
        /// <param name="conditionExpression">The condition to filter the products.</param>
        /// <returns>A list of products that satisfy the specified condition.</returns>
        Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);

        /// <summary>
        /// Asynchronously retrieves a single product that satisfies the specified condition from the data source.
        /// </summary>
        /// <param name="conditionExpression">The condition to filter the product.</param>
        /// <returns>A single product that satisfies the specified condition.</returns>
        Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);

        /// <summary>
        /// Asynchronously adds a new product to the data source and returns the added product.
        /// </summary>
        /// <param name="product">The product to add to the data source.</param>
        /// <returns>The added product.</returns>
        Task<Product?> AddProduct(Product product);

        /// <summary>
        /// Asynchronously updates an existing product in the data source and returns the updated product.
        /// </summary>
        /// <param name="product">The product to update in the data source.</param>
        /// <returns>The updated product.</returns>
        Task<Product?> UpdateProduct(Product product);

        /// <summary>
        /// Asynchronously deletes a product from the data source and returns a boolean indicating whether the deletion was successful.
        /// </summary>
        /// <param name="product">The product to delete from the data source.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteProduct(Guid productID);

    }
}
