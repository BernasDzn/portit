using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Domain;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PhysicalResourceController : ControllerBase
{
    private readonly ApiContext _context;

    public PhysicalResourceController(ApiContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetAll")]
    public ActionResult<IEnumerable<object>> GetAll()
    {
        var allResources = new List<object>();

        allResources.AddRange(_context.PhysicalResources.OfType<STSCrane>().ToList());
        allResources.AddRange(_context.PhysicalResources.OfType<YardCrane>().ToList());
        allResources.AddRange(_context.PhysicalResources.OfType<Truck>().ToList());

        List<object> resourceDtos = new List<object>();
        foreach (var resource in allResources)
        {
            if (resource is STSCrane stsCrane) resourceDtos.Add(((IDTOAble<STSCraneDto>) stsCrane).ToDTO());
            else if (resource is YardCrane yardCrane) resourceDtos.Add(((IDTOAble<YardCraneDto>)yardCrane).ToDTO());
            else if (resource is Truck truck) resourceDtos.Add(((IDTOAble<TruckDto>)truck).ToDTO());
        }

        return Ok(resourceDtos);
    }


}
