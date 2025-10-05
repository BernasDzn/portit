namespace Api.Application.DataTransfer.Filters;

using Api.Infrastructure.Utilities;

public class QualificationFilter : Pageable
{
    public string? Code { get; set; }
    public string? QualificationName { get; set; }
}