using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Models.SaleBidding
{
    public class SaleBiddingDetailEntityModel
    {
        public Guid? SaleBiddingDetailId { get; set; }
        public Guid? SaleBiddingId { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<FileInFolderEntityModel> ListFile { get; set; }
    }
}
