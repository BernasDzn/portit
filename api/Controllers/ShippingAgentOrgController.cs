using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain.Model.Generic;
using Domain;
using DAL;

namespace Api.Controllers;

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
}
