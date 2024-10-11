using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;

namespace ITU.CourseWatch.Api.Mapping;

public static class CourseMapping
{
    public static CourseSummaryDto ToCourseSummaryDto(this Course course)
    {
        return new CourseSummaryDto(
            course.Crn,
            course.Code,
            course.Name,
            course.Instructor,
            course.Capacity.ToString(),
            course.Enrolled.ToString(),
            course.Branch.BranchCode
        );
    }

    public static List<CourseSummaryDto> ToCourseSummaryDto(this List<Course> courses)
    {
        List<CourseSummaryDto> coursesDtos = new List<CourseSummaryDto>();
        foreach (var course in courses)
        {
            coursesDtos.Add(course.ToCourseSummaryDto());
        }
        return coursesDtos;
    }

    public static async Task<List<CourseDto>> ToCourseDtoListAsync(this CourseResponseDto courses)
    {
        List<CourseDto> coursesDto = new List<CourseDto>();

        await Task.Run(() =>
        {
            foreach (var course in courses.CourseList!)
            {
                coursesDto.Add(new CourseDto
                {
                    Crn = course.Crn,
                    CourseCode = course.CourseCode,
                    CourseName = course.CourseName!,
                    CourseInstructor = course.Instructor!,
                    CourseCapacity = course.Capacity,
                    CourseEnrolled = course.Enrolled,
                    CourseBranchId = course.BranchId,
                });
            }
        });

        return coursesDto;
    }
}
