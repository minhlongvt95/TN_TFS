using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;
using TN.TNM.DataAccess.Messages.Results.Promotion;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.LinkOfDocument;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class PromotionDAO : BaseDAO, IPromotionDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public PromotionDAO(Databases.TNTN8Context _content, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            _hostingEnvironment = hostingEnvironment;
        }

        public GetMasterDataCreatePromotionResult GetMasterDataCreatePromotion(GetMasterDataCreatePromotionParameter parameter)
        {
            try
            {
                var listProduct = new List<ProductEntityModel>();

                listProduct = context.Product.Where(x => x.Active == true).Select(y =>
                    new ProductEntityModel
                    {
                        ProductId = y.ProductId,
                        ProductCode = y.ProductCode,
                        ProductName = y.ProductName,
                        ProductCodeName = y.ProductCode.Trim() + " - " + y.ProductName.Trim()
                    }).OrderBy(z => z.ProductName).ToList();

                var customerGroupType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA");
                var listCustomerGroup = context.Category
                    .Where(x => x.CategoryTypeId == customerGroupType.CategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName
                        }).ToList();

                return new GetMasterDataCreatePromotionResult()
                {
                    Status = true,
                    Message = "Success",
                    ListProduct = listProduct,
                    ListCustomerGroup = listCustomerGroup
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataCreatePromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreatePromotionResult CreatePromotion(CreatePromotionParameter parameter)
        {
            try
            {
                var Code = "";
                var datenow = DateTime.Now;
                string year = datenow.Year.ToString().Substring(datenow.Year.ToString().Length - 2, 2);
                string month = datenow.Month < 10 ? "0" + datenow.Month.ToString() : datenow.Month.ToString();
                string day = datenow.Day < 10 ? "0" + datenow.Day.ToString() : datenow.Day.ToString();

                var listCodeToDay = context.Promotion.Where(c =>
                    Convert.ToDateTime(c.CreatedDate).Day == datenow.Day &&
                    Convert.ToDateTime(c.CreatedDate).Month == datenow.Month &&
                    Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Select(y => new
                {
                    PromotionCode = y.PromotionCode
                    }).ToList();

                if (listCodeToDay.Count == 0)
                {
                    Code = "CTKM-" + year + month + day + "0001";
                }
                else
                {
                    var listNumber = new List<int>();
                    listCodeToDay.ForEach(item =>
                    {
                        var stringNumber = item.PromotionCode.Substring(11);
                        var number = Int32.Parse(stringNumber);
                        listNumber.Add(number);
                    });

                    var maxNumber = listNumber.OrderByDescending(x => x).FirstOrDefault();
                    var newNumber = maxNumber + 1;

                    if (newNumber > 9999)
                    {
                        Code = "CTKM-" + year + month + day + newNumber;
                    }
                    else
                    {
                        Code = "CTKM-" + year + month + day + newNumber.ToString("D4");
                    }
                }

                parameter.Promotion.PromotionCode = Code;

                var existsCode =
                    context.Promotion.FirstOrDefault(x => x.PromotionCode == parameter.Promotion.PromotionCode.Trim());

                if (existsCode != null)
                {
                    return new CreatePromotionResult()
                    {
                        Status = false,
                        Message = "Mã chương trình đã tồn tại"
                    };
                }

                var promotion = new Promotion();
                promotion.PromotionId = Guid.NewGuid();
                promotion.PromotionCode = parameter.Promotion.PromotionCode.Trim();
                promotion.PromotionName = parameter.Promotion.PromotionName.Trim();
                promotion.EffectiveDate = parameter.Promotion.EffectiveDate;
                promotion.ExpirationDate = parameter.Promotion.ExpirationDate;
                promotion.Description = parameter.Promotion.Description;
                promotion.ConditionsType = parameter.Promotion.ConditionsType;
                promotion.PropertyType = parameter.Promotion.PropertyType;
                promotion.FilterContent = parameter.Promotion.FilterContent;
                promotion.FilterQuery = parameter.Promotion.FilterQuery;
                promotion.NotMultiplition = parameter.Promotion.NotMultiplition;
                promotion.CustomerHasOrder = parameter.Promotion.CustomerHasOrder;
                promotion.Active = parameter.Promotion.Active;
                promotion.CreatedDate = DateTime.Now;
                promotion.CreatedById = parameter.UserId;

                var listPromotionMapping = new List<PromotionMapping>();
                var listPromotionProductMapping = new List<PromotionProductMapping>();
                parameter.ListPromotionMapping.ForEach(item =>
                {
                    var promotionMapping = new PromotionMapping();
                    promotionMapping.PromotionMappingId = Guid.NewGuid();
                    promotionMapping.PromotionId = promotion.PromotionId;
                    promotionMapping.IndexOrder = item.IndexOrder;
                    promotionMapping.HangKhuyenMai = item.HangKhuyenMai;
                    promotionMapping.SoLuongTang = item.SoLuongTang;
                    promotionMapping.SoTienTu = item.SoTienTu;
                    promotionMapping.SanPhamMua = item.SanPhamMua;
                    promotionMapping.SoLuongMua = item.SoLuongMua;
                    promotionMapping.ChiChonMot = item.ChiChonMot;
                    promotionMapping.LoaiGiaTri = item.LoaiGiaTri;
                    promotionMapping.GiaTri = item.GiaTri;

                    item.ListPromotionProductMapping.ForEach(product =>
                    {
                        var newProduct = new PromotionProductMapping();
                        newProduct.PromotionProductMappingId = Guid.NewGuid();
                        newProduct.PromotionMappingId = promotionMapping.PromotionMappingId;
                        newProduct.ProductId = product.ProductId;

                        listPromotionProductMapping.Add(newProduct);
                    });

                    listPromotionMapping.Add(promotionMapping);
                });

                context.Promotion.Add(promotion);
                context.PromotionMapping.AddRange(listPromotionMapping);
                context.PromotionProductMapping.AddRange(listPromotionProductMapping);
                context.SaveChanges();

                return new CreatePromotionResult()
                {
                    Status = true,
                    Message = "Success",
                    PromotionId = promotion.PromotionId
                };
            }
            catch (Exception e)
            {
                return new CreatePromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataListPromotionResult GetMasterDataListPromotion(GetMasterDataListPromotionParameter parameter)
        {
            try
            {



                return new GetMasterDataListPromotionResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataListPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchListPromotionResult SearchListPromotion(SearchListPromotionParameter parameter)
        {
            try
            {
                var listPromotion = new List<PromotionEntityModel>();

                listPromotion = context.Promotion
                    .Where(x => (parameter.PromotionCode == null || parameter.PromotionCode == "" ||
                                 x.PromotionCode.Contains(parameter.PromotionCode)) &&
                                (parameter.PromotionName == null || parameter.PromotionName == "" ||
                                 x.PromotionName.Contains(parameter.PromotionName))
                                ).Select(y =>
                        new PromotionEntityModel
                        {
                            PromotionId = y.PromotionId,
                            PromotionCode = y.PromotionCode,
                            PromotionName = y.PromotionName.Trim(),
                            ExpirationDate = y.ExpirationDate,
                            Active = y.Active,
                            QuantityQuote = 0,
                            QuantityOrder = 0,
                            QuantityContract = 0
                        }).OrderByDescending(z => z.ExpirationDate).ToList();

                // search list chương trình khuyến mãi theo ngày hết hạn
                if (parameter.ExpirationDateFrom != null && parameter.ExpirationDateTo != null)
                {
                    listPromotion = listPromotion.Where(x =>
                        parameter.ExpirationDateFrom.Value.Date <= x.ExpirationDate.Date
                        && x.ExpirationDate.Date <= parameter.ExpirationDateTo.Value.Date).ToList();
                }
                else if (parameter.ExpirationDateFrom != null && parameter.ExpirationDateTo == null)
                {
                    listPromotion = listPromotion.Where(x =>
                        parameter.ExpirationDateFrom.Value.Date <= x.ExpirationDate.Date).ToList();
                }
                else if (parameter.ExpirationDateFrom == null && parameter.ExpirationDateTo != null)
                {
                    listPromotion = listPromotion.Where(x =>
                        x.ExpirationDate.Date <= parameter.ExpirationDateTo.Value.Date).ToList();
                }

                // search list chương trình khuyến mãi theo số lượng báo giá
                if (parameter.QuantityQuote != null)
                {
                    listPromotion = listPromotion.Where(x => x.QuantityQuote == parameter.QuantityQuote).ToList();
                }

                return new SearchListPromotionResult()
                {
                    Status = true,
                    Message = "Success",
                    ListPromotion = listPromotion
                };
            }
            catch (Exception e)
            {
                return new SearchListPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataDetailPromotionResult GetMasterDataDetailPromotion(GetMasterDataDetailPromotionParameter parameter)
        {
            try
            {
                var listProduct = new List<ProductEntityModel>();

                listProduct = context.Product.Where(x => x.Active == true).Select(y =>
                    new ProductEntityModel
                    {
                        ProductId = y.ProductId,
                        ProductCode = y.ProductCode,
                        ProductName = y.ProductName,
                        ProductCodeName = y.ProductCode.Trim() + " - " + y.ProductName.Trim()
                    }).OrderBy(z => z.ProductName).ToList();

                var customerGroupType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA");
                var listCustomerGroup = context.Category
                    .Where(x => x.CategoryTypeId == customerGroupType.CategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName
                        }).ToList();

                return new GetMasterDataDetailPromotionResult()
                {
                    Status = true,
                    Message = "Success",
                    ListProduct = listProduct,
                    ListCustomerGroup = listCustomerGroup
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataDetailPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDetailPromotionResult GetDetailPromotion(GetDetailPromotionParameter parameter)
        {
            try
            {
                var promotion = context.Promotion.Where(x => x.PromotionId == parameter.PromotionId).Select(y =>
                    new PromotionEntityModel
                    {
                        PromotionId = y.PromotionId,
                        PromotionCode = y.PromotionCode,
                        PromotionName = y.PromotionName,
                        EffectiveDate = y.EffectiveDate,
                        ExpirationDate = y.ExpirationDate,
                        Description = y.Description,
                        Active = y.Active,
                        ConditionsType = y.ConditionsType,
                        PropertyType = y.PropertyType,
                        NotMultiplition = y.NotMultiplition,
                        FilterContent = y.FilterContent,
                        FilterQuery = y.FilterQuery,
                        CustomerHasOrder = y.CustomerHasOrder
                    }).FirstOrDefault();

                if (promotion == null)
                {
                    return new GetDetailPromotionResult()
                    {
                        Status = false,
                        Message = "Chương trình khuyến mãi không tồn tại trên hệ thống"
                    };
                }

                var listPromotionMapping = context.PromotionMapping.Where(x => x.PromotionId == parameter.PromotionId)
                    .Select(y => new PromotionMappingEntityModel
                    {
                        PromotionMappingId = y.PromotionMappingId,
                        PromotionId = y.PromotionId,
                        IndexOrder = y.IndexOrder,
                        HangKhuyenMai = y.HangKhuyenMai,
                        SoLuongTang = y.SoLuongTang,
                        SoTienTu = y.SoTienTu,
                        SanPhamMua = y.SanPhamMua,
                        SoLuongMua = y.SoLuongMua,
                        ChiChonMot = y.ChiChonMot,
                        LoaiGiaTri = y.LoaiGiaTri,
                        GiaTri = y.GiaTri,
                        ListPromotionProductMapping = new List<PromotionProductMappingEntityModel>()
                    }).OrderBy(z => z.IndexOrder).ToList();

                var listPromotionMappingId = listPromotionMapping.Select(y => y.PromotionMappingId).ToList();
                var listAllPromotionProductMapping = context.PromotionProductMapping
                    .Where(x => listPromotionMappingId.Contains(x.PromotionMappingId)).ToList();

                if (listAllPromotionProductMapping.Count > 0)
                {
                    listPromotionMapping.ForEach(item =>
                    {
                        item.ListPromotionProductMapping = listAllPromotionProductMapping
                            .Where(x => x.PromotionMappingId == item.PromotionMappingId).Select(y =>
                                new PromotionProductMappingEntityModel
                                {
                                    PromotionProductMappingId = y.PromotionProductMappingId,
                                    PromotionMappingId = y.PromotionMappingId,
                                    ProductId = y.ProductId
                                }).ToList();
                    });
                }

                #region Lấy list link và file

                var listLinkAndFile = new List<LinkAndFileModel>();

                //Lấy list link của Promotion
                var listLink = context.LinkOfDocument
                    .Where(x => x.ObjectId == parameter.PromotionId && x.ObjectType == "CSKH-CTKM")
                    .Select(y => new LinkAndFileModel
                    {
                        LinkOfDocumentId = y.LinkOfDocumentId,
                        LinkName = y.LinkName,
                        LinkValue = y.LinkValue,
                        ObjectType = y.ObjectType,
                        ObjectId = y.ObjectId,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        Type = false,
                        Name = y.LinkName
                    }).ToList();

                //Lấy list file của Promotion
                var listFile = context.FileInFolder
                    .Where(x => x.ObjectId == parameter.PromotionId && x.ObjectType == "CSKH-CTKM").Select(y =>
                        new LinkAndFileModel
                        {
                            FileInFolderId = y.FileInFolderId,
                            FolderId = y.FolderId,
                            FileName = y.FileName,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Size = y.Size,
                            FileExtension = y.FileExtension,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            Type = true,
                            Name = FormatFileName(y.FileName, y.FileExtension)
                        }).ToList();

                listLinkAndFile.AddRange(listLink);
                listLinkAndFile.AddRange(listFile);
                listLinkAndFile = listLinkAndFile.OrderBy(z => z.CreatedDate).ToList();

                //Lấy tên người tạo
                var listUserId = listLinkAndFile.Select(y => y.CreatedById).Distinct().ToList();
                var listUser = context.User.Where(x => listUserId.Contains(x.UserId)).ToList();
                var listEmployeeId = context.User.Where(x => listUserId.Contains(x.UserId)).Select(y => y.EmployeeId)
                    .Distinct().ToList();
                var listEmployee = context.Employee.Where(x => listEmployeeId.Contains(x.EmployeeId)).ToList();

                listLinkAndFile.ForEach(item =>
                {
                    var user = listUser.FirstOrDefault(x => x.CreatedById == item.CreatedById);
                    if (user != null)
                    {
                        var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                        if (employee != null)
                        {
                            item.CreatedName = employee.EmployeeName;
                        }
                    }
                });

                #endregion

                #region Lấy list ghi chú

                var noteHistory = new List<NoteEntityModel>();

                noteHistory = context.Note
                    .Where(x => x.ObjectId == parameter.PromotionId && x.ObjectType == "PROMOTION" &&
                                x.Active == true).Select(
                        y => new NoteEntityModel
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

                if (noteHistory.Count > 0)
                {
                    var listNoteId = noteHistory.Select(x => x.NoteId).ToList();
                    var listAllUser = context.User.ToList();
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
                        }).ToList();

                    noteHistory.ForEach(item =>
                    {
                        var _user = listAllUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                            .OrderByDescending(z => z.UpdatedDate).ToList();
                    });

                    //Sắp xếp lại listNote
                    noteHistory = noteHistory.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                return new GetDetailPromotionResult()
                {
                    Status = true,
                    Message = "Success",
                    Promotion = promotion,
                    ListPromotionMapping = listPromotionMapping,
                    ListLinkAndFile = listLinkAndFile,
                    NoteHistory = noteHistory
                };
            }
            catch (Exception e)
            {
                return new GetDetailPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DeletePromotionResult DeletePromotion(DeletePromotionParameter parameter)
        {
            try
            {
                var promotion = context.Promotion.FirstOrDefault(x => x.PromotionId == parameter.PromotionId);

                if (promotion == null)
                {
                    return new DeletePromotionResult()
                    {
                        Status = false,
                        Message = "Chương trình khuyến mãi không tồn tại trên hệ thống"
                    };
                }

                var listPromotionMapping =
                    context.PromotionMapping.Where(x => x.PromotionId == parameter.PromotionId).ToList();

                var listPromotionMappingId = listPromotionMapping.Select(y => y.PromotionMappingId).ToList();

                var listPromotionProductMapping = context.PromotionProductMapping
                    .Where(x => listPromotionMappingId.Contains(x.PromotionMappingId)).ToList();

                var listNote = context.Note
                    .Where(x => x.ObjectId == parameter.PromotionId && x.ObjectType == "PROMOTION").ToList();

                var listFile = context.FileInFolder
                    .Where(x => x.ObjectId == parameter.PromotionId && x.ObjectType == "CSKH-CTKM").ToList();

                var listLink = context.LinkOfDocument
                    .Where(x => x.ObjectId == parameter.PromotionId && x.ObjectType == "CSKH-CTKM").ToList();

                #region Xóa file vật lý

                if (listFile.Count > 0)
                {
                    var folder = context.Folder.FirstOrDefault(x => x.FolderType == "CSKH");
                    string folderName = ConvertFolderUrl(folder.Url);
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);

                    listFile.ForEach(file =>
                    {
                        string fileName = file.FileName + "." + file.FileExtension;
                        string fullPath = Path.Combine(newPath, fileName);
                        File.Delete(fullPath);
                    });
                }

                #endregion

                context.Promotion.Remove(promotion);
                context.PromotionMapping.RemoveRange(listPromotionMapping);
                context.PromotionProductMapping.RemoveRange(listPromotionProductMapping);
                context.Note.RemoveRange(listNote);
                context.FileInFolder.RemoveRange(listFile);
                context.LinkOfDocument.RemoveRange(listLink);
                context.SaveChanges();

                return new DeletePromotionResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DeletePromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public UpdatePromotionResult UpdatePromotion(UpdatePromotionParameter parameter)
        {
            try
            {
                var promotion = context.Promotion.FirstOrDefault(x => x.PromotionId == parameter.Promotion.PromotionId);

                if (promotion == null)
                {
                    return new UpdatePromotionResult()
                    {
                        Status = false,
                        Message = "Chương trình khuyến mãi không tồn tại trên hệ thống"
                    };
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    promotion.PromotionName = parameter.Promotion.PromotionName;
                    promotion.EffectiveDate = parameter.Promotion.EffectiveDate;
                    promotion.ExpirationDate = parameter.Promotion.ExpirationDate;
                    promotion.Description = parameter.Promotion.Description;
                    promotion.Active = parameter.Promotion.Active;
                    promotion.ConditionsType = parameter.Promotion.ConditionsType;
                    promotion.PropertyType = parameter.Promotion.PropertyType;
                    promotion.FilterContent = parameter.Promotion.FilterContent;
                    promotion.FilterQuery = parameter.Promotion.FilterQuery;
                    promotion.NotMultiplition = parameter.Promotion.NotMultiplition;
                    promotion.CustomerHasOrder = parameter.Promotion.CustomerHasOrder;

                    context.Update(promotion);
                    context.SaveChanges();

                    var listOldPromotionMapping = context.PromotionMapping
                        .Where(x => x.PromotionId == parameter.Promotion.PromotionId).ToList();
                    var listOldPromotionMappingId = listOldPromotionMapping.Select(y => y.PromotionMappingId).ToList();
                    var listOldPromotionProductMapping = context.PromotionProductMapping
                        .Where(x => listOldPromotionMappingId.Contains(x.PromotionMappingId)).ToList();

                    context.PromotionMapping.RemoveRange(listOldPromotionMapping);
                    context.PromotionProductMapping.RemoveRange(listOldPromotionProductMapping);
                    context.SaveChanges();

                    var listPromotionMapping = new List<PromotionMapping>();
                    var listPromotionProductMapping = new List<PromotionProductMapping>();
                    parameter.ListPromotionMapping.ForEach(item =>
                    {
                        var promotionMapping = new PromotionMapping();
                        promotionMapping.PromotionMappingId = Guid.NewGuid();
                        promotionMapping.PromotionId = promotion.PromotionId;
                        promotionMapping.IndexOrder = item.IndexOrder;
                        promotionMapping.HangKhuyenMai = item.HangKhuyenMai;
                        promotionMapping.SoLuongTang = item.SoLuongTang;
                        promotionMapping.SoTienTu = item.SoTienTu;
                        promotionMapping.SanPhamMua = item.SanPhamMua;
                        promotionMapping.SoLuongMua = item.SoLuongMua;
                        promotionMapping.ChiChonMot = item.ChiChonMot;
                        promotionMapping.LoaiGiaTri = item.LoaiGiaTri;
                        promotionMapping.GiaTri = item.GiaTri;

                        item.ListPromotionProductMapping.ForEach(product =>
                        {
                            var newProduct = new PromotionProductMapping();
                            newProduct.PromotionProductMappingId = Guid.NewGuid();
                            newProduct.PromotionMappingId = promotionMapping.PromotionMappingId;
                            newProduct.ProductId = product.ProductId;

                            listPromotionProductMapping.Add(newProduct);
                        });

                        listPromotionMapping.Add(promotionMapping);
                    });
                    
                    context.PromotionMapping.AddRange(listPromotionMapping);
                    context.PromotionProductMapping.AddRange(listPromotionProductMapping);
                    context.SaveChanges();

                    transaction.Commit();

                    return new UpdatePromotionResult()
                    {
                        Status = true,
                        Message = "Success"
                    };
                }
            }
            catch (Exception e)
            {
                return new UpdatePromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateLinkForPromotionResult CreateLinkForPromotion(CreateLinkForPromotionParameter parameter)
        {
            try
            {
                var linkOfDocument = new LinkOfDocument();
                linkOfDocument.LinkOfDocumentId = Guid.NewGuid();
                linkOfDocument.LinkName = parameter.LinkOfDocument.LinkName;
                linkOfDocument.LinkValue = parameter.LinkOfDocument.LinkValue;
                linkOfDocument.ObjectType = parameter.LinkOfDocument.ObjectType;
                linkOfDocument.ObjectId = parameter.LinkOfDocument.ObjectId;
                linkOfDocument.Active = true;
                linkOfDocument.CreatedById = parameter.UserId;
                linkOfDocument.CreatedDate = DateTime.Now;

                context.LinkOfDocument.Add(linkOfDocument);
                context.SaveChanges();

                var listLinkAndFile = new List<LinkAndFileModel>();

                //Lấy list link của Promotion
                var listLink = context.LinkOfDocument
                    .Where(x => x.ObjectId == linkOfDocument.ObjectId && x.ObjectType == linkOfDocument.ObjectType)
                    .Select(y => new LinkAndFileModel
                    {
                        LinkOfDocumentId = y.LinkOfDocumentId,
                        LinkName = y.LinkName,
                        LinkValue = y.LinkValue,
                        ObjectType = y.ObjectType,
                        ObjectId = y.ObjectId,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        Type = false,
                        Name = y.LinkName
                    }).ToList();

                //Lấy list file của Promotion
                var listFile = context.FileInFolder
                    .Where(x => x.ObjectId == linkOfDocument.ObjectId && x.ObjectType == "CSKH-CTKM").Select(y =>
                        new LinkAndFileModel
                        {
                            FileInFolderId = y.FileInFolderId,
                            FolderId = y.FolderId,
                            FileName = y.FileName,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Size = y.Size,
                            FileExtension = y.FileExtension,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            Type = true,
                            Name = FormatFileName(y.FileName, y.FileExtension)
                        }).ToList();

                listLinkAndFile.AddRange(listLink);
                listLinkAndFile.AddRange(listFile);
                listLinkAndFile = listLinkAndFile.OrderBy(z => z.CreatedDate).ToList();

                //Lấy tên người tạo
                var listUserId = listLinkAndFile.Select(y => y.CreatedById).Distinct().ToList();
                var listUser = context.User.Where(x => listUserId.Contains(x.UserId)).ToList();
                var listEmployeeId = context.User.Where(x => listUserId.Contains(x.UserId)).Select(y => y.EmployeeId)
                    .Distinct().ToList();
                var listEmployee = context.Employee.Where(x => listEmployeeId.Contains(x.EmployeeId)).ToList();

                listLinkAndFile.ForEach(item =>
                {
                    var user = listUser.FirstOrDefault(x => x.CreatedById == item.CreatedById);
                    if (user != null)
                    {
                        var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                        if (employee != null)
                        {
                            item.CreatedName = employee.EmployeeName;
                        }
                    }
                });

                return new CreateLinkForPromotionResult()
                {
                    Status = true,
                    Message = "Success",
                    ListLinkAndFile = listLinkAndFile
                };
            }
            catch (Exception e)
            {
                return new CreateLinkForPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DeleteLinkFromPromotionResult DeleteLinkFromPromotion(DeleteLinkFromPromotionParameter parameter)
        {
            try
            {
                var link = context.LinkOfDocument.FirstOrDefault(x => x.LinkOfDocumentId == parameter.LinkOfDocumentId);

                if (link == null)
                {
                    return new DeleteLinkFromPromotionResult()
                    {
                        Status = false,
                        Message = "Liên kết không tồn tại trên hệ thống"
                    };
                }

                context.LinkOfDocument.Remove(link);
                context.SaveChanges();

                return new DeleteLinkFromPromotionResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DeleteLinkFromPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateFileForPromotionResult CreateFileForPromotion(CreateFileForPromotionParameter parameter)
        {
            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);

            if (folder == null)
            {
                return new CreateFileForPromotionResult()
                {
                    Status = false,
                    Message = "Thư mục upload không tồn tại"
                };
            }

            var listFileDelete = new List<string>();
            try
            {
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

                        return new CreateFileForPromotionResult()
                        {
                            Status = false,
                            Message = "Bạn phải cấu hình thư mục để lưu"
                        };
                    }
                }

                context.SaveChanges();

                #region Lấy list file và link

                var listLinkAndFile = new List<LinkAndFileModel>();

                //Lấy list link của Promotion
                var listLink = context.LinkOfDocument
                    .Where(x => x.ObjectId == parameter.ObjectId && x.ObjectType == "CSKH-CTKM")
                    .Select(y => new LinkAndFileModel
                    {
                        LinkOfDocumentId = y.LinkOfDocumentId,
                        LinkName = y.LinkName,
                        LinkValue = y.LinkValue,
                        ObjectType = y.ObjectType,
                        ObjectId = y.ObjectId,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        Type = false,
                        Name = y.LinkName
                    }).ToList();

                //Lấy list file của Promotion
                var listFile = context.FileInFolder
                    .Where(x => x.ObjectId == parameter.ObjectId && x.ObjectType == "CSKH-CTKM").Select(y =>
                        new LinkAndFileModel
                        {
                            FileInFolderId = y.FileInFolderId,
                            FolderId = y.FolderId,
                            FileName = y.FileName,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Size = y.Size,
                            FileExtension = y.FileExtension,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            Type = true,
                            Name = FormatFileName(y.FileName, y.FileExtension)
                        }).ToList();

                listLinkAndFile.AddRange(listLink);
                listLinkAndFile.AddRange(listFile);
                listLinkAndFile = listLinkAndFile.OrderBy(z => z.CreatedDate).ToList();

                //Lấy tên người tạo
                var listUserId = listLinkAndFile.Select(y => y.CreatedById).Distinct().ToList();
                var listUser = context.User.Where(x => listUserId.Contains(x.UserId)).ToList();
                var listEmployeeId = context.User.Where(x => listUserId.Contains(x.UserId)).Select(y => y.EmployeeId)
                    .Distinct().ToList();
                var listEmployee = context.Employee.Where(x => listEmployeeId.Contains(x.EmployeeId)).ToList();

                listLinkAndFile.ForEach(item =>
                {
                    var user = listUser.FirstOrDefault(x => x.CreatedById == item.CreatedById);
                    if (user != null)
                    {
                        var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                        if (employee != null)
                        {
                            item.CreatedName = employee.EmployeeName;
                        }
                    }
                });

                #endregion

                return new CreateFileForPromotionResult()
                {
                    Status = true,
                    ListLinkAndFile = listLinkAndFile
                };
            }
            catch (Exception ex)
            {
                listFileDelete.ForEach(item =>
                {
                    Directory.Delete(item);
                });

                return new CreateFileForPromotionResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public DeleteFileFromPromotionResult DeleteFileFromPromotion(DeleteFileFromPromotionParameter parameter)
        {
            try
            {
                var file = context.FileInFolder.FirstOrDefault(x => x.FileInFolderId == parameter.FileInFolderId);

                if (file == null)
                {
                    return new DeleteFileFromPromotionResult()
                    {
                        Status = false,
                        Message = "Tệp không tồn tại trên hệ thống"
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

                return new DeleteFileFromPromotionResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DeleteFileFromPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateNoteForPromotionDetailResult CreateNoteForPromotionDetail(CreateNoteForPromotionDetailParameter parameter)
        {
            try
            {
                var note = new Note();
                note.NoteId = Guid.NewGuid();
                note.Description = parameter.Note.Description;
                note.Type = parameter.Note.Type;
                note.ObjectId = parameter.Note.ObjectId;
                note.ObjectType = parameter.Note.ObjectType;
                note.Active = true;
                note.NoteTitle = parameter.Note.NoteTitle;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;

                context.Note.Add(note);
                context.SaveChanges();

                #region Lấy list ghi chú

                var noteHistory = new List<NoteEntityModel>();

                noteHistory = context.Note
                    .Where(x => x.ObjectId == parameter.Note.ObjectId && x.ObjectType == "PROMOTION" &&
                                x.Active == true).Select(
                        y => new NoteEntityModel
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

                if (noteHistory.Count > 0)
                {
                    var listNoteId = noteHistory.Select(x => x.NoteId).ToList();
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
                        }).ToList();

                    noteHistory.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                            .OrderByDescending(z => z.UpdatedDate).ToList();
                    });

                    //Sắp xếp lại listNote
                    noteHistory = noteHistory.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                return new CreateNoteForPromotionDetailResult()
                {
                    Status = true,
                    Message = "Success",
                    NoteHistory = noteHistory
                };
            }
            catch (Exception e)
            {
                return new CreateNoteForPromotionDetailResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CheckPromotionByCustomerResult CheckPromotionByCustomer(CheckPromotionByCustomerParameter parameter)
        {
            try
            {
                //Khách hàng có nhận được CTKM hay không
                bool isPromotionCustomer = false;
                var tenantId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId).TenantId;

                //Lấy danh sách các chương trình khuyến mãi thỏa mãn
                var listPromotion = context.Promotion.Where(x =>
                    x.Active && x.EffectiveDate <= DateTime.Now && x.ExpirationDate >= DateTime.Now &&
                    x.ConditionsType == 1).ToList();

                if (listPromotion.Count > 0)
                {
                    #region Lấy list khách hàng có phát sinh đơn hàng ở trạng thái Đóng

                    //Trạng thái Đóng của Đơn hàng
                    var statusOrderClose =
                        context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "COMP")?.OrderStatusId;

                    //Lấy tất cả khách hàng đã phát sinh đơn hàng có trạng thái Đóng
                    var listCustomerIdHasOrder = context.CustomerOrder.Where(x => x.Active == true &&
                                                                                x.StatusId == statusOrderClose)
                        .Select(y => y.CustomerId).Distinct().ToList();

                    #endregion

                    var listCustomerId = new List<Guid>();
                    listPromotion.ForEach(item =>
                    {
                        //Nếu có ít nhất 1 CTKM lấy tất cả khách hàng thì
                        if (item.FilterContent == null && item.CustomerHasOrder == false)
                        {
                            isPromotionCustomer = true;
                        }
                        else if (item.FilterContent == null && item.CustomerHasOrder)
                        {
                            if (listCustomerIdHasOrder.Count > 0)
                            {
                                var existsCustomer = listCustomerIdHasOrder.FirstOrDefault(x => x == parameter.CustomerId);

                                if (existsCustomer != Guid.Empty && existsCustomer != null)
                                {
                                    isPromotionCustomer = true;
                                }
                            }
                        }

                        var listCurrentCustomerId = new List<Guid>();
                        //Lọc theo Bộ tiêu chí
                        if (item.FilterQuery != null)
                        {
                            if (item.FilterQuery.IndexOf("CreatedDate =") != -1)
                            {
                                var index_prefix = item.FilterQuery.IndexOf("CreatedDate =");
                                var date_postfix = item.FilterQuery.Substring(index_prefix + 15, 10);

                                var sub_query = item.FilterQuery.Substring(index_prefix, 26);

                                var temp = "datediff(d, 'temp', CreatedDate) = 0";
                                var result_temp = item.FilterQuery.Replace(sub_query, temp.Replace("temp", date_postfix));

                                item.FilterQuery = result_temp;
                            }
                            if (item.FilterQuery.IndexOf("CreatedDate >") != -1)
                            {
                                var index_prefix = item.FilterQuery.IndexOf("CreatedDate >");
                                var date_postfix = item.FilterQuery.Substring(index_prefix + 15, 10);

                                var sub_query = item.FilterQuery.Substring(index_prefix, 26);

                                var temp = "datediff(d, 'temp', CreatedDate) > 0";
                                var result_temp = item.FilterQuery.Replace(sub_query, temp.Replace("temp", date_postfix));

                                item.FilterQuery = result_temp;
                            }
                            if (item.FilterQuery.IndexOf("CreatedDate <") != -1)
                            {
                                var index_prefix = item.FilterQuery.IndexOf("CreatedDate <");
                                var date_postfix = item.FilterQuery.Substring(index_prefix + 15, 10);

                                var sub_query = item.FilterQuery.Substring(index_prefix, 26);

                                var temp = "datediff(d, 'temp', CreatedDate) < 0";
                                var result_temp = item.FilterQuery.Replace(sub_query, temp.Replace("temp", date_postfix));

                                item.FilterQuery = result_temp;
                            }

                            var sqlCondition = item.FilterQuery + " AND TenantId = '" + tenantId.ToString() + "'";

                            using (var command = context.Database.GetDbConnection().CreateCommand())
                            {
                                command.CommandText = "SearchCustomer";
                                DbParameter param1 = command.CreateParameter();
                                param1.ParameterName = "@FilterWhere";
                                param1.DbType = DbType.String;
                                param1.Value = sqlCondition;
                                command.Parameters.Add(param1);

                                command.CommandType = CommandType.StoredProcedure;

                                context.Database.OpenConnection();

                                var dataReader = command.ExecuteReader();

                                while (dataReader.Read())
                                {
                                    listCurrentCustomerId.Add(dataReader.GetGuid(0));
                                }

                                context.Database.CloseConnection();
                            }
                        }

                        if (listCurrentCustomerId.Count > 0)
                        {
                            //Nếu có điều kiện lấy những Khách hàng đã phát sinh đơn hàng
                            if (item.CustomerHasOrder)
                            {
                                var _listCurrentCustomerId = listCurrentCustomerId
                                    .Where(x => listCustomerIdHasOrder.Contains(x)).ToList();

                                if (_listCurrentCustomerId.Count > 0)
                                {
                                    listCustomerId.AddRange(_listCurrentCustomerId);
                                }
                            }
                            else
                            {
                                listCustomerId.AddRange(listCurrentCustomerId);
                            }
                        }
                    });

                    if (isPromotionCustomer != true)
                    {
                        if (listCustomerId.Count > 0)
                        {
                            var existsCustomer = listCustomerId.FirstOrDefault(x => x == parameter.CustomerId);

                            if (existsCustomer != Guid.Empty)
                            {
                                isPromotionCustomer = true;
                            }
                        }
                    }
                }

                return new CheckPromotionByCustomerResult()
                {
                    Status = true,
                    Message = "Success",
                    IsPromotionCustomer = isPromotionCustomer
                };
            }
            catch (Exception e)
            {
                return new CheckPromotionByCustomerResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetApplyPromotionResult GetApplyPromotion(GetApplyPromotionParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var tenantId = user.TenantId;
                var listAllProduct = context.Product.ToList();

                var listPromotionApply = new List<PromotionApplyModel>();
                var listPromotionId = new List<Guid>();
                var listPromotion = new List<Promotion>();

                var productUnitTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH")
                        .CategoryTypeId;
                var listProductUnit = context.Category.Where(x => x.CategoryTypeId == productUnitTypeId)
                    .ToList();

                if (parameter.ConditionsType == 1)
                {
                    listPromotion = context.Promotion.Where(x =>
                    x.Active && x.EffectiveDate <= DateTime.Now && x.ExpirationDate >= DateTime.Now &&
                    x.ConditionsType == 1).ToList();

                    if (listPromotion.Count > 0)
                    {
                        #region Lấy list khách hàng có phát sinh đơn hàng ở trạng thái Đóng

                        //Trạng thái Đóng của Đơn hàng
                        var statusOrderClose =
                            context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "COMP")?.OrderStatusId;

                        //Lấy tất cả khách hàng đã phát sinh đơn hàng có trạng thái Đóng
                        var listCustomerIdHasOrder = context.CustomerOrder.Where(x => x.Active == true &&
                                                                                      x.StatusId == statusOrderClose)
                            .Select(y => y.CustomerId).Distinct().ToList();

                        #endregion

                        listPromotion.ForEach(item =>
                        {
                            if (item.FilterQuery == null && item.CustomerHasOrder == false)
                            {
                                listPromotionId.Add(item.PromotionId);
                            }
                            else if (item.FilterQuery == null && item.CustomerHasOrder)
                            {
                                if (listCustomerIdHasOrder.Count > 0)
                                {
                                    var existsCustomer =
                                        listCustomerIdHasOrder.FirstOrDefault(x => x == parameter.CustomerId);

                                    if (existsCustomer != Guid.Empty && existsCustomer != null)
                                    {
                                        listPromotionId.Add(item.PromotionId);
                                    }
                                }
                            }

                            var listCurrentCustomerId = new List<Guid>();
                            //Lọc theo Bộ tiêu chí
                            if (item.FilterQuery != null)
                            {
                                if (item.FilterQuery.IndexOf("CreatedDate =") != -1)
                                {
                                    var index_prefix = item.FilterQuery.IndexOf("CreatedDate =");
                                    var date_postfix = item.FilterQuery.Substring(index_prefix + 15, 10);

                                    var sub_query = item.FilterQuery.Substring(index_prefix, 26);

                                    var temp = "datediff(d, 'temp', CreatedDate) = 0";
                                    var result_temp = item.FilterQuery.Replace(sub_query, temp.Replace("temp", date_postfix));

                                    item.FilterQuery = result_temp;
                                }
                                if (item.FilterQuery.IndexOf("CreatedDate >") != -1)
                                {
                                    var index_prefix = item.FilterQuery.IndexOf("CreatedDate >");
                                    var date_postfix = item.FilterQuery.Substring(index_prefix + 15, 10);

                                    var sub_query = item.FilterQuery.Substring(index_prefix, 26);

                                    var temp = "datediff(d, 'temp', CreatedDate) > 0";
                                    var result_temp = item.FilterQuery.Replace(sub_query, temp.Replace("temp", date_postfix));

                                    item.FilterQuery = result_temp;
                                }
                                if (item.FilterQuery.IndexOf("CreatedDate <") != -1)
                                {
                                    var index_prefix = item.FilterQuery.IndexOf("CreatedDate <");
                                    var date_postfix = item.FilterQuery.Substring(index_prefix + 15, 10);

                                    var sub_query = item.FilterQuery.Substring(index_prefix, 26);

                                    var temp = "datediff(d, 'temp', CreatedDate) < 0";
                                    var result_temp = item.FilterQuery.Replace(sub_query, temp.Replace("temp", date_postfix));

                                    item.FilterQuery = result_temp;
                                }

                                var sqlCondition =
                                    item.FilterQuery + " AND TenantId = '" + tenantId.ToString() + "'";

                                var listCustomerId = new List<Guid>();
                                using (var command = context.Database.GetDbConnection().CreateCommand())
                                {
                                    command.CommandText = "SearchCustomer";
                                    DbParameter param1 = command.CreateParameter();
                                    param1.ParameterName = "@FilterWhere";
                                    param1.DbType = DbType.String;
                                    param1.Value = sqlCondition;
                                    command.Parameters.Add(param1);

                                    command.CommandType = CommandType.StoredProcedure;

                                    context.Database.OpenConnection();

                                    var dataReader = command.ExecuteReader();

                                    while (dataReader.Read())
                                    {
                                        listCurrentCustomerId.Add(dataReader.GetGuid(0));
                                    }

                                    context.Database.CloseConnection();
                                }

                                if (listCurrentCustomerId.Count > 0)
                                {
                                    //Nếu có điều kiện lấy những Khách hàng đã phát sinh đơn hàng
                                    if (item.CustomerHasOrder)
                                    {
                                        var _listCurrentCustomerId = listCurrentCustomerId
                                            .Where(x => listCustomerIdHasOrder.Contains(x)).ToList();

                                        if (_listCurrentCustomerId.Count > 0)
                                        {
                                            listCustomerId.AddRange(_listCurrentCustomerId);
                                        }
                                    }
                                    else
                                    {
                                        listCustomerId.AddRange(listCurrentCustomerId);
                                    }
                                }

                                var existsCustomer = listCustomerId.FirstOrDefault(x => x == parameter.CustomerId);

                                if (existsCustomer != Guid.Empty)
                                {
                                    listPromotionId.Add(item.PromotionId);
                                }
                            }
                        });

                        if (listPromotionId.Count > 0)
                        {
                            var listPromotionMapping = context.PromotionMapping
                                .Where(x => listPromotionId.Contains(x.PromotionId)).ToList();
                            listPromotionApply = listPromotion.Where(x => listPromotionId.Contains(x.PromotionId))
                                .Select(y =>
                                    new PromotionApplyModel
                                    {
                                        PromotionId = y.PromotionId,
                                        ConditionsType = y.ConditionsType,
                                        NotMultiplition = y.NotMultiplition,
                                        PropertyType = y.PropertyType,
                                        PropertyTypeName = ConvertPropertyTypeName(1, y.PropertyType),
                                        PromotionName = y.PromotionName,
                                        ListPromotionProductApply = new List<PromotionProductApplyModel>(),
                                        SelectedPromotionProductApply = new List<PromotionProductApplyModel>()
                                    }).ToList();
                            
                            listPromotionApply.ForEach(item =>
                            {
                                var listPromotionProduct = listPromotionMapping
                                    .Where(x => x.PromotionId == item.PromotionId).Select(y =>
                                        new PromotionProductApplyModel
                                        {
                                            PromotionMappingId = y.PromotionMappingId,
                                            IndexOrder = y.IndexOrder,
                                            ProductId = y.HangKhuyenMai,
                                            SoLuongTang = y.SoLuongTang,
                                            LoaiGiaTri = y.LoaiGiaTri,
                                            GiaTri = y.GiaTri,
                                            SoTienTu = 0
                                        }).OrderBy(z => z.IndexOrder).ToList();

                                var listProductId = listPromotionProduct.Where(x => x.ProductId != null)
                                    .Select(y => y.ProductId).ToList();
                                var listProduct = listAllProduct.Where(x => listProductId.Contains(x.ProductId))
                                    .ToList();

                                listPromotionProduct.ForEach(_temp =>
                                {
                                    _temp.PromotionProductName = ConvertPromotionProductName(1, 1,
                                        item.PropertyType, _temp.ProductId, listProduct, _temp.SoLuongTang,
                                        _temp.LoaiGiaTri, _temp.GiaTri);
                                    _temp.PromotionProductNameConvert = ConvertPromotionProductName(2, 1,
                                        item.PropertyType, _temp.ProductId, listProduct, _temp.SoLuongTang,
                                        _temp.LoaiGiaTri, _temp.GiaTri);

                                    if (_temp.ProductId != null)
                                    {
                                        _temp.ProductUnitName = GetProductUnitName(_temp.ProductId, listProductUnit,
                                            listProduct);
                                    }
                                });

                                item.ListPromotionProductApply = listPromotionProduct;

                                if (listPromotionProduct.Count == 1)
                                {
                                    item.SelectedPromotionProductApply = listPromotionProduct;
                                }
                            });
                        }
                    }
                }
                else if (parameter.ConditionsType == 3)
                {
                    listPromotion = context.Promotion.Where(x =>
                        x.Active && x.EffectiveDate <= DateTime.Now && x.ExpirationDate >= DateTime.Now &&
                        x.ConditionsType == 3).ToList();
                    var _listPromotionId = listPromotion.Select(y => y.PromotionId).ToList();

                    var listPromotionMapping = context.PromotionMapping
                        .Where(x => _listPromotionId.Contains(x.PromotionId)).ToList();

                    listPromotion.ForEach(promotion =>
                    {
                        var _listPromotionMapping =
                            listPromotionMapping.Where(x => x.PromotionId == promotion.PromotionId)
                                .OrderByDescending(z => z.SoTienTu)
                                .ToList();

                        var _promotionMapping =
                            _listPromotionMapping.FirstOrDefault(x => x.SoTienTu <= parameter.Amount);

                        if (_promotionMapping != null)
                        {
                            var promotionApply = new PromotionApplyModel
                            {
                                PromotionId = promotion.PromotionId,
                                ConditionsType = promotion.ConditionsType,
                                NotMultiplition = promotion.NotMultiplition,
                                PropertyType = promotion.PropertyType,
                                PropertyTypeName = ConvertPropertyTypeName(1, promotion.PropertyType),
                                PromotionName = promotion.PromotionName,
                                ListPromotionProductApply = new List<PromotionProductApplyModel>(),
                                SelectedPromotionProductApply = new List<PromotionProductApplyModel>()
                            };

                            var listProduct = listAllProduct.Where(x => x.ProductId == _promotionMapping.HangKhuyenMai)
                                .ToList();

                            var promotionProductApply = new PromotionProductApplyModel
                            {
                                PromotionMappingId = _promotionMapping.PromotionMappingId,
                                IndexOrder = _promotionMapping.IndexOrder,
                                ProductId = _promotionMapping.HangKhuyenMai,
                                SoLuongTang = _promotionMapping.SoLuongTang,
                                LoaiGiaTri = _promotionMapping.LoaiGiaTri,
                                GiaTri = _promotionMapping.GiaTri,
                                PromotionProductName = ConvertPromotionProductName(1, 3,
                                    promotion.PropertyType, _promotionMapping.HangKhuyenMai, listProduct,
                                    _promotionMapping.SoLuongTang,
                                    _promotionMapping.LoaiGiaTri, _promotionMapping.GiaTri),
                                PromotionProductNameConvert = ConvertPromotionProductName(2, 3,
                                    promotion.PropertyType, _promotionMapping.HangKhuyenMai, listProduct,
                                    _promotionMapping.SoLuongTang,
                                    _promotionMapping.LoaiGiaTri, _promotionMapping.GiaTri),
                                ProductUnitName = _promotionMapping.HangKhuyenMai != null
                                    ? GetProductUnitName(_promotionMapping.HangKhuyenMai, listProductUnit,
                                        listProduct)
                                    : null,
                                SoTienTu = _promotionMapping.SoTienTu
                            };

                            promotionApply.ListPromotionProductApply.Add(promotionProductApply);
                            promotionApply.SelectedPromotionProductApply.Add(promotionProductApply);

                            listPromotionApply.Add(promotionApply);
                        }
                    });
                }
                else if (parameter.ConditionsType == 2)
                {
                    listPromotion = context.Promotion.Where(x =>
                        x.Active && x.EffectiveDate <= DateTime.Now && x.ExpirationDate >= DateTime.Now &&
                        x.ConditionsType == 2).ToList();
                    listPromotionId = listPromotion.Select(y => y.PromotionId).ToList();

                    var listPromotionMapping = context.PromotionMapping
                        .Where(x => listPromotionId.Contains(x.PromotionId)).ToList();

                    var listPromotionProductMapping = context.PromotionProductMapping.ToList();

                    listPromotion.ForEach(promotion =>
                    {
                        var _promotionMapping =
                            listPromotionMapping.FirstOrDefault(x =>
                                x.PromotionId == promotion.PromotionId && x.SanPhamMua == parameter.ProductId &&
                                x.SoLuongMua <= parameter.Quantity);

                        if (_promotionMapping != null)
                        {
                            var promotionApply = new PromotionApplyModel
                            {
                                PromotionId = promotion.PromotionId,
                                ConditionsType = promotion.ConditionsType,
                                NotMultiplition = promotion.NotMultiplition,
                                PropertyType = promotion.PropertyType,
                                PropertyTypeName = ConvertPropertyTypeName(2, promotion.PropertyType),
                                PromotionName = promotion.PromotionName,
                                ListPromotionProductApply = new List<PromotionProductApplyModel>(),
                                SelectedPromotionProductApply = new List<PromotionProductApplyModel>(),
                                //Trường hợp khuyến mãi theo sản phẩm (mua hàng giảm giá hàng, mua hàng tặng hàng)
                                PromotionMappingId = null,
                                SoLuongTang = null,
                                ChiChonMot = false,
                                ListPromotionProductMappingApply = new List<PromotionProductMappingApplyModel>(),
                                SelectedPromotionProductMappingApply = new List<PromotionProductMappingApplyModel>(),
                                SelectedDetail = null
                            };

                            //Nếu là mua hàng tặng phiếu giảm giá
                            if (promotionApply.PropertyType == 3)
                            {
                                var promotionProductApply = new PromotionProductApplyModel
                                {
                                    PromotionMappingId = _promotionMapping.PromotionMappingId,
                                    IndexOrder = _promotionMapping.IndexOrder,
                                    ProductId = _promotionMapping.HangKhuyenMai,
                                    SoLuongTang = _promotionMapping.SoLuongTang,
                                    LoaiGiaTri = _promotionMapping.LoaiGiaTri,
                                    GiaTri = _promotionMapping.GiaTri,
                                    PromotionProductName = ConvertPromotionProductName(1, 2,
                                        promotion.PropertyType, null, new List<Product>(),
                                        _promotionMapping.SoLuongTang,
                                        _promotionMapping.LoaiGiaTri, _promotionMapping.GiaTri),
                                    PromotionProductNameConvert = ConvertPromotionProductName(2, 2,
                                        promotion.PropertyType, null, new List<Product>(),
                                        _promotionMapping.SoLuongTang,
                                        _promotionMapping.LoaiGiaTri, _promotionMapping.GiaTri),
                                    ProductUnitName = null,
                                    SoTienTu = _promotionMapping.SoTienTu
                                };

                                promotionApply.ListPromotionProductApply.Add(promotionProductApply);
                                promotionApply.SelectedPromotionProductApply.Add(promotionProductApply);
                            }
                            //Nếu là mua hàng giảm giá hàng hoặc mua hàng tặng hàng
                            else if (promotionApply.PropertyType == 1 || promotionApply.PropertyType == 2)
                            {
                                promotionApply.PromotionMappingId = _promotionMapping.PromotionMappingId;
                                promotionApply.SoLuongTang = _promotionMapping.SoLuongTang;
                                promotionApply.ChiChonMot = _promotionMapping.ChiChonMot;

                                var listProductId = listPromotionProductMapping
                                    .Where(x => x.PromotionMappingId == _promotionMapping.PromotionMappingId)
                                    .Select(y => y.ProductId).ToList();

                                //Lấy list sản phẩm
                                if (listProductId.Count > 0)
                                {
                                    var listProduct = listAllProduct
                                        .Where(x => listProductId.Contains(x.ProductId))
                                        .ToList();

                                    listProductId.ForEach(productId =>
                                    {
                                        var productInfor = listProduct.FirstOrDefault(x => x.ProductId == productId);

                                        var _newProduct = new PromotionProductMappingApplyModel();
                                        _newProduct.PromotionMappingId = _promotionMapping.PromotionMappingId;
                                        _newProduct.ProductId = productId;
                                        _newProduct.Quantity = 0;
                                        _newProduct.ProductName = productInfor.ProductName;
                                        _newProduct.ProductUnitName =
                                            GetProductUnitName(productId, listProductUnit, listProduct);

                                        promotionApply.ListPromotionProductMappingApply.Add(_newProduct);
                                    });
                                }
                            }

                            listPromotionApply.Add(promotionApply);
                        }
                    });
                }

                return new GetApplyPromotionResult()
                {
                    Status = true,
                    Message = "Success",
                    ListPromotionApply = listPromotionApply
                };
            }
            catch (Exception e)
            {
                return new GetApplyPromotionResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CheckPromotionByAmountResult CheckPromotionByAmount(CheckPromotionByAmountParameter parameter)
        {
            try
            {
                var IsPromotionAmount = false;

                var listPromotion = context.Promotion.Where(x =>
                    x.Active && x.EffectiveDate <= DateTime.Now && x.ExpirationDate >= DateTime.Now &&
                    x.ConditionsType == 3).ToList();
                var listPromotionId = listPromotion.Select(y => y.PromotionId).ToList();
                var listPromotionMapping = context.PromotionMapping.Where(x => listPromotionId.Contains(x.PromotionId))
                    .ToList();

                listPromotionMapping.ForEach(item =>
                {
                    if (item.SoTienTu <= parameter.Amount)
                    {
                        IsPromotionAmount = true;
                    }
                });

                return new CheckPromotionByAmountResult()
                {
                    Status = true,
                    Message = "Success",
                    IsPromotionAmount = IsPromotionAmount
                };
            }
            catch (Exception e)
            {
                return new CheckPromotionByAmountResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CheckPromotionByProductResult CheckPromotionByProduct(CheckPromotionByProductParameter parameter)
        {
            try
            {
                bool IsPromotionProduct = false;

                var listPromotion = context.Promotion.Where(x =>
                    x.Active && x.EffectiveDate <= DateTime.Now && x.ExpirationDate >= DateTime.Now &&
                    x.ConditionsType == 2).ToList();
                var listPromotionId = listPromotion.Select(y => y.PromotionId).ToList();
                var listPromotionMapping = context.PromotionMapping.Where(x => listPromotionId.Contains(x.PromotionId))
                    .ToList();

                listPromotionMapping.ForEach(item =>
                {
                    if (item.SanPhamMua != null && item.SanPhamMua == parameter.ProductId &&
                        item.SoLuongMua <= parameter.Quantity)
                    {
                        IsPromotionProduct = true;
                    }
                });

                return new CheckPromotionByProductResult()
                {
                    Status = true,
                    Message = "Success",
                    IsPromotionProduct = IsPromotionProduct
                };
            }
            catch (Exception e)
            {
                return new CheckPromotionByProductResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private string FormatFileName(string FileName, string FileExtension)
        {
            string newFileName = "";

            //Bỏ đi Guid Id trong FileName
            var index = FileName.LastIndexOf("_");
            var prefixFileName = FileName.Substring(0, index);

            newFileName = prefixFileName + "." + FileExtension;

            return newFileName;
        }

        private string ConvertFolderUrl(string url)
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

        private string ConvertPropertyTypeName(int ConditionsType, int PropertyType)
        {
            var PropertyTypeName = "";

            if (ConditionsType == 1)
            {
                switch (PropertyType)
                {
                    case 1:
                        PropertyTypeName = "Tặng hàng";
                        break;
                    case 2:
                        PropertyTypeName = "Phiếu giảm giá";
                        break;
                    default:
                        PropertyTypeName = "";
                        break;
                }
            }

            if (ConditionsType == 3)
            {
                switch (PropertyType)
                {
                    case 1:
                        PropertyTypeName = "Tặng hàng";
                        break;
                    case 2:
                        PropertyTypeName = "Phiếu giảm giá";
                        break;
                    default:
                        PropertyTypeName = "";
                        break;
                }
            }

            if (ConditionsType == 2)
            {
                switch (PropertyType)
                {
                    case 1:
                        PropertyTypeName = "Mua hàng giảm giá hàng";
                        break;
                    case 2:
                        PropertyTypeName = "Mua hàng tặng hàng";
                        break;
                    default:
                        PropertyTypeName = "Mua hàng tặng phiếu giảm giá";
                        break;
                }
            }

            return PropertyTypeName;
        }

        private string ConvertPromotionProductName(int mode, int ConditionsType, int PropertyType, Guid? ProductId, 
            List<Product> listProduct, decimal SoLuongTang, bool LoaiGiaTri, decimal GiaTri)
        {
            string Name = "";

            if (ConditionsType == 1 || ConditionsType == 3)
            {
                //Nếu tặng hàng
                if (PropertyType == 1)
                {
                    var product = listProduct.FirstOrDefault(x => x.ProductId == ProductId);
                    if (product != null)
                    {
                        //Lấy tên
                        if (mode == 1)
                        {
                            Name = product.ProductName;
                        }
                        //Lấy tên kèm số lượng
                        else if (mode == 2)
                        {
                            Name = product.ProductName + " - Số lượng: " + Math.Round(SoLuongTang, 2);
                        }
                    }
                }
                //Nếu tặng phiếu giảm giá
                else if (PropertyType == 2)
                {
                    //Nếu là %
                    if (LoaiGiaTri)
                    {
                        //Lấy tên
                        if (mode == 1)
                        {
                            Name = "Phiếu giảm giá";
                        }
                        //Lấy tên kèm số lượng
                        else
                        {
                            Name = "Giảm " + Math.Round(GiaTri, 2) + "%" + " - Số lượng: " + Math.Round(SoLuongTang, 0) ;
                        }
                    }
                    //Nếu là số tiền
                    else
                    {
                        //Lấy tên
                        if (mode == 1)
                        {
                            Name = "Phiếu giảm giá";
                        }
                        //Lấy tên kèm số lượng
                        else
                        {
                            Name = "Giảm " + Math.Round(GiaTri, 2) + " VNĐ" + " - Số lượng: " + Math.Round(SoLuongTang, 0);
                        }
                    }
                }
            }

            //Nếu khuyến mãi Theo sản phẩm
            if (ConditionsType == 2)
            {
                //Nếu hình thức là Mua hàng tặng phiếu giảm giá
                if (PropertyType == 3)
                {
                    //Nếu là %
                    if (LoaiGiaTri)
                    {
                        //Lấy tên
                        if (mode == 1)
                        {
                            Name = "Phiếu giảm giá";
                        }
                        //Lấy tên kèm số lượng
                        else
                        {
                            Name = "Giảm " + Math.Round(GiaTri, 2) + "%" + " - Số lượng: " + Math.Round(SoLuongTang, 0);
                        }
                    }
                    //Nếu là số tiền
                    else
                    {
                        //Lấy tên
                        if (mode == 1)
                        {
                            Name = "Phiếu giảm giá";
                        }
                        //Lấy tên kèm số lượng
                        else
                        {
                            Name = "Giảm " + Math.Round(GiaTri, 2) + " VNĐ" + " - Số lượng: " + Math.Round(SoLuongTang, 0);
                        }
                    }
                }
            }

            return Name;
        }

        private string GetProductUnitName(Guid? ProductId, List<Category> ListProductUnit, List<Product> ListProduct)
        {
            var ProductUnitName = "";

            var product = ListProduct.FirstOrDefault(x => x.ProductId == ProductId);
            if (product != null)
            {
                var unit = ListProductUnit.FirstOrDefault(x => x.CategoryId == product.ProductUnitId);
                if (unit != null)
                {
                    ProductUnitName = unit.CategoryName;
                }
            }

            return ProductUnitName;
        }
    }
}
