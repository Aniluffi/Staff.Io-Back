namespace StaffIo.IService.Models.EmployeesServices.Response
{
    public class EmployeesGetListResponse
    {
        /// <summary>
        /// список пользователей
        /// </summary>
        public List<EmployeeListItem> Items { get; set; }
    }
}
