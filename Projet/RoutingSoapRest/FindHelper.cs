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
            return station.station;
        }

        public Way[] GetWays(string start, string end)
        {
            Geometry startLocation = GetCoordinate(start);
            Geometry endLocation = GetCoordinate(end);

            InfoStation closestStationFromStart = FindClosestStationByFeetFrom(startLocation, true);
            InfoStation closestStationFromEnd = FindClosestStationByFeetFrom(endLocation, false);
            InfoStation whereDropFirstBike = null;
            InfoStation whereGetSecondBike = null;
            ReponseDist a2b = GetDuration(startLocation, endLocation);
            double timeFeet = a2b.features[0].properties.summary.duration;
            if (closestStationFromStart.station.contractName != closestStationFromEnd.station.contractName)
            {
                whereDropFirstBike = FindClosestStationForContract(endLocation,false, closestStationFromStart.station.contractName);
                whereGetSecondBike = FindClosestStationForContract(startLocation,true, closestStationFromEnd.station.contractName);
                if (whereDropFirstBike.station.number.Equals(closestStationFromStart.station.number))
                {
                    whereDropFirstBike = null;
                }
                if (whereGetSecondBike.station.number.Equals(closestStationFromEnd.station.number))
                {
                    whereGetSecondBike = null;
                }
            }
            if(whereDropFirstBike != null && whereGetSecondBike != null)
            {
                var statOne2statTwo = GetDurationBike(closestStationFromStart.station.position, whereDropFirstBike.station.position);
                var statThree2statFour = GetDurationBike(closestStationFromEnd.station.position, whereGetSecondBike.station.position);
                var getInterCity = GetDuration(whereDropFirstBike.station.position, whereGetSecondBike.station.position);

                return new Way[] { new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromStart.reponseDist.features[0].geometry), new Way(statOne2statTwo.features[0].properties, statOne2statTwo.features[0].geometry), new Way(getInterCity.features[0].properties, getInterCity.features[0].geometry), new Way(statThree2statFour.features[0].properties, statThree2statFour.features[0].geometry), new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromEnd.reponseDist.features[0].geometry) };

            }
            else {
                ReponseDist station2station = GetDurationBike(closestStationFromStart.station.position, closestStationFromEnd.station.position);
                double totalTime = closestStationFromStart.reponseDist.features[0].properties.summary.duration + station2station.features[0].properties.summary.duration + closestStationFromEnd.reponseDist.features[0].properties.summary.duration;
                if (timeFeet < totalTime)
                {
                    return new Way[] { new Way(a2b.features[0].properties, a2b.features[0].geometry) };
                }
                return new Way[] { new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromStart.reponseDist.features[0].geometry), new Way(station2station.features[0].properties, station2station.features[0].geometry), new Way(closestStationFromStart.reponseDist.features[0].properties, closestStationFromEnd.reponseDist.features[0].geometry) };
            }
        }

        public Geometry GetCoordinate(string from)
        {
            HttpResponseMessage response = clientSocket.GetAsync("https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&text=" + from).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            Reponse resp = JsonSerializer.Deserialize<Reponse>(responseBody);
            return resp.features[0].geometry;
        }

        public ReponseDist GetDuration(Geometry start, Geometry end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (start.getLongitude() + "").Replace(',', '.') + "," + (start.getLatitude() + "").Replace(',', '.') + "&end=" + (end.getLongitude() + "").Replace(',', '.') + "," + (end.getLatitude() + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        public ReponseDist GetDuration(Position start, Position end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (start.longitude + "").Replace(',', '.') + "," + (start.latitude + "").Replace(',', '.') + "&end=" + (end.longitude + "").Replace(',', '.') + "," + (end.latitude + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        public ReponseDist GetDurationBike(Position start, Position end)
        {
            string adresse = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (start.longitude + "").Replace(',', '.') + "," + (start.latitude + "").Replace(',', '.') + "&end=" + (end.longitude + "").Replace(',', '.') + "," + (end.latitude + "").Replace(',', '.');
            HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);
            return resp;
        }

        public Station[] FindClosestStationFrom(Geometry coordonnee, bool fromTheStart, string contractName)
        {
            SortedDictionary<double, Station> dico = new SortedDictionary<double, Station>();
            Station[] stations;
            ProxyClient client = new ProxyClient();
            if (contractName == "All") { stations = allStations; }
            else {
                stations = client.GetAllStationsFromContract(contractName).stations;
            }
            foreach (Station station in stations)
            {
                GeoCoordinate me = new GeoCoordinate(coordonnee.getLatitude(), coordonnee.getLongitude());
                GeoCoordinate to = new GeoCoordinate(station.position.latitude, station.position.longitude);
                try
                {
                    if (station.contractName != "jcdecauxbike")
                    {
                        dico.Add(me.GetDistanceTo(to), station);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Station[] result = new Station[5];
            int i = 0;

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

        public InfoStation FindClosestStationByFeetFrom(Geometry coordonnee, bool fromStart)
        {
            Station[] result = FindClosestStationFrom(coordonnee, fromStart, "All");

            HttpClient clientSocket = new HttpClient();
            Station final = new Station();
            InfoStation info = new InfoStation();
            double distance = double.PositiveInfinity;

            foreach (Station station in result)
            {
                string adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (coordonnee.getLongitude() + "").Replace(',', '.') + "," + (coordonnee.getLatitude() + "").Replace(',', '.') + "&end=" + (station.position.longitude + "").Replace(',', '.') + "," + (station.position.latitude + "").Replace(',', '.');
                HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);

                if (resp.features[0].properties.summary.duration < distance)
                {
                    distance = resp.features[0].properties.summary.duration;
                    final = station;
                    info = new InfoStation(final, resp);
                }
            }
            return info;
        }

        public InfoStation FindClosestStationForContract(Geometry coordonnee,bool fromStart, string contractName)
        {
            Station[] result = FindClosestStationFrom(coordonnee, fromStart, contractName);
            HttpClient clientSocket = new HttpClient();
            Station final = new Station();
            InfoStation info = new InfoStation();
            double distance = double.PositiveInfinity;

            foreach (Station station in result)
            {
                string adresse = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (coordonnee.getLongitude() + "").Replace(',', '.') + "," + (coordonnee.getLatitude() + "").Replace(',', '.') + "&end=" + (station.position.longitude + "").Replace(',', '.') + "," + (station.position.latitude + "").Replace(',', '.');
                if (fromStart)
                {
                    adresse = "https://api.openrouteservice.org/v2/directions/foot-walking?api_key=5b3ce3597851110001cf624861912097812a436daaae0aca02220957&start=" + (coordonnee.getLongitude() + "").Replace(',', '.') + "," + (coordonnee.getLatitude() + "").Replace(',', '.') + "&end=" + (station.position.longitude + "").Replace(',', '.') + "," + (station.position.latitude + "").Replace(',', '.');
                }
                HttpResponseMessage response = clientSocket.GetAsync(adresse).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                ReponseDist resp = JsonSerializer.Deserialize<ReponseDist>(responseBody);

                if (resp.features[0].properties.summary.duration < distance)
                {
                    distance = resp.features[0].properties.summary.duration;
                    final = station;
                    info = new InfoStation(final, resp);
                }
            }
            return info;
        }

    }
}
