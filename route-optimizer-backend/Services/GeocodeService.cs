using Newtonsoft.Json;

public class GeocodeService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiKey;

    public GeocodeService(ApiSettings apiSettings)
    {
        _apiKey = apiSettings.ApiKey;
    }   

    private class GeocodeResponse
    {
        [JsonProperty("results")]
        public List<Result> Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }   

    private class Result
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }
    }

    private class Geometry
    {
        [JsonProperty("location")]
        public LocationCoordinates Location { get; set; }
    }

    private class LocationCoordinates
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    private class AddressNotFoundException : Exception
    {
        public AddressNotFoundException(string message) : base(message) { }
    }


    public async Task<GeoLocation> GetCoordinatesAsync(string address)
    {
        try
        {
            var url = new UriBuilder("https://maps.googleapis.com/maps/api/geocode/json")
            {
                Query = $"address={Uri.EscapeDataString(address)}&key={_apiKey}"
            }.ToString();

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            Validator.ValidateJsonNullOrEmpty(json);

            GeocodeResponse geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(json);

            if (geocodeResponse == null || geocodeResponse.Results.Count == 0)
            {
                throw new AddressNotFoundException($"Address not found: {address}");
            }

            var location = geocodeResponse.Results[0].Geometry.Location;
            
            return new GeoLocation
            {
                Latitude = location.Lat,
                Longitude = location.Lng,
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while getting coordinates for address: {address}", ex);
        }
    }

    public async Task<string> GetAddressAsync(global::GeoLocation geoLocation)
    {
        try
        {
            var lat = geoLocation.Latitude;
            var lng = geoLocation.Longitude;
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}&key={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            Validator.ValidateJsonNullOrEmpty(json);

            GeocodeResponse geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(json);

            if (geocodeResponse == null || geocodeResponse.Results.Count == 0) 
            {
                throw new AddressNotFoundException($"Address not found");
            }            

            return geocodeResponse.Results[0].FormattedAddress;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while getting address for coordinates: {geoLocation.Latitude}, {geoLocation.Longitude}", ex);
        }
    }    
}