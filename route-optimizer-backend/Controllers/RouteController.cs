using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly GoogleMapsService _googleMapsService;
    private readonly RouteOptimizer _routeOptimizer;   

     public RoutesController(GoogleMapsService googleMapsService, RouteOptimizer routeOptimizer)
    {
        _googleMapsService = googleMapsService;
        _routeOptimizer = routeOptimizer;
    }

    [HttpPost("optimizeRoute")]
    public async Task<IActionResult> OptimizeRoute([FromBody] AddressRequestFormat addressRequest)
    {
        try
        {
            Validator.ValidateRequest(addressRequest);
            Validator.ValidateAddresses(addressRequest.Addresses);        

            var distributionCenter = addressRequest.Addresses[0];
            var waypoints = addressRequest.Addresses.Skip(1).ToList();

            List<int> optimizedRouteIndexes = await _googleMapsService.OptimizeRoutes(distributionCenter, waypoints);
            
            RouteOptimizerHelper.HandleNegativeValueResponse(ref optimizedRouteIndexes);       

            List<string> sortedOptimizedAddresses = RouteOptimizerHelper.SortAddressesWithIndexes(distributionCenter, optimizedRouteIndexes, waypoints);

            List<GeoLocation> geoLocations = await _routeOptimizer.GetGeolocationsForAddresses(sortedOptimizedAddresses);

            List<RouteData> routeDatas = RouteOptimizerHelper.ConvertToRouteData(sortedOptimizedAddresses, geoLocations);

            List<RouteData> routes = await _routeOptimizer.GetRoutesForGeolocations(routeDatas);

            return Ok(routes);            
        }
        catch (JsonException ex) when (ex is JsonReaderException || ex is JsonSerializationException)
        {
            return BadRequest(new { message = "Invalid JSON format.", details = ex.Message });
        }
        catch (ValidationException ex)
        {        
            return BadRequest(new { message = "Validation error.", details = ex.Message });
        }
        catch (Exception ex)
        {        
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }    
}

public class AddressRequestFormat
{
    public List<string> Addresses { get; set; }
}
