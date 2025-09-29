namespace Domain.Model.Generic;

public class Designation
{

	private string _designation;
	public string Value { get => _designation; set => _designation = value; }

	//EF Core
	private Designation() { }

	public Designation(string designation)
	{
		_designation = designation;
	}
}