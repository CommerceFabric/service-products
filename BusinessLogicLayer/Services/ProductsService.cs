using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BusinessLogicLayer.Services
{
    public class ProductsService : IProductsService
    {
        //dependencies
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;

        public ProductsService(IMapper mapper, IProductsRepository productsRepository)
        {
            _mapper = mapper;
            _productsRepository = productsRepository;
        }

        public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
        {
            var product = _mapper.Map<Product>(productAddRequest);
            var addedProduct = await _productsRepository.AddProduct(product);
            var productResponse = _mapper.Map<ProductResponse>(addedProduct);
            return productResponse;
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            return await _productsRepository.DeleteProduct(productID);
        }

        public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> condition)
        {
            var product = await _productsRepository.GetProductByCondition(condition);
            var productResponse = _mapper.Map<ProductResponse>(product);
            return productResponse;
        }

        public async Task<List<ProductResponse?>> GetProducts()
        {
            var products = await _productsRepository.GetProducts();

            return products
                .Select(_mapper.Map<ProductResponse?>)
                .ToList();
        }

        public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> condition)
        {
            var products = await _productsRepository.GetProductsByCondition(condition);
            return products
                .Select(_mapper.Map<ProductResponse?>)
                .ToList();
        }

        public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
        {
            var product = _mapper.Map<Product>(productUpdateRequest);
            var updatedProduct = await _productsRepository.UpdateProduct(product);
            var productResponse = _mapper.Map<ProductResponse>(updatedProduct);
            return productResponse;
        }
    }
}
