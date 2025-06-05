using MVC_POS.Services;

namespace MVC_POS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>();
            builder.Services.AddHttpClient<IBalanceService, BalanceService>();

            builder.Services.AddSession(options =>
            {
             
                // POS operations are typically quick, so shorter sessions improve security
                options.IdleTimeout = TimeSpan.FromMinutes(15);

                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.Name = "POSSession";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Authentication/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AuthenticationAccess}/{action=index}/{id?}");

            app.Run();
        }
    }
}
