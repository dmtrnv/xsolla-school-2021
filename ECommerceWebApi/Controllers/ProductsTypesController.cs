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
    [Route("api/products/types")]
    [ApiController]
    public class ProductsTypesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsTypesController> _logger;

        public ProductsTypesController(IRepositoryWrapper repository, IMapper mapper, ILogger<ProductsTypesController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET api/products/types
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTypeReadDto>>> GetAllProductsTypesAsync()
        {
            var types = await _repository.Types.GetAllTypesAsync();

            _logger.LogInformation("Repository: successfully returned list of all types");
            return Ok(_mapper.Map<IEnumerable<ProductTypeReadDto>>(types));
        }

        // GET api/products/types/{id}
        [HttpGet("{id}", Name = nameof(GetProductTypeByIdAsync))]
        public async Task<ActionResult<ProductTypeReadDto>> GetProductTypeByIdAsync(Guid id)
        {
            var type = await _repository.Types.GetTypeByIdAsync(id);
            if (type == null)
            {
                _logger.LogError($"Repository: type with id={id} is not found");
                return NotFound();
            }

            _logger.LogInformation($"Repository: successfully returned type with id={id}");
            return Ok(_mapper.Map<ProductTypeReadDto>(type));
        }

        // POST api/products/types
        [HttpPost]
        public async Task<ActionResult<ProductTypeReadDto>> CreateProductTypeAsync(ProductTypeCreateUpdateDto typeDto)
        {
            var type = _mapper.Map<ProductType>(typeDto);
            if (type.Name == null)
            {
                _logger.LogError("Repository: could not create type, because type's \"Name\" is null");
                return BadRequest("\"Name\" is null");
            }

            _repository.Types.CreateType(type);

            try
            {
                await _repository.SaveAsync();
            }
            catch 
            {
                _logger.LogError("Repository: an error occurred while creating type");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var typeReadDto = _mapper.Map<ProductTypeReadDto>(type);

            _logger.LogInformation("Repository: successfully created type");
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
            var type = await _repository.Types.GetTypeByIdAsync(id);
            if (type == null)
            {
                _logger.LogError($"Repository: type with id={id} is not found");
                return NotFound();
            }

            _mapper.Map(typeDto, type);

            _repository.Types.UpdateType(type);

            try
            {
                await _repository.SaveAsync();
            }
            catch
            {
                _logger.LogError("Repository: an error occurred while updating type");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            _logger.LogInformation($"Repository: successfully updated type with id={id}");
            return NoContent();
        }

        // DELETE api/products/types/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveProductTypeAsync(Guid id)
        {
            var type = await _repository.Types.GetTypeByIdAsync(id);
            if (type == null)
            {
                _logger.LogError($"Repository: type with id={id} is not found");
                return NotFound();
            }

            _repository.Types.DeleteType(type);

            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                _logger.LogError("Repository: an error occurred while deleting type. Possibly type is referenced");
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly type is referenced");
            }

            _logger.LogInformation($"Repository: successfully deleted type with id={id}");
            return NoContent();
        }
    }
}
