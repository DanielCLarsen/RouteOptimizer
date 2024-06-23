class RouteOptimizerHelper
{
    public static List<string> SortAddressesWithIndexes(string distributionCenter, List<int> optimizedRouteIndexes, List<string> addresses)
    {
        List<string> sortedAddress = new List<string>(){distributionCenter};
        foreach (var routeIndex in optimizedRouteIndexes){
            sortedAddress.Add(addresses[routeIndex]);
        }
        sortedAddress.Add(distributionCenter);
        
        return sortedAddress;
    }

    public static List<RouteData> ConvertToRouteData(List<string> request, List<GeoLocation> geoLocations)
    {
        List<RouteData> routeDataList = new List<RouteData>();

        if (request == null || request.Count == 0 || geoLocations == null || geoLocations.Count == 0){
            return routeDataList;
        }
        
        for (int i = 0; i < request.Count-1; i++)
        {
            routeDataList.Add(new RouteData{
                Origin = new Location{
                    Geolocation = geoLocations[i],
                    Address = request[i],
                },
                Destination = new Location{
                    Geolocation = geoLocations[i+1],
                    Address = request[i+1],

                }
            });
        }
        return routeDataList;
    }

    public static void HandleNegativeValueResponse(ref List<int> optimizedRouteIndexes)
    {
        //If there arent at least 2 waypoints, the google API returns -1 which messes with later logic, so we set the only waypoint to have the 0 index.
        if(optimizedRouteIndexes[0] == -1) {
            optimizedRouteIndexes[0] = 0;
        } 
    }
}