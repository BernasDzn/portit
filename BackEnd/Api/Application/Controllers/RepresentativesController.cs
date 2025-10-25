namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;
using Api.Infrastructure.Persistence;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;

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
        var rep = _context.Representatives.AsEnumerable()
            .FirstOrDefault(r => r.EmailAddress.Value == email);
        if (rep == null)
        {
            return NotFound();
        }
        return Ok(rep.ToDTO());
    }

    [HttpGet("citizen/{citizenId}")]
    public ActionResult<RepresentativeDto> GetByCitizenId(string citizenId)
    {
        if (!uint.TryParse(citizenId, out var citizenIdUInt))
        {
            return BadRequest("Invalid citizen id format");
        }

        var rep = _context.Representatives.FirstOrDefault(r => r.CitizenshipId == citizenIdUInt);
        if (rep == null)
        {
            return NotFound();
        }
        return Ok(rep.ToDTO());
    }

    [HttpPost(Name = "CreateRepresentative")]
    public ActionResult<RepresentativeDto> Create(RepresentativeDto repDto)
    {
        Representative rep = new Representative(
            Guid.NewGuid(),
            repDto.CitizenshipId,
            new Designation { Value = repDto.Name },
            new Email { Value = repDto.EmailAddress },
            new PhoneNumber { Value = repDto.Phone }
        );
        var createdRep = _context.Representatives.Add(rep);
        _context.SaveChanges();
        return CreatedAtRoute("GetRepresentatives", new { id = createdRep.Entity.Id }, createdRep.Entity.ToDTO());
    }
}