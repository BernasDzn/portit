namespace Api.Application.DataTransfer.Filters;

using Api.Domain.Entities;
using Api.Infrastructure.Utilities;

public class StorageAreaFilter : Pageable
{
	public string? NameCode { get; set; }
}