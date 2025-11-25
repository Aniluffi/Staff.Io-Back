using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Files.IService.Models.Request
{
    /// <summary>
    /// запрос на создание файлов
    /// </summary>
    public class CreateFileRequest
    {
        public string? name { get; set; }
        public string mimeType { get; set; }
        public byte[] File { get; set; }
    }
}
