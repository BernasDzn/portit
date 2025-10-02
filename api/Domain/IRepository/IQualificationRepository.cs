namespace Domain.IRepository;

using Api.Domain.Model;

public interface IQualificationRepository : IGenericRepository<Qualification>
{
	Task<IEnumerable<Qualification>> GetQualificationsAsync();
	Task<Qualification?> GetQualificationByNameAsync(string name);
	Task<Qualification?> GetQualificationByIdAsync(string id);

	new Task<Qualification> Add(Qualification qualification);
	Task<Qualification> Update(Qualification qualification);
}