using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace Examen.Platforms.Android;

[Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeLocation)]
public class LocationForegroundService : Service
{
    private const string CHANNEL_ID = "location_tracking_channel";
    private const int NOTIFICATION_ID = 1001;

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        var notification = CreateNotification();
        StartForeground(NOTIFICATION_ID, notification);
        return StartCommandResult.Sticky;
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                CHANNEL_ID,
                "Rastreo de Ubicacion",
                NotificationImportance.Low)
            {
                Description = "Servicio de rastreo GPS activo"
            };

            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    private Notification CreateNotification()
    {
        var pendingIntentFlags = PendingIntentFlags.UpdateCurrent;
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            pendingIntentFlags |= PendingIntentFlags.Immutable;
        }

        var intent = new Intent(this, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(this, 0, intent, pendingIntentFlags);

        return new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("Rastreo GPS Activo")
            .SetContentText("Tu ubicacion esta siendo rastreada")
            .SetSmallIcon(Resource.Drawable.dotnet_bot)
            .SetContentIntent(pendingIntent)
            .SetOngoing(true)
            .Build();
    }

    public override void OnDestroy()
    {
        StopForeground(StopForegroundFlags.Remove);
        base.OnDestroy();
    }
}
