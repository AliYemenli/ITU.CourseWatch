using System;
using ITU.CourseWatch.Api.Entities;

namespace ITU.CourseWatch.Api.Repository.CourseRepositories;

public interface ICourseRepository
{
    Task<Course?> GetAsync(Course course);
    Task<Course?> GetAsync(string crn);
    Task<IEnumerable<Course>> GetAllAsync();
    Task CreateAsync(Course course);
    Task UpdateAsync(Course course);
    Task CreateBulkAsync(IEnumerable<Course> courses);
    Task UpdateBulkAsync(IEnumerable<Course> courses);
}
