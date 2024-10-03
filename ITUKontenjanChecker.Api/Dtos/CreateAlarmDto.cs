using System.ComponentModel.DataAnnotations;

namespace ITUKontenjanChecker.Api.Dtos;

public record class CreateAlarmDto(
    [Required] string Crn,
    [Required] string Email
);
