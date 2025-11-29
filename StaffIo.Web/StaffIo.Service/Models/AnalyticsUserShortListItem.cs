using StaffIo.Data.Enums;

namespace StaffIo.Service.Models
{
    public class AnalyticsUserShortListItem
    {
        public Guid UserId { get; set; }
        public EnumUserStatus Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateDeleted { get; set; }
    }
}
