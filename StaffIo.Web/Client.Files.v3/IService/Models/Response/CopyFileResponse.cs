using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Files.IService.Models.Response
{
    /// <summary>
    /// ответ при создании копии файла
    /// </summary>
    public class CopyFileResponse
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string mimeType { get; set; }
    }
}
