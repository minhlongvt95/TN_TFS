using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Interfaces.SaleBidding;
using TN.TNM.BusinessLogic.Messages.Requests.SaleBidding;
using TN.TNM.BusinessLogic.Messages.Responses.SaleBidding;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.SaleBidding;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Factories.SaleBidding
{
    public class SaleBiddingFactory : BaseFactory, ISaleBidding
    {
        private ISaleBiddingDataAccess _iSaleBiddingDataAccess;

        public SaleBiddingFactory(ISaleBiddingDataAccess iSaleBiddingDataAccess, ILogger<SaleBiddingFactory> _logger)
        {
            _iSaleBiddingDataAccess = iSaleBiddingDataAccess;
            this.logger = _logger;
        }

        public CreateSaleBiddingResponse CreateSaleBidding(CreateSaleBiddingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.CreateSaleBidding(parameter);
                var response = new CreateSaleBiddingResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    SaleBiddingId = result.SaleBiddingId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateSaleBiddingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public DownloadTemplateProductResponse DownloadTemplateProduct(DownloadTemplateProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Vendor By ProductId");
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.DownloadTemplateProduct(parameter);

                var response = new DownloadTemplateProductResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    FileName = result.FileName,
                    TemplateExcel = result.TemplateExcel
                };

                return response;
            }
            catch (Exception e)
            {
                return new DownloadTemplateProductResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public EditSaleBiddingResponse EditSaleBidding(EditSaleBiddingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.EditSaleBidding(parameter);
                var response = new EditSaleBiddingResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListSaleBiddingDetail = new List<SaleBiddingDetailModel>()
                };
                result.ListSaleBiddingDetail?.ForEach(item =>
                {
                    var temp = new SaleBiddingDetailModel(item);
                    temp.ListFile = new List<FileInFolderModel>();
                    item.ListFile.ForEach(file =>
                    {
                        temp.ListFile.Add(new FileInFolderModel(file));
                    });
                    response.ListSaleBiddingDetail.Add(temp);
                });

                return response;
            }
            catch (Exception e)
            {
                return new EditSaleBiddingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataCreateSaleBiddingResponse GetMasterDataCreateSaleBidding(GetMasterDataCreateSaleBiddingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetMasterDataCreateSaleBidding(parameter);
                var response = new GetMasterDataCreateSaleBiddingResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Employee = new EmployeeModel(result.Employee),
                    Lead = new LeadModel(result.Lead),
                    ListCustomer = new List<CustomerModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    ListLeadDetail = result.ListLeadDetail,
                    ListMoneyUnit = new List<CategoryModel>(),
                    ListProduct = new List<ProductModel>(),
                    ListPerson = new List<EmployeeModel>(),
                    ListTypeContact = new List<CategoryModel>()
                };

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });
                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListPerson.ForEach(item =>
                {
                    response.ListPerson.Add(new EmployeeModel(item));
                });

                result.ListMoneyUnit.ForEach(item =>
                {
                    response.ListMoneyUnit.Add(new CategoryModel(item));
                });

                result.ListTypeContact.ForEach(item =>
                {
                    response.ListTypeContact.Add(new CategoryModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateSaleBiddingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataSaleBiddingAddEditProductDialogResponse GetMasterDataSaleBiddingAddEditProductDialog(GetMasterDataSaleBiddingAddEditProductDialogRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Data Quote Add Edit Product Dialog");
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetMasterDataSaleBiddingAddEditProductDialog(parameter);

                var response = new GetMasterDataSaleBiddingAddEditProductDialogResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListUnitMoney = new List<CategoryModel>(),
                    ListUnitProduct = new List<CategoryModel>(),
                    ListVendor = new List<VendorModel>(),
                    ListProduct = new List<ProductModel>()
                };

                result.ListUnitMoney.ForEach(item =>
                {
                    response.ListUnitMoney.Add(new CategoryModel(item));
                });

                result.ListUnitProduct.ForEach(item =>
                {
                    response.ListUnitProduct.Add(new CategoryModel(item));
                });

                result.ListVendor.ForEach(vendor =>
                {
                    response.ListVendor.Add(new VendorModel(vendor));
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSaleBiddingAddEditProductDialogResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSaleBiddingApprovedResponse GetMasterDataSaleBiddingApproved(GetMasterDataSaleBiddingApprovedRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetMasterDataSaleBiddingApproved(parameter);
                var response = new GetMasterDataSaleBiddingApprovedResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListCustomer = new List<CustomerModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    ListSaleBidding = new List<SaleBiddingModel>(),
                    ListStatus = new List<CategoryModel>()
                };

                result.ListSaleBidding?.ForEach(item =>
                {
                    response.ListSaleBidding.Add(new SaleBiddingModel(item));
                });

                result.ListCustomer?.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                result.ListStatus?.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.ListEmployee?.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSaleBiddingApprovedResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataSaleBiddingDashboardResponse GetMasterDataSaleBiddingDashboard(GetMasterDataSaleBiddingDashboardRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetMasterDataSaleBiddingDashBoard(parameter);

                var response = new GetMasterDataSaleBiddingDashboardResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListSaleBiddingWaitApproval = new List<Models.SaleBidding.SaleBiddingModel>(),
                    ListSaleBiddingExpired = new List<Models.SaleBidding.SaleBiddingModel>(),
                    ListSaleBiddingInWeek = new List<Models.SaleBidding.SaleBiddingModel>(),
                    ListSaleBiddingSlowStartDate = new List<Models.SaleBidding.SaleBiddingModel>(),
                    ListSaleBiddingChart = new List<SaleBiddingModel>(),
                    ListTypeContact = new List<CategoryModel>(),
                    ListStatus = new List<CategoryModel>()
                };

                result.ListSaleBiddingWaitApproval.ForEach(item =>
                {
                    response.ListSaleBiddingWaitApproval.Add(new Models.SaleBidding.SaleBiddingModel(item));
                });

                result.ListSaleBiddingExpired.ForEach(item =>
                {
                    response.ListSaleBiddingExpired.Add(new Models.SaleBidding.SaleBiddingModel(item));
                });

                result.ListSaleBiddingSlowStartDate.ForEach(item =>
                {
                    response.ListSaleBiddingSlowStartDate.Add(new Models.SaleBidding.SaleBiddingModel(item));
                });

                result.ListSaleBiddingInWeek.ForEach(item =>
                {
                    response.ListSaleBiddingInWeek.Add(new Models.SaleBidding.SaleBiddingModel(item));
                });

                result.ListSaleBiddingChart?.ForEach(item =>
                {
                    response.ListSaleBiddingChart.Add(new SaleBiddingModel(item));
                });

                result.ListTypeContact.ForEach(item =>
                {
                    response.ListTypeContact.Add(new CategoryModel(item));
                });

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataSaleBiddingDashboardResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataSaleBiddingDetailResponse GetMasterDataSaleBiddingDetail(GetMasterDataSaleBiddingDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetMasterDataSaleBiddingDetail(parameter);

                var response = new GetMasterDataSaleBiddingDetailResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmployee = new List<EmployeeModel>(),
                    SaleBidding = new SaleBiddingModel(result.SaleBidding),
                    ListSaleBiddingDetail = new List<SaleBiddingDetailModel>(),
                    ListNote = new List<NoteModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListMoneyUnit = new List<CategoryModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListEmployeeMapping = result.ListEmployeeMapping,
                    ListProduct = new List<ProductModel>(),
                    Contact = new ContactModel(result.Contact),
                    CustomerMeetingInfor = new CustomerMeetingInforBusinessModel(),
                    ListCustomerCareInfor = new List<CustomerCareInforBusinessModel>(),
                    ListPerson = new List<EmployeeModel>(),
                    IsApproved = result.IsApproved,
                    ListTypeContact = new List<CategoryModel>(),
                    isEdit = result.isEdit,
                    isEmployeeSupport = result.isEmployeeSupport,
                    isLoginEmployeeJoin = result.isLoginEmployeeJoin
                };

                response.SaleBidding.SaleBiddingDetail = new List<CostQuoteModel>();

                result.SaleBidding.SaleBiddingDetail.ForEach(item =>
                {
                    response.SaleBidding.SaleBiddingDetail.Add(item);
                });

                result.ListTypeContact.ForEach(item =>
                {
                    response.ListTypeContact.Add(new CategoryModel(item));
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });

                result.ListSaleBiddingDetail.ForEach(item =>
                {
                    var temp = new SaleBiddingDetailModel(item);
                    temp.ListFile = new List<FileInFolderModel>();
                    item.ListFile.ForEach(x =>
                    {
                        temp.ListFile.Add(new FileInFolderModel(x));
                    });

                    response.ListSaleBiddingDetail.Add(temp);
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListPerson.ForEach(item =>
                {
                    response.ListPerson.Add(new EmployeeModel(item));
                });

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                result.ListMoneyUnit.ForEach(item =>
                {
                    response.ListMoneyUnit.Add(new CategoryModel(item));
                });

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.ListCustomerCareInfor.ForEach(item =>
                {
                    var customerCareInforBusiness = new CustomerCareInforBusinessModel();
                    customerCareInforBusiness.EmployeeName = item.EmployeeName;
                    customerCareInforBusiness.EmployeeCharge = item.EmployeeCharge;
                    customerCareInforBusiness.EmployeePosition = item.EmployeePosition;

                    customerCareInforBusiness.Week1 = new List<CustomerCareForWeekBusinessModel>();
                    customerCareInforBusiness.Week2 = new List<CustomerCareForWeekBusinessModel>();
                    customerCareInforBusiness.Week3 = new List<CustomerCareForWeekBusinessModel>();
                    customerCareInforBusiness.Week4 = new List<CustomerCareForWeekBusinessModel>();
                    customerCareInforBusiness.Week5 = new List<CustomerCareForWeekBusinessModel>();

                    item.Week1.ForEach(week1 =>
                    {
                        var customerCareForWeekBusiness = new CustomerCareForWeekBusinessModel();
                        customerCareForWeekBusiness.CustomerCareId = week1.CustomerCareId;
                        customerCareForWeekBusiness.EmployeeCharge = week1.EmployeeCharge;
                        customerCareForWeekBusiness.ActiveDate = week1.ActiveDate;
                        customerCareForWeekBusiness.FeedBackStatus = week1.FeedBackStatus;
                        customerCareForWeekBusiness.Background = week1.Background;
                        customerCareForWeekBusiness.SubTitle = week1.SubTitle;
                        customerCareForWeekBusiness.Title = week1.Title;
                        customerCareForWeekBusiness.Type = week1.Type;

                        customerCareInforBusiness.Week1.Add(customerCareForWeekBusiness);
                    });

                    item.Week2.ForEach(week2 =>
                    {
                        var customerCareForWeekBusiness = new CustomerCareForWeekBusinessModel();
                        customerCareForWeekBusiness.CustomerCareId = week2.CustomerCareId;
                        customerCareForWeekBusiness.EmployeeCharge = week2.EmployeeCharge;
                        customerCareForWeekBusiness.ActiveDate = week2.ActiveDate;
                        customerCareForWeekBusiness.FeedBackStatus = week2.FeedBackStatus;
                        customerCareForWeekBusiness.Background = week2.Background;
                        customerCareForWeekBusiness.SubTitle = week2.SubTitle;
                        customerCareForWeekBusiness.Title = week2.Title;
                        customerCareForWeekBusiness.Type = week2.Type;

                        customerCareInforBusiness.Week2.Add(customerCareForWeekBusiness);
                    });

                    item.Week3.ForEach(week3 =>
                    {
                        var customerCareForWeekBusiness = new CustomerCareForWeekBusinessModel();
                        customerCareForWeekBusiness.CustomerCareId = week3.CustomerCareId;
                        customerCareForWeekBusiness.EmployeeCharge = week3.EmployeeCharge;
                        customerCareForWeekBusiness.ActiveDate = week3.ActiveDate;
                        customerCareForWeekBusiness.FeedBackStatus = week3.FeedBackStatus;
                        customerCareForWeekBusiness.Background = week3.Background;
                        customerCareForWeekBusiness.SubTitle = week3.SubTitle;
                        customerCareForWeekBusiness.Title = week3.Title;
                        customerCareForWeekBusiness.Type = week3.Type;

                        customerCareInforBusiness.Week3.Add(customerCareForWeekBusiness);
                    });

                    item.Week4.ForEach(week4 =>
                    {
                        var customerCareForWeekBusiness = new CustomerCareForWeekBusinessModel();
                        customerCareForWeekBusiness.CustomerCareId = week4.CustomerCareId;
                        customerCareForWeekBusiness.EmployeeCharge = week4.EmployeeCharge;
                        customerCareForWeekBusiness.ActiveDate = week4.ActiveDate;
                        customerCareForWeekBusiness.FeedBackStatus = week4.FeedBackStatus;
                        customerCareForWeekBusiness.Background = week4.Background;
                        customerCareForWeekBusiness.SubTitle = week4.SubTitle;
                        customerCareForWeekBusiness.Title = week4.Title;
                        customerCareForWeekBusiness.Type = week4.Type;

                        customerCareInforBusiness.Week4.Add(customerCareForWeekBusiness);
                    });

                    item.Week5.ForEach(week5 =>
                    {
                        var customerCareForWeekBusiness = new CustomerCareForWeekBusinessModel();
                        customerCareForWeekBusiness.CustomerCareId = week5.CustomerCareId;
                        customerCareForWeekBusiness.EmployeeCharge = week5.EmployeeCharge;
                        customerCareForWeekBusiness.ActiveDate = week5.ActiveDate;
                        customerCareForWeekBusiness.FeedBackStatus = week5.FeedBackStatus;
                        customerCareForWeekBusiness.Background = week5.Background;
                        customerCareForWeekBusiness.SubTitle = week5.SubTitle;
                        customerCareForWeekBusiness.Title = week5.Title;
                        customerCareForWeekBusiness.Type = week5.Type;

                        customerCareInforBusiness.Week5.Add(customerCareForWeekBusiness);
                    });

                    response.ListCustomerCareInfor.Add(customerCareInforBusiness);
                });

                response.CustomerMeetingInfor.EmployeeId = result.CustomerMeetingInfor.EmployeeId;
                response.CustomerMeetingInfor.EmployeeName = result.CustomerMeetingInfor.EmployeeName;
                response.CustomerMeetingInfor.EmployeePosition = result.CustomerMeetingInfor.EmployeePosition;

                response.CustomerMeetingInfor.Week1 = new List<CustomerMeetingForWeekBusinessModel>();
                response.CustomerMeetingInfor.Week2 = new List<CustomerMeetingForWeekBusinessModel>();
                response.CustomerMeetingInfor.Week3 = new List<CustomerMeetingForWeekBusinessModel>();
                response.CustomerMeetingInfor.Week4 = new List<CustomerMeetingForWeekBusinessModel>();
                response.CustomerMeetingInfor.Week5 = new List<CustomerMeetingForWeekBusinessModel>();

                result.CustomerMeetingInfor.Week1.ForEach(item =>
                {
                    var temp = new CustomerMeetingForWeekBusinessModel();
                    temp.CustomerMeetingId = item.CustomerMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.CustomerMeetingInfor.Week1.Add(temp);
                });

                result.CustomerMeetingInfor.Week2.ForEach(item =>
                {
                    var temp = new CustomerMeetingForWeekBusinessModel();
                    temp.CustomerMeetingId = item.CustomerMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.CustomerMeetingInfor.Week2.Add(temp);
                });

                result.CustomerMeetingInfor.Week3.ForEach(item =>
                {
                    var temp = new CustomerMeetingForWeekBusinessModel();
                    temp.CustomerMeetingId = item.CustomerMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.CustomerMeetingInfor.Week3.Add(temp);
                });

                result.CustomerMeetingInfor.Week4.ForEach(item =>
                {
                    var temp = new CustomerMeetingForWeekBusinessModel();
                    temp.CustomerMeetingId = item.CustomerMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.CustomerMeetingInfor.Week4.Add(temp);
                });

                result.CustomerMeetingInfor.Week5.ForEach(item =>
                {
                    var temp = new CustomerMeetingForWeekBusinessModel();
                    temp.CustomerMeetingId = item.CustomerMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.CustomerMeetingInfor.Week5.Add(temp);
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataSaleBiddingDetailResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataSearchSaleBiddingResponse GetMasterDataSearchSaleBidding(GetMasterDataSearchSaleBiddingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetMasterDataSearchSaleBidding(parameter);

                var response = new GetMasterDataSearchSaleBiddingResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListContract = new List<CategoryModel>(),
                    ListLeadType = new List<CategoryModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListPersonalInChange = new List<EmployeeModel>(),
                };

                result.ListLeadType?.ForEach(item =>
                {
                    response.ListLeadType.Add(new CategoryModel(item));
                });

                result.ListContract?.ForEach(item =>
                {
                    response.ListContract.Add(new CategoryModel(item));
                });

                result.ListStatus?.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.ListPersonalInChange?.ForEach(item =>
                {
                    response.ListPersonalInChange.Add(new EmployeeModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchSaleBiddingResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public GetVendorByProductIdReponse GetVendorByProductId(GetVendorByProductIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Vendor By ProductId");
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetVendorByProductId(parameter);

                var response = new GetVendorByProductIdReponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListVendor = new List<VendorModel>(),
                    ListObjectAttributeNameProduct = new List<ObjectAttributeNameProductModel>(),
                    ListObjectAttributeValueProduct = new List<ObjectAttributeValueProductModel>()
                };

                result.ListVendor.ForEach(vendor =>
                {
                    response.ListVendor.Add(new VendorModel(vendor));
                });

                result.ListObjectAttributeNameProduct.ForEach(item =>
                {
                    var option = new ObjectAttributeNameProductModel();
                    option.ProductAttributeCategoryId = item.ProductAttributeCategoryId;
                    option.ProductAttributeCategoryName = item.ProductAttributeCategoryName;
                    response.ListObjectAttributeNameProduct.Add(option);
                });

                result.ListObjectAttributeValueProduct.ForEach(item =>
                {
                    var option = new ObjectAttributeValueProductModel();
                    option.ProductAttributeCategoryValueId = item.ProductAttributeCategoryValueId;
                    option.ProductAttributeCategoryValue = item.ProductAttributeCategoryValue;
                    option.ProductAttributeCategoryId = item.ProductAttributeCategoryId;
                    response.ListObjectAttributeValueProduct.Add(option);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetVendorByProductIdReponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchSaleBiddingResponse SearchBidding(SearchSaleBiddingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.SearchSaleBidding(parameter);

                var response = new SearchSaleBiddingResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListSaleBidding = new List<Models.SaleBidding.SaleBiddingModel>()
                };

                result.ListSaleBidding.ForEach(item =>
                {
                    response.ListSaleBidding.Add(new Models.SaleBidding.SaleBiddingModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new SearchSaleBiddingResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public SearchSaleBiddingApprovedResponse SearchSaleBiddingApproved(SearchSaleBiddingApprovedRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.SearchSaleBiddingApproved(parameter);
                var response = new SearchSaleBiddingApprovedResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListSaleBidding = new List<SaleBiddingModel>()
                };

                result.ListSaleBidding?.ForEach(item =>
                {
                    response.ListSaleBidding.Add(new SaleBiddingModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new SearchSaleBiddingApprovedResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateStatusSaleBiddingResponse UpdateStatusSaleBidding(UpdateStatusSaleBiddingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.UpdateStatusSaleBidding(parameter);
                var response = new UpdateStatusSaleBiddingResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Note = result.Note == null ? null : new NoteModel(result.Note)
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateStatusSaleBiddingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetVendorMappingResponse GetVendorMapping(GetVendorMappingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetVendorMapping(parameter);
                var response = new GetVendorMappingResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListVendor = new List<ProductVendorMappingModel>()
                };
                result.ListVendor.ForEach(item =>
                {
                    response.ListVendor.Add(new ProductVendorMappingModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetVendorMappingResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetCustomerByEmployeeIdResponse GetCustomerByEmployeeId(GetCustomerByEmployeeIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetCustomerByEmployeeId(parameter);
                var response = new GetCustomerByEmployeeIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListCustomer = new List<CustomerModel>()
                };
                result.ListCustomer?.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetCustomerByEmployeeIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public SendEmailEmployeeResponse SendEmailEmployee(SendEmailEmployeeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.SendEmailEmployee(parameter);
                var response = new SendEmailEmployeeResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SendEmailEmployeeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetPersonInChargeByCustomerIdResponse GetPersonInChargeByCustomerId(GetPersonInChargeByCustomerIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iSaleBiddingDataAccess.GetPersonInChargeByCustomerId(parameter);
                var response = new GetPersonInChargeByCustomerIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListPersonInCharge = new List<EmployeeModel>()
                };

                result.ListPersonInCharge?.ForEach(item =>
                {
                    response.ListPersonInCharge.Add(new EmployeeModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetPersonInChargeByCustomerIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }
    }
}
