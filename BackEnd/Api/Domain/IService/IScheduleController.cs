using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface ISchedulingService
{
    Task<ActionResult<IEnumerable<QualificationDto>>> Schedule(DateTime date);
}