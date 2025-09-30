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
    public ActionResult<IEnumerable<VesselDto>> GetByOwner([FromQuery] ShippingAgentOrganization owner)
    {
        var vessels = _context.Vessels
            .Where(v => v.Owner.Id == owner.Id)
            .ToList();

        if (vessels.Count == 0)
        {
            return NotFound();
        }

        var vesselDtos = vessels.Select(vessel => vessel.ToDTO()).ToList();
        return Ok(vesselDtos);
    }

    [HttpPost(Name = "CreateVessel")]
    public ActionResult<VesselDto> Create(Vessel vessel)
    {
        _context.Vessels.Add(vessel);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetAll), new { id = vessel.Id }, vessel.ToDTO());
    }

    [HttpDelete("{ImoNumber}", Name = "DeleteVessel")]
    public IActionResult Delete(string ImoNumber)
    {
        var vessel = _context.Vessels.FirstOrDefault(v => v.ImoNumber == ImoNumber);
        if (vessel == null)
        {
            return NotFound();
        }

        _context.Vessels.Remove(vessel);
        _context.SaveChanges();
        return Ok();
    }

    [HttpPut("{ImoNumber}", Name = "UpdateVessel")]
    public IActionResult Update(string ImoNumber, VesselDto vesselDto)
    {
        if (ImoNumber != vesselDto.ImoNumber)
        {
            return BadRequest();
        }

        var vessel = _context.Vessels.FirstOrDefault(v => v.ImoNumber == ImoNumber);
        if (vessel == null)
        {
            return NotFound();
        }

        vessel.Update(vesselDto);
        _context.SaveChanges();
        return Ok();
    }
}
