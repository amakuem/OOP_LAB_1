using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Lab2.Document;

namespace Lab2.Users
{
    public class User: IObserver
    {
        public string Name { get; }
        public UserRole Role { get; set; }
        [JsonConstructor]
        public User(string name, UserRole role)
        {
            Name = name;
            Role = role;
        }

        public void Update(string message)
        {
            Console.WriteLine($"[Notification to {Name}]: {message}");
        }
        public override string ToString() => $"{Name}";
    }

}
