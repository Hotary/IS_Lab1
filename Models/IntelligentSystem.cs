using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class IntelligentSystem
    {
        public Client Client { get; set; }
        public Familly Familly => Client.Familly;
        public Placement Placement => Client.Placement;
        public Washer Washer { get; set; }
        public Database Database { get; set; }

        private List<Rule> rules;

        public IntelligentSystem() 
        {
            Client = new Client();
            Washer = new Washer();
            rules = new List<Rule>()
            {
                new Rule() //1
                {
                    Value = "LG FH0B8LD7",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.small && w.LeakageProtection == false; }
                },
                new Rule() //2
                {
                    Value = "Indesit MTW A51051",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.normal && f.CntPeople <=1 &&
                        w.LoadType == LoadType.Vertical && w.DelayStart == false; }
                },
                new Rule() //3
                {
                    Value = "LG M1222WD3",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.small && w.LeakageProtection == true; }
                },
                new Rule() //4
                {
                    Value = "Zanussi ZWY50924CI",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.normal && f.CntPeople <= 1 &&
                        w.LoadType == LoadType.Vertical && w.DelayStart == true; }
                },
                new Rule() //5
                {
                    Value = "Samsung WW70K62E00W",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.normal && f.CntPeople >= 5 &&
                        w.AddingLaundry == false; }
                },
                new Rule() //6
                {
                    Value = "Bosch Serie 6 3D Washing WLT24460OE",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.normal && f.CntPeople >= 5 &&
                        w.AddingLaundry == true; }
                },
                new Rule() //7
                {
                    Value = "LG FH0B8LD6",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.normal && f.CntPeople <= 1 &&
                        w.LoadType == LoadType.Frontal; }
                },
                new Rule() //8
                {
                    Value = "Bosch Serie | 6 WAT286H2OE",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.big && (f.CntPeople >= 2 && f.CntPeople <= 4) &&
                        w.LoadType == LoadType.Frontal && p.Area == Area.big; }
                },
                new Rule() //9
                {
                    Value = "Haier HW70-BP12969A",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.big && (f.CntPeople >= 2 && f.CntPeople <= 4) &&
                        w.LoadType == LoadType.Frontal && p.Area == Area.small; }
                },
                new Rule() //10
                {
                    Value = "Bosch Serie | 4 WAN24290OE",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.big && (f.CntPeople >= 2 && f.CntPeople <= 4) &&
                        w.LoadType == LoadType.Frontal && p.Area == Area.big; }
                },
                new Rule() //11
                {
                    Value = "Electrolux PerfectCare 800 EW8T3R562",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.big && (f.CntPeople >= 2 && f.CntPeople <= 4) &&
                        w.LoadType == LoadType.Vertical; }
                },
                new Rule() //12
                {
                    Value = "Electrolux EWC1150",
                    Func = (w,p,f) => {return w.Drying == false && f.Budget == Budget.big && f.CntPeople <= 1; }
                },
                new Rule() //13
                {
                    Value = "Siemens WD14H442OE",
                    Func = (w,p,f) => {return w.Drying == true && f.Budget == Budget.big && f.CntPeople >= 5 &&
                        w.LoadType == LoadType.Frontal && p.Area == Area.big && w.SpecialPrograms == false; }
                },
                new Rule() //14
                {
                    Value = "Samsung WD806U2GAWQ",
                    Func = (w,p,f) => {return w.Drying == true && f.Budget == Budget.big && f.CntPeople >= 5 &&
                        w.LoadType == LoadType.Frontal && p.Area == Area.normal; }
                },
                new Rule() //15
                {
                    Value = "Electrolux PerfectCare 800 EW8T3R562",
                    Func = (w,p,f) => {return w.Drying == true && f.Budget == Budget.big && (f.CntPeople >= 2 && f.CntPeople <= 4); }
                },
                new Rule() //16
                {
                    Value = "Bosch Serie|6 WDU28590OE",
                    Func = (w,p,f) => {return w.Drying == true && f.Budget == Budget.big && f.CntPeople >= 5 &&
                        w.LoadType == LoadType.Frontal && p.Area == Area.big && w.SpecialPrograms == true; }
                }
            };
        }

        public List<Offer> GetOffers() 
        {
            Rule rule = null;
            foreach(var r in rules) 
            {
                if (r.Check(Washer, Placement, Familly)) 
                {
                    rule = r;
                    break;
                }
            }
            if (rule == null)
                return null;
            var list = new List<Offer>();
            foreach (var o in Database.Offers) 
            {
                if (rule.Check(o.Washer, o.Placement, o.Familly))
                    list.Add(o);
            }
            return list;
        }
    }
}
