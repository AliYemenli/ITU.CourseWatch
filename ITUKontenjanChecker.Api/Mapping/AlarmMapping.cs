using System;
using ITUKontenjanChecker.Api.Dtos;
using ITUKontenjanChecker.Api.Entities;

namespace ITUKontenjanChecker.Api.Mapping;

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
