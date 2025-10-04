using Api.Domain.Model;
using Domain.Model.Generic;

public class VesselFilter : Pageable
{ 
    public string? Name { get; set; }
    public string? ImoNumber { get; set; }
}