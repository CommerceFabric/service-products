using DataAccessLayer.Context;
using DataAccessLayer.Domain;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DataAccessLayer.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        // dependencies
        private readonly ApplicationDbContext _dbContext;

        public ProductsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Product?> AddProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            // find the matching product
            var product = await _dbContext.Products.FindAsync(productID);
            if (product == null) return false;

            // and delete it
            _dbContext.Products.Remove(product);
            var rowsAffected = await _dbContext.SaveChangesAsync();

            return rowsAffected > 0;
        }

        public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(conditionExpression);
        }

        public async Task<IEnumerable<Product?>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            return await _dbContext.Products.Where(conditionExpression).ToListAsync();
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            // find the matching product
            var existingProduct = await _dbContext.Products.FindAsync(product.ProductID);
            if (existingProduct == null) return null;

            // and update its properties
            _dbContext.Entry(existingProduct).CurrentValues.SetValues(product);
            await _dbContext.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> DecreaseProductStock(List<StockReduction> stockReductions)
        {
            // Start a transaction to ensure that all stock updates are atomic
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            // Check if all products have enough stock before making any changes
            foreach (var item in stockReductions)
            {
                // Retrieve the product from the database and check if it has enough stock, if not, rollback the transaction and return false
                var product = await _dbContext.Products.SingleAsync(p => p.ProductID == item.ProductID);

                if (product.QuantityInStock < item.Quantity)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Decrease the stock for the product
                product.QuantityInStock -= item.Quantity;
            }

            // If we reach this point, it means all products have enough stock, so we can save the changes and commit the transaction
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
    }
}
