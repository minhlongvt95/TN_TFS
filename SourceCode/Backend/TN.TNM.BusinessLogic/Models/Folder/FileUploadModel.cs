using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Models.Folder
{
    public class FileUploadModel
    {
        public FileInFolderModel FileInFolder { get; set; }
        public IFormFile FileSave { get; set; }
    }
}
