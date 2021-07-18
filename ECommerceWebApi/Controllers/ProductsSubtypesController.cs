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
    [Route("api/products/subtypes")]
    [ApiController]
    public class ProductsSubtypesController : ControllerBase
    {
        private readonly ProductApiContext _context;
        private readonly IMapper _mapper;

        public ProductsSubtypesController(ProductApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET api/products/subtypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductSubtypeReadDto>>> GetAllProductsSubtypesAsync()
        {
            var subtypes = await _context.Subtypes.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProductSubtypeReadDto>>(subtypes));
        }

        // GET api/products/subtypes/{id}
        [HttpGet("{id}", Name = nameof(GetProductSubtypeByIdAsync))]
        public async Task<ActionResult<ProductSubtypeReadDto>> GetProductSubtypeByIdAsync(Guid id)
        {
            var subtype = await _context.Subtypes.FirstOrDefaultAsync(s => s.Id == id);
            if (subtype == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductSubtypeReadDto>(subtype));
        }

        // POST api/products/subtypes
        [HttpPost]
        public async Task<ActionResult<ProductSubtypeReadDto>> CreateProductSubtypeAsync(ProductSubtypeCreateUpdateDto subtypeDto)
        {
            var subtype = _mapper.Map<ProductSubtype>(subtypeDto);
            if (subtype.Name == null)
            {
                return BadRequest("\"Name\" is null.");
            }

            await _context.Subtypes.AddAsync(subtype);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var subtypeReadDto = _mapper.Map<ProductSubtypeReadDto>(subtype);

            return CreatedAtAction(
                actionName: "GetProductSubtypeById",
                controllerName: "ProductsSubtypes",
                routeValues: new { Id = subtypeReadDto.Id },
                value: subtypeReadDto);
        }

        // PUT api/products/subtypes/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductSubtypeAsync(Guid id, ProductSubtypeCreateUpdateDto subtypeDto)
        {
            var subtype = await _context.Subtypes.FirstOrDefaultAsync(s => s.Id == id);
            if (subtype == null)
            {
                return NotFound();
            }

            _mapper.Map(subtypeDto, subtype);

            _context.Subtypes.Update(subtype);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/products/subtypes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveProductSubtypeAsync(Guid id)
        {
            var subtype = await _context.Subtypes.FirstOrDefaultAsync(s => s.Id == id);
            if (subtype == null)
            {
                return NotFound();
            }

            _context.Remove(subtype);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly subtype you are trying to remove is referenced.");
            }

            return NoContent();
        }
    }
}
