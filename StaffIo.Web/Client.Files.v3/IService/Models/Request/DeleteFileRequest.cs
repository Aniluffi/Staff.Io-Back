using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Files.IService.Models.Request
{
    /// <summary>
    /// запрос на удаление файла
    /// </summary>
    public class DeleteFileRequest
    {
        /// <summary>
        /// айди файла
        /// </summary>
        public string id { get; set; }
    }
}
