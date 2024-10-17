using System;
using System.ComponentModel.DataAnnotations;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Mapping;
using ITU.CourseWatch.Api.Repository.AlarmRepositories;
using ITU.CourseWatch.Api.Repository.CourseRepositories;
using ITU.CourseWatch.Api.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ITU.CourseWatch.Api.Endpoints;

public static class AlarmsEndpoints
{
    private static readonly MailService _mailService = new MailService();
    private static bool IsValidEmail(string email)
    {
        return new EmailAddressAttribute().IsValid(email);
    }
    public static RouteGroupBuilder MapAlarmsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/alarms")
            .WithParameterValidation();

        group.MapPost("/", async (CreateAlarmDto newAlarm, IAlarmRepository alarmRepository, ICourseRepository courseRepository) =>

        {
            if (!IsValidEmail(newAlarm.Email))
            {
                return Results.BadRequest("Invalid email format.");
            }

            var course = await courseRepository
                        .GetAsync(newAlarm.Crn);

            if (course is null)
            {
                return Results
                    .NotFound("There is no course for given CRN");
            }

            if (course.Capacity > course.Enrolled)
            {
                return Results
                    .Conflict("Course you provided is already available for registering.");
            }

            try
            {
                Alarm alarm = new Alarm
                {
                    Course = course,
                    Subscriber = newAlarm.Email
                };

                await Task.WhenAll(
                        alarmRepository.CreateAsync(alarm),
                        _mailService.SendRegisterNotificationAsync(alarm)
                        );

                Log.Information("Alarm created for subscriber: {Email}, CRN: {Crn}", newAlarm.Email, newAlarm.Crn);  // Log success


                return Results.Ok(new
                {
                    Message = alarm.ToAlarmSummaryDto(),
                    Data = alarm.ToAlarmSummaryDto()
                });
            }
            catch (Exception e)
            {
                Log.Error(" [AlarmAPI] An error occured.  Exception: {Exception}", e.Message);
                return Results.Problem("Internal Server Error", statusCode: 500);
            }
        });

        return group;
    }
}
