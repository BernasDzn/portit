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
        throw new NotImplementedException();
    }
}
