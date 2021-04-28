using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Hosting;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Order;
using TN.TNM.DataAccess.Messages.Results.Order;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;
using OrderStatus = TN.TNM.DataAccess.Databases.Entities.OrderStatus;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.LocalAddress;
using TN.TNM.DataAccess.Models.LocalPoint;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CustomerOrderDAO : BaseDAO, ICustomerOrderDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public CustomerOrderDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }

        public CreateCustomerOrderResult CreateCustomerOrder(CreateCustomerOrderParameter parameter)
        {
            try
            {
                bool isValidParameterNumber = true;
                if (parameter.CustomerOrder?.DaysAreOwed < 0 || parameter.CustomerOrder?.MaxDebt < 0 ||
                    parameter.CustomerOrder?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                }
                parameter.CustomerOrderDetail.ForEach(item =>
                {
                    if (item.Quantity <= 0 || item.UnitPrice < 0 || item?.Vat < 0 || item?.DiscountValue < 0 ||
                        item?.ExchangeRate <= 0 || item?.GuaranteeTime < 0)
                    {
                        isValidParameterNumber = false;
                    }
                });

                if (!isValidParameterNumber)
                {
                    return new CreateCustomerOrderResult
                    {
                        Message = CommonMessage.Order.CREATE_FAIL,
                        Status = false
                    };
                }

                //Kiểm tra chiết khấu của đơn hàng
                if (parameter.CustomerOrder.DiscountValue == null)
                {
                    parameter.CustomerOrder.DiscountValue = 0;
                }

                //Kiểm tra chiết khấu của sản phẩm
                if (parameter.CustomerOrderDetail.Count > 0)
                {
                    var listProduct = parameter.CustomerOrderDetail.ToList();
                    listProduct.ForEach(item =>
                    {
                        if (item.DiscountValue == null)
                        {
                            item.DiscountValue = 0;
                        }
                    });
                }

                if (parameter.TypeAccount == 1)
                {
                    var AccountKHL = context.Customer.Where(item => item.CustomerCode == "KHL001").SingleOrDefault();
                    parameter.CustomerOrder.CustomerId = AccountKHL.CustomerId;
                }

                parameter.CustomerOrderDetail.ForEach(item =>
                {
                    //Nếu là chi phí khác thì tên sản phẩm dịch vụ sẽ bằng trường Description
                    if (item.OrderDetailType == 1)
                    {
                        item.ProductName = item.Description;
                    }

                    item.CreatedById = parameter.UserId;
                    item.CreatedDate = DateTime.Now;
                    item.Active = true;
                    item.OrderDetailId = Guid.NewGuid();
                    foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                    {
                        itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                    }
                });

                parameter.OrderCostDetail.ForEach(item =>
                {
                    item.OrderCostDetailId = Guid.NewGuid();
                    item.CreatedById = parameter.CustomerOrder.CreatedById;
                    item.CreatedDate = DateTime.Now;
                });

                parameter.CustomerOrder.OrderId = Guid.NewGuid();
                parameter.CustomerOrder.OrderCode = GenerateOrderCode(1);

                //Kiểm tra trường hợp đã tồn tại Mã đơn hàng
                var duplicateOrder =
                    context.CustomerOrder.FirstOrDefault(x => x.OrderCode == parameter.CustomerOrder.OrderCode);
                if (duplicateOrder != null)
                {
                    return new CreateCustomerOrderResult
                    {
                        Status = false,
                        Message = CommonMessage.Order.ORDER_EXIST
                    };
                }

                #region Kiểm tra khách hàng là Lead thì thêm Lead vào Customer

                if (parameter.Contact != null)
                {
                    if (parameter.Contact.ObjectType == "LEA")
                    {
                        var leadId = parameter.Contact.ObjectId;
                        var checkCustomer =
                            context.Customer.FirstOrDefault(f => f.LeadId == parameter.Contact.ObjectId);
                        if (checkCustomer == null)
                        {

                            #region Thêm Lead vào bảng khách hàng (Customer)

                            var newContact = InsertLeadToCustomer(parameter.Contact, parameter.UserId);
                            parameter.CustomerOrder.CustomerId = newContact == null ? Guid.Empty : newContact.ObjectId;

                            #endregion
                        }
                        else
                        {
                            parameter.CustomerOrder.CustomerId = checkCustomer.CustomerId;
                        }

                        #region Đổi trạng thái báo giá: Đóng-Trúng thầu

                        UpdateStatusQuote(parameter.QuoteId.Value);

                        #endregion

                        #region Đổi Trạng thái Lead: Ký hợp đồng

                        var checkUpdateStatus = UpdateStatusLead(leadId);

                        #endregion
                    }
                }

                #endregion

                #region Nếu đơn hàng có số tiền đã thanh toán > 0 thì chuyển trạng thái đơn hàng thành đã thanh toán 

                if (parameter.CustomerOrder.ReceiptInvoiceAmount > 0)
                {
                    var orderSttID = context.OrderStatus.FirstOrDefault(ord => ord.OrderStatusCode == "PD")
                        ?.OrderStatusId;
                    parameter.CustomerOrder.StatusId = orderSttID;
                }

                #endregion

                parameter.CustomerOrder.CreatedById = parameter.UserId;
                parameter.CustomerOrder.CreatedDate = DateTime.Now;
                parameter.CustomerOrder.CustomerOrderDetail = parameter.CustomerOrderDetail;
                parameter.CustomerOrder.OrderCostDetail = parameter.OrderCostDetail;
                context.CustomerOrder.Add(parameter.CustomerOrder);
                context.SaveChanges();

                #region Gửi thông báo

                NotificationHelper.AccessNotification(context, TypeModel.CustomerOrder, "CRE", new CustomerOrder(),
                    parameter.CustomerOrder, true);

                #endregion

                #region Tạo ReceiptInvoice, BankReceiptInvoice

                string PaymentMethodCode = "CASH";
                if (parameter.CustomerOrder.PaymentMethod != null)
                {
                    PaymentMethodCode = context.Category
                        .FirstOrDefault(cat => cat.CategoryId == parameter.CustomerOrder.PaymentMethod)?.CategoryCode;
                }

                if (parameter.CustomerOrder.ReceiptInvoiceAmount > 0)
                {
                    if (PaymentMethodCode == "CASH")
                    {
                        var newReceiptInvoice = new ReceiptInvoice();
                        var newReceiptInvoiceMapping = new ReceiptInvoiceMapping();
                        //Lấy ra "Thu tiền khách hàng"
                        var ReceiptInvoiceReasonID = (from cat in context.Category
                                                      join CaType in context.CategoryType on cat.CategoryTypeId equals CaType.CategoryTypeId
                                                      where cat.CategoryCode == "THA" && CaType.CategoryTypeCode == "LTH"
                                                      select cat).FirstOrDefault();

                        newReceiptInvoice.ReceiptInvoiceReason = ReceiptInvoiceReasonID.CategoryId;
                        //Lấy ra OrganizationId
                        var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                        var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);

                        newReceiptInvoice.OrganizationId = employee.OrganizationId;
                        //Lấy ra status "Đã vào sổ"
                        var ReceiptInvoiceStatusId = (from cat in context.Category
                                                      join CaType in context.CategoryType on cat.CategoryTypeId equals CaType.CategoryTypeId
                                                      where cat.CategoryName.Trim() == "Đã vào sổ" && CaType.CategoryTypeCode == "TCH"
                                                      select cat).FirstOrDefault();

                        newReceiptInvoice.StatusId = ReceiptInvoiceStatusId.CategoryId;
                        newReceiptInvoice.ReceiptDate = DateTime.Now;
                        newReceiptInvoice.RecipientName = context.Customer
                            .Where(w => w.CustomerId == parameter.CustomerOrder.CustomerId).FirstOrDefault()
                            .CustomerName;
                        newReceiptInvoice.ReceiptInvoiceDetail = string.Format("Thu tiền khách hàng {0} ngay",
                            newReceiptInvoice.RecipientName);
                        newReceiptInvoice.UnitPrice = parameter.CustomerOrder.ReceiptInvoiceAmount;
                        ///
                        var UnitName = (from cat in context.Category
                                        join CaType in context.CategoryType on cat.CategoryTypeId equals CaType.CategoryTypeId
                                        where cat.CategoryCode == "VND" && CaType.CategoryTypeCode == "DTI"
                                        select cat).FirstOrDefault();
                        newReceiptInvoice.CurrencyUnit = UnitName.CategoryId;


                        var organizationCode = context.Organization
                            .FirstOrDefault(o => o.OrganizationId == employee.OrganizationId)?.OrganizationCode;

                        newReceiptInvoice.ReceiptInvoiceId = Guid.NewGuid();
                        newReceiptInvoice.CreatedById = parameter.UserId;
                        newReceiptInvoice.CreatedDate = DateTime.Now;
                        newReceiptInvoice.ReceiptInvoiceCode = "PT" + "-" + organizationCode + DateTime.Now.Year
                                                               + (context.ReceiptInvoice.Count(r =>
                                                                      r.CreatedDate.Year == DateTime.Now.Year) + 1)
                                                               .ToString("D5");

                        newReceiptInvoiceMapping.ReceiptInvoiceMappingId = Guid.NewGuid();
                        newReceiptInvoiceMapping.ReceiptInvoiceId = newReceiptInvoice.ReceiptInvoiceId;
                        newReceiptInvoiceMapping.CreatedById = parameter.UserId;
                        newReceiptInvoiceMapping.CreatedDate = DateTime.Now;
                        newReceiptInvoiceMapping.ObjectId = parameter.CustomerOrder.CustomerId;

                        context.ReceiptInvoiceMapping.Add(newReceiptInvoiceMapping);
                        context.ReceiptInvoice.Add(newReceiptInvoice);
                        context.SaveChanges();
                    }
                    /////Tao BankReceiptInvoice
                    else
                    {
                        var newBankReceiptInvoice = new BankReceiptInvoice();
                        var newBankReceiptInvoiceMapping = new BankReceiptInvoiceMapping();
                        //Lấy ra "Thu tiền khách hàng"
                        var ReceiptInvoiceReasonID = (from cat in context.Category
                                                      join CaType in context.CategoryType on cat.CategoryTypeId equals CaType.CategoryTypeId
                                                      where cat.CategoryCode == "THA" && CaType.CategoryTypeCode == "LTH"
                                                      select cat).FirstOrDefault();

                        newBankReceiptInvoice.BankReceiptInvoiceReason = ReceiptInvoiceReasonID.CategoryId;
                        //Lấy ra OrganizationId
                        var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                        var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);

                        newBankReceiptInvoice.OrganizationId = employee.OrganizationId;
                        //Lấy ra status "Đã vào sổ"
                        var ReceiptInvoiceStatusId = (from cat in context.Category
                                                      join CaType in context.CategoryType on cat.CategoryTypeId equals CaType.CategoryTypeId
                                                      where cat.CategoryName.Trim() == "Đã vào sổ" && CaType.CategoryTypeCode == "TCH"
                                                      select cat).FirstOrDefault();

                        newBankReceiptInvoice.StatusId = ReceiptInvoiceStatusId.CategoryId;
                        newBankReceiptInvoice.BankReceiptInvoicePaidDate = DateTime.Now;
                        newBankReceiptInvoice.BankReceiptInvoiceDetail =
                            string.Format("Thu tiền khách hàng tại hóa đơn {0} ", parameter.CustomerOrder.OrderCode);
                        newBankReceiptInvoice.BankReceiptInvoicePrice = parameter.CustomerOrder.ReceiptInvoiceAmount;
                        newBankReceiptInvoice.BankReceiptInvoiceAmount = parameter.CustomerOrder.ReceiptInvoiceAmount;
                        ///
                        var UnitName = (from cat in context.Category
                                        join CaType in context.CategoryType on cat.CategoryTypeId equals CaType.CategoryTypeId
                                        where cat.CategoryCode == "VND" && CaType.CategoryTypeCode == "DTI"
                                        select cat).FirstOrDefault();
                        newBankReceiptInvoice.BankReceiptInvoicePriceCurrency = UnitName.CategoryId;
                        newBankReceiptInvoice.BankReceiptInvoiceBankAccountId = parameter.CustomerOrder.BankAccountId;

                        var organizationCode = context.Organization
                            .FirstOrDefault(o => o.OrganizationId == employee.OrganizationId)?.OrganizationCode;

                        var newBankReceiptInvoiceId = Guid.NewGuid();
                        newBankReceiptInvoice.BankReceiptInvoiceId = newBankReceiptInvoiceId;
                        newBankReceiptInvoice.CreatedDate = DateTime.Now;
                        newBankReceiptInvoice.CreatedById = parameter.UserId;
                        newBankReceiptInvoice.BankReceiptInvoiceCode =
                            "BC" + "-" + organizationCode + DateTime.Now.Year +
                            (context.BankReceiptInvoice.Count(b => b.CreatedDate.Year == DateTime.Now.Year) + 1)
                            .ToString("D5");


                        newBankReceiptInvoiceMapping.BankReceiptInvoiceMappingId = Guid.NewGuid();
                        newBankReceiptInvoiceMapping.CreatedById = parameter.UserId;
                        newBankReceiptInvoiceMapping.CreatedDate = DateTime.Now;
                        newBankReceiptInvoiceMapping.BankReceiptInvoiceId = newBankReceiptInvoiceId;
                        newBankReceiptInvoiceMapping.ObjectId = parameter.CustomerOrder.CustomerId;

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
                        context.BankReceiptInvoiceMapping.Add(newBankReceiptInvoiceMapping);
                        context.BankReceiptInvoice.Add(newBankReceiptInvoice);
                        context.SaveChanges();
                    }
                }

                #endregion

                #region Get CustomerOrder Infor To Send Email

                //chỉ gửi email cho những đơn hàng KHÁC trạng thái nháp
                var draftStatus = context.OrderStatus.FirstOrDefault(f => f.OrderStatusCode == "DRA").OrderStatusId;
                var SendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();

                #region Giang comment: Không còn trạng thái Nháp nữa nên luôn gửi email khi tạo đơn hàng

                //if (parameter.CustomerOrder.StatusId != draftStatus)
                //{

                //}

                #endregion

                SendEmailEntityModel.OrderCode = parameter.CustomerOrder.OrderCode;
                SendEmailEntityModel.OrderStatus = context.OrderStatus.FirstOrDefault(f => f.OrderStatusId == parameter.CustomerOrder.StatusId)?.Description ?? "";
                var _orderDate = parameter.CustomerOrder.OrderDate;
                SendEmailEntityModel.OrderDate = _orderDate.Day.ToString("00") + "/" + _orderDate.Month.ToString("00") + "/" + _orderDate.Year.ToString("0000");

                //thong tin khach hang va nguoi nhan
                var _contactCustomer = context.Contact.FirstOrDefault(w => w.ObjectId == parameter.CustomerOrder.CustomerId && w.ObjectType == "CUS");
                SendEmailEntityModel.CustomerName = _contactCustomer?.FirstName + " " + _contactCustomer?.LastName;
                SendEmailEntityModel.CustomerEmail = _contactCustomer.Email ?? "";
                SendEmailEntityModel.CustomerPhone = _contactCustomer.Phone ?? "";
                SendEmailEntityModel.RecipientName = parameter.CustomerOrder.RecipientName ?? "";
                SendEmailEntityModel.PlaceOfDelivery = parameter.CustomerOrder.PlaceOfDelivery ?? "";
                var _receivedDate = parameter.CustomerOrder.ReceivedDate;
                var _receivedHour = parameter.CustomerOrder.ReceivedHour;
                var _receivedDateHour = "";
                if (_receivedDate != null)
                {
                    _receivedDateHour += _receivedDate.Value.Day.ToString("00") + "/" + _receivedDate.Value.Month.ToString("00") + "/" + _receivedDate.Value.Year.ToString("0000");
                }
                if (_receivedHour != null)
                {
                    _receivedDateHour += " " + _receivedHour.Value.Hours.ToString("00") + ":" + _receivedHour.Value.Minutes.ToString("00");
                }
                SendEmailEntityModel.ReceivedDateHour = _receivedDateHour ?? "";
                SendEmailEntityModel.RecipientPhone = parameter.CustomerOrder.RecipientPhone ?? "";
                var _company = context.CompanyConfiguration.FirstOrDefault();
                SendEmailEntityModel.CompanyName = _company?.CompanyName ?? "";
                SendEmailEntityModel.CompanyAddress = _company?.CompanyAddress ?? "";
                SendEmailEntityModel.CompanyEmail = _company?.Email ?? "";
                SendEmailEntityModel.CompanyPhone = _company?.Phone ?? "";

                //gửi email đến: người tạo, nhân viên bán hàng, khách hàng
                SendEmailEntityModel.ListSendToEmail.Add(_contactCustomer.Email); //khách hàng
                                                                                  //người tạo
                var createdUserContact = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;
                var createdUserEmail = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == createdUserContact).Email;
                if (createdUserEmail != null && createdUserEmail != _contactCustomer.Email)
                {
                    SendEmailEntityModel.ListSendToEmail.Add(createdUserEmail);
                }
                //nhân viên bán hàng
                var sellerContactEmail = context.Contact.FirstOrDefault(f => f.ObjectId == parameter.CustomerOrder.Seller).Email;
                if (sellerContactEmail != null && sellerContactEmail != createdUserEmail && sellerContactEmail != _contactCustomer.Email)
                {
                    {
                        SendEmailEntityModel.ListSendToEmail.Add(sellerContactEmail);
                    }
                }

                #endregion

                return new CreateCustomerOrderResult
                {
                    Message = CommonMessage.Order.CREATE_SUCCESS,
                    Status = true,
                    CustomerOrderID = parameter.CustomerOrder.OrderId,
                    SendEmailEntityModel = SendEmailEntityModel
                };
            }
            catch (Exception e)
            {
                return new CreateCustomerOrderResult
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }

        private bool UpdateStatusQuote(Guid _QuoteId)
        {
            var quote = context.Quote.FirstOrDefault(f => f.QuoteId == _QuoteId);
            if (quote != null)
            {
                var categoryTypeQuote = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TGI");
                if (categoryTypeQuote != null)
                {
                    var statusQuote = context.Category.FirstOrDefault(f => f.CategoryTypeId == categoryTypeQuote.CategoryTypeId && f.CategoryCode == "DTH");
                    if (statusQuote != null)
                    {
                        quote.StatusId = statusQuote.CategoryId;
                        context.Quote.Update(quote);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UpdateStatusLead(Guid leadId)
        {
            var lead = context.Lead.FirstOrDefault(f => f.LeadId == leadId);
            if (lead != null)
            {
                var categoryTypeLead = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE");
                if (categoryTypeLead != null)
                {
                    var statusLead = context.Category.FirstOrDefault(f => f.CategoryTypeId == categoryTypeLead.CategoryTypeId && f.CategoryCode == "KHD");
                    if (statusLead != null)
                    {
                        lead.StatusId = statusLead.CategoryId;
                        context.Lead.Update(lead);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        private string GenerateCustomerCode()
        {
            //Auto gen CustomerCode 1911190001
            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;
            int currentDate = DateTime.Now.Day;
            var customer = context.Customer.OrderByDescending(or => or.CreatedDate).FirstOrDefault();
            int MaxNumberCode = 0;
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
            return string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
        }
        public Contact InsertLeadToCustomer(Contact _Contact, Guid _UserId)
        {
            if (_Contact != null)
            {
                var statusType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "THA");
                var groupType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA");
                var StatusCustomer = context.Category.FirstOrDefault(f =>
                    f.CategoryTypeId == statusType.CategoryTypeId && f.CategoryCode == "MOI");
                var groupCusstomer =
                    context.Category.FirstOrDefault(f =>
                        f.CategoryTypeId == groupType.CategoryTypeId && f.IsDefauld == true) ??
                    context.Category.FirstOrDefault(f => f.CategoryTypeId == groupType.CategoryTypeId);
                var typeLead = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHL");
                var listTypeLead =
                    context.Category.Where(x => x.Active == true && x.CategoryTypeId == typeLead.CategoryTypeId)
                        .ToList();
                var typeLeadId = context.Lead.FirstOrDefault(x => x.LeadId == _Contact.ObjectId).LeadTypeId;

                short customerType = 2;   //Khách hàng cá nhân
                if (typeLeadId != null)
                {
                    var code = listTypeLead.FirstOrDefault(x => x.CategoryId == typeLeadId).CategoryCode;
                    if (code == "KPL")
                    {
                        customerType = 2;   //Khách hàng cá nhân
                    }
                    else if (code == "KCL")
                    {
                        customerType = 1;   //Khách hàng doanh nghiệp
                    }
                }

                Customer newCustomer = new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerCode = GenerateCustomerCode(),
                    CustomerName = _Contact.FirstName + " " + _Contact.LastName,
                    CustomerGroupId = groupCusstomer != null ? groupCusstomer.CategoryId : Guid.Empty,
                    LeadId = _Contact.ObjectId,
                    StatusId = StatusCustomer != null ? StatusCustomer.CategoryId : Guid.Empty,
                    CustomerServiceLevelId = null,
                    PersonInChargeId = null,
                    CustomerType = customerType,
                    PaymentId = null,
                    FieldId = null,
                    ScaleId = null,
                    MaximumDebtValue = 0,
                    MaximumDebtDays = 0,
                    TotalSaleValue = 0,
                    TotalReceivable = 0,
                    NearestDateTransaction = null,
                    TotalCapital = 0,
                    BusinessRegistrationDate = null,
                    EnterpriseType = null,
                    TotalEmployeeParticipateSocialInsurance = 0,
                    TotalRevenueLastYear = 0,
                    BusinessType = null,
                    BusinessScale = null,
                    NumberCode = null,
                    YearCode = null,
                    MonthCode = null,
                    DateCode = null,
                    MainBusinessSector = null,
                    CustomerCareStaff = null,
                    Active = true,
                    CreatedById = _UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedById = null,
                    UpdatedDate = null
                };

                context.Customer.Add(newCustomer);

                Contact newContact = _Contact;
                newContact.ContactId = Guid.NewGuid();
                newContact.ObjectId = newCustomer.CustomerId;
                newContact.ObjectType = "CUS";
                newContact.CreatedById = _UserId;
                newContact.CreatedDate = DateTime.Now;

                context.Contact.Add(newContact);
                context.SaveChanges();

                return newContact;
            }
            return null;
        }

        public GetAllCustomerOrderResult GetAllCustomerOrder(GetAllCustomerOrderParameter parameter)
        {
            List<CustomerOrderEntityModel> listOrder = new List<CustomerOrderEntityModel>();
            List<CustomerOrderEntityModel> newListOrderBy = new List<CustomerOrderEntityModel>();
            var listAllUser = context.User.ToList();
            var listAllEmployee = context.Employee.ToList();
            var listAllStatus = context.OrderStatus.ToList();
            var listAllContact = context.Contact.ToList();

            //check isManager
            var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
            if (user == null)
            {
                return new GetAllCustomerOrderResult()
                {
                    Status = false,
                    Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                };
            }
            if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
            {
                return new GetAllCustomerOrderResult()
                {
                    Status = false,
                    Message = "Lỗi dữ liệu"
                };
            }

            var employeeId = user.EmployeeId;
            var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
            if (employee == null)
            {
                return new GetAllCustomerOrderResult()
                {
                    Status = false,
                    Message = "Lỗi dữ liệu"
                };
            }
            var isManager = employee.IsManager;

            var orderCode = parameter.OrderCode == null ? "" : parameter.OrderCode.Trim();
            var customerName = parameter.CustomerName == null ? "" : parameter.CustomerName.Trim();
            var phone = parameter.Phone == null ? "" : parameter.Phone.Trim();
            var fromDate = parameter.OrderDateStart;
            var toDate = parameter.OrderDateEnd;
            var listStatusCode = new List<string>()
            {
                "DLV", // Đơn hàng bán
                "DRA", // Mới
                "IP", // Chờ phê duyệt
                "COMP", // Đóng
                "RTN", // Từ chối
                "CAN", // Hủy
            };
            int vat = parameter.Vat;

            var lstOrderBy = context.CustomerOrder.Join(context.Customer, or => or.CustomerId, cus => cus.CustomerId,
                    (or, cus) => new { or, cus })
                .Where(x => x.or.Active == true && (customerName == "" || x.cus.CustomerName.Contains(customerName)) &&
                            (orderCode == "" || x.or.OrderCode.Contains(orderCode)) &&
                            (listStatusCode == null || listStatusCode.Count == 0 || listStatusCode.Contains(x.or.Status.OrderStatusCode)) &&
                            (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.or.OrderDate) &&
                            (toDate == null || toDate == DateTime.MinValue || toDate >= x.or.OrderDate))
                .Select(m => new CustomerOrderEntityModel
                {
                    OrderId = m.or.OrderId,
                    OrderCode = m.or.OrderCode,
                    OrderDate = m.or.OrderDate,
                    Seller = m.or.Seller,
                    SellerName = listAllEmployee.FirstOrDefault(e => e.EmployeeId == m.or.Seller) == null ? "" : listAllEmployee.FirstOrDefault(e => e.EmployeeId == m.or.Seller).EmployeeName,
                    Description = m.or.Description,
                    Note = m.or.Note,
                    CustomerId = m.or.CustomerId,
                    CustomerName = m.cus.CustomerName,
                    CustomerContactId = Guid.Empty,
                    PaymentMethod = m.or.PaymentMethod,
                    DaysAreOwed = m.or.DaysAreOwed,
                    MaxDebt = m.or.MaxDebt,
                    ReceivedDate = m.or.ReceivedDate,
                    ReceivedHour = m.or.ReceivedHour,
                    RecipientName = m.or.RecipientName,
                    LocationOfShipment = m.or.LocationOfShipment,
                    ShippingNote = m.or.ShippingNote,
                    RecipientPhone = m.or.RecipientPhone,
                    RecipientEmail = m.or.RecipientEmail,
                    PlaceOfDelivery = m.or.PlaceOfDelivery,
                    Amount = (decimal)((m.or.DiscountType == true)
                        ? (m.or.Amount * (1 - (m.or.DiscountValue / 100)))
                        : (m.or.Amount - m.or.DiscountValue)),
                    DiscountValue = m.or.DiscountValue,
                    StatusId = m.or.StatusId,
                    OrderStatusName = listAllStatus.FirstOrDefault(s => s.OrderStatusId == m.or.StatusId).Description,
                    CreatedById = m.or.CreatedById,
                    CreatedDate = m.or.CreatedDate,
                    UpdatedById = m.or.UpdatedById,
                    UpdatedDate = m.or.UpdatedDate,
                    Active = m.or.Active,
                    DiscountType = m.or.DiscountType,
                    SellerAvatarUrl = "",
                    SellerFirstName = "",
                    SellerLastName = "",
                    ListOrderDetail = ""
                }).ToList();

            #region Kiểu LinQ
            //var lstOrderByParam = (from order in context.CustomerOrder
            //                       join customer in context.Customer on order.CustomerId equals customer.CustomerId
            //                       where order.Active == true &&
            //                             (orderCode == "" || order.OrderCode.Contains(orderCode)) &&
            //                             (customerName == "" || customer.CustomerName.Contains(customerName)) &&
            //                             (listStatusId == null || listStatusId.Count == 0 || listStatusId.Contains(order.StatusId)) &&
            //                             (fromDate == null || fromDate == DateTime.MinValue || fromDate <= order.OrderDate) &&
            //                             (toDate == null || toDate == DateTime.MinValue || toDate >= order.OrderDate)
            //                       select new CustomerOrderEntityModel
            //                       {
            //                           OrderId = order.OrderId,
            //                           OrderCode = order.OrderCode,
            //                           OrderDate = order.OrderDate,
            //                           Seller = order.Seller,
            //                           SellerName = listAllEmployee.FirstOrDefault(e => e.EmployeeId == order.Seller) == null ? "" : listAllEmployee.FirstOrDefault(e => e.EmployeeId == order.Seller).EmployeeName,
            //                           Description = order.Description,
            //                           Note = order.Note,
            //                           CustomerId = order.CustomerId,
            //                           CustomerName = customer.CustomerName,
            //                           CustomerContactId = Guid.Empty,
            //                           PaymentMethod = order.PaymentMethod,
            //                           DaysAreOwed = order.DaysAreOwed,
            //                           MaxDebt = order.MaxDebt,
            //                           ReceivedDate = order.ReceivedDate,
            //                           ReceivedHour = order.ReceivedHour,
            //                           RecipientName = order.RecipientName,
            //                           LocationOfShipment = order.LocationOfShipment,
            //                           ShippingNote = order.ShippingNote,
            //                           RecipientPhone = order.RecipientPhone,
            //                           RecipientEmail = order.RecipientEmail,
            //                           PlaceOfDelivery = order.PlaceOfDelivery,
            //                           Amount = (decimal)((order.DiscountType == true)
            //                               ? (order.Amount * (1 - (order.DiscountValue / 100)))
            //                               : (order.Amount - order.DiscountValue)),
            //                           DiscountValue = order.DiscountValue,
            //                           StatusId = order.StatusId,
            //                           OrderStatusName = listAllStatus.FirstOrDefault(x => x.OrderStatusId == order.StatusId).Description,
            //                           CreatedById = order.CreatedById,
            //                           CreatedDate = order.CreatedDate,
            //                           UpdatedById = order.UpdatedById,
            //                           UpdatedDate = order.UpdatedDate,
            //                           Active = order.Active,
            //                           DiscountType = order.DiscountType,
            //                           SellerAvatarUrl = "",
            //                           SellerFirstName = "",
            //                           SellerLastName = "",
            //                           ListOrderDetail = ""
            //                       }).ToList();
            #endregion

            var lstCustomerId = new List<Guid>();
            lstOrderBy.ForEach(item =>
            {
                var dupblicate = lstCustomerId.FirstOrDefault(x => x == item.CustomerId);
                if (dupblicate == Guid.Empty)
                {
                    lstCustomerId.Add(item.CustomerId);
                }
            });

            var lstContact = context.Contact.Where(x =>
                (x.ObjectType == ObjectType.CUSTOMER || x.ObjectType == ObjectType.CUSTOMERCONTACT) &&
                (lstCustomerId == null || lstCustomerId.Count == 0 || lstCustomerId.Contains(x.ObjectId)) &&
                (phone == "" || (x.Phone != null && x.Phone.ToLower().Contains(phone.ToLower())) ||
                 (x.WorkPhone != null && x.WorkPhone.ToLower().Contains(phone.ToLower())) ||
                 (x.OtherPhone != null && x.OtherPhone.ToLower().Contains(phone.ToLower())))).ToList();

            if (isManager)
            {
                //Nếu là quản lý

                //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                List<Guid?> listGetAllChild = new List<Guid?>();
                if (employee.OrganizationId != null)
                {
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                }
                //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();

                listEmployeeInChargeByManager.ForEach(item =>
                {
                    if (item.EmployeeId != Guid.Empty)
                        listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                });

                lstOrderBy = lstOrderBy.Where(x =>
                    (listEmployeeInChargeByManagerId.Count == 0 ||
                     (x.Seller != null && listEmployeeInChargeByManagerId.Contains(x.Seller.Value)))).ToList();

                List<Guid> lstOrderId = new List<Guid>();
                lstOrderBy.ForEach(item =>
                {
                    lstOrderId.Add(item.OrderId);
                });
                var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();
                List<Guid> lstProductId = new List<Guid>();
                listOrderDetail.ForEach(item =>
                {
                    var dublicateProduct = lstProductId.FirstOrDefault(x => x == item.ProductId);
                    if (dublicateProduct == Guid.Empty)
                    {
                        if (item.ProductId != null)
                            lstProductId.Add(item.ProductId.Value);
                    }
                });
                var listProduct = context.Product.Where(x => (lstProductId == null || lstProductId.Count == 0 || lstProductId.Contains(x.ProductId))).ToList();

                lstOrderBy.ForEach(item =>
                {
                    var contact = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                    if (contact != null)
                    {
                        var contactCustomer = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == ObjectType.CUSTOMER);
                        var contactSeller = listAllContact.FirstOrDefault(x => x.ObjectId == item.Seller && x.ObjectType == ObjectType.EMPLOYEE);

                        item.CustomerContactId = contactCustomer == null ? Guid.Empty : contactCustomer.ContactId;
                        item.SellerFirstName = contactSeller == null ? "" : contactSeller.FirstName;
                        item.SellerLastName = contactSeller == null ? "" : contactSeller.LastName;

                        var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
                        if (orderDetail.Count > 0)
                        {
                            orderDetail.ForEach(currentOrderDetail =>
                            {
                                var productName = "";
                                var product = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId);
                                if (product != null)
                                {
                                    productName = product.ProductName + "; ";
                                }
                                item.ListOrderDetail += productName;
                            });
                        }
                        else
                        {
                            item.ListOrderDetail = "";
                        }

                        newListOrderBy.Add(item);
                    }
                });
            }
            else
            {
                //Nếu là nhân viên
                lstOrderBy = lstOrderBy.Where(x => x.Seller == employeeId).ToList();

                List<Guid> lstOrderId = new List<Guid>();
                lstOrderBy.ForEach(item =>
                {
                    lstOrderId.Add(item.OrderId);
                });
                var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();
                List<Guid> lstProductId = new List<Guid>();
                listOrderDetail.ForEach(item =>
                {
                    var dublicateProduct = lstProductId.FirstOrDefault(x => x == item.ProductId);
                    if (dublicateProduct == Guid.Empty)
                    {
                        if (item.ProductId != null)
                            lstProductId.Add(item.ProductId.Value);
                    }
                });
                var listProduct = context.Product.Where(x => (lstProductId == null || lstProductId.Count == 0 || lstProductId.Contains(x.ProductId))).ToList();

                lstOrderBy.ForEach(item =>
                {
                    var contact = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                    if (contact != null)
                    {
                        var contactCustomer = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == ObjectType.CUSTOMER);
                        var contactSeller = listAllContact.FirstOrDefault(x => x.ObjectId == item.Seller && x.ObjectType == ObjectType.EMPLOYEE);

                        item.CustomerContactId = contactCustomer == null ? Guid.Empty : contactCustomer.ContactId;
                        item.SellerFirstName = contactSeller == null ? "" : contactSeller.FirstName;
                        item.SellerLastName = contactSeller == null ? "" : contactSeller.LastName;

                        var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
                        if (orderDetail.Count > 0)
                        {
                            orderDetail.ForEach(currentOrderDetail =>
                            {
                                var productName = "";
                                var product = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId);
                                if (product != null)
                                {
                                    productName = product.ProductName + "; ";
                                }
                                item.ListOrderDetail += productName;
                            });
                        }
                        else
                        {
                            item.ListOrderDetail = "";
                        }

                        newListOrderBy.Add(item);
                    }
                });
            }

            #region comment by Giang
            //parameter.CustomerName = parameter.CustomerName == null ? parameter.CustomerName : parameter.CustomerName.Trim();
            //parameter.OrderCode = parameter.OrderCode == null ? parameter.OrderCode : parameter.OrderCode.Trim();
            //var commonOrganization = context.Organization.ToList();
            //int organizationCount = parameter.ListOrganizationId == null ? 0 : parameter.ListOrganizationId.Count;

            //if (parameter.ListOrganizationId != null && parameter.ListOrganizationId.Count == 1)
            //{
            //    parameter.ListOrganizationId = getOrganizationChildrenId(commonOrganization, parameter.ListOrganizationId[0], parameter.ListOrganizationId);
            //}
            #endregion

            #region By Hung code
            //var orderList = (from or in context.CustomerOrder
            //                 join employee in context.Employee on or.Seller equals employee.EmployeeId
            //                 join organization in context.Organization on employee.OrganizationId equals organization.OrganizationId
            //                 where (parameter.OrderStatusId==null||parameter.OrderStatusId.Contains(or.StatusId) || parameter.OrderStatusId.Count == 0)
            //                  && (parameter.Seller == null || parameter.Seller == or.Seller)
            //                  && (string.IsNullOrEmpty(parameter.OrderCode) || or.OrderCode.Contains(parameter.OrderCode))
            //                  //&& (string.IsNullOrEmpty(parameter.CustomerName) || cus.CustomerName.Contains(parameter.CustomerName))
            //                  && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value <= or.OrderDate)
            //                  && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value >= or.OrderDate)
            //                  && (parameter.ListOrganizationId.Contains(organization.OrganizationId) || organizationCount == 0)
            //                 select new CustomerOrderEntityModel
            //                 {
            //                     OrderId = or.OrderId,
            //                     OrderCode = or.OrderCode,
            //                     OrderDate = or.OrderDate,
            //                     Seller = or.Seller,
            //                     Description = or.Description,
            //                     Note = or.Note,
            //                     CustomerId = or.CustomerId,
            //                     CustomerContactId = Guid.Empty,    //add by Giang
            //                     PaymentMethod = or.PaymentMethod,
            //                     DaysAreOwed = or.DaysAreOwed,
            //                     MaxDebt = or.MaxDebt,
            //                     ReceivedDate = or.ReceivedDate,
            //                     ReceivedHour = or.ReceivedHour,
            //                     RecipientName = or.RecipientName,
            //                     LocationOfShipment = or.LocationOfShipment,
            //                     ShippingNote = or.ShippingNote,
            //                     RecipientPhone = or.RecipientPhone,
            //                     RecipientEmail = or.RecipientEmail,
            //                     PlaceOfDelivery = or.PlaceOfDelivery,
            //                     Amount = (decimal)((or.DiscountType == true) ? (or.Amount * (1 - (or.DiscountValue / 100))) : (or.Amount - or.DiscountValue)),
            //                     DiscountValue = or.DiscountValue,
            //                     StatusId = or.StatusId,
            //                     CreatedById = or.CreatedById,
            //                     CreatedDate = or.CreatedDate,
            //                     UpdatedById = or.UpdatedById,
            //                     UpdatedDate = or.UpdatedDate,
            //                     Active = or.Active,
            //                     DiscountType = or.DiscountType,
            //                     SellerAvatarUrl = "",
            //                     SellerFirstName = "",
            //                     SellerLastName = "",
            //                     OrderStatusName = "",
            //                     CustomerName = ""
            //                 }).ToList();
            //List<Guid> listCustomerId = new List<Guid>();
            //List<Guid> listSeller = new List<Guid>();
            //List<Guid> listOrderStatusId = new List<Guid>();
            //List<Guid> listEmployeeId = new List<Guid>();
            //List<Guid> listOrderId = new List<Guid>();  //add by Giang

            //orderList.ForEach(item =>
            //{
            //    if (item.CustomerId != null)
            //        listCustomerId.Add(item.CustomerId);
            //    if (item.Seller != null)
            //    {
            //        listSeller.Add(item.Seller.Value);
            //        listEmployeeId.Add(item.Seller.Value);
            //    }
            //    if (item.StatusId != null)
            //        listOrderStatusId.Add(item.StatusId.Value);
            //    if (item.OrderId != null)               //add by Giang
            //        listOrderId.Add(item.OrderId);

            //});
            //var commonContact = context.Contact.Where(w => w.ObjectType == ObjectType.CUSTOMER || w.ObjectType == ObjectType.EMPLOYEE).ToList();
            //var listContact = commonContact.Where(e => listSeller.Contains(e.ObjectId)).ToList();
            //var listContactCustomer = commonContact.Where(e => listCustomerId.Contains(e.ObjectId) && e.ObjectType == "CUS").ToList();    //add by Giang
            //var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
            //var listOrderStatus = context.OrderStatus.Where(e => listOrderStatusId.Contains(e.OrderStatusId)).ToList();
            //var listEmployee = context.Employee.Where(e => listEmployeeId.Contains(e.EmployeeId)).ToList();
            //var listOrderDetail = context.CustomerOrderDetail.Where(e => listOrderId.Contains(e.OrderId)).ToList();
            //var listProduct = context.Product.ToList();

            //orderList.ForEach(item=> {
            //    if (item.Seller != null) {
            //        //item.SellerAvatarUrl = listContact.FirstOrDefault(e=>e.ObjectId==item.Seller).AvatarUrl;
            //        item.SellerFirstName = listEmployee.FirstOrDefault(e => e.EmployeeId == item.Seller).EmployeeName;
            //        item.SellerLastName = listContact.FirstOrDefault(e => e.ObjectId == item.Seller).LastName;
            //    }
            //    if (item.StatusId != null)
            //        item.OrderStatusName = listOrderStatus.FirstOrDefault(e => e.OrderStatusId == item.StatusId).Description;
            //    if (item.CustomerId != null)
            //    {
            //        var cus = listCustomer.FirstOrDefault(e => e.CustomerId == item.CustomerId);
            //        if (cus != null)
            //            item.CustomerName = cus.CustomerName;
            //        else
            //            item.CustomerName = "";
            //        var con = listContactCustomer.FirstOrDefault(e => e.ObjectId == item.CustomerId);
            //        if (con != null)
            //            item.CustomerContactId = con.ContactId;
            //        else
            //            item.CustomerContactId = Guid.Empty;
            //    }
            //    if (item.OrderId != null)
            //    {
            //        var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
            //        if (orderDetail.Count > 0)
            //        {
            //            orderDetail.ForEach(currentOrderDetail =>
            //            {
            //                var productName = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId) != null ?
            //                                    (listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId).ProductName + "; ") : "";
            //                item.ListOrderDetail += productName;
            //            });
            //        } else
            //        {
            //            item.ListOrderDetail = "";
            //        }
            //    }
            //});
            //orderList = orderList.Where(e=> string.IsNullOrEmpty(parameter.CustomerName) || e.CustomerName.Contains(parameter.CustomerName)).OrderByDescending(or => or.CreatedDate).ToList();
            #endregion

            #region Comment By Hung
            //var orderList = (from c in context.Contact
            //                 join or in context.CustomerOrder on c.ObjectId equals or.Seller
            //                 join cus in context.Customer on or.CustomerId equals cus.CustomerId
            //                 join os in context.OrderStatus on or.StatusId equals os.OrderStatusId
            //                 join e in context.Employee on or.Seller equals e.EmployeeId
            //                 where (parameter.OrderStatusId.Contains(or.StatusId) || parameter.OrderStatusId.Count == 0)
            //                 && (parameter.Seller == null || parameter.Seller == or.Seller)
            //                 && (string.IsNullOrEmpty(parameter.OrderCode) || or.OrderCode.Contains(parameter.OrderCode))
            //                 && (string.IsNullOrEmpty(parameter.CustomerName) || cus.CustomerName.Contains(parameter.CustomerName))
            //                 && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value <= or.OrderDate)
            //                 && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value >= or.OrderDate)
            //                 select new CustomerOrderEntityModel
            //                 {
            //                     OrderId = or.OrderId,
            //                     OrderCode = or.OrderCode,
            //                     OrderDate = or.OrderDate,
            //                     Description = or.Description,
            //                     Note = or.Note,
            //                     CustomerId = or.CustomerId,
            //                     PaymentMethod = or.PaymentMethod,
            //                     DaysAreOwed = or.DaysAreOwed,
            //                     MaxDebt = or.MaxDebt,
            //                     ReceivedDate = or.ReceivedDate,
            //                     ReceivedHour = or.ReceivedHour,
            //                     RecipientName = or.RecipientName,
            //                     LocationOfShipment = or.LocationOfShipment,
            //                     ShippingNote = or.ShippingNote,
            //                     RecipientPhone = or.RecipientPhone,
            //                     RecipientEmail = or.RecipientEmail,
            //                     PlaceOfDelivery = or.PlaceOfDelivery,
            //                     Amount = or.Amount,
            //                     DiscountValue = or.DiscountValue,
            //                     StatusId = or.StatusId,
            //                     CreatedById = or.CreatedById,
            //                     CreatedDate = or.CreatedDate,
            //                     UpdatedById = or.UpdatedById,
            //                     UpdatedDate = or.UpdatedDate,
            //                     Active = or.Active,
            //                     DiscountType = or.DiscountType,
            //                     SellerAvatarUrl = c.AvatarUrl,
            //                     SellerFirstName = e.EmployeeName,
            //                     SellerLastName = c.LastName,
            //                     OrderStatusName = os.Description,
            //                     CustomerName = cus.CustomerName
            //                 }
            //                 ).OrderByDescending(or => or.CreatedDate).ToList();
            #endregion

            #region Tìm những đơn hàng liên quan đến hóa đơn VAT
            if (parameter.Vat == 1)
            {
                //Đơn hàng có cả hóa đơn VAT và không có hóa đơn VAT
                listOrder = newListOrderBy;
            }
            else if (parameter.Vat == 2)
            {
                //Đơn hàng có hóa đơn VAT
                List<Guid> lstOrderId = new List<Guid>();
                newListOrderBy.ForEach(item =>
                {
                    lstOrderId.Add(item.OrderId);
                });
                var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();

                newListOrderBy.ForEach(item =>
                {
                    int countVat = listOrderDetail.Where(c => c.OrderId == item.OrderId && c.Vat > 0).Count();
                    if (countVat > 0)
                    {
                        listOrder.Add(item);
                    }
                });
            }
            else if (parameter.Vat == 3)
            {
                //Đơn hàng không có hóa đơn VAT
                List<Guid> lstOrderId = new List<Guid>();
                newListOrderBy.ForEach(item =>
                {
                    lstOrderId.Add(item.OrderId);
                });
                var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();

                newListOrderBy.ForEach(item =>
                {
                    var countVat = listOrderDetail.Where(c => c.OrderId == item.OrderId && c.Vat > 0).Count();
                    if (countVat == 0)
                    {
                        listOrder.Add(item);
                    }
                });
            }
            #endregion

            listOrder = listOrder.OrderByDescending(x => x.OrderDate).ToList();

            if (parameter.Top3NewOrder != null)
            {
                var orderTop3List = listOrder.OrderByDescending(or => or.OrderCode).Take(5).ToList();
                return new GetAllCustomerOrderResult()
                {
                    Status = true,
                    OrderTop5List = orderTop3List
                };
            }

            return new GetAllCustomerOrderResult()
            {
                Status = true,
                OrderList = listOrder
            };
        }

        public ProfitAccordingCustomersResult SearchProfitAccordingCustomers(ProfitAccordingCustomersParameter parameter)
        {
            try
            {
                var listOrder = new List<ProfitAccordingCustomersModel>();

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();

                //check isManager
                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);

                var isManager = employee.IsManager;

                var orderCode = parameter.OrderCode == null ? "" : parameter.OrderCode.Trim();
                var customerName = parameter.CustomerName == null ? "" : parameter.CustomerName.Trim();
                var fromDate = parameter.FromDate;
                var toDate = parameter.ToDate;
                var listStatusId = parameter.ListStatusId;

                var allStatusOrder = context.OrderStatus.Where(c => c.Active == true).ToList();
                var commonOrderStatus = allStatusOrder.Where(w =>
                    w.OrderStatusCode == "DLV" ||
                    w.OrderStatusCode == "COMP").Select(w => w.OrderStatusId).ToList();

                #region Thêm điều kiện lọc sản phẩm
                List<Guid> lstOrdId = new List<Guid>();
                if (parameter.ProductId != null && parameter.ProductId != Guid.Empty)
                {
                    lstOrdId = context.CustomerOrderDetail.Where(co => co.ProductId == parameter.ProductId).Select(co => co.OrderId).ToList();
                }
                #endregion

                var listOrderDetail = context.CustomerOrder.Join(context.CustomerOrderDetail, or => or.OrderId, od => od.OrderId,
                    (or, od) => new { or, od })
                .Where(x => x.or.Active == true &&
                            (orderCode == "" || x.or.OrderCode.Contains(orderCode)) &&
                            (listStatusId == null || listStatusId.Count == 0 || listStatusId.Contains(x.or.StatusId.Value)) &&
                            (commonOrderStatus == null || commonOrderStatus.Count == 0 || commonOrderStatus.Contains(x.or.StatusId.Value)) &&
                            (lstOrdId == null || lstOrdId.Count == 0 || lstOrdId.Contains(x.or.OrderId)) &&
                            (parameter.QuoteId == null || parameter.QuoteId == Guid.Empty || x.or.QuoteId == parameter.QuoteId) &&
                            (parameter.Seller == null || parameter.Seller == Guid.Empty || x.or.Seller == parameter.Seller) &&
                            (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.or.OrderDate) &&
                            (toDate == null || toDate == DateTime.MinValue || toDate >= x.or.OrderDate))
                        .Select(m => new ProfitAccordingCustomersModel
                        {
                            CustomerId = m.or.CustomerId,
                            CustomerName = null,
                            CustomerCode = null,
                            CapitalMoney = m.od.PriceInitial == null ? 0 : m.od.PriceInitial.Value,
                            GrossCapitalMoney = 0,
                            GrossProfit = 0,
                            GrossProfitMoney = 0,
                            ProfitMoney = m.or.Amount - (m.or.DiscountType == true ? (m.or.Amount * m.or.DiscountValue.Value / 100) : m.or.DiscountValue.Value),
                            STT = 0
                        }).ToList();

                listOrder = listOrderDetail.Join(context.Customer, or => or.CustomerId, cus => cus.CustomerId,
                    (or, cus) => new { or, cus })
                .Where(x => (customerName == "" || x.cus.CustomerName.Contains(customerName)))
                .Select(m => new ProfitAccordingCustomersModel
                {
                    CustomerId = m.or.CustomerId,
                    CustomerName = m.cus.CustomerName,
                    CustomerCode = m.cus.CustomerCode,
                    CapitalMoney = m.or.CapitalMoney,
                    GrossCapitalMoney = m.or.GrossCapitalMoney,
                    GrossProfit = m.or.GrossProfit,
                    GrossProfitMoney = m.or.GrossProfitMoney,
                    ProfitMoney = m.or.ProfitMoney,
                    STT = m.or.STT
                }).ToList();

                // lấy các phiếu nhập hàng bị trả lại
                var inventoryReceiving = context.InventoryReceivingVoucher.Where(i => i.Active == true && i.InventoryReceivingVoucherType == 2).ToList();
                listOrder = listOrder.GroupBy(x => new { x.CustomerId, x.CustomerName, x.CustomerCode }).Select(y => new ProfitAccordingCustomersModel
                {
                    CustomerId = y.Key.CustomerId,
                    CustomerName = y.Key.CustomerName,
                    CustomerCode = y.Key.CustomerCode,
                    CapitalMoney = y.Sum(s => s.CapitalMoney),
                    GrossCapitalMoney = 0,
                    GrossProfit = 0,
                    GrossProfitMoney = 0,
                    ProfitMoney = y.Sum(s => s.ProfitMoney),
                    STT = 0
                }).ToList();

                int index = 0;

                decimal SumProfitMoney = 0;
                decimal SumCapitalMoney = 0;
                decimal SumGrossProfit = 0;
                decimal SumGrossCapitalMoney = 0;
                decimal SumGrossProfitMoney = 0;

                listOrder.ForEach((item) =>
                {
                    index++;
                    item.STT = index;
                    item.GrossProfit = item.ProfitMoney - item.CapitalMoney;
                    item.GrossProfitMoney = item.ProfitMoney == 0 ? 100 : Math.Round((item.GrossProfit / item.ProfitMoney), 2);
                    item.GrossCapitalMoney = item.CapitalMoney == 0 ? 100 : Math.Round((item.GrossProfit / item.CapitalMoney), 2);
                    SumProfitMoney = SumProfitMoney + item.ProfitMoney;
                    SumCapitalMoney = SumCapitalMoney + item.CapitalMoney;
                    SumGrossProfit = SumGrossProfit + item.GrossProfit;
                    SumGrossCapitalMoney = SumGrossCapitalMoney + item.GrossCapitalMoney;
                    SumGrossProfitMoney = SumGrossProfitMoney + item.GrossProfitMoney;
                });

                return new ProfitAccordingCustomersResult
                {
                    Message = "Success",
                    ListProfitAccordingCustomers = listOrder,
                    Status = true,
                    SumCapitalMoney = SumCapitalMoney,
                    SumGrossProfit = SumGrossProfit,
                    SumGrossCapitalMoney = SumGrossCapitalMoney,
                    SumGrossProfitMoney = SumGrossProfitMoney,
                    SumProfitMoney = SumProfitMoney
                };
            }
            catch (Exception e)
            {
                return new ProfitAccordingCustomersResult
                {
                    Message = e.Message,
                    Status = false
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

        public GetCustomerOrderByIDResult GetCustomerOrderByID(GetCustomerOrderByIDParameter parameter)
        {
            try
            {
                #region By Hung Edit code

                var customerOrderObject = (from or in context.CustomerOrder
                                           where or.OrderId == parameter.CustomerOrderId
                                           select new CustomerOrderEntityModel
                                           {
                                               OrderId = or.OrderId,
                                               BankAccountId = or.BankAccountId,
                                               OrderCode = or.OrderCode,
                                               OrderDate = or.OrderDate,
                                               Description = or.Description,
                                               Note = or.Note,
                                               CustomerId = or.CustomerId,
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
                                               ReceiptInvoiceAmount = or.ReceiptInvoiceAmount,
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
                                               ReasonCancel = or.ReasonCancel
                                           }).FirstOrDefault();

                var contact = context.Contact.Where(e => e.ObjectId == customerOrderObject.Seller).FirstOrDefault();
                if (contact != null)
                {
                    customerOrderObject.SellerAvatarUrl = contact.AvatarUrl;
                    customerOrderObject.SellerFirstName = contact.FirstName;
                    customerOrderObject.SellerLastName = contact.LastName;
                }

                #endregion

                var statusList = context.OrderStatus.ToList();
                customerOrderObject.StatusCode =
                    statusList.FirstOrDefault(x => x.OrderStatusId == customerOrderObject.StatusId) != null
                        ? statusList.FirstOrDefault(x => x.OrderStatusId == customerOrderObject.StatusId)
                            .OrderStatusCode
                        : "";

                if (customerOrderObject.CustomerId == getIDKHL001())
                {
                    customerOrderObject.TypeAccount = 1;
                }
                else
                {
                    customerOrderObject.TypeAccount = 2;
                }

                #region By Hung Edit Code

                var listCustomerOrderObjectType0 = (from cod in context.CustomerOrderDetail
                                                    where cod.OrderId == parameter.CustomerOrderId && cod.OrderDetailType == 0
                                                    select (new CustomerOrderDetailEntityModel
                                                    {
                                                        Active = cod.Active,
                                                        CreatedById = cod.CreatedById,
                                                        OrderId = cod.OrderId,
                                                        VendorId = cod.VendorId,
                                                        CreatedDate = cod.CreatedDate,
                                                        CurrencyUnit = cod.CurrencyUnit,
                                                        Description = cod.Description,
                                                        DiscountType = cod.DiscountType,
                                                        DiscountValue = cod.DiscountValue,
                                                        ExchangeRate = cod.ExchangeRate,
                                                        OrderDetailId = cod.OrderDetailId,
                                                        OrderDetailType = cod.OrderDetailType,
                                                        ProductId = cod.ProductId.Value,
                                                        UpdatedById = cod.UpdatedById,
                                                        Quantity = cod.Quantity,
                                                        UnitId = cod.UnitId,
                                                        IncurredUnit = cod.IncurredUnit,
                                                        UnitPrice = cod.UnitPrice,
                                                        UpdatedDate = cod.UpdatedDate,
                                                        GuaranteeTime = cod.GuaranteeTime,
                                                        GuaranteeDatetime = cod.GuaranteeDatetime != null
                                                            ? cod.GuaranteeDatetime
                                                            : customerOrderObject.ReceivedDate.Value.AddMonths(cod.GuaranteeTime.Value),
                                                        ExpirationDate = cod.ExpirationDate,
                                                        Vat = cod.Vat,
                                                        NameVendor = "",
                                                        NameProduct = "",
                                                        NameProductUnit = "",
                                                        NameMoneyUnit = "",
                                                        SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.Vat, cod.DiscountValue, cod.DiscountType,
                                                            cod.ExchangeRate),
                                                        OrderNumber = cod.OrderNumber
                                                    })).ToList();

                if (listCustomerOrderObjectType0.Count > 0)
                {
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listCurrencyUnitId = new List<Guid>();
                    List<Guid> listUnitId = new List<Guid>();
                    listCustomerOrderObjectType0.ForEach(item =>
                    {
                        if (item.ProductId != null)
                            listProductId.Add(item.ProductId.Value);
                        if (item.VendorId != null)
                            listVendorId.Add(item.VendorId.Value);
                        if (item.CurrencyUnit != null)
                            listCurrencyUnitId.Add(item.CurrencyUnit.Value);
                        if (item.UnitId != null)
                            listUnitId.Add(item.UnitId.Value);
                    });

                    var listProduct = context.Product.Where(e => listProductId.Contains(e.ProductId)).ToList();
                    var listVendor = context.Vendor.Where(e => listVendorId.Contains(e.VendorId)).ToList();
                    var listCurrencyUnit =
                        context.Category.Where(e => listCurrencyUnitId.Contains(e.CategoryId)).ToList();
                    var listUnit = context.Category.Where(e => listUnitId.Contains(e.CategoryId)).ToList();
                    listCustomerOrderObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null)
                            item.NameVendor = listVendor.FirstOrDefault(e => e.VendorId == item.VendorId).VendorName;
                        if (item.ProductId != null)
                        {
                            item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductName;
                            item.ProductCode = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductCode;
                        }

                        if (item.CurrencyUnit != null)
                            item.NameProductUnit =
                                listUnit.FirstOrDefault(e => e.CategoryId == item.UnitId).CategoryName;
                        if (item.UnitId != null)
                            item.NameMoneyUnit = listCurrencyUnit.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });
                }

                #endregion

                #region By Hung Edit Code

                var listCustomerOrderObjectType1 = (from cod in context.CustomerOrderDetail
                                                    where cod.OrderId == parameter.CustomerOrderId && cod.OrderDetailType == 1
                                                    select (new CustomerOrderDetailEntityModel
                                                    {
                                                        Active = cod.Active,
                                                        CreatedById = cod.CreatedById,
                                                        OrderId = cod.OrderId,
                                                        VendorId = cod.VendorId,
                                                        CreatedDate = cod.CreatedDate,
                                                        CurrencyUnit = cod.CurrencyUnit,
                                                        Description = cod.Description,
                                                        DiscountType = cod.DiscountType,
                                                        DiscountValue = cod.DiscountValue,
                                                        ExchangeRate = cod.ExchangeRate,
                                                        OrderDetailId = cod.OrderDetailId,
                                                        OrderDetailType = cod.OrderDetailType,
                                                        ProductId = cod.ProductId.Value,
                                                        UpdatedById = cod.UpdatedById,
                                                        Quantity = cod.Quantity,
                                                        UnitId = cod.UnitId,
                                                        IncurredUnit = cod.IncurredUnit,
                                                        UnitPrice = cod.UnitPrice,
                                                        UpdatedDate = cod.UpdatedDate,
                                                        GuaranteeTime = cod.GuaranteeTime,
                                                        GuaranteeDatetime = cod.GuaranteeDatetime != null
                                                            ? cod.GuaranteeDatetime
                                                            : customerOrderObject.ReceivedDate.Value.AddMonths(cod.GuaranteeTime.Value),
                                                        ExpirationDate = cod.ExpirationDate,
                                                        Vat = cod.Vat,
                                                        NameVendor = "",
                                                        NameProduct = "",
                                                        NameProductUnit = "",
                                                        NameMoneyUnit = "",
                                                        SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.Vat, cod.DiscountValue, cod.DiscountType,
                                                            cod.ExchangeRate),
                                                        OrderNumber = cod.OrderNumber
                                                    })).ToList();

                if (listCustomerOrderObjectType1.Count > 0)
                {
                    List<Guid> listCurrencyUnitId = new List<Guid>();

                    listCustomerOrderObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null)
                            listCurrencyUnitId.Add(item.CurrencyUnit.Value);
                    });

                    var listCurrencyUnit = context.Category.Where(e => listCurrencyUnitId.Contains(e.CategoryId)).ToList();
                    listCustomerOrderObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null)
                            item.NameMoneyUnit = listCurrencyUnit.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });
                }

                #endregion

                listCustomerOrderObjectType0.ForEach(item =>
                {
                    item.NameGene = item.NameProduct + "(" + getNameGEn(item.OrderDetailId) + ")";
                    item.OrderProductDetailProductAttributeValue =
                        getListOrderProductDetailProductAttributeValue(item.OrderDetailId);
                });

                listCustomerOrderObjectType0.AddRange(listCustomerOrderObjectType1);

                listCustomerOrderObjectType0 = listCustomerOrderObjectType0.OrderBy(z => z.OrderNumber).ToList();

                #region Lấy list ghi chú

                var listNote = new List<NoteEntityModel>();

                listNote = context.Note.Where(x => x.ObjectId == parameter.CustomerOrderId && x.ObjectType == "ORDER" && x.Active == true).Select(y => new NoteEntityModel
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
                    var listEmployee = context.Employee.ToList();
                    var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                        y => new NoteDocumentEntityModel
                        {
                            DocumentName = y.DocumentName,
                            DocumentSize = y.DocumentSize,
                            DocumentUrl = y.DocumentUrl,
                            CreatedById = y.CreatedById,
                            NoteDocumentId = y.NoteDocumentId,
                            NoteId = y.NoteId
                        }).ToList();
                    listNote.ForEach(item =>
                    {
                        var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                        item.ResponsibleName = employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                    });

                    //Sắp xếp lại listNote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                return new GetCustomerOrderByIDResult
                {
                    CustomerOrderObject = customerOrderObject,
                    ListCustomerOrderDetail = listCustomerOrderObjectType0,
                    ListNote = listNote,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetCustomerOrderByIDResult
                {
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        private Guid getIDKHL001()
        {
            Guid result = Guid.Empty;
            var AccountKHL = context.Customer.Where(item => item.CustomerCode == "KHL001").SingleOrDefault();
            if (AccountKHL != null)
                result = AccountKHL.CustomerId;
            return result;
        }

        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? Vat, decimal? DiscountValue, bool? DiscountType, decimal? ExChangeRate)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;
            decimal SumAmount = Quantity.Value * UnitPrice.Value * ExChangeRate.Value;

            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = ((SumAmount * DiscountValue.Value) / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }
            result = SumAmount - CacuDiscount;
            if (Vat != null)
            {
                CaculateVAT = (result * Vat.Value) / 100;
            }
            result = result + CaculateVAT;
            return result;
        }

        public string getNameGEn(Guid CustomerDetailID)
        {
            string Result = string.Empty;
            List<OrderProductDetailProductAttributeValueEntityModel> listResult = new List<OrderProductDetailProductAttributeValueEntityModel>();
            var OrderProductDetailProductAttributeValueModelList = (from OPDPV in context.OrderProductDetailProductAttributeValue
                                                                    join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                    where OPDPV.OrderDetailId == CustomerDetailID
                                                                    select (ProductAttributeCategoryV)).ToList();

            OrderProductDetailProductAttributeValueModelList.ForEach(item => { Result = Result + item.ProductAttributeCategoryValue1 + ";"; });

            return Result;

        }

        public List<OrderProductDetailProductAttributeValueEntityModel> getListOrderProductDetailProductAttributeValue(Guid CustomerDetailID)
        {
            List<OrderProductDetailProductAttributeValueEntityModel> listResult = new List<OrderProductDetailProductAttributeValueEntityModel>();
            var OrderProductDetailProductAttributeValueModelList = (from OPDPV in context.OrderProductDetailProductAttributeValue
                                                                    join ProductAttributeC in context.ProductAttributeCategory on OPDPV.ProductAttributeCategoryId equals ProductAttributeC.ProductAttributeCategoryId
                                                                    join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                    where OPDPV.OrderDetailId == CustomerDetailID
                                                                    select (new OrderProductDetailProductAttributeValueEntityModel
                                                                    {
                                                                        OrderDetailId = OPDPV.OrderDetailId,
                                                                        OrderProductDetailProductAttributeValueId = OPDPV.OrderProductDetailProductAttributeValueId,
                                                                        ProductAttributeCategoryId = OPDPV.ProductAttributeCategoryId,
                                                                        ProductId = OPDPV.ProductId,
                                                                        ProductAttributeCategoryValueId = OPDPV.ProductAttributeCategoryValueId,
                                                                        NameProductAttributeCategory = ProductAttributeC.ProductAttributeCategoryName,
                                                                        NameProductAttributeCategoryValue = ProductAttributeCategoryV.ProductAttributeCategoryValue1
                                                                    })).ToList();
            listResult = OrderProductDetailProductAttributeValueModelList;
            return listResult;
        }

        private decimal GetDiscountValue(decimal quantity, decimal unitPrice, decimal exchangeRate, bool discountType, decimal discountValue)
        {
            if (discountType == true)
            {
                //Nếu chiết khấu là %
                var amount = quantity * unitPrice * exchangeRate;
                var newDiscountValue = Math.Round(((amount * discountValue) / 100), 0);
                return newDiscountValue; //.ToString("#,#.");
            }
            else
            {
                //Nếu chiết khấu là tiền mặt
                return discountValue; //.ToString("#,#.");
            }
        }

        public ExportCustomerOrderPDFResult ExportPdfCustomerOrder(ExportCustomerOrderPDFParameter parameter)
        {
            try
            {
                GetCustomerOrderByIDResult resultCustomerOrderId = new GetCustomerOrderByIDResult();
                GetCustomerOrderByIDParameter customerOrderByIDParameter = new GetCustomerOrderByIDParameter();
                customerOrderByIDParameter.CustomerOrderId = parameter.CustomerOrderId;
                customerOrderByIDParameter.UserId = parameter.UserId;
                resultCustomerOrderId = this.GetCustomerOrderByID(customerOrderByIDParameter);
                var customerOrder = resultCustomerOrderId.CustomerOrderObject;
                var companyConfig = context.CompanyConfiguration.FirstOrDefault();
                var customer = context.Customer.Where(item => item.CustomerId == customerOrder.CustomerId).FirstOrDefault();
                Guid contactID = context.Contact
                    .Where(c => c.ObjectId == customer.CustomerId && c.ObjectType != ObjectType.CUSTOMERCONTACT)
                    .FirstOrDefault().ContactId;
                var contact = context.Contact.FirstOrDefault(c => c.ContactId == contactID);
                var PaymentMethodObj =
                    context.Category.FirstOrDefault(item => item.CategoryId == customerOrder.PaymentMethod);

                if (customerOrder != null)
                {
                    #region add by Giang

                    PDFOrderModel PDFOrder = new PDFOrderModel();
                    PDFOrder.OrderDate = "Ngày " + customerOrder.OrderDate.Day + " tháng " +
                                         customerOrder.OrderDate.Month + " năm " + customerOrder.OrderDate.Year;
                    PDFOrder.CompanyName = companyConfig.CompanyName == null ? "" : companyConfig.CompanyName;
                    PDFOrder.Website = "";
                    PDFOrder.TaxCode = companyConfig.TaxCode == null ? "" : companyConfig.TaxCode;
                    PDFOrder.CompanyPhone = companyConfig.Phone == null ? "" : companyConfig.Phone;
                    PDFOrder.CompanyEmail = companyConfig.Email == null ? "" : companyConfig.Email;
                    PDFOrder.CompanyAddress = companyConfig.CompanyAddress == null ? "" : companyConfig.CompanyAddress;
                    PDFOrder.OrderCode = customerOrder.OrderCode;
                    PDFOrder.Seller = (customerOrder.SellerFirstName == null ? "" : customerOrder.SellerFirstName) + ' ' +
                                      (customerOrder.SellerLastName == null ? "" : customerOrder.SellerLastName);
                    PDFOrder.LocationOfShipment =
                        customerOrder.LocationOfShipment == null ? "" : customerOrder.LocationOfShipment;
                    PDFOrder.Description = customerOrder.Description == null ? "" : customerOrder.Description;
                    PDFOrder.CustomerName = customer.CustomerName == null ? "" : customer.CustomerName;
                    PDFOrder.CustomerCode = customer.CustomerCode == null ? "" : customer.CustomerCode;
                    PDFOrder.CustomerPhone = contact.Phone == null ? "" : contact.Phone;
                    PDFOrder.CustomerAddress = contact.Address == null ? "" : contact.Address;
                    PDFOrder.CustomerTaxCode = contact.TaxCode == null ? "" : contact.TaxCode;
                    if (PaymentMethodObj != null)
                    {
                        PDFOrder.CustomerPaymentMethod =
                            PaymentMethodObj.CategoryName == null ? "" : PaymentMethodObj.CategoryName;
                    }
                    else
                    {
                        PDFOrder.CustomerPaymentMethod = "";
                    }
                    PDFOrder.RecipientName = customerOrder.RecipientName == null ? "" : customerOrder.RecipientName;
                    PDFOrder.PlaceOfDelivery = customerOrder.PlaceOfDelivery == null ? "" : customerOrder.PlaceOfDelivery;
                    string displayReceivedHour = string.Empty;
                    if (customerOrder.ReceivedHour.HasValue == false)
                    {
                        displayReceivedHour = "";
                    }
                    else
                    {
                        DateTime timeReceivedHour = DateTime.Today.Add(customerOrder.ReceivedHour.Value);
                        displayReceivedHour = timeReceivedHour.ToString("hh:mm tt");
                    }

                    PDFOrder.ReceivedDate =
                        (customerOrder.ReceivedDate.HasValue == false
                            ? ""
                            : customerOrder.ReceivedDate.Value.ToString("dd/M/yyyy")) + " - " + displayReceivedHour;
                    PDFOrder.ShippingNote = customerOrder.ShippingNote == null ? "" : customerOrder.ShippingNote;

                    var lstCustomerOrderDetail = resultCustomerOrderId.ListCustomerOrderDetail;
                    List<PDFOrderAttributeModel> lstPDFOrderAttribute = new List<PDFOrderAttributeModel>();
                    List<PDFOrderAttributeModel> lstPDFOrderAttributeOther = new List<PDFOrderAttributeModel>();
                    decimal totalAmountBeforVat = 0;
                    decimal totalAmountVat = 0;
                    decimal totalDiscountValue = 0;
                    int stt = 1;
                    int sttOther = 1;
                    for (int i = 0; i < lstCustomerOrderDetail.Count; ++i)
                    {
                        PDFOrderAttributeModel PDFOrderAttribute = new PDFOrderAttributeModel();
                        if (lstCustomerOrderDetail[i].OrderDetailType == 0)
                        {
                            var quantity = lstCustomerOrderDetail[i].Quantity == null ? 0 : lstCustomerOrderDetail[i].Quantity;
                            var unitPrice = lstCustomerOrderDetail[i].UnitPrice == null ? 0 : lstCustomerOrderDetail[i].UnitPrice;
                            var exchangeRate = lstCustomerOrderDetail[i].ExchangeRate == null ? 0 : lstCustomerOrderDetail[i].ExchangeRate;

                            var discountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            );

                            var amount = (quantity * unitPrice * exchangeRate - discountValue);
                            var vatAmount = amount * lstCustomerOrderDetail[i].Vat / 100;
                            totalAmountBeforVat = totalAmountBeforVat + amount.Value;
                            totalDiscountValue = totalDiscountValue + discountValue;
                            totalAmountVat = totalAmountVat + vatAmount.Value;

                            //Không có chi phí phát sinh
                            PDFOrderAttribute.Stt = (stt++).ToString("#,#.");
                            PDFOrderAttribute.ProductName = lstCustomerOrderDetail[i].NameProduct == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProduct;
                            PDFOrderAttribute.ProductCode = lstCustomerOrderDetail[i].ProductCode == null
                                ? ""
                                : lstCustomerOrderDetail[i].ProductCode;
                            PDFOrderAttribute.UnitName = lstCustomerOrderDetail[i].NameProductUnit == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProductUnit;
                            PDFOrderAttribute.Quantity = lstCustomerOrderDetail[i].Quantity == null
                                ? ""
                                : lstCustomerOrderDetail[i].Quantity.Value.ToString("#,#.");
                            PDFOrderAttribute.UnitPrice = lstCustomerOrderDetail[i].UnitPrice == null
                                ? ""
                                : lstCustomerOrderDetail[i].UnitPrice.Value.ToString("#,#.");
                            PDFOrderAttribute.ExchangeRate = lstCustomerOrderDetail[i].ExchangeRate.HasValue == false
                                ? ""
                                : (lstCustomerOrderDetail[i].ExchangeRate == 1
                                    ? ""
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value.ToString("#,#."));
                            PDFOrderAttribute.VAT = lstCustomerOrderDetail[i].Vat.HasValue == false
                                ? "0%"
                                : (lstCustomerOrderDetail[i].Vat == null || lstCustomerOrderDetail[i].Vat == 0
                                    ? "0%"
                                    : lstCustomerOrderDetail[i].Vat.Value.ToString("#,#.") + "%");
                            PDFOrderAttribute.DiscountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            ).ToString("#,#.");
                            //PDFOrderAttribute.Amount = lstCustomerOrderDetail[i].SumAmount.ToString("#,#.");
                            PDFOrderAttribute.Amount = amount.Value.ToString("#,#.");
                            lstPDFOrderAttribute.Add(PDFOrderAttribute);
                        }
                        else
                        {
                            var quantity = lstCustomerOrderDetail[i].Quantity == null ? 0 : lstCustomerOrderDetail[i].Quantity;
                            var unitPrice = lstCustomerOrderDetail[i].UnitPrice == null ? 0 : lstCustomerOrderDetail[i].UnitPrice;
                            var exchangeRate = lstCustomerOrderDetail[i].ExchangeRate == null ? 0 : lstCustomerOrderDetail[i].ExchangeRate;

                            var discountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            );

                            var amount = (quantity * unitPrice * exchangeRate - discountValue);
                            var vatAmount = amount * lstCustomerOrderDetail[i].Vat / 100;
                            totalAmountBeforVat = totalAmountBeforVat + amount.Value;
                            totalDiscountValue = totalDiscountValue + discountValue;
                            totalAmountVat = totalAmountVat + vatAmount.Value;

                            //Có chi phí phát sinh
                            PDFOrderAttribute.Stt = (sttOther++).ToString("#,#.");
                            PDFOrderAttribute.ProductName = lstCustomerOrderDetail[i].NameProduct == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProduct;
                            PDFOrderAttribute.ProductCode = lstCustomerOrderDetail[i].ProductCode == null
                                ? ""
                                : lstCustomerOrderDetail[i].ProductCode;
                            PDFOrderAttribute.UnitName = lstCustomerOrderDetail[i].NameProductUnit == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProductUnit;
                            PDFOrderAttribute.Quantity = lstCustomerOrderDetail[i].Quantity == null
                                ? ""
                                : lstCustomerOrderDetail[i].Quantity.Value.ToString("#,#.");
                            PDFOrderAttribute.UnitPrice = lstCustomerOrderDetail[i].UnitPrice == null
                                ? ""
                                : lstCustomerOrderDetail[i].UnitPrice.Value.ToString("#,#.");
                            PDFOrderAttribute.ExchangeRate = lstCustomerOrderDetail[i].ExchangeRate.HasValue == false
                                ? ""
                                : (lstCustomerOrderDetail[i].ExchangeRate == 1
                                    ? ""
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value.ToString("#,#."));
                            PDFOrderAttribute.VAT = lstCustomerOrderDetail[i].Vat.HasValue == false
                                ? "0%"
                                : (lstCustomerOrderDetail[i].Vat == null || lstCustomerOrderDetail[i].Vat == 0
                                    ? "0%"
                                    : lstCustomerOrderDetail[i].Vat.Value.ToString("#,#.") + "%");
                            PDFOrderAttribute.DiscountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            ).ToString("#,#.");
                            //PDFOrderAttribute.Amount = lstCustomerOrderDetail[i].SumAmount.ToString("#,#.");
                            PDFOrderAttribute.Amount = amount.Value.ToString("#,#.");
                            lstPDFOrderAttributeOther.Add(PDFOrderAttribute);
                        }
                    }

                    PDFOrder.ListPDFOrderAttribute = lstPDFOrderAttribute;
                    PDFOrder.ListPDFOrderAttributeOther = lstPDFOrderAttributeOther;
                    PDFOrder.TotalVat = totalAmountVat.ToString("#,#.");
                    PDFOrder.TotalBeforVat = totalAmountBeforVat.ToString("#,#.");
                    PDFOrder.TotalDiscountValue = totalDiscountValue.ToString("#,#.");

                    PDFOrder.TotalAmount = customerOrder.Amount.ToString("#,#.");
                    if (customerOrder.DiscountType == true)
                    {
                        //Nếu chiết khấu là %
                        var DiscountValueOrder =
                            Math.Round(
                                (customerOrder.Amount * (customerOrder.DiscountValue == null
                                     ? 0
                                     : customerOrder.DiscountValue.Value)) / 100, 0);
                        PDFOrder.DiscountValue = DiscountValueOrder.ToString("#,#.");
                        var TotalAmountAfter = (customerOrder.Amount - DiscountValueOrder);
                        PDFOrder.TotalAmountAfter = TotalAmountAfter.ToString("#,#.");
                        PDFOrder.TotalAmountAfterText = MoneyHelper.Convert(TotalAmountAfter);
                    }
                    else
                    {
                        //Nếu chiết khấu là tiền mặt
                        PDFOrder.DiscountValue = customerOrder.DiscountValue == null
                            ? "0"
                            : customerOrder.DiscountValue.Value.ToString("#,#.");
                        var TotalAmountAfter = (customerOrder.Amount -
                                                (customerOrder.DiscountValue == null
                                                    ? 0
                                                    : customerOrder.DiscountValue.Value));
                        PDFOrder.TotalAmountAfter = TotalAmountAfter.ToString("#,#.");
                        PDFOrder.TotalAmountAfterText = MoneyHelper.Convert(TotalAmountAfter);
                    }

                    #endregion

                    return new ExportCustomerOrderPDFResult
                    {
                        Status = true,
                        Message = "Success",
                        Code = customerOrder.OrderCode,
                        PDFOrder = PDFOrder
                    };
                }

                return new ExportCustomerOrderPDFResult()
                {
                    Status = false,
                    Message = "Không tồn tại đơn hàng"
                };
            }
            catch (Exception e)
            {
                return new ExportCustomerOrderPDFResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public UpdateCustomerOrderResult UpdateCustomerOrder(UpdateCustomerOrderParameter parameter)
        {
            try
            {
                bool isValidParameterNumber = true;

                if (parameter.CustomerOrder?.DaysAreOwed < 0 || parameter.CustomerOrder?.MaxDebt < 0 ||
                    parameter.CustomerOrder?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                }

                parameter.CustomerOrderDetail.ForEach(item =>
                {
                    if (item?.Quantity <= 0 || item?.ExchangeRate <= 0 || item?.UnitPrice < 0 ||
                        item?.GuaranteeTime < 0 || item?.Vat < 0 || item?.DiscountValue < 0)
                    {
                        isValidParameterNumber = false;
                    }
                });

                if (!isValidParameterNumber)
                {
                    return new UpdateCustomerOrderResult
                    {
                        Message = CommonMessage.Order.EDIT_ORDER_FAIL,
                        Status = false
                    };
                }

                var oldOrder =
                    context.CustomerOrder.FirstOrDefault(co => co.OrderId == parameter.CustomerOrder.OrderId);

                if (oldOrder == null)
                {
                    return new UpdateCustomerOrderResult()
                    {
                        Status = false,
                        Message = CommonMessage.Order.ORDER_NO_EXIST
                    };
                }

                #region Kiểm tra đơn hàng có thay đổi khách hàng hay không

                if (oldOrder.CustomerId != parameter.CustomerOrder.CustomerId)
                {
                    //Nếu đơn hàng thay đổi khách hàng thì kiểm tra xem đơn hàng đã phát sinh thanh toán chưa?
                    var order_reciept = context.ReceiptOrderHistory.FirstOrDefault(x => x.OrderId == oldOrder.OrderId);
                    if (order_reciept != null)
                    {
                        //Nếu đơn hàng đã phát sinh thanh toán thì không cho thay đổi khách hàng
                        return new UpdateCustomerOrderResult()
                        {
                            Status = false,
                            Message = CommonMessage.Order.ORDER_NO_PAY
                        };
                    }
                }

                #endregion

                #region Kiểm tra trạng thái đơn hàng có thỏa mãn điều kiện cập nhật đơn hàng hay không

                var oldStatus = context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == oldOrder.StatusId);
                var oldStatusCode = oldStatus?.OrderStatusCode;

                var newStatus =
                    context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == parameter.CustomerOrder.StatusId);
                var newStatusCode = newStatus?.OrderStatusCode;

                switch (oldStatusCode)
                {
                    case "DRA":
                        if (newStatusCode != "DRA" && newStatusCode != "IP")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                Status = false,
                                Message = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "IP":
                        if (newStatusCode != "IP" && newStatusCode != "PD" && newStatusCode != "DLV" &&
                            newStatusCode != "COMP" && newStatusCode != "ON" && newStatusCode != "RTN" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                Status = false,
                                Message = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "ON":
                        if (newStatusCode != "ON" && newStatusCode != "IP" && newStatusCode != "PD" &&
                            newStatusCode != "DLV" && newStatusCode != "COMP" && newStatusCode != "RTN" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                Status = false,
                                Message = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "DLV":
                        if (newStatusCode != "DLV" && newStatusCode != "RTN" && newStatusCode != "PD" &&
                            newStatusCode != "CAN" && newStatusCode != "COMP")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                Status = false,
                                Message = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "PD":
                        if (newStatusCode != "PD" && newStatusCode != "DLV" && newStatusCode != "COMP" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                Status = false,
                                Message = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "COMP":
                        return new UpdateCustomerOrderResult()
                        {
                            Status = false,
                            Message = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                    case "CAN":
                        return new UpdateCustomerOrderResult()
                        {
                            Status = false,
                            Message = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                    case "RTN":
                        return new UpdateCustomerOrderResult()
                        {
                            Status = false,
                            Message = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                }

                #endregion

                using (var transaction = context.Database.BeginTransaction())
                {
                    #region Delete all item Relation

                    var List_Delete_OrderProductDetailProductAttributeValue =
                        new List<OrderProductDetailProductAttributeValue>();
                    var List_Delete_CustomerOrderDetail = new List<CustomerOrderDetail>();
                    var List_Delete_OrderCostDetail = new List<OrderCostDetail>();

                    List_Delete_CustomerOrderDetail = context.CustomerOrderDetail
                        .Where(item => item.OrderId == parameter.CustomerOrder.OrderId).ToList();

                    List_Delete_CustomerOrderDetail.ForEach(item =>
                    {
                        if (item.OrderDetailId != Guid.Empty)
                        {
                            var OrderProductDetailProductAttributeValueList = context
                                .OrderProductDetailProductAttributeValue
                                .Where(OPDPAV => OPDPAV.OrderDetailId == item.OrderDetailId).ToList();
                            List_Delete_OrderProductDetailProductAttributeValue.AddRange(
                                OrderProductDetailProductAttributeValueList);
                        }
                    });

                    List_Delete_OrderCostDetail = context.OrderCostDetail
                        .Where(item => item.OrderId == parameter.CustomerOrder.OrderId).ToList();

                    context.OrderProductDetailProductAttributeValue.RemoveRange(
                        List_Delete_OrderProductDetailProductAttributeValue);
                    context.SaveChanges();
                    context.CustomerOrderDetail.RemoveRange(List_Delete_CustomerOrderDetail);
                    context.SaveChanges();
                    context.OrderCostDetail.RemoveRange(List_Delete_OrderCostDetail);
                    context.SaveChanges();

                    context.CustomerOrder.Remove(oldOrder);
                    context.SaveChanges();

                    #endregion

                    #region Create new Order base on Old Order

                    parameter.CustomerOrderDetail.ForEach(item =>
                    {
                        //Nếu là chi phí khác thì tên sản phẩm dịch vụ sẽ bằng trường Description
                        if (item.OrderDetailType == 1)
                        {
                            item.ProductName = item.Description;
                        }

                        item.OrderDetailId = Guid.NewGuid();
                        if (item.OrderProductDetailProductAttributeValue != null)
                        {
                            foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                            {
                                itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                            }
                        }
                    });

                    parameter.OrderCostDetail.ForEach(item =>
                    {
                        item.OrderCostDetailId = Guid.NewGuid();
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;
                    });

                    #region Kiểm tra có phải là khách lẻ không?

                    if (parameter.TypeAccount == 1)
                    {
                        var customer = context.Customer.FirstOrDefault(x => x.CustomerCode == "KHL001");

                        parameter.CustomerOrder.CustomerId = customer.CustomerId;
                        parameter.CustomerOrder.PaymentMethod = null;
                        parameter.CustomerOrder.DaysAreOwed = null;
                        parameter.CustomerOrder.MaxDebt = null;
                        parameter.CustomerOrder.BankAccountId = null;
                    }

                    #endregion

                    parameter.CustomerOrder.CreatedById = oldOrder.CreatedById;
                    parameter.CustomerOrder.CreatedDate = oldOrder.CreatedDate;
                    parameter.CustomerOrder.UpdatedById = parameter.UserId;
                    parameter.CustomerOrder.UpdatedDate = DateTime.Now;
                    parameter.CustomerOrder.CustomerOrderDetail = parameter.CustomerOrderDetail;
                    parameter.CustomerOrder.OrderCostDetail = parameter.OrderCostDetail;

                    context.CustomerOrder.Add(parameter.CustomerOrder);
                    context.SaveChanges();

                    transaction.Commit();

                    #region Gửi thông báo

                    NotificationHelper.AccessNotification(context, TypeModel.CustomerOrderDetail, "UPD", new CustomerOrder(),
                        parameter.CustomerOrder, true);

                    #endregion

                    #endregion

                    #region Kiểm tra xem đơn hàng đã được sử dụng để tạo Đơn đặt hàng Nhà cung cấp hay chưa?

                    var vendorOrder = context.VendorOrder.Where(item => item.CustomerOrderId == parameter.CustomerOrder.OrderId).FirstOrDefault();

                    #endregion

                    return new UpdateCustomerOrderResult
                    {
                        Message = CommonMessage.Order.EDIT_ORDER_SUCCESS,
                        CustomerOrderID = parameter.CustomerOrder.OrderId,
                        Status = true,
                        VendorOrderID = vendorOrder?.VendorOrderId
                    };
                }
            }
            catch (Exception ex)
            {
                return new UpdateCustomerOrderResult
                {
                    Message = CommonMessage.Order.EDIT_ORDER_FAIL,
                    Status = false,
                };
            }
        }
        public UpdateCustomerOrderResult UpdateStatusOrder(UpdateStatusOrderParameter parameter)
        {
            try
            {
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == employeeId);
                var employeeContact = context.Contact.FirstOrDefault(u => u.ObjectId == employeeId && u.ObjectType == "EMP");
                var statusOrder = context.OrderStatus.Where(s => s.Active == true).ToList();
                var customerOrderObj = context.CustomerOrder.FirstOrDefault(c => c.OrderId == parameter.CustomerOrderId);
                var objDetail = context.CustomerOrderDetail.Where(d => d.OrderId == customerOrderObj.OrderId).ToList();
                var warehouseObj = context.Warehouse.FirstOrDefault(w => w.Active == true && w.WarehouseParent == null);
                var message = "";

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "ORDER";
                note.ObjectId = parameter.CustomerOrderId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = CommonMessage.Note.NOTE_TITLE;

                Guid? ProcurementRequestId = null;
                switch (parameter.ObjectType)
                {
                    case "SEND_APPROVAL":
                        var statusIP = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "IP");
                        customerOrderObj.StatusId = statusIP.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;
                        if (customerOrderObj.OrderContractId != null && customerOrderObj.OrderContractId != Guid.Empty)
                        {
                            var THDStatusType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THD" && ct.Active == true);
                            var DTHStatus = context.Category.FirstOrDefault(ct => ct.CategoryTypeId == THDStatusType.CategoryTypeId && ct.Active == true && ct.CategoryCode == "DTH");
                            var contractObj = context.Contract.FirstOrDefault(ctr => ctr.ContractId == customerOrderObj.OrderContractId);
                            contractObj.StatusId = DTHStatus.CategoryId;
                            context.Contract.Update(contractObj);
                            context.SaveChanges();
                        }
                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = CommonMessage.Note.NOTE_CONTENT_SEND_APPROVAL;
                        break;
                    case "APPROVAL":
                        var statusDLV = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DLV");
                        customerOrderObj.StatusId = statusDLV.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        #region Tự động sinh phiếu xuất kho: Giang comment

                        //// Tự động sinh Phiếu xuất kho
                        //var datenow = DateTime.Now;
                        //TimeSpan today = new TimeSpan(datenow.Hour, datenow.Minute, datenow.Second);
                        //var totalInvertoryCreate = context.InventoryDeliveryVoucher.Where(c => Convert.ToDateTime(c.CreatedDate).Day == datenow.Day && Convert.ToDateTime(c.CreatedDate).Month == datenow.Month && Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Count();
                        //var statusInventoryDeliveryVoucher = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TPHX");
                        //var statusInventoryDeliveryVoucherCXL = context.Category.FirstOrDefault(f => f.CategoryCode == "CXK" && f.CategoryTypeId == statusInventoryDeliveryVoucher.CategoryTypeId);

                        //var inventoryDeliveryVoucher = new InventoryDeliveryVoucher();
                        //inventoryDeliveryVoucher.InventoryDeliveryVoucherId = Guid.NewGuid();
                        //inventoryDeliveryVoucher.InventoryDeliveryVoucherCode = "PX-" + ConverCreateId(totalInvertoryCreate + 1); ;
                        //inventoryDeliveryVoucher.StatusId = statusInventoryDeliveryVoucherCXL.CategoryId;
                        //inventoryDeliveryVoucher.InventoryDeliveryVoucherType = 1;
                        //inventoryDeliveryVoucher.WarehouseId = (customerOrderObj.WarehouseId == null || customerOrderObj.WarehouseId == Guid.Empty) ? warehouseObj.WarehouseId : customerOrderObj.WarehouseId.Value;
                        //inventoryDeliveryVoucher.ObjectId = customerOrderObj.OrderId;
                        //inventoryDeliveryVoucher.Receiver = customerOrderObj.RecipientName;
                        ////inventoryDeliveryVoucher.Reason = Reason;
                        //inventoryDeliveryVoucher.InventoryDeliveryVoucherDate = datenow;
                        //inventoryDeliveryVoucher.InventoryDeliveryVoucherTime = today;
                        //inventoryDeliveryVoucher.LicenseNumber = 1;
                        //inventoryDeliveryVoucher.Active = true;
                        //inventoryDeliveryVoucher.CreatedDate = datenow;
                        //inventoryDeliveryVoucher.CreatedById = parameter.UserId;
                        //context.InventoryDeliveryVoucher.Add(inventoryDeliveryVoucher);

                        //objDetail.ForEach(item =>
                        //{
                        //    if (item.ProductId != null)
                        //    {
                        //        var inventoryDeliveryVoucherDetail = new InventoryDeliveryVoucherMapping();
                        //        inventoryDeliveryVoucherDetail.InventoryDeliveryVoucherMappingId = Guid.NewGuid();
                        //        inventoryDeliveryVoucherDetail.InventoryDeliveryVoucherId = inventoryDeliveryVoucher.InventoryDeliveryVoucherId;
                        //        inventoryDeliveryVoucherDetail.ProductId = item.ProductId.Value;
                        //        inventoryDeliveryVoucherDetail.QuantityRequest = item.Quantity.Value;
                        //        inventoryDeliveryVoucherDetail.QuantityInventory = 0;
                        //        inventoryDeliveryVoucherDetail.QuantityActual = item.Quantity.Value;
                        //        inventoryDeliveryVoucherDetail.PriceProduct = item.UnitPrice.Value;
                        //        inventoryDeliveryVoucherDetail.WarehouseId = (item.WarehouseId == null || item.WarehouseId == Guid.Empty) ? warehouseObj.WarehouseId : item.WarehouseId.Value;
                        //        inventoryDeliveryVoucherDetail.Description = item.Description;
                        //        inventoryDeliveryVoucherDetail.Active = true;
                        //        inventoryDeliveryVoucherDetail.CreatedDate = datenow;
                        //        inventoryDeliveryVoucherDetail.CreatedById = parameter.UserId;
                        //        inventoryDeliveryVoucherDetail.CurrencyUnit = item.CurrencyUnit;
                        //        inventoryDeliveryVoucherDetail.UnitId = item.UnitId;
                        //        inventoryDeliveryVoucherDetail.ExchangeRate = item.ExchangeRate;
                        //        inventoryDeliveryVoucherDetail.Vat = item.Vat;
                        //        inventoryDeliveryVoucherDetail.DiscountType = item.DiscountType;
                        //        inventoryDeliveryVoucherDetail.DiscountValue = item.DiscountValue;
                        //        context.InventoryDeliveryVoucherMapping.Add(inventoryDeliveryVoucherDetail);
                        //    }
                        //});

                        #endregion

                        context.SaveChanges();
                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = employee.EmployeeName + CommonMessage.Note.NOTE_CONTENT_APPROVAL;
                        break;
                    case "REJECT":
                        var statusRTN = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "RTN");
                        customerOrderObj.StatusId = statusRTN.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = employee.EmployeeName + CommonMessage.Note.NOTE_CONTENT_REJECT + parameter.Description;
                        break;
                    case "CANCEL":
                        var statusCAN = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "CAN");
                        customerOrderObj.StatusId = statusCAN.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = employee.EmployeeName + CommonMessage.Note.NOTE_CONTENT_CANCEL;// + parameter.Description;
                        break;
                    case "EDIT_NEW":
                        var statusNEW = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DRA");
                        customerOrderObj.StatusId = statusNEW.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = CommonMessage.Note.NOTE_CONTENT_EDIT_NEW;
                        break;
                    case "PAY_ORDER":
                        // Đề xuất mua hàng
                        var datetimenow = DateTime.Now;
                        TimeSpan timetoday = new TimeSpan(datetimenow.Hour, datetimenow.Minute, datetimenow.Second);
                        var numberOfPr = context.ProcurementRequest.Count();
                        var code = "RQP" + datetimenow.ToString("yy") + (numberOfPr + 1).ToString("D5");

                        var statusProcurementRequest = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DDU");
                        var statusProcurementRequestCXL = context.Category.FirstOrDefault(f => f.CategoryCode == "DR" && f.CategoryTypeId == statusProcurementRequest.CategoryTypeId);

                        var procurementRequest = new ProcurementRequest();
                        procurementRequest.ProcurementRequestId = Guid.NewGuid();
                        procurementRequest.OrderId = parameter.CustomerOrderId;
                        procurementRequest.ProcurementCode = code;
                        procurementRequest.ProcurementContent = "Đặt hàng cho đơn hàng bán " + customerOrderObj.OrderCode + ", ngày " + customerOrderObj.OrderDate.ToString("dd/MM/yyy");
                        procurementRequest.RequestEmployeeId = employeeId;
                        procurementRequest.EmployeePhone = employeeContact.Phone;
                        procurementRequest.Unit = employee.OrganizationId;
                        procurementRequest.ApproverPostion = employee.PositionId;
                        procurementRequest.StatusId = statusProcurementRequestCXL.CategoryId;
                        procurementRequest.CreatedById = parameter.UserId;
                        procurementRequest.CreatedDate = datetimenow;
                        procurementRequest.Active = true;

                        context.ProcurementRequest.Add(procurementRequest);

                        // bảng giá ncc
                        var productVendorMapping = context.ProductVendorMapping.Where(p => p.Active == true).ToList();
                        objDetail.ForEach(item =>
                        {
                            var procurementRequestDetail = new ProcurementRequestItem();
                            procurementRequestDetail.ProcurementRequestItemId = Guid.NewGuid();
                            procurementRequestDetail.ProductId = item.ProductId;
                            procurementRequestDetail.VendorId = item.VendorId;
                            procurementRequestDetail.Quantity = item.Quantity;
                            procurementRequestDetail.UnitPrice = item.UnitPrice;
                            procurementRequestDetail.ProcurementRequestId = procurementRequest.ProcurementRequestId;
                            //procurementRequestDetail.ProcurementPlanId = item.ProcurementPlanId;
                            procurementRequestDetail.Vat = item.Vat;
                            procurementRequestDetail.DiscountType = item.DiscountType;
                            procurementRequestDetail.DiscountValue = item.DiscountValue;
                            procurementRequestDetail.IncurredUnit = item.IncurredUnit;
                            procurementRequestDetail.Description = item.Description;
                            procurementRequestDetail.CreatedById = parameter.UserId;
                            procurementRequestDetail.CreatedDate = datetimenow;
                            procurementRequestDetail.CurrencyUnit = item.CurrencyUnit;
                            procurementRequestDetail.ExchangeRate = item.ExchangeRate;
                            procurementRequestDetail.OrderDetailType = item.OrderDetailType;

                            context.ProcurementRequestItem.Add(procurementRequestDetail);
                        });
                        ProcurementRequestId = procurementRequest.ProcurementRequestId;
                        break;
                    //case "CLONE":
                    //    var statusNEWClone = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DRA");
                    //    int count = customerOrderObj.CloneCount == null ? 1 : customerOrderObj.CloneCount.Value + 1;
                    //    var cloneObj = customerOrderObj;
                    //    cloneObj.OrderId = Guid.NewGuid();
                    //    cloneObj.OrderCode = GenerateOrderCode(1);

                    //    customerOrderObj.CloneCount = count;

                    //    message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                    //    note.Description = CommonMessage.Note.NOTE_CONTENT_EDIT_NEW;
                    //    break;
                    case "CANCEL_APPROVAL":
                        var statusNew = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DRA");
                        if (customerOrderObj != null)
                        {
                            if (statusNew != null) customerOrderObj.StatusId = statusNew.OrderStatusId;
                            customerOrderObj.UpdatedById = parameter.UserId;
                            customerOrderObj.UpdatedDate = DateTime.Now;
                        }

                        message = CommonMessage.Order.CANCEL_APPROVAL_SUCCESS;
                        note.Description = "Đơn hàng đã được hủy yêu cầu phê duyệt bởi nhân viên: " + employee.EmployeeCode + " - " + employee.EmployeeName;
                        break;
                }

                if (parameter.ObjectType != "PAY_ORDER")
                {
                    context.CustomerOrder.Update(customerOrderObj);
                    context.Note.Add(note);
                }
                context.SaveChanges();

                #region gửi mail thông báo

                switch (parameter.ObjectType)
                {
                    case "CANCEL_APPROVAL":
                        //var configEntity = context.SystemParameter.ToList();

                        //var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE").CategoryTypeId;

                        //var listEmailTempType =
                        //    context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

                        //var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "OCA") // ORDER_CANCEL_APPROVAL - OCA
                        //    .CategoryId;

                        //var emailTemplate = context.EmailTemplate.FirstOrDefault(w => w.Active && w.EmailTemplateTypeId == emailCategoryId);

                        //#region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        //var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        //var listAllContact = context.Contact.Where(x =>
                        //    x.Active == true && (x.ObjectType == "CUS" ||
                        //                         x.ObjectType == "CUS_CON" ||
                        //                         x.ObjectType == "EMP")).ToList();

                        //#endregion

                        //#region Lấy danh sách email cần gửi thông báo

                        //var listEmailSendTo = new List<string>();

                        //#region Lấy email người phê duyệt

                        ////Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                        //var listManager = listAllEmployee.Where(x => x.IsManager)
                        //    .Select(y => y.EmployeeId).ToList();
                        //var listEmailManager = listAllContact
                        //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
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
                        //var empCreaterId =
                        //    context.User.FirstOrDefault(x => x.UserId == customerOrderObj.CreatedById)
                        //        ?.EmployeeId;

                        //var email_created = "";

                        //if (empCreaterId != null)
                        //{
                        //    email_created = listAllContact.FirstOrDefault(x =>
                        //        x.ObjectId == empCreaterId && x.ObjectType == "EMP")?.Email;

                        //    if (!String.IsNullOrEmpty(email_created))
                        //    {
                        //        listEmailSendTo.Add(email_created.Trim());
                        //    }
                        //}

                        //#endregion

                        //#region Lấy email người phụ trách (Nhân viên bán hàng)

                        //var email_seller = listAllContact.FirstOrDefault(x =>
                        //    x.ObjectId == customerOrderObj.Seller && x.ObjectType == "EMP")?.Email;

                        //if (!String.IsNullOrEmpty(email_seller))
                        //{
                        //    listEmailSendTo.Add(email_seller.Trim());
                        //}

                        //#endregion

                        //#region Lấy email người hủy phê duyệt

                        //var empId =
                        //    context.User.FirstOrDefault(x => x.UserId == customerOrderObj.UpdatedById)
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

                        //listEmailSendTo = listEmailSendTo.Distinct().ToList();

                        //#endregion

                        //var subject = ReplaceTokenForContent(context, customerOrderObj, emailTemplate.EmailTemplateTitle,
                        //    configEntity);
                        //var content = ReplaceTokenForContent(context, customerOrderObj, emailTemplate.EmailTemplateContent,
                        //    configEntity);

                        //Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);
                        NotificationHelper.AccessNotification(context, TypeModel.CustomerOrderDetail, "CANCEL_APPROVAL", new CustomerOrder(),
                            customerOrderObj, true);
                        break;
                }

                #endregion

                return new UpdateCustomerOrderResult
                {
                    ProcurementRequestId = ProcurementRequestId,
                    Message = message,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new UpdateCustomerOrderResult
                {
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        #region Phần hỗ trợ gửi mail
        
        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity)
        {
            var result = emailContent;

            #region Common Token

            const string Logo = "[LOGO]";
            const string OrderCode = "[ORDER_CODE]";
            const string EmployeeName = "[EMPLOYEE_NAME]";
            const string EmployeeCode = "[EMPLOYEE_CODE]";
            const string UpdatedDate = "[UPDATED_DATE]";
            const string Url_Login = "[URL]";

            #endregion

            var _model = model as CustomerOrder;

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

            #region replace order code

            if (result.Contains(OrderCode) && _model.OrderCode != null)
            {
                result = result.Replace(OrderCode, _model.OrderCode.Trim());
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

        #endregion

        public string ConverCreateId(int totalRecordCreate)
        {
            var datenow = DateTime.Now;
            string year = datenow.Year.ToString().Substring(datenow.Year.ToString().Length - 2, 2);
            string month = datenow.Month < 10 ? "0" + datenow.Month.ToString() : datenow.Month.ToString();
            string day = datenow.Day < 10 ? "0" + datenow.Day.ToString() : datenow.Day.ToString();
            string total = "";
            if (totalRecordCreate > 999)
            {
                total = totalRecordCreate.ToString();
            }
            else if (totalRecordCreate > 99 && totalRecordCreate < 1000)
            {
                total = "0" + totalRecordCreate.ToString();
            }
            else if (totalRecordCreate > 9 && totalRecordCreate < 100)
            {
                total = "00" + totalRecordCreate.ToString();
            }
            else
            {
                total = "000" + totalRecordCreate.ToString();
            }
            var result = year + month + day + total;
            var code = "PX-" + result;
            var InventoryDeliveryVoucherObj = context.InventoryDeliveryVoucher.FirstOrDefault(i => i.InventoryDeliveryVoucherCode == code);
            if (InventoryDeliveryVoucherObj != null)
            {
                ConverCreateId(totalRecordCreate + 1);
            }
            return result;
        }

        public CheckBeforCreateOrUpdateOrderResult CheckBeforCreateOrUpdateOrder(CheckBeforCreateOrUpdateOrderParameter parameter)
        {
            try
            {
                bool isCheck = false;
                if (parameter.MaxDebt != 0)
                {
                    var statusDBH =
                        context.OrderStatus.FirstOrDefault(o => o.OrderStatusCode == "DLV" && o.Active == true);

                    if (statusDBH != null)
                    {
                        //Lấy những Đơn hàng có trạng thái Đơn hàng bán của khách hàng
                        var orderByCustomer = context.CustomerOrder.Where(co =>
                            co.CustomerId == parameter.CustomerId && co.StatusId == statusDBH.OrderStatusId).ToList();
                        var listOrderId = orderByCustomer.Select(y => y.OrderId).ToList();

                        //Lấy tổng thanh toán một phần của các đơn hàng trên nếu có
                        var tongThanhToanMotPhan = context.ReceiptOrderHistory
                            .Where(x => listOrderId.Contains(x.OrderId)).Sum(y => y.AmountCollected);

                        var amountOrderCustomer = parameter.AmountOrder; //Tổng thanh toán của đơn hàng hiện tại
                        orderByCustomer.ForEach(item =>
                        {
                            var discountMoney = item.DiscountType == true
                                ? (item.Amount * item.DiscountValue / 100)
                                : item.DiscountValue;
                            amountOrderCustomer =
                                amountOrderCustomer + item.Amount - decimal.Parse(discountMoney.ToString());
                        });

                        if ((parameter.MaxDebt - amountOrderCustomer + tongThanhToanMotPhan) < 0)
                        {
                            isCheck = true;
                        }
                    }
                }
                return new CheckBeforCreateOrUpdateOrderResult
                {
                    isCheckMaxDebt = isCheck,
                    Message = CommonMessage.Order.EDIT_ORDER_SUCCESS,
                    Status = true,
                };
            }
            catch (Exception ex)
            {
                return new CheckBeforCreateOrUpdateOrderResult
                {
                    Message = CommonMessage.Order.EDIT_ORDER_FAIL,
                    Status = false,
                };
            }
        }

        private string GenerateOrderCode(int maxIndex)
        {
            var orderCode = "";
            var prefix = "SO";
            var now = DateTime.Now;
            var _day = now.Day.ToString("D2");
            var _month = now.Month.ToString("D2");
            var _year = (now.Year % 100).ToString();

            var new_final_order = context.CustomerOrder.OrderByDescending(z => z.CreatedDate)
                .FirstOrDefault(x => x.OrderCode.Contains("_" + _day + _month + _year + ""));

            if (new_final_order != null)
            {
                var currentOrderCode = new_final_order.OrderCode;
                //var currentMaxIndex = currentOrderCode.Substring(currentOrderCode.LastIndexOf("_") + 1);
                //maxIndex = Convert.ToInt32(currentMaxIndex);
                //maxIndex = maxIndex + 1;
                orderCode = prefix + "_" + _year + _month + _day + maxIndex.ToString("D4");
                var duplicateOrderFinal = context.CustomerOrder.FirstOrDefault(x => x.OrderCode == orderCode);
                if (duplicateOrderFinal != null)
                {
                    orderCode = GenerateOrderCode(maxIndex + 1);
                }
                return orderCode;
            }

            //maxIndex = maxIndex + 1;
            orderCode = prefix + "_" + _year + _month + _day + maxIndex.ToString("D4");
            var duplicateOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderCode == orderCode);
            if (duplicateOrder != null)
            {
                orderCode = GenerateOrderCode(maxIndex + 1);
            }
            return orderCode;
        }

        private string GenerateorderCode()
        {
            string currentYear = DateTime.Now.Year.ToString();
            int count = context.CustomerOrder.Count();
            string result = "DH-" + currentYear.Substring(currentYear.Length - 2) + returnNumberOrder(count.ToString());
            return result;
        }

        private string returnNumberOrder(string number)
        {
            switch (number.Length)
            {
                case 1: { return "000" + number; }
                case 2: { return "00" + number; }
                case 3: { return "0" + number; }
                default:
                    return number;
                    break;
            }
        }

        private string returnOrderDetail(string parameter0, string parameter1, string parameter2, string parameter3, string parameter4, string parameter5, string parameter6, string parameter7, string parameter8)
        {
            string Result = string.Format(@"<tr><td class='title'>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>",
                parameter0, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8);
            return Result;
        }

        public GetCustomerOrderBySellerResult GetCustomerOrderBySeller(GetCustomerOrderBySellerParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.ORDER, "GetAllOrderBySeller", parameter.UserId);

            var commonProductCategory = context.ProductCategory.ToList();
            var commonProducts = context.Product.ToList();
            var commonProductCategories = context.ProductCategory.ToList();
            var commonCustomers = context.Customer.ToList();
            var commonOrderStatus = context.OrderStatus.Where(x => x.OrderStatusCode == "IP" || x.OrderStatusCode == "DLV" || x.OrderStatusCode == "PD" || x.OrderStatusCode == "COMP").ToList();
            var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
            var custormerIdList = commonCustomers.Select(c => c.CustomerId).ToList();
            var commonCustomerOrder = context.CustomerOrder.Where(c => c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                                          && parameter.Seller == c.Seller
                                          && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                                          && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();

            var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
            var commonCustomerOrderDetails = context.CustomerOrderDetail.Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
            var productIdList = commonCustomerOrderDetails.Select(c => c.ProductId).ToList();
            var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

            #region Lấy list đơn hàng mới nhất

            #region LongNH comment code

            //var commonProductCategory = context.ProductCategory.ToList();
            //var commonCustomerOrderDetails = context.CustomerOrderDetail.ToList();
            //var commonProducts = context.Product.ToList();
            //var commonProductCategories = context.ProductCategory.ToList();
            //var commonCustomerOrder = context.CustomerOrder.ToList();
            //var commonCustomers = context.Customer.ToList();
            //var commonOrderStatus = context.OrderStatus.Where(x => x.OrderStatusCode == "IP" || x.OrderStatusCode == "DLV" || x.OrderStatusCode == "PD" || x.OrderStatusCode == "COMP").ToList();     
            //var orderListAll = (from or in commonCustomerOrder
            //                    join cus in commonCustomers on or.CustomerId equals cus.CustomerId
            //                    join orst in commonOrderStatus on or.StatusId equals orst.OrderStatusId
            //                    where parameter.Seller == or.Seller
            //                    && (orst.OrderStatusCode == "IP" || orst.OrderStatusCode == "DLV" || orst.OrderStatusCode == "PD" || orst.OrderStatusCode == "COMP")
            //                    && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= or.OrderDate.Date)
            //                    && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= or.OrderDate.Date)
            //                    select new CustomerOrderEntityModel
            //                    {
            //                        OrderId = or.OrderId,
            //                        OrderCode = or.OrderCode,
            //                        DiscountType = or.DiscountType,
            //                        DiscountValue = or.DiscountValue,
            //                        Amount = (decimal)((or.DiscountType == true) ? (or.Amount * (1 - (or.DiscountValue / 100))) : (or.Amount - or.DiscountValue)),
            //                        CreatedDate = or.CreatedDate,
            //                        CustomerName = cus.CustomerName
            //                    }
            //                 ).OrderByDescending(or => or.CreatedDate).ToList();

            #endregion

            var orderListAll = new List<CustomerOrderEntityModel>();
            commonCustomerOrder.ForEach(item =>
            {
                var customer = commonCustomers.FirstOrDefault(c => c.CustomerId == item.CustomerId);
                if (customer != null)
                {
                    var customerOrder = new CustomerOrderEntityModel();
                    customerOrder.OrderId = item.OrderId;
                    customerOrder.OrderCode = item.OrderCode;
                    customerOrder.DiscountType = item.DiscountType;
                    customerOrder.DiscountValue = item.DiscountValue;
                    customerOrder.Amount = (decimal)((item.DiscountType == true) ? (item.Amount * (1 - (item.DiscountValue / 100))) : (item.Amount - item.DiscountValue));
                    customerOrder.CreatedDate = item.CreatedDate;
                    customerOrder.CustomerName = customer.CustomerName;
                    orderListAll.Add(customerOrder);
                }
            });

            var orderList = orderListAll.OrderByDescending(c => c.CreatedDate).Take(5).ToList();

            #endregion

            #region Lấy tỉ lệ doanh số theo nhóm sản phẩm

            var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                  where productcategory.ProductCategoryLevel == 0
                                                  select new
                                                  {
                                                      productcategory.ProductCategoryId,
                                                      productcategory.ProductCategoryName
                                                  }).ToList();

            int? levelMaxProductCategory = commonProductCategory.Max(x => x.ProductCategoryLevel);

            List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
            listProductCaterogyFirstParent.ForEach(item =>
            {
                List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId, newListProductCategoryIdChildren, commonProductCategory);

                var listCategoryId = new ListCategoryId();
                listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                newListProductCategory.Add(listCategoryId);
            });

            #region Long comment

            //newListProductCategory.ForEach(item =>
            //{

            //    var newProductCategoryList = (from customerOderDetail in commonCustomerOrderDetails
            //                                  join product in commonProducts on customerOderDetail.ProductId equals product.ProductId
            //                                  join productCategory in commonProductCategories on product.ProductCategoryId equals productCategory.ProductCategoryId
            //                                  join customerOrder in commonCustomerOrder on customerOderDetail.OrderId equals customerOrder.OrderId
            //                                  join orderStatus in commonOrderStatus on customerOrder.StatusId equals orderStatus.OrderStatusId
            //                                  where parameter.Seller == customerOrder.Seller
            //                                  && (item.ListChildrent.Contains(productCategory.ProductCategoryId) || item.ListChildrent.Count == 0)
            //                                  && (orderStatus.OrderStatusCode == "IP" || orderStatus.OrderStatusCode == "DLV" || orderStatus.OrderStatusCode == "PD" || orderStatus.OrderStatusCode == "COMP")
            //                                  && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= customerOrder.OrderDate.Date)
            //                                  && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= customerOrder.OrderDate.Date)
            //                                  select new
            //                                  {
            //                                      productCategory.ProductCategoryId,
            //                                      productCategory.ProductCategoryName,
            //                                      Total = CalculatorTotal(customerOderDetail.Vat.Value, customerOderDetail.DiscountType.Value, customerOderDetail.DiscountValue.Value, customerOderDetail.Quantity.Value, customerOderDetail.UnitPrice.Value, customerOderDetail.ExchangeRate.Value)
            //                                  }
            //                           ).ToList();

            //    var new_ListProductCategory = newProductCategoryList.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
            //    {
            //        Id = y.Key,
            //        y.Key.ProductCategoryId,
            //        y.Key.ProductCategoryName,
            //        Total = y.Sum(s => s.Total)
            //    }).OrderByDescending(x => x.Total).ToList();

            //    var listtt = new ListCategoryResult();
            //    if (new_ListProductCategory.Count > 0)
            //    {
            //        decimal total_money = 0;
            //        listtt.ParentProductCategoryId = item.ParentProductCategoryId;
            //        listtt.ParentProductCategoryName = item.ParentProductCategoryName;

            //        new_ListProductCategory.ForEach(element =>
            //        {
            //            total_money += element.Total;
            //        });
            //        listtt.Total = total_money;
            //        newListProductCategoryResult.Add(listtt);
            //    }
            //});

            #endregion

            #region Lấy ra danh sách tất cả danh mục bán được sản phẩm

            List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
            var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
            var productCategoryList = commonProductCategory.Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();
            List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();
            commonCustomerOrderDetails.ForEach(item =>
            {
                #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng
                var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                decimal? discountPerOrder = 0;
                if (customerOrder.DiscountType == true)
                {
                    //chiết khấu phần trăm
                    discountPerOrder = customerOrder?.DiscountValue ?? 0;
                }
                else
                {
                    //chiết khấu theo số tiền
                    discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                }
                #endregion

                var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product != null)
                {
                    var category = commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                    var productCategoryModel = new ProductCategoryModel();
                    productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                    productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                    //productCategoryModel.Total = CalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value);
                    productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value, discountPerOrder);
                    listProductCategoryModel.Add(productCategoryModel);
                }
            });

            #endregion

            // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
            var new_ListProductCategory = listProductCategoryModel.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
            {
                Id = y.Key,
                y.Key.ProductCategoryId,
                y.Key.ProductCategoryName,
                Total = y.Sum(s => s.Total)
            }).OrderByDescending(x => x.Total).ToList();

            // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
            newListProductCategory.ForEach(item =>
            {
                var newProductCategoryId = productCategoryList.Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId).ToList();

                // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                if (newProductCategoryId.Count != 0)
                {
                    var listCategoryResult = new ListCategoryResult();
                    var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains(x.ProductCategoryId)).Sum(y => y.Total);
                    listCategoryResult.Total = total;
                    listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                    listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                    newListProductCategoryResult.Add(listCategoryResult);
                }
            });

            //Tinh tong sau chiet khau
            decimal total_money_all = 0;
            orderListAll.ForEach(element =>
            {
                total_money_all += element.Amount;
            });

            //Tinh tong gia tri cua cac Nhom san pham
            //decimal total_product = 0;
            //newListProductCategoryResult.ForEach(item =>
            //{
            //    total_product += item.Total;
            //});

            List<dynamic> lstResult = new List<dynamic>();
            newListProductCategoryResult.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                sampleObject.Add("Total", item.Total);
                decimal percent = 0;
                percent = (item.Total / total_money_all) * 100;
                percent = Math.Round(percent, 1);
                sampleObject.Add("Percent", percent);
                lstResult.Add(sampleObject);
            });

            #endregion

            return new GetCustomerOrderBySellerResult()
            {
                OrderList = orderList,
                lstResult = lstResult,
                totalProduct = total_money_all,
                levelMaxProductCategory = levelMaxProductCategory
            };
        }

        private decimal CalculatorTotal(decimal? Vat, bool DiscountType, decimal? DiscountValue, decimal Quantity, decimal UnitPrice, decimal? ExchangeRate)
        {
            decimal total = (Quantity * UnitPrice * ExchangeRate.Value);
            if (Vat >= 0)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
                total = total + (total * Vat.Value / 100);
            }
            else if (Vat == null)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
            }
            return total;
        }

        //add by dung
        private decimal ReCalculatorTotal(decimal? Vat, bool DiscountType, decimal? DiscountValue, decimal Quantity, decimal UnitPrice, decimal? ExchangeRate, decimal? DiscountPerOrder)
        {
            decimal total = (Quantity * UnitPrice * ExchangeRate.Value);
            if (Vat >= 0)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
                total = total + (total * Vat.Value / 100);
            }
            else if (Vat == null)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
            }

            var discountValue = (total * DiscountPerOrder.Value) / 100;

            total = total - discountValue;

            return total;
        }

        private List<Guid> getProductCategoryChildrenId(Guid? Id, List<Guid> lst, List<ProductCategory> productCategoryList)
        {
            var ProductCategory = productCategoryList.Where(m => m.ParentId == Id).ToList();
            ProductCategory.ForEach(item =>
            {
                lst.Add(item.ProductCategoryId);
                getProductCategoryChildrenId(item.ProductCategoryId, lst, productCategoryList);
            });
            return lst;
        }

        public GetProductCategoryGroupByLevelResult GetProductCategoryGroupByLevel(GetProductCategoryGroupByLevelParameter parameter)
        {
            var commonProductCategory = context.ProductCategory.ToList();
            var commonOrderStatus = context.OrderStatus.Where(x => x.OrderStatusCode == "IP" || x.OrderStatusCode == "DLV" || x.OrderStatusCode == "PD" || x.OrderStatusCode == "COMP").ToList();
            var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();

            var commonCustomerOrder = context.CustomerOrder.Where(c => c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                                          && parameter.Seller == c.Seller
                                          && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                                          && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();
            var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
            var commonCustomerOrderDetails = context.CustomerOrderDetail.Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
            var productIdList = commonCustomerOrderDetails.Select(c => c.ProductId).ToList();
            var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

            var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                  where productcategory.ProductCategoryLevel == parameter.ProductCategoryLevel
                                                  select new
                                                  {
                                                      productcategory.ProductCategoryId,
                                                      productcategory.ProductCategoryName
                                                  }).ToList();

            List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
            listProductCaterogyFirstParent.ForEach(item =>
            {
                List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId, newListProductCategoryIdChildren, commonProductCategory);

                var listCategoryId = new ListCategoryId();
                listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                newListProductCategory.Add(listCategoryId);
            });

            #region LongNH comment

            //newListProductCategory.ForEach(item =>
            //{
            //    var newProductCategoryList = (from customerOderDetail in commonCustomerOrderDetails
            //                                  join product in commonProducts on customerOderDetail.ProductId equals product.ProductId
            //                                  join productCategory in commonProductCategories on product.ProductCategoryId equals productCategory.ProductCategoryId
            //                                  where (item.ListChildrent.Contains(productCategory.ProductCategoryId) || item.ListChildrent.Count == 0)
            //                                  select new
            //                                  {
            //                                      productCategory.ProductCategoryId,
            //                                      productCategory.ProductCategoryName,
            //                                      Total = (decimal)(customerOderDetail.Quantity * customerOderDetail.UnitPrice)
            //                                  }
            //                           ).ToList();
            //    var new_List = newProductCategoryList.GroupBy(x => new { x.ProductCategoryName, x.ProductCategoryId }).Select(y => new
            //    {
            //        Id = y.Key,
            //        y.Key.ProductCategoryName,
            //        y.Key.ProductCategoryId,
            //        Total = y.Sum(s => s.Total)
            //    }).OrderByDescending(x => x.Total).ToList();
            //    var listtt = new ListCategoryResult();
            //    if (new_List.Count > 0)
            //    {
            //        decimal total_money = 0;
            //        listtt.ParentProductCategoryId = item.ParentProductCategoryId;
            //        listtt.ParentProductCategoryName = item.ParentProductCategoryName;
            //        new_List.ForEach(element =>
            //        {
            //            total_money += element.Total;
            //        });
            //        listtt.Total = total_money;
            //        newListProductCategoryResult.Add(listtt);
            //    }
            //});

            #endregion

            List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
            var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
            var productCategoryList = commonProductCategory.Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();
            List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();


            commonCustomerOrderDetails.ForEach(item =>
            {
                #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng
                var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                decimal? discountPerOrder = 0;
                if (customerOrder.DiscountType == true)
                {
                    //chiết khấu phần trăm
                    discountPerOrder = customerOrder?.DiscountValue ?? 0;
                }
                else
                {
                    //chiết khấu theo số tiền
                    discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                }
                #endregion

                var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product != null)
                {
                    var category = commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                    var productCategoryModel = new ProductCategoryModel();
                    productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                    productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                    //productCategoryModel.Total = CalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value);
                    productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value, discountPerOrder);
                    listProductCategoryModel.Add(productCategoryModel);
                }
            });

            // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
            var new_ListProductCategory = listProductCategoryModel.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
            {
                Id = y.Key,
                y.Key.ProductCategoryId,
                y.Key.ProductCategoryName,
                Total = y.Sum(s => s.Total)
            }).OrderByDescending(x => x.Total).ToList();

            // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
            newListProductCategory.ForEach(item =>
            {
                var newProductCategoryId = productCategoryList.Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId).ToList();

                // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                if (newProductCategoryId.Count != 0)
                {
                    var listCategoryResult = new ListCategoryResult();
                    var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains(x.ProductCategoryId)).Sum(y => y.Total);
                    listCategoryResult.Total = total;
                    listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                    listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                    newListProductCategoryResult.Add(listCategoryResult);
                }
            });

            //Tinh tong gia tri cua cac Nhom san pham
            decimal total_product = 0;
            newListProductCategoryResult.ForEach(item =>
            {
                total_product += item.Total;
            });
            newListProductCategoryResult.OrderByDescending(c => c.Total);
            List<dynamic> lstResult = new List<dynamic>();
            newListProductCategoryResult.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                sampleObject.Add("Total", item.Total);
                decimal percent = 0;
                percent = (item.Total / total_product) * 100;
                percent = Math.Round(percent, 1);
                sampleObject.Add("Percent", percent);
                lstResult.Add(sampleObject);
            });

            return new GetProductCategoryGroupByLevelResult()
            {
                lstResult = lstResult
            };
        }

        public GetEmployeeListByOrganizationIdResult GetEmployeeListByOrganizationId(GetEmployeeListByOrganizationIdParameter parameter)
        {
            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
            var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == user.EmployeeId);
            var commonProductCategory = context.ProductCategory.ToList();
            var allStatusOrder = context.OrderStatus.Where(c => c.Active == true).ToList();
            var organizationId = parameter.OrganizationId;
            List<Guid?> listGetAllChild = new List<Guid?>();
            if (organizationId != null)
            {
                listGetAllChild.Add(organizationId);
                listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
            }

            var commonOrganization = context.Organization
                .Where(x => listGetAllChild.Contains(x.OrganizationId)).ToList();
            var organizationIdList = commonOrganization.Select(x => x.OrganizationId).ToList();
            var commonEmployee = context.Employee
                .Where(x => x.OrganizationId != null && organizationIdList.Contains((Guid)x.OrganizationId)).ToList();
            var employeeIdList = commonEmployee.Select(x => x.EmployeeId).ToList();
            var customerOrderAllStatus = context.CustomerOrder.Where(c =>
                c.StatusId != null
                && (parameter.OrderDateStart == null ||
                    parameter.OrderDateStart == DateTime.MinValue ||
                    parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue ||
                    parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();
            if (employee.IsManager)
            {
                customerOrderAllStatus =
                    customerOrderAllStatus.Where(c => employeeIdList.Contains(c.Seller.Value)).ToList();
            }
            else
            {
                customerOrderAllStatus =
                    customerOrderAllStatus.Where(c => c.Seller.Value == employee.EmployeeId).ToList();
            }
            var listOrderCode = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "OrderStatus")
                .SystemValueString.Split(';').ToList();
            var commonOrderStatus = allStatusOrder.Where(x => listOrderCode.Contains(x.OrderStatusCode)).ToList();
            var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
            var commonCustomerOrder = customerOrderAllStatus.Where(c =>
                c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
            ).ToList();

            var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
            var commonCustomerOrderDetail = context.CustomerOrderDetail
                .Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
            var productIdList = commonCustomerOrderDetail.Select(c => c.ProductId).ToList();
            var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

            var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                  where productcategory.ProductCategoryLevel == 0
                                                  select new
                                                  {
                                                      productcategory.ProductCategoryId,
                                                      productcategory.ProductCategoryName
                                                  }).ToList();
            var listProductCategoryLevel = (from productcategory in commonProductCategory
                                            select new
                                            { levelMaxProductCategory = productcategory.ProductCategoryLevel }).ToList();
            int? levelMaxProductCategory = listProductCategoryLevel.Max(x => x.levelMaxProductCategory);

            #region Lấy danh sách 5 nhân viên bán hàng đạt doanh số cao nhất

            var employeeList = new List<EmployeeModel>();

            commonCustomerOrder.ForEach(item =>
            {
                var employeeSeller = commonEmployee.FirstOrDefault(c => c.EmployeeId == item.Seller);
                if (employeeSeller != null)
                {
                    var newOrganization =
                        commonOrganization.FirstOrDefault(c => c.OrganizationId == employeeSeller.OrganizationId);
                    if (newOrganization != null)
                    {
                        var employeeTemp = new EmployeeModel();
                        employeeTemp.EmployeeId = employeeSeller.EmployeeId;
                        employeeTemp.EmployeeName = employeeSeller.EmployeeName;
                        employeeTemp.OrganizationName = newOrganization.OrganizationName;
                        employeeTemp.Amount = item.Amount;
                        employeeTemp.DiscountType = item.DiscountType;
                        employeeTemp.DiscountValue = item.DiscountValue;
                        employeeList.Add(employeeTemp);
                    }
                }
            });

            var newList = (employeeList.GroupBy(x => new { x.EmployeeId, x.EmployeeName, x.OrganizationName }).Select(y =>
                  new
                  {
                      Id = y.Key,
                      y.Key.EmployeeId,
                      y.Key.EmployeeName,
                      y.Key.OrganizationName,
                      Total = y.Sum(s =>
                          (s.DiscountType == true)
                              ? (s.Amount * (1 - s.DiscountValue / 100))
                              : (s.Amount - s.DiscountValue))
                  }).OrderByDescending(x => x.Total)).Take(5).ToList();

            #endregion

            #region Lấy danh sách đơn hàng 

            List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
            listProductCaterogyFirstParent.ForEach(item =>
            {
                List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId,
                    newListProductCategoryIdChildren, commonProductCategory);

                var listCategoryId = new ListCategoryId();
                listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                newListProductCategory.Add(listCategoryId);
            });

            List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
            var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
            var productCategoryList = commonProductCategory
                .Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();

            List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();
            commonCustomerOrderDetail.ForEach(item =>
            {
                #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng

                var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                decimal? discountPerOrder = 0;
                if (customerOrder.DiscountType == true)
                {
                    //chiết khấu phần trăm
                    discountPerOrder = customerOrder?.DiscountValue ?? 0;
                }
                else
                {
                    //chiết khấu theo số tiền
                    discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                }

                #endregion

                var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product != null)
                {
                    var category =
                        commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                    var productCategoryModel = new ProductCategoryModel();
                    productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                    productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                    productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value,
                        item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value,
                        discountPerOrder);
                    listProductCategoryModel.Add(productCategoryModel);
                }
            });

            #endregion

            // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
            var new_ListProductCategory = listProductCategoryModel
                .GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
                {
                    Id = y.Key,
                    y.Key.ProductCategoryId,
                    y.Key.ProductCategoryName,
                    Total = y.Sum(s => s.Total)
                }).OrderByDescending(x => x.Total).ToList();

            // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
            newListProductCategory.ForEach(item =>
            {
                var newProductCategoryId = productCategoryList
                    .Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId)
                    .ToList();

                // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                if (newProductCategoryId.Count != 0)
                {
                    var listCategoryResult = new ListCategoryResult();
                    var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains(x.ProductCategoryId))
                        .Sum(y => y.Total);
                    listCategoryResult.Total = total;
                    listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                    listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                    newListProductCategoryResult.Add(listCategoryResult);
                }
            });

            decimal total_money_search = 0;
            employeeList.ForEach(item =>
            {
                total_money_search = total_money_search + (decimal)((item.DiscountType == true)
                                         ? (item.Amount * (1 - item.DiscountValue / 100))
                                         : (item.Amount - item.DiscountValue));
            });

            List<dynamic> lstResultEmployee = new List<dynamic>();
            newList.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("employeeId", item.EmployeeId);
                sampleObject.Add("employeeName", item.EmployeeName);
                sampleObject.Add("avatarUrl", "");
                sampleObject.Add("organizationName", item.OrganizationName);
                sampleObject.Add("total", item.Total);
                sampleObject.Add("totalSearch", total_money_search);
                lstResultEmployee.Add(sampleObject);
            });

            List<dynamic> lstResult = new List<dynamic>();
            newListProductCategoryResult.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                sampleObject.Add("Total", item.Total);
                decimal percent = 0;

                percent = (item.Total / total_money_search) * 100;

                percent = Math.Round(percent, 1);
                sampleObject.Add("Percent", percent);
                lstResult.Add(sampleObject);
            });

            #region Danh sách đơn hàng chờ xuất kho

            // Lấy đơn hàng có phiếu xuất kho ở trạng thái Chờ xuất
            var statusInventoryDeliveryVoucher = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TPHX");
            var statusInventoryDeliveryVoucherCXL = context.Category.FirstOrDefault(f =>
                f.CategoryCode == "CXK" && f.CategoryTypeId == statusInventoryDeliveryVoucher.CategoryTypeId);
            var listOrderId = context.InventoryDeliveryVoucher
                .Where(i => i.StatusId == statusInventoryDeliveryVoucherCXL.CategoryId).Select(i => i.ObjectId)
                .ToList();

            var lstOrderInventoryDelivery =
                customerOrderAllStatus.Where(ci => listOrderId.Contains(ci.OrderId)).ToList();

            #endregion

            #region Danh sách đơn hàng chờ xuất hóa đơn

            //Lấy các trạng thái ở Đơn hàng bán
            var orderStatusView = allStatusOrder.Where(w =>
               w.OrderStatusCode == "DLV").ToList();
            var orderStatusIdViewList = orderStatusView.Select(o => o.OrderStatusId).ToList();
            var lstOrderBill = customerOrderAllStatus.Where(c =>
                c.StatusId != null && orderStatusIdViewList.Contains((Guid)c.StatusId)).ToList();
            // Lấy các hóa đơn ở trạng thái mới
            var categoryTypeBill =
                context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "BILL" && ct.Active == true);
            var statusNew = context.Category.FirstOrDefault(c =>
                c.CategoryTypeId == categoryTypeBill.CategoryTypeId && c.Active == true && c.CategoryCode == "NEW");

            var billNotOrder = context.BillOfSale.Where(b => b.Active == true && b.StatusId != statusNew.CategoryId)
                .Select(b => b.OrderId).ToList();
            lstOrderBill = lstOrderBill.Where(ob =>
                billNotOrder == null || billNotOrder.Count == 0 || !billNotOrder.Contains(ob.OrderId)).ToList();

            #endregion

            #region Giá trị đơn hàng theo trạng thái

            List<StatusOrderModel> statustList = new List<StatusOrderModel>();
            customerOrderAllStatus.ForEach(item =>
            {
                if (allStatusOrder != null)
                {
                    var statusTemp = new StatusOrderModel();
                    statusTemp.StatusId = item.StatusId.Value;
                    statusTemp.StatusName =
                        allStatusOrder.FirstOrDefault(s => s.OrderStatusId == item.StatusId).Description;
                    statusTemp.Amount = item.Amount;
                    statusTemp.DiscountType = item.DiscountType;
                    statusTemp.DiscountValue = item.DiscountValue;
                    statustList.Add(statusTemp);
                }
            });

            var statusOrderList = (statustList.GroupBy(x => new { x.StatusId, x.StatusName }).Select(y => new
            {
                Id = y.Key,
                y.Key.StatusId,
                y.Key.StatusName,
                Total = y.Sum(s =>
                    (s.DiscountType == true) ? (s.Amount * (1 - s.DiscountValue / 100)) : (s.Amount - s.DiscountValue))
            }).OrderByDescending(x => x.Total)).ToList();

            List<dynamic> lstResultStatusOrder = new List<dynamic>();
            statusOrderList.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("statusId", item.StatusId);
                sampleObject.Add("statusName", item.StatusName);
                sampleObject.Add("total", item.Total);
                lstResultStatusOrder.Add(sampleObject);
            });

            #endregion

            #region Giá trị đơn hàng giữa các tháng

            List<MonthOrderModel> monthList = new List<MonthOrderModel>();
            var customerMonth = commonCustomerOrder.OrderBy(cm => cm.OrderDate).ToList();
            var montStart = parameter.OrderDateStart.Value.Month + 1;
            customerMonth.ForEach(item =>
            {
                var monthTemp = new MonthOrderModel();
                monthTemp.STT = montStart - item.OrderDate.Month;
                monthTemp.MonthName = "Tháng " + item.OrderDate.Month + " Năm " + item.OrderDate.Year;
                monthTemp.TimeNode = new DateTime(item.OrderDate.Year, item.OrderDate.Month, 1, 0, 0, 0, 0);
                monthTemp.Amount = item.Amount;
                monthTemp.DiscountType = item.DiscountType;
                monthTemp.DiscountValue = item.DiscountValue;
                monthList.Add(monthTemp);
            });
            var monthOrderList = (monthList.GroupBy(x => new { x.MonthName, x.TimeNode }).Select(y => new
            {
                Id = y.Key,
                y.Key.MonthName,
                y.Key.TimeNode,
                Total = y.Sum(s =>
                    (s.DiscountType == true) ? (s.Amount * (1 - s.DiscountValue / 100)) : (s.Amount - s.DiscountValue))
            })).OrderBy(x => x.TimeNode).ToList();

            List<dynamic> lstResultMonthOrder = new List<dynamic>();
            monthOrderList.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("monthName", item.MonthName);
                sampleObject.Add("total", item.Total);
                lstResultMonthOrder.Add(sampleObject);
            });

            #endregion

            return new GetEmployeeListByOrganizationIdResult()
            {
                employeeList = lstResultEmployee,
                lstOrderBill = lstOrderBill,
                lstOrderInventoryDelivery = lstOrderInventoryDelivery,
                lstResult = lstResult,
                statusOrderList = lstResultStatusOrder,
                monthOrderList = lstResultMonthOrder,
                levelMaxProductCategory = levelMaxProductCategory
            };
        }

        public GetMonthsListResult GetMonthsList(GetMonthsListParameter parameter)
        {
            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
            var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == user.EmployeeId);

            var organizationId = parameter.OrganizationId;
            List<Guid?> listGetAllChild = new List<Guid?>();
            listGetAllChild.Add(organizationId);
            listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
            var commonOrganization = context.Organization
                .Where(x => listGetAllChild.Contains(x.OrganizationId)).ToList();
            var organizationIdList = commonOrganization.Select(x => x.OrganizationId).ToList();
            if (organizationId != null)
            {
                listGetAllChild.Add(organizationId);
                listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
            }
            var commonEmployee = context.Employee
                .Where(x => x.OrganizationId != null && organizationIdList.Contains((Guid)x.OrganizationId)).ToList();
            var employeeIdList = commonEmployee.Select(x => x.EmployeeId).ToList();
            var allStatusOrder = context.OrderStatus.Where(c => c.Active == true).ToList();

            var dateFrom = parameter.OrderDateTo.Value.Date.AddMonths(-parameter.MonthAdd + 1);
            dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, 1, 0, 0, 0, 0);
            var customerOrderAllStatus = context.CustomerOrder.Where(c =>
                c.StatusId != null
                && (parameter.OrderDateTo != null) && (dateFrom.Date <= c.OrderDate.Date && c.OrderDate.Date <= parameter.OrderDateTo.Value.Date)).ToList();

            customerOrderAllStatus = employee.IsManager ? customerOrderAllStatus.Where(c => employeeIdList.Contains(c.Seller.Value)).ToList() : customerOrderAllStatus.Where(c => c.Seller.Value == employee.EmployeeId).ToList();

            var listOrderCode = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "OrderStatus")
                .SystemValueString.Split(';').ToList();
            var commonOrderStatus = allStatusOrder.Where(x => listOrderCode.Contains(x.OrderStatusCode)).ToList();
            var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
            var commonCustomerOrder = customerOrderAllStatus.Where(c =>
                c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
            ).ToList();

            

            
            
            #region Giá trị đơn hàng giữa các tháng

            List<MonthOrderModel> monthList = new List<MonthOrderModel>();
            var customerMonth = commonCustomerOrder.OrderBy(cm => cm.OrderDate).ToList();
            if (parameter.OrderDateTo != null)
            {
                var montStart = dateFrom.Month;
                customerMonth.ForEach(item =>
                {
                    var monthTemp = new MonthOrderModel();
                    monthTemp.STT = montStart - item.OrderDate.Month;
                    monthTemp.MonthName = item.OrderDate.Month.ToString("d2") + "/" + item.OrderDate.Year;
                    monthTemp.TimeNode = new DateTime(item.OrderDate.Year, item.OrderDate.Month, 1, 0, 0, 0, 0);
                    monthTemp.Amount = item.Amount;
                    monthTemp.DiscountType = item.DiscountType;
                    monthTemp.DiscountValue = item.DiscountValue;
                    monthList.Add(monthTemp);
                });
                
            }

            var startDate = parameter.OrderDateTo.Value.Date.AddMonths(-parameter.MonthAdd + 1);
            var endDate = parameter.OrderDateTo.Value;

            while (startDate <= endDate)
            {
                var monthName = startDate.Month.ToString("d2") + "/" + startDate.Year;
                var count = 0;
                monthList.ForEach(x =>
                {
                    if (x.MonthName == monthName)
                    {
                        count++;
                    }
                });
                if (count == 0)
                {
                    var monthTemp = new MonthOrderModel();
                    monthTemp.STT = -1;
                    monthTemp.MonthName = monthName;
                    monthTemp.TimeNode = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, 0);
                    monthTemp.Amount = 0;
                    monthTemp.DiscountType = true;
                    monthTemp.DiscountValue = 0;
                    monthList.Add(monthTemp);
                }

                startDate = startDate.AddMonths(1);
            }

            var monthOrderList = (monthList.GroupBy(x => new { x.MonthName, x.TimeNode }).Select(y => new
            {
                Id = y.Key,
                y.Key.MonthName,
                y.Key.TimeNode,
                Total = y.Sum(s =>
                    (s.DiscountType == true) ? (s.Amount * (1 - s.DiscountValue / 100)) : (s.Amount - s.DiscountValue))
            })).OrderBy(x => x.TimeNode).ToList();

            List<dynamic> lstResultMonthOrder = new List<dynamic>();
            monthOrderList.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("monthName", item.MonthName);
                sampleObject.Add("total", item.Total);
                lstResultMonthOrder.Add(sampleObject);
            });

            #endregion


            return new GetMonthsListResult()
            {
                monthOrderList = lstResultMonthOrder,
            };
        }

        private List<Guid?> getOrganizationChildrenId(List<Organization> organizationList, Guid? id, List<Guid?> list)
        {
            var organizations = organizationList.Where(o => o.ParentId == id).ToList();
            organizations.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(organizationList, item.OrganizationId, list);
            });

            return list;
        }

        public GetProductCategoryGroupByManagerResult GetProductCategoryGroupByManager(GetProductCategoryGroupByManagerParameter parameter)
        {
            var commonProductCategory = context.ProductCategory.ToList();

            var organizationId = parameter.OrganizationId;
            List<Guid?> listGetAllChild = new List<Guid?>();
            if (organizationId != null)
            {
                listGetAllChild.Add(organizationId);
                listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
            }

            var commonOrganization = context.Organization.Where(c => listGetAllChild.Contains(c.OrganizationId)).ToList();
            var organizationIdList = commonOrganization.Select(c => c.OrganizationId).ToList();
            var commonEmployee = context.Employee.Where(x => x.OrganizationId != null && organizationIdList.Contains((Guid)x.OrganizationId)).ToList();
            var employeeIdList = commonEmployee.Select(x => x.EmployeeId).ToList();

            var commonOrderStatus = context.OrderStatus.Where(w => w.OrderStatusCode == "IP" || w.OrderStatusCode == "DLV" || w.OrderStatusCode == "PD" || w.OrderStatusCode == "COMP").ToList();
            var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
            var commonCustomerOrder = context.CustomerOrder.Where(c => c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                                          && employeeIdList.Contains((Guid)c.Seller)
                                          && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                                          && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();
            var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
            var commonCustomerOrderDetail = context.CustomerOrderDetail.Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
            var productIdList = commonCustomerOrderDetail.Select(c => c.ProductId).ToList();
            var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

            var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                  where productcategory.ProductCategoryLevel == parameter.ProductCategoryLevel
                                                  select new
                                                  {
                                                      productcategory.ProductCategoryId,
                                                      productcategory.ProductCategoryName
                                                  }).ToList();
            List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
            listProductCaterogyFirstParent.ForEach(item =>
            {
                List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId, newListProductCategoryIdChildren, commonProductCategory);

                var listCategoryId = new ListCategoryId();
                listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                newListProductCategory.Add(listCategoryId);
            });

            List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
            var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
            var productCategoryList = commonProductCategory.Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();

            List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();
            commonCustomerOrderDetail.ForEach(item =>
            {
                #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng
                var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                decimal? discountPerOrder = 0;
                if (customerOrder.DiscountType == true)
                {
                    //chiết khấu phần trăm
                    discountPerOrder = customerOrder?.DiscountValue ?? 0;
                }
                else
                {
                    //chiết khấu theo số tiền
                    discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                }
                #endregion

                var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product != null)
                {
                    var category = commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                    var productCategoryModel = new ProductCategoryModel();
                    productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                    productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                    //productCategoryModel.Total = SumAmount(item.Quantity, item.UnitPrice, item.Vat, item.DiscountValue, item.DiscountType, item.ExchangeRate);
                    productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value, discountPerOrder);
                    listProductCategoryModel.Add(productCategoryModel);
                }
            });


            // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
            var new_ListProductCategory = listProductCategoryModel.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
            {
                Id = y.Key,
                y.Key.ProductCategoryId,
                y.Key.ProductCategoryName,
                Total = y.Sum(s => s.Total)
            }).OrderByDescending(x => x.Total).ToList();

            // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
            newListProductCategory.ForEach(item =>
            {
                var newProductCategoryId = productCategoryList.Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId).ToList();

                // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                if (newProductCategoryId.Count != 0)
                {
                    var listCategoryResult = new ListCategoryResult();
                    var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains(x.ProductCategoryId)).Sum(y => y.Total);
                    listCategoryResult.Total = total;
                    listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                    listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                    newListProductCategoryResult.Add(listCategoryResult);
                }
            });

            //Tinh tong gia tri cua cac Nhom san pham
            decimal total_product = 0;
            newListProductCategoryResult.ForEach(item =>
            {
                total_product += item.Total;
            });
            List<dynamic> lstResult = new List<dynamic>();
            newListProductCategoryResult.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                sampleObject.Add("Total", item.Total);
                decimal percent = 0;
                percent = (item.Total / total_product) * 100;
                percent = Math.Round(percent, 1);
                sampleObject.Add("Percent", percent);
                lstResult.Add(sampleObject);
            });

            return new GetProductCategoryGroupByManagerResult()
            {
                lstResult = lstResult
            };
        }

        public GetMasterDataOrderSearchResult GetMasterDataOrderSearch(GetMasterDataOrderSearchParameter parameter)
        {
            try
            {
                var listOrderStatus = new List<OrderStatus>();
                listOrderStatus = context.OrderStatus.Where(x => x.Active).OrderBy(z => z.Description).ToList();
                var listQuote = new List<QuoteEntityModel>();
                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();

                var listCategoryType = context.CategoryType.Where(x => x.Active == true).ToList();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();
                var statusCodeQuote = listCategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "TGI");
                var statusQuoteBG = listCategory.FirstOrDefault(ca => ca.CategoryTypeId == statusCodeQuote.CategoryTypeId && ca.CategoryCode == "DTH");
                var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listAllQuote = context.Quote.Where(x => x.Active == true && x.StatusId == statusQuoteBG.CategoryId).ToList();
                List<Guid> listEmpId = listAllEmployee.Select(x => x.EmployeeId).ToList();
                listQuote = listAllQuote.Where(x => x.Seller != null && listEmpId.Contains(Guid.Parse(x.Seller.ToString())))
                    .Select(y => new QuoteEntityModel
                    {
                        QuoteId = y.QuoteId,
                        QuoteCode = y.QuoteCode,
                        QuoteName = y.QuoteName,
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
                        ReceivedHour = y.ReceivedHour,
                        RecipientName = y.RecipientName,
                        LocationOfShipment = y.LocationOfShipment,
                        ShippingNote = y.ShippingNote,
                        RecipientPhone = y.RecipientPhone,
                        RecipientEmail = y.RecipientEmail,
                        PlaceOfDelivery = y.PlaceOfDelivery,
                        Amount = y.Amount,
                        DiscountValue = y.DiscountValue,
                        IntendedQuoteDate = y.IntendedQuoteDate,
                        StatusId = y.StatusId,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        PersonInChargeId = y.PersonInChargeId,
                        ApprovalStep = y.ApprovalStep,
                        IsSendQuote = y.IsSendQuote,
                        LeadId = y.LeadId,
                        SaleBiddingId = y.SaleBiddingId,

                        QuoteCodeName = y.QuoteCode + "" + (y.QuoteName == null ? "" : " - " + y.QuoteName),
                    }).ToList();

                #region Lấy hợp đồng
                var listContract = context.Contract.Where(ctr => ctr.Active == true && ctr.EmployeeId != null && listEmpId.Contains(ctr.EmployeeId.Value)).ToList();
                #endregion

                var listOrder = new List<CustomerOrderEntityModel>();
                var newListOrderBy = new List<CustomerOrderEntityModel>();
                var newListEmployee = new List<Employee>();

                var listAllUser = context.User.ToList();

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;
                if (isManager)
                {
                    //Nếu là quản lý

                    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    newListEmployee = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();

                }
                else
                {
                    newListEmployee = listAllEmployee.Where(x => x.EmployeeId == employeeId).ToList();
                }

                var companyConfig = context.CompanyConfiguration.FirstOrDefault();
                PDFOrderModel PDFOrder = new PDFOrderModel();
                PDFOrder.CompanyName = companyConfig.CompanyName == null ? "" : companyConfig.CompanyName;
                PDFOrder.Website = "";
                PDFOrder.TaxCode = companyConfig.TaxCode == null ? "" : companyConfig.TaxCode;
                PDFOrder.CompanyPhone = companyConfig.Phone == null ? "" : companyConfig.Phone;
                PDFOrder.CompanyEmail = companyConfig.Email == null ? "" : companyConfig.Email;
                PDFOrder.CompanyAddress = companyConfig.CompanyAddress == null ? "" : companyConfig.CompanyAddress;

                var listProduct = listAllProduct.Select(x => new ProductEntityModel
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
                return new GetMasterDataOrderSearchResult()
                {
                    Status = true,
                    Message = "Success",
                    ListOrderStatus = listOrderStatus,
                    ListContract = listContract,
                    ListProduct = listProduct,
                    ListQuote = listQuote,
                    ListEmployee = newListEmployee,
                    PDFOrder = PDFOrder
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderSearchResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchOrderResult SearchOrder(SearchOrderParameter parameter)
        {
            try
            {
                var listOrder = new List<CustomerOrderEntityModel>();
                var newListOrderBy = new List<CustomerOrderEntityModel>();

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllStatus = context.OrderStatus.ToList();
                var listAllContact = context.Contact.ToList();

                //check isManager
                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new SearchOrderResult()
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchOrderResult()
                    {
                        Status = false,
                        Message = "Lỗi dữ liệu"
                    };
                }

                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (employee == null)
                {
                    return new SearchOrderResult()
                    {
                        Status = false,
                        Message = "Lỗi dữ liệu"
                    };
                }
                var isManager = employee.IsManager;

                var orderCode = parameter.OrderCode == null ? "" : parameter.OrderCode.Trim();
                var customerName = parameter.CustomerName == null ? "" : parameter.CustomerName.Trim();
                var phone = parameter.Phone == null ? "" : parameter.Phone.Trim();
                var fromDate = parameter.FromDate;
                var toDate = parameter.ToDate;
                var listStatusId = parameter.ListStatusId;

                var lstOrderBy = context.CustomerOrder.Join(context.Customer, or => or.CustomerId, cus => cus.CustomerId,
                    (or, cus) => new { or, cus })
                .Where(x => x.or.Active == true && (customerName == "" || x.cus.CustomerName.Contains(customerName)) &&
                            (orderCode == "" || x.or.OrderCode.Contains(orderCode)) &&
                            (listStatusId == null || listStatusId.Count == 0 || listStatusId.Contains(x.or.StatusId.Value)) &&
                            (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.or.OrderDate) &&
                            (toDate == null || toDate == DateTime.MinValue || toDate >= x.or.OrderDate))
                .Select(m => new CustomerOrderEntityModel
                {
                    OrderId = m.or.OrderId,
                    OrderCode = m.or.OrderCode,
                    OrderDate = m.or.OrderDate,
                    Seller = m.or.Seller,
                    SellerName = listAllEmployee.FirstOrDefault(e => e.EmployeeId == m.or.Seller) == null ? "" : listAllEmployee.FirstOrDefault(e => e.EmployeeId == m.or.Seller).EmployeeName,
                    Description = m.or.Description,
                    Note = m.or.Note,
                    CustomerId = m.or.CustomerId,
                    CustomerName = m.cus.CustomerName,
                    CustomerContactId = Guid.Empty,
                    PaymentMethod = m.or.PaymentMethod,
                    DaysAreOwed = m.or.DaysAreOwed,
                    MaxDebt = m.or.MaxDebt,
                    ReceivedDate = m.or.ReceivedDate,
                    ReceivedHour = m.or.ReceivedHour,
                    RecipientName = m.or.RecipientName,
                    LocationOfShipment = m.or.LocationOfShipment,
                    ShippingNote = m.or.ShippingNote,
                    RecipientPhone = m.or.RecipientPhone,
                    RecipientEmail = m.or.RecipientEmail,
                    PlaceOfDelivery = m.or.PlaceOfDelivery,
                    Amount = (decimal)((m.or.DiscountType == true)
                        ? (m.or.Amount * (1 - (m.or.DiscountValue / 100)))
                        : (m.or.Amount - m.or.DiscountValue)),
                    DiscountValue = m.or.DiscountValue,
                    StatusId = m.or.StatusId,
                    StatusCode = listAllStatus.FirstOrDefault(f => f.OrderStatusId == m.or.StatusId).OrderStatusCode ?? "",
                    OrderStatusName = listAllStatus.FirstOrDefault(s => s.OrderStatusId == m.or.StatusId).Description,
                    CreatedById = m.or.CreatedById,
                    CreatedDate = m.or.CreatedDate,
                    UpdatedById = m.or.UpdatedById,
                    UpdatedDate = m.or.UpdatedDate,
                    Active = m.or.Active,
                    DiscountType = m.or.DiscountType,
                    SellerAvatarUrl = "",
                    SellerFirstName = "",
                    SellerLastName = "",
                    ListOrderDetail = "",
                    QuoteId = m.or.QuoteId,
                    OrderContractId = m.or.OrderContractId
                }).ToList();

                var lstCustomerId = new List<Guid>();
                lstOrderBy.ForEach(item =>
                {
                    var dupblicate = lstCustomerId.FirstOrDefault(x => x == item.CustomerId);
                    if (dupblicate == Guid.Empty)
                    {
                        lstCustomerId.Add(item.CustomerId);
                    }
                });

                var lstContact = context.Contact.Where(x =>
                    (x.ObjectType == ObjectType.CUSTOMER || x.ObjectType == ObjectType.CUSTOMERCONTACT) &&
                    (lstCustomerId == null || lstCustomerId.Count == 0 || lstCustomerId.Contains(x.ObjectId)) &&
                    (phone == "" || (x.Phone != null && x.Phone.ToLower().Contains(phone.ToLower())) ||
                     (x.WorkPhone != null && x.WorkPhone.ToLower().Contains(phone.ToLower())) ||
                     (x.OtherPhone != null && x.OtherPhone.ToLower().Contains(phone.ToLower())))).ToList();

                if (isManager)
                {
                    //Nếu là quản lý

                    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                    List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();

                    listEmployeeInChargeByManager.ForEach(item =>
                    {
                        if (item.EmployeeId != Guid.Empty)
                            listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                    });

                    lstOrderBy = lstOrderBy.Where(x =>
                        (listEmployeeInChargeByManagerId.Count == 0 ||
                         (x.Seller != null && listEmployeeInChargeByManagerId.Contains(x.Seller.Value)))).ToList();

                    List<Guid> lstOrderId = new List<Guid>();
                    lstOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();
                    List<Guid> lstProductId = new List<Guid>();
                    listOrderDetail.ForEach(item =>
                    {
                        var dublicateProduct = lstProductId.FirstOrDefault(x => x == item.ProductId);
                        if (dublicateProduct == Guid.Empty)
                        {
                            if (item.ProductId != null)
                                lstProductId.Add(item.ProductId.Value);
                        }
                    });
                    var listProduct = context.Product.Where(x => (lstProductId == null || lstProductId.Count == 0 || lstProductId.Contains(x.ProductId))).ToList();

                    lstOrderBy.ForEach(item =>
                    {
                        var contact = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (contact != null)
                        {
                            var contactCustomer = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == ObjectType.CUSTOMER);
                            var contactSeller = listAllContact.FirstOrDefault(x => x.ObjectId == item.Seller && x.ObjectType == ObjectType.EMPLOYEE);

                            item.CustomerContactId = contactCustomer == null ? Guid.Empty : contactCustomer.ContactId;
                            item.SellerFirstName = contactSeller == null ? "" : contactSeller.FirstName;
                            item.SellerLastName = contactSeller == null ? "" : contactSeller.LastName;

                            var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
                            if (orderDetail.Count > 0)
                            {
                                orderDetail.ForEach(currentOrderDetail =>
                                {
                                    var productName = "";
                                    var product = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId);
                                    if (product != null)
                                    {
                                        productName = product.ProductName + "; ";
                                    }
                                    item.ListOrderDetail += productName;
                                });
                            }
                            else
                            {
                                item.ListOrderDetail = "";
                            }

                            newListOrderBy.Add(item);
                        }
                    });
                }
                else
                {
                    //Nếu là nhân viên
                    lstOrderBy = lstOrderBy.Where(x => x.Seller == employeeId).ToList();

                    List<Guid> lstOrderId = new List<Guid>();
                    lstOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();
                    List<Guid> lstProductId = new List<Guid>();
                    listOrderDetail.ForEach(item =>
                    {
                        var dublicateProduct = lstProductId.FirstOrDefault(x => x == item.ProductId);
                        if (dublicateProduct == Guid.Empty)
                        {
                            if (item.ProductId != null)
                                lstProductId.Add(item.ProductId.Value);
                        }
                    });
                    var listProduct = context.Product.Where(x => (lstProductId == null || lstProductId.Count == 0 || lstProductId.Contains(x.ProductId))).ToList();

                    lstOrderBy.ForEach(item =>
                    {
                        var contact = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (contact != null)
                        {
                            var contactCustomer = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == ObjectType.CUSTOMER);
                            var contactSeller = listAllContact.FirstOrDefault(x => x.ObjectId == item.Seller && x.ObjectType == ObjectType.EMPLOYEE);

                            item.CustomerContactId = contactCustomer == null ? Guid.Empty : contactCustomer.ContactId;
                            item.SellerFirstName = contactSeller == null ? "" : contactSeller.FirstName;
                            item.SellerLastName = contactSeller == null ? "" : contactSeller.LastName;

                            var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
                            if (orderDetail.Count > 0)
                            {
                                orderDetail.ForEach(currentOrderDetail =>
                                {
                                    var productName = "";
                                    var product = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId);
                                    if (product != null)
                                    {
                                        productName = product.ProductName + "; ";
                                    }
                                    item.ListOrderDetail += productName;
                                });
                            }
                            else
                            {
                                item.ListOrderDetail = "";
                            }

                            newListOrderBy.Add(item);
                        }
                    });
                }

                #region Tìm những đơn hàng liên quan đến hóa đơn VAT

                if (parameter.Vat == 1)
                {
                    //Đơn hàng có cả hóa đơn VAT và không có hóa đơn VAT
                    listOrder = newListOrderBy;
                }
                else if (parameter.Vat == 2)
                {
                    //Đơn hàng có hóa đơn VAT
                    List<Guid> lstOrderId = new List<Guid>();
                    newListOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();

                    newListOrderBy.ForEach(item =>
                    {
                        int countVat = listOrderDetail.Where(c => c.OrderId == item.OrderId && c.Vat > 0).Count();
                        if (countVat > 0)
                        {
                            listOrder.Add(item);
                        }
                    });
                }
                else if (parameter.Vat == 3)
                {
                    //Đơn hàng không có hóa đơn VAT
                    List<Guid> lstOrderId = new List<Guid>();
                    newListOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();

                    newListOrderBy.ForEach(item =>
                    {
                        var countVat = listOrderDetail.Where(c => c.OrderId == item.OrderId && c.Vat > 0).Count();
                        if (countVat == 0)
                        {
                            listOrder.Add(item);
                        }
                    });
                }

                #endregion

                listOrder = listOrder.OrderByDescending(x => x.OrderDate).ToList();

                #region Thêm điều kiện xóa đơn hàng
                var drapStatus = listAllStatus.FirstOrDefault(f => f.OrderStatusCode == "DRA")?.OrderStatusId;
                if (drapStatus != null)
                {
                    listOrder.ForEach(order =>
                    {
                        if (order.StatusId == drapStatus)
                        {
                            order.CanDelete = true;
                        }
                        else
                        {
                            order.CanDelete = false;
                        }
                    });
                }
                #endregion

                #region Thêm điều kiện lọc báo giá
                if (parameter.QuoteId != null && parameter.QuoteId != Guid.Empty)
                {
                    listOrder = listOrder.Where(l => l.QuoteId == parameter.QuoteId).ToList();
                }
                #endregion

                #region Thêm điều kiện lọc sản phẩm
                if (parameter.ProductId != null && parameter.ProductId != Guid.Empty)
                {
                    List<Guid> lstOrdId = context.CustomerOrderDetail.Where(co => co.ProductId == parameter.ProductId).Select(co => co.OrderId).ToList();
                    listOrder = listOrder.Where(l => lstOrdId.Contains(l.OrderId)).ToList();
                }
                #endregion

                #region Thêm điều kiện lọc hợp đồng
                if (parameter.ContractId != null && parameter.ContractId != Guid.Empty)
                {
                    listOrder = listOrder.Where(l => l.OrderContractId == parameter.ContractId).ToList();
                }
                #endregion

                var lstIP = listOrder.Where(l => l.StatusCode == "IP").ToList();
                var lstDRA = listOrder.Where(l => l.StatusCode == "DRA").ToList();
                var lstDLV = listOrder.Where(l => l.StatusCode == "DLV").ToList();
                var lstRTN = listOrder.Where(l => l.StatusCode == "RTN").ToList();
                var lstCOMP = listOrder.Where(l => l.StatusCode == "COMP").ToList();
                var lstCAN = listOrder.Where(l => l.StatusCode == "CAN").ToList();

                var lstOrder = lstIP;
                lstDRA.ForEach(item =>
                {
                    lstOrder.Add(item);
                });
                lstDLV.ForEach(item =>
                {
                    lstOrder.Add(item);
                });
                lstRTN.ForEach(item =>
                {
                    lstOrder.Add(item);
                });
                lstCOMP.ForEach(item =>
                {
                    lstOrder.Add(item);
                });
                lstCAN.ForEach(item =>
                {
                    lstOrder.Add(item);
                });
                return new SearchOrderResult()
                {
                    Status = true,
                    Message = "Success",
                    ListOrder = lstOrder
                };
            }
            catch (Exception e)
            {
                return new SearchOrderResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataOrderCreateResult GetMasterDataOrderCreate(GetMasterDataOrderCreateParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listOrderStatus = new List<OrderStatus>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listCustomerBankAccount = new List<BankAccountEntityModel>();
                var listCustomerGroup = new List<CategoryEntityModel>();
                var listPaymentMethod = new List<CategoryEntityModel>();
                var listQuote = new List<QuoteEntityModel>();
                var listWare = new List<WareHouseEntityModel>();
                var listCustomerCode = new List<string>();

                var listCategoryType = context.CategoryType.Where(x => x.Active == true).ToList();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();
                var statusCodeQuote = listCategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "TGI");
                var statusQuoteBG = listCategory.FirstOrDefault(ca =>
                    ca.CategoryTypeId == statusCodeQuote.CategoryTypeId && ca.CategoryCode == "DTH");
                var listAllCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllQuote = context.Quote.Where(x => x.Active == true && x.StatusId == statusQuoteBG.CategoryId)
                    .ToList();
                var listAllQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listAllQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listAllQuoteDetailAttribute = context.QuoteProductDetailProductAttributeValue.ToList();
                var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "CUS").ToList();
                var listAllVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();
                var listAllCost = context.Cost.Where(x => x.Active == true).ToList();

                #region Lấy List Order Status

                listOrderStatus = context.OrderStatus
                    .Where(x => x.Active && (x.OrderStatusCode == "IP" || x.OrderStatusCode == "DRA"))
                    .OrderBy(z => z.Description).ToList();

                #endregion

                #region Lấy List nhân viên bán hàng

                if (parameter.CreateType == 1)
                {
                    
                }
                else if (parameter.CreateType == 2)
                {

                }
                else if (parameter.CreateType == 3)
                {
                    var _quote = listAllQuote.FirstOrDefault(x => x.QuoteId == parameter.CreateObjectId);
                    var customerQuoteId = _quote.ObjectTypeId;
                    var customerQuote = listAllCustomer.FirstOrDefault(x => x.CustomerId == customerQuoteId);
                    var employeeCustomerId = customerQuote.PersonInChargeId;

                    employee = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeCustomerId);
                }
                else if (parameter.CreateType == 4)
                {
                    var _contract = context.Contract.FirstOrDefault(x => x.ContractId == parameter.CreateObjectId);
                    var customerContractId = _contract.CustomerId;
                    var customerContract = listAllCustomer.FirstOrDefault(x => x.CustomerId == customerContractId);
                    var employeeCustomerId = customerContract.PersonInChargeId;

                    employee = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeCustomerId);
                }

                if (employee.IsManager)
                {
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }

                    //Lấy danh sách nhân viên Employyee mà user phụ trách
                    listEmployee =
                        listAllEmployee.Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(y => new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeCode = y.EmployeeCode,
                            EmployeeName = y.EmployeeName,
                            EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                            OrganizationId = y.OrganizationId,
                            IsManager = y.IsManager,
                            Active = y.Active
                        }).ToList();
                }
                else
                {
                    listEmployee = listAllEmployee.Where(x => x.EmployeeId == employee.EmployeeId).Select(y =>
                        new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeCode = y.EmployeeCode,
                            EmployeeName = y.EmployeeName,
                            EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                            OrganizationId = y.OrganizationId,
                            IsManager = y.IsManager,
                            Active = y.Active
                        }).ToList();
                }

                #region Lấy nhân viên phụ trách trong trường hợp ngoại lệ

                if (parameter.CreateType == 3)
                {
                    var _quote = listAllQuote.FirstOrDefault(x => x.QuoteId == parameter.CreateObjectId);
                    var employeeQuoteId = _quote.Seller;

                    var existsEmployee = listEmployee.FirstOrDefault(x => x.EmployeeId == employeeQuoteId.Value);
                    if (existsEmployee == null && employeeQuoteId != null)
                    {
                        var employeeQuote = listAllEmployee.Where(x => x.EmployeeId == employeeQuoteId).Select(y =>
                        new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeCode = y.EmployeeCode,
                            EmployeeName = y.EmployeeName,
                            EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                            OrganizationId = y.OrganizationId,
                            IsManager = y.IsManager
                        }).FirstOrDefault();

                        listEmployee.Add(employeeQuote);
                    }
                }

                #endregion

                #endregion

                #region Lấy List Customer

                var listEmployeeId = listEmployee.Select(x => x.EmployeeId).ToList();
                var categoryTypeTHA = context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "THA");
                var categoryNew = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "MOI" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);
                var categoryHDO = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "HDO" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);

                if (listEmployeeId.Count > 0)
                {
                    var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId))
                        .Select(y => y.UserId);
                    listCustomer = listAllCustomer.Where(x => x.StatusId == categoryHDO.CategoryId &&
                        listEmployeeId.Contains(x.PersonInChargeId) ||
                        (x.PersonInChargeId == null && listUserId.Contains(x.CreatedById))).Select(
                        y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerGroupId = y.CustomerGroupId,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            PaymentId = y.PaymentId,
                            PersonInChargeId = y.PersonInChargeId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            CustomerCodeName = y.CustomerCode + " - " + y.CustomerName,
                        }).ToList();

                    #region Lấy khách hàng trong trường hợp ngoại lệ

                    if (parameter.CreateType == 3)
                    {
                        var _quote = listAllQuote.FirstOrDefault(x => x.QuoteId == parameter.CreateObjectId);
                        var _customerId = _quote.ObjectTypeId;

                        var existsCustomer = listCustomer.FirstOrDefault(x => x.CustomerId == _customerId.Value);
                        if (existsCustomer == null && _customerId != null)
                        {
                            var _customer = listAllCustomer.Where(x => x.CustomerId == _customerId).Select(y =>
                                new CustomerEntityModel
                                {
                                    CustomerId = y.CustomerId,
                                    CustomerCode = y.CustomerCode,
                                    CustomerName = y.CustomerName,
                                    CustomerGroupId = y.CustomerGroupId,
                                    CustomerEmail = "",
                                    CustomerPhone = "",
                                    FullAddress = "",
                                    PaymentId = y.PaymentId,
                                    PersonInChargeId = y.PersonInChargeId,
                                    MaximumDebtDays = y.MaximumDebtDays,
                                    MaximumDebtValue = y.MaximumDebtValue,
                                    CustomerCodeName = y.CustomerCode + " - " + y.CustomerName,
                                }).FirstOrDefault();

                            listCustomer.Add(_customer);
                        }
                    }

                    #endregion

                    var listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();

                    if (listCustomerId.Count > 0)
                    {
                        var listCustomerContact =
                            listAllContact.Where(x => listCustomerId.Contains(x.ObjectId)).ToList();

                        listCustomer.ForEach(item =>
                        {
                            var contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            var email = contact == null ? "" : (contact.Email == null ? "" : contact.Email.Trim());
                            var phone = contact == null ? "" : (contact.Phone == null ? "" : contact.Phone.Trim());
                            var taxCode = contact == null
                                ? ""
                                : (contact.TaxCode == null ? "" : contact.TaxCode.Trim());

                            item.CustomerEmail = email;
                            item.CustomerPhone = phone;
                            item.TaxCode = taxCode;
                        });
                    }
                }

                #endregion

                #region Lấy List Tài khoản thanh toán của tất cả khách hàng

                listCustomerBankAccount = context.BankAccount.Where(x => x.Active == true && x.ObjectType == "CUS")
                    .Select(y => new BankAccountEntityModel
                    {
                        BankAccountId = y.BankAccountId,
                        ObjectId = y.ObjectId,
                        AccountNumber = (y.BankName ?? "") + " - " + (y.AccountNumber ?? "")
                    }).ToList();

                #endregion

                #region Lấy List Nhóm khách hàng

                var categoryType1 = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA")
                    .CategoryTypeId;
                listCustomerGroup = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryType1)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Phương thức thanh toán

                var categoryType2 = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PTO")
                    .CategoryTypeId;
                listPaymentMethod = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryType2)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Customer Code

                listCustomerCode = listAllCustomer.Where(x => (x.CustomerCode ?? "").Trim() != "")
                    .Select(y => y.CustomerCode.ToLower()).ToList();

                #endregion

                #region Lấy List Quote

                List<Guid> listEmpId = listAllEmployee.Select(x => x.EmployeeId).ToList();
                listQuote = listAllQuote.Where(x => x.Seller != null && listEmpId.Contains(Guid.Parse(x.Seller.ToString())))
                    .Select(y => new QuoteEntityModel
                    {
                        QuoteId = y.QuoteId,
                        QuoteCode = y.QuoteCode,
                        QuoteName = y.QuoteName,
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
                        ReceivedHour = y.ReceivedHour,
                        RecipientName = y.RecipientName,
                        LocationOfShipment = y.LocationOfShipment,
                        ShippingNote = y.ShippingNote,
                        RecipientPhone = y.RecipientPhone,
                        RecipientEmail = y.RecipientEmail,
                        PlaceOfDelivery = y.PlaceOfDelivery,
                        Amount = y.Amount,
                        DiscountValue = y.DiscountValue,
                        IntendedQuoteDate = y.IntendedQuoteDate,
                        StatusId = y.StatusId,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        PersonInChargeId = y.PersonInChargeId,
                        ApprovalStep = y.ApprovalStep,
                        IsSendQuote = y.IsSendQuote,
                        LeadId = y.LeadId,
                        SaleBiddingId = y.SaleBiddingId,
                        QuoteCodeName = y.QuoteCode + "" + (y.QuoteName == null ? "" : " - " + y.QuoteName),
                    }).ToList();

                listQuote.ForEach(item =>
                {
                    item.ListDetail = listAllQuoteDetail.Where(d => d.QuoteId == item.QuoteId)
                        .Select(d => new QuoteDetailEntityModel
                        {
                            QuoteDetailId = d.QuoteDetailId,
                            VendorId = d.VendorId,
                            QuoteId = d.QuoteId,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            CurrencyUnit = d.CurrencyUnit,
                            ExchangeRate = d.ExchangeRate,
                            Vat = d.Vat,
                            DiscountType = d.DiscountType,
                            DiscountValue = d.DiscountValue,
                            Description = d.Description,
                            OrderDetailType = d.OrderDetailType,
                            UnitId = d.UnitId,
                            IncurredUnit = d.IncurredUnit,
                            Active = d.Active,
                            CreatedById = d.CreatedById,
                            CreatedDate = d.CreatedDate,
                            UpdatedById = d.UpdatedById,
                            UpdatedDate = d.UpdatedDate,
                            NameVendor = listAllVendor.FirstOrDefault(f => f.VendorId == d.VendorId) == null
                                ? null
                                : listAllVendor.FirstOrDefault(f => f.VendorId == d.VendorId).VendorName,
                            NameMoneyUnit = listCategory.FirstOrDefault(f => f.CategoryId == d.CurrencyUnit) == null
                                ? null
                                : listCategory.FirstOrDefault(f => f.CategoryId == d.CurrencyUnit).CategoryName,
                            NameProductUnit = listCategory.FirstOrDefault(f => f.CategoryId == d.UnitId) == null
                                ? null
                                : listCategory.FirstOrDefault(f => f.CategoryId == d.UnitId).CategoryName,
                            NameProduct = listAllProduct.FirstOrDefault(f => f.ProductId == d.ProductId) == null
                                ? null
                                : listAllProduct.FirstOrDefault(f => f.ProductId == d.ProductId).ProductName,
                            PriceInitial = d.PriceInitial,
                            IsPriceInitial = d.IsPriceInitial,
                            ProductName = d.ProductName,
                            OrderNumber = d.OrderNumber
                        }).OrderBy(z => z.OrderNumber).ToList();

                    item.ListCostDetail = listAllQuoteCostDetail.Where(d => d.QuoteId == item.QuoteId).Select(d =>
                        new QuoteCostDetailEntityModel
                        {
                            QuoteCostDetailId = d.QuoteCostDetailId,
                            CostId = d.CostId,
                            QuoteId = d.QuoteId,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            CostName = d.CostName,
                            CostCode = listAllCost.FirstOrDefault(c => c.CostId == d.CostId) == null
                                ? null
                                : listAllCost.FirstOrDefault(c => c.CostId == d.CostId).CostName,
                            Active = d.Active,
                            CreatedById = d.CreatedById,
                            CreatedDate = d.CreatedDate,
                            UpdatedById = d.UpdatedById,
                            UpdatedDate = d.UpdatedDate,
                        }).ToList();

                    item.ListDetail.ForEach(detail =>
                    {
                        detail.QuoteProductDetailProductAttributeValue = listAllQuoteDetailAttribute
                            .Where(dt => dt.QuoteDetailId == detail.QuoteDetailId).Select(dt =>
                                new QuoteProductDetailProductAttributeValueEntityModel
                                {
                                    QuoteDetailId = dt.QuoteDetailId,
                                    ProductId = dt.ProductId,
                                    ProductAttributeCategoryId = dt.ProductAttributeCategoryId,
                                    ProductAttributeCategoryValueId = dt.ProductAttributeCategoryValueId,
                                    QuoteProductDetailProductAttributeValueId =
                                        dt.QuoteProductDetailProductAttributeValueId
                                }).ToList();
                    });
                });

                #endregion

                #region Lấy List Warehouse
                listWare = context.Warehouse.Where(d => d.WarehouseParent == null)
                    .Select(d => new WareHouseEntityModel
                    {
                        WarehouseId = d.WarehouseId,
                        WarehouseCode = d.WarehouseCode + " - " + d.WarehouseName,
                        WarehouseName = d.WarehouseName,
                        WarehouseParent = d.WarehouseParent,
                        WarehouseAddress = d.WarehouseAddress,
                        WarehousePhone = d.WarehousePhone,
                        Storagekeeper = d.Storagekeeper,
                        WarehouseDescription = d.WarehouseDescription,
                        Active = d.Active,
                        CreatedDate = d.CreatedDate,
                        CreatedById = d.CreatedById,
                        UpdatedDate = d.UpdatedDate,
                        UpdatedById = d.UpdatedById,
                        TenantId = d.TenantId,
                    }).ToList();

                #endregion

                #region Lấy hợp đồng

                var lstEmpId = listEmployee.Select(em => em.EmployeeId).ToList();
                var listContract = context.Contract.Where(ctr =>
                        ctr.Active == true && ctr.EmployeeId != null && lstEmpId.Contains(ctr.EmployeeId.Value))
                    .Select(y => new ContractEntityModel
                    {
                        ContractId = y.ContractId,
                        QuoteId = y.QuoteId,
                        CustomerId = y.CustomerId,
                        ContractCode = y.ContractCode,
                        ContractTypeId = y.ContractTypeId,
                        EmployeeId = y.EmployeeId,
                        MainContractId = y.MainContractId,
                        ContractNote = y.ContractNote,
                        ContractDescription = y.ContractDescription,
                        ValueContract = y.ValueContract,
                        PaymentMethodId = y.PaymentMethodId,
                        BankAccountId = y.BankAccountId,
                        EffectiveDate = y.EffectiveDate,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        TenantId = y.TenantId,
                        DiscountType = y.DiscountType,
                        DiscountValue = y.DiscountValue,
                        Amount = y.Amount,
                        StatusId = y.StatusId,
                        ListDetail = null,
                    }).ToList();

                var contractDetail = context.ContractDetail.ToList();
                var contractDetailProductAttribute = context.ContractDetailProductAttribute.ToList();

                listContract.ForEach(item =>
                {
                    var detail = contractDetail.Where(d => d.ContractId == item.ContractId)
                        .Select(d => new ContractDetailEntityModel
                        {
                            ContractDetailId = d.ContractDetailId,
                            ContractId = d.ContractId,
                            VendorId = d.VendorId,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            QuantityOdered = d.QuantityOdered,
                            UnitPrice = d.UnitPrice,
                            CurrencyUnit = d.CurrencyUnit,
                            ExchangeRate = d.ExchangeRate,
                            Tax = d.Tax,
                            Vat = d.Tax,
                            GuaranteeTime = d.GuaranteeTime,
                            DiscountType = d.DiscountType,
                            DiscountValue = d.DiscountValue,
                            Description = d.Description,
                            OrderDetailType = d.OrderDetailType,
                            UnitId = d.UnitId,
                            IncurredUnit = d.IncurredUnit,
                            CostsQuoteType = d.CostsQuoteType,
                            CreatedById = d.CreatedById,
                            CreatedDate = d.CreatedDate,
                            UpdatedById = d.UpdatedById,
                            UpdatedDate = d.UpdatedDate,
                            NameVendor = listAllVendor.FirstOrDefault(f => f.VendorId == d.VendorId) == null
                                ? null
                                : listAllVendor.FirstOrDefault(f => f.VendorId == d.VendorId).VendorName,
                            NameMoneyUnit = listCategory.FirstOrDefault(f => f.CategoryId == d.CurrencyUnit) == null
                                ? null
                                : listCategory.FirstOrDefault(f => f.CategoryId == d.CurrencyUnit).CategoryName,
                            NameProductUnit = listCategory.FirstOrDefault(f => f.CategoryId == d.UnitId) == null
                                ? null
                                : listCategory.FirstOrDefault(f => f.CategoryId == d.UnitId).CategoryName,
                            NameProduct = listAllProduct.FirstOrDefault(f => f.ProductId == d.ProductId) == null
                                ? null
                                : listAllProduct.FirstOrDefault(f => f.ProductId == d.ProductId).ProductName,
                            ContractProductDetailProductAttributeValue = null,
                            OrderNumber = d.OrderNumber
                        }).OrderBy(z => z.OrderNumber).ToList();

                    detail.ForEach(dt =>
                    {
                        var productAtt = contractDetailProductAttribute.Where(at => at.ContractDetailId == dt.ContractDetailId)
                        .Select(at => new ContractDetailProductAttributeEntityModel
                        {
                            ContractDetailProductAttributeId = at.ContractDetailProductAttributeId,
                            ContractDetailId = at.ContractDetailId,
                            ProductId = at.ProductId,
                            ProductAttributeCategoryId = at.ProductAttributeCategoryId,
                            ProductAttributeCategoryValueId = at.ProductAttributeCategoryValueId
                        }).ToList();
                        dt.ContractProductDetailProductAttributeValue = productAtt;
                    });

                    item.ListDetail = detail;
                });

                #endregion

                return new GetMasterDataOrderCreateResult()
                {
                    Status = true,
                    Message = "Success",
                    ListOrderStatus = listOrderStatus,
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListCustomerBankAccount = listCustomerBankAccount,
                    ListCustomerGroup = listCustomerGroup,
                    ListPaymentMethod = listPaymentMethod,
                    ListCustomerCode = listCustomerCode,
                    ListQuote = listQuote,
                    ListWare = listWare,
                    ListProduct = listAllProduct,
                    ListContract = listContract
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderCreateResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataOrderDetailDialogResult GetMasterDataOrderDetailDialog(GetMasterDataOrderDetailDialogParameter parameter)
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

                var listProduct = context.Product.Where(x => x.Active == true)
                    .Select(x => new ProductEntityModel
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

                var listWareHouse = context.Warehouse.Where(v => v.Active == true && v.WarehouseParent == null)
                    .Select(v => new WareHouseEntityModel
                    {
                        WarehouseId = v.WarehouseId,
                        WarehouseCode = v.WarehouseCode,
                        WarehouseName = v.WarehouseName,
                        WarehouseParent = v.WarehouseParent,
                        WarehouseAddress = v.WarehouseAddress,
                        WarehousePhone = v.WarehousePhone,
                        Storagekeeper = v.Storagekeeper,
                        WarehouseDescription = v.WarehouseDescription,
                        Active = v.Active,
                        WarehouseCodeName = v.WarehouseCode + " - " + v.WarehouseName,
                    }).ToList();

                return new GetMasterDataOrderDetailDialogResult()
                {
                    Status = true,
                    Message = "Success",
                    ListUnitMoney = listUnitMoney,
                    ListUnitProduct = listUintProduct,
                    ListProduct = listProduct,
                    ListWareHouse = listWareHouse
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderDetailDialogResult()
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
                        .Where(x => x.Active && x.EffectiveDate.Date <= parameter.OrderDate.Date &&
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
                    PriceProduct = priceProduct,
                    ListVendor = listVendor,
                    ListObjectAttributeValueProduct = listObjectAttributeValueProduct,
                    ListObjectAttributeNameProduct = listObjectAttributeNameProduct
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

        public GetMasterDataOrderDetailResult GetMasterDataOrderDetail(GetMasterDataOrderDetailParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listOrderStatus = new List<OrderStatus>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listCustomerBankAccount = new List<BankAccountEntityModel>();
                var listCustomerGroup = new List<CategoryEntityModel>();
                var listPaymentMethod = new List<CategoryEntityModel>();
                var listCustomerCode = new List<string>();
                var listQuote = new List<QuoteEntityModel>();
                var listWare = new List<WareHouseEntityModel>();

                var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listAllUser = context.User.Where(x => x.Active == true).ToList();
                var listAllCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "CUS").ToList();
                var listCategoryType = context.CategoryType.Where(x => x.Active == true).ToList();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();
                var listAllCost = context.Cost.Where(x => x.Active == true).ToList();
                var listAllQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listAllQuoteDetailAttribute = context.QuoteProductDetailProductAttributeValue.ToList();
                var listAllVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();
                var statusCodeQuote = listCategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "TGI");
                var statusQuoteBG = listCategory.FirstOrDefault(ca => ca.CategoryTypeId == statusCodeQuote.CategoryTypeId && ca.CategoryCode == "DTH");
                var listAllQuote = context.Quote.Where(x => x.Active == true && x.StatusId == statusQuoteBG.CategoryId).ToList();
                var listAllQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();

                var eployeeUser = new List<EmployeeModel>();
                eployeeUser = listAllEmployee.Select(item => new EmployeeModel
                {
                    EmployeeId = item.EmployeeId,
                    EmployeeName = item.EmployeeName,
                    UserId = listAllUser.FirstOrDefault(u => u.EmployeeId == item.EmployeeId).UserId
                }).ToList();

                #region Lấy List Order Status

                listOrderStatus = context.OrderStatus.Where(x => x.Active).OrderBy(z => z.Description).ToList();

                #endregion

                #region Lấy List nhân viên bán hàng

                if (employee.IsManager)
                {
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }

                    //Lấy danh sách nhân viên Employyee mà user phụ trách
                    listEmployee =
                        listAllEmployee.Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(y => new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeCode = y.EmployeeCode,
                            EmployeeName = y.EmployeeName,
                            EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                            OrganizationId = y.OrganizationId,
                            IsManager = y.IsManager
                        }).ToList();
                }
                else
                {
                    listEmployee = listAllEmployee.Where(x => x.EmployeeId == employee.EmployeeId).Select(y =>
                        new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeCode = y.EmployeeCode,
                            EmployeeName = y.EmployeeName,
                            EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                            OrganizationId = y.OrganizationId,
                            IsManager = y.IsManager
                        }).ToList();
                }

                #endregion

                #region Lấy List Customer

                var listEmployeeId = listEmployee.Select(x => x.EmployeeId).ToList();

                if (listEmployeeId.Count > 0)
                {
                    var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId))
                        .Select(y => y.UserId);
                    listCustomer = listAllCustomer.Where(x =>
                        listEmployeeId.Contains(x.PersonInChargeId) ||
                        (x.PersonInChargeId == null && listUserId.Contains(x.CreatedById))).Select(
                        y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            PaymentId = y.PaymentId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue,
                            CustomerCodeName = y.CustomerCode + " - " + y.CustomerName,
                            PersonInChargeId = y.PersonInChargeId,
                        }).ToList();

                    var listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();

                    if (listCustomerId.Count > 0)
                    {
                        var listCustomerContact =
                            listAllContact.Where(x => listCustomerId.Contains(x.ObjectId)).ToList();

                        listCustomer.ForEach(item =>
                        {
                            var contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            var email = contact == null ? "" : (contact.Email == null ? "" : contact.Email.Trim());
                            var phone = contact == null ? "" : (contact.Phone == null ? "" : contact.Phone.Trim());
                            var taxCode = contact == null ? "" : (contact.TaxCode == null ? "" : contact.TaxCode.Trim());

                            item.CustomerEmail = email;
                            item.CustomerPhone = phone;
                            item.TaxCode = taxCode;
                        });
                    }
                }

                #endregion

                #region Lấy List Tài khoản thanh toán của tất cả khách hàng

                listCustomerBankAccount = context.BankAccount.Where(x => x.Active == true && x.ObjectType == "CUS")
                    .Select(y => new BankAccountEntityModel
                    {
                        BankAccountId = y.BankAccountId,
                        ObjectId = y.ObjectId,
                        AccountNumber = (y.BankName ?? "") + " - " + (y.AccountNumber ?? "")
                    }).ToList();

                #endregion

                #region Lấy List Nhóm khách hàng

                var categoryType1 = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA")
                    .CategoryTypeId;
                listCustomerGroup = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryType1)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Phương thức thanh toán

                var categoryType2 = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PTO")
                    .CategoryTypeId;
                listPaymentMethod = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryType2)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Customer Code

                listCustomerCode = listAllCustomer.Where(x => (x.CustomerCode ?? "").Trim() != "")
                    .Select(y => y.CustomerCode.ToLower()).ToList();

                #endregion

                #region Lấy đơn hàng

                var customerOrderObject = (from or in context.CustomerOrder
                                           where or.OrderId == parameter.OrderId
                                           select new CustomerOrderEntityModel
                                           {
                                               OrderId = or.OrderId,
                                               BankAccountId = or.BankAccountId,
                                               OrderCode = or.OrderCode,
                                               OrderDate = or.OrderDate,
                                               Description = or.Description,
                                               Note = or.Note,
                                               CustomerId = or.CustomerId,
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
                                               ReceiptInvoiceAmount = or.ReceiptInvoiceAmount,
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
                                               ReasonCancel = or.ReasonCancel,
                                               CustomerName = or.CustomerName,
                                               CustomerAddress = or.CustomerAddress,
                                               OrderContractId = or.OrderContractId,
                                               WarehouseId = or.WarehouseId,
                                               QuoteId = or.QuoteId,
                                               IsAutoGenReceiveInfor = or.IsAutoGenReceiveInfor ?? false
                                           }).FirstOrDefault();

                var statusList = context.OrderStatus.ToList();
                customerOrderObject.StatusCode =
                    statusList.FirstOrDefault(x => x.OrderStatusId == customerOrderObject.StatusId) != null
                        ? statusList.FirstOrDefault(x => x.OrderStatusId == customerOrderObject.StatusId)
                            .OrderStatusCode
                        : "";

                if (customerOrderObject.CustomerId == getIDKHL001())
                {
                    customerOrderObject.TypeAccount = 1;
                }
                else
                {
                    customerOrderObject.TypeAccount = 2;
                }

                #endregion

                #region Kiểm tra xem khách hàng của đơn hàng có thuộc quyền phụ trách của nhân viên đang đăng nhập hay không

                var checkCustomer = listCustomer.FirstOrDefault(x => x.CustomerId == customerOrderObject.CustomerId);

                if (checkCustomer == null)
                {
                    var newCustomer = listAllCustomer.Where(x => x.CustomerId == customerOrderObject.CustomerId).Select(
                        y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerEmail = "",
                            CustomerPhone = "",
                            FullAddress = "",
                            PaymentId = y.PaymentId,
                            MaximumDebtDays = y.MaximumDebtDays,
                            MaximumDebtValue = y.MaximumDebtValue
                        }).FirstOrDefault();

                    var newCustomerContact = listAllContact.FirstOrDefault(x => x.ObjectId == newCustomer.CustomerId);
                    var newEmail = newCustomerContact == null ? "" : (newCustomerContact.Email == null ? "" : newCustomerContact.Email.Trim());
                    var newPhone = newCustomerContact == null ? "" : (newCustomerContact.Phone == null ? "" : newCustomerContact.Phone.Trim());

                    newCustomer.CustomerEmail = newEmail;
                    newCustomer.CustomerPhone = newPhone;

                    //Thêm vào listCustomer
                    listCustomer.Add(newCustomer);
                }

                #endregion

                #region Lấy Chi tiết đơn hàng (Sản phẩm dịch vụ)

                var listCustomerOrderObjectType0 = (from cod in context.CustomerOrderDetail
                                                    where cod.OrderId == parameter.OrderId && cod.OrderDetailType == 0
                                                    select (new CustomerOrderDetailEntityModel
                                                    {
                                                        Active = cod.Active,
                                                        CreatedById = cod.CreatedById,
                                                        OrderId = cod.OrderId,
                                                        VendorId = cod.VendorId,
                                                        CreatedDate = cod.CreatedDate,
                                                        CurrencyUnit = cod.CurrencyUnit,
                                                        Description = cod.Description,
                                                        DiscountType = cod.DiscountType,
                                                        DiscountValue = cod.DiscountValue,
                                                        ExchangeRate = cod.ExchangeRate,
                                                        OrderDetailId = cod.OrderDetailId,
                                                        OrderDetailType = cod.OrderDetailType,
                                                        ProductId = cod.ProductId.Value,
                                                        UpdatedById = cod.UpdatedById,
                                                        Quantity = cod.Quantity,
                                                        UnitId = cod.UnitId,
                                                        IncurredUnit = cod.IncurredUnit,
                                                        UnitPrice = cod.UnitPrice,
                                                        UpdatedDate = cod.UpdatedDate,
                                                        GuaranteeTime = cod.GuaranteeTime,
                                                        GuaranteeDatetime = cod.GuaranteeDatetime != null
                                                            ? cod.GuaranteeDatetime
                                                            : customerOrderObject.ReceivedDate.Value.AddMonths(cod.GuaranteeTime.Value),
                                                        ExpirationDate = cod.ExpirationDate,
                                                        Vat = cod.Vat,
                                                        NameVendor = "",
                                                        NameProduct = "",
                                                        NameProductUnit = "",
                                                        NameMoneyUnit = "",
                                                        WarehouseId = cod.WarehouseId,
                                                        WarrantyPeriod = cod.WarrantyPeriod,
                                                        PriceInitial = cod.PriceInitial,
                                                        IsPriceInitial = cod.IsPriceInitial,
                                                        ActualInventory = cod.ActualInventory,
                                                        BusinessInventory = cod.BusinessInventory,
                                                        ProductName = cod.ProductName,
                                                        SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.Vat, cod.DiscountValue, cod.DiscountType,
                                                            cod.ExchangeRate),
                                                        OrderNumber = cod.OrderNumber
                                                    })).ToList();

                if (listCustomerOrderObjectType0.Count > 0)
                {
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listCurrencyUnitId = new List<Guid>();
                    List<Guid> listUnitId = new List<Guid>();

                    listCustomerOrderObjectType0.ForEach(item =>
                    {
                        if (item.ProductId != null)
                            listProductId.Add(item.ProductId.Value);
                        if (item.VendorId != null)
                            listVendorId.Add(item.VendorId.Value);
                        if (item.CurrencyUnit != null)
                            listCurrencyUnitId.Add(item.CurrencyUnit.Value);
                        if (item.UnitId != null)
                            listUnitId.Add(item.UnitId.Value);
                    });

                    var listProduct = context.Product.Where(e => listProductId.Contains(e.ProductId)).ToList();
                    var listVendor = context.Vendor.Where(e => listVendorId.Contains(e.VendorId)).ToList();
                    var listCurrencyUnit =
                        context.Category.Where(e => listCurrencyUnitId.Contains(e.CategoryId)).ToList();
                    var listUnit = context.Category.Where(e => listUnitId.Contains(e.CategoryId)).ToList();

                    listCustomerOrderObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null)
                            item.NameVendor = listVendor.FirstOrDefault(e => e.VendorId == item.VendorId).VendorName;
                        if (item.ProductId != null)
                            item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductName;
                        if (item.CurrencyUnit != null)
                            item.NameProductUnit =
                                listUnit.FirstOrDefault(e => e.CategoryId == item.UnitId).CategoryName;
                        if (item.UnitId != null)
                            item.NameMoneyUnit = listCurrencyUnit.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });

                    listCustomerOrderObjectType0.ForEach(item =>
                    {
                        item.NameGene = item.NameProduct + "(" + getNameGEn(item.OrderDetailId) + ")";
                        item.OrderProductDetailProductAttributeValue =
                            getListOrderProductDetailProductAttributeValue(item.OrderDetailId);
                    });
                }

                #endregion

                #region Lấy Chi tiết đơn hàng (Chi phí khác)

                var listCustomerOrderObjectType1 = (from cod in context.CustomerOrderDetail
                                                    where cod.OrderId == parameter.OrderId && cod.OrderDetailType == 1
                                                    select (new CustomerOrderDetailEntityModel
                                                    {
                                                        Active = cod.Active,
                                                        CreatedById = cod.CreatedById,
                                                        OrderId = cod.OrderId,
                                                        VendorId = cod.VendorId,
                                                        CreatedDate = cod.CreatedDate,
                                                        CurrencyUnit = cod.CurrencyUnit,
                                                        Description = cod.Description,
                                                        DiscountType = cod.DiscountType,
                                                        DiscountValue = cod.DiscountValue,
                                                        ExchangeRate = cod.ExchangeRate,
                                                        OrderDetailId = cod.OrderDetailId,
                                                        OrderDetailType = cod.OrderDetailType,
                                                        ProductId = cod.ProductId.Value,
                                                        UpdatedById = cod.UpdatedById,
                                                        Quantity = cod.Quantity,
                                                        UnitId = cod.UnitId,
                                                        IncurredUnit = cod.IncurredUnit,
                                                        UnitPrice = cod.UnitPrice,
                                                        UpdatedDate = cod.UpdatedDate,
                                                        GuaranteeTime = cod.GuaranteeTime,
                                                        GuaranteeDatetime = cod.GuaranteeDatetime != null
                                                            ? cod.GuaranteeDatetime
                                                            : customerOrderObject.ReceivedDate.Value.AddMonths(cod.GuaranteeTime.Value),
                                                        ExpirationDate = cod.ExpirationDate,
                                                        Vat = cod.Vat,
                                                        ProductName = cod.ProductName,
                                                        NameVendor = "",
                                                        NameProduct = "",
                                                        NameProductUnit = "",
                                                        NameMoneyUnit = "",
                                                        SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.Vat, cod.DiscountValue, cod.DiscountType,
                                                            cod.ExchangeRate),
                                                        OrderNumber = cod.OrderNumber
                                                    })).ToList();

                if (listCustomerOrderObjectType1.Count > 0)
                {
                    List<Guid> listCurrencyUnitId = new List<Guid>();

                    listCustomerOrderObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null)
                            listCurrencyUnitId.Add(item.CurrencyUnit.Value);
                    });

                    var listCurrencyUnit =
                        context.Category.Where(e => listCurrencyUnitId.Contains(e.CategoryId)).ToList();

                    listCustomerOrderObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null)
                            item.NameMoneyUnit = listCurrencyUnit.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });
                }

                #endregion

                #region Ghép 2 loại Chi tiết đơn hàng (Sản phẩm dịch vụ và Chi phí khác) với nhau

                listCustomerOrderObjectType0.AddRange(listCustomerOrderObjectType1);
                listCustomerOrderObjectType0 = listCustomerOrderObjectType0.OrderBy(z => z.OrderNumber).ToList();

                #endregion

                #region Lấy list ghi chú

                var listNote = new List<NoteEntityModel>();

                listNote = context.Note
                    .Where(x => x.ObjectId == parameter.OrderId && x.ObjectType == "ORDER" && x.Active == true).Select(
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
                        }).ToList();

                    listNote.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                            .OrderByDescending(z => z.UpdatedDate).ToList();
                    });

                    //Sắp xếp lại listNote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                #region Lấy List Quote

                List<Guid> listEmpId = listAllEmployee.Select(x => x.EmployeeId).ToList();
                listQuote = listAllQuote
                    .Where(x => x.Seller != null && listEmpId.Contains(Guid.Parse(x.Seller.ToString())))
                    .Select(y => new QuoteEntityModel
                    {
                        QuoteId = y.QuoteId,
                        QuoteCode = y.QuoteCode,
                        QuoteName = y.QuoteName,
                        Seller = y.Seller,
                        ObjectTypeId = y.ObjectTypeId,
                        QuoteCodeName = y.QuoteCode + "" + (y.QuoteName == null ? "" : " - " + y.QuoteName),
                    }).ToList();

                listQuote.ForEach(item =>
                {
                    item.ListDetail = listAllQuoteDetail.Where(d => d.QuoteId == item.QuoteId)
                        .Select(d => new QuoteDetailEntityModel
                        {
                            QuoteDetailId = d.QuoteDetailId,
                            VendorId = d.VendorId,
                            QuoteId = d.QuoteId,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            CurrencyUnit = d.CurrencyUnit,
                            ExchangeRate = d.ExchangeRate,
                            Vat = d.Vat,
                            DiscountType = d.DiscountType,
                            DiscountValue = d.DiscountValue,
                            Description = d.Description,
                            OrderDetailType = d.OrderDetailType,
                            UnitId = d.UnitId,
                            IncurredUnit = d.IncurredUnit,
                            Active = d.Active,
                            CreatedById = d.CreatedById,
                            CreatedDate = d.CreatedDate,
                            UpdatedById = d.UpdatedById,
                            UpdatedDate = d.UpdatedDate,
                            NameVendor = listAllVendor.FirstOrDefault(f => f.VendorId == d.VendorId) == null
                                ? null
                                : listAllVendor.FirstOrDefault(f => f.VendorId == d.VendorId).VendorName,
                            NameMoneyUnit = listCategory.FirstOrDefault(f => f.CategoryId == d.CurrencyUnit) == null
                                ? null
                                : listCategory.FirstOrDefault(f => f.CategoryId == d.CurrencyUnit).CategoryName,
                            NameProductUnit = listCategory.FirstOrDefault(f => f.CategoryId == d.UnitId) == null
                                ? null
                                : listCategory.FirstOrDefault(f => f.CategoryId == d.UnitId).CategoryName,
                            NameProduct = listAllProduct.FirstOrDefault(f => f.ProductId == d.ProductId) == null
                                ? null
                                : listAllProduct.FirstOrDefault(f => f.ProductId == d.ProductId).ProductName,
                            PriceInitial = d.PriceInitial,
                            IsPriceInitial = d.IsPriceInitial,
                            OrderNumber = d.OrderNumber
                        }).OrderBy(z => z.OrderNumber).ToList();

                    item.ListCostDetail = listAllQuoteCostDetail.Where(d => d.QuoteId == item.QuoteId).Select(d =>
                        new QuoteCostDetailEntityModel
                        {
                            QuoteCostDetailId = d.QuoteCostDetailId,
                            CostId = d.CostId,
                            QuoteId = d.QuoteId,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            CostName = d.CostName,
                            CostCode = listAllCost.FirstOrDefault(c => c.CostId == d.CostId) == null
                                ? null
                                : listAllCost.FirstOrDefault(c => c.CostId == d.CostId).CostName,
                            Active = d.Active,
                            CreatedById = d.CreatedById,
                            CreatedDate = d.CreatedDate,
                            UpdatedById = d.UpdatedById,
                            UpdatedDate = d.UpdatedDate,
                        }).ToList();

                    item.ListDetail.ForEach(detail =>
                    {
                        detail.QuoteProductDetailProductAttributeValue = listAllQuoteDetailAttribute
                            .Where(dt => dt.QuoteDetailId == detail.QuoteDetailId).Select(dt =>
                                new QuoteProductDetailProductAttributeValueEntityModel
                                {
                                    QuoteDetailId = dt.QuoteDetailId,
                                    ProductId = dt.ProductId,
                                    ProductAttributeCategoryId = dt.ProductAttributeCategoryId,
                                    ProductAttributeCategoryValueId = dt.ProductAttributeCategoryValueId,
                                    QuoteProductDetailProductAttributeValueId =
                                        dt.QuoteProductDetailProductAttributeValueId
                                }).ToList();
                    });
                });

                #endregion

                #region Lấy List Warehouse
                listWare = context.Warehouse.Where(d => d.WarehouseParent == null)
                    .Select(d => new WareHouseEntityModel
                    {
                        WarehouseId = d.WarehouseId,
                        WarehouseCode = d.WarehouseCode + " - " + d.WarehouseName,
                        WarehouseName = d.WarehouseName,
                        WarehouseParent = d.WarehouseParent,
                        WarehouseAddress = d.WarehouseAddress,
                        WarehousePhone = d.WarehousePhone,
                        Storagekeeper = d.Storagekeeper,
                        WarehouseDescription = d.WarehouseDescription,
                        Active = d.Active,
                        CreatedDate = d.CreatedDate,
                        CreatedById = d.CreatedById,
                        UpdatedDate = d.UpdatedDate,
                        UpdatedById = d.UpdatedById,
                        TenantId = d.TenantId,
                    }).ToList();

                #endregion

                #region Lấy OrderCostDetail
                var listCostDetail = context.OrderCostDetail.Where(o => o.OrderId == parameter.OrderId).Select(o => new OrderCostDetailEntityModel
                {
                    OrderCostDetailId = o.OrderCostDetailId,
                    CostId = o.CostId,
                    OrderId = o.OrderId,
                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPrice,
                    CostName = listAllCost.FirstOrDefault(c => c.CostId == o.CostId) == null ? null : listAllCost.FirstOrDefault(c => c.CostId == o.CostId).CostName,
                    CostCode = listAllCost.FirstOrDefault(c => c.CostId == o.CostId) == null ? null : listAllCost.FirstOrDefault(c => c.CostId == o.CostId).CostCode,
                    Active = o.Active,
                    CreatedById = o.CreatedById,
                    CreatedDate = o.CreatedDate,
                    UpdatedById = o.UpdatedById,
                    UpdatedDate = o.UpdatedDate,
                }).ToList();
                #endregion

                #region Lấy thông tin giao hàng
                var listInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.Where(l => l.ObjectId == parameter.OrderId)
                    .Select(l => new InventoryDeliveryVoucherEntityModel
                    {
                        InventoryDeliveryVoucherId = l.InventoryDeliveryVoucherId,
                        InventoryDeliveryVoucherCode = l.InventoryDeliveryVoucherCode,
                        StatusId = l.StatusId,
                        InventoryDeliveryVoucherType = l.InventoryDeliveryVoucherType,
                        WarehouseId = l.WarehouseId,
                        ObjectId = l.ObjectId,
                        Receiver = l.Receiver,
                        Reason = l.Reason,
                        InventoryDeliveryVoucherDate = l.InventoryDeliveryVoucherDate,
                        InventoryDeliveryVoucherTime = l.InventoryDeliveryVoucherTime,
                        LicenseNumber = l.LicenseNumber,
                        Active = l.Active,
                        CreatedDate = l.CreatedDate,
                        CreatedById = l.CreatedById,
                        UpdatedDate = l.UpdatedDate,
                        UpdatedById = l.UpdatedById,
                        NameCreate = eployeeUser.FirstOrDefault(em => em.UserId == l.CreatedById).EmployeeName,
                        NameStatus = listCategory.FirstOrDefault(ca => ca.CategoryId == l.StatusId).CategoryName,
                    }).ToList();
                #endregion

                #region Lấy hợp đồng
                var lstEmpId = listEmployee.Select(em => em.EmployeeId).ToList();
                var listContract = context.Contract.Where(ctr => ctr.Active == true && ctr.EmployeeId != null && lstEmpId.Contains(ctr.EmployeeId.Value)).ToList();
                #endregion

                #region Lấy hóa đơn
                var categoryTypeBill = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "BILL" && ct.Active == true);
                var listStatus = context.Category.Where(c => c.CategoryTypeId == categoryTypeBill.CategoryTypeId && c.Active == true).ToList();
                var lstBill = context.BillOfSale.Where(b => b.Active == true && b.OrderId == parameter.OrderId)
                    .Select(x => new BillSaleEntityModel
                    {
                        BillOfSaLeId = x.BillOfSaLeId,
                        BillOfSaLeCode = x.BillOfSaLeCode,
                        OrderId = x.OrderId,
                        BillDate = x.BillDate,
                        EndDate = x.EndDate,
                        StatusId = x.StatusId,
                        TermsOfPaymentId = x.TermsOfPaymentId,
                        CustomerId = x.CustomerId,
                        CustomerName = x.CustomerName,
                        DebtAccountId = x.DebtAccountId,
                        Mst = x.Mst,
                        PaymentMethodId = x.PaymentMethodId,
                        EmployeeId = x.EmployeeId,
                        Description = x.Description,
                        Note = x.Note,
                        AccountBankId = x.AccountBankId,
                        Active = x.Active,
                        CreatedById = x.CreatedById,
                        CreatedDate = x.CreatedDate,
                        UpdatedById = x.UpdatedById,
                        UpdatedDate = x.UpdatedDate,
                        InvoiceSymbol = x.InvoiceSymbol,
                        Amount = 0,
                        StatusName = (x.StatusId == null || x.StatusId == Guid.Empty) ? "" : listStatus.FirstOrDefault(c => c.CategoryId == x.StatusId.Value) == null ? "" : listStatus.FirstOrDefault(c => c.CategoryId == x.StatusId.Value).CategoryName,
                        Seller = (x.EmployeeId == null || x.EmployeeId == Guid.Empty) ? "" : listAllEmployee.FirstOrDefault(c => c.EmployeeId == x.EmployeeId.Value) == null ? "" : listAllEmployee.FirstOrDefault(c => c.EmployeeId == x.EmployeeId.Value).EmployeeName,
                    }).OrderByDescending(x => x.BillDate).ToList();

                var billDetail = context.BillOfSaleDetail.Where(d => d.Active == true).ToList();
                lstBill.ForEach(item =>
                {
                    var detail = billDetail.Where(d => d.Active == true && d.BillOfSaleId == item.BillOfSaLeId).ToList();
                    item.Amount = 0;
                    detail.ForEach(de =>
                    {
                        var quantity = de.Quantity == null ? 0 : de.Quantity.Value;
                        var unitPrice = de.UnitPrice == null ? 0 : de.UnitPrice.Value;
                        var exchangeRate = de.ExchangeRate == null ? 0 : de.ExchangeRate.Value;
                        var discountValue = de.DiscountValue == null ? 0 : de.DiscountValue.Value;
                        var vat = de.Vat == null ? 0 : de.Vat.Value;
                        var moneyRecord = quantity * unitPrice * exchangeRate;
                        var discountMoney = de.DiscountType == true ? (moneyRecord * discountValue / 100) : discountValue;
                        var vatMoney = (moneyRecord - discountMoney) * vat / 100;
                        item.Amount = item.Amount + (moneyRecord - discountMoney + vatMoney);
                    });
                });

                #endregion

                return new GetMasterDataOrderDetailResult()
                {
                    Status = true,
                    Message = "Success",
                    ListOrderStatus = listOrderStatus,
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListCustomerBankAccount = listCustomerBankAccount,
                    ListCustomerGroup = listCustomerGroup,
                    ListPaymentMethod = listPaymentMethod,
                    ListCustomerCode = listCustomerCode,
                    CustomerOrderObject = customerOrderObject,
                    ListCustomerOrderDetail = listCustomerOrderObjectType0,
                    ListNote = listNote,
                    ListQuote = listQuote,
                    ListWare = listWare,
                    ListInventoryDeliveryVoucher = listInventoryDeliveryVoucher,
                    ListProduct = listAllProduct,
                    ListCustomerOrderCostDetail = listCostDetail,
                    IsManager = employee.IsManager,
                    ListContract = listContract,
                    ListBillSale = lstBill
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderDetailResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DeleteOrderResult DeleteOrder(DeleteOrderParameter parameter)
        {
            try
            {
                var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);

                if (order == null)
                {
                    return new DeleteOrderResult()
                    {
                        Status = false,
                        Message = "Không tồn tại đơn hàng này trên hệ thống"
                    };
                }

                #region Kiểm tra đơn hàng có phải trạng thái Nháp không

                var status = context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == order.StatusId);

                if (status.OrderStatusCode != "DRA")
                {
                    var description = status.Description;
                    return new DeleteOrderResult()
                    {
                        Status = false,
                        Message = "Không thể xóa đơn hàng có trạng thái " + description
                    };
                }

                #endregion

                #region Xóa NoteDocument và Note của đơn hàng

                var listNote = context.Note.Where(x => x.ObjectId == parameter.OrderId && x.ObjectType == "DH")
                    .ToList();

                var listNoteId = listNote.Select(x => x.NoteId).ToList();
                if (listNoteId.Count > 0)
                {
                    var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).ToList();
                    context.NoteDocument.RemoveRange(listNoteDocument);
                }

                context.Note.RemoveRange(listNote);

                #endregion

                #region Xóa đơn hàng và các reference của nó

                var listOrderDetail = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.OrderId).ToList();

                if (listOrderDetail.Count > 0)
                {
                    var listOrderDetailId = listOrderDetail.Select(x => x.OrderDetailId).ToList();
                    var listOrderProductDetailProductAttributeValue = context.OrderProductDetailProductAttributeValue
                        .Where(x => listOrderDetailId.Contains(x.OrderDetailId)).ToList();

                    context.OrderProductDetailProductAttributeValue.RemoveRange(
                        listOrderProductDetailProductAttributeValue);

                    context.CustomerOrderDetail.RemoveRange(listOrderDetail);
                }

                var listOrderCostDetail = context.OrderCostDetail.Where(x => x.OrderId == parameter.OrderId).ToList();
                context.OrderCostDetail.RemoveRange(listOrderCostDetail);

                var listOrderLocalPointMapping = context.CustomerOrderLocalPointMapping
                    .Where(x => x.OrderId == parameter.OrderId).ToList();
                context.CustomerOrderLocalPointMapping.RemoveRange(listOrderLocalPointMapping);

                //Cập nhật lại trạng thái các Điểm gắn với Đơn hàng thành Sẵn sàng
                var listLocalPointId = listOrderLocalPointMapping.Select(y => y.LocalPointId).ToList();
                var listLocalPoint = context.LocalPoint.Where(x => listLocalPointId.Contains(x.LocalPointId)).ToList();
                listLocalPoint.ForEach(item => { item.StatusId = 1; });
                context.LocalPoint.UpdateRange(listLocalPoint);

                context.CustomerOrder.Remove(order);
                context.SaveChanges();

                #endregion

                return new DeleteOrderResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DeleteOrderResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
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

        public GetDataDashboardHomeResult GetDataDashboardHome(GetDataDashboardHomeParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listCommonOrganization = context.Organization.ToList();
                var listCommonPosition = context.Position.ToList();
                var listCommonCustomer = context.Customer.ToList();
                var listCommonContact = context.Contact.ToList();
                var listCommonLead = context.Lead.ToList();
                var listCommonUser = context.User.ToList();
                var listCommonEmployee = context.Employee.ToList();
                var listCommonOrder = context.CustomerOrder.ToList();
                var listCommonOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                //var listOrderCode = new List<string> { "IP", "PD", "DLV", "COMP" };
                var listOrderCode = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "OrderStatus")
                    .SystemValueString.Split(';').ToList();
                var listOrderStatusId = listCommonOrderStatus.Where(x => listOrderCode.Contains(x.OrderStatusCode))
                    .Select(y => y.OrderStatusId);

                DateTime firstDateOfWeek = FirstDateOfWeek();
                DateTime lastDateOfWeek = LastDateOfWeek();
                DateTime firstDateOfMonth = FirstDateOfMonth();
                DateTime lastDateOfMonth = LastDateOfMonth();
                var dateOfQuarter = GetDateOfQuarter(DateTime.Now);
                var dateOfQuarterPress = GetDateOfQuarter(DateTime.Now.AddMonths(-4));

                decimal totalSalesOfWeek = 0;   //Tổng doanh số của các đơn hàng trong tuần
                decimal totalSalesOfMonth = 0;   //Tổng doanh số của các đơn hàng trong tháng
                decimal totalSalesOfQuarter = 0;   //Tổng doanh số của các đơn hàng trong Qúy

                decimal totalSalesOfWeekPress = 0;   //Tổng doanh số của các đơn hàng trong tuần trước
                decimal totalSalesOfMonthPress = 0;   //Tổng doanh số của các đơn hàng trong tháng trước
                decimal totalSalesOfQuarterPress = 0;   //Tổng doanh số của các đơn hàng trong Qúy trước

                var listQuote = new List<QuoteEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listOrderNew = new List<CustomerOrderEntityModel>();
                var listCustomerMeeting = new List<CustomerMeetingEntityModel>();
                var listLeadMeeting = new List<LeadMeetingEntityModel>();
                var listCusBirthdayOfWeek_1 = new List<CustomerEntityModel>();
                var listEmployeeBirthDayOfWeek = new List<EmployeeEntityModel>();    

                // Lịch hẹn khách hàng
                listCustomerMeeting = context.CustomerMeeting
                    .Where(x => x.Participants.Contains(employee.EmployeeId.ToString())
                                && x.Active == true
                                ).Select(y =>
                        new CustomerMeetingEntityModel
                        {
                            CustomerMeetingId = y.CustomerMeetingId,
                            CustomerId = y.CustomerId,
                            EmployeeId = y.EmployeeId,
                            StartDate = y.StartDate,
                            EndDate = y.EndDate,
                            Title = y.Title,
                            Content = y.Content,
                            LocationMeeting = y.LocationMeeting,
                            CreatedById = y.CreatedById,
                            CustomerName = "",
                            EmployeeName = "",
                            CreateByName = "",
                            IsShowLink = false,
                            Participants = y.Participants
                        }).OrderByDescending(z => z.StartDate).ToList();

                // Lịch hẹn cơ hội
                //  c.StartDate >= firstDateOfWeek && c.StartDate <= lastDateOfMonth
                listLeadMeeting = context.LeadMeeting.Where(c => c.Participant.Contains(employee.EmployeeId.ToString())
                                                 && c.Active == true)
                                                 .Select(m => new LeadMeetingEntityModel
                                                 {
                                                     LeadMeetingId = m.LeadMeetingId,
                                                     LeadId = m.LeadId,
                                                     EmployeeId = m.EmployeeId,
                                                     StartDate = m.StartDate,
                                                     EndDate = m.EndDate,
                                                     Title = m.Title,
                                                     Content = m.Content,
                                                     LocationMeeting = m.LocationMeeting,
                                                     Active = m.Active,
                                                     CreatedById = m.CreatedById,
                                                     CreateByName = "",
                                                     LeadName = "",
                                                     EmployeeName = "",
                                                     IsShowLink = false,
                                                 }).OrderByDescending(z => z.StartDate).ToList();

                if (employee.IsManager)
                {
                    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }

                    //List nhân viên thuộc quyền của quản lý
                    var listAllEmp = listCommonEmployee.Where(x => listGetAllChild.Contains(x.OrganizationId)).ToList();
                    var listAllEmpId = listAllEmp.Where(x => x.EmployeeId != null && x.EmployeeId != Guid.Empty)
                        .Select(y => y.EmployeeId).ToList();
                    var listAllUserId = context.User.Where(x => listAllEmpId.Contains(x.EmployeeId.Value))
                        .Select(y => y.UserId).ToList();
                    var _listCommonCustomer = listCommonCustomer.Where(x =>
                        (x.PersonInChargeId != null && listAllEmpId.Contains(x.PersonInChargeId.Value)) ||
                        (x.PersonInChargeId == null && listAllUserId.Contains(x.CreatedById))).ToList();


                    var _listCommonOrder = listCommonOrder.Where(x => listAllEmpId.Contains(x.Seller.Value)).ToList();

                    #region Doanh số bán hàng trong tuần

                    var listOrderOfWeek = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfWeek &&
                            x.OrderDate <= lastDateOfWeek)
                        .ToList();

                    listOrderOfWeek.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount - ((item.Amount * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount - item.DiscountValue.Value;
                        }

                        totalSalesOfWeek = totalSalesOfWeek + sales;
                    });

                    #endregion

                    #region Doanh số bán hàng trong tuần trước

                    var listOrderOfWeekPress = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfWeek.AddDays(-7) &&
                            x.OrderDate <= lastDateOfWeek.AddDays(-7))
                        .ToList();

                    listOrderOfWeekPress.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount - ((item.Amount * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount - item.DiscountValue.Value;
                        }

                        totalSalesOfWeekPress = totalSalesOfWeekPress + sales;
                    });

                    #endregion

                    #region Doanh số bán hàng trong tháng

                    var listOrderOfMonth = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfMonth &&
                            x.OrderDate <= lastDateOfMonth)
                        .ToList();

                    listOrderOfMonth.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount - ((item.Amount * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount - item.DiscountValue.Value;
                        }

                        totalSalesOfMonth = totalSalesOfMonth + sales;
                    });

                    #endregion

                    #region Doanh số bán hàng trong tháng trước

                    var listOrderOfMonthPress = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfMonth.AddMonths(-1) &&
                            x.OrderDate <= firstDateOfMonth.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59).AddTicks(999))
                        .ToList();

                    listOrderOfMonthPress.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount - ((item.Amount * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount - item.DiscountValue.Value;
                        }

                        totalSalesOfMonthPress = totalSalesOfMonthPress + sales;
                    });

                    #endregion

                    #region Doanh số bán hàng trong Qúy

                    var listOrderOfQuarter = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) &&
                            x.OrderDate >= dateOfQuarter.FirstDateOfQuarter &&
                            x.OrderDate <= dateOfQuarter.LastDateOfQuarter)
                        .ToList();

                    listOrderOfQuarter.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount - ((item.Amount * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount - item.DiscountValue.Value;
                        }

                        totalSalesOfQuarter = totalSalesOfQuarter + sales;
                    });

                    #endregion

                    #region Doanh số bán hàng trong Qúy Trước

                    var listOrderOfQuarterPress = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) &&
                            x.OrderDate >= dateOfQuarterPress.FirstDateOfQuarter &&
                            x.OrderDate <= dateOfQuarterPress.LastDateOfQuarter)
                        .ToList();

                    listOrderOfQuarterPress.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount - ((item.Amount * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount - item.DiscountValue.Value;
                        }

                        totalSalesOfQuarterPress = totalSalesOfQuarterPress + sales;
                    });

                    #endregion

                    #region Lấy 5 báo giá mới

                    var categoryTypeCodeOfQuote = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI");
                    var quoteStatusNewId = context.Category.FirstOrDefault(x =>
                        x.CategoryTypeId == categoryTypeCodeOfQuote.CategoryTypeId && x.CategoryCode == "MTA").CategoryId;

                    listQuote = context.Quote.Where(x =>
                            x.StatusId == quoteStatusNewId &&
                            (listAllEmpId.Contains(x.Seller.Value) || listAllEmpId.Contains(x.PersonInChargeId.Value)) &&
                            x.Active == true)
                        .OrderByDescending(z => z.QuoteDate).Take(5).Select(y => new QuoteEntityModel
                        {
                            QuoteId = y.QuoteId,
                            QuoteCode = y.QuoteCode,
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            ObjectTypeId = y.ObjectTypeId,
                            ObjectType = y.ObjectType,
                            CustomerName = "",
                            CustomerContactId = null,
                            Seller = y.Seller,
                            SellerName = "",
                            QuoteDate = y.QuoteDate
                        }).ToList();

                    listQuote.ForEach(item =>
                    {
                        if (item.ObjectType == "LEAD")
                        {
                            var lead = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "LEA");
                            item.CustomerName = lead == null
                                ? ""
                                : ((lead.FirstName == null ? "" : lead.FirstName.Trim()) + " " +
                                   (lead.LastName == null ? "" : lead.LastName.Trim()));
                            item.CustomerContactId = lead == null ? Guid.Empty : (lead.ContactId);
                        }
                        else if (item.ObjectType == "CUSTOMER")
                        {
                            var cus = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "CUS");
                            item.CustomerName = cus == null
                                ? ""
                                : ((cus.FirstName == null ? "" : cus.FirstName.Trim()) + " " +
                                   (cus.LastName == null ? "" : cus.LastName.Trim()));
                            item.CustomerContactId = cus == null ? Guid.Empty : (cus.ContactId);
                        }
                    });

                    var listSellerId = listQuote.Select(x => x.Seller).ToList();
                    if (listSellerId.Count > 0)
                    {
                        var listSellerOfQuote =
                            listCommonEmployee.Where(x => listSellerId.Contains(x.EmployeeId)).ToList();
                        listQuote.ForEach(item =>
                        {
                            var emp = listSellerOfQuote.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            if (emp != null)
                            {
                                item.SellerName = emp.EmployeeName.Trim();
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 khách hàng mới

                    var categoryTypeCodeCustomer =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                    var statusCustomerNewId = context.Category.FirstOrDefault(x =>
                            x.CategoryTypeId == categoryTypeCodeCustomer.CategoryTypeId && x.CategoryCode == "HDO")
                        .CategoryId;

                    listCustomer = _listCommonCustomer.Where(x =>
                            x.StatusId == statusCustomerNewId && x.Active == true)
                        .OrderByDescending(z => z.CreatedDate).Take(5).Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerName = y.CustomerName,
                            CustomerPhone = "",
                            CustomerEmail = "",
                            PersonInChargeId = y.PersonInChargeId,
                            PicName = "",
                            PicContactId = null
                        }).ToList();

                    var listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                    var listEmployeeId = listCustomer.Select(x => x.PersonInChargeId).ToList();

                    if (listCustomerId.Count > 0)
                    {
                        var listContactCustomer = listCommonContact
                            .Where(x => listCustomerId.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var cus_contact = listContactCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            item.CustomerPhone = cus_contact == null
                                ? ""
                                : (cus_contact.Phone == null ? "" : cus_contact.Phone.Trim());
                            item.CustomerEmail = cus_contact == null
                                ? ""
                                : (cus_contact.Email == null ? "" : cus_contact.Email.Trim());
                        });
                    }

                    if (listEmployeeId.Count > 0)
                    {
                        var listContactEmployee = listCommonContact
                            .Where(x => listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var emp_contact =
                                listContactEmployee.FirstOrDefault(x => x.ObjectId == item.PersonInChargeId);
                            item.PicName = emp_contact == null
                                ? ""
                                : (emp_contact.FirstName ?? "" + emp_contact.LastName ?? "");
                            if (emp_contact != null)
                            {
                                item.PicContactId = emp_contact.ContactId;
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 đơn hàng mới

                    listOrderNew = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.Active == true).OrderByDescending(z => z.OrderDate).Take(5)
                        .Select(y => new CustomerOrderEntityModel
                        {
                            OrderId = y.OrderId,
                            OrderCode = y.OrderCode,
                            CustomerId = y.CustomerId,
                            CustomerName = "",
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            Seller = y.Seller,
                            SellerName = "",
                            SellerContactId = Guid.Empty
                        }).ToList();

                    var listEmloyeeIdOfOrder = listOrderNew.Select(x => x.Seller).ToList();
                    if (listEmloyeeIdOfOrder.Count > 0)
                    {
                        var listEmployeeOfOrder = listCommonEmployee
                            .Where(x => listEmloyeeIdOfOrder.Contains(x.EmployeeId))
                            .ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var emp = listEmployeeOfOrder.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            item.SellerName = emp == null ? "" : emp.EmployeeName.Trim();
                        });
                    }

                    var listCustomerIdOfOrder = listOrderNew.Select(x => x.CustomerId).ToList();
                    if (listCustomerIdOfOrder.Count > 0)
                    {
                        var listCustomerOfOrder = listCommonCustomer
                            .Where(x => listCustomerIdOfOrder.Contains(x.CustomerId)).ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var customer = listCustomerOfOrder.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                            item.CustomerName = customer == null ? "" : customer.CustomerName.Trim();
                        });
                    }

                    #endregion

                    #region Các khách hàng có sinh nhật trong tuần hiện tại

                    #region Khách hàng doanh nghiệp lấy theo ngày cấp (Ngày thành lập)

                    var _yearNow = DateTime.Now.Year;
                    listCusBirthdayOfWeek_1 = _listCommonCustomer.Where(x =>
                        x.CustomerType == 1 && x.BusinessRegistrationDate != null && (new DateTime(_yearNow,
                            x.BusinessRegistrationDate.Value.Month, x.BusinessRegistrationDate.Value.Day)) >=
                        firstDateOfWeek &&
                        (new DateTime(_yearNow, x.BusinessRegistrationDate.Value.Month,
                            x.BusinessRegistrationDate.Value.Day)) <= lastDateOfWeek).Select(y => new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName,
                                CustomerPhone = "",
                                CustomerEmail = "",
                                PersonInChargeId = y.PersonInChargeId,
                                PicName = "",
                                PicContactId = null,
                                DateOfBirth = y.BusinessRegistrationDate
                            }).ToList();
                    #endregion

                    #region Khách hàng cá nhân lấy theo ngày sinh nhật

                    var listCusBirthdayOfWeek_2 = new List<CustomerEntityModel>();
                    listCusBirthdayOfWeek_2 =
                        _listCommonCustomer.Where(x => x.CustomerType == 2 && x.Active == true).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName,
                                CustomerPhone = "",
                                CustomerEmail = "",
                                PersonInChargeId = y.PersonInChargeId,
                                PicName = "",
                                PicContactId = null,
                                DateOfBirth = null
                            }).ToList();
                    var listCusId = listCusBirthdayOfWeek_2.Select(x => x.CustomerId).ToList();

                    if (listCusId.Count > 0)
                    {
                        var yearNow = DateTime.Now.Year;
                        var listCusContact = listCommonContact.Where(x =>
                                listCusId.Contains(x.ObjectId) && x.DateOfBirth != null &&
                                (new DateTime(yearNow, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) >=
                                firstDateOfWeek &&
                                (new DateTime(yearNow, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) <=
                                lastDateOfWeek)
                            .ToList();
                        var listCusIdByContact = listCusContact.Select(x => x.ObjectId).Distinct().ToList();

                        if (listCusIdByContact.Count > 0)
                        {
                            listCusBirthdayOfWeek_2 = listCusBirthdayOfWeek_2
                                .Where(x => listCusIdByContact.Contains(x.CustomerId)).ToList();

                            listCusBirthdayOfWeek_2.ForEach(item =>
                            {
                                var cus_contact = listCusContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                                item.DateOfBirth = cus_contact == null ? null : cus_contact.DateOfBirth;
                            });
                        }
                        else
                        {
                            listCusBirthdayOfWeek_2 = new List<CustomerEntityModel>();
                        }
                    }

                    #endregion

                    #region Ghép 2 nhóm khách hàng lại với nhau

                    listCusBirthdayOfWeek_1.AddRange(listCusBirthdayOfWeek_2);

                    listCusBirthdayOfWeek_1 =
                        listCusBirthdayOfWeek_1.OrderByDescending(x => x.DateOfBirth).ToList();

                    listCusBirthdayOfWeek_1.ForEach(item =>
                    {
                        var cus_cont = listCommonContact.FirstOrDefault(x =>
                            x.ObjectId == item.CustomerId && x.ObjectType == "CUS");
                        var emp = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                        item.CustomerPhone =
                            cus_cont == null ? "" : (cus_cont.Phone == null ? "" : (cus_cont.Phone.Trim()));
                        item.CustomerEmail =
                            cus_cont == null ? "" : (cus_cont.Email == null ? "" : (cus_cont.Email.Trim()));
                        item.PicName = emp == null ? "" : emp.EmployeeName.Trim();
                    });

                    #endregion

                    #endregion

                    #region Các nhân viên có sinh nhật trong tuần

                    listEmployeeBirthDayOfWeek = listCommonEmployee.Where(x => x.Active == true).Select(y =>
                        new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeName = y.EmployeeName,
                            OrganizationId = y.OrganizationId,
                            OrganizationName = "",
                            PositionId = y.PositionId,
                            PositionName = "",
                            DateOfBirth = null,
                            ContactId = null
                        }).ToList();

                    var listEmpId = listEmployeeBirthDayOfWeek.Select(x => x.EmployeeId).ToList();
                    if (listEmpId.Count > 0)
                    {
                        var _year = DateTime.Now.Year;
                        var listEmpContact = listCommonContact
                            .Where(x => listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP" && x.DateOfBirth != null &&
                                        (new DateTime(_year, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) >=
                                        firstDateOfWeek &&
                                        (new DateTime(_year, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) <=
                                        lastDateOfWeek).ToList();
                        var listEmpContactId = listEmpContact.Select(x => x.ObjectId).Distinct().ToList();

                        if (listEmpContactId.Count > 0)
                        {
                            listEmployeeBirthDayOfWeek = listEmployeeBirthDayOfWeek
                                .Where(x => listEmpContactId.Contains(x.EmployeeId.Value)).ToList();

                            listEmployeeBirthDayOfWeek.ForEach(item =>
                            {
                                var emp_cont = listEmpContact.FirstOrDefault(x => x.ObjectId == item.EmployeeId);
                                item.DateOfBirth = emp_cont == null ? null : emp_cont.DateOfBirth;
                                item.ContactId = emp_cont.ContactId;

                                var organization =
                                    listCommonOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                                var position = listCommonPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                                item.OrganizationName = organization == null ? "" : (organization.OrganizationName.Trim());
                                item.PositionName = position == null ? "" : (position.PositionName.Trim());
                            });

                            listEmployeeBirthDayOfWeek =
                                listEmployeeBirthDayOfWeek.OrderByDescending(z => z.DateOfBirth).ToList();
                        }
                        else
                        {
                            listEmployeeBirthDayOfWeek = new List<EmployeeEntityModel>();
                        }
                    }

                    #endregion

                    #region Các lịch hẹn trong tuần
                    listCustomerMeeting.ForEach(item =>
                    {
                        var cus = listCommonCustomer.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                        var emp = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        item.CustomerName = cus == null ? "" : (cus.CustomerName.Trim());
                        item.EmployeeName = emp == null ? "" : (emp.EmployeeName.Trim());
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        var cusPersonInChargeId = listCommonCustomer.FirstOrDefault(c => c.CustomerId == item.CustomerId)?.PersonInChargeId ?? Guid.Empty;
                        if (listAllUserId.Contains(item.CreatedById) || listAllEmpId.Contains(cusPersonInChargeId))
                            item.IsShowLink = true;
                    });

                    listLeadMeeting.ForEach(item =>
                    {
                        item.LeadName = listCommonContact.FirstOrDefault(c => c.ObjectId == item.LeadId && c.ObjectType == "LEA")?.FirstName ?? "";
                        item.EmployeeName = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)?.EmployeeName ?? "";
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        var leadPersonInChargeId = listCommonLead.FirstOrDefault(c => c.LeadId == item.LeadId)?.PersonInChargeId ?? Guid.Empty;
                        if (listAllUserId.Contains(item.CreatedById) || listAllEmpId.Contains(leadPersonInChargeId))
                            item.IsShowLink = true;
                    });
                    #endregion
                }
                else
                {
                    var _listCommonCustomer = listCommonCustomer.Where(x =>
                        (x.PersonInChargeId != null && x.PersonInChargeId == employee.EmployeeId) ||
                        (x.PersonInChargeId == null && x.CreatedById == user.UserId)).ToList();
                    var _listCommonOrder = listCommonOrder.Where(x => x.Seller == employee.EmployeeId).ToList();

                    #region Lấy 5 báo giá mới

                    var categoryTypeCodeOfQuote = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI");
                    var quoteStatusNewId = context.Category.FirstOrDefault(x =>
                        x.CategoryTypeId == categoryTypeCodeOfQuote.CategoryTypeId && x.CategoryCode == "MTA").CategoryId;

                    listQuote = context.Quote.Where(x =>
                            x.StatusId == quoteStatusNewId &&
                            (x.Seller == employee.EmployeeId || x.PersonInChargeId == employee.EmployeeId) &&
                            x.Active == true)
                        .OrderByDescending(z => z.QuoteDate).Take(5).Select(y => new QuoteEntityModel
                        {
                            QuoteId = y.QuoteId,
                            QuoteCode = y.QuoteCode,
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            ObjectTypeId = y.ObjectTypeId,
                            ObjectType = y.ObjectType,
                            CustomerName = "",
                            CustomerContactId = null,
                            Seller = y.Seller,
                            SellerName = "",
                            QuoteDate = y.QuoteDate
                        }).ToList();

                    listQuote.ForEach(item =>
                    {
                        if (item.ObjectType == "LEAD")
                        {
                            var lead = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "LEA");
                            item.CustomerName = lead == null
                                ? ""
                                : ((lead.FirstName == null ? "" : lead.FirstName.Trim()) + " " +
                                   (lead.LastName == null ? "" : lead.LastName.Trim()));
                            item.CustomerContactId = lead == null ? Guid.Empty : (lead.ContactId);
                        }
                        else if (item.ObjectType == "CUSTOMER")
                        {
                            var cus = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "CUS");
                            item.CustomerName = cus == null
                                ? ""
                                : ((cus.FirstName == null ? "" : cus.FirstName.Trim()) + " " +
                                   (cus.LastName == null ? "" : cus.LastName.Trim()));
                            item.CustomerContactId = cus == null ? Guid.Empty : (cus.ContactId);
                        }
                    });

                    var listSellerId = listQuote.Select(x => x.Seller).ToList();
                    if (listSellerId.Count > 0)
                    {
                        var listSellerOfQuote =
                            listCommonEmployee.Where(x => listSellerId.Contains(x.EmployeeId)).ToList();
                        listQuote.ForEach(item =>
                        {
                            var emp = listSellerOfQuote.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            if (emp != null)
                            {
                                item.SellerName = emp.EmployeeName.Trim();
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 khách hàng mới

                    var categoryTypeCodeCustomer =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                    var statusCustomerNewId = context.Category.FirstOrDefault(x =>
                            x.CategoryTypeId == categoryTypeCodeCustomer.CategoryTypeId && x.CategoryCode == "HDO")
                        .CategoryId;

                    listCustomer = _listCommonCustomer.Where(x =>
                            x.StatusId == statusCustomerNewId && x.Active == true)
                        .OrderByDescending(z => z.CreatedDate).Take(5).Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerName = y.CustomerName,
                            CustomerPhone = "",
                            CustomerEmail = "",
                            PersonInChargeId = y.PersonInChargeId,
                            PicName = "",
                            PicContactId = null
                        }).ToList();

                    var listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                    var listEmployeeId = listCustomer.Select(x => x.PersonInChargeId).ToList();

                    if (listCustomerId.Count > 0)
                    {
                        var listContactCustomer = listCommonContact
                            .Where(x => listCustomerId.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var cus_contact = listContactCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            item.CustomerPhone = cus_contact == null
                                ? ""
                                : (cus_contact.Phone == null ? "" : cus_contact.Phone.Trim());
                            item.CustomerEmail = cus_contact == null
                                ? ""
                                : (cus_contact.Email == null ? "" : cus_contact.Email.Trim());
                        });
                    }

                    if (listEmployeeId.Count > 0)
                    {
                        var listContactEmployee = listCommonContact
                            .Where(x => listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var emp_contact =
                                listContactEmployee.FirstOrDefault(x => x.ObjectId == item.PersonInChargeId);
                            item.PicName = emp_contact == null
                                ? ""
                                : (emp_contact.FirstName ?? "" + emp_contact.LastName ?? "");
                            if (emp_contact != null)
                            {
                                item.PicContactId = emp_contact.ContactId;
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 đơn hàng mới

                    listOrderNew = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.Active == true).OrderByDescending(z => z.OrderDate).Take(5)
                        .Select(y => new CustomerOrderEntityModel
                        {
                            OrderId = y.OrderId,
                            OrderCode = y.OrderCode,
                            CustomerId = y.CustomerId,
                            CustomerName = "",
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            Seller = y.Seller,
                            SellerName = "",
                            SellerContactId = Guid.Empty
                        }).ToList();

                    var listEmloyeeIdOfOrder = listOrderNew.Select(x => x.Seller).ToList();
                    if (listEmloyeeIdOfOrder.Count > 0)
                    {
                        var listEmployeeOfOrder = listCommonEmployee
                            .Where(x => listEmloyeeIdOfOrder.Contains(x.EmployeeId))
                            .ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var emp = listEmployeeOfOrder.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            item.SellerName = emp == null ? "" : emp.EmployeeName.Trim();
                        });
                    }

                    var listCustomerIdOfOrder = listOrderNew.Select(x => x.CustomerId).ToList();
                    if (listCustomerIdOfOrder.Count > 0)
                    {
                        var listCustomerOfOrder = listCommonCustomer
                            .Where(x => listCustomerIdOfOrder.Contains(x.CustomerId)).ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var customer = listCustomerOfOrder.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                            item.CustomerName = customer == null ? "" : customer.CustomerName.Trim();
                        });
                    }

                    #endregion

                    #region Các lịch hẹn trong tuần
                    listCustomerMeeting.ForEach(item =>
                    {
                        var cus = listCommonCustomer.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                        var emp = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        item.CustomerName = cus == null ? "" : (cus.CustomerName.Trim());
                        item.EmployeeName = emp == null ? "" : (emp.EmployeeName.Trim());
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        var cusPersonInChargeId = listCommonCustomer.FirstOrDefault(c => c.CustomerId == item.CustomerId)?.PersonInChargeId ?? Guid.Empty;
                        if (item.CreatedById == user.UserId || employee.EmployeeId == cusPersonInChargeId)
                            item.IsShowLink = true;
                    });

                    listLeadMeeting.ForEach(item =>
                    {
                        item.LeadName = listCommonContact.FirstOrDefault(c => c.ObjectId == item.LeadId && c.ObjectType == "LEA")?.FirstName ?? "";
                        item.EmployeeName = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)?.EmployeeName ?? "";
                        var leadPersonInChargeId = listCommonLead.FirstOrDefault(c => c.LeadId == item.LeadId)?.PersonInChargeId ?? Guid.Empty;
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        if (item.CreatedById == user.UserId || employee.EmployeeId == leadPersonInChargeId)
                            item.IsShowLink = true;
                    });
                    #endregion
                }

                var listParticipants = context.Employee.Where(c => c.Active == true).Select(
                    c => new EmployeeEntityModel
                    {
                        EmployeeId = c.EmployeeId,
                        EmployeeCode = c.EmployeeCode,
                        EmployeeName = c.EmployeeName
                    }).ToList();

                var listEmployee = listCommonEmployee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeName = y.EmployeeName,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                }).OrderBy(z => z.EmployeeName).ToList();

                return new GetDataDashboardHomeResult()
                {
                    Status = true,
                    Message = "Success",
                    TotalSalesOfWeek = totalSalesOfWeek,
                    TotalSalesOfMonth = totalSalesOfMonth,
                    TotalSalesOfQuarter = totalSalesOfQuarter,
                    TotalSalesOfWeekPress = totalSalesOfWeekPress,
                    TotalSalesOfMonthPress = totalSalesOfMonthPress,
                    TotalSalesOfQuarterPress = totalSalesOfQuarterPress,
                    ListQuote = listQuote,
                    ListCustomer = listCustomer,
                    ListOrderNew = listOrderNew,
                    ListCustomerMeeting = listCustomerMeeting,
                    ListCusBirthdayOfWeek = listCusBirthdayOfWeek_1,
                    ListEmployeeBirthDayOfWeek = listEmployeeBirthDayOfWeek,
                    ListLeadMeeting = listLeadMeeting,
                    ListParticipants = listParticipants,
                    ListEmployee = listEmployee
                };
            }
            catch (Exception e)
            {
                return new GetDataDashboardHomeResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
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
            dateReturn = new DateTime(_year, _month, _day, 23, 59, 59, 999);
            return dateReturn;
        }

        private DateTime FirstDateOfMonth()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;

            dateReturn = new DateTime(dateNow.Year, dateNow.Month, 1, 0, 0, 0, 0);

            return dateReturn;
        }

        private DateTime LastDateOfMonth()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;

            var firstDateOfMonth = FirstDateOfMonth();
            dateReturn = firstDateOfMonth.AddMonths(1).AddDays(-1);
            dateReturn = new DateTime(dateReturn.Year, dateReturn.Month, dateReturn.Day, 23, 59, 59, 999);

            return dateReturn;
        }

        private DateOfQuarter GetDateOfQuarter(DateTime dateNow)
        {
            //DateTime dateNow = DateTime.Now;
            DateOfQuarter dateOfQuarter = new DateOfQuarter();

            int quarter = 0;

            //Tìm quý hiện tại
            var month = dateNow.Month;

            if (month == 1 || month == 2 || month == 3)
            {
                quarter = 1;
            }
            else if (month == 4 || month == 5 || month == 6)
            {
                quarter = 2;
            }
            else if (month == 7 || month == 8 || month == 9)
            {
                quarter = 3;
            }
            else
            {
                quarter = 4;
            }

            switch (quarter)
            {
                case 1:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 1, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year, 4, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;

                case 2:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 4, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year, 7, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;

                case 3:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 7, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year, 10, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;

                case 4:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 10, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year + 1, 1, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;
            }

            return dateOfQuarter;
        }

        private decimal AmountAfterDiscount(decimal? Amount, bool? DiscountType, decimal? DiscountValue)
        {
            decimal value;

            if (DiscountType.Value == true)
            {
                value = Amount.Value - ((Amount.Value * DiscountValue.Value) / 100);
            }
            else
            {
                value = Amount.Value - DiscountValue.Value;
            }

            return value;
        }

        public CheckReceiptOrderHistoryResult CheckReceiptOrderHistory(CheckReceiptOrderHistoryParameter parameter)
        {
            try
            {
                var orderById = context.CustomerOrder.FirstOrDefault(c => c.OrderId == parameter.OrderId);
                if (orderById != null)
                {
                    //var listReceiptInvoiceMapping = context.ReceiptInvoiceMapping.Where(r => r.ObjectId == orderById.CustomerId).Select(r=>r.ReceiptInvoiceId).ToList();

                    //var receiptOrder = context.ReceiptOrderHistory.FirstOrDefault(ro => ro.OrderId == parameter.OrderId && listReceiptInvoiceMapping.Contains(ro.ObjectId));
                    //if (receiptOrder != null)
                    //{
                    //    if (parameter.MoneyOrder <= receiptOrder.AmountCollected)
                    //    {
                    //        return new CheckReceiptOrderHistoryResult()
                    //        {
                    //            Status = true,
                    //            CheckReceiptOrderHistory = false,
                    //            Message = ""
                    //        };
                    //    }
                    //}
                    var receiptOrder = context.ReceiptOrderHistory.Where(ro => ro.OrderId == parameter.OrderId).Sum(ro => ro.AmountCollected);
                    if (parameter.MoneyOrder <= receiptOrder)
                    {
                        return new CheckReceiptOrderHistoryResult()
                        {
                            Status = true,
                            CheckReceiptOrderHistory = false,
                            Message = ""
                        };
                    }

                }
                return new CheckReceiptOrderHistoryResult()
                {
                    Status = true,
                    CheckReceiptOrderHistory = true,
                    Message = ""
                };
            }
            catch (Exception e)
            {
                return new CheckReceiptOrderHistoryResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataOrderServiceCreateResult GetMasterDataOrderServiceCreate(GetMasterDataOrderServiceCreateParameter parameter)
        {
            try
            {
                var listProductCategory = new List<ProductCategoryEntityModel>();
                var listProduct = new List<ProductEntityModel>();
                var listLocalAddress = new List<LocalAddressEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Lấy list Khu vực theo nhân viên đăng nhập
                var listComonLocalPoint = context.LocalPoint.ToList();
                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName,

                        //Lấy list Điểm thuộc khu vực
                        ListLocalPoint = listComonLocalPoint.Where(p => p.LocalAddressId == y.LocalAddressId).Select(
                            q => new LocalPointEntityModel
                            {
                                LocalPointId = q.LocalPointId,
                                LocalPointCode = q.LocalPointCode,
                                LocalPointName = q.LocalPointName,
                                StatusId = q.StatusId,
                                LocalAddressId = q.LocalAddressId,
                                StatusName = q.StatusId == 1 ? "Sẵn sàng" : "Đang dùng"
                            }).OrderBy(z => z.LocalPointCode).ToList()
                    }).ToList();

                //Lấy list trạng thái của Đơn hàng
                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).Select(y => new OrderStatus
                {
                    OrderStatusId = y.OrderStatusId,
                    OrderStatusCode = y.OrderStatusCode
                }).ToList();

                //Lấy list Đơn vị tiền của hệ thống
                var moneyUnitType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI")?.CategoryTypeId;
                var listMoneyUnit = context.Category.Where(x => x.Active == true && x.CategoryTypeId == moneyUnitType)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode
                    }).ToList();

                //Lấy nhóm khách hàng điềm đạm
                var customerGroupCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA")
                    ?.CategoryTypeId;
                var customerGroupId = context.Category.FirstOrDefault(x =>
                    x.CategoryCode == "TPH" && x.CategoryTypeId == customerGroupCategoryType)?.CategoryId;

                var listPriceProduct = context.PriceProduct.Where(x =>
                        x.Active && x.EffectiveDate.Date <= DateTime.Now.Date &&
                        x.CustomerGroupCategory == customerGroupId)
                    .ToList();

                //list đơn vị tính
                var productUnitCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH")
                    ?.CategoryTypeId;
                var listProductUnit = context.Category.Where(x => x.CategoryTypeId == productUnitCategoryType).ToList();

                listProductCategory = context.ProductCategory.Where(x => x.Active == true).Select(y =>
                    new ProductCategoryEntityModel
                    {
                        ProductCategoryId = y.ProductCategoryId,
                        ProductCategoryCode = y.ProductCategoryCode,
                        ProductCategoryName = y.ProductCategoryName,
                        ProductCategoryLevel = y.ProductCategoryLevel
                    }).ToList();

                listProduct = context.Product.Where(x => x.Active == true).Select(y => new ProductEntityModel
                {
                    ProductId = y.ProductId,
                    ProductCode = y.ProductCode,
                    ProductName = y.ProductName,
                    ProductCategoryId = y.ProductCategoryId,
                    ProductUnitId = y.ProductUnitId
                }).ToList();

                //Lấy giá bán của các sản phẩm
                listProduct.ForEach(item =>
                {
                    decimal priceProduct = 0;

                    var listCurrentPriceProduct = listPriceProduct
                        .Where(x => x.ProductId == item.ProductId)
                        .OrderByDescending(z => z.EffectiveDate)
                        .ToList();

                    var price = listCurrentPriceProduct.FirstOrDefault();

                    if (price != null)
                    {
                        priceProduct = price.PriceVnd;
                    }

                    item.Price1 = priceProduct;

                    //Lấy tên đơn vị tính
                    item.ProductUnitName = listProductUnit.FirstOrDefault(x => x.CategoryId == item.ProductUnitId)?
                        .CategoryName;
                });

                return new GetMasterDataOrderServiceCreateResult()
                {
                    Status = true,
                    Message = "Success",
                    ListOrderStatus = listOrderStatus,
                    ListMoneyUnit = listMoneyUnit,
                    ListProductCategory = listProductCategory,
                    ListProduct = listProduct,
                    ListLocalAddress = listLocalAddress
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderServiceCreateResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateOrderServiceResult CreateOrderService(CreateOrderServiceParameter parameter)
        {
            try
            {
                bool isValidParameterNumber = true;
                if (parameter.CustomerOrder?.DaysAreOwed < 0 || parameter.CustomerOrder?.MaxDebt < 0 ||
                    parameter.CustomerOrder?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                }
                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    if (item.Quantity <= 0 || item.UnitPrice < 0 || item?.Vat < 0 || item?.DiscountValue < 0 ||
                        item?.ExchangeRate <= 0 || item?.GuaranteeTime < 0)
                    {
                        isValidParameterNumber = false;
                    }
                });

                if (!isValidParameterNumber)
                {
                    return new CreateOrderServiceResult
                    {
                        Message = CommonMessage.Order.CREATE_FAIL,
                        Status = false
                    };
                }

                //Kiểm tra chiết khấu của đơn hàng
                if (parameter.CustomerOrder.DiscountValue == null)
                {
                    parameter.CustomerOrder.DiscountValue = 0;
                }

                //Kiểm tra chiết khấu của sản phẩm
                if (parameter.ListCustomerOrderDetail.Count > 0)
                {
                    var listProduct = parameter.ListCustomerOrderDetail.ToList();
                    listProduct.ForEach(item =>
                    {
                        if (item.DiscountValue == null)
                        {
                            item.DiscountValue = 0;
                        }
                    });
                }

                var AccountKHL = context.Customer.Where(item => item.CustomerCode == "KHL001").SingleOrDefault();
                parameter.CustomerOrder.CustomerId = AccountKHL.CustomerId;

                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    item.CreatedById = parameter.UserId;
                    item.CreatedDate = DateTime.Now;
                    item.Active = true;
                    item.OrderDetailId = Guid.NewGuid();
                    foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                    {
                        itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                    }
                });

                parameter.CustomerOrder.OrderId = Guid.NewGuid();
                parameter.CustomerOrder.OrderCode = GenerateOrderCode(1);

                //Kiểm tra trường hợp đã tồn tại Mã đơn hàng
                var duplicateOrder =
                    context.CustomerOrder.FirstOrDefault(x => x.OrderCode == parameter.CustomerOrder.OrderCode);
                if (duplicateOrder != null)
                {
                    return new CreateOrderServiceResult
                    {
                        Status = false,
                        Message = CommonMessage.Order.ORDER_EXIST
                    };
                }

                #region Thêm Điểm vào bảng mapping với Đơn hàng và kiểm tra Điểm đã được đặt chưa

                bool hasUsing = false;
                var listLocalPoint = context.LocalPoint.ToList();
                var listMapping = new List<CustomerOrderLocalPointMapping>();
                var listUpdateLocalPoint = new List<LocalPoint>();
                parameter.ListLocalPointId.ForEach(item =>
                {
                    var localPoint = listLocalPoint.FirstOrDefault(x => x.LocalPointId == item);
                    listUpdateLocalPoint.Add(localPoint);

                    //Nếu trong list Điểm có Điểm đang được sử dụng thì báo lỗi
                    if (localPoint.StatusId != 1)
                    {
                        hasUsing = true;
                    }

                    var mapping = new CustomerOrderLocalPointMapping();
                    mapping.CustomerOrderLocalPointMappingId = Guid.NewGuid();
                    mapping.OrderId = parameter.CustomerOrder.OrderId;
                    mapping.LocalPointId = item;

                    listMapping.Add(mapping);
                });

                //Nếu có Điểm đang được sử dụng thì báo lỗi
                if (hasUsing)
                {
                    return new CreateOrderServiceResult()
                    {
                        Status = false,
                        Message = "Có bàn đang dùng vui lòng kiểm tra lại các bàn đã đặt"
                    };
                }
                //Nếu tất cả Điểm đều thỏa mãn thì đổi trạng thái các Điểm sang Đang dùng
                else
                {
                    listUpdateLocalPoint.ForEach(item => { item.StatusId = 0; });
                    context.LocalPoint.UpdateRange(listUpdateLocalPoint);
                }

                context.CustomerOrderLocalPointMapping.AddRange(listMapping);

                #endregion

                parameter.CustomerOrder.CreatedById = parameter.UserId;
                parameter.CustomerOrder.CreatedDate = DateTime.Now;
                parameter.CustomerOrder.CustomerOrderDetail = parameter.ListCustomerOrderDetail;
                context.CustomerOrder.Add(parameter.CustomerOrder);
                context.SaveChanges();

                #region Gửi thông báo

                NotificationHelper.AccessNotification(context, TypeModel.CustomerOrder, "CRE", new CustomerOrder(),
                    parameter.CustomerOrder, true);

                #endregion

                return new CreateOrderServiceResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new CreateOrderServiceResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataPayOrderServiceResult GetMasterDataPayOrderService(GetMasterDataPayOrderServiceParameter parameter)
        {
            try
            {
                var listLocalAddress = new List<LocalAddressEntityModel>();
                var listLocalPoint = new List<LocalPointEntityModel>();

                //Lấy tỷ lệ quy đổi Tiền thành Điểm của khách hàng
                var pointRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "PointRate")
                    .SystemValueString;
                decimal pointRate = decimal.Parse(pointRateString);

                //Lấy tỷ lệ quy đổi Điểm thành Tiền của khách hàng
                var moneyRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "MoneyRate")
                    .SystemValueString;
                decimal moneyRate = decimal.Parse(moneyRateString);

                //Lấy list khu vực theo Branch của nhân viên đăng nhập
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName
                    }).OrderBy(z => z.LocalAddressCode).ToList();

                var listLocalAddressId = listLocalAddress.Select(y => y.LocalAddressId).ToList();
                listLocalPoint = context.LocalPoint.Where(x => listLocalAddressId.Contains(x.LocalAddressId)).Select(
                    y => new LocalPointEntityModel
                    {
                        LocalPointId = y.LocalPointId,
                        LocalPointCode = y.LocalPointCode,
                        LocalPointName = y.LocalPointName,
                        LocalAddressId = y.LocalAddressId,
                        StatusId = y.StatusId
                    }).OrderBy(z => z.LocalPointCode).ToList();

                return new GetMasterDataPayOrderServiceResult()
                {
                    Status = true,
                    Message = "Success",
                    ListLocalAddress = listLocalAddress,
                    ListLocalPoint = listLocalPoint,
                    PointRate = pointRate,
                    MoneyRate = moneyRate
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataPayOrderServiceResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetListOrderByLocalPointResult GetListOrderByLocalPoint(GetListOrderByLocalPointParameter parameter)
        {
            try
            {
                var listOrder = new List<CustomerOrderEntityModel>();
                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                var statusDRA = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DRA");

                #region Lấy những đơn hàng thuộc Điểm có trạng thái Nháp

                var listOrderId = context.CustomerOrderLocalPointMapping
                    .Where(x => x.LocalPointId == parameter.LocalPointId).Select(y => y.OrderId).Distinct().ToList();

                listOrder = context.CustomerOrder
                    .Where(x => listOrderId.Contains(x.OrderId) && x.StatusId == statusDRA.OrderStatusId).Select(y =>
                        new CustomerOrderEntityModel
                        {
                            OrderId = y.OrderId,
                            OrderCode = y.OrderCode,
                            OrderDate = y.OrderDate,
                            Amount = y.Amount
                        }).ToList();

                #endregion

                return new GetListOrderByLocalPointResult()
                {
                    Status = true,
                    Message = "Success",
                    ListOrder = listOrder
                };
            }
            catch (Exception e)
            {
                return new GetListOrderByLocalPointResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public PayOrderByLocalPointResult PayOrderByLocalPoint(PayOrderByLocalPointParameter parameter)
        {
            try
            {
                //Lấy list Khách hàng
                var listCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var customer = listCustomer.FirstOrDefault(x => x.CustomerId == parameter.CustomerId);

                //Lấy nhóm khách hàng điềm đạm
                var customerGroupCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA")
                    ?.CategoryTypeId;
                var customerGroupId = context.Category.FirstOrDefault(x =>
                    x.CategoryCode == "TPH" && x.CategoryTypeId == customerGroupCategoryType)?.CategoryId;

                //Trạng thái khách hàng
                var customerStatusType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                var customerStatusHDO = context.Category.FirstOrDefault(x =>
                    x.CategoryCode == "HDO" && x.CategoryTypeId == customerStatusType.CategoryTypeId);

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Lấy tỷ lệ quy đổi điểm của khách hàng
                var pointRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "PointRate")
                    .SystemValueString;

                decimal pointRate = decimal.Parse(pointRateString);

                var listLocalPointId = new List<Guid?>();
                DateTime? orderDate = null;
                string customerName = "";
                string customerPhone = "";
                string cashierName = "";

                Guid? newCustomerId = customer?.CustomerId;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                    var statusCOMP = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "COMP");

                    var listOrder = context.CustomerOrder.Where(x => parameter.ListOrderId.Contains(x.OrderId)).ToList();

                    #region Kiểm tra xem khách hàng mới hay khách hàng cũ

                    //Nếu bỏ trống Số điện thoại thì lấy khách lẻ
                    if (String.IsNullOrEmpty(parameter.CustomerPhone))
                    {
                        var customerPersonal = context.Customer.FirstOrDefault(x => x.CustomerCode == "KHL001");
                        newCustomerId = customerPersonal.CustomerId;
                        parameter.CustomerName = customerPersonal.CustomerName;
                    }
                    //Nếu là khách hàng mới thì tạo khách hàng
                    else if (customer.CustomerCode == "KHL001")
                    {
                        var newCustomer = new Customer();
                        newCustomer.CustomerId = Guid.NewGuid();
                        newCustomer.CustomerCode = GenerateCustomerCodeByOrderService();
                        newCustomer.CustomerGroupId = customerGroupId;
                        newCustomer.CustomerName = parameter.CustomerName?.Trim() ?? "";
                        newCustomer.StatusId = customerStatusHDO.CategoryId;
                        newCustomer.PersonInChargeId = user.EmployeeId; //Người phụ trách khách hàng mới tạo sẽ là nhân viên Thanh toán
                        newCustomer.CustomerType = 2; //Khách hàng cá nhân
                        newCustomer.MaximumDebtValue = 0;
                        newCustomer.MaximumDebtDays = 0;
                        newCustomer.TotalSaleValue = 0;
                        newCustomer.TotalReceivable = 0;
                        newCustomer.TotalCapital = 0;
                        newCustomer.Active = true;
                        newCustomer.CreatedById = parameter.UserId;
                        newCustomer.CreatedDate = DateTime.Now;

                        #region Tính điểm cho khách hàng

                        var totalAmout = listOrder.Sum(x => x.Amount);
                        var point = Math.Round(totalAmout / pointRate);
                        newCustomer.Point = point;
                        newCustomer.PayPoint = 0;

                        #endregion

                        context.Customer.Add(newCustomer);

                        newCustomerId = newCustomer.CustomerId;

                        //Tạo contact
                        var newContact = new Contact();
                        newContact.ContactId = Guid.NewGuid();
                        newContact.ObjectId = newCustomer.CustomerId;
                        newContact.ObjectType = "CUS";
                        newContact.FirstName = parameter.CustomerName?.Trim() ?? "";
                        newContact.Gender = "NAM";
                        newContact.Phone = parameter.CustomerPhone.Trim();
                        newContact.Email = "";
                        newContact.Active = true;
                        newContact.OptionPosition = "CUS";
                        newContact.CreatedById = parameter.UserId;
                        newContact.CreatedDate = DateTime.Now;

                        context.Contact.Add(newContact);
                    }
                    //Nếu khách hàng đã tồn tại trên hệ thống thì cập nhật lại tên khách hàng
                    else
                    {
                        if (!String.IsNullOrEmpty(parameter.CustomerName))
                        {
                            customer.CustomerName = parameter.CustomerName;

                            #region Tính điểm cho khách hàng

                            customer.Point = parameter.Point;
                            var currentPayPoint = customer.PayPoint ?? 0;
                            customer.PayPoint = currentPayPoint + parameter.PayPoint;

                            #endregion

                            context.Customer.Update(customer);

                            var _contact = context.Contact.FirstOrDefault(x =>
                                x.ObjectId == customer.CustomerId && x.ObjectType == "CUS");
                            _contact.FirstName = parameter.CustomerName;
                            _contact.LastName = "";
                            context.Contact.Update(_contact);
                        }
                    }

                    #endregion

                    //Lấy phòng ban
                    var organization = context.Organization.FirstOrDefault();

                    //Lấy danh sách lý do thu
                    var reasonType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTH");
                    //Thu tiền khách hàng
                    var reason = context.Category.FirstOrDefault(x =>
                        x.CategoryCode == "THA" && x.CategoryTypeId == reasonType.CategoryTypeId);

                    //Trạng thái phiếu thu
                    var statusType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH");
                    var status = context.Category.FirstOrDefault(x =>
                        x.CategoryCode == "DSO" && x.CategoryTypeId == statusType.CategoryTypeId);

                    //Lấy đơn vị tiền
                    var unitPriceType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI");
                    //VND
                    var unitPrice = context.Category.FirstOrDefault(x =>
                        x.CategoryCode == "VND" && x.CategoryTypeId == unitPriceType.CategoryTypeId);

                    var listReceiptInvoice = new List<ReceiptInvoice>();
                    var listReceiptInvoiceMapping = new List<ReceiptInvoiceMapping>();
                    //Đổi trạng thái list Order sang Đóng
                    listOrder.ForEach(item =>
                    {
                        item.StatusId = statusCOMP.OrderStatusId;
                        item.CustomerId = newCustomerId.Value;
                        item.CustomerName = parameter.CustomerName?.Trim() ?? "";
                        item.DiscountType = parameter.DiscountType;
                        item.DiscountValue = parameter.DiscountValue;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;

                        //Với mỗi đơn hàng tạo ra một phiếu thu tiền mặt
                        var receiptInvoice = new ReceiptInvoice();
                        receiptInvoice.ReceiptInvoiceId = Guid.NewGuid();
                        receiptInvoice.ReceiptInvoiceCode =
                            "PT" + "-" + organization.OrganizationCode + DateTime.Now.Year
                            + (context.ReceiptInvoice.Count(r =>
                                   r.CreatedDate.Year == DateTime.Now.Year) + 1)
                            .ToString("D5");
                        receiptInvoice.ReceiptInvoiceDetail = "Thanh toán";
                        receiptInvoice.ReceiptInvoiceReason = reason.CategoryId;
                        receiptInvoice.ReceiptInvoiceNote = "";
                        receiptInvoice.OrganizationId = organization.OrganizationId;
                        receiptInvoice.StatusId = status.CategoryId;
                        receiptInvoice.RecipientName = parameter.CustomerName?.Trim() ?? "";
                        receiptInvoice.RecipientAddress = "";
                        decimal? amout = 0;
                        if (item.DiscountType == true)
                        {
                            amout = item.Amount - item.Amount * (item.DiscountValue ?? 0) / 100;
                        }
                        else
                        {
                            amout = item.Amount - (item.DiscountValue ?? 0);
                        }
                        receiptInvoice.UnitPrice = amout;
                        receiptInvoice.CurrencyUnit = unitPrice.CategoryId;
                        receiptInvoice.ExchangeRate = 1;
                        receiptInvoice.Amount = amout;
                        receiptInvoice.ReceiptDate = DateTime.Now;
                        receiptInvoice.VouchersDate = DateTime.Now;
                        receiptInvoice.Active = true;
                        receiptInvoice.CreatedById = parameter.UserId;
                        receiptInvoice.CreatedDate = DateTime.Now;

                        listReceiptInvoice.Add(receiptInvoice);

                        //mapping
                        var receiptInvoiceMapping = new ReceiptInvoiceMapping();
                        receiptInvoiceMapping.ReceiptInvoiceMappingId = Guid.NewGuid();
                        receiptInvoiceMapping.ReceiptInvoiceId = receiptInvoice.ReceiptInvoiceId;
                        receiptInvoiceMapping.ObjectId = newCustomerId;
                        receiptInvoiceMapping.CreatedById = parameter.UserId;
                        receiptInvoiceMapping.CreatedDate = DateTime.Now;

                        listReceiptInvoiceMapping.Add(receiptInvoiceMapping);
                    });

                    context.CustomerOrder.UpdateRange(listOrder);
                    context.ReceiptInvoice.AddRange(listReceiptInvoice);
                    context.ReceiptInvoiceMapping.AddRange(listReceiptInvoiceMapping);

                    //Đổi trạng thái của các Điểm gắn với đơn hàng
                    listLocalPointId = context.CustomerOrderLocalPointMapping
                        .Where(x => parameter.ListOrderId.Contains(x.OrderId.Value)).Select(y => y.LocalPointId).ToList();
                    var listLocalPoint = context.LocalPoint.Where(x => listLocalPointId.Contains(x.LocalPointId)).ToList();
                    listLocalPoint.ForEach(item => { item.StatusId = 1; });
                    context.LocalPoint.UpdateRange(listLocalPoint);

                    var order = listOrder.FirstOrDefault();
                    orderDate = order.OrderDate;
                    customerName = order.CustomerName;
                    if (String.IsNullOrEmpty(parameter.CustomerPhone))
                    {
                        customerPhone = "";
                    }
                    else
                    {
                        customerPhone = parameter.CustomerPhone.Trim();
                    }
                    cashierName = employee.EmployeeName.Trim();

                    context.SaveChanges();

                    transaction.Commit();
                }

                return new PayOrderByLocalPointResult()
                {
                    Status = true,
                    Message = "Success",
                    ListLocalPointId = listLocalPointId,
                    OrderDate = orderDate,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CashierName = cashierName
                };
            }
            catch (Exception e)
            {
                return new PayOrderByLocalPointResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CheckExistsCustomerByPhoneResult CheckExistsCustomerByPhone(CheckExistsCustomerByPhoneParameter parameter)
        {
            try
            {
                var customer = new CustomerEntityModel();

                var contact =
                    context.Contact.FirstOrDefault(x => x.Phone == parameter.CustomerPhone && x.ObjectType == "CUS");

                //Lấy tỷ lệ quy đổi điểm của khách hàng
                var pointRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "PointRate")
                    .SystemValueString;

                decimal pointRate = decimal.Parse(pointRateString);

                //Nếu đã tồn tại khách hàng
                if (contact != null)
                {
                    customer = context.Customer.Where(x => x.CustomerId == contact.ObjectId).Select(y =>
                        new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            Point = y.Point,
                            PayPoint = y.PayPoint
                        }).FirstOrDefault();
                }
                //Nếu chưa tồn tại khách hàng thì trả về khách lẻ
                else
                {
                    customer = context.Customer.Where(x => x.CustomerCode == "KHL001").Select(y =>
                        new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            Point = 0,
                            PayPoint = 0
                        }).FirstOrDefault();
                }

                return new CheckExistsCustomerByPhoneResult()
                {
                    Status = true,
                    Message = "Success",
                    Customer = customer,
                    PointRate = pointRate
                };
            }
            catch (Exception e)
            {
                return new CheckExistsCustomerByPhoneResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public RefreshLocalPointResult RefreshLocalPoint(RefreshLocalPointParameter parameter)
        {
            try
            {
                var listLocalAddress = new List<LocalAddressEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Lấy list Khu vực theo nhân viên đăng nhập
                var listComonLocalPoint = context.LocalPoint.ToList();
                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName,

                        //Lấy list Điểm thuộc khu vực
                        ListLocalPoint = listComonLocalPoint.Where(p => p.LocalAddressId == y.LocalAddressId).Select(
                            q => new LocalPointEntityModel
                            {
                                LocalPointId = q.LocalPointId,
                                LocalPointCode = q.LocalPointCode,
                                LocalPointName = q.LocalPointName,
                                StatusId = q.StatusId,
                                LocalAddressId = q.LocalAddressId,
                                StatusName = q.StatusId == 1 ? "Sẵn sàng" : "Đang dùng"
                            }).OrderBy(z => z.LocalPointCode).ToList()
                    }).ToList();

                return new RefreshLocalPointResult()
                {
                    Status = true,
                    Message = "Success",
                    ListLocalAddress = listLocalAddress
                };
            }
            catch (Exception e)
            {
                return new RefreshLocalPointResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetLocalPointByLocalAddressResult GetLocalPointByLocalAddress(GetLocalPointByLocalAddressParameter parameter)
        {
            try
            {
                var listLocalAddress = new List<LocalAddressEntityModel>();
                var listLocalPoint = new List<LocalPointEntityModel>();

                //Lấy list khu vực theo Branch của nhân viên đăng nhập
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName
                    }).OrderBy(z => z.LocalAddressCode).ToList();

                //Nếu lấy tất cả
                if (parameter.LocalAddressId == Guid.Empty)
                {
                    var listLocalAddressId = listLocalAddress.Select(y => y.LocalAddressId).ToList();
                    listLocalPoint = context.LocalPoint.Where(x => listLocalAddressId.Contains(x.LocalAddressId))
                        .Select(
                            y => new LocalPointEntityModel
                            {
                                LocalPointId = y.LocalPointId,
                                LocalPointCode = y.LocalPointCode,
                                LocalPointName = y.LocalPointName,
                                LocalAddressId = y.LocalAddressId,
                                StatusId = y.StatusId
                            }).OrderBy(z => z.LocalPointCode).ToList();
                }
                //Nếu lấy theo 1 Khu vực xác định
                else
                {
                    listLocalPoint = context.LocalPoint.Where(x => x.LocalAddressId == parameter.LocalAddressId).Select(
                        y => new LocalPointEntityModel
                        {
                            LocalPointId = y.LocalPointId,
                            LocalPointCode = y.LocalPointCode,
                            LocalPointName = y.LocalPointName,
                            LocalAddressId = y.LocalAddressId,
                            StatusId = y.StatusId
                        }).OrderBy(z => z.LocalPointCode).ToList();
                }

                return new GetLocalPointByLocalAddressResult()
                {
                    Status = true,
                    Message = "Success",
                    ListLocalPoint = listLocalPoint
                };
            }
            catch (Exception e)
            {
                return new GetLocalPointByLocalAddressResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private string GenerateCustomerCodeByOrderService()
        {
            var result = "";

            var now = DateTime.Now;
            var hour = now.Hour.ToString("00");
            var minute = now.Minute.ToString("00");
            var second = now.Second.ToString("00");
            var day = now.Day.ToString("00");
            var month = now.Month.ToString("00");
            var year = now.Year.ToString("0000");

            result = day + month + year + hour + minute + second;

            return result;
        }

        public GetDataSearchTopReVenueResult GetDataSearchTopReVenue(GetDataSearchTopReVenueParameter parameter)
            {
            try
            {
                #region Lấy danh sách nhân viên bán hàng
                var listEmployeeEntity = context.Employee.ToList();

                var listEmployeeResult = new List<DataAccess.Models.Employee.EmployeeEntityModel>();
                listEmployeeEntity?.ForEach(emp =>
                {
                    listEmployeeResult.Add(new EmployeeEntityModel
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeName = emp.EmployeeName,
                        EmployeeCode = emp.EmployeeCode
                    });
                });
                #endregion

                #region lấy ID phòng ban hiện tại của nhân viên đang đăng nhập
                var currentEmpId = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;
                var currentOrganizationId = context.Employee.FirstOrDefault(f => f.EmployeeId == currentEmpId).OrganizationId;
                var currentOrganizationEntity = context.Organization.FirstOrDefault(f => f.OrganizationId == currentOrganizationId);
                var currentOrganization = new DataAccess.Models.OrganizationEntityModel()
                {
                    OrganizationId = currentOrganizationEntity.OrganizationId,
                    OrganizationName = currentOrganizationEntity.OrganizationName
                };
                #endregion

                #region Lấy danh sách khách hàng
                var listCustomerEntity = context.Customer.Where(w => w.Active == true).ToList();
                var listCustomer = new List<Models.Customer.CustomerEntityModel>();
                listCustomerEntity?.ForEach(cus =>
                {
                    listCustomer.Add(new CustomerEntityModel
                    {
                        CustomerId = cus.CustomerId,
                        CustomerName = cus.CustomerName,
                        CustomerCode = cus.CustomerCode
                    });
                });
                #endregion

                #region Nhóm khách hàng
                var customerGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA").CategoryTypeId;
                var customerGroupEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == customerGroupTypeId).ToList();
                var listCustomerGroupCategory = new List<Models.CategoryEntityModel>();
                customerGroupEntity?.ForEach(cate =>
                {
                    listCustomerGroupCategory.Add(new CategoryEntityModel
                    {
                        CategoryId = cate.CategoryId,
                        CategoryCode = cate.CategoryCode,
                        CategoryName = cate.CategoryName
                    });
                });
                #endregion

                #region Nhóm sản phẩm, dịch vụ
                var listProductCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList();
                var listProductCategory = new List<Models.ProductCategory.ProductCategoryEntityModel>();
                listProductCategoryEntity?.ForEach(cate =>
                {
                    listProductCategory.Add(new ProductCategoryEntityModel
                    {
                        ProductCategoryId = cate.ProductCategoryId,
                        ProductCategoryName = cate.ProductCategoryName,
                        ProductCategoryCode = cate.ProductCategoryCode
                    });
                });
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                return new GetDataSearchTopReVenueResult()
                {
                    ListEmployee = listEmployeeResult.OrderBy(o => o.EmployeeName).ToList(),
                    CurrentOrganization = currentOrganization,
                    ListCustomer = listCustomer.OrderBy(o => o.CustomerName).ToList(),
                    ListCustomerGroupCategory = listCustomerGroupCategory.OrderBy(o => o.CategoryName).ToList(),
                    ListProductCategory = listProductCategory.OrderBy(o => o.ProductCategoryName).ToList(),
                    InforExportExcel = inforExportExcel,
                    Status = true,
                    Message = ""
                };
            }
            catch (Exception e)
            {
                return new GetDataSearchTopReVenueResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchTopReVenueResult SearchTopReVenue(SearchTopReVenueParameter parameter)
        {
            try
            {
                #region Master data và  Phân quyền
                var listOrganizationentity = context.Organization.Where(w => w.Active == true).OrderBy(w => w.OrganizationName).ToList();
                var orderStatusEntity = context.OrderStatus.Where(w => w.Active == true).ToList();
                var listStatusIdValid = orderStatusEntity.Where(w => (w.OrderStatusCode == "DLV" || w.OrderStatusCode == "COMP") && w.Active == true)
                                                         .Select(w => w.OrderStatusId)
                                                         .ToList();
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listEmployeeAuthorization = new List<Employee>();
                var listAllCustomer = context.Customer.ToList();
                var listProductEntity = context.Product.ToList();
                var listProductCategoryEntity = context.ProductCategory.ToList();

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = GetOrganizationChildren(employee.OrganizationId.Value, listGetAllChild, listOrganizationentity);
                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        listEmployeeAuthorization = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                    }
                }
                else
                {
                    //Nếu không phải quản lý
                    listEmployeeAuthorization = listAllEmployee.Where(w => w.EmployeeId == employeeId).ToList();
                }
                #endregion

                #region Lấy thông tin để filter
                //nếu có điều kiện tìm theo khách hàng (loại khách hàng, khách hàng , nhóm khách hàng)
                //loại khách hàng
                var listCustomerFilter = listAllCustomer.ToList();
                if (parameter.ListCustomerType.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerType != null && parameter.ListCustomerType.Contains(w.CustomerType)).ToList();
                }

                //khách hàng 
                if (parameter.ListCustomer.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerId != null && parameter.ListCustomer.Contains(w.CustomerId)).ToList();
                }

                //nhóm khách hàng
                if (parameter.ListCustomerGroup.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerGroupId != null && parameter.ListCustomerGroup.Contains(w.CustomerGroupId)).ToList();
                }

                var listCustomerFilterId = listCustomerFilter.Select(w => w.CustomerId).ToList();
                #endregion

                #region master data

                var listCustomerOrderDetailEntity = context.CustomerOrderDetail.Where(w => w.Active == true).ToList();

                //chỉ lấy những đơn hàng có trạng thái đóng hoặc đơn hàng bán
                var listCustomerOrderEntity = context.CustomerOrder
                                                     .Where(w => w.Active == true
                                                                 && w.Seller != null && listEmployeeAuthorization.Select(emp => emp.EmployeeId).Contains(w.Seller.Value) //phân quyền dữ liệu
                                                                 && listStatusIdValid.Contains(w.StatusId.Value) //filter theo trạng thái
                                                                 && (parameter.ListSellerId.Count == 0 || (parameter.ListSellerId.Count != 0 && parameter.ListSellerId.Contains(w.Seller))) //filter theo người bán hàng
                                                                 && (listCustomerFilterId.Count == 0 || (listCustomerFilterId.Count != 0 && listCustomerFilterId.Contains(w.CustomerId))) //filter khách hàng
                                                                 && (parameter.OrderFromDate == null || (parameter.OrderFromDate != null && parameter.OrderFromDate <= w.OrderDate)) //filter theo ngày bán hàng
                                                                 && (parameter.OrderToDate == null || (parameter.OrderToDate != null && w.OrderDate <= parameter.OrderToDate)) //filter theo ngày bán hàng
                                                                 )
                                                     .ToList();

                //tìm theo nhóm hàng
                if (parameter.ListProductGroup.Count != 0)
                {
                    var listProductFilter = listProductEntity.Where(w => w.ProductCategory != null && parameter.ListProductGroup.Contains(w.ProductCategoryId)).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ?? new List<Guid>();
                    listCustomerOrderEntity = listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //nếu có tìm theo mã hàng hóa sản phẩm
                if (!String.IsNullOrWhiteSpace(parameter.ProductCode))
                {
                    var listProductFilter = listProductEntity.Where(w => w.ProductCode.ToLower().Contains(parameter.ProductCode.ToLower())).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ?? new List<Guid>();
                    listCustomerOrderEntity = listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //là quản lý + nếu chọn phòng ban -> lấy phòng ban đó và phòng ban dưới thuộc phân quyền
                if (isManager == true && parameter.OrganizationId != null)
                {
                    var listEmpByOrganizationIdFilter = new List<Guid>();

                    if (listGetAllChild.Contains(parameter.OrganizationId))
                    {
                        //phòng ban được chọn thuộc danh sách phòng ban trong phân quyền
                        var listGetAllChildFilter = new List<Guid?>();
                        listGetAllChildFilter.Add(parameter.OrganizationId);
                        listGetAllChildFilter = GetOrganizationChildren(parameter.OrganizationId, listGetAllChildFilter, listOrganizationentity);

                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChildFilter == null || listGetAllChildFilter.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        var listEmployeeAuthorizationFilter = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                        var listEmployeeAuthorizationFilterId = listEmployeeAuthorizationFilter.Select(w => w.EmployeeId).ToList();

                        listCustomerOrderEntity = listCustomerOrderEntity.Where(w => listEmployeeAuthorizationFilterId.Contains(w.Seller.Value)).ToList();
                    }
                    else
                    {
                        listCustomerOrderEntity = new List<CustomerOrder>();
                    }
                }
                #endregion

                #region get list Top ReVenue result
                var listTopEmployeeRevenue = new List<DataAccess.Models.Order.TopEmployeeRevenueEntityModel>();

                //lấy danh sách nhân viên bán hàng theo danh sách order đã lọc
                var listSellerIdFilter = listCustomerOrderEntity.Where(w => w.Seller != null).Select(w => w.Seller).ToList();
                var listSeller = listAllEmployee.Where(w => listSellerIdFilter.Contains(w.EmployeeId)).ToList();

                listSeller?.ForEach(emp =>
                {
                    decimal? amount = 0; //doanh thu
                    decimal? totalProductInOrder = 0;   //số lượng bán
                    var totalOrder = 0; //số lượng đơn hàng

                    var listCustomerOrderBySeller = listCustomerOrderEntity.Where(w => w.Seller == emp.EmployeeId).ToList() ?? new List<CustomerOrder>();
                    var listOrderId = listCustomerOrderBySeller.Select(w => w.OrderId).ToList() ?? new List<Guid>();
                    var listCustomerOrderDetailBySeller = listCustomerOrderDetailEntity.Where(w => listOrderId.Contains(w.OrderId)).ToList() ?? new List<CustomerOrderDetail>();

                    totalOrder = listCustomerOrderBySeller.Count();
                    totalProductInOrder = listCustomerOrderDetailBySeller.Select(w => w.Quantity).DefaultIfEmpty(0).Sum();

                    //Amount(tiền hàng trước thuế) = SUM(đơn giá sp * số lượng sp * tỷ giá sp - chiết khấu) của sản phẩm trong đơn hàng
                    listCustomerOrderDetailBySeller?.ForEach(item =>
                    {
                        decimal? price = item.UnitPrice * item.Quantity * item.ExchangeRate;
                        decimal? discount = 0;
                        if (item.DiscountType == true)
                        {
                            discount = price * item.DiscountValue / 100;
                        }
                        else
                        {
                            discount = item.DiscountValue;
                        }

                        amount += price - discount;
                    });

                    listTopEmployeeRevenue.Add(new TopEmployeeRevenueEntityModel
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeCode = emp.EmployeeCode,
                        EmployeeName = emp.EmployeeName,
                        TotalOrder = totalOrder,
                        Amount = amount,
                        TotalProductInOrder = totalProductInOrder
                    });
                });

                #region Filter sau khi tính toán dữ liệu
                //doanh thu
                if (parameter.AmountFrom != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => parameter.AmountFrom <= w.Amount).ToList();
                }
                if (parameter.AmountTo != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => w.Amount <= parameter.AmountTo).ToList();
                }
                //số lượng bán
                if (parameter.TotalProductInOrderFrom != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => parameter.TotalProductInOrderFrom <= w.TotalProductInOrder).ToList();
                }
                if (parameter.TotalProductInOrderTo != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => w.TotalProductInOrder <= parameter.TotalProductInOrderTo).ToList();
                }
                //số lượng đơn hàng
                if (parameter.TotalOrderFrom != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => parameter.TotalOrderFrom <= w.TotalOrder).ToList();
                }
                if (parameter.TotalOrderTo != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => w.TotalOrder <= parameter.TotalOrderTo).ToList();
                }
                #endregion

                listTopEmployeeRevenue = listTopEmployeeRevenue.OrderBy(w => w.EmployeeCode.ToLower()).ToList();
                #endregion

                return new SearchTopReVenueResult()
                {
                    ListTopEmployeeRevenue = listTopEmployeeRevenue,
                    Status = true,
                    Message = ""
                };
            }
            catch (Exception e)
            {
                return new SearchTopReVenueResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDataSearchRevenueProductResult GetDataSearchRevenueProduct(GetDataSearchRevenueProductParameter parameter)
        {
            try
            {
                #region danh sách nhóm sản phẩm
                var productCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList() ?? new List<ProductCategory>();
                var productCategoryResult = new List<DataAccess.Models.ProductCategory.ProductCategoryEntityModel>();

                productCategoryEntity?.ForEach(cate =>
                {
                    productCategoryResult.Add(new Models.ProductCategory.ProductCategoryEntityModel
                    {
                        ProductCategoryId = cate.ProductCategoryId,
                        ProductCategoryName = cate.ProductCategoryName,
                        ProductCategoryCode = cate.ProductCategoryCode
                    });
                });
                #endregion

                #region Kho hàng
                var listWarehouseEntity = context.Warehouse.Where(w => w.Active == true).ToList();
                var listWarehouse = new List<Models.WareHouse.WareHouseEntityModel>();
                listWarehouseEntity?.ForEach(warehouse =>
                {
                    listWarehouse.Add(new WareHouseEntityModel
                    {
                        WarehouseId = warehouse.WarehouseId,
                        WarehouseCode = warehouse.WarehouseCode,
                        WarehouseName = warehouse.WarehouseName,

                    });
                });
                #endregion

                #region Lấy danh sách nhân viên bán hàng
                var listEmployeeEntity = context.Employee.ToList();

                var listEmployeeResult = new List<DataAccess.Models.Employee.EmployeeEntityModel>();
                listEmployeeEntity?.ForEach(emp =>
                {
                    listEmployeeResult.Add(new EmployeeEntityModel
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeName = emp.EmployeeName,
                        EmployeeCode = emp.EmployeeCode
                    });
                });
                #endregion

                #region Nhóm khách hàng
                var customerGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA").CategoryTypeId;
                var customerGroupEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == customerGroupTypeId).ToList();
                var listCustomerGroupCategory = new List<Models.CategoryEntityModel>();
                customerGroupEntity?.ForEach(cate =>
                {
                    listCustomerGroupCategory.Add(new CategoryEntityModel
                    {
                        CategoryId = cate.CategoryId,
                        CategoryCode = cate.CategoryCode,
                        CategoryName = cate.CategoryName
                    });
                });
                #endregion

                #region Lấy danh sách khách hàng
                var listCustomerEntity = context.Customer.Where(w => w.Active == true).ToList();
                var listCustomer = new List<Models.Customer.CustomerEntityModel>();
                listCustomerEntity?.ForEach(cus =>
                {
                    listCustomer.Add(new CustomerEntityModel
                    {
                        CustomerId = cus.CustomerId,
                        CustomerName = cus.CustomerName,
                        CustomerCode = cus.CustomerCode
                    });
                });
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                return new GetDataSearchRevenueProductResult()
                {
                    ListEmployee = listEmployeeResult,
                    ListProductCategory = productCategoryResult,
                    ListWarehouse = listWarehouse.OrderBy(o => o.WarehouseName).ToList(),
                    ListCustomerGroupCategory = listCustomerGroupCategory.OrderBy(o => o.CategoryName).ToList(),
                    ListCustomer = listCustomer.OrderBy(o => o.CustomerName).ToList(),
                    InforExportExcel = inforExportExcel,
                    Status = true,
                    Message = ""
                };
            }
            catch (Exception e)
            {
                return new GetDataSearchRevenueProductResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchRevenueProductResult SearchRevenueProduct(SearchRevenueProductParameter parameter)
        {
            try
            {
                #region master data và  Phân quyền
                var listProductEntity = context.Product.Where(w => w.Active == true).ToList() ?? new List<Product>();
                var unitProductTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listProductUnit = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitProductTypeId).ToList();

                var orderStatusEntity = context.OrderStatus.Where(w => w.Active == true).ToList();
                var listStatusIdValid = orderStatusEntity.Where(w => (w.OrderStatusCode == "DLV" || w.OrderStatusCode == "COMP") && w.Active == true)
                                                         .Select(w => w.OrderStatusId)
                                                         .ToList();

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listEmployeeAuthorization = new List<Employee>();
                var listAllCustomer = context.Customer.ToList();

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        listEmployeeAuthorization = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                    }
                }
                else
                {
                    //Nếu không phải quản lý
                    listEmployeeAuthorization = listAllEmployee.Where(w => w.EmployeeId == employeeId).ToList();
                }

                var listCustomerOrderDetailEntity = context.CustomerOrderDetail.Where(w => w.Active == true && w.ProductId != null).ToList(); //chỉ lấy những đơn hàng có sản phẩm
                var listCustomerOrderDetail = listCustomerOrderDetailEntity.ToList();

                //filter theo mã hàng hóa sản phẩm
                var listProductFilter = listProductEntity.ToList();
                var listOrderIdFilterByDetail = new List<Guid>();
                if (!string.IsNullOrWhiteSpace(parameter.ProductCode))
                {

                    listProductFilter = listProductEntity.Where(w => w.ProductCode != null && w.ProductCode.ToLower().Contains(parameter.ProductCode.ToLower())).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    listCustomerOrderDetail = listCustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    listOrderIdFilterByDetail = listCustomerOrderDetail.Select(w => w.OrderId).ToList();
                }
                //filter theo nhóm hàng
                var listOrderFilterByProductCategory = new List<Guid>();
                if (parameter.ProductCategory.Count != 0)
                {
                    listProductFilter = listProductEntity.Where(w => w.ProductCategoryId != null && parameter.ProductCategory.Contains(w.ProductCategoryId)).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    listCustomerOrderDetail = listCustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    listOrderFilterByProductCategory = listCustomerOrderDetail.Select(w => w.OrderId).ToList();
                }

                #region Lấy thông tin để filter
                //nếu có điều kiện tìm theo khách hàng (loại khách hàng, khách hàng , nhóm khách hàng)
                //loại khách hàng
                var listCustomerFilter = listAllCustomer.ToList();
                if (parameter.CustomerType.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerType != null && parameter.CustomerType.Contains(w.CustomerType)).ToList();
                }

                //khách hàng 
                if (parameter.Customer.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerId != null && parameter.Customer.Contains(w.CustomerId)).ToList();
                }

                //nhóm khách hàng
                if (parameter.CustomerGroup.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerGroupId != null && parameter.CustomerGroup.Contains(w.CustomerGroupId)).ToList();
                }

                var listCustomerFilterId = listCustomerFilter.Select(w => w.CustomerId).ToList();
                #endregion

                var listCustomerOrderEntity = context.CustomerOrder
                                                  .Where(
                                                         w => w.Active == true
                                                              && w.Seller != null && listEmployeeAuthorization.Select(emp => emp.EmployeeId).Contains(w.Seller.Value) //phân quyền dữ liệu
                                                              && listStatusIdValid.Contains(w.StatusId.Value) //filter theo trạng thái
                                                              && (listOrderIdFilterByDetail.Count == 0 || (listOrderIdFilterByDetail.Count != 0 && listOrderIdFilterByDetail.Contains(w.OrderId))) //filter theo mã hàng hóa, sản phẩm
                                                              && (listOrderFilterByProductCategory.Count == 0 || (listOrderFilterByProductCategory.Count != 0 && listOrderFilterByProductCategory.Contains(w.OrderId))) //filter theo nhóm hàng
                                                              && (parameter.Seller == null || (parameter.Seller != null && parameter.Seller == w.Seller)) //filter theo người bán hàng
                                                              && (listCustomerFilterId.Count == 0 || (listCustomerFilterId.Count != 0 && listCustomerFilterId.Contains(w.CustomerId))) //filter khách hàng
                                                              && (parameter.OrderFromDate == null || (parameter.OrderFromDate != null && parameter.OrderFromDate <= w.OrderDate)) //filter theo ngày bán hàng
                                                              && (parameter.OrderToDate == null || (parameter.OrderToDate != null && w.OrderDate <= parameter.OrderToDate)) //filter theo ngày bán hàng
                                                              && (parameter.Warehouse.Count == 0 || (w.WarehouseId != null && parameter.Warehouse.Count != 0 && parameter.Warehouse.Contains(w.WarehouseId))) //filter warehouse
                                                              )
                                                  .ToList();
                var listOrderIdFilter = listCustomerOrderEntity.Select(w => w.OrderId).ToList();
                listCustomerOrderDetail = listCustomerOrderDetail.Where(w => listOrderIdFilter.Contains(w.OrderId)).ToList();
                #endregion

                #region Lấy dữ liệu báo cáo theo từng sản phẩm
                var listProductRevenue = new List<DataAccess.Models.Order.ProductRevenueEntityModel>();

                //chỉ hiển thị những sản phẩm có phát sinh đơn hàng bán
                var listProductIdInOrder = listCustomerOrderDetail.Where(w => w.ProductId != null).Select(w => w.ProductId).Distinct().ToList();
                var listProductInOrder = listProductEntity.Where(w => listProductIdInOrder.Contains(w.ProductId)).ToList();

                listProductInOrder?.ForEach(product =>
                {
                    //lấy danh chi tiết đơn hàng theo từng sản phẩm
                    var listCustomerOrderDetailByProduct = listCustomerOrderDetail.Where(w => w.ProductId == product.ProductId).ToList() ?? new List<CustomerOrderDetail>();
                    //lấy danh sách đơn hàng theo từng sản phẩm
                    var listUniqueOrderId = listCustomerOrderDetailByProduct.Select(w => w.OrderId).Distinct().ToList() ?? new List<Guid>();
                    var listCustomerOrderByProduct = listCustomerOrderEntity.Where(w => listUniqueOrderId.Contains(w.OrderId)).ToList() ?? new List<CustomerOrder>();

                    var getSaleRevenueByProduct = GetSaleRevenueByProduct(listCustomerOrderDetailByProduct);

                    var unitName = listProductUnit.FirstOrDefault(f => f.CategoryId == product.ProductUnitId)?.CategoryName ?? "";

                    var productRevenue = new DataAccess.Models.Order.ProductRevenueEntityModel();
                    productRevenue.ProductName = product.ProductName; //Mã sản phẩm
                    productRevenue.ProductCode = product.ProductCode; //Tên sản phẩm
                    productRevenue.UnitName = unitName; //tên đơn vị tính
                    productRevenue.OrderCount = listCustomerOrderByProduct.Count(); //Số lượng đơn hàng
                    productRevenue.ProductInOrderCount = listCustomerOrderDetailByProduct.Select(w => w.Quantity).DefaultIfEmpty(0).Sum() ?? 0; //Số lượng bán
                    productRevenue.SaleRevenue = getSaleRevenueByProduct.SaleRevenueByProduct; //Doanh thu
                    productRevenue.TotalDiscount = getSaleRevenueByProduct.AmountDiscountProduct; //Chiết khấu
                    productRevenue.TotalPriceInitial = getSaleRevenueByProduct.AmountPriceInitial; // Tiền vốn

                    /* vì pending phần phiếu nhập hàng bán trả lại nên số liệu liên quan phần này = 0 */
                    productRevenue.ProductRefundCount = 0; //Số lượng trả lại
                    productRevenue.SaleRevenueRefund = 0; //Doanh thu trả lại
                    productRevenue.TotalPriceInitialRefund = 0; //Tiền vốn trả lại

                    productRevenue.TotalGrossProfit = (productRevenue.SaleRevenue - productRevenue.SaleRevenueRefund - productRevenue.TotalDiscount - productRevenue.TotalPriceInitial + productRevenue.TotalPriceInitialRefund); //Lãi gộp = Doanh thu bán-Doanh thu trả lại- chiết khấu- Tiền vốn+Tiền vốn trả lại

                    if (productRevenue.SaleRevenue - productRevenue.SaleRevenueRefund == 0)
                    {
                        productRevenue.TotalProfitPerSaleRevenue = 0;
                    }
                    else
                    {
                        productRevenue.TotalProfitPerSaleRevenue = (productRevenue.TotalGrossProfit) / (productRevenue.SaleRevenue - productRevenue.SaleRevenueRefund) * 100; //% lãi/DT =Lãi gộp/(Doanh thu bán- Doanh thu trả lại)
                    }

                    if (productRevenue.TotalPriceInitial - productRevenue.TotalPriceInitialRefund == 0)
                    {
                        productRevenue.TotalProfitPerPriceInitial = 0;
                    }
                    else
                    {
                        productRevenue.TotalProfitPerPriceInitial = (productRevenue.TotalGrossProfit) / (productRevenue.TotalPriceInitial - productRevenue.TotalPriceInitialRefund) * 100; //% lãi/Giá vốn =Lãi gộp/(Tiền vốn- Tiền vốn trả lại)
                    }

                    listProductRevenue.Add(productRevenue);
                });

                listProductRevenue = listProductRevenue.OrderBy(w => w.ProductCode).ToList();
                #endregion

                #region Nếu filter theo doanh thu, số lượng bán, số lượng đơn hàng, số lượng trả lại
                //doanh thu
                if (parameter.SaleRevenueFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.SaleRevenueFrom <= w.SaleRevenue).ToList();
                }
                if (parameter.SaleRevenueTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.SaleRevenue <= parameter.SaleRevenueFrom).ToList();
                }
                //số lượng bán
                if (parameter.ProductInOrderCountFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.ProductInOrderCountFrom <= w.ProductInOrderCount).ToList();
                }

                if (parameter.ProductInOrderCountTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.ProductInOrderCount <= parameter.ProductInOrderCountTo).ToList();
                }
                //số lượng đơn hàng
                if (parameter.OrderCountFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.OrderCountFrom <= w.OrderCount).ToList();
                }

                if (parameter.OrderCountTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.OrderCount <= parameter.OrderCountTo).ToList();
                }

                // số lượng trả lại
                if (parameter.ProductRefundCountFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.ProductRefundCountFrom <= w.ProductRefundCount).ToList();
                }

                if (parameter.ProductRefundCountTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.ProductRefundCount <= parameter.ProductRefundCountTo).ToList();
                }
                #endregion

                return new SearchRevenueProductResult()
                {
                    ListProductRevenue = listProductRevenue,
                    Status = true,
                    Message = ""
                };
            }
            catch (Exception e)
            {
                return new SearchRevenueProductResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        //hàm tính tổng doanh số theo từng sản phẩm
        private SaleRevenueByProductModel GetSaleRevenueByProduct(List<CustomerOrderDetail> listCustomerOrderDetailByProduct)
        {
            var result = new SaleRevenueByProductModel();

            listCustomerOrderDetailByProduct?.ForEach(order_detail =>
            {
                //decimal? amountProduct = 0;
                //decimal? amountDiscountProduct = 0;
                //decimal? amountVatProduct = 0;

                decimal? price = order_detail.UnitPrice * order_detail.Quantity;
                decimal? discount = 0;
                if (order_detail.DiscountType == true)
                {
                    discount = price * order_detail.DiscountValue / 100;
                }
                else
                {
                    discount = order_detail.DiscountValue;
                }

                result.SaleRevenueByProduct += price; //tổng tiền theo từng sản phẩm
                result.AmountDiscountProduct += discount; //tổng chiết khấu
                result.AmountPriceInitial += order_detail.PriceInitial.HasValue ? order_detail.PriceInitial * order_detail.Quantity : 0; //tổng tiền vốn ban đầu
            });

            return result;
        }

        public GetListOrderDetailByOrderResult GetListOrderDetailByOrder(GetListOrderDetailByOrderParameter parameter)
        {
            try
            {
                var listOrderDetail = new List<CustomerOrderDetailEntityModel>();
                listOrderDetail = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.OrderId).Select(y =>
                    new CustomerOrderDetailEntityModel
                    {
                        OrderDetailId = y.OrderDetailId,
                        OrderId = y.OrderId,
                        ProductId = y.ProductId,
                        ProductName = "",
                        ExchangeRate = y.ExchangeRate,
                        Quantity = y.Quantity,
                        UnitPrice = y.UnitPrice,
                        SumAmount = y.Quantity.Value * y.UnitPrice.Value * y.ExchangeRate.Value
                    }).ToList();

                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();
                listOrderDetail.ForEach(item =>
                {
                    var product = listAllProduct.FirstOrDefault(x => x.ProductId == item.ProductId);
                    item.ProductName = product?.ProductName ?? "";
                });

                return new GetListOrderDetailByOrderResult()
                {
                    Status = true,
                    Message = "Success",
                    ListOrderDetail = listOrderDetail
                };
            }
            catch (Exception e)
            {
                return new GetListOrderDetailByOrderResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetListProductWasOrderResult GetListProductWasOrder(GetListProductWasOrderParameter parameter)
        {
            try
            {
                var listProductWasOrder = new List<ProductEntityModel>();

                //Lấy ra các orderId gắn với Điểm
                var listOrderId = context.CustomerOrderLocalPointMapping
                    .Where(x => x.LocalPointId == parameter.LocalPointId).Select(y => y.OrderId).Distinct().ToList();

                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                var statusDRA = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DRA");

                //Lấy ra Order có trạng thái Nháp ở thời điểm hiện tại
                var order = context.CustomerOrder.FirstOrDefault(x =>
                    listOrderId.Contains(x.OrderId) && x.StatusId == statusDRA.OrderStatusId);

                //Lấy list sản phẩm theo order
                listProductWasOrder = context.CustomerOrderDetail.Where(x => x.OrderId == order.OrderId).Select(y =>
                    new ProductEntityModel
                    {
                        ProductId = y.ProductId.Value,
                        Quantity = y.Quantity
                    }).ToList();

                return new GetListProductWasOrderResult()
                {
                    Status = true,
                    Message = "Success",
                    ListProductWasOrder = listProductWasOrder,
                    OrderId = order.OrderId
                };
            }
            catch (Exception e)
            {
                return new GetListProductWasOrderResult()
                {
                    Status = true,
                    Message = e.Message
                };
            }
        }

        public UpdateCustomerServiceResult UpdateCustomerService(UpdateCustomerServiceParameter parameter)
        {
            try
            {
                var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);

                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                var statusDRA = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DRA");

                if (order.StatusId != statusDRA.OrderStatusId)
                {
                    return new UpdateCustomerServiceResult()
                    {
                        Status = false,
                        Message = "Đơn hàng đã được thanh toán"
                    };
                }

                var listOrderDetail = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.OrderId).ToList();

                context.CustomerOrderDetail.RemoveRange(listOrderDetail);
                context.SaveChanges();

                decimal totalAmount = 0;
                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    item.CreatedById = order.CreatedById;
                    item.CreatedDate = order.CreatedDate;
                    item.UpdatedById = parameter.UserId;
                    item.UpdatedDate = DateTime.Now;
                    item.Active = true;
                    item.OrderDetailId = Guid.NewGuid();
                    foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                    {
                        itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                    }

                    totalAmount += item.Quantity.Value * item.UnitPrice.Value * item.ExchangeRate.Value;
                });

                order.Amount = totalAmount;

                context.CustomerOrder.Update(order);
                context.SaveChanges();

                context.CustomerOrderDetail.AddRange(parameter.ListCustomerOrderDetail);
                context.SaveChanges();

                return new UpdateCustomerServiceResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateCustomerServiceResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDataProfitByCustomerResult GetDataProfitByCustomer(GetDataProfitByCustomerParameter parameter)
        {
            try
            {
                #region Lấy danh sách khách hàng
                var listCustomerEntity = context.Customer.Where(w => w.Active == true).ToList();
                var listCustomer = new List<Models.Customer.CustomerEntityModel>();
                listCustomerEntity?.ForEach(cus =>
                {
                    listCustomer.Add(new CustomerEntityModel
                    {
                        CustomerId = cus.CustomerId,
                        CustomerName = cus.CustomerName,
                        CustomerCode = cus.CustomerCode
                    });
                });
                #endregion

                #region Nhóm khách hàng
                var customerGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA").CategoryTypeId;
                var customerGroupEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == customerGroupTypeId).ToList();
                var listCustomerGroupCategory = new List<Models.CategoryEntityModel>();
                customerGroupEntity?.ForEach(cate =>
                {
                    listCustomerGroupCategory.Add(new CategoryEntityModel
                    {
                        CategoryId = cate.CategoryId,
                        CategoryCode = cate.CategoryCode,
                        CategoryName = cate.CategoryName
                    });
                });
                #endregion

                #region Nhóm sản phẩm, dịch vụ
                var listProductCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList();
                var listProductCategory = new List<Models.ProductCategory.ProductCategoryEntityModel>();
                listProductCategoryEntity?.ForEach(cate =>
                {
                    listProductCategory.Add(new ProductCategoryEntityModel
                    {
                        ProductCategoryId = cate.ProductCategoryId,
                        ProductCategoryName = cate.ProductCategoryName,
                        ProductCategoryCode = cate.ProductCategoryCode
                    });
                });
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                #region Lấy danh sách hợp đồng
                var contractStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "THD").CategoryTypeId;
                var listContractStatus = context.Category.Where(w => w.Active == true && w.CategoryTypeId == contractStatusTypeId).ToList();
                var listInvalidStatus = new List<string>();
                listInvalidStatus.Add("MOI"); //loại những hợp đồng có trạng thái mới
                var listContractStatusValid = listContractStatus.Where(w => !listInvalidStatus.Contains(w.CategoryCode)).ToList();
                var listContractStatusValidId = listContractStatusValid.Select(w => w.CategoryId).ToList();

                var listContractEntity = context.Contract.Where(w => w.Active == true && w.StatusId != null && listContractStatusValidId.Contains(w.StatusId.Value)).ToList();
                var listContract = new List<DataAccess.Models.Contract.ContractEntityModel>();
                listContractEntity?.ForEach(contract =>
                {
                    listContract.Add(new ContractEntityModel
                    {
                        ContractId = contract.ContractId,
                        ContractCode = contract.ContractCode,
                        Amount = contract.Amount
                    });
                });
                #endregion

                return new GetDataProfitByCustomerResult()
                {
                    ListCustomer = listCustomer.OrderBy(w => w.CustomerName).ToList(),
                    ListCustomerGroupCategory = listCustomerGroupCategory.OrderBy(w => w.CategoryName).ToList(),
                    ListProductCategory = listProductCategory.OrderBy(w => w.ProductCategoryName).ToList(),
                    InforExportExcel = inforExportExcel,
                    ListContract = listContract.OrderBy(w => w.ContractCode).ToList(),
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetDataProfitByCustomerResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchProfitCustomerResult SearchProfitCustomer(SearchProfitCustomerParameter parameter)
        {
            try
            {
                #region master data
                var orderStatusEntity = context.OrderStatus.Where(w => w.Active == true).ToList();
                var listStatusIdValid = orderStatusEntity.Where(w => (w.OrderStatusCode == "DLV" || w.OrderStatusCode == "COMP") && w.Active == true)
                                                        .Select(w => w.OrderStatusId)
                                                        .ToList();
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listEmployeeAuthorization = new List<Employee>();
                var listAllCustomer = context.Customer.ToList();

                var listProductEntity = context.Product.ToList();
                var listProductCategoryEntity = context.ProductCategory.ToList();

                var listCustomerOrderDetailEntity = context.CustomerOrderDetail.Where(w => w.Active == true).ToList();
                #endregion

                #region phân quyền dữ liệu

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó

                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        listEmployeeAuthorization = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                    }
                }
                else
                {
                    //Nếu không phải quản lý
                    listEmployeeAuthorization = listAllEmployee.Where(w => w.EmployeeId == employeeId).ToList();
                }

                #endregion

                #region nếu có điều kiện tìm theo khách hàng (loại khách hàng, khách hàng , nhóm khách hàng)

                //loại khách hàng
                var listCustomerFilter = listAllCustomer.ToList();
                if (parameter.ListCustomerType.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerType != null && parameter.ListCustomerType.Contains(w.CustomerType)).ToList();
                }

                //khách hàng 
                if (parameter.ListCustomer.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerId != null && parameter.ListCustomer.Contains(w.CustomerId)).ToList();
                }

                //nhóm khách hàng
                if (parameter.ListCustomerGroup.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerGroupId != null && parameter.ListCustomerGroup.Contains(w.CustomerGroupId)).ToList();
                }

                var listCustomerFilterId = listCustomerFilter.Select(w => w.CustomerId).ToList();

                #endregion

                #region lọc ra customer order theo điều kiện tìm kiếm

                //chỉ lấy những đơn hàng có trạng thái đóng hoặc đơn hàng bán
                var listCustomerOrderEntity = context.CustomerOrder
                    .Where(
                        w => w.Active == true
                             && w.Seller != null &&
                             listEmployeeAuthorization.Select(emp => emp.EmployeeId)
                                 .Contains(w.Seller.Value) //phân quyền dữ liệu
                             && listStatusIdValid.Contains(w.StatusId.Value) //filter theo trạng thái
                             && (listCustomerFilterId.Count == 0 ||
                                 (listCustomerFilterId.Count != 0 && listCustomerFilterId.Contains(w.CustomerId))
                             ) //filter khách hàng
                             && (string.IsNullOrWhiteSpace(parameter.OrderCode) ||
                                 (!string.IsNullOrWhiteSpace(parameter.OrderCode) &&
                                  !string.IsNullOrWhiteSpace(w.OrderCode) &&
                                  w.OrderCode.ToLower().Trim().Contains(parameter.OrderCode.ToLower().Trim()))
                             ) //filter theo mã đơn hàng
                             && (parameter.OrderFromDate == null ||
                                 (parameter.OrderFromDate != null && parameter.OrderFromDate <= w.OrderDate)
                             ) //filter theo ngày bán hàng
                             && (parameter.OrderToDate == null ||
                                 (parameter.OrderToDate != null &&
                                  w.OrderDate <= parameter.OrderToDate)) //filter theo ngày bán hàng
                    )
                    .ToList();

                //nếu có điều kiện tìm theo nhóm hàng
                if (parameter.ListProductCategory.Count != 0)
                {
                    var listProductFilter = listProductEntity.Where(w =>
                            w.ProductCategory != null && parameter.ListProductCategory.Contains(w.ProductCategoryId))
                        .ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail
                        .Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ??
                                          new List<Guid>();
                    listCustomerOrderEntity =
                        listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //nếu có điều kiện tìm theo mã hàng hóa, sản phẩm
                if (!string.IsNullOrWhiteSpace(parameter.ProductCode))
                {
                    var listProductFilter = listProductEntity.Where(w =>
                        w.ProductCode != null && w.ProductCode.ToLower().Trim()
                            .Contains(parameter.ProductCode.ToLower().Trim())).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail
                        .Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ??
                                          new List<Guid>();
                    listCustomerOrderEntity =
                        listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //nếu có điều kiện tìm theo mã hợp đồng
                if (parameter.ListContract.Count != 0)
                {
                    var listContractIdFilter = context.Contract
                        .Where(w => w.Active == true && parameter.ListContract.Contains(w.ContractId))
                        .Select(w => w.ContractId).ToList();
                    listCustomerOrderEntity = listCustomerOrderEntity.Where(w =>
                        w.OrderContractId != null && listContractIdFilter.Contains(w.OrderContractId.Value)).ToList();
                }

                //nếu tìm theo số báo giá
                if (!string.IsNullOrWhiteSpace(parameter.QuoteCode))
                {
                    var listQuoteIdFilter = context.Quote
                        .Where(w => w.Active == true && w.QuoteCode != null && w.QuoteCode.ToLower().Trim()
                                        .Contains(parameter.QuoteCode.ToLower().Trim())).Select(w => w.QuoteId)
                        .ToList();
                    listCustomerOrderEntity = listCustomerOrderEntity
                        .Where(w => w.QuoteId != null && listQuoteIdFilter.Contains(w.QuoteId.Value)).ToList();
                }

                #endregion

                #region get list doanh số theo nhân viên

                var listSearchProfitCustomer = new List<DataAccess.Models.CustomerOrder.SearchProfitCustomerModel>();
                //chỉ hiển thị nhân viên có customer order
                var listCustomerInOrderId = listCustomerOrderEntity.Where(w => w.CustomerId != null)
                    .Select(w => w.CustomerId).Distinct().ToList();
                var listCustomerInOrder =
                    listAllCustomer.Where(w => listCustomerInOrderId.Contains(w.CustomerId)).ToList();

                var listCustomerOrderId = listCustomerOrderEntity.Select(w => w.OrderId).ToList();
                var listOrderDetail = context.CustomerOrderDetail
                    .Where(w => w.Active == true && listCustomerOrderId.Contains(w.OrderId)).ToList(); //lấy danh sách chi tiết đơn hàng sau khi filter


                listCustomerInOrder.ForEach(customer =>
                {
                    var listOrderByCustomer = listCustomerOrderEntity
                        .Where(w => w.CustomerId != null && w.CustomerId == customer.CustomerId).ToList();
                    var listOrderByCustomerId = listOrderByCustomer.Select(w => w.OrderId).ToList();
                    var listOrderDetailByCustomer =
                        listOrderDetail.Where(w => listOrderByCustomerId.Contains(w.OrderId)).ToList();

                    var saleRevenueByCustomer = GetSaleRevenueByCustomer(listOrderByCustomer, listOrderDetailByCustomer);

                    listSearchProfitCustomer.Add(new Models.CustomerOrder.SearchProfitCustomerModel
                    {
                        CustomerName = customer.CustomerName?.Trim() ?? "",
                        CustomerCode = customer.CustomerCode?.Trim() ?? "",
                        SaleRevenue = saleRevenueByCustomer.SaleRevenue,
                        TotalPriceInitial = saleRevenueByCustomer.TotalPriceInitial,
                        TotalGrossProfit = saleRevenueByCustomer.TotalGrossProfit,
                        TotalProfitPerSaleRevenue = saleRevenueByCustomer.TotalProfitPerSaleRevenue,
                        TotalProfitPerPriceInitial = saleRevenueByCustomer.TotalProfitPerPriceInitial
                    });
                });

                //filter by doanh thu
                if (parameter.SaleRevenueFrom != null)
                {
                    listSearchProfitCustomer = listSearchProfitCustomer
                        .Where(w => parameter.SaleRevenueFrom <= w.SaleRevenue).ToList();
                }
                if (parameter.SaleRevenueTo != null)
                {
                    listSearchProfitCustomer = listSearchProfitCustomer
                        .Where(w => w.SaleRevenue <= parameter.SaleRevenueTo).ToList();
                }

                //sắp xếp theo mã khách hàng
                listSearchProfitCustomer = listSearchProfitCustomer.OrderBy(w => w.CustomerCode).ToList();

                #endregion

                return new SearchProfitCustomerResult()
                {
                    ListSearchProfitCustomer = listSearchProfitCustomer,
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new SearchProfitCustomerResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        //hàm tính doanh số theo từng khách hàng
        public SaleRevenueByCustomerModel GetSaleRevenueByCustomer(List<CustomerOrder> ListCustomerOrder, List<CustomerOrderDetail> ListCustomerOrderDetail)
        {
            decimal? saleRevenue = 0; //doanh thu
            decimal? totalPriceInitial = 0; //Tiền vốn

            //Tổng tiền hàng trước thuế: TotalSumAmountProduct = SUM(đơn giá sp * số lượng sp * tỷ giá sp - chiết khấu)
            //chỉ lấy những chi tiết có số lượng và giá vốn hợp lệ
            ListCustomerOrderDetail = ListCustomerOrderDetail.Where(w => w.Quantity != null && w.PriceInitial != null).ToList();

            ListCustomerOrderDetail?.ForEach(order_detail =>
            {
                //tiền vốn
                totalPriceInitial += order_detail.Quantity * order_detail.PriceInitial;

                //Doanh thu (Tổng tiền vốn): TotalPriceInitial = SUM(số lượng sp * giá vốn) của chi tiết đơn hàng
                decimal? price = order_detail.UnitPrice * order_detail.Quantity;
                decimal? discount = 0;
                if (order_detail.DiscountType == true)
                {
                    discount = price * order_detail.DiscountValue / 100;
                }
                else
                {
                    discount = order_detail.DiscountValue;
                }

                saleRevenue += price;
            });

            //Lãi gộp = Doanh thu - Tiền vốn
            var totalGrossProfit = saleRevenue - totalPriceInitial;
            // %lãi/DT = = Lãi gộp/ Doanh thu
            decimal? totalProfitPerSaleRevenue = 0;
            if (saleRevenue == 0)
            {
                totalProfitPerSaleRevenue = 0;
            }
            else
            {
                totalProfitPerSaleRevenue = (totalGrossProfit / saleRevenue) * 100;
            }

            //%Lãi/Giá vốn = Lãi gộp/ Tiền vốn
            decimal? totalProfitPerPriceInitial = 0;
            if (totalPriceInitial == 0)
            {
                totalProfitPerPriceInitial = 0;
            }
            else
            {
                totalProfitPerPriceInitial = (totalGrossProfit / totalPriceInitial) * 100;
            }

            var saleRevenueByCustomer = new SaleRevenueByCustomerModel();
            saleRevenueByCustomer.SaleRevenue = saleRevenue;
            saleRevenueByCustomer.TotalPriceInitial = totalPriceInitial;
            saleRevenueByCustomer.TotalGrossProfit = totalGrossProfit;
            saleRevenueByCustomer.TotalProfitPerSaleRevenue = totalProfitPerSaleRevenue;
            saleRevenueByCustomer.TotalProfitPerPriceInitial = totalProfitPerPriceInitial;

            return saleRevenueByCustomer;
        }

        //Function lấy phòng ban cấp dưới
        private List<Guid?> GetOrganizationChildren(Guid? id, List<Guid?> list, List<Organization> ListOrganization)
        {
            var Organization = ListOrganization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                GetOrganizationChildren(item.OrganizationId, list, ListOrganization);
            });

            return list;
        }
    }

    public class ListCategoryId
    {
        public Guid ParentProductCategoryId { get; set; }
        public string ParentProductCategoryName { get; set; }
        public List<Guid> ListChildrent { get; set; }
    }

    public class ListCategoryResult
    {
        public Guid ParentProductCategoryId { get; set; }
        public string ParentProductCategoryName { get; set; }
        public decimal Total { get; set; }
    }

    public class EmployeeModel
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string OrganizationName { get; set; }
        public decimal Amount { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid UserId { get; set; }
    }

    public class StatusOrderModel
    {
        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
        public decimal Amount { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
    }

    public class MonthOrderModel
    {
        public int STT { get; set; }
        public string MonthName { get; set; }
        public decimal Amount { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime TimeNode { get; set; }
    }

    public class ProductCategoryModel
    {
        public Guid ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public decimal Total { get; set; }
    }

    public class DateOfQuarter
    {
        public DateTime FirstDateOfQuarter { get; set; }
        public DateTime LastDateOfQuarter { get; set; }
    }

    public class SaleRevenueByProductModel
    {
        public decimal? SaleRevenueByProduct { get; set; } //Doanh thu
        public decimal? AmountDiscountProduct { get; set; } //Chiết khấu
        public decimal? AmountPriceInitial { get; set; } //tổng giá vốn

        public SaleRevenueByProductModel()
        {
            SaleRevenueByProduct = 0;
            AmountDiscountProduct = 0;
            AmountPriceInitial = 0;
        }
    }

    public class SaleRevenueByCustomerModel
    {
        public decimal? SaleRevenue { get; set; } //Doanh thu
        public decimal? TotalPriceInitial { get; set; } //Tiền vốn
        public decimal? TotalGrossProfit { get; set; } //Lãi gộp
        public decimal? TotalProfitPerSaleRevenue { get; set; } // % (lãi/doanh thu)
        public decimal? TotalProfitPerPriceInitial { get; set; } // % (lãi/giá vốn)

        public SaleRevenueByCustomerModel()
        {
            SaleRevenue = 0;
            TotalPriceInitial = 0;
            TotalGrossProfit = 0;
            TotalProfitPerSaleRevenue = 0;
            TotalProfitPerPriceInitial = 0;
        }
    }
}
