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
    [Route("api/products/manufacturers")]
    [ApiController]
    public class ProductsManufacturersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsManufacturersController> _logger;

        public ProductsManufacturersController(IRepositoryWrapper repository, IMapper mapper, ILogger<ProductsManufacturersController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET api/products/manufacturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductManufacturerReadDto>>> GetAllProductsManufacturersAsync()
        {
            var manufacturers = await _repository.Manufacturers.GetAllManufacturersAsync();

            _logger.LogInformation("Repository: successfully returned list of all manufacturers");
            return Ok(_mapper.Map<IEnumerable<ProductManufacturerReadDto>>(manufacturers));
        }

        // GET api/products/manufacturers/{id}
        [HttpGet("{id}", Name = nameof(GetProductManufacturerByIdAsync))]
        public async Task<ActionResult<ProductManufacturerReadDto>> GetProductManufacturerByIdAsync(Guid id)
        {
            var manufacturer = await _repository.Manufacturers.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
            {
                _logger.LogError($"Repository: manufacturer with id={id} is not found");
                return NotFound();
            }

            _logger.LogInformation($"Repository: successfully returned manufacturer with id={id}");
            return Ok(_mapper.Map<ProductManufacturerReadDto>(manufacturer));
        }

        // POST api/products/manufacturers
        [HttpPost]
        public async Task<ActionResult<ProductManufacturerReadDto>> CreateProductManufacturerAsync(ProductManufacturerCreateUpdateDto manufacturerDto)
        {
            var manufacturer = _mapper.Map<ProductManufacturer>(manufacturerDto);
            if (manufacturer.Name == null || manufacturer.Abbreviation == null)
            {
                _logger.LogError("Repository: could not create manufacturer, because manufacturer's \"Name\" or \"Abbreviation\" is null");
                return BadRequest("Manufacturer's \"Name\" or \"Abbreviation\" is null");
            }

            _repository.Manufacturers.CreateManufacturer(manufacturer);

            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2627)
            {
                _logger.LogError("Repository: an error occurred while creating manufacturer. Possibly manufacturer's \"Abbreviation\" is duplicated");
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly manufacturer's \"Abbreviation\" is duplicated");
            }

            var manufacturerReadDto = _mapper.Map<ProductManufacturerReadDto>(manufacturer);

            _logger.LogInformation("Repository: successfully created manufacturer");
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
            var manufacturer = await _repository.Manufacturers.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
            {
                _logger.LogError($"Repository: manufacturer with id={id} is not found");
                return NotFound();
            }

            _mapper.Map(manufacturerDto, manufacturer);

            _repository.Manufacturers.UpdateManufacturer(manufacturer);

            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2627)
            {
                _logger.LogError("Repository: an error occurred while updating manufacturer. Possibly manufacturer's \"Abbreviation\" is duplicated");
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly manufacturer's \"Abbreviation\" is duplicated");
            }

            _logger.LogInformation($"Repository: successfully updated manufacturer with id={id}");
            return NoContent();
        }

        // DELETE api/products/manufacturers/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveProductManufacturerAsync(Guid id)
        {
            var manufacturer = await _repository.Manufacturers.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
            {
                _logger.LogError($"Repository: manufacturer with id={id} is not found");
                return NotFound();
            }

            _repository.Manufacturers.DeleteManufacturer(manufacturer);

            try
            {
                await _repository.SaveAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                _logger.LogError("Repository: an error occurred while deleting manufacturer. Possibly manufacturer is referenced");
                return StatusCode(StatusCodes.Status500InternalServerError, "Possibly manufacturer is referenced");
            }

            _logger.LogInformation($"Repository: successfully deleted manufacturer with id={id}");
            return NoContent();
        }
    }
}