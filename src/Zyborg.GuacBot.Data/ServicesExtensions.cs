using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Zyborg.GuacBot.GuacDB.Data
{
    public static class ServicesExtensions
    {
        public static void AddGuacDBContext(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<GuacDBContext>(options => options.UseMySQL(connectionString));
        }
    }
}