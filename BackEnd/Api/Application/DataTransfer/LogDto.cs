public class LogDto
{
    public required string Timestamp { get; set; }
    public required string RequestId { get; set; } = string.Empty;
    public required string Level { get; set; } = string.Empty;
    public required string Message { get; set; } = string.Empty;
}