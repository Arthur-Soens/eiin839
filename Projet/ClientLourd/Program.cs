

using ServiceReference1;

class Program
{

    static void Main(string[] arg)
    {
        Console.WriteLine("-------------Client lourd-------------\n");
        while (true)
        {
            
            SoapRoutingClient client = new SoapRoutingClient();

            Console.WriteLine("Taper \"1\" pour compter le temps necessaire à la recuperation de la station la plus proche du point, sinon taper \"entrer\"");
            if (Console.ReadLine() == "1")
            {
                Console.WriteLine("Départ : ");
                string variable = Console.ReadLine();
                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine("Execution en cours ...\n");
                    DateTime tempsdeb = DateTime.Now;
                    Station test = client.GetDirectionAsync(variable).Result;
                    TimeSpan diffTemps = DateTime.Now - tempsdeb;
                    Console.WriteLine(string.Format("Temps d'éxecution :  {0} \n", diffTemps.ToString()));
                }
            }
            else
            {
                Console.WriteLine("Départ : ");
                string variable = Console.ReadLine();
                Console.WriteLine("Arrivé : ");
                string variable2 = Console.ReadLine();

                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine("Execution en cours ...\n");
                    DateTime tempsdeb = DateTime.Now;
                    Way[] test = client.GetDirectionsAsync(variable, variable2).Result;
                    TimeSpan diffTemps = DateTime.Now - tempsdeb;
                    Console.WriteLine(string.Format("Temps d'éxecution :  {0} \n", diffTemps.ToString()));
                }
            }
            client.CloseAsync();
            Console.WriteLine("Taper \"quit\" pour quitter, sinon entrer");
            if (Console.ReadLine() == "quit")
            {
                break;
            }
        }
    }
}