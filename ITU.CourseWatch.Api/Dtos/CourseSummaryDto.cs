namespace ITU.CourseWatch.Api.Dtos;

public record class CourseSummaryDto(
    string Crn,
    string Code,
    string Name,
    string Instructor,
    string Capacity,
    string Enrolled,
    string BranchCode
);

