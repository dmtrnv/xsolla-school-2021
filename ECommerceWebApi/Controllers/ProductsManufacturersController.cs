using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.DataTransferObjects;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/products/manufacturers")]
    [ApiController]
    public class ProductsManufacturersController : ControllerBase
    {
        private readonly ProductApiContext _context;
        private readonly IMapper _mapper;

        public ProductsManufacturersController(ProductApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET api/products/manufacturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductManufacturerReadDto>>> GetAllProductsManufacturersAsync()
        {
            var manufacturers = await _context.Manufacturers.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProductManufacturerReadDto>>(manufacturers));
        }

        // GET api/products/manufacturers/{id}
        [HttpGet("{id}", Name = nameof(GetProductManufacturerByIdAsync))]
        public async Task<ActionResult<ProductManufacturerReadDto>> GetProductManufacturerByIdAsync(Guid id)
        {
            var manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductManufacturerReadDto>(manufacturer));
        }

        // POST api/products/manufacturers
        [HttpPost]
        public async Task<ActionResult<ProductManufacturerReadDto>> CreateProductManufacturerAsync(ProductManufacturerCreateUpdateDto manufacturerDto)
        {
            var manufacturer = _mapper.Map<ProductManufacturer>(manufacturerDto);
            if (manufacturer.Name == null || manufacturer.Abbreviation == null)
            {
                return BadRequest("\"Name\" or \"Abbreviation\" is null.");
            }

            await _context.Manufacturers.AddAsync(manufacturer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2627)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly \"Abbreviation\" is duplicated.");
            }

            var manufacturerReadDto = _mapper.Map<ProductManufacturerReadDto>(manufacturer);

            return CreatedAtAction(
                actionName: "GetProductManufacturerById", 
                controllerName: "ProductsManufacturers",
                routeValues: new { Id = manufacturerReadDto.Id }, 
                value: manufacturerReadDto);
        }

        // PUT api/products/manufacturers/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductManufacturerAsync(Guid id, ProductManufacturerCreateUpdateDto manufacturerDto)
        {
            var manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            _mapper.Map(manufacturerDto, manufacturer);

            _context.Manufacturers.Update(manufacturer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/products/manufacturers/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveProductManufacturerAsync(Guid id)
        {
            var manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            _context.Manufacturers.Remove(manufacturer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly manufacturer you are trying to remove is referenced.");
            }

            return NoContent();
        }
    }
}
