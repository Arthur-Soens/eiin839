﻿

using ServiceReference2;

class Program
{

    static async Task Main(string[] arg)
    {
        {
            ServiceReference2.MathsOperationsClient client = new MathsOperationsClient();

            int result = await  client.AddAsync(3, 4);
            Console.WriteLine(result);
            result = await client.SubstractAsync(3, 4);
            Console.WriteLine(result);
            result = await client.MultiplyAsync(3, 4);
            Console.WriteLine(result);
            float resultD = await client.DivideAsync(3, 4);
            Console.WriteLine(resultD);

            // Utilisez la variable 'client' pour appeler des opérations sur le service.

            // Fermez toujours le client.
            client.Close();
        }
    }
}