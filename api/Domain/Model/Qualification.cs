using Domain.Model.Generic;

namespace Api.Domain.Model;

public class Qualification : IDTOAble<QualificationDto>
{
	public Guid Id { get; private set; }
	public string QualificationName { get ; private set; }

	//EF Core
	protected Qualification() { }

	public Qualification(Guid id, string qualificationName)
	{
		Id = id;
		QualificationName = qualificationName;
	}
	
	public void UpdateQualificationName(string qualificationName) {
		QualificationName = qualificationName;
	}

    public QualificationDto ToDTO()
	{
		return new QualificationDto
		{
			Id = this.Id,
			QualificationName = this.QualificationName
		};
	}

    internal static Qualification FromDTO(QualificationDto qualDto){
        return new Qualification(
			qualDto.Id,
			qualDto.QualificationName
		);
    }
}