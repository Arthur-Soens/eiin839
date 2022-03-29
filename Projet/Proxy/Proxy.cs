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
        public async Task<List<Contract>> GetContracts(int value)
        {
            ObjectCache cache = MemoryCache.Default;
            var expirationTime = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30.0),
            };
            if (cache.Get("Contracts") != null)
            {
                return (List<Contract>)cache.Get("Contracts");
            }
            HttpClient clientSocket = new HttpClient();
            HttpResponseMessage response = await clientSocket.GetAsync("https://api.jcdecaux.com/vls/v3/contracts?apiKey=2ba463e0d63cedfd5374762396b92c89cd41ec62");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            List<Contract> list = JsonSerializer.Deserialize<List<Contract>>(responseBody);
            cache.Set("Contracts", list, expirationTime);
            return list;
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
