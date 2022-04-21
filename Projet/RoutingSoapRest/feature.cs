using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoutingSoapRest
{
    internal class Reponse
    {
        public Feature[] features { get; set; }
    }

    internal class Feature
    {
        public Geometry geometry { get; set; }
        public Properties properties { set; get; }
    }

    public class Properties
    {

    }

    public class Geometry
    {
        public double[] coordinates { get; set; }

        public double getLatitude()
        {
            return coordinates[1];
        }

        public double getLongitude()
        {
            return coordinates[0];
        }
    }

    public class ReponseDist
    {
        public FeatureDist[] features { get; set; }
    }

    public class FeatureDist
    {
        public GeometryDist geometry { get; set; }
        public PropertiesDist properties { set; get; }
    }

    [DataContract]
    public class PropertiesDist
    {
        [DataMember]
        public Summary summary { get; set; }

        [DataMember]
        public SegmentDist[] segments { get; set; }

    }

    [DataContract]
    public class SegmentDist
    {
        [DataMember]
        public Step[] steps { get; set; }
    }

    [DataContract]
    public class Step
    {
        [DataMember]
        public double distance { get; set; }
        [DataMember]
        public double duration { get; set; }
        [DataMember]
        public string instruction { get; set; }

    }

    [DataContract]
    public class Summary
    {
        [DataMember]
        public double distance { get; set; }
        [DataMember]
        public double duration { get; set; }
    }

    [DataContract]
    public class GeometryDist
    {
        [DataMember]
        public double[][] coordinates { get; set; }
    }
}
