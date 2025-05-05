using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Domain
{
    public static class StudentFactory
    {
        public static Student Create(string name, int grade)
        {
            return new Student(name, grade);
        }
    }
}
