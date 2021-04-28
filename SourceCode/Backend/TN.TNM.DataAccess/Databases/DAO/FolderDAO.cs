using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Folder;
using TN.TNM.DataAccess.Messages.Results.Folder;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class FolderDAO : BaseDAO, IFolderDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public FolderDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }

        public AddOrUpdateFolderResult AddOrUpdateFolderResult(AddOrUpdateFolderParameter parameter)
        {
            try
            {
                var listResult = new List<FolderEntityModel>();
                var listFolderCommon = context.Folder.ToList();
                var listFolderActive = listFolderCommon.Where(x => x.Active == true).ToList();

                if (parameter.ListFolder != null && parameter.ListFolder.Count > 0)
                {
                    var listFolderId = parameter.ListFolder.Select(x => x.FolderId).ToList();

                    var listFolderUpdateNotActive = listFolderActive.Where(x => !listFolderId.Contains(x.FolderId)).ToList();
                    listFolderUpdateNotActive.ForEach(item =>
                    {
                        item.Active = false;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;
                        context.Folder.Update(item);
                    });

                    var listFolderUpdateActive = new List<Folder>();

                    parameter.ListFolder.ForEach(item =>
                    {
                        var forder = listFolderCommon.FirstOrDefault(x => item.FolderId == x.FolderId);
                        listFolderUpdateActive.Add(forder);
                        GetFolderParent(listFolderCommon, forder, listFolderUpdateActive);
                    });
                    listFolderUpdateActive = listFolderUpdateActive.Distinct().ToList();
                    listFolderUpdateActive = listFolderUpdateActive.OrderBy(x => x.FolderLevel).ToList();
                    listFolderUpdateActive.ForEach(item =>
                    {
                        item.Active = true;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;
                        context.Folder.Update(item);

                        string folderName = ConvertFolderUrl(item.Url);
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        webRootPath = _hostingEnvironment.WebRootPath + "\\";
                        string newPath = Path.Combine(webRootPath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                    });

                    listResult = listFolderUpdateActive.Select(x => new FolderEntityModel()
                    {
                        Active = x.Active,
                        FolderId = x.FolderId,
                        FolderLevel = x.FolderLevel,
                        FolderType = x.FolderType,
                        IsDelete = x.IsDelete,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        Url = x.Url
                    }).ToList();

                    var listResultId = listFolderUpdateActive.Select(x => x.FolderId).ToList();
                    var listFile = context.FileInFolder.Where(x => listFolderId.Contains(x.FolderId)).ToList();

                    listResult.ForEach(item =>
                    {
                        var listFileInFolder = listFile.Where(x => x.FolderId == item.FolderId).Select(y => new FileInFolderEntityModel()
                        {
                            Active = y.Active,
                            FileInFolderId = y.FileInFolderId,
                            FileName = y.FileName,
                            FolderId = y.FolderId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Size = y.Size
                        }).ToList();
                        item.HasChild = listResult.FirstOrDefault(z => z.ParentId == item.FolderId) != null;
                        item.ListFile = listFileInFolder;
                    });

                }
                else
                {
                    listFolderCommon.ForEach(item =>
                    {
                        item.Active = false;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;
                        context.Folder.Update(item);
                    });
                }

                context.SaveChanges();

                return new AddOrUpdateFolderResult()
                {
                    Status = true,
                    ListFolder = listResult
                };

            }
            catch (Exception ex)
            {
                return new AddOrUpdateFolderResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetAllFolderActiveResult GetAllFolderActive(GetAllFolderActiveParameter parameter)
        {
            try
            {
                #region Lấy danh sách tất cả các folder và các file trong folder mặc định chưa active

                var listFolderResult = context.Folder.Where(x => x.Active == true).Select(y => new FolderEntityModel()
                {
                    FolderId = y.FolderId,
                    IsDelete = y.IsDelete,
                    Active = y.Active,
                    FolderType = y.FolderType,
                    ListFile = new List<FileInFolderEntityModel>(),
                    Name = y.Name,
                    ParentId = y.ParentId,
                    Url = y.Url
                }).ToList();

                var listFolderId = listFolderResult.Select(x => x.FolderId).ToList();
                var listFile = context.FileInFolder.Where(x => listFolderId.Contains(x.FolderId)).ToList();

                listFolderResult.ForEach(item =>
                {
                    var listFileInFolder = listFile.Where(x => x.FolderId == item.FolderId).Select(y => new FileInFolderEntityModel()
                    {
                        Active = y.Active,
                        FileInFolderId = y.FileInFolderId,
                        FileName = y.FileName,
                        FolderId = y.FolderId,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        Size = y.Size,
                        FileExtension = y.FileExtension
                    }).ToList();
                    item.HasChild = listFolderResult.FirstOrDefault(z => z.ParentId == item.FolderId) != null;
                    item.ListFile = listFileInFolder;
                });

                #endregion

                return new GetAllFolderActiveResult()
                {
                    Status = true,
                    ListFolderActive = listFolderResult
                };
            }
            catch (Exception ex)
            {
                return new GetAllFolderActiveResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public CreateFolderResult CreateFolder(CreateFolderParameter parameter)
        {
            try
            {
                var folder = new Folder()
                {
                    FolderId = Guid.NewGuid(),
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    FolderLevel = parameter.FolderParent.FolderLevel + 1,
                    IsDelete = true,
                    Name = parameter.FolderName,
                    ParentId = parameter.FolderParent.FolderId,
                    Url = parameter.FolderParent.Url + @"\" + parameter.FolderName
                };

                string folderName = ConvertFolderUrl(folder.Url);
                string webRootPath = _hostingEnvironment.WebRootPath;
                webRootPath = _hostingEnvironment.WebRootPath + "\\";
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    context.Folder.Add(folder);
                    context.SaveChanges();
                    Directory.CreateDirectory(newPath);
                }
                else
                {
                    int index = folder.Name.LastIndexOf("(");
                    if (index == -1)
                    {
                        string url = folder.Url;
                        folder.Name = folder.Name + "(1)";
                        folder.Url = url + "(1)";
                        folderName = ConvertFolderUrl(folder.Url);
                        webRootPath = _hostingEnvironment.WebRootPath;
                        webRootPath = _hostingEnvironment.WebRootPath + "\\";
                        newPath = Path.Combine(webRootPath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            context.Folder.Add(folder);
                            context.SaveChanges();
                            Directory.CreateDirectory(newPath);
                        }
                        else
                        {
                            var indexNew = folder.Name.LastIndexOf("(");
                            var nameFolder = folder.Name.Substring(0, indexNew + 1);
                            var listFolderChildren = context.Folder.Where(x => x.Name.Contains(nameFolder)).OrderByDescending(y => y.Name).ToList();
                            var indexLast = listFolderChildren[0].Name.Length - 1;
                            var number = listFolderChildren[0].Name.Substring(indexNew + 1, indexLast - indexNew - 1);
                            int numberFolder = Convert.ToInt32(number) + 1;
                            folder.Name = parameter.FolderName + "(" + numberFolder + ")";
                            folder.Url = url + "(" + numberFolder + ")";
                            folderName = ConvertFolderUrl(folder.Url);
                            webRootPath = _hostingEnvironment.WebRootPath;
                            webRootPath = _hostingEnvironment.WebRootPath + "\\";
                            newPath = Path.Combine(webRootPath, folderName);
                            context.Folder.Add(folder);
                            context.SaveChanges();
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }
                            else
                            {
                                return new CreateFolderResult()
                                {
                                    Status = false,
                                    Message = "Đường dẫn tạo file lỗi!"
                                };
                            }
                        }
                    }
                }

                var folderResult = new FolderEntityModel()
                {
                    Active = folder.Active,
                    FolderId = folder.FolderId,
                    FolderLevel = folder.FolderLevel,
                    IsDelete = folder.IsDelete,
                    Name = folder.Name,
                    ParentId = folder.ParentId,
                    Url = folder.Url,
                    FolderType = folder.FolderType
                };

                return new CreateFolderResult()
                {
                    Status = true,
                    Folder = folderResult
                };
            }
            catch (Exception ex)
            {
                return new CreateFolderResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetAllFolderDefaultResult GetAllFolderDefault(GetAllFolderDefaultNotActiveParameter parameter)
        {
            try
            {
                #region Lấy danh sách tất cả các folder và các file trong folder mặc định chưa active

                var listFolderResult = context.Folder.Where(x => x.IsDelete == false).Select(y => new FolderEntityModel()
                {
                    FolderId = y.FolderId,
                    IsDelete = y.IsDelete,
                    Active = y.Active,
                    FolderType = y.FolderType,
                    ListFile = new List<FileInFolderEntityModel>(),
                    Name = y.Name,
                    ParentId = y.ParentId,
                    Url = y.Url
                }).ToList();

                var listFolderId = listFolderResult.Select(x => x.FolderId).ToList();
                var listFile = context.FileInFolder.Where(x => listFolderId.Contains(x.FolderId) && x.Active == true).ToList();

                listFolderResult.ForEach(item =>
                {
                    var listFileInFolder = listFile.Where(x => x.FolderId == item.FolderId).Select(y => new FileInFolderEntityModel()
                    {
                        Active = y.Active,
                        FileInFolderId = y.FileInFolderId,
                        FileName = y.FileName,
                        FolderId = y.FolderId,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        Size = y.Size,
                        FileExtension = y.FileExtension
                    }).ToList();
                    item.HasChild = listFolderResult.FirstOrDefault(z => z.ParentId == item.FolderId) != null;
                    item.ListFile = listFileInFolder;
                });

                #endregion

                return new GetAllFolderDefaultResult()
                {
                    Status = true,
                    ListFolderDefault = listFolderResult
                };
            }
            catch (Exception ex)
            {
                return new GetAllFolderDefaultResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public DeleteFolderResult DeleteFolder(DeleteFolderParameter parameter)
        {
            try
            {
                var listFolderCommon = context.Folder.ToList();
                var folderRemove = listFolderCommon.FirstOrDefault(x => x.FolderId == parameter.Folder.FolderId);
                if (folderRemove == null)
                {
                    return new DeleteFolderResult()
                    {
                        Status = false,
                        Message = "Không tìm thấy folder để xóa"
                    };
                }

                if (folderRemove.IsDelete == false)
                {
                    return new DeleteFolderResult()
                    {
                        Status = false,
                        Message = "Không thể xóa folder mặc định này!"
                    };
                }
                context.Folder.Remove(folderRemove);

                var listFolderChildren = GetFolderChildren(listFolderCommon, folderRemove);
                if (listFolderChildren.Count > 0)
                {
                    context.Folder.RemoveRange(listFolderChildren);
                }

                var listFolderId = listFolderChildren.Select(x => x.FolderId).ToList();
                listFolderId.Add(folderRemove.FolderId);
                var listFile = context.FileInFolder.Where(x => listFolderId.Contains(x.FolderId)).ToList();
                if (listFile.Count > 0)
                {
                    context.FileInFolder.RemoveRange(listFile);
                }

                string folderName = ConvertFolderUrl(parameter.Folder.Url);
                string webRootPath = _hostingEnvironment.WebRootPath;
                webRootPath = _hostingEnvironment.WebRootPath + "\\";
                string newPath = Path.Combine(webRootPath, folderName);
                if (Directory.Exists(newPath))
                {
                    Directory.Delete(newPath, true);
                }
                else
                {
                    return new DeleteFolderResult()
                    {
                        Status = false,
                        Message = "Folder không tồn tại!"
                    };
                }
                context.SaveChanges();

                return new DeleteFolderResult()
                {
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new DeleteFolderResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public UploadFileResult UploadFile(UploadFileParameter parameter)
        {
            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);

            if (folder == null)
            {
                return new UploadFileResult()
                {
                    Status = false,
                    Message = "Thư mục upload không tồn tại"
                };
            }

            var listFileDelete = new List<string>();
            try
            {
                var listFileResult = new List<FileInFolderEntityModel>();
                if (parameter.ListFile != null && parameter.ListFile.Count > 0)
                {
                    bool isSave = true;
                    parameter.ListFile.ForEach(item =>
                    {
                        if (folder == null)
                        {
                            isSave = false;
                        }
                        string folderName = ConvertFolderUrl(folder.Url);
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        if (!Directory.Exists(newPath))
                        {
                            isSave = false;
                        }

                        if (isSave)
                        {
                            var file = new FileInFolder()
                            {
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                FileInFolderId = Guid.NewGuid(),
                                FileName = item.FileInFolder.FileName + "_" + Guid.NewGuid().ToString(),
                                FolderId = folder.FolderId,
                                ObjectId = item.FileInFolder.ObjectId,
                                ObjectType = item.FileInFolder.ObjectType,
                                Size = item.FileInFolder.Size,
                                FileExtension =
                                    item.FileSave.FileName.Substring(item.FileSave.FileName.LastIndexOf(".") + 1)
                            };
                            context.Add(file);

                            string fileName = file.FileName + "."+ file.FileExtension;

                            if (isSave)
                            {
                                string fullPath = Path.Combine(newPath, fileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    item.FileSave.CopyTo(stream);
                                    listFileDelete.Add(fullPath);
                                }
                            }
                        }
                    });
                    if (!isSave)
                    {
                        listFileDelete.ForEach(item =>
                        {
                            File.Delete(item);
                        });

                        return new UploadFileResult()
                        {
                            Status = false,
                            Message = "Bạn phải cấu hình thư mục để lưu"
                        };
                    }
                }

                context.SaveChanges();

                #region Lấy list file theo ObjectId

                listFileResult = context.FileInFolder
                    .Where(x => x.ObjectId == parameter.ObjectId && x.FolderId == folder.FolderId).Select(y =>
                        new FileInFolderEntityModel
                        {
                            FileInFolderId = y.FileInFolderId,
                            FileName = y.FileName,
                            FileExtension = y.FileExtension,
                            Size = y.Size,
                            CreatedDate = y.CreatedDate
                        }).OrderBy(z => z.CreatedDate).ToList();

                #endregion

                return new UploadFileResult()
                {
                    Status = true,
                    ListFileInFolder = listFileResult
                };
            }
            catch (Exception ex)
            {
                listFileDelete.ForEach(item =>
                {
                    Directory.Delete(item);
                });

                return new UploadFileResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public DownloadFileResult DownloadFile(DownloadFileParameter parameter)
        {
            try
            {
                var file = context.FileInFolder.FirstOrDefault(x => x.FileInFolderId == parameter.FileInFolderId);
                if (file == null)
                {
                    return new DownloadFileResult()
                    {
                        Status = false,
                        Message = "Không tìm thấy file"
                    };
                }
                var folder = context.Folder.FirstOrDefault(x => x.FolderId == file.FolderId);
                if (folder == null)
                {
                    return new DownloadFileResult()
                    {
                        Status = false,
                        Message = "Không tìm thấy file"
                    };
                }
                string webRootPath = _hostingEnvironment.WebRootPath;
                string extension = "."+file.FileExtension;
                string url = webRootPath+"\\" +folder.Url + "\\" + file.FileName + "."+file.FileExtension;

                string type = "";
                byte[] base64Array = System.IO.File.ReadAllBytes(url);

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
                    default:
                        type = ""; break;
                }

                return new DownloadFileResult()
                {
                    Status = true,
                    FileType = type,
                    FileAsBase64 = base64Array
                };
            }
            catch (Exception ex)
            {
                return new DownloadFileResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public UploadFileByFolderIdResult UploadFileByFolderId(UploadFileByFolderIdParameter parameter)
        {
            var listFileDelete = new List<string>();
            try
            {
                var listFileResult = new List<FileInFolderEntityModel>();
                if (parameter.ListFile != null && parameter.ListFile.Count > 0)
                {
                    var listFolderCommon = context.Folder.ToList();
                    bool isSave = true;
                    parameter.ListFile.ForEach(item =>
                    {
                        var folder = listFolderCommon.FirstOrDefault(x => x.FolderId == parameter.FolderId);
                        if (folder == null)
                        {
                            isSave = false;
                        }
                        string folderName = ConvertFolderUrl(folder.Url);
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        if (!Directory.Exists(newPath))
                        {
                            isSave = false;
                        }

                        if (isSave)
                        {
                            var file = new FileInFolder()
                            {
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                FileInFolderId = Guid.NewGuid(),
                                FileName = item.FileInFolder.FileName + "_" + Guid.NewGuid().ToString(),
                                FolderId = folder.FolderId,
                                ObjectId = item.FileInFolder.ObjectId,
                                ObjectType = item.FileInFolder.ObjectType,
                                Size = item.FileInFolder.Size,
                                FileExtension = item.FileSave.FileName.Substring(item.FileSave.FileName.LastIndexOf(".") + 1)
                            };
                            context.Add(file);
                            listFileResult.Add(new FileInFolderEntityModel()
                            {
                                Active = true,
                                FileInFolderId = file.FileInFolderId,
                                FileName = file.FileName,
                                FolderId = file.FolderId,
                                ObjectId = file.ObjectId,
                                ObjectType = file.ObjectType,
                                Size = file.Size,
                                FileExtension = file.FileExtension
                            });

                            string fileName = file.FileName + "." + file.FileExtension;

                            if (isSave)
                            {
                                string fullPath = Path.Combine(newPath, fileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    item.FileSave.CopyTo(stream);
                                    listFileDelete.Add(fullPath);
                                }
                            }
                        }
                    });
                    if (!isSave)
                    {
                        listFileDelete.ForEach(item =>
                        {
                            File.Delete(item);
                        });

                        return new UploadFileByFolderIdResult()
                        {
                            Status = false,
                            Message = "Bạn phải cấu hình thư mục để lưu"
                        };
                    }
                }

                context.SaveChanges();
                return new UploadFileByFolderIdResult()
                {
                    Status = true,
                    ListFileInFolder = listFileResult
                };
            }
            catch (Exception ex)
            {
                listFileDelete.ForEach(item =>
                {
                    Directory.Delete(item);
                });

                return new UploadFileByFolderIdResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public DeleteFileResult DeleteFile(DeleteFileParameter parameter)
        {
            try
            {
                var file = context.FileInFolder.FirstOrDefault(x => x.FileInFolderId == parameter.FileInFolderId);

                if (file == null)
                {
                    return new DeleteFileResult()
                    {
                        Status = false,
                        Message = "File không tồn tại trên hệ thống"
                    };
                }

                context.FileInFolder.Remove(file);

                #region Xóa file vật lý

                var folder = context.Folder.FirstOrDefault(x => x.FolderId == file.FolderId);
                string folderName = ConvertFolderUrl(folder.Url);
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string fileName = file.FileName + "." + file.FileExtension;
                string fullPath = Path.Combine(newPath, fileName);
                File.Delete(fullPath);

                #endregion

                context.SaveChanges();

                return new DeleteFileResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DeleteFileResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public List<Folder> GetFolderParent(List<Folder> listFolderCommon, Folder folder, List<Folder> listFolderResult)
        {
            var parent = listFolderCommon.FirstOrDefault(x => x.FolderId == folder.ParentId);
            if (parent != null)
            {
                listFolderResult.Add(parent);
                GetFolderParent(listFolderCommon, parent, listFolderResult);
            }
            return listFolderResult;
        }

        public List<Folder> GetFolderChildren(List<Folder> listFolderCommon, Folder folder)
        {
            var listFolderResult = new List<Folder>();
            var parent = listFolderCommon.Where(x => x.ParentId == folder.FolderId).ToList();
            if (parent.Count > 0)
            {
                listFolderResult.AddRange(parent);
                parent.ForEach(item =>
                {
                    GetFolderChildren(listFolderCommon, item);
                });
            }
            return listFolderResult;
        }

        public string ConvertFolderUrl(string url)
        {
            var stringResult = url.Split(@"\");
            string result = "";
            for (int i = 0; i < stringResult.Length; i++)
            {
                result = result + stringResult[i] + "\\";
            }

            result = result.Substring(0, result.Length - 1);

            return result;
        }

       
    }
}
