using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Quote;
using TN.TNM.DataAccess.Messages.Results.Quote;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Cost;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.SaleBidding;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class QuoteDAO : BaseDAO, IQuoteDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
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
        public static string Domain;
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

            var configEntity = context.SystemParameter.ToList();
            Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
        }

        public QuoteDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
            this.Configuration = iconfiguration;
        }

        public CreateQuoteResult CreateQuote(CreateQuoteParameter parameter)
        {
            bool isValidParameterNumber = true;
            if (parameter.Quote?.DaysAreOwed < 0 || parameter.Quote?.DiscountValue < 0 ||
                parameter.Quote?.EffectiveQuoteDate <= 0 || parameter.Quote?.MaxDebt < 0)
            {
                isValidParameterNumber = false;
            }

            parameter.QuoteDetail.ForEach(item =>
            {
                if (item?.DiscountValue < 0 || item?.ExchangeRate < 0 || item?.Quantity <= 0 || item?.UnitPrice < 0 ||
                    item?.Vat < 0)
                {
                    isValidParameterNumber = false;
                }
            });
            if (!isValidParameterNumber)
            {
                return new CreateQuoteResult
                {
                    Message = CommonMessage.Quote.CREATE_FAIL,
                    Status = false
                };
            }

            //Kiểm tra chiết khấu của đơn hàng
            if (parameter.Quote.DiscountValue == null)
            {
                parameter.Quote.DiscountValue = 0;
            }

            //Kiểm tra chiết khấu của sản phẩm
            if (parameter.Quote.QuoteDetail.Count > 0)
            {
                var listProduct = parameter.Quote.QuoteDetail.ToList();
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
                var quoteCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI");
                var listQuoteStatus = context.Category.Where(x => x.CategoryTypeId == quoteCategoryType.CategoryTypeId)
                    .ToList();

                List<QuoteDocument> listQuoteDocuments = new List<QuoteDocument>();

                if (parameter.Quote.QuoteId != null && parameter.Quote.QuoteId != Guid.Empty)
                {
                    var oldQuote = context.Quote.FirstOrDefault(co => co.QuoteId == parameter.Quote.QuoteId);

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        #region Delete all item Relation 

                        var List_Delete_QuoteProductDetailProductAttributeValue =
                            new List<QuoteProductDetailProductAttributeValue>();
                        var List_Delete_QuoteDetail = new List<QuoteDetail>();
                        List_Delete_QuoteDetail = context.QuoteDetail
                            .Where(item => item.QuoteId == parameter.Quote.QuoteId).ToList();

                        List_Delete_QuoteDetail.ForEach(item =>
                        {
                            var objectQuoteDetail = new QuoteDetail();
                            if (item.QuoteDetailId != Guid.Empty)
                            {
                                objectQuoteDetail = item;
                                var QuoteProductDetailProductAttributeValueList = context
                                    .QuoteProductDetailProductAttributeValue
                                    .Where(OPDPAV => OPDPAV.QuoteDetailId == item.QuoteDetailId).ToList();
                                List_Delete_QuoteProductDetailProductAttributeValue.AddRange(
                                    QuoteProductDetailProductAttributeValueList);
                            }
                        });

                        var List_QuoteDocument = context.QuoteDocument.Where(w => w.QuoteId == parameter.Quote.QuoteId)
                            .ToList();
                        var List_AdditionalInformation = context.AdditionalInformation
                            .Where(x => x.ObjectId == parameter.Quote.QuoteId && x.ObjectType == "QUOTE").ToList();

                        var List_Delete_QuoteCostDetail = new List<QuoteCostDetail>();
                        List_Delete_QuoteCostDetail = context.QuoteCostDetail
                            .Where(item => item.QuoteId == parameter.Quote.QuoteId).ToList();

                        var listDeleteParticipant = context.QuoteParticipantMapping
                            .Where(x => x.QuoteId == parameter.Quote.QuoteId).ToList();

                        var listDeletePromotion = context.PromotionObjectApply
                            .Where(x => x.ObjectId == parameter.Quote.QuoteId && x.ObjectType == "QUOTE").ToList();

                        context.QuoteProductDetailProductAttributeValue.RemoveRange(
                            List_Delete_QuoteProductDetailProductAttributeValue);
                        context.SaveChanges();
                        context.QuoteDetail.RemoveRange(List_Delete_QuoteDetail);
                        context.SaveChanges();
                        context.QuoteCostDetail.RemoveRange(List_Delete_QuoteCostDetail);
                        context.SaveChanges();
                        context.QuoteDocument.RemoveRange(List_QuoteDocument);
                        context.SaveChanges();
                        context.AdditionalInformation.RemoveRange(List_AdditionalInformation);
                        context.SaveChanges();
                        context.QuoteParticipantMapping.RemoveRange(listDeleteParticipant);
                        context.SaveChanges();
                        context.Quote.Remove(oldQuote);
                        context.SaveChanges();
                        context.PromotionObjectApply.RemoveRange(listDeletePromotion);
                        context.SaveChanges();

                        #endregion

                        #region Create new Order base on Old Order

                        parameter.QuoteDetail.ForEach(item =>
                        {
                            item.CreatedById = parameter.UserId;
                            item.CreatedDate = DateTime.Now;
                            if (item.QuoteDetailId == null || item.QuoteDetailId == Guid.Empty)
                                item.QuoteDetailId = Guid.NewGuid();
                            if (item.QuoteProductDetailProductAttributeValue != null)
                            {
                                foreach (var itemX in item.QuoteProductDetailProductAttributeValue)
                                {
                                    if (itemX.QuoteProductDetailProductAttributeValueId == null ||
                                        itemX.QuoteProductDetailProductAttributeValueId == Guid.Empty)
                                        itemX.QuoteProductDetailProductAttributeValueId = Guid.NewGuid();
                                }
                            }
                        });

                        parameter.QuoteCostDetail.ForEach(item =>
                        {
                            item.Active = true;
                            item.CreatedById = parameter.UserId;
                            item.CreatedDate = DateTime.Now;
                            if (item.QuoteCostDetailId == null || item.QuoteCostDetailId == Guid.Empty)
                                item.QuoteCostDetailId = Guid.NewGuid();
                        });

                        parameter.QuoteDocument.ForEach(item =>
                        {
                            string folderName = "FileUpload";
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string newPath = Path.Combine(webRootPath, folderName);
                            item.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                            if (item.QuoteDocumentId == null || item.QuoteDocumentId == Guid.Empty)
                                item.QuoteDocumentId = Guid.NewGuid();
                        });

                        List<AdditionalInformation> listAdditionalInformation = new List<AdditionalInformation>();
                        parameter.ListAdditionalInformation.ForEach(item =>
                        {
                            var additionalInformation = new AdditionalInformation();
                            additionalInformation.AdditionalInformationId = Guid.NewGuid();
                            additionalInformation.ObjectId = parameter.Quote.QuoteId;
                            additionalInformation.ObjectType = "QUOTE";
                            additionalInformation.Title = item.Title;
                            additionalInformation.Content = item.Content;
                            additionalInformation.Ordinal = item.Ordinal;
                            additionalInformation.Active = true;
                            additionalInformation.CreatedById = parameter.UserId;
                            additionalInformation.CreatedDate = DateTime.Now;
                            additionalInformation.UpdatedById = null;
                            additionalInformation.UpdatedDate = null;

                            listAdditionalInformation.Add(additionalInformation);
                        });
                        context.AdditionalInformation.AddRange(listAdditionalInformation);
                        context.SaveChanges();

                        parameter.Quote.QuoteDetail = parameter.QuoteDetail;
                        parameter.Quote.QuoteCostDetail = parameter.QuoteCostDetail;
                        parameter.Quote.QuoteDocument = parameter.QuoteDocument;
                        parameter.Quote.QuoteId = oldQuote.QuoteId;
                        parameter.Quote.QuoteCode = oldQuote.QuoteCode;
                        parameter.Quote.QuoteName = parameter.Quote.QuoteName;
                        parameter.Quote.QuoteDate = oldQuote.QuoteDate;
                        parameter.Quote.CreatedById = oldQuote.CreatedById;
                        parameter.Quote.CreatedDate = oldQuote.CreatedDate;
                        parameter.Quote.UpdatedById = parameter.UserId;
                        parameter.Quote.UpdatedDate = DateTime.Now;
                        context.Quote.Add(parameter.Quote);
                        context.SaveChanges();

                        listQuoteDocuments.AddRange(parameter.QuoteDocument);

                        for (int i = 0; i < listQuoteDocuments.Count; i++)
                        {
                            var isCheck = false;
                            for (int j = 0; j < List_QuoteDocument.Count; j++)
                            {
                                if (listQuoteDocuments[i].QuoteDocumentId == List_QuoteDocument[j].QuoteDocumentId)
                                {
                                    isCheck = true;
                                    break;
                                }
                            }
                            if (!isCheck)
                            {
                                listQuoteDocuments[i].UpdatedDate = DateTime.Now;
                            }
                        }

                        #region Thêm người tham gia

                        List<QuoteParticipantMapping> listParticipantMapping = new List<QuoteParticipantMapping>();
                        parameter.ListParticipant.ForEach(item =>
                        {
                            var quoteParticipantMapping = new QuoteParticipantMapping();
                            quoteParticipantMapping.QuoteParticipantMappingId = Guid.NewGuid();
                            quoteParticipantMapping.EmployeeId = item;
                            quoteParticipantMapping.QuoteId = parameter.Quote.QuoteId;

                            listParticipantMapping.Add(quoteParticipantMapping);
                        });
                        context.QuoteParticipantMapping.AddRange(listParticipantMapping);

                        #endregion

                        #region Thêm quà khuyến mãi

                        var listPromotionObjectApply = new List<PromotionObjectApply>();
                        parameter.ListPromotionObjectApply.ForEach(item =>
                        {
                            var promotionObjectApply = new PromotionObjectApply();
                            promotionObjectApply.PromotionObjectApplyId = Guid.NewGuid();
                            promotionObjectApply.ObjectId = parameter.Quote.QuoteId;
                            promotionObjectApply.ObjectType = "QUOTE";
                            promotionObjectApply.PromotionId = item.PromotionId;
                            promotionObjectApply.ConditionsType = item.ConditionsType;
                            promotionObjectApply.PropertyType = item.PropertyType;
                            promotionObjectApply.NotMultiplition = item.NotMultiplition;
                            promotionObjectApply.PromotionMappingId = item.PromotionMappingId;
                            promotionObjectApply.ProductId = item.ProductId;
                            promotionObjectApply.SoLuongTang = item.SoLuongTang;
                            promotionObjectApply.LoaiGiaTri = item.LoaiGiaTri;
                            promotionObjectApply.GiaTri = item.GiaTri;
                            promotionObjectApply.Amount = item.Amount;
                            promotionObjectApply.SoTienTu = item.SoTienTu;

                            listPromotionObjectApply.Add(promotionObjectApply);
                        });

                        context.PromotionObjectApply.AddRange(listPromotionObjectApply);

                        #endregion

                        context.SaveChanges();
                        transaction.Commit();

                        #region Gửi thông báo

                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "UPD", new Quote(),
                            parameter.Quote, true);

                        #endregion

                        #endregion
                    }
                }
                else
                {
                    if (parameter.isClone)
                    {
                        var quoteClone = context.Quote.FirstOrDefault(cl => cl.QuoteId == parameter.QuoteIdClone);
                        quoteClone.CloneCount = quoteClone.CloneCount == null ? 1 : quoteClone.CloneCount + 1;
                        context.Quote.Update(quoteClone);

                        parameter.Quote.CloneCount = null;
                        parameter.Quote.StatusId = listQuoteStatus
                            .FirstOrDefault(c => c.Active == true && c.CategoryCode == "MTA").CategoryId;
                        parameter.Quote.QuoteName = parameter.Quote.QuoteName + "_" + quoteClone.CloneCount;
                    }

                    parameter.Quote.QuoteDate = DateTime.Now;
                    parameter.Quote.CreatedById = parameter.UserId;
                    parameter.Quote.CreatedDate = DateTime.Now;
                    parameter.Quote.UpdatedById = parameter.UserId;
                    parameter.Quote.UpdatedDate = DateTime.Now;

                    #region Create New Order with GenerateorderCode

                    parameter.QuoteDetail.ForEach(item =>
                    {
                        item.QuoteDetailId = Guid.NewGuid();
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;

                        foreach (var itemX in item.QuoteProductDetailProductAttributeValue)
                        {
                            itemX.QuoteProductDetailProductAttributeValueId = Guid.NewGuid();
                        }
                    });

                    parameter.QuoteCostDetail.ForEach(item =>
                    {
                        item.QuoteCostDetailId = Guid.NewGuid();
                        item.CreatedById = parameter.Quote.CreatedById;
                        item.CreatedDate = parameter.Quote.CreatedDate;
                        item.Active = true;
                    });

                    parameter.QuoteDocument.ForEach(item =>
                    {
                        string folderName = "FileUpload";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);
                        item.DocumentUrl = Path.Combine(newPath, item.DocumentName);
                        item.QuoteDocumentId = Guid.NewGuid();
                    });

                    parameter.Quote.QuoteCode = GenerateorderCode();
                    //Kiểm tra trùng quote
                    var dublicateQuote = context.Quote.FirstOrDefault(x => x.QuoteCode == parameter.Quote.QuoteCode);
                    if (dublicateQuote != null)
                    {
                        return new CreateQuoteResult
                        {
                            Status = false,
                            Message = "Báo giá đã tồn tại trên hệ thống",
                            QuoteID = Guid.Empty
                        };
                    }

                    parameter.Quote.QuoteDetail = parameter.QuoteDetail;
                    parameter.Quote.QuoteCostDetail = parameter.QuoteCostDetail;
                    parameter.Quote.QuoteDocument = parameter.QuoteDocument;
                    context.Quote.Add(parameter.Quote);
                    context.SaveChanges();

                    listQuoteDocuments.AddRange(parameter.QuoteDocument);

                    #region Thêm người tham gia

                    List<QuoteParticipantMapping> listParticipantMapping = new List<QuoteParticipantMapping>();
                    parameter.ListParticipant.ForEach(item =>
                    {
                        var quoteParticipantMapping = new QuoteParticipantMapping();
                        quoteParticipantMapping.QuoteParticipantMappingId = Guid.NewGuid();
                        quoteParticipantMapping.EmployeeId = item;
                        quoteParticipantMapping.QuoteId = parameter.Quote.QuoteId;

                        listParticipantMapping.Add(quoteParticipantMapping);
                    });
                    context.QuoteParticipantMapping.AddRange(listParticipantMapping);

                    #endregion

                    #region Thêm thông tin bổ sung cho báo giá

                    parameter.ListAdditionalInformation.ForEach(item =>
                    {
                        item.AdditionalInformationId = Guid.NewGuid();
                        item.ObjectId = parameter.Quote.QuoteId;
                        item.ObjectType = "QUOTE";
                        item.Active = true;
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;
                        item.UpdatedById = null;
                        item.UpdatedDate = null;
                    });

                    context.AdditionalInformation.AddRange(parameter.ListAdditionalInformation);

                    #endregion

                    #region Thêm quà khuyến mãi

                    var listPromotionObjectApply = new List<PromotionObjectApply>();
                    parameter.ListPromotionObjectApply.ForEach(item =>
                    {
                        var promotionObjectApply = new PromotionObjectApply();
                        promotionObjectApply.PromotionObjectApplyId = Guid.NewGuid();
                        promotionObjectApply.ObjectId = parameter.Quote.QuoteId;
                        promotionObjectApply.ObjectType = "QUOTE";
                        promotionObjectApply.PromotionId = item.PromotionId;
                        promotionObjectApply.ConditionsType = item.ConditionsType;
                        promotionObjectApply.PropertyType = item.PropertyType;
                        promotionObjectApply.NotMultiplition = item.NotMultiplition;
                        promotionObjectApply.PromotionMappingId = item.PromotionMappingId;
                        promotionObjectApply.ProductId = item.ProductId;
                        promotionObjectApply.SoLuongTang = item.SoLuongTang;
                        promotionObjectApply.LoaiGiaTri = item.LoaiGiaTri;
                        promotionObjectApply.GiaTri = item.GiaTri;
                        promotionObjectApply.Amount = item.Amount;
                        promotionObjectApply.SoTienTu = item.SoTienTu;

                        listPromotionObjectApply.Add(promotionObjectApply);
                    });

                    context.PromotionObjectApply.AddRange(listPromotionObjectApply);

                    #endregion

                    context.SaveChanges();

                    #endregion

                    #region Gửi thông báo

                    NotificationHelper.AccessNotification(context, TypeModel.Quote, "CRE", new Quote(),
                        parameter.Quote, true);

                    #endregion
                }

                List<QuoteDocumentEntityModel> listQuoteDocumentEntityModels = new List<QuoteDocumentEntityModel>();
                for (int i = 0; i < listQuoteDocuments.Count; i++)
                {
                    var quoteEntityModel = new QuoteDocumentEntityModel();
                    quoteEntityModel.QuoteId = listQuoteDocuments[i].QuoteId;
                    quoteEntityModel.QuoteDocumentId = listQuoteDocuments[i].QuoteDocumentId;
                    quoteEntityModel.DocumentName = listQuoteDocuments[i].DocumentName;
                    quoteEntityModel.DocumentSize = listQuoteDocuments[i].DocumentSize;
                    quoteEntityModel.DocumentUrl = listQuoteDocuments[i].DocumentUrl;
                    quoteEntityModel.Active = listQuoteDocuments[i].Active;
                    quoteEntityModel.CreatedById = listQuoteDocuments[i].CreatedById;
                    quoteEntityModel.CreatedDate = listQuoteDocuments[i].CreatedDate;
                    quoteEntityModel.UpdatedById = listQuoteDocuments[i].UpdatedById;
                    quoteEntityModel.UpdatedDate = listQuoteDocuments[i].UpdatedDate;

                    listQuoteDocumentEntityModels.Add(quoteEntityModel);
                }

                return new CreateQuoteResult
                {
                    Message = CommonMessage.Quote.CREATE_SUCCESS,
                    Status = true,
                    QuoteID = parameter.Quote.QuoteId,
                    ListQuoteDocument = listQuoteDocumentEntityModels
                };

            }
            catch (Exception ex)
            {
                return new CreateQuoteResult
                {
                    Message = CommonMessage.Quote.CREATE_FAIL + ex.Message,
                    Status = false
                };
            }
        }

        public ExportPdfQuotePDFResult ExportPdfQuote(ExportPdfQuoteParameter parameter)
        {
            throw new NotImplementedException();
        }

        public GetAllQuoteResult GetAllQuote(GetAllQuoteParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.QUOTE, "GetAllQuote", parameter.UserId);
            var customerOrder = context.CustomerOrder.ToList();
            try
            {
                parameter.QuoteCode = parameter.QuoteCode == null ? parameter.QuoteCode : parameter.QuoteCode.Trim();
                parameter.ProductCode = parameter.ProductCode == null ? parameter.ProductCode : parameter.ProductCode.Trim();

                var quoteList = (from or in context.Quote
                                     //join os in context.Category on or.StatusId equals os.CategoryId -> Comment By Hung
                                 where (or.Active == true) && (parameter.QuoteStatusId.Count == 0 || parameter.QuoteStatusId.Contains(or.StatusId))
                                 && (string.IsNullOrEmpty(parameter.QuoteCode) || or.QuoteCode.Contains(parameter.QuoteCode))
                                 select new QuoteEntityModel
                                 {
                                     QuoteId = or.QuoteId,
                                     QuoteCode = or.QuoteCode,
                                     QuoteDate = or.QuoteDate,
                                     Description = or.Description,
                                     Note = or.Note,
                                     ObjectTypeId = or.ObjectTypeId,
                                     ObjectType = or.ObjectType,
                                     PaymentMethod = or.PaymentMethod,
                                     DaysAreOwed = or.DaysAreOwed,
                                     MaxDebt = or.MaxDebt,
                                     ExpirationDate = or.ExpirationDate,
                                     ReceivedDate = or.ReceivedDate,
                                     ReceivedHour = or.ReceivedHour,
                                     RecipientName = or.RecipientName,
                                     LocationOfShipment = or.LocationOfShipment,
                                     ShippingNote = or.ShippingNote,
                                     RecipientPhone = or.RecipientPhone,
                                     RecipientEmail = or.RecipientEmail,
                                     PlaceOfDelivery = or.PlaceOfDelivery,
                                     Amount = or.Amount,
                                     DiscountValue = or.DiscountValue,
                                     StatusId = or.StatusId,
                                     CreatedById = or.CreatedById,
                                     CreatedDate = or.CreatedDate,
                                     UpdatedById = or.UpdatedById,
                                     UpdatedDate = or.UpdatedDate,
                                     Active = or.Active,
                                     DiscountType = or.DiscountType,
                                     CountQuoteInOrder = CountQuoteInCustomerOrder(or.QuoteId, customerOrder),
                                     //SellerAvatarUrl = c.AvatarUrl,-> Comment By Hung
                                     //SellerFirstName = e.EmployeeName,-> Comment By Hung
                                     //SellerLastName = c.LastName,-> Comment By Hung
                                     QuoteStatusName = "",//os.CategoryName,-> Comment By Hung
                                     //CustomerName = GetCustomerName(or.ObjectType, or.ObjectTypeId)-> Comment By Hung
                                 }).OrderByDescending(or => or.CreatedDate).ToList();

                #region Add by Hung
                if (quoteList != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    List<Guid> listLeadId = new List<Guid>();
                    List<Guid> listCustomerId = new List<Guid>();
                    quoteList.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            if (!listCategoryId.Contains(item.StatusId.Value))
                                listCategoryId.Add(item.StatusId.Value);
                        }
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                if (!listLeadId.Contains(item.ObjectTypeId.Value))
                                    listLeadId.Add(item.ObjectTypeId.Value);
                                break;
                            case "CUSTOMER":
                                if (!listCustomerId.Contains(item.ObjectTypeId.Value))
                                    listCustomerId.Add(item.ObjectTypeId.Value);
                                break;
                        }
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
                    var listContact = context.Contact.Where(e => listLeadId.Contains(e.ObjectId)).ToList();
                    quoteList.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                            item.QuoteStatusName = listCategory.FirstOrDefault(e => e.CategoryId == item.StatusId.Value).CategoryName;
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                var contact = listContact.LastOrDefault(e => e.ObjectId == item.ObjectTypeId);
                                if (contact != null)
                                    item.CustomerName = contact.FirstName + ' ' + contact.LastName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                            case "CUSTOMER":
                                var customer = listCustomer.FirstOrDefault(e => e.CustomerId == item.ObjectTypeId);
                                if (customer != null)
                                    item.CustomerName = customer.CustomerName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                        }
                    });
                }
                #endregion

                return new GetAllQuoteResult
                {
                    QuoteList = quoteList,
                    Message = CommonMessage.Quote.SEARCH_SUCCESS,
                    Status = true
                };

            }
            catch (Exception ex)
            {
                return new GetAllQuoteResult
                {
                    Message = ex.ToString(),
                    Status = false
                };

            }

        }
        public int CountQuoteInCustomerOrder(Guid quoteId, List<CustomerOrder> customerOrder)
        {
            var count = customerOrder.Where(c => c.QuoteId == quoteId).Count();
            return count;
        }
        public GetTop3QuotesOverdueResult GetTop3QuotesOverdue(GetTop3QuotesOverdueParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3QuotesOverdue", parameter.UserId);
            try
            {
                var employeeList = context.Employee.ToList();
                var customerList = context.Customer.ToList();
                var quoteDataList = context.Quote.Where(x => x.Active == true).ToList();
                var categoryList = context.Category.Where(x => x.Active == true).ToList();
                var quoteList = new List<GetTop3QuotesOverdueModel>();
                List<string> listCategoryCode = new List<string>() { "CHO" };
                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI").CategoryTypeId;
                var listStatusQuote = context.Category.Where(x =>
                        x.Active == true && x.CategoryTypeId == categoryTypeId &&
                        listCategoryCode.Contains(x.CategoryCode)).Select(y => y.CategoryId)
                    .ToList();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = employeeList.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                if (!employee.IsManager)
                {
                    quoteList = (from oq in quoteDataList
                                 join oe in employeeList on parameter.PersonInChangeId equals oe.EmployeeId
                                 join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                 join ca in categoryList on oq.StatusId equals ca.CategoryId
                                 where (oq.Seller == employee.EmployeeId &&
                                        DateTime.Parse(oq.UpdatedDate.ToString()).AddDays(double.Parse(oq.EffectiveQuoteDate.ToString())) < DateTime.Now.Date &&
                                        //oq.ExpirationDate < DateTime.Now.Date &&
                                        oq.UpdatedDate != null &&
                                        (listStatusQuote == null || listStatusQuote.Count == 0 ||
                                         listStatusQuote.Contains(oq.StatusId.Value)))
                                 select new GetTop3QuotesOverdueModel
                                 {
                                     QuoteId = oq.QuoteId,
                                     QuoteCode = oq.QuoteCode,
                                     QuoteName = oq.QuoteName,
                                     QuoteDate = oq.QuoteDate,
                                     SendQuoteDate = oq.SendQuoteDate,
                                     Amount = oq.Amount,
                                     CustomerName = oc.CustomerName,
                                     EmployeeName = oe.EmployeeName,
                                     IntendedQuoteDate = oq.IntendedQuoteDate,
                                     ExpirationDate = oq.ExpirationDate,
                                     UpdatedDate = oq.UpdatedDate,
                                     Status = ca.CategoryName,
                                     EffectiveQuoteDate = oq.EffectiveQuoteDate
                                 }).OrderBy(oq => DateTime.Parse(oq.UpdatedDate.ToString()).AddDays(double.Parse(oq.EffectiveQuoteDate.ToString()))).Take(5).ToList();
                }
                else
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    employeeList = employeeList
                        .Where(x => (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();

                    quoteList = (from oq in quoteDataList
                                 join oe in employeeList on oq.Seller equals oe.EmployeeId
                                 join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                 join ca in categoryList on oq.StatusId equals ca.CategoryId
                                 where (
                                        DateTime.Parse(oq.UpdatedDate.ToString()).AddDays(double.Parse(oq.EffectiveQuoteDate.ToString())) < DateTime.Now.Date &&
                                        oq.UpdatedDate != null &&
                                        (listStatusQuote == null || listStatusQuote.Count == 0 ||
                                         listStatusQuote.Contains(oq.StatusId.Value)))
                                 select new GetTop3QuotesOverdueModel
                                 {
                                     QuoteId = oq.QuoteId,
                                     QuoteCode = oq.QuoteCode,
                                     QuoteName = oq.QuoteName,
                                     QuoteDate = oq.QuoteDate,
                                     SendQuoteDate = oq.SendQuoteDate,
                                     Amount = oq.Amount,
                                     CustomerName = oc.CustomerName,
                                     EmployeeName = oe.EmployeeName,
                                     IntendedQuoteDate = oq.IntendedQuoteDate,
                                     ExpirationDate = oq.ExpirationDate,
                                     Status = ca.CategoryName,
                                     UpdatedDate = oq.UpdatedDate,
                                     EffectiveQuoteDate = oq.EffectiveQuoteDate
                                 }).OrderBy(oq => DateTime.Parse(oq.UpdatedDate.ToString()).AddDays(double.Parse(oq.EffectiveQuoteDate.ToString()))).Take(5).ToList();
                }

                return new GetTop3QuotesOverdueResult
                {
                    QuotesOverdueList = quoteList,
                    Message = CommonMessage.Quote.SEARCH_SUCCESS,
                    Status = true
                };

            }
            catch (Exception ex)
            {
                return new GetTop3QuotesOverdueResult
                {
                    Message = ex.ToString(),
                    Status = false
                };

            }
        }

        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public GetTop3WeekQuotesOverdueResult GetTop3WeekQuotesOverdue(GetTop3WeekQuotesOverdueParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3WeekQuotesOverdue", parameter.UserId);
            try
            {
                DateTime firstDate = FirstDateOfWeek();
                DateTime lastDate = LastDateOfWeek();

                var employeeList = context.Employee.ToList();
                var customerList = context.Customer.ToList();
                var categoryList = context.Category.ToList();
                var quoteDataList = context.Quote.Where(x => x.Active == true).ToList();
                var quoteList = new List<GetTop3WeekQuotesOverdueModel>();

                List<string> listCategoryCode = new List<string>() { "MTA", "DLY", "CHO" };
                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI").CategoryTypeId;
                var listStatusQuote = context.Category.Where(x =>
                        x.Active == true && x.CategoryTypeId == categoryTypeId &&
                        listCategoryCode.Contains(x.CategoryCode)).Select(y => y.CategoryId)
                    .ToList();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = employeeList.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var dateNow = DateTime.Now;
                int hour = 23 - dateNow.Hour;
                int minute = 59 - dateNow.Minute;
                int second = 59 - dateNow.Second;
                var dateReturnStart = dateNow.AddHours(-dateNow.Hour).AddMinutes(-dateNow.Minute).AddSeconds(-dateNow.Second);
                var dateReturnEnd = dateNow.AddHours(hour).AddMinutes(minute).AddSeconds(second).AddDays(7);

                if (!employee.IsManager)
                {
                    quoteList = (from oq in quoteDataList
                                 join oe in employeeList on parameter.PersonInChangeId equals oe.EmployeeId
                                 join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                 join ca in categoryList on oq.StatusId equals ca.CategoryId
                                 where (oq.Seller == employee.EmployeeId &&
                                        //oq.IntendedQuoteDate >= firstDate && 
                                        DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7).Date >= firstDate.Date &&
                                        DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7).Date <= lastDate.Date &&
                                        //oq.IntendedQuoteDate <= lastDate && 
                                        oq.SendQuoteDate == null &&
                                        (listStatusQuote == null || listStatusQuote.Count == 0 ||
                                         listStatusQuote.Contains(oq.StatusId.Value)))
                                 select new GetTop3WeekQuotesOverdueModel
                                 {
                                     QuoteId = oq.QuoteId,
                                     QuoteCode = oq.QuoteCode,
                                     QuoteDate = oq.QuoteDate,
                                     SendQuoteDate = oq.SendQuoteDate,
                                     Amount = CalculatorDiscount(oq.Amount, oq.DiscountType.Value, oq.DiscountValue.Value),
                                     EmployeeName = oe.EmployeeName,
                                     CustomerName = oc.CustomerName,
                                     QuoteName = oq.QuoteName,
                                     Status = ca.CategoryName,
                                     IntendedQuoteDate = oq.IntendedQuoteDate,
                                     IntendedQuoteDateWeek = DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7),
                                 }).OrderBy(or => or.IntendedQuoteDateWeek).Take(5).ToList();
                }
                else
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    employeeList = employeeList
                        .Where(x => (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();

                    quoteList = (from oq in quoteDataList
                                 join oe in employeeList on oq.Seller equals oe.EmployeeId
                                 join oc in customerList on oq.ObjectTypeId equals oc.CustomerId
                                 join ca in categoryList on oq.StatusId equals ca.CategoryId
                                 where (//oq.IntendedQuoteDate >= firstDate && oq.IntendedQuoteDate <= lastDate &&
                                        DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7).Date >= firstDate.Date &&
                                        DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7).Date <= lastDate.Date &&
                                        oq.SendQuoteDate == null &&
                                        (listStatusQuote == null || listStatusQuote.Count == 0 ||
                                         listStatusQuote.Contains(oq.StatusId.Value)))
                                 select new GetTop3WeekQuotesOverdueModel
                                 {
                                     QuoteId = oq.QuoteId,
                                     QuoteCode = oq.QuoteCode,
                                     QuoteDate = oq.QuoteDate,
                                     SendQuoteDate = oq.SendQuoteDate,
                                     Amount = CalculatorDiscount(oq.Amount, oq.DiscountType.Value, oq.DiscountValue.Value),
                                     EmployeeName = oe.EmployeeName,
                                     CustomerName = oc.CustomerName,
                                     QuoteName = oq.QuoteName,
                                     Status = ca.CategoryName,
                                     IntendedQuoteDate = oq.IntendedQuoteDate,
                                     IntendedQuoteDateWeek = DateTime.Parse(oq.IntendedQuoteDate.ToString()).AddDays(-7),
                                 }).OrderBy(or => or.IntendedQuoteDateWeek).Take(5).ToList();
                }
                return new GetTop3WeekQuotesOverdueResult
                {
                    QuotesOverdueList = quoteList,
                    Message = CommonMessage.Quote.SEARCH_SUCCESS,
                    Status = true
                };

            }
            catch (Exception ex)
            {
                return new GetTop3WeekQuotesOverdueResult
                {
                    Message = ex.ToString(),
                    Status = false
                };

            }
        }

        private decimal CalculatorDiscount(decimal amount, bool discountType, decimal discountValue)
        {
            decimal result = 0;

            if (discountType)
            {
                //Chiết khấu được tính theo %
                result = amount - (amount * discountValue) / 100;
            }
            else
            {
                //Chiết khấu được tính theo tiền mặt
                result = amount - discountValue;
            }

            return result;
        }

        public GetTop3PotentialCustomersResult GetTop3PotentialCustomers(GetTop3PotentialCustomersParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3WeekQuotesOverdue", parameter.UserId);
            try
            {
                var listLead = new List<GetTop3PotentialCustomersModel>();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listEmp = context.Employee.ToList();
                var statusCustomer = context.CategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "THA");
                var statusDD = context.Category.FirstOrDefault(c => c.CategoryTypeId == statusCustomer.CategoryTypeId && c.Active == true && c.CategoryCode == "HDO");

                if (!employee.IsManager)
                {
                    //Lấy list khách hàng tiềm năng do người đang đăng nhập phụ trách
                    //listLead = context.Lead.Where(x =>
                    //        x.Active == true && x.PersonInChargeId == employee.EmployeeId)
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId.Value,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    listLead = context.Customer.Where(x =>
                            x.Active == true && x.PersonInChargeId == employee.EmployeeId && x.StatusId == statusDD.CategoryId)
                        .Select(y => new GetTop3PotentialCustomersModel
                        {
                            LeadId = y.CustomerId,
                            ContactId = Guid.Empty,
                            LeadFirstName = "",
                            LeadLastName = "",
                            Email = "",
                            Phone = "",
                            PersonInChargeId = y.PersonInChargeId.Value,
                            PersonInChargeName = "",
                            CreatedDate = y.CreatedDate
                        }).ToList();

                    //Lấy list khách hàng tiềm năng không có người phụ trách nhưng do người đang đăng nhập tạo ra
                    //var listLead2 = context.Lead
                    //    .Where(x => x.Active == true && x.PersonInChargeId == null &&
                    //                x.CreatedById.ToString().ToLower() == user.UserId.ToString().ToLower())
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId.Value,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    var listLead2 = context.Customer
                       .Where(x => x.Active == true && x.PersonInChargeId == null && x.StatusId == statusDD.CategoryId &&
                                   x.CreatedById.ToString().ToLower() == user.UserId.ToString().ToLower())
                       .Select(y => new GetTop3PotentialCustomersModel
                       {
                           LeadId = y.CustomerId,
                           ContactId = Guid.Empty,
                           LeadFirstName = "",
                           LeadLastName = "",
                           Email = "",
                           Phone = "",
                           PersonInChargeId = y.PersonInChargeId.Value,
                           PersonInChargeName = "",
                           CreatedDate = y.CreatedDate
                       }).ToList();

                    listLead.AddRange(listLead2);
                }
                else
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    var employeeList = context.Employee
                        .Where(x => (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();

                    var listEmployeeId = employeeList.Select(x => x.EmployeeId).ToList();

                    //Lấy list khách hàng tiềm năng do người đang đăng nhập, hoặc nhân viên cấp dưới phụ trách
                    //listLead = context.Lead.Where(x =>
                    //        x.Active == true &&
                    //        (listEmployeeId.Contains(x.PersonInChargeId.Value)))
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId.Value,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    listLead = context.Customer.Where(x =>
                           x.Active == true && x.StatusId == statusDD.CategoryId &&
                           (listEmployeeId.Contains(x.PersonInChargeId.Value)))
                       .Select(y => new GetTop3PotentialCustomersModel
                       {
                           LeadId = y.CustomerId,
                           ContactId = Guid.Empty,
                           LeadFirstName = "",
                           LeadLastName = "",
                           Email = "",
                           Phone = "",
                           PersonInChargeId = y.PersonInChargeId.Value,
                           PersonInChargeName = "",
                           CreatedDate = y.CreatedDate
                       }).ToList();

                    /*
                     * Lấy list khách hàng tiềm năng không có người phụ trách nhưng do người đang đăng nhập hoặc
                     * nhân viên cấp dưới tạo ra
                     */

                    //var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value))
                    //    .Select(y => y.UserId.ToString().ToLower()).ToList();
                    var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value))
                       .Select(y => y.UserId).ToList();

                    //var listLead2 = context.Lead.Where(x => x.Active == true && x.PersonInChargeId == null &&
                    //                                        listUserId.Contains(x.CreatedById))
                    //    .Select(y => new GetTop3PotentialCustomersModel
                    //    {
                    //        LeadId = y.LeadId,
                    //        ContactId = Guid.Empty,
                    //        LeadFirstName = "",
                    //        LeadLastName = "",
                    //        Email = "",
                    //        Phone = "",
                    //        PersonInChargeId = y.PersonInChargeId,
                    //        PersonInChargeName = "",
                    //        CreatedDate = y.CreatedDate
                    //    }).ToList();
                    var listLead2 = context.Customer.Where(x => x.Active == true && x.PersonInChargeId == null && x.StatusId == statusDD.CategoryId &&
                                                            listUserId.Contains(x.CreatedById))
                        .Select(y => new GetTop3PotentialCustomersModel
                        {
                            LeadId = y.CustomerId,
                            ContactId = Guid.Empty,
                            LeadFirstName = "",
                            LeadLastName = "",
                            Email = "",
                            Phone = "",
                            PersonInChargeId = y.PersonInChargeId,
                            PersonInChargeName = "",
                            CreatedDate = y.CreatedDate
                        }).ToList();

                    listLead.AddRange(listLead2);
                }

                listLead = listLead.OrderByDescending(z => z.CreatedDate).Take(5).ToList();

                #region Lấy thông tin thêm cho listLead

                var listLeadId = listLead.Select(x => x.LeadId).ToList();
                var listContactLead = context.Contact.Where(x => x.ObjectType == "CUS" && (listLeadId.Contains(x.ObjectId)))
                    .ToList();

                listLead.ForEach(item =>
                {
                    var contactLead = listContactLead.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    var firstName = contactLead != null
                        ? (contactLead.FirstName != null ? (contactLead.FirstName.Trim()) : "")
                        : "";
                    var lastName = contactLead != null
                       ? (contactLead.LastName != null ? (contactLead.LastName.Trim()) : "")
                       : "";
                    var email = contactLead != null
                        ? (contactLead.Email != null ? (contactLead.Email.Trim()) : "")
                        : "";
                    var phone = contactLead != null
                        ? (contactLead.Phone != null ? (contactLead.Phone.Trim()) : "")
                        : "";
                    var personInCharge = listEmp.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                    var personInChargeName = personInCharge != null
                        ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "")
                        : "";

                    item.ContactId = contactLead.ContactId;
                    item.LeadFirstName = firstName;
                    item.LeadLastName = lastName;
                    item.Email = email;
                    item.Phone = phone;
                    item.PersonInChargeName = personInChargeName;
                });

                #endregion

                return new GetTop3PotentialCustomersResult
                {
                    PotentialCustomersList = listLead,
                    Message = CommonMessage.Quote.SEARCH_SUCCESS,
                    Status = true
                };

            }
            catch (Exception ex)
            {
                return new GetTop3PotentialCustomersResult
                {
                    Message = ex.ToString(),
                    Status = false
                };

            }
        }

        public GetTotalAmountQuoteResult GetTotalAmountQuote(GetTotalAmountQuoteParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETTOP, ObjectName.QUOTE, "GetTop3WeekQuotesOverdue", parameter.UserId);
            try
            {
                List<Guid> leadIdList = new List<Guid>();
                var leadList = context.Lead.Where(w => w.PersonInChargeId == parameter.PersonInChangeId).ToList();

                leadList.ForEach(item =>
                {
                    leadIdList.Add(item.LeadId);
                });

                var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.PersonInChangeId);
                bool isManager = employee.IsManager;

                var totalAmount = new List<Quote>();
                if (!isManager)
                {
                    totalAmount = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).ToList();
                }
                else
                {
                    var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId);
                    if (organization.ParentId == null)
                    {
                        totalAmount = context.Quote.Where(w => DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).ToList();
                    }
                    else
                    {
                        var parentId = context.Organization.Where(o => o.ParentId == employee.OrganizationId).ToList();
                        List<Guid> organizationId = new List<Guid>();
                        organizationId.Add(organization.OrganizationId);

                        foreach (var item in parentId)
                        {
                            organizationId.Add(item.OrganizationId);
                        }

                        List<Guid> employIdList = new List<Guid>();
                        foreach (var item in organizationId)
                        {
                            var idlist = context.Employee.Where(e => e.OrganizationId == item).ToList();
                            foreach (var eml in idlist)
                            {
                                employIdList.Add(eml.EmployeeId);
                            }
                        }

                        foreach (var perId in employIdList)
                        {
                            var quoteIdList = context.Quote.Where(w => w.Seller == perId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).ToList();
                            foreach (var quote in quoteIdList)
                            {
                                totalAmount.Add(quote);
                            }
                        }
                    }
                }

                var categoryTypeID = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI" && ct.Active == true).CategoryTypeId;
                var categoryList = context.Category.Where(w => w.CategoryTypeId == categoryTypeID && (w.CategoryCode != "DTR" && w.CategoryCode != "HUY") && w.Active == true).ToList();
                var listqk = new List<decimal>();

                totalAmount.ForEach(item =>
                {
                    if (categoryList.FirstOrDefault(f => f.CategoryId == item.StatusId) != null)
                        listqk.Add(CalculatorDiscount(item.Amount, item.DiscountType.Value, item.DiscountValue.Value));
                });


                GetTotalAmountQuoteModel quoteList = new GetTotalAmountQuoteModel()
                {
                    TotalAmount = listqk.Sum()
                };

                return new GetTotalAmountQuoteResult
                {
                    ToTalAmount = quoteList,
                    Message = CommonMessage.Quote.SEARCH_SUCCESS,
                    Status = true
                };

            }
            catch (Exception ex)
            {
                return new GetTotalAmountQuoteResult
                {
                    Message = ex.ToString(),
                    Status = false
                };

            }
        }

        public UpdateQuoteResult UpdateQuote(UpdateQuoteParameter parameter)
        {
            try
            {
                var oldQuote = context.Quote.FirstOrDefault(co => co.QuoteId == parameter.Quote.QuoteId);
                var oldAmount = oldQuote.Amount;
                using (var transaction = context.Database.BeginTransaction())
                {
                    #region Delete all item Relation 
                    var List_Delete_QuoteProductDetailProductAttributeValue = new List<QuoteProductDetailProductAttributeValue>();
                    var List_Delete_QuoteDetail = new List<QuoteDetail>();
                    List_Delete_QuoteDetail = context.QuoteDetail.Where(item => item.QuoteId == parameter.Quote.QuoteId).ToList();

                    List_Delete_QuoteDetail.ForEach(item =>
                    {
                        var objectQuoteDetail = new QuoteDetail();
                        if (item.QuoteDetailId != Guid.Empty)
                        {
                            objectQuoteDetail = item;
                            var QuoteProductDetailProductAttributeValueList = context.QuoteProductDetailProductAttributeValue.Where(OPDPAV => OPDPAV.QuoteDetailId == item.QuoteDetailId).ToList();
                            List_Delete_QuoteProductDetailProductAttributeValue.AddRange(QuoteProductDetailProductAttributeValueList);
                        }
                    });

                    context.QuoteProductDetailProductAttributeValue.RemoveRange(List_Delete_QuoteProductDetailProductAttributeValue);
                    context.SaveChanges();
                    context.QuoteDetail.RemoveRange(List_Delete_QuoteDetail);
                    context.SaveChanges();

                    context.Quote.Remove(oldQuote);
                    context.SaveChanges();
                    #endregion

                    #region Create new Order base on Old Order
                    parameter.QuoteDetail.ForEach(item =>
                    {
                        item.QuoteDetailId = Guid.NewGuid();
                        if (item.QuoteProductDetailProductAttributeValue != null)
                        {
                            foreach (var itemX in item.QuoteProductDetailProductAttributeValue)
                            {
                                itemX.QuoteProductDetailProductAttributeValueId = Guid.NewGuid();
                            }
                        }
                    });

                    parameter.Quote.QuoteDetail = parameter.QuoteDetail;
                    context.Quote.Add(parameter.Quote);
                    context.SaveChanges();

                    transaction.Commit();
                    #endregion

                    return new UpdateQuoteResult
                    {
                        Message = CommonMessage.Quote.EDIT_QUOTE_SUCCESS,
                        QuoteID = parameter.Quote.QuoteId,
                        Status = true,
                    };

                }

            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    Message = CommonMessage.Quote.EDIT_QUOTE_FAIL,
                    Status = false,
                };
            }
        }

        public GetQuoteByIDResult GetQuoteByID(GetQuoteByIDParameter parameter)
        {
            var customerOrder = context.CustomerOrder.ToList();

            try
            {
                #region Get Quote By ID

                var quoteObject = (from or in context.Quote
                                   where or.QuoteId == parameter.QuoteId
                                   select new QuoteEntityModel
                                   {
                                       QuoteId = or.QuoteId,
                                       BankAccountId = or.BankAccountId,
                                       QuoteCode = or.QuoteCode,
                                       QuoteDate = or.QuoteDate,
                                       SendQuoteDate = or.SendQuoteDate,
                                       EffectiveQuoteDate = or.EffectiveQuoteDate,
                                       IntendedQuoteDate = or.IntendedQuoteDate,
                                       Description = or.Description,
                                       Note = or.Note,
                                       ObjectTypeId = or.ObjectTypeId,
                                       ObjectType = or.ObjectType,
                                       PaymentMethod = or.PaymentMethod,
                                       DaysAreOwed = or.DaysAreOwed,
                                       MaxDebt = or.MaxDebt,
                                       ReceivedDate = or.ReceivedDate,
                                       ReceivedHour = or.ReceivedHour,
                                       RecipientName = or.RecipientName,
                                       LocationOfShipment = or.LocationOfShipment,
                                       ShippingNote = or.ShippingNote,
                                       RecipientPhone = or.RecipientPhone,
                                       RecipientEmail = or.RecipientEmail,
                                       PlaceOfDelivery = or.PlaceOfDelivery,
                                       Amount = or.Amount,
                                       Seller = or.Seller,
                                       CustomerContactId = or.CustomerContactId,
                                       DiscountValue = or.DiscountValue,
                                       StatusId = or.StatusId,
                                       CreatedById = or.CreatedById,
                                       CreatedDate = or.CreatedDate,
                                       UpdatedById = or.UpdatedById,
                                       UpdatedDate = or.UpdatedDate,
                                       Active = or.Active,
                                       DiscountType = or.DiscountType,
                                       SellerAvatarUrl = "",
                                       SellerFirstName = "",
                                       SellerLastName = "",
                                       CountQuoteInOrder = CountQuoteInCustomerOrder(or.QuoteId, customerOrder)
                                   }).FirstOrDefault();

                #endregion

                #region Get QuoteDetail with OrderDetailType == 0 and QuoteId (Sản phẩm dịch vụ)

                var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                    cod.DiscountType),
                                            })).ToList();

                if (listQuoteObjectType0 != null)
                {
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
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

                    listQuoteObjectType0.ForEach(item =>
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
                    });
                }

                #endregion

                #region Get QuoteDetail with OrderDetailType == 1 and QuoteId (Chi phí phát sinh khác)

                var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue,
                                                    cod.DiscountType),
                                            })).ToList();

                if (listQuoteObjectType1 != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                        item.NameGene = item.NameProduct + "(" + getNameGEn(item.QuoteDetailId) + ")";
                        item.QuoteProductDetailProductAttributeValue = getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                    });
                }

                #endregion

                listQuoteObjectType0.AddRange(listQuoteObjectType1);

                #region Get QuoteDocument with QuoteId

                var listQuoteDocument = (from QD in context.QuoteDocument
                                         where QD.QuoteId == parameter.QuoteId
                                         select new QuoteDocumentEntityModel
                                         {
                                             QuoteDocumentId = QD.QuoteDocumentId,
                                             QuoteId = QD.QuoteId,
                                             DocumentName = QD.DocumentName,
                                             DocumentSize = QD.DocumentSize,
                                             DocumentUrl = QD.DocumentUrl,
                                             CreatedById = QD.CreatedById,
                                             CreatedDate = QD.CreatedDate,
                                             UpdatedById = QD.UpdatedById,
                                             UpdatedDate = QD.UpdatedDate,
                                             Active = QD.Active
                                         }).ToList();

                #endregion

                return new GetQuoteByIDResult
                {
                    QuoteEntityObject = quoteObject,
                    ListQuoteDetail = listQuoteObjectType0,
                    ListQuoteDocument = listQuoteDocument,
                    Message = "Success",
                    Status = true
                };

            }
            catch (Exception ex)
            {
                return new GetQuoteByIDResult
                {
                    Message = ex.ToString(),
                    Status = false
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

        public List<QuoteProductDetailProductAttributeValueEntityModel> getListQuoteProductDetailProductAttributeValue(Guid QuoteDetailId)
        {
            List<QuoteProductDetailProductAttributeValueEntityModel> listResult = new List<QuoteProductDetailProductAttributeValueEntityModel>();

            var OrderProductDetailProductAttributeValueModelList = (from OPDPV in context.QuoteProductDetailProductAttributeValue
                                                                    join ProductAttributeC in context.ProductAttributeCategory on OPDPV.ProductAttributeCategoryId equals ProductAttributeC.ProductAttributeCategoryId
                                                                    join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                    where OPDPV.QuoteDetailId == QuoteDetailId
                                                                    select (new QuoteProductDetailProductAttributeValueEntityModel
                                                                    {
                                                                        QuoteDetailId = OPDPV.QuoteDetailId,
                                                                        QuoteProductDetailProductAttributeValueId = OPDPV.QuoteProductDetailProductAttributeValueId,
                                                                        ProductAttributeCategoryId = OPDPV.ProductAttributeCategoryId,
                                                                        ProductId = OPDPV.ProductId,
                                                                        ProductAttributeCategoryValueId = OPDPV.ProductAttributeCategoryValueId,
                                                                        NameProductAttributeCategory = ProductAttributeC.ProductAttributeCategoryName,
                                                                        NameProductAttributeCategoryValue = ProductAttributeCategoryV.ProductAttributeCategoryValue1
                                                                    })).ToList();
            listResult = OrderProductDetailProductAttributeValueModelList;
            return listResult;

        }

        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? ExchangeRate, decimal? Vat, decimal? DiscountValue, bool? DiscountType)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;

            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value * DiscountValue.Value) / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }
            if (Vat != null)
            {
                CaculateVAT = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value - CacuDiscount) * Vat.Value) / 100;
            }
            result = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value) + CaculateVAT - CacuDiscount;
            return result;
        }

        private string GenerateorderCode()
        {
            //string currentYear = DateTime.Now.Year.ToString();
            //int count = context.Quote.Count();
            //string result = "BG-" + currentYear.Substring(currentYear.Length - 2) + count.ToString("D4");
            //return result;

            // sửa định dạng gen code thành "BG-yyMMdd + 4 số"
            var todayQuotes = context.Quote.Where(w => w.CreatedDate.Date == DateTime.Now.Date)
                                                .OrderByDescending(w => w.CreatedDate)
                                                .ToList();
            #region comment by giang
            //var todayQuotes = context.Quote.Where(w => w.QuoteDate.Value.Date == DateTime.Now.Date)
            //                                    .OrderByDescending(w => w.QuoteDate)
            //                                    .ToList();
            #endregion

            var count = todayQuotes.Count() == 0 ? 0 : todayQuotes.Count();
            string currentYear = DateTime.Now.Year.ToString();
            string result = "BG-" + currentYear.Substring(currentYear.Length - 2) + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + (count + 1).ToString("D4");
            return result;
        }

        private static string GetCustomerName(string ObjectType, Guid? ObjectTypeId, TNTN8Context context)
        {
            string Result = string.Empty;

            if (!string.IsNullOrEmpty(ObjectType) && ObjectTypeId != null)
            {
                switch (ObjectType.ToUpper())
                {
                    case "LEAD":
                        Result = (from c in context.Contact
                                  join l in context.Lead on c.ObjectId equals l.LeadId
                                  where l.LeadId == ObjectTypeId.Value
                                  select new { FullName = c.FirstName + ' ' + c.LastName }).DefaultIfEmpty(new { FullName = string.Empty }).FirstOrDefault().FullName.ToString();
                        break;
                    case "CUSTOMER":
                        Result = (from cus in context.Customer
                                  where cus.CustomerId == ObjectTypeId
                                  select new { FullName = cus.CustomerName == null ? "" : cus.CustomerName }).DefaultIfEmpty(new { FullName = string.Empty }).FirstOrDefault().FullName.ToString();
                        break;
                    default:
                        Result = string.Empty;
                        break;
                }
            }
            return Result;
        }

        private DateTime FirstDateOfWeek()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow;
                    break;
                case DayOfWeek.Tuesday:
                    dateReturn = dateNow.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    dateReturn = dateNow.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    dateReturn = dateNow.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    dateReturn = dateNow.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    dateReturn = dateNow.AddDays(-5);
                    break;
                case DayOfWeek.Sunday:
                    dateReturn = dateNow.AddDays(-6);
                    break;
            }
            int hour = dateNow.Hour;
            int minute = dateNow.Minute;
            int second = dateNow.Second;
            dateReturn = dateReturn.AddHours(-hour).AddMinutes(-minute).AddSeconds(-second);
            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 0, 0, 0, 0);
            return dateReturn;
        }

        private DateTime LastDateOfWeek()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow.AddDays(7);
                    break;
                case DayOfWeek.Tuesday:
                    dateReturn = dateNow.AddDays(5);
                    break;
                case DayOfWeek.Wednesday:
                    dateReturn = dateNow.AddDays(4);
                    break;
                case DayOfWeek.Thursday:
                    dateReturn = dateNow.AddDays(3);
                    break;
                case DayOfWeek.Friday:
                    dateReturn = dateNow.AddDays(2);
                    break;
                case DayOfWeek.Saturday:
                    dateReturn = dateNow.AddDays(1);
                    break;
                case DayOfWeek.Sunday:
                    dateReturn = dateNow;
                    break;
            }
            int hour = 23 - dateNow.Hour;
            int minute = 59 - dateNow.Minute;
            int second = 59 - dateNow.Second;
            dateReturn = dateReturn.AddHours(hour).AddMinutes(minute).AddSeconds(second);
            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 0, 0, 0, 0);
            return dateReturn;
        }

        public GetDashBoardQuoteResult GetDashBoardQuote(GetDashBoardQuoteParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETCOUNT, ObjectName.LEAD, "Count Lead for Dashboard", parameter.UserId);
            var result = new GetDashBoardQuoteResult();
            //List<Guid> orgList = new List<Guid>();
            var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.PersonInChangeId);
            var categoryTypeID = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI" && ct.Active == true).CategoryTypeId;
            bool isManager = employee.IsManager;

            var statusNew = context.Category.FirstOrDefault(c => c.CategoryCode == "MTA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
            var statusWait = context.Category.FirstOrDefault(c => c.CategoryCode == "CHO" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
            var statusProcess = context.Category.FirstOrDefault(c => c.CategoryCode == "DLY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
            var statusCloseSuccess = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
            var statusCloseFailure = context.Category.FirstOrDefault(c => c.CategoryCode == "DON" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
            var statusCancel = context.Category.FirstOrDefault(c => c.CategoryCode == "HUY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
            //var statusStop = context.Category.FirstOrDefault(c => c.CategoryCode == "HOA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
            var quoteList = context.Quote.Where(c => c.Seller != null && c.Active == true).ToList();

            var countNew = 0;
            var countInProgress = 0;
            var countWaiting = 0;
            var countDone = 0;
            var countAbort = 0;
            var countClose = 0;
            var countPause = 0;

            if (!isManager)
            {
                #region Nếu không phải Quản lý

                // Tạo mới
                countNew = quoteList.Where(w => w.Seller == parameter.PersonInChangeId &&
                        w.StatusId == statusNew && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                        DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                // Đang xử lý
                countInProgress = quoteList.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == statusProcess && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                // Chờ phản hồi
                countWaiting = quoteList.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == statusWait && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                // Đóng - Trúng Thầu
                countDone = quoteList.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == statusCloseSuccess && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                // Huỷ
                countAbort = quoteList.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == statusCancel && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                // Đóng 
                countClose = quoteList.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == statusCloseFailure && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                // Hoãn
                //countPause = quoteList.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == statusStop && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                #endregion
            }
            else
            {
                #region Comment by Giang: Code cũ của Ms.NgọcPhạm

                //var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId);
                //if (organization.ParentId == null)
                //{
                //    // Tạo mới
                //    var categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "MTA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    countNew = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                //    // Đang xử lý
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DLY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    countInProgress = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                //    // Chờ phản hồi
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "CHO" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    countWaiting = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                //    // Đóng - Trúng Thầu
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    countDone = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                //    // Huỷ
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HUY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    countAbort = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                //    // Đóng - Không Trúng
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTR" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    countClose = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                //    // Hoãn
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HOA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    countPause = context.Quote.Where(w => w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();

                //}
                //else
                //{
                //    var parentId = context.Organization.Where(o => o.ParentId == employee.OrganizationId).ToList();
                //    List<Guid> organizationId = new List<Guid>();
                //    organizationId.Add(organization.OrganizationId);

                //    foreach (var item in parentId)
                //    {
                //        organizationId.Add(item.OrganizationId);
                //    }

                //    List<Guid> employIdList = new List<Guid>();
                //    foreach (var item in organizationId)
                //    {
                //        var idlist = context.Employee.Where(e => e.OrganizationId == item).ToList();
                //        foreach (var eml in idlist)
                //        {
                //            employIdList.Add(eml.EmployeeId);
                //        }
                //    }

                //    // Tạo mới
                //    var categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "MTA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    foreach (var perId in employIdList)
                //    {
                //        var countQuote = context.Quote.Where(w => w.Seller == perId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                //        countNew = countNew + countQuote;
                //    }

                //    // Đang xử lý
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DLY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    foreach (var perId in employIdList)
                //    {
                //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                //        countInProgress = countInProgress + countQuote;
                //    }

                //    // Chờ phản hồi
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "CHO" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    foreach (var perId in employIdList)
                //    {
                //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                //        countWaiting = countWaiting + countQuote;
                //    }

                //    // Đóng - Trúng Thầu
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    foreach (var perId in employIdList)
                //    {
                //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                //        countDone = countDone + countQuote;
                //    }

                //    // Huỷ
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HUY" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    foreach (var perId in employIdList)
                //    {
                //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                //        countAbort = countAbort + countQuote;
                //    }

                //    // Đóng - Không Trúng
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DTR" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    foreach (var perId in employIdList)
                //    {
                //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                //        countClose = countClose + countQuote;
                //    }

                //    // Hoãn
                //    categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "HOA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //    foreach (var perId in employIdList)
                //    {
                //        var countQuote = context.Quote.Where(w => w.Seller == parameter.PersonInChangeId && w.StatusId == categoryId && DateTime.Parse(w.QuoteDate.ToString()).Month == parameter.MonthQuote && DateTime.Parse(w.QuoteDate.ToString()).Year == parameter.YearQuote).Count();
                //        countPause = countPause + countQuote;
                //    }
                //}

                #endregion

                #region Nếu là quản lý

                //Lấy list phòng ban con của user
                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                var listEmployeeId = context.Employee
                    .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                    .Select(y => y.EmployeeId).ToList();

                // Mới tạo
                countNew = quoteList.Where(x =>
                    (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                    DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                    DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                    x.StatusId == statusNew).Count();

                // Đang xử lý
                countInProgress = quoteList.Where(x =>
                    (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                    DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                    DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                    x.StatusId == statusProcess).Count();

                // Chờ phản hồi
                countWaiting = quoteList.Where(x =>
                    (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                    DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                    DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                    x.StatusId == statusWait).Count();

                // Đóng - Trúng Thầu
                countDone = quoteList.Where(x =>
                    (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                    DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                    DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                    x.StatusId == statusCloseSuccess).Count();

                // Huỷ
                countAbort = quoteList.Where(x =>
                    (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                    DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                    DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                    x.StatusId == statusCancel).Count();

                // Đóng 
                countClose = quoteList.Where(x =>
                (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) &&
                DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                x.StatusId == statusCloseFailure).Count();

                #endregion
            }
            result = new GetDashBoardQuoteResult
            {
                Status = true,
                DashBoardQuote = new GetDashBoardQuoteModel
                {
                    CountInProgress = countInProgress,
                    CountNew = countNew,
                    CountDone = countDone,
                    CountClose = countClose,
                    CountAbort = countAbort,
                    CountWaiting = countWaiting
                }
            };
            return result;
        }

        public UpdateActiveQuoteResult UpdateActiveQuote(UpdateActiveQuoteParameter parameter)
        {
            try
            {
                var oldQuote = context.Quote.FirstOrDefault(co => co.QuoteId == parameter.QuoteId);
                oldQuote.Active = false;
                oldQuote.UpdatedById = parameter.UserId;
                oldQuote.UpdatedDate = DateTime.Now;
                context.Quote.Update(oldQuote);
                context.SaveChanges();

                return new UpdateActiveQuoteResult
                {
                    Message = CommonMessage.Quote.DELETE_QUOTE_SUCCESS,
                    Status = true,
                };

            }
            catch (Exception ex)
            {
                return new UpdateActiveQuoteResult
                {
                    Message = CommonMessage.Quote.DELETE_QUOTE_FAIL,
                    Status = false,
                };
            }
        }

        public GetDataQuoteToPieChartResult GetDataQuoteToPieChart(GetDataQuoteToPieChartParameter parameter)
        {
            try
            {
                List<string> categoriesPieChart = new List<string>();
                List<decimal> dataPieChart = new List<decimal>();

                var user = context.User.Where(x => x.UserId == parameter.UserId).FirstOrDefault();
                var employee = context.Employee.Where(x => x.EmployeeId == user.EmployeeId).FirstOrDefault();
                List<Guid> listStatus = new List<Guid>();
                var categoryTypeID = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI" && ct.Active == true).CategoryTypeId;
                // Báo giá
                var categoryDLY = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                listStatus.Add(categoryDLY);
                // Đóng
                var categoryCHO = context.Category.FirstOrDefault(c => c.CategoryCode == "DON" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                listStatus.Add(categoryCHO);
                // Đóng - Trúng Thầu
                //var categoryDTH = context.Category.FirstOrDefault(c => c.CategoryCode == "DTH" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //listStatus.Add(categoryDTH);
                // Hoãn
                //var categoryHOA = context.Category.FirstOrDefault(c => c.CategoryCode == "HOA" && c.CategoryTypeId == categoryTypeID && c.Active == true).CategoryId;
                //listStatus.Add(categoryHOA);

                if (employee.IsManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    var listEmployeeId = context.Employee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                        .Select(y => y.EmployeeId).ToList();

                    var listQuote = context.Quote.Where(x =>
                        DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                        DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                        (listStatus.Count == 0 || listStatus.Contains(x.StatusId.Value)) &&
                        (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)))
                        .Select(y => new
                        {
                            y.QuoteDate,
                            Total = CalculatorAmount(y.DiscountType.Value, y.DiscountValue.Value, y.Amount)
                        }).ToList();

                    listQuote = listQuote.GroupBy(x => DateTime.Parse(x.QuoteDate.Value.ToString()).Date).Select(y => new
                    {
                        y.First().QuoteDate,
                        Total = y.Sum(s => s.Total)
                    }).OrderBy(z => z.QuoteDate).ToList();

                    listQuote.ForEach(item =>
                    {
                        categoriesPieChart.Add(DateTime.Parse(item.QuoteDate.ToString()).Day.ToString());
                        dataPieChart.Add(item.Total);
                    });
                }
                else
                {
                    var listQuote = context.Quote.Where(x =>
                            DateTime.Parse(x.QuoteDate.ToString()).Month == parameter.MonthQuote &&
                            DateTime.Parse(x.QuoteDate.ToString()).Year == parameter.YearQuote &&
                            (listStatus.Count == 0 || listStatus.Contains(x.StatusId.Value)) &&
                            x.Seller == employee.EmployeeId)
                        .Select(y => new
                        {
                            y.QuoteDate,
                            Total = CalculatorAmount(y.DiscountType.Value, y.DiscountValue.Value, y.Amount)
                        }).ToList();

                    listQuote = listQuote.GroupBy(x => DateTime.Parse(x.QuoteDate.Value.ToString()).Date).Select(y => new
                    {
                        y.First().QuoteDate,
                        Total = y.Sum(s => s.Total)
                    }).OrderBy(z => z.QuoteDate).ToList();

                    listQuote.ForEach(item =>
                    {
                        categoriesPieChart.Add(DateTime.Parse(item.QuoteDate.ToString()).Day.ToString());
                        dataPieChart.Add(item.Total);
                    });
                }

                return new GetDataQuoteToPieChartResult()
                {
                    Status = true,
                    Message = "Success",
                    CategoriesPieChart = categoriesPieChart,
                    DataPieChart = dataPieChart
                };
            }
            catch (Exception e)
            {
                return new GetDataQuoteToPieChartResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchQuoteResult SearchQuote(SearchQuoteParameter parameter)
        {
            try
            {
                var listQuote = new List<QuoteEntityModel>();
                var customerOrder = context.CustomerOrder.ToList();

                #region Lấy list status của báo giá

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true).CategoryTypeId;
                var listStatus = context.Category.Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                                    new CategoryEntityModel
                                    {
                                        CategoryId = y.CategoryId,
                                        CategoryName = y.CategoryName,
                                        CategoryCode = y.CategoryCode,
                                        CategoryTypeId = Guid.Empty,
                                        CreatedById = Guid.Empty,
                                        CountCategoryById = 0
                                    }).ToList();

                #endregion

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                parameter.QuoteCode = parameter.QuoteCode == null ? "" : parameter.QuoteCode.Trim();
                parameter.QuoteName = parameter.QuoteName == null ? "" : parameter.QuoteName.Trim();

                #region Lấy list báo giá mà người dùng được tham gia

                var listQuoteId = context.QuoteParticipantMapping.Where(x => x.EmployeeId == employee.EmployeeId)
                    .Select(y => y.QuoteId).ToList();

                #endregion

                listQuote = context.Quote.Where(x =>
                        (parameter.QuoteCode == "" || x.QuoteCode.Contains(parameter.QuoteCode)) &&
                        (parameter.QuoteName == "" || x.QuoteName.Contains(parameter.QuoteName)) &&
                        (parameter.ListStatusQuote.Count == 0 || parameter.ListStatusQuote.Contains(x.StatusId)) &&
                        x.Active == true && x.Seller != null)
                    .Select(y => new QuoteEntityModel
                    {
                        QuoteId = y.QuoteId,
                        QuoteCode = y.QuoteCode,
                        QuoteName = y.QuoteName,
                        QuoteDate = y.QuoteDate,
                        Seller = y.Seller,
                        Description = y.Description,
                        Note = y.Note,
                        ObjectTypeId = y.ObjectTypeId,
                        ObjectType = y.ObjectType,
                        PaymentMethod = y.PaymentMethod,
                        DaysAreOwed = y.DaysAreOwed,
                        IntendedQuoteDate = y.IntendedQuoteDate,
                        SendQuoteDate = y.SendQuoteDate,
                        MaxDebt = y.MaxDebt,
                        ExpirationDate = y.ExpirationDate,
                        ReceivedDate = y.ReceivedDate,
                        ReceivedHour = y.ReceivedHour,
                        RecipientName = y.RecipientName,
                        LocationOfShipment = y.LocationOfShipment,
                        ShippingNote = y.ShippingNote,
                        RecipientPhone = y.RecipientPhone,
                        RecipientEmail = y.RecipientEmail,
                        PlaceOfDelivery = y.PlaceOfDelivery,
                        Amount = (y.DiscountType == true
                            ? (y.Amount - (y.Amount * y.DiscountValue) / 100)
                            : (y.Amount - y.DiscountValue)),
                        DiscountValue = y.DiscountValue,
                        StatusId = y.StatusId,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        Active = y.Active,
                        DiscountType = y.DiscountType,
                        PersonInChargeId = y.PersonInChargeId,
                        CountQuoteInOrder = CountQuoteInCustomerOrder(y.QuoteId, customerOrder),
                        QuoteStatusName = "",
                        BackgroundColorForStatus = "",
                        CustomerName = "",
                        EffectiveQuoteDate = y.EffectiveQuoteDate
                    }).OrderByDescending(z => z.QuoteDate).ToList();

                if (parameter.IsCompleteInWeek)
                {
                    // Báo giá phải hoàn thành trong tuần
                    parameter.StartDate = FirstDateOfWeek();
                    parameter.EndDate = LastDateOfWeek();

                    listQuote = listQuote.Where(x =>
                        (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                        parameter.StartDate <= DateTime.Parse(x.IntendedQuoteDate.ToString()).AddDays(-7).Date) &&
                        (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                         parameter.EndDate >= DateTime.Parse(x.IntendedQuoteDate.ToString()).AddDays(-7).Date) &&
                        x.SendQuoteDate == null).ToList();
                }
                else
                {
                    listQuote = listQuote.Where(x =>
                                    (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                                     parameter.StartDate <= x.QuoteDate) &&
                                    (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                                     parameter.EndDate >= x.QuoteDate)).ToList();
                }

                if (parameter.IsOutOfDate)
                {
                    var statusDD = listStatus.FirstOrDefault(f => f.CategoryCode == "CHO");

                    listQuote = listQuote.Where(x =>
                            DateTime.Parse(x.UpdatedDate.ToString()).AddDays(x.EffectiveQuoteDate != null
                                ? int.Parse(x.EffectiveQuoteDate.ToString())
                                : 0) < DateTime.Now.Date && x.UpdatedDate != null
                                                         && x.StatusId == statusDD.CategoryId)
                        .OrderByDescending(z => z.QuoteDate).ToList();
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

                    var listEmployeeId = context.Employee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                        .Select(y => y.EmployeeId).ToList();

                    listQuote = listQuote.Where(x =>
                        listEmployeeId.Contains(x.Seller.Value) ||
                        x.PersonInChargeId == employee.EmployeeId ||
                        listQuoteId.Contains(x.QuoteId)).ToList();
                }
                else
                {
                    listQuote = listQuote.Where(x =>
                        x.Seller == employee.EmployeeId || x.PersonInChargeId == employee.EmployeeId ||
                        listQuoteId.Contains(x.QuoteId)).ToList();
                }

                #region Lấy tên Đối tượng và tên Trạng thái của Báo giá

                if (listQuote != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    List<Guid> listLeadId = new List<Guid>();
                    List<Guid> listCustomerId = new List<Guid>();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            if (!listCategoryId.Contains(item.StatusId.Value))
                                listCategoryId.Add(item.StatusId.Value);
                        }
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                if (!listLeadId.Contains(item.ObjectTypeId.Value))
                                    listLeadId.Add(item.ObjectTypeId.Value);
                                break;
                            case "CUSTOMER":
                                if (!listCustomerId.Contains(item.ObjectTypeId.Value))
                                    listCustomerId.Add(item.ObjectTypeId.Value);
                                break;
                        }
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
                    var listContact = context.Contact.Where(e => listLeadId.Contains(e.ObjectId)).ToList();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            var quoteStatus = listCategory.FirstOrDefault(e => e.CategoryId == item.StatusId.Value);
                            switch (quoteStatus.CategoryCode)
                            {
                                case "MTA":
                                    item.BackgroundColorForStatus = "#FFC000";
                                    break;
                                case "CHO":
                                    item.BackgroundColorForStatus = " #9C00FF";
                                    break;
                                case "DTH":
                                    item.BackgroundColorForStatus = "#6D98E7";
                                    break;
                                case "DTR":
                                    item.BackgroundColorForStatus = "#FF0000";
                                    break;
                                case "DLY":
                                    item.BackgroundColorForStatus = "#46B678";
                                    break;
                                case "HUY":
                                    item.BackgroundColorForStatus = "#333333";
                                    break;
                                case "HOA":
                                    item.BackgroundColorForStatus = "#666666";
                                    break;
                                case "TUCHOI":
                                    item.BackgroundColorForStatus = "#878d96";
                                    break;
                            }

                            item.QuoteStatusName = quoteStatus.CategoryName;
                        }

                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                var contact = listContact.LastOrDefault(e => e.ObjectId == item.ObjectTypeId);
                                if (contact != null)
                                    item.CustomerName = contact.FirstName + ' ' + contact.LastName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                            case "CUSTOMER":
                                var customer = listCustomer.FirstOrDefault(e => e.CustomerId == item.ObjectTypeId);
                                if (customer != null)
                                    item.CustomerName = customer.CustomerName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                        }
                    });
                }

                #endregion

                if (parameter.IsOutOfDate)
                {
                    listQuote = listQuote.OrderBy(oq => DateTime.Parse(oq.UpdatedDate.ToString()).AddDays(double.Parse(oq.EffectiveQuoteDate.ToString()))).ToList();
                }
                if (parameter.IsCompleteInWeek)
                {
                    listQuote = listQuote.OrderBy(or => DateTime.Parse(or.IntendedQuoteDate.ToString()).AddDays(-7)).ToList();
                }
                return new SearchQuoteResult()
                {
                    Status = true,
                    Message = "Success",
                    ListQuote = listQuote,
                    ListStatus = listStatus
                };
            }
            catch (Exception e)
            {
                return new SearchQuoteResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDataExportExcelQuoteResult GetDataExportExcelQuote(GetDataExportExcelQuoteParameter parameter)
        {
            try
            {
                var inforExportExcel = new InforExportExcelModel();

                var quote = new QuoteEntityModel();
                var listQuoteDetail = new List<QuoteDetailEntityModel>();
                var listQuoteDocument = new List<QuoteDocumentEntityModel>();
                var listAdditionalInformation = new List<AdditionalInformationEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                quote = context.Quote.Where(x => x.QuoteId == parameter.QuoteId).Select(y => new QuoteEntityModel
                {
                    QuoteId = y.QuoteId,
                    QuoteCode = y.QuoteCode,
                    QuoteDate = y.QuoteDate,
                    SendQuoteDate = y.SendQuoteDate,
                    Seller = y.Seller,
                    EffectiveQuoteDate = y.EffectiveQuoteDate,
                    ExpirationDate = y.ExpirationDate,
                    Description = y.Description,
                    Note = y.Note,
                    ObjectTypeId = y.ObjectTypeId,
                    ObjectType = y.ObjectType,
                    CustomerContactId = y.CustomerContactId,
                    PaymentMethod = y.PaymentMethod,
                    DiscountType = y.DiscountType,
                    BankAccountId = y.BankAccountId,
                    DaysAreOwed = y.DaysAreOwed,
                    MaxDebt = y.MaxDebt,
                    ReceivedDate = y.ReceivedDate,
                    Amount = y.Amount,
                    DiscountValue = y.DiscountValue,
                    IntendedQuoteDate = y.IntendedQuoteDate,
                    StatusId = y.StatusId,
                    CreatedDate = y.CreatedDate,
                    PersonInChargeId = y.PersonInChargeId,
                    SellerName = "",
                    UpdatedDate = y.UpdatedDate
                }).FirstOrDefault();

                var customerName = quote.CustomerName;

                #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 0)

                var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue, cod.DiscountType),
                                            })).ToList();

                if (listQuoteObjectType0 != null)
                {
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
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

                    listQuoteObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null && item.VendorId != Guid.Empty)
                            item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId).VendorName;
                        if (item.ProductId != null && item.ProductId != Guid.Empty)
                            item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId).ProductName;
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                        if (item.UnitId != null && item.UnitId != Guid.Empty)
                            item.NameProductUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId).CategoryName;
                        item.NameGene = item.NameProduct + "(" + getNameGEn(item.QuoteDetailId) + ")";
                        item.QuoteProductDetailProductAttributeValue = getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType0);

                #endregion

                #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 1)

                var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue, cod.DiscountType),
                                            })).ToList();

                if (listQuoteObjectType1 != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType1);

                #endregion

                #region Lấy list thông tin bổ sung của báo giá

                listAdditionalInformation = context.AdditionalInformation
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                    .Select(y =>
                        new AdditionalInformationEntityModel
                        {
                            AdditionalInformationId = y.AdditionalInformationId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Title = y.Title,
                            Content = y.Content,
                            Ordinal = y.Ordinal
                        }).OrderBy(z => z.Ordinal).ToList();

                #endregion

                #region Lấy thông tin để export excel báo giá

                if (parameter.QuoteId != null)
                {
                    var company = context.CompanyConfiguration.FirstOrDefault();
                    inforExportExcel.CompanyName = company.CompanyName;
                    inforExportExcel.Address = company.CompanyAddress;
                    inforExportExcel.Phone = company.Phone;
                    inforExportExcel.Website = "";
                    inforExportExcel.Email = company.Email;

                    decimal totalMoney = 0;
                    decimal totalMoneyNotVat = 0;

                    listQuoteDetail.ForEach(item =>
                    {
                        totalMoneyNotVat = item.Quantity.Value * item.UnitPrice.Value * item.ExchangeRate.Value;
                        if (item.DiscountValue != null)
                        {
                            switch (item.DiscountType)
                            {
                                case true:
                                    totalMoney += totalMoneyNotVat - totalMoneyNotVat * ((decimal)item.DiscountValue / 100) +
                                    (totalMoneyNotVat - totalMoneyNotVat * ((decimal)item.DiscountValue / 100)) * ((decimal)item.Vat / 100);
                                    break;
                                case false:
                                    totalMoney += (totalMoneyNotVat - (decimal)item.DiscountValue) +
                                    (totalMoneyNotVat - (decimal)item.DiscountValue) * ((decimal)item.Vat / 100);
                                    break;
                                case null:
                                    break;
                            }
                        }
                    });

                    decimal discountQuoteMoney = 0;
                    switch (quote.DiscountType)
                    {
                        case true:
                            discountQuoteMoney = totalMoney * (decimal)quote.DiscountValue / 100;
                            break;
                        case false:
                            discountQuoteMoney = (decimal)quote.DiscountValue;
                            break;
                        case null:
                            break;
                    }
                    totalMoney = totalMoney - discountQuoteMoney;

                    inforExportExcel.TextTotalMoney = MoneyHelper.Convert(totalMoney);
                }

                #endregion

                return new GetDataExportExcelQuoteResult()
                {
                    Status = true,
                    Message = "Success",
                    Quote = quote,
                    ListQuoteDetail = listQuoteDetail,
                    ListAdditionalInformation = listAdditionalInformation,
                    InforExportExcel = inforExportExcel
                };
            }
            catch (Exception e)
            {
                return new GetDataExportExcelQuoteResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDataCreateUpdateQuoteResult GetDataCreateUpdateQuote(GetDataCreateUpdateQuoteParameter parameter)
        {
            try
            {
                var listCustomer = new List<CustomerEntityModel>();
                var customerAssigned = new CustomerEntityModel();
                var listLead = new List<LeadEntityModel>();
                var leadAssigned = new LeadEntityModel();
                var listEmployee = new List<EmployeeEntityModel>();
                var employeeAssigned = new EmployeeEntityModel();
                var listNote = new List<NoteEntityModel>();
                var listSaleBidding = new List<SaleBiddingEntityModel>();
                bool isAproval = false;

                var contactList = context.Contact.Where(c => c.ObjectType == "LEA" && c.Active == true).ToList();
                var listAdditionalInformationTemplates = new List<CategoryEntityModel>();
                var INVEST_CODE = "IVF";  //nguon tiem nang code IVF
                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INVEST_CODE)
                    .CategoryTypeId;

                #region Nếu là xem chi tiết báo giá thì lấy thêm thông tin của Quote

                var quote = new QuoteEntityModel();
                var listQuoteDetail = new List<QuoteDetailEntityModel>();
                var listQuoteCostDetail = new List<QuoteCostDetailEntityModel>();
                var listQuoteDocument = new List<QuoteDocumentEntityModel>();
                var listAdditionalInformation = new List<AdditionalInformationEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var leadDetailList = context.LeadDetail.ToList();
                var costQuoteList = context.CostsQuote.ToList();
                var leadProductDetailProductAttributeValueList =
                    context.LeadProductDetailProductAttributeValue.ToList();
                var saleBiddingDetailProductAttributeList = context.SaleBiddingDetailProductAttribute.ToList();
                var statusLead = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS");
                var statusSaleBidding = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST");
                var statusLeadXN = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == statusLead.CategoryTypeId && c.CategoryCode == "APPR");
                var statusSaleBiddingTT = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == statusSaleBidding.CategoryTypeId && c.CategoryCode == "APPR");
                var categoryList = context.Category.ToList();
                var vendorList = context.Vendor.ToList();
                var productList = context.Product.ToList();
                var customerAllList = context.Customer.Where(c =>
                    c.Active == true && c.PersonInChargeId != null && c.PersonInChargeId != Guid.Empty).ToList();
                var sellerQuote = new EmployeeEntityModel();
                var listParticipantId = new List<Guid>();

                if (parameter.QuoteId != null)
                {
                    quote = context.Quote.Where(x => x.QuoteId == parameter.QuoteId).Select(y => new QuoteEntityModel
                    {
                        QuoteId = y.QuoteId,
                        QuoteCode = y.QuoteCode,
                        QuoteDate = y.QuoteDate,
                        QuoteName = y.QuoteName,
                        SendQuoteDate = y.SendQuoteDate,
                        Seller = y.Seller,
                        EffectiveQuoteDate = y.EffectiveQuoteDate,
                        ExpirationDate = y.ExpirationDate,
                        Description = y.Description,
                        Note = y.Note,
                        ObjectTypeId = y.ObjectTypeId,
                        ObjectType = y.ObjectType,
                        CustomerContactId = y.CustomerContactId,
                        PaymentMethod = y.PaymentMethod,
                        DiscountType = y.DiscountType,
                        BankAccountId = y.BankAccountId,
                        DaysAreOwed = y.DaysAreOwed,
                        MaxDebt = y.MaxDebt,
                        ReceivedDate = y.ReceivedDate,
                        Amount = y.Amount,
                        DiscountValue = y.DiscountValue,
                        IntendedQuoteDate = y.IntendedQuoteDate,
                        StatusId = y.StatusId,
                        CreatedDate = y.CreatedDate,
                        PersonInChargeId = y.PersonInChargeId,
                        SellerName = "",
                        IsSendQuote = y.IsSendQuote,
                        LeadId = y.LeadId,
                        SaleBiddingId = y.SaleBiddingId,
                        ApprovalStep = y.ApprovalStep,
                        InvestmentFundId = y.InvestmentFundId
                    }).FirstOrDefault();

                    if (quote.Seller != null && quote.Seller != Guid.Empty)
                    {
                        var empSeller = context.Employee.FirstOrDefault(x => x.EmployeeId == quote.Seller);
                        sellerQuote = new EmployeeEntityModel()
                        {
                            EmployeeId = empSeller.EmployeeId,
                            EmployeeCode = empSeller.EmployeeCode,
                            EmployeeName = empSeller.EmployeeName,
                            IsManager = empSeller.IsManager,
                            PositionId = empSeller.PositionId,
                            OrganizationId = empSeller.OrganizationId,
                            Active = empSeller.Active
                        };
                    }

                    if (quote.PersonInChargeId != null)
                    {
                        employeeAssigned = context.Employee.Where(x => x.EmployeeId == quote.PersonInChargeId)
                            .Select(y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName
                            }).FirstOrDefault();
                    }

                    if (quote.ObjectType == "CUSTOMER")
                    {
                        customerAssigned = context.Customer.Where(x => x.CustomerId == quote.ObjectTypeId).Select(y =>
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
                                MaximumDebtDays = y.MaximumDebtDays,
                                MaximumDebtValue = y.MaximumDebtValue,
                                PersonInChargeId = y.PersonInChargeId
                            }).FirstOrDefault();
                    }
                    else if (quote.ObjectType == "LEAD")
                    {
                        leadAssigned = context.Lead.Where(x => x.LeadId == quote.ObjectTypeId).Select(y =>
                            new LeadEntityModel
                            {
                                LeadId = y.LeadId,
                                FullName = "",
                                Email = "",
                                Phone = "",
                                FullAddress = ""
                            }).FirstOrDefault();
                    }

                    #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 0)

                    var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                                where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                                select (new QuoteDetailEntityModel
                                                {
                                                    Active = cod.Active,
                                                    CreatedById = cod.CreatedById,
                                                    QuoteId = cod.QuoteId,
                                                    VendorId = cod.VendorId,
                                                    CreatedDate = cod.CreatedDate,
                                                    CurrencyUnit = cod.CurrencyUnit,
                                                    Description = cod.Description,
                                                    DiscountType = cod.DiscountType,
                                                    DiscountValue = cod.DiscountValue,
                                                    ExchangeRate = cod.ExchangeRate,
                                                    QuoteDetailId = cod.QuoteDetailId,
                                                    OrderDetailType = cod.OrderDetailType,
                                                    ProductId = cod.ProductId.Value,
                                                    UpdatedById = cod.UpdatedById,
                                                    Quantity = cod.Quantity,
                                                    UnitId = cod.UnitId,
                                                    IncurredUnit = cod.IncurredUnit,
                                                    UnitPrice = cod.UnitPrice,
                                                    UpdatedDate = cod.UpdatedDate,
                                                    Vat = cod.Vat,
                                                    NameVendor = "",
                                                    NameProduct = "",
                                                    NameProductUnit = "",
                                                    NameMoneyUnit = "",
                                                    IsPriceInitial = cod.IsPriceInitial,
                                                    PriceInitial = cod.PriceInitial,
                                                    ProductName = cod.ProductName,
                                                    SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                        cod.DiscountValue, cod.DiscountType),
                                                    OrderNumber = cod.OrderNumber
                                                })).ToList();

                    if (listQuoteObjectType0 != null)
                    {
                        List<Guid> listVendorId = new List<Guid>();
                        List<Guid> listProductId = new List<Guid>();
                        List<Guid> listCategoryId = new List<Guid>();
                        listQuoteObjectType0.ForEach(item =>
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

                        listQuoteObjectType0.ForEach(item =>
                        {
                            if (item.VendorId != null && item.VendorId != Guid.Empty)
                                item.NameVendor = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId).VendorName;
                            if (item.ProductId != null && item.ProductId != Guid.Empty)
                                item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId).ProductName;
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                            if (item.UnitId != null && item.UnitId != Guid.Empty)
                                item.NameProductUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId).CategoryName;
                            //item.NameGene = item.NameProduct + "(" + getNameGEn(item.QuoteDetailId) + ")";
                            item.NameGene = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId).ProductCode;
                            item.QuoteProductDetailProductAttributeValue = getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                        });
                    }

                    listQuoteDetail.AddRange(listQuoteObjectType0);


                    #endregion

                    #region Lấy chi tiết báo giá theo sản phẩm dịch vụ (OrderDetailType = 1)

                    var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                                where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                                select (new QuoteDetailEntityModel
                                                {
                                                    Active = cod.Active,
                                                    CreatedById = cod.CreatedById,
                                                    QuoteId = cod.QuoteId,
                                                    VendorId = cod.VendorId,
                                                    CreatedDate = cod.CreatedDate,
                                                    CurrencyUnit = cod.CurrencyUnit,
                                                    Description = cod.Description,
                                                    DiscountType = cod.DiscountType,
                                                    DiscountValue = cod.DiscountValue,
                                                    ExchangeRate = cod.ExchangeRate,
                                                    QuoteDetailId = cod.QuoteDetailId,
                                                    OrderDetailType = cod.OrderDetailType,
                                                    ProductId = cod.ProductId.Value,
                                                    UpdatedById = cod.UpdatedById,
                                                    Quantity = cod.Quantity,
                                                    UnitId = cod.UnitId,
                                                    IncurredUnit = cod.IncurredUnit,
                                                    UnitPrice = cod.UnitPrice,
                                                    UpdatedDate = cod.UpdatedDate,
                                                    ProductName = cod.ProductName,
                                                    Vat = cod.Vat,
                                                    NameVendor = "",
                                                    NameProduct = "",
                                                    NameProductUnit = "",
                                                    NameMoneyUnit = "",
                                                    SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                        cod.DiscountValue, cod.DiscountType),
                                                    OrderNumber = cod.OrderNumber
                                                })).ToList();

                    if (listQuoteObjectType1 != null)
                    {
                        List<Guid> listCategoryId = new List<Guid>();
                        listQuoteObjectType1.ForEach(item =>
                        {
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                listCategoryId.Add(item.CurrencyUnit.Value);
                        });
                        var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                        listQuoteObjectType1.ForEach(item =>
                        {
                            if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                                item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit).CategoryName;
                        });
                    }

                    listQuoteDetail.AddRange(listQuoteObjectType1);

                    listQuoteDetail = listQuoteDetail.OrderBy(z => z.OrderNumber).ToList();

                    #endregion

                    #region Lấy list file đính kèm của báo giá

                    listQuoteDocument = (from QD in context.QuoteDocument
                                         where QD.QuoteId == parameter.QuoteId
                                         select new QuoteDocumentEntityModel
                                         {
                                             QuoteDocumentId = QD.QuoteDocumentId,
                                             QuoteId = QD.QuoteId,
                                             DocumentName = QD.DocumentName,
                                             DocumentSize = QD.DocumentSize,
                                             DocumentUrl = QD.DocumentUrl,
                                             CreatedById = QD.CreatedById,
                                             CreatedDate = QD.CreatedDate,
                                             UpdatedById = QD.UpdatedById,
                                             UpdatedDate = QD.UpdatedDate,
                                             Active = QD.Active,
                                         }).ToList();

                    #endregion

                    #region Lấy list thông tin bổ sung của báo giá

                    listAdditionalInformation = context.AdditionalInformation
                        .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                        .Select(y =>
                            new AdditionalInformationEntityModel
                            {
                                AdditionalInformationId = y.AdditionalInformationId,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                Title = y.Title,
                                Content = y.Content,
                                Ordinal = y.Ordinal
                            }).OrderBy(z => z.Ordinal).ToList();

                    #endregion

                    #region Lấy list note(ghi chú)

                    listNote = context.Note
                        .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                        .Select(
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

                    #endregion

                    #region Lấy list chi phí của báo giá

                    var quoteCost = context.QuoteCostDetail
                        .Where(c => c.QuoteId == parameter.QuoteId && c.Active == true).ToList();
                    quoteCost.ForEach(item =>
                    {
                        var cost = context.Cost.FirstOrDefault(c => c.CostId == item.CostId);
                        QuoteCostDetailEntityModel obj = new QuoteCostDetailEntityModel();
                        obj.QuoteCostDetailId = item.QuoteCostDetailId;
                        obj.CostId = item.CostId;
                        obj.QuoteId = item.QuoteId;
                        obj.Quantity = item.Quantity;
                        obj.UnitPrice = item.UnitPrice;
                        obj.CostName = cost.CostName;
                        obj.CostCode = cost.CostCode;
                        obj.Active = item.Active;
                        obj.CreatedById = item.CreatedById;
                        obj.CreatedDate = item.CreatedDate;
                        obj.UpdatedById = item.UpdatedById;
                        obj.UpdatedDate = item.UpdatedDate;

                        listQuoteCostDetail.Add(obj);
                    });

                    #endregion

                    #region Kiểm tra điều kiện để được phê duyệt báo giá

                    var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                    // lấy trạng thái chờ phê duyệt báo giá
                    var statusQuote = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TGI");
                    var statusQuoteDLY = context.Category.FirstOrDefault(c =>
                        c.CategoryTypeId == statusQuote.CategoryTypeId && c.CategoryCode == "DLY");

                    if (quote.ApprovalStep != null && quote.StatusId == statusQuoteDLY.CategoryId)
                    {
                        var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws =>
                            ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == quote.ApprovalStep);

                        if (workFlowStep == null)
                        {
                            workFlowStep = context.WorkFlowSteps.Where(x => x.WorkflowId == workFlows.WorkFlowId)
                                .OrderByDescending(z => z.StepNumber).FirstOrDefault();
                        }

                        if ((workFlowStep.ApprovebyPosition && workFlowStep.ApproverPositionId == employee.PositionId)
                            || (!workFlowStep.ApprovebyPosition && workFlowStep.ApproverId == employee.EmployeeId))
                        {
                            isAproval = true;
                        }
                    }

                    #endregion

                    #region Lấy người liên quan

                    listParticipantId = context.QuoteParticipantMapping
                        .Where(x => x.QuoteId == parameter.QuoteId && x.EmployeeId != null)
                        .Select(y => y.EmployeeId.Value).ToList();

                    #endregion
                }

                #endregion

                #region Lấy list phương thức thanh toán và list status của báo giá

                //List Payment Method
                var paymentMethodCategoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true).CategoryTypeId;
                var listPaymentMethod = context.Category
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
                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true).CategoryTypeId;
                var listQuoteStatus = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
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

                #endregion

                #region Lấy list Tỉnh, Huyện, Phường-Xã

                var listProvince = context.Province.ToList();
                var listDistrict = context.District.ToList();
                var listWard = context.Ward.ToList();

                #endregion

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

                    listCustomer = customerAllList
                        .Where(x => (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.PersonInChargeId.Value)) &&
                                    x.Active == true)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerEmailWork = "",
                            CustomerEmailOther = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            CustomerCompany = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listLead = context.Lead
                        .Where(x => (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.PersonInChargeId.Value)) &&
                                    x.StatusId == statusLeadXN.CategoryId &&
                                    x.Active == true)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            Email = "",
                            Phone = "",
                            FullAddress = "",
                            ListLeadDetail = null,
                            LeadCode = y.LeadCode,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listSaleBidding = context.SaleBidding
                        .Where(x => (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.PersonInChargeId)) &&
                                    x.StatusId == statusSaleBiddingTT.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingName = y.SaleBiddingName,
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            SaleBiddingCode = y.SaleBiddingCode,
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = null
                        }).ToList();
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

                    listCustomer = customerAllList
                        .Where(x => x.PersonInChargeId == employee.EmployeeId && x.Active == true).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerCode = y.CustomerCode,
                                CustomerName = y.CustomerName,
                                CustomerGroupId = y.CustomerGroupId,
                                CustomerEmail = "",
                                CustomerEmailWork = "",
                                CustomerEmailOther = "",
                                CustomerPhone = "",
                                FullAddress = "",
                                CustomerCompany = "",
                                StatusId = y.StatusId,
                                MaximumDebtDays = y.MaximumDebtDays,
                                MaximumDebtValue = y.MaximumDebtValue,
                                PersonInChargeId = y.PersonInChargeId
                            }).ToList();

                    listLead = context.Lead
                        .Where(x => x.PersonInChargeId == employee.EmployeeId &&
                                    x.StatusId == statusLeadXN.CategoryId &&
                                    x.Active == true)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            Email = "",
                            Phone = "",
                            FullAddress = "",
                            LeadCode = y.LeadCode,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listSaleBidding = context.SaleBidding
                        .Where(x => x.PersonInChargeId == employee.EmployeeId &&
                                    x.StatusId == statusSaleBiddingTT.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingName = y.SaleBiddingName,
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingDetail = null
                        }).ToList();
                }

                if (sellerQuote != null)
                {
                    var obj = listEmployee.Where(e => e.EmployeeId == sellerQuote.EmployeeId).ToList();
                    if (obj.Count() == 0)
                    {
                        listEmployee.Add(sellerQuote);
                        listEmployee.Distinct();
                    }
                }

                #region Thêm dữ liệu Assigned

                /*
                 * Trong trường hợp xem/sửa chi tiết của Báo giá, nếu user đăng nhập không được phân quyền dữ liệu đối
                 * với các dữ liệu được phân quyền dữ liệu như: listEmployee, listCustomer, listLead thì
                 * lấy thông tin của các loại dữ liệu ấy trong Báo giá và thêm vào các dữ liệu ấy
                 */
                if (parameter.QuoteId != null)
                {
                    if (employeeAssigned.EmployeeId != null && employeeAssigned.EmployeeId != Guid.Empty)
                    {
                        var checkAssignedEmp =
                            listEmployee.FirstOrDefault(x => x.EmployeeId == employeeAssigned.EmployeeId);
                        if (checkAssignedEmp == null)
                        {
                            listEmployee.Add(employeeAssigned);
                            listEmployee = listEmployee.OrderBy(x => x.EmployeeName).ToList();
                        }
                    }

                    if (customerAssigned.CustomerId != null && customerAssigned.CustomerId != Guid.Empty)
                    {
                        var checAssignedCus = listCustomer.FirstOrDefault(x => x.CustomerId == customerAssigned.CustomerId);
                        if (checAssignedCus == null)
                        {
                            listCustomer.Add(customerAssigned);
                        }
                    }

                    if (leadAssigned.LeadId != null && leadAssigned.LeadId != Guid.Empty)
                    {
                        var checkAssignedLea = listLead.FirstOrDefault(x => x.LeadId == leadAssigned.LeadId);
                        if (checkAssignedLea == null)
                        {
                            listLead.Add(leadAssigned);
                        }
                    }
                }

                #endregion

                #region Lấy thêm các customer, lead chưa có người phụ trách theo phân quyền dữ liệu

                //Lấy thêm những customer chưa có người phụ trách theo phân quyền dữ liệu
                var listCustomerNoPersonInCharge =
                    context.Customer.Where(x =>
                        x.PersonInChargeId == null && x.CreatedById == user.UserId && x.Active == true
                        ).Select(y =>
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
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                listCustomerNoPersonInCharge.ForEach(item =>
                {
                    bool checkDublicate;
                    checkDublicate = !listCustomer.Contains(item);
                    if (checkDublicate)
                    {
                        listCustomer.Add(item);
                    }
                });

                //Lấy thêm những lead chưa có người phụ trách theo phân quyền dữ liệu
                var listLeadNoPersonInCharge = context.Lead
                    .Where(x => x.PersonInChargeId == null &&
                                x.StatusId == statusLeadXN.CategoryId &&
                                x.CreatedById.ToLower() == user.UserId.ToString().ToLower() && x.Active == true)
                    .Select(y => new LeadEntityModel
                    {
                        LeadId = y.LeadId,
                        CustomerId = y.CustomerId,
                        FullName = "",
                        Email = "",
                        Phone = "",
                        FullAddress = ""
                    }).ToList();

                listLeadNoPersonInCharge.ForEach(item =>
                {
                    bool checkDublicate;
                    checkDublicate = !listLead.Contains(item);
                    if (checkDublicate)
                    {
                        listLead.Add(item);
                    }
                });

                listLead.ForEach(item =>
                {
                    item.ListLeadDetail = leadDetailList.Where(ld => ld.LeadId == item.LeadId && ld.Active == true)
                        .Select(ld => new LeadDetailModel
                        {
                            LeadId = ld.LeadId,
                            LeadDetailId = ld.LeadDetailId,
                            VendorId = ld.VendorId,
                            ProductId = ld.ProductId,
                            Quantity = ld.Quantity,
                            UnitPrice = ld.UnitPrice,
                            CurrencyUnit = ld.CurrencyUnit,
                            ExchangeRate = ld.ExchangeRate,
                            Vat = ld.Vat,
                            DiscountType = ld.DiscountType,
                            DiscountValue = ld.DiscountValue,
                            Description = ld.Description,
                            OrderDetailType = ld.OrderDetailType,
                            UnitId = ld.UnitId,
                            IncurredUnit = ld.IncurredUnit,
                            ProductName = ld.ProductName,
                            ProductCode = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : productList.FirstOrDefault(p => p.ProductId == ld.ProductId).ProductCode,
                            NameMoneyUnit = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit).CategoryName,
                            ProductNameUnit = categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId).CategoryName,
                            NameVendor = vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                ? ""
                                : vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                            LeadProductDetailProductAttributeValue = null
                        }).ToList();
                });

                listLead.ForEach(item =>
                {
                    var contactObj = contactList.FirstOrDefault(c => c.ObjectId == item.LeadId);
                    if (contactObj != null)
                    {
                        item.ContactId = contactObj.ContactId;
                    }
                    item.ListLeadDetail.ForEach(detail =>
                    {
                        detail.LeadProductDetailProductAttributeValue = leadProductDetailProductAttributeValueList
                            .Where(c => c.LeadDetailId == detail.LeadDetailId)
                            .Select(ld => new LeadProductDetailProductAttributeValueModel
                            {
                                LeadProductDetailProductAttributeValue1 = ld.LeadProductDetailProductAttributeValue1,
                                LeadDetailId = ld.LeadDetailId,
                                ProductId = ld.ProductId,
                                ProductAttributeCategoryId = ld.ProductAttributeCategoryId,
                                ProductAttributeCategoryValueId = ld.ProductAttributeCategoryValueId
                            }).ToList();
                    });
                });

                listSaleBidding.ForEach(item =>
                {
                    item.SaleBiddingDetail = costQuoteList
                        .Where(ld => ld.SaleBiddingId == item.SaleBiddingId && ld.CostsQuoteType == 2)
                        .Select(ld => new CostQuoteModel
                        {
                            SaleBiddingId = ld.SaleBiddingId,
                            CostsQuoteId = ld.CostsQuoteId,
                            VendorId = ld.VendorId,
                            ProductId = ld.ProductId,
                            Quantity = ld.Quantity,
                            UnitPrice = ld.UnitPrice,
                            CurrencyUnit = ld.CurrencyUnit,
                            ExchangeRate = ld.ExchangeRate,
                            Vat = ld.Vat,
                            DiscountType = ld.DiscountType,
                            DiscountValue = ld.DiscountValue,
                            Description = ld.Description,
                            OrderDetailType = ld.OrderDetailType,
                            UnitId = ld.UnitId,
                            IncurredUnit = ld.IncurredUnit,
                            ProductCode = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : productList.FirstOrDefault(p => p.ProductId == ld.ProductId).ProductCode,
                            ProductName = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ld.Description
                                : productList.FirstOrDefault(p => p.ProductId == ld.ProductId).ProductName,
                            NameMoneyUnit = productList.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit).CategoryName,
                            ProductNameUnit = categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                ? ""
                                : categoryList.FirstOrDefault(cu => cu.CategoryId == ld.UnitId).CategoryName,
                            NameVendor = vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                ? ""
                                : vendorList.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                            SaleBiddingDetailProductAttribute = null
                        }).ToList();
                });
                listSaleBidding.ForEach(item =>
                {
                    item.SaleBiddingDetail.ForEach(detail =>
                    {
                        detail.SaleBiddingDetailProductAttribute = saleBiddingDetailProductAttributeList
                            .Where(c => c.SaleBiddingDetailId == detail.CostsQuoteId)
                            .Select(ld => new SaleBiddingDetailProductAttributeEntityModel
                            {
                                SaleBiddingDetailProductAttributeId = ld.SaleBiddingDetailProductAttributeId,
                                SaleBiddingDetailId = ld.SaleBiddingDetailId,
                                ProductId = ld.ProductId,
                                ProductAttributeCategoryId = ld.ProductAttributeCategoryId,
                                ProductAttributeCategoryValueId = ld.ProductAttributeCategoryValueId
                            }).ToList();
                    });
                });

                #endregion

                #region Lấy Email, Phone, Address, Name cho listCustomer và listLead 

                List<Guid> listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                List<Guid> listLeadId = listLead.Select(x => x.LeadId.Value).ToList();

                //danh sach khach hang (tiem nang va dinh danh)
                var listContactCustomer = context.Contact.Where(x =>
                        (listCustomerId.Count == 0 || listCustomerId.Contains(x.ObjectId)) &&
                        (x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS"))
                    .ToList();

                //danh sach co hoi
                var listContactLead = context.Contact.Where(x =>
                    (listLeadId.Count == 0 || listLeadId.Contains(x.ObjectId)) && x.ObjectType == "LEA").ToList();

                listCustomer.ForEach(item =>
                {
                    var customerContact = listContactCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId) ??
                                          new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.CustomerEmail = customerContact?.Email?.Trim() ?? "";
                    item.CustomerEmailOther = customerContact?.OtherEmail?.Trim() ?? "";
                    item.CustomerEmailWork = customerContact?.WorkEmail?.Trim() ?? "";
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + wardName + districtName + provinceName;
                    item.CustomerCompany = customerContact?.CompanyName?.Trim() ?? "";
                });

                listLead.ForEach(item =>
                {
                    var leadContact = listContactLead.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    var firstName = leadContact?.FirstName ?? "";
                    var lastName = leadContact?.LastName ?? "";
                    var address = leadContact?.Address ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == leadContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == leadContact.DistrictId);
                    var districtName = district?.DistrictName?.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == leadContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.Email = leadContact?.Email?.Trim() ?? "";
                    item.Phone = leadContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + provinceName + districtName + wardName;
                    item.FullName = firstName + " " + lastName;
                });

                #endregion

                #region Lấy nguồn tiềm năng

                var investFundList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                #endregion

                listCustomer = listCustomer.OrderBy(x => x.CustomerName).ToList();
                listLead = listLead.OrderBy(x => x.FullName).ToList();

                var categoryTypeTHA =
                    context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "THA");
                var categoryNew = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "MOI" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);
                var categoryHDO = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "HDO" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);

                var customerDD = listCustomer.Where(d => d.StatusId == categoryHDO.CategoryId).ToList();
                var customerTD = listCustomer.Where(d => d.StatusId == categoryNew.CategoryId).ToList();

                var listContactAllCustomer = context.Contact.Where(x =>
                        ((x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS")))
                    .ToList();

                var customerAll = customerAllList.Select(y =>
                    new CustomerEntityModel
                    {
                        CustomerId = y.CustomerId,
                        CustomerCode = y.CustomerCode,
                        CustomerName = y.CustomerName,
                        CustomerGroupId = y.CustomerGroupId,
                        CustomerEmail = "",
                        CustomerEmailWork = "",
                        CustomerEmailOther = "",
                        CustomerPhone = "",
                        FullAddress = "",
                        CustomerCompany = "",
                        StatusId = y.StatusId,
                        MaximumDebtDays = y.MaximumDebtDays,
                        MaximumDebtValue = y.MaximumDebtValue,
                        PersonInChargeId = y.PersonInChargeId,
                        StatusName = y.StatusId == categoryHDO.CategoryId ? "DD" : "TD"
                    }).ToList();

                customerAll.ForEach(item =>
                {
                    //add by dungpt
                    var customerContact = listContactAllCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId) ??
                                          new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.CustomerEmail = customerContact?.Email?.Trim() ?? "";
                    item.CustomerEmailOther = customerContact?.OtherEmail?.Trim() ?? "";
                    item.CustomerEmailWork = customerContact?.WorkEmail?.Trim() ?? "";
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + wardName + districtName + provinceName;
                    item.CustomerCompany = customerContact?.CompanyName?.Trim() ?? "";
                });

                #region Lấy list ProductVendorMapping

                var listProductVendorMapping = new List<ProductVendorMappingEntityModel>();
                listProductVendorMapping = context.ProductVendorMapping.Where(x => x.Active == true).Select(y =>
                    new ProductVendorMappingEntityModel
                    {
                        ProductVendorMappingId = y.ProductVendorMappingId,
                        ProductId = y.ProductId,
                        VendorId = y.VendorId,
                        VendorCode = vendorList.FirstOrDefault(z => z.VendorId == y.VendorId).VendorCode,
                        VendorName = vendorList.FirstOrDefault(z => z.VendorId == y.VendorId).VendorName
                    }).ToList();

                #endregion

                #region Lấy List người tham gia

                var listParticipant = new List<EmployeeEntityModel>();
                listParticipant = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode.Trim(),
                    EmployeeName = y.EmployeeName.Trim(),
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                #endregion

                return new GetDataCreateUpdateQuoteResult()
                {
                    Status = true,
                    Message = "Success",
                    IsAprovalQuote = isAproval,
                    ListCustomerAll = customerAll,
                    ListCustomer = customerDD,
                    ListCustomerNew = customerTD,
                    ListLead = listLead,
                    ListPaymentMethod = listPaymentMethod,
                    ListQuoteStatus = listQuoteStatus,
                    ListEmployee = listEmployee.Distinct().ToList(),
                    Quote = quote,
                    ListQuoteDetail = listQuoteDetail,
                    ListQuoteCostDetail = listQuoteCostDetail,
                    ListQuoteDocument = listQuoteDocument,
                    ListAdditionalInformation = listAdditionalInformation,
                    ListAdditionalInformationTemplates = listAdditionalInformationTemplates,
                    ListNote = listNote,
                    ListInvestFund = investFundList?.OrderBy(w => w.CategoryName).ToList() ??
                                     new List<CategoryEntityModel>(),
                    ListSaleBidding = listSaleBidding,
                    ListProductVendorMapping = listProductVendorMapping,
                    ListParticipant = listParticipant,
                    ListParticipantId = listParticipantId
                };
            }
            catch (Exception e)
            {
                return new GetDataCreateUpdateQuoteResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }
        public GetEmployeeSaleResult GetEmployeeSale(GetEmployeeSaleParameter parameter)
        {
            try
            {
                var listEmployee = new List<EmployeeEntityModel>();
                //Lấy người phụ trách của Khách hàng
                var eployeePerInChange = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);

                //Nếu người phụ trách của khách hàng không phải là Quản lý
                if (!eployeePerInChange.IsManager)
                {
                    listEmployee.Add(new EmployeeEntityModel
                    {
                        EmployeeId = eployeePerInChange.EmployeeId,
                        EmployeeCode = eployeePerInChange.EmployeeCode,
                        EmployeeName = eployeePerInChange.EmployeeCode + " - " + eployeePerInChange.EmployeeName,
                        IsManager = eployeePerInChange.IsManager,
                        PositionId = eployeePerInChange.PositionId,
                        OrganizationId = eployeePerInChange.OrganizationId,
                        EmployeeCodeName = eployeePerInChange.EmployeeCode + " - " + eployeePerInChange.EmployeeName
                    });
                }
                //Nếu người phụ trách của khách hàng là Quản lý
                else
                {
                    // Lấy nhân viên cấp dưới cùng phòng ban
                    listEmployee = parameter.ListEmployeeByAccount
                        .Where(x => x.OrganizationId == eployeePerInChange.OrganizationId.Value).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                            }).OrderBy(z => z.EmployeeName).ToList();

                    #region Giang comment

                    //// Thêm nhân viên quản lý là cùng phòng ban nó
                    //var empList = parameter.ListEmployeeByAccount.Where(e =>
                    //    e.IsManager == true && e.OrganizationId == eployeePerInChange.OrganizationId).ToList();

                    //empList.ForEach(item =>
                    //{
                    //    var emp = listEmployee.Where(e => e.EmployeeId == item.EmployeeId).ToList();
                    //    if (emp.Count() == 0)
                    //    {
                    //        listEmployee.Add(new EmployeeEntityModel
                    //        {
                    //            EmployeeId = item.EmployeeId,
                    //            EmployeeCode = item.EmployeeCode,
                    //            EmployeeName = item.EmployeeCode + " - " + item.EmployeeName,
                    //            IsManager = item.IsManager,
                    //            PositionId = item.PositionId,
                    //            OrganizationId = item.OrganizationId,
                    //            EmployeeCodeName = item.EmployeeCode + " - " + item.EmployeeName
                    //        });
                    //    }
                    //});

                    #endregion

                    // Lấy nhân viên phòng ban dưới nó
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(eployeePerInChange.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(eployeePerInChange.OrganizationId.Value, listGetAllChild);

                    // Bỏ phòng ban chính nó
                    listGetAllChild.Remove(eployeePerInChange.OrganizationId.Value);

                    var listEmployeeIsManager = parameter.ListEmployeeByAccount
                       .Where(x => (listGetAllChild.Contains(x.OrganizationId))).Select(
                           y => new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               IsManager = y.IsManager,
                               PositionId = y.PositionId,
                               OrganizationId = y.OrganizationId,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                           }).OrderBy(z => z.EmployeeName).ToList();

                    listEmployeeIsManager.ForEach(item =>
                    {
                        listEmployee.Add(item);
                    });
                }
                return new GetEmployeeSaleResult()
                {
                    Status = true,
                    Message = "Success",
                    ListEmployee = listEmployee.OrderBy(z => z.EmployeeName).ToList()
                };
            }
            catch (Exception e)
            {
                return new GetEmployeeSaleResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DownloadTemplateProductResult DownloadTemplateProduct(DownloadTemplateProductParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_import_BOM_lines.xls";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateProductResult
                {
                    TemplateExcel = data,
                    Message = string.Format("Đã dowload file Template_import_BOM_lines"),
                    FileName = "Template_import_BOM_lines",
                    Status = true
                };

            }
            catch (Exception)
            {
                return new DownloadTemplateProductResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình download",
                    Status = false
                };
            }
        }
        public GetDataQuoteAddEditProductDialogResult GetDataQuoteAddEditProductDialog(
            GetDataQuoteAddEditProductDialogParameter parameter)
        {
            try
            {
                var categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI" && x.Active == true);
                var listUnitMoney = context.Category
                    .Where(x => x.CategoryTypeId == categoryType.CategoryTypeId && x.Active == true).Select(
                        y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName,
                            IsDefault = y.IsDefauld
                        }).ToList();

                var categoryTypeUnitProduct = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH" && x.Active == true);
                var listUintProduct = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeUnitProduct.CategoryTypeId && x.Active == true).Select(
                        y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        }).ToList();

                var listProduct = context.Product.Where(x => x.Active == true).Select(x => new ProductEntityModel
                {
                    ProductId = x.ProductId,
                    ProductCategoryId = x.ProductCategoryId,
                    ProductName = x.ProductName,
                    ProductCode = x.ProductCode,
                    Price1 = x.Price1,
                    Price2 = x.Price2,
                    CreatedById = x.CreatedById,
                    CreatedDate = x.CreatedDate,
                    UpdatedById = x.UpdatedById,
                    UpdatedDate = x.UpdatedDate,
                    Active = x.Active,
                    Quantity = x.Quantity,
                    ProductUnitId = x.ProductUnitId,
                    //ProductUnitName = x.ProductUnitName,
                    ProductDescription = x.ProductDescription,
                    Vat = x.Vat,
                    MinimumInventoryQuantity = x.MinimumInventoryQuantity,
                    ProductMoneyUnitId = x.ProductMoneyUnitId,
                    //ProductCategoryName = x.ProductCategoryName,
                    //ListVendorName = x.ListVendorName,
                    Guarantee = x.Guarantee,
                    GuaranteeTime = x.GuaranteeTime,
                    //CountProductInformation = x.GuaranteeTime,
                    ExWarehousePrice = x.ExWarehousePrice,
                    CalculateInventoryPricesId = x.CalculateInventoryPricesId,
                    PropertyId = x.PropertyId,
                    WarehouseAccountId = x.WarehouseAccountId,
                    RevenueAccountId = x.RevenueAccountId,
                    PayableAccountId = x.PayableAccountId,
                    ImportTax = x.ImportTax,
                    CostPriceAccountId = x.CostPriceAccountId,
                    AccountReturnsId = x.AccountReturnsId,
                    FolowInventory = x.FolowInventory,
                    ManagerSerialNumber = x.ManagerSerialNumber,
                    ProductCodeName = x.ProductCode + " - " + x.ProductName
                }).ToList();

                var listVendor = context.Vendor.Where(x => x.Active == true).Select(y => new VendorEntityModel
                {
                    VendorId = y.VendorId,
                    VendorCode = y.VendorCode,
                    VendorName = y.VendorName
                }).ToList();

                var date = DateTime.Now.Date.Add(new TimeSpan(23, 59, 59));
                var listPriceProduct = context.PriceProduct.Where(x => x.Active == true && x.EffectiveDate <= date).ToList() ?? new List<PriceProduct>();

                return new GetDataQuoteAddEditProductDialogResult()
                {
                    Status = true,
                    Message = "Success",
                    ListUnitMoney = listUnitMoney,
                    ListUnitProduct = listUintProduct,
                    ListVendor = listVendor,
                    ListProduct = listProduct,
                    ListPriceProduct = listPriceProduct
                };
            }
            catch (Exception e)
            {
                return new GetDataQuoteAddEditProductDialogResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter)
        {
            try
            {
                var listVendorId = context.ProductVendorMapping.Where(x => x.ProductId == parameter.ProductId)
                    .Select(y => y.VendorId).ToList();

                var listVendor = new List<VendorEntityModel>();

                if (listVendorId.Count > 0)
                {
                    listVendor = context.Vendor
                        .Where(x => listVendorId.Contains(x.VendorId) && x.Active == true)
                        .Select(y => new VendorEntityModel
                        {
                            VendorId = y.VendorId,
                            VendorCode = y.VendorCode,
                            VendorName = y.VendorName
                        }).ToList();
                }

                #region Lấy list thuộc tính của sản phẩm

                var listObjectAttributeNameProduct = new List<ObjectAttributeNameProductModel>();
                var listObjectAttributeValueProduct = new List<ObjectAttributeValueProductModel>();

                var listProductAttribute =
                    context.ProductAttribute.Where(x => x.ProductId == parameter.ProductId).ToList();

                List<Guid> listProductAttributeCategoryId = new List<Guid>();
                listProductAttribute.ForEach(item =>
                {
                    listProductAttributeCategoryId.Add(item.ProductAttributeCategoryId);
                });

                if (listProductAttributeCategoryId.Count > 0)
                {
                    listObjectAttributeNameProduct = context.ProductAttributeCategory
                        .Where(x => listProductAttributeCategoryId.Contains(x.ProductAttributeCategoryId))
                        .Select(y => new ObjectAttributeNameProductModel
                        {
                            ProductAttributeCategoryId = y.ProductAttributeCategoryId,
                            ProductAttributeCategoryName = y.ProductAttributeCategoryName
                        })
                        .ToList();

                    listObjectAttributeValueProduct = context.ProductAttributeCategoryValue
                        .Where(x => listProductAttributeCategoryId.Contains(x.ProductAttributeCategoryId))
                        .Select(y => new ObjectAttributeValueProductModel
                        {
                            ProductAttributeCategoryValueId = y.ProductAttributeCategoryValueId,
                            ProductAttributeCategoryValue = y.ProductAttributeCategoryValue1,
                            ProductAttributeCategoryId = y.ProductAttributeCategoryId
                        })
                        .ToList();
                }

                #endregion

                decimal priceProduct = 0;

                if (parameter.CustomerGroupId != null && parameter.CustomerGroupId != Guid.Empty)
                {
                    var listPriceProduct = context.PriceProduct
                        .Where(x => x.Active && x.EffectiveDate.Date <= parameter.OrderDate.Value.Date &&
                                    x.ProductId == parameter.ProductId &&
                                    x.CustomerGroupCategory == parameter.CustomerGroupId)
                        .OrderByDescending(z => z.EffectiveDate)
                        .ToList();

                    var price = listPriceProduct.FirstOrDefault();

                    if (price != null)
                    {
                        priceProduct = price.PriceVnd;
                    }
                }

                return new GetVendorByProductIdResult()
                {
                    Status = true,
                    Message = "Success",
                    ListVendor = listVendor,
                    ListObjectAttributeNameProduct = listObjectAttributeNameProduct,
                    ListObjectAttributeValueProduct = listObjectAttributeValueProduct,
                    PriceProduct = priceProduct
                };
            }
            catch (Exception e)
            {
                return new GetVendorByProductIdResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
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

        private decimal CalculatorAmount(bool DiscountType, decimal DiscountValue, decimal Amount)
        {
            decimal result;
            if (DiscountType)
            {
                result = Amount - (Amount * DiscountValue) / 100;
                return result;
            }

            result = Amount - DiscountValue;
            return result;
        }

        private string GenerateCustomerCode(int maxCode)
        {
            //Auto gen CustomerCode 1911190001
            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;
            int currentDate = DateTime.Now.Day;
            int MaxNumberCode = 0;
            if (maxCode == 0)
            {
                var customer = context.Customer.OrderByDescending(or => or.CreatedDate).FirstOrDefault();
                if (customer != null)
                {
                    var customerCode = customer.CustomerCode;
                    if (customerCode.Contains(currentYear.ToString()) && customerCode.Contains(currentMonth.ToString()) && customerCode.Contains(currentDate.ToString()))
                    {
                        try
                        {
                            customerCode = customerCode.Substring(customerCode.Length - 4);
                            if (customerCode != "")
                            {
                                MaxNumberCode = Convert.ToInt32(customerCode) + 1;
                            }
                            else
                            {
                                MaxNumberCode = 1;
                            }
                        }
                        catch
                        {
                            MaxNumberCode = 1;
                        }

                    }
                    else
                    {
                        MaxNumberCode = 1;
                    }
                }
                else
                {
                    MaxNumberCode = 1;
                }
            }
            else
            {
                MaxNumberCode = maxCode + 1;
            }
            return string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
        }

        public CreateCostResult CreateCost(CreateCostParameter parameter)
        {
            var typteStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DSP").CategoryTypeId;
            var listStatus = context.Category.Where(c => c.CategoryTypeId == typteStatusId).ToList();
            var listOrg = context.Organization.ToList();

            var cost = new Cost()
            {
                CostId = Guid.NewGuid(),
                CostCode = parameter.CostCode,
                CostName = parameter.CostName,
                StatusId = parameter.StatusId,
                OrganizationId = parameter.OrganzationId,
                Active = true,
                CreatedById = parameter.UserId,
                CreatedDate = DateTime.Now,
            };
            context.Cost.Add(cost);
            context.SaveChanges();

            var listCost = context.Cost.ToList();
            var listCostEntity = new List<CostEntityModel>();

            listCost.ForEach(item =>
            {
                var costEntity = new CostEntityModel()
                {
                    CostId = item.CostId,
                    CostCode = item.CostCode,
                    CostName = item.CostName,
                    StatusId = item.StatusId,
                    OrganizationId = item.OrganizationId,
                    Active = item.Active,
                    CreatedById = item.CreatedById,
                    CreatedDate = item.CreatedDate,
                    StatusName = listStatus.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "",
                    OrganizationName = listOrg.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? "",
                };
                listCostEntity.Add(costEntity);
            });
            return new CreateCostResult
            {
                Status = true,
                Message = CommonMessage.Cost.CREATE_SUCCESS,
                ListCost = listCostEntity,
            };
        }

        public UpdateCostResult UpdateCost(UpdateCostParameter parameter)
        {
            var typteStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DSP").CategoryTypeId;
            var listStatus = context.Category.Where(c => c.CategoryTypeId == typteStatusId).ToList();
            var listOrg = context.Organization.ToList();

            var cost = context.Cost.FirstOrDefault(c => c.CostId == parameter.CostId);
            if (cost != null)
            {
                cost.CostName = parameter.CostName;
                cost.CostCode = parameter.CostCode;
                cost.OrganizationId = parameter.OrganzationId;
                cost.StatusId = parameter.StatusId;
                cost.UpdatedDate = DateTime.Now;
                cost.UpdatedById = parameter.UserId;

                context.Cost.Update(cost);
                context.SaveChanges();
            }
            else
            {
                return new UpdateCostResult
                {
                    Status = false,
                    Message = "Không tồn tại chi phí!"
                };
            }

            var listCost = context.Cost.ToList();
            var listCostEntity = new List<CostEntityModel>();

            listCost.ForEach(item =>
            {
                var costEntity = new CostEntityModel()
                {
                    CostId = item.CostId,
                    CostCode = item.CostCode,
                    CostName = item.CostName,
                    StatusId = item.StatusId,
                    OrganizationId = item.OrganizationId,
                    Active = item.Active,
                    CreatedById = item.CreatedById,
                    CreatedDate = item.CreatedDate,
                    StatusName = listStatus.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "",
                    OrganizationName = listOrg.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? "",
                };
                listCostEntity.Add(costEntity);
            });

            return new UpdateCostResult
            {
                Status = true,
                Message = "Cập nhật chi phí thành công",
                ListCost = listCostEntity
            };
        }
        public GetMasterDataCreateCostResult GetMasterDataCreateCost(GetMasterDataCreateCostParameter parameter)
        {
            var typteStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DSP").CategoryTypeId;
            var listStatus = context.Category.Where(c => c.CategoryTypeId == typteStatusId).ToList();
            var listOrg = context.Organization.ToList();

            var listCost = context.Cost.Where(c => c.Active == true).ToList();
            var listCostEntity = new List<CostEntityModel>();

            listCost.ForEach(item =>
            {
                var costEntity = new CostEntityModel()
                {
                    CostId = item.CostId,
                    CostCode = item.CostCode,
                    CostName = item.CostName,
                    CostCodeName = item.CostCode + " - " + item.CostName,
                    StatusId = item.StatusId,
                    OrganizationId = item.OrganizationId,
                    Active = item.Active,
                    CreatedById = item.CreatedById,
                    CreatedDate = item.CreatedDate,
                    StatusName = listStatus.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "",
                    OrganizationName = listOrg.FirstOrDefault(c => c.OrganizationId == item.OrganizationId)?.OrganizationName ?? "",
                };
                listCostEntity.Add(costEntity);
            });

            if (parameter.UserId != null && parameter.UserId != Guid.Empty)
            {
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;

                var employees = context.Employee.Where(e => e.Active == true).ToList();

                var employee = employees.FirstOrDefault(e => e.EmployeeId == employeeId);

                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                listCostEntity = listCostEntity
                    .Where(c => listGetAllChild.Contains(c.OrganizationId) || c.OrganizationId == null).ToList();
            }

            return new GetMasterDataCreateCostResult
            {
                Status = true,
                ListStatus = listStatus,
                ListCost = listCostEntity,
            };
        }

        public UpdateQuoteResult UpdateStatusQuote(GetQuoteByIDParameter parameter)
        {
            try
            {
                var employee = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var contact = context.Contact.Where(c => c.Active == true).ToList();
                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI");
                var quote = context.Quote.FirstOrDefault(q => q.QuoteId == parameter.QuoteId);
                string message = "";

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "QUOTE";
                note.ObjectId = quote.QuoteId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = "Đã thêm ghi chú";

                switch (parameter.ObjectType)
                {
                    case "SEND_APROVAL":
                        var categoryCHO = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "DLY");
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;
                        quote.ApprovalStep = 2;
                        quote.StatusId = categoryCHO.CategoryId;
                        context.Quote.Update(quote);

                        //var employeeApproval = StepApprovalQuote(quote.ApprovalStep, employeeId);
                        //employeeApproval.ForEach(item =>
                        //{
                        //    var emailApproval = contact.FirstOrDefault(e => e.ObjectId == item && e.ObjectType == "EMP");
                        //    if (emailApproval.Email != null)
                        //    {
                        //        GetConfiguration();
                        //        string webRootPath = _hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
                        //        var file = Path.Combine(webRootPath, "SendEmailQuoteApprove.html");
                        //        string body = string.Empty;
                        //        using (StreamReader reader = new StreamReader(file))
                        //        {
                        //            body = reader.ReadToEnd();
                        //        }

                        //        //Thay doi cac thuoc tinh can thiet trong htmltemplate
                        //        body = body.Replace("[NameApprove]", emailApproval.FirstName + " " + emailApproval.LastName);
                        //        body = body.Replace("[QuoteName]", quote.QuoteCode);
                        //        body = body.Replace("[TotalAmount]", string.Format("{0:#,0}", quote.Amount));
                        //        body = body.Replace("{forgotUrl}", Domain + "/customer/quote-detail;quoteId=" + quote.QuoteId);

                        //        MailMessage mail = new MailMessage();
                        //        SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                        //        mail.From = new MailAddress(Email, "N8");
                        //        mail.To.Add(emailApproval.Email); // Email người nhận
                        //        mail.Subject = string.Format("Yêu cầu phê duyệt báo giá {0}", quote.QuoteCode);
                        //        mail.Body = body;
                        //        mail.IsBodyHtml = true;
                        //        SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                        //        SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                        //        SmtpServer.Send(mail);
                        //    }
                        //});
                        message = "Gửi phê duyệt thành công";

                        note.Description = "Đã gửi phê duyệt thành công";
                        break;
                    case "CANCEL_QUOTE":
                        var categoryHUY = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "HUY");
                        quote.StatusId = categoryHUY.CategoryId;
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        context.Quote.Update(quote);
                        message = "Hủy báo giá thành công";
                        note.Description = "Chuyển trạng thái hủy báo giá thành công";
                        break;
                    case "APPROVAL_QUOTE":
                        var categoryBG = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "DTH");
                        quote.StatusId = categoryBG.CategoryId;
                        quote.SendQuoteDate = DateTime.Now;
                        quote.UpdatedById = parameter.UserId;

                        quote.UpdatedDate = DateTime.Now;

                        if (quote.LeadId != null && quote.LeadId != Guid.Empty)
                        {
                            var quoteByLead = context.Quote.Where(q => q.LeadId == quote.LeadId).ToList();
                            var quoteByLeadClose = quoteByLead.Where(q => q.StatusId == categoryBG.CategoryId).ToList();
                            if (quoteByLead.Count() == quoteByLeadClose.Count())
                            {
                                var categoryTypeLead = context.CategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "CHS");
                                var categoryTypeLeadClose = context.Category.FirstOrDefault(ca => ca.CategoryTypeId == categoryTypeLead.CategoryTypeId && ca.CategoryCode == "CLOSE");
                                var lead = context.Lead.FirstOrDefault(l => l.LeadId == quote.LeadId);
                                lead.StatusId = categoryTypeLeadClose.CategoryId;
                                lead.UpdatedDate = DateTime.Now;

                                context.Lead.Update(lead);
                                context.SaveChanges();
                            }
                        }
                        if (quote.SaleBiddingId != null && quote.SaleBiddingId != Guid.Empty)
                        {
                            //var quoteByLead = context.Quote.Where(q => q.LeadId == quote.LeadId).ToList();
                            //var quoteByLeadClose = quoteByLead.Where(q => q.StatusId == categoryBG.CategoryId).ToList();
                            //if (quoteByLead.Count() == quoteByLeadClose.Count())
                            //{
                            //    var categoryTypeLead = context.CategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "CHS");
                            //    var categoryTypeLeadClose = context.Category.FirstOrDefault(ca => ca.CategoryTypeId == categoryTypeLead.CategoryTypeId && ca.CategoryCode == "CLOSE");
                            //    var lead = context.Lead.FirstOrDefault(l => l.LeadId == quote.LeadId);
                            //    lead.StatusId = categoryTypeLeadClose.CategoryId;

                            //    context.Lead.Update(lead);
                            //    context.SaveChanges();
                            //}
                        }

                        context.Quote.Update(quote);
                        note.Description = "Xác nhận báo giá thành công";
                        message = "Xác nhận báo giá thành công";
                        break;
                    case "NEW_QUOTE":
                        var categoryMTA = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "MTA");
                        quote.StatusId = categoryMTA.CategoryId;
                        quote.IsSendQuote = false;
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        context.Quote.Update(quote);
                        note.Description = "Đặt lại về trạng thái Mới thành công";
                        message = "";
                        break;
                    case "CANCEL_APROVAL":
                        quote.StatusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "MTA").CategoryId;
                        quote.IsSendQuote = false;
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        context.Quote.Update(quote);
                        note.Description = "Báo giá được hủy gửi phê duyệt bởi nhân viên: " + employee.UserName;
                        message = "Yêu cầu phê duyệt đã được hủy";
                        break;
                }

                context.Note.Add(note);
                context.SaveChanges();

                #region Gửi thông báo

                switch (parameter.ObjectType)
                {
                    case "SEND_APROVAL":
                        //Gửi thông báo khi gửi phê duyệt báo giá
                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "SEND_APPROVAL", new Queue(),
                            quote, true);
                        break;
                    case "CANCEL_QUOTE":
                        //Gửi thông báo khi hủy báo giá
                        break;
                    case "APPROVAL_QUOTE":
                        //Gửi thông báo khi xác nhận báo giá
                        break;
                    case "NEW_QUOTE":
                        //Gửi thông báo khi đặt về nháp báo giá
                        break;
                    case "CANCEL_APROVAL":
                        //var configEntity = context.SystemParameter.ToList();

                        //var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE").CategoryTypeId;

                        //var listEmailTempType =
                        //    context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

                        //var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "HGPD")
                        //    .CategoryId;

                        //var emailTemplate = context.EmailTemplate.FirstOrDefault(w => w.Active && w.EmailTemplateTypeId == emailCategoryId);

                        //#region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        //var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        //var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        //#endregion

                        //#region Lấy danh sách email cần gửi thông báo

                        //var listEmailSendTo = new List<string>();

                        //#region Lấy email người tham gia

                        ////var listParticipantId = context.QuoteParticipantMapping
                        ////    .Where(x => x.QuoteId == quote.QuoteId).Select(y => y.EmployeeId).ToList();

                        ////if (listParticipantId.Count > 0)
                        ////{
                        ////    var listEmailParticipant = listAllContact.Where(x =>
                        ////            listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                        ////        .Select(y => y.Email)
                        ////        .ToList();

                        ////    listEmailParticipant.ForEach(email =>
                        ////    {
                        ////        if (!String.IsNullOrEmpty(email))
                        ////        {
                        ////            listEmailSendTo.Add(email.Trim());
                        ////        }
                        ////    });
                        ////}


                        //#endregion

                        //#region Lấy email người phê duyệt

                        ////Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá
                        //var listApproved =
                        //    GetListEmployeeApproved(context, "PDBG", quote.ApprovalStep,
                        //        listAllEmployee);

                        //var listEmailManager = listAllContact
                        //    .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                        //    .Select(y => y.Email).ToList();

                        //listEmailManager.ForEach(emailManager =>
                        //{
                        //    if (!String.IsNullOrEmpty(emailManager))
                        //    {
                        //        listEmailSendTo.Add(emailManager.Trim());
                        //    }
                        //});

                        //#endregion

                        //#region Lấy email người tạo

                        ////Người tạo
                        //var employeeId =
                        //    context.User.FirstOrDefault(x => x.UserId == quote.CreatedById)
                        //        ?.EmployeeId;

                        //var email_created = "";

                        //if (employeeId != null)
                        //{
                        //    email_created = listAllContact.FirstOrDefault(x =>
                        //        x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                        //    if (!String.IsNullOrEmpty(email_created))
                        //    {
                        //        listEmailSendTo.Add(email_created.Trim());
                        //    }
                        //}

                        //#endregion

                        //#region Lấy email người hủy phê duyệt

                        //var empId =
                        //    context.User.FirstOrDefault(x => x.UserId == quote.UpdatedById)
                        //        ?.EmployeeId;

                        //var email_cancel = "";

                        //if (employeeId != null)
                        //{
                        //    email_cancel = listAllContact.FirstOrDefault(x =>
                        //        x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                        //    if (!String.IsNullOrEmpty(email_cancel))
                        //    {
                        //        listEmailSendTo.Add(email_cancel.Trim());
                        //    }
                        //}

                        //#endregion

                        //#region Lấy email người phụ trách (Nhân viên bán hàng)

                        //var email_seller = listAllContact.FirstOrDefault(x =>
                        //    x.ObjectId == quote.Seller && x.ObjectType == "EMP")?.Email;

                        //if (!String.IsNullOrEmpty(email_seller))
                        //{
                        //    listEmailSendTo.Add(email_seller.Trim());
                        //}

                        //#endregion

                        //listEmailSendTo = listEmailSendTo.Distinct().ToList();

                        //#endregion

                        //var subject = ReplaceTokenForContent(context, quote, emailTemplate.EmailTemplateTitle,
                        //    configEntity);
                        //var content = ReplaceTokenForContent(context, quote, emailTemplate.EmailTemplateContent,
                        //    configEntity);

                        //Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);

                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "CANCEL_APPROVAL", new Queue(),
                            quote, true);
                        break;
                }

                #endregion

                return new UpdateQuoteResult
                {
                    Status = true,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        private static List<Guid> GetListEmployeeApproved(TNTN8Context context, string WorkflowCode, int? StepNumber, List<Employee> ListAllEmployee)
        {
            var result = new List<Guid>();

            var workflow = context.WorkFlows.FirstOrDefault(x => x.WorkflowCode == WorkflowCode);

            if (workflow != null)
            {
                var workflowSteps = context.WorkFlowSteps.Where(x => x.WorkflowId == workflow.WorkFlowId).ToList();

                if (workflowSteps.Count > 0)
                {
                    //Nếu đối tượng có trạng thái Mới tạo (StepNumber == null) hoặc có trạng thái Từ chối (StepNumber == 0)
                    if (StepNumber == 0 || StepNumber == null)
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == 2);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                    else
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == StepNumber);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity)
        {
            var result = emailContent;

            #region Common Token

            const string Logo = "[LOGO]";
            const string QuoteCode = "[QUOTE_CODE]";
            const string QuoteName = "[QUOTE_NAME]";
            const string EmployeeName = "[EMPLOYEE_NAME]";
            const string EmployeeCode = "[EMPLOYEE_CODE]";
            const string UpdatedDate = "[UPDATED_DATE]";
            const string Url_Login = "[URL]";

            #endregion

            var _model = model as Quote;

            #region Replace token

            #region replace logo

            if (result.Contains(Logo))
            {
                var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                if (!String.IsNullOrEmpty(logo))
                {
                    var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                    result = result.Replace(Logo, temp_logo);
                }
                else
                {
                    result = result.Replace(Logo, "");
                }
            }

            #endregion

            #region replace quote name

            if (result.Contains(QuoteName) && _model.QuoteName != null)
            {
                result = result.Replace(QuoteName, _model.QuoteName.Trim());
            }

            #endregion

            #region replace quote code

            if (result.Contains(QuoteCode) && _model.QuoteCode != null)
            {
                result = result.Replace(QuoteCode, _model.QuoteCode.Trim());
            }

            #endregion

            #region replaca change employee code

            if (result.Contains(EmployeeCode))
            {
                var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                if (!String.IsNullOrEmpty(employeeCode))
                {
                    result = result.Replace(EmployeeCode, employeeCode);
                }
                else
                {
                    result = result.Replace(EmployeeCode, "");
                }
            }

            #endregion

            #region replace change employee name

            if (result.Contains(EmployeeName))
            {
                var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                if (!String.IsNullOrEmpty(employeeName))
                {
                    result = result.Replace(EmployeeName, employeeName);
                }
                else
                {
                    result = result.Replace(EmployeeName, "");
                }
            }

            #endregion

            #region replace updated date

            if (result.Contains(UpdatedDate))
            {
                result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
            }

            #endregion

            #region replace url 

            if (result.Contains(Url_Login))
            {
                var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                if (!String.IsNullOrEmpty(loginLink))
                {
                    result = result.Replace(Url_Login, loginLink);
                }
            }

            #endregion
            
            #endregion

            return result;
        }

        private static string FormatDateToString(DateTime? date)
        {
            var result = "";

            if (date != null)
            {
                result = date.Value.Day.ToString("00") + "/" +
                         date.Value.Month.ToString("00") + "/" +
                         date.Value.Year.ToString("0000") + " " +
                         date.Value.Hour.ToString("00") + ":" +
                         date.Value.Minute.ToString("00");
            }

            return result;
        }


        public UpdateQuoteResult SendEmailCustomerQuote(SendEmailCustomerQuoteParameter parameter)
        {
            try
            {
                var quote = context.Quote.FirstOrDefault(q => q.QuoteId == parameter.QuoteId);
                quote.IsSendQuote = true;

                var now = DateTime.Now;
                var _day = now.Day.ToString("D2");
                var _month = now.Month.ToString("D2");
                var _year = (now.Year % 100).ToString();

                string folderName = "Báo giá_" + quote.QuoteCode + ".pdf";
                string webRootPath = _hostingEnvironment.WebRootPath + "\\ExportedPDFQuote\\";
                if (!Directory.Exists(webRootPath))
                {
                    Directory.CreateDirectory(webRootPath);
                }
                string newPath = Path.Combine(webRootPath, folderName);

                if (!File.Exists(newPath))
                {
                    Directory.Delete(webRootPath, true);
                    Directory.CreateDirectory(webRootPath);

                    byte[] imageBytes = Convert.FromBase64String(parameter.Base64Pdf);
                    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                    using (var stream = new FileStream(newPath, FileMode.Create))
                    {
                        ms.CopyTo(stream);
                    }
                }

                parameter.ListEmail.ForEach(item =>
                {
                    GetConfiguration();

                    Attachment attachment = new Attachment(newPath);

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                    mail.From = new MailAddress(Email, "N8");
                    mail.To.Add(item); // Email người nhận
                    mail.Subject = string.Format(parameter.TitleEmail);
                    mail.Body = parameter.ContentEmail;
                    mail.Attachments.Add(attachment);
                    mail.IsBodyHtml = true;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                    SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                    SmtpServer.Send(mail);
                });
                context.Quote.Update(quote);

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "QUOTE";
                note.ObjectId = quote.QuoteId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = "Đã thêm ghi chú";
                note.Description = "Gửi mail báo giá khách hàng thành công";

                context.Note.Add(note);
                context.SaveChanges();

                return new UpdateQuoteResult
                {
                    Status = true,
                    Message = "Gửi mail thành công"
                };
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetMasterDataCreateQuoteResult GetMasterDataCreateQuote(GetMasterDataCreateQuoteParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                #region List Kênh bán hàng

                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == "IVF")?
                    .CategoryTypeId;
                var listInvestFund = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                #endregion

                #region List người tham gia

                var listParticipant = new List<EmployeeEntityModel>();
                listParticipant = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode.Trim(),
                    EmployeeName = y.EmployeeName.Trim(),
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                #endregion

                #region List thông tin bổ sung mẫu của báo giá

                var additionalInformationTemplateId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "GCB" && x.Active == true)?.CategoryTypeId;
                var listAdditionalInformationTemplates = context.Category
                    .Where(x => x.CategoryTypeId == additionalInformationTemplateId).Select(y =>
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

                #endregion

                #region List phương thức thanh toán

                var paymentMethodCategoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true)?.CategoryTypeId;
                var listPaymentMethod = context.Category
                    .Where(x => x.CategoryTypeId == paymentMethodCategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region List trạng thái báo giá

                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true)?.CategoryTypeId;
                var listQuoteStatus = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region Lấy Data theo phân quyền dữ liệu

                #region Các List Common

                var listCommonEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listCommonCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var listCommonContact = context.Contact
                    .Where(x => x.Active == true &&
                                (x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS" || x.ObjectType == "LEA"))
                    .ToList();

                //Cơ hội
                var listCommonLead = context.Lead.Where(x => x.Active == true).ToList();
                var listCommonLeadDetail = context.LeadDetail.Where(x => x.Active == true).ToList();
                var listCommonLeadProductDetailProductAttributeValue =
                    context.LeadProductDetailProductAttributeValue.ToList();

                //Hồ sơ thầu
                var listCommonSaleBidding = context.SaleBidding.Where(x => x.Active == true).ToList();
                var listCommonCostQuote = context.CostsQuote.ToList();
                var listCommonSaleBiddingDetailProductAttribute =
                    context.SaleBiddingDetailProductAttribute.ToList();

                var listProvince = context.Province.ToList();
                var listDistrict = context.District.ToList();
                var listWard = context.Ward.ToList();

                var listCommonCategory = context.Category.Where(x => x.Active == true).ToList();
                var listCommonVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listCommonProduct = context.Product.Where(x => x.Active == true).ToList();

                #endregion

                #region Các list data cần lấy theo phân quyền dữ liệu

                //List nhân viên bán hàng (Người phụ trách)
                var listEmployee = new List<EmployeeEntityModel>();

                //List khách hàng định danh
                var listCustomer = new List<CustomerEntityModel>();

                //List khách hàng tiềm năng
                var listCustomerNew = new List<CustomerEntityModel>();

                //List Cơ hội có trạng thái Xác nhận
                var listAllLead = new List<LeadEntityModel>();

                //List Hồ sơ thầu có trạng thái Đã duyệt
                var listAllSaleBidding = new List<SaleBiddingEntityModel>();

                #endregion

                #region Các trạng thái cần dùng để lọc các list data

                //Lấy TypeId Trạng thái khách hàng
                var customerTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA")
                    ?.CategoryTypeId;

                //Trạng thái Khách hàng Định danh
                var statusIdentityCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "HDO" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Trạng thái Khách hàng Tiềm năng
                var statusNewCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "MOI" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Lấy TypeId Trạng thái Cơ hội
                var leadTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS");

                //Trạng thái Xác nhận của Cơ hội
                var statusApprLead = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == leadTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                //Lấy TypeId Trạng thái Hồ sơ thầu
                var saleBiddingTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST");

                //Trạng thái Đã duyệt của Hồ sơ thầu
                var statusApprSaleBidding = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == saleBiddingTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                #endregion

                //Nếu là Quản lý
                if (employee?.IsManager == true)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employee.OrganizationId
                    };
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId, listGetAllChild);

                    listEmployee = listCommonEmployee
                        .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }
                //Nếu là Nhân viên
                else if (employee?.IsManager == false)
                {
                    listEmployee = listCommonEmployee
                        .Where(x => x.EmployeeId == employee.EmployeeId).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }

                #region Lấy thông tin Email, Điện thoại và Địa chỉ của Khách hàng định danh

                listCustomer.ForEach(item =>
                {
                    var customerContact =
                        listCommonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId) ?? new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.CustomerEmail = customerContact?.Email?.Trim() ?? "";
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = (address + wardName + districtName + provinceName).Trim();

                    var length = item.FullAddress.Length;
                    var checkLength = item.FullAddress.LastIndexOf(",");
                    if ((checkLength + 1) == length && length != 0)
                    {
                        item.FullAddress = item.FullAddress.Substring(0, length - 1);
                    }
                });

                #endregion

                #region Lấy thông tin Email, Điện thoại và Địa chỉ của Khách hàng tiềm năng

                listCustomerNew.ForEach(item =>
                {
                    var customerContact =
                        listCommonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId) ?? new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : wardName;

                    item.CustomerEmail = item.CustomerType == 1 ? customerContact?.WorkEmail?.Trim() : customerContact?.Email?.Trim();
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + wardName + districtName + provinceName;

                    var length = item.FullAddress.Length;
                    var checkLength = item.FullAddress.LastIndexOf(",");
                    if ((checkLength + 1) == length && length != 0)
                    {
                        item.FullAddress = item.FullAddress.Substring(0, length - 1);
                    }
                });

                #endregion

                #region Lấy thông tin FullName của Cơ hội

                listAllLead.ForEach(item =>
                {
                    var leadContact = listCommonContact.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    var firstName = leadContact?.FirstName ?? "";
                    var lastName = leadContact?.LastName ?? "";
                    item.FullName = firstName + " " + lastName;
                    item.LeadCodeName = item.LeadCode?.Trim() + " - " + item.FullName;
                    item.ContactId = leadContact.ContactId;
                });

                #endregion

                #endregion

                #region Lấy các dữ liệu ngoại lệ nếu có

                if (parameter.ObjectId != null && !String.IsNullOrEmpty(parameter.ObjectType))
                {
                    //Báo giá được tạo từ Cơ hội
                    if (parameter.ObjectType == "LEAD")
                    {

                    }
                    //Báo giá được tạo từ Hồ sơ thầu
                    else if (parameter.ObjectType == "SALEBIDDING")
                    {

                    }
                    //Báo giá được tạo từ Khách hàng (Định danh hoặc Tiềm năng)
                    else if (parameter.ObjectType == "CUSTOMER")
                    {

                    }
                }

                #endregion

                return new GetMasterDataCreateQuoteResult()
                {
                    Status = true,
                    Message = "Success",
                    ListInvestFund = listInvestFund,
                    ListAdditionalInformationTemplates = listAdditionalInformationTemplates,
                    ListPaymentMethod = listPaymentMethod,
                    ListQuoteStatus = listQuoteStatus,
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListCustomerNew = listCustomerNew,
                    ListAllLead = listAllLead,
                    ListAllSaleBidding = listAllSaleBidding,
                    ListParticipant = listParticipant
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateQuoteResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetEmployeeByPersonInChargeResult GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeParameter parameter)
        {
            try
            {
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId);

                var listEmployee = new List<EmployeeEntityModel>();

                //Nếu người phụ trách là Quản lý
                if (employee.IsManager == true)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employee.OrganizationId
                    };
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId, listGetAllChild);

                    listEmployee = context.Employee
                        .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();
                }
                //Nếu người phụ trách là Nhân viên
                else
                {
                    listEmployee.Add(new EmployeeEntityModel
                    {
                        EmployeeId = employee.EmployeeId,
                        EmployeeCode = employee.EmployeeCode,
                        EmployeeName = employee.EmployeeName,
                        EmployeeCodeName = employee.EmployeeCode.Trim() + " - " + employee.EmployeeName.Trim(),
                        IsManager = employee.IsManager,
                        PositionId = employee.PositionId,
                        OrganizationId = employee.OrganizationId,
                        Active = employee.Active
                    });
                }

                return new GetEmployeeByPersonInChargeResult()
                {
                    Status = true,
                    Message = "Success",
                    ListEmployee = listEmployee
                };
            }
            catch (Exception e)
            {
                return new GetEmployeeByPersonInChargeResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataUpdateQuoteResult GetMasterDataUpdateQuote(GetMasterDataUpdateQuoteParameter parameter)
        {
            try
            {
                var quote = context.Quote.Where(x => x.QuoteId == parameter.QuoteId).Select(y => new QuoteEntityModel
                {
                    QuoteId = y.QuoteId,
                    QuoteCode = y.QuoteCode,
                    QuoteDate = y.QuoteDate,
                    QuoteName = y.QuoteName,
                    SendQuoteDate = y.SendQuoteDate,
                    Seller = y.Seller,
                    EffectiveQuoteDate = y.EffectiveQuoteDate,
                    ExpirationDate = y.ExpirationDate,
                    Description = y.Description,
                    Note = y.Note,
                    ObjectTypeId = y.ObjectTypeId,
                    ObjectType = y.ObjectType,
                    CustomerContactId = y.CustomerContactId,
                    PaymentMethod = y.PaymentMethod,
                    DiscountType = y.DiscountType,
                    BankAccountId = y.BankAccountId,
                    DaysAreOwed = y.DaysAreOwed,
                    MaxDebt = y.MaxDebt,
                    ReceivedDate = y.ReceivedDate,
                    Amount = y.Amount,
                    DiscountValue = y.DiscountValue,
                    IntendedQuoteDate = y.IntendedQuoteDate,
                    StatusId = y.StatusId,
                    CreatedDate = y.CreatedDate,
                    PersonInChargeId = y.PersonInChargeId,
                    SellerName = "",
                    IsSendQuote = y.IsSendQuote,
                    LeadId = y.LeadId,
                    SaleBiddingId = y.SaleBiddingId,
                    ApprovalStep = y.ApprovalStep,
                    InvestmentFundId = y.InvestmentFundId,
                    UpdatedById = y.UpdatedById,
                    UpdatedDate = y.UpdatedDate
                }).FirstOrDefault();

                if (quote == null)
                {
                    return new GetMasterDataUpdateQuoteResult()
                    {
                        Status = false,
                        Message = "Báo giá không tồn tại trên hệ thống"
                    };
                }

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                #region List Kênh bán hàng

                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == "IVF")?
                    .CategoryTypeId;
                var listInvestFund = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                #endregion

                #region List người tham gia

                var listParticipant = new List<EmployeeEntityModel>();
                listParticipant = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode.Trim(),
                    EmployeeName = y.EmployeeName.Trim(),
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                #endregion

                #region List phương thức thanh toán

                var paymentMethodCategoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true)?.CategoryTypeId;
                var listPaymentMethod = context.Category
                    .Where(x => x.CategoryTypeId == paymentMethodCategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region List trạng thái báo giá

                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true)?.CategoryTypeId;
                var listQuoteStatus = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region Lấy Data theo phân quyền dữ liệu

                #region Các List Common

                var listCommonEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listCommonCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var listCommonContact = context.Contact
                    .Where(x => x.Active == true &&
                                (x.ObjectType == "CUS" || x.ObjectType == "POTENT_CUS" || x.ObjectType == "LEA"))
                    .ToList();

                //Cơ hội
                var listCommonLead = context.Lead.Where(x => x.Active == true).ToList();
                var listCommonLeadDetail = context.LeadDetail.Where(x => x.Active == true).ToList();
                var listCommonLeadProductDetailProductAttributeValue =
                    context.LeadProductDetailProductAttributeValue.ToList();

                //Hồ sơ thầu
                var listCommonSaleBidding = context.SaleBidding.Where(x => x.Active == true).ToList();
                var listCommonCostQuote = context.CostsQuote.ToList();
                var listCommonSaleBiddingDetailProductAttribute =
                    context.SaleBiddingDetailProductAttribute.ToList();

                var listProvince = context.Province.ToList();
                var listDistrict = context.District.ToList();
                var listWard = context.Ward.ToList();

                var listCommonCategory = context.Category.Where(x => x.Active == true).ToList();
                var listCommonVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listCommonProduct = context.Product.Where(x => x.Active == true).ToList();

                #endregion

                #region Các list data cần lấy theo phân quyền dữ liệu

                //List nhân viên bán hàng (Người phụ trách)
                var listEmployee = new List<EmployeeEntityModel>();

                //List khách hàng định danh
                var listCustomer = new List<CustomerEntityModel>();

                //List khách hàng tiềm năng
                var listCustomerNew = new List<CustomerEntityModel>();

                //List Cơ hội có trạng thái Xác nhận
                var listAllLead = new List<LeadEntityModel>();

                //List Hồ sơ thầu có trạng thái Đã duyệt
                var listAllSaleBidding = new List<SaleBiddingEntityModel>();

                #endregion

                #region Các trạng thái cần dùng để lọc các list data

                //Lấy TypeId Trạng thái khách hàng
                var customerTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA")
                    ?.CategoryTypeId;

                //Trạng thái Khách hàng Định danh
                var statusIdentityCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "HDO" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Trạng thái Khách hàng Tiềm năng
                var statusNewCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "MOI" && x.CategoryTypeId == customerTypeId)?.CategoryId;

                //Lấy TypeId Trạng thái Cơ hội
                var leadTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS");

                //Trạng thái Xác nhận của Cơ hội
                var statusApprLead = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == leadTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                //Lấy TypeId Trạng thái Hồ sơ thầu
                var saleBiddingTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST");

                //Trạng thái Đã duyệt của Hồ sơ thầu
                var statusApprSaleBidding = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == saleBiddingTypeId.CategoryTypeId && c.CategoryCode == "APPR");

                #endregion

                //Nếu là Quản lý
                if (employee?.IsManager == true)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employee.OrganizationId
                    };
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId, listGetAllChild);

                    listEmployee = listCommonEmployee
                        .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }
                //Nếu là Nhân viên
                else if (employee?.IsManager == false)
                {
                    listEmployee = listCommonEmployee
                        .Where(x => x.EmployeeId == employee.EmployeeId).Select(
                            y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                IsManager = y.IsManager,
                                PositionId = y.PositionId,
                                OrganizationId = y.OrganizationId,
                                Active = y.Active
                            }).OrderBy(z => z.EmployeeName).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    listCustomer = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusIdentityCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listCustomerNew = listCommonCustomer
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusNewCustomer)
                        .Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerType = y.CustomerType ?? 1,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            StatusId = y.StatusId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                    listAllLead = listCommonLead
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value) &&
                                    x.StatusId == statusApprLead.CategoryId)
                        .Select(y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    listAllSaleBidding = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    listEmployeeId.Contains(x.PersonInChargeId) &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();
                }

                #region Nếu khách hàng của Báo giá không thuộc phân quyền dữ liệu của người đăng nhập

                var _customer = listCommonCustomer.Where(x => x.CustomerId == quote.ObjectTypeId).Select(y =>
                    new CustomerEntityModel
                    {
                        CustomerId = y.CustomerId,
                        CustomerCode = y.CustomerCode,
                        CustomerName = y.CustomerName,
                        CustomerType = y.CustomerType ?? 1,
                        CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName,
                        CustomerGroupId = y.CustomerGroupId,
                        CustomerEmail = "",
                        CustomerPhone = "",
                        FullAddress = "",
                        StatusId = y.StatusId,
                        MaximumDebtDays = y.MaximumDebtDays,
                        MaximumDebtValue = y.MaximumDebtValue,
                        PersonInChargeId = y.PersonInChargeId
                    }).FirstOrDefault();
                var exists_customer = listCustomer.FirstOrDefault(x => x.CustomerId == quote.ObjectTypeId);
                var exists_customerNew = listCustomerNew.FirstOrDefault(x => x.CustomerId == quote.ObjectTypeId);

                //Nếu không thuộc phân quyền dữ liệu thì
                if (exists_customer == null && exists_customerNew == null)
                {
                    //Nếu là khách hàng
                    if (_customer.StatusId == statusIdentityCustomer)
                    {
                        listCustomer.Add(_customer);
                    }
                    //Nếu là khách hàng tiềm năng
                    else if (_customer.StatusId == statusNewCustomer)
                    {
                        listCustomerNew.Add(_customer);
                    }

                    //Lấy thêm các cơ hội của khách hàng này
                    var listLeadForCustomer = listCommonLead.Where(x =>
                        x.CustomerId == _customer.CustomerId && x.PersonInChargeId != null &&
                        x.StatusId == statusApprLead.CategoryId).Select(
                        y => new LeadEntityModel
                        {
                            LeadId = y.LeadId,
                            CustomerId = y.CustomerId,
                            FullName = "",
                            LeadCode = y.LeadCode,
                            LeadCodeName = "",
                            PersonInChargeId = y.PersonInChargeId,
                            ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                        }).ToList();

                    //Nếu Cơ hội không nằm trong listAllLead thì thêm vào
                    listLeadForCustomer.ForEach(item =>
                    {
                        var existLead = listAllLead.FirstOrDefault(x => x.LeadId == item.LeadId);

                        if (existLead == null)
                        {
                            listAllLead.Add(item);
                        }
                    });

                    //Lấy thêm các Hồ sơ thầu của khách hàng này
                    var listSaleBiddingForCustomer = listCommonSaleBidding
                        .Where(x => x.PersonInChargeId != null &&
                                    x.CustomerId == _customer.CustomerId &&
                                    x.StatusId == statusApprSaleBidding.CategoryId)
                        .Select(y => new SaleBiddingEntityModel
                        {
                            SaleBiddingId = y.SaleBiddingId,
                            SaleBiddingCode = y.SaleBiddingCode,
                            SaleBiddingName = y.SaleBiddingName,
                            SaleBiddingCodeName = y.SaleBiddingCode.Trim() + " - " + y.SaleBiddingName.Trim(),
                            PersonInChargeId = y.PersonInChargeId,
                            LeadId = y.LeadId,
                            Email = "",
                            Phone = "",
                            CustomerId = y.CustomerId,
                            SaleBiddingDetail = listCommonCostQuote
                                .Where(ld => ld.SaleBiddingId == y.SaleBiddingId && ld.CostsQuoteType == 2)
                                .Select(ld => new CostQuoteModel
                                {
                                    SaleBiddingId = ld.SaleBiddingId,
                                    CostsQuoteId = ld.CostsQuoteId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    ProductName = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ld.Description
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductName,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(cu => cu.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    SaleBiddingDetailProductAttribute = listCommonSaleBiddingDetailProductAttribute
                                        .Where(c => c.SaleBiddingDetailId == ld.CostsQuoteId)
                                        .Select(attr => new SaleBiddingDetailProductAttributeEntityModel
                                        {
                                            SaleBiddingDetailProductAttributeId =
                                                attr.SaleBiddingDetailProductAttributeId,
                                            SaleBiddingDetailId = attr.SaleBiddingDetailId,
                                            ProductId = attr.ProductId,
                                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                        }).ToList()
                                }).ToList()
                        }).ToList();

                    //Nếu Hồ sơ thầu không nằm trong listAllSaleBidding thì thêm vào
                    listSaleBiddingForCustomer.ForEach(item =>
                    {
                        var existSaleBidding =
                            listAllSaleBidding.FirstOrDefault(x => x.SaleBiddingId == item.SaleBiddingId);

                        if (existSaleBidding == null)
                        {
                            listAllSaleBidding.Add(item);
                        }
                    });
                }

                #endregion

                #region Lấy thêm cơ hội trạng thái khác xác nhận

                var existsLead = listAllLead.FirstOrDefault(c => c.LeadId == quote.LeadId);
                if (existsLead == null)
                {
                    var leadExtend = context.Lead.Where(x => x.LeadId == quote.LeadId).Select(y => new LeadEntityModel
                    {
                        LeadId = y.LeadId,
                        CustomerId = y.CustomerId,
                        FullName = "",
                        LeadCode = y.LeadCode,
                        LeadCodeName = "",
                        PersonInChargeId = y.PersonInChargeId,
                        ListLeadDetail = listCommonLeadDetail.Where(ld => ld.LeadId == y.LeadId)
                                .Select(ld => new LeadDetailModel
                                {
                                    LeadId = ld.LeadId,
                                    LeadDetailId = ld.LeadDetailId,
                                    VendorId = ld.VendorId,
                                    ProductId = ld.ProductId,
                                    Quantity = ld.Quantity,
                                    UnitPrice = ld.UnitPrice,
                                    CurrencyUnit = ld.CurrencyUnit,
                                    ExchangeRate = ld.ExchangeRate,
                                    Vat = ld.Vat,
                                    DiscountType = ld.DiscountType,
                                    DiscountValue = ld.DiscountValue,
                                    Description = ld.Description,
                                    OrderDetailType = ld.OrderDetailType,
                                    UnitId = ld.UnitId,
                                    IncurredUnit = ld.IncurredUnit,
                                    ProductName = ld.ProductName,
                                    ProductCode = listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) ==
                                                  null
                                        ? ""
                                        : listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId)
                                            .ProductCode,
                                    NameMoneyUnit =
                                        listCommonProduct.FirstOrDefault(p => p.ProductId == ld.ProductId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.CurrencyUnit)
                                                .CategoryName,
                                    ProductNameUnit =
                                        listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId) == null
                                            ? ""
                                            : listCommonCategory.FirstOrDefault(c => c.CategoryId == ld.UnitId)
                                                .CategoryName,
                                    NameVendor = listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId) == null
                                        ? ""
                                        : listCommonVendor.FirstOrDefault(v => v.VendorId == ld.VendorId).VendorName,
                                    LeadProductDetailProductAttributeValue =
                                        listCommonLeadProductDetailProductAttributeValue
                                            .Where(c => c.LeadDetailId == ld.LeadDetailId)
                                            .Select(attr => new LeadProductDetailProductAttributeValueModel
                                            {
                                                LeadProductDetailProductAttributeValue1 =
                                                    attr.LeadProductDetailProductAttributeValue1,
                                                LeadDetailId = attr.LeadDetailId,
                                                ProductId = attr.ProductId,
                                                ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                                                ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                                            }).ToList()
                                }).ToList()
                    }).FirstOrDefault();

                    if (leadExtend != null)
                    {
                        listAllLead.Add(leadExtend);
                    }
                }

                #endregion

                #region Lấy thông tin Email, Điện thoại và Địa chỉ của Khách hàng định danh

                listCustomer.ForEach(item =>
                {
                    var customerContact =
                        listCommonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId) ?? new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : (wardName + ", ");

                    item.CustomerEmail = customerContact?.Email?.Trim() ?? "";
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = (address + wardName + districtName + provinceName).Trim();

                    var length = item.FullAddress.Length;
                    var checkLength = item.FullAddress.LastIndexOf(",");
                    if ((checkLength + 1) == length && length != 0)
                    {
                        item.FullAddress = item.FullAddress.Substring(0, length - 1);
                    }
                });

                #endregion

                #region Lấy thông tin Email, Điện thoại và Địa chỉ của Khách hàng tiềm năng

                listCustomerNew.ForEach(item =>
                {
                    var customerContact =
                        listCommonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId) ?? new Contact();
                    var address = customerContact?.Address?.Trim() ?? "";
                    address = address == "" ? "" : (address + ", ");
                    var province = listProvince.FirstOrDefault(x => x.ProvinceId == customerContact.ProvinceId);
                    var provinceName = province?.ProvinceName?.Trim() ?? "";
                    provinceName = provinceName == "" ? "" : (provinceName + ", ");
                    var district = listDistrict.FirstOrDefault(x => x.DistrictId == customerContact.DistrictId);
                    var districtName = district?.DistrictName.Trim() ?? "";
                    districtName = districtName == "" ? "" : (districtName + ", ");
                    var ward = listWard.FirstOrDefault(x => x.WardId == customerContact.WardId);
                    var wardName = ward?.WardName?.Trim() ?? "";
                    wardName = wardName == "" ? "" : wardName;

                    item.CustomerEmail = item.CustomerType == 1
                        ? customerContact?.WorkEmail?.Trim()
                        : customerContact?.Email?.Trim();
                    item.CustomerPhone = customerContact?.Phone?.Trim() ?? "";
                    item.FullAddress = address + wardName + districtName + provinceName;

                    var length = item.FullAddress.Length;
                    var checkLength = item.FullAddress.LastIndexOf(",");
                    if ((checkLength + 1) == length && length != 0)
                    {
                        item.FullAddress = item.FullAddress.Substring(0, length - 1);
                    }
                });

                #endregion

                #region Lấy thông tin FullName của Cơ hội

                listAllLead.ForEach(item =>
                {
                    var leadContact = listCommonContact.FirstOrDefault(x => x.ObjectId == item.LeadId);

                    var firstName = leadContact?.FirstName ?? "";
                    var lastName = leadContact?.LastName ?? "";
                    item.FullName = firstName + " " + lastName;
                    item.LeadCodeName = item.LeadCode?.Trim() + " - " + item.FullName;
                    item.ContactId = leadContact.ContactId;
                });

                #endregion

                #endregion

                #region Các thông tin của Báo giá

                var listQuoteDetail = new List<QuoteDetailEntityModel>();
                var listQuoteDocument = new List<QuoteDocumentEntityModel>();
                var listAdditionalInformation = new List<AdditionalInformationEntityModel>();
                var listNote = new List<NoteEntityModel>();
                var listQuoteCostDetail = new List<QuoteCostDetailEntityModel>();
                bool isApproval = false;
                var listParticipantId = new List<Guid>();

                #endregion

                #region Lấy chi tiết báo giá theo sản phẩm (OrderDetailType = 0)

                var listQuoteObjectType0 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 0
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                IsPriceInitial = cod.IsPriceInitial,
                                                PriceInitial = cod.PriceInitial,
                                                ProductName = cod.ProductName,
                                                SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                    cod.DiscountValue, cod.DiscountType),
                                                OrderNumber = cod.OrderNumber
                                            })).ToList();

                if (listQuoteObjectType0 != null)
                {
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType0.ForEach(item =>
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

                    listQuoteObjectType0.ForEach(item =>
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
                            item.ProductNameUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.UnitId)
                                .CategoryName;

                        item.ProductCode = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId).ProductCode;
                        item.QuoteProductDetailProductAttributeValue =
                            getListQuoteProductDetailProductAttributeValue(item.QuoteDetailId);
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType0);

                #endregion

                #region Lấy chi tiết báo giá theo dịch vụ (OrderDetailType = 1)

                var listQuoteObjectType1 = (from cod in context.QuoteDetail
                                            where cod.QuoteId == parameter.QuoteId && cod.OrderDetailType == 1
                                            select (new QuoteDetailEntityModel
                                            {
                                                Active = cod.Active,
                                                CreatedById = cod.CreatedById,
                                                QuoteId = cod.QuoteId,
                                                VendorId = cod.VendorId,
                                                CreatedDate = cod.CreatedDate,
                                                CurrencyUnit = cod.CurrencyUnit,
                                                Description = cod.Description,
                                                DiscountType = cod.DiscountType,
                                                DiscountValue = cod.DiscountValue,
                                                ExchangeRate = cod.ExchangeRate,
                                                QuoteDetailId = cod.QuoteDetailId,
                                                OrderDetailType = cod.OrderDetailType,
                                                ProductId = cod.ProductId.Value,
                                                UpdatedById = cod.UpdatedById,
                                                Quantity = cod.Quantity,
                                                UnitId = cod.UnitId,
                                                IncurredUnit = cod.IncurredUnit,
                                                UnitPrice = cod.UnitPrice,
                                                UpdatedDate = cod.UpdatedDate,
                                                ProductName = cod.ProductName,
                                                ProductCode = "",
                                                Vat = cod.Vat,
                                                NameVendor = "",
                                                NameProduct = "",
                                                NameProductUnit = "",
                                                NameMoneyUnit = "",
                                                ProductNameUnit = "",
                                                SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat,
                                                    cod.DiscountValue, cod.DiscountType),
                                                OrderNumber = cod.OrderNumber
                                            })).ToList();

                if (listQuoteObjectType1 != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            listCategoryId.Add(item.CurrencyUnit.Value);
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    listQuoteObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null && item.CurrencyUnit != Guid.Empty)
                            item.NameMoneyUnit = listCategory.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });
                }

                listQuoteDetail.AddRange(listQuoteObjectType1);

                listQuoteDetail = listQuoteDetail.OrderBy(z => z.OrderNumber).ToList();

                #endregion

                #region Lấy list file đính kèm của báo giá

                listQuoteDocument = (from QD in context.QuoteDocument
                                     where QD.QuoteId == parameter.QuoteId
                                     select new QuoteDocumentEntityModel
                                     {
                                         QuoteDocumentId = QD.QuoteDocumentId,
                                         QuoteId = QD.QuoteId,
                                         DocumentName = QD.DocumentName,
                                         DocumentSize = QD.DocumentSize,
                                         DocumentUrl = QD.DocumentUrl,
                                         CreatedById = QD.CreatedById,
                                         CreatedDate = QD.CreatedDate,
                                         UpdatedById = QD.UpdatedById,
                                         UpdatedDate = QD.UpdatedDate,
                                         Active = QD.Active,
                                     }).ToList();

                #endregion

                #region Lấy list thông tin bổ sung của báo giá

                listAdditionalInformation = context.AdditionalInformation
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                    .Select(y =>
                        new AdditionalInformationEntityModel
                        {
                            AdditionalInformationId = y.AdditionalInformationId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            Title = y.Title,
                            Content = y.Content,
                            Ordinal = y.Ordinal
                        }).OrderBy(z => z.Ordinal).ToList();

                #endregion

                #region Lấy list note (ghi chú)

                listNote = context.Note
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE" && x.Active == true)
                    .Select(
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

                #endregion

                #region Lấy list chi phí của báo giá

                var quoteCost = context.QuoteCostDetail
                    .Where(c => c.QuoteId == parameter.QuoteId && c.Active == true).ToList();
                quoteCost.ForEach(item =>
                {
                    var cost = context.Cost.FirstOrDefault(c => c.CostId == item.CostId);
                    QuoteCostDetailEntityModel obj = new QuoteCostDetailEntityModel();
                    obj.QuoteCostDetailId = item.QuoteCostDetailId;
                    obj.CostId = item.CostId;
                    obj.QuoteId = item.QuoteId;
                    obj.Quantity = item.Quantity;
                    obj.UnitPrice = item.UnitPrice;
                    obj.CostName = cost.CostName;
                    obj.CostCode = cost.CostCode;
                    obj.Active = item.Active;
                    obj.CreatedById = item.CreatedById;
                    obj.CreatedDate = item.CreatedDate;
                    obj.UpdatedById = item.UpdatedById;
                    obj.UpdatedDate = item.UpdatedDate;

                    listQuoteCostDetail.Add(obj);
                });

                #endregion

                #region Kiểm tra điều kiện để được phê duyệt báo giá

                var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                // lấy trạng thái chờ phê duyệt báo giá
                var statusQuote = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TGI");
                var statusQuoteDLY = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == statusQuote.CategoryTypeId && c.CategoryCode == "DLY");

                if (quote.ApprovalStep != null && quote.StatusId == statusQuoteDLY.CategoryId)
                {
                    var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws =>
                        ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == quote.ApprovalStep);

                    if (workFlowStep == null)
                    {
                        workFlowStep = context.WorkFlowSteps.Where(x => x.WorkflowId == workFlows.WorkFlowId)
                            .OrderByDescending(z => z.StepNumber).FirstOrDefault();
                    }

                    if ((workFlowStep.ApprovebyPosition && workFlowStep.ApproverPositionId == employee.PositionId)
                        || (!workFlowStep.ApprovebyPosition && workFlowStep.ApproverId == employee.EmployeeId))
                    {
                        isApproval = true;
                    }
                }

                #endregion

                #region Lấy người tham gia

                listParticipantId = context.QuoteParticipantMapping
                    .Where(x => x.QuoteId == parameter.QuoteId && x.EmployeeId != null)
                    .Select(y => y.EmployeeId.Value).ToList();

                #endregion

                #region Kiểm tra người đang đăng nhập có phải người tham gia không

                bool isParticipant = false;
                var existsParticipant = listParticipantId.FirstOrDefault(x => x == employee.EmployeeId);
                //Nếu là người tham gia và người tham gia không phải người phụ trách thì
                if (existsParticipant != null && existsParticipant != Guid.Empty &&
                    existsParticipant != quote.PersonInChargeId)
                {
                    isParticipant = true;

                    if (isApproval)
                    {
                        isParticipant = false;
                    }
                }

                #endregion

                #region Lấy list quà khuyến mãi

                var listPromotionObjectApply = context.PromotionObjectApply
                    .Where(x => x.ObjectId == parameter.QuoteId && x.ObjectType == "QUOTE").Select(y =>
                        new PromotionObjectApplyEntityModel
                        {
                            PromotionObjectApplyId = y.PromotionObjectApplyId,
                            ObjectId = y.ObjectId,
                            ObjectType = y.ObjectType,
                            PromotionId = y.PromotionId,
                            ConditionsType = y.ConditionsType,
                            PropertyType = y.PropertyType,
                            NotMultiplition = y.NotMultiplition,
                            PromotionMappingId = y.PromotionMappingId,
                            ProductId = y.ProductId,
                            SoLuongTang = y.SoLuongTang,
                            LoaiGiaTri = y.LoaiGiaTri,
                            GiaTri = y.GiaTri,
                            Amount = y.Amount,
                            SoTienTu = y.SoTienTu
                        }).ToList();

                if (listPromotionObjectApply.Count > 0)
                {
                    var listPromotionId = listPromotionObjectApply.Select(y => y.PromotionId).Distinct().ToList();
                    var listProductId = listPromotionObjectApply.Where(x => x.ProductId != null)
                        .Select(y => y.ProductId).Distinct().ToList();
                    var productUnitTypeId =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH")
                            .CategoryTypeId;
                    var listProductUnit = context.Category.Where(x => x.CategoryTypeId == productUnitTypeId)
                        .ToList();

                    var listPromotion = context.Promotion.Where(x => listPromotionId.Contains(x.PromotionId)).ToList();
                    var listProduct = context.Product.Where(x => listProductId.Contains(x.ProductId)).ToList();

                    listPromotionObjectApply.ForEach(item =>
                    {
                        var promotion = listPromotion.FirstOrDefault(x => x.PromotionId == item.PromotionId);

                        if (promotion != null)
                        {
                            item.PromotionName = promotion.PromotionName;
                        }

                        var product = listProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                        if (product != null)
                        {
                            item.PromotionProductName = product.ProductName;

                            var unitName = listProductUnit.FirstOrDefault(x => x.CategoryId == product.ProductUnitId);

                            if (unitName != null)
                            {
                                item.ProductUnitName = unitName.CategoryName;
                            }
                        }
                        else
                        {
                            item.PromotionProductName = "Phiếu giảm giá";
                        }
                    });
                }

                #endregion

                return new GetMasterDataUpdateQuoteResult()
                {
                    Status = true,
                    Message = "Success",
                    ListInvestFund = listInvestFund,
                    ListPaymentMethod = listPaymentMethod,
                    ListQuoteStatus = listQuoteStatus,
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListCustomerNew = listCustomerNew,
                    ListAllLead = listAllLead,
                    ListAllSaleBidding = listAllSaleBidding,
                    ListParticipant = listParticipant,
                    Quote = quote,
                    ListQuoteDetail = listQuoteDetail,
                    ListQuoteDocument = listQuoteDocument,
                    ListAdditionalInformation = listAdditionalInformation,
                    ListNote = listNote,
                    ListQuoteCostDetail = listQuoteCostDetail,
                    IsApproval = isApproval,
                    ListParticipantId = listParticipantId,
                    IsParticipant = isParticipant,
                    ListPromotionObjectApply = listPromotionObjectApply
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataUpdateQuoteResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public UpdateQuoteResult ApprovalOrRejectQuote(ApprovalOrRejectQuoteParameter parameter)
        {
            try
            {
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var contact = context.Contact.Where(c => c.Active == true).ToList();
                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TGI");
                if (parameter.IsApproval)
                {
                    //đồng ý phê duyệt
                    var category = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "CHO");
                    parameter.ListQuoteId = parameter.ListQuoteId.Distinct().ToList();
                    parameter.ListQuoteId.ForEach(quoteId =>
                    {
                        var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                        var quote = context.Quote.FirstOrDefault(q => q.QuoteId == quoteId);
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;

                        var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws =>
                            ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == quote.ApprovalStep);

                        if (workFlowStep == null)
                        {
                            workFlowStep = context.WorkFlowSteps.Where(x => x.WorkflowId == workFlows.WorkFlowId)
                                .OrderByDescending(z => z.StepNumber).FirstOrDefault();
                        }

                        quote.ApprovalStep = workFlowStep.NextStepNumber;
                        if (workFlowStep.NextStepNumber == 0)
                        {
                            quote.StatusId = category.CategoryId;
                            Note note = new Note();
                            note.NoteId = Guid.NewGuid();
                            note.ObjectType = "QUOTE";
                            note.ObjectId = quote.QuoteId;
                            note.Type = "ADD";
                            note.Description = "Chuyển trạng thái đã duyệt thành công";

                            if (!string.IsNullOrEmpty(parameter.Description))
                            {
                                note.Description = "Chuyển trạng thái đã duyệt thành công với lý do: " +
                                                   parameter.Description.Trim();
                            }

                            note.Active = true;
                            note.CreatedById = parameter.UserId;
                            note.CreatedDate = DateTime.Now;
                            note.NoteTitle = "Đã thêm ghi chú";

                            context.Note.Add(note);
                        }
                        else
                        {
                            var employeeApproval = StepApprovalQuote(quote.ApprovalStep, employeeId);
                            employeeApproval.ForEach(item =>
                            {
                                var emailApproval = contact.FirstOrDefault(e => e.ObjectId == item && e.ObjectType == "EMP");
                                if (emailApproval.Email != null)
                                {
                                    GetConfiguration();
                                    string webRootPath = _hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
                                    var file = Path.Combine(webRootPath, "SendEmailQuoteApprove.html");
                                    string body = string.Empty;
                                    using (StreamReader reader = new StreamReader(file))
                                    {
                                        body = reader.ReadToEnd();
                                    }

                                    //Thay doi cac thuoc tinh can thiet trong htmltemplate
                                    body = body.Replace("[NameApprove]", emailApproval.FirstName + " " + emailApproval.LastName);
                                    body = body.Replace("[QuoteName]", quote.QuoteCode);
                                    body = body.Replace("[TotalAmount]", string.Format("{0:#,0}", quote.Amount));
                                    body = body.Replace("{forgotUrl}", Domain + "/customer/quote-detail;quoteId=" + quote.QuoteId);

                                    MailMessage mail = new MailMessage();
                                    SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                                    mail.From = new MailAddress(Email, "N8");
                                    mail.To.Add(emailApproval.Email); // Email người nhận
                                    mail.Subject = string.Format("Yêu cầu phê duyệt báo giá {0}", quote.QuoteCode);
                                    mail.Body = body;
                                    mail.IsBodyHtml = true;
                                    SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                                    SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                                    SmtpServer.Send(mail);
                                }
                            });

                            Note note = new Note();
                            note.NoteId = Guid.NewGuid();
                            note.ObjectType = "QUOTE";
                            note.ObjectId = quote.QuoteId;
                            note.Type = "ADD";
                            note.Description = "Đã duyệt thành công, chuyển sang bước duyệt thứ: " + quote.ApprovalStep;

                            if (!string.IsNullOrEmpty(parameter.Description))
                            {
                                note.Description = "Đã duyệt thành công với lý do: " + parameter.Description.Trim() +
                                                   ", chuyển sang bước duyệt thứ: " +
                                                   quote.ApprovalStep;
                            }

                            note.Active = true;
                            note.CreatedById = parameter.UserId;
                            note.CreatedDate = DateTime.Now;
                            note.NoteTitle = "Đã thêm ghi chú";

                            context.Note.Add(note);
                        }

                        context.Quote.Update(quote);

                        context.SaveChanges();

                        #region Gửi thông báo

                        var _note = new Note();
                        _note.Description = parameter.Description;
                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "APPROVAL", new Queue(),
                            quote, true, _note);

                        #endregion

                    });

                    return new UpdateQuoteResult
                    {
                        Status = true,
                        Message = "Gửi phê duyệt thành công"
                    };
                }
                else
                {
                    //từ chối phê duyệt
                    var category = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "TUCHOI");
                    var statusCloseNotHit = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "DTR");
                    var statusNew = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "MTA");
                    parameter.ListQuoteId.ForEach(quoteId =>
                    {
                        var quote = context.Quote.FirstOrDefault(q => q.QuoteId == quoteId);
                        quote.UpdatedById = parameter.UserId;
                        quote.UpdatedDate = DateTime.Now;
                        quote.ApprovalStep = 0;
                        if (parameter.RejectReason.Equals("EMP"))
                        {
                            // Quản lý từ chối, về trạng thái Nháp
                            quote.StatusId = statusNew.CategoryId;
                        }
                        else if (parameter.RejectReason.Equals("CUS"))
                        {
                            // Khách hàng từ chối về trạng thái Đóng-Không trúng
                            quote.StatusId = statusCloseNotHit.CategoryId;
                        }
                        else
                        {
                            quote.StatusId = category.CategoryId;
                        }
                        context.Quote.Update(quote);

                        Note note = new Note();
                        note.NoteId = Guid.NewGuid();
                        note.ObjectType = "QUOTE";
                        note.ObjectId = quote.QuoteId;
                        note.Type = "ADD";
                        note.Description = "Báo giá đã bị từ chối vì: " + parameter.Description;
                        note.Active = true;
                        note.CreatedById = parameter.UserId;
                        note.CreatedDate = DateTime.Now;
                        note.NoteTitle = "Đã thêm ghi chú";

                        context.Note.Add(note);
                        context.SaveChanges();

                        #region Gửi thông báo

                        var _note = new Note();
                        _note.Description = parameter.Description;
                        NotificationHelper.AccessNotification(context, TypeModel.QuoteDetail, "REJECT", new Queue(),
                            quote, true, _note);
                    });


                    #endregion
                    return new UpdateQuoteResult
                    {
                        Status = true,
                        Message = "Từ chối phê duyệt thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public SearchQuoteResult SearchQuoteAprroval(SearchQuoteParameter parameter)
        {
            try
            {
                var listQuote = new List<QuoteEntityModel>();
                var customerOrder = context.CustomerOrder.ToList();
                var customerOrderCost = context.QuoteCostDetail.ToList();

                #region Lấy list status của báo giá

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI" && x.Active == true).CategoryTypeId;
                var listStatus = context.Category.Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                                    new CategoryEntityModel
                                    {
                                        CategoryId = y.CategoryId,
                                        CategoryName = y.CategoryName,
                                        CategoryCode = y.CategoryCode,
                                        CategoryTypeId = Guid.Empty,
                                        CreatedById = Guid.Empty,
                                        CountCategoryById = 0
                                    }).ToList();

                #endregion

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                parameter.QuoteCode = parameter.QuoteCode == null ? "" : parameter.QuoteCode.Trim();

                listQuote = context.Quote.Where(x => (parameter.QuoteCode == "" || x.QuoteCode.Contains(parameter.QuoteCode)) &&
                                    (parameter.ListStatusQuote.Count == 0 || parameter.ListStatusQuote.Contains(x.StatusId)) &&
                                    x.Active == true && x.Seller != null)
                                    .Select(y => new QuoteEntityModel
                                    {
                                        QuoteId = y.QuoteId,
                                        QuoteCode = y.QuoteCode,
                                        QuoteDate = y.QuoteDate,
                                        Seller = y.Seller,
                                        Description = y.Description,
                                        Note = y.Note,
                                        ObjectTypeId = y.ObjectTypeId,
                                        ObjectType = y.ObjectType,
                                        PaymentMethod = y.PaymentMethod,
                                        DaysAreOwed = y.DaysAreOwed,
                                        IntendedQuoteDate = y.IntendedQuoteDate,
                                        SendQuoteDate = y.SendQuoteDate,
                                        MaxDebt = y.MaxDebt,
                                        ExpirationDate = y.ExpirationDate,
                                        ReceivedDate = y.ReceivedDate,
                                        ReceivedHour = y.ReceivedHour,
                                        RecipientName = y.RecipientName,
                                        LocationOfShipment = y.LocationOfShipment,
                                        ShippingNote = y.ShippingNote,
                                        RecipientPhone = y.RecipientPhone,
                                        RecipientEmail = y.RecipientEmail,
                                        PlaceOfDelivery = y.PlaceOfDelivery,
                                        Amount = y.Amount,
                                        DiscountValue = y.DiscountValue,
                                        StatusId = y.StatusId,
                                        CreatedById = y.CreatedById,
                                        CreatedDate = y.CreatedDate,
                                        UpdatedById = y.UpdatedById,
                                        UpdatedDate = y.UpdatedDate,
                                        Active = y.Active,
                                        DiscountType = y.DiscountType,
                                        PersonInChargeId = y.PersonInChargeId,
                                        CountQuoteInOrder = CountQuoteInCustomerOrder(y.QuoteId, customerOrder),
                                        QuoteStatusName = "",
                                        BackgroundColorForStatus = "",
                                        CustomerName = "",
                                        ApprovalStep = y.ApprovalStep
                                        //}).OrderByDescending(z => z.QuoteDate).ToList();
                                    }).OrderBy(z => z.UpdatedDate).ToList();

                if (parameter.IsCompleteInWeek)
                {
                    // Báo giá phải hoàn thành trong tuần
                    parameter.StartDate = FirstDateOfWeek();
                    parameter.EndDate = LastDateOfWeek();

                    listQuote = listQuote.Where(x =>
                        (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                         parameter.StartDate <= x.IntendedQuoteDate) &&
                        (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                         parameter.EndDate >= x.IntendedQuoteDate) &&
                        x.SendQuoteDate == null).ToList();
                }
                else
                {
                    listQuote = listQuote.Where(x =>
                                    (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue ||
                                     parameter.StartDate <= x.QuoteDate) &&
                                    (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue ||
                                     parameter.EndDate >= x.QuoteDate)).ToList();
                }

                if (parameter.IsOutOfDate)
                {
                    // Báo giá hết hiệu lực
                    listQuote = listQuote.Where(x => x.ExpirationDate < DateTime.Now.Date && x.ExpirationDate != null)
                        .OrderBy(z => z.UpdatedDate).ToList();
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

                    var listEmployeeId = context.Employee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))
                        .Select(y => y.EmployeeId).ToList();

                    listQuote = listQuote.Where(x =>
                        (listEmployeeId.Count == 0 || listEmployeeId.Contains(x.Seller.Value)) ||
                        x.PersonInChargeId == employee.EmployeeId).ToList();
                }
                else
                {
                    listQuote = listQuote.Where(x =>
                        x.Seller == employee.EmployeeId || x.PersonInChargeId == employee.EmployeeId).ToList();
                }

                #region Lấy tên Đối tượng và tên Trạng thái của Báo giá

                if (listQuote != null)
                {
                    List<Guid> listCategoryId = new List<Guid>();
                    List<Guid> listLeadId = new List<Guid>();
                    List<Guid> listCustomerId = new List<Guid>();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            if (!listCategoryId.Contains(item.StatusId.Value))
                                listCategoryId.Add(item.StatusId.Value);
                        }
                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                if (!listLeadId.Contains(item.ObjectTypeId.Value))
                                    listLeadId.Add(item.ObjectTypeId.Value);
                                break;
                            case "CUSTOMER":
                                if (!listCustomerId.Contains(item.ObjectTypeId.Value))
                                    listCustomerId.Add(item.ObjectTypeId.Value);
                                break;
                        }
                    });
                    var listCategory = context.Category.Where(e => listCategoryId.Contains(e.CategoryId)).ToList();
                    var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
                    var listContact = context.Contact.Where(e => listLeadId.Contains(e.ObjectId)).ToList();
                    listQuote.ForEach(item =>
                    {
                        if (item.StatusId != null || item.StatusId != Guid.Empty)
                        {
                            var quoteStatus = listCategory.FirstOrDefault(e => e.CategoryId == item.StatusId.Value);
                            switch (quoteStatus.CategoryCode)
                            {
                                case "MTA":
                                    item.BackgroundColorForStatus = "#FFC000";
                                    break;
                                case "CHO":
                                    item.BackgroundColorForStatus = " #9C00FF";
                                    break;
                                case "DTH":
                                    item.BackgroundColorForStatus = "#6D98E7";
                                    break;
                                case "DTR":
                                    item.BackgroundColorForStatus = "#FF0000";
                                    break;
                                case "DLY":
                                    item.BackgroundColorForStatus = "#46B678";
                                    break;
                                case "HUY":
                                    item.BackgroundColorForStatus = "#333333";
                                    break;
                                case "HOA":
                                    item.BackgroundColorForStatus = "#666666";
                                    break;
                                case "TUCHOI":
                                    item.BackgroundColorForStatus = "#878d96";
                                    break;
                            }

                            item.QuoteStatusName = quoteStatus.CategoryName;
                        }

                        switch (item.ObjectType)
                        {
                            case "LEAD":
                                var contact = listContact.LastOrDefault(e => e.ObjectId == item.ObjectTypeId);
                                if (contact != null)
                                    item.CustomerName = contact.FirstName + ' ' + contact.LastName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                            case "CUSTOMER":
                                var customer = listCustomer.FirstOrDefault(e => e.CustomerId == item.ObjectTypeId);
                                if (customer != null)
                                    item.CustomerName = customer.CustomerName;
                                else
                                    item.CustomerName = string.Empty;
                                break;
                        }
                    });
                }

                #endregion

                var statusApprove = listStatus.FirstOrDefault(c => c.CategoryCode == "DLY").CategoryId;
                listQuote = listQuote.Where(q => q.StatusId == statusApprove).ToList();


                var listQuoteApproval = new List<QuoteEntityModel>();
                var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
                var workFlowStep = context.WorkFlowSteps
                    .Where(ws => ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber > 1).ToList();
                var maxStep = workFlowStep.OrderByDescending(z => z.StepNumber).FirstOrDefault().StepNumber;

                workFlowStep.ForEach(item =>
                {
                    if ((item.ApprovebyPosition && item.ApproverPositionId == employee.PositionId)
                    || (!item.ApprovebyPosition && item.ApproverId == employee.EmployeeId))
                    {
                        var customerStep = listQuote.Where(ca => ca.ApprovalStep == item.StepNumber).ToList();

                        listQuoteApproval.AddRange(customerStep);

                        #region Trong trường hợp quy trình có thay đổi và ApprovalStep của Quote không còn tôn tại trong quy trình mới

                        if (item.StepNumber == maxStep)
                        {
                            var ignoreStep = listQuote.Where(x => x.ApprovalStep > maxStep).ToList();
                            listQuoteApproval.AddRange(ignoreStep);
                        }

                        #endregion
                    }
                });

                // cộng chi phí
                listQuote.ForEach(item =>
                {
                    var costDetail = customerOrderCost.Where(d => d.QuoteId == item.QuoteId).Sum(d => (d.UnitPrice * d.Quantity));
                    item.Amount += costDetail;
                });

                return new SearchQuoteResult()
                {
                    Status = true,
                    Message = "Success",
                    ListQuote = listQuoteApproval,
                    ListStatus = listStatus
                };
            }
            catch (Exception e)
            {
                return new SearchQuoteResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public List<Guid> StepApprovalQuote(int? step, Guid? employeeId)
        {
            List<Guid> employeeList = new List<Guid>();
            var employees = context.Employee.Where(e => e.Active == true).ToList();
            var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "PDBG");
            var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws => ws.WorkflowId == workFlows.WorkFlowId && ws.StepNumber == step);

            if (workFlowStep.ApprovebyPosition)
            {
                var employee = employees.FirstOrDefault(e => e.EmployeeId == employeeId);

                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                employeeList = employees.Where(e => e.PositionId == workFlowStep.ApproverPositionId
                && (listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId))
                ).Select(e => e.EmployeeId).ToList();
            }
            else
            {
                employeeList.Add(Guid.Parse(workFlowStep.ApproverId.ToString()));
            }
            return employeeList;
        }

        //public DeleteCostResult DeleteCost(DeleteCostParameter parameter)
        //{
        //    try
        //    {

        //    }
        //    catch(Exception ex)
        //    {
        //        return new DeleteCostResult
        //        {
        //            Status = false,
        //            Message = ex.Message
        //        };
        //    }
        //}
    }
}
