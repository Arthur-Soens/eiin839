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
            Console.WriteLine("Request received");
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
            Console.WriteLine("Request received");
            try
            {
                Console.WriteLine("Start research (Best directions between "+start+" and " +end+")");
                var result = RestRouting.helper.GetWays(start, end);
                Console.WriteLine("Research finish, sending info to Heavy client");
                return result;
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

        public Station GetDirection(string start)
        {
            Console.WriteLine("Request received");
            try
            {
                Console.WriteLine("Start research (The closest station from " + start + ")");
                var result = RestRouting.helper.GetStation(start);
                Console.WriteLine("Research finish, sending info to Heavy client");
                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Due to a wrong address");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
