using RoutingSoapRest.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingSoapRest
{
    public class InfoStation
    {
        public Station station { get; set; }
        public ReponseDist reponseDist { get; set; }

        public InfoStation() { }
        public InfoStation(Station s, ReponseDist r)
        {
            station = s;
            reponseDist = r;
        }
    }
}
