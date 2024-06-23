using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public IConfiguration Configuration { get; }
    public ApiSettings ApiSettings { get; private set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        ApiSettings = new ApiSettings();
        Configuration.GetSection("ApiSettings").Bind(ApiSettings);
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton(ApiSettings);
        services.AddSingleton<GoogleMapsService>();
        services.AddSingleton<GeocodeService>();
        services.AddTransient<RouteOptimizer>();
        
         services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost3000",
                builder => builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("AllowLocalhost3000");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

