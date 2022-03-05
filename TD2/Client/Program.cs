using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace Echo
{
    class EchoClient
    {
        static HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            HttpResponseMessage response =await client.GetAsync("http://localhost:8080/incr?param1=" + args[0]);
            string responseBody =await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
    }
}