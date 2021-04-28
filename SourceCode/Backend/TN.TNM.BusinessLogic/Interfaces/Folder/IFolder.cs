using TN.TNM.BusinessLogic.Messages.Requests.Folder;
using TN.TNM.BusinessLogic.Messages.Responses.Folder;

namespace TN.TNM.BusinessLogic.Interfaces.Folder
{
    public interface IFolder
    {
        GetAllFolderDefaultNotActiveResponse GetAllFolderDefault(GetAllFolderDefaultNotActiveRequest request);
        GetAllFolderActiveResponse GetAllFolderActive(GetAllFolderActiveRequest request);
        AddOrUpdateFolderResponse AddOrUpdateFolder(AddOrUpdateFolderRequest request);
        CreateFolderResponse CreateFolder(CreateFolderRequest request);
        DeleteFolderResponse DeleteFolder(DeleteFolderRequest request);
        UploadFileResponse UploadFile(UploadFileRequest request);
        DownloadFileResponse DownloadFile(DownloadFileRequest request);
        UploadFileByFolderIdResponse UploadFileByFolderId(UploadFileByFolderIdRequest request);
        DeleteFileResponse DeleteFile(DeleteFileRequest request);
    }
}
