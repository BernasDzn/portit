using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain.Model.Generic;
using Domain;
using DAL;

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
}
