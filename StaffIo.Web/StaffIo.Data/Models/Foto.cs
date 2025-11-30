using StaffIo.Data.Enums;

namespace StaffIo.Data.Models
{
    /// <summary>
    /// Фотография
    /// </summary>
    public class Foto
    {
        /// <summary>
        /// Идентификатор фотографии
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор фото в GoogleDisk
        /// </summary>
        public string FotoId { get; set; }

        /// <summary>
        /// Тип фотографии
        /// </summary>
        public EnumTypeFoto TypeFoto { get; set; }

        /// <summary>
        /// Дата создания фотографии
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string FotoUrl { get; set; }

        public Guid UserId { get; set; }
    }
}
