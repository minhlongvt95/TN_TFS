using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using SelectPdf;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;
using TN.TNM.DataAccess.Messages.Results.ReceiptInvoice;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.ReceiptInvoice;
//using Syncfusion.Pdf;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ReceiptInvoiceDAO : BaseDAO, IReceiptInvoiceDataAccess
    {
        private IConverter _converter;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReceiptInvoiceDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IConverter converter, IHostingEnvironment hostingEnvironment)
        {
            context = _content;
            iAuditTrace = _iAuditTrace;
            iAuditTrace = _iAuditTrace;
            converter = _converter;
            _hostingEnvironment = hostingEnvironment;
        }

        public CreateReceiptInvoiceResult CreateReceiptInvoice(CreateReceiptInvoiceParameter parameter)
        {

            if (parameter.ReceiptInvoice.Amount <= 0)
            {
                return new CreateReceiptInvoiceResult()
                {
                    Status = false,
                    Message = "Thành tiền không được âm hoặc bằng 0"
                };
            }
            else if (parameter.ReceiptInvoice.UnitPrice <= 0)
            {
                return new CreateReceiptInvoiceResult()
                {
                    Status = false,
                    Message = "Số tiền không được âm hoặc bằng 0"
                };
            }
            else if (parameter.ReceiptInvoice.ExchangeRate <= 0)
            {
                return new CreateReceiptInvoiceResult()
                {
                    Status = false,
                    Message = "Tỉ giá không được âm hoặc bằng 0"
                };
            }

            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.RECEIPTINVOICE, "Create Receipt Invoice", parameter.UserId);
            var newReceiptInvoice = parameter.ReceiptInvoice;
            var newReceiptInvoiceMapping = parameter.ReceiptInvoiceMapping;
            var organizationCode = context.Organization
                .FirstOrDefault(o => o.OrganizationId == parameter.ReceiptInvoice.OrganizationId)?.OrganizationCode;

            if (newReceiptInvoice.UnitPrice <= 0)
            {
                return new CreateReceiptInvoiceResult
                {
                    Status = false,
                    Message = "Số tiền không được là số âm hoặc bằng 0"
                };
            }
            if (newReceiptInvoice.ExchangeRate != null)
            {
                if (newReceiptInvoice.ExchangeRate <= 0)
                {
                    return new CreateReceiptInvoiceResult
                    {
                        Status = false,
                        Message = "Tỷ giá không được là số âm hoặc bằng 0"
                    };
                }
            }

            newReceiptInvoice.ReceiptInvoiceId = Guid.NewGuid();
            newReceiptInvoice.CreatedById = parameter.UserId;
            newReceiptInvoice.CreatedDate = DateTime.Now;
            newReceiptInvoice.ReceiptInvoiceCode = "PT" + "-" + organizationCode + DateTime.Now.Year
                + (context.ReceiptInvoice.Count(r => r.CreatedDate.Year == DateTime.Now.Year) + 1).ToString("D5");
            var receiptInvoiceDupblicase = context.ReceiptInvoice.FirstOrDefault(x => x.ReceiptInvoiceCode == newReceiptInvoice.ReceiptInvoiceCode);
            if (receiptInvoiceDupblicase != null)
            {
                return new CreateReceiptInvoiceResult
                {
                    Status = false,
                    Message = "Mã phiếu thu đã tồn tại"
                };
            }
            newReceiptInvoiceMapping.ReceiptInvoiceMappingId = Guid.NewGuid();
            newReceiptInvoiceMapping.ReceiptInvoiceId = parameter.ReceiptInvoice.ReceiptInvoiceId;
            newReceiptInvoiceMapping.CreatedById = parameter.UserId;
            newReceiptInvoiceMapping.CreatedDate = DateTime.Now;
            newReceiptInvoiceMapping.ObjectId = parameter.ReceiptInvoiceMapping.ObjectId;

            var statusIP = context.OrderStatus.FirstOrDefault(o => o.OrderStatusCode == "COMP").OrderStatusId;

            parameter.ReceiptOrderHistory.ForEach(item =>
            {
                var orderHistory = new ReceiptOrderHistory()
                {
                    ReceiptOrderHistoryId = Guid.NewGuid(),
                    ObjectId = newReceiptInvoice.ReceiptInvoiceId,
                    ObjectType = "THU",
                    OrderId = item.OrderId,
                    AmountCollected = item.AmountCollected,
                    CreatedDate = DateTime.Now,
                    CreatedById = parameter.UserId
                };
                context.ReceiptOrderHistory.Add(orderHistory);

                if (item.AmountCollected == item.Amount)
                {
                    var customerOrder = context.CustomerOrder.FirstOrDefault(c => c.OrderId == item.OrderId);
                    customerOrder.StatusId = statusIP;
                    context.CustomerOrder.Update(customerOrder);
                }
            });

            context.ReceiptInvoiceMapping.Add(newReceiptInvoiceMapping);
            context.ReceiptInvoice.Add(newReceiptInvoice);
            context.SaveChanges();

            #region Comment by Giang

            //Update total receipt invoice on table customer
            //var totalReceiptCustomer = context.Customer.FirstOrDefault(v => v.CustomerId == newReceiptInvoiceMapping.ObjectId);
            //if (totalReceiptCustomer != null)
            //{
            //    var TotalReceivable = totalReceiptCustomer.TotalReceivable;
            //    totalReceiptCustomer.TotalReceivable = ((TotalReceivable == null || TotalReceivable == 0) ? totalReceiptCustomer.TotalSaleValue : TotalReceivable) -
            //                                             (newReceiptInvoice.UnitPrice * (newReceiptInvoice.ExchangeRate ?? 1));
            //    context.SaveChanges();
            //}

            #endregion

            return new CreateReceiptInvoiceResult
            {
                Status = true,
                Message = CommonMessage.ReceiptInvoice.ADD_SUCCESS
            };
        }

        public EditReceiptInvoiceResult EditReceiptInvoice(EditReceiptInvoiceParameter parameter)
        {
            throw new NotImplementedException();
        }

        public GetReceiptInvoiceByIdResult GetReceiptInvoiceById(GetReceiptInvoiceByIdParameter parameter)
        {

            var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
            var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId).ToList();

            var statusCateoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
            var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

            var contact = context.Contact.ToList();
            var listAllOrg = context.Organization.ToList();

            var currencyUnitId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DTI").CategoryTypeId;
            var listAllCurrencyUnit = context.Category.Where(x => x.CategoryTypeId == currencyUnitId).ToList();

            var receiptInvoice = context.ReceiptInvoice.FirstOrDefault(c => parameter.ReceiptInvoiceId == null || parameter.ReceiptInvoiceId == Guid.Empty || c.ReceiptInvoiceId == parameter.ReceiptInvoiceId);

            var receiptInvoiceMapping = context.ReceiptInvoiceMapping.FirstOrDefault(c => parameter.ReceiptInvoiceId == null || parameter.ReceiptInvoiceId == Guid.Empty || c.ReceiptInvoiceId == parameter.ReceiptInvoiceId);

            ReceiptInvoiceEntityModel data = new ReceiptInvoiceEntityModel();
            data.ReceiptInvoiceId = receiptInvoice.ReceiptInvoiceId;
            data.ReceiptInvoiceCode = receiptInvoice.ReceiptInvoiceCode;
            data.ReceiptInvoiceDetail = receiptInvoice.ReceiptInvoiceDetail;
            data.ReceiptInvoiceReason = receiptInvoice.ReceiptInvoiceReason;
            data.ReceiptInvoiceNote = receiptInvoice.ReceiptInvoiceNote;
            data.RegisterType = receiptInvoice.RegisterType;
            data.OrganizationId = receiptInvoice.OrganizationId;
            data.StatusId = receiptInvoice.StatusId;
            data.RecipientName = receiptInvoice.RecipientName;
            data.RecipientAddress = receiptInvoice.RecipientAddress;
            data.UnitPrice = receiptInvoice.UnitPrice;
            data.CurrencyUnit = receiptInvoice.CurrencyUnit;
            data.ExchangeRate = receiptInvoice.ExchangeRate;
            data.Amount = receiptInvoice.Amount;
            data.AmountText = receiptInvoice.AmountText;
            data.Active = receiptInvoice.Active;
            data.CreatedById = receiptInvoice.CreatedById;
            data.CreatedDate = receiptInvoice.CreatedDate;
            data.UpdatedById = receiptInvoice.UpdatedById;
            data.UpdatedDate = receiptInvoice.UpdatedDate;
            data.ObjectId = receiptInvoiceMapping.ObjectId;
            data.ReceiptDate = receiptInvoice.ReceiptDate;
            data.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == receiptInvoice.StatusId).CategoryName;
            data.AmountText = MoneyHelper.Convert((decimal)data.Amount);
            data.NameCreateBy = GetCreateByName(receiptInvoice.CreatedById);
            data.NameObjectReceipt = GetObjectName(data.ObjectId);
            if (data.ReceiptInvoiceReason != null && data.ReceiptInvoiceReason != Guid.Empty)
            {
                data.NameReceiptInvoiceReason = listAllReason.FirstOrDefault(c => c.CategoryId == receiptInvoice.ReceiptInvoiceReason).CategoryName;
            }
            if (data.OrganizationId != null && data.OrganizationId != Guid.Empty)
            {
                data.OrganizationName = listAllOrg.FirstOrDefault(c => c.OrganizationId == data.OrganizationId).OrganizationName;
            }
            if (data.CurrencyUnit != null && data.CurrencyUnit != Guid.Empty)
            {
                data.CurrencyUnitName = listAllCurrencyUnit.FirstOrDefault(c => c.CategoryId == data.CurrencyUnit).CategoryName;
            }

            return new GetReceiptInvoiceByIdResult
            {
                Status = true,
                ReceiptInvoice = data
            };
        }

        public SearchReceiptInvoiceResult SearchReceiptInvoice(SearchReceiptInvoiceParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.RECEIPTINVOICE, "Search Receipt Invoice", parameter.UserId);

                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusCategoryTypeId).ToList();

                var receiptInvoiceCode = parameter.ReceiptInvoiceCode == null ? "" : parameter.ReceiptInvoiceCode.Trim();
                var receiptInvoiceReason = parameter.ReceiptInvoiceReason;
                var objectIds = parameter.ObjectReceipt;
                var listIdUser = parameter.CreateById;
                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                        createdByIds.Add(temp);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var fromDate = parameter.CreateDateFrom;
                var toDate = parameter.CreateDateTo;
                var statusIds = parameter.Status;

                var lstReceiptMap = context.ReceiptInvoiceMapping.Where(c => objectIds == null || objectIds.Count == 0 || objectIds.Contains(c.ObjectId)).ToList();
                var lstReceiptMapId = lstReceiptMap.Select(c => c.ReceiptInvoiceId).ToList();
                
                var lst = context.ReceiptInvoice
                                .Where(x => (receiptInvoiceCode == "" || x.ReceiptInvoiceCode.Contains(receiptInvoiceCode)) &&
                                    (receiptInvoiceReason == null || receiptInvoiceReason.Count == 0 || receiptInvoiceReason.Contains(x.ReceiptInvoiceReason.Value)) &&
                                    (lstReceiptMapId.Contains(x.ReceiptInvoiceId)) &&
                                    (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(x.CreatedById)) &&
                                    (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.CreatedDate) &&
                                    (toDate == null || toDate == DateTime.MinValue || toDate >= x.CreatedDate) &&
                                    (statusIds == null || statusIds.Count == 0 || statusIds.Contains(x.StatusId)))
                                 .Select(m => new ReceiptInvoiceEntityModel
                                 {
                                     ReceiptInvoiceId = m.ReceiptInvoiceId,
                                     ReceiptInvoiceCode = m.ReceiptInvoiceCode,
                                     ReceiptInvoiceDetail = m.ReceiptInvoiceDetail,
                                     ReceiptInvoiceReason = m.ReceiptInvoiceReason,
                                     ReceiptInvoiceNote = m.ReceiptInvoiceNote,
                                     RegisterType = m.RegisterType,
                                     OrganizationId = m.OrganizationId,
                                     StatusId = m.StatusId,
                                     RecipientName = m.RecipientName,
                                     RecipientAddress = m.RecipientAddress,
                                     UnitPrice = m.UnitPrice,
                                     CurrencyUnit = m.CurrencyUnit,
                                     ExchangeRate = m.ExchangeRate,
                                     Amount = m.Amount,
                                     AmountText = m.AmountText,
                                     Active = m.Active,
                                     CreatedById = m.CreatedById,
                                     CreatedDate = m.CreatedDate,
                                     UpdatedById = m.UpdatedById,
                                     UpdatedDate = m.UpdatedDate,
                                     NameReceiptInvoiceReason = listAllReason.FirstOrDefault(s => s.CategoryId == m.ReceiptInvoiceReason).CategoryName,
                                     ReceiptDate = m.ReceiptDate,
                                     StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == m.StatusId).CategoryName,
                                 }).ToList();
                lst.ForEach(item =>
                {
                    var temp = lstReceiptMap.FirstOrDefault(c => c.ReceiptInvoiceId == item.ReceiptInvoiceId);
                    if(temp != null)
                    {
                        item.ObjectId = temp.ObjectId;
                        item.NameObjectReceipt = GetObjectName(temp.ObjectId);
                    }
                    item.NameCreateBy = GetCreateByName(item.CreatedById);
                    if (item.StatusId != null && item.StatusId != Guid.Empty)
                    {
                        var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                        switch (status.CategoryCode)
                        {
                            case "CSO":
                                item.BackgroundColorForStatus = "#ffcc00";
                                break;
                            case "DSO":
                                item.BackgroundColorForStatus = "#007aff";
                                break;
                        }
                    }
                });

                lst = lst.OrderByDescending(x => x.CreatedDate).ToList();
                return new SearchReceiptInvoiceResult
                {
                    Status = true,
                    Message = "Success",
                    lstReceiptInvoiceEntity = lst
                };

            }
            catch (Exception ex)
            {
                return new SearchReceiptInvoiceResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public SearchCashBookReceiptInvoiceResult SearchCashBookReceiptInvoice(SearchCashBookReceiptInvoiceParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.RECEIPTINVOICE, "Search Cash Book Receipt Invoice", parameter.UserId);
                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId);

                var listIdUser = parameter.CreateById;
                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                        createdByIds.Add(temp);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }
                var fromDate = parameter.ReceiptDateFrom;
                var toDate = parameter.ReceiptDateTo;
                var organizations = parameter.OrganizationList;
                var receiptInvoiceList = context.ReceiptInvoice
                                                    .Where(c => (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(c.CreatedById)) &&
                                                                (organizations == null || organizations.Count == 0 || organizations.Contains(c.OrganizationId)) &&
                                                                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= c.CreatedDate) &&
                                                                (toDate == null || toDate == DateTime.MinValue || toDate >= c.CreatedDate))
                                                    .Select(v => new ReceiptInvoiceEntityModel
                                                    {
                                                        Active = v.Active,
                                                        Amount = v.Amount,
                                                        CreatedById = v.CreatedById,
                                                        CreatedDate = v.CreatedDate,
                                                        CurrencyUnit = v.CurrencyUnit,
                                                        ExchangeRate = v.ExchangeRate,
                                                        OrganizationId = v.OrganizationId,
                                                        ReceiptInvoiceCode = v.ReceiptInvoiceCode,
                                                        ReceiptInvoiceDetail = v.ReceiptInvoiceDetail,
                                                        ReceiptInvoiceId = v.ReceiptInvoiceId,
                                                        ReceiptInvoiceNote = v.ReceiptInvoiceNote ?? "",
                                                        ReceiptInvoiceReason = v.ReceiptInvoiceReason,
                                                        ReceiptDate = v.ReceiptDate,
                                                        RecipientAddress = v.RecipientAddress ?? "",
                                                        RecipientName = v.RecipientName ?? "",
                                                        RegisterType = v.RegisterType,
                                                        StatusId = v.StatusId,
                                                        UnitPrice = v.UnitPrice,
                                                        UpdatedById = v.UpdatedById,
                                                        UpdatedDate = v.UpdatedDate,
                                                        NameReceiptInvoiceReason = listAllReason.FirstOrDefault(c => c.CategoryId == v.ReceiptInvoiceReason).CategoryName ?? ""
                                                    }).ToList();

                receiptInvoiceList.ForEach(item =>
                {
                    item.NameCreateBy = GetCreateByName(item.CreatedById);
                });

                receiptInvoiceList.OrderByDescending(c => c.CreatedDate).ToList();
                return new SearchCashBookReceiptInvoiceResult
                {
                    Message = "Success!",
                    Status = true,
                    lstReceiptInvoiceEntity = receiptInvoiceList,
                };
            }
            catch (Exception ex)
            {
                return new SearchCashBookReceiptInvoiceResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }
        public SearchBankReceiptInvoiceResult SearchBankReceiptInvoice(SearchBankReceiptInvoiceParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.RECEIPTINVOICE, "Search receipt Invoice", parameter.UserId);
                var vndCurrencyId = context.Category.FirstOrDefault(ct => ct.CategoryCode == "VND").CategoryId;

                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId);

                var statusCateoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

                var bankReceiptInvoice = parameter.ReceiptInvoiceCode == null ? "" : parameter.ReceiptInvoiceCode;
                var bankReceiptInvoiceReasonIds = parameter.ReceiptReasonIdList;
                var objectIds = parameter.ObjectIdList;
                var listIdUser = parameter.CreatedByIdList;
                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                        createdByIds.Add(temp);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var fromDate = parameter.FromDate;
                var toDate = parameter.ToDate;
                var statusIds = parameter.SttList;
                var lstBankReceiptMap = context.BankReceiptInvoiceMapping
                    .Where(c => objectIds == null || objectIds.Count == 0 || objectIds.Contains(c.ObjectId.Value)).ToList();
                var lstBankReceiptMapId = lstBankReceiptMap.Select(c => c.BankReceiptInvoiceId).ToList();

                var lst = context.BankReceiptInvoice
                                    .Where(x => (bankReceiptInvoice == "" || x.BankReceiptInvoiceCode.Contains(bankReceiptInvoice)) &&
                                        (bankReceiptInvoiceReasonIds == null || bankReceiptInvoiceReasonIds.Count == 0 || bankReceiptInvoiceReasonIds.Contains(x.BankReceiptInvoiceReason.Value)) &&
                                        (lstBankReceiptMapId.Contains(x.BankReceiptInvoiceId)) &&
                                        (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(x.CreatedById)) &&
                                        (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.CreatedDate) &&
                                        (toDate == null || toDate == DateTime.MinValue || toDate >= x.CreatedDate) &&
                                        (statusIds == null || statusIds.Count == 0 || statusIds.Contains(x.StatusId.Value)))
                                    .Select(m => new BankReceiptInvoiceEntityModel
                                    {
                                        BankReceiptInvoiceId = m.BankReceiptInvoiceId,
                                        BankReceiptInvoiceCode = m.BankReceiptInvoiceCode,
                                        BankReceiptInvoiceDetail = m.BankReceiptInvoiceDetail,
                                        BankReceiptInvoicePrice = m.BankReceiptInvoicePrice,
                                        BankReceiptInvoicePriceCurrency = m.BankReceiptInvoicePriceCurrency,
                                        BankReceiptInvoiceExchangeRate = m.BankReceiptInvoiceExchangeRate,
                                        BankReceiptInvoiceReason = m.BankReceiptInvoiceReason,
                                        BankReceiptInvoiceNote = m.BankReceiptInvoiceNote,
                                        BankReceiptInvoiceBankAccountId = m.BankReceiptInvoiceBankAccountId,
                                        BankReceiptInvoiceAmount = m.BankReceiptInvoiceAmount,
                                        BankReceiptInvoiceAmountText = m.BankReceiptInvoiceAmountText,
                                        BankReceiptInvoicePaidDate = m.BankReceiptInvoicePaidDate,
                                        OrganizationId = m.OrganizationId,
                                        StatusId = m.StatusId,
                                        Active = m.Active,
                                        CreatedById = m.CreatedById,
                                        CreatedDate = m.CreatedDate,
                                        UpdatedById = m.UpdatedById,
                                        UpdatedDate = m.UpdatedDate,
                                        BankReceiptInvoiceReasonText = listAllReason.FirstOrDefault(c => c.CategoryId == m.BankReceiptInvoiceReason).CategoryName,
                                        StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == m.StatusId).CategoryName,
                                    }).ToList();
                lst.ForEach(item =>
                {
                    var tmp = lstBankReceiptMap.FirstOrDefault(c => c.BankReceiptInvoiceId == item.BankReceiptInvoiceId);
                    if (tmp != null)
                    {
                        item.ObjectId = tmp.ObjectId;
                        item.ObjectName = GetObjectName(item.ObjectId);
                    }
                    item.CreatedByName = GetCreateByName(item.CreatedById);
                    if (item.StatusId != null && item.StatusId != Guid.Empty)
                    {
                        var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                        switch (status.CategoryCode)
                        {
                            case "CSO":
                                item.BackgroundColorForStatus = "#ffcc00";
                                break;
                            case "DSO":
                                item.BackgroundColorForStatus = "#007aff";
                                break;
                        }
                    }
                });
                lst = lst.OrderByDescending(x => x.CreatedDate).ToList();
                return new SearchBankReceiptInvoiceResult
                {
                    Status = true,
                    Message = "Success",
                    BankReceiptInvoiceList = lst
                };
            }
            catch (Exception ex)
            {
                return new SearchBankReceiptInvoiceResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
        public CreateBankReceiptInvoiceResult CreateBankReceiptInvoice(CreateBankReceiptInvoiceParameter parameter)
        {
            if (parameter.BankReceiptInvoice.BankReceiptInvoiceAmount <= 0)
            {
                return new CreateBankReceiptInvoiceResult()
                {
                    Status = false,
                    Message = "Thành tiền không được âm hoặc bằng 0"
                };
            }
            else if (parameter.BankReceiptInvoice.BankReceiptInvoicePrice <= 0)
            {
                return new CreateBankReceiptInvoiceResult()
                {
                    Status = false,
                    Message = "Số tiền không được âm hoặc bằng 0"
                };
            }
            else if (parameter.BankReceiptInvoice.BankReceiptInvoiceExchangeRate <= 0)
            {
                return new CreateBankReceiptInvoiceResult()
                {
                    Status = false,
                    Message = "Tỉ giá không được âm hoặc bằng 0"
                };
            }

            var newBankReceiptInvoiceId = Guid.NewGuid();
            var organ = context.Organization.FirstOrDefault(or => or.OrganizationId == parameter.BankReceiptInvoice.OrganizationId);
            parameter.BankReceiptInvoice.BankReceiptInvoiceId = newBankReceiptInvoiceId;
            parameter.BankReceiptInvoice.CreatedDate = DateTime.Now;
            parameter.BankReceiptInvoice.CreatedById = parameter.UserId;
            parameter.BankReceiptInvoice.BankReceiptInvoiceCode = "BC" + "-" + organ.OrganizationCode + DateTime.Now.Year +
                (context.BankReceiptInvoice.Count(b => b.CreatedDate.Year == DateTime.Now.Year) + 1).ToString("D5");
            var bankReceiptInvoiceDupblicase = context.BankReceiptInvoice.FirstOrDefault(x => x.BankReceiptInvoiceCode == parameter.BankReceiptInvoice.BankReceiptInvoiceCode);
            if (bankReceiptInvoiceDupblicase != null)
            {
                return new CreateBankReceiptInvoiceResult
                {
                    Status = false,
                    Message = "Mã báo có đã tồn tại"
                };
            }

            var newBankReceiptInvoiceMapping = parameter.BankReceiptInvoiceMapping;
            newBankReceiptInvoiceMapping.BankReceiptInvoiceMappingId = Guid.NewGuid();
            newBankReceiptInvoiceMapping.CreatedById = parameter.UserId;
            newBankReceiptInvoiceMapping.CreatedDate = DateTime.Now;
            newBankReceiptInvoiceMapping.BankReceiptInvoiceId = newBankReceiptInvoiceId;
            newBankReceiptInvoiceMapping.ObjectId = parameter.BankReceiptInvoiceMapping.ObjectId;

            var isEmp = context.Employee.Any(e => e.EmployeeId == newBankReceiptInvoiceMapping.ObjectId);
            var isCus = context.Customer.Any(c => c.CustomerId == newBankReceiptInvoiceMapping.ObjectId);
            var isVen = context.Vendor.Any(v => v.VendorId == newBankReceiptInvoiceMapping.ObjectId);

            if (isEmp)
            {
                newBankReceiptInvoiceMapping.ReferenceType = 1;
            }
            else if (isVen)
            {
                newBankReceiptInvoiceMapping.ReferenceType = 2;
            }
            else if (isCus)
            {
                newBankReceiptInvoiceMapping.ReferenceType = 3;
            }
            else
            {
                newBankReceiptInvoiceMapping.ReferenceType = 0;
            }

            var statusPD = context.OrderStatus.FirstOrDefault(o => o.OrderStatusCode == "COMP").OrderStatusId;

            parameter.ReceiptOrderHistory.ForEach(item =>
            {
                var orderHistory = new ReceiptOrderHistory()
                {
                    ReceiptOrderHistoryId = Guid.NewGuid(),
                    ObjectId = parameter.BankReceiptInvoice.BankReceiptInvoiceId,
                    ObjectType = "BAOCO",
                    OrderId = item.OrderId,
                    AmountCollected = item.AmountCollected,
                    CreatedDate = DateTime.Now,
                    CreatedById = parameter.UserId
                };
                context.ReceiptOrderHistory.Add(orderHistory);

                if (item.AmountCollected == item.Amount)
                {
                    var customerOrder = context.CustomerOrder.FirstOrDefault(c => c.OrderId == item.OrderId);
                    customerOrder.StatusId = statusPD;
                    context.CustomerOrder.Update(customerOrder);
                }
            });

            context.BankReceiptInvoice.Add(parameter.BankReceiptInvoice);
            context.BankReceiptInvoiceMapping.Add(newBankReceiptInvoiceMapping);
            context.SaveChanges();

            //Update total receipt invoice on table customer
            //var totalReceiptCustomer = context.Customer.FirstOrDefault(v => v.CustomerId == newBankReceiptInvoiceMapping.ObjectId);
            //if (totalReceiptCustomer != null)
            //{
            //    //totalReceiptCustomer.TotalReceivable = (totalReceiptCustomer.TotalReceivable ?? 0) + parameter.BankReceiptInvoice.BankReceiptInvoiceAmount
            //    //                                       * (parameter.BankReceiptInvoice.BankReceiptInvoiceExchangeRate ?? 1);
            //    var TotalReceivable = totalReceiptCustomer.TotalReceivable;

            //    totalReceiptCustomer.TotalReceivable = ((TotalReceivable == null || TotalReceivable == 0) ? totalReceiptCustomer.TotalSaleValue : TotalReceivable) - (parameter.BankReceiptInvoice.BankReceiptInvoiceAmount
            //                           * (parameter.BankReceiptInvoice.BankReceiptInvoiceExchangeRate ?? 1));
            //    context.SaveChanges();
            //}

            return new CreateBankReceiptInvoiceResult()
            {
                Status = true,
                Message = CommonMessage.BankReceiptInvoice.ADD_SUCCESS
            };
        }

        public SearchBankBookReceiptResult SearchBankBookReceipt(SearchBankBookReceiptParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.RECEIPTINVOICE, "Search receipt Invoice", parameter.UserId);

            var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
            var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId);

            var listIdUser = parameter.ListCreateById;
            var createdByIds = new List<Guid>();

            if (listIdUser != null)
            {
                foreach (var item in listIdUser)
                {
                    var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                    createdByIds.Add(temp);
                }
            }
            else
            {
                createdByIds = listIdUser;
            }
            var fromDate = parameter.FromPaidDate;
            var toDate = parameter.ToPaidDate;

            var lst = context.BankReceiptInvoice.Join(context.BankReceiptInvoiceMapping, bi => bi.BankReceiptInvoiceId, bm => bm.BankReceiptInvoiceId,
                (bi, bm) => new { bi, bm })
                .Where(x => (parameter.BankAccountId == null || parameter.BankAccountId.Count == 0 || parameter.BankAccountId.Contains(x.bi.BankReceiptInvoiceBankAccountId.Value)) &&
                (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(x.bi.CreatedById)) &&
                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.bi.CreatedDate) &&
                (toDate == null || toDate == DateTime.MinValue || toDate >= x.bi.CreatedDate))
                .Select(m => new BankReceiptInvoiceEntityModel
                {
                    BankReceiptInvoiceId = m.bi.BankReceiptInvoiceId,
                    BankReceiptInvoiceCode = m.bi.BankReceiptInvoiceCode,
                    BankReceiptInvoiceDetail = m.bi.BankReceiptInvoiceDetail,
                    BankReceiptInvoicePrice = m.bi.BankReceiptInvoicePrice,
                    BankReceiptInvoicePriceCurrency = m.bi.BankReceiptInvoicePriceCurrency,
                    BankReceiptInvoiceExchangeRate = m.bi.BankReceiptInvoiceExchangeRate,
                    BankReceiptInvoiceReason = m.bi.BankReceiptInvoiceReason,
                    BankReceiptInvoiceNote = m.bi.BankReceiptInvoiceNote,
                    BankReceiptInvoiceBankAccountId = m.bi.BankReceiptInvoiceBankAccountId,
                    BankReceiptInvoiceAmount = m.bi.BankReceiptInvoiceAmount,
                    BankReceiptInvoiceAmountText = m.bi.BankReceiptInvoiceAmountText,
                    BankReceiptInvoicePaidDate = m.bi.BankReceiptInvoicePaidDate,
                    OrganizationId = m.bi.OrganizationId,
                    StatusId = m.bi.StatusId,
                    Active = m.bi.Active,
                    CreatedById = m.bi.CreatedById,
                    CreatedDate = m.bi.CreatedDate,
                    UpdatedById = m.bi.UpdatedById,
                    UpdatedDate = m.bi.UpdatedDate,
                    BankReceiptInvoiceReasonText = listAllReason.FirstOrDefault(c => c.CategoryId == m.bi.BankReceiptInvoiceReason).CategoryName ?? "",
                    ObjectId = m.bm.ObjectId,
                    StatusName = "",
                    BackgroundColorForStatus = ""
                }).ToList();

            lst.ForEach(item =>
            {
                item.ObjectName = GetObjectName(item.ObjectId);
                item.CreatedByName = GetCreateByName(item.CreatedById);
                item.BankReceiptInvoiceDetail = item.BankReceiptInvoiceDetail ?? "";
                item.BankReceiptInvoiceNote = item.BankReceiptInvoiceNote ?? "";
            });

            lst = lst.OrderByDescending(x => x.CreatedDate).ToList();

            return new SearchBankBookReceiptResult()
            {
                Status = lst.Count > 0,
                BankReceiptInvoiceList = lst.OrderByDescending(l => l.CreatedDate).ToList(),
                Message = lst.Count > 0 ? "" : CommonMessage.BankReceiptInvoice.NO_INVOICE
            };

            //var lst = (from pi in context.BankReceiptInvoice
            //           join pim in context.BankReceiptInvoiceMapping on pi.BankReceiptInvoiceId equals pim.BankReceiptInvoiceId
            //           where (parameter.BankAccountId.Contains(pi.BankReceiptInvoiceBankAccountId.Value) || parameter.BankAccountId == null ||
            //                  parameter.BankAccountId.Count == 0)
            //           select new BankReceiptInvoiceEntityModel(pi)).ToList();

            //lst.ForEach(item =>
            //{
            //    item.BankReceiptInvoiceReasonText = context.Category.FirstOrDefault(c => c.CategoryId == item.BankReceiptInvoiceReason.Value).CategoryName;
            //    var empId = context.User.FirstOrDefault(u => u.UserId == item.CreatedById).EmployeeId;
            //    var contact = context.Contact.FirstOrDefault(c => c.ObjectId == empId.Value);
            //    item.AvatarUrl = contact.AvatarUrl;
            //    item.CreatedByName = contact.FirstName + " " + contact.LastName;
            //    var mapping = context.BankReceiptInvoiceMapping.FirstOrDefault(pim => pim.BankReceiptInvoiceId == item.BankReceiptInvoiceId);
            //    item.ObjectName = GetObjectName(mapping.ObjectId);
            //    item.StatusName = context.Category.FirstOrDefault(ct => ct.CategoryId == item.StatusId.Value).CategoryName;
            //});

            //if (parameter.ToPaidDate != null && parameter.ToPaidDate != DateTime.MinValue)
            //{
            //    lst = lst.Where(l => parameter.ToPaidDate.Value.Date >= l.BankReceiptInvoicePaidDate.Date).ToList();
            //}
            //if (parameter.FromPaidDate != null && parameter.FromPaidDate != DateTime.MinValue)
            //{
            //    lst = lst.Where(l => parameter.FromPaidDate.Value.Date <= l.BankReceiptInvoicePaidDate.Date).ToList();
            //}
        }
        public GetBankReceiptInvoiceByIdResult GetBankReceiptInvoiceById(GetBankReceiptInvoiceByIdParameter parameter)
        {
            var bri = context.BankReceiptInvoice.FirstOrDefault(b => b.BankReceiptInvoiceId == parameter.BankReceiptInvoiceId);
            var brim = context.BankReceiptInvoiceMapping.FirstOrDefault(b => b.BankReceiptInvoiceId == parameter.BankReceiptInvoiceId).ObjectId;
            var reasontext = context.Category.FirstOrDefault(rt => rt.CategoryId == bri.BankReceiptInvoiceReason);
            var org = context.Organization.FirstOrDefault(o => o.OrganizationId == bri.OrganizationId);
            var status = context.Category.FirstOrDefault(st => st.CategoryId == bri.StatusId);
            var pricecrr = context.Category.FirstOrDefault(pr => pr.CategoryId == bri.BankReceiptInvoicePriceCurrency);
            var empId = context.User.FirstOrDefault(u => u.UserId == bri.CreatedById).EmployeeId;
            var createdName = context.Employee.FirstOrDefault(e => e.EmployeeId == empId).EmployeeName;
            var objectName = GetObjectName(brim);
            var bankaccount = context.BankAccount.FirstOrDefault(ba => ba.BankAccountId == bri.BankReceiptInvoiceBankAccountId);

            bri.BankReceiptInvoiceAmountText = MoneyHelper.Convert(bri.BankReceiptInvoiceAmount.Value);

            return new GetBankReceiptInvoiceByIdResult()
            {
                BankReceiptInvoice = bri,
                BankReceiptInvoiceReasonText = reasontext.CategoryName,
                BankReceiptTypeText = (bankaccount != null) ? bankaccount.BankName : "",
                OrganizationText = org.OrganizationName,
                StatusText = status.CategoryName,
                PriceCurrencyText = pricecrr.CategoryName,
                CreateName = createdName,
                ObjectName = objectName,
                Status = true
            };
        }

        public ExportReceiptinvoiceResult ExportPdfReceiptInvoice(ExportReceiptInvoiceParameter parameter)
        {
            string html = ExportPdf.GetStringHtml("ReceiptInvoiceTemplatePDF.html");
            string css = ExportPdf.GetstrgCss("bootstrap.min.css");
            var company = context.CompanyConfiguration.FirstOrDefault(c => c.CompanyId != null);
            var receiptInvoice =
                context.ReceiptInvoice.FirstOrDefault(r => r.ReceiptInvoiceId == parameter.ReceiptInvoiceId);
            //var html = new StringBuilder();
            if (receiptInvoice != null)
            {
                var reason = context.Category.FirstOrDefault(rs => rs.CategoryId == receiptInvoice.ReceiptInvoiceReason).CategoryName;
                html = html.Replace("[CompanyName]", company.CompanyName.ToUpper());
                html = html.Replace("[CompanyAddress]", company.CompanyAddress);
                html = html.Replace("[ReceiptInvCode]", receiptInvoice.ReceiptInvoiceCode);
                html = html.Replace("[Date]", receiptInvoice.CreatedDate.Day.ToString());
                html = html.Replace("[Month]", receiptInvoice.CreatedDate.Month.ToString());
                html = html.Replace("[Year]", receiptInvoice.CreatedDate.Year.ToString());
                html = html.Replace("[RecipientName]", receiptInvoice.RecipientName);
                if (receiptInvoice.RecipientAddress != null)
                    html = html.Replace("[ReceipientAddress]", receiptInvoice.RecipientAddress);
                html = html.Replace("[ReceiptInvReason]", reason);
                html = html.Replace("[ReceiptInvDetail]", receiptInvoice.ReceiptInvoiceDetail);
                decimal price = (decimal)((receiptInvoice.ExchangeRate != null) ? receiptInvoice.ExchangeRate * receiptInvoice.UnitPrice : receiptInvoice.UnitPrice);
                html = html.Replace("[ReceiptInvPrice]", price.ToString("#,#."));
                html = html.Replace("[ReceiptInvPriceText]", MoneyHelper.Convert(price));
                html = html.Replace("[Note]", receiptInvoice.ReceiptInvoiceNote == null ? "" : receiptInvoice.ReceiptInvoiceNote);

                // Export html to Pdf
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExportedPDF";
                string fileName = @"ExportedReceipt.pdf";
                var receiptInvoicePdf = ExportPdf.HtmlToPdfExport(html, Path.Combine(rootFolder, fileName), PdfPageSize.A5, PdfPageOrientation.Landscape, string.Empty);

                return new ExportReceiptinvoiceResult
                {
                    ReceiptInvoicePdf = receiptInvoicePdf,
                    Code = receiptInvoice.ReceiptInvoiceCode
                };
            }
            return null;
        }

        public ExportBankReceiptInvoiceResult ExportBankReceiptInvoice(ExportBankReceiptInvoiceParameter parameter)
        {
            string html = ExportPdf.GetStringHtml("BankReceiptInvTemplate.html");
            string css = ExportPdf.GetstrgCss("bootstrap.min.css");
            var company = context.CompanyConfiguration.FirstOrDefault(c => c.CompanyId != null);
            var bankInvoice =
                context.BankReceiptInvoice.FirstOrDefault(r => r.BankReceiptInvoiceId == parameter.BankReceiptInvoiceId);
            if (bankInvoice != null)
            {
                var reason = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.BankReceiptInvoiceReason).CategoryName;
                var status = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.StatusId).CategoryName;
                var currency = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.BankReceiptInvoicePriceCurrency).CategoryName;
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == bankInvoice.OrganizationId).OrganizationName;
                var obj = context.BankReceiptInvoiceMapping.FirstOrDefault(bp => bp.BankReceiptInvoiceId == bankInvoice.BankReceiptInvoiceId);
                var objectId = obj == null ? Guid.Empty : obj.ObjectId;
                var empId = context.User.FirstOrDefault(u => u.UserId == bankInvoice.CreatedById).EmployeeId;
                var name = context.Employee.FirstOrDefault(e => e.EmployeeId == empId).EmployeeName;
                string objectName = GetObjectNameWithoutCode(objectId);
                html = html.Replace("[CompanyName]", company.CompanyName.ToUpper());
                html = html.Replace("[CompanyAddress]", company.CompanyAddress);
                html = html.Replace("[Code]", bankInvoice.BankReceiptInvoiceCode);
                html = html.Replace("[CreateDateDay]", bankInvoice.CreatedDate.Day.ToString());
                html = html.Replace("[CreateMonth]", bankInvoice.CreatedDate.Month.ToString());
                html = html.Replace("[CreateYear]", bankInvoice.CreatedDate.Year.ToString());
                html = html.Replace("[Content]", bankInvoice.BankReceiptInvoiceDetail);
                html = html.Replace("[Price]", bankInvoice.BankReceiptInvoiceAmount.Value.ToString("#,#."));
                html = html.Replace("[PriceString]", MoneyHelper.Convert(bankInvoice.BankReceiptInvoiceAmount.Value));
                html = html.Replace("[Note]", bankInvoice.BankReceiptInvoiceNote);
                html = html.Replace("[PaidDate]", bankInvoice.BankReceiptInvoicePaidDate.ToString("dd/MM/yyyy"));
                html = html.Replace("[Reason]", reason);
                html = html.Replace("[Object]", objectName);
                html = html.Replace("[Status]", status);
                html = html.Replace("[Organization]", org);
                html = html.Replace("[CurrencyCode]", currency);
                html = html.Replace("[CreatedBy]", name);

                // Export html to Pdf
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExportedPDF";
                string fileName = @"ExportedBankReceipt.pdf";
                var bankInvoicePdf = ExportPdf.HtmlToPdfExport(html, Path.Combine(rootFolder, fileName), PdfPageSize.A5, PdfPageOrientation.Landscape, string.Empty);

                return new ExportBankReceiptInvoiceResult()
                {
                    BankReceiptInvoicePdf = bankInvoicePdf,
                    Code = bankInvoice.BankReceiptInvoiceCode
                };
            }
            return null;
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
                var cus = context.Customer.FirstOrDefault(cu => cu.CustomerId == objId);
                var con = context.Contact.FirstOrDefault(c => c.ObjectId == objId);
                var ven = context.Vendor.FirstOrDefault(e => e.VendorId == objId);

                if (emp != null && con != null)
                {
                    return con.IdentityId + " - " + emp.EmployeeName;
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

        private string GetObjectNameWithoutCode(Guid? objId)
        {
            if (objId != null && objId != Guid.Empty)
            {
                var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == objId);
                var cus = context.Customer.FirstOrDefault(cu => cu.CustomerId == objId);
                var con = context.Contact.FirstOrDefault(c => c.ObjectId == objId);
                var ven = context.Vendor.FirstOrDefault(e => e.VendorId == objId);

                if (emp != null && con != null)
                {
                    return emp.EmployeeName;
                }

                if (ven != null)
                {
                    return ven.VendorName;
                }
                if (cus != null)
                {
                    return cus.CustomerName;
                }

                return "";
            }

            return "";
        }

        public GetOrderByCustomerIdResult GetOrderByCustomerId(GetOrderByCustomerIdParameter parameter)
        {
            try
            {
                List<Guid> listOrderId = new List<Guid>();
                List<ReceiptOrderHistory> listOrderInReceiptOrderHistory = new List<ReceiptOrderHistory>();
                decimal totalAmountReceivable = 0;
                List<ReceiptInvoiceOrderModel> listReceiptInvoiceOrderModel = new List<ReceiptInvoiceOrderModel>();

                var statusInprocess = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "IP").OrderStatusId; //Đang xử lý
                var statusWasSend = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DLV").OrderStatusId; //Đã giao hàng

                //Lấy danh sách đơn hàng theo khách hàng
                var listOrder = context.CustomerOrder.Where(x => (x.StatusId == statusInprocess || x.StatusId == statusWasSend) && x.CustomerId == parameter.CustomerId)
                                                     .OrderBy(y => y.OrderDate)
                                                     .ToList();

                if (listOrder.Count > 0)
                {
                    listOrder.ForEach(item =>
                    {
                        if (item.OrderId != null && item.OrderId != Guid.Empty)
                            listOrderId.Add(item.OrderId);
                    });
                }

                if (listOrderId.Count > 0)
                {
                    listOrderInReceiptOrderHistory = context.ReceiptOrderHistory.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                    //Lấy danh sách đơn hàng đã thu tiền
                    var new_list = listOrderInReceiptOrderHistory.GroupBy(x => new { x.OrderId }).Select(y => new
                    {
                        Id = y.Key,
                        y.Key.OrderId,
                        TotalAmountCollected = y.Sum(s => s.AmountCollected)
                    }).ToList();

                    if (new_list.Count > 0)
                    {
                        listOrder.ForEach(item =>
                        {
                            var order = new_list.FirstOrDefault(x => x.OrderId == item.OrderId);
                            var totalOrder = CalculatorTotalPurchaseProduct(item.Amount, item.DiscountType.Value, item.DiscountValue.Value);
                            if (order != null)
                            {
                                //Lấy Đơn hàng chưa được thanh toán hết (Số tiền đã thanh toán < Số tiền của đơn hàng)
                                if (order.TotalAmountCollected < totalOrder)
                                {
                                    ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                    receiptInvoiceOrder.OrderId = order.OrderId;
                                    receiptInvoiceOrder.OrderCode = item.OrderCode;
                                    receiptInvoiceOrder.AmountCollected = totalOrder - order.TotalAmountCollected;
                                    receiptInvoiceOrder.AmountReceivable = totalOrder - order.TotalAmountCollected;
                                    receiptInvoiceOrder.Total = totalOrder;
                                    receiptInvoiceOrder.OrderDate = item.OrderDate;

                                    listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                                };
                            }
                            else
                            {
                                //Nếu đơn hàng chưa được thanh toán lần nào
                                ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                receiptInvoiceOrder.OrderId = item.OrderId;
                                receiptInvoiceOrder.OrderCode = item.OrderCode;
                                receiptInvoiceOrder.AmountCollected = totalOrder;
                                receiptInvoiceOrder.AmountReceivable = totalOrder;
                                receiptInvoiceOrder.Total = totalOrder;
                                receiptInvoiceOrder.OrderDate = item.OrderDate;

                                listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                            }
                        });
                    }
                    else
                    {
                        //Nếu chưa có đơn hàng nào được thanh toán
                        listOrder.ForEach(item =>
                        {
                            var totalOrder = CalculatorTotalPurchaseProduct(item.Amount, item.DiscountType.Value, item.DiscountValue.Value);
                            ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                            receiptInvoiceOrder.OrderId = item.OrderId;
                            receiptInvoiceOrder.OrderCode = item.OrderCode;
                            receiptInvoiceOrder.AmountCollected = totalOrder;
                            receiptInvoiceOrder.AmountReceivable = totalOrder;
                            receiptInvoiceOrder.Total = totalOrder;
                            receiptInvoiceOrder.OrderDate = item.OrderDate;

                            listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                        });
                    }

                    totalAmountReceivable = listReceiptInvoiceOrderModel.Sum(x => x.AmountReceivable);
                }

                return new GetOrderByCustomerIdResult
                {
                    Status = true,
                    Message = "Lấy danh sách đơn hàng thành công",
                    listOrder = listReceiptInvoiceOrderModel,
                    totalAmountReceivable = totalAmountReceivable
                };
            }
            catch (Exception e)
            {
                return new GetOrderByCustomerIdResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private decimal CalculatorTotalPurchaseProduct(decimal amount, bool discountType, decimal discountValue)
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

        public GetMasterDateSearchBankReceiptInvoiceResult GetMasterDateSearchBankReceiptInvoice(GetMasterDateSearchBankReceiptInvoiceParameter parameter)
        {
            try
            {
                var listEmpployee = new List<Employee>();

                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCateoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

                listEmpployee = context.Employee.Where(x => x.Active == true).ToList();

                return new GetMasterDateSearchBankReceiptInvoiceResult
                {
                    Status = true,
                    Message = "Success",
                    ReasonOfReceiptList = listAllReason,
                    StatusOfReceiptList = listAllStatus,
                    Employees = listEmpployee
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDateSearchBankReceiptInvoiceResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetMasterDataReceiptInvoiceResult GetMasterDataReceiptInvoice(GetMasterDataReceiptInvoiceParameter parameter)
        {
            try
            {
                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();
                var listOrganization = context.Organization.Where(o => o.IsFinancialIndependence.Value == true).ToList();

                var categoryReasonType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LTH");
                var reasonReceiptList = new List<CategoryEntityModel>();

                if (categoryReasonType != null)
                {
                    reasonReceiptList = listCategory.Where(ct => ct.Active == true && ct.CategoryTypeId == categoryReasonType.CategoryTypeId).Select(c => new CategoryEntityModel()
                    {
                        CategoryTypeId = c.CategoryTypeId,
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        CategoryCode = c.CategoryCode,
                        IsDefault = c.IsDefauld
                    }).ToList();
                }

                var categoryStatusType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCH");
                var statusOfReceiptList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    statusOfReceiptList = listCategory.Where(c => c.Active == true && c.CategoryTypeId == categoryStatusType.CategoryTypeId).Select(c => new CategoryEntityModel()
                    {
                        CategoryTypeId = c.CategoryTypeId,
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        CategoryCode = c.CategoryCode,
                        IsDefault = c.IsDefauld
                    }).ToList();
                }

                var categoryType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LSO");
                var typeOfReceiptList = new List<CategoryEntityModel>();
                if (categoryType != null)
                {
                    typeOfReceiptList = listCategory.Where(c => c.Active == true && c.CategoryTypeId == categoryType.CategoryTypeId).Select(c => new CategoryEntityModel()
                    {
                        CategoryTypeId = c.CategoryTypeId,
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        CategoryCode = c.CategoryCode,
                        IsDefault = c.IsDefauld
                    }).ToList();
                }

                var categoryUnitMoneyType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "DTI");
                var unitMoneyOfReceiptList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    unitMoneyOfReceiptList = listCategory.Where(c => c.Active == true && c.CategoryTypeId == categoryUnitMoneyType.CategoryTypeId).Select(c => new CategoryEntityModel()
                    {
                        CategoryTypeId = c.CategoryTypeId,
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        CategoryCode = c.CategoryCode,
                        IsDefault = c.IsDefauld
                    }).ToList();
                }

                var customerList = context.Customer.Where(c => c.Active == true).OrderBy(x => x.CustomerName).ToList();

                return new GetMasterDataReceiptInvoiceResult
                {
                    Status = true,
                    ReasonOfReceiptList = reasonReceiptList,
                    StatusOfReceiptList = statusOfReceiptList,
                    TypesOfReceiptList = typeOfReceiptList,
                    UnitMoneyList = unitMoneyOfReceiptList,
                    OrganizationList = listOrganization,
                    CustomerList = customerList
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataReceiptInvoiceResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }


        public GetMasterDataSearchReceiptInvoiceResult GetGetMasterDataSearchReceiptInvoice(GetMasterDataSearchReceiptInvoiceParameter parameter)
        {
            try
            {

                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();


                var categoryReasonType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LTH");
                var reasonReceiptList = new List<CategoryEntityModel>();

                if (categoryReasonType != null)
                {
                    reasonReceiptList = listCategory.Where(ct => ct.Active == true && ct.CategoryTypeId == categoryReasonType.CategoryTypeId).Select(c => new CategoryEntityModel()
                    {
                        CategoryTypeId = c.CategoryTypeId,
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        CategoryCode = c.CategoryCode,
                        IsDefault = c.IsDefauld
                    }).ToList();
                }

                var categoryStatusType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCH");
                var statusOfReceiptList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    statusOfReceiptList = listCategory.Where(c => c.CategoryTypeId == categoryStatusType.CategoryTypeId).Select(c => new CategoryEntityModel()
                    {
                        CategoryTypeId = c.CategoryTypeId,
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        CategoryCode = c.CategoryCode,
                        IsDefault = c.IsDefauld
                    }).ToList();
                }

                var listEmployee = context.Employee.Where(e => e.Active == true).ToList();
                return new GetMasterDataSearchReceiptInvoiceResult
                {
                    Status = true,
                    Message = "Success",
                    ReasonOfReceiptList = reasonReceiptList,
                    StatusOfReceiptList = statusOfReceiptList,
                    Employees = listEmployee,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchReceiptInvoiceResult
                {
                    Status = false,
                    Message = ex.Message.ToString()
                };
            }
        }
    }
}
