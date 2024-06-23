# Route Optimizer
## How to run the demo:
- Input apikey in route-optimizer-frontend/.env (instead of "x")
- Input apikey in route-optimizer-backend/appsettings.json (instead of "x")
- Run route-optimizer-backend with "dotnet run" f.example
- Run route-optimizer-frontend with "npm start"
- Input a few addresses - pressing enter to add the address is possible
- Its both possible to clear all addresses (with button) or individual addresses (with x on the addresses)
- Clearing addresses with the button will also reset the map view.
- Press the optimize-button to send the list to the backend, which will return routedata that is then displayed
- Click on the markers to see marker info, click on the routes to get route info.

## Info:
-  Right now the line is shown as bird-view instead of following the roads, but the calculated durations and distances are ofcourse based on the roads.

## Additional work that could be done:
- Make it look like normal google maps by expanding backend to send encrypted polylines that can be decrypted and displayed in steps on the front-end map.
- Display errors in frontend in a user-friendly manner - for example when an address is invalid. (Right now errors are displayed in the console)
 
## Technologies that were used to create this project:
- Github and VSCode git extension
- C# .net core
- React+Typescript
- Figma(For mockup UI, class diagrams and notes)
- Google API/Cloud
  - Routes API
  - Geocoding API
  - Maps JavaScript API
- Postman
- VScode with the extensions:
  - .NET Install Tool
  - C#
  - C# Dev Kit
  - Git Graph
  - Intellicode for C# Dev Kit
  - Todo Tree

## Issues and considerations along the way:
  - **Address validation**
    - I attempted to build some nice address validation using the address validation API that google has, but sadly the validation is not very accurate. I could give it an address that did NOT exist and it would still approximate to an address that sounded like it. I was not able to get a strict address validation which is what i was looking after.
  - **Geocoding**
    - Geocoding and uncoding addresses do not necessarily give the same address
  - **Optimisation of routes**
    - Instead of building and using the nearest neighbour algorithm to optimize the addresses, i decided that the google routes optimization was sufficient for this project. Its better performant given that i dont need to make a bunch of getroute calls to get all the possible combinations of edges in a graph of addresses. For the usecase of last-mile delivery, it makes sense to have a distribution center where you deliver from - so i dont think its a farfetched assumption.
    - Google Routes API does not allow full optimization of a list of addresses. It requires an origin and destination. For this challenge, i chose to have the first address in the list be the distribution center (both origin and destination) such that the rest of the addresses could be optimized with the Routes API endpoint
  - **Unit testing**
    - An attempt was made to have unit tests, but it was descoped as i couldnt figure out how to get VScode to run my xunit tests.
  - **Exception handling**
    - It was considered to add an exceptionhandling middleware - but was descoped
  - **Logging**
    - It was considered adding logging, but was descoped for simple console.logs - given that its a demo challenge.
