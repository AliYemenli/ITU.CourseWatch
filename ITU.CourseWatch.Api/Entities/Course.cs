
namespace ITU.CourseWatch.Api.Entities;

public class Course
{
    public int Id { get; set; }
    public required string Crn { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Instructor { get; set; }
    public int Capacity { get; set; }
    public int Enrolled { get; set; }
    public int BranchId { get; set; }
    public required Branch Branch { get; set; }
}
