using Examen.Models;
using System.Globalization;

namespace Examen.Services;

public class LocationTrackingService
{
    private readonly LocationStorageService _storage;
    private CancellationTokenSource? _cts;
    private bool _isTracking = false;
    private string? _lastBackendError;

    public event Action<LocationRecord>? OnLocationUpdated;
    public event Action<bool>? OnTrackingStateChanged;
    public event Action<string>? OnError;

    public bool IsTracking => _isTracking;
    public LocationRecord? CurrentLocation { get; private set; }
    public string? LastBackendError => _lastBackendError;

    public LocationTrackingService(LocationStorageService storage)
    {
        _storage = storage;
    }

    public async Task<bool> RequestPermissionsAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                var bgStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (bgStatus != PermissionStatus.Granted)
                {
                    bgStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
                }
            }

            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            OnError?.Invoke($"Error de permisos: {ex.Message}");
            return false;
        }
    }

    public async Task StartTrackingAsync()
    {
        if (_isTracking) return;

        var hasPermission = await RequestPermissionsAsync();
        if (!hasPermission)
        {
            OnError?.Invoke("Permisos de ubicacion no concedidos");
            return;
        }

        _isTracking = true;
        _cts = new CancellationTokenSource();
        OnTrackingStateChanged?.Invoke(true);

#if ANDROID
        StartAndroidForegroundService();
#endif

        _ = TrackingLoopAsync(_cts.Token);
    }

    public void StopTracking()
    {
        _isTracking = false;
        _cts?.Cancel();
        OnTrackingStateChanged?.Invoke(false);

#if ANDROID
        StopAndroidForegroundService();
#endif
    }

#if ANDROID
    private void StartAndroidForegroundService()
    {
        try
        {
            var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(Platforms.Android.LocationForegroundService));
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                Android.App.Application.Context.StartForegroundService(intent);
            }
            else
            {
                Android.App.Application.Context.StartService(intent);
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke($"Error iniciando servicio: {ex.Message}");
        }
    }

    private void StopAndroidForegroundService()
    {
        try
        {
            var intent = new Android.Content.Intent(Android.App.Application.Context, typeof(Platforms.Android.LocationForegroundService));
            Android.App.Application.Context.StopService(intent);
        }
        catch { }
    }
#endif

    private async Task TrackingLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && _isTracking)
        {
            try
            {
                var settings = _storage.GetSettings();
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location != null)
                {
                    var record = new LocationRecord
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Timestamp = DateTime.Now,
                        Accuracy = location.Accuracy ?? 0
                    };

                    CurrentLocation = record;
                    _storage.SaveLocation(record);
                    OnLocationUpdated?.Invoke(record);

                    // Enviar al backend
                    await SendToBackendAsync(record);
                }

                await Task.Delay(settings.UpdateIntervalSeconds * 1000, token);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                OnError?.Invoke($"Error GPS: {ex.Message}");
                await Task.Delay(5000, token);
            }
        }
    }

    private async Task SendToBackendAsync(LocationRecord record)
    {
        try
        {
            var settings = _storage.GetSettings();

            // Obtener o crear device_id
            var deviceId = await SecureStorage.GetAsync("device_id");
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                await SecureStorage.SetAsync("device_id", deviceId);
            }

            var baseUrl = settings.BackendUrl.TrimEnd('/');

            // Usar InvariantCulture para asegurar formato correcto de decimales (punto, no coma)
            var lat = record.Latitude.ToString(CultureInfo.InvariantCulture);
            var lon = record.Longitude.ToString(CultureInfo.InvariantCulture);
            var acc = record.Accuracy.ToString(CultureInfo.InvariantCulture);
            var timestamp = record.Timestamp.ToString("o"); // ISO 8601

            var url = $"{baseUrl}/api/location?device_id={deviceId}&lat={lat}&lon={lon}&accuracy={acc}&timestamp={Uri.EscapeDataString(timestamp)}";

            System.Diagnostics.Debug.WriteLine($"[GPS] Enviando a: {url}");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                _lastBackendError = null;
                System.Diagnostics.Debug.WriteLine($"[GPS] Enviado OK: {response.StatusCode}");
            }
            else
            {
                _lastBackendError = $"HTTP {(int)response.StatusCode}";
                System.Diagnostics.Debug.WriteLine($"[GPS] Error: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            _lastBackendError = $"Red: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[GPS] HttpError: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            _lastBackendError = "Timeout";
            System.Diagnostics.Debug.WriteLine("[GPS] Timeout");
        }
        catch (Exception ex)
        {
            _lastBackendError = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[GPS] Error: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
