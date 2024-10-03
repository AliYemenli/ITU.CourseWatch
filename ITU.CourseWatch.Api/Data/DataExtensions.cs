using ITU.CourseWatch.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ITU.CourseWatch.Api.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CourseWatchContext>();
        dbContext.Database.Migrate();
    }

    public static List<CourseDto> GetAsCourseDto(this CourseResponseDto courses)
    {
        List<CourseDto> coursesDto = new List<CourseDto>();
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
        return coursesDto;
    }


}
