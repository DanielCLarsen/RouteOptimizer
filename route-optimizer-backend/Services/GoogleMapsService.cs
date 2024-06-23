using System.Text;
using Newtonsoft.Json;

public class GoogleMapsService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiKey;

    public GoogleMapsService(ApiSettings apiSettings)
    {
        _apiKey = apiSettings.ApiKey;
    }


    private class Route
    {
        [JsonProperty("distanceMeters")]
        public int DistanceMeters { get; set; }

        [JsonProperty("duration")]
        public string Duration {get; set; }
    }

    private class DirectionsResponse
    {
        [JsonProperty("routes")]
        public List<Route> Routes { get; set; }
    }

    private class GoogleOptimizationResponse
    {
        [JsonProperty("routes")]
        public List<RoutesOpt> RoutesOpt { get; set; }
    }

    private class RoutesOpt
    {
        [JsonProperty("optimizedIntermediateWaypointIndex")]
        public List<int> OptimizedIntermediateWaypointIndex { get; set; }
    }

    private object GetRouteAsyncBody(GeoLocation origin, GeoLocation destination)
    {
        return new
        {
            origin = new
            {
                location = new
                {
                    latLng = new
                    {
                        latitude = origin.Latitude,
                        longitude = origin.Longitude,
                    }
                }
            },
            destination = new
            {
                location = new
                {
                    latLng = new
                    {
                        latitude = destination.Latitude,
                        longitude = destination.Longitude,
                    }
                }
            }
        };
    } 

    public async Task<List<int>> OptimizeRoutes(string distributionCenter, List<string> waypoints)
    {
        try
        {
            var url = new UriBuilder("https://routes.googleapis.com/directions/v2:computeRoutes")
            {
                Query = $"key={_apiKey}"
            }.ToString();            

            var requestBody = new
            {
                origin = new { address = distributionCenter},
                destination = new { address = distributionCenter },
                intermediates = waypoints.Select(address => new { address }).ToArray(),
                travelMode = "DRIVE",
                optimizeWaypointOrder = true
            };

            var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", "routes.optimizedIntermediateWaypointIndex");

            var response = await _httpClient.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Validator.ValidateJsonNullOrEmpty(json);

            GoogleOptimizationResponse routeOptimizationResponse = JsonConvert.DeserializeObject<GoogleOptimizationResponse>(json);

            if (routeOptimizationResponse?.RoutesOpt == null || 
                routeOptimizationResponse.RoutesOpt.Count == 0 || 
                !routeOptimizationResponse.RoutesOpt[0].OptimizedIntermediateWaypointIndex.Any()) 
            {
                throw new InvalidOperationException("No optimized routes were found in response");
            }

            return routeOptimizationResponse.RoutesOpt[0].OptimizedIntermediateWaypointIndex; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while optimizing routes: {ex.Message}");
            throw;
        }    
    }
   
    
    public async Task<RouteData> GetRouteAsync(Location origin, Location destination)
    {
        var apiUrl = new UriBuilder("https://routes.googleapis.com/directions/v2:computeRoutes")
        {
            Query = $"key={_apiKey}"
        }.ToString();


        var requestBody = GetRouteAsyncBody(origin.Geolocation, destination.Geolocation);
        string jsonContent = JsonConvert.SerializeObject(requestBody);
        HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        try
        {   
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", "routes.distanceMeters,routes.duration");

            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);      
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Validator.ValidateJsonNullOrEmpty(json);

            var directionsResponse = JsonConvert.DeserializeObject<DirectionsResponse>(json);
            if (directionsResponse == null || directionsResponse.Routes == null || directionsResponse.Routes.Count == 0){
                throw new InvalidOperationException("No routes found in the response");
            }

            var route = directionsResponse.Routes[0];

            return new RouteData{
                Distance_meters = route.DistanceMeters,
                Duration_seconds = int.Parse(route.Duration.TrimEnd('s')),
                Origin = origin,
                Destination = destination,                            
            };             
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the route information: {ex.Message}");
            throw;
        }
        
    }
}