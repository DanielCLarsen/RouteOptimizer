public class RouteData
{
    public int Distance_meters { get; set; }
    public int Duration_seconds{ get; set; }
    public Location Origin{ get; set; }
    public Location Destination{ get; set; }
}

public class Location{
    public string Address { get; set;}
    public GeoLocation Geolocation { get; set; }
}

public class GeoLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
