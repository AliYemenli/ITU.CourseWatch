namespace ITU.CourseWatch.Api.Services;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Repository.BranchRepositories;
using Serilog;

public class BranchService
{
    private const string Url = "https://obs.itu.edu.tr/public/DersProgram/SearchBransKoduByProgramSeviye?programSeviyeTipiAnahtari=LS";
    private readonly IBranchRepository _branchRepository;
    public BranchService(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    private async Task<List<BranchDto>> FetchBranchesAsync()
    {
        using HttpClient client = new HttpClient();
        List<BranchDto> branches = new List<BranchDto>();

        var request = new HttpRequestMessage(HttpMethod.Get, Url);

        HttpResponseMessage? response = null;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception e)
        {
            Log.Error("[{Class}] An error occured while trying to send a request to get Branches. Exception: {Exception}", this, e.Message);
        }

        if (response is not null && response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    branches = JsonSerializer.Deserialize<List<BranchDto>>(content) ?? new List<BranchDto>();
                }
                catch (Exception e)
                {
                    Log.Error("[{Class}] Can not convert branches into BranchDto. Exception: {Exception}", this, e.Message);
                }
            }
        }
        else
        {
            Log.Warning(" [{Class}] There is an issue occured while consuming branch api. Response is null or not 200", this);
        }

        return branches;
    }

    private async Task<List<Branch>> FetchBranchEntitiesAsync()
    {
        List<Branch> entities = new List<Branch>();

        try
        {
            var branches = await FetchBranchesAsync();

            foreach (var branch in branches)
            {
                entities.Add(new Branch
                {
                    BranchId = branch.BranchId,
                    BranchCode = branch.BranchCode!
                });
            }


        }
        catch (Exception e)
        {
            Log.Error(" [{Class}] There is an error occured when trying to get branch entities list. May returned a empty list. Exception: {Exception}", nameof(BranchService), e.Message);
        }

        return entities;
    }


    public async Task RefreshBranchesAsync()
    {
        var newBranches = await FetchBranchEntitiesAsync();
        var tasks = new List<Task>();
        try
        {
            foreach (var branch in newBranches)
            {
                var existingBranch = await _branchRepository.GetAsync(branch);

                if (existingBranch is null)
                {
                    tasks.Add(_branchRepository.CreateAsync(branch));
                }
                else
                {
                    existingBranch.BranchCode = branch.BranchCode;
                    existingBranch.BranchId = branch.BranchId;
                    tasks.Add(_branchRepository.UpdateAsync(existingBranch));
                }
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            Log.Error(" [{Class}] Problem occured when trying to update branch table. Exception: {Exception}", this, e.Message);
        }
    }
}
