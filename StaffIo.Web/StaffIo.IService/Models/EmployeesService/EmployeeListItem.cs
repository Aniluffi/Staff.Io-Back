using StaffIo.Data.Enums;

namespace StaffIo.IService.Models.EmployeesServices
{
    public class EmployeeListItem
    {
        public Guid UserId { get; set; }

        public EnumUserStatus Status { get; set; }

        public Guid? OwnerId { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public decimal Salary { get; set; }

        public bool IsAdmin { get; set; }

        public bool? AccessCanManage { get; set; }

        public string? FotoUrl { get; set; }

        public List<EmployeeListItem> Items { get; set; }
    }
}
