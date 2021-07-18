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
    [Route("api/products/types")]
    [ApiController]
    public class ProductsTypesController : ControllerBase
    {
        private readonly ProductApiContext _context;
        private readonly IMapper _mapper;

        public ProductsTypesController(ProductApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET api/products/types
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTypeReadDto>>> GetAllProductsTypesAsync()
        {
            var types = await _context.Types.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProductTypeReadDto>>(types));
        }

        // GET api/products/types/{id}
        [HttpGet("{id}", Name = nameof(GetProductTypeByIdAsync))]
        public async Task<ActionResult<ProductTypeReadDto>> GetProductTypeByIdAsync(Guid id)
        {
            var type = await _context.Types.FirstOrDefaultAsync(t => t.Id == id);
            if (type == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductTypeReadDto>(type));
        }

        // POST api/products/types
        [HttpPost]
        public async Task<ActionResult<ProductTypeReadDto>> CreateProductTypeAsync(ProductTypeCreateUpdateDto typeDto)
        {
            var type = _mapper.Map<ProductType>(typeDto);
            if (type.Name == null)
            {
                return BadRequest("\"Name\" is null.");
            }

            await _context.Types.AddAsync(type);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var typeReadDto = _mapper.Map<ProductTypeReadDto>(type);

            return CreatedAtAction(
                actionName: "GetProductTypeById",
                controllerName: "ProductsTypes",
                routeValues: new { Id = typeReadDto.Id },
                value: typeReadDto);
        }

        // PUT api/products/types/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductTypeAsync(Guid id, ProductTypeCreateUpdateDto typeDto)
        {
            var type = await _context.Types.FirstOrDefaultAsync(t => t.Id == id);
            if (type == null)
            {
                return NotFound();
            }

            _mapper.Map(typeDto, type);

            _context.Types.Update(type);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/products/types/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveProductTypeAsync(Guid id)
        {
            var type = await _context.Types.FirstOrDefaultAsync(t => t.Id == id);
            if (type == null)
            {
                return NotFound();
            }

            _context.Types.Remove(type);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly type you are trying to remove is referenced.");
            }

            return NoContent();
        }
    }
}
