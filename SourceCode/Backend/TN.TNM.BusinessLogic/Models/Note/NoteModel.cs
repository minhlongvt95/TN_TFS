using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Models.Note
{
    public class NoteModel : BaseModel<DataAccess.Databases.Entities.Note>
    {
        public Guid NoteId { get; set; }
        public string NoteTitle { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ResponsibleName { get; set; }
        public string ResponsibleAvatar { get; set; }
        public List<NoteDocumentModel> NoteDocList { get; set; }
        public List<FileInFolderModel> ListFile { get; set; }

        public NoteModel() { }

        public NoteModel(NoteEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
            if (model.NoteDocList != null)
            {
                var cList = new List<NoteDocumentModel>();
                model.NoteDocList.ForEach(child =>
                {
                    cList.Add(new NoteDocumentModel(child));
                });
                this.NoteDocList = cList;
            }

            if (model.ListFile != null)
            {
                var cList = new List<FileInFolderModel>();
                model.ListFile.ForEach(child =>
                {
                    cList.Add(new FileInFolderModel(child));
                });
                this.ListFile = cList;
            }
        }

        public NoteModel(DataAccess.Databases.Entities.Note entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.Note ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Note();
            Mapper(this, entity);
            return entity;
        }

        public NoteEntityModel ToEntityModel()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new NoteEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
