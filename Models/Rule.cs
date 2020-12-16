using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Rule
    {
        public Func<Washer, Placement, Familly, bool> Func { get; set; }
        public string Value = "";

        public Rule() 
        {
        }

        public virtual bool Check(Washer washer, Placement placement, Familly familly) 
        {
            return Func(washer, placement, familly);
        }
    }
}
