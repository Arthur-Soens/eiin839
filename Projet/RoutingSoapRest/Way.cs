using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoutingSoapRest
{
    [DataContract]
    public class Way
    {
        [DataMember]
        public PropertiesDist properties { get; set; }
        [DataMember]
        public GeometryDist geometry { get; set; }
        [DataMember]
        public bool isError { get; set; }
        [DataMember]
        public string message { get; set; }

        public Way(PropertiesDist p,GeometryDist g)
        {
            properties = p;
            geometry = g;
            isError = false;
            message = "All is good";
        }

        public Way(bool p, string g)
        {
            isError = p;
            message = g;
        }

    }
}
