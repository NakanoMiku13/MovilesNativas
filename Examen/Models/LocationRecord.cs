namespace Examen.Models;

public class LocationRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public double Accuracy { get; set; }
}
