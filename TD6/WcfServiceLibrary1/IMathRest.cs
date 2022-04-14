using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.ServiceModel.Web;

namespace WcfServiceLibrary1
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IMathRest
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Multiply?n1={n1}&n2={n2}", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        int Multiply(int n1, int n2);

    }
}
