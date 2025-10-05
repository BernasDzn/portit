namespace Api.Domain.IRepository;

using Api.Domain.Entities;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

public interface IQualificationRepository : IGenericRepository<Qualification>
{
	Task<IEnumerable<Qualification>> GetQualificationsAsync();
	Task<Page<Qualification>> FilterQualificationsAsync(QualificationFilter filter);
	Task<Qualification?> GetQualificationByIdAsync(string id);
	new Task<Qualification> Add(Qualification qualification);
	Task<Qualification> Update(Qualification qualification);
}