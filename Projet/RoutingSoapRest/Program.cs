using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace RoutingSoapRest
{
    internal class Program
    {
        public static void Main()
        {
            var svc = new ServiceHost(typeof(RoutingSoapRest.SoapRouting));
            svc.Open();
            var svc2 = new ServiceHost(typeof(RoutingSoapRest.RestRouting));
            svc2.Open();
            Console.WriteLine("Routing Ready");
            Console.WriteLine("Awaiting Request !");
            Console.ReadLine();
        }
    }
}
