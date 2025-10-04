using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain.Model.Generic;
using Domain;
using DAL;
using Api.Domain.Model;
using Application.Services;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VesselController : ControllerBase
{
    private readonly ILogger<VesselController> _logger;
    private readonly VesselService _vesselService;

    public VesselController(VesselService vesselService, ILogger<VesselController> logger)
    {
        _vesselService = vesselService;
        _logger = logger;
    }

    [HttpGet(Name = "GetVessels")]
    public async Task<ActionResult<IEnumerable<VesselDto>>> GetAll()
    {
        IEnumerable<VesselDto> vesselsDto = await _vesselService.GetVessels();
        return Ok(vesselsDto);
    }

    [HttpPost(Name = "PostVessel")]
    public async Task<ActionResult<VesselDto>> Create(VesselDto vesselDto)
    {
        try
        {
            var createdVessel = await _vesselService.Add(vesselDto);
            if (createdVessel == null)
                return BadRequest("Could not create vessel");

            return CreatedAtAction(nameof(GetAll), new { name = createdVessel.Name }, createdVessel);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("filter")]
    public async Task<ActionResult<Page<VesselDto>>> Filter([FromQuery] VesselFilter filter)
    {
        try
        {
            var vesselDtos = await _vesselService.FilterVessels(filter);

            return Ok(vesselDtos);
        }
        catch (System.Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{name}", Name = "UpdateVessel")]
    public async Task<ActionResult<VesselDto>> Update(string name, VesselDto vesselDto)
    {
        try
        {
            var updatedVessel = await _vesselService.Update(name, vesselDto);
            if (updatedVessel == null)
                return BadRequest("Could not update vessel");

            return Ok(updatedVessel);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
