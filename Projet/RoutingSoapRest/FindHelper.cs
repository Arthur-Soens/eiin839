using RoutingSoapRest.Proxy;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RoutingSoapRest
{
    public class FindHelper
    {
        HttpClient clientSocket = new HttpClient();
        Station[] allStations = null;
        string api_key = "5b3ce3597851110001cf624861912097812a436daaae0aca02220957";
        public FindHelper()
        {
            //Here we retrieve all stations at the start of the server and we store them
            ProxyClient client = new ProxyClient();
            allStations = client.GetAllStations().stations;
            client.Close();
        }

        //Get the closest station from start
        public Station GetStation(string start)
        {
            var coord = GetCoordinate(start);
            var station = FindClosestStationByFeetFrom(coord, true);
            return station.station;
        }

        //Get all direction with all alternative station
        public Way[] GetWays(string start, string end)
        {
            Console.WriteLine("Getting coordinates");
            Geometry startLocation = GetCoordinate(start);
            Geometry endLocation = GetCoordinate(end);

            InfoStation closestStationFromStart = FindClosestStationByFeetFrom(startLocation, true);
            InfoStation closestStationFromEnd = FindClosestStationByFeetFrom(endLocation, false);
            InfoStation whereDropFirstBike = null;
            InfoStation whereGetSecondBike = null;
            ReponseDist a2b = GetDuration(startLocation, endLocation);
            double timeFeet = a2b.features[0].properties.summary.duration;

            //If both station are not in the same contract (the same city), we must find 2 more station, one for drop the bike and another one to get another.
            if (closestStationFromStart.station.contractName != closestStationFromEnd.station.contractName)
            {
                whereDropFirstBike = FindClosestStationForContract(endLocation,false, closestStationFromStart.station.contractName);
                whereGetSecondBike = FindClosestStationForContract(startLocation,true, closestStationFromEnd.station.contractName);
                //we don't want to get bike from a station if it's the same where we drop bike
                if (whereDropFirstBike.station.number.Equals(closestStationFromStart.station.number))
                {
                    whereDropFirstBike = null;
                }
                if (whereGetSecondBike.station.number.Equals(closestStationFromEnd.station.number))
                {
                    whereGetSecondBike = null;
                }
            }
            //If we have a road between two city, we force the usage of the bike
            if(whereDropFirstBike != null && whereGetSecondBike != null)
            {
                //if it's intercities, we choose to force the usage of the bikes because it's always better to go the the end without searching bike and the application no longer have sense in this case. (We want to use the bike as much as possible)
                var statOne2statTwo = GetDurationBike(closestStationFromStart.station.position, whereDropFirstBike.station.position);
                var statThree2statFour = GetDurationBike(closestStationFromEnd.station.position, whereGetSecondBike.station.position);
                var getInterCity = GetDuration(whereDropFirstBike.station.position, whereGetSecondBike.station.position);

                return new Way[] { new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromStart.reponseDist.features[0].geometry), new Way(statOne2statTwo.features[0].properties, statOne2statTwo.features[0].geometry), new Way(getInterCity.features[0].properties, getInterCity.features[0].geometry), new Way(statThree2statFour.features[0].properties, statThree2statFour.features[0].geometry), new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromEnd.reponseDist.features[0].geometry) };

            }
            else if(whereDropFirstBike == null && whereGetSecondBike != null)
            {
                var depart2stationthree = GetDuration(startLocation, whereGetSecondBike.station.position);
                var statThree2statFour = GetDurationBike(closestStationFromEnd.station.position, whereGetSecondBike.station.position);
                return new Way[] { new Way(depart2stationthree.features[0].properties, depart2stationthree.features[0].geometry), new Way(statThree2statFour.features[0].properties, statThree2statFour.features[0].geometry), new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromEnd.reponseDist.features[0].geometry) };
            }
            else if (whereGetSecondBike == null && whereDropFirstBike != null)
            {
                var statOne2statTwo = GetDurationBike(closestStationFromStart.station.position, whereDropFirstBike.station.position);
                var statTwo2end = GetDuration(whereDropFirstBike.station.position, endLocation);
                return new Way[] { new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromStart.reponseDist.features[0].geometry), new Way(statOne2statTwo.features[0].properties, statOne2statTwo.features[0].geometry),new Way(statTwo2end.features[0].properties, statTwo2end.features[0].geometry) };
            }
            else {
                Console.WriteLine("Same city");
                //If we are in the same city, we choose the shortest path to go to the end point, so, If using feet is shorter, we don't use bike
                ReponseDist station2station = GetDurationBike(closestStationFromStart.station.position, closestStationFromEnd.station.position);
                double totalTime = closestStationFromStart.reponseDist.features[0].properties.summary.duration + station2station.features[0].properties.summary.duration + closestStationFromEnd.reponseDist.features[0].properties.summary.duration;
                if (timeFeet < totalTime)
                {
                    Console.WriteLine("Quicker to go by feet");
                    return new Way[] { new Way(a2b.features[0].properties, a2b.features[0].geometry) };
                }
                return new Way[] { new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromStart.reponseDist.features[0].geometry), new Way(station2station.features[0].properties, station2station.features[0].geometry), new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromEnd.reponseDist.features[0].geometry) };
            }
        }

        //Just get all coordinate
        public Geometry GetCoordinate(string from)
        {
            HttpResponseMessage response = clientSocket.GetAsync("https://api.openrouteservice.org/geocode/search?api_key="+api_key+"&text=" + from).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            Reponse resp = JsonSerializer.Deserialize<Reponse>(responseBody);
            return resp.features[0].geometry;
        }

        //The next five methods are there to get the duration between two coordinates
        public ReponseDist GetDuration(Geometry start, Geometry end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key="+ api_key+"&start=" + (start.getLongitude() + "").Replace(',', '.') + "," + (start.getLatitude() + "").Replace(',', '.') + "&end=" + (end.getLongitude() + "").Replace(',', '.') + "," + (end.getLatitude() + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        public ReponseDist GetDuration(Position start, Geometry end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=" + api_key + "&start=" + (start.longitude + "").Replace(',', '.') + "," + (start.latitude + "").Replace(',', '.') + "&end=" + (end.getLongitude() + "").Replace(',', '.') + "," + (end.getLatitude() + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        public ReponseDist GetDuration(Position start, Position end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=" + api_key + "&start=" + (start.longitude + "").Replace(',', '.') + "," + (start.latitude + "").Replace(',', '.') + "&end=" + (end.longitude + "").Replace(',', '.') + "," + (end.latitude + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        public ReponseDist GetDuration(Geometry start, Position end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=" + api_key + "&start=" + (start.getLongitude() + "").Replace(',', '.') + "," + (start.getLatitude() + "").Replace(',', '.') + "&end=" + (end.longitude + "").Replace(',', '.') + "," + (end.latitude + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        //Here we want tho get the duration but with a bike
        public ReponseDist GetDurationBike(Position start, Position end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=" + api_key + "&start=" + (start.longitude + "").Replace(',', '.') + "," + (start.latitude + "").Replace(',', '.') + "&end=" + (end.longitude + "").Replace(',', '.') + "," + (end.latitude + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        //Here we get the 5 closest station using the coordinates
        public Station[] FindClosestStationFrom(Geometry coordonnee, bool fromTheStart, string contractName)
        {
            //We use a dictionnary to have all station sorted by their distance
            SortedDictionary<double, Station> dico = new SortedDictionary<double, Station>();
            Station[] stations;
            ProxyClient client = new ProxyClient();
            //We choose if we want the 5 closest between all station or just stations for one contract
            if (contractName == "All") { stations = allStations; }
            else {
                stations = client.GetAllStationsFromContract(contractName).stations;
            }
            //here we calculate the distance between station and we put the station in the dictionnary
            foreach (Station station in stations)
            {
                GeoCoordinate me = new GeoCoordinate(coordonnee.getLatitude(), coordonnee.getLongitude());
                GeoCoordinate to = new GeoCoordinate(station.position.latitude, station.position.longitude);
                try
                {
                    //We don't use the contract jcdecauxbike because it's does some weird stuff
                    if (station.contractName != "jcdecauxbike")
                    {
                        dico.Add(me.GetDistanceTo(to), station);
                    }
                }
                catch (Exception ex)
                {
                    //If we have the same distance between 2 stations we just keep the first station. The probability for that to append is low
                    Console.WriteLine(ex.Message);
                }
            }
            Station[] result = new Station[3];
            int i = 0;
            //Here we get the 5 first station from the dictionnary
            foreach (Station s in dico.Values)
            {
                if (i == 3)
                {
                    break;
                }
                var key = s.number + "_" + s.contractName;
                Console.WriteLine("Request to proxy !");
                var test = client.GetStations(key).stations[0];
                //We keep only station with bikes or a place to store a bike
                if ((fromTheStart && test.totalStands.availabilities.bikes > 0) || (!fromTheStart && ((test.totalStands.capacity - test.totalStands.availabilities.bikes) > 0)))
                {
                    result[i] = s;
                    i++;
                }
            }
            client.Close();
            return result;
        }

        //Find closest station from coordonnee. The closest station is between all station disponnible in all contract
        public InfoStation FindClosestStationByFeetFrom(Geometry coordonnee, bool fromStart)
        {
            Console.WriteLine("Find closest station by feet");
            Station[] result = FindClosestStationFrom(coordonnee, fromStart, "All");

            HttpClient clientSocket = new HttpClient();
            Station final = new Station();
            InfoStation info = new InfoStation();
            double distance = double.PositiveInfinity;

            //Here we get the station where it's quicker to go by feet
            foreach (Station station in result)
            {
                string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=" + api_key + "&start=" + (coordonnee.getLongitude() + "").Replace(',', '.') + "," + (coordonnee.getLatitude() + "").Replace(',', '.') + "&end=" + (station.position.longitude + "").Replace(',', '.') + "," + (station.position.latitude + "").Replace(',', '.');
                HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
                Console.WriteLine("Actual station processed : " + station.number + " Temps :" + resp.features[0].properties.summary.duration);
                if (resp.features[0].properties.summary.duration < distance)
                {
                    distance = resp.features[0].properties.summary.duration;
                    final = station;
                    info = new InfoStation(final, resp);
                }
            }
            Console.WriteLine("Closest station : " + info.station.number + " Temps :" + info.reponseDist.features[0].properties.summary.duration);
            return info;
        }

        //Find closest station from coordonnee. The closest station is between all station disponible in contractName
        public InfoStation FindClosestStationForContract(Geometry coordonnee,bool fromStart, string contractName)
        {
            Console.WriteLine("Find closest station for the contract : " + contractName);
            Station[] result = FindClosestStationFrom(coordonnee, fromStart, contractName);

            HttpClient clientSocket = new HttpClient();
            Station final = new Station();
            InfoStation info = new InfoStation();
            double distance = double.PositiveInfinity;

            //Here we get the station where it's quicker to go by feet or in bike depending on fromStart
            foreach (Station station in result)
            {
                string adresse = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=" + api_key + "&start=" + (coordonnee.getLongitude() + "").Replace(',', '.') + "," + (coordonnee.getLatitude() + "").Replace(',', '.') + "&end=" + (station.position.longitude + "").Replace(',', '.') + "," + (station.position.latitude + "").Replace(',', '.');
                if (fromStart)
                {
                    adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=" + api_key + "&start=" + (coordonnee.getLongitude() + "").Replace(',', '.') + "," + (coordonnee.getLatitude() + "").Replace(',', '.') + "&end=" + (station.position.longitude + "").Replace(',', '.') + "," + (station.position.latitude + "").Replace(',', '.');
                }
                HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
                Console.WriteLine("Actual station processed : " + station.number + " Temps :" + resp.features[0].properties.summary.duration);
                if (resp.features[0].properties.summary.duration < distance)
                {
                    distance = resp.features[0].properties.summary.duration;
                    final = station;
                    info = new InfoStation(final, resp);
                }
            }
            Console.WriteLine("Closest station : " + info.station.number + " Temps :" + info.reponseDist.features[0].properties.summary.duration);
            return info;
        }

    }
}
