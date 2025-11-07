using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Microsoft.AspNetCore.Mvc;

public interface IAdminController
{
    public Task<ActionResult<IEnumerable<LogDto>>> AuditLogs();
}