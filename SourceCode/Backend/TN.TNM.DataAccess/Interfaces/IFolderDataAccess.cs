using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Folder;
using TN.TNM.DataAccess.Messages.Results.Folder;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IFolderDataAccess
    {
        GetAllFolderDefaultResult GetAllFolderDefault(GetAllFolderDefaultNotActiveParameter parameter);
        GetAllFolderActiveResult GetAllFolderActive(GetAllFolderActiveParameter parameter);
        AddOrUpdateFolderResult AddOrUpdateFolderResult(AddOrUpdateFolderParameter parameter);
        CreateFolderResult CreateFolder(CreateFolderParameter parameter);
        DeleteFolderResult DeleteFolder(DeleteFolderParameter parameter);
        UploadFileResult UploadFile(UploadFileParameter parameter);
        DownloadFileResult DownloadFile(DownloadFileParameter parameter);
        UploadFileByFolderIdResult UploadFileByFolderId(UploadFileByFolderIdParameter parameter);
        DeleteFileResult DeleteFile(DeleteFileParameter parameter);
    }
}
