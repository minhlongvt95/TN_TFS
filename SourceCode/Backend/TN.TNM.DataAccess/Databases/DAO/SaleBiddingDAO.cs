using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;
using TN.TNM.DataAccess.Messages.Results.SaleBidding;
using TN.TNM.DataAccess.Models.SaleBidding;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;
using TN.TNM.DataAccess.Models.ProductAttributeCategoryValue;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.Quote;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using Microsoft.AspNetCore.Http;
using TN.TNM.Common;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class SaleBiddingDAO : BaseDAO, ISaleBiddingDataAccess
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
        public SaleBiddingDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
            Configuration = iconfiguration;
        }

        public GetMasterDataCreateSaleBiddingResult GetMasterDataCreateSaleBidding(GetMasterDataCreateSaleBiddingParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new GetMasterDataCreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_USER
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetMasterDataCreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_EMPLOYEE
                    };
                }
                var listEmployeeCommon = context.Employee.ToList();
                var listCustomerCommon = context.Customer.ToList();
                var employeeLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.Active == true);
                if (employeeLogin == null)
                {
                    return new GetMasterDataCreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_EMPLOYEE
                    };
                }
                var listPersonResult = new List<EmployeeEntityModel>();// Danh sách người phụ trách 
                var listEmployeeResult = new List<EmployeeEntityModel>();// Danh sách người phụ trách 
                // Lấy thông tin của cơ hội đang tạo hồ sơ thầu
                var lead = context.Lead.FirstOrDefault(x => x.LeadId == parameter.LeadId && x.Active == true);
                if (lead == null)
                {
                    return new GetMasterDataCreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LEAD
                    };
                }
                // Lấy thông tin người phụ trách cơ hội
                var personLead = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == lead.PersonInChargeId);
                if (personLead == null)
                {
                    return new GetMasterDataCreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_PERSONINCHARGE
                    };
                }

                // Lấy thông tin khách hàng bên cơ hội
                var customerLead = listCustomerCommon.FirstOrDefault(x => x.CustomerId == lead.CustomerId && x.Active == true);
                var listOrganizationCommon = context.Organization.ToList();
                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();

                // Kiểm tra coi người đang đăng nhập có phải là quản lí của người phụ trách cơ hội hoặc người phụ trách cơ hội  hay không
                var org = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == personLead.OrganizationId);
                var listOrgParentId = new List<Guid?>();
                listOrgParentId.Add(org.OrganizationId);
                GetOrganizationParentId(org, listOrganizationCommon, listOrgParentId);

                var listEmployeeManager = listEmployeeCommon.Where(x =>
                    listOrgParentId.Contains(x.OrganizationId) && x.IsManager && x.Active == true).ToList();
                listEmployeeManager.Add(personLead);
                var isAddSaleBidding = listEmployeeManager.FirstOrDefault(x => x.EmployeeId == user.EmployeeId) == null;
                if (isAddSaleBidding)
                {
                    return new GetMasterDataCreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL_ROLE
                    };
                }
                // Check coi người đang nhập là quản lí hay là nhân viên
                if (employeeLogin.IsManager)
                {
                    #region Lấy danh sách người phụ trách hồ sơ thầu theo người đang đăng nhập

                    // Là quản lí đăng nhập thì lấy danh sách tất cả các nhân viên cấp dưới của nó
                    var organization = listOrganizationCommon.FirstOrDefault(x =>
                        x.OrganizationId == employeeLogin.OrganizationId && x.Active == true);
                    if (organization != null)
                    {
                        var listPersonCustomerId = new List<Guid>();
                        if (customerLead == null)
                        {
                            listPersonResult.Add(new EmployeeEntityModel()
                            {
                                EmployeeId = personLead.EmployeeId,
                                EmployeeName = personLead.EmployeeName,
                                EmployeeCode = personLead.EmployeeCode,
                                IsManager = personLead.IsManager,
                                Active = personLead.Active
                            });
                        }
                        else
                        {
                            // Lấy nhân viên phụ trách khách hàng ở cơ hội
                            var personCustomerLead =
                                listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == customerLead.PersonInChargeId);

                            if (customerLead.PersonInChargeId != null)
                            {
                                listPersonCustomerId.Add(personCustomerLead.EmployeeId);
                                // Lấy danh sách cấp dưới và người phụ trách khách hàng
                                if (personCustomerLead.IsManager)
                                {
                                    var listOrgPersonCustomerId = new List<Guid?>();
                                    _getOrganizationChildrenId(listOrganizationCommon,
                                        personCustomerLead.OrganizationId, listOrgPersonCustomerId);
                                    listOrgPersonCustomerId.Add(personCustomerLead.OrganizationId);
                                    var listPersonCustomer = listEmployeeCommon.Where(x =>
                                            x.Active == true && listOrgPersonCustomerId.Contains(x.OrganizationId))
                                        .ToList();
                                    listPersonCustomerId = listPersonCustomer.Select(x => x.EmployeeId).ToList();
                                }
                            }
                        }
                        listPersonCustomerId = listPersonCustomerId.Distinct().ToList();

                        var listOrganizationId = new List<Guid?>();
                        _getOrganizationChildrenId(listOrganizationCommon, organization.OrganizationId,
                            listOrganizationId);
                        listOrganizationId.Add(organization.OrganizationId);
                        listOrganizationId = listOrganizationId.Distinct().ToList();
                        var listEmployeeTemp = listEmployeeCommon
                            .Where(x => x.Active == true && listOrganizationId.Contains(x.OrganizationId)).ToList();
                        var listEmployeeTempId = listEmployeeTemp.Select(x => x.EmployeeId).ToList();
                        listEmployeeTempId.Add(personLead.EmployeeId);
                        listEmployeeTempId = listEmployeeTempId.Distinct().ToList();

                        listPersonResult = listEmployeeCommon.Where(x =>
                                x.EmployeeId != null && listEmployeeTempId.Contains(x.EmployeeId) &&
                                (listPersonCustomerId.Count == 0 || listPersonCustomerId.Contains(x.EmployeeId)))
                            .Select(y => new EmployeeEntityModel()
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeName = y.EmployeeName,
                                EmployeeCode = y.EmployeeCode,
                                IsManager = y.IsManager,
                                Active = y.Active
                            }).ToList();

                        // Danh sách nhân viên bán hàng hoặc nhân viên tham gia
                        listEmployeeResult = listEmployeeTemp.Select(y => new EmployeeEntityModel()
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeName = y.EmployeeName,
                            EmployeeCode = y.EmployeeCode,
                            IsManager = y.IsManager,
                            Active = y.Active
                        }).ToList();
                    }

                    #endregion
                }
                else
                {
                    #region Lấy danh sách người phụ trách hồ sơ thầu theo người đang đăng nhập

                    listPersonResult.Add(new EmployeeEntityModel()
                    {
                        EmployeeId = employeeLogin.EmployeeId,
                        EmployeeName = employeeLogin.EmployeeName,
                        EmployeeCode = employeeLogin.EmployeeCode,
                        IsManager = employeeLogin.IsManager,
                        Active = employeeLogin.Active
                    });
                    if (employeeLogin.EmployeeId != personLead.EmployeeId)
                    {
                        listPersonResult.Add(new EmployeeEntityModel()
                        {
                            EmployeeId = personLead.EmployeeId,
                            EmployeeName = personLead.EmployeeName,
                            EmployeeCode = personLead.EmployeeCode,
                            IsManager = personLead.IsManager,
                            Active = personLead.Active
                        });
                    }

                    listEmployeeResult.Add(new EmployeeEntityModel()
                    {
                        EmployeeId = employeeLogin.EmployeeId,
                        EmployeeName = employeeLogin.EmployeeName,
                        EmployeeCode = employeeLogin.EmployeeCode,
                        IsManager = employeeLogin.IsManager,
                        Active = employeeLogin.Active
                    });

                    #endregion
                }

                #region Lấy danh sách tất cả khách hàng do người đăng nhập phụ trách hoặc cấp dưới phụ trách

                var listPersonInChargeId = listEmployeeResult.Select(x => x.EmployeeId).ToList();

                var listCustomerResult = context.Customer
                    .Where(x => (x.Active == true && listPersonInChargeId.Contains(x.PersonInChargeId))).Select(y =>
                        new CustomerEntityModel()
                        {
                            CustomerId = y.CustomerId,
                            CustomerName = y.CustomerName,
                            CustomerCode = y.CustomerCode,
                            CustomerGroupId = y.CustomerGroupId,
                            PersonInChargeId = y.PersonInChargeId
                        }).ToList();

                if (customerLead != null)
                {
                    listCustomerResult.Add(new CustomerEntityModel()
                    {
                        CustomerId = customerLead.CustomerId,
                        CustomerName = customerLead.CustomerName,
                        CustomerCode = customerLead.CustomerCode,
                        CustomerGroupId = customerLead.CustomerGroupId,
                        PersonInChargeId = customerLead.PersonInChargeId
                    });
                    listCustomerResult = listCustomerResult.Distinct().ToList();
                }

                var listCustomerId = listCustomerResult.Select(x => x.CustomerId).ToList();

                var listEmployeeId = listCustomerResult.Select(x => x.PersonInChargeId).Distinct().ToList();

                var listEmployeeCustomer = listEmployeeCommon.Where(x => listEmployeeId.Contains(x.EmployeeId)).ToList();
                var listCustomerGroupId = listCustomerResult.Select(x => x.CustomerGroupId).Distinct().ToList();
                var listCustomerGroup = commonCategory.Where(x => listCustomerGroupId.Contains(x.CategoryId)).ToList();
                // Lấy danh sách contact
                var commonContact = context.Contact.Where(x => listCustomerId.Contains(x.ObjectId)).ToList();

                // Lấy danh sách xã
                var listWardId = commonContact.Select(x => x.WardId).ToList();
                var listWardCommon = context.Ward.Where(x => listWardId.Contains(x.WardId)).ToList();
                // Lấy danh sách phường
                var listDistrictId = commonContact.Select(x => x.DistrictId).ToList();
                var listDistrictCommon = context.District.Where(x => listDistrictId.Contains(x.DistrictId)).ToList();
                // Lấy danh sách tỉnh
                var listProvinceId = commonContact.Select(x => x.ProvinceId).ToList();
                var listProvinceCommon = context.Province.Where(x => listProvinceId.Contains(x.ProvinceId)).ToList();
                listCustomerResult.ForEach(item =>
                {
                    var temp = commonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS");
                    var ward = listWardCommon.FirstOrDefault(x => x.WardId == temp.WardId);
                    var district = listDistrictCommon.FirstOrDefault(x => x.DistrictId == temp.DistrictId);
                    var province = listProvinceCommon.FirstOrDefault(x => x.ProvinceId == temp.ProvinceId);
                    if (temp != null)
                    {
                        item.FullAddress = temp.Address;
                    }

                    if (ward != null)
                    {
                        item.FullAddress = item.FullAddress + "," + ward.WardName;
                    }

                    if (district != null)
                    {
                        item.FullAddress = item.FullAddress + "," + district.DistrictName;
                    }

                    if (province != null)
                    {
                        item.FullAddress = item.FullAddress + "," + province.ProvinceName;
                    }
                    item.CustomerPhone = temp?.Phone;
                    item.TaxCode = temp?.TaxCode;
                    item.CustomerGroup = listCustomerGroup.FirstOrDefault(x => x.CategoryId == item.CustomerGroupId)?.CategoryName;
                    item.PersonInCharge = listEmployeeCustomer.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId)?.EmployeeName;
                });

                #endregion

                #region Lấy thông tin cơ hội

                var contact = context.Contact.FirstOrDefault(x => x.ObjectId == lead.LeadId && x.ObjectType == "LEA");

                var leadResult = new LeadEntityModel()
                {
                    LeadId = lead.LeadId,
                    FullName = contact.FirstName,
                    PersonInChargeId = lead.PersonInChargeId,
                    PersonInChargeFullName = personLead.EmployeeName,
                    ContactId = contact.ContactId,
                    CustomerId = lead.CustomerId,
                    LeadCode = lead.LeadCode
                };

                #endregion

                #region Lấy danh sách chi tiết cơ hội

                var listDetailLead = new List<LeadDetailModel>();
                var commonLeadDetail = context.LeadDetail.Where(w => w.LeadId == leadResult.LeadId).ToList();

                var listDetailId = commonLeadDetail.Select(w => w.LeadDetailId).ToList();
                var listVendorId = commonLeadDetail.Select(w => w.VendorId).ToList();
                var listProductId = commonLeadDetail.Select(w => w.ProductId).ToList();

                var listVendorEntity = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                var listProductEntity = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();

                // Lấy loại tiền
                var leadMoneyTypeId =
                    commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI").CategoryTypeId;
                var unitTypeId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH").CategoryTypeId;

                var moneyList = context.Category.Where(w => w.CategoryTypeId == leadMoneyTypeId).Select(w =>
                    new CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var unitList = context.Category.Where(w => w.CategoryTypeId == unitTypeId).Select(w =>
                    new CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                var commonLeadProductDetailProductAttributeValue =
                    context.LeadProductDetailProductAttributeValue.ToList();

                var leadProductDetailProductAttributeValueEntity = commonLeadProductDetailProductAttributeValue
                    .Where(w => listDetailId.Contains(w.LeadDetailId.Value)).ToList();

                commonLeadDetail.ForEach(leadDetail =>
                {
                    var listAttribute = new List<LeadProductDetailProductAttributeValueModel>();
                    var listAttributeEntity = leadProductDetailProductAttributeValueEntity
                        .Where(w => w.LeadDetailId == leadDetail.LeadDetailId).ToList();
                    listAttributeEntity.ForEach(attri =>
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

                    var nameMoneyUnit = moneyList.FirstOrDefault(f => f.CategoryId == leadDetail.CurrencyUnit)
                                            ?.CategoryName ?? "";
                    var nameVendor =
                        listVendorEntity.FirstOrDefault(f => f.VendorId == leadDetail.VendorId)?.VendorName ?? "";
                    var productCode = listProductEntity.FirstOrDefault(f => f.ProductId == leadDetail.ProductId)
                                          ?.ProductCode ?? "";
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
                        SumAmount = SumAmount(leadDetail.Quantity, leadDetail.UnitPrice, leadDetail.ExchangeRate,
                            leadDetail.Vat, leadDetail.DiscountValue,
                            leadDetail.DiscountType),
                        OrderNumber = leadDetail.OrderNumber
                    });
                });

                #endregion

                #region Lấy danh sách sản phẩm

                var listProductResult = context.Product.Where(z => z.Active == true).Select(x => new ProductEntityModel()
                {
                    ProductId = x.ProductId,
                    ProductCategoryId = x.ProductCategoryId,
                    ProductName = x.ProductName,
                    ProductCode = x.ProductCode,
                    Price1 = x.Price1,
                    Price2 = x.Price2,
                    Quantity = x.Quantity,
                    ProductUnitId = x.ProductUnitId,
                    ProductUnitName = x.ProductName,
                    ProductDescription = x.ProductDescription,
                    Vat = x.Vat,
                    MinimumInventoryQuantity = x.MinimumInventoryQuantity,
                    ProductMoneyUnitId = x.ProductMoneyUnitId,
                    ExWarehousePrice = x.ExWarehousePrice,
                }).ToList();

                #endregion

                listEmployeeResult = listEmployeeCommon.Where(x => x.Active == true).Select(y =>
                    new EmployeeEntityModel()
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeName = y.EmployeeName,
                        EmployeeCode = y.EmployeeCode,
                        IsManager = y.IsManager,
                        Active = y.Active
                    }).ToList();

                // Lấy loại hợp đồng
                var typeCONTRACT = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CONTRACT")
                    ?.CategoryTypeId;
                var contactResult = context.Category.Where(c => c.CategoryTypeId == typeCONTRACT && c.Active == true)
                    .Select(x => new CategoryEntityModel()
                    {
                        CategoryId = x.CategoryId,
                        CategoryName = x.CategoryName,
                        CategoryCode = x.CategoryCode
                    }).ToList();

                return new GetMasterDataCreateSaleBiddingResult()
                {
                    Status = true,
                    ListCustomer = listCustomerResult,
                    ListPerson = listPersonResult,
                    Lead = leadResult,
                    Employee = new EmployeeEntityModel()
                    {
                        EmployeeId = personLead.EmployeeId,
                        EmployeeName = personLead.EmployeeName,
                        EmployeeCode = personLead.EmployeeCode,
                        IsManager = personLead.IsManager,
                        Active = personLead.Active
                    },
                    ListLeadDetail = listDetailLead,
                    ListMoneyUnit = moneyList,
                    ListProduct = listProductResult,
                    ListEmployee = listEmployeeResult,
                    ListTypeContact = contactResult
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateSaleBiddingResult()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetMasterDataSaleBiddingDashboardResult GetMasterDataSaleBiddingDashBoard(GetMasterDataSaleBiddingDashboardParameter parameter)
        {
            try
            {
                var listCategoryCommon = context.Category.ToList();
                var typeStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST")?.CategoryTypeId;
                var listStatus = listCategoryCommon.Where(c => c.CategoryTypeId == typeStatusId).ToList();
                var statusNew = listStatus.FirstOrDefault(x => x.CategoryCode == "NEW");
                var statusCancel = listStatus.FirstOrDefault(x => x.CategoryCode == "CANC");
                var statusWin = listStatus.FirstOrDefault(x => x.CategoryCode == "WIN");
                var statusApprove = listStatus.FirstOrDefault(x => x.CategoryCode == "APPR");
                var statusLose = listStatus.FirstOrDefault(x => x.CategoryCode == "LOSE");
                var statusWaitApprove = listStatus.FirstOrDefault(x => x.CategoryCode == "CHO");
                var statusReject = listStatus.FirstOrDefault(x => x.CategoryCode == "REFU");
                var listSaleBidding = context.SaleBidding.ToList();
                var listCostQuoteCommon = context.CostsQuote.ToList();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetMasterDataSaleBiddingDashboardResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_FAIL
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetMasterDataSaleBiddingDashboardResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_FAIL
                    };
                }
                var listSaleBiddingModel = new List<SaleBiddingEntityModel>();
                var listAllUser = context.User.Select(w => new { w.EmployeeId, w.UserId }).ToList();
                var listEmployeeCommon = context.Employee.ToList();
                var employeeId = user.EmployeeId;
                var employee = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (employee == null)
                {
                    return new GetMasterDataSaleBiddingDashboardResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_FAIL
                    };
                }
                var typeCONTRACT = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CONTRACT")?.CategoryTypeId;
                // Lấy loại hợp đồng
                var contact = listCategoryCommon.Where(c => c.CategoryTypeId == typeCONTRACT && c.Active == true).Select(x => new CategoryEntityModel()
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    CategoryCode = x.CategoryCode
                }).ToList();
                var listEmployee = new List<Employee>();
                if (employee.IsManager)
                {
                    var org = context.Organization.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId);
                    if (org == null)
                    {
                        return new GetMasterDataSaleBiddingDashboardResult
                        {
                            Status = false,
                            Message = CommonMessage.SaleBidding.GET_FAIL
                        };
                    }
                    var listOrganizationCommon = context.Organization.ToList();
                    var listOrgan = new List<Guid?>();
                    listOrgan.Add(org.OrganizationId);
                    _getOrganizationChildrenId(listOrganizationCommon, org.OrganizationId, listOrgan);
                    listEmployee = listEmployeeCommon.Where(x => listOrgan.Contains(x.OrganizationId)).ToList();
                }
                else
                {
                    listEmployee.Add(employee);
                }
                var listEmployeeId = listEmployee.Select(x => x.EmployeeId).ToList();
                var listSaleBiddingEmployeeJoinId = context.SaleBiddingEmployeeMapping.Where(x => listEmployeeId.Contains(x.EmployeeId.Value)).Select(x => x.SaleBiddingId).ToList();

                #region Danh sách hồ sơ thầu chờ phê duyệt

                var statusApprovalId = listStatus.FirstOrDefault(c => c.CategoryCode == "CHO").CategoryId;
                var listSaleBiddingStatusApproval = listSaleBidding.Where(c => (listSaleBiddingEmployeeJoinId.Contains(c.SaleBiddingId) || listEmployeeId.Contains(c.PersonInChargeId)) && c.StatusId == statusApprovalId &&
                    (statusCancel == null || statusCancel.CategoryId != c.StatusId) &&
                    (statusReject == null || statusReject.CategoryId != c.StatusId) &&
                    (string.IsNullOrEmpty(parameter.SaleBiddingName) || c.SaleBiddingName.Contains(parameter.SaleBiddingName)))
                            .Select(m => new SaleBiddingEntityModel
                            {
                                SaleBiddingId = m.SaleBiddingId,
                                SaleBiddingName = m.SaleBiddingName,
                                SaleBiddingCode = m.SaleBiddingCode,
                                StartDate = m.StartDate,
                                ValueBid = m.ValueBid,
                                StatusId = m.StatusId,
                                StatusName = listStatus.FirstOrDefault(c => c.CategoryId == statusApprovalId)?.CategoryName ?? "",
                                TypeContractId = m.TypeContractId,
                                TypeContractName = contact.FirstOrDefault(t => t.CategoryId == m.TypeContractId)?.CategoryName,
                                Ros = GetRos(m.SaleBiddingId, listCostQuoteCommon)
                            }).OrderByDescending(z => z.StartDate)
                            .ToList();
                #endregion Danh sách hồ sơ thầu chờ phê duyệt

                #region Danh sách hồ sơ thầu hết hạn hiệu lực
                var listSaleBiddingExpired = listSaleBidding.Where(c => (listSaleBiddingEmployeeJoinId.Contains(c.SaleBiddingId) || listEmployeeId.Contains(c.PersonInChargeId)) && (c.BidStartDate.Value.AddDays(c.EffecTime).Date < parameter.EffectiveDate.Value.Date)
                            && (statusCancel.CategoryId != c.StatusId) &&
                               (statusWin.CategoryId != c.StatusId) &&
                               (statusLose.CategoryId != c.StatusId) &&
                            (string.IsNullOrEmpty(parameter.SaleBiddingName) || c.SaleBiddingName.Contains(parameter.SaleBiddingName)))
                            .Select(c => new SaleBiddingEntityModel
                            {
                                SaleBiddingId = c.SaleBiddingId,
                                SaleBiddingName = c.SaleBiddingName,
                                SaleBiddingCode = c.SaleBiddingCode,
                                StartDate = c.StartDate,
                                BidStartDate = c.BidStartDate,
                                ValueBid = c.ValueBid,
                                StatusId = c.StatusId,
                                StatusName = listStatus.FirstOrDefault(m => m.CategoryId == c.StatusId)?.CategoryName ?? "",
                                TypeContractId = c.TypeContractId,
                                TypeContractName = contact.FirstOrDefault(t => t.CategoryId == c.TypeContractId)?.CategoryName,
                                EffecTime = c.EffecTime,
                                Ros = GetRos(c.SaleBiddingId, listCostQuoteCommon)
                            }).OrderByDescending(t => t.BidStartDate).ToList();


                #endregion Danh sách hồ sơ thầu hết hạn hiệu lực

                #region Danh sách hồ sơ thầu chậm so với ngày mở thầu

                var listSaleBiddingSlowStartDate = listSaleBidding.Where(c => (listSaleBiddingEmployeeJoinId.Contains(c.SaleBiddingId) || listEmployeeId.Contains(c.PersonInChargeId))
                            && c.BidStartDate.Value.Date > c.StartDate.Date &&
                            (statusCancel == null || statusCancel.CategoryId != c.StatusId) &&
                            (statusLose == null || statusLose.CategoryId != c.StatusId) &&
                            (statusWin == null || statusWin.CategoryId != c.StatusId) &&
                            (string.IsNullOrEmpty(parameter.SaleBiddingName) || c.SaleBiddingName.Contains(parameter.SaleBiddingName)))
                             .Select(m => new SaleBiddingEntityModel
                             {
                                 SaleBiddingId = m.SaleBiddingId,
                                 SaleBiddingName = m.SaleBiddingName,
                                 SaleBiddingCode = m.SaleBiddingCode,
                                 StartDate = m.StartDate,
                                 BidStartDate = m.BidStartDate,
                                 ValueBid = m.ValueBid,
                                 StatusId = m.StatusId,
                                 StatusName = listStatus.FirstOrDefault(c => c.CategoryId == m.StatusId)?.CategoryName ?? "",
                                 TypeContractId = m.TypeContractId,
                                 TypeContractName = contact.FirstOrDefault(t => t.CategoryId == m.TypeContractId)?.CategoryName,
                                 SlowDay = (m.BidStartDate.Value.Date - m.StartDate.Date).Days,
                                 Ros = GetRos(m.SaleBiddingId, listCostQuoteCommon)
                             }).OrderByDescending(y => y.BidStartDate).ToList();

                #endregion 

                #region List nộp thầu phải nộp trong tuần này
                // Tính tuần cần nộp thầu 
                DateTime date = DateTime.Now.AddDays(7);
                while (date.DayOfWeek != DayOfWeek.Monday)
                {
                    date = date.AddDays(-1);
                }
                DateTime startLastWeek = date;
                DateTime endLastWeek = date.AddDays(6);
                // Lấy ra những hồ sơ thầu thuộc tuần cần nộp
                var listSaleBiddingInWeek = listSaleBidding.Where(c => (listSaleBiddingEmployeeJoinId.Contains(c.SaleBiddingId) || listEmployeeId.Contains(c.PersonInChargeId)) && c.StartDate.Date >= startLastWeek.Date && c.StartDate.Date <= endLastWeek.Date &&
                            statusCancel.CategoryId != c.StatusId &&
                            statusLose.CategoryId != c.StatusId &&
                            statusWin.CategoryId != c.StatusId &&
                            (string.IsNullOrEmpty(parameter.SaleBiddingName) || c.SaleBiddingName.Contains(parameter.SaleBiddingName)))
                            .Select(m => new SaleBiddingEntityModel
                            {
                                SaleBiddingId = m.SaleBiddingId,
                                SaleBiddingName = m.SaleBiddingName,
                                SaleBiddingCode = m.SaleBiddingCode,
                                StartDate = m.StartDate,
                                BidStartDate = m.BidStartDate,
                                ValueBid = m.ValueBid,
                                StatusId = m.StatusId,
                                StatusName = listStatus.FirstOrDefault(c => c.CategoryId == m.StatusId)?.CategoryName ?? "",
                                TypeContractId = m.TypeContractId,
                                TypeContractName = contact.FirstOrDefault(t => t.CategoryId == m.TypeContractId)?.CategoryName,
                                Ros = GetRos(m.SaleBiddingId, listCostQuoteCommon)
                            }).OrderBy(z => z.StartDate).ToList();
                #endregion

                #region Lấy danh sách hồ sơ thầu của biểu đồ

                // Lấy trạng thái hồ sơ thầu 
                var listSaleBiddingChart = listSaleBidding.Where(x => (listSaleBiddingEmployeeJoinId.Contains(x.SaleBiddingId) || listEmployeeId.Contains(x.PersonInChargeId))
                                            && x.StatusId != statusCancel.CategoryId &&
                                           x.StatusId != statusReject.CategoryId &&
                                           (x.StatusId == statusWin.CategoryId ||
                                           x.StatusId == statusLose.CategoryId) &&
                                           (parameter.FromDate == null || x.BidStartDate.Value.Date >= parameter.FromDate) &&
                                           (parameter.ToDate == null || x.BidStartDate.Value.Date <= parameter.ToDate)
                                           ).Select(m => new SaleBiddingEntityModel
                                           {
                                               SaleBiddingId = m.SaleBiddingId,
                                               SaleBiddingName = m.SaleBiddingName,
                                               SaleBiddingCode = m.SaleBiddingCode,
                                               StartDate = m.StartDate,
                                               BidStartDate = m.BidStartDate,
                                               ValueBid = m.ValueBid,
                                               TypeContractId = m.TypeContractId,
                                               StatusId = m.StatusId
                                           }).ToList();

                #endregion

                var listStatusResult = listStatus.Select(x => new CategoryEntityModel()
                {
                    CategoryCode = x.CategoryCode,
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                }).ToList();

                return new GetMasterDataSaleBiddingDashboardResult
                {
                    Status = true,
                    Message = "Success",
                    ListSaleBiddingWaitApproval = listSaleBiddingStatusApproval,
                    ListSaleBiddingExpired = listSaleBiddingExpired,
                    ListSaleBiddingSlowStartDate = listSaleBiddingSlowStartDate,
                    ListSaleBiddingInWeek = listSaleBiddingInWeek,
                    ListTypeContact = contact,
                    ListSaleBiddingChart = listSaleBiddingChart,
                    ListStatus = listStatusResult
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSaleBiddingDashboardResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetMasterDataSearchSaleBiddingResult GetMasterDataSearchSaleBidding(GetMasterDataSearchSaleBiddingParamter paramter)
        {
            #region Lấy dữ liệu category
            var LEADTYPE_CODE = "LHL"; //loai khach hang tiem nang
            var STATUS_CODE = "HST"; // 
            #endregion
            var user = context.User.FirstOrDefault(x => x.UserId == paramter.UserId && x.Active == true);
            if (user == null)
            {
                return new GetMasterDataSearchSaleBiddingResult
                {
                    Status = false,
                    Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                };
            }
            if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
            {
                return new GetMasterDataSearchSaleBiddingResult
                {
                    Status = false,
                    Message = "Lỗi dữ liệu"
                };
            }

            var listCategoryTypeCommon = context.CategoryType.ToList();
            var listCategoryCommon = context.Category.ToList();
            // Lấy danh sách trạng thái hồ sơ thầu
            var typeStatusId = listCategoryTypeCommon.FirstOrDefault(c => c.CategoryTypeCode == STATUS_CODE)?.CategoryTypeId;
            var listStatus = listCategoryCommon.Where(c => c.CategoryTypeId == typeStatusId)
                .Select(m => new CategoryEntityModel
                {
                    CategoryId = m.CategoryId,
                    CategoryName = m.CategoryName,
                    CategoryCode = m.CategoryCode,
                    CategoryTypeId = m.CategoryTypeId,
                    IsDefault = m.IsDefauld,
                    Active = m.Active
                })
                .ToList();

            // Lấy danh sách loại khách hàng
            var typeLeadTypeId = listCategoryTypeCommon.FirstOrDefault(c => c.CategoryTypeCode == LEADTYPE_CODE)?.CategoryTypeId;
            var listLeadType = listCategoryCommon.Where(c => c.CategoryTypeId == typeLeadTypeId)
                .Select(m => new CategoryEntityModel
                {
                    CategoryId = m.CategoryId,
                    CategoryName = m.CategoryName,
                    CategoryCode = m.CategoryCode,
                    CategoryTypeId = m.CategoryTypeId,
                    IsDefault = m.IsDefauld,
                    Active = m.Active
                })
                .ToList();

            // Lấy loại hợp đồng
            var typeCONTRACT = listCategoryTypeCommon.FirstOrDefault(c => c.CategoryTypeCode == "CONTRACT")?.CategoryTypeId;
            var typeContact = listCategoryCommon.Where(c => c.CategoryTypeId == typeCONTRACT && c.Active == true).Select(x => new CategoryEntityModel()
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                CategoryCode = x.CategoryCode
            }).ToList();

            var _employeeId = context.User.FirstOrDefault(w => w.UserId == paramter.UserId).EmployeeId;
            var listEmployeeEntity = context.Employee.Where(w => w.Active == true).ToList();
            var listPersonalInChange = new List<EmployeeEntityModel>();

            var employeeById = listEmployeeEntity.FirstOrDefault(w => w.EmployeeId == _employeeId);
            //check Is Manager
            var isManage = employeeById.IsManager;
            if (isManage)
            {
                //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                var currentOrganization = employeeById.OrganizationId;
                List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                listOrganizationChildrenId.Add(currentOrganization);
                var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                _getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                var listEmployeeFiltered = listEmployeeEntity.Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new EmployeeEntityModel
                {
                    EmployeeId = w.EmployeeId,
                    EmployeeName = w.EmployeeName,
                    EmployeeCode = w.EmployeeCode,
                }).ToList();

                listEmployeeFiltered?.ForEach(emp =>
                {
                    listPersonalInChange.Add(new EmployeeEntityModel
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

            return new GetMasterDataSearchSaleBiddingResult
            {
                Status = true,
                Message = "Success",
                ListLeadType = listLeadType,
                ListPersonalInChange = listPersonalInChange,
                ListStatus = listStatus,
                ListContract = typeContact
            };
        }

        public SearchSaleBiddingResult SearchSaleBidding(SearchSaleBiddingParameter parameter)
        {

            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
            if (user == null)
            {
                return new SearchSaleBiddingResult
                {
                    Status = false,
                    Message = CommonMessage.SaleBidding.GET_FAIL
                };
            }
            if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
            {
                return new SearchSaleBiddingResult
                {
                    Status = false,
                    Message = CommonMessage.SaleBidding.GET_FAIL
                };
            }
            var listSaleBiddingModel = new List<SaleBiddingEntityModel>();
            var listAllUser = context.User.Select(w => new { w.EmployeeId, w.UserId }).ToList();
            var listEmployeeCommon = context.Employee.ToList();
            var typeStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "HST")?.CategoryTypeId;
            var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusId).ToList();
            var employeeId = user.EmployeeId;
            var employee = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == employeeId);
            if (employee == null)
            {
                return new SearchSaleBiddingResult
                {
                    Status = false,
                    Message = CommonMessage.SaleBidding.GET_FAIL
                };
            }
            var listEmployee = new List<Employee>();
            if (employee.IsManager)
            {
                var org = context.Organization.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId);
                if (org == null)
                {
                    return new SearchSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_FAIL
                    };
                }
                var listOrganizationCommon = context.Organization.ToList();
                var listOrgan = new List<Guid?>();
                listOrgan.Add(org.OrganizationId);
                _getOrganizationChildrenId(listOrganizationCommon, org.OrganizationId, listOrgan);
                listEmployee = listEmployeeCommon.Where(x => listOrgan.Contains(x.OrganizationId)).ToList();
            }
            else
            {
                listEmployee.Add(employee);
            }
            var listEmployeeId = listEmployee.Select(x => x.EmployeeId).ToList();
            var listSaleBiddingEmployeeJoinId = context.SaleBiddingEmployeeMapping.Where(x => listEmployeeId.Contains(x.EmployeeId.Value)).Select(x => x.SaleBiddingId).ToList();


            var listSaleBidding = context.SaleBidding.Where(x => (listSaleBiddingEmployeeJoinId.Contains(x.SaleBiddingId) || listEmployeeId.Contains(x.PersonInChargeId)) && (
                                  (parameter.ToDate == null || parameter.ToDate.Value.Date >= x.BidStartDate.Value.Date) &&
                                  (parameter.FromDate == null || parameter.FromDate.Value.Date <= x.BidStartDate.Value.Date) &&
                                  (parameter.SaleBiddingName == null || parameter.SaleBiddingName.Trim().Length == 0 || x.SaleBiddingName.ToLower().Contains(parameter.SaleBiddingName.ToLower())) &&
                                  (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(x.StatusId.Value)) &&
                                 (parameter.ListContractId == null || parameter.ListContractId.Count == 0 || parameter.ListContractId.Contains(x.TypeContractId.Value)) &&
                                  (parameter.ListPersonalInChangeId == null || parameter.ListPersonalInChangeId.Count == 0 || parameter.ListPersonalInChangeId.Contains(x.PersonInChargeId))
                                  )).ToList();
            // Lấy danh sách cơ hội theo điều kiện tìm
            var listCustomerType = context.Category.Where(x => parameter.ListTypeCustomer != null && parameter.ListTypeCustomer.Contains(x.CategoryId)).ToList();
            List<int> typeCustomer = new List<int>();
            if (listCustomerType.Count > 0)
            {
                var isKPL = listCustomerType.FirstOrDefault(x => x.CategoryCode == "KPL") != null;
                if (isKPL)
                {
                    typeCustomer.Add(2);
                }
                var isKCL = listCustomerType.FirstOrDefault(x => x.CategoryCode == "KCL") != null;
                if (isKCL)
                {
                    typeCustomer.Add(1);
                }
            }
            // Lấy danh sách khách hàng theo điều kiện tìm kiếm
            var listCustomerId = listSaleBidding.Select(x => x.CustomerId).ToList();
            var listCustomer = context.Customer.Where(x => listCustomerId.Contains(x.CustomerId) && (typeCustomer.Count == 0 || typeCustomer.Contains(x.CustomerType.Value)) &&
                                        (parameter.CusName == null || parameter.CusName.Trim().Length == 0 || x.CustomerName.ToLower().Contains(parameter.CusName.ToLower()))).ToList();
            var listCustomerIdSearch = listCustomer.Select(x => x.CustomerId).ToList();
            var listContact = context.Contact.Where(x => x.ObjectType == "CUS"
                                                    && (parameter.Email == null || parameter.Email.Trim().Length == 0 || x.Email.ToLower().Contains(parameter.Email.ToLower()))
                                                    && (parameter.Phone == null || parameter.Phone.Trim().Length == 0 || x.Phone.ToLower().Contains(parameter.Phone.ToLower()))
                                                    ).ToList();
            var listObjectId = listContact.Select(x => x.ObjectId).ToList();
            var iss = listCustomerIdSearch.Contains(Guid.NewGuid());
            listSaleBiddingModel = listSaleBidding.Where(x => (parameter.Email == null || listObjectId.Contains(x.CustomerId))
                                                                && (parameter.Phone == null || listObjectId.Contains(x.CustomerId)) &&
                                                        (parameter.CusName == null || parameter.CusName.Trim().Length == 0 || listCustomerIdSearch.Contains(x.CustomerId))
            ).Select(y => new SaleBiddingEntityModel()
            {
                SaleBiddingId = y.SaleBiddingId,
                SaleBiddingName = y.SaleBiddingName,
                SaleBiddingCode = y.SaleBiddingCode,
                StatusId = y.StatusId,
                StatusName = listStatus.FirstOrDefault(c => c.CategoryId == y.StatusId)?.CategoryName ?? "",
                EffecTime = y.EffecTime,
                Phone = listContact.FirstOrDefault(z => z.ObjectId == y.CustomerId && z.ObjectType == "CUS")?.Phone ?? "",
                Email = listContact.FirstOrDefault(z => z.ObjectId == y.CustomerId && z.ObjectType == "CUS")?.Email ?? "",
                PersonInChargeName = listEmployeeCommon.FirstOrDefault(c => c.EmployeeId == y.PersonInChargeId)?.EmployeeName ?? "",
                PersonInChargeId = y.PersonInChargeId,
                ValueBid = y.ValueBid,
                CustomerId = y.CustomerId,
                CustomerName = listCustomer.FirstOrDefault(c => c.CustomerId == y.CustomerId)?.CustomerName ?? "",
                CreateDate = y.CreatedDate,
                BidStartDate = y.BidStartDate,
                StartDate = y.StartDate
            }).OrderByDescending(b => b.CreateDate).ToList();


            return new SearchSaleBiddingResult
            {
                Status = true,
                Message = "Success",
                ListSaleBidding = listSaleBiddingModel
            };
        }

        public CreateSaleBiddingResult CreateSaleBidding(CreateSaleBiddingParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL
                    };
                }
                //kiểm tra ngày có kết quả dự kiên scos lơn hơn hoặc bằng ngày mở thầu không
                var startDate = parameter.SaleBidding.BidStartDate.Value;
                var endDate = parameter.SaleBidding.EndDate;
                if (endDate?.Date < startDate.Date)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.END_DATE_FAIL
                    };
                }

                // Lấy cơ hội 
                var lead = context.Lead.FirstOrDefault(x => x.LeadId == parameter.SaleBidding.LeadId && x.Active == true);
                // Kiểm tra coi còn hoạt động không 
                if (lead == null)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL_LEAD_NOT_EXIT
                    };
                }
                // Check coi cơ hội đã tạo báo giá chưa và check coi cơ hội đã tạo hồ sơ thầu chưa. Check coi cơ hội có ở trạng thái xác nhận k
                var statusLeadType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "CHS");
                var statusLead = context.Category.FirstOrDefault(x => x.CategoryTypeId == statusLeadType.CategoryTypeId && x.CategoryCode == "APPR").CategoryId;

                var isQuote = context.Quote.FirstOrDefault(x => x.LeadId == lead.LeadId) != null;
                // Check coi cơ hội đã tạo hồ sơ thầu chưa
                var isCreate = context.SaleBidding.FirstOrDefault(x => x.LeadId == parameter.SaleBidding.LeadId) != null;
                if (isQuote)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL_QUOTE_EXIT
                    };
                }
                //if (isCreate)
                //{
                //    return new CreateSaleBiddingResult
                //    {
                //        Status = false,
                //        Message = CommonMessage.SaleBidding.CREATE_FAIL_SALEBIDDING_EXIT
                //    };
                //}
                if (lead.StatusId != statusLead)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL_STATUS
                    };
                }
                // Tìm người phụ trách của cơ hội
                var listEmployeeCommon = context.Employee.ToList();
                var employee = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == lead.PersonInChargeId);
                if (employee == null)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL
                    };
                }
                var listOrganizationCommon = context.Organization.ToList();
                // Lấy phòng ban của người phụ trách cơ hội
                var organization = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId);
                if (organization == null)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL_ROLE
                    };
                }
                // Lấy danh sách tất cả phòng ban quản lí phòng bạn người phụ trách hiện tại
                var listOrganization = new List<Guid?>();
                listOrganization.Add(organization.OrganizationId);
                listOrganization = GetOrganizationParentId(organization, listOrganizationCommon, listOrganization);
                if (listOrganization == null || listOrganization.Count == 0)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL_ROLE
                    };
                }
                listOrganization.Add(organization.OrganizationId);
                // Lấy tất cả nhân viên quản lí của người phụ trách cơ hội
                var listEmployee = listEmployeeCommon.Where(x => listOrganization.Contains(x.OrganizationId) && x.IsManager == true && x.Active == true).ToList();
                listEmployee.Add(employee);
                // Check coi người tạo có phải là người người lí người phụ trách cơ hội hay cơ hội hay k
                var employeeCheck = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (employeeCheck == null)
                {
                    return new CreateSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.CREATE_FAIL_ROLE
                    };
                }

                var saleBiddingTemp = context.SaleBidding.Where(z => z.CreatedDate.Date == DateTime.Now.Date).OrderByDescending(x => x.CreatedDate).ToList().FirstOrDefault();
                var saleBiddingCode = "";
                var year = DateTime.Now.Year.ToString().Substring(2);
                var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
                if (saleBiddingTemp == null)
                {
                    saleBiddingCode = "HST-" + year + month + day + "0001";
                }
                else
                {
                    var code = saleBiddingTemp.SaleBiddingCode.Substring(saleBiddingTemp.SaleBiddingCode.Length - 4);
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

                    saleBiddingCode = "HST-" + year + month + day + identity;
                }

                var statusSaleBiddingTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HST").CategoryTypeId;
                var statusSaleBiddingId = context.Category.FirstOrDefault(x => x.CategoryCode == "NEW" && x.CategoryTypeId == statusSaleBiddingTypeId).CategoryId;
                var saleBidding = new SaleBidding();
                saleBidding.SaleBiddingId = Guid.NewGuid();
                saleBidding.SaleBiddingName = parameter.SaleBidding.SaleBiddingName;
                saleBidding.SaleBiddingCode = saleBiddingCode;
                saleBidding.StartDate = parameter.SaleBidding.StartDate;
                saleBidding.EffecTime = parameter.SaleBidding.EffecTime;
                saleBidding.EndDate = parameter.SaleBidding.EndDate;
                saleBidding.BidStartDate = parameter.SaleBidding.BidStartDate;
                saleBidding.StatusId = statusSaleBiddingId;
                saleBidding.LeadId = parameter.SaleBidding.LeadId;
                saleBidding.Note = parameter.SaleBidding.Note;
                saleBidding.PersonInChargeId = parameter.SaleBidding.PersonInChargeId;
                saleBidding.TypeContractId = parameter.SaleBidding.TypeContractId;
                saleBidding.ValueBid = parameter.SaleBidding.ValueBid;
                saleBidding.Address = parameter.SaleBidding.Address;
                saleBidding.CurrencyUnitId = parameter.SaleBidding.CurrencyUnitId;
                saleBidding.CustomerId = parameter.SaleBidding.CustomerId;
                saleBidding.FormOfBid = parameter.SaleBidding.FormOfBid;
                saleBidding.CreatedById = parameter.UserId;
                saleBidding.CreatedDate = DateTime.Now;
                saleBidding.EmployeeId = parameter.SaleBidding.EmployeeId;

                context.SaleBidding.Add(saleBidding);
                var folder = context.Folder.FirstOrDefault(x => x.Active == true && x.FolderType == "HST");
                if (folder == null)
                {
                    return new CreateSaleBiddingResult()
                    {
                        Status = false,
                        Message = "Chưa có thư mục để lưu. Bạn phải cấu hình thư mục để lưu"
                    };
                }
                parameter.ListSaleBiddingDetail.ForEach(item =>
                {
                    var saleBiddingDetail = new SaleBiddingDetail();
                    saleBiddingDetail.Category = item.Category;
                    saleBiddingDetail.Content = item.Content;
                    saleBiddingDetail.CreatedById = parameter.UserId;
                    saleBiddingDetail.CreatedDate = DateTime.Now;
                    saleBiddingDetail.SaleBiddingDetailId = Guid.NewGuid();
                    saleBiddingDetail.SaleBiddingId = saleBidding.SaleBiddingId;

                    if (item.ListFormFile != null && item.ListFormFile.Count > 0)
                    {
                        var folderName = folder.Url + "\\";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (IFormFile file in item.ListFormFile)
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
                                fileInForder.FileName = fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + Guid.NewGuid();
                                fileInForder.FolderId = folder.FolderId;
                                fileInForder.ObjectId = saleBiddingDetail.SaleBiddingDetailId;
                                fileInForder.ObjectType = "HST";
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
                    context.SaleBiddingDetail.Add(saleBiddingDetail);
                });

                parameter.ListCost.ForEach(item =>
                {
                    var costQuote = new CostsQuote();
                    costQuote.CostsQuoteId = Guid.NewGuid();
                    costQuote.CostsQuoteType = 1;
                    costQuote.CreatedById = parameter.UserId;
                    costQuote.CreatedDate = DateTime.Now;
                    costQuote.CurrencyUnit = item.CurrencyUnit;
                    costQuote.Description = item.Description;
                    costQuote.DiscountType = item.DiscountType;
                    costQuote.DiscountValue = item.DiscountValue;
                    costQuote.ExchangeRate = item.ExchangeRate;
                    costQuote.IncurredUnit = item.IncurredUnit;
                    costQuote.ProductId = item.ProductId;
                    costQuote.Quantity = item.Quantity;
                    costQuote.SaleBiddingId = saleBidding.SaleBiddingId;
                    costQuote.UnitId = item.UnitId;
                    costQuote.UnitPrice = item.UnitPrice;
                    costQuote.Vat = item.Vat;
                    costQuote.VendorId = item.VendorId;
                    costQuote.ProductName = item.ProductName;
                    costQuote.OrderDetailType = item.OrderDetailType;
                    item.LeadProductDetailProductAttributeValue.ForEach(map =>
                    {
                        var mappingProduct = new SaleBiddingDetailProductAttribute();
                        mappingProduct.ProductAttributeCategoryId = map.ProductAttributeCategoryId;
                        mappingProduct.ProductAttributeCategoryValueId = map.ProductAttributeCategoryValueId;
                        mappingProduct.ProductId = item.ProductId;
                        mappingProduct.SaleBiddingDetailId = costQuote.CostsQuoteId;
                        mappingProduct.SaleBiddingDetailProductAttributeId = Guid.NewGuid();

                        context.SaleBiddingDetailProductAttribute.Add(mappingProduct);
                    });

                    context.CostsQuote.Add(costQuote);
                });

                parameter.ListQuocte.ForEach(item =>
                {
                    var costQuote = new CostsQuote();
                    costQuote.CostsQuoteId = Guid.NewGuid();
                    costQuote.CostsQuoteType = 2;
                    costQuote.CreatedById = parameter.UserId;
                    costQuote.CreatedDate = DateTime.Now;
                    costQuote.CurrencyUnit = item.CurrencyUnit;
                    costQuote.Description = item.Description;
                    costQuote.DiscountType = item.DiscountType;
                    costQuote.DiscountValue = item.DiscountValue;
                    costQuote.ExchangeRate = 1;
                    costQuote.IncurredUnit = item.IncurredUnit;
                    costQuote.ProductId = item.ProductId;
                    costQuote.Quantity = item.Quantity;
                    costQuote.SaleBiddingId = saleBidding.SaleBiddingId;
                    costQuote.UnitId = item.UnitId;
                    costQuote.UnitPrice = item.UnitPrice;
                    costQuote.Vat = item.Vat;
                    costQuote.VendorId = item.VendorId;
                    costQuote.ProductName = item.ProductName;
                    costQuote.OrderDetailType = item.OrderDetailType;

                    item.LeadProductDetailProductAttributeValue.ForEach(map =>
                    {
                        var mappingProduct = new SaleBiddingDetailProductAttribute();
                        mappingProduct.ProductAttributeCategoryId = map.ProductAttributeCategoryId;
                        mappingProduct.ProductAttributeCategoryValueId = map.ProductAttributeCategoryValueId;
                        mappingProduct.ProductId = item.ProductId;
                        mappingProduct.SaleBiddingDetailId = costQuote.CostsQuoteId;
                        mappingProduct.SaleBiddingDetailProductAttributeId = Guid.NewGuid();

                        context.SaleBiddingDetailProductAttribute.Add(mappingProduct);
                    });

                    context.CostsQuote.Add(costQuote);
                });

                parameter.ListEmployee.ForEach(item =>
                {
                    var employeeMapping = new SaleBiddingEmployeeMapping();
                    employeeMapping.SaleBiddingEmployeeMappingId = Guid.NewGuid();
                    employeeMapping.SaleBiddingId = saleBidding.SaleBiddingId;
                    employeeMapping.EmployeeId = item;
                    employeeMapping.CreatedById = parameter.UserId;
                    employeeMapping.CreatedDate = DateTime.Now;

                    context.SaleBiddingEmployeeMapping.Add(employeeMapping);
                });

                context.SaveChanges();

                #region Gửi thông báo

                NotificationHelper.AccessNotification(context, TypeModel.SaleBidding, "CRE", new SaleBidding(),
                    saleBidding, true);

                #endregion

                return new CreateSaleBiddingResult()
                {
                    Status = true,
                    Message = "Thêm thành công",
                    SaleBiddingId = saleBidding.SaleBiddingId
                };
            }
            catch (Exception ex)
            {
                return new CreateSaleBiddingResult()
                {
                    Status = false,
                    Message = ex.Message
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
                CaculateVAT = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value - CacuDiscount) * (Vat.Value / 100));
            }
            result = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value) + CaculateVAT - CacuDiscount;
            return result;
        }

        public GetMasterDataSaleBiddingAddEditProductDialogResult GetMasterDataSaleBiddingAddEditProductDialog
                                                                        (GetMasterDataSaleBiddingAddEditProductDialogParameter parameter)
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

                var listProduct = context.Product.Where(x => x.Active == true).ToList();

                var listVendor = context.Vendor.Where(x => x.Active == true).Select(y => new VendorEntityModel
                {
                    VendorId = y.VendorId,
                    VendorCode = y.VendorCode,
                    VendorName = y.VendorName
                }).ToList();

                return new GetMasterDataSaleBiddingAddEditProductDialogResult()
                {
                    Status = true,
                    Message = "Success",
                    ListUnitMoney = listUnitMoney,
                    ListUnitProduct = listUintProduct,
                    ListVendor = listVendor,
                    ListProduct = listProduct
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataSaleBiddingAddEditProductDialogResult()
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

                return new GetVendorByProductIdResult()
                {
                    Status = true,
                    Message = "Success",
                    ListVendor = listVendor,
                    ListObjectAttributeNameProduct = listObjectAttributeNameProduct,
                    ListObjectAttributeValueProduct = listObjectAttributeValueProduct
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

        public DownloadTemplateProductResult DownloadTemplateProduct(DownloadTemplateProductParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_import_BOM_lines.xls";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = System.IO.File.ReadAllBytes(newFilePath);

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

        public GetMasterDataSaleBiddingDetailResult GetMasterDataSaleBiddingDetail(GetMasterDataSaleBiddingDetailParameter parameter)
        {
            try
            {
                // Kiểm tra coi người đăng nhập có quyền được sửa không
                bool isEdit = false;
                bool isLoginEmployeeJoin = false;
                bool isApproved = false;

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new GetMasterDataSaleBiddingDetailResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_USER
                    };
                }

                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetMasterDataSaleBiddingDetailResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_EMPLOYEE
                    };
                }

                // Lấy hồ sơ thầu 
                var saleBidding = context.SaleBidding.FirstOrDefault(x => x.SaleBiddingId == parameter.SaleBiddingId);
                if (saleBidding == null)
                {
                    return new GetMasterDataSaleBiddingDetailResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }

                // Lấy người phụ trách
                var listEmployeeCommon = context.Employee.ToList();
                var personInChargeSaleBidding = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == saleBidding.PersonInChargeId);
                if (personInChargeSaleBidding == null)
                {
                    return new GetMasterDataSaleBiddingDetailResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_PERSONINCHARGE
                    };
                }
                var employeeLogin = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                // Lấy danh sách nhân viên tham gia
                var listEmployeeJoinId = context.SaleBiddingEmployeeMapping.Where(x => x.SaleBiddingId == saleBidding.SaleBiddingId).Select(y => y.EmployeeId).ToList();
                var listOrganizationCommon = context.Organization.Where(x => x.Active == true).ToList();
                // Phân quyền người tham gia quản lí người tham gia , người phụ trách, quản lí người phụ trách được quyền vào màn chi tiết
                if (personInChargeSaleBidding.EmployeeId == employeeLogin.EmployeeId)
                {
                    isEdit = true;
                    if (employeeLogin.IsManager)
                    {
                        isApproved = true;
                    }
                    else
                    {
                        isApproved = false;
                    }
                }
                else if (listEmployeeJoinId.Contains(employeeLogin.EmployeeId))
                {
                    isLoginEmployeeJoin = true;
                    if (saleBidding.IsSupport)
                    {
                        isEdit = true;
                    }
                    // Lấy phòng ban người phụ trách
                    var personOrgId = personInChargeSaleBidding.OrganizationId;
                    var personOrg = listOrganizationCommon.FirstOrDefault(x => personOrgId == x.OrganizationId && x.Active == true);
                    var listOrgPersonManagerId = new List<Guid?>();
                    if (personOrg != null)
                    {
                        listOrgPersonManagerId.Add(personOrg.OrganizationId);
                        GetOrganizationParentId(personOrg, listOrganizationCommon, listOrgPersonManagerId);
                        if (listOrgPersonManagerId.Contains(employeeLogin.OrganizationId))
                        {
                            if (employeeLogin.IsManager == true)
                            {
                                isApproved = true;
                            }
                        }
                    }
                }
                else
                {
                    bool isView = false;
                    // Lấy danh sách phòng ban của nhân viên tham gia
                    var listEmployeeJoin = listEmployeeCommon.Where(x => listEmployeeJoinId.Contains(x.EmployeeId)).ToList();
                    var listOrgEmployeeJoinId = listEmployeeJoin.Select(x => x.OrganizationId).ToList();
                    var listOrgEmployeeJoin = listOrganizationCommon.Where(x => listOrgEmployeeJoinId.Contains(x.OrganizationId) && x.Active == true).ToList();
                    var listOrgParentEmpJoinId = new List<Guid?>();

                    listOrgEmployeeJoin.ForEach(item =>
                    {
                        listOrgParentEmpJoinId.Add(item.OrganizationId);
                        GetOrganizationParentId(item, listOrganizationCommon, listOrgParentEmpJoinId);
                    });

                    var listEmployeeJoinSaleBidding = listEmployeeCommon.Where(x => x.IsManager == true && x.Active == true && listOrgParentEmpJoinId.Contains(x.OrganizationId)).ToList();
                    var IsManagerEmployeeJoinLogin = listEmployeeJoinSaleBidding.FirstOrDefault(x => x.EmployeeId == employeeLogin.EmployeeId) != null;
                    if (IsManagerEmployeeJoinLogin)
                    {
                        isEdit = false;
                        isApproved = false;
                        isView = true;
                    }

                    // Lấy phòng ban người phụ trách
                    var personOrgId = personInChargeSaleBidding.OrganizationId;
                    var personOrg = listOrganizationCommon.FirstOrDefault(x => personOrgId == x.OrganizationId && x.Active == true);
                    var listOrgPersonManagerId = new List<Guid?>();
                    if (personOrg != null)
                    {
                        listOrgPersonManagerId.Add(personOrg.OrganizationId);
                        GetOrganizationParentId(personOrg, listOrganizationCommon, listOrgPersonManagerId);
                        var listPersonManager = listEmployeeCommon.Where(x => x.Active == true && listOrgPersonManagerId.Contains(x.OrganizationId) && x.IsManager == true).ToList();
                        var isOrgPersonManagerLogin = listPersonManager.FirstOrDefault(x => x.EmployeeId == employeeLogin.EmployeeId) != null;
                        if (isOrgPersonManagerLogin)
                        {
                            isEdit = true;
                            isApproved = true;
                            isView = true;
                        }
                    }

                    if (!isView)
                    {
                        return new GetMasterDataSaleBiddingDetailResult()
                        {
                            Status = false,
                            Message = CommonMessage.SaleBidding.EDIT_FAIL_ROLE
                        };
                    }
                }
                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();
                var customerCares = context.CustomerCare.Where(cc => cc.ActiveDate != null).ToList();
                var listCategoryType = commonCategoryType;

                var listCustomerCommon = context.Customer.ToList();
                var listPersonResult = new List<EmployeeEntityModel>();
                var listEmployeeResult = new List<EmployeeEntityModel>();
                var customerLead = listCustomerCommon.FirstOrDefault(x => x.CustomerId == saleBidding.CustomerId);
                // Kiểm tra người đăng nhập là nhân viên hay quản lí
                if (employeeLogin.IsManager)
                {
                    #region Lấy danh sách người phụ trách hồ sơ thầu theo người đang đăng nhập

                    // Là quản lí đăng nhập thì lấy danh sách tất cả các nhân viên cấp dưới của nó
                    var organization = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == employeeLogin.OrganizationId && x.Active == true);
                    if (organization != null)
                    {
                        var listPersonCustomerId = new List<Guid>();
                        if (customerLead == null)
                        {
                            return new GetMasterDataSaleBiddingDetailResult()
                            {
                                Status = false,
                                Message = CommonMessage.SaleBidding.GET_FAIL
                            };
                        }
                        else
                        {
                            // Lấy nhân viên phụ trách khách hàng ở cơ hội
                            var personCustomerLead = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == customerLead.PersonInChargeId);
                            listPersonCustomerId.Add(personCustomerLead.EmployeeId);
                            // Lấy danh sách cấp dưới và người phụ trách khách hàng
                            if (personCustomerLead.IsManager)
                            {
                                var listOrgPersonCustomerId = new List<Guid?>();
                                _getOrganizationChildrenId(listOrganizationCommon, personCustomerLead.OrganizationId, listOrgPersonCustomerId);
                                listOrgPersonCustomerId.Add(personCustomerLead.OrganizationId);
                                var listPersonCustomer = listEmployeeCommon.Where(x => x.Active == true && listOrgPersonCustomerId.Contains(x.OrganizationId)).ToList();
                                listPersonCustomerId = listPersonCustomer.Select(x => x.EmployeeId).ToList();
                            }

                        }
                        listPersonCustomerId = listPersonCustomerId.Distinct().ToList();

                        var listOrganizationId = new List<Guid?>();
                        _getOrganizationChildrenId(listOrganizationCommon, organization.OrganizationId, listOrganizationId);
                        listOrganizationId.Add(organization.OrganizationId);
                        listOrganizationId = listOrganizationId.Distinct().ToList();
                        var listEmployeeTemp = listEmployeeCommon.Where(x => x.Active == true && listOrganizationId.Contains(x.OrganizationId)).ToList();
                        var listEmpTempId = listEmployeeTemp.Select(x => x.EmployeeId).ToList();
                        listEmpTempId.Add(personInChargeSaleBidding.EmployeeId);
                        listEmpTempId = listEmpTempId.Distinct().ToList();

                        listPersonResult = listEmployeeCommon.Where(x => x.EmployeeId != null && listEmpTempId.Contains(x.EmployeeId) && (listPersonCustomerId.Count == 0 || listPersonCustomerId.Contains(x.EmployeeId)))
                        .Select(y => new EmployeeEntityModel()
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeName = y.EmployeeName,
                            EmployeeCode = y.EmployeeCode,
                            IsManager = y.IsManager,
                            Active = y.Active
                        }).ToList();

                        // Danh sách nhân viên bán hàng hoặc nhân viên tham gia
                        listEmployeeResult = listEmployeeTemp.Select(y => new EmployeeEntityModel()
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeName = y.EmployeeName,
                            EmployeeCode = y.EmployeeCode,
                            IsManager = y.IsManager,
                            Active = y.Active
                        }).ToList();
                    }
                    #endregion
                }
                else
                {
                    #region Lấy danh sách người phụ trách hồ sơ thầu theo người đang đăng nhập

                    listPersonResult.Add(new EmployeeEntityModel()
                    {
                        EmployeeId = employeeLogin.EmployeeId,
                        EmployeeName = employeeLogin.EmployeeName,
                        EmployeeCode = employeeLogin.EmployeeCode,
                        IsManager = employeeLogin.IsManager,
                        Active = employeeLogin.Active
                    });
                    if (employeeLogin.EmployeeId != personInChargeSaleBidding.EmployeeId)
                    {
                        listPersonResult.Add(new EmployeeEntityModel()
                        {
                            EmployeeId = personInChargeSaleBidding.EmployeeId,
                            EmployeeName = personInChargeSaleBidding.EmployeeName,
                            EmployeeCode = personInChargeSaleBidding.EmployeeCode,
                            IsManager = personInChargeSaleBidding.IsManager,
                            Active = personInChargeSaleBidding.Active
                        });
                    }

                    listEmployeeResult.Add(new EmployeeEntityModel()
                    {
                        EmployeeId = employeeLogin.EmployeeId,
                        EmployeeName = employeeLogin.EmployeeName,
                        EmployeeCode = employeeLogin.EmployeeCode,
                        IsManager = employeeLogin.IsManager,
                        Active = employeeLogin.Active
                    });

                    #endregion
                }

                #region Lấy danh sách tất cả khách hàng do người đăng nhập phụ trách hoặc cấp dưới phụ trách

                var listPersonInChargeId = listEmployeeResult.Select(x => x.EmployeeId).ToList();

                var listCustomerResult = listCustomerCommon.Where(x => (x.Active == true && listPersonInChargeId.Contains(x.PersonInChargeId))).Select(y => new CustomerEntityModel()
                {
                    CustomerId = y.CustomerId,
                    CustomerName = y.CustomerName,
                    CustomerCode = y.CustomerCode,
                    CustomerGroupId = y.CustomerGroupId,
                    PersonInChargeId = y.PersonInChargeId
                }).ToList();
                listCustomerResult.Add(new CustomerEntityModel()
                {
                    CustomerId = customerLead.CustomerId,
                    CustomerName = customerLead.CustomerName,
                    CustomerCode = customerLead.CustomerCode,
                    CustomerGroupId = customerLead.CustomerGroupId,
                    PersonInChargeId = customerLead.PersonInChargeId
                });

                listCustomerResult = listCustomerResult.Distinct().ToList();

                var listCustomerId = listCustomerResult.Select(x => x.CustomerId).ToList();

                var listEmployeeCustomerId = listCustomerResult.Select(x => x.PersonInChargeId).Distinct().ToList();

                var listEmployeeCustomer = listEmployeeCommon.Where(x => listEmployeeCustomerId.Contains(x.EmployeeId)).ToList();
                var listCustomerGroupId = listCustomerResult.Select(x => x.CustomerGroupId).Distinct().ToList();
                var listCustomerGroup = commonCategory.Where(x => listCustomerGroupId.Contains(x.CategoryId)).ToList();
                // Lấy danh sách contact
                var commonContact = context.Contact.Where(x => listCustomerId.Contains(x.ObjectId)).ToList();

                // Lấy danh sách xã
                var listWardId = commonContact.Select(x => x.WardId).ToList();
                var listWardCommon = context.Ward.Where(x => listWardId.Contains(x.WardId)).ToList();
                // Lấy danh sách phường
                var listDistrictId = commonContact.Select(x => x.DistrictId).ToList();
                var listDistrictCommon = context.District.Where(x => listDistrictId.Contains(x.DistrictId)).ToList();
                // Lấy danh sách tỉnh
                var listProvinceId = commonContact.Select(x => x.ProvinceId).ToList();
                var listProvinceCommon = context.Province.Where(x => listProvinceId.Contains(x.ProvinceId)).ToList();
                listCustomerResult.ForEach(item =>
                {
                    var temp = commonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS");
                    var ward = listWardCommon.FirstOrDefault(x => x.WardId == temp.WardId);
                    var district = listDistrictCommon.FirstOrDefault(x => x.DistrictId == temp.DistrictId);
                    var province = listProvinceCommon.FirstOrDefault(x => x.ProvinceId == temp.ProvinceId);
                    if (temp != null)
                    {
                        item.FullAddress = temp.Address;
                    }

                    if (ward != null)
                    {
                        item.FullAddress = item.FullAddress + "," + ward.WardName;
                    }

                    if (district != null)
                    {
                        item.FullAddress = item.FullAddress + "," + district.DistrictName;
                    }

                    if (province != null)
                    {
                        item.FullAddress = item.FullAddress + "," + province.ProvinceName;
                    }
                    item.CustomerPhone = temp?.Phone;
                    item.TaxCode = temp?.TaxCode;
                    item.CustomerGroup = listCustomerGroup.FirstOrDefault(x => x.CategoryId == item.CustomerGroupId)?.CategoryName;
                    item.PersonInCharge = listEmployeeCustomer.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId)?.EmployeeName;
                });

                #endregion

                #region Lấy danh sách trạng thái hồ sơ thầu

                var categoryTypeId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HST").CategoryTypeId;

                var listStatusResult = commonCategory.Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                new CategoryEntityModel()
                {
                    CategoryId = y.CategoryId,
                    CategoryName = y.CategoryName,
                    CategoryCode = y.CategoryCode,
                }).ToList();

                #endregion             

                #region Lấy thông tin hồ sơ thầu

                var customerCareCustomer = context.CustomerCareCustomer.Where(cu => cu.CustomerId == saleBidding.CustomerId).ToList();

                // Tìm cơ hội
                var lead = context.Lead.FirstOrDefault(x => x.LeadId == saleBidding.LeadId);
                if (lead == null)
                {
                    return new GetMasterDataSaleBiddingDetailResult
                    {
                        Status = false,
                        Message = "Không tìm cơ hội"
                    };
                }

                // Tìm contact để lấy tên cơ hội lấy contactId
                var contact = context.Contact.FirstOrDefault(x => x.ObjectId == lead.LeadId && x.ObjectType == "LEA");
                if (contact == null)
                {
                    return new GetMasterDataSaleBiddingDetailResult
                    {
                        Status = false,
                        Message = "Không tìm cơ hội"
                    };
                }

                // Tìm bên mời thầu(khách hàng)
                var customer = context.Customer.FirstOrDefault(x => x.CustomerId == saleBidding.CustomerId);

                if (customer == null)
                {
                    return new GetMasterDataSaleBiddingDetailResult
                    {
                        Status = false,
                        Message = "Không tìm thấy khách hàng"
                    };
                }

                var saleBiddingResult = new SaleBiddingEntityModel();

                saleBiddingResult.Address = saleBidding.Address;
                saleBiddingResult.BidStartDate = saleBidding.BidStartDate;
                saleBiddingResult.CreateDate = saleBidding.CreatedDate;
                saleBiddingResult.CurrencyUnitId = saleBidding.CurrencyUnitId;
                saleBiddingResult.CustomerId = saleBidding.CustomerId;
                saleBiddingResult.EffecTime = saleBidding.EffecTime;
                saleBiddingResult.EndDate = saleBidding.EndDate;
                saleBiddingResult.FormOfBid = saleBidding.FormOfBid;
                saleBiddingResult.LeadId = saleBidding.LeadId;
                saleBiddingResult.LeadName = contact.FirstName;
                saleBiddingResult.LeadCode = lead.LeadCode;
                saleBiddingResult.ContactId = contact.ContactId;
                saleBiddingResult.PersonInChargeId = saleBidding.PersonInChargeId;
                saleBiddingResult.SaleBiddingCode = saleBidding.SaleBiddingCode;
                saleBiddingResult.SaleBiddingId = saleBidding.SaleBiddingId;
                saleBiddingResult.SaleBiddingDetail = new List<CostQuoteModel>();
                saleBiddingResult.SaleBiddingName = saleBidding.SaleBiddingName;
                saleBiddingResult.StartDate = saleBidding.StartDate;
                saleBiddingResult.StatusId = saleBidding.StatusId;
                saleBiddingResult.TypeContractId = saleBidding.TypeContractId;
                saleBiddingResult.ValueBid = saleBidding.ValueBid;
                saleBiddingResult.Note = saleBidding.Note;
                saleBiddingResult.EmployeeId = saleBidding.EmployeeId;
                saleBiddingResult.IsSupport = saleBidding.IsSupport;
                saleBiddingResult.UpdatedById = saleBidding.UpdatedById;


                // Lấy danh sách sản phẩm dịch vụ trong hồ sơ thầu
                var listCostsQuote = context.CostsQuote.Where(x => x.SaleBiddingId == saleBiddingResult.SaleBiddingId)
                    .ToList();

                var listCostsQuoteResult = new List<CostQuoteModel>();
                var listCostsQuoteId = listCostsQuote.Select(w => w.CostsQuoteId).ToList();
                var listVendorId = listCostsQuote.Select(w => w.VendorId).ToList();
                var listProductId = listCostsQuote.Select(w => w.ProductId).ToList();
                // Lấy danh sách nhà cung cấp
                var listVendorEntity = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();

                // Lấy danh sách sản phẩm
                var listProductEntity = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();

                // Lấy loại tiền
                var leadMoneyTypeId =
                    commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI").CategoryTypeId;

                // Lấy danh sách đơn vị tính
                var unitTypeId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH").CategoryTypeId;

                var moneyList = commonCategory.Where(w => w.CategoryTypeId == leadMoneyTypeId).Select(w => new CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var unitList = commonCategory.Where(w => w.CategoryTypeId == unitTypeId).Select(w => new CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                var commonSaleBiddingDetailProductAttribute = context.SaleBiddingDetailProductAttribute.ToList();

                var leadProductDetailProductAttributeValueEntity = commonSaleBiddingDetailProductAttribute.Where(w => listCostsQuoteId.Contains(w.SaleBiddingDetailId.Value)).ToList();

                listCostsQuote.ForEach(costsQuote =>
                {
                    var listAttribute = new List<SaleBiddingDetailProductAttributeEntityModel>();
                    var listAttributeEntity = leadProductDetailProductAttributeValueEntity.Where(w => w.SaleBiddingDetailId == costsQuote.CostsQuoteId).ToList();
                    listAttributeEntity.ForEach(attri =>
                    {
                        listAttribute.Add(new SaleBiddingDetailProductAttributeEntityModel
                        {
                            SaleBiddingDetailProductAttributeId = attri.SaleBiddingDetailProductAttributeId,
                            SaleBiddingDetailId = attri.SaleBiddingDetailId,
                            ProductId = attri.ProductId,
                            ProductAttributeCategoryId = attri.ProductAttributeCategoryId,
                            ProductAttributeCategoryValueId = attri.ProductAttributeCategoryValueId
                        });
                    });

                    var nameMoneyUnit = moneyList.FirstOrDefault(f => f.CategoryId == costsQuote.CurrencyUnit)?.CategoryName ?? "";
                    var nameVendor = listVendorEntity.FirstOrDefault(f => f.VendorId == costsQuote.VendorId)?.VendorName ?? "";
                    var productCode = listProductEntity.FirstOrDefault(f => f.ProductId == costsQuote.ProductId)?.ProductCode ?? "";
                    var unitName = unitList.FirstOrDefault(f => f.CategoryId == costsQuote.UnitId)?.CategoryName ?? "";

                    listCostsQuoteResult.Add(new CostQuoteModel
                    {
                        CostsQuoteId = costsQuote.CostsQuoteId,
                        CostsQuoteType = costsQuote.CostsQuoteType,
                        ExchangeRate = costsQuote.ExchangeRate,
                        SaleBiddingId = costsQuote.SaleBiddingId,
                        VendorId = costsQuote.VendorId,
                        ProductId = costsQuote.ProductId,
                        Quantity = costsQuote.Quantity,
                        UnitPrice = costsQuote.UnitPrice,
                        CurrencyUnit = costsQuote.CurrencyUnit,
                        Vat = costsQuote.Vat,
                        DiscountType = costsQuote.DiscountType,
                        DiscountValue = costsQuote.DiscountValue,
                        Description = costsQuote.Description,
                        OrderDetailType = costsQuote.OrderDetailType,
                        UnitId = costsQuote.UnitId,
                        IncurredUnit = costsQuote.IncurredUnit,
                        ProductName = costsQuote.ProductName,
                        SaleBiddingDetailProductAttribute = listAttribute,
                        //label
                        NameMoneyUnit = nameMoneyUnit,
                        NameVendor = nameVendor,
                        ProductCode = productCode,
                        ProductNameUnit = unitName,
                        SumAmount = SumAmount(costsQuote.Quantity, costsQuote.UnitPrice, costsQuote.ExchangeRate, costsQuote.Vat, costsQuote.DiscountValue,
                                                    costsQuote.DiscountType),
                    });
                });
                saleBiddingResult.SaleBiddingDetail = listCostsQuoteResult;

                // Lây thông tin danh sách tất cả chi tiết hồ sơ thầu
                var listSaleBiddingDetail = context.SaleBiddingDetail.Where(x => x.SaleBiddingId == saleBiddingResult.SaleBiddingId).Select(y =>
                new SaleBiddingDetailEntityModel()
                {
                    SaleBiddingId = y.SaleBiddingId,
                    SaleBiddingDetailId = y.SaleBiddingDetailId,
                    Category = y.Category,
                    Content = y.Content,
                    ListFile = new List<FileInFolderEntityModel>()
                }).ToList();

                var listSaleBiddingDetailtId = listSaleBiddingDetail.Select(x => x.SaleBiddingDetailId).ToList();
                var listFileInFolder = context.FileInFolder.Where(x => listSaleBiddingDetailtId.Contains(x.ObjectId) && x.ObjectType == "HST" && x.Active == true).
                    Select(y => new FileInFolderEntityModel()
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

                listSaleBiddingDetail.ForEach(item =>
                {
                    item.ListFile = listFileInFolder.Where(x => x.ObjectId == item.SaleBiddingDetailId).ToList();
                });

                #endregion

                #region Lấy list ghi chú

                var listNote = new List<NoteEntityModel>();

                listNote = context.Note
                    .Where(x => x.ObjectId == parameter.SaleBiddingId && x.ObjectType == "HST" && x.Active == true).Select(
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

                #region Lấy danh sách sản phẩm

                var listProductResult = context.Product.Where(z => z.Active == true).Select(x => new ProductEntityModel()
                {
                    ProductId = x.ProductId,
                    ProductCategoryId = x.ProductCategoryId,
                    ProductName = x.ProductName,
                    ProductCode = x.ProductCode,
                    Price1 = x.Price1,
                    Price2 = x.Price2,
                    Quantity = x.Quantity,
                    ProductUnitId = x.ProductUnitId,
                    ProductUnitName = x.ProductName,
                    ProductDescription = x.ProductDescription,
                    Vat = x.Vat,
                    MinimumInventoryQuantity = x.MinimumInventoryQuantity,
                    ProductMoneyUnitId = x.ProductMoneyUnitId,
                    ExWarehousePrice = x.ExWarehousePrice,
                }).ToList();

                #endregion

                #region Lấy thông tin contact

                var contactTemp = context.Contact.FirstOrDefault(x => x.ObjectId == saleBidding.CustomerId && x.ObjectType == "CUS" && x.Active == true);
                var contactResult = new ContactEntityModel();
                if (contactTemp != null)
                {
                    contactResult.Gender = contactTemp.Gender;
                    contactResult.Address = contactTemp.Address;
                    contactResult.Agency = contactTemp.Agency;
                    contactResult.AvatarUrl = contactTemp.AvatarUrl;
                    contactResult.Birthplace = contactTemp.Birthplace;
                    contactResult.CompanyAddress = contactTemp.CompanyAddress;
                    contactResult.CompanyName = contactTemp.CompanyName;
                    contactResult.ContactId = contactTemp.ContactId;
                    contactResult.CountryId = contactTemp.CountryId;
                    contactResult.CustomerPosition = contactTemp.CustomerPosition;
                    contactResult.DateOfBirth = contactTemp.DateOfBirth;
                    contactResult.District = contactTemp.District;
                    contactResult.DistrictId = contactTemp.DistrictId;
                    contactResult.Email = contactTemp.Email;
                    contactResult.FirstName = contactTemp.FirstName;
                    contactResult.HealthInsuranceDateOfIssue = contactTemp.HealthInsuranceDateOfIssue;
                    contactResult.HealthInsuranceDateOfParticipation = contactTemp.HealthInsuranceDateOfParticipation;
                    contactResult.HealthInsuranceNumber = contactTemp.HealthInsuranceNumber;
                    contactResult.IdentityId = contactTemp.IdentityId;
                    contactResult.IdentityIddateOfIssue = contactTemp.IdentityIddateOfIssue;
                    contactResult.IdentityIddateOfParticipation = contactTemp.IdentityIddateOfParticipation;
                    contactResult.IdentityIdplaceOfIssue = contactTemp.IdentityIdplaceOfIssue;
                    contactResult.Job = contactTemp.Job;
                    contactResult.LastName = contactTemp.LastName;
                    contactResult.MaritalStatus = contactTemp.MaritalStatus;
                    contactResult.MaritalStatusId = contactTemp.MaritalStatusId;
                    contactResult.Note = contactTemp.Note;
                    contactResult.ObjectId = contactTemp.ObjectId;
                    contactResult.ObjectType = contactTemp.ObjectType;
                    contactResult.Other = contactTemp.Other;
                    contactResult.OtherEmail = contactTemp.OtherEmail;
                    contactResult.OtherPhone = contactTemp.OtherPhone;
                    contactResult.Phone = contactTemp.Phone;
                    contactResult.PostCode = contactTemp.PostCode;
                    contactResult.Province = contactTemp.Province;
                    contactResult.ProvinceId = contactTemp.ProvinceId;
                    contactResult.Role = contactTemp.Role;
                    contactResult.SocialInsuranceDateOfIssue = contactTemp.SocialInsuranceDateOfIssue;
                    contactResult.SocialInsuranceDateOfParticipation = contactTemp.SocialInsuranceDateOfParticipation;
                    contactResult.SocialInsuranceNumber = contactTemp.SocialInsuranceNumber;
                    contactResult.SocialUrl = contactTemp.SocialUrl;
                    contactResult.TaxCode = contactTemp.TaxCode;
                    contactResult.VisaDateOfIssue = contactTemp.VisaDateOfIssue;
                    contactResult.TypePaid = contactTemp.TypePaid;
                    contactResult.VisaExpirationDate = contactTemp.VisaExpirationDate;
                    contactResult.VisaNumber = contactTemp.VisaNumber;
                    contactResult.Ward = contactTemp.Ward;
                    contactResult.WardId = contactTemp.WardId;
                    contactResult.WebsiteUrl = contactTemp.WebsiteUrl;
                    contactResult.WorkEmail = contactTemp.WorkEmail;
                    contactResult.WorkHourOfEnd = contactTemp.WorkHourOfEnd;
                    contactResult.WorkHourOfStart = contactTemp.WorkHourOfStart;
                    contactResult.WorkPermitNumber = contactTemp.WorkPermitNumber;
                    contactResult.WorkPhone = contactTemp.WorkPhone;
                }

                #endregion

                #region Lấy list thông tin CSKH theo tháng hiện tại

                var listEmployeePosition = context.Position.ToList();

                //Hình thức
                var customerCareCategoryTypeId =
                    listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HCS").CategoryTypeId;
                var listCustomerCareCategory =
                    commonCategory.Where(x => x.CategoryTypeId == customerCareCategoryTypeId).ToList();
                var listTypeOfCustomerCare1 = listCustomerCareCategory
                    .Where(x => x.CategoryCode == "Gift" || x.CategoryCode == "CallPhone").Select(y => y.CategoryId)
                    .ToList();
                var listTypeOfCustomerCare2 = listCustomerCareCategory
                    .Where(x => x.CategoryCode == "Email" || x.CategoryCode == "SMS").Select(y => y.CategoryId)
                    .ToList();

                //Trạng thái
                var statusOfCustomerCareCategoryId =
                    listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCS").CategoryTypeId;
                var statusActiveOfCustomerCare = commonCategory.FirstOrDefault(x =>
                    x.CategoryTypeId == statusOfCustomerCareCategoryId && x.CategoryCode == "Active").CategoryId;

                var listCustomerCareInfor = new List<CustomerCareInforModel>();
                var listAllCustomerCareForWeek = new List<CustomerCareForWeekModel>();

                /*
                 * Lọc ra các chương trình CSKH thõa mãn các điều kiện:
                 * - Hình thức là Tặng quà và Gọi điện
                 * - Có ngày kích hoạt trong tháng, năm được chọn
                 */
                var listCustomerCare1 = customerCares.Where(x =>
                        listTypeOfCustomerCare1.Contains(x.CustomerCareContactType.Value) &&
                        x.ActiveDate.Value.Month == (DateTime.Now).Month &&
                        x.ActiveDate.Value.Year == (DateTime.Now).Year)
                    .ToList();

                //Lọc ra các chương trình CSKH mà có Khách hàng hiện tại tham gia
                var listCustomerCareId1 = listCustomerCare1.Select(x => x.CustomerCareId).ToList();

                if (listCustomerCareId1.Count > 0)
                {
                    var listCustomerCareIdForCustomer = customerCareCustomer
                        .Where(x => listCustomerCareId1.Contains(x.CustomerCareId.Value) &&
                                    x.CustomerId == saleBidding.CustomerId).Select(x => x.CustomerCareId).ToList();

                    if (listCustomerCareIdForCustomer.Count > 0)
                    {
                        listCustomerCare1 = listCustomerCare1
                            .Where(x => listCustomerCareIdForCustomer.Contains(x.CustomerCareId)).ToList();
                    }
                    else
                    {
                        listCustomerCare1 = new List<CustomerCare>();
                    }
                }

                /*
                 * Lấy list CustomerCareId trong bảng Queue thõa màn các điều kiện sau:
                 * - IsSend = true (Đã gửi)
                 * - Là gửi Email hoặc SMS
                 * - Có ngày gửi (SenDate) trong tháng, năm được chọn
                 */
                var listQueueCustomerCare2 = context.Queue.Where(x =>
                    x.IsSend == true &&
                    (x.Method == "Email" || x.Method == "SMS") && x.SenDate.Value.Month == (DateTime.Now).Month &&
                    x.SenDate.Value.Year == (DateTime.Now).Year && x.CustomerId == saleBidding.CustomerId).ToList();

                var listQueueCustomerCare2Id = listQueueCustomerCare2.Select(y => y.CustomerCareId).Distinct().ToList();

                var listCustomerCare2 = new List<CustomerCare>();
                if (listQueueCustomerCare2Id.Count > 0)
                {
                    listCustomerCare2 = customerCares
                        .Where(x => listQueueCustomerCare2Id.Contains(x.CustomerCareId)).ToList();
                }

                //merge 2 list CustomerCare
                listCustomerCare1.AddRange(listCustomerCare2);

                var listEmployeeId = new List<Guid>();
                if (listCustomerCare1.Count > 0)
                {
                    var listCustomerCareId = listCustomerCare1.Select(y => y.CustomerCareId).ToList();
                    var listAllFeedBack = context.CustomerCareFeedBack
                        .Where(x => listCustomerCareId.Contains(x.CustomerCareId.Value) && x.CustomerId == saleBidding.CustomerId).ToList();
                    listEmployeeId = listCustomerCare1.Select(y => y.EmployeeCharge.Value).Distinct().ToList();

                    listEmployeeId.ForEach(employeeId =>
                    {
                        var customerCareInfor = new CustomerCareInforModel();
                        var emp = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == employeeId);
                        customerCareInfor.EmployeeCharge = emp.EmployeeId;
                        customerCareInfor.EmployeeName = emp.EmployeeName;
                        customerCareInfor.EmployeePosition = listEmployeePosition
                            .FirstOrDefault(x => x.PositionId == emp.PositionId).PositionName;

                        listCustomerCareInfor.Add(customerCareInfor);
                    });

                    listCustomerCare1.ForEach(item =>
                    {
                        var customerCareForWeek = new CustomerCareForWeekModel();
                        customerCareForWeek.CustomerCareId = item.CustomerCareId;
                        customerCareForWeek.EmployeeCharge = item.EmployeeCharge.Value;
                        customerCareForWeek.Title = listCustomerCareCategory
                            .FirstOrDefault(x => x.CategoryId == item.CustomerCareContactType).CategoryName;

                        /*
                         * Gửi SMS: 1
                         * Gửi email: 2
                         * Tặng quà: 3
                         * Gọi điện: 4
                         */
                        var customerCareCategoryCode = listCustomerCareCategory
                            .FirstOrDefault(x => x.CategoryId == item.CustomerCareContactType).CategoryCode;

                        switch (customerCareCategoryCode)
                        {
                            case "SMS":
                                customerCareForWeek.Type = 1;
                                customerCareForWeek.SubTitle = "Xem chi tiết";
                                customerCareForWeek.FeedBackStatus = 0;
                                customerCareForWeek.Background = "#fbe8ba";
                                customerCareForWeek.ActiveDate = listQueueCustomerCare2
                                    .FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId).SenDate.Value;
                                break;
                            case "Email":
                                customerCareForWeek.Type = 2;
                                customerCareForWeek.SubTitle = "Xem chi tiết";
                                customerCareForWeek.FeedBackStatus = 0;
                                customerCareForWeek.Background = "#e5cbf2";
                                customerCareForWeek.ActiveDate = listQueueCustomerCare2
                                    .FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId).SenDate.Value;
                                break;
                            case "Gift":
                                customerCareForWeek.Type = 3;
                                var checkFeedBackGift = listAllFeedBack.FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId);
                                customerCareForWeek.SubTitle = checkFeedBackGift != null
                                    ? (checkFeedBackGift.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                customerCareForWeek.FeedBackStatus = checkFeedBackGift != null ? 1 : 2;
                                customerCareForWeek.Background = "#cfdefa";
                                customerCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                            case "CallPhone":
                                customerCareForWeek.Type = 4;
                                var checkFeedBackCallPhone = listAllFeedBack.FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId);
                                customerCareForWeek.SubTitle = checkFeedBackCallPhone != null
                                    ? (checkFeedBackCallPhone.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                customerCareForWeek.FeedBackStatus = checkFeedBackCallPhone != null ? 1 : 2;
                                customerCareForWeek.Background = "#f4d4e4";
                                customerCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                        }

                        listAllCustomerCareForWeek.Add(customerCareForWeek);
                    });

                    #region Nhóm theo nhân viên CSKH và theo tuần

                    var current_month = (DateTime.Now).Month;
                    var current_year = (DateTime.Now).Year;
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

                    var listWeek1 = new List<CustomerCareForWeekModel>();
                    var listWeek2 = new List<CustomerCareForWeekModel>();
                    var listWeek3 = new List<CustomerCareForWeekModel>();
                    var listWeek4 = new List<CustomerCareForWeekModel>();
                    var listWeek5 = new List<CustomerCareForWeekModel>();
                    listCustomerCareInfor.ForEach(item =>
                    {
                        listWeek1 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek1 &&
                            x.ActiveDate < endDateWeek1).OrderBy(z => z.ActiveDate).ToList();
                        item.Week1 = listWeek1;

                        listWeek2 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek2 &&
                            x.ActiveDate < endDateWeek2).OrderBy(z => z.ActiveDate).ToList();
                        item.Week2 = listWeek2;

                        listWeek3 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek3 &&
                            x.ActiveDate < endDateWeek3).OrderBy(z => z.ActiveDate).ToList();
                        item.Week3 = listWeek3;

                        listWeek4 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek4 &&
                            x.ActiveDate < endDateWeek4).OrderBy(z => z.ActiveDate).ToList();
                        item.Week4 = listWeek4;

                        if (check != 1)
                        {
                            listWeek5 = listAllCustomerCareForWeek.Where(x =>
                                x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek5 &&
                                x.ActiveDate < endDateWeek5).OrderBy(z => z.ActiveDate).ToList();
                            item.Week5 = listWeek5;
                        }
                    });

                    #endregion

                }

                #endregion

                #region Lấy list thông tin lịch hẹn theo tháng hiện tại

                var customerMeetingInforModel = new CustomerMeetingInforModel();
                customerMeetingInforModel.EmployeeId = employeeLogin.EmployeeId;
                customerMeetingInforModel.EmployeeName = employeeLogin.EmployeeName;
                customerMeetingInforModel.EmployeePosition = listEmployeePosition
                    .FirstOrDefault(x => x.PositionId == employeeLogin.PositionId) != null ? listEmployeePosition
                    .FirstOrDefault(x => x.PositionId == employeeLogin.PositionId).PositionName : string.Empty;

                var listAllCustomerMeetingForWeek = new List<CustomerMeetingForWeekModel>();

                var listAllCustomerMeeting = context.CustomerMeeting.Where(x =>
                        x.EmployeeId == employeeLogin.EmployeeId && x.CustomerId == saleBidding.CustomerId &&
                        x.StartDate.Value.Month == (DateTime.Now).Month &&
                        x.StartDate.Value.Year == (DateTime.Now).Year)
                    .ToList();

                listAllCustomerMeeting.ForEach(item =>
                {
                    var customerMeetingForWeek = new CustomerMeetingForWeekModel();
                    customerMeetingForWeek.CustomerMeetingId = item.CustomerMeetingId;
                    customerMeetingForWeek.EmployeeId = item.EmployeeId;
                    customerMeetingForWeek.Title = item.Title;
                    customerMeetingForWeek.Subtitle = item.StartDate.Value.ToString("dd/MM/yyyy") + " - " + item.StartDate.Value.ToString("HH:mm");
                    customerMeetingForWeek.Background = "#ffcc00";
                    customerMeetingForWeek.StartDate = item.StartDate;
                    customerMeetingForWeek.StartHours = item.StartHours;
                    listAllCustomerMeetingForWeek.Add(customerMeetingForWeek);
                });

                var current_month_meeting = (DateTime.Now).Month;
                var current_year_meeting = (DateTime.Now).Year;
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

                var week1 = new List<CustomerMeetingForWeekModel>();
                var week2 = new List<CustomerMeetingForWeekModel>();
                var week3 = new List<CustomerMeetingForWeekModel>();
                var week4 = new List<CustomerMeetingForWeekModel>();
                var week5 = new List<CustomerMeetingForWeekModel>();

                week1 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek1 && x.StartDate < endDateMeetingWeek1)
                    .OrderBy(z => z.StartDate).ToList();

                week2 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek2 && x.StartDate < endDateMeetingWeek2)
                    .OrderBy(z => z.StartDate).ToList();

                week3 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek3 && x.StartDate < endDateMeetingWeek3)
                    .OrderBy(z => z.StartDate).ToList();

                week4 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek4 && x.StartDate < endDateMeetingWeek4)
                    .OrderBy(z => z.StartDate).ToList();

                if (checkMeeting != 1)
                {
                    week5 = listAllCustomerMeetingForWeek
                        .Where(x => x.StartDate >= startDateMeetingWeek5 && x.StartDate < endDateMeetingWeek5)
                        .OrderBy(z => z.StartDate).ToList();
                }

                customerMeetingInforModel.Week1 = week1;
                customerMeetingInforModel.Week2 = week2;
                customerMeetingInforModel.Week3 = week3;
                customerMeetingInforModel.Week4 = week4;
                customerMeetingInforModel.Week5 = week5;

                #endregion


                // Lấy loại hợp đồng
                var typeCONTRACT = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CONTRACT")?.CategoryTypeId;
                var typeContact = commonCategory.Where(c => c.CategoryTypeId == typeCONTRACT && c.Active == true).Select(x => new CategoryEntityModel()
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    CategoryCode = x.CategoryCode
                }).ToList();

                listEmployeeResult = listEmployeeCommon.Where(x => x.Active == true).Select(y => new EmployeeEntityModel()
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeName = y.EmployeeName,
                    EmployeeCode = y.EmployeeCode,
                    IsManager = y.IsManager,
                    Active = y.Active
                }).ToList();

                listPersonResult = listPersonResult.Distinct().ToList();

                return new GetMasterDataSaleBiddingDetailResult
                {
                    Status = true,
                    ListEmployee = listEmployeeResult,
                    ListCustomer = listCustomerResult,
                    ListMoneyUnit = moneyList,
                    SaleBidding = saleBiddingResult,
                    ListSaleBiddingDetail = listSaleBiddingDetail,
                    ListNote = listNote,
                    ListStatus = listStatusResult,
                    ListEmployeeMapping = listEmployeeJoinId,
                    ListProduct = listProductResult,
                    Contact = contactResult,
                    ListCustomerCareInfor = listCustomerCareInfor,
                    CustomerMeetingInfor = customerMeetingInforModel,
                    ListPerson = listPersonResult,
                    IsApproved = isApproved,
                    ListTypeContact = typeContact,
                    isEdit = isEdit,
                    isLoginEmployeeJoin = isLoginEmployeeJoin
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSaleBiddingDetailResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public EditSaleBiddingResult EditSaleBidding(EditSaleBiddingParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL
                    };
                }

                //kiểm tra ngày có kết quả dự kiên scos lơn hơn hoặc bằng ngày mở thầu không
                var startDate = parameter.SaleBidding.BidStartDate.Value;
                var endDate = parameter.SaleBidding.EndDate;
                if (endDate?.Date < startDate.Date)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.END_DATE_FAIL
                    };
                }
                // Tìm người phụ trách của hồ sơ thầu
                var saleBiddingUpdate = context.SaleBidding.FirstOrDefault(x => parameter.SaleBidding.SaleBiddingId == x.SaleBiddingId);

                if (saleBiddingUpdate == null)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL_STATUS
                    };
                }

                var listEmployeeJoin = new List<Guid?>();
                if (saleBiddingUpdate.IsSupport)
                {
                    // Lấy danh sách nhân viên tham gia
                    listEmployeeJoin = context.SaleBiddingEmployeeMapping.Where(x => x.SaleBiddingId == saleBiddingUpdate.SaleBiddingId).Select(y => y.EmployeeId).ToList();
                }

                var isEdit = context.Category.FirstOrDefault(x => x.CategoryId == saleBiddingUpdate.StatusId && x.CategoryCode == "NEW") == null;
                if (isEdit)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL
                    };
                }
                var listEmployeeCommon = context.Employee.ToList();
                var employee = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == saleBiddingUpdate.PersonInChargeId);
                if (employee == null)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL_ROLE
                    };
                }
                var listOrganizationCommon = context.Organization.ToList();
                // Lấy phòng ban của người phụ trách cơ hội
                var organization = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId);
                if (organization == null)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL_ROLE
                    };
                }
                // Lấy danh sách tất cả phòng ban quản lí phòng bạn người phụ trách hiện tại
                var listOrganization = new List<Guid?>();
                listOrganization.Add(organization.OrganizationId);
                listOrganization = GetOrganizationParentId(organization, listOrganizationCommon, listOrganization);
                if (listOrganization == null || listOrganization.Count == 0)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL_ROLE
                    };
                }
                listOrganization.Add(organization.OrganizationId);
                // Lấy tất cả nhân viên quản lí của người phụ trách cơ hội
                var isEditAll = true;
                var listEmployee = listEmployeeCommon.Where(x => listOrganization.Contains(x.OrganizationId) && x.IsManager == true).ToList();
                listEmployee.Add(employee);
                if (listEmployeeJoin.Count > 0)
                {
                    var listEmployeeJoinLogin = listEmployeeCommon.Where(x => listEmployeeJoin.Contains(x.EmployeeId)).ToList();
                    listEmployee.AddRange(listEmployeeJoinLogin);
                    var isCheckEdit = listEmployeeJoinLogin.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                    if (isCheckEdit != null)
                    {
                        isEditAll = false;
                    }
                }
                // Check coi người tạo có phải là người người lí người phụ trách cơ hội hay cơ hội hay k
                var employeeCheck = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (employeeCheck == null)
                {
                    return new EditSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL_ROLE
                    };
                }

                if (isEditAll)
                {
                    #region Update thông tin hồ sơ thầu

                    saleBiddingUpdate.Address = parameter.SaleBidding.Address;
                    saleBiddingUpdate.BidStartDate = parameter.SaleBidding.BidStartDate;
                    saleBiddingUpdate.CurrencyUnitId = parameter.SaleBidding.CurrencyUnitId;
                    saleBiddingUpdate.CustomerId = parameter.SaleBidding.CustomerId;
                    saleBiddingUpdate.CreatedById = parameter.UserId;
                    saleBiddingUpdate.EffecTime = parameter.SaleBidding.EffecTime;
                    saleBiddingUpdate.EmployeeId = parameter.SaleBidding.EmployeeId;
                    saleBiddingUpdate.EndDate = parameter.SaleBidding.EndDate;
                    saleBiddingUpdate.FormOfBid = parameter.SaleBidding.FormOfBid;
                    saleBiddingUpdate.LeadId = parameter.SaleBidding.LeadId;
                    saleBiddingUpdate.Note = parameter.SaleBidding.Note;
                    saleBiddingUpdate.PersonInChargeId = parameter.SaleBidding.PersonInChargeId;
                    saleBiddingUpdate.SaleBiddingName = parameter.SaleBidding.SaleBiddingName;
                    saleBiddingUpdate.SaleBiddingCode = parameter.SaleBidding.SaleBiddingCode;
                    saleBiddingUpdate.StartDate = parameter.SaleBidding.StartDate;
                    saleBiddingUpdate.StatusId = parameter.SaleBidding.StatusId;
                    saleBiddingUpdate.TypeContractId = parameter.SaleBidding.TypeContractId;
                    saleBiddingUpdate.UpdatedById = parameter.UserId;
                    saleBiddingUpdate.UpdatedDate = DateTime.Now;
                    saleBiddingUpdate.ValueBid = parameter.SaleBidding.ValueBid;

                    context.Update(saleBiddingUpdate);

                    #endregion
                }

                #region Update thông tin danh sách nhân viên tham gia

                var listEmployeeMappingUpdate = context.SaleBiddingEmployeeMapping.Where(x => x.SaleBiddingId == saleBiddingUpdate.SaleBiddingId).ToList();
                if (listEmployeeMappingUpdate.Count > 0)
                {
                    context.RemoveRange(listEmployeeMappingUpdate);
                }

                parameter.ListEmployee?.ForEach(emp =>
                {
                    var empMappingSaleBiddingNew = new SaleBiddingEmployeeMapping()
                    {
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        EmployeeId = emp,
                        SaleBiddingEmployeeMappingId = Guid.NewGuid(),
                        SaleBiddingId = saleBiddingUpdate.SaleBiddingId
                    };

                    context.SaleBiddingEmployeeMapping.Add(empMappingSaleBiddingNew);
                });

                #endregion

                var listCostsQuocTeUpdate = context.CostsQuote
                    .Where(x => x.SaleBiddingId == saleBiddingUpdate.SaleBiddingId).ToList();

                #region Update thông tin danh sách sản phẩm chi phí dịch vụ tab chi phí đầu vào

                var listCostUpdate = listCostsQuocTeUpdate.Where(x => x.CostsQuoteType == 1).ToList();
                if (listCostUpdate.Count > 0)
                {
                    context.CostsQuote.RemoveRange(listCostUpdate);
                }

                parameter.ListCost.ForEach(costs =>
                {
                    var costQuote = new CostsQuote();
                    costQuote.CostsQuoteId = Guid.NewGuid();
                    costQuote.CostsQuoteType = 1;
                    costQuote.CreatedById = parameter.UserId;
                    costQuote.CreatedDate = DateTime.Now;
                    costQuote.CurrencyUnit = costs.CurrencyUnit;
                    costQuote.Description = costs.Description;
                    costQuote.DiscountType = costs.DiscountType;
                    costQuote.DiscountValue = costs.DiscountValue;
                    costQuote.ExchangeRate = costs.ExchangeRate;
                    costQuote.IncurredUnit = costs.IncurredUnit;
                    costQuote.ProductId = costs.ProductId;
                    costQuote.Quantity = costs.Quantity;
                    costQuote.SaleBiddingId = saleBiddingUpdate.SaleBiddingId;
                    costQuote.UnitId = costs.UnitId;
                    costQuote.UnitPrice = costs.UnitPrice;
                    costQuote.Vat = costs.Vat;
                    costQuote.VendorId = costs.VendorId;
                    costQuote.ProductName = costs.ProductName;
                    costQuote.OrderDetailType = costs.OrderDetailType;
                    costs.SaleBiddingDetailProductAttribute = costs.SaleBiddingDetailProductAttribute == null ?
                    new List<SaleBiddingDetailProductAttributeEntityModel>() : costs.SaleBiddingDetailProductAttribute;
                    costs.SaleBiddingDetailProductAttribute.ForEach(map =>
                    {
                        var mappingProduct = new SaleBiddingDetailProductAttribute();
                        mappingProduct.ProductAttributeCategoryId = map.ProductAttributeCategoryId;
                        mappingProduct.ProductAttributeCategoryValueId = map.ProductAttributeCategoryValueId;
                        mappingProduct.ProductId = costs.ProductId;
                        mappingProduct.SaleBiddingDetailId = costQuote.CostsQuoteId;
                        mappingProduct.SaleBiddingDetailProductAttributeId = Guid.NewGuid();

                        context.SaleBiddingDetailProductAttribute.Add(mappingProduct);
                    });

                    context.CostsQuote.Add(costQuote);
                });

                #endregion

                #region Update thông tin danh sách sản phẩm chi phí dịch vụ tab chi tiết báo giá

                var listQuocteUpdate = listCostsQuocTeUpdate.Where(x => x.CostsQuoteType == 2).ToList();
                if (listQuocteUpdate.Count > 0)
                {
                    context.CostsQuote.RemoveRange(listQuocteUpdate);
                }

                parameter.ListQuocte.ForEach(costs =>
                {
                    var costQuote = new CostsQuote();
                    costQuote.CostsQuoteId = Guid.NewGuid();
                    costQuote.CostsQuoteType = 2;
                    costQuote.CreatedById = parameter.UserId;
                    costQuote.CreatedDate = DateTime.Now;
                    costQuote.CurrencyUnit = costs.CurrencyUnit;
                    costQuote.Description = costs.Description;
                    costQuote.DiscountType = costs.DiscountType;
                    costQuote.DiscountValue = costs.DiscountValue;
                    costQuote.ExchangeRate = costs.ExchangeRate;
                    costQuote.IncurredUnit = costs.IncurredUnit;
                    costQuote.ProductId = costs.ProductId;
                    costQuote.Quantity = costs.Quantity;
                    costQuote.SaleBiddingId = saleBiddingUpdate.SaleBiddingId;
                    costQuote.UnitId = costs.UnitId;
                    costQuote.UnitPrice = costs.UnitPrice;
                    costQuote.Vat = costs.Vat;
                    costQuote.VendorId = costs.VendorId;
                    costQuote.ProductName = costs.ProductName;
                    costQuote.OrderDetailType = costs.OrderDetailType;
                    costs.SaleBiddingDetailProductAttribute = costs.SaleBiddingDetailProductAttribute == null ?
                    new List<SaleBiddingDetailProductAttributeEntityModel>() : costs.SaleBiddingDetailProductAttribute;
                    costs.SaleBiddingDetailProductAttribute.ForEach(map =>
                    {
                        var mappingProduct = new SaleBiddingDetailProductAttribute();
                        mappingProduct.ProductAttributeCategoryId = map.ProductAttributeCategoryId;
                        mappingProduct.ProductAttributeCategoryValueId = map.ProductAttributeCategoryValueId;
                        mappingProduct.ProductId = costs.ProductId;
                        mappingProduct.SaleBiddingDetailId = costQuote.CostsQuoteId;
                        mappingProduct.SaleBiddingDetailProductAttributeId = Guid.NewGuid();

                        context.SaleBiddingDetailProductAttribute.Add(mappingProduct);
                    });

                    context.CostsQuote.Add(costQuote);
                });

                #endregion

                #region Update tab chi tiêt hồ sơ thầu

                var folder = context.Folder.FirstOrDefault(x => x.Active == true && x.FolderType == "HST");

                if (folder == null)
                {
                    return new EditSaleBiddingResult()
                    {
                        Message = "Chưa có thư mục để lưu. Bạn phải cấu hình thư mục để lưu"
                    };
                }

                var listSaleBiddingDetailUpdate = context.SaleBiddingDetail.Where(x => x.SaleBiddingId == saleBiddingUpdate.SaleBiddingId).ToList();
                var listSaleBiddingDetailUpdateId = listSaleBiddingDetailUpdate.Select(x => x.SaleBiddingDetailId).ToList();
                var listFileUpdate = context.FileInFolder.Where(x => x.ObjectId != null && listSaleBiddingDetailUpdateId.Contains(x.ObjectId.Value)).ToList();
                context.SaleBiddingDetail.RemoveRange(listSaleBiddingDetailUpdate);

                var listFileNotDeleteId = new List<Guid>();


                parameter.ListSaleBiddingDetail.ForEach(item =>
                {
                    var saleBiddingDetail = new SaleBiddingDetail();
                    saleBiddingDetail.Category = item.Category;
                    saleBiddingDetail.Content = item.Content;
                    saleBiddingDetail.CreatedById = parameter.UserId;
                    saleBiddingDetail.CreatedDate = DateTime.Now;
                    saleBiddingDetail.SaleBiddingDetailId = Guid.NewGuid();
                    saleBiddingDetail.SaleBiddingId = saleBiddingUpdate.SaleBiddingId;
                    if (item.ListFile != null && item.ListFile.Count > 0)
                    {
                        item.ListFile.ForEach(file =>
                        {
                            var fileUpdate = listFileUpdate.FirstOrDefault(x => file.FileInFolderId == x.FileInFolderId);
                            if (fileUpdate != null)
                            {
                                fileUpdate.ObjectId = saleBiddingDetail.SaleBiddingDetailId;
                                fileUpdate.UpdatedById = parameter.UserId;
                                fileUpdate.UpdatedDate = DateTime.Now;
                                context.Update(fileUpdate);
                                listFileNotDeleteId.Add(fileUpdate.FileInFolderId);
                            }
                        });
                    }
                    if (item.ListFormFile != null && item.ListFormFile.Count > 0)
                    {
                        var folderName = folder.Url + "\\";
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (IFormFile file in item.ListFormFile)
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
                                fileInForder.FileName = fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + Guid.NewGuid();
                                fileInForder.FolderId = folder.FolderId;
                                fileInForder.ObjectId = saleBiddingDetail.SaleBiddingDetailId;
                                fileInForder.ObjectType = "HST";
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
                    context.SaleBiddingDetail.Add(saleBiddingDetail);
                });

                // Xóa file vật lí 
                var listFileDelete = listFileUpdate.Where(x => !listFileNotDeleteId.Contains(x.FileInFolderId)).ToList();
                if (listFileDelete.Count > 0)
                {
                    context.FileInFolder.RemoveRange(listFileDelete);
                    listFileDelete.ForEach(file =>
                    {
                        var fileName = folder.Url + "\\" + file.FileName + "." + file.FileExtension;
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, fileName);
                        if (Directory.Exists(newPath))
                        {
                            Directory.Delete(newPath);
                        }
                    });
                }

                #endregion

                context.SaveChanges();

                #region Gửi thông báo

                NotificationHelper.AccessNotification(context, TypeModel.SaleBiddingDetail, "UPD", new SaleBidding(),
                    saleBiddingUpdate, true);

                #endregion

                // Lấy lại danh sach chi tiết hồ sơ thầu
                var listSaleBiddingDetail = context.SaleBiddingDetail.Where(x => x.SaleBiddingId == saleBiddingUpdate.SaleBiddingId).Select(y =>
                new SaleBiddingDetailEntityModel()
                {
                    SaleBiddingId = y.SaleBiddingId,
                    SaleBiddingDetailId = y.SaleBiddingDetailId,
                    Category = y.Category,
                    Content = y.Content,
                    ListFile = new List<FileInFolderEntityModel>()
                }).ToList();

                var listSaleBiddingDetailtId = listSaleBiddingDetail.Select(x => x.SaleBiddingDetailId).ToList();
                var listFileInFolder = context.FileInFolder.Where(x => listSaleBiddingDetailtId.Contains(x.ObjectId) && x.ObjectType == "HST" && x.Active == true).
                    Select(y => new FileInFolderEntityModel()
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

                listSaleBiddingDetail.ForEach(item =>
                {
                    item.ListFile = listFileInFolder.Where(x => x.ObjectId == item.SaleBiddingDetailId).ToList();
                });

                return new EditSaleBiddingResult
                {
                    Status = true,
                    ListSaleBiddingDetail = listSaleBiddingDetail
                };
            }
            catch (Exception ex)
            {
                return new EditSaleBiddingResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public UpdateStatusSaleBiddingResult UpdateStatusSaleBidding(UpdateStatusSaleBiddingParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new UpdateStatusSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new UpdateStatusSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL
                    };
                }
                // Tìm người phụ trách của hồ sơ thầu


                var listEmployeeCommon = context.Employee.ToList();

                var noteResult = new NoteEntityModel();
                if (parameter.ListStaus == null || parameter.ListStaus.Count == 0)
                {
                    return new UpdateStatusSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL
                    };
                }
                var listSaleBiddingUpdateId = parameter.ListStaus.Select(x => x.SaleBiddingId).ToList();
                var listSaleBiddingUpdate = context.SaleBidding.Where(x => listSaleBiddingUpdateId.Contains(x.SaleBiddingId)).ToList();
                var listPersonInChargeId = listSaleBiddingUpdate.Select(x => x.PersonInChargeId).ToList();
                var listEmployee = listEmployeeCommon.Where(x => listPersonInChargeId.Contains(x.EmployeeId)).ToList();
                var listOrganizationCommon = context.Organization.ToList();
                var listOrganization = listEmployee.Select(x => x.OrganizationId).Distinct().ToList();
                var listOrganizationId = new List<Guid?>();
                listOrganizationId.AddRange(listOrganization);
                // Lấy tất cả nhân viên quản lí của người phụ trách của tất cả hồ sơ thầu đang được đổi trạng thái
                listOrganization.ForEach(item =>
                {
                    var tempOrg = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == item && x.Active == true);
                    if (tempOrg != null)
                    {
                        GetOrganizationParentId(tempOrg, listOrganizationCommon, listOrganizationId);
                    }
                });
                var listEmployeeManager = listEmployeeCommon.Where(x => listOrganizationId.Contains(x.OrganizationId) && x.IsManager == true).ToList();
                var isManagerLogin = listEmployeeManager.FirstOrDefault(x => x.EmployeeId == user.EmployeeId) != null;
                if (!listPersonInChargeId.Contains(user.EmployeeId.Value) && !isManagerLogin)
                {
                    return new UpdateStatusSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL_ROLE
                    };
                }
                var listQuocte = context.Quote.Where(x => listSaleBiddingUpdateId.Contains(x.SaleBiddingId.Value)).ToList();
                var listCategory = context.Category.ToList();
                var listSaleBiddingCommon = context.SaleBidding.ToList();
                var isSave = true;
                parameter.ListStaus.ForEach(item =>
                {
                    if (isSave)
                    {
                        var saleBidding = listSaleBiddingCommon.FirstOrDefault(x => x.SaleBiddingId == item.SaleBiddingId);
                        if (saleBidding == null)
                        {
                            isSave = false;
                        }
                        var statusSaleBidding = listCategory.FirstOrDefault(x => x.CategoryId == saleBidding?.StatusId);
                        var status = listCategory.FirstOrDefault(x => x.CategoryId == item.StatusId);

                        if (item.StatusId == null || item.StatusId == Guid.Empty || status == null)
                        {
                            isSave = false;
                        }
                        if (status.CategoryCode == "REFU")
                        {
                            if (statusSaleBidding.CategoryCode != "CHO")
                            {
                                isSave = false;
                            }
                            isSave = listEmployeeManager.FirstOrDefault(x => x.EmployeeId == user.EmployeeId) != null;
                            if (item.Note == null || item.Note.Trim().Length == 0)
                            {
                                isSave = false;
                            }
                            var note = new Note()
                            {
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                ObjectId = item.SaleBiddingId,
                                Description = item.Note,
                                NoteId = Guid.NewGuid(),
                                NoteTitle = "dã từ chối phê duyệt hồ sơ thầu",
                                ObjectType = "HST",
                                Type = "ADD"
                            };

                            context.Note.Add(note);
                            noteResult.NoteId = note.NoteId;
                            noteResult.Description = note.Description;
                            noteResult.Type = note.Type;
                            noteResult.ObjectId = note.ObjectId;
                            noteResult.ObjectType = note.ObjectType;
                            noteResult.NoteTitle = note.NoteTitle;
                            noteResult.Active = note.Active;
                            noteResult.CreatedById = note.CreatedById;
                            noteResult.CreatedDate = note.CreatedDate;
                            noteResult.UpdatedById = note.UpdatedById;
                            noteResult.UpdatedDate = note.UpdatedDate;
                            noteResult.ResponsibleName = "";
                            noteResult.ResponsibleAvatar = "";
                            noteResult.NoteDocList = new List<NoteDocumentEntityModel>();

                            //Gửi email thông báo
                            #region Gửi thông báo

                            if (saleBidding != null)
                            {
                                saleBidding.UpdatedById = parameter.UserId;
                                saleBidding.UpdatedDate = DateTime.Now;
                                NotificationHelper.AccessNotification(context, TypeModel.SaleBiddingDetail,
                                    "REJECT", new SaleBidding(), saleBidding, true, note);
                            }

                            #endregion

                        }
                        else if (status.CategoryCode == "CANC")
                        {
                            var quote = listQuocte.FirstOrDefault(x => x.SaleBiddingId == item.SaleBiddingId);
                            if (quote != null)
                            {
                                isSave = false;
                            }
                            else
                            {
                                var note = new Note()
                                {
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    ObjectId = item.SaleBiddingId,
                                    Description = item.Note,
                                    NoteId = Guid.NewGuid(),
                                    NoteTitle = "dã huỷ hồ sơ thầu",
                                    ObjectType = "HST",
                                    Type = "ADD"
                                };

                                context.Note.Add(note);
                                noteResult.NoteId = note.NoteId;
                                noteResult.Description = note.Description;
                                noteResult.Type = note.Type;
                                noteResult.ObjectId = note.ObjectId;
                                noteResult.ObjectType = note.ObjectType;
                                noteResult.NoteTitle = note.NoteTitle;
                                noteResult.Active = note.Active;
                                noteResult.CreatedById = note.CreatedById;
                                noteResult.CreatedDate = note.CreatedDate;
                                noteResult.UpdatedById = note.UpdatedById;
                                noteResult.UpdatedDate = note.UpdatedDate;
                                noteResult.ResponsibleName = "";
                                noteResult.ResponsibleAvatar = "";
                                noteResult.NoteDocList = new List<NoteDocumentEntityModel>();
                            }
                        }
                        else if (status.CategoryCode == "NEW")
                        {
                            var statusApp = listCategory.FirstOrDefault(x => x.CategoryId == saleBidding.StatusId && (x.CategoryCode == "CANC" || x.CategoryCode == "REFU" || x.CategoryCode == "CHO"));
                            if (statusApp == null)
                            {
                                isSave = false;
                            }
                            else
                            {
                                var note = new Note()
                                {
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    ObjectId = item.SaleBiddingId,
                                    Description = item.Note,
                                    NoteId = Guid.NewGuid(),
                                    ObjectType = "HST",
                                    Type = "ADD"
                                };

                                if (statusApp.CategoryCode == "CHO")
                                {
                                    note.NoteTitle = "Hồ sơ thầu đã được hủy yêu cầu phê duyệt bởi nhân viên: " +
                                                     user.UserName;
                                }
                                else
                                {
                                    note.NoteTitle = "dã đặt về nháp hồ sơ thầu";
                                }



                                context.Note.Add(note);
                                noteResult.NoteId = note.NoteId;
                                noteResult.Description = note.Description;
                                noteResult.Type = note.Type;
                                noteResult.ObjectId = note.ObjectId;
                                noteResult.ObjectType = note.ObjectType;
                                noteResult.NoteTitle = note.NoteTitle;
                                noteResult.Active = note.Active;
                                noteResult.CreatedById = note.CreatedById;
                                noteResult.CreatedDate = note.CreatedDate;
                                noteResult.UpdatedById = note.UpdatedById;
                                noteResult.UpdatedDate = note.UpdatedDate;
                                noteResult.ResponsibleName = "";
                                noteResult.ResponsibleAvatar = "";
                                noteResult.NoteDocList = new List<NoteDocumentEntityModel>();

                            }
                            if (statusSaleBidding.CategoryCode != "CHO" && statusSaleBidding.CategoryCode != "CANC" && statusSaleBidding.CategoryCode != "REFU")
                            {
                                isSave = false;
                            }
                            else
                            {
                                #region Gửi thông báo

                                if (saleBidding != null)
                                {
                                    NotificationHelper.AccessNotification(context, TypeModel.SaleBiddingDetail,
                                        "CANCEL_APPROVAL", new SaleBidding(),
                                        saleBidding, true);
                                }

                                #endregion
                            }

                        }
                        else if (status.CategoryCode == "CHO")
                        {
                            var statusApp = listCategory.FirstOrDefault(x => x.CategoryId == saleBidding.StatusId && x.CategoryCode == "NEW");
                            if (statusApp == null)
                            {
                                isSave = false;
                            }
                            else
                            {
                                var note = new Note()
                                {
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    ObjectId = item.SaleBiddingId,
                                    Description = item.Note,
                                    NoteId = Guid.NewGuid(),
                                    NoteTitle = "dã yêu cầu phê duyệt hồ sơ thầu",
                                    ObjectType = "HST",
                                    Type = "ADD"
                                };

                                context.Note.Add(note);
                                noteResult.NoteId = note.NoteId;
                                noteResult.Description = note.Description;
                                noteResult.Type = note.Type;
                                noteResult.ObjectId = note.ObjectId;
                                noteResult.ObjectType = note.ObjectType;
                                noteResult.NoteTitle = note.NoteTitle;
                                noteResult.Active = note.Active;
                                noteResult.CreatedById = note.CreatedById;
                                noteResult.CreatedDate = note.CreatedDate;
                                noteResult.UpdatedById = note.UpdatedById;
                                noteResult.UpdatedDate = note.UpdatedDate;
                                noteResult.ResponsibleName = "";
                                noteResult.ResponsibleAvatar = "";
                                noteResult.NoteDocList = new List<NoteDocumentEntityModel>();

                                //Gửi email thông báo
                                #region Gửi thông báo

                                if (saleBidding != null)
                                {
                                    saleBidding.UpdatedById = parameter.UserId;
                                    saleBidding.UpdatedDate = DateTime.Now;
                                    NotificationHelper.AccessNotification(context, TypeModel.SaleBiddingDetail,
                                        "SEND_APPROVAL", new SaleBidding(),
                                        saleBidding, true);
                                }

                                #endregion
                            }
                            if (statusSaleBidding.CategoryCode != "NEW")
                            {
                                isSave = false;
                            }

                        }
                        else if (status.CategoryCode == "LOSE")
                        {
                            var statusApp = listCategory.FirstOrDefault(x => x.CategoryId == saleBidding.StatusId && x.CategoryCode == "APPR");
                            if (statusApp == null)
                            {
                                isSave = false;
                            }
                            if (statusSaleBidding.CategoryCode != "APPR")
                            {
                                isSave = false;
                            }

                            var note = new Note()
                            {
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                ObjectId = item.SaleBiddingId,
                                Description = item.Note,
                                NoteId = Guid.NewGuid(),
                                NoteTitle = "dã đánh dấu thua hồ sơ thầu",
                                ObjectType = "HST",
                                Type = "ADD"
                            };

                            context.Note.Add(note);
                            noteResult.NoteId = note.NoteId;
                            noteResult.Description = note.Description;
                            noteResult.Type = note.Type;
                            noteResult.ObjectId = note.ObjectId;
                            noteResult.ObjectType = note.ObjectType;
                            noteResult.NoteTitle = note.NoteTitle;
                            noteResult.Active = note.Active;
                            noteResult.CreatedById = note.CreatedById;
                            noteResult.CreatedDate = note.CreatedDate;
                            noteResult.UpdatedById = note.UpdatedById;
                            noteResult.UpdatedDate = note.UpdatedDate;
                            noteResult.ResponsibleName = "";
                            noteResult.ResponsibleAvatar = "";
                            noteResult.NoteDocList = new List<NoteDocumentEntityModel>();
                        }
                        else if (status.CategoryCode == "APPR")
                        {
                            isSave = listEmployeeManager.FirstOrDefault(x => x.EmployeeId == user.EmployeeId) != null;
                            if (statusSaleBidding.CategoryCode != "CHO")
                            {
                                isSave = false;
                            }

                            var note = new Note()
                            {
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                ObjectId = item.SaleBiddingId,
                                Description = item.Note,
                                NoteId = Guid.NewGuid(),
                                NoteTitle = "dã duyệt hồ sơ thầu",
                                ObjectType = "HST",
                                Type = "ADD"
                            };

                            context.Note.Add(note);
                            noteResult.NoteId = note.NoteId;
                            noteResult.Description = note.Description;
                            noteResult.Type = note.Type;
                            noteResult.ObjectId = note.ObjectId;
                            noteResult.ObjectType = note.ObjectType;
                            noteResult.NoteTitle = note.NoteTitle;
                            noteResult.Active = note.Active;
                            noteResult.CreatedById = note.CreatedById;
                            noteResult.CreatedDate = note.CreatedDate;
                            noteResult.UpdatedById = note.UpdatedById;
                            noteResult.UpdatedDate = note.UpdatedDate;
                            noteResult.ResponsibleName = "";
                            noteResult.ResponsibleAvatar = "";
                            noteResult.NoteDocList = new List<NoteDocumentEntityModel>();

                            //Gửi email thông báo
                            #region Gửi thông báo

                            if (saleBidding != null)
                            {
                                saleBidding.UpdatedById = parameter.UserId;
                                saleBidding.UpdatedDate = DateTime.Now;
                                NotificationHelper.AccessNotification(context, TypeModel.SaleBiddingDetail,
                                    "APPROVAL", new SaleBidding(),
                                    saleBidding, true, note);
                            }

                            #endregion

                        }
                        saleBidding.StatusId = item.StatusId;
                        saleBidding.UpdatedById = parameter.UserId;
                        saleBidding.UpdatedDate = DateTime.Now;

                        context.SaleBidding.Update(saleBidding);
                    }

                });
                if (isSave)
                {
                    context.SaveChanges();
                }
                else
                {
                    return new UpdateStatusSaleBiddingResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.EDIT_FAIL
                    };
                }


                return new UpdateStatusSaleBiddingResult
                {
                    Status = true,
                    Note = noteResult
                };
            }
            catch (Exception)
            {
                return new UpdateStatusSaleBiddingResult
                {
                    Status = false,
                    Message = CommonMessage.SaleBidding.EDIT_FAIL
                };
            }
        }

        public GetVendorMappingResult GetVendorMapping(GetVendorMappingParameter parameter)
        {
            try
            {
                var listVendorMapping = context.ProductVendorMapping.Where(x => x.ProductId == parameter.ProductId
                                                            && x.Active == true
                                                            && x.FromDate.Value.Date <= DateTime.Now.Date
                                                            && (x.ToDate == null || x.ToDate >= DateTime.Now.Date))
                .Select(y => new ProductVendorMappingEntityModel()
                {
                    FromDate = y.FromDate.Value,
                    MiniumQuantity = y.MiniumQuantity.Value,
                    Price = y.Price.Value,
                    ProductVendorMappingId = y.ProductVendorMappingId,
                    ToDate = y.ToDate,
                    Active = y.Active,
                    ProductId = y.ProductId,
                    VendorId = y.VendorId
                }).ToList();
                return new GetVendorMappingResult()
                {
                    Status = true,
                    ListVendor = listVendorMapping
                };
            }
            catch (Exception ex)
            {
                return new GetVendorMappingResult()
                {
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public GetMasterDataSaleBiddingApprovedResult GetMasterDataSaleBiddingApproved(GetMasterDataSaleBiddingApprovedParameter parameter)
        {
            try
            {

                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new GetMasterDataSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetMasterDataSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }

                // Kiểm tra coi người đang đăng nhập có phải là quản lí hay không
                var employeeLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.Active == true);
                if (employeeLogin == null)
                {
                    return new GetMasterDataSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }

                if (!employeeLogin.IsManager)
                {
                    return new GetMasterDataSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }

                #region Lấy danh sách nhân viên

                // Lấy danh sách tất cả phòng ban cấp dưới 
                var listOrganizationCommon = context.Organization.ToList();
                var listOrganizationId = new List<Guid?>();
                var listOrganization = _getOrganizationChildrenId(listOrganizationCommon, employeeLogin.OrganizationId, listOrganizationId);
                listOrganization.Add(employeeLogin.OrganizationId);

                var listEmployeeCommon = context.Employee.Where(x => listOrganization.Contains(x.OrganizationId)).ToList();
                var listEmployeeResult = listEmployeeCommon.Select(y => new EmployeeEntityModel()
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeName = y.EmployeeName
                }).ToList();
                var listEmployeeResultId = listEmployeeCommon.Select(x => x.EmployeeId).ToList();
                listEmployeeResultId.Add(employeeLogin.EmployeeId);

                #endregion

                #region Lấy danh sách trạng thái hồ sơ thầu

                var categoryTypeId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HST").CategoryTypeId;

                var listStatusResult = commonCategory.Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true).Select(y =>
                new CategoryEntityModel()
                {
                    CategoryId = y.CategoryId,
                    CategoryName = y.CategoryName,
                    CategoryCode = y.CategoryCode,
                }).ToList();

                #endregion


                #region Lấy danh sách tât cả khách hàng định danh


                var categoryTypeCustomerId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                var categoryCustomerId = commonCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeCustomerId && x.CategoryCode == "HDO").CategoryId;

                var listCustomerResult = context.Customer.Where(x => x.StatusId == categoryCustomerId && x.Active == true).Select(y => new CustomerEntityModel()
                {
                    CustomerId = y.CustomerId,
                    CustomerName = y.CustomerName,
                    CustomerCode = y.CustomerCode,
                    CustomerGroupId = y.CustomerGroupId,
                    PersonInChargeId = y.PersonInChargeId
                }).ToList();

                #endregion

                // Lấy id trạng thái chờ phê duyệt
                var statusTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HST").CategoryTypeId;
                var status = context.Category.FirstOrDefault(x => x.CategoryTypeId == statusTypeId && x.CategoryCode == "CHO");

                #region Lấy danh sách hồ sơ thầu ở trạng thái chờ phê duyệt

                var listSaleBidding = context.SaleBidding.Where(x => listEmployeeResultId.Contains(x.PersonInChargeId) && x.StatusId == status.CategoryId
                ).Select(y => new SaleBiddingEntityModel()
                {
                    Address = y.Address,
                    BidStartDate = y.BidStartDate,
                    CreateDate = y.CreatedDate,
                    CurrencyUnitId = y.CurrencyUnitId,
                    CustomerId = y.CustomerId,
                    EffecTime = y.EffecTime,
                    EmployeeId = y.EmployeeId,
                    EndDate = y.EndDate,
                    FormOfBid = y.FormOfBid,
                    LeadId = y.LeadId,
                    SaleBiddingName = y.SaleBiddingName,
                    PersonInChargeId = y.PersonInChargeId,
                    SaleBiddingCode = y.SaleBiddingCode,
                    SaleBiddingId = y.SaleBiddingId,
                    StatusId = y.StatusId,
                    ValueBid = y.ValueBid,
                    TypeContractId = y.TypeContractId,
                    StartDate = y.StartDate,
                    StatusName = status.CategoryName
                }).ToList();

                var listPersonInChargeId = listSaleBidding.Select(x => x.PersonInChargeId).ToList();
                var employees = context.Employee.Where(x => listPersonInChargeId.Contains(x.EmployeeId)).Select(x => new EmployeeEntityModel()
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.EmployeeName
                }).ToList();

                listEmployeeResult.ForEach(item =>
                {
                    var isAddEmp = employees.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    if (isAddEmp != null)
                    {
                        employees.Remove(isAddEmp);
                    }
                });

                listEmployeeResult.AddRange(employees);
                // Gán tên nhân viên phụ trách và tên khách hàng
                listSaleBidding.ForEach(item =>
                {
                    var employee = listEmployeeResult.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                    if (employee != null)
                    {
                        item.PersonInChargeName = employee.EmployeeName;
                    }

                    var customer = listCustomerResult.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                    if (customer != null)
                    {
                        item.CustomerName = customer.CustomerName;
                    }
                });

                #endregion

                return new GetMasterDataSaleBiddingApprovedResult
                {
                    Status = true,
                    ListSaleBidding = listSaleBidding,
                    ListCustomer = listCustomerResult,
                    ListEmployee = listEmployeeResult,
                    ListStatus = listStatusResult
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSaleBiddingApprovedResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
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

        private List<Guid?> GetOrganizationParentId(Organization organization, List<Organization> listOrganization, List<Guid?> listParentId)
        {
            if (organization.ParentId != null)
            {
                var organizationCommon = listOrganization.Where(x => organization.ParentId == x.OrganizationId).ToList();
                organizationCommon.ForEach(item =>
                {
                    listParentId.Add(item.OrganizationId);
                    GetOrganizationParentId(item, listOrganization, listParentId);
                });
            }
            return listParentId;
        }

        public SearchSaleBiddingApprovedResult SearchSaleBiddingApproved(SearchSaleBiddingApprovedParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new SearchSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }

                // Kiểm tra coi người đang đăng nhập có phải là quản lí hay không
                var employeeLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.Active == true);
                if (employeeLogin == null)
                {
                    return new SearchSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }

                if (!employeeLogin.IsManager)
                {
                    return new SearchSaleBiddingApprovedResult
                    {
                        Status = true,
                        Message = CommonMessage.SaleBidding.NOT_FOUND_LIST_SALEBIDDING
                    };
                }

                #region Lấy danh sách nhân viên

                // Lấy danh sách tất cả phòng ban cấp dưới 
                var listOrganizationCommon = context.Organization.ToList();
                var listOrganizationId = new List<Guid?>();
                var listOrganization = _getOrganizationChildrenId(listOrganizationCommon, employeeLogin.OrganizationId, listOrganizationId);
                listOrganization.Add(employeeLogin.OrganizationId);

                var listEmployeeCommon = context.Employee.Where(x => listOrganization.Contains(x.OrganizationId)).ToList();
                var listEmployeeResult = listEmployeeCommon.Select(y => new EmployeeEntityModel()
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeName = y.EmployeeName
                }).ToList();
                var listEmployeeResultId = listEmployeeCommon.Select(x => x.EmployeeId).ToList();

                #endregion

                #region Lấy danh sách tât cả khách hàng định danh

                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();

                var listCustomerResult = context.Customer.Where(x => x.Active == true).Select(y => new CustomerEntityModel()
                {
                    CustomerId = y.CustomerId,
                    CustomerName = y.CustomerName,
                    CustomerCode = y.CustomerCode,
                    CustomerGroupId = y.CustomerGroupId,
                    PersonInChargeId = y.PersonInChargeId
                }).ToList();

                #endregion

                // Lấy id trạng thái chờ phê duyệt
                var statusTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HST").CategoryTypeId;
                var status = context.Category.FirstOrDefault(x => x.CategoryTypeId == statusTypeId && x.CategoryCode == "CHO");

                if (parameter.IsApproved)
                {
                    status = context.Category.FirstOrDefault(x => x.CategoryTypeId == statusTypeId && x.CategoryCode == "APPR");
                }

                #region Lấy danh sách hồ sơ thầu ở trạng thái chờ phê duyệt

                var listSaleBidding = context.SaleBidding.Where(x =>
                                                                listEmployeeResultId.Contains(x.PersonInChargeId) &&
                                                                (parameter.SaleBiddingName == null || parameter.SaleBiddingName == "" || x.SaleBiddingName.ToLower().Contains(parameter.SaleBiddingName.ToLower())) &&
                                                                (parameter.EmployeeId == null || parameter.EmployeeId.Count == 0 || parameter.EmployeeId.Contains(x.PersonInChargeId)) &&
                                                                (parameter.BidStartDateForm == null || x.BidStartDate.Value.Date >= parameter.BidStartDateForm.Value.Date) &&
                                                                (parameter.BidStartDateTo == null || x.BidStartDate.Value.Date <= parameter.BidStartDateTo.Value.Date) &&
                                                                (x.StatusId == status.CategoryId)
                ).Select(y => new SaleBiddingEntityModel()
                {
                    Address = y.Address,
                    BidStartDate = y.BidStartDate,
                    CreateDate = y.CreatedDate,
                    CurrencyUnitId = y.CurrencyUnitId,
                    CustomerId = y.CustomerId,
                    EffecTime = y.EffecTime,
                    EmployeeId = y.EmployeeId,
                    EndDate = y.EndDate,
                    FormOfBid = y.FormOfBid,
                    LeadId = y.LeadId,
                    PersonInChargeId = y.PersonInChargeId,
                    SaleBiddingCode = y.SaleBiddingCode,
                    SaleBiddingId = y.SaleBiddingId,
                    StatusId = y.StatusId,
                    ValueBid = y.ValueBid,
                    TypeContractId = y.TypeContractId,
                    StartDate = y.StartDate,
                    StatusName = status.CategoryName,
                    SaleBiddingName = y.SaleBiddingName
                }).ToList();

                var listPersonInChargeId = listSaleBidding.Select(x => x.PersonInChargeId).ToList();
                var employees = context.Employee.Where(x => listPersonInChargeId.Contains(x.EmployeeId)).Select(x => new EmployeeEntityModel()
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.EmployeeName
                }).ToList();

                listEmployeeResult.ForEach(item =>
                {
                    var isAddEmp = employees.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    if (isAddEmp != null)
                    {
                        employees.Remove(isAddEmp);
                    }
                });

                listEmployeeResult.AddRange(employees);

                // Gán tên nhân viên phụ trách và tên khách hàng
                listSaleBidding.ForEach(item =>
                {
                    var employee = listEmployeeResult.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                    if (employee != null)
                    {
                        item.PersonInChargeName = employee.EmployeeName;
                    }

                    var customer = listCustomerResult.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                    if (customer != null)
                    {
                        item.CustomerName = customer.CustomerName;
                    }
                });

                var listSaleBiddingResult = listSaleBidding.Where(x => parameter.CustomerName == null || parameter.CustomerName.Trim().Length == 0 || parameter.CustomerName.Trim().Length == 0 || x.CustomerName.ToLower().Contains(parameter.CustomerName.ToLower())).OrderByDescending(y => y.CreateDate).ToList();

                #endregion

                return new SearchSaleBiddingApprovedResult
                {
                    Status = true,
                    ListSaleBidding = listSaleBiddingResult
                };
            }
            catch (Exception ex)
            {
                return new SearchSaleBiddingApprovedResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public GetCustomerByEmployeeIdResult GetCustomerByEmployeeId(GetCustomerByEmployeeIdParameter parameter)
        {
            try
            {
                var listOrganizationCommon = context.Organization.ToList();
                var listEmployeeCommon = context.Employee.ToList();
                var personInCharge = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId);

                if (personInCharge == null)
                {
                    return new GetCustomerByEmployeeIdResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_FAIL
                    };
                }
                var orgPersonInCharge = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == personInCharge.OrganizationId);
                if (orgPersonInCharge == null)
                {
                    return new GetCustomerByEmployeeIdResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_FAIL
                    };
                }
                var org = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == personInCharge.OrganizationId);
                if (org == null)
                {
                    return new GetCustomerByEmployeeIdResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_FAIL
                    };
                }
                var listorgPersonInChargeResult = new List<EmployeeEntityModel>();
                if (personInCharge.IsManager)
                {
                    // Lấy danh sách tất cả nhân viên của quản lí đang hoạt động của các phòng ban
                    var listOrgId = new List<Guid?>();
                    listOrgId = _getOrganizationChildrenId(listOrganizationCommon, orgPersonInCharge.OrganizationId, listOrgId);
                    listOrgId.Add(org.OrganizationId);

                    listorgPersonInChargeResult = listEmployeeCommon.Where(x => x.Active == true && listOrgId.Contains(x.OrganizationId)).Select(y => new EmployeeEntityModel()
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeName = y.EmployeeName,
                        Active = y.Active
                    }).ToList();
                    var isAdd = listorgPersonInChargeResult.FirstOrDefault(x => x.EmployeeId == personInCharge.EmployeeId) == null;
                    if (isAdd)
                    {
                        listorgPersonInChargeResult.Add(new EmployeeEntityModel()
                        {
                            EmployeeId = personInCharge.EmployeeId,
                            EmployeeName = personInCharge.EmployeeName,
                            Active = personInCharge.Active
                        });
                    }
                }
                else
                {
                    listorgPersonInChargeResult.Add(new EmployeeEntityModel()
                    {
                        EmployeeId = personInCharge.EmployeeId,
                        EmployeeName = personInCharge.EmployeeName,
                        Active = personInCharge.Active
                    });
                }

                #region Lấy danh sách tât cả khách hàng định danh

                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();

                var categoryTypeCustomerId = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                var categoryCustomerId = commonCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeCustomerId && x.CategoryCode == "HDO").CategoryId;
                // Lấy dánh sách tất cả khách hàng của người phụ trách hoặc cấp dưới của người phụ trách
                var listEmployeePersonInChargeId = listorgPersonInChargeResult.Select(x => x.EmployeeId).ToList();

                var listCustomerResult = context.Customer.Where(x => x.StatusId == categoryCustomerId && x.Active == true && listEmployeePersonInChargeId.Contains(x.PersonInChargeId)).Select(y => new CustomerEntityModel()
                {
                    CustomerId = y.CustomerId,
                    CustomerName = y.CustomerName,
                    CustomerCode = y.CustomerCode,
                    CustomerGroupId = y.CustomerGroupId,
                    PersonInChargeId = y.PersonInChargeId
                }).ToList();
                var listCustomerId = listCustomerResult.Select(x => x.CustomerId).ToList();
                var listEmployeeId = listCustomerResult.Select(x => x.PersonInChargeId).Distinct().ToList();

                var listEmployeeCustomer = listEmployeeCommon.Where(x => listEmployeeId.Contains(x.EmployeeId)).ToList();
                var listCustomerGroupId = listCustomerResult.Select(x => x.CustomerGroupId).Distinct().ToList();
                var listCustomerGroup = commonCategory.Where(x => listCustomerGroupId.Contains(x.CategoryId)).ToList();
                // Lấy danh sách contact
                var commonContact = context.Contact.Where(x => listCustomerId.Contains(x.ObjectId)).ToList();

                // Lấy danh sách xã
                var listWardId = commonContact.Select(x => x.WardId).ToList();
                var listWardCommon = context.Ward.Where(x => listWardId.Contains(x.WardId)).ToList();
                // Lấy danh sách phường
                var listDistrictId = commonContact.Select(x => x.DistrictId).ToList();
                var listDistrictCommon = context.District.Where(x => listDistrictId.Contains(x.DistrictId)).ToList();
                // Lấy danh sách tỉnh
                var listProvinceId = commonContact.Select(x => x.ProvinceId).ToList();
                var listProvinceCommon = context.Province.Where(x => listProvinceId.Contains(x.ProvinceId)).ToList();
                listCustomerResult.ForEach(item =>
                {
                    var temp = commonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS");
                    var ward = listWardCommon.FirstOrDefault(x => x.WardId == temp.WardId);
                    var district = listDistrictCommon.FirstOrDefault(x => x.DistrictId == temp.DistrictId);
                    var province = listProvinceCommon.FirstOrDefault(x => x.ProvinceId == temp.ProvinceId);
                    if (temp != null)
                    {
                        item.FullAddress = temp.Address;
                    }

                    if (ward != null)
                    {
                        item.FullAddress = item.FullAddress + "," + ward.WardName;
                    }

                    if (district != null)
                    {
                        item.FullAddress = item.FullAddress + "," + district.DistrictName;
                    }

                    if (province != null)
                    {
                        item.FullAddress = item.FullAddress + "," + province.ProvinceName;
                    }
                    item.CustomerPhone = temp?.Phone;
                    item.TaxCode = temp?.TaxCode;
                    item.CustomerGroup = listCustomerGroup.FirstOrDefault(x => x.CategoryId == item.CustomerGroupId)?.CategoryName;
                    item.PersonInCharge = listEmployeeCustomer.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId)?.EmployeeName;
                });

                #endregion

                return new GetCustomerByEmployeeIdResult
                {
                    Status = true,
                    ListCustomer = listCustomerResult
                };
            }
            catch (Exception ex)
            {
                return new GetCustomerByEmployeeIdResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public decimal? GetRos(Guid saleBiddingId, List<CostsQuote> listCostsQuoteCommon)
        {

            decimal result = 0;
            decimal sumCost = 0;
            decimal sumQuote = 0;
            var listCost = listCostsQuoteCommon.Where(x => x.SaleBiddingId == saleBiddingId && x.CostsQuoteType == 1).ToList();

            listCost.ForEach(item =>
            {
                decimal CacuDiscount = 0;
                if (item.DiscountValue != null)
                {
                    if (item.DiscountType == true)
                    {
                        CacuDiscount = ((item.Quantity.Value * item.UnitPrice.Value * item.ExchangeRate.Value * item.DiscountValue.Value) / 100);
                    }
                    else
                    {
                        CacuDiscount = item.DiscountValue.Value;
                    }
                }
                sumCost = sumCost + item.Quantity.Value * item.UnitPrice.Value - CacuDiscount;
            });

            var listQuote = listCostsQuoteCommon.Where(x => x.SaleBiddingId == saleBiddingId && x.CostsQuoteType == 2).ToList();
            listQuote.ForEach(item =>
            {
                decimal CacuDiscount = 0;
                if (item.DiscountValue != null)
                {
                    if (item.DiscountType == true)
                    {
                        CacuDiscount = ((item.Quantity.Value * item.UnitPrice.Value * item.ExchangeRate.Value * item.DiscountValue.Value) / 100);
                    }
                    else
                    {
                        CacuDiscount = item.DiscountValue.Value;
                    }
                }
                sumQuote = sumQuote + item.Quantity.Value * item.UnitPrice.Value - CacuDiscount;
            });

            if (sumQuote == 0)
            {
                return 0;
            }
            result = (sumQuote - sumCost) * 100 / sumQuote;


            return result;
        }

        public SendEmailEmployeeResult SendEmailEmployee(SendEmailEmployeeParameter parameter)
        {
            try
            {
                var saleBidding = context.SaleBidding.FirstOrDefault(q => q.SaleBiddingId == parameter.SaleBiddingId);

                if (saleBidding == null)
                {
                    return new SendEmailEmployeeResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.SEND_EMAIL_FAIL
                    };
                }
                var tenantHost = context.Tenants.FirstOrDefault(x => x.TenantId == saleBidding.TenantId).TenantHost;
                // Lấy danh sách nhân viên tham gia hồ sơ thầu
                var listEmployeeId = context.SaleBiddingEmployeeMapping.Where(x => x.SaleBiddingId == saleBidding.SaleBiddingId).Select(y => y.EmployeeId).ToList();
                var customerName = context.Customer.FirstOrDefault(x => x.CustomerId == saleBidding.CustomerId).CustomerName;
                if (listEmployeeId.Count == 0)
                {
                    return new SendEmailEmployeeResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.SEND_EMAIL_FAIL
                    };
                }

                var listContactEmail = context.Contact.Where(x => x.Email != null && x.Email.Trim() != "" && listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP").Select(y => y.Email).ToList();
                var body = "";
                body = body + "Có yêu cầu hỗ trợ hoàn thành <a href=\"http://" + tenantHost + "/sale-bidding/detail;saleBiddingId=" + saleBidding.SaleBiddingId + "\">" + saleBidding.SaleBiddingName + "</a>" + " - <b>" + customerName + "</b> cần được thực hiện <br />";
                body = body + "Link hồ sơ thầu: <a href=\"http://" + tenantHost + "/sale-bidding/detail;saleBiddingId=" + saleBidding.SaleBiddingId + "\">" + saleBidding.SaleBiddingCode + "</a><br />";
                body = body + "Vui lòng thực hiện để tiếp tục quy trình xử lý công việc của Hồ sơ thầu này <br />";
                body = body + "Xin trân trọng cảm ơn!";

                listContactEmail.ForEach(item =>
                {

                    try
                    {
                        GetConfiguration();
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                        mail.From = new MailAddress(Email, "N8");
                        mail.To.Add(item); // Email người nhận
                        mail.Subject = string.Format("Yêu cầu hoàn thành Hồ sơ thầu - " + saleBidding.SaleBiddingName);
                        mail.Body = body.ToString();
                        mail.IsBodyHtml = true;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                        SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;

                        SmtpServer.Send(mail);
                    }
                    catch
                    {
                        //throw;
                    }
                });

                saleBidding.IsSupport = true;
                context.SaleBidding.Update(saleBidding);
                context.SaveChanges();

                return new SendEmailEmployeeResult
                {
                    Status = true,
                    Message = CommonMessage.SaleBidding.SEND_EMAIL_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new SendEmailEmployeeResult
                {
                    Status = false,
                    Message = CommonMessage.SaleBidding.SEND_EMAIL_FAIL
                };
            }
        }

        public GetPersonInChargeByCustomerIdResult GetPersonInChargeByCustomerId(GetPersonInChargeByCustomerIdParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new GetPersonInChargeByCustomerIdResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_USER
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetPersonInChargeByCustomerIdResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_EMPLOYEE
                    };
                }
                var listEmployeeCommon = context.Employee.ToList();
                var employeeLogin = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.Active == true);
                if (employeeLogin == null)
                {
                    return new GetPersonInChargeByCustomerIdResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.NOT_EMPLOYEE
                    };
                }
                var listOrganizationCommon = context.Organization.ToList();
                var customerLead = context.Customer.FirstOrDefault(x => x.CustomerId == parameter.CustomerId);
                if (customerLead == null)
                {
                    return new GetPersonInChargeByCustomerIdResult
                    {
                        Status = false,
                        Message = CommonMessage.SaleBidding.GET_PERSON_FAIL
                    };
                }

                var listPersonResult = new List<EmployeeEntityModel>();// Danh sách người phụ trách 
                // Check coi người đang nhập là quản lí hay là nhân viên
                if (employeeLogin.IsManager)
                {
                    #region Lấy danh sách người phụ trách hồ sơ thầu theo người đang đăng nhập

                    // Là quản lí đăng nhập thì lấy danh sách tất cả các nhân viên cấp dưới của nó
                    var organization = listOrganizationCommon.FirstOrDefault(x => x.OrganizationId == employeeLogin.OrganizationId && x.Active == true);
                    if (organization != null)
                    {
                        var listPersonCustomerId = new List<Guid>();

                        // Lấy nhân viên phụ trách khách hàng ở cơ hội
                        var personCustomerLead = listEmployeeCommon.FirstOrDefault(x => x.EmployeeId == customerLead.PersonInChargeId);
                        listPersonCustomerId.Add(personCustomerLead.EmployeeId);
                        // Lấy danh sách cấp dưới và người phụ trách khách hàng
                        if (personCustomerLead.IsManager)
                        {
                            var listOrgPersonCustomerId = new List<Guid?>();
                            _getOrganizationChildrenId(listOrganizationCommon, personCustomerLead.OrganizationId, listOrgPersonCustomerId);
                            listOrgPersonCustomerId.Add(personCustomerLead.OrganizationId);
                            var listPersonCustomer = listEmployeeCommon.Where(x => x.Active == true && listOrgPersonCustomerId.Contains(x.OrganizationId)).ToList();
                            listPersonCustomerId = listPersonCustomer.Select(x => x.EmployeeId).ToList();
                        }

                        listPersonCustomerId = listPersonCustomerId.Distinct().ToList();

                        var listOrganizationId = new List<Guid?>();
                        _getOrganizationChildrenId(listOrganizationCommon, organization.OrganizationId, listOrganizationId);
                        listOrganizationId.Add(organization.OrganizationId);
                        listOrganizationId = listOrganizationId.Distinct().ToList();
                        var listEmployeeTemp = listEmployeeCommon.Where(x => x.Active == true && listOrganizationId.Contains(x.OrganizationId)).ToList();
                        var listEmployeeTempId = listEmployeeTemp.Select(x => x.EmployeeId).ToList();
                        listEmployeeTempId = listEmployeeTempId.Distinct().ToList();

                        listPersonResult = listEmployeeCommon.Where(x => x.EmployeeId != null && listEmployeeTempId.Contains(x.EmployeeId) && (listPersonCustomerId.Count == 0 || listPersonCustomerId.Contains(x.EmployeeId)))
                        .Select(y => new EmployeeEntityModel()
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeName = y.EmployeeName,
                            EmployeeCode = y.EmployeeCode,
                            IsManager = y.IsManager,
                            Active = y.Active
                        }).ToList();

                    }

                    #endregion
                }
                else
                {
                    #region Lấy danh sách người phụ trách hồ sơ thầu theo người đang đăng nhập

                    listPersonResult.Add(new EmployeeEntityModel()
                    {
                        EmployeeId = employeeLogin.EmployeeId,
                        EmployeeName = employeeLogin.EmployeeName,
                        EmployeeCode = employeeLogin.EmployeeCode,
                        IsManager = employeeLogin.IsManager,
                        Active = employeeLogin.Active
                    });

                    #endregion
                }

                return new GetPersonInChargeByCustomerIdResult
                {
                    Status = true,
                    ListPersonInCharge = listPersonResult
                };
            }
            catch (Exception)
            {
                return new GetPersonInChargeByCustomerIdResult
                {
                    Status = false,
                    Message = CommonMessage.SaleBidding.GET_PERSON_FAIL
                };
            }
        }
    }
}
