using ServiceReference1;

namespace TD4
{

    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            var client = new ServiceReference1.CalculatorSoapClient(CalculatorSoapClient.EndpointConfiguration.CalculatorSoap);
            Console.WriteLine(await client.ChannelFactory.CreateChannel().AddAsync(73, 30));
        }
    }
}