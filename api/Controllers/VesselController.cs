using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain.Model.Generic;
using Domain;
using DAL;
using Api.Domain.Model;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VesselController : ControllerBase
{
    private readonly ApiContext _context;

    public VesselController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetVessels")]
    public ActionResult<IEnumerable<VesselDto>> GetAll()
    {

        var vessels = _context.Vessels.ToList();
        var vesselDtos = vessels.Select(vessel => vessel.ToDTO()).ToList();
        return Ok(vesselDtos);
    }

    [HttpGet("searchByName", Name = "GetVesselByName")]
    public ActionResult<IEnumerable<VesselDto>> GetByName([FromQuery] string name)
    {
        var vessels = _context.Vessels
            .Where(v => v.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (vessels.Count == 0)
        {
            return NotFound();
        }

        var vesselDtos = vessels.Select(vessel => vessel.ToDTO()).ToList();
        return Ok(vesselDtos);
    }

    [HttpGet("searchByIMO", Name = "GetVesselByIMO")]
    public ActionResult<VesselDto> GetByIMO([FromQuery] string imo)
    {
        var vessel = _context.Vessels
            .FirstOrDefault(v => v.ImoNumber.Equals(imo, StringComparison.OrdinalIgnoreCase));

        if (vessel == null)
        {
            return NotFound();
        }

        return Ok(vessel.ToDTO());
    }

    [HttpGet("searchByOwner", Name = "GetVesselByOwner")]
    public ActionResult<IEnumerable<VesselDto>> GetByOwner([FromQuery] uint owner)
    {
        var vessels = _context.Vessels
            .Where(v => v.OwnerCitizenshipId == owner)
            .ToList();

        if (vessels.Count == 0)
        {
            return NotFound();
        }

        var vesselDtos = vessels.Select(vessel => vessel.ToDTO()).ToList();
        return Ok(vesselDtos);
    }

    [HttpPost(Name = "CreateVessel")]
    public ActionResult<VesselDto> Create(VesselDto vesselDto)
    {
        var vessel = Vessel.FromDTO(vesselDto);
        _context.Vessels.Add(vessel);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetAll), new { id = vessel.Id }, vessel.ToDTO());
    }

    [HttpDelete("{id}", Name = "DeleteVessel")]
    public IActionResult Delete(Guid id)
    {
        var vessel = _context.Vessels.Find(id);
        if (vessel == null)
        {
            return NotFound();
        }

        _context.Vessels.Remove(vessel);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPut("{id}", Name = "UpdateVessel")]
    public IActionResult Update(Guid id, VesselDto vesselDto)
    {
        if (id != vesselDto.Id)
        {
            return BadRequest();
        }

        var vessel = _context.Vessels.Find(id);
        if (vessel == null)
        {
            return NotFound();
        }

        vessel = Vessel.FromDTO(vesselDto);
        _context.Vessels.Update(vessel);
        _context.SaveChanges();
        return NoContent();
    }
}
