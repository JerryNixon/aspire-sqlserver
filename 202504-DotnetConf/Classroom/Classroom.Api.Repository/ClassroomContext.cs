using Classroom.Poco;

using Microsoft.EntityFrameworkCore;

namespace Classroom.Api.Repository;

internal class ClassroomContext : DbContext
{
    public DbSet<StudentPoco> Students => Set<StudentPoco>();
    public DbSet<ClassPoco> Classes => Set<ClassPoco>();
    public DbSet<AttendancePoco> Attendance => Set<AttendancePoco>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=localhost;Database=Classroom;User Id=sa;Password=P@ssw0rd!;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClassPoco>().HasData(
            new ClassPoco { Id = 1, Name = "Math" },
            new ClassPoco { Id = 2, Name = "English" },
            new ClassPoco { Id = 3, Name = "Science" }
        );

        modelBuilder.Entity<StudentPoco>().HasData(
            new StudentPoco { Id = 1, Name = "Alice", ClassId = 1 },
            new StudentPoco { Id = 2, Name = "Bob", ClassId = 1 },
            new StudentPoco { Id = 3, Name = "Charlie", ClassId = 1 },
            new StudentPoco { Id = 4, Name = "Daisy", ClassId = 2 },
            new StudentPoco { Id = 5, Name = "Ethan", ClassId = 2 },
            new StudentPoco { Id = 6, Name = "Fiona", ClassId = 2 },
            new StudentPoco { Id = 7, Name = "George", ClassId = 3 },
            new StudentPoco { Id = 8, Name = "Hannah", ClassId = 3 },
            new StudentPoco { Id = 9, Name = "Isaac", ClassId = 3 },
            new StudentPoco { Id = 10, Name = "Jill", ClassId = 3 }
        );
    }
}
