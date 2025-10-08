namespace Api.Application.DataTransfer;

public class CargoManifestDto
{
    public ICollection<CargoTransportDto> Items { get; set; }
}