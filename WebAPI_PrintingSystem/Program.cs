using DAL;
using Microsoft.EntityFrameworkCore;
using WebAPI_PrintingSystem.Business;

namespace WebAPI_PrintingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IAuthentificationHelper, AuthenticationHelper>();
            builder.Services.AddScoped<IBalanceHelper, BalanceHelper>();

            // Register the database context with Azure SQL connection
            builder.Services.AddDbContext<PrintingSystemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            //Enable Swagger in all environments for testing
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Printing System API V1");
                c.RoutePrefix = "swagger"; // Swagger will be available at /swagger
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}