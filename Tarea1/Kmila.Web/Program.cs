using Kmila.Web.Components;
using Kmila.Shared.Services;
using Kmila.Web.Services;
using Kmila.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmila;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Add device-specific services used by the Kmila.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=app.db")
        );

        builder.Services.AddBlazorBootstrap();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(Kmila.Shared._Imports).Assembly);

        app.Run();
    }
}
