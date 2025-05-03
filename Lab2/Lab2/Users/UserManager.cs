using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Users
{
    public static class UserManager
    {
        private const string UsersFilePath = "D:\\!\\4sem\\OOP\\LABS\\Lab2\\Lab2\\UserData.json";
        private static List<User> _users;

        static UserManager()
        {
            LoadUsers();
        }

        public static List<User> Users => _users;

        private static void LoadUsers()
        {
            if (!File.Exists(UsersFilePath))
            {
                _users = new List<User>();
                return;
            }

            var json = File.ReadAllText(UsersFilePath);
            _users = JsonConvert.DeserializeObject<List<User>>(json);
        }

        public static void SaveUsers()
        {
            var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
            File.WriteAllText(UsersFilePath, json);
        }
        public static void AddUser(string userName)
        {
            var newUser = new User(userName, UserRole.None);
            _users.Add(newUser);
            SaveUsers();
        }
        public static void CheckUser(User user, Document.Document doc)
        {
            for (int i = 0; i < doc.Editors.Count; i++)
            {
                if (user.Name == doc.Editors[i])
                {
                    if(user.Role != UserRole.Admin)
                        UpdateUserRole(user.Name, UserRole.Editor);
                    return;
                }
            }
            for (int i = 0; i < doc.Viewers.Count; i++)
            {
                if(user.Name == doc.Viewers[i])
                {
                    UpdateUserRole(user.Name, UserRole.Viewer);
                    return;
                }
            }
            UpdateUserRole(user.Name, UserRole.None);
        }
        public static void UpdateUserRole(string userName, UserRole newRole)
        {
            var user = _users.FirstOrDefault(u => u.Name == userName);
            if (user != null)
            {
                user.Role = newRole;
                SaveUsers();
            }
        }
        public static IEnumerable<User> GetAdmins()
        => Users.Where(u => u.Role == UserRole.Admin);
    }
}
