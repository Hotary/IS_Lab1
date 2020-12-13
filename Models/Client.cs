using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Client
    {
        public Familly Familly { get; } = new Familly();
        public Placement Placement { get; } = new Placement();
    }
}
