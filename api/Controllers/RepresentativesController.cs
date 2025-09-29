using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RepresentativesController : ControllerBase
{
    [HttpGet(Name = "GetRepresentative")]
    public ActionResult<ShippingAgentOrganizationRepresentative> GetRepresentative()
    {
        var rep = new ShippingAgentOrganizationRepresentative(
            Guid.NewGuid(),
            "Representante exemplo olá",
            123456789,
            "ola@example.com",
            "1234567890"
        );

        return Ok(rep);
    }
}
