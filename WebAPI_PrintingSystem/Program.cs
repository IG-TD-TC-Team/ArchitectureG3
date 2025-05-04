
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IAuthentificationHelper, AuthenticationHelper>();
            builder.Services.AddScoped<IBalanceHelper, BalanceHelper>();

            // Register the database context with dependency injection
            builder.Services.AddDbContext<PrintingSystemContext>(options =>
                options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PrintingSystemDB;Trusted_Connection=True"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
