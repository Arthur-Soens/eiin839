using System.Text.Json;

namespace Exercice1
{
    public class Contract
    {
        public string name { get; set; }
        public string commercial_name { get; set; }
        public List<string> cities { get; set; }
        public string country_code { get; set; }

        public String toString()
        {
            if (cities == null)
            {
                return "Name : " + name + " Commercial : " + commercial_name + " Country code : " + country_code;
            }
            return "Name : " + name + ", Commercial : " + commercial_name + ", Country code : " + country_code + ", Cities : " + String.Join(";", cities) + "\n";
        }
    }

    class Client
    {
        static HttpClient clientSocket;

        static async Task Main(string[] args)
        {
            clientSocket = new HttpClient();
            HttpResponseMessage response = await clientSocket.GetAsync("https://api.jcdecaux.com/vls/v3/contracts?apiKey=2ba463e0d63cedfd5374762396b92c89cd41ec62");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            List<Contract> list = JsonSerializer.Deserialize<List<Contract>>(responseBody);

            foreach (Contract contract in list)
            {
                Console.WriteLine(contract.toString());
            }
            Console.WriteLine();
        }
    }
}