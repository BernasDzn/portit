using Domain.Model.Generic;

namespace Api.Domain.Model;

public class Qualification : IDTOAble<QualificationDto>
{
	public Guid Id { get; private set; }
	public Designation QualificationName { get ; private set; }
	ICollection<Staff> StaffReference { get; set; }

	//EF Core
	protected Qualification() { }

	public Qualification(Guid id, string qualificationName)
	{
		Id = id;
		QualificationName = new Designation { Value = qualificationName };
	}
	
	public void UpdateQualificationName(string qualificationName) {
		QualificationName = new Designation { Value = qualificationName };
	}

    public QualificationDto ToDTO()
	{
		return new QualificationDto
		{
			QualificationName = this.QualificationName.Value
		};
	}
}