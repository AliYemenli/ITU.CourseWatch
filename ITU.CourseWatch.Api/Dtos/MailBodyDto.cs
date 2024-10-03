namespace ITU.CourseWatch.Api.Dtos;

public record class MailBodyDto(
    string Reciever,
    string Subject,
    string Body
);