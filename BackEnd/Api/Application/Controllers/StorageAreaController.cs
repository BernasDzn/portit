namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;


[ApiController]
[Route("[controller]")]
public class StorageAreaController : ControllerBase, IStorageAreaController
{
    private readonly StorageAreaService _storageAreaService;
    private readonly ILogger<StorageAreaController> _logger;

    public StorageAreaController(StorageAreaService service, ILogger<StorageAreaController> logger)
    {
        _storageAreaService = service;
        _logger = logger;
    }

    [HttpGet(Name = "GetAllStorageAreas")]
    public async Task<ActionResult<IEnumerable<StorageAreaDto>>> GetAll()
    {
        IEnumerable<StorageAreaDto> storageAreaDtos = await _storageAreaService.GetStorageAreas();
        return Ok(storageAreaDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StorageAreaDto>> Get(string id)

    {
        try
        {
            var storageAreaDto = await _storageAreaService.GetStorageAreaByCode(id);
            return Ok(storageAreaDto);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error retrieving storage area by id, {Message}", e.Message);
            return NotFound();
        }
    }

    [HttpPost(Name = "CreateStorageArea")]
    public async Task<ActionResult<StorageAreaDto>> Create(StorageAreaDto createStorageAreaDto)
    {
        try
        {
            var storageAreaDto = await _storageAreaService.CreateStorageArea(createStorageAreaDto);
            if (storageAreaDto == null)
                return BadRequest("Unable to create storage area");
                
            return CreatedAtAction(nameof(Get), new { id = storageAreaDto.NameCode }, storageAreaDto);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error creating storage area, {Message}", e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StorageAreaDto>> Update(string id, [FromBody] StorageAreaDto updateStorageAreaDto)
    {
        try
        {
            var storageAreaDto = await _storageAreaService.UpdateStorageArea(id, updateStorageAreaDto);
            return Ok(storageAreaDto);
        }
        catch (System.Exception e)
        {
            _logger.LogError("Error updating storage area, {Message}", e.Message);
            return BadRequest(e.Message);
        }
    }

}
