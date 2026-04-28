using System;
using System.Windows.Forms;
using VetClinic.Data;
using VetClinic.Forms;

/*
 * ════════════════════════════════════════════════════════
 *  Veterinary Clinic – Windows Forms + EF Core (Code First)
 * ════════════════════════════════════════════════════════
 *
 * SETUP INSTRUCTIONS (run once):
 *
 *   1. Open Package Manager Console in Visual Studio:
 *      Tools → NuGet Package Manager → Package Manager Console
 *
 *   2. Create the initial migration:
 *      Add-Migration InitialCreate
 *
 *   3. Apply the migration and create / update the database:
 *      Update-Database
 *
 *   The database (VetClinicDB) will be created on:
 *      (localdb)\mssqllocaldb
 *
 *   Seed data (1 owner, 1 address, 1 animal) is applied automatically.
 *
 * REQUIRED NuGet packages (install via NuGet Package Manager):
 *   • Microsoft.EntityFrameworkCore            (9.x)
 *   • Microsoft.EntityFrameworkCore.SqlServer  (9.x)
 *   • Microsoft.EntityFrameworkCore.Tools      (9.x)
 *   • Microsoft.EntityFrameworkCore.Design     (9.x)
 */

namespace VetClinic
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ── Ensure database is created and migrations are applied ──────────
            // EnsureCreated() creates the DB from scratch if it doesn't exist.
            // For a migration workflow use: context.Database.Migrate() instead.
            try
            {
                using var context = new VetClinicContext();
                context.Database.EnsureCreated();
                // Alternative (after running Add-Migration):
                // context.Database.Migrate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not connect to the database.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new DashboardForm());
        }
    }
}