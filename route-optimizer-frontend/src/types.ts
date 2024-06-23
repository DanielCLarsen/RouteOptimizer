export interface GeoLocation {
    latitude: number;
    longitude: number;
  }
  
  export interface Location {
    address: string;
    geolocation: GeoLocation;
  } 
  
  export interface RouteData {
    distance_meters: number;
    duration_seconds: number;
    origin: Location;
    destination: Location;
  }
  
  