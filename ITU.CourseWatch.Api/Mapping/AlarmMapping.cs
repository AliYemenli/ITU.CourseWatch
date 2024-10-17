using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;

namespace ITU.CourseWatch.Api.Mapping;

public static class AlarmMapping
{
    public static AlarmSummaryDto ToAlarmSummaryDto(this Alarm alarm)
    {
        return new AlarmSummaryDto(
            alarm.Subscriber,
            alarm.Course.ToCourseSummaryDto()
        );
    }

}
