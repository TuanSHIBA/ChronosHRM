using System.Reflection;

namespace Chronos.Domain.Constants
{
    public static class Permissions
    {
        public static class Employees
        {
            public const string View = "Permissions.Employees.View";
            public const string Create = "Permissions.Employees.Create";
            public const string Edit = "Permissions.Employees.Edit";
            public const string Delete = "Permissions.Employees.Delete";
        }

        public static class Departments
        {
            public const string View = "Permissions.Departments.View";
            public const string Create = "Permissions.Departments.Create";
            public const string Edit = "Permissions.Departments.Edit";
            public const string Delete = "Permissions.Departments.Delete";
        }
        public static class EmploymentContracts
        {
            public const string View = "Permissions.EmploymentContracts.View";
            public const string Create = "Permissions.EmploymentContracts.Create";
            public const string Edit = "Permissions.EmploymentContracts.Edit";
            public const string Delete = "Permissions.EmploymentContracts.Delete";
        }
        public static class Attendances
        {
            public const string View = "Permissions.Attendances.View";
            public const string Create = "Permissions.Attendances.Create";
            public const string Edit = "Permissions.Attendances.Edit";
            public const string Delete = "Permissions.Attendances.Delete";
            public const string Approve = "Permissions.Attendances.Approve";
        }
        public static class LeaveTypes
        {
            public const string View = "Permissions.LeaveTypes.View";
            public const string Create = "Permissions.LeaveTypes.Create";
            public const string Edit = "Permissions.LeaveTypes.Edit";
            public const string Delete = "Permissions.LeaveTypes.Delete";
        }
        public static class LeaveRequest
        {
            public const string View = "Permissions.LeaveRequest.View";
            public const string Create = "Permissions.LeaveRequest.Create";
            public const string Edit = "Permissions.LeaveRequest.Edit";
            public const string Delete = "Permissions.LeaveRequest.Delete";
            public const string Approve = "Permissions.LeaveRequest.Approve";
        }
        public static class System
        {
            public const string Manage = "Permissions.System.Manage";
        }
        public static List<string> GetAllPermissions()
        {
            var permissions = new List<string>();

            // 1. Lấy tất cả các class con (Nested Classes) như Employees, Departments...
            var nestedTypes = typeof(Permissions).GetNestedTypes();

            foreach (var type in nestedTypes)
            {
                // 2. Lấy tất cả các trường (Field) là public, static và const
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                 .Where(fi => fi.IsLiteral && !fi.IsInitOnly); // IsLiteral = const

                foreach (var field in fields)
                {
                    // 3. Lấy giá trị của hằng số đó
                    var value = field.GetValue(null);
                    if (value is string permString)
                    {
                        permissions.Add(permString);
                    }
                }
            }

            return permissions;
        }
    }
}