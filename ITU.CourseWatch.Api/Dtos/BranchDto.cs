using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITU.CourseWatch.Api.Dtos;


public class BranchDto
{
    [JsonPropertyName("bransKoduId")]
    public int BranchId { get; set; }
    [JsonPropertyName("dersBransKodu")]
    public string? BranchCode { get; set; }

}
