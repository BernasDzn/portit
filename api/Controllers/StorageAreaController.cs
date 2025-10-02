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
}
