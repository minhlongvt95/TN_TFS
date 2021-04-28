using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Folder;
using TN.TNM.BusinessLogic.Messages.Requests.Folder;
using TN.TNM.BusinessLogic.Messages.Responses.Folder;

namespace TN.TNM.Api.Controllers
{
    public class FolderController : Controller
    {
        private readonly IFolder _iFolder;
        public FolderController(IFolder iFolder)
        {
            _iFolder = iFolder;
        }

        /// <summary>
        ///  Get All Folder Default Not Active
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/folder/getAllFolderDefault")]
        [Authorize(Policy = "Member")]
        public GetAllFolderDefaultNotActiveResponse GetAllFolderDefault([FromBody]GetAllFolderDefaultNotActiveRequest request)
        {
            return this._iFolder.GetAllFolderDefault(request);
        }

        /// <summary>
        ///  Get All Folder Default Not Active
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/folder/getAllFolderActive")]
        [Authorize(Policy = "Member")]
        public GetAllFolderActiveResponse GetAllFolderActive([FromBody]GetAllFolderActiveRequest request)
        {
            return this._iFolder.GetAllFolderActive(request);
        }

        /// <summary>
        ///  Get All Folder Default Not Active
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/folder/addOrUpdateFolder")]
        [Authorize(Policy = "Member")]
        public AddOrUpdateFolderResponse AddOrUpdateFolder([FromBody]AddOrUpdateFolderRequest request)
        {
            return this._iFolder.AddOrUpdateFolder(request);
        }

        /// <summary>
        ///  Get All Folder Default Not Active
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/folder/createFolder")]
        [Authorize(Policy = "Member")]
        public CreateFolderResponse CreateFolder([FromBody]CreateFolderRequest request)
        {
            return this._iFolder.CreateFolder(request);
        }

        [HttpPost]
        [Route("api/folder/deleteFolder")]
        [Authorize(Policy = "Member")]
        public DeleteFolderResponse DeleteFolder([FromBody]DeleteFolderRequest request)
        {
            return this._iFolder.DeleteFolder(request);
        }

        [HttpPost]
        [Route("api/folder/uploadFile")]
        [Authorize(Policy = "Member")]
        public  UploadFileResponse UploadFile([FromForm]UploadFileRequest request)
        {
            return this._iFolder.UploadFile(request);
        }

        [HttpPost]
        [Route("api/folder/downloadFile")]
        [Authorize(Policy = "Member")]
        public DownloadFileResponse DownloadFile([FromBody]DownloadFileRequest request)
        {
            return this._iFolder.DownloadFile(request);
        }

        [HttpPost]
        [Route("api/folder/uploadFileByFolderId")]
        [Authorize(Policy = "Member")]
        public UploadFileByFolderIdResponse UploadFileByFolderId([FromForm]UploadFileByFolderIdRequest request)
        {
            return this._iFolder.UploadFileByFolderId(request);
        }

        //
        [HttpPost]
        [Route("api/folder/deleteFile")]
        [Authorize(Policy = "Member")]
        public DeleteFileResponse DeleteFile([FromBody]DeleteFileRequest request)
        {
            return this._iFolder.DeleteFile(request);
        }
    }
}