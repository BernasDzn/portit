namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;


public class Qualification : IDTOAble<QualificationDto>
{
	public Guid Id { get; private set; }
	public Code NameCode { get; private set; }
	public Designation QualificationName { get; private set; }
	public virtual ICollection<Staff> Staffs { get; private set; }
	public virtual ICollection<PhysicalResource> PhysicalResources { get; private set; }

	//EF Core
	protected Qualification() { }

	public Qualification(Guid id, Code idCode, Designation qualificationName)
	{
		Id = id;
		NameCode = idCode ?? throw new ArgumentNullException(nameof(idCode));
		QualificationName = qualificationName ?? throw new ArgumentNullException(nameof(qualificationName));
	}

	public void UpdateQualificationName(string qualificationName)
	{
		QualificationName = new Designation { Value = qualificationName };
	}

	public QualificationDto ToDTO()
	{
		return new QualificationDto
		{
			IdCode = this.NameCode.Value,
			QualificationName = this.QualificationName.Value
		};
	}

    public override bool Equals(object? obj)
    {
        return obj is Qualification q && q.NameCode.Value == NameCode.Value;
    }

    public override int GetHashCode()
    {
        return NameCode.Value.GetHashCode();
    }
	public override string ToString()
	{
		return $"Qualification [Id={Id}, NameCode={NameCode.Value}, QualificationName={QualificationName.Value}]";
	}
}