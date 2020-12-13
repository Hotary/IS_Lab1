using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Washer
    {
        public LoadType LoadType { get; set; }
        public Boolean Drying { get; set; }
        public Boolean SpecialPrograms { get; set; }
        public Boolean InverterMotor { get; set; }
        public Boolean AddingLaundry { get; set; }
        public Boolean DelayStart { get; set; }
        public Boolean LeakageProtection { get; set; }
    }

    public enum LoadType 
    {
        Vertical,
        Frontal
    }
}
