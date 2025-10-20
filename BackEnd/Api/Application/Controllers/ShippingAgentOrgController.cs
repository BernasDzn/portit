namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;
using Api.Infrastructure.Persistence;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;

[ApiController]
[Route("[controller]")]
public class ShippingAgentOrganizationController : ControllerBase
{
    private readonly ApiContext _context;

    public ShippingAgentOrganizationController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetShippingAgentOrganizations")]
    public ActionResult<IEnumerable<ShippingAgentOrganizationDto>> GetAll()
    {

        var saos = _context.ShippingAgentOrganizations.ToList();
        var saoDtos = saos.Select(sao => sao.ToDTO()).ToList();
        return Ok(saoDtos);
    }

    [HttpPost(Name = "CreateShippingAgentOrganization")]
    public ActionResult<ShippingAgentOrganizationDto> Create(CreateShippingAgentOrganizationDto saoDto)
    {
        List<Designation> altNames = saoDto.AltNames?
            .Select(name => new Designation { Value = name })
            .ToList() ?? new List<Designation>();

        HashSet<Representative> representatives = _context.Representatives
            .Where(rep => saoDto.RepresentativesIds.Contains(rep.CitizenshipId))
            .ToHashSet();

        ShippingAgentOrganization sao = new ShippingAgentOrganization(
            Guid.NewGuid(),
            new Designation{Value = saoDto.Name},
            altNames,
            saoDto.Address,
            new TaxNumber{Value = saoDto.TaxNumber},
            representatives
        );
        var createdSAO = _context.ShippingAgentOrganizations.Add(sao);
        _context.SaveChanges();
        return CreatedAtRoute("GetShippingAgentOrganizations", new { id = createdSAO.Entity.Id }, createdSAO.Entity.ToDTO());
    }
}
