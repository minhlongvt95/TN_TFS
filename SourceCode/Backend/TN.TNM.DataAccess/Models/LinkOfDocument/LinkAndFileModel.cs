using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.LinkOfDocument
{
    public class LinkAndFileModel
    {
        public Guid? FileInFolderId { get; set; }
        public Guid? FolderId { get; set; }
        public string FileName { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Size { get; set; }
        public string FileExtension { get; set; }

        public Guid? LinkOfDocumentId { get; set; }
        public string LinkName { get; set; }
        public string LinkValue { get; set; }

        public string Name { get; set; }            //Tên tài liệu hoặc liên kết
        public string CreatedName { get; set; }     //Tên người đính kèm
        public DateTime? CreatedDate { get; set; }   //Ngày đính kèm
        public Guid? CreatedById { get; set; }      //Id người tạo
        public bool Type { get; set; }              //true: file, false: liên kết
    }
}
