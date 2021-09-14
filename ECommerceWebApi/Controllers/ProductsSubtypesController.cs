using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductApi.Data;
using ProductApi.DataTransferObjects;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/products/subtypes")]
    [ApiController]
    public class ProductsSubtypesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsSubtypesController> _logger;

        public ProductsSubtypesController(IRepositoryWrapper repository, IMapper mapper, ILogger<ProductsSubtypesController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET api/products/subtypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductSubtypeReadDto>>> GetAllProductsSubtypesAsync()
        {
            var subtypes = await _repository.Subtypes.GetAllSubtypesAsync();

            _logger.LogInformation("Repository: successfully returned list of all subtypes");
            return Ok(_mapper.Map<IEnumerable<ProductSubtypeReadDto>>(subtypes));
        }

        // GET api/products/subtypes/{id}
        [HttpGet("{id}", Name = nameof(GetProductSubtypeByIdAsync))]
        public async Task<ActionResult<ProductSubtypeReadDto>> GetProductSubtypeByIdAsync(Guid id)
        {
            var subtype = await _repository.Subtypes.GetSubtypeById(id);
            if (subtype == null)
            {
                _logger.LogError($"Repository: subtype with id={id} is not found");
                return NotFound();
            }

            _logger.LogInformation($"Repository: successfully returned subtype with id={id}");
            return Ok(_mapper.Map<ProductSubtypeReadDto>(subtype));
        }

        // POST api/products/subtypes
        [HttpPost]
        public async Task<ActionResult<ProductSubtypeReadDto>> CreateProductSubtypeAsync(ProductSubtypeCreateUpdateDto subtypeDto)
        {
            var subtype = _mapper.Map<ProductSubtype>(subtypeDto);
            if (subtype.Name == null)
            {
                _logger.LogError("Repository: could not create subtype, because subtype's \"Name\" is null");
                return BadRequest("\"Name\" is null");
            }

            _repository.Subtypes.CreateSubtype(subtype);

            try
            {
                await _repository.SaveAsync();
            }
            catch
            {
                _logger.LogError("Repository: an error occurred while creating subtype");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var subtypeReadDto = _mapper.Map<ProductSubtypeReadDto>(subtype);

            _logger.LogInformation("Repository: successfully created subtype");
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
            var subtype = await _repository.Subtypes.GetSubtypeById(id);
            if (subtype == null)
            {
                _logger.LogError($"Repository: subtype with id={id} is not found");
                return NotFound();
            }

            _mapper.Map(subtypeDto, subtype);

            _repository.Subtypes.UpdateSubtype(subtype);

            try
            {
                await _repository.SaveAsync();
            }
            catch
            {
                _logger.LogError("Repository: an error occurred while updating subtype");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            _logger.LogInformation($"Repository: successfully updated subtype with id={id}");
            return NoContent();
        }

        // DELETE api/products/subtypes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveProductSubtypeAsync(Guid id)
        {
            var subtype = await _repository.Subtypes.GetSubtypeById(id);
            if (subtype == null)
            {
                _logger.LogError($"Repository: subtype with id={id} is not found");
                return NotFound();
            }

            _repository.Subtypes.DeleteSubtype(subtype);

            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                _logger.LogError("Repository: an error occurred while deleting subtype. Possibly subtype is referenced");
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly subtype is referenced");
            }

            _logger.LogInformation($"Repository: successfully deleted subtype with id={id}");
            return NoContent();
        }
    }
}
