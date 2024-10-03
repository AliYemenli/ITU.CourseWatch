

namespace ITUKontenjanChecker.Api.Entities;

public class Alarm
{
    public int Id { get; set; }
    public required Course Course { get; set; }
    public required string Subscriber { get; set; }
}
