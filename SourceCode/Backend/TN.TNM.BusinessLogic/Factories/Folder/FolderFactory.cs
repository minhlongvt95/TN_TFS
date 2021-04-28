using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.Folder;
using TN.TNM.BusinessLogic.Messages.Requests.Folder;
using TN.TNM.BusinessLogic.Messages.Responses.Folder;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Folder
{
    public class FolderFactory:BaseFactory, IFolder
    {
        private IFolderDataAccess _iFolderDataAccess;

        public FolderFactory(IFolderDataAccess iFolderDataAccess, ILogger<FolderFactory> _logger)
        {
            _iFolderDataAccess = iFolderDataAccess;
            this.logger = _logger;
        }

        public AddOrUpdateFolderResponse AddOrUpdateFolder(AddOrUpdateFolderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.AddOrUpdateFolderResult(parameter);

                var response = new AddOrUpdateFolderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListFolder = new List<FolderModel>()
                };

                result.ListFolder.ForEach(item =>
                {
                    response.ListFolder.Add(new FolderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new AddOrUpdateFolderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateFolderResponse CreateFolder(CreateFolderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.CreateFolder(parameter);

                var response = new CreateFolderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Folder = new FolderModel(result.Folder)
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateFolderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteFolderResponse DeleteFolder(DeleteFolderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.DeleteFolder(parameter);

                var response = new DeleteFolderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteFolderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DownloadFileResponse DownloadFile(DownloadFileRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.DownloadFile(parameter);

                var response = new DownloadFileResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    FileAsBase64 = result.FileAsBase64,
                    FileType = result.FileType
                };

                return response;
            }
            catch (Exception e)
            {
                return new DownloadFileResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllFolderActiveResponse GetAllFolderActive(GetAllFolderActiveRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.GetAllFolderActive(parameter);

                var response = new GetAllFolderActiveResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListFolderActive = new List<FolderModel>()
                };

                result.ListFolderActive.ForEach(item =>
                {
                    var folderActive = new FolderModel(item);
                    folderActive.ListFile = new List<FileInFolderModel>();
                    item.ListFile.ForEach(file =>
                    {
                        folderActive.ListFile.Add(new FileInFolderModel(file));
                    });
                    response.ListFolderActive.Add(folderActive);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetAllFolderActiveResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllFolderDefaultNotActiveResponse GetAllFolderDefault(GetAllFolderDefaultNotActiveRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.GetAllFolderDefault(parameter);

                var response = new GetAllFolderDefaultNotActiveResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListFolderDefault = new List<FolderModel>()
                };

                result.ListFolderDefault.ForEach(item =>
                {
                    var folder = new FolderModel(item);
                    folder.ListFile = new List<FileInFolderModel>();
                    item.ListFile.ForEach(x =>
                    {
                        folder.ListFile.Add(new FileInFolderModel(x));
                    });

                    response.ListFolderDefault.Add(folder);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetAllFolderDefaultNotActiveResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UploadFileResponse UploadFile(UploadFileRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.UploadFile(parameter);

                var response = new UploadFileResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListFileInFolder = new List<FileInFolderModel>()
                };

                result.ListFileInFolder.ForEach(item =>
                {
                    response.ListFileInFolder.Add(new FileInFolderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new UploadFileResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UploadFileByFolderIdResponse UploadFileByFolderId(UploadFileByFolderIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.UploadFileByFolderId(parameter);

                var response = new UploadFileByFolderIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListFileInFolder = new List<FileInFolderModel>()
                };

                result.ListFileInFolder.ForEach(item =>
                {
                    response.ListFileInFolder.Add(new FileInFolderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new UploadFileByFolderIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteFileResponse DeleteFile(DeleteFileRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iFolderDataAccess.DeleteFile(parameter);

                var response = new DeleteFileResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteFileResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
