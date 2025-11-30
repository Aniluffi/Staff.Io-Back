using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Files.IService.Models.Response
{
    public class CreateFileResponse
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string mimeType { get; set; }

        /// <summary>
        /// сыылка для скачивания
        /// </summary>
        public string webContentLink { get; set; }
        /// <summary>
        /// сылка для просмотра
        /// </summary>
        public string webViewLink { get; set; }
    }
}
