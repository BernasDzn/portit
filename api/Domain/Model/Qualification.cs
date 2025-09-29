namespace Api.Domain.Model;

public class Qualification : IDTOAble<QualificationDto>
{
	public Guid Id { get; private set; }
	public string QualificationName { get ; private set; }

	//EF Core
	protected Qualification() { }

	public Qualification(Guid id, string qualification_name)
	{
		Id = id;
		QualificationName = qualification_name;
	}
	
	public bool UpdateName(string new_qualification_name)
	{
		if (string.IsNullOrWhiteSpace(new_qualification_name))
			return false;

		QualificationName = new_qualification_name;
		return true;
	}

    public QualificationDto ToDTO()
    {
        return new QualificationDto
		{
			Id = this.Id,
			QualificationName = this.QualificationName
		};
    }
}