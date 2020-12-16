using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.ViewModel
{
    public class VM_IS
    {
        private Models.IntelligentSystem IS = new Models.IntelligentSystem();
        public Models.Question Question { get; private set; }
        public Models.Database Database { get; private set; } = new Models.Database();

        public VM_IS()
        {
            IS.Database = Database;
            var q11 = new Models.Question()
            {
                Value = "Вам нужна функция защиты от протечек?",
                Action = delegate (Models.Question q) { IS.Washer.LeakageProtection = q.ValueBoolean; },
                Type = typeof(bool),
                Next = null
            };
            var q10 = new Models.Question()
            {
                Value = "Вам нужна функция отсрочки старта?",
                Action = delegate (Models.Question q) { IS.Washer.DelayStart = q.ValueBoolean; },
                Type = typeof(bool),
                Next = q11
            };
            var q9 = new Models.Question()
            {
                Value = "Вам нужна функция дозагрузки белья?",
                Action = delegate (Models.Question q) { IS.Washer.AddingLaundry = q.ValueBoolean; },
                Type = typeof(bool),
                Next = q10
            };
            var q8 = new Models.Question()
            {
                Value = "Вам нужна с/м с инверторным двигателем?",
                Action = delegate (Models.Question q) { IS.Washer.InverterMotor = q.ValueBoolean; },
                Type = typeof(bool),
                Next = q9
            };
            var q7 = new Models.Question()
            {
                Value = "Нужны ли вам особые программы для стирки (пуховое одеяло, спорт. режим и тп)?",
                Action = delegate (Models.Question q) { IS.Washer.SpecialPrograms = q.ValueBoolean; },
                Type = typeof(bool),
                Next = q8
            };
            var q6 = new Models.Question()
            {
                Value = "В вашей семье есть дети?",
                Action = delegate (Models.Question q) { IS.Familly.Children = q.ValueBoolean; },
                Type = typeof(bool),
                Next = q7
            };
            var q5 = new Models.Question()
            {
                Value = "Какова свободная площадь для размещения с/м?",
                Action = delegate (Models.Question q) { },
                Type = typeof(Models.Answer),
                Next = q6
            };
            q5.Answers.Add(new Models.Answer()
            {
                Value = "Менее 45 см^2 в глубину и ширину – узкая",
                Action = delegate (Models.Question q) { IS.Placement.Area = Models.Area.small; }
            });
            q5.Answers.Add(new Models.Answer()
            {
                Value = "45.1-55 см^2 в глубину и ширину – компактная",
                Action = delegate (Models.Question q) { IS.Placement.Area = Models.Area.normal; }
            });
            q5.Answers.Add(new Models.Answer()
            {
                Value = "Более 55.1 см^2 в глубину и ширину – стандартная",
                Action = delegate (Models.Question q) { IS.Placement.Area = Models.Area.big; }
            });
            var q4 = new Models.Question()
            {
                Value = "Какой способ загрузки вам удобен?",
                Action = delegate (Models.Question q) {},
                Type = typeof(Models.Answer),
                Next = q5
            };
            q4.Answers.Add(new Models.Answer()
            {
                Value = "Вертикальный",
                Action = delegate (Models.Question q) { IS.Washer.LoadType = Models.LoadType.Vertical; }
            });
            q4.Answers.Add(new Models.Answer()
            {
                Value = "Фронтальный",
                Action = delegate (Models.Question q) { IS.Washer.LoadType = Models.LoadType.Frontal; }
            });

            var q3 = new Models.Question()
            {
                Value = "Сколько у вас человек в семье?",
                Action = delegate (Models.Question q) { IS.Familly.CntPeople = q.ValueInt; },
                Type = typeof(int),
                Next = q4
            };
            var q2 = new Models.Question()
            {
                Value = "Какой у вас бюджет?",
                Action = delegate (Models.Question q) { },
                Type = typeof(Models.Answer),
                Next = q3
            };
            q2.Answers.Add(new Models.Answer()
            {
                Value = "Менее 20,000р",
                Action = delegate (Models.Question q) { IS.Familly.Budget = Models.Budget.small; }
            });
            q2.Answers.Add(new Models.Answer()
            {
                Value = "20,000р - 35,000р",
                Action = delegate (Models.Question q) { IS.Familly.Budget = Models.Budget.normal; }
            });
            q2.Answers.Add(new Models.Answer()
            {
                Value = "Более 35,000р",
                Action = delegate (Models.Question q) { IS.Familly.Budget = Models.Budget.big; }
            });
            var q1 = new Models.Question()
            {
                Value = "Вам нужна стиральная машинка с сушкой?",
                Action = delegate (Models.Question q) { IS.Washer.Drying = q.ValueBoolean; },
                Type = typeof(bool),
                Next = q2
            };
            Question = q1;
        }

        public string GetWasher() 
        {
            var str = "";
            var result = IS.GetOffers();

            if (result is null)
                return "404";
            foreach (var o in result) 
            {
                str += o.Name + "; ";
            }
            return str;
        }

        public List<Models.Offer> GetOffers()
        {
            return IS.GetOffers();
        }
    }
}
