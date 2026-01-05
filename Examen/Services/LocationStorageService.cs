using System.Text.Json;
using Examen.Models;

namespace Examen.Services;

public class LocationStorageService
{
    private readonly string _filePath;
    private readonly string _settingsPath;
    private List<LocationRecord> _locations = new();
    private AppSettings _settings = new();

    public LocationStorageService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "location_history.json");
        _settingsPath = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
        LoadData();
        LoadSettings();
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _locations = JsonSerializer.Deserialize<List<LocationRecord>>(json) ?? new();
            }
        }
        catch { _locations = new(); }
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new();
            }
        }
        catch { _settings = new(); }
    }

    public void SaveLocation(LocationRecord record)
    {
        _locations.Add(record);
        SaveData();
    }

    private void SaveData()
    {
        try
        {
            var json = JsonSerializer.Serialize(_locations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch { }
    }

    public void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }
        catch { }
    }

    public List<LocationRecord> GetAllLocations() => _locations.OrderByDescending(l => l.Timestamp).ToList();

    public LocationRecord? GetLastLocation() => _locations.OrderByDescending(l => l.Timestamp).FirstOrDefault();

    public void ClearHistory()
    {
        _locations.Clear();
        SaveData();
    }

    public AppSettings GetSettings() => _settings;

    public void UpdateSettings(AppSettings settings)
    {
        _settings = settings;
        SaveSettings();
    }
}
