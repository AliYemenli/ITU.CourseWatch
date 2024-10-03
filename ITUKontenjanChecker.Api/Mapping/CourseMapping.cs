using System;
using ITUKontenjanChecker.Api.Dtos;
using ITUKontenjanChecker.Api.Entities;

namespace ITUKontenjanChecker.Api.Mapping;

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
            coursesDtos.Add(ToCourseSummaryDto(course));
        }
        return coursesDtos;
    }
}
