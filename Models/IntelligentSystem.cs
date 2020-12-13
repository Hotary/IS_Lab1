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
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.small && Washer.LeakageProtection == false; }
                },
                new Rule() //2
                {
                    Value = "Indesit MTW A51051",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.normal && Familly.CntPeople <=1 &&
                        Washer.LoadType == LoadType.Vertical && Washer.DelayStart == false; }
                },
                new Rule() //3
                {
                    Value = "LG M1222WD3",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.small && Washer.LeakageProtection == true; }
                },
                new Rule() //4
                {
                    Value = "Zanussi ZWY50924CI",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.normal && Familly.CntPeople <= 1 &&
                        Washer.LoadType == LoadType.Vertical && Washer.DelayStart == true; }
                },
                new Rule() //5
                {
                    Value = "Samsung WW70K62E00W",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.normal && Familly.CntPeople >= 5 &&
                        Washer.AddingLaundry == false; }
                },
                new Rule() //6
                {
                    Value = "Bosch Serie 6 3D Washing WLT24460OE",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.normal && Familly.CntPeople >= 5 &&
                        Washer.AddingLaundry == true; }
                },
                new Rule() //7
                {
                    Value = "LG FH0B8LD6",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.normal && Familly.CntPeople <= 1 &&
                        Washer.LoadType == LoadType.Frontal; }
                },
                new Rule() //8
                {
                    Value = "Bosch Serie | 6 WAT286H2OE",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.big && (Familly.CntPeople >= 2 && Familly.CntPeople <= 4) &&
                        Washer.LoadType == LoadType.Frontal && Placement.Area == Area.big; }
                },
                new Rule() //9
                {
                    Value = "Haier HW70-BP12969A",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.big && (Familly.CntPeople >= 2 && Familly.CntPeople <= 4) &&
                        Washer.LoadType == LoadType.Frontal && Placement.Area == Area.small; }
                },
                new Rule() //10
                {
                    Value = "Bosch Serie | 4 WAN24290OE",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.big && (Familly.CntPeople >= 2 && Familly.CntPeople <= 4) &&
                        Washer.LoadType == LoadType.Frontal && Placement.Area == Area.big; }
                },
                new Rule() //11
                {
                    Value = "Electrolux PerfectCare 800 EW8T3R562",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.big && (Familly.CntPeople >= 2 && Familly.CntPeople <= 4) &&
                        Washer.LoadType == LoadType.Vertical; }
                },
                new Rule() //12
                {
                    Value = "Electrolux EWC1150",
                    Func = () => {return Washer.Drying == false && Familly.Budget == Budget.big && Familly.CntPeople <= 1; }
                },
                new Rule() //13
                {
                    Value = "Siemens WD14H442OE",
                    Func = () => {return Washer.Drying == true && Familly.Budget == Budget.big && Familly.CntPeople >= 5 &&
                        Washer.LoadType == LoadType.Frontal && Placement.Area == Area.big && Washer.SpecialPrograms == false; }
                },
                new Rule() //14
                {
                    Value = "Samsung WD806U2GAWQ",
                    Func = () => {return Washer.Drying == true && Familly.Budget == Budget.big && Familly.CntPeople >= 5 &&
                        Washer.LoadType == LoadType.Frontal && Placement.Area == Area.normal; }
                },
                new Rule() //15
                {
                    Value = "Electrolux PerfectCare 800 EW8T3R562",
                    Func = () => {return Washer.Drying == true && Familly.Budget == Budget.big && (Familly.CntPeople >= 2 && Familly.CntPeople <= 4); }
                },
                new Rule() //16
                {
                    Value = "Bosch Serie|6 WDU28590OE",
                    Func = () => {return Washer.Drying == true && Familly.Budget == Budget.big && Familly.CntPeople >= 5 &&
                        Washer.LoadType == LoadType.Frontal && Placement.Area == Area.big && Washer.SpecialPrograms == true; }
                }
            };
        }

        public string GetWasher() 
        {
            Rule rule = null;
            foreach(var r in rules) 
            {
                if (r.Check()) 
                {
                    rule = r;
                    break;
                }
            }
            if (rule == null)
                return "404: Washer not found";
            return rule.Value;
        }
    }
}
