using Lab3.Application;
using Lab3.DataAccess;
using Lab3.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace TestProject1
{
    [TestClass]
    public class StudentServiceTests
    {
        private Mock<IStudentRepository> _mockRepo;
        private StudentService _studentService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IStudentRepository>();
            _studentService = new StudentService(_mockRepo.Object);
        }

        [TestMethod]
        public void AddStudent_ValidStudent_CallsRepositoryAdd()
        {
            // Arrange
            var studentDto = new StudentDTO { Name = "John Doe", Grade = 85 };

            // Act
            _studentService.AddStudent(studentDto);

            // Assert
            _mockRepo.Verify(r => r.AddStudent(It.Is<Student>(s =>
                s.Name == "John Doe" && s.Grade == 85)), Times.Once);
        }

        [TestMethod]
        public void EditStudent_ValidStudent_CallsRepositoryUpdate()
        {
            // Arrange
            var studentDto = new StudentDTO { Name = "John Doe", Grade = 90 };

            // Act
            _studentService.EditStudent(studentDto);

            // Assert
            _mockRepo.Verify(r => r.UpdateStudent(It.Is<Student>(s =>
                s.Name == "John Doe" && s.Grade == 90)), Times.Once);
        }

        [TestMethod]
        public void GetAllStudents_ReturnsStudentsFromRepository()
        {
            // Arrange
            var expectedStudents = new List<Student>
            {
                new Student("John", 80),
                new Student("Jane", 95)
            };
            _mockRepo.Setup(r => r.GetAllStudents()).Returns(expectedStudents);

            // Act
            var result = _studentService.GetAllStudents();

            // Assert
            CollectionAssert.AreEqual(expectedStudents, result);
        }
    }

    [TestClass]
    public class ValidatorTests
    {
        [TestMethod]
        public void ValidateNewStudent_EmptyName_ThrowsValidationException()
        {
            // Arrange
            var student = new StudentDTO { Name = "", Grade = 50 };

            // Act & Assert
            Assert.ThrowsException<ValidationException>(() =>
                Lab3.Domain.Validator.ValidateNewStudent(student));
        }

        [TestMethod]
        public void ValidateNewStudent_GradeBelowZero_ThrowsValidationException()
        {
            // Arrange
            var student = new StudentDTO { Name = "John", Grade = -5 };

            // Act & Assert
            Assert.ThrowsException<ValidationException>(() =>
                Lab3.Domain.Validator.ValidateNewStudent(student));
        }

        [TestMethod]
        public void ValidateNewStudent_GradeAbove100_ThrowsValidationException()
        {
            // Arrange
            var student = new StudentDTO { Name = "John", Grade = 105 };

            // Act & Assert
            Assert.ThrowsException<ValidationException>(() =>
                Lab3.Domain.Validator.ValidateNewStudent(student));
        }

        [TestMethod]
        public void ValidateNewStudent_ValidData_NoExceptionThrown()
        {
            // Arrange
            var student = new StudentDTO { Name = "John", Grade = 85 };

            // Act & Assert
            Lab3.Domain.Validator.ValidateNewStudent(student);
        }
    }

    [TestClass]
    public class StudentFactoryTests
    {
        [TestMethod]
        public void Create_ReturnsStudentWithCorrectProperties()
        {
            // Arrange
            string name = "Alice";
            int grade = 90;

            // Act
            var student = StudentFactory.Create(name, grade);

            // Assert
            Assert.AreEqual(name, student.Name);
            Assert.AreEqual(grade, student.Grade);
        }
    }

    
}
