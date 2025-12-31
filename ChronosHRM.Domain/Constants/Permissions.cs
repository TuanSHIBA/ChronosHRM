namespace Chronos.Domain.Constants
{
    public static class Permissions
    {
        // Module Nhân viên
        public static class Employees
        {
            public const string View = "Permissions.Employees.View";
            public const string Create = "Permissions.Employees.Create";
            public const string Edit = "Permissions.Employees.Edit";
            public const string Delete = "Permissions.Employees.Delete";
        }

        // Module Phòng ban
        public static class Departments
        {
            public const string View = "Permissions.Departments.View";
            public const string Create = "Permissions.Departments.Create";
            public const string Edit = "Permissions.Departments.Edit";
            public const string Delete = "Permissions.Departments.Delete";
        }
    }
}