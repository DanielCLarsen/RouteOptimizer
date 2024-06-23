public class RouteOptimizer
{
    private GeocodeService _geocodeService;
    private GoogleMapsService _googleMapsService;

    public RouteOptimizer(GeocodeService geocodeService, GoogleMapsService googleMapsService)
    {
        _geocodeService = geocodeService;
        _googleMapsService = googleMapsService;        
    }

    public async Task<List<GeoLocation>> GetGeolocationsForAddresses(List<string> addresses)
    {
        var tasks = addresses.Select(address => _geocodeService.GetCoordinatesAsync(address)).ToList();
        GeoLocation[] coordinates = await Task.WhenAll(tasks);
        return coordinates.ToList();        
    }    

    public async Task<List<RouteData>> GetRoutesForGeolocations(List<RouteData> routeDatas)
    {
        if (routeDatas.Count == 0)
        {
            throw new ArgumentException("Provided routedata-list is empty");
        }

        var tasks = new List<Task<RouteData>>();

        for (int i = 0; i < routeDatas.Count; i++)
        {
            var origin = routeDatas[i].Origin;
            var destination = routeDatas[i].Destination;
            tasks.Add(_googleMapsService.GetRouteAsync(origin, destination));
        }

        RouteData[] routeInfo = await Task.WhenAll(tasks); 
        return routeInfo.ToList();       
    }
}