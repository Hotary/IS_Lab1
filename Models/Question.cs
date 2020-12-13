using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Question
    {
        public string Value { get; set; }
        public Type Type { get; set; }
        public Question Next = null;
        public List<Answer> Answers { get; } = new List<Answer>();

        public Action<Question> Action { get; set; }
        public void SetValue() 
        {
            Action(this);
        }
        public int ValueInt { get; set; }
        public bool ValueBoolean { get; set; }
    }
}
