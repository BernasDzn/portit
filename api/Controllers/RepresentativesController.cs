using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RepresentativesController : ControllerBase
{
    private readonly ApiContext _context;

    public RepresentativesController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetAllRepresentatives")]
    public ActionResult<IEnumerable<Representative>> GetAll()
    {
        var saors = _context.Representatives.ToList();
        var saorDtos = saors.Select(saor => saor.ToDTO()).ToList();
        return Ok(saorDtos);
    }
}
