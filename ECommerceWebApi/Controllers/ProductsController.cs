using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductApi.Data;
using ProductApi.DataTransferObjects;
using ProductApi.Misc;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductApiContext _context;
        private readonly IMapper _mapper;
        private readonly ProductsManufacturersController _manufacturersController;
        private readonly ProductsTypesController _typesController;
        private readonly ProductsSubtypesController _subtypesController;

        public ProductsController(
            ProductApiContext context, 
            IMapper mapper, 
            ProductsManufacturersController manufacturersController,
            ProductsTypesController typesController,
            ProductsSubtypesController subtypesController)
        {
            _context = context;
            _mapper = mapper;
            _manufacturersController = manufacturersController;
            _typesController = typesController;
            _subtypesController = subtypesController;
        }

        // GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProductsAsync([FromQuery]ProductParameters parameters)
        {
            if (!parameters.ValidCost)
            {
                return BadRequest("MaxCost < MinCost, should be the other way around.");
            }

            IQueryable<Product> products;

            if (parameters.TypeName == null)
            {
                products = _context.Products
                    .Where(p => (p.Cost >= parameters.MinCost) && (p.Cost <= parameters.MaxCost))
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Type)
                    .Include(p => p.Subtype)
                    .OrderBy(p => p.Name);
            }
            else
            {
                products = _context.Products
                    .Where(p => (p.Cost >= parameters.MinCost) && (p.Cost <= parameters.MaxCost))
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Type)
                    .Include(p => p.Subtype)
                    .Where(p => p.Type.Name == parameters.TypeName)
                    .OrderBy(p => p.Name);
            }

            var productsPaged = await Task.Run(() => PagedList<Product>.ToPagedList(
                                                                                    products, 
                                                                                    parameters.PageNumber,
                                                                                    parameters.PageSize));

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

            return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(productsPaged));
        }

        // GET api/products/{idOrSku}
        [HttpGet("{idOrSku}", Name = nameof(GetProductByIdOrSkuAsync))]
        public async Task<ActionResult<ProductReadDto>> GetProductByIdOrSkuAsync(string idOrSku)
        {
            Product product;

            if (Guid.TryParse(idOrSku, out var id))
            {
                product = await _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Type)
                    .Include(p => p.Subtype)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            else
            {
                product = await _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Type)
                    .Include(p => p.Subtype)
                    .FirstOrDefaultAsync(p => p.Sku == idOrSku);
            }

            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        // POST api/products
        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> CreateProductAsync(ProductCreateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            if (product.Name == null || product.Manufacturer == null || product.Type == null || product.Subtype == null)
            {
                return BadRequest("\"Name\" or \"Manufacturer\" or \"TypeName\" or \"Subtype\" is null.");
            }

            // Get or create manufacturer.
            product.Manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Name == product.Manufacturer.Name);
            if (product.Manufacturer == null)
            {
                await _manufacturersController.CreateProductManufacturerAsync(productDto.Manufacturer);
                product.Manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Name == productDto.Manufacturer.Name);
            }
            if (product.Manufacturer == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly \"Manufacturer.Abbreviation\" value is already in use. Choose another value please.");
            }
            product.ManufacturerId = product.Manufacturer.Id;

            // Get or create type.
            product.Type = await _context.Types.FirstOrDefaultAsync(t => t.Name == product.Type.Name);
            if (product.Type == null)
            {
                await _typesController.CreateProductTypeAsync(productDto.Type);
                product.Type = await _context.Types.FirstOrDefaultAsync(t => t.Name == productDto.Type.Name);
            }
            product.TypeId = product.Type.Id;

            // Get or create subtype.
            product.Subtype = await _context.Subtypes.FirstOrDefaultAsync(s => s.Name == product.Subtype.Name);
            if (product.Subtype == null)
            {
                await _subtypesController.CreateProductSubtypeAsync(productDto.Subtype);
                product.Subtype = await _context.Subtypes.FirstOrDefaultAsync(s => s.Name == productDto.Subtype.Name);
            }
            product.SubtypeId = product.Subtype.Id;

            product.Sku = $"{product.Manufacturer.Abbreviation}-{product.Type.Code}-{product.Subtype.Code}-{product.Name.Replace(" ", string.Empty).ToUpper()}";

            await _context.Products.AddAsync(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "\"Manufacturer.Abbreviation\" value is already in use. Choose another please.");
            }

            var productReadDto = _mapper.Map<ProductReadDto>(product);

            return CreatedAtAction(
                actionName: "GetProductByIdOrSku",
                controllerName: "Products",
                routeValues: new { IdOrSku = productReadDto.Id.ToString() },
                value: productReadDto);
        }

        /// <summary>
        /// Updates single product with given id or all products with given sku.
        /// </summary>
        // PUT api/products/{idOrSku}
        [HttpPut("{idOrSku}")]
        public async Task<ActionResult> UpdateProductAsync(string idOrSku, ProductUpdateDto productDto)
        {
            if (productDto.Name == null || productDto.Cost == 0)
            {
                return BadRequest("\"Name\" is null or \"Cost\" = 0.");
            }

            if (Guid.TryParse(idOrSku, out var id))
            {
                var product = await _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Type)
                    .Include(p => p.Subtype)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return NotFound();
                }

                _mapper.Map(productDto, product);
                product.Sku = $"{product.Manufacturer.Abbreviation}-{product.Type.Code}-{product.Subtype.Code}-{product.Name.Replace(" ", string.Empty).ToUpper()}";

                _context.Products.Update(product);
            }
            else
            {
                var products = await _context.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Type)
                    .Include(p => p.Subtype)
                    .Where(p => p.Sku == idOrSku)
                    .ToListAsync();
                if (products.Count == 0)
                {
                    return NotFound();
                }

                foreach (var product in products)
                {
                    _mapper.Map(productDto, product);
                    product.Sku = $"{product.Manufacturer.Abbreviation}-{product.Type.Code}-{product.Subtype.Code}-{product.Name.Replace(" ", string.Empty).ToUpper()}";

                    _context.Products.Update(product);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/products/{idOrSku}
        [HttpDelete("{idOrSku}")]
        public async Task<ActionResult> RemoveProductAsync(string idOrSku)
        {
            Product product;

            if (Guid.TryParse(idOrSku, out var id))
            {
                product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            }
            else
            {
                product = await _context.Products.FirstOrDefaultAsync(p => p.Sku == idOrSku);
            }

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}