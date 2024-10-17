namespace ITU.CourseWatch.Api.Services;
using ITU.CourseWatch.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Data;
using Serilog;
using System.ComponentModel.DataAnnotations;
using ITU.CourseWatch.Api.Repository.AlarmRepositories;

public class AlarmService
{
    private readonly IAlarmRepository _alarmRepository;
    private readonly MailService _mailService;


    public AlarmService(IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
        _mailService = new MailService();
    }

    public async Task HandleAlarmsAsync()
    {
        var alarms = _alarmRepository.GetAvailablesAsync();
        var taskList = new List<Task>();

        try
        {
            foreach (var alarm in await alarms)
            {
                taskList.Add(_mailService.SendAlarmMailAsync(alarm).ContinueWith(async (finishedTask) =>
                {

                    await _alarmRepository.DeleteAsync(alarm);

                }));
            }

            await Task.WhenAll(taskList);
        }
        catch (Exception e)
        {
            Log.Fatal(" [{Class}] Error occured. Exception:. Exception: {Exception}", this, e.Message);
        }
    }



}
