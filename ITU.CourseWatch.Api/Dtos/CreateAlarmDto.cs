using System.ComponentModel.DataAnnotations;

namespace ITU.CourseWatch.Api.Dtos;

public record class CreateAlarmDto(
    [Required] string Crn,
    [Required] string Email
);
