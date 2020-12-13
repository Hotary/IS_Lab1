using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Placement
    {
        public Area Area { get; set; }
    }

    public enum Area 
    {
        small,
        normal,
        big
    }
}
