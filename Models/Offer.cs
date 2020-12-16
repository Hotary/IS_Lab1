using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Offer
    {
        public Washer Washer { get; set; } = new Washer();
        public Client Client { get; set; } = new Client();
        public Familly Familly => Client.Familly;
        public Placement Placement => Client.Placement;
        public int Price { get; set; } 
        public string PriceId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; } // url
        public string Character { get; set; }
        public string Rating { get; set; }
        public string URL { get; set; }
        public string guid { get; set; }
    }
}
