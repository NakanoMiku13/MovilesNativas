namespace Examen.Models;

public class AppSettings
{
    public int UpdateIntervalSeconds { get; set; } = 10;
    public bool ShowNotification { get; set; } = true;
    public string Theme { get; set; } = "Guinda"; // Guinda o Azul
    public string BackendUrl { get; set; } = "http://10.0.2.2:5000"; // Default para emulador Android
}
