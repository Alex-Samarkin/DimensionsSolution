using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim5
{
    public class ToleranceIntervalJson
    {
        public string Range { get; set; }
        public Dictionary<string, double> HoleDeviations { get; set; }
        public Dictionary<string, double> ShaftDeviations { get; set; }
    }
}
