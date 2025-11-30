using StaffIo.Data.Enums;

namespace StaffIo.IService.Models.HistoryService
{
    public class HistoryGetListItem
    {
        public EnumTypeHistory Type { get; set; }

        public string? FotoUrlUserCreated { get; set; }

        public string FullNameUserCreated { get; set; }

        public string? Value { get; set; }
    }
}
