using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;
using Serilog;

namespace ITU.CourseWatch.Api.Services;

public class MailService
{
    private MailSettingsDto _mailSettings;

    public MailService()
    {
        _mailSettings = GetMailSettings();
    }

    private MailSettingsDto GetMailSettings()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        MailSettingsDto mailSettings = configuration
            .GetSection("MailSettings")
            .Get<MailSettingsDto>()!;

        return mailSettings;
    }
    private async Task SendEmailAsync(MailBodyDto mailBodyModel)
    {
        try
        {
            using MailMessage mail = new MailMessage()
            {
                From = new MailAddress(_mailSettings.Address!),
                Subject = mailBodyModel.Subject,
                Body = mailBodyModel.Body,
                IsBodyHtml = true
            };

            mail.To.Add(mailBodyModel.Reciever);

            using SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_mailSettings.Address, _mailSettings.Password),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mail);
        }
        catch (Exception e)
        {
            Log.Error(" [{Class}] Error with the email service, can not send the mail. Exception:. Exception: {Exception}", this, e.Message);
        }
    }

    public bool IsValidEmail(CreateAlarmDto newAlarm)
    {
        return new EmailAddressAttribute().IsValid(newAlarm.Email);
    }
    public async Task SendAlarmMailAsync(Alarm alarm)
    {
        await SendEmailAsync(new MailBodyDto(
            alarm.Subscriber,
            "Course Availability Notification",
            GetAlarmBody(alarm)));

        Log.Information(" [{Class}] Sent alarm for user {Subscriber}", this, alarm.Subscriber);

    }

    public async Task SendRegisterNotificationAsync(Alarm alarm)
    {
        await SendEmailAsync(new MailBodyDto(
            alarm.Subscriber,
            "Alarm Registration Confirmation",
            GetRegisterBody(alarm)));

        Log.Information(" [{Class}] Sent register notification for user {Subscriber}", this, alarm.Subscriber);
    }

    private string GetAlarmBody(Alarm alarm)
    {
        return $@"
        <h1>Course Availability Notification</h1>
        <p>We are pleased to inform you that the course you subscribed to is now available:</p>
        <ul>
            <li><strong>Course Name:</strong> {alarm.Course.Name}</li>
            <li><strong>Course Code (CRN):</strong> {alarm.Course.Crn}</li>
            <li><strong>Instructor:</strong> {alarm.Course.Instructor}</li>
            <li><strong>Available Seats:</strong> {alarm.Course.Capacity - alarm.Course.Enrolled}</li>
        </ul>
        <p>Hurry up and register as soon as possible, because you might not the only person that I notify :)</p>
        <p>Thank you for using our service.</p>
    ";
    }

    private string GetRegisterBody(Alarm alarm)
    {
        return
        $@"
        <h1>Alarm Registration Confirmation</h1>
        <p>You have successfully registered for course availability notifications. We will notify you as soon as a seat becomes available for the following course:</p>
        <ul>
            <li><strong>Course Name:</strong> {alarm.Course.Name}</li>
            <li><strong>Course Code (CRN):</strong> {alarm.Course.Crn}</li>
            <li><strong>Instructor:</strong> {alarm.Course.Instructor}</li>
        </ul>
        <p>Thank you for using our service, and we hope you find it helpful!</p>
    ";
    }
}
