using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class Proxy : IProxy
    {

        ObjectCache cache;
        ProxyCache<JCDecauxItem> decauxCache = new ProxyCache<JCDecauxItem>();
        public List<Contract> GetContracts()
        {
            cache = MemoryCache.Default;
            if (cache.Get("Contracts") != null)
            {
                return (List<Contract>)cache.Get("Contracts");
            }
            HttpClient clientSocket = new HttpClient();
            HttpResponseMessage response = clientSocket.GetAsync("https://api.jcdecaux.com/vls/v3/contracts?apiKey=2ba463e0d63cedfd5374762396b92c89cd41ec62").Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            List<Contract> list = JsonSerializer.Deserialize<List<Contract>>(responseBody);
            cache.Set("Contracts", list, null);
            return list;
        }

        public JCDecauxItem GetAllStations()
        {
            return (JCDecauxItem) decauxCache.Get("All");
        }

        public JCDecauxItem GetStations(string key)
        {
            return (JCDecauxItem)decauxCache.Get(key, 60.0);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
