using Microsoft.EntityFrameworkCore;

namespace URlShortener.Extentions
{
    public static class MigrationExtentions
    {
        public static void ApplyMigration(this WebApplication webApplication)
        {
            using var scope = webApplication.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
