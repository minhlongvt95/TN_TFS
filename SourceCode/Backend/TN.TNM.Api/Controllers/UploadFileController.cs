using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Messages.Requests.File;
using TN.TNM.BusinessLogic.Messages.Responses.File;
using TN.TNM.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TN.TNM.BusinessLogic.Models.File;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TN.TNM.Api.Controllers
{
    public class UploadFileController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public UploadFileController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Get all company info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/file/uploadImage")]
        //[Authorize(Policy = "Member")]
        //public UploadImageResponse GetAllCompany([FromBody]UploadImageRequest request)
        //{
        //    string base64 = request.Base64Img.Substring(request.Base64Img.IndexOf(',') + 1);
        //    byte[] imageBytes = Convert.FromBase64String(base64);
        //    // Convert byte[] to Image
        //    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
        //    {
        //        Image image = Image.FromStream(ms, true);
        //        using (Bitmap bm2 = new Bitmap(ms))
        //        {
        //            bm2.Save(@"D:\\Projects\\N8\\Src\\TNT.N8\\TNT.N8.Web\\ClientApp\\src\\assets\\images\\" + request.ImageName + ".jpeg", ImageFormat.Jpeg);
        //        }
        //    }

        //    return new UploadImageResponse()
        //    {
        //        StatusCode = HttpStatusCode.OK
        //    };
        //}

        /// <summary>
        /// Upload file to server
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/file/uploadFile")]
        [Authorize(Policy = "Member")]
        public UploadFileResponse UploadFile(UploadFileRequest request)
        {
            if (request.FileList != null && request.FileList.Count > 0)
            {
                string folderName = "FileUpload";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                foreach (IFormFile item in request.FileList)
                {
                    if (item.Length > 0)
                    {
                        string fileName = item.FileName.Trim();
                        string fullPath = Path.Combine(newPath, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            item.CopyTo(stream);
                        }
                    }
                }

                return new UploadFileResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.FileUpload.UPLOAD_SUCCESS,
                };
            }
            
            return new UploadFileResponse()
            {
                StatusCode = HttpStatusCode.ExpectationFailed,
                MessageCode = CommonMessage.FileUpload.NO_FILE
            };
        }

        /// <summary>
        /// Upload file to server
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/file/uploadProductImage")]
        [Authorize(Policy = "Member")]
        public UploadFileResponse UploadProductImage(UploadFileRequest request)
        {
            if (request.FileList != null && request.FileList.Count > 0)
            {
                string folderName = "ProductImage";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
               
                foreach (IFormFile item in request.FileList)
                {
                    if (item.Length > 0)
                    {
                        //var plusName = DateTime.Now.ToBinary().ToString();
                        //int idx = item.FileName.LastIndexOf('.');
                        //string realName = item.FileName.Substring(0, idx);
                        //string fileType = item.FileName.Substring(idx + 1);

                        //string fileName = realName + "_" + plusName + "." + fileType;
                        string fileName = item.FileName.Trim();
                        string fullPath = Path.Combine(newPath, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            item.CopyTo(stream);
                        }
                    }
                }

                return new UploadFileResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.FileUpload.UPLOAD_SUCCESS,
                };
            }

            return new UploadFileResponse()
            {
                StatusCode = HttpStatusCode.ExpectationFailed,
                MessageCode = CommonMessage.FileUpload.NO_FILE
            };
        }

        [HttpPost]
        [Route("api/file/downloadFile")]
        [Authorize(Policy = "Member")]
        public DownloadFileResponse DownloadFile([FromBody]DownloadFileRequest request)
        {
            try
            {
                string extension = Path.GetExtension(request.FileUrl);
                string type = "";
                byte[] base64Array = System.IO.File.ReadAllBytes(request.FileUrl);

                switch (extension)
                {
                    case ".zip": type = "application/zip";break;
                    case ".rar": type = "application/x-rar-compressed"; break;
                    case ".ppt": type = "application/vnd.ms-powerpoint"; break;
                    case ".pptx": type = "application/vnd.openxmlformats-officedocument.presentationml.presentation"; break;
                    case ".pdf": type = "application/pdf"; break;
                    case ".txt": type = "text/plain"; break;
                    case ".jpeg":
                    case ".jpg": type = "image/jpeg"; break;
                    case ".tif":
                    case ".tiff": type = "image/tiff"; break;
                    case ".png": type = "image/png"; break;
                    case ".bmp": type = "image/bmp"; break;
                    case ".gif": type = "image/gif"; break;
                    case ".ico": type = "image/x-icon"; break;
                    case ".svg": type = "image/svg+xml"; break;
                    case ".webp": type = "image/webp"; break;
                    case ".doc": type = "application/msword"; break;
                    case ".docx": type = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                    case ".xls": type = "application/vnd.ms-excel"; break;
                    case ".xlsx": type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                    default: type = "";break;
                }

                return new DownloadFileResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    FileType = type,
                    FileAsBase64 = base64Array
                };
            }
            catch (Exception)
            {
                return new DownloadFileResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }         
        }

        [HttpPost]
        [Route("api/file/downloadProductImage")]
        [Authorize(Policy = "Member")]
        public DownloadProductImageResponse DownloadProductImage([FromBody]DownloadProductImageRequest request)
        {
            try
            {
                var ListProductImageResponseModel = new List<BusinessLogic.Models.File.ProductImageResponseModel>();

                if (request.ListFileUrl.Count > 0)
                {                  
                    request.ListFileUrl.ForEach(fileUrl =>
                    {
                        var productImage = new BusinessLogic.Models.File.ProductImageResponseModel();
                        string extension = Path.GetExtension(fileUrl);
                        Uri uri = new Uri(fileUrl);
                        if (uri.IsFile)
                        {
                            productImage.FileName = System.IO.Path.GetFileName(uri.LocalPath);
                        }
                        var type = "";
                        byte[] base64Array = System.IO.File.ReadAllBytes(fileUrl);

                        switch (extension)
                        {
                            case ".zip": type = "application/zip"; break;
                            case ".rar": type = "application/x-rar-compressed"; break;
                            case ".ppt": type = "application/vnd.ms-powerpoint"; break;
                            case ".pptx": type = "application/vnd.openxmlformats-officedocument.presentationml.presentation"; break;
                            case ".pdf": type = "application/pdf"; break;
                            case ".txt": type = "text/plain"; break;
                            case ".jpeg":
                            case ".jpg": type = "image/jpeg"; break;
                            case ".tif":
                            case ".tiff": type = "image/tiff"; break;
                            case ".png": type = "image/png"; break;
                            case ".bmp": type = "image/bmp"; break;
                            case ".gif": type = "image/gif"; break;
                            case ".ico": type = "image/x-icon"; break;
                            case ".svg": type = "image/svg+xml"; break;
                            case ".webp": type = "image/webp"; break;
                            case ".doc": type = "application/msword"; break;
                            case ".docx": type = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                            case ".xls": type = "application/vnd.ms-excel"; break;
                            case ".xlsx": type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                            default: type = ""; break;
                        }
                        productImage.FileAsBase64 = base64Array;
                        productImage.FileType = type;

                        ListProductImageResponseModel.Add(productImage);
                    });             
                }

                return new DownloadProductImageResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ListProductImageResponseModel = ListProductImageResponseModel,
                };
            }
            catch (Exception)
            {
                return new DownloadProductImageResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        //
        [HttpPost]
        [Route("api/file/uploadFileForOption")]
        [Authorize(Policy = "Member")]
        public UploadFileForOptionResponse UploadFileForOption(UploadFileForOptionRequest request)
        {
            try
            {
                if (request.FileList != null && request.FileList.Count > 0)
                {
                    string folderName = "FileUpload";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);

                    newPath = Path.Combine(newPath, request.Option);

                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    
                    var listFileNameExists = new List<FileNameExistsModel>();
                    foreach (IFormFile item in request.FileList)
                    {
                        if (item.Length > 0)
                        {
                            string fileName = item.FileName.Trim();
                            string fullPath = Path.Combine(newPath, fileName);

                            string newfullPath = GetUniqueFilePath(fullPath);

                            if (fullPath != newfullPath)
                            {
                                var _file = new FileNameExistsModel();
                                _file.OldFileName = fileName;
                                _file.NewFileName = newfullPath.Substring(newfullPath.LastIndexOf("\\") + 1);
                                listFileNameExists.Add(_file);
                            }

                            using (var stream = new FileStream(newfullPath, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }
                        }
                    }

                    return new UploadFileForOptionResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.FileUpload.UPLOAD_SUCCESS,
                        ListFileNameExists = listFileNameExists
                    };
                }

                return new UploadFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.FileUpload.NO_FILE
                };
            }
            catch (Exception e)
            {
                return new UploadFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //
        [HttpPost]
        [Route("api/file/deleteFileForOption")]
        [Authorize(Policy = "Member")]
        public DeleteFileForOptionResponse DeleteFileForOption(DeleteFileForOptionRequest request)
        {
            try
            {
                string folderName = "FileUpload";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName, request.Option, request.FileName);

                if (System.IO.File.Exists(newPath))
                {
                    System.IO.File.Delete(newPath);

                    return new DeleteFileForOptionResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success"
                    };
                }

                return new DeleteFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Không tồn tại file vật lý này trên hệ thống"
                };
            }
            catch (Exception e)
            {
                return new DeleteFileForOptionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private static string GetUniqueFilePath(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                string folderPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExtension = Path.GetExtension(filePath);
                int number = 1;

                Match regex = Regex.Match(fileName, @"^(.+) \((\d+)\)$");

                if (regex.Success)
                {
                    fileName = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    string newFileName = $"{fileName} ({number}){fileExtension}";
                    filePath = Path.Combine(folderPath, newFileName);
                }
                while (System.IO.File.Exists(filePath));
            }

            return filePath;
        }
    }
}
