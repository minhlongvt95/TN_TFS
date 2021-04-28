using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Models.Folder
{
    public class FileInFolderModel : BaseModel<DataAccess.Databases.Entities.FileInFolder>
    {
        public Guid FileInFolderId { get; set; }
        public Guid FolderId { get; set; }
        public string FileName { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Size { get; set; }
        public bool Active { get; set; }
        public string FileExtension { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public FileInFolderModel() { }

        public FileInFolderModel(FileInFolderModel entity)
        {
            Mapper(entity, this);
        }

        public FileInFolderModel(FileInFolderEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.FileInFolder ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.FileInFolder();
            Mapper(this, entity);
            return entity;
        }

        public FileInFolderEntityModel ToEntityModel()
        {
            var entity = new FileInFolderEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
