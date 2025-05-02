using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Users
{
    public class ViewerPermission : IPermissionStrategy
    {
        public bool CanView() => true;
        public bool CanEdit() => false;
        public bool CanManageUsers() => false;
    }

    public class EditorPermission : IPermissionStrategy
    {
        public bool CanView() => true;
        public bool CanEdit() => true;
        public bool CanManageUsers() => false;
    }
    public class AuditorPermission : IPermissionStrategy
    {
        public bool CanView() => true;
        public bool CanEdit() => true;
        public bool CanManageUsers() => false;
    }

    public class AdminPermission : IPermissionStrategy
    {
        public bool CanView() => true;
        public bool CanEdit() => true;
        public bool CanManageUsers() => true;
    }
}
