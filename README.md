# VetClinic



4. How to Transfer/Restore on a New Device
When you download the project on a new computer, follow these three steps to bring it back to life:

NuGet Restore: Right-click the Solution in Visual Studio and select Restore NuGet Packages. This downloads EF Core and SQL Server support automatically.

Update Connection String: If the new computer has a different SQL Server instance name, you may need to tweak the optionsBuilder.UseSqlServer line in ClinicContext.cs.

Run Migrations: Open the Package Manager Console and run:
Update-Database
