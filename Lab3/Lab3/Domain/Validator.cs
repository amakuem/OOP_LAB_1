using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Domain
{
    public static class Validator
    {
        public static void ValidateNewStudent(StudentDTO student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
                throw new ValidationException("The name cannot be empty.");
            if (student.Grade < 0 || student.Grade > 100)
                throw new ValidationException("The score should be from 0 to 100");
        }
    }
}
