import React, { useState } from 'react';
import axios from 'axios';
import Map from '../GoogleMap/GoogleMap';
import './AddressList.css';
import { RouteData } from '../types';

const AddressList = () => {
  const [addresses, setAddresses] = useState<string[]>([]);
  const [inputValue, setInputValue] = useState<string>('');
  const [routeData, setRouteData] = useState<RouteData[]>([]);
  const [mapKey, setMapKey] = useState<number>(0);

  const handleAddAddress = () => {
    if (inputValue.trim() !== '') {
      setAddresses([...addresses, inputValue]);
      setInputValue('');
    }
  };

  const handleClearAddresses = () => {
    setAddresses([]);
    setRouteData([]);
    setMapKey(prevKey => prevKey + 1); // To force re-render of map
  };

  const handleRemoveAddress = (index: number) => {
    const newAddresses = addresses.filter((_, i) => i !== index);
    setAddresses(newAddresses);
  };

  const validateResponse = (response: RouteData[]): boolean => {
    for (const element of response) {
      if (
        element == null || 
        element.destination?.address == null || 
        element.destination?.geolocation.latitude == null || 
        element.destination?.geolocation.longitude == null ||
        element.origin?.address == null ||
        element.origin?.geolocation.latitude == null ||
        element.origin?.geolocation.longitude == null ||
        element.distance_meters == null ||
        element.duration_seconds == null
      ) {
        return false;
      }
    }
    return true;
  };

  const handleOptimizeRoute = async () => {
    if(addresses.length < 2){
      console.error('Please input at least 2 addresses');
      return
    }
    try {
      const response = await axios.post<RouteData[]>('http://localhost:5000/api/Routes/optimizeRoute', {
        addresses,
      });
      console.log('Response data:', response.data);
      if(validateResponse(response.data)){
        setRouteData(response.data);
      } else{
        console.error('Response contains null: ', response.data);
      }
      
    } catch (error) {
      console.error('Error fetching route data:', error);
    }
  };

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === 'Enter') {
      handleAddAddress();
    }
  };

  return (
    <div className="address-list-container">
    <Map key={mapKey} routeData={routeData}/>
      <div className="address-input-container">
        <input
          type="text"
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          placeholder="Input address here...       (Press enter to complete)"
          className="address-input"
          onKeyDown={handleKeyDown}
        />
        <button onClick={handleAddAddress} className="add-button">Add route</button>
      </div>
      <ul className="address-list">
        {addresses.map((address, index) => (
          <li key={index} className="address-item">
            <span className="address-text">{address}</span>
            <button onClick={() => handleRemoveAddress(index)} className="remove-button">x</button>
          </li>
        ))}
      </ul>
      <div className="button-container">
        <button onClick={handleOptimizeRoute} className="optimize-button">Optimize route</button>
        <button onClick={handleClearAddresses} className="clear-button">Clear routes</button>
      </div>      
    </div>
  );
};

export default AddressList;
