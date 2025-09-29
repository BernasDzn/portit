using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain.Model.Generic;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ShippingAgentOrganizationController : ControllerBase
{
    [HttpGet(Name = "GetShippingAgentOrganization")]
    public ActionResult<ShippingAgentOrganization> GetShippingAgentOrganization()
    {
        var rep = new ShippingAgentOrganizationRepresentative(
            Guid.NewGuid(),
            "Representante exemplo olá",
            123456789,
            "ola@example.com",
            "1234567890"
        );

        var sao = new ShippingAgentOrganization(
            Guid.NewGuid(),
            "Nome Legal Exemplo",
            new List<string> { "Nome Alternativo 1", "Nome Alternativo 2" },
            new Address("Rua Exemplo, 123", "Cidade Exemplo", "12345-678", "País Exemplo"),
            "1234-1234-1234",
            new List<ShippingAgentOrganizationRepresentative> { rep }
        );

        return Ok(sao);
    }
}
