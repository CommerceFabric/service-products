using DataAccessLayer.Context;
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
    }
}
