using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MathLibraryRest
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IMathsOperations
    {
        //Chaque [webInvoke] permet de definir la methode utilisé et les arguments à passer dans l'url ainsi que leur format
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Add?x={x}&y={y}")]
        //Pour tester add voici une url : http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Add?x=1&y=2
        int Add(int x, int y);
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Mult?x={x}&y={y}")]
        //Pour tester multiply voici une url : http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Mult?x=2&y=4
        int Multiply(int x, int y);
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Sub?x={x}&y={y}")]
        //Pour tester substract voici une url : http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Sub?x=1&y=2
        int Substract(int x, int y);
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Div?x={x}&y={y}")]
        //Pour tester divide voici une url : http://localhost:8734/Design_Time_Addresses/MathsLibrary/MathsOperations/Div?x=1&y=2
        float Divide(int x, int y);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: ajoutez vos opérations de service ici
    }

    // Utilisez un contrat de données comme indiqué dans l'exemple ci-après pour ajouter les types composites aux opérations de service.
    // Vous pouvez ajouter des fichiers XSD au projet. Une fois le projet généré, vous pouvez utiliser directement les types de données qui y sont définis, avec l'espace de noms "MathLibraryRest.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
