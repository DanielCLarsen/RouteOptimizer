import React, { useState, useEffect } from 'react';
import { GoogleMap, InfoWindow, LoadScript, Marker, Polyline } from '@react-google-maps/api';
import { RouteData } from '../types';

const containerStyle = {
  width: '100%',
  height: '400px'
};

// Copenhagen in center.
const defaultCenter = {
  lat: 55.68,
  lng: 12.55
};

interface MapProps {
  routeData: RouteData[];
}

const Map = ({ routeData }: MapProps) => {
  const [activeMarker, setActiveMarker] = useState<null | number>(null);
  const [activeRoute, setActiveRoute] = useState<null | number>(null);
  const [center, setCenter] = useState(defaultCenter);

  useEffect(() => {
    if (routeData.length > 0) {
      setCenter({ lat: routeData[0].origin.geolocation.latitude, lng: routeData[0].origin.geolocation.longitude });
    }
  }, [routeData]);

  const markers = routeData.flatMap((route, index) => [
    {
      position: { lat: route.origin.geolocation.latitude, lng: route.origin.geolocation.longitude },
      label: `${route.origin.address.substring(0, 2)}${index + 1}`,
      address: route.origin.address,
    },
  ]);

  const polylines = routeData.map(route => [
    { lat: route.origin.geolocation.latitude, lng: route.origin.geolocation.longitude },
    { lat: route.destination.geolocation.latitude, lng: route.destination.geolocation.longitude },
  ]);

  const calculateMidpoint = (start: { lat: number, lng: number }, end: { lat: number, lng: number }) => {
    return {
      lat: (start.lat + end.lat) / 2,
      lng: (start.lng + end.lng) / 2
    };
  };

  return (
    <LoadScript googleMapsApiKey={process.env.REACT_APP_GOOGLE_MAPS_API_KEY || ''}>

      <GoogleMap
        mapContainerStyle={containerStyle}
        center={center}
        zoom={10}
      >
        {markers.map((marker, index) => (
          <Marker
            key={index}
            position={marker.position}
            label={marker.label}
            onClick={() => setActiveMarker(index)}
          >
            {activeMarker === index && (
              <InfoWindow position={marker.position} onCloseClick={() => setActiveMarker(null)}>
                <div className="info-window">
                  <p><strong>Address:</strong> {marker.address}</p>
                </div>
              </InfoWindow>
            )}
          </Marker>
        ))}
        {polylines.map((path, index) => {
          const midpoint = calculateMidpoint(path[0], path[1]);
          return (
            <React.Fragment key={index}>
              <Polyline
                path={path}
                options={{ strokeColor: '#FF0000' }}
                onClick={() => setActiveRoute(index)}
              />
              {activeRoute === index && (
                <InfoWindow
                  position={midpoint}
                  onCloseClick={() => setActiveRoute(null)}
                >
                  <div className="info-window">
                    <p><strong>Origin:</strong> {routeData[index].origin.address}</p>
                    <p><strong>Destination:</strong> {routeData[index].destination.address}</p>
                    <p><strong>Distance:</strong> {routeData[index].distance_meters} meters</p>
                    <p><strong>Duration:</strong> {Math.floor(routeData[index].duration_seconds / 60)} minutes {routeData[index].duration_seconds % 60} seconds</p>
                  </div>
                </InfoWindow>
              )}
            </React.Fragment>
          );
        })}
      </GoogleMap>
    </LoadScript>
  );
};

export default Map;
