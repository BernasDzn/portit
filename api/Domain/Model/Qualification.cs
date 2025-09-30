using Domain.Model.Generic;

namespace Api.Domain.Model;

public class Qualification : IDTOAble<QualificationDto>
{
	public Guid Id { get; private set; }
	public string QualificationName { get ; private set; }
	ICollection<Staff> StaffReference { get; set; }

	//EF Core
	protected Qualification() { }

	public Qualification(string qualificationName)
	{
		Id = Guid.NewGuid();
		QualificationName = qualificationName;
	}
	
	public void UpdateQualificationName(string qualificationName) {
		QualificationName = qualificationName;
	}

    public QualificationDto ToDTO()
	{
		return new QualificationDto
		{
			QualificationName = this.QualificationName
		};
	}
}