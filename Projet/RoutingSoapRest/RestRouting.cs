using RoutingSoapRest.Proxy;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;


namespace RoutingSoapRest
{
    public class RestRouting : IRestRouting
    {

        HttpClient clientSocket = new HttpClient();
        public static FindHelper helper = new FindHelper();

        public Way[] GetDirections(string start, string end)
        {
            try
            {
                return helper.GetWays(start, end);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                return new Way[] { new Way(true, "Entrez des noms valide de ville, ou soyez plus précis") };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Way[] { new Way(true, "L'api de google à reçu trop de requêtes, veuillez patienter quelque minute avant de réessayer") };
            }
        }
    }
}