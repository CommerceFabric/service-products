using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CommerceFabric.ProductService.API.ApiEndpoints
{
    public static class ProductApiEndpoints
    {
        // extension method to map product related endpoints
        public static IEndpointRouteBuilder MapProductApiEndpoints(this IEndpointRouteBuilder app)
        {
            // get all products endpoint
            app.MapGet("/api/products", 
                async(IProductsService productService) =>
                {
                    var products = await productService.GetProducts();
                    return Results.Ok(products);
                });

            #region Product(s) Search Endpoints
            // get product by id endpoint
            app.MapGet("/api/products/search/product-id/{productId:guid}",
                async (Guid productId, [FromServices] IProductsService productService) =>
                {
                    var product = await productService.GetProductByCondition(p => p.ProductID == productId);
                    if (product == null)
                    {
                        return Results.NotFound();
                    }
                    return Results.Ok(product);
                });

            // get products by category endpoint
            app.MapGet("/api/products/search/category/{category}",
                async (string category, [FromServices] IProductsService productService) =>
                {
                    var products = await productService.GetProductsByCondition(p => p.Category != null && p.Category.ToString().Equals(category, StringComparison.OrdinalIgnoreCase));
                    return Results.Ok(products);
                });

            // get products by search query (searches in product name) endpoint
            app.MapGet("/api/products/search",
                async (string query, [FromServices] IProductsService productService) =>
                {
                    var products = await productService.GetProductsByCondition(p => p.ProductName != null && p.ProductName.Contains(query, StringComparison.OrdinalIgnoreCase));



                    return Results.Ok(products);
                });
            #endregion

            // add new product endpoint
            app.MapPost("/api/products",
                async (ProductAddRequest productAddRequest, [FromServices] IProductsService productService, [FromServices] ProductAddRequestValidator productAddRequestValidator) =>
                {
                    #region validate the request
                    if (productAddRequest == null)
                    {
                        throw new ArgumentNullException(nameof(productAddRequest));
                    }

                    //Validate the product using Fluent Validation
                    ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);

                    // Check the validation result
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray()
                            );
                        return Results.ValidationProblem(errors);
                    }
                    #endregion

                    // perform the add product operation
                    var addedProduct = await productService.AddProduct(productAddRequest);

                    if (addedProduct == null || addedProduct.ProductID == Guid.Empty) return Results.BadRequest("Failed to add product.");

                    // return the created product with a 201 Created response
                    return Results.Created($"/api/products/search/product-id/{addedProduct.ProductID}", addedProduct);
                });

            // update existing product endpoint
            app.MapPut("/api/products",
                async (ProductUpdateRequest productUpdateRequest, [FromServices] IProductsService productService, [FromServices] ProductUpdateRequestValidator productUpdateRequestValidator) =>
                {
                    #region validate the request
                    if (productUpdateRequest == null)
                    {
                        throw new ArgumentNullException(nameof(productUpdateRequest));
                    }

                    //Validate the product using Fluent Validation
                    ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

                    // Check the validation result
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray()
                            );
                        return Results.ValidationProblem(errors);
                    }
                    #endregion

                    // perform the update product operation
                    var updatedProduct = await productService.UpdateProduct(productUpdateRequest);

                    if (updatedProduct == null || updatedProduct.ProductID == Guid.Empty) return Results.BadRequest("Failed to update product.");

                    return Results.Ok(updatedProduct);
                });

            // delete product endpoint
            app.MapDelete("/api/products/{productId:guid}",
                async (Guid productId, [FromServices] IProductsService productService) =>
                {
                    var isDeleted = await productService.DeleteProduct(productId);
                    if (!isDeleted) return Results.NotFound();
                    
                    return Results.NoContent();
                });


            return app;
        }
    }
}
