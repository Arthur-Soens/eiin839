using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    internal class Program
    {
        public static void Main()
        {
            var svc = new ServiceHost(typeof(Proxy));
            svc.Open();
            Console.WriteLine("Proxy Ready");
            Console.WriteLine("Awaiting Request !");
            Console.ReadLine();
        }
    }
}
