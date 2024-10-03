using System;

namespace ITU.CourseWatch.Api.Dtos;

public class CourseDto
{
    public required string Crn { get; set; }
    public required string CourseCode { get; set; }
    public required string CourseName { get; set; }
    public required string CourseInstructor { get; set; }
    public int CourseCapacity { get; set; }
    public int CourseEnrolled { get; set; }
    public int CourseBranchId { get; set; }
}
