using StaffIo.Data.Enums;

namespace StaffIo.IService.Models.EmployeesServices.Request
{
    public class EmployeesGetListRequest
    {
        public EnumUserStatus? Status { get; set; }

        public string? Search {  get; set; }
    }
}
