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
            Console.WriteLine("Rest Request received");
            try
            {
                Console.WriteLine("Start research");
                var result = helper.GetWays(start, end);
                Console.WriteLine("Research finish, sending info to thin client");
                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Due to a wrong address");
                return new Way[] { new Way(true, "Entrez des noms valide de ville, ou soyez plus précis") };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Way[] { new Way(true, "L'api de google a reçu trop de requêtes, veuillez patienter quelque minutes avant de réessayer") };
            }
        }
    }
}