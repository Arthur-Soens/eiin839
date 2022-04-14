using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using RoutingSoapRest.Proxy;
using System.Device.Location;

namespace RoutingSoapRest
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IRestRouting
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "Coordinate?start={start}&end={end}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Way[] GetDirections(string start, string end);
        
    }
}