

using RoutingServer;

class Program
{

    static void Main(string[] arg)
    {
        {
            SoapRoutingClient client = new SoapRoutingClient();
            string test = client.GetDataAsync(5).Result;
  
            client.CloseAsync();
        }
    }
}