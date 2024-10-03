namespace ITU.CourseWatch.Api.Dtos;
using System;
using System.Text.Json.Serialization;

public class CourseResponseDto
{
    [JsonPropertyName("dersProgramList")]
    public List<CourseInnerResponseDto>? CourseList { get; set; }

    [JsonPropertyName("guncellenmeSaati")]
    public string? LastUpdatedTime { get; set; }
}


public class CourseInnerResponseDto
{
    [JsonPropertyName("dersTanimiId")]
    public int? DersTanimiId { get; set; }

    [JsonPropertyName("akademikDonemKodu")]
    public string? AkademikDonemKodu { get; set; }

    [JsonPropertyName("crn")]
    public required string Crn { get; set; }

    [JsonPropertyName("dersKodu")]
    public required string CourseCode { get; set; }

    [JsonPropertyName("dersBransKoduId")]
    public int BranchId { get; set; }

    [JsonPropertyName("dilKodu")]
    public string? DilKodu { get; set; }

    [JsonPropertyName("programSeviyeTipi")]
    public string? ProgramSeviyeTipi { get; set; }

    [JsonPropertyName("dersAdi")]
    public string? CourseName { get; set; }

    [JsonPropertyName("ogretimYontemi")]
    public string? OgretimYontemi { get; set; }

    [JsonPropertyName("adSoyad")]
    public string? Instructor { get; set; }

    [JsonPropertyName("mekanAdi")]
    public string? MekanAdi { get; set; }

    [JsonPropertyName("gunAdiTR")]
    public string? GunAdiTR { get; set; }

    [JsonPropertyName("gunAdiEN")]
    public string? GunAdiEN { get; set; }

    [JsonPropertyName("baslangicSaati")]
    public string? BaslangicSaati { get; set; }

    [JsonPropertyName("bitisSaati")]
    public string? BitisSaati { get; set; }

    [JsonPropertyName("webdeGoster")]
    public bool? WebdeGoster { get; set; }

    [JsonPropertyName("binaKodu")]
    public string? BinaKodu { get; set; }

    [JsonPropertyName("kontenjan")]
    public int Capacity { get; set; }

    [JsonPropertyName("ogrenciSayisi")]
    public int Enrolled { get; set; }

    [JsonPropertyName("programSeviyeTipiId")]
    public int? ProgramSeviyeTipiId { get; set; }

    [JsonPropertyName("rezervasyon")]
    public string? Rezervasyon { get; set; }

    [JsonPropertyName("sinifProgram")]
    public string? SinifProgram { get; set; }

    [JsonPropertyName("onSart")]
    public string? OnSart { get; set; }

    [JsonPropertyName("sinifOnsart")]
    public string? SinifOnsart { get; set; }
}