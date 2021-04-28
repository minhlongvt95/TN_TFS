using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Messages.Results.Lead;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using System.Text;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class LeadSearchDAO : BaseDAO, ILeadSearchDataAccess
    {
        private IHostingEnvironment hostingEnvironment;
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


        public LeadSearchDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
            this.Configuration = iconfiguration;
        }

        /// <summary>
        /// GetLeadById
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public GetLeadByIdResult GetLeadById(GetLeadByIdParamater parameter)
        {
            // Add history access to trace log
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.LEAD, "Get Lead By Id", parameter.UserId);
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.COMPANY, "Get Company By Id", parameter.UserId);
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.CONTACT, "Get Contact By Id", parameter.UserId);

            // Get lead by leadId
            Lead lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.LeadId.Value);

            //Get all company
            var listCompany = context.Company.ToList();

            // Get company by companyId
            Company company = context.Company.FirstOrDefault(l => l.CompanyId == lead.CompanyId);

            // Get contact by contactId
            //Contact contact = context.Contact.FirstOrDefault(l => l.ContactId == parameter.ContactId.Value);
            Contact contact = context.Contact.FirstOrDefault(l => l.ObjectId == parameter.LeadId.Value);

            var quote = context.Quote.ToList();
            var note = context.Note.ToList();

            // Get master data by code
            var category = context.Category.FirstOrDefault(c => c.CategoryId == lead.InterestedGroupId)?.CategoryName;
            var statusCode = context.Category.FirstOrDefault(c => c.CategoryId == lead.StatusId)?.CategoryCode;
            var potentialName = context.Category.FirstOrDefault(c => c.CategoryId == lead.PotentialId)?.CategoryName;
            var status_name = context.Category.FirstOrDefault(c => c.CategoryId == lead.StatusId)?.CategoryName;
            var pic_name = context.Employee.FirstOrDefault(e => e.EmployeeId == lead.PersonInChargeId)?.EmployeeName;
            var status = context.Category.Where(ct => ct.CategoryTypeId == context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == parameter.StatusCode).CategoryTypeId).ToList();
            var potential = context.Category.Where(ct => ct.CategoryTypeId == context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == parameter.PotentialCode).CategoryTypeId).ToList();
            var province = context.Province.FirstOrDefault(p => p.ProvinceId == contact.ProvinceId);
            var district = context.District.FirstOrDefault(d => d.DistrictId == contact.DistrictId);
            var ward = context.Ward.FirstOrDefault(w => w.WardId == contact.WardId);

            // Get all employee
            //var employee = context.Employee.Where(e => e.Active == true).ToList();
            // Update SRS: lấy tất cả nhân viên
            var employee = context.Employee.ToList();

            // Get position of employee
            //var positionName = context.Position.FirstOrDefault(p => p.PositionId == context.Employee.FirstOrDefault(e =>
            //e.EmployeeId == lead.PersonInChargeId).PositionId)?.PositionName;

            // Get responsible person
            var responsiblePerson = context.Employee.FirstOrDefault(e => e.EmployeeId == lead.PersonInChargeId)
                ?.EmployeeId;

            // Get interested group
            var interestedList = context.Category.Where(c => c.CategoryTypeId == context.CategoryType.FirstOrDefault(cc =>
                                 cc.CategoryTypeCode == parameter.InterestedCode).CategoryTypeId).ToList();

            // Get payment method
            var paymentMethod = context.Category.Where(c => c.CategoryTypeId == context.CategoryType.FirstOrDefault(cc =>
                                cc.CategoryTypeCode == parameter.PaymentCode).CategoryTypeId).ToList();

            // Get payment name
            var paymentName = context.Category.FirstOrDefault(c => c.CategoryId == lead.PaymentMethodId)?.CategoryName;

            // Get interested name
            var interestedName = context.Category.FirstOrDefault(c => c.CategoryId == lead.InterestedGroupId)?.CategoryName;

            // Get genders
            var genders = context.Category.Where(c => c.CategoryTypeId == context.CategoryType.FirstOrDefault(cc =>
                                cc.CategoryTypeCode == "GTI").CategoryTypeId)?.ToList() ?? new List<Category>();

            var countLead = CheckCountInformationLead(parameter.LeadId, quote, note);

            var saleBidding = context.SaleBidding.FirstOrDefault(c => c.LeadId == lead.LeadId);


            return new GetLeadByIdResult
            {
                Status = true,
                Lead = lead,
                ListCompany = listCompany,
                Company = company,
                Contact = contact,
                InterestedGroupName = category,
                Employee = employee,
                StatusCategory = status,
                Potential = potential,
                PotentialName = potentialName,
                Status_Name = status_name,
                PIC_Name = pic_name,
                Status_Code = statusCode,
                ResponsibleName = responsiblePerson,
                PaymentMethod = paymentMethod,
                InterestedList = interestedList,
                PaymentMethodName = paymentName,
                InterestedName = interestedName,
                Genders = genders,
                CountLead = countLead,
                FullAddress = ward?.WardType + " " + ward?.WardName + ", " + district?.DistrictType + " " + district?.DistrictName + ", " + province?.ProvinceType + " " + province?.ProvinceName
            };
        }

        public CreateLeadResult CreateLead(CreateLeadParameter parameter)
        {
            try
            {
                parameter.Lead.LeadId = Guid.NewGuid();

                //mặc định trạng thái là NHÁP
                var leadStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "CHS")
                    .CategoryTypeId;
                var leadStatusDraftId =
                    context.Category
                        .FirstOrDefault(f => f.CategoryTypeId == leadStatusTypeId && f.CategoryCode == "DRAFT")
                        ?.CategoryId ?? new Guid();

                parameter.Lead.CreatedDate = DateTime.Now;
                parameter.Lead.StatusId = leadStatusDraftId;
                parameter.Lead.RequirementDetail = parameter.Lead.RequirementDetail != null
                    ? parameter.Lead.RequirementDetail.Trim()
                    : parameter.Lead.RequirementDetail;
                parameter.Lead.WaitingForApproval = false;
                parameter.Lead.LeadCode = GenerateCode();

                parameter.Contact.ContactId = Guid.NewGuid();
                parameter.Contact.ObjectId = parameter.Lead.LeadId;
                parameter.Contact.ObjectType = ObjectType.LEAD;
                parameter.Contact.FirstName = parameter.Contact.FirstName.Trim();
                parameter.Contact.LastName = parameter.Contact.LastName != null
                    ? parameter.Contact.LastName.Trim()
                    : parameter.Contact.LastName;
                parameter.Contact.Address = parameter.Contact.Address != null
                    ? parameter.Contact.Address.Trim()
                    : parameter.Contact.Address;
                parameter.Contact.Email = parameter.Contact.Email != null
                    ? parameter.Contact.Email.Trim()
                    : parameter.Contact.Email;

                string picName = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.Lead.PersonInChargeId)
                    ?.EmployeeName;
                string potential = context.Category.FirstOrDefault(c => c.CategoryId == parameter.Lead.PotentialId)
                    ?.CategoryName;
                string statusName = context.Category.FirstOrDefault(c => c.CategoryId == parameter.Lead.StatusId)
                    ?.CategoryName;

                if (parameter.IsCreateCompany)
                {
                    Company company = new Company()
                    {
                        CompanyId = Guid.NewGuid(),
                        CompanyName = parameter.CompanyName.Trim(),
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        Active = true
                    };
                    parameter.Lead.CompanyId = company.CompanyId;
                    context.Company.Add(company);
                }

                #region Save to [LeadInterestedGroupMapping]

                var listLeadInterestedGroupMapping = new List<LeadInterestedGroupMapping>();
                parameter.ListInterestedId?.ForEach(item =>
                {
                    listLeadInterestedGroupMapping.Add(new LeadInterestedGroupMapping
                    {
                        LeadInterestedGroupMappingId = Guid.NewGuid(),
                        LeadId = parameter.Lead.LeadId,
                        InterestedGroupId = item.Value,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });

                #endregion

                #region Add List Lead contact

                var listContact = new List<Contact>();
                parameter.ListContactDAO?.ForEach(_contact =>
                {
                    listContact.Add(new Contact
                    {
                        ContactId = Guid.NewGuid(),
                        ObjectId = parameter.Lead.LeadId,
                        ObjectType = "LEA_CON",
                        FirstName = _contact.FirstName,
                        LastName = _contact.LastName,
                        Gender = _contact.Gender,
                        DateOfBirth = _contact.DateOfBirth,
                        Phone = _contact.Phone,
                        Email = _contact.Email,
                        Address = _contact.Address,
                        Note = _contact.Note,
                        Role = _contact.Role,
                        RelationShip = _contact.RelationShip,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });

                #endregion

                #region Add Product detail

                var listLeadDetail = new List<LeadDetail>();
                var listProductAttribute = new List<LeadProductDetailProductAttributeValue>();

                parameter?.ListLeadDetail.ForEach(_leadDetail =>
                {
                    var newLeadDetailId = Guid.NewGuid();

                    var newLeadDetail = new LeadDetail
                    {
                        LeadDetailId = newLeadDetailId,
                        LeadId = parameter.Lead.LeadId,
                        VendorId = _leadDetail.VendorId,
                        ProductId = _leadDetail.ProductId,
                        Quantity = _leadDetail.Quantity,
                        UnitPrice = _leadDetail.UnitPrice,
                        CurrencyUnit = _leadDetail.CurrencyUnit,
                        ExchangeRate = _leadDetail.ExchangeRate,
                        Vat = _leadDetail.Vat,
                        DiscountType = _leadDetail.DiscountType,
                        DiscountValue = _leadDetail.DiscountValue,
                        Description = _leadDetail.Description,
                        OrderDetailType = _leadDetail.OrderDetailType,
                        UnitId = _leadDetail.UnitId,
                        IncurredUnit = _leadDetail.IncurredUnit,
                        ProductName = _leadDetail.ProductName,
                        OrderNumber = _leadDetail.OrderNumber,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null,
                    };
                    listLeadDetail.Add(newLeadDetail);
                    //them thuoc tinh neu co
                    _leadDetail?.LeadProductDetailProductAttributeValue?.ForEach(attr =>
                    {
                        var productAtribute = new LeadProductDetailProductAttributeValue
                        {
                            LeadProductDetailProductAttributeValue1 = Guid.NewGuid(),
                            LeadDetailId = newLeadDetail.LeadDetailId,
                            ProductId = attr.ProductId,
                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                        };
                        listProductAttribute.Add(productAtribute);
                    });
                });

                #endregion

                context.Lead.Add(parameter.Lead);
                context.Contact.Add(parameter.Contact);
                context.Contact.AddRange(listContact);

                context.LeadInterestedGroupMapping.AddRange(listLeadInterestedGroupMapping);

                context.LeadDetail.AddRange(listLeadDetail);
                context.LeadProductDetailProductAttributeValue.AddRange(listProductAttribute);
                context.SaveChanges();

                #region Gửi thông báo

                NotificationHelper.AccessNotification(context, TypeModel.Lead, "CRE", new Lead(),
                    parameter.Lead, true);

                #endregion

                #region Get Infor Lead Created

                var SendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();
                try
                {
                    SendEmailEntityModel.ListSendToEmail = new List<string>();
                    //lead infor
                    SendEmailEntityModel.LeadName = parameter.Contact.FirstName + " " + parameter.Contact.LastName;
                    SendEmailEntityModel.LeadEmail = parameter.Contact.Email;
                    SendEmailEntityModel.LeadPhone = parameter.Contact.Phone;
                    //get address lead
                    var province = context.Province.Where(w => w.ProvinceId == parameter.Contact.ProvinceId)
                        .FirstOrDefault();
                    var district = context.District.FirstOrDefault(w => w.DistrictId == parameter.Contact.DistrictId);
                    var ward = context.Ward.FirstOrDefault(w => w.WardId == parameter.Contact.WardId);
                    var lead_address = province?.ProvinceName + ", " + district?.DistrictName + ", " + ward?.WardName;
                    SendEmailEntityModel.LeadAddress = lead_address;
                    SendEmailEntityModel.LeadInterested =
                        context.Category.Where(w => w.CategoryId == parameter.Lead.InterestedGroupId).FirstOrDefault()
                            ?.CategoryName ?? "";
                    SendEmailEntityModel.LeadPotential = potential;
                    // lead personal in change
                    var pic = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.Lead.PersonInChargeId); //nguoi phu trach
                    SendEmailEntityModel.LeadPicCode = pic?.EmployeeCode ?? "";
                    SendEmailEntityModel.LeadPicName = pic?.EmployeeName ?? "";
                    //employee infor
                    var employeeId = context.User.FirstOrDefault(e => e.UserId == parameter.UserId).EmployeeId;
                    var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == employeeId); //nhan vien tao
                    SendEmailEntityModel.EmployeeCode = emp?.EmployeeCode ?? "";
                    SendEmailEntityModel.EmployeeName = emp?.EmployeeName ?? "";
                    //company infor
                    var companyEntity = context.CompanyConfiguration.FirstOrDefault();
                    SendEmailEntityModel.CompanyName = companyEntity?.CompanyName ?? "";
                    SendEmailEntityModel.CompanyAddress = companyEntity?.CompanyAddress ?? "";
                    // gửi email cho người tạo và người phụ trách
                    var empEmail = context.Contact.Where(w => w.ObjectId == emp.EmployeeId).FirstOrDefault()?.Email
                        ?.Trim();
                    SendEmailEntityModel.ListSendToEmail.Add(empEmail);

                    //lay email nguoi phu trach
                    if (pic != null && pic?.EmployeeId != employeeId)
                    {
                        var picEmail = context.Contact.Where(w => w.ObjectId == pic.EmployeeId).FirstOrDefault()?.Email
                            ?.Trim();
                        SendEmailEntityModel.ListSendToEmail.Add(picEmail);
                    }

                    //lay lead detail
                    SendEmailEntityModel.LeadId = parameter.Lead.LeadId;
                    SendEmailEntityModel.LeadContactId = parameter.Contact.ContactId;
                }
                catch
                {
                    return new CreateLeadResult
                    {
                        Status = true,
                        Message = CommonMessage.Lead.CREATE_SUCCESS,
                        LeadId = parameter.Lead.LeadId,
                        StatusName = statusName,
                        Potential = potential,
                        PicName = picName,
                        SendEmailEntityModel = SendEmailEntityModel
                    };
                }

                #endregion

                return new CreateLeadResult
                {
                    Status = true,
                    Message = CommonMessage.Lead.CREATE_SUCCESS,
                    LeadId = parameter.Lead.LeadId,
                    StatusName = statusName ?? "",
                    Potential = potential ?? "",
                    PicName = picName ?? "",
                    SendEmailEntityModel = SendEmailEntityModel
                };
            }
            catch (Exception e)
            {
                return new CreateLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }

        }

        public CloneLeadResult CloneLead(CloneLeadParameter parameter)
        {
            try
            {
                var contact =
                    context.Contact.FirstOrDefault(c => c.ObjectId == parameter.LeadId && c.ObjectType == "LEA") ??
                    new Contact();

                var lead = context.Lead.FirstOrDefault(c => c.LeadId == parameter.LeadId) ?? new Lead();
                Lead cloneLead = new Lead();
                Contact cloneContact = new Contact();

                cloneLead.LeadId = Guid.NewGuid();
                var leadStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "CHS").CategoryTypeId;
                var leadStatusDraftId = context.Category.FirstOrDefault(f => f.CategoryTypeId == leadStatusTypeId && f.CategoryCode == "DRAFT")?.CategoryId ?? new Guid();

                cloneLead.CreatedDate = DateTime.Now;
                cloneLead.CreatedById = parameter.UserId.ToString();
                cloneLead.LeadCode = GenerateCode();
                cloneLead.StatusId = leadStatusDraftId;
                cloneLead.RequirementDetail = lead.RequirementDetail != null ? lead.RequirementDetail.Trim() : lead.RequirementDetail;
                cloneLead.RequirementDetail = lead?.RequirementDetail;
                cloneLead.CompanyId = lead?.CompanyId;
                cloneLead.PersonInChargeId = lead.PersonInChargeId;
                cloneLead.LeadTypeId = lead?.LeadTypeId;
                cloneLead.PotentialId = lead?.PotentialId;
                cloneLead.PaymentMethodId = lead?.PaymentMethodId;
                cloneLead.InterestedGroupId = lead?.InterestedGroupId;
                cloneLead.Role = lead?.Role;
                cloneLead.WaitingForApproval = lead.WaitingForApproval;
                cloneLead.LeadGroupId = lead?.LeadGroupId;
                cloneLead.CustomerId = lead?.CustomerId;
                cloneLead.ExpectedSale = lead?.ExpectedSale;
                cloneLead.InvestmentFundId = lead?.InvestmentFundId;
                cloneLead.ProbabilityId = lead?.ProbabilityId;
                cloneLead.BusinessTypeId = lead?.BusinessTypeId;
                cloneLead.WaitingForApproval = false;
                cloneLead.CloneCount = 0;
                lead.CloneCount = (lead?.CloneCount ?? 0) + 1;


                cloneContact.ContactId = Guid.NewGuid();
                cloneContact.ObjectId = cloneLead.LeadId;
                cloneContact.ObjectType = ObjectType.LEAD;
                cloneContact.FirstName = (contact.FirstName + "_" + lead.CloneCount).Trim();
                cloneContact.LastName = contact.LastName != null ? contact.LastName.Trim() : contact.LastName;
                cloneContact.Address = contact.Address != null ? contact.Address.Trim() : contact.Address;
                cloneContact.Email = contact.Email != null ? contact.Email.Trim() : contact.Email;
                cloneContact.Phone = contact.Phone != null ? contact.Phone.Trim() : contact.Phone;
                cloneContact.Role = contact.Role != null ? contact.Role.Trim() : contact.Role;
                cloneContact.CreatedById = parameter.UserId;
                cloneContact.CreatedDate = DateTime.Now;

                string picName = context.Employee.FirstOrDefault(e => e.EmployeeId == lead.PersonInChargeId)?.EmployeeName;
                string potential = context.Category.FirstOrDefault(c => c.CategoryId == lead.PotentialId)?.CategoryName;
                string statusName = context.Category.FirstOrDefault(c => c.CategoryId == leadStatusDraftId)?.CategoryName;

                #region Save to [LeadInterestedGroupMapping]
                var ListInterestedId = context.LeadInterestedGroupMapping.Where(c => c.LeadId == lead.LeadId).Select(c => c.InterestedGroupId).ToList();
                var listLeadInterestedGroupMapping = new List<LeadInterestedGroupMapping>();
                ListInterestedId?.ForEach(item =>
                {
                    listLeadInterestedGroupMapping.Add(new LeadInterestedGroupMapping
                    {
                        LeadInterestedGroupMappingId = Guid.NewGuid(),
                        LeadId = cloneLead.LeadId,
                        InterestedGroupId = item,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });
                #endregion

                #region Add List Lead contact
                var ListContactDAO = context.Contact.Where(c => c.ObjectType == "LEA_CON" && c.ObjectId == lead.LeadId)?.ToList();
                var listContact = new List<Contact>();
                ListContactDAO?.ForEach(_contact =>
                {
                    listContact.Add(new Contact
                    {
                        ContactId = Guid.NewGuid(),
                        ObjectId = cloneLead.LeadId,
                        ObjectType = "LEA_CON",
                        FirstName = _contact.FirstName,
                        LastName = _contact.LastName,
                        Gender = _contact.Gender,
                        DateOfBirth = _contact.DateOfBirth,
                        Phone = _contact.Phone,
                        Email = _contact.Email,
                        Address = _contact.Address,
                        Note = _contact.Note,
                        Role = _contact.Role,
                        RelationShip = _contact.RelationShip,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now
                    });
                });
                #endregion

                #region Add Product detail
                var listLeadDetailOld = context.LeadDetail.Where(c => c.LeadId == lead.LeadId)?.ToList();
                var listLeadDetail = new List<LeadDetail>();
                var listProductAttribute = new List<LeadProductDetailProductAttributeValue>();
                var leadProductDetail = context.LeadProductDetailProductAttributeValue.ToList();

                listLeadDetailOld.ForEach(_leadDetail =>
                {
                    var newLeadDetailId = Guid.NewGuid();

                    var newLeadDetail = new LeadDetail
                    {
                        LeadDetailId = newLeadDetailId,
                        LeadId = cloneLead.LeadId,
                        VendorId = _leadDetail.VendorId,
                        ProductId = _leadDetail.ProductId,
                        Quantity = _leadDetail.Quantity,
                        UnitPrice = _leadDetail.UnitPrice,
                        CurrencyUnit = _leadDetail.CurrencyUnit,
                        ExchangeRate = _leadDetail.ExchangeRate,
                        Vat = _leadDetail.Vat,
                        DiscountType = _leadDetail.DiscountType,
                        DiscountValue = _leadDetail.DiscountValue,
                        Description = _leadDetail.Description,
                        OrderDetailType = _leadDetail.OrderDetailType,
                        UnitId = _leadDetail.UnitId,
                        IncurredUnit = _leadDetail.IncurredUnit,
                        ProductName = _leadDetail.ProductName,

                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null,
                    };
                    listLeadDetail.Add(newLeadDetail);
                    var list = leadProductDetail.Where(c => c.LeadDetailId == _leadDetail.LeadDetailId).ToList();
                    //them thuoc tinh neu co
                    list?.ForEach(attr =>
                    {
                        var productAtribute = new LeadProductDetailProductAttributeValue
                        {
                            LeadProductDetailProductAttributeValue1 = Guid.NewGuid(),
                            LeadDetailId = newLeadDetail.LeadDetailId,
                            ProductId = attr.ProductId,
                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                        };
                        listProductAttribute.Add(productAtribute);
                    });
                });

                #endregion

                context.Lead.Update(lead);
                context.Lead.Add(cloneLead);
                context.SaveChanges();
                context.Contact.Add(cloneContact);
                context.SaveChanges();
                context.Contact.AddRange(listContact);
                context.SaveChanges();

                context.LeadInterestedGroupMapping.AddRange(listLeadInterestedGroupMapping);
                context.SaveChanges();

                context.LeadDetail.AddRange(listLeadDetail);
                context.SaveChanges();
                context.LeadProductDetailProductAttributeValue.AddRange(listProductAttribute);
                context.SaveChanges();

                #region Get Infor Lead Created
                var SendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();
                try
                {
                    SendEmailEntityModel.ListSendToEmail = new List<string>();
                    //lead infor
                    SendEmailEntityModel.LeadName = cloneContact.FirstName + " " + cloneContact.LastName;
                    SendEmailEntityModel.LeadEmail = cloneContact.Email;
                    SendEmailEntityModel.LeadPhone = cloneContact.Phone;
                    //get address lead
                    var province = context.Province.Where(w => w.ProvinceId == cloneContact.ProvinceId).FirstOrDefault();
                    var district = context.District.FirstOrDefault(w => w.DistrictId == cloneContact.DistrictId);
                    var ward = context.Ward.FirstOrDefault(w => w.WardId == cloneContact.WardId);
                    var lead_address = province?.ProvinceName + ", " + district?.DistrictName + ", " + ward?.WardName;
                    SendEmailEntityModel.LeadAddress = lead_address;
                    SendEmailEntityModel.LeadInterested = context.Category.Where(w => w.CategoryId == cloneLead.InterestedGroupId).FirstOrDefault()?.CategoryName ?? "";
                    SendEmailEntityModel.LeadPotential = potential;
                    // lead personal in change
                    var pic = context.Employee.FirstOrDefault(e => e.EmployeeId == cloneLead.PersonInChargeId); //nguoi phu trach
                    SendEmailEntityModel.LeadPicCode = pic?.EmployeeCode ?? "";
                    SendEmailEntityModel.LeadPicName = pic?.EmployeeName ?? "";
                    //employee infor
                    var employeeId = context.User.FirstOrDefault(e => e.UserId == parameter.UserId).EmployeeId;
                    var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == employeeId); //nhan vien tao
                    SendEmailEntityModel.EmployeeCode = emp?.EmployeeCode ?? "";
                    SendEmailEntityModel.EmployeeName = emp?.EmployeeName ?? "";
                    //company infor
                    var companyEntity = context.CompanyConfiguration.FirstOrDefault();
                    SendEmailEntityModel.CompanyName = companyEntity?.CompanyName ?? "";
                    SendEmailEntityModel.CompanyAddress = companyEntity?.CompanyAddress ?? "";
                    // gửi email cho người tạo và người phụ trách
                    var empEmail = context.Contact.Where(w => w.ObjectId == emp.EmployeeId).FirstOrDefault()?.Email?.Trim();
                    SendEmailEntityModel.ListSendToEmail.Add(empEmail);

                    //lay email nguoi phu trach
                    if (pic != null && pic?.EmployeeId != employeeId)
                    {
                        var picEmail = context.Contact.Where(w => w.ObjectId == pic.EmployeeId).FirstOrDefault()?.Email?.Trim();
                        SendEmailEntityModel.ListSendToEmail.Add(picEmail);
                    }

                    //lay lead detail
                    SendEmailEntityModel.LeadId = cloneLead.LeadId;
                    SendEmailEntityModel.LeadContactId = cloneContact.ContactId;
                }
                catch
                {
                    return new CloneLeadResult
                    {
                        Status = true,
                        Message = CommonMessage.Lead.CREATE_SUCCESS,
                        LeadId = cloneLead.LeadId,
                        ContactId = cloneContact.ContactId,
                        //StatusName = statusName,
                        //Potential = potential,
                        //PicName = picName,
                        SendEmailEntityModel = SendEmailEntityModel
                    };
                }

                #endregion

                return new CloneLeadResult
                {
                    Status = true,
                    Message = CommonMessage.Lead.CREATE_SUCCESS,
                    LeadId = cloneLead.LeadId,
                    ContactId = cloneContact.ContactId,
                    StatusName = statusName ?? "",
                    Potential = potential ?? "",
                    PicName = picName ?? "",
                    SendEmailEntityModel = SendEmailEntityModel
                };
            }
            catch (Exception e)
            {
                return new CloneLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private string GenerateCode()
        {
            var leadTemp = context.Lead.Where(z => z.CreatedDate.Date == DateTime.Now.Date && z.LeadCode != null).OrderByDescending(x => x.CreatedDate).ToList().FirstOrDefault();
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            if (leadTemp == null)
            {
                return "LEAD-" + year + month + day + "0001";
            }
            else
            {
                var code = leadTemp.LeadCode?.Substring(leadTemp.LeadCode.Length - 4);
                int temp = Convert.ToInt32(code);
                temp++;
                string identity = "";
                if (temp < 10)
                {
                    identity = "000" + temp;
                }
                else if (temp < 100)
                {
                    identity = "00" + temp;
                }
                else if (temp < 1000)
                {
                    identity = "0" + temp;
                }
                else if (temp < 10000)
                {
                    identity = temp.ToString();
                }
                return "LEAD-" + year + month + day + identity;
            }
        }
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


        /// <summary>
        /// DeleteLead
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DeleteLeadResult DeleteLead(DeleteLeadParameter parameter)
        {
            try
            {
                return new DeleteLeadResult
                {
                    Status = true,
                };
            }
            catch (Exception e)
            {
                //Log.Error(e.Message);
                return new DeleteLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }
        /// <summary>
        /// GetAllLead
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public GetAllLeadResult GetAllLead(GetAllLeadParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.LEAD, "Search Lead", parameter.UserId);

            List<LeadEntityModel> listLead = new List<LeadEntityModel>();

            var listAllUser = context.User.Select(w => new { w.EmployeeId, w.UserId }).ToList();
            var listAllEmployee = context.Employee.Select(w => new { w.EmployeeId, w.IsManager, w.OrganizationId, w.EmployeeName }).ToList();

            var commonCategoryType = context.CategoryType.ToList();
            var commonCategory = context.Category.ToList();
            var commonCustomer = context.Customer.ToList();

            //var categoryType = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TLE");
            var categoryType = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "CHS");
            var probabilityType = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PROB");
            var statusSupportType = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTPCH");

            var listAllCategory = commonCategory.Where(w => w.Active == true
                                                            && w.CategoryTypeId == categoryType.CategoryTypeId)
                                                   .ToList();
            var listProbabilityCategory = commonCategory.Where(w => w.Active == true
                                                           && w.CategoryTypeId == probabilityType.CategoryTypeId)
                                                   .ToList();
            var listAllStatusSupport = commonCategory
                .Where(x => x.Active == true && x.CategoryTypeId == statusSupportType.CategoryTypeId).ToList();

            //check isManager
            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
            if (user == null)
            {
                return new GetAllLeadResult
                {
                    Status = false,
                    Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                };
            }
            if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
            {
                return new GetAllLeadResult
                {
                    Status = false,
                    Message = "Lỗi dữ liệu"
                };
            }

            var quotes = context.Quote.ToList();
            var saleBiddings = context.SaleBidding.ToList();

            var employeeId = user.EmployeeId;
            var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
            var isManager = employee.IsManager;

            var fullName = (parameter.FirstName == null || parameter.FirstName == "") ? "" : parameter.FirstName.Trim();
            var email = parameter.Email == null ? "" : parameter.Email.Trim();
            var phone = parameter.Phone == null ? "" : parameter.Phone.Trim();
            var listPotentialId = parameter.PotentialId;                     //list Mức độ tiềm năng
            var listStatusId = parameter.StatusId;                           //list Trạng thái
            var listInterestedGroupId = parameter.InterestedGroupId ?? new List<Guid?>();         //list Nhóm dịch vụ
            var listPersonInChargeId = parameter.PersonInChargeId;           //list Người phụ trách
            var listLeadType = parameter.LeadType;
            var personInChargeIdIsNull = parameter.NoActivePic;              //Chưa có người phụ trách?

            #region Lấy StatusId của KHTN có trạng thái khác: Ký hợp đồng và Ngừng theo dõi

            List<Category> listCategoryDefault = new List<Category>();
            List<Guid> listStatusDefaultId = new List<Guid>();
            if (categoryType != null)
            {
                //listCategoryDefault = listAllCategory.Where(x => x.CategoryTypeId == categoryType.CategoryTypeId && x.CategoryCode != "KHD" && x.CategoryCode != "NDO").ToList();
                listCategoryDefault.ForEach(item =>
                {
                    if (item.CategoryId != null && item.CategoryId != Guid.Empty)
                    {
                        listStatusDefaultId.Add(item.CategoryId);
                    }
                });
            }
            else
            {
                return new GetAllLeadResult
                {
                    Status = false,
                    Message = "Lỗi dữ liệu"
                };
            }

            if (listStatusId == null || listStatusId.Count == 0)
            {
                listStatusId = new List<Guid?>();
                if (listStatusDefaultId.Count > 0)
                {
                    listStatusDefaultId.ForEach(item =>
                    {
                        listStatusId.Add(item);
                    });
                }
            }
            #endregion

            List<Lead> listAllLead = new List<Lead>();

            listAllLead = context.Lead.Where(x => (x.Active == true) &&
                                                  (listPotentialId == null || listPotentialId.Count == 0 || listPotentialId.Contains(x.PotentialId)) &&
                                                  (listStatusId == null || listStatusId.Count == 0 || listStatusId.Contains(x.StatusId)) &&
                                                  //(listInterestedGroupId == null || listInterestedGroupId.Count == 0 || listInterestedGroupId.Contains(x.InterestedGroupId)) &&
                                                  (listPersonInChargeId == null || listPersonInChargeId.Count == 0 || listPersonInChargeId.Contains(x.PersonInChargeId)) &&
                                                  (listLeadType == null || listLeadType.Count == 0 || listLeadType.Contains(x.LeadTypeId)) &&
                                                  (parameter.FromDate == null || parameter.FromDate.Value <= x.CreatedDate) &&
                                                  (parameter.ToDate == null || x.CreatedDate <= parameter.ToDate.Value) &&
                                                  (parameter.ListSourceId == null || parameter.ListSourceId.Count == 0 || parameter.ListSourceId.Contains(x.InvestmentFundId)) &&
                                                  (parameter.ListAreaId == null || parameter.ListAreaId.Count == 0 || parameter.ListAreaId.Contains(x.GeographicalAreaId)) &&
                                                  (parameter.ListCusGroupId == null || parameter.ListCusGroupId.Count == 0 || parameter.ListCusGroupId.Contains(x.LeadGroupId))
                                            ).ToList() ?? new List<Lead>();
            var listAllLeadId = listAllLead.Select(w => w.LeadId).ToList() ?? new List<Guid>();
            //var commonLeadInterestedGroupMapping = context.LeadInterestedGroupMapping.Where(w => listAllLeadId.Contains(w.LeadId)).ToList() ?? new List<LeadInterestedGroupMapping>();
            var commonLeadInterestedGroupMapping = context.LeadInterestedGroupMapping.ToList() ?? new List<LeadInterestedGroupMapping>();

            //var leadInterestedGroupFiltered = commonLeadInterestedGroupMapping.ToList();
            if (listInterestedGroupId.Count != 0)
            {
                //nếu filter theo nhu cầu sản phẩm, dịch vụ
                //leadInterestedGroupFiltered = leadInterestedGroupFiltered.Where(w => listInterestedGroupId.Contains(w.InterestedGroupId)).ToList() ?? new List<LeadInterestedGroupMapping>();
                var leadInterestedGroupFilteredId = commonLeadInterestedGroupMapping.Where(w => listInterestedGroupId.Contains(w.InterestedGroupId)).ToList().Select(w => w.LeadId).ToList() ?? new List<Guid>();
                listAllLead = listAllLead.Where(w => leadInterestedGroupFilteredId.Contains(w.LeadId)).ToList() ?? new List<Lead>();
            }
            else
            {
                //nếu không filter theo nhu cầu sản phẩm, dịch vụ
            }


            //var listLeadIdFiltered = leadInterestedGroupFiltered.Select(w => w.LeadId).ToList() ?? new List<Guid>();

            //listAllLead = listAllLead.Where(w => listLeadIdFiltered.Contains(w.LeadId)).ToList() ?? new List<Lead>();

            //lấy danh sách những lead đang chờ phê duyệt
            if (parameter.WaitingForApproval == true)
            {
                listAllLead = listAllLead.Where(w => w.WaitingForApproval == true).ToList();
            }

            //lấy dữ liệu kiểm tra điều kiện xóa lead
            var listLeadId = listAllLead.Select(w => w.LeadId).ToList();

            var listQuote = context.Quote.Where(w => w.Active == true && w.ObjectType == "LEAD" && listLeadId.Contains(w.ObjectTypeId.Value)).Select(w => w.ObjectTypeId).ToList();

            var listCustomerFromLead = context.Customer.Where(w => w.Active == true && listLeadId.Contains(w.LeadId.Value)).Select(w => w.LeadId).ToList(); //danh sách khách hàng được tạo từ lead

            var leadStatusType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE").CategoryTypeId;
            //var MOI_Status_Id = context.Category.FirstOrDefault(f => f.CategoryTypeId == leadStatusType && f.CategoryCode == "MOI").CategoryId;

            if (personInChargeIdIsNull)
            {
                //Nếu lấy những KHTN chưa có người phụ trách
                listAllLead = listAllLead.Where(x => x.PersonInChargeId == null).ToList();
            }

            //Lấy tất cả contact của KHTN
            var listAllLeadContact = context.Contact.Where(x => x.Active == true && x.ObjectType == ObjectType.LEAD &&
                                                                (fullName == "" || (x.FirstName ?? "" + " " + x.LastName ?? "").Contains(fullName.ToLower())) &&
                                                                (email == "" || (x.Email != null && x.Email.ToLower().Contains(email.ToLower()))) &&
                                                                (phone == "" || (x.Phone != null && x.Phone.ToLower().Contains(phone.ToLower())))
                                                          )
                                                          .Select(s => new
                                                          {
                                                              s.ObjectId,
                                                              s.ContactId,
                                                              s.FirstName,
                                                              s.LastName,
                                                              s.Email,
                                                              s.Phone
                                                          })
                                                          .ToList();

            if (isManager)
            {
                //Nếu user là quản lý

                //Lấy list phòng ban con của user
                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                if (employee.OrganizationId != null && isManager)
                {
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                }
                //Lấy danh sách người dùng mà user phụ trách
                var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                List<Guid> listUserByManagerId = new List<Guid>();

                listEmployeeInChargeByManager.ForEach(item =>
                {
                    if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                        listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                });

                listEmployeeInChargeByManagerId.ForEach(item =>
                {
                    var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                    if (user_employee != null)
                        listUserByManagerId.Add(user_employee.UserId);
                });

                //thì lấy những KHTN có người phụ trách là employeeId hoặc KHTN không có người phụ trách thì lấy CreatedById là employeeId
                //và của những nhân viên thuộc phòng cùng phòng ban và các phòng ban con của user
                listAllLead = listAllLead.Where(x => (x.PersonInChargeId != null && (listEmployeeInChargeByManagerId == null || listEmployeeInChargeByManagerId.Count == 0 || listEmployeeInChargeByManagerId.FirstOrDefault(y => y.Equals(x.PersonInChargeId.Value)) != Guid.Empty)) ||
                                                     (x.PersonInChargeId == null && (listUserByManagerId == null || listUserByManagerId.Count == 0 || listUserByManagerId.FirstOrDefault(y => y.Equals(Guid.Parse(x.CreatedById))) != Guid.Empty))
                                               ).ToList();
                listAllLead.ForEach(item =>
                {
                    var lead_contact = listAllLeadContact.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    if (lead_contact != null)
                    {
                        LeadEntityModel lead = new LeadEntityModel();
                        var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                        var status = listAllCategory.FirstOrDefault(x => x.CategoryId == item.StatusId);
                        lead.LeadId = item.LeadId;
                        lead.ContactId = lead_contact.ContactId;
                        lead.CompanyId = item.CompanyId;
                        lead.PotentialId = item.PotentialId;
                        lead.InterestedGroupId = item.InterestedGroupId;
                        lead.PersonInChargeId = item.PersonInChargeId;
                        lead.FullName = (lead_contact.FirstName != null ? lead_contact.FirstName.Trim() : "") + " " + (lead_contact.LastName != null ? lead_contact.LastName.Trim() : "");
                        lead.PersonInChargeFullName = personInCharge != null ? personInCharge.EmployeeName.Trim() : "";
                        lead.Email = lead_contact.Email;
                        lead.Phone = lead_contact.Phone;
                        lead.StatusName = status != null ? status.CategoryName : "";
                        lead.StatusCode = status != null ? status.CategoryCode : "";
                        lead.AvatarUrl = "";
                        lead.PersonInChargeAvatarUrl = "";
                        lead.NoActivePic = false;   //giá trị là false hay true đều đc
                        lead.Active = item.Active;
                        lead.Role = item.Role;
                        lead.CreatedById = item.CreatedById;
                        lead.CreatedDate = item.CreatedDate;
                        //lead.CountLead = CheckCountInformationLead(item.LeadId, listAllQuote, listAllNote);
                        lead.WaitingForApproval = item.WaitingForApproval;
                        lead.CustomerId = item.CustomerId;
                        lead.BusinessTypeId = item.BusinessTypeId;
                        lead.InvestmentFundId = item.InvestmentFundId;
                        lead.ProbabilityId = item.ProbabilityId;
                        lead.ExpectedSale = item.ExpectedSale ?? 0;
                        lead.ProbabilityName = listProbabilityCategory.FirstOrDefault(f => f.CategoryId == item.ProbabilityId)?.CategoryName ?? "";
                        lead.CustomerName = commonCustomer.FirstOrDefault(f => f.CustomerId == item.CustomerId)?.CustomerName ?? "";
                        lead.IsStatusConnect = GetStatusTypeConnectTW(item.LeadId, saleBiddings, quotes);
                        lead.CanDeleteLead = CheckDeleteLeadCondition(item, listQuote, listCustomerFromLead);
                        lead.StatusSuportId = item.StatusSuportId;
                        lead.StatusSupportName = listAllStatusSupport
                                                     .FirstOrDefault(x => x.CategoryId == item.StatusSuportId)
                                                     ?.CategoryName ?? "";

                        listLead.Add(lead);
                    }
                });
            }
            else
            {
                //Nếu user không phải quản lý
                //thì lấy những KHTN có người phụ trách là employeeId hoặc KHTN không có người phụ trách thì lấy CreatedById là employeeId
                listAllLead = listAllLead.Where(x => (x.PersonInChargeId != null && x.PersonInChargeId == employeeId) || (x.PersonInChargeId == null && x.CreatedById.Equals(user.UserId.ToString()))).ToList();

                listAllLead.ForEach(item =>
                {
                    var lead_contact = listAllLeadContact.FirstOrDefault(x => x.ObjectId == item.LeadId);
                    if (lead_contact != null)
                    {
                        LeadEntityModel lead = new LeadEntityModel();
                        var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                        var status = listAllCategory.FirstOrDefault(x => x.CategoryId == item.StatusId);
                        lead.LeadId = item.LeadId;
                        lead.ContactId = lead_contact.ContactId;
                        lead.CompanyId = item.CompanyId;
                        lead.PotentialId = item.PotentialId;
                        lead.InterestedGroupId = item.InterestedGroupId;
                        lead.PersonInChargeId = item.PersonInChargeId;
                        lead.FullName = (lead_contact.FirstName != null ? lead_contact.FirstName.Trim() : "") + " " + (lead_contact.LastName != null ? lead_contact.LastName.Trim() : "");
                        lead.PersonInChargeFullName = personInCharge != null ? personInCharge.EmployeeName.Trim() : "";
                        lead.Email = lead_contact.Email;
                        lead.Phone = lead_contact.Phone;
                        lead.StatusName = status != null ? status.CategoryName : "";
                        lead.StatusCode = status != null ? status.CategoryCode : "";
                        lead.AvatarUrl = "";
                        lead.PersonInChargeAvatarUrl = "";
                        lead.NoActivePic = false;   //giá trị là false hay true đều đc
                        lead.Active = item.Active;
                        lead.Role = item.Role;
                        lead.CreatedById = item.CreatedById;
                        lead.CreatedDate = item.CreatedDate;
                        // lead.CountLead = CheckCountInformationLead(item.LeadId, listAllQuote, listAllNote);
                        lead.WaitingForApproval = item.WaitingForApproval;
                        lead.CustomerId = item.CustomerId;
                        lead.BusinessTypeId = item.BusinessTypeId;
                        lead.InvestmentFundId = item.InvestmentFundId;
                        lead.ProbabilityId = item.ProbabilityId;
                        lead.ExpectedSale = item.ExpectedSale ?? 0;
                        lead.CustomerName = commonCustomer.FirstOrDefault(f => f.CustomerId == item.CustomerId)?.CustomerName ?? "";
                        lead.IsStatusConnect = GetStatusTypeConnectTW(item.LeadId, saleBiddings, quotes);
                        lead.CanDeleteLead = CheckDeleteLeadCondition(item, listQuote, listCustomerFromLead);
                        lead.StatusSuportId = item.StatusSuportId;
                        lead.StatusSupportName = listAllStatusSupport
                                                     .FirstOrDefault(x => x.CategoryId == item.StatusSuportId)
                                                     ?.CategoryName ?? "";

                        listLead.Add(lead);
                    }
                });
            }

            listLead = listLead.OrderByDescending(x => x.CreatedDate).ToList();

            return new GetAllLeadResult
            {
                ListLead = listLead,
                Status = true
            };
        }


        private int GetStatusTypeConnect(Guid? leadId, List<SaleBidding> lstSaleBidding, List<Quote> quotes)
        {
            // 0 điều kiện hủy khi ko có hồ sơ thầu và báo giá
            // 1 khi đã tồn tại hồ sơ thầu
            // 3 điều kiện hủy khi tồn tại hồ sơ thầu
            // 2 tồn tại báo giá
            // 4 điều kiện hủy khi tồn tại báo giá

            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST")?.CategoryTypeId;
            var statusDestroyId = context.Category
                .FirstOrDefault(c => c.CategoryCode == "CANC" && c.CategoryTypeId == statusTypeId)?.CategoryId;
            var statusQuoteTypeId =
                context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TGI")?.CategoryTypeId;
            var statusQuoteDestroyId = context.Category
                .FirstOrDefault(c => c.CategoryCode == "HUY" && c.CategoryTypeId == statusQuoteTypeId)?.CategoryId;

            //if (saleBidding != null)
            //{
            //    if (saleBidding.StatusId == statusDestroyId)
            //        return 3;
            //    return 1;
            //}
            if (lstSaleBidding.Count != 0)
            {
                var isCheck = false;
                foreach (var item in lstSaleBidding)
                {
                    if (item.StatusId != statusDestroyId)
                    {
                        isCheck = true;
                        break;
                    }
                }
                if (!isCheck)
                    return 3;
                return 1;
            }

            if (quotes.Count != 0)
            {
                var isCheck = false;
                foreach (var item in quotes)
                {
                    if (item.StatusId != statusQuoteDestroyId)
                    {
                        isCheck = true;
                        break;
                    }
                }
                if (!isCheck)
                {
                    return 4;
                }
                return 2;
            }

            return 0;
        }
        private int GetStatusTypeConnectTW(Guid? leadId, List<SaleBidding> lstSaleBidding, List<Quote> quotes)
        {
            var isExistsSaleBidding = lstSaleBidding?.FirstOrDefault(c => c.LeadId == leadId.Value);
            if (isExistsSaleBidding != null)
            {
                return 1;
            }
            var isExistsQuote = quotes?.FirstOrDefault(c => c.LeadId == leadId);
            if (isExistsQuote != null)
            {
                return 2;
            }
            return 0;
        }

        private bool CheckDeleteLeadCondition(Lead lead, List<Guid?> listQuote, List<Guid?> listCustomer)
        {
            //if (lead.StatusId != MOI_Status_Id)
            //{
            //    return false;
            //}
            var hasQuote = listQuote.Contains(lead.LeadId);

            if (hasQuote == true)
            {
                return false;
            }
            var createdCustomer = listCustomer.Contains(lead.LeadId);

            if (createdCustomer == true)
            {
                return false;
            }
            return true;
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

        public EditLeadByIdResult EditLeadById(EditLeadByIdParameter parameter)
        {
            var currentUserEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentUserEmpName = context.Employee.FirstOrDefault(e => e.EmployeeId == currentUserEmpId).EmployeeName;
            bool isChangePic = false;
            bool isChangePotential = false;
            bool isChangeStatus = false;
            string picName = "";
            string potential = "";
            string statusName = "";
            // Save model to database
            var lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.Lead.LeadId);
            var contact =
                context.Contact.FirstOrDefault(c => c.ObjectId == parameter.Lead.LeadId && c.ObjectType == "LEA");

            if (lead?.PersonInChargeId != parameter.Lead.PersonInChargeId)
            {
                isChangePic = true;
                picName = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.Lead.PersonInChargeId)?.EmployeeName;
            }

            if (lead?.PotentialId != parameter.Lead.PotentialId)
            {
                isChangePotential = true;
                potential = context.Category.FirstOrDefault(c => c.CategoryId == parameter.Lead.PotentialId)?.CategoryName;
            }

            if (lead?.StatusId != parameter.Lead.StatusId && parameter.Lead.WaitingForApproval == false)
            {
                isChangeStatus = true;
                statusName = context.Category.FirstOrDefault(c => c.CategoryId == parameter.Lead.StatusId)?.CategoryName;
            }

            contact.FirstName = parameter.Contact.FirstName?.Trim();
            contact.LastName = parameter.Contact.LastName?.Trim();
            contact.ProvinceId = parameter.Contact.ProvinceId;
            contact.DistrictId = parameter.Contact.DistrictId;
            contact.WardId = parameter.Contact.WardId;
            contact.Address = parameter.Contact.Address?.Trim();
            contact.Email = parameter.Contact?.Email;
            contact.IdentityId = parameter.Contact?.IdentityId;
            contact.Phone = parameter.Contact?.Phone;
            contact.SocialUrl = parameter.Contact?.SocialUrl;
            contact.Gender = parameter.Contact?.Gender;
            contact.AvatarUrl = parameter.Contact?.AvatarUrl;

            lead.RequirementDetail = parameter.Lead?.RequirementDetail;
            lead.CompanyId = parameter.Lead?.CompanyId;
            lead.PersonInChargeId = parameter.Lead?.PersonInChargeId;
            lead.LeadTypeId = parameter.Lead?.LeadTypeId;
            lead.PotentialId = parameter.Lead?.PotentialId;
            lead.PaymentMethodId = parameter.Lead?.PaymentMethodId;
            lead.InterestedGroupId = parameter.Lead?.InterestedGroupId;
            lead.Role = parameter.Lead?.Role;
            lead.WaitingForApproval = parameter.Lead.WaitingForApproval;
            lead.LeadGroupId = parameter.Lead?.LeadGroupId;
            lead.CustomerId = parameter.Lead?.CustomerId;
            lead.ExpectedSale = parameter.Lead?.ExpectedSale;
            lead.InvestmentFundId = parameter.Lead?.InvestmentFundId;
            lead.ProbabilityId = parameter.Lead?.ProbabilityId;
            lead.BusinessTypeId = parameter.Lead?.BusinessTypeId;
            lead.GeographicalAreaId = parameter.Lead?.GeographicalAreaId;

            //nếu đang đợi phê duyệt(WaitingForApproval) thì ko thay đổi trạng thái
            if (parameter.Lead.WaitingForApproval == false)
            {
                lead.StatusId = parameter.Lead.StatusId;
            }


            #region Remove list document
            parameter.ListDocumentIdNeedRemove =
                parameter.ListDocumentIdNeedRemove.Where(w => w != Guid.Empty && w != null).ToList();

            var listOldDocument =
                context.FileInFolder.Where(w => parameter.ListDocumentIdNeedRemove.Contains(w.FileInFolderId)).ToList();
            context.FileInFolder.RemoveRange(listOldDocument);
            #endregion

            #region Remove old Link of document and add new 
            var listOldLinkOfDocument =
                context.LinkOfDocument.Where(w => w.ObjectId == parameter.Lead.LeadId && w.ObjectType == "LEAD").ToList();
            context.LinkOfDocument.RemoveRange(listOldLinkOfDocument);

            var listNewLinkOfDocument = new List<Entities.LinkOfDocument>();
            parameter.ListLinkOfDocument?.ForEach(linkOfDoc =>
            {
                var newItem = new Entities.LinkOfDocument();
                newItem.LinkOfDocumentId = Guid.NewGuid();
                newItem.LinkName = linkOfDoc.LinkName;
                newItem.LinkValue = linkOfDoc.LinkValue;
                newItem.ObjectType = "LEAD";
                newItem.ObjectId = parameter.Lead.LeadId;
                newItem.Active = true;
                newItem.CreatedById = parameter.UserId;
                newItem.CreatedDate = DateTime.Now;
                newItem.UpdatedById = null;
                newItem.UpdatedDate = null;

                listNewLinkOfDocument.Add(newItem);
            });
            context.LinkOfDocument.AddRange(listNewLinkOfDocument);
            #endregion


            #region Add List Lead contact
            //remove old records
            var listOldContact = context.Contact.Where(w => w.ObjectId == parameter.Lead.LeadId && w.ObjectType == "LEA_CON").ToList();
            var listNewContact = new List<Contact>();
            parameter.ListContactDAO?.ForEach(_contact =>
            {
                listNewContact.Add(new Contact
                {
                    ContactId = Guid.NewGuid(),
                    ObjectId = parameter.Lead.LeadId,
                    ObjectType = "LEA_CON",
                    FirstName = _contact.FirstName,
                    LastName = _contact.LastName,
                    Gender = _contact.Gender,
                    DateOfBirth = _contact.DateOfBirth,
                    Phone = _contact.Phone,
                    Email = _contact.Email,
                    Address = _contact.Address,
                    Note = _contact.Note,
                    Role = _contact.Role,
                    RelationShip = _contact.RelationShip,
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now
                });
            });
            #endregion

            #region Insert to intersted mapping
            //remove old records
            var listOldInterested = context.LeadInterestedGroupMapping.Where(w => w.LeadId == parameter.Lead.LeadId).ToList();
            var listNewInterested = new List<LeadInterestedGroupMapping>();
            parameter.ListInterestedId?.ForEach(interested =>
            {
                listNewInterested.Add(new LeadInterestedGroupMapping
                {
                    LeadInterestedGroupMappingId = Guid.NewGuid(),
                    LeadId = parameter.Lead.LeadId,
                    InterestedGroupId = interested.Value,
                    Active = true,
                    CreatedDate = DateTime.Now,
                    CreatedById = parameter.UserId,
                });
            });
            #endregion

            #region Add Product detail
            //remove old record
            var listOldDetail = context.LeadDetail.Where(w => w.LeadId == parameter.Lead.LeadId).ToList() ?? new List<LeadDetail>();
            var listOldDetailId = listOldDetail.Select(w => w.LeadDetailId).ToList() ?? new List<Guid>();
            var listOldDetailItem = context.LeadProductDetailProductAttributeValue.Where(w => listOldDetailId.Contains(w.LeadDetailId.Value)).ToList();

            var listLeadDetail = new List<LeadDetail>();
            var listProductAttribute = new List<LeadProductDetailProductAttributeValue>();

            parameter?.ListLeadDetail.ForEach(_leadDetail =>
            {
                var newLeadDetailId = Guid.NewGuid();

                var newLeadDetail = new LeadDetail
                {
                    LeadDetailId = newLeadDetailId,
                    LeadId = parameter.Lead.LeadId,
                    VendorId = _leadDetail.VendorId,
                    ProductId = _leadDetail.ProductId,
                    Quantity = _leadDetail.Quantity,
                    UnitPrice = _leadDetail.UnitPrice,
                    CurrencyUnit = _leadDetail.CurrencyUnit,
                    ExchangeRate = _leadDetail.ExchangeRate,
                    Vat = _leadDetail.Vat,
                    DiscountType = _leadDetail.DiscountType,
                    DiscountValue = _leadDetail.DiscountValue,
                    Description = _leadDetail.Description,
                    OrderDetailType = _leadDetail.OrderDetailType,
                    UnitId = _leadDetail.UnitId,
                    IncurredUnit = _leadDetail.IncurredUnit,
                    ProductName = _leadDetail.ProductName,

                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedById = null,
                    UpdatedDate = null,
                };
                listLeadDetail.Add(newLeadDetail);
                //them thuoc tinh neu co
                _leadDetail?.LeadProductDetailProductAttributeValue?.ForEach(attr =>
                {
                    var productAtribute = new LeadProductDetailProductAttributeValue
                    {
                        LeadProductDetailProductAttributeValue1 = Guid.NewGuid(),
                        LeadDetailId = newLeadDetail.LeadDetailId,
                        ProductId = attr.ProductId,
                        ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                        ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId
                    };
                    listProductAttribute.Add(productAtribute);
                });
            });
            #endregion

            context.Lead.Update(lead);
            context.Contact.Update(contact);
            context.Contact.RemoveRange(listOldContact);
            context.Contact.AddRange(listNewContact);
            context.LeadInterestedGroupMapping.RemoveRange(listOldInterested);
            context.LeadInterestedGroupMapping.AddRange(listNewInterested);

            //remove old 
            context.LeadDetail.RemoveRange(listOldDetail);
            context.LeadProductDetailProductAttributeValue.RemoveRange(listOldDetailItem);
            //add new 
            context.LeadDetail.AddRange(listLeadDetail);
            context.LeadProductDetailProductAttributeValue.AddRange(listProductAttribute);

            context.SaveChanges();

            #region Gửi thông báo

            if (parameter.IsGetNoti)
            {
                NotificationHelper.AccessNotification(context, TypeModel.LeadDetail, "UPD", new Lead(),
                    parameter.Lead, true);
            }

            #endregion

            return new EditLeadByIdResult
            {
                Status = true,
                Message = CommonMessage.Lead.EDIT_SUCCESS,
                IsChangePic = isChangePic,
                IsChangePotential = isChangePotential,
                IsChangeStatus = isChangeStatus,
                StatusName = statusName,
                PicName = picName,
                Potential = potential,
                SendEmailEntityModel = new Models.Email.SendEmailEntityModel()
            };
        }
        /// <summary>
        /// GetNoteHistory
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public GetNoteHistoryResult GetNoteHistory(GetNoteHistoryParameter parameter)
        {
            // Add history access to trace log
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.LEAD, "Get Lead By Id", parameter.UserId);
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.COMPANY, "Get Company By Id", parameter.UserId);
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.CONTACT, "Get Contact By Id", parameter.UserId);

            // Get list note by lead id
            var listNote = context.Note.Where(n => n.ObjectId == parameter.LeadId && n.Active == true).Select(s => new NoteEntityModel()
            {
                Active = s.Active,
                NoteId = s.NoteId,
                NoteTitle = s.NoteTitle,
                CreatedById = s.CreatedById,
                CreatedDate = s.CreatedDate,
                Description = s.Description,
                ObjectType = s.ObjectType,
                Type = s.Type,
                UpdatedById = s.UpdatedById,
                UpdatedDate = s.UpdatedDate,
                ObjectId = s.ObjectId,
                NoteDocList = null,
                //NoteDocList = context.NoteDocument.Where(nd => nd.NoteId == s.NoteId).Select(nd => new NoteDocumentEntityModel()
                //{
                //    DocumentName = nd.DocumentName,
                //    DocumentSize = nd.DocumentSize,
                //    DocumentUrl = nd.DocumentUrl,
                //    CreatedById = nd.CreatedById,
                //    NoteDocumentId = nd.NoteDocumentId,
                //    NoteId = nd.NoteId
                //}
                //).ToList()
            }).ToList();
            if (listNote.Count > 0)
            {
                List<Guid> listNoteId = new List<Guid>();
                List<Guid> listUserId = new List<Guid>();
                List<Guid> listEmployeeId = new List<Guid>();
                listNote.ForEach(item =>
                {
                    listNoteId.Add(item.NoteId);
                    if (item.CreatedById != null && !listUserId.Contains(item.CreatedById))
                    {
                        listUserId.Add(item.CreatedById);
                    }
                });
                var listNoteDoc = context.NoteDocument.Where(w => listNoteId.Contains(w.NoteId)).ToList();
                var listUser = context.User.Where(w => listUserId.Contains(w.UserId)).ToList();
                listUser.ForEach(item =>
                {
                    if (item.EmployeeId != null && !listEmployeeId.Contains(item.EmployeeId.Value))
                        listEmployeeId.Add(item.EmployeeId.Value);
                });
                var listEmployee = context.Employee.Where(w => listEmployeeId.Contains(w.EmployeeId)).ToList();
                var listContact = context.Contact.Where(w => listEmployeeId.Contains(w.ObjectId) && w.ObjectType == ObjectType.EMPLOYEE);

                listNote.ForEach(item =>
                {
                    var user = listUser.FirstOrDefault(f => f.UserId == item.CreatedById);
                    if (user != null && user.EmployeeId != null)
                    {
                        var employee = listEmployee.FirstOrDefault(f => f.EmployeeId == user.EmployeeId);
                        if (employee != null)
                            item.ResponsibleName = employee.EmployeeName;
                        var contact = listContact.FirstOrDefault(f => f.ObjectId == user.EmployeeId);
                        if (contact != null)
                            item.ResponsibleAvatar = contact.AvatarUrl;
                    }
                    item.NoteDocList = listNoteDoc.Where(nd => nd.NoteId == item.NoteId).Select(nd => new NoteDocumentEntityModel()
                    {
                        DocumentName = nd.DocumentName,
                        DocumentSize = nd.DocumentSize,
                        DocumentUrl = nd.DocumentUrl,
                        CreatedById = nd.CreatedById,
                        NoteDocumentId = nd.NoteDocumentId,
                        NoteId = nd.NoteId
                    }).ToList();

                    if (item.NoteDocList != null)
                    {
                        item.NoteDocList.ForEach(noteDoc =>
                        {
                            bool isImageOrPdf = (noteDoc.DocumentUrl.IndexOf(".jpg", StringComparison.Ordinal) > 0 ||
                                                 noteDoc.DocumentUrl.IndexOf(".jpeg", StringComparison.Ordinal) > 0 ||
                                                 noteDoc.DocumentUrl.IndexOf(".png", StringComparison.Ordinal) > 0 ||
                                                 noteDoc.DocumentUrl.IndexOf(".gif", StringComparison.Ordinal) > 0 ||
                                                 noteDoc.DocumentUrl.IndexOf(".pdf", StringComparison.Ordinal) > 0);
                            if (isImageOrPdf)
                            {
                                Byte[] bytes = File.ReadAllBytes(noteDoc.DocumentUrl);
                                String base64File = Convert.ToBase64String(bytes);
                                noteDoc.Base64Url = base64File;
                            }
                        });
                    }
                });
            }
            return new GetNoteHistoryResult
            {
                Status = true,
                ListNode = listNote
            };
        }

        public GetLeadByStatusResult GetLeadByStatus(GetLeadByStatusParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.LEAD, "Get Lead by Status", parameter.UserId);
            var categoryTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE").CategoryTypeId;
            var stt = context.Category.FirstOrDefault(c => c.CategoryCode == parameter.StatusName && c.CategoryTypeId == categoryTypeId).CategoryId;
            #region Add by Hung
            GetAllLeadParameter parameterAllLead = new GetAllLeadParameter();
            parameterAllLead.StatusId = new List<Guid?>();
            parameterAllLead.UserId = parameter.UserId;
            parameterAllLead.StatusId.Add(stt);
            var allLead = GetAllLead(parameterAllLead);
            var leadList = allLead.ListLead;
            #endregion

            #region Comment By Hung
            //var empId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            //var manager = context.Employee.FirstOrDefault(e => e.EmployeeId == empId);
            //var empInCurrentOrg = context.Employee.Where(e => e.OrganizationId == manager.OrganizationId);
            //var userInCurrentOrg = new List<Guid>();
            //empInCurrentOrg.ToList().ForEach(emp =>
            //{
            //    var userId = context.User.FirstOrDefault(u => u.EmployeeId == emp.EmployeeId).UserId;
            //    userInCurrentOrg.Add(userId);
            //});
            //var leadList = new List<LeadEntityModel>();
            //if (parameter.StatusName.ToLower() == "unfollowed")
            //{
            //    leadList = (from l in context.Lead
            //                join c in context.Contact on l.LeadId equals c.ObjectId
            //                join ctx in context.Category on l.StatusId equals ctx.CategoryId into cl
            //                from y in cl.DefaultIfEmpty()
            //                where (l.Active == false || l.WaitingForApproval) &&
            //                (l.PersonInChargeId == empId || (l.PersonInChargeId == Guid.Empty && l.CreatedById == parameter.UserId.ToString())
            //                || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById))))
            //                orderby l.CreatedDate descending
            //                select new LeadEntityModel
            //                {
            //                    LeadId = l.LeadId,
            //                    ContactId = c.ContactId,
            //                    CompanyId = l.CompanyId,
            //                    FullName = c.FirstName + " " + c.LastName,
            //                    Email = c.Email,
            //                    Phone = c.Phone,
            //                    PersonInChargeId = l.PersonInChargeId,
            //                    StatusName = y.CategoryName,
            //                    StatusCode = y.CategoryCode,
            //                    AvatarUrl = string.IsNullOrEmpty(c.AvatarUrl) ? "" : c.AvatarUrl,
            //                    Active = l.Active
            //                }).ToList();

            //    leadList.ForEach(lead =>
            //    {
            //        var picId = lead.PersonInChargeId;
            //        if (picId != Guid.Empty)
            //        {
            //            var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == picId);
            //            lead.PersonInChargeFullName = emp.EmployeeName;
            //            lead.PersonInChargeAvatarUrl = context.Contact.FirstOrDefault(c => c.ObjectId == emp.EmployeeId).AvatarUrl;
            //        }
            //    });
            //}
            //else
            //{
            //    leadList = (from l in context.Lead
            //                join c in context.Contact on l.LeadId equals c.ObjectId
            //                join ctx in context.Category on l.StatusId equals ctx.CategoryId into cl
            //                from y in cl.DefaultIfEmpty()
            //                where l.StatusId == stt && (l.PersonInChargeId == empId || (l.PersonInChargeId == Guid.Empty && l.CreatedById == parameter.UserId.ToString())
            //                || (manager.IsManager && userInCurrentOrg.Contains(Guid.Parse(l.CreatedById))))
            //                && !l.WaitingForApproval
            //                orderby l.CreatedDate descending
            //                select new LeadEntityModel
            //                {
            //                    LeadId = l.LeadId,
            //                    ContactId = c.ContactId,
            //                    CompanyId = l.CompanyId,
            //                    FullName = c.FirstName + " " + c.LastName,
            //                    Email = c.Email,
            //                    Phone = c.Phone,
            //                    StatusName = y.CategoryName,
            //                    StatusCode = y.CategoryCode,
            //                    PersonInChargeId = l.PersonInChargeId,
            //                    AvatarUrl = string.IsNullOrEmpty(c.AvatarUrl) ? "" : c.AvatarUrl,
            //                    Active = l.Active
            //                }).ToList();

            //    leadList.ForEach(lead =>
            //    {
            //        var picId = lead.PersonInChargeId;
            //        if (picId != Guid.Empty && picId != null)
            //        {
            //            var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == picId);
            //            lead.PersonInChargeFullName = emp.EmployeeName;
            //            lead.PersonInChargeAvatarUrl = context.Contact.FirstOrDefault(c => c.ObjectId == emp.EmployeeId).AvatarUrl;
            //        }
            //    });
            //}
            #endregion

            return new GetLeadByStatusResult
            {
                ListLead = leadList,
            };
        }

        public GetLeadByNameResult GetLeadByName(GetLeadByNameParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.LEAD, "Get Lead by name", parameter.UserId);
            var empId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var quote = context.Quote.ToList();
            var note = context.Note.ToList();
            var leadList = (from l in context.Lead
                            join c in context.Contact on l.LeadId equals c.ObjectId
                            join c1 in context.Contact on l.PersonInChargeId equals c1.ObjectId into gj
                            join ctx in context.Category on l.StatusId equals ctx.CategoryId into cl
                            from y in cl.DefaultIfEmpty()
                            from x in gj.DefaultIfEmpty()
                            where (c.FirstName.Contains(parameter.Name) || c.LastName.Contains(parameter.Name)) && (l.PersonInChargeId == empId || (l.PersonInChargeId == Guid.Empty && l.CreatedById == parameter.UserId.ToString()))
                            orderby l.CreatedDate descending
                            select new LeadEntityModel
                            {
                                LeadId = l.LeadId,
                                ContactId = c.ContactId,
                                CompanyId = l.CompanyId,
                                FullName = c.FirstName + " " + c.LastName,
                                PersonInChargeFullName = x.ObjectType == "EMP" ? x.FirstName + " " + x.LastName : "",
                                Email = c.Email,
                                Phone = c.Phone,
                                StatusName = y.CategoryName,
                                StatusCode = y.CategoryCode,
                                PersonInChargeAvatarUrl = x.ObjectType == "EMP" ? x.AvatarUrl : "",
                                AvatarUrl = string.IsNullOrEmpty(c.AvatarUrl) ? "" : c.AvatarUrl,
                                CountLead = CheckCountInformationLead(l.LeadId, quote, note)
                            }).ToList();

            return new GetLeadByNameResult
            {
                ListLead = leadList
            };
        }

        public GetEmployeeWithNotificationPermisisonResult GetEmployeeWithNotificationPermisison(GetEmployeeWithNotificationPermisisonParameter parameter)
        {
            var currentUserEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentUserOrgId = context.Employee.FirstOrDefault(e => e.EmployeeId == currentUserEmpId).OrganizationId;
            var notificationPermission = context.Permission.FirstOrDefault(p => p.PermissionCode == "lead/notification").PermissionId;
            var permisisonSetContainsNoti = context.PermissionSet.Where(ps => ps.PermissionId.Contains(notificationPermission.ToString())).Select(ps => ps.PermissionSetId);
            var userHasPermisisonSetContainsNoti = context.PermissionMapping.Where(pm => permisisonSetContainsNoti.Contains(pm.PermissionSetId)).Select(pm => pm.UserId);
            var userIdList = context.User.Where(u => userHasPermisisonSetContainsNoti.Contains(u.UserId)).Select(e => e.EmployeeId);
            var empWithSameOrg = context.Employee.Where(e => e.OrganizationId == currentUserOrgId && userIdList.Contains(e.EmployeeId)).ToList();

            for (int i = 0; i < empWithSameOrg.Count; i++)
            {
                if (empWithSameOrg[i].EmployeeId == currentUserEmpId)
                {
                    empWithSameOrg[i].EmployeeName = empWithSameOrg[i].EmployeeName + " (tôi)";
                    break;
                }
            }

            return new GetEmployeeWithNotificationPermisisonResult()
            {
                EmployeeList = empWithSameOrg,
                Status = true
            };
        }

        public ChangeLeadStatusToUnfollowResult ChangeLeadStatusToUnfollow(ChangeLeadStatusToUnfollowParameter parameter)
        {
            parameter.LeadIdList.ForEach(leadId =>
            {
                //var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TLE");
                //var statusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == categoryType.CategoryTypeId && c.CategoryCode == "NDO");
                var lead = context.Lead.FirstOrDefault(l => l.LeadId == leadId);
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == leadId);
                lead.WaitingForApproval = true;
                //lead.StatusId = statusId.CategoryId;
                lead.UpdatedById = parameter.UserId.ToString();
                lead.UpdatedDate = DateTime.Now;
                contact.UpdatedById = parameter.UserId;
                contact.UpdatedDate = DateTime.Now;
                context.Lead.Update(lead);
                context.Contact.Update(contact);
            });

            context.SaveChanges();
            return new ChangeLeadStatusToUnfollowResult()
            {
                Status = true,
                Message = CommonMessage.Workflow.CHANGE_SUCCESS
            };
        }

        public ChangeLeadStatusToDeleteResult ChangeLeadStatusToDelete(ChangeLeadStatusToDeleteParameter parameter)
        {
            try
            {
                var lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.LeadId);
                var listContact = context.Contact.Where(c => c.ObjectId == parameter.LeadId && c.ObjectType == "LEA").ToList();
                var quote = context.Quote.Where(q => q.LeadId == parameter.LeadId).ToList();
                var saleBidding = context.SaleBidding.FirstOrDefault(s => s.LeadId == parameter.LeadId);
                var categoryId = context.Category.FirstOrDefault(c => c.CategoryCode == "DRAFT").CategoryId;

                if (lead == null)
                {
                    return new ChangeLeadStatusToDeleteResult()
                    {
                        Status = false,
                        Message = CommonMessage.Lead.LEAD_NOT_EXIST
                    };
                }

                if (saleBidding != null)
                {
                    return new ChangeLeadStatusToDeleteResult()
                    {
                        Status = false,
                        Message = CommonMessage.Lead.DELETE_FAIL_SALE_BIDDING_REFERENCES
                    };
                }

                if (quote.Count > 0)
                {
                    return new ChangeLeadStatusToDeleteResult()
                    {
                        Status = false,
                        Message = CommonMessage.Lead.DELETE_FAIL_QUOTE_REFERENCES
                    };
                }

                if (lead.StatusId == categoryId)
                {
                    context.Lead.Remove(lead);
                    context.Contact.RemoveRange(listContact);
                    context.SaveChanges();
                }

                return new ChangeLeadStatusToDeleteResult()
                {
                    Status = true,
                    Message = CommonMessage.Lead.DELETE_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new ChangeLeadStatusToDeleteResult()
                {
                    Status = false,
                    Message = CommonMessage.Lead.DELETE_FAIL
                };
            }
        }

        public ApproveRejectUnfollowLeadResult ApproveRejectUnfollowLead(ApproveRejectUnfollowLeadParameter parameter)
        {
            var currentEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            parameter.LeadIdList.ForEach(leadId =>
            {
                var lead = context.Lead.FirstOrDefault(l => l.LeadId == leadId);
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == leadId);
                lead.WaitingForApproval = false;
                lead.UpdatedById = parameter.UserId.ToString();
                lead.UpdatedDate = DateTime.Now;
                contact.UpdatedById = parameter.UserId;
                contact.UpdatedDate = DateTime.Now;
                if (parameter.IsApprove)
                {
                    var unfollowId = context.Category.FirstOrDefault(c => c.CategoryCode.ToLower() == "unfollowed").CategoryId;
                    lead.StatusId = unfollowId;
                    //Lead được phê duyệt ngừng theo dõi chuyển về Không có người phụ trách và chỉ quản lý có quyền phê duyệt lead mới nhìn thấy lead ngừng theo dõi
                    lead.PersonInChargeId = currentEmpId.Value;
                    //lead.PersonInChargeId = Guid.Empty;
                    //lead.CreatedById = parameter.UserId.ToString();
                }
                context.Lead.Update(lead);
                context.Contact.Update(contact);
            });

            context.SaveChanges();
            return new ApproveRejectUnfollowLeadResult()
            {
                Status = true,
                Message = parameter.IsApprove ? CommonMessage.Workflow.APPROVE : CommonMessage.Workflow.REJECT
            };
        }

        public GetEmployeeManagerResult GetEmployeeManager(GetEmployeeManagerParameter parameter)
        {
            var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
            var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
            var currentEmpOrg = emp.OrganizationId;
            var managerList = context.Employee.Where(e => e.OrganizationId == currentEmpOrg && e.IsManager).ToList();

            if (!managerList.Contains(emp))
            {
                managerList.Add(emp);
            }

            managerList.ForEach(manager =>
            {
                var managerAsUser = context.User.FirstOrDefault(u => u.EmployeeId == manager.EmployeeId);
                manager.EmployeeName = parameter.UserId == managerAsUser.UserId
                    ? manager.EmployeeName + " (tôi)"
                    : manager.EmployeeName;
            });

            return new GetEmployeeManagerResult()
            {
                Status = true,
                ManagerList = managerList
            };
        }

        public SendEmailLeadResult SendEmailLead(SendEmailLeadParameter parameter)
        {
            try
            {
                #region Add by Dung: Chặn gửi sau nhưng chọn giờ quá khứ
                var d = DateTime.Now;
                var currentTime = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0); //loại bỏ nhunwgx giá trị nhỏ hơn phút
                if (parameter.IsSendEmailNow == false && (parameter.SendEmailDate < currentTime || parameter.SendEmailHour < currentTime))
                {
                    return new SendEmailLeadResult
                    {
                        Message = "Không được chọn ngày giờ quá khứ",
                        Status = false,
                        ListCustomerEmailIgnored = new List<DataAccess.Databases.Entities.Contact>()
                    };
                }
                #endregion

                var listCustomerEmailIgnored = new List<DataAccess.Databases.Entities.Contact>(); //thêm danh sách những khách hàng hoặc lead không gửi email
                var listCustomerGotCustomerCareId = new List<Guid>(); //danh sách những khách hàng đã có chương trình chăm sóc khách hàng
                if (parameter.LeadIdList.Count > 0)
                {
                    var EmployeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                    var FromTo = context.Contact.FirstOrDefault(c => c.ObjectId == EmployeeId && c.ObjectType == "EMP")?.Email;
                    List<Queue> lstQueue = new List<Queue>();
                    List<Note> lstNote = new List<Note>();

                    if (FromTo != null && parameter.LeadIdList != null)
                    {
                        var customers = context.Customer.Where(c => parameter.LeadIdList.Contains(c.CustomerId)).ToList();
                        var contacts = context.Contact.Where(c => parameter.LeadIdList.Contains(c.ObjectId)
                        && (c.ObjectType == ObjectType.CUSTOMER || c.ObjectType == ObjectType.LEAD || c.ObjectType == ObjectType.CUSTOMERCONTACT)).ToList();//gửi người liên hệ của khách hàng nếu có

                        parameter.LeadIdList.ForEach(item =>
                        {
                            var contact = contacts.FirstOrDefault(c => c.ObjectId == item && (c.ObjectType == ObjectType.CUSTOMER || c.ObjectType == ObjectType.LEAD)); //Edit By Hung
                            var customer = customers.FirstOrDefault(f => f.CustomerId == item);
                            if (contact != null)
                            {

                                var sendContent = replaceTokenForContent(parameter.SendContent, contact, customer); //Edit By Hung
                                var title = "";
                                if (customer != null)
                                {
                                    title = parameter.Title + " - " + customer.CustomerName;

                                    #region Tạo CSKH
                                    //Tạo CSKH
                                    listCustomerGotCustomerCareId.Add(customer.CustomerId);

                                    var customerCare = new CustomerCare();
                                    var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                                    var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                                    var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                                    var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                                    var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                                    var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                                    var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Email" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                                    int currentYear = DateTime.Now.Year % 100;
                                    int currentMonth = DateTime.Now.Month;
                                    int currentDate = DateTime.Now.Day;
                                    var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                                    int MaxNumberCode = 0;
                                    if (lstRequestPayment.Count > 0)
                                    {
                                        MaxNumberCode = lstRequestPayment.Max();
                                    }
                                    customerCare.CustomerCareCode = string.Format("CSKH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                                    customerCare.NumberCode = MaxNumberCode + 1;
                                    customerCare.YearCode = currentYear;
                                    customerCare.MonthCode = currentMonth;
                                    customerCare.DateCode = currentDate;
                                    customerCare.EmployeeCharge = employeeId;
                                    customerCare.EffecttiveFromDate = DateTime.Now;
                                    customerCare.EffecttiveToDate = DateTime.Now;
                                    customerCare.CustomerCareContactType = customerCareContactType;
                                    customerCare.CustomerCareTitle = title;
                                    customerCare.CustomerCareContent = "";
                                    customerCare.CustomerCareContentEmail = sendContent;
                                    customerCare.IsSendEmailNow = true;
                                    customerCare.CustomerCareType = 1;
                                    customerCare.StatusId = customerCareStatusId;
                                    customerCare.CreateDate = DateTime.Now;
                                    customerCare.CreateById = parameter.UserId;
                                    context.CustomerCare.Add(customerCare);
                                    context.SaveChanges();

                                    //Tạo khách hàng của chương trình CSKH
                                    CustomerCareCustomer CustomerCareCustomerE = new CustomerCareCustomer
                                    {
                                        CustomerCareId = customerCare.CustomerCareId,
                                        CustomerId = customer.CustomerId,
                                        StatusId = statusId,
                                        CreateDate = DateTime.Now,
                                        CreateById = parameter.UserId,
                                    };
                                    context.CustomerCareCustomer.Add(CustomerCareCustomerE);

                                    //Tạo bộ lọc
                                    CustomerCareFilter customerCareFilter = new CustomerCareFilter
                                    {
                                        CustomerCareId = customerCare.CustomerCareId,
                                        QueryContent = "",
                                        CreateDate = DateTime.Now,
                                        CreateById = parameter.UserId
                                    };
                                    context.CustomerCareFilter.Add(customerCareFilter);
                                    context.SaveChanges();
                                    #endregion
                                }
                                else
                                {
                                    title = parameter.Title + " - " + contact.FirstName + " " + contact.LastName;
                                }

                                #region contact Email
                                if (contact.Email != null)
                                {
                                    if (contact.Email.Trim() != "")
                                    {
                                        Queue queueCreate = new Queue
                                        {
                                            FromTo = FromTo,
                                            SendTo = contact.Email, //Edit By Hung
                                            SendContent = sendContent,
                                            Method = "Email",
                                            Title = title, //Edit By Hung
                                            IsSend = false,
                                            SenDate = parameter.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.SendEmailDate, parameter.SendEmailHour),
                                            CreateDate = DateTime.Now,
                                            CreateById = parameter.UserId
                                        };
                                        lstQueue.Add(queueCreate);

                                        Note noteCreate = new Note
                                        {
                                            Description = "Đã gửi email cho khách hàng",
                                            Type = "ADD",
                                            ObjectId = item,
                                            ObjectType = contact.ObjectType, //Edit By Hung
                                            Active = true,
                                            CreatedById = parameter.UserId,
                                            CreatedDate = DateTime.Now,
                                            NoteTitle = "đã thêm ghi chú"
                                        };
                                        lstNote.Add(noteCreate);
                                    }
                                }
                                #endregion

                                #region contact WorkEmail
                                if (contact.WorkEmail != null && contact.WorkEmail != contact.Email)
                                {
                                    if (contact.WorkEmail.Trim() != "")
                                    {
                                        Queue queueCreate = new Queue
                                        {
                                            FromTo = FromTo,
                                            SendTo = contact.WorkEmail, //Edit By Hung
                                            SendContent = sendContent,
                                            Method = "Email",
                                            Title = title, //Edit By Hung
                                            IsSend = false,
                                            SenDate = parameter.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.SendEmailDate, parameter.SendEmailHour),
                                            CreateDate = DateTime.Now,
                                            CreateById = parameter.UserId
                                        };
                                        lstQueue.Add(queueCreate);

                                        Note noteCreate = new Note
                                        {
                                            Description = "Đã gửi email cho khách hàng",
                                            Type = "ADD",
                                            ObjectId = item,
                                            ObjectType = contact.ObjectType, //Edit By Hung
                                            Active = true,
                                            CreatedById = parameter.UserId,
                                            CreatedDate = DateTime.Now,
                                            NoteTitle = "đã thêm ghi chú"
                                        };
                                        lstNote.Add(noteCreate);
                                    }
                                }
                                #endregion

                                #region contact OtherEmail
                                if (contact.OtherEmail != null && contact.OtherEmail != contact.WorkEmail && contact.OtherEmail != contact.Email)
                                {
                                    if (contact.OtherEmail.Trim() != "")
                                    {
                                        Queue queueCreate = new Queue
                                        {
                                            FromTo = FromTo,
                                            SendTo = contact.OtherEmail, //Edit By Hung
                                            SendContent = sendContent,
                                            Method = "Email",
                                            Title = title, //Edit By Hung
                                            IsSend = false,
                                            SenDate = parameter.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.SendEmailDate, parameter.SendEmailHour),
                                            CreateDate = DateTime.Now,
                                            CreateById = parameter.UserId
                                        };
                                        lstQueue.Add(queueCreate);

                                        Note noteCreate = new Note
                                        {
                                            Description = "Đã gửi email cho khách hàng",
                                            Type = "ADD",
                                            ObjectId = item,
                                            ObjectType = contact.ObjectType, //Edit By Hung
                                            Active = true,
                                            CreatedById = parameter.UserId,
                                            CreatedDate = DateTime.Now,
                                            NoteTitle = "đã thêm ghi chú"
                                        };
                                        lstNote.Add(noteCreate);
                                    }
                                }
                                #endregion

                                #region lấy danh sách những khách hàng hoặc lead không có email (vì KHCN không có người liên hệ nên không cần check thêm)
                                if (
                                (contact.Email == null || contact.Email?.Trim() == "")
                                && (contact.WorkEmail == null || contact.WorkEmail?.Trim() == "")
                                && (contact.OtherEmail == null || contact.OtherEmail?.Trim() == ""))
                                {
                                    listCustomerEmailIgnored.Add(contact);
                                }
                                #endregion
                            }

                            #region gửi người liên hệ nếu có của KHDN
                            var cus_con_list = contacts.Where(c => c.ObjectId == item && (c.ObjectType == ObjectType.CUSTOMERCONTACT)).ToList();
                            cus_con_list?.ForEach(customer_contact =>
                            {
                                var sendContent = replaceTokenForContent(parameter.SendContent, customer_contact, customer);
                                var title = parameter.Title + " - " + customer.CustomerName;
                                //gửi email cho người liên hệ
                                #region customer_contact Email
                                if (customer_contact.Email != null)
                                {
                                    if (customer_contact.Email.Trim() != "")
                                    {
                                        Queue queueCreate = new Queue
                                        {
                                            FromTo = FromTo,
                                            SendTo = customer_contact.Email,
                                            SendContent = sendContent,
                                            Method = "Email",
                                            Title = title,
                                            IsSend = false,
                                            SenDate = parameter.IsSendEmailNow == true ? DateTime.Now : SetDate(parameter.SendEmailDate, parameter.SendEmailHour),
                                            CreateDate = DateTime.Now,
                                            CreateById = parameter.UserId
                                        };
                                        lstQueue.Add(queueCreate);
                                    }
                                }
                                #endregion
                            });
                            #endregion
                        });

                        if (lstQueue.Count > 0)
                        {
                            context.Queue.AddRange(lstQueue);
                            context.SaveChanges();
                        }

                        if (lstNote.Count > 0)
                        {
                            context.Note.AddRange(lstNote);
                            context.SaveChanges();
                        }
                    }
                }

                return new SendEmailLeadResult
                {
                    Message = "Gửi email thành công",
                    Status = true,
                    ListCustomerEmailIgnored = listCustomerEmailIgnored
                };
            }
            catch (Exception e)
            {
                return new SendEmailLeadResult
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }

        private DateTime SetDate(DateTime? SendEmailDate, DateTime? SendEmailHour)
        {
            DateTime newDate = DateTime.Now;
            if (SendEmailDate.HasValue && SendEmailHour.HasValue)
            {
                int Year = SendEmailDate.Value.Year;
                int Month = SendEmailDate.Value.Month;
                int Day = SendEmailDate.Value.Day;
                int Hour = SendEmailHour.Value.Hour;
                int Minute = SendEmailHour.Value.Minute;

                newDate = new DateTime(Year, Month, Day, Hour, Minute, 0);
            }
            return newDate;
        }

        private DateTime SetDate2(DateTime? SendEmailDate, TimeSpan? SendEmailHour)
        {
            DateTime newDate = DateTime.Now;
            if (SendEmailDate.HasValue && SendEmailHour.HasValue)
            {
                int Year = SendEmailDate.Value.Year;
                int Month = SendEmailDate.Value.Month;
                int Day = SendEmailDate.Value.Day;

                newDate = new DateTime(Year, Month, Day) + SendEmailHour.Value;
            }
            return newDate;
        }

        public SendSMSLeadResult SendSMSLead(SendSMSLeadParameter parameter)
        {
            try
            {
                #region Add by Dung: Chặn gửi sau nhưng chọn giờ quá khứ
                var d = DateTime.Now;
                var currentTime = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0); //loại bỏ nhunwgx giá trị nhỏ hơn phút
                if (parameter.IsSendSMSNow == false && (parameter.SendSMSDate < currentTime || parameter.SendSMSHour < currentTime))
                {
                    return new SendSMSLeadResult
                    {
                        Message = "Không được chọn ngày giờ quá khứ",
                        Status = false
                    };
                }
                #endregion

                if (parameter.LeadIdList.Count > 0)
                {
                    var EmployeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                    List<Queue> lstQueue = new List<Queue>();
                    List<Note> lstNote = new List<Note>();
                    var customers = context.Customer.Where(c => parameter.LeadIdList.Contains(c.CustomerId)).ToList();
                    var contacts = context.Contact.Where(c => parameter.LeadIdList.Contains(c.ObjectId) && (c.ObjectType == ObjectType.CUSTOMER || c.ObjectType == ObjectType.LEAD)).ToList();
                    parameter.LeadIdList.ForEach(item =>
                    {
                        var contact = contacts.FirstOrDefault(c => c.ObjectId == item && (c.ObjectType == ObjectType.CUSTOMER || c.ObjectType == ObjectType.LEAD)); //Edit By Hung
                        var customer = customers.FirstOrDefault(f => f.CustomerId == item);

                        if (contact != null)
                        {

                            var sendContent = replaceTokenForContent(parameter.SendContent, contact, customer); //Edit By Hung
                            var title = "";
                            if (customer != null)
                            {
                                title = "Gửi SMS cho khách hàng";
                                #region Tạo CSKH
                                //Tạo CSKH
                                var customerCare = new CustomerCare();
                                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                                var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                                var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                                var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                                var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Email" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                                int currentYear = DateTime.Now.Year % 100;
                                int currentMonth = DateTime.Now.Month;
                                int currentDate = DateTime.Now.Day;
                                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                                int MaxNumberCode = 0;
                                if (lstRequestPayment.Count > 0)
                                {
                                    MaxNumberCode = lstRequestPayment.Max();
                                }
                                customerCare.CustomerCareCode = string.Format("CSKH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                                customerCare.NumberCode = MaxNumberCode + 1;
                                customerCare.YearCode = currentYear;
                                customerCare.MonthCode = currentMonth;
                                customerCare.DateCode = currentDate;
                                customerCare.EmployeeCharge = employeeId;
                                customerCare.EffecttiveFromDate = DateTime.Now;
                                customerCare.EffecttiveToDate = DateTime.Now;
                                customerCare.CustomerCareContactType = customerCareContactType;
                                customerCare.CustomerCareTitle = title;
                                customerCare.CustomerCareContent = "";
                                customerCare.CustomerCareContentEmail = sendContent;
                                customerCare.IsSendEmailNow = true;
                                customerCare.CustomerCareType = 1;
                                customerCare.StatusId = customerCareStatusId;
                                customerCare.CreateDate = DateTime.Now;
                                customerCare.CreateById = parameter.UserId;
                                context.CustomerCare.Add(customerCare);
                                context.SaveChanges();

                                //Tạo khách hàng của chương trình CSKH
                                CustomerCareCustomer CustomerCareCustomerE = new CustomerCareCustomer
                                {
                                    CustomerCareId = customerCare.CustomerCareId,
                                    CustomerId = customer.CustomerId,
                                    StatusId = statusId,
                                    CreateDate = DateTime.Now,
                                    CreateById = parameter.UserId,
                                };
                                context.CustomerCareCustomer.Add(CustomerCareCustomerE);

                                //Tạo bộ lọc
                                CustomerCareFilter customerCareFilter = new CustomerCareFilter
                                {
                                    CustomerCareId = customerCare.CustomerCareId,
                                    QueryContent = "",
                                    CreateDate = DateTime.Now,
                                    CreateById = parameter.UserId
                                };
                                context.CustomerCareFilter.Add(customerCareFilter);
                                context.SaveChanges();
                                #endregion
                            }
                            else
                            {
                                title = "Gửi SMS cho khách hàng - " + contact.FirstName + " " + contact.LastName;
                            }

                            #region Send Contact Phone
                            if (contact.Phone != null)
                            {
                                if (contact.Phone.Trim() != "")
                                {
                                    Queue queueCreate = new Queue
                                    {
                                        FromTo = "",
                                        SendTo = contact.Phone,
                                        SendContent = sendContent,
                                        Method = "SMS",
                                        Title = title,
                                        IsSend = false,
                                        SenDate = parameter.IsSendSMSNow == true ? DateTime.Now : SetDate(parameter.SendSMSDate, parameter.SendSMSHour),
                                        CreateDate = DateTime.Now,
                                        CreateById = parameter.UserId
                                    };
                                    lstQueue.Add(queueCreate);

                                    Note noteCreate = new Note
                                    {
                                        Description = "Đã gửi SMS cho khách hàng",
                                        Type = "ADD",
                                        ObjectId = item,
                                        ObjectType = contact.ObjectType,
                                        Active = true,
                                        CreatedById = parameter.UserId,
                                        CreatedDate = DateTime.Now,
                                        NoteTitle = "đã thêm ghi chú"
                                    };
                                    lstNote.Add(noteCreate);
                                }
                            }
                            #endregion

                            #region Send Contact WorkPhone
                            if (contact.WorkPhone != null && contact.Phone != contact.WorkPhone)
                            {
                                if (contact.WorkPhone.Trim() != "")
                                {
                                    Queue queueCreate = new Queue
                                    {
                                        FromTo = "",
                                        SendTo = contact.WorkPhone,
                                        SendContent = sendContent,
                                        Method = "SMS",
                                        Title = "Gửi SMS cho khách hàng tiềm năng",
                                        IsSend = false,
                                        SenDate = parameter.IsSendSMSNow == true ? DateTime.Now : SetDate(parameter.SendSMSDate, parameter.SendSMSHour),
                                        CreateDate = DateTime.Now,
                                        CreateById = parameter.UserId
                                    };
                                    lstQueue.Add(queueCreate);

                                    Note noteCreate = new Note
                                    {
                                        Description = "Đã gửi SMS cho khách hàng",
                                        Type = "ADD",
                                        ObjectId = item,
                                        ObjectType = contact.ObjectType,
                                        Active = true,
                                        CreatedById = parameter.UserId,
                                        CreatedDate = DateTime.Now,
                                        NoteTitle = "đã thêm ghi chú"
                                    };
                                    lstNote.Add(noteCreate);
                                }
                            }
                            #endregion

                            #region Send Contact OtherPhone
                            if (contact.OtherPhone != null && contact.OtherPhone != contact.WorkPhone && contact.OtherPhone != contact.Phone)
                            {
                                if (contact.OtherPhone.Trim() != "")
                                {
                                    Queue queueCreate = new Queue
                                    {
                                        FromTo = "",
                                        SendTo = contact.OtherPhone,
                                        SendContent = sendContent,
                                        Method = "SMS",
                                        Title = "Gửi SMS cho khách hàng tiềm năng",
                                        IsSend = false,
                                        SenDate = parameter.IsSendSMSNow == true ? DateTime.Now : SetDate(parameter.SendSMSDate, parameter.SendSMSHour),
                                        CreateDate = DateTime.Now,
                                        CreateById = parameter.UserId
                                    };
                                    lstQueue.Add(queueCreate);

                                    Note noteCreate = new Note
                                    {
                                        Description = "Đã gửi SMS cho khách hàng",
                                        Type = "ADD",
                                        ObjectId = item,
                                        ObjectType = contact.ObjectType,
                                        Active = true,
                                        CreatedById = parameter.UserId,
                                        CreatedDate = DateTime.Now,
                                        NoteTitle = "đã thêm ghi chú"
                                    };
                                    lstNote.Add(noteCreate);
                                }
                            }
                            #endregion
                        }
                    });

                    if (lstQueue.Count > 0)
                    {
                        context.Queue.AddRange(lstQueue);
                        context.SaveChanges();
                    }

                    if (lstNote.Count > 0)
                    {
                        context.Note.AddRange(lstNote);
                        context.SaveChanges();
                    }
                }
                return new SendSMSLeadResult
                {
                    Message = "Gửi SMS thành công",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new SendSMSLeadResult
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }

        private string replaceTokenForContent(string current_content, Contact contact, Customer customer)
        {
            var Name = TokenForContent.Name;
            var Hotline = TokenForContent.Hotline;
            var Address = TokenForContent.Address;

            if (current_content.Contains(Name))
            {
                if (customer != null)
                    current_content = current_content.Replace(Name, customer.CustomerName);
                else
                    current_content = current_content.Replace(Name, contact.FirstName + " " + contact.LastName);
            }
            if (current_content.Contains(Hotline))
            {
                current_content = current_content.Replace(Hotline, contact.Phone);
            }
            if (current_content.Contains(Address))
            {
                current_content = current_content.Replace(Address, contact.Address);
            }

            return current_content;
        }

        public ImportLeadResult ImportLead(ImportLeadParameter parameter)
        {
            string MessageError = string.Empty;
            using (var dbcxtransaction = context.Database.BeginTransaction())
            {
                try
                {
                    string Message = string.Empty;
                    List<Contact> lstcontact = new List<Contact>();
                    List<Lead> lstcontactLeadDuplicate = new List<Lead>();
                    List<Contact> lstcontactContactDuplicate = new List<Contact>();
                    List<Contact> lstcontactCustomerDuplicate = new List<Contact>();
                    List<Contact> lstcontactContact_CON_Duplicate = new List<Contact>();
                    bool checkIsDublicate = false;

                    if (parameter.FileList != null && parameter.FileList.Count > 0)
                    {
                        using (var stream = new MemoryStream())
                        {

                            parameter.FileList[0].CopyTo(stream);
                            using (ExcelPackage package = new ExcelPackage(stream))
                            {

                                var listProvince = context.Province.ToList();
                                var listDistrict = context.District.ToList();
                                var listWard = context.Ward.ToList();
                                //Nhu cầu sản phẩm dịch vụ 
                                var listInterestedGroupId = (from categoryT in context.CategoryType
                                                             join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                             where categoryT.CategoryTypeCode == "NHU"
                                                             select category).ToList();
                                //Mức độ tiềm năng
                                var listPotentialId = (from categoryT in context.CategoryType
                                                       join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                       where categoryT.CategoryTypeCode == "MTN"
                                                       select category).ToList();
                                //Phương thức thanh toán
                                var listPaymentMethodId = (from categoryT in context.CategoryType
                                                           join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                           where categoryT.CategoryTypeCode == "PTO"
                                                           select category).ToList();
                                //Trạng thái Lead
                                var statusType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TLE" && ct.Active == true).CategoryTypeId;
                                var statusLeadId = context.Category.FirstOrDefault(c => c.CategoryTypeId == statusType && c.Active == true && c.CategoryCode == "NDO").CategoryId;

                                ExcelWorksheet worksheet = package.Workbook.Worksheets["Lead"];

                                if (worksheet == null)
                                {
                                    return new ImportLeadResult
                                    {
                                        Message = "File excel không đúng theo template",
                                        Status = false
                                    };
                                }

                                //Group cells by row
                                var rowcellgroups = worksheet.Cells["A:N"].GroupBy(c => c.Start.Row);
                                //Loại bỏ 2 dòng tiêu đề
                                var groups = rowcellgroups.Skip(1);
                                //Group theo từng ngày
                                var cv = (from item in groups
                                          group item by new
                                          {
                                              item.First().Value,
                                          } into gcs
                                          select gcs).ToList();

                                if (cv.Count == 0)
                                {
                                    return new ImportLeadResult
                                    {
                                        Message = "File excel không tồn tại bản ghi nào!",
                                        Status = false
                                    };
                                }

                                var checkNameLead = (from item in cv
                                                     where item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() == null
                                                     select item).ToList();
                                if (checkNameLead.Count == 0)
                                {
                                    var checkNameLeadValue = (from item in cv
                                                              where item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value == null
                                                              select item).ToList();
                                    if (checkNameLeadValue.Count > 0)
                                    {
                                        string MessageOut = "Không được để trống tên tại vị trí có số thứ tự ";
                                        checkNameLeadValue.ForEach(item =>
                                        {
                                            MessageOut = MessageOut + item.Key.Value.ToString() + ";";
                                        });
                                        return new ImportLeadResult
                                        {
                                            Message = MessageOut,
                                            Status = false
                                        };
                                    }
                                }
                                else
                                {
                                    string MessageOut = "Không được để trống tên tại vị trí có số thứ tự ";
                                    checkNameLead.ForEach(item =>
                                    {
                                        MessageOut = MessageOut + item.Key.Value.ToString() + ";";
                                    });
                                    return new ImportLeadResult
                                    {
                                        Message = MessageOut,
                                        Status = false
                                    };
                                }
                                var checkPhoneLead = (from item in cv
                                                      where item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() == null
                                                      select item).ToList();
                                if (checkPhoneLead.Count == 0)
                                {
                                    var checkPhoneLeadValue = (from item in cv
                                                               where item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value == null
                                                               select item).ToList();
                                    if (checkPhoneLeadValue.Count > 0)
                                    {
                                        string MessageOut = "Không được để trống Phone tại vị trí có số thứ tự ";
                                        checkPhoneLeadValue.ForEach(item =>
                                        {
                                            MessageOut = MessageOut + item.Key.Value.ToString() + ";";
                                        });
                                        return new ImportLeadResult
                                        {
                                            Message = MessageOut,
                                            Status = false
                                        };
                                    }
                                }
                                else
                                {
                                    string MessageOut = "Không được để trống Phone tại vị trí có số thứ tự ";
                                    checkPhoneLead.ForEach(item =>
                                    {
                                        MessageOut = MessageOut + item.Key.Value.ToString() + ";";
                                    });
                                    return new ImportLeadResult
                                    {
                                        Message = MessageOut,
                                        Status = false
                                    };
                                }

                                // tìm các bản ghi có email/sdt trùng nhau trong cv


                                List<string> dulicateEmail = new List<string>();
                                List<string> lstDublicateEmail = new List<string>();
                                List<string> dulicatePhone = new List<string>();
                                List<string> lstDublicatePhone = new List<string>();
                                List<string> stt = new List<string>();
                                cv.ForEach(g =>
                                {
                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                        {
                                            string Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                            dulicateEmail.Add(Email);
                                        }
                                    }
                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                        {
                                            string Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString().Trim();
                                            dulicatePhone.Add(Phone);
                                        }
                                    }
                                });

                                //Lọc các giá trị email != ""
                                dulicateEmail = dulicateEmail.Where(x => x.Trim() != "").ToList();
                                //Danh sách email không có giá trị bị trùng
                                var lstDistinctEmail = dulicateEmail.Distinct().ToList();
                                //Danh sách email bị trùng
                                lstDublicateEmail = dulicateEmail.GroupBy(x => x)
                                                      .Where(g => g.Count() > 1)
                                                      .Select(y => y.Key)
                                                      .ToList();

                                //Danh sách số điện thoại không có giá trị bị trùng
                                var lstDistinctPhone = dulicatePhone.Distinct().ToList();
                                //Danh sách số điện thoại bị trùng
                                lstDublicatePhone = dulicatePhone.GroupBy(x => x)
                                                      .Where(g => g.Count() > 1)
                                                      .Select(y => y.Key)
                                                      .ToList();

                                if (lstDublicatePhone.Count() > 0 || lstDublicateEmail.Count() > 0)
                                {
                                    checkIsDublicate = true;

                                    if (lstDublicateEmail.Count > 0)
                                    {
                                        cv.ForEach(item =>
                                        {
                                            if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                            {
                                                var currentEmail = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Text.Trim();
                                                if (lstDublicateEmail.FirstOrDefault(x => x == currentEmail) != null)
                                                {
                                                    stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                }
                                            }
                                        });
                                    }

                                    if (lstDublicatePhone.Count > 0)
                                    {
                                        cv.ForEach(item =>
                                        {
                                            if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                            {
                                                var currentPhone = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Text.Trim();
                                                if (lstDublicatePhone.FirstOrDefault(x => x == currentPhone) != null)
                                                {
                                                    stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                }
                                            }
                                        });
                                    }
                                }

                                stt = stt.Distinct().ToList();

                                cv.ForEach(g =>
                                {
                                    if (g.Key.Value == null) return;
                                    string Email = string.Empty;
                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                        {
                                            Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                        }
                                    }

                                    string Phone = string.Empty;
                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                        {
                                            Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString().Trim();
                                        }
                                    }

                                    Contact leadEntityCheckEmailDuplicate = null;
                                    Contact leadEntityCheckPhoneDuplicate = null;

                                    if (!string.IsNullOrEmpty(Email))
                                    {
                                        var contactCus = context.Contact.Where(w => w.Email.ToLower() == Email.Trim().ToLower() && w.ObjectType == "CUS" && w.Active == true).FirstOrDefault();
                                        if (contactCus == null)
                                        {
                                            var contactLead = context.Contact.Where(w => w.Email.ToLower() == Email.Trim().ToLower() && w.ObjectType == "LEA" && w.Active == true).FirstOrDefault();
                                            if (contactLead != null)
                                            {
                                                var lead = context.Lead.FirstOrDefault(lea => lea.LeadId == contactLead.ObjectId && lea.Active == true);
                                                if (lead.StatusId != statusLeadId)
                                                {
                                                    leadEntityCheckEmailDuplicate = contactLead;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            leadEntityCheckEmailDuplicate = contactCus;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(Phone))
                                    {
                                        var contactCus = context.Contact.Where(w => w.Phone.ToLower() == Phone.Trim().ToLower() && w.ObjectType == "CUS" && w.Active == true).FirstOrDefault();
                                        if (contactCus == null)
                                        {
                                            var contactLead = context.Contact.Where(w => w.Phone.ToLower() == Phone.Trim().ToLower() && w.ObjectType == "LEA" && w.Active == true).FirstOrDefault();
                                            if (contactLead != null)
                                            {
                                                var lead = context.Lead.FirstOrDefault(lea => lea.LeadId == contactLead.ObjectId && lea.Active == true);
                                                if (lead.StatusId != statusLeadId)
                                                {
                                                    leadEntityCheckPhoneDuplicate = contactLead;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            leadEntityCheckPhoneDuplicate = contactCus;
                                        }
                                    }
                                    if (leadEntityCheckEmailDuplicate == null && leadEntityCheckPhoneDuplicate == null)
                                    {
                                        Lead CreateNewLead = new Lead();

                                        if (stt.FirstOrDefault(x => x == g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim()) == null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString().Trim();
                                                    var objectInterestedGroupId = listInterestedGroupId.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    if (objectInterestedGroupId == null)
                                                    {
                                                        MessageError = "Không được để trống->>Nhu cầu sản phẩm";
                                                    }
                                                    else
                                                    {
                                                        CreateNewLead.InterestedGroupId = objectInterestedGroupId.CategoryId;
                                                    }
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString().Trim();
                                                    var objectPotentialId = listPotentialId.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    if (objectPotentialId == null)
                                                    {
                                                        MessageError = "Không được để trống->>Mức độ tiềm năng";
                                                    }
                                                    else
                                                    {
                                                        CreateNewLead.PotentialId = objectPotentialId.CategoryId;
                                                    }
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "M")).First().Value.ToString().Trim();
                                                    var objectPaymentMethodId = listPaymentMethodId.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    if (objectPaymentMethodId == null)
                                                    {
                                                        MessageError = "Không được để trống->>Mức độ tiềm năng";
                                                    }
                                                    else
                                                    {
                                                        CreateNewLead.PaymentMethodId = objectPaymentMethodId.CategoryId;
                                                    }
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                {
                                                    Company company = new Company()
                                                    {
                                                        CompanyId = Guid.NewGuid(),
                                                        CompanyName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString().Trim(),
                                                        CreatedById = parameter.UserId,
                                                        CreatedDate = DateTime.Now,
                                                        Active = true
                                                    };
                                                    CreateNewLead.CompanyId = company.CompanyId;
                                                    context.Company.Add(company);
                                                }
                                            }

                                            CreateNewLead.StatusId = (from c in context.Category
                                                                      join ct in context.CategoryType on c.CategoryTypeId equals ct.CategoryTypeId
                                                                      where (c.CategoryCode == "MOI" && ct.CategoryTypeCode == "TLE")
                                                                      select new
                                                                      {
                                                                          c.CategoryId
                                                                      }).FirstOrDefault().CategoryId;
                                            var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                                            var empId = user.EmployeeId;
                                            CreateNewLead.PersonInChargeId = empId ?? empId.Value;
                                            CreateNewLead.CreatedById = parameter.UserId.ToString();
                                            CreateNewLead.CreatedDate = DateTime.Now;
                                            //khách hàng
                                            context.Lead.Add(CreateNewLead);
                                            context.SaveChanges();

                                            string FullNameContact_COn = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                {
                                                    FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                }
                                            }
                                            string[] array = FullNameContact_COn.Split(' ');
                                            string FirstName = array[0];
                                            string LastName = string.Empty;
                                            for (int i = 1; i < array.Length; ++i)
                                            {
                                                LastName = LastName + " " + array[i];
                                            };
                                            Contact CreateContact = new Contact();
                                            CreateContact.ObjectId = CreateNewLead.LeadId;
                                            CreateContact.FirstName = FirstName;
                                            CreateContact.LastName = LastName;
                                            CreateContact.ObjectType = "LEA";
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                {
                                                    CreateContact.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString().Trim()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                {
                                                    CreateContact.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString().Trim()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                {
                                                    CreateContact.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString().Trim()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                {
                                                    CreateContact.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                {
                                                    CreateContact.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                {
                                                    CreateContact.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString().Trim();
                                                }
                                            }


                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    CreateContact.Gender = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                {
                                                    CreateContact.IdentityId = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString().Trim();
                                                }
                                            }

                                            CreateContact.CreatedById = parameter.UserId;
                                            CreateContact.CreatedDate = DateTime.Now;
                                            lstcontact.Add(CreateContact);
                                        }
                                    }
                                    else
                                    {
                                        Contact contact;
                                        contact = leadEntityCheckEmailDuplicate != null ? leadEntityCheckEmailDuplicate : leadEntityCheckPhoneDuplicate;
                                        string CustomerNameV = string.Empty;
                                        if (contact.ObjectType == "LEA")
                                        {
                                            Lead LeadDuplicate = new Lead
                                            {
                                                LeadId = contact.ObjectId,
                                            };

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString().Trim();
                                                    var objectInterestedGroupId = listInterestedGroupId.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    if (objectInterestedGroupId == null)
                                                    {
                                                        MessageError = "Không được để trống->>Nhu cầu sản phẩm";
                                                    }
                                                    else
                                                    {
                                                        LeadDuplicate.InterestedGroupId = objectInterestedGroupId.CategoryId;
                                                    }
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString().Trim();
                                                    var objectPotentialId = listPotentialId.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    if (objectPotentialId == null)
                                                    {
                                                        MessageError = "Không được để trống->>Mức độ tiềm năng";
                                                    }
                                                    else
                                                    {
                                                        LeadDuplicate.PotentialId = objectPotentialId.CategoryId;
                                                    }
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "M")).First().Value.ToString().Trim();
                                                    var objectPaymentMethodId = listPaymentMethodId.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    if (objectPaymentMethodId == null)
                                                    {
                                                        MessageError = "Không được để trống->>Mức độ tiềm năng";
                                                    }
                                                    else
                                                    {
                                                        LeadDuplicate.PaymentMethodId = objectPaymentMethodId.CategoryId;
                                                    }
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                {
                                                    Company company = new Company()
                                                    {
                                                        CompanyId = Guid.NewGuid(),
                                                        CompanyName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString().Trim(),
                                                        CreatedById = parameter.UserId,
                                                        CreatedDate = DateTime.Now,
                                                        Active = true
                                                    };
                                                    LeadDuplicate.CompanyId = company.CompanyId;
                                                    context.Company.Add(company);
                                                }
                                            }
                                            lstcontactLeadDuplicate.Add(LeadDuplicate);
                                            ///
                                            string FullNameContact_COn = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                {
                                                    FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                }
                                            }
                                            string[] array = FullNameContact_COn.Split(' ');
                                            string FirstName = array[0];
                                            string LastName = string.Empty;
                                            for (int i = 1; i < array.Length; ++i)
                                            {
                                                LastName = LastName + " " + array[i];
                                            };

                                            Contact ContactDuplicate = new Contact();
                                            ContactDuplicate.ObjectId = contact.ObjectId;
                                            ContactDuplicate.ContactId = contact.ContactId;
                                            ContactDuplicate.FirstName = FirstName;
                                            ContactDuplicate.LastName = LastName;

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                {
                                                    ContactDuplicate.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString().Trim()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                {
                                                    ContactDuplicate.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString().Trim()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                {
                                                    ContactDuplicate.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString().Trim()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                {
                                                    ContactDuplicate.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                {
                                                    ContactDuplicate.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                {
                                                    ContactDuplicate.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString().Trim();
                                                }
                                            }


                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    ContactDuplicate.Gender = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                {
                                                    ContactDuplicate.IdentityId = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString().Trim();
                                                }
                                            }
                                            lstcontactContactDuplicate.Add(ContactDuplicate);
                                        }
                                        if (contact.ObjectType == "CUS")
                                        {
                                            string FullNameContact_COn = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                {
                                                    FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                }
                                            }
                                            string[] array = FullNameContact_COn.Split(' ');
                                            string FirstName = array[0];
                                            string LastName = string.Empty;
                                            for (int i = 1; i < array.Length; ++i)
                                            {
                                                LastName = LastName + " " + array[i];
                                            };

                                            Contact ContactDuplicate = new Contact();
                                            ContactDuplicate.ObjectId = contact.ObjectId;
                                            ContactDuplicate.ContactId = contact.ContactId;
                                            ContactDuplicate.FirstName = FirstName;
                                            ContactDuplicate.LastName = LastName;

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                {
                                                    ContactDuplicate.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString().Trim()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                {
                                                    ContactDuplicate.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString().Trim()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                {
                                                    ContactDuplicate.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString().Trim()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                {
                                                    ContactDuplicate.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                {
                                                    ContactDuplicate.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                {
                                                    ContactDuplicate.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString().Trim();
                                                }
                                            }


                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    ContactDuplicate.Gender = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                {
                                                    ContactDuplicate.IdentityId = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString().Trim();
                                                }
                                            }
                                            lstcontactCustomerDuplicate.Add(ContactDuplicate);
                                        }
                                    }
                                });

                                context.Contact.AddRange(lstcontact);
                                context.SaveChanges();

                            }
                        }
                    }
                    if (lstcontactContactDuplicate.Count > 0 || lstcontactLeadDuplicate.Count > 0 || checkIsDublicate == true)
                    {
                        Message = "Đã import thành công,một số dòng đã bị lặp";
                    }
                    else
                    {
                        Message = "Đã import thành công";
                    }
                    dbcxtransaction.Commit();
                    return new ImportLeadResult
                    {
                        lstcontactLeadDuplicate = lstcontactLeadDuplicate,
                        lstcontactContactDuplicate = lstcontactContactDuplicate,
                        lstcontactCustomerDuplicate = lstcontactCustomerDuplicate,
                        Message = Message,
                        Status = true,
                    };
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return new ImportLeadResult
                    {
                        Message = "Đã có lỗi xảy ra trong quá trình import",
                        Status = true,
                    };
                }
            }
        }

        public UpdateLeadDuplicateResult UpdateLeadDuplicate(UpdateLeadDuplicateParameter parameter)
        {
            try
            {
                if (parameter.lstcontactLeadDuplicate.Count > 0)
                {
                    parameter.lstcontactLeadDuplicate.ForEach(item =>
                    {
                        var leadUpdate = context.Lead.FirstOrDefault(w => w.LeadId == item.LeadId);
                        if (leadUpdate != null)
                        {


                            leadUpdate.LeadId = item.LeadId;
                            if (item.InterestedGroupId != Guid.Empty)
                            {
                                if (item.InterestedGroupId != null)
                                {
                                    leadUpdate.InterestedGroupId = item.InterestedGroupId;
                                }
                            }
                            if (item.PotentialId != Guid.Empty)
                            {
                                if (item.PotentialId != null)
                                {
                                    leadUpdate.PotentialId = item.PotentialId;
                                }
                            }
                            if (item.PaymentMethodId != Guid.Empty)
                            {
                                if (item.PaymentMethodId != null)
                                {
                                    leadUpdate.PaymentMethodId = item.PaymentMethodId;
                                }
                            }

                            if (item.CompanyId != Guid.Empty)
                            {
                                if (item.CompanyId != null)
                                {
                                    leadUpdate.CompanyId = item.CompanyId;
                                }
                            }
                            leadUpdate.UpdatedById = parameter.UserId.ToString();
                            leadUpdate.UpdatedDate = DateTime.Now;
                            context.Lead.Update(leadUpdate);
                            context.SaveChanges();
                        }
                    });


                }

                if (parameter.lstcontactContactDuplicate.Count > 0)
                {
                    parameter.lstcontactContactDuplicate.ForEach(item =>
                    {
                        var contactUpdate = context.Contact.FirstOrDefault(w => w.ContactId == item.ContactId);
                        if (contactUpdate != null)
                        {
                            if (item.ProvinceId != Guid.Empty)
                            {
                                if (item.ProvinceId != null)
                                {
                                    contactUpdate.ProvinceId = item.ProvinceId;
                                }
                            }
                            if (item.DistrictId != Guid.Empty)
                            {
                                if (item.DistrictId != null)
                                {
                                    contactUpdate.DistrictId = item.DistrictId;
                                }
                            }
                            if (item.WardId != Guid.Empty)
                            {
                                if (item.WardId != null)
                                {
                                    contactUpdate.WardId = item.WardId;
                                }
                            }
                            contactUpdate.FirstName = item.FirstName;
                            contactUpdate.LastName = item.LastName;
                            contactUpdate.Address = item.Address;
                            contactUpdate.Gender = item.Gender;
                            contactUpdate.Email = item.Email;
                            contactUpdate.Phone = item.Phone;
                            contactUpdate.IdentityId = item.IdentityId;
                            contactUpdate.UpdatedById = parameter.UserId;
                            contactUpdate.UpdatedDate = DateTime.Now;

                            context.Contact.Update(contactUpdate);
                            context.SaveChanges();
                        }
                    });
                }
                return new UpdateLeadDuplicateResult
                {
                    Message = "Đã chỉnh sửa lại cơ hội",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new UpdateLeadDuplicateResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình cập nhật lại cơ hội",
                    Status = false
                };
            }
        }

        public DownloadTemplateLeadResult DownloadTemplateLead(DownloadTemplateLeadParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";

                string fileName = @"Template_Lead.xlsm";

                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateLeadResult
                {
                    ExcelFile = data,
                    Message = string.Format("Đã dowload file Template_Lead"),
                    NameFile = "Template_Lead",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new DownloadTemplateLeadResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình download",
                    Status = false
                };
            }
        }

        public int CheckCountInformationLead(Guid? LeadId, List<Quote> quote, List<Note> note)
        {
            var leadCount = quote.Where(l => l.ObjectTypeId == LeadId).Count();
            //leadCount += note.Where(l => l.ObjectId == LeadId).Count();

            return leadCount;
        }

        public CheckEmailLeadResult CheckEmailLead(CheckEmailLeadParameter parameter)
        {
            try
            {
                CheckEmailLeadResult result = new CheckEmailLeadResult();
                var CheckEmail = context.Contact.FirstOrDefault(e => e.Email == parameter.Email);
                if (CheckEmail != null)
                {
                    result.CheckEmail = true;
                    result.ObjectType = CheckEmail.ObjectType;
                    result.ContactId = CheckEmail.ContactId;
                    result.ObjectId = CheckEmail.ObjectId;
                }
                else
                {
                    result.CheckEmail = false;
                }
                return result;
            }
            catch (Exception e)
            {
                return new CheckEmailLeadResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình tìm kiếm",
                    Status = false
                };
            }
        }

        public CheckPhoneLeadResult CheckPhoneLead(CheckPhoneLeadParameter parameter)
        {
            try
            {
                CheckPhoneLeadResult result = new CheckPhoneLeadResult();
                var CheckPhone = context.Contact.FirstOrDefault(e => e.Phone == parameter.PhoneNumber);
                if (CheckPhone != null)
                {
                    result.CheckPhone = true;
                    result.ObjectType = CheckPhone.ObjectType;
                    result.ContactId = CheckPhone.ContactId;
                    result.ObjectId = CheckPhone.ObjectId;
                }
                else
                {
                    result.CheckPhone = false;
                }
                return result;
            }
            catch (Exception e)
            {
                return new CheckPhoneLeadResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình tìm kiếm",
                    Status = false
                };
            }
        }

        public GetPersonInChargeResult GetPersonInCharge(GetPersonInChargeParameter parameter)
        {
            try
            {
                //Tìm ActionResource của màn hình Chi tiết chăm sóc khách hàng(view)
                var actionResourceDetailLeadView = context.ActionResource.FirstOrDefault(e => e.ActionResource1.Contains("crm/lead/detail/view"));
                if (actionResourceDetailLeadView == null)
                {
                    return new GetPersonInChargeResult
                    {
                        ListPersonInCharge = new List<Models.Employee.EmployeeEntityModel>(),
                        Status = true,
                        Message = "Chưa có nhân viên nào được gán chức năng Quản lý khách hàng tiềm năng"
                    };
                }
                var actionResourceId = actionResourceDetailLeadView.ActionResourceId;

                //lấy list roleId theo actionResourceId
                var listRole = context.RoleAndPermission.Where(e => e.ActionResourceId == actionResourceId).ToList();
                if (listRole.Count == 0)
                {
                    return new GetPersonInChargeResult
                    {
                        ListPersonInCharge = new List<Models.Employee.EmployeeEntityModel>(),
                        Status = true,
                        Message = "Chưa có nhân viên nào được gán chức năng Quản lý khách hàng tiềm năng"
                    };
                }
                List<Guid> listRoleId = new List<Guid>();
                listRole.ForEach(item =>
                {
                    listRoleId.Add(item.RoleId.Value);
                });

                //Lấy list userId
                var listUserFromUserRole = context.UserRole.Where(e => listRoleId.Contains(e.RoleId.Value)).ToList();
                if (listUserFromUserRole.Count == 0)
                {
                    return new GetPersonInChargeResult
                    {
                        ListPersonInCharge = new List<Models.Employee.EmployeeEntityModel>(),
                        Status = true,
                        Message = "Chưa có nhân viên nào được gán chức năng Quản lý khách hàng tiềm năng"
                    };
                }
                List<Guid> listUserId = new List<Guid>();
                listUserFromUserRole.ForEach(item =>
                {
                    listUserId.Add(item.UserId.Value);
                });

                var newList = listUserId.Distinct();

                //Lấy list Employee theo list userId
                var listUser = context.User.Where(e => newList.Contains(e.UserId)).ToList();
                if (listUser.Count == 0)
                {
                    return new GetPersonInChargeResult
                    {
                        ListPersonInCharge = new List<Models.Employee.EmployeeEntityModel>(),
                        Status = true,
                        Message = "Chưa có nhân viên nào được gán chức năng Quản lý khách hàng tiềm năng"
                    };
                }

                List<Guid> listEmployeeId = new List<Guid>();
                listUser.ForEach(item =>
                {
                    listEmployeeId.Add(item.EmployeeId.Value);
                });

                var listPersonInCharge = context.Employee.Where(e => listEmployeeId.Contains(e.EmployeeId) && e.Active == true).ToList();
                List<Models.Employee.EmployeeEntityModel> list = new List<Models.Employee.EmployeeEntityModel>();
                listPersonInCharge.ForEach(item =>
                {
                    list.Add(new Models.Employee.EmployeeEntityModel(item));
                });

                return new GetPersonInChargeResult
                {
                    Status = true,
                    Message = "Lấy dữ liệu thành công",
                    ListPersonInCharge = list
                };
            }
            catch (Exception e)
            {
                return new GetPersonInChargeResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public EditPersonInChargeResult EditPersonInCharge(EditPersonInChargeParameter parameter)
        {
            try
            {
                var listLead = context.Lead.Where(e => parameter.ListLeadId.Contains(e.LeadId)).ToList();
                if (listLead.Count == 0)
                {
                    return new EditPersonInChargeResult
                    {
                        Status = false,
                        Message = "Danh sách KH tiềm năng không tồn tại"
                    };
                };
                listLead.ForEach(item =>
                {
                    item.PersonInChargeId = parameter.EmployeeId;
                    var nameLead = context.Contact.FirstOrDefault(ct => ct.ObjectId == item.PersonInChargeId && ct.ObjectType == "EMP");
                    Note note = new Note()
                    {
                        ObjectId = item.LeadId,
                        Type = "EDT",
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        ObjectType = "LEA",
                        NoteId = Guid.NewGuid(),
                        Description = "",
                        NoteTitle = "đã chỉnh sửa Người phụ trách thành <b>" + nameLead.FirstName + " " + nameLead.LastName + "</b>"
                    };
                    context.Note.Add(note);
                });

                context.Lead.UpdateRange(listLead);
                context.SaveChanges();

                return new EditPersonInChargeResult
                {
                    Status = true,
                    Message = "Cập nhật thành công"
                };
            }
            catch (Exception e)
            {
                return new EditPersonInChargeResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public EditLeadStatusByIdResult EditLeadStatusById(EditLeadStatusByIdParameter parameter)
        {
            try
            {
                var lead = context.Lead.Where(l => l.LeadId == parameter.LeadId).FirstOrDefault();
                var leadStatus = context.CategoryType.Where(ctt => ctt.CategoryTypeCode == "TLE").FirstOrDefault().CategoryTypeId;
                var statusId = context.Category.Where(ct => ct.CategoryTypeId == leadStatus
                                                         && ct.CategoryCode == parameter.LeadStatusCode).FirstOrDefault().CategoryId;//status từ parameter          
                if (parameter.LeadStatusCode == "NDO")
                {
                    lead.WaitingForApproval = true;
                }
                else
                {
                    lead.StatusId = statusId;
                }

                context.Lead.Update(lead);

                if (parameter.LeadStatusCode == "KHD")
                {
                    //thêm khách hàng mới nếu chuyển trạng thái Lead thành Ký hợp đồng
                    var statusType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "THA");
                    var groupType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA");
                    var StatusCustomer = context.Category.FirstOrDefault(f => f.CategoryTypeId == statusType.CategoryTypeId && f.CategoryCode == "MOI"); //trạng thái KH mới
                    var groupCusstomer = context.Category.FirstOrDefault(f => f.CategoryTypeId == groupType.CategoryTypeId && f.CategoryCode == "TPH"); //nhóm khách hàng thu phí
                    var leadContact = context.Contact.Where(c => c.ObjectId == lead.LeadId).FirstOrDefault();
                    Customer newCustomer = new Customer()
                    {
                        CustomerId = Guid.NewGuid(),
                        CustomerCode = GenerateCustomerCode(),
                        CustomerName = leadContact.FirstName + " " + leadContact.LastName,
                        CustomerGroupId = GetGroupCustomer(),
                        LeadId = lead.LeadId,
                        StatusId = StatusCustomer != null ? StatusCustomer.CategoryId : Guid.Empty,
                        CustomerServiceLevelId = null,
                        PersonInChargeId = null,
                        CustomerType = 2,
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
                        CreatedById = Guid.Parse(lead.CreatedById),
                        CreatedDate = DateTime.Now,
                        UpdatedById = Guid.Parse(lead.CreatedById),
                        UpdatedDate = DateTime.Now
                    };
                    context.Customer.Add(newCustomer);
                    Contact newContact = leadContact;
                    newContact.ContactId = Guid.NewGuid();
                    newContact.ObjectId = newCustomer.CustomerId;
                    newContact.ObjectType = "CUS";
                    context.Contact.Add(newContact);
                }

                context.SaveChanges();

                return new EditLeadStatusByIdResult
                {
                    Status = true,
                    LeadId = lead.LeadId
                };
            }
            catch (Exception e)
            {
                return new EditLeadStatusByIdResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private string GenerateCustomerCode()
        {
            var todayCustomer = context.Customer.Where(w => w.CreatedDate.Date == DateTime.Now.Date)
                                                .OrderByDescending(w => w.CreatedDate)
                                                .ToList();
            var count = todayCustomer.Count() == 0 ? 0 : todayCustomer.Count();
            string currentYear = DateTime.Now.Year.ToString();
            string result = "CTM" + currentYear.Substring(currentYear.Length - 2) + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + (count + 1).ToString("D4");
            return result;
        }

        private Guid GetGroupCustomer()
        {
            var defaultGroup = context.Category.Where(c => c.CategoryTypeId == context.CategoryType.Where(ct => ct.CategoryTypeCode == "NHA").FirstOrDefault().CategoryTypeId
                                                         && c.IsDefauld == true).FirstOrDefault();
            var TPHGroup = context.Category.Where(c => c.CategoryTypeId == context.CategoryType.Where(ct => ct.CategoryTypeCode == "NHA").FirstOrDefault().CategoryTypeId
                                                       && c.CategoryCode == "TPH").FirstOrDefault();
            var defaultGroupId = defaultGroup == null ? TPHGroup.CategoryId : defaultGroup.CategoryId;
            return defaultGroupId;
        }

        public GetDataCreateLeadResult GetDataCreateLead(GetDataCreateLeadParameter parameter)
        {
            try
            {
                var contact = context.Contact.Where(c => c.Active == true).ToList();
                var contactId = contact.Select(c => c.ObjectId).ToList();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listCustomerEntity = new List<LeadReferenceCustomerModel>();
                var customerLeadEntity = new List<LeadReferenceCustomerModel>();

                var categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                var category = context.Category.Where(x => x.CategoryTypeId == categoryType.CategoryTypeId).ToList();



                #region Get Address

                var listProvince = context.Province.Select(w => new Models.Address.ProvinceEntityModel
                {
                    ProvinceId = w.ProvinceId,
                    ProvinceName = w.ProvinceName,
                    ProvinceCode = w.ProvinceCode,
                    ProvinceType = w.ProvinceType,
                    Active = w.Active
                }).ToList();

                var listDistrict = context.District.Select(w => new Models.Address.DistrictEntityModel
                {
                    DistrictId = w.DistrictId,
                    ProvinceId = w.ProvinceId,
                    DistrictName = w.DistrictName,
                    DistrictCode = w.DistrictCode,
                    DistrictType = w.DistrictType,
                    Active = w.Active
                }).ToList();

                var listWard = context.Ward.Select(w => new Models.Address.WardEntityModel
                {
                    WardId = w.WardId,
                    DistrictId = w.DistrictId,
                    WardName = w.WardName,
                    WardCode = w.WardCode,
                    WardType = w.WardType,
                    Active = w.Active
                }).ToList();

                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName
                    }).ToList();
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
                    var listEmployee = context.Employee
                        .Where(x => x.Active == true &&
                                    (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)))
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
                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();
                    var listUserId = context.User.Where(c => c.Active == true && listEmployeeId.Contains(c.EmployeeId))
                        .Select(c => c.UserId).ToList();

                    #region Khách hàng từ Lead

                    customerLeadEntity = context.Customer.Where(w =>
                            w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                            contactId.Contains(w.CustomerId) &&
                            (w.CustomerId == parameter.CustomerId) &&
                            ((w.PersonInChargeId != null && listEmployeeId.Contains(w.PersonInChargeId))
                             || (w.PersonInChargeId == null && listUserId.Contains(w.CreatedById))))
                        .Select(w => new Models.Lead.LeadReferenceCustomerModel
                        {

                            CustomerId = w.CustomerId,
                            CustomerStatus = category.FirstOrDefault(x => x.CategoryId == w.StatusId).CategoryCode ?? "",
                            CustomerCode = w.CustomerCode,
                            CustomerName = w.CustomerName,
                            CustomerType = w.CustomerType,
                            Email = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                            Phone = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                            Address = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                            AddressWard = GetAddress(w.CustomerId, contact, listProvince, listDistrict, listWard),
                            PersonInChargeId = w.PersonInChargeId,
                        }).ToList();

                    #endregion

                    #region Danh sách khách hàng

                    listCustomerEntity = context.Customer.Where(w =>
                            w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                            contactId.Contains(w.CustomerId) &&
                            ((w.PersonInChargeId != null && listEmployeeId.Contains(w.PersonInChargeId))
                             || (w.PersonInChargeId == null && listUserId.Contains(w.CreatedById))))
                        .Select(w => new Models.Lead.LeadReferenceCustomerModel
                        {

                            CustomerId = w.CustomerId,
                            CustomerStatus = category.FirstOrDefault(x => x.CategoryId == w.StatusId).CategoryCode ?? "",
                            CustomerCode = w.CustomerCode,
                            CustomerName = w.CustomerName,
                            CustomerType = w.CustomerType,
                            Email = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                            Phone = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                            Address = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                            AddressWard = GetAddress(w.CustomerId, contact, listProvince, listDistrict, listWard),
                            PersonInChargeId = w.PersonInChargeId,
                        }).ToList();

                    #endregion
                }
                else
                {
                    #region Khách hàng từ Lead

                    if (parameter.CustomerId != null)
                    {
                        customerLeadEntity = context.Customer.Where(w =>
                                w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                                contactId.Contains(w.CustomerId) &&
                                (w.CustomerId == parameter.CustomerId) &&
                                ((w.PersonInChargeId.Value == employee.EmployeeId && w.PersonInChargeId != null)
                                 || (w.PersonInChargeId == null && w.CreatedById == user.UserId)))
                            .Select(w => new Models.Lead.LeadReferenceCustomerModel
                            {

                                CustomerId = w.CustomerId,
                                CustomerStatus = category.FirstOrDefault(x => x.CategoryId == w.StatusId).CategoryCode ?? "",
                                CustomerCode = w.CustomerCode,
                                CustomerName = w.CustomerName,
                                CustomerType = w.CustomerType,
                                Email = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                                Phone = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                                Address = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                                AddressWard = GetAddress(w.CustomerId, contact, listProvince, listDistrict, listWard),
                                PersonInChargeId = w.PersonInChargeId,
                            }).ToList();
                    }

                    #endregion

                    #region Danh sách khách hàng

                    listCustomerEntity = context.Customer.Where(w =>
                            w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                            contactId.Contains(w.CustomerId) &&
                            ((w.PersonInChargeId.Value == employee.EmployeeId && w.PersonInChargeId != null)
                             || (w.PersonInChargeId == null && w.CreatedById == user.UserId)))
                        .Select(w => new Models.Lead.LeadReferenceCustomerModel
                        {

                            CustomerId = w.CustomerId,
                            CustomerStatus = category.FirstOrDefault(x => x.CategoryId == w.StatusId).CategoryCode ?? "",
                            CustomerCode = w.CustomerCode,
                            CustomerName = w.CustomerName,
                            CustomerType = w.CustomerType,
                            Email = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                            Phone = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                            Address = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                            AddressWard = GetAddress(w.CustomerId, contact, listProvince, listDistrict, listWard),
                            PersonInChargeId = w.PersonInChargeId,
                        }).ToList();

                    #endregion
                }

                #region Lấy dữ liệu Category

                var GENDER_CODE = "GTI"; //gioi tinh
                var INTERESTED_CODE = "NHU"; //nhu cau san pham, dich vu
                var POTENTIAL_CODE = "MTN"; //muc do tiem nang
                var LEADTYPE_CODE = "LHL"; //loai khach hang tiem nang
                var LEADGROUP_CODE = "NHA"; //nhom khach hang
                var CARESTATE_CODE = "TTCS";

                //lay Customer voi customer type = 1||2 && active
                var BUSINESSTYPE_CODE = "BTL"; //loai hinh doan nghiep code   BTL
                var INVEST_CODE = "IVF";  //nguon tiem nang code IVF
                var PROB_CODE = "PROB"; //xac suat thang code PROB

                var genderTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == GENDER_CODE)
                    .CategoryTypeId;
                var interestedTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INTERESTED_CODE)
                    .CategoryTypeId;
                var potentialTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == POTENTIAL_CODE)
                    .CategoryTypeId;
                var leadTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEADTYPE_CODE)
                    .CategoryTypeId;
                var leadGroupTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEADGROUP_CODE)
                    .CategoryTypeId;
                var careStateTypeID = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == CARESTATE_CODE)?.CategoryTypeId;

                //// NEW
                var businessTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == BUSINESSTYPE_CODE)
                    .CategoryTypeId;
                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INVEST_CODE)
                    .CategoryTypeId;
                var probabilityTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == PROB_CODE)
                    .CategoryTypeId;

                var listGender = context.Category.Where(w => w.Active == true && w.CategoryTypeId == genderTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var listInterestedGroup = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == interestedTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                var listPotential = context.Category.Where(w => w.Active == true && w.CategoryTypeId == potentialTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var listLeadType = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var listLeadGroup = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadGroupTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                ///// NEW 
                var businessTypeList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == businessTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                var investFundList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                var probabilityList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == probabilityTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                var listCareState = context.Category.Where(c => c.Active == true && c.CategoryTypeId == careStateTypeID)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryName = m.CategoryName,
                        CategoryCode = m.CategoryCode,
                        IsDefault = m.IsDefauld
                    }).ToList();

                #endregion

                #region Get email and phone

                //lấy danh sách email và số điện thoại khách hàng khác trạng thái ngừng theo dõi và ký hợp đồng
                var leadStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE")
                    .CategoryTypeId;
                var leadStatusId = context.Category.Where(w => w.CategoryTypeId == leadStatusTypeId
                                                               && w.CategoryCode != "NDO" && w.CategoryCode != "KHD")
                    .Select(w => w.CategoryId).ToList();
                var listLeadId = context.Lead.Where(w => w.Active == true && leadStatusId.Contains(w.StatusId))
                    .Select(w => w.LeadId).ToList();
                var listEmailLead = new List<string>();
                var listPhoneLead = new List<string>();


                if (listLeadId != null)
                {
                    var contactLead = context.Contact.Where(w => listLeadId.Contains(w.ObjectId)).Select(w => new
                    {
                        w.Email,
                        w.Phone
                    }).ToList();

                    contactLead.ForEach(e =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Email) && !listEmailLead.Contains(e.Email))
                        {
                            listEmailLead.Add(e.Email.Trim());
                        }
                        if (!string.IsNullOrWhiteSpace(e.Phone) && !listEmailLead.Contains(e.Phone))
                        {
                            listPhoneLead.Add(e.Phone.Trim());
                        }
                    });
                }

                var listCustomerContact = context.Contact.Where(w => w.Active == true && w.ObjectType == "CUS")
                    .Select(w => new Models.Lead.CheckDuplicateLeadWithCustomerEntityModel
                    {
                        CustomerId = w.ObjectId,
                        CustomerFullName = w.FirstName + " " + w.LastName,
                        Email = w.Email,
                        Phone = w.Phone
                    }).ToList();

                #endregion

                #region Get Employee Care Staff List (Nhân viên chăm sóc)

                var _employeeId = context.User.FirstOrDefault(w => w.UserId == parameter.UserId).EmployeeId;
                var portalUserCode = "PortalUser"; //loại portalUser
                var listEmployeeEntity = context.Employee
                    .Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user
                var listPersonalInChange = new List<Models.Employee.EmployeeEntityModel>();

                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == _employeeId).FirstOrDefault();
                //check Is Manager
                var isManage = employeeById.IsManager;
                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employeeById.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                    _getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                    var listEmployeeFiltered = listEmployeeEntity
                        .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
                        {
                            EmployeeId = w.EmployeeId,
                            EmployeeName = w.EmployeeName,
                            EmployeeCode = w.EmployeeCode,
                            IsManager = w.IsManager,
                            PositonId = w.PositionId,
                            OrganizationId = w.OrganizationId,
                        }).ToList();

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        listPersonalInChange.Add(new Models.Employee.EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode,
                            IsManager = emp.IsManager,
                            PositionId = emp.PositonId,
                            OrganizationId = emp.OrganizationId
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == _employeeId).FirstOrDefault();
                    listPersonalInChange.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode,
                        IsManager = employeeId.IsManager,
                        PositionId = employeeId.PositionId,
                        OrganizationId = employeeId.OrganizationId
                    });
                }

                #endregion

                return new GetDataCreateLeadResult
                {
                    Status = true,
                    Message = "Success",
                    ListEmailLead = listEmailLead ?? new List<string>(),
                    ListPhoneLead = listPhoneLead ?? new List<string>(),
                    ListCustomerContact = listCustomerContact ??
                                          new List<CheckDuplicateLeadWithCustomerEntityModel>(),
                    ListGender = listGender?.OrderBy(w => w.CategoryName).ToList() ??
                                 new List<CategoryEntityModel>(),
                    ListInterestedGroup = listInterestedGroup?.OrderBy(w => w.CategoryName).ToList() ??
                                          new List<CategoryEntityModel>(),
                    ListPotential = listPotential?.OrderBy(w => w.CategoryName).ToList() ??
                                    new List<CategoryEntityModel>(),
                    ListLeadType = listLeadType?.OrderBy(w => w.CategoryName).ToList() ??
                                   new List<CategoryEntityModel>(),
                    ListLeadGroup = listLeadGroup?.OrderBy(w => w.CategoryName).ToList() ??
                                    new List<CategoryEntityModel>(),
                    ListPersonalInChange = listPersonalInChange?.OrderBy(w => w.EmployeeName).ToList() ??
                                           new List<EmployeeEntityModel>(),
                    // NEW 
                    ListBusinessType = businessTypeList?.OrderBy(w => w.CategoryName).ToList() ??
                                       new List<CategoryEntityModel>(),
                    ListInvestFund = investFundList?.OrderBy(w => w.CategoryName).ToList() ??
                                     new List<CategoryEntityModel>(),
                    ListProbability = probabilityList?.OrderBy(w => w.CategoryName).ToList() ??
                                      new List<CategoryEntityModel>(),
                    ListLeadReferenceCustomer = listCustomerEntity?.OrderBy(w => w.CustomerName).ToList() ??
                                                new List<LeadReferenceCustomerModel>(),
                    ListProvince = listProvince,
                    ListCareState = listCareState,
                    ListArea = listArea
                };
            }
            catch (Exception e)
            {
                return new GetDataCreateLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetListCustomerByTypeResult GetListCustomerByType(GetListCustomerByTypeParameter parameter)
        {
            try
            {
                var contact = context.Contact.Where(c => c.Active == true).ToList();
                var contactId = contact.Select(c => c.ObjectId).ToList();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listCustomerEntity = new List<LeadReferenceCustomerModel>();

                var categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                var category = context.Category.FirstOrDefault(x => x.CategoryTypeId == categoryType.CategoryTypeId &&
                                                                    x.CategoryCode == parameter.CustomerType);

                #region Get Address

                var listProvince = context.Province.Select(w => new Models.Address.ProvinceEntityModel
                {
                    ProvinceId = w.ProvinceId,
                    ProvinceName = w.ProvinceName,
                    ProvinceCode = w.ProvinceCode,
                    ProvinceType = w.ProvinceType,
                    Active = w.Active
                }).ToList();

                var listDistrict = context.District.Select(w => new Models.Address.DistrictEntityModel
                {
                    DistrictId = w.DistrictId,
                    ProvinceId = w.ProvinceId,
                    DistrictName = w.DistrictName,
                    DistrictCode = w.DistrictCode,
                    DistrictType = w.DistrictType,
                    Active = w.Active
                }).ToList();

                var listWard = context.Ward.Select(w => new Models.Address.WardEntityModel
                {
                    WardId = w.WardId,
                    DistrictId = w.DistrictId,
                    WardName = w.WardName,
                    WardCode = w.WardCode,
                    WardType = w.WardType,
                    Active = w.Active
                }).ToList();

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
                    var listEmployee = context.Employee
                        .Where(x => x.Active == true &&
                                    (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)))
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
                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();
                    var listUserId = context.User.Where(c => c.Active == true && listEmployeeId.Contains(c.EmployeeId))
                        .Select(c => c.UserId).ToList();



                    #region Danh sách khách hàng

                    listCustomerEntity = context.Customer.Where(w =>
                            w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                            contactId.Contains(w.CustomerId) &&
                            ((w.PersonInChargeId != null && listEmployeeId.Contains(w.PersonInChargeId))
                             || (w.PersonInChargeId == null && listUserId.Contains(w.CreatedById))) &&
                            (w.StatusId == category.CategoryId))
                        .Select(w => new Models.Lead.LeadReferenceCustomerModel
                        {

                            CustomerId = w.CustomerId,
                            CustomerStatus = category.CategoryCode,
                            CustomerCode = w.CustomerCode,
                            CustomerName = w.CustomerName,
                            CustomerType = w.CustomerType,
                            Email = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                            WorkEmail = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).WorkEmail,
                            Phone = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                            Address = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                            AddressWard = GetAddress(w.CustomerId, contact, listProvince, listDistrict, listWard),
                            PersonInChargeId = w.PersonInChargeId,
                            InvestmentFundId = w.InvestmentFundId,
                        }).ToList();

                    #endregion
                }
                else
                {
                    #region Danh sách khách hàng

                    listCustomerEntity = context.Customer.Where(w =>
                            w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                            contactId.Contains(w.CustomerId) &&
                            ((w.PersonInChargeId.Value == employee.EmployeeId && w.PersonInChargeId != null)
                             || (w.PersonInChargeId == null && w.CreatedById == user.UserId)))
                        .Select(w => new Models.Lead.LeadReferenceCustomerModel
                        {

                            CustomerId = w.CustomerId,
                            CustomerStatus = category.CategoryCode,
                            CustomerCode = w.CustomerCode,
                            CustomerName = w.CustomerName,
                            CustomerType = w.CustomerType,
                            Email = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                            WorkEmail = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).WorkEmail,
                            Phone = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                            Address = contact.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                            AddressWard = GetAddress(w.CustomerId, contact, listProvince, listDistrict, listWard),
                            PersonInChargeId = w.PersonInChargeId,
                            InvestmentFundId = w.InvestmentFundId,
                        }).ToList();

                    #endregion
                }

                #region Lấy dữ liệu Category

                var GENDER_CODE = "GTI"; //gioi tinh
                var INTERESTED_CODE = "NHU"; //nhu cau san pham, dich vu
                var POTENTIAL_CODE = "MTN"; //muc do tiem nang
                var LEADTYPE_CODE = "LHL"; //loai khach hang tiem nang
                var LEADGROUP_CODE = "NHA"; //nhom khach hang

                //lay Customer voi customer type = 1||2 && active
                var BUSINESSTYPE_CODE = "BTL"; //loai hinh doan nghiep code   BTL
                var INVEST_CODE = "IVF";  //nguon tiem nang code IVF
                var PROB_CODE = "PROB"; //xac suat thang code PROB

                var genderTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == GENDER_CODE)
                    .CategoryTypeId;
                var interestedTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INTERESTED_CODE)
                    .CategoryTypeId;
                var potentialTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == POTENTIAL_CODE)
                    .CategoryTypeId;
                var leadTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEADTYPE_CODE)
                    .CategoryTypeId;
                var leadGroupTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEADGROUP_CODE)
                    .CategoryTypeId;

                //// NEW
                var businessTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == BUSINESSTYPE_CODE)
                    .CategoryTypeId;
                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INVEST_CODE)
                    .CategoryTypeId;
                var probabilityTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == PROB_CODE)
                    .CategoryTypeId;

                var listGender = context.Category.Where(w => w.Active == true && w.CategoryTypeId == genderTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var listInterestedGroup = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == interestedTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                var listPotential = context.Category.Where(w => w.Active == true && w.CategoryTypeId == potentialTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var listLeadType = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var listLeadGroup = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadGroupTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                ///// NEW 
                var businessTypeList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == businessTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                var investFundList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                var probabilityList = context.Category
                    .Where(w => w.Active == true && w.CategoryTypeId == probabilityTypeId).Select(w =>
                        new Models.CategoryEntityModel
                        {
                            CategoryId = w.CategoryId,
                            CategoryName = w.CategoryName,
                            CategoryCode = w.CategoryCode,
                            IsDefault = w.IsDefauld
                        }).ToList();

                #endregion

                return new GetListCustomerByTypeResult
                {
                    Status = true,
                    Message = "Success",
                    ListCustomerByType = listCustomerEntity?.OrderBy(w => w.CustomerName).ToList() ??
                                                new List<LeadReferenceCustomerModel>()
                };
            }
            catch (Exception e)
            {
                return new GetListCustomerByTypeResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private string GetAddress(Guid customerId, List<Contact> contacts, List<Models.Address.ProvinceEntityModel> listProvince,
            List<Models.Address.DistrictEntityModel> listDistrict, List<Models.Address.WardEntityModel> listWard)
        {

            var contact = contacts.FirstOrDefault(c => c.ObjectId == customerId) ?? new Contact();
            var result = string.Empty;
            var war = listWard.FirstOrDefault(c => c.WardId == contact.WardId)?.WardName ?? "";
            var district = listDistrict.FirstOrDefault(c => c.DistrictId == contact.DistrictId)?.DistrictName ?? "";
            var province = listProvince.FirstOrDefault(c => c.ProvinceId == contact.ProvinceId)?.ProvinceName ?? "";
            result = war + " " + district + " " + province;

            return result;
        }

        public GetDataEditLeadResult GetDataEditLead(GetDataEditLeadParameter parameter)
        {
            try
            {
                var contacts = context.Contact.Where(c => c.Active == true).ToList();
                var contactIds = contacts.Select(c => c.ObjectId).ToList();
                var listCustomerEntity = new List<LeadReferenceCustomerModel>();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                var category = context.Category.Where(x => x.CategoryTypeId == categoryType.CategoryTypeId).ToList();

                #region Get Lead and Contact Lead

                var leadModelEntity = context.Lead.FirstOrDefault(f => f.LeadId == parameter.LeadId);
                Databases.Entities.Contact leadContactModelEntity = null;
                if (leadModelEntity != null)
                {
                    leadContactModelEntity = context.Contact.FirstOrDefault(f => f.ObjectId == leadModelEntity.LeadId && f.ObjectType == "LEA");
                }

                #endregion

                #region Get Lead Interested Group Mapping id 
                var listLeadInterestedGroupMapping = context.LeadInterestedGroupMapping.Where(w => w.LeadId == parameter.LeadId).ToList() ?? new List<LeadInterestedGroupMapping>();
                var listLeadInterestedGroupMappingId = listLeadInterestedGroupMapping.Select(w => w.InterestedGroupId).ToList() ?? new List<Guid>();
                #endregion

                #region Lấy dữ liệu Category

                var GENDER_CODE = "GTI";
                var INTERESTED_CODE = "NHU";
                var PAYMENT_CODE = "PTO";
                var POTENTIAL_CODE = "MTN";
                var LEADTYPE_CODE = "LHL";
                var LEAD_STATUS = "CHS";
                var LEADGROUP_CODE = "NHA";
                var MONEY_CODE = "DTI";
                var UNIT_CODE = "DNH";

                //lay Customer voi customer type = 1||2 && active
                var BUSINESSTYPE_CODE = "BTL"; //loai hinh doan nghiep code   BTL
                var INVEST_CODE = "IVF";  //nguon tiem nang code IVF
                var PROB_CODE = "PROB"; //xac suat thang code PROB

                var genderTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == GENDER_CODE).CategoryTypeId;
                var interestedTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INTERESTED_CODE).CategoryTypeId;
                var paymentTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == PAYMENT_CODE).CategoryTypeId;
                var potentialTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == POTENTIAL_CODE).CategoryTypeId;
                var leadTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEADTYPE_CODE).CategoryTypeId;
                var leadStatusType = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEAD_STATUS).CategoryTypeId;
                var leadGroupTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEADGROUP_CODE).CategoryTypeId;
                var leadMoneyTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == MONEY_CODE).CategoryTypeId;
                var unitTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == UNIT_CODE).CategoryTypeId;

                //// NEW
                var businessTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == BUSINESSTYPE_CODE).CategoryTypeId;
                var investFundTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INVEST_CODE).CategoryTypeId;
                var probabilityTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == PROB_CODE).CategoryTypeId;

                var listGender = context.Category.Where(w => w.Active == true && w.CategoryTypeId == genderTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listInterestedGroup = context.Category.Where(w => w.Active == true && w.CategoryTypeId == interestedTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listPaymentMethod = context.Category.Where(w => w.Active == true && w.CategoryTypeId == paymentTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listPotential = context.Category.Where(w => w.Active == true && w.CategoryTypeId == potentialTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listLeadType = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listLeadStatus = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadStatusType).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listLeadGroup = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadGroupTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                ///// NEW 
                var businessTypeList = context.Category.Where(w => w.Active == true && w.CategoryTypeId == businessTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var investFundList = context.Category.Where(w => w.Active == true && w.CategoryTypeId == investFundTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var probabilityList = context.Category.Where(w => w.Active == true && w.CategoryTypeId == probabilityTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var moneyList = context.Category.Where(w => w.Active == true && w.CategoryTypeId == leadMoneyTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var unitList = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                #endregion

                #region get list notes
                var listNote = context.Note.Where(w => w.Active == true && w.ObjectId == parameter.LeadId).Select(w => new Models.Note.NoteEntityModel
                {
                    NoteId = w.NoteId,
                    Description = w.Description,
                    Type = w.Type,
                    ObjectId = w.ObjectId,
                    ObjectType = w.ObjectType,
                    NoteTitle = w.NoteTitle,
                    CreatedById = w.CreatedById,
                    CreatedDate = w.CreatedDate,
                    UpdatedById = w.UpdatedById,
                    UpdatedDate = w.UpdatedDate,
                    NoteDocList = context.NoteDocument.Where(ws => ws.NoteId == w.NoteId && ws.Active == true).Select(s => new NoteDocumentEntityModel
                    {
                        NoteDocumentId = s.NoteDocumentId,
                        NoteId = s.NoteId,
                        DocumentName = s.DocumentName,
                        DocumentSize = s.DocumentSize,
                        DocumentUrl = s.DocumentUrl,
                    }).ToList() ?? new List<NoteDocumentEntityModel>()
                }).ToList();
                //lấy tên người tạo, người chỉnh sửa cho note
                listNote.ForEach(note =>
                {
                    var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                    var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                    if (contact != null)
                    {
                        note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                    }
                });
                #endregion

                #region Get email and phone (trừ lead hiện tại)
                //lấy danh sách email và số điện thoại khách hàng khác trạng thái ngừng theo dõi và ký hợp đồng
                var leadStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE").CategoryTypeId;
                var leadStatusId = context.Category.Where(w => w.CategoryTypeId == leadStatusTypeId
                                                            && w.CategoryCode != "NDO" && w.CategoryCode != "KHD").Select(w => w.CategoryId).ToList();
                var listLeadId = context.Lead.Where(w => w.Active == true && leadStatusId.Contains(w.StatusId) && w.LeadId != parameter.LeadId).Select(w => w.LeadId).ToList();
                var listEmailLead = new List<string>();
                var listPhoneLead = new List<string>();

                if (listLeadId != null)
                {
                    var contactLead = context.Contact.Where(w => listLeadId.Contains(w.ObjectId)).Select(w => new
                    {
                        w.Email,
                        w.Phone
                    }).ToList();

                    contactLead.ForEach(e =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Email) && !listEmailLead.Contains(e.Email))
                        {
                            listEmailLead.Add(e.Email.Trim());
                        }
                        if (!string.IsNullOrWhiteSpace(e.Phone) && !listEmailLead.Contains(e.Phone))
                        {
                            listPhoneLead.Add(e.Phone.Trim());
                        }
                    });
                }

                var listCustomerContact = context.Contact.Where(w => w.Active == true && w.ObjectType == "CUS")
                                                                      .Select(w => new Models.Lead.CheckDuplicateLeadWithCustomerEntityModel
                                                                      {
                                                                          CustomerId = w.ObjectId,
                                                                          CustomerFullName = w.FirstName + " " + w.LastName,
                                                                          Email = w.Email,
                                                                          Phone = w.Phone
                                                                      }).ToList();
                #endregion

                #region Get Address

                var listProvince = context.Province.Select(w => new Models.Address.ProvinceEntityModel
                {
                    ProvinceId = w.ProvinceId,
                    ProvinceName = w.ProvinceName,
                    ProvinceCode = w.ProvinceCode,
                    ProvinceType = w.ProvinceType,
                    Active = w.Active
                }).ToList();

                var listDistrict = context.District.Select(w => new Models.Address.DistrictEntityModel
                {
                    DistrictId = w.DistrictId,
                    ProvinceId = w.ProvinceId,
                    DistrictName = w.DistrictName,
                    DistrictCode = w.DistrictCode,
                    DistrictType = w.DistrictType,
                    Active = w.Active
                }).ToList();

                var listWard = context.Ward.Select(w => new Models.Address.WardEntityModel
                {
                    WardId = w.WardId,
                    DistrictId = w.DistrictId,
                    WardName = w.WardName,
                    WardCode = w.WardCode,
                    WardType = w.WardType,
                    Active = w.Active
                }).ToList();

                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName,
                        Active = m.Active
                    }).ToList();

                #endregion

                var listEmployeeEntityModel = context.Employee.Where(c => c.Active == true)
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

                var customerOfLead = context.Customer.FirstOrDefault(c => c.CustomerId == leadModelEntity.CustomerId);
                var customerOfLeadEntityModel = new LeadReferenceCustomerModel();
                if (customerOfLead != null)
                {
                    customerOfLeadEntityModel = new LeadReferenceCustomerModel
                    {
                        CustomerId = customerOfLead.CustomerId,
                        CustomerStatus = category.FirstOrDefault(x => x.CategoryId == customerOfLead.StatusId)?.CategoryCode ?? "",
                        CustomerCode = customerOfLead.CustomerCode,
                        CustomerName = customerOfLead.CustomerName,
                        CustomerType = customerOfLead.CustomerType,
                        Email = contacts.FirstOrDefault(c => c.ObjectId == customerOfLead.CustomerId)?.Email ?? "",
                        Phone = contacts.FirstOrDefault(c => c.ObjectId == customerOfLead.CustomerId)?.Phone ?? "",
                        Address = contacts.FirstOrDefault(c => c.ObjectId == customerOfLead.CustomerId)?.Address ?? "",
                        AddressWard = GetAddress(customerOfLead.CustomerId, contacts, listProvince, listDistrict, listWard),
                        PersonInChargeId = customerOfLead.PersonInChargeId,
                    };
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
                    var listEmployee = listEmployeeEntityModel
                        .Where(x => (listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)))
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
                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();
                    var listUserId = context.User.Where(c => c.Active == true && listEmployeeId.Contains(c.EmployeeId))
                        .Select(c => c.UserId).ToList();

                    #region Danh sách khách hàng

                    listCustomerEntity = context.Customer.Where(w =>
                            w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                            contactIds.Contains(w.CustomerId) &&
                            ((w.PersonInChargeId != null && listEmployeeId.Contains(w.PersonInChargeId))
                             || (w.PersonInChargeId == null && listUserId.Contains(w.CreatedById))))
                        .Select(w => new LeadReferenceCustomerModel
                        {

                            CustomerId = w.CustomerId,
                            CustomerStatus = category.FirstOrDefault(x => x.CategoryId == w.StatusId).CategoryCode ?? "",
                            CustomerCode = w.CustomerCode,
                            CustomerName = w.CustomerName,
                            CustomerType = w.CustomerType,
                            Email = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                            WorkEmail = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).WorkEmail,
                            Phone = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                            Address = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                            AddressWard = GetAddress(w.CustomerId, contacts, listProvince, listDistrict, listWard),
                            PersonInChargeId = w.PersonInChargeId,
                        }).ToList();

                    #endregion
                }
                else
                {
                    #region Danh sách khách hàng

                    listCustomerEntity = context.Customer.Where(w =>
                            w.Active == true && (w.CustomerType == 1 || w.CustomerType == 2) &&
                            contactIds.Contains(w.CustomerId) &&
                            ((w.PersonInChargeId.Value == employee.EmployeeId && w.PersonInChargeId != null)
                             || (w.PersonInChargeId == null && w.CreatedById == user.UserId)))
                        .Select(w => new LeadReferenceCustomerModel
                        {
                            CustomerId = w.CustomerId,
                            CustomerStatus = category.FirstOrDefault(x => x.CategoryId == w.StatusId).CategoryCode ?? "",
                            CustomerCode = w.CustomerCode,
                            CustomerName = w.CustomerName,
                            CustomerType = w.CustomerType,
                            Email = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).Email,
                            WorkEmail = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).WorkEmail,
                            Phone = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).Phone,
                            Address = contacts.FirstOrDefault(c => c.ObjectId == w.CustomerId).Address ?? "",
                            AddressWard = GetAddress(w.CustomerId, contacts, listProvince, listDistrict, listWard),
                            PersonInChargeId = w.PersonInChargeId,
                        }).ToList();

                    #endregion
                }

                //Kiểm tra xem khách hàng của Cơ hội hiện tại có còn thuộc quyền phụ trách của người đang đăng nhập không?
                var existsCustomer =
                    listCustomerEntity.FirstOrDefault(x => x.CustomerId == customerOfLeadEntityModel.CustomerId);

                //Nếu khách hàng không còn thuộc quyền phụ trách của người đang đăng nhập thì thêm vào để không bị lỗi
                if (existsCustomer == null)
                {
                    if (customerOfLead != null)
                    {
                        listCustomerEntity.Add(customerOfLeadEntityModel);
                    }
                }

                #region Lấy danh sách người phụ trách
                var listEmployeeEntity = context.Employee.Where(w => w.Active == true).ToList();
                var listPersonalInChange = new List<EmployeeEntityModel>();

                //lấy người phụ trách của cơ hội
                var personalInChange =
                    listEmployeeEntity.FirstOrDefault(x => x.EmployeeId == leadModelEntity?.PersonInChargeId);
                if (personalInChange != null)
                {
                    listPersonalInChange.Add(new EmployeeEntityModel
                    {
                        EmployeeId = personalInChange.EmployeeId,
                        EmployeeName = personalInChange.EmployeeName,
                        EmployeeCode = personalInChange.EmployeeCode,
                        EmployeeCodeName = personalInChange.EmployeeCode.Trim() + " - " +
                                           personalInChange.EmployeeName.Trim()
                    });
                }

                // nếu cơ hội không có khách hàng thì lấy theo phân quyền dữ liệu của người đang đăng nhập
                if (customerOfLead == null)
                {

                    var employeeByPersonInCharge =
                        listEmployeeEntity.FirstOrDefault(w => w.EmployeeId == employee.EmployeeId);

                    //check Is Manager
                    if (employeeByPersonInCharge?.IsManager == true)
                    {
                        /*
                         * Lấy list phòng ban con của user
                         * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                         */
                        List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employeeByPersonInCharge.OrganizationId
                    };
                        listGetAllChild = getOrganizationChildrenId(employeeByPersonInCharge.OrganizationId, listGetAllChild);

                        listPersonalInChange = listEmployeeEntity
                            .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                                y => new EmployeeEntityModel
                                {
                                    EmployeeId = y.EmployeeId,
                                    EmployeeCode = y.EmployeeCode,
                                    EmployeeName = y.EmployeeName,
                                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                }).OrderBy(z => z.EmployeeName).ToList();
                    }
                    else if (employeeByPersonInCharge?.IsManager == false)
                    {
                        //Nhân viên: chỉ lấy nhân viên đó
                        listPersonalInChange.Add(new EmployeeEntityModel
                        {
                            EmployeeId = employeeByPersonInCharge.EmployeeId,
                            EmployeeName = employeeByPersonInCharge.EmployeeName,
                            EmployeeCode = employeeByPersonInCharge.EmployeeCode,
                            EmployeeCodeName = employeeByPersonInCharge.EmployeeCode.Trim() + " - " +
                                               employeeByPersonInCharge.EmployeeName.Trim()
                        });
                    }
                }
                // nếu cơ hội có khách hàng thì lấy theo phân quyền dữ liệu của người phụ trách của khách hàng
                else
                {
                    var employeeByPersonInCharge =
                        listEmployeeEntity.FirstOrDefault(w => w.EmployeeId == customerOfLead?.PersonInChargeId);

                    //check Is Manager
                    if (employeeByPersonInCharge?.IsManager == true)
                    {
                        /*
                         * Lấy list phòng ban con của user
                         * List phòng ban: chính nó và các phòng ban cấp dưới của nó
                         */
                        List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        employeeByPersonInCharge.OrganizationId
                    };
                        listGetAllChild = getOrganizationChildrenId(employeeByPersonInCharge.OrganizationId, listGetAllChild);

                        listPersonalInChange = listEmployeeEntity
                            .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                                y => new EmployeeEntityModel
                                {
                                    EmployeeId = y.EmployeeId,
                                    EmployeeCode = y.EmployeeCode,
                                    EmployeeName = y.EmployeeName,
                                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                }).OrderBy(z => z.EmployeeName).ToList();
                    }
                    else if (employeeByPersonInCharge?.IsManager == false)
                    {
                        //Nhân viên: chỉ lấy nhân viên đó
                        listPersonalInChange.Add(new EmployeeEntityModel
                        {
                            EmployeeId = employeeByPersonInCharge.EmployeeId,
                            EmployeeName = employeeByPersonInCharge.EmployeeName,
                            EmployeeCode = employeeByPersonInCharge.EmployeeCode,
                            EmployeeCodeName = employeeByPersonInCharge.EmployeeCode.Trim() + " - " +
                                               employeeByPersonInCharge.EmployeeName.Trim()
                        });
                    }


                    //Kiểm tra xem người phụ trách của Cơ hội hiện tại có còn phụ trách Khách hàng không?
                    var existsPerson =
                        listPersonalInChange.FirstOrDefault(x => x.EmployeeId == customerOfLead.PersonInChargeId);

                    //Nếu không còn phụ trách khách hàng thì thêm vào list người phụ trách để không bị lỗi
                    if (existsPerson == null)
                    {
                        var empl_extend =
                            listEmployeeEntity.Where(x => x.EmployeeId == customerOfLead?.PersonInChargeId).Select(y =>
                                new EmployeeEntityModel
                                {
                                    EmployeeId = y.EmployeeId,
                                    EmployeeCode = y.EmployeeCode,
                                    EmployeeName = y.EmployeeName,
                                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                }).FirstOrDefault();

                        if (empl_extend != null)
                        {
                            listPersonalInChange.Add(empl_extend);
                        }
                    }

                }

                #endregion

                #region Lấy chi tiết lead và thuộc tính
                var listDetailLead = new List<LeadDetailModel>();
                var listDetailLeadEntity = context.LeadDetail.Where(w => w.LeadId == parameter.LeadId).ToList() ?? new List<LeadDetail>();

                var listDetailId = listDetailLeadEntity.Select(w => w.LeadDetailId).ToList() ?? new List<Guid>();
                var listVendorId = listDetailLeadEntity.Select(w => w.VendorId).ToList() ?? new List<Guid?>();
                var listProductId = listDetailLeadEntity.Select(w => w.ProductId).ToList() ?? new List<Guid?>();

                var listVendorEntity = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList() ?? new List<Vendor>();
                var listProductEntity = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList() ?? new List<Product>();

                var leadProductDetailProductAttributeValueEntity = context.LeadProductDetailProductAttributeValue.Where(w => listDetailId.Contains(w.LeadDetailId.Value)).ToList();

                listDetailLeadEntity?.ForEach(leadDetail =>
                {
                    //product attributes
                    //var attribute = new LeadProductDetailProductAttributeValueModel();
                    var listAttribute = new List<LeadProductDetailProductAttributeValueModel>();
                    var listAttributeEntity = leadProductDetailProductAttributeValueEntity.Where(w => w.LeadDetailId == leadDetail.LeadDetailId).ToList() ?? new List<LeadProductDetailProductAttributeValue>();
                    listAttributeEntity?.ForEach(attri =>
                    {
                        listAttribute.Add(new LeadProductDetailProductAttributeValueModel
                        {
                            LeadProductDetailProductAttributeValue1 = attri.LeadProductDetailProductAttributeValue1,
                            LeadDetailId = attri.LeadDetailId,
                            ProductId = attri.ProductId,
                            ProductAttributeCategoryId = attri.ProductAttributeCategoryId,
                            ProductAttributeCategoryValueId = attri.ProductAttributeCategoryValueId
                        });
                    });

                    var nameMoneyUnit = moneyList.FirstOrDefault(f => f.CategoryId == leadDetail.CurrencyUnit)?.CategoryName ?? "";
                    var nameVendor = listVendorEntity.FirstOrDefault(f => f.VendorId == leadDetail.VendorId)?.VendorName ?? "";
                    var productCode = listProductEntity.FirstOrDefault(f => f.ProductId == leadDetail.ProductId)?.ProductName ?? "";
                    var unitName = unitList.FirstOrDefault(f => f.CategoryId == leadDetail.UnitId)?.CategoryName ?? "";

                    listDetailLead.Add(new LeadDetailModel
                    {
                        LeadDetailId = leadDetail.LeadDetailId,
                        LeadId = leadDetail.LeadDetailId,
                        VendorId = leadDetail.VendorId,
                        ProductId = leadDetail.ProductId,
                        Quantity = leadDetail.Quantity,
                        UnitPrice = leadDetail.UnitPrice,
                        CurrencyUnit = leadDetail.CurrencyUnit,
                        ExchangeRate = leadDetail.ExchangeRate,
                        Vat = leadDetail.Vat,
                        DiscountType = leadDetail.DiscountType,
                        DiscountValue = leadDetail.DiscountValue,
                        Description = leadDetail.Description,
                        OrderDetailType = leadDetail.OrderDetailType,
                        UnitId = leadDetail.UnitId,
                        IncurredUnit = leadDetail.IncurredUnit,
                        ProductName = leadDetail.ProductName,
                        LeadProductDetailProductAttributeValue = listAttribute,
                        //label
                        NameMoneyUnit = nameMoneyUnit,
                        NameVendor = nameVendor,
                        ProductCode = productCode,
                        ProductNameUnit = unitName,
                        SumAmount = SumAmount(leadDetail.Quantity, leadDetail.UnitPrice, leadDetail.ExchangeRate, leadDetail.Vat, leadDetail.DiscountValue,
                                                    leadDetail.DiscountType),
                        OrderNumber = leadDetail.OrderNumber,
                    });
                });

                #endregion

                #region Lấy danh sách liên hệ
                var listContactLeadEntity = context.Contact.Where(w => w.ObjectId == parameter.LeadId && w.ObjectType == "LEA_CON").ToList() ?? new List<Contact>();
                #endregion

                #region Kiểm tra điều kiện xóa
                //var MOI_Status_Id = context.Category.FirstOrDefault(f => f.CategoryTypeId == leadStatusType && f.CategoryCode == "MOI").CategoryId;

                //var listQuote = context.Quote.Where(w => w.Active == true && w.ObjectType == "LEAD" && w.ObjectTypeId == parameter.LeadId).Select(w => w.ObjectTypeId).ToList();

                //var listCustomerFromLead = context.Customer.Where(w => w.Active == true && w.LeadId == parameter.LeadId).Select(w => w.LeadId).ToList(); //danh sách khách hàng được tạo từ lead

                //var canDeleteLead = this.CheckDeleteLeadCondition(leadModelEntity, listQuote, listCustomerFromLead, MOI_Status_Id);
                var canDeleteLead = true;
                //TODO: sua lai logic dieu kien xoa
                #endregion

                #region Kiểm tra điều kiện hồ sơ thầu
                var canCreateSaleBidding = true;
                var saleBidding = context.SaleBidding.FirstOrDefault(f => f.LeadId == parameter.LeadId);
                var listSaleBidding = context.SaleBidding.Where(c => c.LeadId == parameter.LeadId).ToList();
                //if (saleBidding != null)
                //{
                //    canCreateSaleBidding = false;
                //}
                #endregion

                #region Lấy danh sách báo giá được tạo từ cơ hội
                var listQuoteByLeadResult = new List<DataAccess.Models.Quote.QuoteEntityModel>();

                var quoteStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TGI").CategoryTypeId;
                var listQuoteStatus = context.Category.Where(w => w.Active == true && w.CategoryTypeId == quoteStatusTypeId).ToList() ?? new List<Category>();
                var listQuoteByLeadEntity = context.Quote.Where(w => w.Active == true && w.LeadId == parameter.LeadId).ToList() ?? new List<Quote>();
                listQuoteByLeadEntity.ForEach(quote =>
                {
                    var statusName = listQuoteStatus.FirstOrDefault(f => f.CategoryId == quote.StatusId)?.CategoryName ?? "";

                    DateTime? expirationDate = null;
                    if (quote.QuoteDate != null && quote.EffectiveQuoteDate != null) expirationDate = quote.QuoteDate.Value.AddDays(quote.EffectiveQuoteDate.Value);

                    listQuoteByLeadResult.Add(new QuoteEntityModel
                    {
                        LeadId = quote.LeadId,
                        QuoteId = quote.QuoteId,
                        QuoteCode = quote.QuoteCode,
                        QuoteDate = quote.QuoteDate,
                        EffectiveQuoteDate = quote.EffectiveQuoteDate,
                        ExpirationDate = expirationDate,
                        Amount = quote.Amount,
                        StatusId = quote.StatusId,
                        QuoteStatusName = statusName,
                        Note = quote.Note
                    });
                });

                #endregion

                #region Lấy danh sách đơn hàng có báo giá đc tạo từ cơ hội

                var listAllStatus = context.OrderStatus.ToList();
                var listQuoteId = listQuoteByLeadResult.Select(x => x.QuoteId).ToList();
                var lstOrder = context.CustomerOrder.Where(x => x.QuoteId != null && x.Active == true && listQuoteId.Contains((Guid)x.QuoteId))
                    .Select(m => new CustomerOrderEntityModel
                    {
                        OrderId = m.OrderId,
                        OrderCode = m.OrderCode,
                        OrderDate = m.OrderDate,
                        Seller = m.Seller,
                        SellerName = listEmployeeEntityModel.FirstOrDefault(e => e.EmployeeId == m.Seller) == null
                            ? ""
                            : listEmployeeEntityModel.FirstOrDefault(e => e.EmployeeId == m.Seller).EmployeeName,
                        CustomerId = m.CustomerId,
                        CustomerName = m.CustomerName,
                        CustomerContactId = Guid.Empty,
                        Amount = (decimal)((m.DiscountType == true)
                            ? (m.Amount * (1 - (m.DiscountValue / 100)))
                            : (m.Amount - m.DiscountValue)),
                        StatusId = m.StatusId,
                        StatusCode =
                            listAllStatus.FirstOrDefault(f => f.OrderStatusId == m.StatusId).OrderStatusCode ?? "",
                        OrderStatusName = listAllStatus.FirstOrDefault(s => s.OrderStatusId == m.StatusId)
                            .Description,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        UpdatedById = m.UpdatedById,
                        UpdatedDate = m.UpdatedDate,
                        Active = m.Active,
                        QuoteId = m.QuoteId,
                        OrderContractId = m.OrderContractId
                    }).ToList();


                #endregion

                #region Lấy danh sách hồ sơ thầu được tạo từ cơ hội
                var listCustomer = context.Customer.ToList() ?? new List<Customer>();
                //var listEmployeeEntity = context.Employee.ToList() ?? new List<Employee>();

                var listSaleBiddingResult = new List<DataAccess.Models.SaleBidding.SaleBiddingEntityModel>();

                var saleBiddingStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "HST").CategoryTypeId;
                var listSaleBiddingStatus = context.Category.Where(w => w.Active == true && w.CategoryTypeId == quoteStatusTypeId).ToList() ?? new List<Category>();

                var listSaleBiddingEntity = context.SaleBidding.Where(w => w.LeadId == parameter.LeadId && w.Active == true).ToList() ?? new List<SaleBidding>();
                listSaleBiddingEntity.ForEach(item =>
                {
                    var customerName = listCustomer.FirstOrDefault(f => f.CustomerId == item.CustomerId)?.CustomerName ?? "";
                    var persionInChargeName = listEmployeeEntity.FirstOrDefault(f => f.EmployeeId == item.EmployeeId)?.EmployeeName ?? "";

                    listSaleBiddingResult.Add(new Models.SaleBidding.SaleBiddingEntityModel
                    {
                        SaleBiddingId = item.SaleBiddingId,
                        SaleBiddingName = item.SaleBiddingName,
                        CustomerName = customerName,
                        ValueBid = item.ValueBid,
                        PersonInChargeName = persionInChargeName
                    });
                });
                #endregion

                #region Lấy thông tin File đính kèm
                var listFileDocument =
                    context.FileInFolder.Where(w => w.Active == true &&
                                                    w.ObjectId == parameter.LeadId &&
                                                    w.ObjectType == "QLCH").ToList() ?? new List<FileInFolder>();

                var listUser = context.User.ToList();

                var listFile = new List<DataAccess.Models.Folder.FileInFolderEntityModel>();
                listFileDocument.ForEach(file =>
                {
                    //convert name of file
                    List<string> fullnameArr = file.FileName?.Split('_')?.ToList() ?? new List<string>();
                    if (fullnameArr.Any()) //prevent IndexOutOfRangeException for empty list
                    {
                        fullnameArr.RemoveAt(fullnameArr.Count - 1);
                    }
                    string fullName = string.Join("", fullnameArr);
                    var fileExtension = file.FileExtension;
                    var createByUserName = listUser.FirstOrDefault(f => f.UserId == file.CreatedById)?.UserName ?? "";

                    var newItem = new DataAccess.Models.Folder.FileInFolderEntityModel();
                    newItem.FileInFolderId = file.FileInFolderId;
                    newItem.FolderId = file.FolderId;
                    newItem.FileName = fullName + "." + fileExtension;
                    newItem.ObjectId = file.ObjectId;
                    newItem.ObjectType = file.ObjectType;
                    newItem.Size = file.Size;
                    newItem.Active = file.Active;
                    newItem.FileExtension = file.FileExtension;
                    newItem.CreatedById = file.CreatedById;
                    newItem.CreatedDate = file.CreatedDate;
                    newItem.UpdatedById = file.UpdatedById;
                    newItem.UpdatedDate = file.UpdatedDate;
                    newItem.CreatedByName = createByUserName;

                    listFile.Add(newItem);
                });
                #endregion

                #region Get list link dinh kem`
                var ListLinkOfDocument = new List<DataAccess.Models.Document.LinkOfDocumentEntityModel>();
                var listLinkOfDocEntity =
                    context.LinkOfDocument.Where(w => w.ObjectId == parameter.LeadId &&
                                                      w.Active == true &&
                                                      w.ObjectType == "LEAD").ToList();
                listLinkOfDocEntity?.ForEach(item =>
                {
                    ListLinkOfDocument.Add(new Models.Document.LinkOfDocumentEntityModel
                    {
                        LinkOfDocumentId = item.LinkOfDocumentId,
                        LinkName = item.LinkName,
                        LinkValue = item.LinkValue,
                        CreatedByName = listUser.FirstOrDefault(f => f.UserId == item.CreatedById)?.UserName ?? "",
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate
                    });
                });
                #endregion

                #region Trạng thái phụ

                var StatusSupportId = leadModelEntity.StatusSuportId;

                var statusSupportType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTPCH")
                    ?.CategoryTypeId;
                var ListStatusSupport = context.Category.Where(x => x.CategoryTypeId == statusSupportType).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        })
                    .OrderBy(z => z.CategoryCode).ToList();

                #endregion

                var listQuoter = context.Quote.Where(c => c.LeadId == leadModelEntity.LeadId).ToList() ?? new List<Quote>();

                return new GetDataEditLeadResult
                {
                    Status = true,
                    LeadModel = leadModelEntity ?? new Lead(),
                    LeadContactModel = leadContactModelEntity ?? new Contact(),
                    ListLeadInterestedGroupMappingId = listLeadInterestedGroupMappingId,
                    ListEmailLead = listEmailLead ?? new List<string>(),
                    ListPhoneLead = listPhoneLead ?? new List<string>(),
                    ListCustomerContact = listCustomerContact ?? new List<CheckDuplicateLeadWithCustomerEntityModel>(),
                    ListGender = listGender?.OrderBy(w => w.CategoryName).ToList() ?? new List<CategoryEntityModel>(),
                    ListInterestedGroup = listInterestedGroup?.OrderBy(w => w.CategoryName).ToList() ??
                                          new List<CategoryEntityModel>(),
                    ListPaymentMethod = listPaymentMethod?.OrderBy(w => w.CategoryName).ToList() ??
                                        new List<CategoryEntityModel>(),
                    ListPotential = listPotential?.OrderBy(w => w.CategoryName).ToList() ??
                                    new List<CategoryEntityModel>(),
                    ListLeadType = listLeadType?.OrderBy(w => w.CategoryName).ToList() ??
                                   new List<CategoryEntityModel>(),
                    ListLeadGroup = listLeadGroup?.OrderBy(w => w.CategoryName).ToList() ??
                                    new List<CategoryEntityModel>(),
                    ListLeadStatus = listLeadStatus?.OrderBy(w => w.CategoryName).ToList() ??
                                     new List<CategoryEntityModel>(),
                    ListPersonalInChange = listPersonalInChange?.OrderBy(w => w.EmployeeName).ToList() ??
                                           new List<EmployeeEntityModel>(),
                    ListNote = listNote?.OrderByDescending(w => w.CreatedDate).ToList() ?? new List<NoteEntityModel>(),
                    // NEW 
                    ListBusinessType = businessTypeList?.OrderBy(w => w.CategoryName).ToList() ??
                                       new List<CategoryEntityModel>(),
                    ListInvestFund = investFundList?.OrderBy(w => w.CategoryName).ToList() ??
                                     new List<CategoryEntityModel>(),
                    ListProbability = probabilityList?.OrderBy(w => w.CategoryName).ToList() ??
                                      new List<CategoryEntityModel>(),
                    ListLinkOfDocument = ListLinkOfDocument ?? new List<Models.Document.LinkOfDocumentEntityModel>(),
                    ListFile = listFile,
                    ListEmployee = listEmployeeEntityModel,
                    ListLeadDetail = listDetailLead ?? new List<LeadDetailModel>(),
                    ListLeadContact = listContactLeadEntity,
                    CanDelete = canDeleteLead,
                    CanCreateSaleBidding = canCreateSaleBidding,
                    StatusSaleBiddingAndQuote = GetStatusTypeConnect(leadModelEntity.LeadId, listSaleBidding, listQuoter),
                    ListLeadReferenceCustomer = listCustomerEntity?.OrderBy(w => w.CustomerName).ToList() ?? new List<LeadReferenceCustomerModel>(),
                    ListOrder = lstOrder,
                    ListQuoteById = listQuoteByLeadResult,
                    ListSaleBiddingById = listSaleBiddingResult,
                    ListProvince = listProvince,
                    ListArea = listArea,
                    ListStatusSupport = ListStatusSupport,
                    StatusSupportId = StatusSupportId
                };
            }
            catch (Exception e)
            {
                return new GetDataEditLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
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

        private List<Guid?> _getOrganizationChildrenId(List<Organization> organizationList, Guid? id, List<Guid?> list)
        {
            var organizations = organizationList.Where(o => o.ParentId == id).ToList();
            organizations.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                _getOrganizationChildrenId(organizationList, item.OrganizationId, list);
            });

            return list;
        }

        public GetDataSearchLeadResult GetDataSearchLead(GetDataSearchLeadParameter parameter)
        {
            try
            {
                #region Get list potential, list status, list interested_group, list lead_type
                var POTENTIAL_CODE = "MTN";
                //var LEAD_STATUS = "TLE";
                var LEAD_STATUS = "CHS";
                var INTERESTED_CODE = "NHU";
                var LEADTYPE_CODE = "LHL";

                var potentialTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == POTENTIAL_CODE).CategoryTypeId;
                var listPotential = context.Category.Where(w => w.CategoryTypeId == potentialTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var leadStatusType = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEAD_STATUS).CategoryTypeId;
                var listLeadStatus = context.Category.Where(w => w.CategoryTypeId == leadStatusType).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var interestedTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INTERESTED_CODE).CategoryTypeId;
                var listInterestedGroup = context.Category.Where(w => w.CategoryTypeId == interestedTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var leadTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == LEADTYPE_CODE).CategoryTypeId;
                var listLeadType = context.Category.Where(w => w.CategoryTypeId == leadTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                // Nhóm khách hàng
                var cusGroupTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NHA")?.CategoryTypeId;
                var listCusGroup = context.Category.Where(c => c.CategoryTypeId == cusGroupTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                // Khu vực
                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName
                    }).ToList();

                // Nguồn tiềm năng
                var ivfId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "IVF")?.CategoryTypeId;
                var listSource = context.Category.Where(c => c.CategoryTypeId == ivfId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();
                #endregion

                #region Get List Personal In Change
                var _employeeId = context.User.FirstOrDefault(w => w.UserId == parameter.UserId).EmployeeId;
                var portalUserCode = "PortalUser"; //loại portalUser
                var listEmployeeEntity = context.Employee.Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user
                var listPersonalInChange = new List<Models.Employee.EmployeeEntityModel>();

                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == _employeeId).FirstOrDefault();
                //check Is Manager
                var isManage = employeeById.IsManager;
                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employeeById.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                    _getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                    var listEmployeeFiltered = listEmployeeEntity.Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
                    {
                        EmployeeId = w.EmployeeId,
                        EmployeeName = w.EmployeeName,
                        EmployeeCode = w.EmployeeCode,
                    }).ToList();

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        listPersonalInChange.Add(new Models.Employee.EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == _employeeId).FirstOrDefault();
                    listPersonalInChange.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode
                    });
                }
                #endregion

                return new GetDataSearchLeadResult
                {
                    ListPotential = listPotential?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListStatus = listLeadStatus?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListInterestedGroup = listInterestedGroup?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListLeadType = listLeadType?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListPersonalInchange = listPersonalInChange?.OrderBy(w => w.EmployeeName).ToList() ?? new List<Models.Employee.EmployeeEntityModel>(),
                    ListCusGroup = listCusGroup,
                    ListArea = listArea,
                    ListSource = listSource,
                    Status = true,
                };
            }
            catch (Exception e)
            {
                return new GetDataSearchLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public ApproveOrRejectLeadUnfollowResult ApproveOrRejectLeadUnfollow(ApproveOrRejectLeadUnfollowParameter parameter)
        {
            try
            {
                #region Kiểm tra điều kiện phê duyệt
                //điều kiện phê duyêt: là QUẢN LÝ của phòng ban đó và những phòng ban cấp cao hơn trong hệ thống
                var currentEmp = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;
                var currentOrganization = context.Employee.FirstOrDefault(f => f.EmployeeId == currentEmp).OrganizationId;
                //lấy danh sách phòng ban hiện tại và phòng ban cấp cao hơn nó
                var listOrganization = context.Organization.Where(w => w.Active == true).ToList();
                var currentTreeId = new List<Guid>();
                var listTreeOrganization = GetTreeOrganization(currentOrganization.Value, currentTreeId, listOrganization);
                //danh sách quản lý của phòng ban hiện tại và phòng ban cao hơn
                var listManagerByTreeOrganization = context.Employee.Where(w => w.Active == true && listTreeOrganization.Contains(w.OrganizationId.Value) && w.IsManager == true).Select(w => w.EmployeeId).ToList();
                //danh sách account quản lý
                var listUserManager = context.User.Where(w => w.Active == true && listManagerByTreeOrganization.Contains(w.EmployeeId.Value)).Select(w => w.UserId).ToList();
                if (!listUserManager.Contains(parameter.UserId))
                {
                    return new ApproveOrRejectLeadUnfollowResult
                    {
                        Status = false,
                        Message = "Bạn không có quyền phê duyệt/từ chối"
                    };
                }
                #endregion
                var leadStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE").CategoryTypeId;
                var unfollowId = context.Category.FirstOrDefault(f => f.CategoryTypeId == leadStatusTypeId && f.CategoryCode == "NDO").CategoryId;

                var listLead = context.Lead.Where(w => parameter.LeadIdList.Contains(w.LeadId)).ToList();
                if (listLead != null)
                {
                    if (parameter.IsApprove)
                    {
                        //phê duyệt
                        listLead.ForEach(lead =>
                        {
                            lead.StatusId = unfollowId;
                            lead.WaitingForApproval = false;
                            lead.UpdatedDate = DateTime.Now;
                        });
                    }
                    else
                    {
                        listLead.ForEach(lead =>
                        {
                            lead.WaitingForApproval = false;
                            lead.UpdatedDate = DateTime.Now;
                        });
                    }

                    context.Lead.UpdateRange(listLead);
                    context.SaveChanges();
                }

                return new ApproveOrRejectLeadUnfollowResult
                {
                    Status = true,
                };
            }
            catch (Exception e)
            {
                return new ApproveOrRejectLeadUnfollowResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private List<Guid> GetTreeOrganization(Guid currentOrgId, List<Guid> currentList, List<Organization> listOrganization)
        {
            var currentOrg = listOrganization.FirstOrDefault(f => f.OrganizationId == currentOrgId);

            currentList.Add(currentOrg.OrganizationId);

            if (currentOrg.ParentId != null)
            {
                GetTreeOrganization(currentOrg.ParentId.Value, currentList, listOrganization);
            }
            return currentList;
        }

        public SetPersonalInChangeResult SetPersonalInChange(SetPersonalInChangeParameter parameter)
        {
            try
            {
                var listPersonalInChange = new List<EmployeeEntityModel>();

                if (parameter.CustomerId != null && parameter.CustomerId != Guid.Empty)
                {
                    var personInChargeId = context.Customer.FirstOrDefault(w => w.CustomerId == parameter.CustomerId)
                        .PersonInChargeId;
                    var listEmployeeEntity = context.Employee
                        .Where(w => w.Active == true).ToList();

                    var employee = listEmployeeEntity.Where(w => w.EmployeeId == personInChargeId).FirstOrDefault();
                    //check Is Manager
                    if (employee.IsManager == true)
                    {
                        //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                        List<Guid?> listGetAllChild = new List<Guid?>
                        {
                            employee.OrganizationId
                        };
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId, listGetAllChild);

                        listPersonalInChange = listEmployeeEntity
                            .Where(x => listGetAllChild.Contains(x.OrganizationId)).Select(
                                y => new EmployeeEntityModel
                                {
                                    EmployeeId = y.EmployeeId,
                                    EmployeeCode = y.EmployeeCode,
                                    EmployeeName = y.EmployeeName,
                                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim(),
                                }).OrderBy(z => z.EmployeeName).ToList();
                    }
                    else
                    {
                        //Nhân viên: chỉ lấy nhân viên đó
                        listPersonalInChange.Add(new EmployeeEntityModel
                        {
                            EmployeeId = employee.EmployeeId,
                            EmployeeName = employee.EmployeeName,
                            EmployeeCode = employee.EmployeeCode,
                            EmployeeCodeName = employee.EmployeeCode.Trim() + " - " + employee.EmployeeName.Trim()
                        });
                    }
                }

                return new SetPersonalInChangeResult
                {
                    ListPersonalInChange = listPersonalInChange?.OrderBy(w => w.EmployeeName).ToList() ??
                                           new List<EmployeeEntityModel>(),
                    Status = true,
                };
            }
            catch (Exception e)
            {
                return new SetPersonalInChangeResult
                {
                    ListPersonalInChange = new List<Models.Employee.EmployeeEntityModel>(),
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public UnfollowListLeadResult UnfollowListLead(UnfollowListLeadParamerter paramerter)
        {
            try
            {
                var listLead = context.Lead.Where(w => paramerter.ListLeadId.Contains(w.LeadId)).ToList();

                if (listLead != null)
                {
                    listLead.ForEach(lead =>
                    {
                        lead.WaitingForApproval = true;
                    });
                    context.UpdateRange(listLead);
                    context.SaveChanges();
                }

                return new UnfollowListLeadResult
                {
                    Status = true,
                };
            }
            catch (Exception e)
            {
                return new UnfollowListLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public ImportLeadDetailResult ImportLeadDetail(ImportLeadDetailParameter parameter)
        {
            try
            {
                #region Get Address
                var listProvince = context.Province.Select(w => new Models.Address.ProvinceEntityModel
                {
                    ProvinceId = w.ProvinceId,
                    ProvinceName = w.ProvinceName.Trim(),
                    ProvinceCode = w.ProvinceCode.Trim(),
                    ProvinceType = w.ProvinceType.Trim(),
                    Active = w.Active
                }).ToList();

                var listDistrict = context.District.Select(w => new Models.Address.DistrictEntityModel
                {
                    DistrictId = w.DistrictId,
                    ProvinceId = w.ProvinceId,
                    DistrictName = w.DistrictName.Trim(),
                    DistrictCode = w.DistrictCode.Trim(),
                    DistrictType = w.DistrictType.Trim(),
                    Active = w.Active
                }).ToList();

                var listWard = context.Ward.Select(w => new Models.Address.WardEntityModel
                {
                    WardId = w.WardId,
                    DistrictId = w.DistrictId,
                    WardName = w.WardName.Trim(),
                    WardCode = w.WardCode.Trim(),
                    WardType = w.WardType.Trim(),
                    Active = w.Active
                }).ToList();
                #endregion

                #region Lấy dữ liệu Category
                var GENDER_CODE = "GTI";
                var INTERESTED_CODE = "NHU";
                var PAYMENT_CODE = "PTO";
                var POTENTIAL_CODE = "MTN";

                var genderTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == GENDER_CODE).CategoryTypeId;
                var interestedTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == INTERESTED_CODE).CategoryTypeId;
                var paymentTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == PAYMENT_CODE).CategoryTypeId;
                var potentialTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == POTENTIAL_CODE).CategoryTypeId;

                var listGender = context.Category.Where(w => w.CategoryTypeId == genderTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listInterestedGroup = context.Category.Where(w => w.CategoryTypeId == interestedTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listPaymentMethod = context.Category.Where(w => w.CategoryTypeId == paymentTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var listPotential = context.Category.Where(w => w.CategoryTypeId == potentialTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();
                #endregion

                #region Lấy email, số điện thoại Lead/Customer
                var listContact = context.Contact.Where(w => w.Active == true &&
                                                            (w.ObjectType == "LEA" || w.ObjectType == "CUS")
                                                            ).ToList();
                var listEmailLead = new List<string>();
                var listPhoneLead = new List<string>();
                var listEmailCustomer = new List<string>();
                var listPhoneCustomer = new List<string>();

                listContact.ForEach(contact =>
                {
                    switch (contact.ObjectType)
                    {
                        case "LEA":
                            var leadTLE = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TLE");
                            var leadExeption = new List<Guid>();
                            if (leadTLE != null)
                            {
                                var leadNHDDK = context.Category.Where(ct => ct.CategoryTypeId == leadTLE.CategoryTypeId && (ct.CategoryCode == "NDO" || ct.CategoryCode == "KHD")).Select(ct => ct.CategoryId).ToList();
                                leadExeption = context.Lead.Where(l => leadNHDDK.Contains(l.StatusId)).Select(l => l.LeadId).ToList();
                            }

                            if (!string.IsNullOrWhiteSpace(contact.Email) && !listEmailLead.Contains(contact.Email) && !leadExeption.Contains(contact.ObjectId))
                            {
                                listEmailLead.Add(contact.Email.Trim());
                            }
                            if (!string.IsNullOrWhiteSpace(contact.Phone) && !listPhoneLead.Contains(contact.Phone) && !leadExeption.Contains(contact.ObjectId))
                            {
                                listPhoneLead.Add(contact.Phone.Trim());
                            }
                            break;
                        case "CUS":
                            if (!string.IsNullOrWhiteSpace(contact.Email) && !listEmailCustomer.Contains(contact.Email))
                            {
                                listEmailCustomer.Add(contact.Email.Trim());
                            }
                            if (!string.IsNullOrWhiteSpace(contact.Phone) && !listPhoneCustomer.Contains(contact.Phone))
                            {
                                listPhoneCustomer.Add(contact.Phone.Trim());
                            }
                            break;
                        default:
                            // code block
                            break;
                    }
                });
                #endregion

                return new ImportLeadDetailResult
                {
                    ListProvince = listProvince?.OrderBy(w => w.ProvinceName).ToList() ?? new List<Models.Address.ProvinceEntityModel>(),
                    ListDistrict = listDistrict?.OrderBy(w => w.DistrictName).ToList() ?? new List<Models.Address.DistrictEntityModel>(),
                    ListWard = listWard?.OrderBy(w => w.WardName).ToList() ?? new List<Models.Address.WardEntityModel>(),
                    ListGender = listGender?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListInterestedGroup = listInterestedGroup?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListPaymentMethod = listPaymentMethod?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListPotential = listPotential?.OrderBy(w => w.CategoryName).ToList() ?? new List<Models.CategoryEntityModel>(),
                    ListEmailLead = listEmailLead,
                    ListPhoneLead = listPhoneLead,
                    ListEmailCustomer = listEmailCustomer,
                    ListPhoneCustomer = listPhoneCustomer,
                    Status = true,
                };
            }
            catch (Exception e)
            {
                return new ImportLeadDetailResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public ImportListLeadResult ImportListLead(ImportListLeadParameter parameter)
        {
            try
            {
                var statusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE").CategoryTypeId;
                var leadNewStatus = context.Category.FirstOrDefault(f => f.CategoryTypeId == statusTypeId && f.CategoryCode == "MOI");
                var potentialId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "MTN").CategoryTypeId;
                var potentialList = context.Category.Where(w => w.CategoryTypeId == potentialId).ToList();

                var listLead = new List<Lead>();
                var listLeadContact = new List<Contact>();
                var listCompany = new List<Company>();
                var listNote = new List<Note>();

                parameter.ListImportLead.ForEach(lead =>
                {
                    var splitFullName = lead.FullName.Split(' ').ToList();
                    var firstName = splitFullName.FirstOrDefault() ?? "";
                    var lastNameList = splitFullName.Where(w => w != firstName).ToList();
                    var lastName = String.Join(' ', lastNameList);

                    var newLeadId = Guid.NewGuid();

                    var newLead = new Lead();
                    newLead.LeadId = newLeadId;
                    newLead.RequirementDetail = null;
                    newLead.PotentialId = lead.PotentialId;
                    newLead.InterestedGroupId = lead.InterestedGroupId;
                    newLead.PersonInChargeId = null;
                    newLead.StatusId = leadNewStatus.CategoryId;
                    newLead.PaymentMethodId = lead.PaymentMethodId;
                    newLead.CreatedById = parameter.UserId.ToString();
                    newLead.CreatedDate = DateTime.Now;
                    newLead.UpdatedById = null;
                    newLead.UpdatedDate = null;
                    newLead.Active = true;

                    if (!string.IsNullOrWhiteSpace(lead.CompanyName))
                    {
                        //tạo công ty
                        var newCompanyId = Guid.NewGuid();

                        var newCompany = new Company();
                        newCompany.CompanyId = newCompanyId;
                        newCompany.CompanyName = lead.CompanyName.Trim();
                        newCompany.Active = true;
                        newCompany.CreatedById = parameter.UserId;
                        newCompany.CreatedDate = DateTime.Now;
                        newCompany.UpdatedById = null;
                        newLead.UpdatedDate = null;
                        newCompany.Active = true;

                        listCompany.Add(newCompany);
                        newLead.CompanyId = newCompanyId;
                    }

                    listLead.Add(newLead);

                    var newLeadContact = new Contact();
                    newLeadContact.ContactId = Guid.NewGuid();
                    newLeadContact.ObjectId = newLeadId;
                    newLeadContact.ObjectType = "LEA";
                    newLeadContact.FirstName = firstName;
                    newLeadContact.LastName = lastName;
                    newLeadContact.Gender = lead.Gender;
                    newLeadContact.Phone = lead.Phone;
                    newLeadContact.Email = lead.Email;
                    newLeadContact.Address = lead.Address;
                    newLeadContact.ProvinceId = lead.ProvinceId;
                    newLeadContact.DistrictId = lead.DistrictId;
                    newLeadContact.WardId = lead.WardId;
                    newLeadContact.CreatedById = parameter.UserId;
                    newLeadContact.CreatedDate = DateTime.Now;
                    newLeadContact.UpdatedById = null;
                    newLeadContact.UpdatedDate = null;
                    newLeadContact.Active = true;

                    listLeadContact.Add(newLeadContact);

                    //thêm ghi chú cho từng lead
                    if (lead.PotentialId != null)
                    {
                        var newNote = new Note();
                        newNote.NoteId = Guid.NewGuid();
                        var potentialName = potentialList.FirstOrDefault(f => f.CategoryId == lead.PotentialId).CategoryName;
                        newNote.Description = "Mức độ tiềm năng - <b>" + potentialName + "</b>, trạng thái - <b>" + leadNewStatus.CategoryName + "</b>, chưa có người phụ trách";

                        newNote.Type = "ADD";
                        newNote.ObjectId = newLeadId;
                        newNote.ObjectType = "LEA";
                        newNote.NoteTitle = "đã thêm ghi chú";
                        newNote.CreatedById = parameter.UserId;
                        newNote.CreatedDate = DateTime.Now;
                        newNote.UpdatedById = null;
                        newNote.UpdatedDate = null;
                        newNote.Active = true;

                        listNote.Add(newNote);
                    }
                });

                context.Lead.AddRange(listLead);
                context.Contact.AddRange(listLeadContact);
                context.Company.AddRange(listCompany);
                context.Note.AddRange(listNote);
                context.SaveChanges();

                return new ImportListLeadResult
                {

                    Status = true,
                };

            }
            catch (Exception e)
            {
                return new ImportListLeadResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDataLeadProductDialogResult GetDataLeadProductDialog(GetDataLeadProductDialogParameter parameter)
        {
            try
            {
                var categoryType =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI" && x.Active == true);
                var listUnitMoney = context.Category
                    .Where(x => x.CategoryTypeId == categoryType.CategoryTypeId && x.Active == true).Select(
                        y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName,
                            IsDefault = y.IsDefauld
                        }).ToList();

                var categoryTypeUnitProduct =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH" && x.Active == true);
                var listUintProduct = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeUnitProduct.CategoryTypeId && x.Active == true).Select(
                        y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        }).ToList();

                var date = DateTime.Now.Date.Add(new TimeSpan(23, 59, 59));
                var listPriceProduct =
                    context.PriceProduct.Where(x => x.Active == true && x.EffectiveDate <= date).ToList() ??
                    new List<PriceProduct>();

                var listProduct = context.Product.Where(x => x.Active == true).ToList();

                var listVendor = context.Vendor.Where(x => x.Active == true).Select(y => new VendorEntityModel
                {
                    VendorId = y.VendorId,
                    VendorCode = y.VendorCode,
                    VendorName = y.VendorName
                }).ToList();

                return new GetDataLeadProductDialogResult()
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
                return new GetDataLeadProductDialogResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SendEmailSupportLeadResult SendEmailSupportLead(SendEmailSupportLeadParameter parameter)
        {
            try
            {
                #region Tạo chương trình chăm sóc khách hàng
                var leadCare = new LeadCare();
                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Email" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                int currentYear = DateTime.Now.Year % 100;
                int currentMonth = DateTime.Now.Month;
                int currentDate = DateTime.Now.Day;
                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                parameter.ListQueue.ForEach(item =>
                {
                    var lead = context.Lead.FirstOrDefault(f => f.LeadId == item.CustomerId);
                    var leadId = lead?.LeadId;
                    var leadContact = context.Contact.FirstOrDefault(f => f.ObjectId == leadId && f.ObjectType == "LEA");

                    leadCare.LeadCareId = Guid.NewGuid();
                    leadCare.LeadCareCode = string.Format("CSCH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                    leadCare.NumberCode = MaxNumberCode + 1;
                    leadCare.YearCode = currentYear;
                    leadCare.MonthCode = currentMonth;
                    leadCare.DateCode = currentDate;
                    leadCare.EmployeeCharge = employeeId;
                    leadCare.EffecttiveFromDate = DateTime.Now;
                    leadCare.EffecttiveToDate = DateTime.Now;
                    leadCare.LeadCareContactType = customerCareContactType;
                    leadCare.LeadCareTitle = item.Title;
                    leadCare.LeadCareContent = "";
                    leadCare.LeadCareContentEmail = replaceTokenForContent(item.SendContent, item.CustomerId.Value);
                    leadCare.IsSendEmailNow = true;
                    leadCare.LeadCareType = 1;
                    leadCare.StatusId = customerCareStatusId;
                    leadCare.CreateDate = DateTime.Now;
                    leadCare.CreateById = parameter.UserId;

                    context.LeadCare.Add(leadCare);

                    #region Tạo cơ hội của chương trình chăm sóc cơ hội
                    ////Tạo khách hàng của chương trình CSKH
                    var leadCareLead = new LeadCareLead
                    {
                        LeadCareLeadId = Guid.NewGuid(),
                        LeadCareId = leadCare.LeadCareId,
                        LeadId = item.CustomerId,
                        StatusId = statusId,
                        CreateDate = DateTime.Now,
                        CreateById = parameter.UserId,
                    };
                    context.LeadCareLead.Add(leadCareLead);
                    #endregion

                    #region Tạo bộ lọc
                    var leadCareFilter = new LeadCareFilter
                    {
                        LeadCareFilterId = Guid.NewGuid(),
                        LeadCareId = leadCare.LeadCareId,
                        QueryContent = "",
                        CreateDate = DateTime.Now,
                        CreateById = parameter.UserId
                    };
                    context.LeadCareFilter.Add(leadCareFilter);
                    #endregion

                    #region Gửi email
                    item.QueueId = Guid.NewGuid();
                    item.Title = item.Title;
                    //parameter.Queue.SendContent =
                    //replaceTokenForContent(parameter.Queue.SendContent, parameter.Queue.CustomerId.Value);
                    item.SendContent = replaceTokenForContent(item.SendContent, item.CustomerId.Value);
                    item.CustomerCareId = leadCare.LeadCareId;
                    item.SenDate = DateTime.Now;
                    item.CreateDate = DateTime.Now;
                    context.Queue.Add(item);
                    #endregion
                });
                #endregion

                context.SaveChanges();

                return new SendEmailSupportLeadResult
                {
                    //QueueId = parameter.Queue.QueueId,
                    Message = "Gửi email thành công",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new SendEmailSupportLeadResult
                {
                    Message = "Đã có lỗi khi gửi email",
                    Status = false
                };
            }
        }

        public SendSMSSupportLeadResult SendSMSSupportLead(SendSMSSupportLeadParameter parameter)
        {
            try
            {
                #region Tạo chương trình chăm sóc cơ hội

                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "SMS" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                int currentYear = DateTime.Now.Year % 100;
                int currentMonth = DateTime.Now.Month;
                int currentDate = DateTime.Now.Day;
                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                parameter.ListQueue.ForEach(item =>
                {
                    var leadCare = new LeadCare();
                    leadCare.LeadCareId = Guid.NewGuid();
                    leadCare.LeadCareCode = string.Format("CSCH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                    leadCare.NumberCode = MaxNumberCode + 1;
                    leadCare.YearCode = currentYear;
                    leadCare.MonthCode = currentMonth;
                    leadCare.DateCode = currentDate;
                    leadCare.EmployeeCharge = employeeId;
                    leadCare.EffecttiveFromDate = DateTime.Now;
                    leadCare.EffecttiveToDate = DateTime.Now;
                    leadCare.LeadCareContactType = customerCareContactType;
                    leadCare.LeadCareTitle = item.Title;
                    leadCare.LeadCareContent = "";
                    leadCare.LeadCareContentSms = replaceTokenForContent(item.SendContent, item.CustomerId.Value);
                    leadCare.IsSendNow = true;
                    leadCare.LeadCareType = 1;
                    leadCare.StatusId = customerCareStatusId;
                    leadCare.CreateDate = DateTime.Now;
                    leadCare.CreateById = parameter.UserId;
                    context.LeadCare.Add(leadCare);

                    #region Tạo cơ hội của chương trình chăm sóc cơ hội
                    var leadCareLead = new LeadCareLead
                    {
                        LeadCareLeadId = Guid.NewGuid(),
                        LeadCareId = leadCare.LeadCareId,
                        LeadId = item.CustomerId,
                        StatusId = statusId,
                        CreateDate = DateTime.Now,
                        CreateById = parameter.UserId,
                    };
                    context.LeadCareLead.Add(leadCareLead);
                    #endregion

                    #region Tạo bộ lọc
                    var leadCareFilter = new LeadCareFilter
                    {
                        LeadCareFilterId = Guid.NewGuid(),
                        LeadCareId = leadCare.LeadCareId,
                        QueryContent = "",
                        CreateDate = DateTime.Now,
                        CreateById = parameter.UserId,
                    };
                    context.LeadCareFilter.Add(leadCareFilter);
                    #endregion

                    #region Save queue
                    item.QueueId = Guid.NewGuid();
                    item.SendContent = replaceTokenForContent(item.SendContent, item.CustomerId.Value);
                    item.CustomerCareId = leadCare.LeadCareId;
                    item.SenDate = DateTime.Now;
                    item.CreateDate = DateTime.Now;
                    context.Queue.Add(item);
                    #endregion
                });
                #endregion

                context.SaveChanges();

                return new SendSMSSupportLeadResult
                {
                    //QueueId = parameter.Queue.QueueId,
                    Message = "Gửi SMS thành công",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new SendSMSSupportLeadResult
                {
                    Message = "Đã có lỗi khi gửi SMS",
                    Status = false
                };
            }
        }

        public SendGiftSupportLeadResult SendGiftSupportLead(SendGiftSupportLeadParameter parameter)
        {
            try
            {
                #region Tạo chương trình chăm sóc khách hàng
                var leadCare = new LeadCare();
                leadCare.LeadCareId = Guid.NewGuid();

                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TKH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "DSO" && ca.CategoryTypeId == categoryTypeId).CategoryId;
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var customerCareStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCS").CategoryTypeId;
                var customerCareStatusId = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Closed" && ca.CategoryTypeId == customerCareStatusTypeId).CategoryId;
                var customerCareContactTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HCS").CategoryTypeId;
                var customerCareContactType = context.Category.FirstOrDefault(ca => ca.CategoryCode == "Gift" && ca.CategoryTypeId == customerCareContactTypeId).CategoryId;

                int currentYear = DateTime.Now.Year % 100;
                int currentMonth = DateTime.Now.Month;
                int currentDate = DateTime.Now.Day;
                var lstRequestPayment = context.CustomerCare.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                leadCare.LeadCareCode = string.Format("CSCH{0}{1}{2}", currentYear, currentMonth, (MaxNumberCode + 1).ToString("D3"));
                leadCare.NumberCode = MaxNumberCode + 1;
                leadCare.YearCode = currentYear;
                leadCare.MonthCode = currentMonth;
                leadCare.DateCode = currentDate;
                leadCare.EmployeeCharge = employeeId;
                leadCare.EffecttiveFromDate = DateTime.Now;
                leadCare.EffecttiveToDate = DateTime.Now;
                leadCare.LeadCareContactType = customerCareContactType;
                leadCare.LeadCareTitle = parameter.Title;
                leadCare.LeadCareContent = "";

                leadCare.GiftLeadType1 = parameter.GiftCustomerType1;
                leadCare.GiftTypeId1 = parameter.GiftTypeId1;
                leadCare.GiftTotal1 = parameter.GiftTotal1;

                leadCare.GiftLeadType2 = parameter.GiftCustomerType2;
                leadCare.GiftTypeId2 = parameter.GiftTypeId2;
                leadCare.GiftTotal2 = parameter.GiftTotal2;

                leadCare.LeadCareType = 1;
                leadCare.StatusId = customerCareStatusId;
                leadCare.ActiveDate = DateTime.Now;
                leadCare.CreateDate = DateTime.Now;
                leadCare.CreateById = parameter.UserId;
                context.LeadCare.Add(leadCare);
                #endregion

                #region Tạo cơ hội của chương trình chăm sóc cơ hội
                var leadCareLead = new LeadCareLead
                {
                    LeadCareLeadId = Guid.NewGuid(),
                    LeadCareId = leadCare.LeadCareId,
                    LeadId = parameter.CustomerId,
                    StatusId = statusId,
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId,
                };
                context.LeadCareLead.Add(leadCareLead);
                #endregion

                #region Tạo bộ lọc
                var leadCareFilter = new LeadCareFilter
                {
                    LeadCareFilterId = Guid.NewGuid(),
                    LeadCareId = leadCare.LeadCareId,
                    QueryContent = "",
                    CreateDate = DateTime.Now,
                    CreateById = parameter.UserId
                };
                context.LeadCareFilter.Add(leadCareFilter);
                #endregion

                context.SaveChanges();

                return new SendGiftSupportLeadResult
                {
                    Message = "Gửi SMS thành công",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new SendGiftSupportLeadResult
                {
                    Message = "Đã có lỗi khi gửi SMS",
                    Status = false
                };
            }
        }

        public CreateLeadMeetingResult CreateLeadMeeting(CreateLeadMeetingParameter parameter)
        {
            try
            {
                GetConfiguration();
                var listEmployeeId = new List<string>();
                var listEmployeeContact = new List<Contact>();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employeeContact =
                    context.Contact.FirstOrDefault(x => x.ObjectId == user.EmployeeId && x.ObjectType == "EMP");
                if (parameter.LeadMeeting.Participant != null && parameter.LeadMeeting.Participant != "")
                {
                    listEmployeeId = parameter.LeadMeeting.Participant.Split(';').ToList();
                    listEmployeeContact = context.Contact.Where(c => listEmployeeId.Contains(c.ObjectId.ToString()) && c.ObjectType == "EMP").ToList();
                }
                if (parameter.LeadMeeting.LeadMeetingId != null)
                {
                    var leadMeeting = context.LeadMeeting.FirstOrDefault(x =>
                        x.LeadMeetingId == parameter.LeadMeeting.LeadMeetingId);

                    leadMeeting.Title = parameter.LeadMeeting.Title;
                    leadMeeting.LocationMeeting = parameter.LeadMeeting.LocationMeeting;
                    leadMeeting.StartDate = SetDate2(parameter.LeadMeeting.StartDate, parameter.LeadMeeting.StartHours.Value.TimeOfDay);
                    leadMeeting.StartHours = parameter.LeadMeeting.StartHours.Value.TimeOfDay;
                    leadMeeting.Content = parameter.LeadMeeting.Content;
                    leadMeeting.UpdatedById = parameter.UserId;
                    leadMeeting.UpdatedDate = DateTime.Now;
                    if (parameter.LeadMeeting.EndDate != null && parameter.LeadMeeting.EndHours != null)
                    {
                        leadMeeting.EndDate = SetDate2(parameter.LeadMeeting.EndDate, parameter.LeadMeeting.EndHours.Value.TimeOfDay);
                        leadMeeting.EndHours = parameter.LeadMeeting.EndHours.Value.TimeOfDay;
                    }

                    if (parameter.LeadMeeting.EndHours != null && parameter.LeadMeeting.EndDate == null)
                    {
                        leadMeeting.EndDate = SetDate2(parameter.LeadMeeting.StartDate, parameter.LeadMeeting.EndHours.Value.TimeOfDay);
                        leadMeeting.EndHours = parameter.LeadMeeting.EndHours.Value.TimeOfDay;
                    }
                    leadMeeting.Participant = parameter.LeadMeeting.Participant;
                    context.LeadMeeting.Update(leadMeeting);

                    //Xóa trong bảng Queue
                    var queue_meeting = context.Queue.Where(x =>
                        x.CustomerMeetingId == parameter.LeadMeeting.LeadMeetingId).ToList();
                    context.Queue.RemoveRange(queue_meeting);

                    var sendDate = DateTime.Now;

                    var timeNow = DateTime.Now;
                    timeNow = timeNow.AddMinutes(60);
                    var compareTime = DateTime.Compare(timeNow, leadMeeting.StartDate.Value);
                    if (compareTime <= 0)
                    {
                        //Nếu thời điểm dự kiến gửi email trước thời điểm hẹn thì tính thời điểm gửi
                        //Ngược lại sẽ gửi luôn
                        sendDate = leadMeeting.StartDate.Value.AddMinutes(-60);
                    }

                    #region Gửi email tới người tạo lịch hẹn
                    var emp_queue_meeting = new Queue();
                    emp_queue_meeting.QueueId = Guid.NewGuid();
                    //emp_queue_meeting.SendTo = employeeContact.Email.Trim();
                    //emp_queue_meeting.SendContent = "Địa điểm: " + leadMeeting.LocationMeeting +
                    //                                ". Nội dung: " + leadMeeting.Content;
                    //emp_queue_meeting.Title = leadMeeting.Title + " - " +
                    //                          leadMeeting.StartDate.Value.ToString("dd/MM/yyyy HH:mm");
                    //emp_queue_meeting.Method = "Email";
                    //emp_queue_meeting.IsSend = false;
                    //emp_queue_meeting.SenDate = sendDate;
                    //emp_queue_meeting.FromTo = Email;
                    //emp_queue_meeting.CustomerId = leadMeeting.LeadId;
                    //emp_queue_meeting.CustomerMeetingId = leadMeeting.LeadMeetingId;
                    //emp_queue_meeting.EmployeeId = user.EmployeeId;
                    //emp_queue_meeting.CreateDate = DateTime.Now;
                    //emp_queue_meeting.CreateById = parameter.UserId;
                    //context.Queue.Add(emp_queue_meeting);

                    // Người tạo lịch hẹn mặc định là người tham gia
                    listEmployeeContact.ForEach(item =>
                    {
                        var par_queue_meeting = new Queue();
                        par_queue_meeting.QueueId = Guid.NewGuid();
                        par_queue_meeting.SendTo = item.Email.Trim();
                        par_queue_meeting.SendContent = "Địa điểm: " + leadMeeting.LocationMeeting +
                                                        ". Nội dung: " + leadMeeting.Content;
                        par_queue_meeting.Title = leadMeeting.Title + " - " +
                                                  leadMeeting.StartDate.Value.ToString("dd/MM/yyyy HH:mm");
                        par_queue_meeting.Method = "Email";
                        par_queue_meeting.IsSend = false;
                        par_queue_meeting.SenDate = sendDate;
                        par_queue_meeting.FromTo = Email;
                        par_queue_meeting.CustomerId = leadMeeting.LeadId;
                        par_queue_meeting.CustomerMeetingId = leadMeeting.LeadMeetingId;
                        par_queue_meeting.EmployeeId = user.EmployeeId;
                        par_queue_meeting.CreateDate = DateTime.Now;
                        par_queue_meeting.CreateById = parameter.UserId;
                        context.Queue.Add(par_queue_meeting);
                    });
                    #endregion

                    #region Gửi email đến người tham gia

                    #endregion
                }
                else
                {
                    var leadMeeting = new LeadMeeting();

                    leadMeeting.LeadMeetingId = Guid.NewGuid();
                    leadMeeting.LeadId = parameter.LeadMeeting.LeadId;
                    leadMeeting.EmployeeId = user.EmployeeId.Value;
                    leadMeeting.Title = parameter.LeadMeeting.Title;
                    leadMeeting.LocationMeeting = parameter.LeadMeeting.LocationMeeting;
                    leadMeeting.StartDate = SetDate2(parameter.LeadMeeting.StartDate, parameter.LeadMeeting.StartHours.Value.TimeOfDay);
                    leadMeeting.StartHours = parameter.LeadMeeting.StartHours.Value.TimeOfDay;
                    if (parameter.LeadMeeting.EndDate != null && parameter.LeadMeeting.EndHours != null)
                    {
                        leadMeeting.EndDate = SetDate2(parameter.LeadMeeting.EndDate, parameter.LeadMeeting.EndHours.Value.TimeOfDay);
                        leadMeeting.EndHours = parameter.LeadMeeting.EndHours.Value.TimeOfDay;
                    }

                    if (parameter.LeadMeeting.EndHours != null && parameter.LeadMeeting.EndDate == null)
                    {
                        leadMeeting.EndDate = SetDate2(parameter.LeadMeeting.StartDate, parameter.LeadMeeting.EndHours.Value.TimeOfDay);
                        leadMeeting.EndHours = parameter.LeadMeeting.EndHours.Value.TimeOfDay;
                    }
                    leadMeeting.Participant = parameter.LeadMeeting.Participant;
                    leadMeeting.Content = parameter.LeadMeeting.Content;
                    leadMeeting.Active = true;
                    leadMeeting.CreatedById = parameter.UserId;
                    leadMeeting.CreatedDate = DateTime.Now;

                    context.LeadMeeting.Add(leadMeeting);

                    #region Gửi email người tham gia
                    var sendDate = DateTime.Now;

                    var timeNow = DateTime.Now;
                    timeNow = timeNow.AddMinutes(60);
                    var compareTime = DateTime.Compare(timeNow, leadMeeting.StartDate.Value);
                    if (compareTime <= 0)
                    {
                        sendDate = leadMeeting.StartDate.Value.AddMinutes(-60);
                    }

                    // người tạo lịch hẹn mặc định là người tham gia
                    listEmployeeContact.ForEach(item =>
                    {
                        var par_queue_meeting = new Queue();
                        par_queue_meeting.QueueId = Guid.NewGuid();
                        par_queue_meeting.SendTo = item.Email.Trim();
                        par_queue_meeting.SendContent = "Địa điểm: " + leadMeeting.LocationMeeting +
                                                        ". Nội dung: " + leadMeeting.Content;
                        par_queue_meeting.Title = leadMeeting.Title + " - " +
                                                  leadMeeting.StartDate.Value.ToString("dd/MM/yyyy HH:mm");
                        par_queue_meeting.Method = "Email";
                        par_queue_meeting.IsSend = false;
                        par_queue_meeting.SenDate = sendDate;
                        par_queue_meeting.FromTo = Email;
                        par_queue_meeting.CustomerId = leadMeeting.LeadId;
                        par_queue_meeting.CustomerMeetingId = leadMeeting.LeadMeetingId;
                        par_queue_meeting.EmployeeId = user.EmployeeId;
                        par_queue_meeting.CreateDate = DateTime.Now;
                        par_queue_meeting.CreateById = parameter.UserId;
                        context.Queue.Add(par_queue_meeting);
                    });
                    #endregion
                }

                context.SaveChanges();

                return new CreateLeadMeetingResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new CreateLeadMeetingResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public ChangeLeadStatusResult ChangeLeadStatus(ChangeLeadStatusParameter parameter)
        {
            try
            {
                var statusType = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS");
                var draftStatus = context.Category.FirstOrDefault(c => c.CategoryCode == "DRAFT" && c.CategoryTypeId == statusType.CategoryTypeId);
                var leadEdit = context.Lead.FirstOrDefault(f => f.LeadId == parameter.LeadId);
                leadEdit.StatusId = parameter.StatusId;
                if (parameter.StatusId == draftStatus?.CategoryId)
                {
                    leadEdit.CreatedDate = DateTime.Now;
                }
                leadEdit.UpdatedDate = DateTime.Now;
                context.Lead.Update(leadEdit);
                context.SaveChanges();

                return new ChangeLeadStatusResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new ChangeLeadStatusResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetHistoryLeadCareResult GetHistoryLeadCare(GetHistoryLeadCareParameter parameter)
        {
            try
            {
                var listEmployeePosition = context.Position.ToList();
                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();
                var listEmployee = context.Employee.ToList();

                //Hình thức
                var leadCareCategoryTypeId = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HCS").CategoryTypeId;
                var listLeadCareCategory = listCategory.Where(x => x.CategoryTypeId == leadCareCategoryTypeId).ToList();
                var listTypeOfLeadCare1 = listLeadCareCategory
                                                .Where(x => x.CategoryCode == "Gift" || x.CategoryCode == "CallPhone").Select(y => y.CategoryId)
                                                .ToList();
                var listTypeOfLeadCare2 = listLeadCareCategory
                                                .Where(x => x.CategoryCode == "Email" || x.CategoryCode == "SMS").Select(y => y.CategoryId)
                                                .ToList();

                //Trạng thái
                var statusOfLeadCareCategoryId = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCS").CategoryTypeId;
                var statusActiveOfLeadCare = listCategory.FirstOrDefault(x => x.CategoryTypeId == statusOfLeadCareCategoryId && x.CategoryCode == "Active").CategoryId;

                var listLeadCareInfor = new List<LeadCareInforModel>();
                var listAllLeadCareForWeek = new List<LeadCareForWeekModel>();

                /*
                 * Lọc ra các chương trình CSKH thõa mãn các điều kiện:
                 * - Hình thức là Tặng quà và Gọi điện
                 * - Có ngày kích hoạt trong tháng, năm được chọn
                 */

                var listLeadCare1 = context.LeadCare.Where(x => listTypeOfLeadCare1.Contains(x.LeadCareContactType.Value) &&
                  x.ActiveDate.Value.Month == parameter.Month && x.ActiveDate.Value.Year == parameter.Year).ToList();

                //Lọc ra các chương trình CSKH mà có Khách hàng hiện tại tham gia
                var listLeadCareId1 = listLeadCare1.Select(x => x.LeadCareId).ToList();

                if (listLeadCareId1.Count > 0)
                {
                    var listLeadCareIdForLead = context.LeadCareLead.Where(x => listLeadCareId1.Contains(x.LeadCareId.Value) &&
                                    x.LeadId == parameter.LeadId).Select(x => x.LeadCareId).ToList();

                    if (listLeadCareIdForLead.Count > 0)
                    {
                        listLeadCare1 = listLeadCare1.Where(x => listLeadCareIdForLead.Contains(x.LeadCareId)).ToList();
                    }
                    else
                    {
                        listLeadCare1 = new List<LeadCare>();
                    }
                }

                /*
               * Lấy list CustomerCareId trong bảng Queue thõa màn các điều kiện sau:
               * - IsSend = true (Đã gửi)
               * - Là gửi Email hoặc SMS
               * - Có ngày gửi (SenDate) trong tháng, năm được chọn
               */

                //test bo isSend//
                //var listQueueLeadCare2 = context.Queue.Where(x =>
                //   x.IsSend == true &&
                //   (x.Method == "Email" || x.Method == "SMS") && x.SenDate.Value.Month == parameter.Month &&
                //   x.SenDate.Value.Year == parameter.Year && x.CustomerId == parameter.CustomerId).ToList();
                var listQueueLeadCare2 = context.Queue.Where(x =>

                   (x.Method == "Email" || x.Method == "SMS") && x.SenDate.Value.Month == parameter.Month &&
                   x.SenDate.Value.Year == parameter.Year && x.CustomerId == parameter.LeadId).ToList();

                var listQueueLeadCare2Id = listQueueLeadCare2.Select(y => y.CustomerCareId).Distinct().ToList();

                var listLeadCare2 = new List<LeadCare>();
                if (listQueueLeadCare2Id.Count > 0)
                {
                    listLeadCare2 = context.LeadCare.Where(x => listQueueLeadCare2Id.Contains(x.LeadCareId)).ToList();
                }

                //merge 2 list CustomerCare
                listLeadCare1.AddRange(listLeadCare2);
                var listEmployeeId = new List<Guid>();

                var smsLeadCareId = listLeadCareCategory.FirstOrDefault(c => c.CategoryCode == "SMS")?.CategoryId;
                var emailLeadCareId = listLeadCareCategory.FirstOrDefault(c => c.CategoryCode == "Email")?.CategoryId;

                if (listLeadCare1.Count > 0)
                {
                    var listLeadCareId = listLeadCare1.Select(y => y.LeadCareId).ToList();
                    var listAllFeedBack = context.LeadCareFeedBack
                        .Where(x => listLeadCareId.Contains(x.LeadCareId.Value) && x.LeadId == parameter.LeadId).ToList();
                    listEmployeeId = listLeadCare1.Select(y => y.EmployeeCharge.Value).Distinct().ToList();

                    listEmployeeId.ForEach(employeeId =>
                    {
                        var customerCareInfor = new LeadCareInforModel();
                        var emp = listEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                        customerCareInfor.EmployeeCharge = emp.EmployeeId;
                        customerCareInfor.EmployeeName = emp.EmployeeName;
                        customerCareInfor.EmployeePosition = listEmployeePosition
                            .FirstOrDefault(x => x.PositionId == emp.PositionId).PositionName;

                        listLeadCareInfor.Add(customerCareInfor);
                    });

                    listLeadCare1.ForEach(item =>
                    {
                        var leadCareForWeek = new LeadCareForWeekModel();
                        leadCareForWeek.LeadCareId = item.LeadCareId;
                        leadCareForWeek.EmployeeCharge = item.EmployeeCharge.Value;
                        //leadCareForWeek.Title = listLeadCareCategory
                        //    .FirstOrDefault(x => x.CategoryId == item.LeadCareContactType).CategoryName;
                        if (item.LeadCareContactType == smsLeadCareId)
                        {
                            if (item.LeadCareContentSms.Length > 50)
                            {
                                leadCareForWeek.Title = item.LeadCareContentSms.Substring(0, 50) + "...";
                            }
                            else
                            {
                                leadCareForWeek.Title = item.LeadCareContentSms;
                            }
                        }
                        else if (item.LeadCareContactType == emailLeadCareId)
                        {
                            leadCareForWeek.Title = item.LeadCareTitle;
                        }

                        /*
                         * Gửi SMS: 1
                         * Gửi email: 2
                         * Tặng quà: 3
                         * Gọi điện: 4
                         */
                        var customerCareCategoryCode = listLeadCareCategory
                            .FirstOrDefault(x => x.CategoryId == item.LeadCareContactType).CategoryCode;

                        switch (customerCareCategoryCode)
                        {
                            case "SMS":
                                leadCareForWeek.Type = 1;
                                leadCareForWeek.SubTitle = "Xem chi tiết";
                                leadCareForWeek.FeedBackStatus = 0;
                                leadCareForWeek.Background = "#fbe8ba";
                                leadCareForWeek.ActiveDate = listQueueLeadCare2
                                    .FirstOrDefault(x => x.CustomerCareId == item.LeadCareId).SenDate.Value;
                                break;
                            case "Email":
                                leadCareForWeek.Type = 2;
                                leadCareForWeek.SubTitle = "Xem chi tiết";
                                leadCareForWeek.FeedBackStatus = 0;
                                leadCareForWeek.Background = "#e5cbf2";
                                leadCareForWeek.ActiveDate = listQueueLeadCare2.FirstOrDefault(x => x.CustomerCareId == item.LeadCareId).SenDate.Value;
                                break;
                            case "Gift":
                                leadCareForWeek.Type = 3;
                                var checkFeedBackGift = listAllFeedBack.FirstOrDefault(x => x.LeadCareId == item.LeadCareId);
                                leadCareForWeek.SubTitle = checkFeedBackGift != null
                                    ? (checkFeedBackGift.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                leadCareForWeek.FeedBackStatus = checkFeedBackGift != null ? 1 : 2;
                                leadCareForWeek.Background = "#cfdefa";
                                leadCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                            case "CallPhone":
                                leadCareForWeek.Type = 4;
                                var checkFeedBackCallPhone = listAllFeedBack.FirstOrDefault(x => x.LeadCareId == item.LeadCareId);
                                leadCareForWeek.SubTitle = checkFeedBackCallPhone != null
                                    ? (checkFeedBackCallPhone.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                leadCareForWeek.FeedBackStatus = checkFeedBackCallPhone != null ? 1 : 2;
                                leadCareForWeek.Background = "#f4d4e4";
                                leadCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                        }

                        listAllLeadCareForWeek.Add(leadCareForWeek);
                    });

                    #region Nhóm theo nhân viên CSKH và theo tuần

                    var current_month = parameter.Month;
                    var current_year = parameter.Year;
                    //Ngày đầu tiên của tháng
                    var first_date_month = new DateTime(current_year, current_month, 1, 0, 0, 0, 0);
                    //Ngày cuối cùng của tháng
                    var last_date_month = first_date_month.AddMonths(1).AddDays(-1);
                    last_date_month = last_date_month.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

                    //Ngày bắt đầu tuần 1 và Ngày cuối cùng tuần 1
                    var startDateWeek1 = first_date_month;
                    var endDateWeek1 = LastDateOfWeek(startDateWeek1);

                    //Ngày bắt đầu tuần 2 và Ngày cuối cùng tuần 2
                    var startDateWeek2 = FirstDateOfWeek(endDateWeek1);
                    var endDateWeek2 = LastDateOfWeek(startDateWeek2);

                    //Ngày bắt đầu tuần 3 và Ngày cuối cùng tuần 3
                    var startDateWeek3 = FirstDateOfWeek(endDateWeek2);
                    var endDateWeek3 = LastDateOfWeek(startDateWeek3);

                    //Ngày bắt đầu tuần 4 và Ngày cuối cùng tuần 4
                    var startDateWeek4 = FirstDateOfWeek(endDateWeek3);
                    var endDateWeek4 = LastDateOfWeek(startDateWeek4);

                    //Ngày bắt đầu tuần 5 và Ngày cuối cùng tuần 5
                    DateTime? startDateWeek5 = FirstDateOfWeek(endDateWeek4);
                    DateTime? endDateWeek5 = last_date_month;

                    int check = checkLastDateOfMonth(endDateWeek4, last_date_month);

                    if (check == 1)
                    {
                        //Nếu là ngày cuối cùng của tháng
                        endDateWeek4 = last_date_month;
                        startDateWeek5 = null;
                        endDateWeek5 = null;
                    }

                    var listWeek1 = new List<LeadCareForWeekModel>();
                    var listWeek2 = new List<LeadCareForWeekModel>();
                    var listWeek3 = new List<LeadCareForWeekModel>();
                    var listWeek4 = new List<LeadCareForWeekModel>();
                    var listWeek5 = new List<LeadCareForWeekModel>();
                    listLeadCareInfor.ForEach(item =>
                    {
                        listWeek1 = listAllLeadCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek1 &&
                            x.ActiveDate < endDateWeek1).OrderBy(z => z.ActiveDate).ToList();
                        item.Week1 = listWeek1;

                        listWeek2 = listAllLeadCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek2 &&
                            x.ActiveDate < endDateWeek2).OrderBy(z => z.ActiveDate).ToList();
                        item.Week2 = listWeek2;

                        listWeek3 = listAllLeadCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek3 &&
                            x.ActiveDate < endDateWeek3).OrderBy(z => z.ActiveDate).ToList();
                        item.Week3 = listWeek3;

                        listWeek4 = listAllLeadCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek4 &&
                            x.ActiveDate < endDateWeek4).OrderBy(z => z.ActiveDate).ToList();
                        item.Week4 = listWeek4;

                        if (check != 1)
                        {
                            listWeek5 = listAllLeadCareForWeek.Where(x =>
                                x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek5 &&
                                x.ActiveDate < endDateWeek5).OrderBy(z => z.ActiveDate).ToList();
                            item.Week5 = listWeek5;
                        }
                    });
                    #endregion
                }

                return new GetHistoryLeadCareResult()
                {
                    Status = true,
                    Message = "Success",
                    ListCustomerCareInfor = listLeadCareInfor
                };
            }
            catch (Exception e)
            {
                return new GetHistoryLeadCareResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private string replaceTokenForContent(string current_content, Guid leadId)
        {
            var Name = TokenForContent.Name;
            var Hotline = TokenForContent.Hotline;
            var Address = TokenForContent.Address;

            if (current_content.Contains(Name))
            {
                current_content = current_content.Replace(Name, context.Contact.FirstOrDefault(cus => cus.ObjectId == leadId && cus.ObjectType == "LEA") != null ?
                                                                    context.Contact.FirstOrDefault(cus => cus.ObjectId == leadId && cus.ObjectType == "LEA").FirstName : "");
            }
            if (current_content.Contains(Hotline))
            {
                current_content = current_content.Replace(Hotline, context.Contact.FirstOrDefault(c => c.ObjectId == leadId && c.ObjectType == "LEA") != null ?
                                                                    context.Contact.FirstOrDefault(c => c.ObjectId == leadId && c.ObjectType == "LEA").Phone : "");
            }
            if (current_content.Contains(Address))
            {
                current_content = current_content.Replace(Address, context.Contact.FirstOrDefault(c => c.ObjectId == leadId && c.ObjectType == "LEA")?.Address);
            }

            return current_content;
        }

        private DateTime LastDateOfWeek(DateTime dateNow)
        {
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow.AddDays(6);
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

            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 23, 59, 59, 999);
            return dateReturn;
        }

        private DateTime FirstDateOfWeek(DateTime dateNow)
        {
            var firstDate = dateNow.AddDays(1);
            var _day = firstDate.Day;
            var _month = firstDate.Month;
            var _year = firstDate.Year;
            firstDate = new DateTime(_year, _month, _day, 0, 0, 0, 0);
            return firstDate;
        }

        private int checkLastDateOfMonth(DateTime endDateWeek4, DateTime last_date_month)
        {
            //Kiểm tra xem ngày cuối của tuần thứ 4 có phải ngày cuối cùng của tháng hay không?
            if (endDateWeek4.Day != last_date_month.Day)
            {
                //Nếu không phải thì có tuần thứ 5
                return 2;
            }
            else
            {
                //Nếu là ngày cuối cùng của tháng
                return 1;
            }
        }

        public GetDataPreviewLeadCareResult GetDataPreviewLeadCare(GetDataPreviewLeadCareParameter parameter)
        {
            try
            {
                var effecttiveFromDate = DateTime.Now;
                var effecttiveToDate = DateTime.Now;
                var sendDate = DateTime.Now;
                var statusName = "";
                var previewEmailContent = "";
                var previewEmailName = "";
                var previewEmailTitle = "";
                var previewSmsPhone = "";
                var previewSmsContent = "";

                var leadCare =
                    context.LeadCare.FirstOrDefault(x => x.LeadCareId == parameter.LeadCareId);
                effecttiveFromDate = leadCare.EffecttiveFromDate.Value;
                effecttiveToDate = leadCare.EffecttiveToDate.Value;

                var queue = context.Queue.FirstOrDefault(x =>
                    x.CustomerId == parameter.LeadId && x.CustomerCareId == parameter.LeadCareId);

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCS").CategoryTypeId;
                statusName = context.Category
                    .FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryId == leadCare.StatusId)
                    .CategoryName;

                switch (parameter.Mode)
                {
                    case "Email":
                        previewEmailTitle = queue.Title;
                        previewEmailContent = queue.SendContent;
                        sendDate = queue.SenDate.Value;
                        break;

                    case "SMS":
                        previewSmsContent = queue.SendContent;
                        sendDate = queue.SenDate.Value;
                        break;
                }

                return new GetDataPreviewLeadCareResult()
                {
                    Status = true,
                    Message = "Success",
                    EffecttiveFromDate = effecttiveFromDate,
                    EffecttiveToDate = effecttiveToDate,
                    SendDate = sendDate,
                    StatusName = statusName,
                    PreviewEmailContent = previewEmailContent,
                    PreviewEmailName = previewEmailName,
                    PreviewEmailTitle = previewEmailTitle,
                    PreviewSmsPhone = previewSmsPhone,
                    PreviewSmsContent = previewSmsContent
                };
            }
            catch (Exception e)
            {
                return new GetDataPreviewLeadCareResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDataLeadCareFeedBackResult GetDataLeadCareFeedBack(GetDataLeadCareFeedBackParameter parameter)
        {
            try
            {
                var name = "";
                var typeName = "";
                Guid? feedBackCode = null;
                var feedBackContent = "";
                var listFeedBackCode = new List<CategoryEntityModel>();

                var feedBack = context.LeadCareFeedBack.FirstOrDefault(x =>
                    x.LeadId == parameter.LeadId && x.LeadCareId == parameter.LeadCareId);

                var leadCare =
                    context.LeadCare.FirstOrDefault(x => x.LeadCareId == parameter.LeadCareId);
                name = leadCare.LeadCareTitle;

                var categoryTypeId1 = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HCS")
                    .CategoryTypeId;
                var fromDate = leadCare.EffecttiveFromDate;
                var toDate = leadCare.EffecttiveToDate;
                typeName = context.Category.FirstOrDefault(x =>
                        x.CategoryId == leadCare.LeadCareContactType.Value &&
                        x.CategoryTypeId == categoryTypeId1)
                    .CategoryName;

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "MHO").CategoryTypeId;
                listFeedBackCode = context.Category.Where(x => x.CategoryTypeId == categoryTypeId).Select(y =>
                    new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName,
                        IsDefault = y.IsDefauld
                    }).ToList();

                if (feedBack != null)
                {
                    feedBackCode = feedBack.FeedBackCode;
                    feedBackContent = feedBack.FeedBackContent;
                }

                return new GetDataLeadCareFeedBackResult()
                {
                    Status = true,
                    Message = "Success",
                    Name = name,
                    FromDate = fromDate.Value,
                    ToDate = toDate.Value,
                    TypeName = typeName,
                    FeedBackCode = feedBackCode,
                    FeedBackContent = feedBackContent,
                    ListFeedBackCode = listFeedBackCode
                };
            }
            catch (Exception e)
            {
                return new GetDataLeadCareFeedBackResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SaveLeadCareFeedBackResult SaveLeadCareFeedBack(SaveLeadCareFeedBackParameter parameter)
        {
            try
            {
                var feedBack = context.LeadCareFeedBack.FirstOrDefault(x =>
                    x.LeadId == parameter.LeadCareFeedBack.LeadId &&
                    x.LeadCareId == parameter.LeadCareFeedBack.LeadCareId);
                var leadCare = context.LeadCare.FirstOrDefault(x =>
                    x.LeadCareId == parameter.LeadCareFeedBack.LeadCareId);
                var feedBackFromDate = leadCare.EffecttiveFromDate;
                var feedBackToDate = leadCare.EffecttiveToDate;
                var feedBackType = leadCare.LeadCareContactType;
                var feedBackCode = parameter.LeadCareFeedBack.FeedBackCode;
                var feedBackContent = parameter.LeadCareFeedBack.FeedBackContent;

                if (feedBack == null)
                {
                    var newFeedBack = new LeadCareFeedBack();
                    newFeedBack.LeadCareFeedBackId = Guid.NewGuid();
                    newFeedBack.FeedBackFromDate = feedBackFromDate;
                    newFeedBack.FeedBackToDate = feedBackToDate;
                    newFeedBack.FeedbackType = feedBackType;
                    newFeedBack.FeedBackCode = feedBackCode;
                    newFeedBack.FeedBackContent = feedBackContent;
                    newFeedBack.LeadId = parameter.LeadCareFeedBack.LeadId;
                    newFeedBack.LeadCareId = parameter.LeadCareFeedBack.LeadCareId;
                    newFeedBack.CreateDate = DateTime.Now;
                    newFeedBack.CreateById = parameter.UserId;

                    context.LeadCareFeedBack.Add(newFeedBack);
                    context.SaveChanges();
                }
                else
                {
                    feedBack.FeedBackCode = feedBackCode;
                    feedBack.FeedBackContent = feedBackContent;
                    feedBack.UpdateDate = DateTime.Now;
                    feedBack.UpdateById = parameter.UserId;

                    context.LeadCareFeedBack.Update(feedBack);
                    context.SaveChanges();
                }

                return new SaveLeadCareFeedBackResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new SaveLeadCareFeedBackResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetHistoryLeadMeetingResult GetHistoryLeadMeeting(GetHistoryLeadMeetingParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listEmployeePosition = context.Position.ToList();

                var leadMeetingInfor = new LeadMeetingInforModel();
                leadMeetingInfor.EmployeeId = employee.EmployeeId;
                leadMeetingInfor.EmployeeName = employee.EmployeeName;
                leadMeetingInfor.EmployeePosition = listEmployeePosition
                    .FirstOrDefault(x => x.PositionId == employee.PositionId).PositionName;

                var listAllLeadMeetingForWeek = new List<LeadMeetingForWeekModel>();

                var listAllLeadMeeting = context.LeadMeeting.Where(x =>
                        x.EmployeeId == employee.EmployeeId && x.LeadId == parameter.LeadId &&
                        x.StartDate.Value.Month == parameter.Month &&
                        x.StartDate.Value.Year == parameter.Year)
                    .ToList();

                listAllLeadMeeting.ForEach(item =>
                {
                    var leadMeetingForWeek = new LeadMeetingForWeekModel();
                    leadMeetingForWeek.LeadMeetingId = item.LeadMeetingId;
                    leadMeetingForWeek.EmployeeId = item.EmployeeId.Value;
                    leadMeetingForWeek.Title = item.Title;
                    leadMeetingForWeek.Subtitle = item.StartDate.Value.ToString("dd/MM/yyyy") + " - " + item.StartDate.Value.ToString("HH:mm");
                    leadMeetingForWeek.Background = "#ffcc00";
                    leadMeetingForWeek.StartDate = item.StartDate;
                    leadMeetingForWeek.StartHours = item.StartHours;
                    listAllLeadMeetingForWeek.Add(leadMeetingForWeek);
                });

                var current_month_meeting = parameter.Month;
                var current_year_meeting = parameter.Year;
                //Ngày đầu tiên của tháng
                var first_date_month_meeting = new DateTime(current_year_meeting, current_month_meeting, 1, 0, 0, 0, 0);
                //Ngày cuối cùng của tháng
                var last_date_month_meeting = first_date_month_meeting.AddMonths(1).AddDays(-1);
                last_date_month_meeting = last_date_month_meeting.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

                //Ngày bắt đầu tuần 1 và Ngày cuối cùng tuần 1
                var startDateMeetingWeek1 = first_date_month_meeting;
                var endDateMeetingWeek1 = LastDateOfWeek(startDateMeetingWeek1);

                //Ngày bắt đầu tuần 2 và Ngày cuối cùng tuần 2
                var startDateMeetingWeek2 = FirstDateOfWeek(endDateMeetingWeek1);
                var endDateMeetingWeek2 = LastDateOfWeek(startDateMeetingWeek2);

                //Ngày bắt đầu tuần 3 và Ngày cuối cùng tuần 3
                var startDateMeetingWeek3 = FirstDateOfWeek(endDateMeetingWeek2);
                var endDateMeetingWeek3 = LastDateOfWeek(startDateMeetingWeek3);

                //Ngày bắt đầu tuần 4 và Ngày cuối cùng tuần 4
                var startDateMeetingWeek4 = FirstDateOfWeek(endDateMeetingWeek3);
                var endDateMeetingWeek4 = LastDateOfWeek(startDateMeetingWeek4);

                //Ngày bắt đầu tuần 5 và Ngày cuối cùng tuần 5
                DateTime? startDateMeetingWeek5 = FirstDateOfWeek(endDateMeetingWeek4);
                DateTime? endDateMeetingWeek5 = last_date_month_meeting;

                int checkMeeting = checkLastDateOfMonth(endDateMeetingWeek4, last_date_month_meeting);

                if (checkMeeting == 1)
                {
                    //Nếu là ngày cuối cùng của tháng
                    endDateMeetingWeek4 = last_date_month_meeting;
                    startDateMeetingWeek5 = null;
                    endDateMeetingWeek5 = null;
                }

                var week1 = new List<LeadMeetingForWeekModel>();
                var week2 = new List<LeadMeetingForWeekModel>();
                var week3 = new List<LeadMeetingForWeekModel>();
                var week4 = new List<LeadMeetingForWeekModel>();
                var week5 = new List<LeadMeetingForWeekModel>();

                week1 = listAllLeadMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek1 && x.StartDate < endDateMeetingWeek1)
                    .OrderBy(z => z.StartDate).ToList();

                week2 = listAllLeadMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek2 && x.StartDate < endDateMeetingWeek2)
                    .OrderBy(z => z.StartDate).ToList();

                week3 = listAllLeadMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek3 && x.StartDate < endDateMeetingWeek3)
                    .OrderBy(z => z.StartDate).ToList();

                week4 = listAllLeadMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek4 && x.StartDate < endDateMeetingWeek4)
                    .OrderBy(z => z.StartDate).ToList();

                if (checkMeeting != 1)
                {
                    week5 = listAllLeadMeetingForWeek
                        .Where(x => x.StartDate >= startDateMeetingWeek5 && x.StartDate < endDateMeetingWeek5)
                        .OrderBy(z => z.StartDate).ToList();
                }

                leadMeetingInfor.Week1 = week1;
                leadMeetingInfor.Week2 = week2;
                leadMeetingInfor.Week3 = week3;
                leadMeetingInfor.Week4 = week4;
                leadMeetingInfor.Week5 = week5;

                return new GetHistoryLeadMeetingResult()
                {
                    Status = true,
                    Message = "Succees",
                    LeadMeetingInfor = leadMeetingInfor
                };
            }
            catch (Exception e)
            {
                return new GetHistoryLeadMeetingResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDataLeadMeetingByIdResult GetDataLeadMeetingById(GetDataLeadMeetingByIdParameter parameter)
        {
            try
            {
                var leadMeeting = new LeadMeetingEntityModel();

                if (parameter.LeadMeetingId != null)
                {
                    leadMeeting =
                        context.LeadMeeting.Where(x => x.LeadMeetingId == parameter.LeadMeetingId).Select(
                            y => new LeadMeetingEntityModel
                            {
                                LeadMeetingId = y.LeadMeetingId,
                                LeadId = y.LeadId,
                                EmployeeId = y.EmployeeId,
                                Title = y.Title,
                                LocationMeeting = y.LocationMeeting,
                                StartDate = y.StartDate,
                                //StartHours = y.StartHours,
                                EndDate = y.EndDate,
                                Content = y.Content,
                                Participant = y.Participant
                            }).FirstOrDefault();
                }

                return new GetDataLeadMeetingByIdResult()
                {
                    Status = true,
                    Message = "Success",
                    LeadMeeting = leadMeeting
                };
            }
            catch (Exception e)
            {
                return new GetDataLeadMeetingByIdResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetEmployeeSellerResult GetEmployeeSeller(GetEmployeeSellerParameter parameter)
        {
            try
            {
                var listEmployee = new List<EmployeeEntityModel>();
                var eployeePerInChange = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
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
                else
                {
                    // Lấy nhân viên cấp dưới cùng phòng ban
                    listEmployee = parameter.ListEmployeeByAccount.Where(x => x.OrganizationId == eployeePerInChange.OrganizationId.Value).Select(
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

                    // Thêm nhân viên quản lý là chính nó
                    //var emp = listEmployee.Where(e => e.EmployeeId == parameter.EmployeeId).ToList();
                    //if (emp.Count() == 0)
                    //{
                    //    listEmployee.Add(new EmployeeEntityModel
                    //    {
                    //        EmployeeId = eployeePerInChange.EmployeeId,
                    //        EmployeeCode = eployeePerInChange.EmployeeCode,
                    //        EmployeeName = eployeePerInChange.EmployeeCode + " - " + eployeePerInChange.EmployeeName,
                    //        IsManager = eployeePerInChange.IsManager,
                    //        PositionId = eployeePerInChange.PositionId,
                    //        OrganizationId = eployeePerInChange.OrganizationId,
                    //        EmployeeCodeName = eployeePerInChange.EmployeeCode + " - " + eployeePerInChange.EmployeeName
                    //    });
                    //}

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
                return new GetEmployeeSellerResult()
                {
                    Status = true,
                    Message = "Success",
                    ListEmployee = listEmployee.OrderBy(z => z.EmployeeName).ToList()
                };
            }
            catch (Exception e)
            {
                return new GetEmployeeSellerResult()
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
                    ListVendor = listVendor ?? new List<VendorEntityModel>(),
                    ListObjectAttributeNameProduct = listObjectAttributeNameProduct ?? new List<ObjectAttributeNameProductModel>(),
                    ListObjectAttributeValueProduct = listObjectAttributeValueProduct ?? new List<ObjectAttributeValueProductModel>(),
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

        public GetEmployeeByPersonInChargeResult GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeParameter parameter)
        {
            try
            {
                var listEmployee = new List<EmployeeEntityModel>();
                if (parameter.EmployeeId != Guid.Empty)
                {
                    var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId);

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
                        });
                    }
                }
                else
                {
                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

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
                        });
                    }
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

        public GetMasterDataReportLeadResult GetMasterDataReportLead(GetMasterDataReportLeadParameter parameter)
        {
            try
            {

                var investFundTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "IVF" && c.Active == true)?.CategoryTypeId;

                var listEmploye = context.Employee
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName
                    }).OrderBy(c => c.EmployeeName).ToList();

                var listAllSource = context.Category.Where(c => c.CategoryTypeId == investFundTypeId && c.Active == true)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName
                    }).OrderBy(c => c.CategoryName).ToList();
                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName
                    }).ToList();

                return new GetMasterDataReportLeadResult
                {
                    Status = true,
                    Message = "Success",
                    ListEmployee = listEmploye,
                    ListSource = listAllSource,
                    ListArea = listArea,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataReportLeadResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public ChangeLeadStatusSupportResult ChangeLeadStatusSupport(ChangeLeadStatusSupportParameter parameter)
        {
            try
            {
                var lead = context.Lead.FirstOrDefault(x => x.LeadId == parameter.LeadId);

                if (lead == null)
                {
                    return new ChangeLeadStatusSupportResult()
                    {
                        Status = false,
                        Message = "Cơ hội không tồn tại trên hệ thống"
                    };
                }

                lead.StatusSuportId = parameter.StatusSupportId;
                context.Lead.Update(lead);
                context.SaveChanges();

                return new ChangeLeadStatusSupportResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new ChangeLeadStatusSupportResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public ReportLeadResult ReportLead(ReportLeadParameter parameter)
        {
            try
            {
                var listLeadFollowAge = new List<ReportLeadModel>();
                var listLeadFollowPic = new List<ReportLeadModel>();
                var listLeadFollowSource = new List<ReportLeadModel>();
                var listLeadFollowAddress = new List<ReportLeadModel>();
                var listLeadFollowStatus = new List<ReportLeadModel>();
                switch (parameter.ReportCode)
                {
                    case "AGE":
                        listLeadFollowAge = GetReportLeadFollowAge();
                        break;
                    case "PIC":
                        listLeadFollowPic = GetReportLeadFollowPic(parameter.ListEmployeeId);
                        break;
                    case "SOURCE":
                        listLeadFollowSource = GetReportLeadFollowSource(parameter.ListSourceId);
                        break;
                    case "ADDRESS":
                        listLeadFollowAddress = GetReportLeadFollowAddress(parameter.ListGeographicalAreaId);
                        break;
                    case "STATUS":
                        listLeadFollowStatus = GetReportLeadFollowMonth(parameter.TimeParameter);
                        break;
                    default:
                        break;
                }

                return new ReportLeadResult
                {
                    Status = true,
                    Message = "Success",
                    ListReportLeadFollowAge = listLeadFollowAge,
                    ListReportLeadFollowPic = listLeadFollowPic,
                    ListReportLeadFollowSource = listLeadFollowSource,
                    ListReportLeadFollowProvincial = listLeadFollowAddress,
                    ListReportLeadFollowMonth = listLeadFollowStatus
                };
            }
            catch (Exception ex)
            {
                return new ReportLeadResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        private List<ReportLeadModel> GetReportLeadFollowAge()
        {
            var lstCommonContactLead = context.Contact.Where(c => c.ObjectType == "LEA").ToList();
            var listCommonEmployee = context.Employee.Where(c => c.Active == true).ToList();

            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS")?.CategoryTypeId;
            var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId).ToList();

            var statusWinId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;
            var statusLoseId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CANC")?.CategoryId;

            var probabilityTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "PROB")?.CategoryTypeId;
            var listAllProbability = context.Category.Where(c => c.CategoryTypeId == probabilityTypeId).ToList();

            var list = context.Lead.Where(c => c.Active == true && (c.StatusId == statusWinId || c.StatusId == statusLoseId))
                .Select(m => new ReportLeadModel
                {
                    LeadName = "",
                    PicName = "",
                    ProbabilityName = "",
                    DayCount = 0,
                    StatusName = "",
                    StatusId = m.StatusId,
                    ProbabilityId = m.ProbabilityId,
                    LeadId = m.LeadId,
                    PersonInChargeId = m.PersonInChargeId,
                    CreatedDate = m.CreatedDate,
                    UpdatedDate = m.UpdatedDate
                }).ToList();

            list.ForEach(item =>
            {
                item.PicName = listCommonEmployee.FirstOrDefault(c => c.EmployeeId == item.PersonInChargeId)?.EmployeeName ?? "";
                item.LeadName = lstCommonContactLead.FirstOrDefault(c => c.ObjectId == item.LeadId)?.FirstName ?? "";
                item.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId)?.CategoryName ?? "";
                item.ProbabilityName = listAllProbability.FirstOrDefault(c => c.CategoryId == item.ProbabilityId)?.CategoryName ?? "";
                item.DayCount = item.UpdatedDate?.Subtract(item.CreatedDate).TotalDays;
            });

            return list;

        }

        private List<ReportLeadModel> GetReportLeadFollowPic(List<Guid> lstEmployeeId)
        {
            var listCommonEmployee = context.Employee.Where(c => c.Active == true).ToList();
            var listCommonLead = context.Lead.Where(c => c.Active == true).ToList();

            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS" && c.Active == true)?.CategoryTypeId;
            var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId && c.Active == true).ToList();
            var statusWinId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;
            var statusLoseId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CANC")?.CategoryId;

            var list = new List<ReportLeadModel>();
            var listLead = listCommonLead.Where(c => c.PersonInChargeId != null && (lstEmployeeId == null || lstEmployeeId.Count == 0 || lstEmployeeId.Contains(c.PersonInChargeId.Value))).GroupBy(c => c.PersonInChargeId)
                .Select(m => new { PersonInChargeId = m.First().PersonInChargeId }).ToList();
            if (listLead != null)
            {
                listLead.ForEach(item =>
                {
                    if (item.PersonInChargeId != null)
                    {
                        var listLeadWin = listCommonLead.Where(c => c.PersonInChargeId == item.PersonInChargeId && c.StatusId == statusWinId).ToList();
                        var listLeadLose = listCommonLead.Where(c => c.PersonInChargeId == item.PersonInChargeId && c.StatusId == statusLoseId).ToList();
                        var listLeadUndefined = listCommonLead.Where(c => c.PersonInChargeId == item.PersonInChargeId && c.StatusId != statusLoseId && c.StatusId != statusWinId).ToList();

                        var report = new ReportLeadModel
                        {
                            PicCode = listCommonEmployee.FirstOrDefault(c => c.EmployeeId == item.PersonInChargeId)?.EmployeeCode,
                            PicName = listCommonEmployee.FirstOrDefault(c => c.EmployeeId == item.PersonInChargeId)?.EmployeeName,
                            WinCount = listLeadWin.Count,
                            LoseCount = listLeadLose.Count,
                            UndefinedCount = listLeadUndefined.Count,
                        };
                        list.Add(report);
                    }
                });
            }

            list = list.OrderBy(c => c.PicName).ToList();

            return list;
        }

        private List<ReportLeadModel> GetReportLeadFollowSource(List<Guid> lstSourceId)
        {
            var listCommonSaleBidding = context.SaleBidding.ToList();
            var listCommonQuote = context.Quote.ToList();
            var listCommonContract = context.Contract.ToList();
            var listCommonCustomerOrder = context.CustomerOrder.ToList();

            var listCommonLead = context.Lead.Where(c => c.Active == true).ToList();
            var investFundTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "IVF" && c.Active == true)?.CategoryTypeId;
            var listAllInvestFund = context.Category.Where(c => c.CategoryTypeId == investFundTypeId && c.Active == true).ToList();

            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS" && c.Active == true)?.CategoryTypeId;
            var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId && c.Active == true).ToList();
            var statusWinId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;
            var statusLoseId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CANC")?.CategoryId;

            var strCodeOrderStatus = context.SystemParameter.FirstOrDefault(c => c.SystemKey == "OrderStatus")?.SystemValueString;

            var listOrderStatus = new List<string>();
            if (strCodeOrderStatus != null && strCodeOrderStatus.Contains(';'))
            {
                listOrderStatus = strCodeOrderStatus.Split(';').ToList();
            }
            else
            {
                listOrderStatus.Add(strCodeOrderStatus);
            }
            var listOrderStatusId = context.OrderStatus.Where(c => listOrderStatus.Contains(c.OrderStatusCode)).Select(c => c.OrderStatusId).ToList();

            var list = new List<ReportLeadModel>();
            var listLead = listCommonLead.Where(c => c.InvestmentFundId != null && (lstSourceId == null || lstSourceId.Count == 0 || lstSourceId.Contains(c.InvestmentFundId.Value))).GroupBy(c => c.InvestmentFundId)
                .Select(m => new { InvestmentFundId = m.First().InvestmentFundId }).ToList();

            if (listLead != null)
            {
                listLead.ForEach(item =>
                {
                    if (item.InvestmentFundId != null)
                    {
                        var listLeadWin = listCommonLead.Where(c => c.InvestmentFundId == item.InvestmentFundId && c.StatusId == statusWinId).ToList();
                        var listLeadLose = listCommonLead.Where(c => c.InvestmentFundId == item.InvestmentFundId && c.StatusId == statusLoseId).ToList();
                        var listLeadUndefined = listCommonLead.Where(c => c.InvestmentFundId == item.InvestmentFundId && c.StatusId != statusLoseId && c.StatusId != statusWinId).ToList();

                        ///
                        var listLeadId = listCommonLead.Where(c => c.InvestmentFundId == item.InvestmentFundId).Select(c => c.LeadId).ToList();

                        var listSaleBiddingId = listCommonSaleBidding.Where(c => listLeadId.Contains(c.LeadId)).Select(c => c.SaleBiddingId).ToList();
                        var listQuoteIdFromSaleBidding = listCommonQuote.Where(c => c.SaleBiddingId != null && listSaleBiddingId.Contains(c.SaleBiddingId.Value)).Select(c => c.QuoteId).ToList();
                        var listQuoteId = listCommonQuote.Where(c => c.LeadId != null && listLeadId.Contains(c.LeadId.Value)).Select(c => c.QuoteId).ToList();

                        listQuoteId.AddRange(listQuoteIdFromSaleBidding);

                        var listContractId = listCommonContract.Where(c => c.QuoteId != null && listQuoteId.Contains(c.QuoteId.Value)).Select(c => c.ContractId).ToList();

                        var listCusOrderFromQuote = listCommonCustomerOrder.Where(c => c.QuoteId != null && listQuoteId.Contains(c.QuoteId.Value)).ToList();
                        var listCusOrderFromContract = listCommonCustomerOrder.Where(c => c.OrderContractId != null && listContractId.Contains(c.OrderContractId.Value)).ToList();

                        listCusOrderFromQuote.AddRange(listCusOrderFromContract);
                        var listCusOrderDistint = listCusOrderFromQuote.GroupBy(c => c.OrderId).Select(m => new
                        {
                            StatusId = m.First().StatusId,
                            Amount = m.First().Amount,
                            DiscountType = m.First().DiscountType.Value,
                            DiscountValue = m.First().DiscountValue.Value
                        }).ToList();

                        var listCusOrder = listCusOrderDistint.Where(c => c.StatusId != null && listOrderStatusId.Contains(c.StatusId.Value))
                            .Select(m => new
                            {
                                SumAmount = m.DiscountType == true ? (m.Amount - m.Amount * m.DiscountValue / 100) : (m.Amount - m.DiscountValue)
                            }).ToList();


                        var report = new ReportLeadModel
                        {
                            PotentialSource = listAllInvestFund.FirstOrDefault(c => c.CategoryId == item.InvestmentFundId)?.CategoryName,
                            WinCount = listLeadWin.Count,
                            LoseCount = listLeadLose.Count,
                            UndefinedCount = listLeadUndefined.Count,
                            SumAmount = listCusOrder.Sum(c => c.SumAmount),
                        };
                        list.Add(report);
                    }
                });
            }

            list = list.Where(c => c.PotentialSource != string.Empty && c.PotentialSource != null).OrderBy(c => c.Provincial).ToList();

            return list;
        }

        private List<ReportLeadModel> GetReportLeadFollowAddress(List<Guid> listGeographicalAreaId)
        {
            //var listCommonContact = context.Contact.Where(c => c.Active == true && c.ObjectType == "LEA").ToList();
            //var listCommonProvince = context.Province.Where(c => c.Active == true).ToList();
            var listCommonArea = context.GeographicalArea.Where(c => c.Active == true).ToList();
            var listCommonLead = context.Lead.Where(c => c.Active == true)
                .Select(m => new LeadEntityModel
                {
                    LeadId = m.LeadId,
                    StatusId = m.StatusId,
                    GeographicalAreaId = m.GeographicalAreaId
                }).ToList();

            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS" && c.Active == true)?.CategoryTypeId;
            var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId && c.Active == true).ToList();
            var statusWinId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;
            var statusLoseId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CANC")?.CategoryId;

            var list = new List<ReportLeadModel>();
            var listLead = listCommonLead.Where(c => c.GeographicalAreaId != null && (listGeographicalAreaId == null || listGeographicalAreaId.Count == 0 || listGeographicalAreaId.Contains(c.GeographicalAreaId.Value))).GroupBy(c => c.GeographicalAreaId)
                .Select(m => new { GeographicalAreaId = m.First().GeographicalAreaId }).ToList();

            if (listLead != null)
            {
                listLead.ForEach(item =>
                {
                    if (item.GeographicalAreaId != null)
                    {
                        var listLeadWin = listCommonLead.Where(c => c.GeographicalAreaId == item.GeographicalAreaId && c.StatusId == statusWinId).ToList();
                        var listLeadLose = listCommonLead.Where(c => c.GeographicalAreaId == item.GeographicalAreaId && c.StatusId == statusLoseId).ToList();
                        var listLeadUndefined = listCommonLead.Where(c => c.GeographicalAreaId == item.GeographicalAreaId && c.StatusId != statusLoseId && c.StatusId != statusWinId).ToList();

                        var report = new ReportLeadModel
                        {
                            Provincial = listCommonArea.FirstOrDefault(c => c.GeographicalAreaId == item.GeographicalAreaId)?.GeographicalAreaName,
                            WinCount = listLeadWin.Count,
                            LoseCount = listLeadLose.Count,
                            UndefinedCount = listLeadUndefined.Count,
                        };
                        list.Add(report);
                    }
                });
            }

            list = list.OrderBy(c => c.Provincial).ToList();

            return list;
        }

        private List<ReportLeadModel> GetReportLeadFollowMonth(TimeSearchModel timeParameter)
        {
            var list = new List<ReportLeadModel>();
            var statusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CHS" && c.Active == true)?.CategoryTypeId;
            var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeId && c.Active == true).ToList();
            var statusWinId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;
            var statusLoseId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "CANC")?.CategoryId;

            if (timeParameter.Code == "IN")
            {
                var listEntity = context.Lead.Where(c => c.UpdatedDate != null && (c.UpdatedDate.Value.Year == timeParameter.Year)).ToList();
                for (int i = 1; i <= 12; i++)
                {
                    var listLeadWin = listEntity.Where(c => c.UpdatedDate.Value.Month == i && c.StatusId == statusWinId).ToList();
                    var listLeadLose = listEntity.Where(c => c.UpdatedDate.Value.Month == i && c.StatusId == statusLoseId).ToList();
                    var listLeadUndefined = listEntity.Where(c => c.UpdatedDate.Value.Month == i && c.StatusId != statusLoseId && c.StatusId != statusWinId).ToList();

                    var report = new ReportLeadModel
                    {
                        Month = $"{i}/{timeParameter.Year}",
                        WinCount = listLeadWin.Count,
                        LoseCount = listLeadLose.Count,
                        UndefinedCount = listLeadUndefined.Count,
                        MonthTime = new DateTime(timeParameter.Year, i, 1),
                    };
                    list.Add(report);
                }
            }
            else if (timeParameter.Code == "EQUAL")
            {
                var listEntity = context.Lead.Where(c => c.UpdatedDate != null && (timeParameter.FromDate == null || timeParameter.FromDate == DateTime.MinValue || c.UpdatedDate.Value >= timeParameter.FromDate)
                          && (timeParameter.ToDate == null || timeParameter.ToDate == DateTime.MinValue || c.UpdatedDate.Value <= timeParameter.ToDate))
                          .Select(m => new LeadEntityModel
                          {
                              LeadId = m.LeadId,
                              LeadCode = m.LeadCode,
                              StatusId = m.StatusId,
                              UpdatedMonthYear = $"{m.UpdatedDate.Value.Month}/{m.UpdatedDate.Value.Year}",
                              UpdatedDate = m.UpdatedDate
                          }).ToList();

                var listLead = listEntity.GroupBy(c => c.UpdatedMonthYear).Select(m => new { UpdatedMonthYear = m.First().UpdatedMonthYear, UpdatedDate = m.First().UpdatedDate }).ToList();
                listLead.ForEach(item =>
                {
                    var listLeadWin = listEntity.Where(c => c.UpdatedMonthYear.Equals(item.UpdatedMonthYear) && c.StatusId == statusWinId).ToList();
                    var listLeadLose = listEntity.Where(c => c.UpdatedMonthYear.Equals(item.UpdatedMonthYear) && c.StatusId == statusLoseId).ToList();
                    var listLeadUndefined = listEntity.Where(c => c.UpdatedMonthYear.Equals(item.UpdatedMonthYear) && c.StatusId != statusLoseId && c.StatusId != statusWinId).ToList();

                    var report = new ReportLeadModel
                    {
                        Month = item.UpdatedMonthYear,
                        WinCount = listLeadWin.Count,
                        LoseCount = listLeadLose.Count,
                        UndefinedCount = listLeadUndefined.Count,
                        MonthTime = new DateTime(item.UpdatedDate.Value.Year, item.UpdatedDate.Value.Month, 1),
                    };
                    list.Add(report);
                });
            }

            list = list.OrderBy(c => c.MonthTime).ToList();
            return list;
        }
    }
}
