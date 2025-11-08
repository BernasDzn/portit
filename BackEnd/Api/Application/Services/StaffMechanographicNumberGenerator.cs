
using Api.Application.DataTransfer.Filters;
using Api.Domain.IRepository;

public class StaffMechanographicNumberGenerator
{

	private string id = "STF";

	IStaffRepository _staffRepository;

	public StaffMechanographicNumberGenerator(IStaffRepository staffRepository)
	{
		_staffRepository = staffRepository;
	}

	public async Task<string> GenerateMechanographicNumber()
	{
		string year = DateTime.Now.Year.ToString().Substring(2, 2);
		StaffFilter yearFilter = new StaffFilter{MechanographicNumber = $"STF{year}", PageNumber=1, PageSize=10000};
		int numOfStaff = (await _staffRepository.FilterStaffsAsync(yearFilter)).Items.Count;
		id += year + (numOfStaff + 1).ToString("D4");
		return id;
	}

}