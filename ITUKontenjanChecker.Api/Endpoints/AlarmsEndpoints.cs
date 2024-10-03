using System;
using System.ComponentModel.DataAnnotations;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Dtos;
using ITUKontenjanChecker.Api.Entities;
using ITUKontenjanChecker.Api.Mapping;
using ITUKontenjanChecker.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ITUKontenjanChecker.Api.Endpoints;

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

        group.MapPost("/", async (CreateAlarmDto newAlarm, KontenjanCheckerContext dbContext) =>
        {
            if (!IsValidEmail(newAlarm.Email))
            {
                return Results.BadRequest("Invalid email format.");
            }

            var course = await dbContext.Courses
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Crn == newAlarm.Crn);

            if (course is null)
            {
                return Results
                    .NotFound("There is no course for given CRN");
            }

            try
            {
                Alarm alarm = new Alarm
                {
                    Course = course,
                    Subscriber = newAlarm.Email
                };

                await dbContext.Alarms.AddAsync(alarm);
                await dbContext.SaveChangesAsync();

                await _mailService.SendRegisterNotificationAsync(alarm);

                return Results.Ok(alarm.ToAlarmSummaryDto());
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        return group;
    }
}
