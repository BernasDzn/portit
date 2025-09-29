namespace Api.Domain.Model;

public class Qualification : IQualification
{
	public long id;

	private string _qualification_name;
	public string QualificationName { get => _qualification_name; set => _qualification_name = value; }

	//EF Core
	private Qualification() { }

	public Qualification(string qualification_name)
	{
		_qualification_name = qualification_name;
	}

	public bool UpdateName(string new_qualification_name)
	{
		if (string.IsNullOrEmpty(new_qualification_name))
		{
			return false;
		}

		_qualification_name = new_qualification_name;
		return true;
	}

}