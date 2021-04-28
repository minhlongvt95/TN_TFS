using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Results.Contract;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Quote;
using System.Linq;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Product;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ContractDAO : BaseDAO, IContractDataAccess
    {
        public IConfiguration Configuration { get; }
        public static string WEB_ENDPOINT;
        public static string PrimaryDomain;
        public static int PrimaryPort;
        public static string SecondayDomain;
        public static int SecondaryPort;
        public static string Email;
        public static string Password;
        public static string BannerUrl;
        public static string Ssl;
        public static string Company;
        public void GetConfiguration()
        {
            PrimaryDomain = Configuration["PrimaryDomain"];
            PrimaryPort = int.Parse(Configuration["PrimaryPort"]);
            SecondayDomain = Configuration["SecondayDomain"];
            SecondaryPort = int.Parse(Configuration["SecondaryPort"]);
            Email = Configuration["Email"];
            Password = Configuration["Password"];
            Ssl = Configuration["Ssl"];
            Company = Configuration["Company"];
            BannerUrl = Configuration["BannerUrl"];
            WEB_ENDPOINT = Configuration["WEB_ENDPOINT"];
        }

        private readonly IHostingEnvironment _hostingEnvironment;
        public ContractDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
            Configuration = iconfiguration;
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

        public CreateOrUpdateContractResult CreateOrUpdateContract(CreateOrUpdateContractParameter parameter)
        {
            bool isValidParameterNumber = true;
            if (parameter.Contract?.DiscountValue < 0)
            {
                isValidParameterNumber = false;
            }
            parameter.ContractDetail.ForEach(item =>
            {
                if (item?.DiscountValue < 0 || item?.ExchangeRate < 0 || item?.Quantity <= 0 || item?.UnitPrice < 0 || item?.Tax < 0)
                {
                    isValidParameterNumber = false;
                }
            });
            if (!isValidParameterNumber)
            {
                return new CreateOrUpdateContractResult
                {
                    Message = CommonMessage.Contract.CREATE_FAIL,
                    Status = false
                };
            }

            //Kiểm tra chiết khấu của đơn hàng
            if (parameter.Contract.DiscountValue == null)
            {
                parameter.Contract.DiscountValue = 0;
            }

            //Kiểm tra chiết khấu của sản phẩm
            if (parameter.ContractDetail.Count > 0)
            {
                var listProduct = parameter.ContractDetail.ToList();
                listProduct.ForEach(item =>
                {
                    if (item.DiscountValue == null)
                    {
                        item.DiscountValue = 0;
                    }
                });
            }

            var folder = context.Folder.FirstOrDefault(x => x.Active == true && x.FolderType == "QLHD");
            if (folder == null)
            {
                return new CreateOrUpdateContractResult()
                {
                    Status = false,
                    Message = "Chưa có thư mục để lưu. Bạn phải cấu hình thư mục để lưu"
                };
            }

            try
            {
                var listContractProductAttribute = new List<ContractDetailProductAttribute>();
                var listFileNotDeleteId = new List<Guid>();

                var contractCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THD");
                var listContracttatus = context.Category
                    .Where(x => x.CategoryTypeId == contractCategoryType.CategoryTypeId).ToList();
                var newStatusContract = listContracttatus.FirstOrDefault(c => c.CategoryCode == "MOI")?.CategoryId;
                if (parameter.Contract.ContractId != null && parameter.Contract.ContractId != Guid.Empty)
                {
                    var listContract =
                        context.Contract.Where(x => x.ContractCode == parameter.Contract.ContractCode &&
                                                    x.ContractId != parameter.Contract.ContractId).ToList();
                    if (listContract.Count > 0)
                    {
                        return new CreateOrUpdateContractResult()
                        {
                            Status = false,
                            Message = CommonMessage.Contract.DUPLICATE_CODE
                        };
                    }

                    var oldContract = context.Contract.FirstOrDefault(co => co.ContractId == parameter.Contract.ContractId);
                    var listFileUpdate = context.FileInFolder.Where(x =>
                            x.ObjectId != null && x.ObjectId.Value == oldContract.ContractId && x.ObjectType == "HD")
                        .ToList();

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        #region Delete all item Relation

                        var List_Delete_ContractProductDetailProductAttributeValue =
                            new List<ContractDetailProductAttribute>();
                        var List_Delete_ContractDetail = new List<ContractDetail>();
                        List_Delete_ContractDetail = context.ContractDetail
                            .Where(item => item.ContractId == parameter.Contract.ContractId).ToList();
                        List_Delete_ContractDetail.ForEach(item =>
                        {
                            if (item.ContractDetailId != Guid.Empty)
                            {
                                var ContractProductDetailProductAttributeValueList = context
                                    .ContractDetailProductAttribute
                                    .Where(OPDPAV => OPDPAV.ContractDetailId == item.ContractDetailId).ToList();
                                List_Delete_ContractProductDetailProductAttributeValue.AddRange(
                                    ContractProductDetailProductAttributeValueList);
                            }
                        });

                        var listDeleteContractCost = context.ContractCostDetail
                            .Where(item => item.ContractId == parameter.Contract.ContractId).ToList();
                        //if (parameter.ListFile != null && parameter.ListFile.Count > 0)
                        //{
                        //    parameter.ListFile.ForEach(file =>
                        //    {
                        //        var fileUpdate =
                        //            listFileUpdate.FirstOrDefault(x => file.FileInFolderId == x.FileInFolderId);
                        //        if (fileUpdate != null)
                        //        {
                        //            fileUpdate.ObjectId = oldContract.ContractId;
                        //            fileUpdate.UpdatedById = parameter.UserId;
                        //            fileUpdate.UpdatedDate = DateTime.Now;
                        //            context.Update(fileUpdate);
                        //            listFileNotDeleteId.Add(fileUpdate.FileInFolderId);
                        //        }
                        //    });
                        //}
                        // Xóa file vật lí 
                        //var listFileDelete = listFileUpdate.Where(x => !listFileNotDeleteId.Contains(x.FileInFolderId))
                        //    .ToList();
                        //if (listFileDelete.Count > 0)
                        //{
                        //    context.FileInFolder.RemoveRange(listFileDelete);
                        //    listFileDelete.ForEach(file =>
                        //    {
                        //        var fileName = folder.Url + "\\" + file.FileName + "." + file.FileExtension;
                        //        string webRootPath = _hostingEnvironment.WebRootPath;
                        //        string newPath = Path.Combine(webRootPath, fileName);
                        //        if (Directory.Exists(newPath))
                        //        {
                        //            Directory.Delete(newPath);
                        //        }
                        //    });
                        //}

                        var List_AdditionalInformation = context.AdditionalInformation
                            .Where(x => x.ObjectId == parameter.Contract.ContractId && x.ObjectType == "CONTRACT")
                            .ToList();

                        context.ContractDetailProductAttribute.RemoveRange(
                            List_Delete_ContractProductDetailProductAttributeValue);
                        context.ContractDetail.RemoveRange(List_Delete_ContractDetail);
                        context.AdditionalInformation.RemoveRange(List_AdditionalInformation);
                        context.ContractCostDetail.RemoveRange(listDeleteContractCost);
                        context.Contract.Remove(oldContract);
                        context.SaveChanges();

                        #endregion

                        #region Create new order base on Old Order

                        parameter.ContractDetail.ForEach(item =>
                        {
                            item.ContractId = parameter.Contract.ContractId;
                            item.CreatedById = parameter.UserId;
                            item.CreatedDate = DateTime.Now;
                            if (item.ContractDetailId == null || item.ContractDetailId == Guid.Empty)
                                item.ContractDetailId = Guid.NewGuid();
                            foreach (var itemX in item.ContractProductDetailProductAttributeValue)
                            {
                                if (itemX.ContractDetailProductAttributeId == null ||
                                    itemX.ContractDetailProductAttributeId == Guid.Empty)
                                    itemX.ContractDetailProductAttributeId = Guid.NewGuid();
                                itemX.ContractDetailId = item.ContractDetailId;
                            }
                        });

                        var contractDetails = parameter.ContractDetail
                            .Select(m => new ContractDetail
                            {
                                ContractDetailId = m.ContractDetailId,
                                ContractId = m.ContractId,
                                VendorId = m.VendorId,
                                ProductId = m.ProductId,
                                Quantity = m.Quantity,
                                QuantityOdered = m.QuantityOdered,
                                UnitPrice = m.UnitPrice,
                                CurrencyUnit = m.CurrencyUnit,
                                ExchangeRate = m.ExchangeRate,
                                Tax = m.Vat,
                                GuaranteeTime = m.GuaranteeTime,
                                DiscountType = m.DiscountType,
                                DiscountValue = m.DiscountValue,
                                Description = m.Description,
                                OrderDetailType = m.OrderDetailType,
                                UnitId = m.UnitId,
                                IncurredUnit = m.IncurredUnit,
                                CreatedById = m.CreatedById,
                                CreatedDate = m.CreatedDate,
                                UpdatedById = m.UpdatedById,
                                UpdatedDate = m.UpdatedDate,
                                ProductName = m.ProductName,
                                OrderNumber = m.OrderNumber
                            }).ToList();

                        parameter.ContractDetail.ForEach(item =>
                        {
                            var contractAttributes = item.ContractProductDetailProductAttributeValue
                                .Select(m => new ContractDetailProductAttribute
                                {
                                    ContractDetailProductAttributeId = m.ContractDetailProductAttributeId,
                                    ContractDetailId = m.ContractDetailId,
                                    ProductId = m.ProductId,
                                    ProductAttributeCategoryId = m.ProductAttributeCategoryId,
                                    ProductAttributeCategoryValueId = m.ProductAttributeCategoryValueId,
                                });
                            listContractProductAttribute.AddRange(contractAttributes);
                        });

                        List<AdditionalInformation> listAdditionalInformation = new List<AdditionalInformation>();
                        parameter.ListAdditionalInformation.ForEach(item =>
                        {
                            var additionalInformation = new AdditionalInformation();
                            additionalInformation.AdditionalInformationId = Guid.NewGuid();
                            additionalInformation.ObjectId = parameter.Contract.ContractId;
                            additionalInformation.ObjectType = "CONTRACT";
                            additionalInformation.Title = item.Title;
                            additionalInformation.Content = item.Content;
                            additionalInformation.Ordinal = item.Ordinal;
                            additionalInformation.Active = true;
                            additionalInformation.CreatedById = parameter.UserId;
                            additionalInformation.CreatedDate = DateTime.Now;
                            additionalInformation.UpdatedById = null;
                            additionalInformation.UpdatedDate = null;
                            additionalInformation.OrderNumber = item.OrderNumber;

                            listAdditionalInformation.Add(additionalInformation);
                        });
                        context.AdditionalInformation.AddRange(listAdditionalInformation);

                        List<ContractCostDetail> contractCostDetails = new List<ContractCostDetail>();
                        parameter.ListContractCost.ForEach(item =>
                        {
                            var contractCost = new ContractCostDetail
                            {
                                ContractCostDetailId = Guid.NewGuid(),
                                ContractId = parameter.Contract.ContractId,
                                CostId = item.CostId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                CostName = item.CostName,
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = null,
                                UpdatedDate = null,
                            };

                            contractCostDetails.Add(contractCost);
                        });

                        if (parameter.IsCreate)
                        {
                            if (parameter.ListFormFile != null && parameter.ListFormFile.Count > 0)
                            {
                                var folderName = folder.Url + "\\";
                                string webRootPath = _hostingEnvironment.WebRootPath;
                                string newPath = Path.Combine(webRootPath, folderName);
                                if (!Directory.Exists(newPath))
                                {
                                    Directory.CreateDirectory(newPath);
                                }
                                foreach (IFormFile file in parameter.ListFormFile)
                                {
                                    if (file.Length > 0)
                                    {
                                        string fileName = file.FileName.Trim();

                                        var fileInForder = new FileInFolder();
                                        fileInForder.Active = true;
                                        fileInForder.CreatedById = parameter.UserId;
                                        fileInForder.CreatedDate = DateTime.Now;
                                        fileInForder.FileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                                        fileInForder.FileInFolderId = Guid.NewGuid();
                                        fileInForder.FileName =
                                            fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + Guid.NewGuid();
                                        fileInForder.FolderId = folder.FolderId;
                                        fileInForder.ObjectId = parameter.Contract.ContractId;
                                        fileInForder.ObjectType = "QLHD";
                                        fileInForder.Size = file.Length.ToString();

                                        context.FileInFolder.Add(fileInForder);
                                        fileName = fileInForder.FileName + "." + fileInForder.FileExtension;
                                        string fullPath = Path.Combine(newPath, fileName);
                                        using (var stream = new FileStream(fullPath, FileMode.Create))
                                        {
                                            file.CopyTo(stream);
                                        }
                                    }
                                }
                            }
                        }

                        parameter.Contract.ContractId = oldContract.ContractId;
                        parameter.Contract.StatusId = oldContract.StatusId;
                        parameter.Contract.Active = oldContract.Active;
                        parameter.Contract.IsExtend = oldContract.IsExtend;
                        parameter.Contract.CreatedById = oldContract.CreatedById;
                        parameter.Contract.CreatedDate = oldContract.CreatedDate;
                        parameter.Contract.UpdatedById = parameter.UserId;
                        parameter.Contract.UpdatedDate = DateTime.Now;
                        context.ContractDetailProductAttribute.AddRange(listContractProductAttribute);
                        context.ContractDetail.AddRange(contractDetails);
                        context.ContractCostDetail.AddRange(contractCostDetails);
                        context.Contract.Add(parameter.Contract);

                        context.SaveChanges();
                        transaction.Commit();
                        #region Gửi thông báo

                        NotificationHelper.AccessNotification(context, TypeModel.ContractDetail, "UPD", new Contract(),
                            parameter.Contract, true);

                        #endregion

                        #endregion
                    }
                }
                else
                {
                    parameter.Contract.StatusId = newStatusContract;
                    parameter.Contract.Active = true;
                    parameter.Contract.IsExtend = false;
                    parameter.Contract.ContractId = Guid.NewGuid();
                    parameter.Contract.CreatedById = parameter.UserId;
                    parameter.Contract.CreatedDate = DateTime.Now;
                    parameter.Contract.UpdatedById = null;
                    parameter.Contract.UpdatedDate = null;

                    #region Create New Order with GenerateorderCode

                    parameter.ContractDetail.ForEach(item =>
                    {
                        item.ContractId = parameter.Contract.ContractId;
                        item.ContractDetailId = Guid.NewGuid();
                        foreach (var itemX in item.ContractProductDetailProductAttributeValue)
                        {
                            itemX.ContractDetailProductAttributeId = Guid.NewGuid();
                            itemX.ContractDetailId = item.ContractDetailId;
                        }
                    });

                    var contractDetails = parameter.ContractDetail
                        .Select(m => new ContractDetail
                        {
                            ContractDetailId = m.ContractDetailId,
                            ContractId = m.ContractId,
                            VendorId = m.VendorId,
                            ProductId = m.ProductId,
                            Quantity = m.Quantity,
                            QuantityOdered = m.QuantityOdered,
                            UnitPrice = m.UnitPrice,
                            CurrencyUnit = m.CurrencyUnit,
                            ExchangeRate = m.ExchangeRate ?? 1,
                            Tax = m.Vat,
                            GuaranteeTime = m.GuaranteeTime,
                            DiscountType = m.DiscountType,
                            DiscountValue = m.DiscountValue,
                            Description = m.Description,
                            OrderDetailType = m.OrderDetailType,
                            UnitId = m.UnitId,
                            IncurredUnit = m.IncurredUnit,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                            ProductName = m.ProductName,
                            OrderNumber = m.OrderNumber
                        }).ToList();

                    parameter.ContractDetail.ForEach(item =>
                    {
                        var contractAttributes = item.ContractProductDetailProductAttributeValue
                            .Select(m => new ContractDetailProductAttribute
                            {
                                ContractDetailProductAttributeId = m.ContractDetailProductAttributeId,
                                ContractDetailId = m.ContractDetailId,
                                ProductId = m.ProductId,
                                ProductAttributeCategoryId = m.ProductAttributeCategoryId,
                                ProductAttributeCategoryValueId = m.ProductAttributeCategoryValueId,
                            });
                        listContractProductAttribute.AddRange(contractAttributes);
                    });

                    List<ContractCostDetail> contractCostDetails = new List<ContractCostDetail>();
                    parameter.ListContractCost.ForEach(item =>
                    {
                        var contractCost = new ContractCostDetail
                        {
                            ContractCostDetailId = Guid.NewGuid(),
                            ContractId = parameter.Contract.ContractId,
                            CostId = item.CostId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            CostName = item.CostName,
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = null,
                            UpdatedDate = null,
                        };

                        contractCostDetails.Add(contractCost);
                    });

                    parameter.Contract.ContractCode = GenerateoContractCode(parameter.Contract.ContractTypeId);

                    //Kiểm tra trùng contract
                    var dublicateQuote =
                        context.Quote.FirstOrDefault(x => x.QuoteCode == parameter.Contract.ContractCode);
                    if (dublicateQuote != null)
                    {
                        return new CreateOrUpdateContractResult
                        {
                            Status = false,
                            Message = CommonMessage.Contract.EXIST,
                            ContractId = Guid.Empty
                        };
                    }
                    context.ContractDetailProductAttribute.AddRange(listContractProductAttribute);
                    context.ContractDetail.AddRange(contractDetails);
                    context.Contract.Add(parameter.Contract);
                    context.ContractCostDetail.AddRange(contractCostDetails);
                    context.SaveChanges();

                    //Thêm thông tin bổ sung cho báo giá
                    parameter.ListAdditionalInformation.ForEach(item =>
                    {
                        item.AdditionalInformationId = Guid.NewGuid();
                        item.ObjectId = parameter.Contract.ContractId;
                        item.ObjectType = "CONTRACT";
                        item.Active = true;
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;
                        item.UpdatedById = null;
                        item.UpdatedDate = null;
                    });

                    context.AdditionalInformation.AddRange(parameter.ListAdditionalInformation);

                    if (parameter.IsCreate)
                    {
                        if (parameter.ListFormFile != null && parameter.ListFormFile.Count > 0)
                        {
                            var folderName = folder.Url + "\\";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }
                            foreach (IFormFile file in parameter.ListFormFile)
                            {
                                if (file.Length > 0)
                                {
                                    string fileName = file.FileName.Trim();

                                    var fileInForder = new FileInFolder();
                                    fileInForder.Active = true;
                                    fileInForder.CreatedById = parameter.UserId;
                                    fileInForder.CreatedDate = DateTime.Now;
                                    fileInForder.FileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                                    fileInForder.FileInFolderId = Guid.NewGuid();
                                    fileInForder.FileName =
                                        fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + Guid.NewGuid();
                                    fileInForder.FolderId = folder.FolderId;
                                    fileInForder.ObjectId = parameter.Contract.ContractId;
                                    fileInForder.ObjectType = "QLHD";
                                    fileInForder.Size = file.Length.ToString();

                                    context.FileInFolder.Add(fileInForder);
                                    fileName = fileInForder.FileName + "." + fileInForder.FileExtension;
                                    string fullPath = Path.Combine(newPath, fileName);
                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        file.CopyTo(stream);
                                    }
                                }
                            }
                        }
                    }

                    context.SaveChanges();

                    #region Gửi thông báo

                    NotificationHelper.AccessNotification(context, TypeModel.Contract, "CRE", new Contract(),
                        parameter.Contract, true);

                    #endregion

                    #endregion
                }

                return new CreateOrUpdateContractResult
                {
                    Status = true,
                    Message = CommonMessage.Contract.CREATE_SUCCESS,
                    ContractId = parameter.Contract.ContractId
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateContractResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public CreateCloneContractResult CreateCloneContract(CreateCloneContractParameter parameter)
        {
            bool isValidParameterNumber = !(parameter.Contract?.DiscountValue < 0);
            parameter.ContractDetail.ForEach(item =>
            {
                if (item?.DiscountValue < 0 || item?.ExchangeRate < 0 || item?.Quantity <= 0 || item?.UnitPrice < 0 || item?.Tax < 0)
                {
                    isValidParameterNumber = false;
                }
            });
            if (!isValidParameterNumber)
            {
                return new CreateCloneContractResult
                {
                    Message = CommonMessage.Contract.CREATE_FAIL,
                    Status = false
                };
            }

            //Kiểm tra chiết khấu của đơn hàng
            if (parameter.Contract.DiscountValue == null)
            {
                parameter.Contract.DiscountValue = 0;
            }

            //Kiểm tra chiết khấu của sản phẩm
            if (parameter.ContractDetail.Count > 0)
            {
                var listProduct = parameter.ContractDetail.ToList();
                listProduct.ForEach(item =>
                {
                    if (item.DiscountValue == null)
                    {
                        item.DiscountValue = 0;
                    }
                });
            }

            try
            {
                var listContractProductAttribute = new List<ContractDetailProductAttribute>();

                var contractCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THD");
                var listContracttatus = context.Category
                    .Where(x => x.CategoryTypeId == contractCategoryType.CategoryTypeId).ToList();
                var newStatusContract = listContracttatus.FirstOrDefault(c => c.CategoryCode == "MOI")?.CategoryId;

                parameter.Contract.StatusId = newStatusContract;
                parameter.Contract.Active = true;
                parameter.Contract.IsExtend = true;
                parameter.Contract.ContractId = Guid.NewGuid();
                parameter.Contract.CreatedById = parameter.UserId;
                parameter.Contract.CreatedDate = DateTime.Now;
                parameter.Contract.UpdatedById = null;
                parameter.Contract.UpdatedDate = null;

                #region Create New Order with GenerateorderCode

                parameter.ContractDetail.ForEach(item =>
                {
                    item.ContractId = parameter.Contract.ContractId;
                    item.ContractDetailId = Guid.NewGuid();
                    foreach (var itemX in item.ContractProductDetailProductAttributeValue)
                    {
                        itemX.ContractDetailProductAttributeId = Guid.NewGuid();
                        itemX.ContractDetailId = item.ContractDetailId;
                    }
                });

                var contractDetails = parameter.ContractDetail
                    .Select(m => new ContractDetail
                    {
                        ContractDetailId = m.ContractDetailId,
                        ContractId = m.ContractId,
                        VendorId = m.VendorId,
                        ProductId = m.ProductId,
                        Quantity = m.Quantity,
                        QuantityOdered = m.QuantityOdered,
                        UnitPrice = m.UnitPrice,
                        CurrencyUnit = m.CurrencyUnit,
                        ExchangeRate = m.ExchangeRate ?? 1,
                        Tax = m.Vat,
                        GuaranteeTime = m.GuaranteeTime,
                        DiscountType = m.DiscountType,
                        DiscountValue = m.DiscountValue,
                        Description = m.Description,
                        OrderDetailType = m.OrderDetailType,
                        UnitId = m.UnitId,
                        IncurredUnit = m.IncurredUnit,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null,
                        ProductName = m.ProductName,
                        OrderNumber = m.OrderNumber
                    }).ToList();

                parameter.ContractDetail.ForEach(item =>
                {
                    var contractAttributes = item.ContractProductDetailProductAttributeValue
                        .Select(m => new ContractDetailProductAttribute
                        {
                            ContractDetailProductAttributeId = m.ContractDetailProductAttributeId,
                            ContractDetailId = m.ContractDetailId,
                            ProductId = m.ProductId,
                            ProductAttributeCategoryId = m.ProductAttributeCategoryId,
                            ProductAttributeCategoryValueId = m.ProductAttributeCategoryValueId,
                        });
                    listContractProductAttribute.AddRange(contractAttributes);
                });

                List<ContractCostDetail> contractCostDetails = new List<ContractCostDetail>();
                parameter.ListContractCost.ForEach(item =>
                {
                    var contractCost = new ContractCostDetail
                    {
                        ContractCostDetailId = Guid.NewGuid(),
                        ContractId = parameter.Contract.ContractId,
                        CostId = item.CostId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        CostName = item.CostName,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null,
                    };

                    contractCostDetails.Add(contractCost);
                });

                // set lại loại và số hợp đồng
                var typeContractCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LHD")?.CategoryTypeId;
                var typeContract = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeContractCategoryTypeId && c.CategoryCode == "PLHD") ?? new Category();

                //var mainContract = context.Contract.FirstOrDefault(x => x.ContractId == parameter.ContractId);
                //var total = context.Contract.Where(x => x.MainContractId == parameter.ContractId).Count() + 1;
                //string totalString = "";
                //if (total < 10)
                //{
                //    totalString = "0" + total;
                //}
                //else
                //{
                //    totalString = total.ToString();
                //}

                //var newCode = mainContract.ContractCode + '/' + totalString;
                //var _index = newCode.IndexOf("-");
                //newCode = newCode.Substring(_index);
                //newCode = "PLHĐ" + newCode;

                parameter.Contract.ContractTypeId = typeContract.CategoryId;
                //parameter.Contract.ContractCode = newCode;
                if (parameter.Contract.ExpiredDate != null)
                {
                    parameter.Contract.EffectiveDate = parameter.Contract.ExpiredDate.Value.AddDays(1);

                    var addDay = parameter.Contract.ContractTime;

                    if (addDay != null)
                    {
                        switch (parameter.Contract.ContractTimeUnit)
                        {
                            case "DAY":
                                parameter.Contract.ExpiredDate = parameter.Contract.EffectiveDate.AddDays((double)addDay);
                                break;
                            case "MONTH":
                                parameter.Contract.ExpiredDate = parameter.Contract.EffectiveDate.AddMonths((int)addDay);
                                break;
                            case "YEAR":
                                parameter.Contract.ExpiredDate = parameter.Contract.EffectiveDate.AddYears((int)addDay);
                                break;
                        }
                    }
                }

                // update lại ngày hiệu lực của hợp đồng chính
                var mainContract =
                    context.Contract.FirstOrDefault(x => x.Active && x.ContractId == parameter.ContractId);

                if (parameter.Contract.ExpiredDate != null)
                    mainContract.ExpiredDate = parameter.Contract.ExpiredDate.Value;


                //Kiểm tra trùng contract
                var dublicateQuote =
                    context.Quote.FirstOrDefault(x => x.QuoteCode == parameter.Contract.ContractCode);
                if (dublicateQuote != null)
                {
                    return new CreateCloneContractResult
                    {
                        Status = false,
                        Message = CommonMessage.Contract.EXIST,
                        ContractId = Guid.Empty
                    };
                }
                context.ContractDetailProductAttribute.AddRange(listContractProductAttribute);
                context.ContractDetail.AddRange(contractDetails);
                context.Contract.Add(parameter.Contract);
                context.Contract.Update(mainContract);
                context.ContractCostDetail.AddRange(contractCostDetails);

                //Thêm thông tin bổ sung cho báo giá
                parameter.ListAdditionalInformation.ForEach(item =>
                {
                    item.AdditionalInformationId = Guid.NewGuid();
                    item.ObjectId = parameter.Contract.ContractId;
                    item.ObjectType = "CONTRACT";
                    item.Active = true;
                    item.CreatedById = parameter.UserId;
                    item.CreatedDate = DateTime.Now;
                    item.UpdatedById = null;
                    item.UpdatedDate = null;
                });

                context.AdditionalInformation.AddRange(parameter.ListAdditionalInformation);

                context.SaveChanges();

                #region Gửi thông báo

                NotificationHelper.AccessNotification(context, TypeModel.Contract, "CRE", new Contract(),
                    parameter.Contract, true);

                #endregion

                #endregion

                #region Get list contract

                var contractType = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeContractCategoryTypeId && c.CategoryCode == "HDKT") ?? new Category();
                var listContract = context.Contract.Where(x => x.ContractTypeId == contractType.CategoryTypeId).ToList();

                #endregion

                return new CreateCloneContractResult
                {
                    Status = true,
                    Message = CommonMessage.Contract.CREATE_SUCCESS,
                    ContractId = parameter.Contract.ContractId,
                };
            }
            catch (Exception ex)
            {
                return new CreateCloneContractResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetListMainContractResult GetListMainContract(GetListMainContractParameter parameter)
        {
            // common data
            var commonCategoryType = context.CategoryType.ToList();
            var commonCategory = context.Category.ToList();

            var statusTypeId =
                commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THD")?.CategoryTypeId ?? Guid.Empty;
            var listAllStatus = commonCategory.Where(c => c.CategoryTypeId == statusTypeId).ToList();

            var confirmStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "APPR")?.CategoryId;
            var processingStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "DTH")?.CategoryId;

            // get nhan vien ban hang va cap duoi
            var listEmployee = context.Employee.ToList(); //Active = false ?
            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId);

            List<Contract> listContract;

            if (employee != null && employee.IsManager)
            {
                /*
             *   Lấy list phòng ban con của nhan vien ban hang
             *   List phòng ban bao gồm: chính phòng ban nó đang thuộc và các phòng ban cấp dưới của nó nếu có
             */

                List<Guid?> listGetAllChild = new List<Guid?>();
                if (employee.OrganizationId != null)
                {
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    listEmployee = listEmployee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)).ToList();
                }

                var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                listContract =
                    context.Contract.Where(c => c.Active == true &&
                                                (c.StatusId == confirmStatusId || c.StatusId == processingStatusId) &&
                                                (listEmployeeId.Contains((Guid)c.EmployeeId)))
                        .ToList() ?? new List<Entities.Contract>();
            }
            else
            {
                listContract = context.Contract.Where(c => c.Active == true &&
                                                           (c.StatusId == confirmStatusId || c.StatusId == processingStatusId) &&
                                                           (c.EmployeeId == employee.EmployeeId)).ToList();
            }

            return new GetListMainContractResult()
            {
                Status = true,
                Message = Common.CommonMessage.Contract.GET_DATA_SUCCESS,
                ListContract = listContract,
            };
        }

        public GetMaterDataContractResult GetMasterDataContract(GetMasterDataContractParameter parameter)
        {
            try
            {
                bool isOutOfDate = false;
                var listCustomer = new List<CustomerEntityModel>();
                var listQuote = new List<QuoteEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var employeeSeller = new EmployeeEntityModel();
                var listNote = new List<NoteEntityModel>();
                var listContractCost = new List<ContractCostDetailEntityModel>();
                var listFile = new List<FileInFolderEntityModel>();
                var listContract = new List<Contract>();

                // common data
                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();
                var commonLead = context.Lead.ToList() ?? new List<Entities.Lead>();
                var commonSaleBidding = context.SaleBidding.ToList() ?? new List<Entities.SaleBidding>();
                var commonProductAttributeCategoryValue =
                    context.ProductAttributeCategoryValue.Where(c => c.Active == true).ToList() ??
                    new List<Entities.ProductAttributeCategoryValue>();
                var commonProductAttributeCategry =
                    context.ProductAttributeCategory.Where(c => c.Active == true).ToList() ??
                    new List<Entities.ProductAttributeCategory>();

                var statusTypeId =
                    commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THD")?.CategoryTypeId ?? Guid.Empty;
                var listAllStatus = commonCategory.Where(c => c.CategoryTypeId == statusTypeId).ToList();

                var commonCost = context.Cost.Where(c => c.Active == true).ToList();

                var confirmStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "APPR")?.CategoryId;
                var processingStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "DTH")?.CategoryId;

                //var listContract =
                //    context.Contract.Where(c =>
                //            c.Active == true && (c.StatusId == confirmStatusId || c.StatusId == processingStatusId))
                //        .ToList() ?? new List<Entities.Contract>();

                #region Nếu xem chi tiết của hợp đồng

                var contract = new ContractEntityModel();
                var listContractDetail = new List<ContractDetailEntityModel>();
                var listAdditionalInformation = new List<AdditionalInformationEntityModel>();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listStatusActive = listAllStatus.Where(c => c.Active == true)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();

                if (parameter.ContractId != Guid.Empty)
                {
                    var oldContract = context.Contract.FirstOrDefault(c => c.ContractId == parameter.ContractId);
                    contract.ContractId = oldContract.ContractId;
                    contract.QuoteId = oldContract.QuoteId;
                    contract.CustomerId = oldContract.CustomerId;
                    contract.ContractCode = oldContract.ContractCode;
                    contract.ContractTypeId = oldContract.ContractTypeId;
                    contract.EmployeeId = oldContract.EmployeeId;
                    contract.MainContractId = oldContract.MainContractId;
                    contract.ContractNote = oldContract.ContractNote;
                    contract.ContractDescription = oldContract.ContractDescription;
                    contract.ValueContract = oldContract.ValueContract;
                    contract.PaymentMethodId = oldContract.PaymentMethodId;
                    contract.BankAccountId = oldContract.BankAccountId;
                    contract.EffectiveDate = oldContract.EffectiveDate;
                    contract.ExpiredDate = oldContract.ExpiredDate;
                    contract.ContractTime = oldContract.ContractTime;
                    contract.ContractTimeUnit = oldContract.ContractTimeUnit;
                    contract.DiscountType = oldContract.DiscountType;
                    contract.DiscountValue = oldContract.DiscountValue;
                    contract.Amount = oldContract.Amount;
                    contract.Active = oldContract.Active;
                    contract.IsExtend = oldContract.IsExtend;
                    contract.CreatedById = oldContract.CreatedById;
                    contract.CreatedDate = oldContract.CreatedDate;
                    contract.UpdatedById = oldContract.UpdatedById;
                    contract.UpdatedDate = oldContract.UpdatedDate;
                    contract.StatusId = oldContract.StatusId.Value;

                    parameter.QuoteId = oldContract.QuoteId;
                    if (contract.EmployeeId != null && contract.EmployeeId != Guid.Empty)
                    {
                        var empSeller = context.Employee.FirstOrDefault(x => x.EmployeeId == contract.EmployeeId);
                    }

                    #region Lấy chi tiết hợp đồng theo sản phẩm dịch vụ (OrderDetailType = 0)

                    var listContractObjectType0 = (from cod in context.ContractDetail
                                                   where cod.ContractId == parameter.ContractId && cod.OrderDetailType == 0
                                                   select (new ContractDetailEntityModel
                                                   {
                                                       CreatedById = cod.CreatedById,
                                                       ContractId = cod.ContractId,
                                                       VendorId = cod.VendorId,
                                                       CreatedDate = cod.CreatedDate,
                                                       CurrencyUnit = cod.CurrencyUnit,
                                                       Description = cod.Description,
                                                       DiscountType = cod.DiscountType,
                                                       DiscountValue = cod.DiscountValue,
                                                       ExchangeRate = cod.ExchangeRate,
                                                       ContractDetailId = cod.ContractDetailId,
                                                       OrderDetailType = cod.OrderDetailType,
                                                       ProductId = cod.ProductId.Value,
                                                       UpdatedById = cod.UpdatedById,
                                                       Quantity = cod.Quantity,
                                                       UnitId = cod.UnitId,
                                                       IncurredUnit = cod.IncurredUnit,
                                                       UnitPrice = cod.UnitPrice,
                                                       UpdatedDate = cod.UpdatedDate,
                                                       Tax = cod.Tax,
                                                       GuaranteeTime = cod.GuaranteeTime,
                                                       Vat = cod.Tax,
                                                       NameVendor = "",
                                                       NameProduct = "",
                                                       NameProductUnit = "",
                                                       NameMoneyUnit = "",
                                                       SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Tax,
                                                           cod.DiscountValue, cod.DiscountType),
                                                       ProductName = cod.ProductName ?? "",
                                                       OrderNumber = cod.OrderNumber
                                                   })).ToList();

                    if (listContractObjectType0 != null)
                    {
                        List<Guid> listVendorId = new List<Guid>();
                        List<Guid> listProductId = new List<Guid>();
                        List<Guid> listCategoryId = new List<Guid>();
                        listContractObjectType0.ForEach(item =>
                        {
                            if (item.VendorId != null && item.VendorId != Guid.Empty)
                                listVendorId.Add(item.VendorId.Value);
                            if (item.ProductId != null && item.ProductId != Guid.Empty)
                                listProductId.Add(item.ProductId.Value);
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                listCategoryId.Add(item.CurrencyUnit.Value);
                            if (item.UnitId != null && item.UnitId != Guid.Empty)
                                listCategoryId.Add(item.UnitId.Value);
                        });

                        var listVendor = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                        var listProduct = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();
                        var listCategory = context.Category.Where(w => listCategoryId.Contains(w.CategoryId)).ToList();

                        listContractObjectType0.ForEach(item =>
                        {
                            if (item.VendorId != null && item.VendorId != Guid.Empty)
                                item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId)
                                    .VendorName;
                            if (item.ProductId != null && item.ProductId != Guid.Empty)
                                item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                    .ProductName;
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                    .CategoryName;
                            if (item.UnitId != null && item.UnitId != Guid.Empty)
                                item.NameProductUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId)
                                    .CategoryName;
                            item.ContractProductDetailProductAttributeValue =
                                getListContractProductDetailProductAttributeValue(item.ContractDetailId);
                        });
                    }

                    listContractDetail.AddRange(listContractObjectType0);

                    #endregion

                    #region Lấy chi tiết hợp đồng theo sản phẩm dịch vụ (OrderDetailType = 1)

                    var listContractObjectType1 = (from cod in context.ContractDetail
                                                   where cod.ContractId == parameter.ContractId && cod.OrderDetailType == 1
                                                   select (new ContractDetailEntityModel
                                                   {
                                                       CreatedById = cod.CreatedById,
                                                       ContractId = cod.ContractId,
                                                       VendorId = cod.VendorId,
                                                       CreatedDate = cod.CreatedDate,
                                                       CurrencyUnit = cod.CurrencyUnit,
                                                       Description = cod.Description,
                                                       DiscountType = cod.DiscountType,
                                                       DiscountValue = cod.DiscountValue,
                                                       ExchangeRate = cod.ExchangeRate,
                                                       ContractDetailId = cod.ContractDetailId,
                                                       OrderDetailType = cod.OrderDetailType,
                                                       ProductId = cod.ProductId.Value,
                                                       UpdatedById = cod.UpdatedById,
                                                       Quantity = cod.Quantity,
                                                       UnitId = cod.UnitId,
                                                       IncurredUnit = cod.IncurredUnit,
                                                       UnitPrice = cod.UnitPrice,
                                                       UpdatedDate = cod.UpdatedDate,
                                                       Tax = cod.Tax,
                                                       Vat = cod.Tax,
                                                       GuaranteeTime = cod.GuaranteeTime,
                                                       NameVendor = "",
                                                       NameProduct = "",
                                                       NameProductUnit = "",
                                                       NameMoneyUnit = "",
                                                       SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Tax,
                                                           cod.DiscountValue, cod.DiscountType),
                                                       ProductName = cod.ProductName ?? "",
                                                       OrderNumber = cod.OrderNumber
                                                   })).ToList();

                    if (listContractObjectType1 != null)
                    {
                        List<Guid> listCategoryId = new List<Guid>();
                        listContractObjectType1.ForEach(item =>
                        {
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                listCategoryId.Add(item.CurrencyUnit.Value);
                        });
                        var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                        listContractObjectType1.ForEach(item =>
                        {
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                    .CategoryName;
                        });
                    }

                    listContractDetail.AddRange(listContractObjectType1);

                    listContractDetail = listContractDetail.OrderBy(z => z.OrderNumber).ToList();

                    #endregion

                    // list thông tin bổ sung của hợp đồng
                    listAdditionalInformation = context.AdditionalInformation
                        .Where(x => x.ObjectId == parameter.ContractId && x.ObjectType == "CONTRACT" && x.Active == true)
                        .Select(y =>
                        new AdditionalInformationEntityModel
                        {
                            AdditionalInformationId = y.AdditionalInformationId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Title = y.Title,
                            Content = y.Content,
                            Ordinal = y.Ordinal,
                            OrderNumber = y.OrderNumber
                        }).OrderBy(z => z.OrderNumber).ToList();

                    // list ghi chú của hợp đồng
                    listNote = context.Note.Where(x =>
                            x.ObjectId == parameter.ContractId && x.ObjectType == "CONTRACT" && x.Active == true)
                        .Select(y => new NoteEntityModel
                        {
                            NoteId = y.NoteId,
                            Description = y.Description,
                            Type = y.Type,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            NoteTitle = y.NoteTitle,
                            Active = y.Active,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            ResponsibleName = "",
                            ResponsibleAvatar = "",
                            NoteDocList = new List<NoteDocumentEntityModel>()
                        }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var _listAllEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }
                        ).ToList();
                        listNote.ForEach(item =>
                        {
                            var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderBy(z => z.UpdatedDate).ToList();
                        });

                        // Sắp xếp lại listnote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    // List chi phí của hợp đồng
                    listContractCost = context.ContractCostDetail.Where(c => c.ContractId == parameter.ContractId)
                        .Select(c => new ContractCostDetailEntityModel
                        {
                            ContractCostDetailId = c.ContractCostDetailId,
                            ContractId = c.ContractId,
                            CostId = c.CostId,
                            CostCode = commonCost.FirstOrDefault(m => m.CostId == c.CostId).CostCode ?? "",
                            CostName = commonCost.FirstOrDefault(m => m.CostId == c.CostId).CostName ?? "",
                            Quantity = c.Quantity,
                            UnitPrice = c.UnitPrice,
                            CreatedById = c.CreatedById,
                            CreatedDate = c.CreatedDate,
                        }).OrderBy(c => c.CreatedDate).ToList();

                    // List file của hợp đồng
                    var listFileInFolder = context.FileInFolder
                        .Where(x => parameter.ContractId == x.ObjectId && x.ObjectType == "QLHD" && x.Active == true)
                        .Select(y => new FileInFolderEntityModel()
                        {
                            Size = y.Size,
                            ObjectId = y.ObjectId,
                            Active = y.Active,
                            FileExtension = y.FileExtension,
                            FileInFolderId = y.FileInFolderId,
                            FileName = y.FileName,
                            FolderId = y.FolderId,
                            ObjectType = y.ObjectType,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate
                        }).ToList();

                    listFile = listFileInFolder.ToList();

                    var today = DateTime.Today;
                    var tmpDate = today.AddDays(30);

                    if (contract.ExpiredDate != null)
                    {
                        var expiredDate = contract.ExpiredDate.Value;
                        if (expiredDate != null)
                        {
                            isOutOfDate = expiredDate.Date <= tmpDate.Date;
                        }
                    }
                }

                var statusQuoteCategoryTypeId =
                    commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TGI")?.CategoryTypeId;
                var statusQuoteCategoryId = commonCategory
                    .FirstOrDefault(c => c.CategoryTypeId == statusQuoteCategoryTypeId && c.CategoryCode == "DTH")
                    ?.CategoryId;
                // Khách hàng định danh
                var categoryTypeTHA =
                    commonCategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "THA");
                var categoryHDO = commonCategory.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "HDO" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);

                // Lấy thêm những khách hàng không do nhân viên phụ trách nhưng được chỉ định phục trách báo giá
                var cusEntityModel = new CustomerEntityModel();
                if (parameter.QuoteId != null && parameter.QuoteId != Guid.Empty)
                {
                    var customerId = context.Quote.FirstOrDefault(c => c.QuoteId == parameter.QuoteId)?.ObjectTypeId;
                    cusEntityModel = context.Customer.Where(c => c.CustomerId == customerId)
                      .Select(y => new CustomerEntityModel
                      {
                          CustomerId = y.CustomerId,
                          CustomerCode = y.CustomerCode,
                          CustomerName = y.CustomerName,
                          CustomerEmail = "",
                          CustomerEmailWork = "",
                          CustomerEmailOther = "",
                          CustomerPhone = "",
                          FullAddress = "",
                          CustomerCompany = "",
                          StatusId = y.StatusId,
                          PersonInChargeId = y.PersonInChargeId,
                          CustomerGroupId = y.CustomerGroupId
                      }).FirstOrDefault();
                }
                if (employee.IsManager)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    listEmployee = context.Employee
                        .Where(x => x.Active == true &&
                                    (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();
                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = context.Customer
                        .Where(x => (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                     x.StatusId == categoryHDO.CategoryId) &&
                                    x.Active == true)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerEmail = "",
                            CustomerEmailWork = "",
                            CustomerEmailOther = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            CustomerCompany = "",
                            StatusId = y.StatusId,
                            PersonInChargeId = y.PersonInChargeId,
                            CustomerGroupId = y.CustomerGroupId
                        }).ToList();

                    if (parameter.QuoteId != null && parameter.QuoteId != Guid.Empty)
                    {
                        var lisId = listCustomer.Select(x => x.CustomerId).ToList();
                        if (!lisId.Contains(cusEntityModel.CustomerId))
                        {
                            listCustomer.Add(cusEntityModel);
                        }
                    }

                    // Get List quote ở trạng thái báo giá
                    listQuote = context.Quote.Where(c => c.Active == true && c.StatusId == statusQuoteCategoryId
                                                                          && (listEmployeeId.Count == 0 ||
                                                                              listEmployeeId.Contains(c.Seller.Value)))
                        .Select(m => new QuoteEntityModel
                        {
                            QuoteId = m.QuoteId,
                            QuoteCode = m.QuoteCode,
                            QuoteName = m.QuoteName,
                            QuoteDate = m.QuoteDate,
                            SendQuoteDate = m.SendQuoteDate,
                            Seller = m.Seller,
                            EffectiveQuoteDate = m.EffectiveQuoteDate,
                            ExpirationDate = m.ExpirationDate,
                            CustomerContactId = m.CustomerContactId,
                            ObjectTypeId = m.ObjectTypeId,
                            ObjectType = m.ObjectType,
                            PaymentMethod = m.PaymentMethod,
                            BankAccountId = m.BankAccountId,
                            DiscountType = m.DiscountType,
                            DiscountValue = m.DiscountValue,
                            LeadId = m.LeadId,
                            LeadCode = commonLead.FirstOrDefault(c => c.LeadId == m.LeadId).LeadCode ?? "",
                            SaleBiddingId = m.SaleBiddingId,
                            SaleBiddingCode = commonSaleBidding.FirstOrDefault(c => c.SaleBiddingId == m.SaleBiddingId)
                                                  .SaleBiddingCode ?? "",
                        }).ToList();

                    #region Lấy list hợp đồng chính của chi tiết Hợp đồng

                    if (parameter.ContractId != Guid.Empty)
                    {
                        listContract =
                            context.Contract.Where(c => c.Active &&
                                                        (listEmployeeId.Contains((Guid)c.EmployeeId)))
                                .OrderBy(x => x.CreatedDate)
                                .ToList() ?? new List<Entities.Contract>();
                    }

                    #endregion
                }
                else
                {
                    listEmployee = context.Employee.Where(x => x.EmployeeId == employee.EmployeeId && x.Active == true)
                        .Select(y =>
                            new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).ToList();

                    listCustomer = context.Customer
                        .Where(x => x.PersonInChargeId == employee.EmployeeId && x.Active == true &&
                                    x.StatusId == categoryHDO.CategoryId).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerCode = y.CustomerCode,
                                CustomerName = y.CustomerName,
                                CustomerEmail = "",
                                CustomerEmailWork = "",
                                CustomerEmailOther = "",
                                CustomerPhone = "",
                                FullAddress = "",
                                CustomerCompany = "",
                                StatusId = y.StatusId,
                                PersonInChargeId = y.PersonInChargeId,
                                CustomerGroupId = y.CustomerGroupId
                            }).ToList();

                    if (parameter.QuoteId != null && parameter.QuoteId != Guid.Empty)
                    {
                        var lisId = listCustomer.Select(x => x.CustomerId).ToList();
                        if (!lisId.Contains(cusEntityModel.CustomerId))
                        {
                            listCustomer.Add(cusEntityModel);
                        }
                    }

                    // Get List quote ở trạng thái báo giá
                    listQuote = context.Quote.Where(c => c.Active == true && c.StatusId == statusQuoteCategoryId
                                                                          && c.Seller == employee.EmployeeId).Select(
                        m => new QuoteEntityModel
                        {
                            QuoteId = m.QuoteId,
                            QuoteCode = m.QuoteCode,
                            QuoteName = m.QuoteName,
                            QuoteDate = m.QuoteDate,
                            SendQuoteDate = m.SendQuoteDate,
                            Seller = m.Seller,
                            EffectiveQuoteDate = m.EffectiveQuoteDate,
                            ExpirationDate = m.ExpirationDate,
                            CustomerContactId = m.CustomerContactId,
                            ObjectTypeId = m.ObjectTypeId,
                            ObjectType = m.ObjectType,
                            PaymentMethod = m.PaymentMethod,
                            BankAccountId = m.BankAccountId,
                            DiscountType = m.DiscountType,
                            DiscountValue = m.DiscountValue,
                            LeadId = m.LeadId,
                            LeadCode = commonLead.FirstOrDefault(c => c.LeadId == m.LeadId).LeadCode ?? "",
                            SaleBiddingId = m.SaleBiddingId,
                            SaleBiddingCode = commonSaleBidding.FirstOrDefault(c => c.SaleBiddingId == m.SaleBiddingId)
                                                  .SaleBiddingCode ?? "",
                        }).ToList();

                    #region Lấy list hợp đồng chính của chi tiết Hợp đồng

                    if (parameter.ContractId != Guid.Empty)
                    {
                        listContract = context.Contract.Where(c => c.Active &&
                                                                   (c.EmployeeId == employee.EmployeeId))
                            .OrderBy(x => x.CreatedDate)
                            .ToList();
                    }

                    #endregion
                }

                #endregion

                // lấy thông tin thuộc tính sản phẩm dịch vụ báo giá
                var listQuoteProductDetailAttibute = context.QuoteProductDetailProductAttributeValue
                    .Select(m => new QuoteProductDetailProductAttributeValueEntityModel
                    {
                        QuoteDetailId = m.QuoteDetailId,
                        ProductId = m.ProductId,
                        ProductAttributeCategoryId = m.ProductAttributeCategoryId,
                        ProductAttributeCategoryValueId = m.ProductAttributeCategoryValueId,
                        QuoteProductDetailProductAttributeValueId = m.QuoteProductDetailProductAttributeValueId,
                        NameProductAttributeCategoryValue =
                            commonProductAttributeCategoryValue
                                .FirstOrDefault(c =>
                                    c.ProductAttributeCategoryValueId == m.ProductAttributeCategoryValueId)
                                .ProductAttributeCategoryValue1 ?? "",
                        NameProductAttributeCategory =
                            commonProductAttributeCategry
                                .FirstOrDefault(c => c.ProductAttributeCategoryId == m.ProductAttributeCategoryId)
                                .ProductAttributeCategoryName ?? ""
                    }).ToList();

                var listQuoteDetail = context.QuoteDetail.Where(c => c.Active == true)
                    .Select(m => new QuoteDetailEntityModel
                    {
                        QuoteDetailId = m.QuoteDetailId,
                        VendorId = m.VendorId,
                        QuoteId = m.QuoteId,
                        ProductId = m.ProductId,
                        Quantity = m.Quantity,
                        UnitPrice = m.UnitPrice,
                        CurrencyUnit = m.CurrencyUnit,
                        ExchangeRate = m.ExchangeRate,
                        Vat = m.Vat,
                        DiscountType = m.DiscountType,
                        DiscountValue = m.DiscountValue,
                        Description = m.Description,
                        OrderDetailType = m.OrderDetailType,
                        UnitId = m.UnitId,
                        IncurredUnit = m.IncurredUnit,
                        Active = m.Active,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        ProductName = m.ProductName,
                        QuoteProductDetailProductAttributeValue =
                            listQuoteProductDetailAttibute.Where(c => c.QuoteDetailId == m.QuoteDetailId).ToList(),
                        OrderNumber = m.OrderNumber
                    }).OrderBy(z => z.OrderNumber).ToList();

                if (listQuoteDetail != null)
                {
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteDetail.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            listVendorId.Add(item.VendorId.Value);
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            listProductId.Add(item.ProductId.Value);
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            listCategoryId.Add(item.UnitId.Value);
                    });

                    var listVendor = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                    var listProduct = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();
                    var listCategory = context.Category.Where(w => listCategoryId.Contains(w.CategoryId)).ToList();

                    listQuoteDetail.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId).VendorName;
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductName;
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            item.NameProductUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId)
                                .CategoryName;
                        item.NameGene = item.NameProduct + "(" + getNameGEn(item.QuoteDetailId) + ")";
                    });
                }

                var listQuoteCostDetail = context.QuoteCostDetail.Where(c => c.Active == true)
                    .Select(m => new QuoteCostDetailEntityModel
                    {
                        QuoteCostDetailId = m.QuoteCostDetailId,
                        QuoteId = m.QuoteId,
                        CostId = m.CostId,
                        CostCode = commonCost.FirstOrDefault(e => e.CostId == m.CostId).CostCode ?? "",
                        CostName = commonCost.FirstOrDefault(e => e.CostId == m.CostId).CostName ?? "",
                        Quantity = m.Quantity,
                        UnitPrice = m.UnitPrice,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate
                    }).OrderBy(y => y.CreatedDate).ToList();

                // List Type contract
                var typeContractCategoryTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LHD").CategoryTypeId;
                var listTypeContract = commonCategory
                    .Where(c => c.CategoryTypeId == typeContractCategoryTypeId && c.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                            CategoryTypeId = Guid.Empty,
                            CreatedById = Guid.Empty,
                            CountCategoryById = 0
                        }).ToList();

                // List Payment Method
                var paymentMethodCategoryTypeId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true).CategoryTypeId;
                var listPaymentMethod = commonCategory
                    .Where(x => x.CategoryTypeId == paymentMethodCategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                            CategoryTypeId = Guid.Empty,
                            CreatedById = Guid.Empty,
                            CountCategoryById = 0
                        }).ToList();

                //List Status
                var listProvince = context.Province.ToList();
                var listDistrict = context.District.ToList();
                var listWard = context.Ward.ToList();

                List<Guid> listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                var listContactCustomer = context.Contact.Where(x =>
                        (listCustomerId.Count == 0 || listCustomerId.Contains(x.ObjectId)) && x.ObjectType == "CUS")
                    .ToList();

                listCustomer.ForEach(item =>
                {
                    var customerContact = listContactCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                    var address = customerContact == null
                        ? ""
                        : (customerContact.Address == null ? "" : (customerContact.Address.Trim()));
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province == null ? "" : (province.ProvinceName);
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district == null ? "" : (district.DistrictName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward == null ? "" : (ward.WardName + ", ");
                    item.CustomerEmail = customerContact == null
                        ? ""
                        : (customerContact.Email == null ? "" : customerContact.Email.Trim());
                    item.CustomerEmailOther = customerContact == null
                        ? ""
                        : (customerContact.OtherEmail == null ? "" : customerContact.OtherEmail.Trim());
                    item.CustomerEmailWork = customerContact == null
                        ? ""
                        : (customerContact.WorkEmail == null ? "" : customerContact.WorkEmail.Trim());
                    item.CustomerPhone = customerContact == null
                        ? ""
                        : (customerContact.Phone == null ? "" : customerContact.Phone.Trim());
                    item.FullAddress = address + wardName + districtName + provinceName;
                    item.CustomerCompany = customerContact == null
                        ? ""
                        : (customerContact.CompanyName == null ? "" : customerContact.CompanyName.Trim());
                    item.TaxCode = customerContact == null
                        ? ""
                        : (customerContact.TaxCode == null ? "" : customerContact.TaxCode.Trim());
                });

                var listBankAccount = context.BankAccount.Where(c => c.Active == true)?.ToList() ??
                                      new List<Entities.BankAccount>();




                return new GetMaterDataContractResult
                {
                    Status = true,
                    Message = Common.CommonMessage.Contract.GET_DATA_SUCCESS,
                    ListCustomer = listCustomer,
                    ListPaymentMethod = listPaymentMethod,
                    ListQuote = listQuote,
                    ListQuoteDetail = listQuoteDetail,
                    ListEmployeeSeller = listEmployee,
                    ListContractStatus = listStatusActive,
                    ListTypeContract = listTypeContract,
                    ListBankAccount = listBankAccount,
                    //ListContract = listContract,
                    Contract = contract,
                    ListContractDetail = listContractDetail,
                    ListContractDetailProductAttribute = new List<ContractDetailProductAttributeEntityModel>(),
                    ListAdditionalInformation = listAdditionalInformation,
                    ListNote = listNote,
                    ListContractCost = listContractCost,
                    ListQuoteCostDetail = listQuoteCostDetail,
                    ListFile = listFile,
                    ListFormFile = new List<IFormFile>(),
                    ListContract = listContract,
                    IsOutOfDate = isOutOfDate,
                };
            }
            catch (Exception ex)
            {
                return new GetMaterDataContractResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
        public string getNameGEn(Guid QuoteDetailId)
        {
            string Result = string.Empty;
            List<QuoteProductDetailProductAttributeValueEntityModel> listResult = new List<QuoteProductDetailProductAttributeValueEntityModel>();

            var QuoteProductDetailProductAttributeValueEntityModelList = (from OPDPV in context.QuoteProductDetailProductAttributeValue
                                                                          join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                          where OPDPV.QuoteDetailId == QuoteDetailId
                                                                          select (ProductAttributeCategoryV)).ToList();

            QuoteProductDetailProductAttributeValueEntityModelList.ForEach(item => { Result = Result + item.ProductAttributeCategoryValue1 + ";"; });

            return Result;

        }
        private decimal SumAmount(decimal? quantity, decimal? unitPrice, decimal? exchangeRate, decimal? vat, decimal? discountValue, bool? discountType)
        {
            decimal result = 0;
            decimal calculateVat = 0;
            decimal calculateDiscount = 0;

            if (quantity != null && unitPrice != null)
            {
                if (discountValue != null)
                {
                    if (discountType == true)
                    {
                        calculateDiscount = ((quantity.Value * unitPrice.Value * (exchangeRate ?? 1) * discountValue.Value) / 100);
                    }
                    else
                    {
                        calculateDiscount = discountValue.Value;
                    }
                }
                if (vat != null)
                {
                    calculateVat = ((quantity.Value * unitPrice.Value * (exchangeRate ?? 1) - calculateDiscount) * vat.Value) / 100;
                }
                result = (quantity.Value * unitPrice.Value * (exchangeRate ?? 1)) + calculateVat - calculateDiscount;
            }
            return result;
        }

        private string GenerateoContractCode(Guid? typeContractId)
        {
            var typeContractCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LHD")?.CategoryTypeId;
            var typeContract = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeContractCategoryTypeId && c.CategoryId == typeContractId) ?? new Category();
            // sửa định dạng gen code thành "BG-yyMMdd + 4 số"
            var todayQuotes = context.Contract.Where(w => w.CreatedDate.Date == DateTime.Now.Date)
                                                .OrderByDescending(w => w.CreatedDate)
                                                .ToList();

            var count = todayQuotes.Count() == 0 ? 0 : todayQuotes.Count();
            string currentYear = DateTime.Now.Year.ToString();
            var temp = string.Empty;
            switch (typeContract.CategoryCode)
            {
                case "HDKT":
                    temp = "HĐKT-";
                    break;
                case "HDNT":
                    temp = "HĐNT-";
                    break;
                case "PLHD":
                    temp = "PLHĐ-";
                    break;
                default:
                    break;
            }
            string result = temp + currentYear.Substring(currentYear.Length - 2) + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + (count + 1).ToString("D4");
            return result;
        }

        public List<ContractDetailProductAttributeEntityModel> getListContractProductDetailProductAttributeValue(Guid ContractDetailId)
        {
            List<ContractDetailProductAttributeEntityModel> listResult = new List<ContractDetailProductAttributeEntityModel>();

            var OrderProductDetailProductAttributeValueModelList = (from OPDPV in context.ContractDetailProductAttribute
                                                                    join ProductAttributeC in context.ProductAttributeCategory on OPDPV.ProductAttributeCategoryId equals ProductAttributeC.ProductAttributeCategoryId
                                                                    join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                    where OPDPV.ContractDetailId == ContractDetailId
                                                                    select (new ContractDetailProductAttributeEntityModel
                                                                    {
                                                                        ContractDetailId = OPDPV.ContractDetailId,
                                                                        ContractDetailProductAttributeId = OPDPV.ContractDetailProductAttributeId,
                                                                        ProductAttributeCategoryId = OPDPV.ProductAttributeCategoryId,
                                                                        ProductId = OPDPV.ProductId,
                                                                        ProductAttributeCategoryValueId = OPDPV.ProductAttributeCategoryValueId,
                                                                        NameProductAttributeCategory = ProductAttributeC.ProductAttributeCategoryName,
                                                                        NameProductAttributeCategoryValue = ProductAttributeCategoryV.ProductAttributeCategoryValue1
                                                                    })).ToList();
            listResult = OrderProductDetailProductAttributeValueModelList;
            return listResult;

        }

        public GetMasterDataSearchContractResult GetMasterDataSearchContract(GetMasterDataSearchContractParameter parameter)
        {
            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
            var listEmployee = context.Employee.ToList(); //Active = false ?
            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

            var commonCustomer = context.Customer.ToList();
            var commonEmployee = context.Employee.ToList();

            // Khách hàng định danh
            var categoryTypeTHA = context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "THA");
            var categoryHDO = context.Category.FirstOrDefault(c => c.Active == true && c.CategoryCode == "HDO" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);

            var listCustomerEntityModel = new List<CustomerEntityModel>();
            var listEmployeeEntityModel = new List<EmployeeEntityModel>();
            var listProductEntityModel = new List<ProductEntityModel>();

            // list status
            var statusContractCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THD").CategoryTypeId;
            var listStatus = context.Category.Where(c => c.CategoryTypeId == statusContractCategoryTypeId && c.Active == true).Select(y =>
                new CategoryEntityModel
                {
                    CategoryId = y.CategoryId,
                    CategoryName = y.CategoryName,
                    CategoryCode = y.CategoryCode,
                    IsDefault = y.IsDefauld,
                    CategoryTypeId = Guid.Empty,
                    CreatedById = Guid.Empty,
                    CountCategoryById = 0
                }).ToList();

            // List Type contract
            var typeContractCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LHD").CategoryTypeId;
            var listTypeContract = context.Category.Where(c => c.CategoryTypeId == typeContractCategoryTypeId && c.Active == true).Select(y =>
                new CategoryEntityModel
                {
                    CategoryId = y.CategoryId,
                    CategoryName = y.CategoryName,
                    CategoryCode = y.CategoryCode,
                    IsDefault = y.IsDefauld,
                    CategoryTypeId = Guid.Empty,
                    CreatedById = Guid.Empty,
                    CountCategoryById = 0
                }).ToList();

            // Phân quyền dropdown khách hàng và dropdown nhân viên
            if (employee.IsManager)
            {
                /*
             * Lấy list phòng ban con của user
             * List phòng ban bao gồm: chính phòng ban nó đang thuộc và các phòng ban cấp dưới của nó nếu có
             */
                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                listEmployee = listEmployee
                    .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)).ToList();

                var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                //Lấy list UserId theo list EmployeeId
                var listUser = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value)).ToList();

                var listUserId = listUser.Select(y => y.UserId).ToList();

                listEmployeeEntityModel = commonEmployee.Where(e => listEmployeeId.Contains(e.EmployeeId))
                    .Select(y => new EmployeeEntityModel
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        IsManager = y.IsManager,
                        PositionId = y.PositionId,
                        OrganizationId = y.OrganizationId,
                        Active = y.Active
                    }).OrderBy(z => z.EmployeeName).ToList();

                listCustomerEntityModel = commonCustomer.Where(c => c.StatusId == categoryHDO.CategoryId &&
                    ((c.PersonInChargeId != null && listEmployeeId.Contains(c.PersonInChargeId.Value)) || (listUserId.Contains(c.CreatedById) && c.PersonInChargeId == null)))
                    .Select(y => new CustomerEntityModel
                    {
                        CustomerId = y.CustomerId,
                        CustomerCode = y.CustomerCode,
                        CustomerName = y.CustomerName,
                        CustomerEmail = "",
                        CustomerEmailWork = "",
                        CustomerEmailOther = "",
                        CustomerPhone = "",
                        FullAddress = "",
                        CustomerCompany = "",
                        StatusId = y.StatusId,
                        PersonInChargeId = y.PersonInChargeId,
                        TaxCode = "",
                    }).OrderBy(c => c.CustomerName).ToList();
            }
            else
            {
                listEmployeeEntityModel = commonEmployee.Where(e => e.EmployeeId == employee.EmployeeId)
                    .Select(y => new EmployeeEntityModel
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        IsManager = y.IsManager,
                        PositionId = y.PositionId,
                        OrganizationId = y.OrganizationId,
                        Active = y.Active
                    }).OrderBy(z => z.EmployeeName).ToList();

                listCustomerEntityModel = commonCustomer.Where(c => c.StatusId == categoryHDO.CategoryId &&
                    (c.PersonInChargeId != null && c.PersonInChargeId.Value == employee.EmployeeId) || (c.PersonInChargeId == null && c.CreatedById == user.UserId))
                    .Select(y => new CustomerEntityModel
                    {
                        CustomerId = y.CustomerId,
                        CustomerCode = y.CustomerCode,
                        CustomerName = y.CustomerName,
                        CustomerEmail = "",
                        CustomerEmailWork = "",
                        CustomerEmailOther = "",
                        CustomerPhone = "",
                        FullAddress = "",
                        CustomerCompany = "",
                        StatusId = y.StatusId,
                        PersonInChargeId = y.PersonInChargeId,
                        TaxCode = "",
                    }).OrderBy(c => c.CustomerName).ToList();
            }

            listProductEntityModel = context.Product
                .Select(c => new ProductEntityModel
                {
                    ProductId = c.ProductId,
                    ProductCategoryId = c.ProductCategoryId,
                    ProductName = c.ProductName,
                    ProductCode = c.ProductCode,
                    Active = c.Active
                }).ToList();

            return new GetMasterDataSearchContractResult
            {
                Status = true,
                Message = "",
                ListCutomer = listCustomerEntityModel,
                ListEmployee = listEmployeeEntityModel,
                ListProduct = listProductEntityModel,
                ListStatus = listStatus,
                ListTypeContract = listTypeContract
            };
        }

        public SearchContractResult SearchContract(SearchContractParameter parameter)
        {
            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
            var listEmployee = context.Employee.ToList(); //Active = false ?
            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THD")?.CategoryTypeId ??
                               Guid.Empty;
            var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId).ToList();
            var newStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "MOI").CategoryId;
            var commonContract = new List<Contract>();

            if (employee.IsManager)
            {
                /*
              * Lấy list phòng ban con của user
              * List phòng ban bao gồm: chính phòng ban nó đang thuộc và các phòng ban cấp dưới của nó nếu có
              */
                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                listEmployee = listEmployee
                    .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)).ToList();

                var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                //Lấy list UserId theo list EmployeeId
                var listUser = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value)).ToList();

                var listUserId = listUser.Select(y => y.UserId).ToList();
                commonContract = context.Contract.Where(c =>
                    c.Active == true &&
                    (listEmployeeId.Contains(c.EmployeeId.Value) || listUserId.Contains(c.CreatedById))).ToList();
            }
            else
            {
                commonContract = context.Contract.Where(c =>
                    c.Active == true && (c.EmployeeId == user.EmployeeId || c.CreatedById == user.UserId)).ToList();
            }

            #region comman by longhdh

            //var listContractProductId = context.ContractDetail.Where(c =>
            //        parameter.ListProductId == null || parameter.ListProductId.Count == 0
            //                                        || parameter.ListProductId.Contains(c.ProductId.Value))
            //    .Select(c => c.ContractId)
            //    .ToList();

            //listContractProductId = listContractProductId.Distinct().ToList();

            #endregion

            List<Guid> listQuoteContractId = new List<Guid>();
            if (!String.IsNullOrEmpty(parameter.QuoteCode))
            {
                listQuoteContractId = context.Quote.Where(c =>
                        parameter.QuoteCode == null || parameter.QuoteCode == "" ||
                        c.QuoteCode.ToLower().Contains(parameter.QuoteCode.ToLower()))
                    .Select(c => c.QuoteId).ToList();
            }

            var listContract = commonContract.Where(c =>
                    (parameter.ContractCode == null || parameter.ContractCode == "" ||
                     c.ContractCode.ToLower().Contains(parameter.ContractCode.ToLower())) &&
                    (listQuoteContractId.Count == 0 ||
                     (c.QuoteId != null && listQuoteContractId.Contains(c.QuoteId.Value))) &&
                    (parameter.ListEmployeeId == null || parameter.ListEmployeeId.Count == 0 ||
                     parameter.ListEmployeeId.Contains(c.EmployeeId.Value)) &&
                    (parameter.ListCutomerId == null || parameter.ListCutomerId.Count == 0 ||
                     parameter.ListCutomerId.Contains(c.CustomerId.Value)) &&
                    (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue ||
                     c.EffectiveDate >= parameter.FromDate) &&
                    (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue ||
                     c.EffectiveDate <= parameter.ToDate) &&
                    (c.Active == true))
                .Select(c => new ContractEntityModel
                {
                    ContractId = c.ContractId,
                    ContractCode = c.ContractCode,
                    CustomerId = c.CustomerId,
                    EffectiveDate = c.EffectiveDate,
                    ValueContract = c.ValueContract,
                    EmployeeId = c.EmployeeId,
                    ContractDescription = c.ContractDescription,
                    StatusId = c.StatusId.Value,
                    CreatedDate = c.CreatedDate,
                    CreatedById = c.CreatedById,
                    Active = c.Active,
                    NameCustomer = "",
                    NameStatus = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryName ?? "",
                    StatusCode = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryCode ?? "",
                    NameEmployee = "",
                    NameCreateBy = ""
                }).OrderByDescending(c => c.CreatedDate).ToList();

            listContract.ForEach(item =>
            {
                item.NameCustomer = GetObjectName(item.CustomerId);
                item.NameEmployee = GetObjectName(item.EmployeeId);
                item.NameCreateBy = GetCreateByName(item.CreatedById);
                item.CanDelete = item.StatusId == newStatusId;
            });

            return new SearchContractResult
            {
                Status = true,
                Message = CommonMessage.Contract.SEARCH_SUCCESS,
                ListContract = listContract
            };
        }

        private string GetCreateByName(Guid? createById)
        {
            if (createById != null && createById != Guid.Empty)
            {
                var empId = context.User.FirstOrDefault(u => u.UserId == createById).EmployeeId;

                if (empId != null && empId != Guid.Empty)
                {
                    var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == empId);

                    if (emp != null)
                    {
                        return emp.EmployeeCode + " - " + emp.EmployeeName;
                    }
                }
            }
            return "";
        }

        private string GetObjectName(Guid? objId)
        {
            if (objId != null && objId != Guid.Empty)
            {
                var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == objId);
                var con = context.Contact.FirstOrDefault(c => c.ObjectId == objId);
                var ven = context.Vendor.FirstOrDefault(e => e.VendorId == objId);
                var cus = context.Customer.FirstOrDefault(c => c.CustomerId == objId);

                if (emp != null && con != null)
                {
                    return emp.EmployeeCode + " - " + emp.EmployeeName;
                }

                if (ven != null)
                {
                    return ven.VendorCode + " - " + ven.VendorName;
                }

                if (cus != null)
                {
                    return cus.CustomerCode + " - " + cus.CustomerName;
                }

                return "";
            }

            return "";
        }

        public ChangeContractStatusResult ChangeContractStatus(ChangeContractStatusParameter parameter)
        {
            try
            {
                var statusContractTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THD")?.CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusContractTypeId);

                var newStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "MOI").CategoryId;
                var confirmStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "APPR").CategoryId;
                var destroyStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "HUY").CategoryId;
                var workingStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "DTH").CategoryId;
                var compleStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "HTH").CategoryId;
                var completeStatusOrderId = context.OrderStatus.FirstOrDefault(c => c.OrderStatusCode == "COMP").OrderStatusId;

                var contract = context.Contract.FirstOrDefault(f => f.ContractId == parameter.ContractId);

                /// đóng hợp đồng
                if (parameter.StatusId == compleStatusId)
                {
                    var isChildren = context.Contract.Where(c => c.MainContractId == parameter.ContractId).ToList();
                    var isOrder = context.CustomerOrder.Where(c => c.OrderContractId == parameter.ContractId).ToList();

                    var listContractChild = isChildren.Where(c => c.StatusId != compleStatusId).ToList() ?? new List<Contract>();
                    var listCustomerOrder = isOrder.Where(c => c.StatusId != completeStatusOrderId).ToList() ?? new List<CustomerOrder>();
                    // kiểm tra các hợp đồng con có trạng thái khác đóng
                    if (listContractChild.Count > 0 || listCustomerOrder.Count > 0)
                    {
                        return new ChangeContractStatusResult
                        {
                            Status = false,
                        };
                    }

                    contract.StatusId = parameter.StatusId;
                    context.Contract.Update(contract);
                    context.SaveChanges();
                    return new ChangeContractStatusResult
                    {
                        Status = true,
                    };
                }
                else
                {
                    // update trạng thái bình thường
                    contract.StatusId = parameter.StatusId;
                    contract.UpdatedDate = DateTime.Now;
                    context.Contract.Update(contract);
                    context.SaveChanges();

                    var mainContract = context.Contract.FirstOrDefault(c => c.ContractId == contract.MainContractId) ?? new Contract();
                    var listContract = context.Contract.Where(c => c.MainContractId == mainContract.ContractId).ToList() ?? new List<Contract>();

                    // nếu chí có 1 hợp đồng con thì trạng thái cha sẽ thay đổi theo trạng thái con
                    if (listContract.Count == 1)
                    {
                        if (contract.StatusId == confirmStatusId)
                        {
                            if (mainContract != null)
                            {
                                mainContract.StatusId = workingStatusId;
                                context.Contract.Update(mainContract);
                            }
                        }
                        else if (contract.StatusId == destroyStatusId)
                        {
                            if (mainContract != null)
                            {
                                mainContract.StatusId = confirmStatusId;
                                context.Contract.Update(mainContract);
                            }
                        }
                    } // có nhiều hợp đồng con
                    else if (listContract.Count > 1)
                    {
                        var isCheckWorking = true;
                        var isCheckCompleteChild = true;
                        foreach (var item in listContract)
                        {
                            // kiểm tra xem có hợp đồng con khác trạng thái mới hủy đóng
                            if (item.StatusId != newStatusId && item.StatusId != destroyStatusId && item.StatusId != compleStatusId)
                            {
                                isCheckWorking = false;
                                break;
                            }
                            // kiểm trả xem toàn bộ list hợp đồng con chỉ là trạng thái mới và hủy
                            else if (item.StatusId == newStatusId || item.StatusId == destroyStatusId)
                            {
                                isCheckCompleteChild = false;
                            }
                            else
                            {
                                isCheckCompleteChild = true;
                            }
                        }
                        // nếu có có 1 hợp đồng con khác trạng thái mới hủy đóng, update hợp đồng cha lên đang thực hiện return chương trình
                        if (!isCheckWorking)
                        {
                            mainContract.StatusId = workingStatusId;
                            context.Contract.Update(mainContract);

                            return new ChangeContractStatusResult
                            {
                                Status = true,
                                Message = "Success"
                            };
                        }
                        // nếu hợp đồng con chỉ có trạng thái mới, hủy, đóng. check nếu chỉ toàn trạng thái mới và hủy thì return hợp đông cha về xác nh
                        if (!isCheckCompleteChild)
                        {
                            mainContract.StatusId = confirmStatusId;
                            context.Contract.Update(mainContract);
                        }
                    }
                }

                context.SaveChanges();
                return new ChangeContractStatusResult
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new ChangeContractStatusResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataDashBoardResult GetMasterDataDashBoard(GetMasterDataDashBoardParameter parameter)
        {

            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
            var listEmployee = context.Employee.ToList(); //Active = false ?
            var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THD")?.CategoryTypeId ?? Guid.Empty;
            var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId).ToList();
            var commonContract = new List<Contract>();
            var workingStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "DTH")?.CategoryId;
            var completeStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "HTH")?.CategoryId;
            if (employee.IsManager)
            {
                /*
              * Lấy list phòng ban con của user
              * List phòng ban bao gồm: chính phòng ban nó đang thuộc và các phòng ban cấp dưới của nó nếu có
              */
                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                listEmployee = listEmployee
                    .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)).ToList();

                var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                //Lấy list UserId theo list EmployeeId
                var listUser = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value)).ToList();

                var listUserId = listUser.Select(y => y.UserId).ToList();
                commonContract = context.Contract.Where(c => c.Active == true && (listEmployeeId.Contains(c.EmployeeId.Value) || listUserId.Contains(c.CreatedById))).ToList();
            }
            else
            {
                commonContract = context.Contract.Where(c => c.Active == true && (c.EmployeeId == user.EmployeeId || c.CreatedById == user.UserId)).ToList();
            }

            // lấy danh sách của những hợp đồng mới nhất
            var listContractNewStatus = commonContract.Where(m => parameter.ContractCode == null || parameter.ContractCode == "" || m.ContractCode.Contains(parameter.ContractCode))
                .Select(c => new ContractEntityModel
                {
                    ContractId = c.ContractId,
                    ContractCode = c.ContractCode,
                    CustomerId = c.CustomerId,
                    EffectiveDate = c.EffectiveDate,
                    ValueContract = c.ValueContract,
                    EmployeeId = c.EmployeeId,
                    ContractDescription = c.ContractDescription,
                    StatusId = c.StatusId.Value,
                    CreatedDate = c.CreatedDate,
                    CreatedById = c.CreatedById,
                    Active = c.Active,
                    NameCustomer = "",
                    NameStatus = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryName ?? "",
                    StatusCode = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryCode ?? "",
                    NameEmployee = "",
                    NameCreateBy = ""
                }).OrderByDescending(c => c.CreatedDate).Take(5).ToList();

            listContractNewStatus.ForEach(item =>
            {
                item.NameCustomer = GetObjectName(item.CustomerId);
                item.NameEmployee = GetObjectName(item.EmployeeId);
                item.NameCreateBy = GetCreateByName(item.CreatedById);
            });

            var listContractWorking = commonContract.Where(m => m.StatusId == workingStatusId &&
                (parameter.ContractCode == null || parameter.ContractCode == "" || m.ContractCode.Contains(parameter.ContractCode)))
                .Select(c => new ContractEntityModel
                {
                    ContractId = c.ContractId,
                    ContractCode = c.ContractCode,
                    CustomerId = c.CustomerId,
                    EffectiveDate = c.EffectiveDate,
                    ValueContract = c.ValueContract,
                    EmployeeId = c.EmployeeId,
                    ContractDescription = c.ContractDescription,
                    StatusId = c.StatusId.Value,
                    CreatedDate = c.CreatedDate,
                    CreatedById = c.CreatedById,
                    Active = c.Active,
                    NameCustomer = "",
                    NameStatus = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryName ?? "",
                    StatusCode = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryCode ?? "",
                    NameEmployee = "",
                    NameCreateBy = ""
                }).OrderByDescending(c => c.UpdatedDate).Take(5).ToList();

            listContractWorking.ForEach(item =>
            {
                item.NameCustomer = GetObjectName(item.CustomerId);
                item.NameEmployee = GetObjectName(item.EmployeeId);
                item.NameCreateBy = GetCreateByName(item.CreatedById);
            });

            #region Lấy list danh sách sắp hết hạn

            var _today = DateTime.Today;
            
            var listContractExpiredDate = commonContract.Where(m => m.Active && !m.IsExtend && (m.ExpiredDate != null && 0 <= (m.ExpiredDate.Value - _today).TotalDays && (m.ExpiredDate.Value - _today).TotalDays <= 30))
                .Select(c => new ContractEntityModel
                {
                    ContractId = c.ContractId,
                    ContractCode = c.ContractCode,
                    CustomerId = c.CustomerId,
                    EffectiveDate = c.EffectiveDate,
                    ContractTime = c.ContractTime,
                    ContractTimeUnit = c.ContractTimeUnit,
                    ExpiredDate = c.ExpiredDate,
                    DayLeft = (int) (c.ExpiredDate.Value - _today).TotalDays,
                    ValueContract = c.ValueContract,
                    EmployeeId = c.EmployeeId,
                    ContractDescription = c.ContractDescription,
                    StatusId = c.StatusId.Value,
                    CreatedDate = c.CreatedDate,
                    CreatedById = c.CreatedById,
                    Active = c.Active,
                    NameCustomer = "",
                    NameStatus = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryName ?? "",
                    StatusCode = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryCode ?? "",
                    NameEmployee = "",
                    NameCreateBy = ""
                }).OrderBy(c => c.ExpiredDate).ToList();

            listContractExpiredDate.ForEach(item =>
            {
                item.NameCustomer = GetObjectName(item.CustomerId);
                item.NameEmployee = GetObjectName(item.EmployeeId);
                item.NameCreateBy = GetCreateByName(item.CreatedById);
            });

            #endregion

            var listValueContract = new List<ContractDashboardEntityModel>();
            var listValueContractFollowMonth = new List<ContractDashboardEntityModel>();
            //var listCommonContractFollowMonth = new List<ContractEntityModel>();

            var listCommonContract = commonContract.Select(c => new ContractEntityModel
            {
                ContractId = c.ContractId,
                ContractCode = c.ContractCode,
                EffectiveDate = c.EffectiveDate,
                ValueContract = c.ValueContract,
                StatusId = c.StatusId.Value,
                NameStatus = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryName ?? "",
                StatusCode = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryCode ?? "",
            }).ToList();

            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1).Date;
            for (int i = 0; i < parameter.NumberMonth; i++)
            {
                var listContractInMonth = commonContract.Where(c => GetYearAndMonth(c.EffectiveDate) == month &&
                                                                (c.StatusId == workingStatusId || c.StatusId == completeStatusId))
                    .Select(c => new ContractEntityModel
                    {
                        ContractId = c.ContractId,
                        ContractCode = c.ContractCode,
                        EffectiveDate = c.EffectiveDate,
                        ValueContract = c.ValueContract,
                        StatusId = c.StatusId.Value,
                        NameStatus = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryName ?? "",
                        StatusCode = listAllStatus.FirstOrDefault(ca => ca.CategoryId == c.StatusId).CategoryCode ?? "",
                    }).ToList();
                var sumValue = listContractInMonth.Sum(c => c.ValueContract);

                var valueContractOfMonth = new ContractDashboardEntityModel
                {
                    Code = "",
                    Name = month.Month.ToString(),
                    Value = sumValue.Value.ToString(),
                };
                listValueContractFollowMonth.Add(valueContractOfMonth);
                month = month.AddMonths(-1);
            }

            // list giá trị của contract theo trạng thái
            listValueContract = listCommonContract.GroupBy(c => c.StatusId)
                .Select(cl => new ContractDashboardEntityModel
                {
                    Code = cl.First().StatusCode,
                    Name = cl.First().NameStatus,
                    Value = cl.Sum(c => c.ValueContract).ToString()
                }).ToList();

            // list giá tị của contract theo tháng
            return new GetMasterDataDashBoardResult
            {
                Status = true,
                Message = "success",
                ListContractNew = listContractNewStatus,
                ListContractWorking = listContractWorking,
                ListContractExpiredDate = listContractExpiredDate,
                ListValueOfStatus = listValueContract,
                ListValueOfMonth = listValueContractFollowMonth
            };
        }

        private List<Guid?> getOrganizationChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Organization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(item.OrganizationId, list);
            });

            return list;
        }

        public DeleteContractResult DeleteContract(DeleteContractParamter paramter)
        {
            try
            {
                var contract = context.Contract.FirstOrDefault(c => c.ContractId == paramter.ContractId);
                if (contract == null)
                {
                    return new DeleteContractResult
                    {
                        Status = false,
                        Message = CommonMessage.Contract.DELETE_FAIL
                    };
                }
                var contractCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THD");
                var listContracttatus = context.Category.Where(x => x.CategoryTypeId == contractCategoryType.CategoryTypeId).ToList();
                var newStatusContract = listContracttatus.FirstOrDefault(c => c.CategoryCode == "MOI")?.CategoryId;
                if (contract.StatusId == newStatusContract)
                {
                    contract.Active = false;
                    contract.UpdatedDate = DateTime.Now;
                    context.Contract.Update(contract);
                    context.SaveChanges();
                }

                return new DeleteContractResult
                {
                    Status = true,
                    Message = CommonMessage.Contract.DELETE_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new DeleteContractResult
                {
                    Status = false,
                    Message = ex.Message
                };


            }
        }

        public DateTime? GetYearAndMonth(DateTime? time)
        {
            if (time == null) return null;
            return new DateTime(time.Value.Year, time.Value.Month, 1).Date;
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
