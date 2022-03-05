using System.Text.Json;

namespace Exercice2
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
            return "Name : " + name + " Commercial : " + commercial_name + " Country code : " + country_code + " Cities : " + String.Join(";", cities);
        }
    }

    public class Position
    {
        public float latitude { get; set; }
        public float longitude { get; set; }

        public string toString()
        {
            return "position : \n\tlatitude : " + latitude + "\n\tlongitude : " + longitude + "\n";
        }
    }

    public class Availibilities
    {
        public int bikes { get; set; }
        public int stands { get; set; }
        public int mechanicalBikes { get; set; }
        public int electricalBikes { get; set; }
        public int electricalInternalBatteryBikes { get; set; }

        public int electricalRemovableBatteryBikes { get; set; }
        public string toString()
        {
            return "\tavailabilities : " +
                    "\n\t\tbikes : " + bikes +
                    "\n\t\tstands : " + stands +
                    "\n\t\tmechanicalBikes : " + mechanicalBikes +
                    "\n\t\telectricalBikes : " + electricalBikes +
                    "\n\t\telectricalInternalBatteryBikes : " + electricalInternalBatteryBikes +
                    "\n\t\telectricalRemovableBatteryBikes : " + electricalRemovableBatteryBikes + "\n";
        }
    }
    public class Stands
    {
        public Availibilities availabilities { get; set; }
        public int capacity { get; set; }
        public string toString()
        {
            return "overflowStands : \n" + availabilities.toString() + "\n\t capacity : " + capacity + "\n";
        }

        
    }

    public class Station
    {
        public int number { get; set; }
        public string contractName { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Position position { get; set; }
        public bool banking { get; set; }
        public bool bonus { get; set; }
        public string status { get; set; }
        public string lastUpdate { get; set; }
        public bool connected { get; set; }
        public bool overflow { get; set; }
        public string shape { get; set; }

        public Stands totalStands { get; set; }
        public Stands mainStands { get; set; }
        public object? overflowStands { get; set; }

        public string toString()
        {
            string text = "number :" + number +
                     "\ncontractName : " + contractName +
                     "\nname : " + name +
                     "\naddress : " + address +
                     position.toString() +
                     "banking : " + banking +
                     "\nbonus : " + bonus +
                     "\nstatus : " + status +
                     "\nlastUpdate : " + lastUpdate +
                     "\nconnected : " + connected +
                     "\noverflow : " + overflow +
                     "\nshape : " + shape + "\n";
            if (totalStands != null)
            {
                text += totalStands.toString();
            }
            if (mainStands != null)
            {
                text += mainStands.toString();
            }
            if (overflowStands != null)
            {
                text += overflowStands;
            }
            return text;
        }
    }
    class Client
    {
        static HttpClient clientSocket;

        static async Task Main(string[] args)
        {
            clientSocket = new HttpClient();
            HttpResponseMessage response = await clientSocket.GetAsync("https://api.jcdecaux.com/vls/v3/stations?contract=" + args[0] + "&apiKey=2ba463e0d63cedfd5374762396b92c89cd41ec62");
            string responseBody = await response.Content.ReadAsStringAsync();
            List<Station>? stationList = JsonSerializer.Deserialize<List<Station>>(responseBody);


            foreach (Station station in stationList)
            {
                Console.WriteLine(station.toString());
            }
            Console.WriteLine();


        }
    }
}