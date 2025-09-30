namespace Domain.IRepository;

using Api.Domain.Model;

public interface IQualificationRepository : IGenericRepository<Qualification>
{
	Task<IEnumerable<Qualification>> GetQualificationsAsync();

	Task<Qualification> GetQualificationByNameAsync(string name);

	new Task<Qualification> Add(Qualification qualification);

	Task<bool> Update(Qualification qualification, List<string> errorMessage);

	Task<bool> QualificationExists(string name);
}