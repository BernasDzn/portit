using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain;
using DAL;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RepresentativeController : ControllerBase
{
    private readonly ApiContext _context;

    public RepresentativeController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetRepresentatives")]
    public ActionResult<IEnumerable<RepresentativeDto>> GetAll()
    {
        var reps = _context.Representatives.ToList();
        var repDtos = reps.Select(rep => rep.ToDTO()).ToList();
        return Ok(repDtos);
    }

    [HttpGet("email/{email}")]
    public ActionResult<RepresentativeDto> GetByEmail(string email)
    {
        var rep = _context.Representatives.FirstOrDefault(r => r.EmailAddress.Value == email);
        if (rep == null)
        {
            return NotFound();
        }
        return Ok(rep.ToDTO());
    }

    [HttpGet("citizen/{citizenId}")]
    public ActionResult<RepresentativeDto> GetByCitizenId(string citizenId)
    {
        var rep = _context.Representatives.FirstOrDefault(r => r.CitizenshipId.ToString() == citizenId);
        if (rep == null)
        {
            return NotFound();
        }
        return Ok(rep.ToDTO());
    }
}