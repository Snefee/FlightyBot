# FlightyBot
A Discord bot written entirely in C# that allows the user to check information about current and upcoming flights based on the flight number. 
<br>
<br>
Technologies used:
- C# - Programming Language
- .NET 8 - Framework
- DsharpPlus - Library for Discord communication
- AeroDataBox API - Source for flight information
- ADSBdb API - Source for data and pictures of aircrafts
- Mapbox API - Allows to dynamically generate static maps with flight path
- Newtonsoft.Json - JSON parsing library

## Currently implemented features:
- /track command - Allows the user to check information like current status, departure time and airport, arrival airport with scheduled and estimated time and current location with a dynamically generated map
- ✈️Plane Info button [attached to the response to **/track** command] - User can check more details about a specific aircraft like it's manufacturer, exact type, registered owner and a photo of the exact aircraft searched


##
Example response after using the **/track** command:
<br>
<br>
<img width="517" height="595" alt="image" src="https://github.com/user-attachments/assets/03839afe-b54b-4ec4-b51a-db8215728fac" />

<br>
Example response after pressing the "✈️Plane Info" button:
<br>
<br>
<img width="520" height="506" alt="image" src="https://github.com/user-attachments/assets/10592e7b-c896-4865-b1ac-1b36f381cc1e" />

## Planned features
- Seat map of a tracked plane
- Track with notifications about arrival / departure
- Calculate how much percent of a trip is done [with progress bar]


##
_FlightyBot is not associated in any way with the Flighty™ app_
