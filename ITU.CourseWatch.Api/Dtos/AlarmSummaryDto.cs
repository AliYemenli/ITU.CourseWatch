namespace ITU.CourseWatch.Api.Dtos;

public record class AlarmSummaryDto(
    string Subscriber,
    CourseSummaryDto CourseSummary
);
