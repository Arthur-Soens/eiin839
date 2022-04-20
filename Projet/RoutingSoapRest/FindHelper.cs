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
        public FindHelper()
        {
            ProxyClient client = new ProxyClient();
            allStations = client.GetAllStations().stations;
            client.Close();
        }

        public Station GetWay(string start)
        {
            var coord = GetCoordinate(start);
            var station = FindClosestStationByFeetFrom(coord, true);
            return station;
        }

        public Way[] GetWays(string start, string end)
        {
            var coordinates = GetCoordinates(start, end);
            Way[] directions;
            if (coordinates.Length == 2)
            {
                directions = new Way[1];
                directions[0] = GetRoute(coordinates[0], coordinates[1], false);

                return directions;
            }

            directions = new Way[3];

            directions[0] = GetRoute(coordinates[0], coordinates[1], false);

            directions[1] = GetRoute(coordinates[1], coordinates[2], true);

            directions[2] = GetRoute(coordinates[2], coordinates[3], false);


            return directions;
        }

        public Way GetRoute(GeoCoordinate start, GeoCoordinate end, bool isOnBike)
        {
            string adresse =
                "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                (start.Longitude + "").Replace(',', '.') + "," + (start.Latitude + "").Replace(',', '.') + "&end=" +
                (end.Longitude + "").Replace(',', '.') + "," + (end.Latitude + "").Replace(',', '.');
            if (isOnBike)
            {
                adresse =
                    "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" +
                    (start.Longitude + "").Replace(',', '.') + "," + (start.Latitude + "").Replace(',', '.') + "&end=" +
                    (end.Longitude + "").Replace(',', '.') + "," + (end.Latitude + "").Replace(',', '.');
            }
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return new Way(resp.features[0].properties, resp.features[0].geometry);
        }
        public GeoCoordinate[] GetCoordinates(string start, string end)
        {
            Geometry startLocation = GetCoordinate(start);
            Geometry endLocation = GetCoordinate(end);

            var closestStationFromStart = FindClosestStationByFeetFrom(startLocation, true);
            var closestStationFromEnd = FindClosestStationByFeetFrom(endLocation, false);

            double timeFeet = GetDuration(startLocation, endLocation);
            double totalTime = GetDuration(startLocation, closestStationFromStart.position) + GetDurationBike(closestStationFromStart.position, closestStationFromEnd.position) + GetDuration(closestStationFromEnd.position, endLocation);

            GeoCoordinate startCoordinate = new GeoCoordinate(startLocation.getLatitude(), startLocation.getLongitude());
            GeoCoordinate endCoordinate = new GeoCoordinate(endLocation.getLatitude(), endLocation.getLongitude());
            if (timeFeet < totalTime)
            {
                return new GeoCoordinate[] { startCoordinate, endCoordinate };
            }
            GeoCoordinate firststopCoordinate = new GeoCoordinate(closestStationFromStart.position.latitude, closestStationFromStart.position.longitude);
            GeoCoordinate secondstopCoordinate = new GeoCoordinate(closestStationFromEnd.position.latitude, closestStationFromEnd.position.longitude);
            return new GeoCoordinate[] { startCoordinate, firststopCoordinate, secondstopCoordinate, endCoordinate };
        }



        public Geometry GetCoordinate(string from)
        {
            HttpResponseMessage response = clientSocket.GetAsync("https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&text=" + from).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            Reponse resp = JsonSerializer.Deserialize<Reponse>(responseBody);
            return resp.features[0].geometry;
        }

        public double GetDuration(Geometry start, Geometry end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (start.getLongitude() + "").Replace(',', '.') + "," + (start.getLatitude() + "").Replace(',', '.') + "&end=" + (end.getLongitude() + "").Replace(',', '.') + "," + (end.getLatitude() + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp.features[0].properties.summary.duration;
        }

        public double GetDuration(Geometry start, Position end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (start.getLongitude() + "").Replace(',', '.') + "," + (start.getLatitude() + "").Replace(',', '.') + "&end=" + (end.longitude + "").Replace(',', '.') + "," + (end.latitude + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp.features[0].properties.summary.duration;
        }

        public double GetDuration(Position start, Geometry end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (start.longitude + "").Replace(',', '.') + "," + (start.latitude + "").Replace(',', '.') + "&end=" + (end.getLongitude() + "").Replace(',', '.') + "," + (end.getLatitude() + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp.features[0].properties.summary.duration;
        }

        public double GetDurationBike(Position start, Position end)
        {
            if (start.latitude == end.latitude && start.longitude == end.longitude)
            {
                return 0;
            }
            string adresse = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (start.longitude + "").Replace(',', '.') + "," + (start.latitude + "").Replace(',', '.') + "&end=" + (end.longitude + "").Replace(',', '.') + "," + (end.latitude + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp.features[0].properties.summary.duration;
        }

        public Station[] FindClosestStationFrom(Geometry coordonnee, bool fromTheStart)
        {
            SortedDictionary<double, Station> dico = new SortedDictionary<double, Station>();
            foreach (Station station in allStations)
            {
                GeoCoordinate me = new GeoCoordinate(coordonnee.getLatitude(), coordonnee.getLongitude());
                GeoCoordinate to = new GeoCoordinate(station.position.latitude, station.position.longitude);
                try
                {
                    dico.Add(me.GetDistanceTo(to), station);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Station[] result = new Station[5];
            int i = 0;
            ProxyClient client = new ProxyClient();

            foreach (Station s in dico.Values)
            {
                if (i == 5)
                {
                    break;
                }
                var key = s.number + "_" + s.contractName;
                var test = client.GetStations(key).stations[0];
                if ((fromTheStart && test.totalStands.availabilities.bikes > 0) || (!fromTheStart && ((test.totalStands.capacity - test.totalStands.availabilities.bikes) > 0)))
                {
                    result[i] = s;
                    i++;
                }
            }
            client.Close();
            return result;
        }

        public Station FindClosestStationByFeetFrom(Geometry coordonnee, bool fromStart)
        {
            Station[] result = FindClosestStationFrom(coordonnee, fromStart);

            HttpClient clientSocket = new HttpClient();
            Station final = new Station();
            double distance = double.PositiveInfinity;

            foreach (Station station in result)
            {
                string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (coordonnee.getLongitude() + "").Replace(',', '.') + "," + (coordonnee.getLatitude() + "").Replace(',', '.') + "&end=" + (station.position.longitude + "").Replace(',', '.') + "," + (station.position.latitude + "").Replace(',', '.');
                HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
                string responseBody = response.Content.ReadAsStringAsync().Result;
                ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);

                if (resp.features[0].properties.summary.distance < distance)
                {
                    distance = resp.features[0].properties.summary.distance;
                    final = station;
                }

            }
            return final;
        }

    }
}
