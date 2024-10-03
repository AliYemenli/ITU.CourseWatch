namespace ITUKontenjanChecker.Api.Services;
using System;
using System.Text.Json;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Dtos;
using ITUKontenjanChecker.Api.Entities;
using Microsoft.EntityFrameworkCore;

public class BranchService
{
    private const string _url = "https://obs.itu.edu.tr/public/DersProgram/SearchBransKoduByProgramSeviye?programSeviyeTipiAnahtari=LS";

    private async Task<List<BranchDto>> GetBranchesAsync()
    {
        using HttpClient client = new HttpClient();
        List<BranchDto> branches = new List<BranchDto>();

        var request = new HttpRequestMessage(HttpMethod.Get, _url);

        HttpResponseMessage? response = null;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception e)
        {
            throw new Exception("API request failed. Exception: " + e.Message);
        }

        if (response is not null && response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                branches = JsonSerializer.Deserialize<List<BranchDto>>(content) ?? new List<BranchDto>();
            }
        }

        return branches;
    }

    public Task<List<Branch>> GetBranchesAsEntities()
    {
        List<Branch> entities = new List<Branch>();
        var branches = GetBranchesAsync().Result;

        foreach (var branch in branches)
        {
            entities.Add(new Branch
            {
                BranchId = branch.BranchId,
                BranchCode = branch.BranchCode!
            });
        }

        return Task.FromResult(entities);
    }


    public async Task UpdateBranchesAsync(KontenjanCheckerContext dbContext)
    {
        var NewBranches = await GetBranchesAsEntities();

        foreach (var branch in NewBranches)
        {
            var existingBranch = await dbContext.Branches
                .FirstOrDefaultAsync(b => b.BranchId == branch.BranchId);

            if (existingBranch is null)
            {
                dbContext.Branches.Add(branch);
            }
            else
            {
                existingBranch.Equals(branch);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
