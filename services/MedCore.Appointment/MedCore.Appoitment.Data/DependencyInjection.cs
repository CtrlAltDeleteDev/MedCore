using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedCore.Appoitment.Data
{
    public static class DependencyInjection
    {
        public static void AddAppointmentData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppoitmentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}