using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Microsoft.AspNetCore.Mvc;

public interface IDockController
{
    public Task<ActionResult<IEnumerable<DockDto>>> GetAll();
    public Task<ActionResult<IEnumerable<DockDto>>> Filter([FromQuery] DockFilter filter);
    public Task<ActionResult<DockDto>> Create(DockDto dockDto);
    public Task<ActionResult<DockDto>> Update(string name, DockDto dockDto);
}