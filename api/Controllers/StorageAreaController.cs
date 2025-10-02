using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StorageAreaController : ControllerBase
{
    private readonly ApiContext _context;

    public StorageAreaController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetAllStorageAreas")]
    public ActionResult<IEnumerable<Representative>> GetAll()
    {
        var areas = _context.StorageAreas.ToList();
        var areasDto = areas.Select(area => area.ToDTO());
        return Ok(areasDto);
    }
}
