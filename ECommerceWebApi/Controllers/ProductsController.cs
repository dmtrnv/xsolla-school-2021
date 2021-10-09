using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProductApi.Data;
using ProductApi.DataTransferObjects;
using ProductApi.Misc;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Produces("application/json")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IRepositoryWrapper repository, 
            IMapper mapper, 
            ILogger<ProductsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET api/products
        /// <summary>
        /// Get all products from database include product's manufacturer, type and subtype.
        /// </summary>
        /// <returns>IEnumerable of ProductReadDto</returns>
        /// <response code="200">Returns existing products</response>
        /// <response code="400">If product parameters cost is not valid (minCost > maxCost)</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProductsAsync([FromQuery]ProductParameters parameters)
        {
            if (!parameters.ValidCost)
            {
                return ValidationProblem("MaxCost < MinCost, should be the other way around");
            }

            var products = (parameters.TypeName == null) ? 
                _repository.Products.GetProductsFilteredByCostWithDetails(parameters) 
                    : 
                _repository.Products.GetProductsFilteredByCostAndTypeNameWithDetails(parameters);

            var productsPaged = await PagedList<Product>.ToPagedListAsync(products, parameters.PageNumber, parameters.PageSize);

            var metadata = new
            {
                productsPaged.TotalCount,
                productsPaged.PageSize,
                productsPaged.CurrentPage,
                productsPaged.TotalPages,
                productsPaged.HasNext,
                productsPaged.HasPrevious
            };

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(metadata));

            _logger.LogInformation("Repository: successfully returned paged list of all products");
            return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(productsPaged));
        }
        
        // GET api/products/{idOrSku}
        /// <summary>
        /// Get product with special id or sku from database include product's manufacturer, type and subtype.
        /// </summary>
        /// <returns>ProductReadDto</returns>
        /// <response code="200">Returns existing product</response>
        /// <response code="404">If product with actual id or sku is not found</response>
        [HttpGet("{idOrSku}", Name = nameof(GetProductByIdOrSkuAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReadDto>> GetProductByIdOrSkuAsync(string idOrSku)
        {
            var product = Guid.TryParse(idOrSku, out var id) ?
                await _repository.Products.GetProductByIdWithDetailsAsync(id)
                    :
                await _repository.Products.GetProductBySkuWithDetailsAsync(idOrSku);

            if (product == null)
            {
                _logger.LogError($"Repository: product with id or sku={idOrSku} is not found");
                return NotFound();
            }

            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        // POST api/products
        /// <summary>
        /// Creates a Product.
        /// </summary>
        /// <returns>A newly created product as ProductReadDto</returns>
        /// <response code="201">Returns the newly created product with it's location</response>
        /// <response code="400">If product's "Name" or "Manufacturer" or "Type" or "Subtype" is null or product's "Count" less or equal to 0; If product with actual name is already exists</response>
        /// <response code="500">If an error occurred while creating the product, maybe "Manufacturer.Abbreviation" value is already in use</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductReadDto>> CreateProductAsync(ProductCreateUpdateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            if (product.Name == null || product.Manufacturer == null || product.Type == null || product.Subtype == null || product.Count <= 0)
            {
                _logger.LogError("Repository: could not create product, because product's \"Name\" or \"Manufacturer\" or \"Type\" or \"Subtype\" is null or product's \"Count\"<= 0");
                return ValidationProblem("\"Name\" or \"Manufacturer\" or \"Type\" or \"Subtype\" is null or product's \"Count\"<= 0");
            }

            if ((await _repository.Products.FindByCondition(p => p.Name.Equals(product.Name)).FirstOrDefaultAsync()) != null)
            {
                _logger.LogError($"Repository: could not create product, because product with \"Name\"= {product.Name} is already exists");
                return ValidationProblem($"Product with \"Name\"= {product.Name} is already exists");
            }

            try
            {
                await _repository.Products.CreateProductAsync(product);
                await _repository.SaveAsync();
            }
            catch
            {
                _logger.LogError("Repository: an error occurred while creating product. Maybe \"Manufacturer.Abbreviation\" value is already in use");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating product. Maybe \"Manufacturer.Abbreviation\" value is already in use");
            }

            var productReadDto = _mapper.Map<ProductReadDto>(product);

            _logger.LogInformation("Repository: successfully created product");
            return CreatedAtAction(
                actionName: "GetProductByIdOrSku",
                controllerName: "Products",
                routeValues: new { IdOrSku = productReadDto.Id.ToString() },
                value: productReadDto);
        }

        // PUT api/products/{idOrSku}
        /// <summary>
        /// Updates all the fields of the existing product.
        /// </summary>
        /// <returns>No content</returns>
        /// <response code="204">If product successfully updated</response>
        /// <response code="400">If product's "Name" or "Manufacturer" or "Type" or "Subtype" is null or product's "Count" less or equal to 0; If product with actual name is already exists</response>
        /// <response code="404">If product with actual id or sku is not found</response>
        /// <response code="500">If an error occurred while updating product, maybe "Manufacturer.Abbreviation" value is already in use</response>
        [HttpPut("{idOrSku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProductAsync(string idOrSku, ProductCreateUpdateDto productDto)
        {
            if (productDto.Name == null || productDto.Manufacturer == null || productDto.Type == null || productDto.Subtype == null || productDto.Count < 0)
            {
                _logger.LogError("Repository: could not update product, because product's \"Name\" or \"Manufacturer\" or \"TypeName\" or \"Subtype\" is null or product's \"Count\"< 0");
                return ValidationProblem("\"Name\" or \"Manufacturer\" or \"TypeName\" or \"Subtype\" is null or product's \"Count\"< 0");
            }

            var existingProduct = Guid.TryParse(idOrSku, out var id) ?
                await _repository.Products.GetProductByIdWithDetailsAsync(id)
                    :
                await _repository.Products.GetProductBySkuWithDetailsAsync(idOrSku);

            if (existingProduct == null)
            {
                _logger.LogError($"Repository: product with id or sku={idOrSku} is not found");
                return NotFound();
            }

            _mapper.Map(productDto, existingProduct);

            try
            {
                await _repository.Products.UpdateProductAsync(existingProduct);
                await _repository.SaveAsync();
            }
            catch 
            {
                _logger.LogError("Repository: an error occurred while updating product. Maybe product's manufacturer's \"Abbreviation\" is duplicated");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating product. Maybe product's manufacturer's \"Abbreviation\" is duplicated");
            }

            _logger.LogInformation($"Repository: successfully updated product with id or sku={idOrSku}");
            return NoContent();
        }

        // PUT api/products/{idOrSku}_{positiveOrNegativeValue}
        /// <summary>
        /// Adds positiveOrNegativeValue value to field "count" of existing product.
        /// </summary>
        /// <returns>No content</returns>
        /// <response code="204">If product's count successfully updated</response>
        /// <response code="404">If product with actual id or sku is not found</response>
        /// <response code="500">If an error occurred while updating the product</response>
        [HttpPut("{idOrSku}_{positiveOrNegativeValue}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangeProductCountAsync(string idOrSku, int positiveOrNegativeValue)
        {
            var existingProduct = Guid.TryParse(idOrSku, out var id) ?
                await _repository.Products.GetProductByIdWithDetailsAsync(id)
                :
                await _repository.Products.GetProductBySkuWithDetailsAsync(idOrSku);

            if (existingProduct == null)
            {
                _logger.LogError($"Repository: product with id or sku={idOrSku} is not found");
                return NotFound();
            }

            existingProduct.Count += positiveOrNegativeValue;
            if (existingProduct.Count < 0)
            {
                existingProduct.Count = 0;
            }

            _repository.Products.Update(existingProduct);

            try
            {
                await _repository.SaveAsync();
            }
            catch
            {
                _logger.LogError("Repository: an error occurred while updating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating product");
            }

            _logger.LogInformation($"Repository: successfully changed count of product with id or sku={idOrSku}");
            return NoContent();
        }

        // DELETE api/products/{idOrSku}
        //[SwaggerOperation(Summary = "Completely removes product from database")]
        /// <summary>
        /// Removes the product from database.
        /// </summary>
        /// <returns>No content</returns>
        /// <response code="204">If product successfully deleted</response>
        /// <response code="404">If product with actual id or sku is not found</response>
        /// <response code="500">If an error occurred while deleting the product</response>
        [HttpDelete("{idOrSku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveProductAsync(string idOrSku)
        {
            var product = Guid.TryParse(idOrSku, out var id) ?
                await _repository.Products.GetProductByIdAsync(id)
                    :
                await _repository.Products.GetProductBySkuAsync(idOrSku);

            if (product == null)
            {
                _logger.LogError($"Repository: product with id or sku={idOrSku} is not found");
                return NotFound();
            }

            _repository.Products.DeleteProduct(product);

            try
            {
                await _repository.SaveAsync();
            }
            catch
            {
                _logger.LogError("Repository: an error occurred while deleting product");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting product");
            }

            _logger.LogInformation($"Repository: successfully deleted product with id or sku={idOrSku}");
            return NoContent();
        }
    }
}