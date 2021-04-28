using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteForObjectRequest : BaseRequest<CreateNoteForObjectParameter>
    {
        public NoteModel Note { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadModel> ListFile { get; set; }

        public override CreateNoteForObjectParameter ToParameter()
        {
            var _listFile = new List<FileUploadEntityModel>();

            if (ListFile != null)
            {
                ListFile.ForEach(item =>
                {
                    var _fileUploadEntityModel = new FileUploadEntityModel();

                    _fileUploadEntityModel.FileInFolder = item.FileInFolder.ToEntityModel();
                    _fileUploadEntityModel.FileSave = item.FileSave;

                    _listFile.Add(_fileUploadEntityModel);
                });
            }

            return new CreateNoteForObjectParameter()
            {
                Note = Note.ToEntityModel(),
                FolderType = FolderType,
                ListFile = _listFile,
                UserId = UserId
            };
        }
    }
}
