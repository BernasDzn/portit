using Api.Domain.Model;
using Domain.Model.Generic;

public class QualificationFilter : Pageable
{ 
    public string? Code { get; set; }
    public string? QualificationName { get; set; }
}