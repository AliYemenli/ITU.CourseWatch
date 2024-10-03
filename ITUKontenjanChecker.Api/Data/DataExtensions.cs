using System;
using ITUKontenjanChecker.Api.Dtos;
using ITUKontenjanChecker.Api.Entities;
using ITUKontenjanChecker.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ITUKontenjanChecker.Api.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KontenjanCheckerContext>();
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
