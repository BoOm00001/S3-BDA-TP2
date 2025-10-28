using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Vols.Data
{
    public class VolsContextFactory : IDesignTimeDbContextFactory<VolsContext>
    {
        public VolsContext CreateDbContext(string[] args) => Create();

        public static VolsContext Create()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var cs = config.GetConnectionString("VolsDbConnString");
            var options = new DbContextOptionsBuilder<VolsContext>()
                .UseSqlServer(cs)
                .Options;

            return new VolsContext(options);
        }
    }
}
