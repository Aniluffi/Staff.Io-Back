namespace StaffIo.IService.Models.AdminService.Request
{
    public class AdminUpdateOwnerRequest
    {
        // Новое имя/название 
        public string Name { get; set; }

        // Ссылка на новое фото (или данные фото)
        public string? Foto { get; set; }
    }
}
