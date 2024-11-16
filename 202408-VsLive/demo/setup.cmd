@echo off

cd..

REM Define solution and project names
set solutionName=VSLiveDemo
set apiProjectName=Api
set sqlConnection=Data Source=127.0.0.1,1234;Database=trek;User id=AppUser;Password=P@ssw0rd!;

REM Create the solution 
dotnet new sln -n %solutionName%

REM Create the API project
dotnet new webapi -n %apiProjectName% -o %apiProjectName%
dotnet add %apiProjectName%\%apiProjectName%.csproj package HotChocolate.AspNetCore
dotnet add %apiProjectName%\%apiProjectName%.csproj package HotChocolate.AspNetCore.Subscriptions
dotnet add %apiProjectName%\%apiProjectName%.csproj package Microsoft.EntityFrameworkCore
dotnet add %apiProjectName%\%apiProjectName%.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet restore %apiProjectName%/%apiProjectName%.csproj
dotnet sln %solutionName%.sln add %apiProjectName%\%apiProjectName%.csproj

mkdir %apiProjectName%\Repository
call :CreateHrContext
call :CreateDepartmentClass
call :CreateEmployeeClass

REM start "%solutionName%.sln"

echo Demo setup complete!
exit /b

:CreateHrContext
> %apiProjectName%\Repository\HrContext.cs (
    echo using Microsoft.EntityFrameworkCore;
    echo.
    echo public class HrContext : DbContext
    echo {
    echo     private const string connectionString = "%sqlConnection%";
    echo.
    echo     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder^)
    echo     ^{
    echo         optionsBuilder.UseSqlServer(connectionString^);
    echo     ^}
    echo.
    echo     public DbSet^<Department^> Departments { get; set; };
    echo.
    echo     public DbSet^<Employee^> Employees { get; set; };
    echo.
    echo     public Employee ReorganizeEmployee(int employeeId, int departmentId^)
    echo     ^{
    echo         Database.ExecuteSqlRaw(
    echo             "EXEC [dbo].[ReorgEmployee] @EmployeeId = {0^}, @DepartmentId = {1^}", 
    echo             employeeId, departmentId^);
    echo.
    echo         return Employees.SingleOrDefault(e ^^^=^> e.Id == employeeId^)
    echo             ?? throw new InvalidOperationException($"Employee with ID {employeeId} not found."^);
    echo     ^}
    echo ^}
)
exit /b

:CreateDepartmentClass
> %apiProjectName%\Repository\Department.cs (
    echo using System.ComponentModel.DataAnnotations.Schema;
    echo.
    echo [Table("Department"^)]
    echo public record class Department(int Id, string Name^);
)
exit /b

:CreateEmployeeClass
> %apiProjectName%\Repository\Employee.cs (
    echo using System.ComponentModel.DataAnnotations.Schema;
    echo.
    echo [Table("Employee"^)]
    echo public record class Employee(int Id, string First, string Last, int DepartmentId^);
)
exit /b
