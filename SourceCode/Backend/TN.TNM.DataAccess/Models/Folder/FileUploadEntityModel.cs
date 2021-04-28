using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Folder
{
    public class FileUploadEntityModel
    {
        public FileInFolderEntityModel FileInFolder { get; set; }
        public IFormFile FileSave { get; set; }
    }
}
