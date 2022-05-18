
public static class Programm
{
    public static IServiceProvider AppServices { get; private set; }
    public static WebApplicationBuilder AppWebBuilder { get; private set; }
    public static IHost AppHost { get; private set; }

    public static void Main(string[] args)
    {
        AppWebBuilder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        AppWebBuilder.Services.AddControllersWithViews();

        AppHost = AppWebBuilder.Build();
        AppServices = AppHost.Services;

        // Configure the HTTP request pipeline.
        if (!((WebApplication)AppHost).Environment.IsDevelopment())
        {
            ((WebApplication)AppHost).UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            ((WebApplication)AppHost).UseHsts();
        }

        ((WebApplication)AppHost).UseHttpsRedirection();
        ((WebApplication)AppHost).UseStaticFiles();

        ((WebApplication)AppHost).UseRouting();

        ((WebApplication)AppHost).UseAuthorization();

        ((WebApplication)AppHost).MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        AppHost.RunAsync();
        
        //mind own business
        
        AppHost.WaitForShutdown();
    }
}

