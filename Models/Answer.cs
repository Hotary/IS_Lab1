using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Lab1.Models
{
    public class Answer
    {
        public string Value;
        public Action<Question> Action { get; set; }
    }
}
