using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain;

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
        return Ok(_context.Representatives.ToList());
    }
}
