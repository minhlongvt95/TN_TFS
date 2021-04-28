using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Company;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CategoryDAO : BaseDAO, ICategoryDataAccess
    {
        public CategoryDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllCategoryByCategoryTypeCodeResult GetAllCategoryByCategoryTypeCode(
            GetAllCategoryByCategoryTypeCodeParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.CATEGORY, "GetAllCategoryByCategoryTypeCode", parameter.UserId);
            #region Add by Hung
            var listCategoryType = context.CategoryType.ToList();
            var listCategory = context.Category.Where(w => w.Active == true).ToList();
            List<CategoryEntityModel> categoryList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryPTOList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryNHAList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryTHAList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryTNHList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryLDOList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryQNGList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryGENDERList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryLHIList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryCVUList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryLNGList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryNCHList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryPMList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryDVIList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryLabourContractList = new List<CategoryEntityModel>();
            List<CategoryEntityModel> categoryNCAList = new List<CategoryEntityModel>();
            listCategoryType.ForEach(item =>
            {

                var cateList = listCategory.Where(w => w.CategoryTypeId == item.CategoryTypeId).ToList();
                cateList.ForEach(cat =>
                {
                    CategoryEntityModel itemCat = new CategoryEntityModel
                    {
                        CategoryTypeId = cat.CategoryTypeId,
                        CategoryId = cat.CategoryId,
                        CategoryName = cat.CategoryName,
                        CategoryCode = cat.CategoryCode,
                        IsDefault = cat.IsDefauld
                    };
                    switch (item.CategoryTypeCode)
                    {
                        case "PTO":
                            categoryPTOList.Add(itemCat);
                            break;
                        case "NHA":
                            categoryNHAList.Add(itemCat);
                            break;
                        case "THA":
                            categoryTHAList.Add(itemCat);
                            break;
                        case "TNH":
                            categoryTNHList.Add(itemCat);
                            break;
                        case "LDO":
                            categoryLDOList.Add(itemCat);
                            break;
                        case "QNG":
                            categoryQNGList.Add(itemCat);
                            break;
                        case "GENDER":
                            categoryGENDERList.Add(itemCat);
                            break;
                        case "LHI":
                            categoryLHIList.Add(itemCat);
                            break;
                        case "CVU":
                            categoryCVUList.Add(itemCat);
                            break;
                        case "LNG":
                            categoryLNGList.Add(itemCat);
                            break;
                        case "NCH":
                            categoryNCHList.Add(itemCat);
                            break;
                        case "PM":
                            categoryPMList.Add(itemCat);
                            break;
                        case "DVI":
                            categoryDVIList.Add(itemCat);
                            break;
                        case "LVI":
                            categoryLabourContractList.Add(itemCat);
                            break;
                        case "NCA":
                            categoryNCAList.Add(itemCat);
                            break;
                    }
                });
            });
            #endregion

            var categorytype = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == parameter.CategoryTypeCode);
            if (categorytype != null)
            {
                categoryList = listCategory.Where(c => c.CategoryTypeId == categorytype.CategoryTypeId).Select(c => new CategoryEntityModel()
                {
                    CategoryTypeId = c.CategoryTypeId,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryCode = c.CategoryCode,
                    IsDefault = c.IsDefauld
                }).ToList();
            }
            return new GetAllCategoryByCategoryTypeCodeResult()
            {
                Status = true,
                Category = categoryList,
                CategoryCVUList=categoryCVUList,
                CategoryGENDERList=categoryGENDERList,
                CategoryLDOList=categoryLDOList,
                CategoryLHIList= categoryLHIList,
                CategoryLNGList= categoryLNGList,
                CategoryNCHList= categoryNCHList,
                CategoryNHAList= categoryNHAList,
                CategoryPTOList= categoryPTOList,
                CategoryQNGList= categoryQNGList,
                CategoryTHAList= categoryTHAList,
                CategoryTNHList= categoryTNHList,
                CategoryPMList= categoryPMList,
                CategoryDVIList= categoryDVIList,
                CategoryLabourContractList= categoryLabourContractList,
                CategoryNCAList= categoryNCAList
            };
        }

        public GetAllCategoryResult GetAllCategory(GetAllCategoryParameter parameter)
        {
            //var listCategory = context.Category.ToList();
            var bankPayableInvoice = context.BankPayableInvoice.ToList();
            var bankReceiptInvoice = context.BankReceiptInvoice.ToList();
            var contact = context.Contact.ToList();
            var customer = context.Customer.ToList();
            var customerOrderDetail = context.CustomerOrderDetail.ToList();
            var inventory = context.Inventory.ToList();
            var lead = context.Lead.ToList();
            var leadDetail = context.LeadDetail.ToList();
            var LeadInterestedGroup = context.LeadInterestedGroupMapping.ToList();
            var payableInvoice = context.PayableInvoice.ToList();
            var product = context.Product.ToList();
            var productVendor = context.ProductVendorMapping.ToList();
            var receiptInvoice = context.ReceiptInvoice.ToList();
            var vendor = context.Vendor.ToList();
            var vendorOrderDetail = context.VendorOrderDetail.ToList();
            var workFlows = context.WorkFlows.ToList();
            var vendorOrder = context.VendorOrder.ToList();
            var requestPayment = context.RequestPayment.ToList();
            var quoteDetail = context.QuoteDetail.ToList();
            var employeeAssessment = context.EmployeeAssessment.ToList();
            var employeeMonthySalary = context.EmployeeMonthySalary.ToList();
            var employeeRequest = context.EmployeeRequest.ToList();
            var procurementRequest = context.ProcurementRequest.ToList();
            var queue = context.Queue.ToList();
            var quote = context.Quote.ToList();
            var cases = context.Case.ToList();
            var caseActivities = context.CaseActivities.ToList();
            var configurationRule = context.ConfigurationRule.ToList();
            var customerCare = context.CustomerCare.ToList();
            var customerCareCustomer = context.CustomerCareCustomer.ToList();
            var customerOrder = context.CustomerOrder.ToList();
            var employee = context.Employee.ToList();
            var customerCareFeedBack = context.CustomerCareFeedBack.ToList();
            var categoryList = context.Category.ToList();
            var categoryType = context.CategoryType.Where(c => c.Active == true).ToList();
            var contractDetails = context.ContractDetail.ToList();
            var billOfSaleDetails = context.BillOfSaleDetail.ToList();
            var procurementRequestItem = context.ProcurementRequestItem.ToList();

            var categoryTypeList = categoryType.Select(ct => new CategoryTypeEntityModel
            {
                CategoryTypeId = ct.CategoryTypeId,
                CategoryTypeName = ct.CategoryTypeName,
                CategoryTypeCode = ct.CategoryTypeCode,
                CategoryList = categoryList.Where(c => c.CategoryTypeId == ct.CategoryTypeId).Select(c => new CategoryEntityModel()
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryCode = c.CategoryCode,
                    CategoryTypeId = c.CategoryTypeId,
                    Active = c.Active,
                    IsEdit = c.IsEdit,
                    IsDefault = c.IsDefauld,
                    CountCategoryById = CountCategory(c.CategoryId, bankPayableInvoice, bankReceiptInvoice, contact, customer, customerOrderDetail, inventory, lead, leadDetail, LeadInterestedGroup, payableInvoice, product , productVendor, receiptInvoice, vendor, vendorOrderDetail, workFlows, vendorOrder, requestPayment, quoteDetail, employeeAssessment, employeeMonthySalary, employeeRequest, procurementRequest, queue, quote, cases, caseActivities, configurationRule, customerCare, customerCareCustomer, customerOrder, employee, customerCareFeedBack, contractDetails, billOfSaleDetails, procurementRequestItem)
                }).ToList()
            }).OrderBy(ordby => ordby.CategoryTypeName).ToList();
            
            //int a = CountCategory(Guid.Parse("44B9EECB-A3CC-4317-B646-4F2EF93ABA3E"));
            return new GetAllCategoryResult()
            {
                Status = true,
                CategoryTypeList = categoryTypeList
            };
        }
        public int CountCategory(Guid categoryId, List<BankPayableInvoice> bankPayableInvoice, List<BankReceiptInvoice> bankReceiptInvoice, List<Contact> contact, List<Customer> customer, List<CustomerOrderDetail> customerOrderDetail, List<Inventory> inventory,
            List<Lead> lead, List<LeadDetail> leadDetails, List<LeadInterestedGroupMapping> leadInterestedGroup, List<PayableInvoice> payableInvoice, List<Product> product, List<ProductVendorMapping> productVendor, List<ReceiptInvoice> receiptInvoice, List<Vendor> vendor, List<VendorOrderDetail> vendorOrderDetail,
            List<WorkFlows> workFlows, List<VendorOrder> vendorOrder, List<RequestPayment> requestPayment, List<QuoteDetail> quoteDetail, List<EmployeeAssessment> employeeAssessment, List<EmployeeMonthySalary> employeeMonthySalary,
            List<EmployeeRequest> employeeRequest, List<ProcurementRequest> procurementRequest, List<Queue> queue, List<Quote> quote, List<Case> cases, List<CaseActivities> caseActivities,
            List<ConfigurationRule> configurationRule, List<CustomerCare> customerCare, List<CustomerCareCustomer> customerCareCustomer, List<CustomerOrder> customerOrder, List<Employee> employee, List<CustomerCareFeedBack> customerCareFeedBack, List<ContractDetail> contractDetails,
            List<BillOfSaleDetail> billOfSaleDetails, List<ProcurementRequestItem> procurementRequestItems)
        {
            int count = 0;
            //var bankPayableInvoice = context.BankPayableInvoice.ToList();
            count += bankPayableInvoice.Where(s => s.BankPayableInvoicePriceCurrency == categoryId).Count();
            count += bankPayableInvoice.Where(s => s.BankPayableInvoiceReason == categoryId).Count();
            count += bankPayableInvoice.Where(s => s.StatusId == categoryId).Count();

            count += bankReceiptInvoice.Where(s => s.BankReceiptInvoicePriceCurrency == categoryId).Count();
            count += bankReceiptInvoice.Where(s => s.BankReceiptInvoiceReason == categoryId).Count();
            count += bankReceiptInvoice.Where(s => s.StatusId == categoryId).Count();

            count += billOfSaleDetails.Count(s => s.CurrencyUnit == categoryId);

            count += contact.Where(s => s.MaritalStatusId == categoryId).Count();
            count += contact.Where(s => s.TypePaid == categoryId).Count();
            count += contact.Where(s => s.CustomerPosition == categoryId).Count();

            count += contractDetails.Count(s => s.CurrencyUnit == categoryId);

            count += customer.Where(s => s.CustomerGroupId == categoryId).Count();
            count += customer.Where(s => s.FieldId == categoryId).Count();
            count += customer.Where(s => s.ScaleId == categoryId).Count();
            count += customer.Where(s => s.StatusId == categoryId).Count();
            count += customer.Where(s => s.PaymentId == categoryId).Count();
            count += customer.Where(s => s.EnterpriseType == categoryId).Count();
            count += customer.Where(s => s.BusinessType == categoryId).Count();
            count += customer.Where(s => s.BusinessScale == categoryId).Count();
            count += customer.Where(s => s.MainBusinessSector == categoryId).Count();

            count += customerOrderDetail.Where(s => s.UnitId == categoryId).Count();
            count += customerOrderDetail.Where(s => s.CurrencyUnit == categoryId).Count();

            count += inventory.Where(s => s.InventoryStatus == categoryId).Count();

            count += lead.Where(s => s.InterestedGroupId == categoryId).Count();
            count += lead.Where(s => s.PaymentMethodId == categoryId).Count();
            count += lead.Count(s => s.Active == true && s.PotentialId == categoryId);
            count += lead.Where(s => s.StatusId == categoryId).Count();
            count += lead.Count(s => s.InvestmentFundId == categoryId);
            count += lead.Count(s => s.BusinessTypeId == categoryId);
            count += lead.Count(s => s.LeadGroupId == categoryId);
            count += lead.Count(s => s.ProbabilityId == categoryId);

            count += leadDetails.Count(s => s.CurrencyUnit == categoryId);

            count += leadInterestedGroup.Count(s => s.InterestedGroupId == categoryId);

            count += payableInvoice.Where(s => s.CurrencyUnit == categoryId).Count();
            count += payableInvoice.Where(s => s.PayableInvoicePriceCurrency == categoryId).Count();
            count += payableInvoice.Where(s => s.PayableInvoiceReason == categoryId).Count();
            count += payableInvoice.Where(s => s.RegisterType == categoryId).Count();
            count += payableInvoice.Where(s => s.StatusId == categoryId).Count();

            count += product.Where(s => s.ProductMoneyUnitId == categoryId).Count();
            count += product.Where(s => s.ProductUnitId == categoryId).Count();

            count += productVendor.Count(s => s.UnitPriceId == categoryId);

            count += procurementRequestItems.Count(s => s.CurrencyUnit == categoryId);

            count += receiptInvoice.Where(s => s.CurrencyUnit == categoryId).Count();
            count += receiptInvoice.Where(s => s.ReceiptInvoiceReason == categoryId).Count();
            count += receiptInvoice.Where(s => s.RegisterType == categoryId).Count();
            count += receiptInvoice.Where(s => s.StatusId == categoryId).Count();

            count += vendor.Where(s => s.PaymentId == categoryId).Count();
            count += vendor.Where(s => s.VendorGroupId == categoryId).Count();

            count += vendorOrderDetail.Where(s => s.UnitId == categoryId).Count();
            count += vendorOrderDetail.Where(s => s.CurrencyUnit == categoryId).Count();

            count += workFlows.Where(s => s.StatusId == categoryId).Count();

            count += vendorOrder.Where(s => s.StatusId == categoryId).Count();
            count += vendorOrder.Where(s => s.VendorContactId == categoryId).Count();
            count += vendorOrder.Where(s => s.PaymentMethod == categoryId).Count();
            count += vendorOrder.Where(s => s.VendorId == categoryId).Count();
            count += vendorOrder.Where(s => s.Orderer == categoryId).Count();
            count += vendorOrder.Where(s => s.CustomerOrderId == categoryId).Count();

            count += requestPayment.Where(s => s.StatusId == categoryId).Count();
            count += requestPayment.Where(s => s.PaymentType == categoryId).Count();

            count += quoteDetail.Where(s => s.UnitId == categoryId).Count();
            count += quoteDetail.Count(s => s.CurrencyUnit == categoryId);

            count += quote.Where(s => s.StatusId == categoryId).Count();
            count += quote.Where(s => s.PaymentMethod == categoryId).Count();

            count += queue.Where(s => s.StatusId == categoryId).Count();

            count += procurementRequest.Where(s => s.StatusId == categoryId).Count();

            count += employeeRequest.Where(s => s.TypeRequest == categoryId).Count();
            count += employeeRequest.Where(s => s.StatusId == categoryId).Count();
            count += employeeRequest.Where(s => s.StartTypeTime == categoryId).Count();
            count += employeeRequest.Where(s => s.EndTypeTime == categoryId).Count();
            count += employeeRequest.Where(s => s.TypeReason == categoryId).Count();
            count += employeeRequest.Where(s => s.ManagerId == categoryId).Count();

            count += employeeMonthySalary.Where(s => s.StatusId == categoryId).Count();

            count += employee.Where(s => s.ContractType == categoryId).Count();

            count += customerOrder.Where(s => s.PaymentMethod == categoryId).Count();

            count += customerCareFeedBack.Where(s => s.FeedbackType == categoryId).Count();
            count += customerCareFeedBack.Where(s => s.FeedBackCode == categoryId).Count();

            count += customerCareCustomer.Where(s => s.StatusId == categoryId).Count();

            count += customerCare.Where(s => s.CustomerCareContactType == categoryId).Count();
            count += customerCare.Where(s => s.CustomerCareEvent == categoryId).Count();
            count += customerCare.Where(s => s.GiftTypeId1 == categoryId).Count();
            count += customerCare.Where(s => s.GiftTypeId2 == categoryId).Count();
            count += customerCare.Where(s => s.StatusId == categoryId).Count();

            count += configurationRule.Where(s => s.Type == categoryId).Count();

            count += caseActivities.Where(s => s.CaseActivitiesType == categoryId).Count();

            count += cases.Where(s => s.StatusId == categoryId).Count();
            count += cases.Where(s => s.AsignTo == categoryId).Count();

            count += employeeAssessment.Where(s => s.Type == categoryId).Count();

            return count;
        }
        public GetCategoryByIdResult GetCategoryById(GetCategoryByIdParameter parameter)
        {
            var category = context.Category.FirstOrDefault(c => c.CategoryId == parameter.CategoryId);


            if (category != null)
            {
                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeId == category.CategoryTypeId)?
                    .CategoryTypeName;
                CategoryEntityModel cem = new CategoryEntityModel()
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryCode = category.CategoryCode,
                    CategoryTypeName = categoryType
                };

                return new GetCategoryByIdResult()
                {
                    Status = true,
                    Category = cem,
                    IsCategory = true
                };
            }
            else
            {
                var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeId == parameter.CategoryId);


                if (categoryType != null)
                {
                    CategoryEntityModel cem = new CategoryEntityModel()
                    {
                        CategoryId = categoryType.CategoryTypeId,
                        CategoryName = categoryType.CategoryTypeName,
                        CategoryTypeCode = categoryType.CategoryTypeCode,
                        CategoryTypeName = ""
                    };

                    return new GetCategoryByIdResult()
                    {
                        Status = true,
                        Category = cem,
                        IsCategory = false
                    };
                }
                else
                {
                    return new GetCategoryByIdResult()
                    {
                        Status = false,
                        Message = CommonMessage.MasterData.GET_FAIL
                    };
                }
            }
        }

        public CreateCategoryResult CreateCategory(CreateCategoryParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.CATEGORY, "Create Category", parameter.UserId);
            if (parameter.CategoryTypeId == Guid.Empty)
            {
                return new CreateCategoryResult()
                {
                    Status = false,
                    Message = CommonMessage.MasterData.PLEASE_CHOOSE
                };
            }
            else
            {
                var categoryType =
                    context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeId == parameter.CategoryTypeId);
                if (categoryType == null)
                {
                    return new CreateCategoryResult()
                    {
                        Status = false,
                        Message = CommonMessage.MasterData.ONLY_2_LVL
                    };
                }
                else
                {
                    Category category = new Category()
                    {
                        CategoryId = Guid.NewGuid(),
                        CategoryName = parameter.CategoryName,
                        CategoryCode = parameter.CategoryCode,
                        CategoryTypeId = parameter.CategoryTypeId,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        Active = true,
                        IsDefauld = false,
                        IsEdit = true
                    };

                    context.Category.Add(category);
                    context.SaveChanges();
                    return new CreateCategoryResult()
                    {
                        Status = true,
                        Message = CommonMessage.MasterData.CREATE_SUCCESS
                    };
                }
            }
        }

        public DeleteCategoryByIdResult DeleteCategoryById(DeleteCategoryByIdParameter parameter)
        {
            var categoryType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeId == parameter.CategoryId);
            if (categoryType != null)
            {
                if (!categoryType.CategoryTypeCode.Equals("NHU") && !categoryType.CategoryTypeCode.Equals("PTO"))
                {
                    return new DeleteCategoryByIdResult()
                    {
                        Status = false,
                        Message = CommonMessage.MasterData.CANNOT_DELETE_TYPE + categoryType.CategoryTypeName
                    };
                }
                else
                {
                    var category = context.Category.Where(c => c.CategoryTypeId == categoryType.CategoryTypeId).ToList();
                    category.ForEach(c => { context.Category.Remove(c); });
                    context.CategoryType.Remove(categoryType);
                }
            }
            else
            {
                var category = context.Category.FirstOrDefault(c => c.CategoryId == parameter.CategoryId);
                if (category != null)
                {
                    context.Category.Remove(category);
                }
            }

            context.SaveChanges();

            return new DeleteCategoryByIdResult()
            {
                Status = true,
                Message = CommonMessage.MasterData.DELETE_SUCCESS
            };
        }

        public EditCategoryByIdResult EditCategoryById(EditCategoryByIdParameter parameter)
        {
            var categoryType = context.Category.FirstOrDefault(ct => ct.CategoryId == parameter.CategoryId);
            if (categoryType == null)
            {
                return new EditCategoryByIdResult()
                {
                    Status = false,
                    Message = CommonMessage.MasterData.CANNOT_EDIT_TYPE
                };
            }
            else
            {
                var category = context.Category.FirstOrDefault(c => c.CategoryId == parameter.CategoryId);
                category.CategoryName = parameter.CategoryName;
                category.CategoryCode = parameter.CategoryCode;
                category.UpdatedById = parameter.UserId;
                category.UpdatedDate = DateTime.Now;

                context.SaveChanges();
                return new EditCategoryByIdResult()
                {
                    Status = true,
                    Message = CommonMessage.MasterData.EDIT_SUCCESS
                };
            }
        }

        public UpdateStatusIsActiveResult UpdateStatusIsActive(UpdateStatusIsActiveParameter parameter)
        {
            var categoryType = context.Category.FirstOrDefault(ct => ct.CategoryId == parameter.CategoryId);
            if (categoryType == null)
            {
                return new UpdateStatusIsActiveResult()
                {
                    Status = false,
                    Message = CommonMessage.MasterData.CANNOT_EDIT_TYPE
                };
            }
            else
            {
                categoryType.Active = parameter.Active;

                context.SaveChanges();
                return new UpdateStatusIsActiveResult()
                {
                    Status = true,
                    Message = CommonMessage.MasterData.EDIT_SUCCESS
                };
            }
        }

        public UpdateStatusIsDefaultResult UpdateStatusIsDefault(UpdateStatusIsDefaultParameter parameter)
        {
            var categoryType = context.Category.FirstOrDefault(ct => ct.CategoryId == parameter.CategoryId);
            if (categoryType == null)
            {
                return new UpdateStatusIsDefaultResult()
                {
                    Status = false,
                    Message = CommonMessage.MasterData.CANNOT_EDIT_TYPE
                };
            }
            else
            {
                var categoryList = context.Category.Where(ct => ct.CategoryTypeId == parameter.CategoryTypeId).ToList();
                foreach (var item in categoryList)
                {
                    item.IsDefauld = false;
                    context.SaveChanges();
                }
                categoryType.IsDefauld = true;
                context.SaveChanges();

                return new UpdateStatusIsDefaultResult()
                {
                    Status = true,
                    Message = CommonMessage.MasterData.EDIT_SUCCESS
                };
            }
        }
    }
}
