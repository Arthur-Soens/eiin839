using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    [DataContract]
    public class JCDecauxItem
    {
        HttpClient clientSocket;

        [DataMember]
        public Station[] stations { get; set; }


        public JCDecauxItem(string cacheItemName)
        {
            clientSocket = new HttpClient();
            if(cacheItemName.Equals("All"))
            {
                HttpResponseMessage response = clientSocket.GetAsync("https://api.jcdecaux.com/vls/v3/stations?apiKey=2ba463e0d63cedfd5374762396b92c89cd41ec62").Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                stations = JsonSerializer.Deserialize<Station[]>(responseBody);
            }
            else
            {
                string[] key = cacheItemName.Split(':');
                HttpResponseMessage response = clientSocket.GetAsync("https://api.jcdecaux.com/vls/v3/stations/"+key[0]+"?contract=" + key[1] + "&apiKey=2ba463e0d63cedfd5374762396b92c89cd41ec62").Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                var station = JsonSerializer.Deserialize<Station>(responseBody);
                stations = new Station[] { station };
            }
        }
    }
}
