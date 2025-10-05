namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Application.DataTransfer;
using Api.Infrastructure.Persistence;

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
