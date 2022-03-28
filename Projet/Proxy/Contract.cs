using System.Collections.Generic;

namespace Proxy
{
    public class Contract
    {
        public string name { get; set; }
        public string commercial_name { get; set; }
        public List<string> cities { get; set; }
        public string country_code { get; set; }

        public string toString()
        {
            if (cities == null)
            {
                return "Name : " + name + " Commercial : " + commercial_name + " Country code : " + country_code;
            }
            return "Name : " + name + ", Commercial : " + commercial_name + ", Country code : " + country_code + ", Cities : " + string.Join(";", cities) + "\n";
        }
    }
}