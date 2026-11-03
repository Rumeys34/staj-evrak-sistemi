
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
        .HasDiscriminator<string>("UserType")
        .HasValue<Student>("Student")
        .HasValue<Admin>("Admin");
    modelBuilder.Entity<DatasetStudent>()
        .HasIndex(s => s.StudentNo)
        .IsUnique();   
}

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<DatasetStudent> DatasetStudents => Set<DatasetStudent>();
    
}
