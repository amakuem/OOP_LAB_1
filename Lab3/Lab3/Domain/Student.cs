using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Domain
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public Student() { }
        public Student(string name, int grade)
        {
            Name = name;
            Grade = grade;
        }
    }
}
