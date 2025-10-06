namespace Practica3.Repositories;
public class Location
{
    public static async Task<Models.Location> GetLocationAsync()
    {
        try
        {
            Models.Location locationData = new();
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var temp = await Geolocation.Default.GetLocationAsync(request);
            if (temp != null)
            {
                locationData.Latitude = temp.Latitude;
                locationData.Longitude = temp.Longitude;
                locationData.Altitude = temp.Altitude ?? 0.0;
                locationData.Accuracy = temp.Accuracy ?? 0.0;
                return locationData;
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting location: {ex.Message}");
        }
        return new();
    }

}