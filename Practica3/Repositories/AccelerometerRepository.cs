using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using AndroidX.Core.App;

namespace Practica3.Repositories;
[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
public class AccelerometerRepository : Service, IDisposable
{
    public override IBinder OnBind(Intent? intent) => default!;
    private readonly CancellationTokenSource _cts = new();
    public static Action<double>? OnAccelerationChanged;
    public static Action<(double, double, double)>? OnAccelerometerChanged;    
    public AccelerometerRepository()
    {
        
    }
    public override void OnCreate()
    {
        #pragma warning disable CA1416 // Validate platform compatibility
        var channel = new NotificationChannel("foreground_service_channel",
                "Foreground Service",
                NotificationImportance.Default);
#pragma warning restore CA1416 // Validate platform compatibility
        var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
        if (notificationManager != null)
#pragma warning disable CA1416 // Validate platform compatibility
            notificationManager.CreateNotificationChannel(channel);
#pragma warning restore CA1416 // Validate platform compatibility
        Accelerometer.ReadingChanged += HandleAccelerometer;
        try
        {
            if (!Accelerometer.IsMonitoring)
                Accelerometer.Start(SensorSpeed.UI);
        }
        catch (Exception ex)
        {
            Log.Warn("[Down man]", ex.Message);
        }
    }
    private void HandleAccelerometer(object? sender, AccelerometerChangedEventArgs e)
    {
        AccelerometerData data = e.Reading;
        double acceleration = Math.Sqrt(Math.Pow(data.Acceleration.X, 2) + Math.Pow(data.Acceleration.Y, 2) + Math.Pow(data.Acceleration.Z, 2));
        OnAccelerationChanged?.Invoke(acceleration);
        OnAccelerometerChanged?.Invoke((data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z));
    }
    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        var iconId = Android.Resource.Drawable.IcMenuInfoDetails;
        // Build a notification for the service
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var notification = new NotificationCompat.Builder(this, "foreground_service_channel")
            .SetContentTitle("Accelerometer service")
            .SetContentText("ESCOM Moviles nativas Practica 3")
            .SetSmallIcon(iconId) // Use an app icon here
            .Build();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        // Start the service in foreground mode
        StartForeground(1, notification);
        //Class1 class1 = new();
        //class1.Start();
        Task.Run(WaitTime, _cts.Token);
        return StartCommandResult.Sticky;
    }
    private async Task WaitTime()
    {
        try
        {
            await Task.Delay(Timeout.Infinite, _cts.Token);
        }catch{}
    }
    public override void OnDestroy()
    {
        _cts.Cancel();
        Accelerometer.ReadingChanged -= HandleAccelerometer;
    }

}