using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Runtime;
using Android.Net;
using Practica3.Repositories;
namespace Practica3;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Window.SetSoftInputMode(SoftInput.AdjustResize);
        //Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);

        Intent intent = new(this, typeof(AccelerometerRepository));
        StartIntent(intent);
    }
    private void StartIntent(Intent intent)
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
#pragma warning disable CA1416 // Validate platform compatibility
            StartForegroundService(intent);
#pragma warning restore CA1416 // Validate platform compatibility
        else
            StartService(intent);
    }

}
