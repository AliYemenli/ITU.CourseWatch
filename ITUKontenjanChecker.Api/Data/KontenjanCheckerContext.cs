namespace ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Entities;
using Microsoft.EntityFrameworkCore;

public class KontenjanCheckerContext(DbContextOptions<KontenjanCheckerContext> options)
    : DbContext(options)
{
    public DbSet<Alarm> Alarms => Set<Alarm>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Branch> Branches => Set<Branch>();

}
