namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Infrastructure.Exceptions;

[ApiController]
[Route("[controller]")]
public class StorageAreaController : ControllerBase, IStorageAreaController
{
    private readonly IStorageAreaService _storageAreaService;
    private readonly ILogger<StorageAreaController> _logger;

    public StorageAreaController(IStorageAreaService service, ILogger<StorageAreaController> logger)
    {
        _storageAreaService = service;
        _logger = logger;
    }

    [HttpGet(Name = "GetAllStorageAreas")]
    public async Task<ActionResult<IEnumerable<StorageAreaDto>>> GetAll()
    {
        try
        {
            IEnumerable<StorageAreaDto> storageAreaDtos = await _storageAreaService.GetStorageAreas();
            return Ok(storageAreaDtos);
        }
        catch (System.Exception)
        {
            _logger.LogCritical("Error retrieving storage areas");
            return StatusCode(500, "An error occurred while retrieving storage areas.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StorageAreaDto>> Get(string id)
    {
        try
        {
            var storageAreaDto = await _storageAreaService.GetStorageAreaByCode(id);
            return Ok(storageAreaDto);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error retrieving storage area by id, {Message}", e.Message);
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            _logger.LogCritical("Error retrieving storage area by id, {Message}", e.Message);
            return StatusCode(500, "An error occurred while retrieving the storage area.");
        }
    }

    [HttpPost(Name = "CreateStorageArea")]
    public async Task<ActionResult<StorageAreaDto>> Create(CreateStorageAreaDto createStorageAreaDto)
    {
        try
        {
            var storageAreaDto = await _storageAreaService.CreateStorageArea(createStorageAreaDto);
            return CreatedAtAction(nameof(Get), new { id = storageAreaDto.NameCode }, storageAreaDto);
        }
        catch (EntityAlreadyExistsException e)
        {
            _logger.LogError("Error creating storage area, {Message}", e.Message);
            return Conflict(e.Message);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error retrieving dependency by id, {Message}", e.Message);
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            if (e is ArgumentException || e is ArgumentNullException || e is StorageFullException)
            {
                _logger.LogError("Invalid arguments provided for creating storage area, {Message}", e.Message);
                return BadRequest(e.Message);
            }

            _logger.LogCritical("Error creating storage area, {Message}", e.Message);
            return StatusCode(500, "An error occurred while retrieving the storage area.");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StorageAreaDto>> Update(string id, [FromBody] CreateStorageAreaDto updateStorageAreaDto)
    {
        try
        {
            var storageAreaDto = await _storageAreaService.UpdateStorageArea(id, updateStorageAreaDto);
            return Ok(storageAreaDto);
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError("Error retrieving dependency by id, {Message}", e.Message);
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            if (e is ArgumentException || e is ArgumentNullException || e is StorageFullException)
            {
                _logger.LogError("Invalid arguments provided for updating storage area, {Message}", e.Message);
                return BadRequest(e.Message);
            }

            _logger.LogCritical("Error updating storage area, {Message}", e.Message);
            return StatusCode(500, "An error occurred while retrieving the storage area.");
        }
    }

}
