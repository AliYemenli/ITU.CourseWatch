namespace ITU.CourseWatch.Api.Data;

using ITU.CourseWatch.Api.Entities;
using Microsoft.EntityFrameworkCore;

public class CourseWatchContext(DbContextOptions<CourseWatchContext> options)
    : DbContext(options)
{
    public DbSet<Alarm> Alarms => Set<Alarm>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Branch> Branches => Set<Branch>();

}
