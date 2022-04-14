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
            return helper.GetWays(start, end);
        }

        
    }
}