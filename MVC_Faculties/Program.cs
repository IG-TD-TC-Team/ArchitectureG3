using MVC_Faculties.Services;

namespace MVC_Faculties;


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
            // Session will expire after 30 minutes of inactivity
            options.IdleTimeout = TimeSpan.FromMinutes(30);

            // Security: Prevent JavaScript access to session cookies
           options.Cookie.HttpOnly = true;

            // Mark session cookies as essential for GDPR compliance
            options.Cookie.IsEssential = true;

            // Use secure cookies in production (HTTPS only)
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            // Set a descriptive name for your session cookie
            options.Cookie.Name = "FacultiesSession";
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/PrintsystemAccess/Error");
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
            pattern: "{controller=AuthenticationAccess}/{action=Index}/{id?}");

        app.Run();
    }
}
