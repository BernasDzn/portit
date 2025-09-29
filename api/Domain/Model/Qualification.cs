namespace Api.Domain.Model;

public class Qualification : IQualification
{
	public long id;

	private string _qualification_name;
	public string QualificationName
	{
		get { return _qualification_name; }
	}

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