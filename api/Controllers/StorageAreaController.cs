using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain;
using DAL;
using Application.Services;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StorageAreaController : ControllerBase
{
    private readonly StorageAreaService _storageAreaService;

    public StorageAreaController(StorageAreaService service)
    {
        _storageAreaService = service;
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
        catch (System.Exception)
        {
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
                return BadRequest();
                
            return CreatedAtAction(nameof(Get), new { id = storageAreaDto.NameCode }, storageAreaDto);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
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
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
