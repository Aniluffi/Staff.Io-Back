using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Files.IService.Models.Request
{
    /// <summary>
    /// запрос на получение файла
    /// </summary>
    public class GetFileRequest
    {
        /// <summary>
        /// айди файла
        /// </summary>
        public string id { get; set; }
    }
}
