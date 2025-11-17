public class PortChunk
{
	public string Name { get; set; }
	public ChunkType Type { get; set; }
	public float X { get; set; }
	public float Y { get; set; }

	public PortChunk(string name, ChunkType type, float x, float y)
	{
		this.Name = name;
		this.Type = type;
		this.X = x;
		this.Y = y;
	}
}

public enum ChunkType
{
	Land,
	Dock,
	Warehouse,
	Yard,
	STSCrane,
	YardCrane,
}