using Microsoft.Extensions.Logging;
using Kmila.Shared.Services;
using Kmila.Shared.Data;
using Kmila.Services;

namespace Kmila;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        /*builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Filename={Path.Combine(FileSystem.AppDataDirectory, "app.db")}"));*/
        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddBlazorBootstrap();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
