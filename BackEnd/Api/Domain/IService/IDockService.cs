using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IDockService
{
    public Task<IEnumerable<DockDto>> GetDocks();
    public Task<Page<DockDto>> FilterDocks(DockFilter filter);
    public Task<DockDto?> Add(DockDto dockDto);
    public Task<DockDto?> Update(string name, DockDto dockDto);
}