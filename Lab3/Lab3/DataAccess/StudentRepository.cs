using Lab3.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace Lab3.DataAccess
{
    public class StudentRepository: IStudentRepository
    {
        private List<Student> _students;
        private readonly string _filePath;
        public StudentRepository()
        {
            _filePath = "students.json";
            LoadStudents();
        }
        private void LoadStudents()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _students = JsonConvert.DeserializeObject<List<Student>>(json) ?? new List<Student>();
            }
            else
            {
                _students = new List<Student>();
            }
        }

        private void SaveStudents()
        {
            var json = JsonConvert.SerializeObject(_students, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void AddStudent(Student student)
        {
            student.Id = _students.Any() ? _students.Max(s => s.Id) + 1 : 1;
            _students.Add(student);
            SaveStudents();
        }
        public void UpdateStudent(Student student)
        {
            var existing = _students.Find(s => s.Name == student.Name);
            if (existing != null)
            {
                _students.Remove(existing);
                _students.Add(student);
                SaveStudents();
            }
        }
        public List<Student> GetAllStudents()
        {
            return _students;
        }
    }
}
