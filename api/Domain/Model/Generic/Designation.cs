namespace Domain.Model.Generic;

public class Designation
{

	private string _designation;
	public string DesignationName
	{
		get { return _designation; }
	}

	public Designation(string designation)
	{
		_designation = designation;
	}

	public bool UpdateDesignation(string new_designation)
	{
		if (string.IsNullOrEmpty(new_designation))
		{
			return false;
		}

		_designation = new_designation;
		return true;
	}

}