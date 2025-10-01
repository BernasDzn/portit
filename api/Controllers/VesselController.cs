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

    [HttpGet("{name}", Name = "GetVesselByName")]
    public async Task<ActionResult<VesselDto>> Get(string name)
    {
        List<string> errors = new List<string>();

        var vesselDto = await _vesselService.GetVesselByName(name, errors);
        if (vesselDto == null)
            return NotFound(errors);
        return Ok(vesselDto);
    }

    [HttpPost(Name = "PostVessel")]
    public async Task<ActionResult<VesselDto>> Create(VesselDto vesselDto)
    {
        List<string> errors = new List<string>();

        var createdVessel = await _vesselService.Add(vesselDto, errors);
        if (createdVessel == null)
            return BadRequest(errors);

        return CreatedAtAction(nameof(Get), new { name = createdVessel?.Name }, createdVessel);
    }

    [HttpPut("{name}", Name = "UpdateVessel")]
    public async Task<ActionResult<VesselDto>> Update(string name, VesselDto vesselDto)
    {
        List<string> errors = new List<string>();

        var updatedVessel = await _vesselService.Update(name, vesselDto, errors);
        if (updatedVessel == null)
            return BadRequest(errors);

        return Ok(updatedVessel);
    }
}
