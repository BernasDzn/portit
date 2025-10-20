using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IDockService
{
    public Task<IEnumerable<DockDto>> GetDocks();
    public Task<DockDto?> GetByCode(string code);
    public Task<Page<DockDto>> FilterDocks(DockFilter filter);
    public Task<DockDto?> Add(CreateDockDto dockDto);
    public Task<DockDto?> Update(string name, CreateDockDto dockDto);
}