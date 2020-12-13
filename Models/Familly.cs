using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Familly
    {
        public int CntPeople { get; set; }
        public Boolean Children { get; set; }
        public Budget Budget { get; set; }
    }

    public enum Budget 
    {
        small,
        normal,
        big
    }
}
