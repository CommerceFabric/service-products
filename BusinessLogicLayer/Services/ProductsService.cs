using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceBus;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Validators;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class ProductsService : IProductsService
    {
        //dependencies
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;
        private readonly ProductAddRequestValidator _productAddRequestValidator;
        private readonly ProductUpdateRequestValidator _productUpdateRequestValidator;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        private readonly IServiceBusPublisher _serviceBusPublisher;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The constructor for the ProductsService class, which initializes the dependencies for the service.
        /// </summary>
        /// <param name="mapper">Dependency Injected IMapper instance</param>
        /// <param name="productsRepository">Dependency Injected IProductsRepository instance</param>
        /// <param name="productAddRequestValidator">Dependency Injected ProductAddRequestValidator instance - as we use Minimal API, not Controller based API, so we need to manually validate requests</param>
        /// <param name="productUpdateRequestValidator">Dependency Injected ProductUpdateRequestValidator instance - as we use Minimal API, not Controller based API, so we need to manually validate requests</param>
        /// <param name="rabbitMQPublisher">Dependency Injected IRabbitMQPublisher instance</param>
        /// <param name="serviceBusPublisher">Dependency Injected IServiceBusPublisher instance</param>
        /// <param name="configuration">Dependency Injected IConfiguration instance</param>
        public ProductsService(IMapper mapper, IProductsRepository productsRepository, ProductAddRequestValidator productAddRequestValidator, ProductUpdateRequestValidator productUpdateRequestValidator, IRabbitMQPublisher rabbitMQPublisher, IServiceBusPublisher serviceBusPublisher, IConfiguration configuration)
        {
            _mapper = mapper;
            _productsRepository = productsRepository;
            _productAddRequestValidator = productAddRequestValidator;
            _productUpdateRequestValidator = productUpdateRequestValidator;
            _rabbitMQPublisher = rabbitMQPublisher;
            _serviceBusPublisher = serviceBusPublisher;
            _configuration = configuration;
        }

        public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
        {
            #region validate the request
            if (productAddRequest == null)
            {
                throw new ArgumentNullException(nameof(productAddRequest));
            }

            //Validate the product using Fluent Validation
            ValidationResult validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);

            // Check the validation result
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }
            #endregion

            var product = _mapper.Map<Product>(productAddRequest);
            var addedProduct = await _productsRepository.AddProduct(product);
            var productResponse = _mapper.Map<ProductResponse>(addedProduct);
            return productResponse;
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            #region validate the request
            if (productID == Guid.Empty || 
                await _productsRepository.GetProductByCondition(p => p.ProductID == productID) == null) 
                throw new ArgumentException($"Product with ID {productID} does not exist.");
            #endregion

            var productDeleted = await _productsRepository.DeleteProduct(productID);

            // if product successfully deleted, publish message to RabbitMQ
            if(productDeleted)
            {
                var headers = new Dictionary<string, object>
                {
                    { "event", "product.delete" },
                    { "RowCount", 1  }
                };
                var message = new ProductDeleteMessage
                {
                    ProductID = productID
                };
                await _rabbitMQPublisher.Publish(headers, message);
                await _serviceBusPublisher.Publish(_configuration["ProductsServiceBus:ProductDeleteTopic"]!, headers, message);
            }
            
            return productDeleted;
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
            #region validate the request
            if (productUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(productUpdateRequest));
            }

            //Validate the product using Fluent Validation
            ValidationResult validationResult = await _productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

            // Check the validation result
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // as we are doing an update, we need to check if the product exists in the database
            var existingProduct = await _productsRepository.GetProductByCondition(p => p.ProductID == productUpdateRequest.ProductID);
            if (existingProduct == null)
            {
                throw new ArgumentException($"Product with ID {productUpdateRequest.ProductID} does not exist.");
            }
            #endregion


            var product = _mapper.Map<Product>(productUpdateRequest);
            var updatedProduct = await _productsRepository.UpdateProduct(product);
            if(updatedProduct == null)
            {
                throw new Exception($"Failed to update product with ID {productUpdateRequest.ProductID}.");
            }

            // publish a message to RabbitMQ
            var headers = new Dictionary<string, object>
            {
                { "event", "product.update" },
                { "RowCount", 1  }
            };
            
            await _rabbitMQPublisher.Publish(headers, product);
            await _serviceBusPublisher.Publish(_configuration["ProductsServiceBus:ProductUpdateTopic"]!, headers, product);

            var productResponse = _mapper.Map<ProductResponse>(updatedProduct);
            return productResponse;
        }
    }
}
