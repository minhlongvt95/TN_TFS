using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Models.Folder
{
    public class FolderModel:BaseModel<DataAccess.Databases.Entities.Folder>
    {
        public Guid FolderId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsDelete { get; set; }
        public bool Active { get; set; }
        public string FolderType { get; set; }
        public bool HasChild { get; set; }
        public int FolderLevel { get; set; }
        public List<FileInFolderModel> ListFile { get; set; }

        public FolderModel() { }

        public FolderModel(FolderModel entity)
        {
            Mapper(entity, this);
        }

        public FolderModel(FolderEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.Folder ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Folder();
            Mapper(this, entity);
            return entity;
        }

        public FolderEntityModel ToEntityModel()
        {
            var entity = new FolderEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
