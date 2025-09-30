using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.Domain.Model;

public class Qualification : IDTOAble<QualificationDto>
{
	public Guid Id { get; set; }
	public Designation QualificationName { get; set; }
	ICollection<Staff> StaffReference { get; set; }

	//EF Core
	protected Qualification() { }

	public Qualification(Designation qualificationName)
	{
		Id = Guid.NewGuid();
		QualificationName = qualificationName;
	}

	public void UpdateQualificationName(string qualificationName)
	{
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