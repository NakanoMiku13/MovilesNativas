using Microsoft.Extensions.Logging;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;
using Microsoft.Maui.LifecycleEvents;

#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#else
using Plugin.Firebase.Bundled.Platforms.Android;
#endif

namespace Practica5;

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
			})
			.ConfigureLifecycleEvents(events =>
			{
#if IOS
				events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => {
					CrossFirebase.Initialize(CreateCrossFirebaseSettings());
					return false;
				}));
#else
				events.AddAndroid(android => android.OnCreate((activity, _) =>
				{
					CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings());
				}));
#endif
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
		return builder.Build();
	}
    private static CrossFirebaseSettings CreateCrossFirebaseSettings()
    {
        return new CrossFirebaseSettings(
            isAuthEnabled: true,
            isCloudMessagingEnabled: true);
    }
}
