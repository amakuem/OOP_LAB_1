using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Users
{
    public static class Session
    {
        public static User CurrentUser { get; private set; }
        public static IPermissionStrategy PermissionStrategy { get; private set; }

        public static void Login(User user)
        {
            CurrentUser = user;
            //PermissionStrategy = user.Role switch
            //{
            //    UserRole.Viewer => new ViewerPermission(),
            //    UserRole.Editor => new EditorPermission(),
            //    UserRole.Admin => new AdminPermission(),
            //    _ => new ViewerPermission()
            //};
        }
        
        public static void SetPermision(User user)
        {
            if(user.Role == UserRole.None)
            {
                throw new Exception("This user don't have any permissions"); //добавить  try-catch чтобы обработать это
                return;
            }
            PermissionStrategy = user.Role switch
            {
                UserRole.Viewer => new ViewerPermission(),
                UserRole.Editor => new EditorPermission(),
                UserRole.Admin => new AdminPermission(),
            };
        }
    }
}
