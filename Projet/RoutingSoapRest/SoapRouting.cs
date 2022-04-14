using RoutingSoapRest.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RoutingSoapRest
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class SoapRouting : ISoapRouting
    {
        public string GetData(int value)
        {
            ProxyClient client = new ProxyClient();
            JCDecauxItem contracts = client.GetAllStations();
            String s = "";
            for(int i = 0; i < 20; i++)
            {
                Station contract = contracts.stations[i];
                s += "[ " + contract.position.latitude + "; " + contract.position.longitude + "]\n";
            }
            client.Close();
            return string.Format("You entered: {0}", s);
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

        public Way[] GetDirections(string start, string end)
        {
            return RestRouting.helper.GetWays(start, end);
        }
    }
}
