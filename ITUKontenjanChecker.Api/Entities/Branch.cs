using System;

namespace ITUKontenjanChecker.Api.Entities;

public class Branch
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public required string BranchCode { get; set; }
}
