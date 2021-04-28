using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Customer;
using TN.TNM.BusinessLogic.Messages.Requests.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Customer;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.BusinessLogic.Factories.Customer
{
    public class CustomerFactory : BaseFactory, ICustomer
    {
        private ICustomerDataAccess iCustomerDataAccess;
        public CustomerFactory(ICustomerDataAccess _iCustomerDataAccess, ILogger<CustomerFactory> _logger)
        {
            iCustomerDataAccess = _iCustomerDataAccess;
            logger = _logger;
        }

        public CreateCustomerResponse CreateCustomer(CreateCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Create Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CreateCustomer(parameter);
                var response = new CreateCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ContactId = result.ContactId,
                    CustomerId = result.CustomerId,
                    DuplicateContact = result.DuplicateContact,
                    SendEmailEntityModel = result.SendEmailEntityModel,
                    ListCustomer = result.ListCustomer
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateCustomerResponse()
                {
                    MessageCode = CommonMessage.Customer.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllCustomerCodeResponse GetAllCustomerCode(GetAllCustomerCodeRequest request)
        {
            try
            {
                logger.LogInformation("Get Customer Code");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetAllCustomerCode(parameter);
                var response = new GetAllCustomerCodeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    CustomerCodeList = result.CustomerCodeList
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllCustomerCodeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchCustomerResponse SearchCustomer(SearchCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Search Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.SearchCustomer(parameter);
                var response = new SearchCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Customer = new List<CustomerSearchModel>()
                };
                result.Customer.ForEach(customer =>
                {
                    response.Customer.Add(new CustomerSearchModel(customer));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchCustomerResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerFromOrderCreateResponse GetCustomerFromOrderCreate(GetCustomerFromOrderCreateRequest request)
        {
            try
            {
                logger.LogInformation("Get Customer From Order Create");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetCustomerFromOrderCreate(parameter);
                var response = new GetCustomerFromOrderCreateResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Customer = new List<CustomerModel>()
                };
                result.Customer.ForEach(customer =>
                {
                    response.Customer.Add(new CustomerModel(customer));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetCustomerFromOrderCreateResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllCustomerServiceLevelResponse GetAllCustomerServiceLevel(GetAllCustomerServiceLevelRequest request)
        {
            try
            {
                logger.LogInformation("Get Customer Service Level");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetAllCustomerServiceLevel(parameter);
                var response = new GetAllCustomerServiceLevelResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    CustomerServiceLevelList = new List<Models.Customer.CustomerServiceLevelModel>()
                };
                result.CustomerServiceLevelList.ForEach(customerSL =>
                {
                    response.CustomerServiceLevelList.Add(new Models.Customer.CustomerServiceLevelModel(customerSL));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllCustomerServiceLevelResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerByIdResponse GetCustomerById(GetCustomerByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Customer by id");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetCustomerById(parameter);
                var response = new GetCustomerByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListStatusCustomerCare = new List<CategoryModel>(),
                    ListCustomerGroup = new List<CategoryModel>(),
                    ListCustomerStatus = new List<CategoryModel>(),
                    ListBusinessType = new List<CategoryModel>(),
                    ListBusinessSize = new List<CategoryModel>(),
                    ListPaymentMethod = new List<CategoryModel>(),
                    ListTypeOfBusiness = new List<CategoryModel>(),
                    ListBusinessCareer = new List<CategoryModel>(),
                    ListLocalTypeBusiness = new List<CategoryModel>(),
                    ListCustomerPosition = new List<CategoryModel>(),
                    ListMaritalStatus = new List<CategoryModel>(),
                    ListPersonInCharge = new List<EmployeeModel>(),
                    ListCareStaff = new List<EmployeeModel>(),
                    ListArea = new List<AreaModel>(),
                    ListProvince = new List<ProvinceModel>(),
                    ListDistrict = new List<DistrictModel>(),
                    ListWard = new List<WardModel>(),
                    ListNote = new List<NoteModel>(),
                    ListCustomerAdditionalInformation = new List<CustomerAdditionalInformationModel>(),
                    ListCusContact = new List<CustomerOtherContactBusinessModel>(),
                    ListOrderOfCustomer = result.ListOrderOfCustomer,
                    ListBankAccount = new List<BankAccountModel>(),
                    ListCustomerCareInfor = new List<CustomerCareInforBusinessModel>(),
                    CustomerMeetingInfor = new CustomerMeetingInforBusinessModel(),
                    ListParticipants = new List<EmployeeModel>(),
                    ListCustomerLead = new List<LeadModel>(),
                    CustomerCode = result.CustomerCode,

                    Contact = new ContactModel(result.Contact),
                    Customer = new CustomerModel(result.Customer),
                    CountryList = new List<CountryModel>(),
                    isSendApproval = result.isSendApproval,
                    isApprovalNew = result.isApprovalNew,
                    isApprovalDD = result.isApprovalDD,
                };

                result.ListStatusCustomerCare.ForEach(item =>
                {
                    response.ListStatusCustomerCare.Add(new CategoryModel(item));
                });

                result.ListCustomerGroup.ForEach(item =>
                {
                    response.ListCustomerGroup.Add(new CategoryModel(item));
                });

                result.ListCustomerStatus.ForEach(item =>
                {
                    response.ListCustomerStatus.Add(new CategoryModel(item));
                });

                result.ListBusinessType.ForEach(item =>
                {
                    response.ListBusinessType.Add(new CategoryModel(item));
                });

                result.ListBusinessSize.ForEach(item =>
                {
                    response.ListBusinessSize.Add(new CategoryModel(item));
                });

                result.ListPaymentMethod.ForEach(item =>
                {
                    response.ListPaymentMethod.Add(new CategoryModel(item));
                });

                result.ListTypeOfBusiness.ForEach(item =>
                {
                    response.ListTypeOfBusiness.Add(new CategoryModel(item));
                });

                result.ListBusinessCareer.ForEach(item =>
                {
                    response.ListBusinessCareer.Add(new CategoryModel(item));
                });

                result.ListLocalTypeBusiness.ForEach(item =>
                {
                    response.ListLocalTypeBusiness.Add(new CategoryModel(item));
                });

                result.ListCustomerPosition.ForEach(item =>
                {
                    response.ListCustomerPosition.Add(new CategoryModel(item));
                });

                result.ListMaritalStatus.ForEach(item =>
                {
                    response.ListMaritalStatus.Add(new CategoryModel(item));
                });

                result.ListPersonInCharge.ForEach(item =>
                {
                    response.ListPersonInCharge.Add(new EmployeeModel(item));
                });

                result.ListCareStaff.ForEach(item =>
                {
                    response.ListCareStaff.Add(new EmployeeModel(item));
                });

                result.ListArea.ForEach(item =>
                {
                    response.ListArea.Add(new AreaModel(item));
                });

                result.ListProvince.ForEach(item =>
                {
                    response.ListProvince.Add(new ProvinceModel(item));
                });

                result.ListDistrict.ForEach(item =>
                {
                    response.ListDistrict.Add(new DistrictModel(item));
                });

                result.ListWard.ForEach(item =>
                {
                    response.ListWard.Add(new WardModel(item));
                });

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                result.ListParticipants.ForEach(item =>
                {
                    response.ListParticipants.Add(new EmployeeModel(item));
                });

                result.ListCustomerLead.ForEach(item =>
                {
                    response.ListCustomerLead.Add(new LeadModel(item));
                });

                result.ListCustomerAdditionalInformation.ForEach(item =>
                {
                    var additionalInformation = new CustomerAdditionalInformationModel();
                    additionalInformation.CustomerAdditionalInformationId = item.CustomerAdditionalInformationId;
                    additionalInformation.CustomerId = item.CustomerId;
                    additionalInformation.Question = item.Question;
                    additionalInformation.Answer = item.Answer;

                    response.ListCustomerAdditionalInformation.Add(additionalInformation);
                });

                result.ListCusContact.ForEach(item =>
                {
                    var cusContact = new CustomerOtherContactBusinessModel();
                    cusContact.ContactId = item.ContactId;
                    cusContact.ObjectId = item.ObjectId;
                    cusContact.ObjectType = item.ObjectType;
                    cusContact.FirstName = item.FirstName;
                    cusContact.LastName = item.LastName;
                    cusContact.ContactName = item.ContactName;
                    cusContact.Role = item.Role;
                    cusContact.Phone = item.Phone;
                    cusContact.Email = item.Email;
                    cusContact.Gender = item.Gender;
                    cusContact.GenderName = item.GenderName;
                    cusContact.DateOfBirth = item.DateOfBirth;
                    cusContact.Address = item.Address;
                    cusContact.Other = item.Other;
                    cusContact.CreatedDate = item.CreatedDate;

                    response.ListCusContact.Add(cusContact);
                });

                result.ListBankAccount.ForEach(item =>
                {
                    response.ListBankAccount.Add(new BankAccountModel(item));
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

                result.CountryList.ForEach(country =>
                {
                    response.CountryList.Add(new CountryModel(country));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetCustomerByIdResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public EditCustomerByIdResponse EditCustomerById(EditCustomerByIdRequest request)
        {
            try
            {
                logger.LogInformation("Edit Customer Service Level");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.EditCustomerById(parameter);
                return new EditCustomerByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditCustomerByIdResponse()
                {
                    MessageCode = CommonMessage.Customer.EDIT_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllCustomerResponse GetAllCustomer(GetAllCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Get Customer List");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetAllCustomer(parameter);
                var response = new GetAllCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerList = new List<CustomerModel>()
                };
                result.CustomerList.ForEach(customer =>
                {
                    response.CustomerList.Add(new CustomerModel(customer));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllCustomerResponse()
                {
                    MessageCode = CommonMessage.Customer.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public QuickCreateCustomerResponse QuickCreateCustomer(QuickCreateCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Quick Create Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.QuickCreateCustomer(parameter);
                var response = new QuickCreateCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    CustomerID = result.CustomerID
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new QuickCreateCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ImportCustomerResponse ImportCustomer(ImportCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Import Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.ImportCustomer(parameter);
                var response = new ImportCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    isDupblicateInFile = result.isDupblicateInFile,
                    lstcontactContactDuplicate = new List<ContactModel>(),
                    lstcontactContact_CON_Duplicate = new List<ContactModel>(),
                    lstcontactCustomerDuplicate = new List<CustomerModel>(),
                };
                if (result.lstcontactContactDuplicate != null)
                {

                    if (result.lstcontactContactDuplicate.Count > 0)
                    {
                        result.lstcontactContactDuplicate.ForEach(item =>
                        {
                            response.lstcontactContactDuplicate.Add(new ContactModel(item));
                        });

                    }
                }
                if (result.lstcontactContact_CON_Duplicate != null)
                {
                    if (result.lstcontactContact_CON_Duplicate.Count > 0)
                    {
                        result.lstcontactContact_CON_Duplicate.ForEach(item =>
                        {
                            response.lstcontactContact_CON_Duplicate.Add(new ContactModel(item));
                        });

                    }
                }
                if (result.lstcontactCustomerDuplicate != null)
                {
                    if (result.lstcontactCustomerDuplicate.Count > 0)
                    {
                        result.lstcontactCustomerDuplicate.ForEach(item =>
                        {
                            response.lstcontactCustomerDuplicate.Add(new CustomerModel(item));
                        });
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ImportCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra trong quá trình import"
                };
            }
        }

        public DownloadTemplateCustomerResponse DownloadTemplateCustomer(DownloadTemplateCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Download Template Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.DownloadTemplateCustomer(parameter);
                var response = new DownloadTemplateCustomerResponse()
                {
                    ExcelFile = result.ExcelFile,
                    NameFile = result.NameFile,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DownloadTemplateCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public UpdateCustomerDuplicateResponse UpdateCustomerDuplicate(UpdateCustomerDuplicateRequest request)
        {
            try
            {
                logger.LogInformation("Update Customer Duplicate");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.UpdateCustomerDuplicate(parameter);
                var response = new UpdateCustomerDuplicateResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new UpdateCustomerDuplicateResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }
        public GetStatisticCustomerForDashboardResponse GetStatisticCustomerForDashboard(GetStatisticCustomerForDashboardRequest request)
        {
            try
            {
                logger.LogInformation("GetStatisticCustomerForDashboard");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetStatisticCustomerForDashboard(parameter);
                var response = new GetStatisticCustomerForDashboardResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListCusCreatedInThisYear = new List<CustomerModel>(),
                    ListCusFollowProduct = result.ListCusFollowProduct,
                    ListCusIsNewBought = new List<CustomerModel>(),
                    ListCusIsNewest = new List<CustomerModel>(),
                    //ListCusSaleTop = new List<CustomerModel>(),
                    ListTopPic = new List<CustomerModel>()
                };
                result.ListCusCreatedInThisYear.ForEach(l => response.ListCusCreatedInThisYear.Add(new CustomerModel(l)));
                result.ListCusIsNewBought.ForEach(l => response.ListCusIsNewBought.Add(new CustomerModel(l)));
                result.ListCusIsNewest.ForEach(l => response.ListCusIsNewest.Add(new CustomerModel(l)));
                //result.ListCusSaleTop.ForEach(l => response.ListCusSaleTop.Add(new CustomerModel(l)));
                result.ListTopPic.ForEach(l => response.ListTopPic.Add(new CustomerModel(l)));

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetStatisticCustomerForDashboardResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }
        public GetListCustomeSaleToprForDashboardResponse GetListCustomeSaleToprForDashboard(GetListCustomeSaleToprForDashboardRequest request)
        {
            try
            {
                logger.LogInformation("GetListCustomeSaleToprForDashboard");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetListCustomeSaleToprForDashboard(parameter);
                var response = new GetListCustomeSaleToprForDashboardResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListCusSaleTop = new List<CustomerModel>(),
                };
                result.ListCusSaleTop.ForEach(l => response.ListCusSaleTop.Add(new CustomerModel(l)));

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetListCustomeSaleToprForDashboardResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }
        public CheckDuplicateCustomerResponse CheckDuplicateCustomerPhoneOrEmail(CheckDuplicateCustomerLeadRequest request)
        {
            try
            {
                logger.LogInformation("CheckDuplicateCustomerPhoneOrEmail");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CheckDuplicateCustomerPhoneOrEmail(parameter);
                var response = new CheckDuplicateCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ContactId = result.ContactId,
                    CustomerId = result.CustomerId,
                    IsDuplicate = result.IsDuplicate
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CheckDuplicateCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
            throw new NotImplementedException();
        }
        public CheckDuplicateCustomerResponse CheckDuplicateCustomer(CheckDuplicateCustomerRequest request)
        {
            try
            {
                logger.LogInformation("CheckDuplicateCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CheckDuplicateCustomer(parameter);
                var response = new CheckDuplicateCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ContactId = result.ContactId,
                    CustomerId = result.CustomerId,
                    LeadId = result.LeadId,
                    IsDuplicate = result.IsDuplicate,
                    IsDuplicateByEmailLead = result.IsDuplicateByEmailLead,
                    IsDuplicateByEmailCustomer = result.IsDuplicateByEmailCustomer,
                    IsDuplicateByPhoneLead = result.IsDuplicateByPhoneLead,
                    IsDuplicateByPhoneCustomer = result.IsDuplicateByPhoneCustomer
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CheckDuplicateCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
            throw new NotImplementedException();
        }

        public CheckDuplicatePersonalCustomerResponse CheckDuplicatePersonalCustomer(CheckDuplicatePersonalCustomerRequest request)
        {
            try
            {
                logger.LogInformation("CheckDuplicatePersonalCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CheckDuplicatePersonalCustomer(parameter);
                var response = new CheckDuplicatePersonalCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    IsDuplicateLead = result.IsDuplicateLead,
                    IsDuplicateCustomerByEmail = result.IsDuplicateCustomerByEmail,
                    IsDuplicateCustomerByPhone = result.IsDuplicateCustomerByPhone,
                    LeadId = result.LeadId,
                    CustomerId = result.CustomerId,
                    ContactId = result.ContactId
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CheckDuplicatePersonalCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public CheckDuplicatePersonalCustomerByEmailOrPhoneResponse CheckDuplicatePersonalCustomerByEmailOrPhone(CheckDuplicatePersonalCustomerByEmailOrPhoneRequest request)
        {
            try
            {
                logger.LogInformation("CheckDuplicatePersonalCustomerByEmailOrPhone");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CheckDuplicatePersonalCustomerByEmailOrPhone(parameter);
                var response = new CheckDuplicatePersonalCustomerByEmailOrPhoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    IsDuplicateCustomer = result.IsDuplicateCustomer,
                    IsDuplicateLead = result.IsDuplicateLead,
                    LeadId = result.LeadId,
                    CustomerId = result.CustomerId,
                    ContactId = result.ContactId
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CheckDuplicatePersonalCustomerByEmailOrPhoneResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public GetAllCustomerAdditionalByCustomerIdResponse GetAllCustomerAdditionalByCustomerId(GetAllCustomerAdditionalByCustomerIdRequest request)
        {
            try
            {
                logger.LogInformation("Get All CustomerAdditionalInformation By CustomerId");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetAllCustomerAdditionalByCustomerId(parameter);
                var response = new GetAllCustomerAdditionalByCustomerIdResponse()
                {
                    CustomerAdditionalInformationList = result.CustomerAdditionalInformationList,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllCustomerAdditionalByCustomerIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public CreateCustomerAdditionalResponse CreateCustomerAdditional(CreateCustomerAdditionalRequest request)
        {
            try
            {
                logger.LogInformation("Create Customer Additional Information");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CreateCustomerAdditional(parameter);
                var response = new CreateCustomerAdditionalResponse()
                {
                    ListCustomerAdditionalInformation = new List<CustomerAdditionalInformationModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                result.ListCustomerAdditionalInformation.ForEach(item =>
                {
                    var cusAdd = new CustomerAdditionalInformationModel();
                    cusAdd.CustomerAdditionalInformationId = item.CustomerAdditionalInformationId;
                    cusAdd.CustomerId = item.CustomerId;
                    cusAdd.Question = item.Question;
                    cusAdd.Answer = item.Answer;

                    response.ListCustomerAdditionalInformation.Add(cusAdd);
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateCustomerAdditionalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCustomerAdditionalResponse DeleteCustomerAdditional(DeleteCustomerAdditionalRequest request)
        {
            try
            {
                logger.LogInformation("Delete Customer Additional");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.DeleteCustomerAdditional(parameter);
                var response = new DeleteCustomerAdditionalResponse()
                {
                    ListCustomerAdditionalInformation = new List<CustomerAdditionalInformationModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                result.ListCustomerAdditionalInformation.ForEach(item =>
                {
                    var cusAdd = new CustomerAdditionalInformationModel();
                    cusAdd.CustomerAdditionalInformationId = item.CustomerAdditionalInformationId;
                    cusAdd.CustomerId = item.CustomerId;
                    cusAdd.Question = item.Question;
                    cusAdd.Answer = item.Answer;

                    response.ListCustomerAdditionalInformation.Add(cusAdd);
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DeleteCustomerAdditionalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public EditCustomerAdditionalResponse EditCustomerAdditional(EditCustomerAdditionalRequest request)
        {
            try
            {
                logger.LogInformation("Edit Customer Additional Information");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.EditCustomerAdditional(parameter);
                var response = new EditCustomerAdditionalResponse()
                {
                    CustomerAdditionalInformationId = result.CustomerAdditionalInformationId,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditCustomerAdditionalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public CreateListQuestionResponse CreateListQuestion(CreateListQuestionRequest request)
        {
            try
            {
                logger.LogInformation("Create List Question");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CreateListQuestion(parameter);
                var response = new CreateListQuestionResponse()
                {
                    ListCustomerAdditionalInformation = new List<CustomerAdditionalInformationModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                result.ListCustomerAdditionalInformation.ForEach(item =>
                {
                    var cusAdd = new CustomerAdditionalInformationModel();
                    cusAdd.CustomerAdditionalInformationId = item.CustomerAdditionalInformationId;
                    cusAdd.CustomerId = item.CustomerId;
                    cusAdd.Question = item.Question;
                    cusAdd.Answer = item.Answer;

                    response.ListCustomerAdditionalInformation.Add(cusAdd);
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateListQuestionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public GetListQuestionAnswerBySearchResponse GetListQuestionAnswerBySearch(GetListQuestionAnswerBySearchRequest request)
        {
            try
            {
                logger.LogInformation("Get List Question Answer By Search");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetListQuestionAnswerBySearch(parameter);
                var response = new GetListQuestionAnswerBySearchResponse()
                {
                    CustomerAdditionalInformationList = result.CustomerAdditionalInformationList,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetListQuestionAnswerBySearchResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public GetAllHistoryProductByCustomerIdResponse GetAllHistoryProductByCustomerId(GetAllHistoryProductByCustomerIdRequest request)
        {
            try
            {
                logger.LogInformation("Get All History Product By CustomerId");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetAllHistoryProductByCustomerId(parameter);
                var response = new GetAllHistoryProductByCustomerIdResponse()
                {
                    listProduct = result.listProduct,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllHistoryProductByCustomerIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public CreateCustomerFromProtalResponse CreateCustomerFromProtal(CreateCustomerFromProtalRequest request)
        {
            try
            {
                logger.LogInformation("Create Customer From Protal");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CreateCustomerFromProtal(parameter);
                var response = new CreateCustomerFromProtalResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateCustomerFromProtalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public ChangeCustomerStatusToDeleteResponse ChangeCustomerStatusToDelete(ChangeCustomerStatusToDeleteRequest request)
        {
            try
            {
                logger.LogInformation("Change Customer Status To Delete");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.ChangeCustomerStatusToDelete(parameter);
                var response = new ChangeCustomerStatusToDeleteResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ChangeCustomerStatusToDeleteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Customer.DELETE_FAIL
                };
            }
        }

        public GetDashBoardCustomerResponse GetDashBoardCustomer(GetDashBoardCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Get Dash Board Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDashBoardCustomer(parameter);
                var response = new GetDashBoardCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCusFollowProduct = result.ListCusFollowProduct,
                    ListTopPic = new List<CustomerModel>(),
                    ListCusCreatedInThisYear = new List<CustomerModel>(),
                    ListCusTopRevenueInMonth = new List<CustomerModel>(),
                    ListCusIdentification = new List<CustomerModel>(),
                    ListCusFree = new List<CustomerModel>(),
                };

                result.ListTopPic.ForEach(item => response.ListTopPic.Add(new CustomerModel(item)));
                result.ListCusCreatedInThisYear.ForEach(item => response.ListCusCreatedInThisYear.Add(new CustomerModel(item)));
                result.ListCusTopRevenueInMonth.ForEach(item => response.ListCusTopRevenueInMonth.Add(new CustomerModel(item)));
                result.ListCusIdentification.ForEach(item => response.ListCusIdentification.Add(new CustomerModel(item)));
                result.ListCusFree.ForEach(item => response.ListCusFree.Add(new CustomerModel(item)));

                return response;
            }
            catch (Exception e)
            {
                return new GetDashBoardCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListCustomerResponse GetListCustomer(GetListCustomerRequest request)
        {
            logger.LogInformation("Get Lis tCustomer");
            var parameter = request.ToParameter();
            var result = iCustomerDataAccess.GetListCustomer(parameter);
            var response = new GetListCustomerResponse()
            {
                ListAreaModel = new List<AreaModel>(),
                ListSourceModel = new List<CategoryModel>(),
                ListStatusCareModel = new List<CategoryModel>(),
                ListCategoryModel = new List<CategoryModel>(),
                ListCustomerServiceLevelModel = new List<Models.Customer.CustomerServiceLevelModel>(),
                StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                MessageCode = result.Message
            };
            result.ListAreaEntityModel.ForEach(x => response.ListAreaModel.Add(new AreaModel(x)));
            result.ListSourceEntityModel.ForEach(x => response.ListSourceModel.Add(new CategoryModel(x)));
            result.ListStatusCareEntityModel.ForEach(x => response.ListStatusCareModel.Add(new CategoryModel(x)));
            result.ListCategoryEntityModel.ForEach(cate => response.ListCategoryModel.Add(new CategoryModel(cate)));
            result.ListCustomerServiceLevel.ForEach(level => response.ListCustomerServiceLevelModel.Add(new Models.Customer.CustomerServiceLevelModel(level)));
            return response;
        }

        public CreateCustomerMasterDataResponse CreateCustomerMasterData(CreateCustomerMasterDataRequest request)
        {
            logger.LogInformation("Create Customer Master Data");
            var parameter = request.ToParameter();
            var result = iCustomerDataAccess.CreateCustomerMasterData(parameter);
            var response = new CreateCustomerMasterDataResponse()
            {
                ListCustomerGroup = new List<CategoryModel>(),
                ListEnterPriseType = new List<CategoryModel>(),
                ListBusinessScale = new List<CategoryModel>(),
                ListPosition = new List<CategoryModel>(),
                ListBusinessLocal = new List<CategoryModel>(),
                ListMainBusiness = new List<CategoryModel>(),
                ListProvinceModel = new List<ProvinceModel>(),
                ListDistrictModel = new List<DistrictModel>(),
                ListWardModel = new List<WardModel>(),
                ListEmployeeModel = new List<Models.Employee.EmployeeModel>(),
                ListCustomerCode = new List<string>(),
                ListCustomerTax = new List<string>(),
                ListArea = result.ListArea,
                ListCustomer = result.ListCustomer,
                StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                MessageCode = result.Message
            };

            result.ListCustomerGroupEntity.ForEach(customerGroup => response.ListCustomerGroup.Add(new CategoryModel(customerGroup)));
            result.ListEnterPriseTypeEntity.ForEach(enterprise => response.ListEnterPriseType.Add(new CategoryModel(enterprise)));
            result.ListBusinessScaleEntity.ForEach(scale => response.ListBusinessScale.Add(new CategoryModel(scale)));
            result.ListPositionEntity.ForEach(pos => response.ListPosition.Add(new CategoryModel(pos)));
            result.ListBusinessLocalEntity.ForEach(local => response.ListBusinessLocal.Add(new CategoryModel(local)));
            result.ListMainBusinessEntity.ForEach(mainbusiness => response.ListMainBusiness.Add(new CategoryModel(mainbusiness)));
            result.ListProvinceEntityModel.ForEach(province => response.ListProvinceModel.Add(new ProvinceModel(province)));
            result.ListDistrictEntityModel.ForEach(district => response.ListDistrictModel.Add(new DistrictModel(district)));
            result.ListWardEntityModel.ForEach(ward => response.ListWardModel.Add(new WardModel(ward)));
            result.ListEmployeeEntityModel.ForEach(emp => response.ListEmployeeModel.Add(new Models.Employee.EmployeeModel(emp)));
            result.ListCustomerCode.ForEach(code => response.ListCustomerCode.Add(code));
            result.ListCustomerTax.ForEach(code => response.ListCustomerTax.Add(code));

            return response;
        }

        public CheckDuplicateCustomerAllTypeResponse CheckDuplicateCustomerAllType(CheckDuplicateCustomerAllTypeRequest request)
        {
            logger.LogInformation("Check Duplicate Customer All Type");
            var parameter = request.ToParameter();
            var result = iCustomerDataAccess.CheckDuplicateCustomerAllType(parameter);
            var response = new CheckDuplicateCustomerAllTypeResponse()
            {
                IsDuplicateLead = result.IsDuplicateLead,
                IsDuplicateCustomer = result.IsDuplicateCustomer,
                DuplicateLeadModel = new Models.Lead.LeadModel(result.DuplicateLeadModel),
                DuplicateLeadContactModel = new Models.Contact.ContactModel(result.DuplicateLeadContactModel),
                DuplicateCustomerContactModel = new Models.Contact.ContactModel(result.DuplicateCustomerContactModel),
                StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                MessageCode = result.Message
            };
            return response;
        }

        public UpdateCustomerByIdResponse UpdateCustomerById(UpdateCustomerByIdRequest request)
        {
            try
            {
                logger.LogInformation("Update Customer By Id");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.UpdateCustomerById(parameter);
                var response = new UpdateCustomerByIdResponse()
                {
                    SendEmailEntityModel = result.SendEmailEntityModel,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateCustomerByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetCustomerImportDetailResponse GetCustomerImportDetail(GetCustomerImportDetailRequest request)
        {
            try
            {
                logger.LogInformation("Get Customer Import Detail");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetCustomerImportDetail(parameter);
                var response = new GetCustomerImportDetailResponse()
                {
                    ListCustomerCompanyCode = new List<string>(),
                    ListCustomerGroup = new List<CategoryModel>(),
                    ListEmail = new List<string>(),
                    ListPhone = new List<string>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
                result.ListCustomerCompanyCode.ForEach(code => response.ListCustomerCompanyCode.Add(code));
                result.ListCustomerGroup.ForEach(group => response.ListCustomerGroup.Add(new CategoryModel(group)));
                result.ListEmail.ForEach(email => response.ListEmail.Add(email));
                result.ListPhone.ForEach(phone => response.ListPhone.Add(phone));
                return response;
            }
            catch (Exception e)
            {
                return new GetCustomerImportDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public ImportListCustomerResponse ImportListCustomer(ImportListCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Import List Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.ImportListCustomer(parameter);
                var response = new ImportListCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new ImportListCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteListCustomerAdditionalResponse DeleteListCustomerAdditional(DeleteListCustomerAdditionalRequest request)
        {
            try
            {
                logger.LogInformation("Delete List CustomerAdditional");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.DeleteListCustomerAdditional(parameter);
                var response = new DeleteListCustomerAdditionalResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCustomerAdditionalInformation = new List<CustomerAdditionalInformationModel>()
                };

                result.ListCustomerAdditionalInformation.ForEach(item =>
                {
                    var cusAdd = new CustomerAdditionalInformationModel();
                    cusAdd.CustomerAdditionalInformationId = item.CustomerAdditionalInformationId;
                    cusAdd.CustomerId = item.CustomerId;
                    cusAdd.Question = item.Question;
                    cusAdd.Answer = item.Answer;

                    response.ListCustomerAdditionalInformation.Add(cusAdd);
                });

                return response;
            }
            catch (Exception e)
            {
                return new DeleteListCustomerAdditionalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetHistoryCustomerCareResponse GetHistoryCustomerCare(GetHistoryCustomerCareRequest request)
        {
            try
            {
                logger.LogInformation("Get History Customer Care");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetHistoryCustomerCare(parameter);
                var response = new GetHistoryCustomerCareResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCustomerCareInfor = new List<CustomerCareInforBusinessModel>(),
                };

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

                return response;
            }
            catch (Exception e)
            {
                return new GetHistoryCustomerCareResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataPreviewCustomerCareResponse GetDataPreviewCustomerCare(GetDataPreviewCustomerCareRequest request)
        {
            try
            {
                logger.LogInformation("Get Data Preview Customer Care");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataPreviewCustomerCare(parameter);
                var response = new GetDataPreviewCustomerCareResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    EffecttiveFromDate = result.EffecttiveFromDate,
                    EffecttiveToDate = result.EffecttiveToDate,
                    SendDate = result.SendDate,
                    StatusName = result.StatusName,
                    PreviewSmsContent = result.PreviewSmsContent,
                    PreviewSmsPhone = result.PreviewSmsPhone,
                    PreviewEmailName = result.PreviewEmailName,
                    PreviewEmailContent = result.PreviewEmailContent,
                    PreviewEmailTitle = result.PreviewEmailTitle
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataPreviewCustomerCareResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCustomerCareFeedBackResponse GetDataCustomerCareFeedBack(GetDataCustomerCareFeedBackRequest request)
        {
            try
            {
                logger.LogInformation("Get Data CustomerCareFeedBack");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataCustomerCareFeedBack(parameter);
                var response = new GetDataCustomerCareFeedBackResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    Name = result.Name,
                    FromDate = result.FromDate,
                    ToDate = result.ToDate,
                    TypeName = result.TypeName,
                    FeedBackCode = result.FeedBackCode,
                    FeedBackContent = result.FeedBackContent,
                    ListFeedBackCode = new List<CategoryModel>()
                };

                result.ListFeedBackCode.ForEach(item =>
                {
                    response.ListFeedBackCode.Add(new CategoryModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataCustomerCareFeedBackResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SaveCustomerCareFeedBackResponse SaveCustomerCareFeedBack(SaveCustomerCareFeedBackRequest request)
        {
            try
            {
                logger.LogInformation("Save CustomerCareFeedBack");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.SaveCustomerCareFeedBack(parameter);
                var response = new SaveCustomerCareFeedBackResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveCustomerCareFeedBackResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCustomerMeetingByIdResponse GetDataCustomerMeetingById(GetDataCustomerMeetingByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Data Customer Meeting By Id");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataCustomerMeetingById(parameter);
                var response = new GetDataCustomerMeetingByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    CustomerMeeting = new CustomerMeetingModel()
                };

                response.CustomerMeeting.CustomerMeetingId = result.CustomerMeeting.CustomerMeetingId;
                response.CustomerMeeting.CustomerId = result.CustomerMeeting.CustomerId;
                response.CustomerMeeting.EmployeeId = result.CustomerMeeting.EmployeeId;
                response.CustomerMeeting.Title = result.CustomerMeeting.Title;
                response.CustomerMeeting.LocationMeeting = result.CustomerMeeting.LocationMeeting;
                response.CustomerMeeting.StartDate = result.CustomerMeeting.StartDate;
                //response.CustomerMeeting.StartHours = result.CustomerMeeting.StartHours;
                response.CustomerMeeting.EndDate = result.CustomerMeeting.EndDate;
                response.CustomerMeeting.Content = result.CustomerMeeting.Content;
                response.CustomerMeeting.Participants = result.CustomerMeeting.Participants;

                return response;
            }
            catch (Exception e)
            {
                return new GetDataCustomerMeetingByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateCustomerMeetingResponse CreateCustomerMeeting(CreateCustomerMeetingRequest request)
        {
            try
            {
                logger.LogInformation("Create Customer Meeting");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CreateCustomerMeeting(parameter);
                var response = new CreateCustomerMeetingResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateCustomerMeetingResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetHistoryCustomerMeetingResponse GetHistoryCustomerMeeting(GetHistoryCustomerMeetingRequest request)
        {
            try
            {
                logger.LogInformation("Get History Customer Meeting");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetHistoryCustomerMeeting(parameter);
                var response = new GetHistoryCustomerMeetingResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    CustomerMeetingInfor = new CustomerMeetingInforBusinessModel()
                };

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
            catch (Exception e)
            {
                return new GetHistoryCustomerMeetingResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SendApprovalResponse SendApproval(SendApprovalRequest request)
        {
            try
            {
                logger.LogInformation("Get Send Approval");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.SendApproval(parameter);
                var response = new SendApprovalResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new SendApprovalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchCustomerResponse GetListCustomerRequestApproval(SearchCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Search Customer Approval");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetListCustomerRequestApproval(parameter);
                var response = new SearchCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Customer = new List<CustomerSearchModel>()
                };
                result.Customer.ForEach(customer =>
                {
                    response.Customer.Add(new CustomerSearchModel(customer));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchCustomerResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendApprovalResponse ApprovalOrRejectCustomer(ApprovalOrRejectCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Approval Or RejectCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.ApprovalOrRejectCustomer(parameter);
                var response = new SendApprovalResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SendApprovalResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetDataCreatePotentialCustomerResponse GetDataCreatePotentialCustomer(GetDataCreatePotentialCustomerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataCreatePotentialCustomer(parameter);
                var response = new GetDataCreatePotentialCustomerResponse()
                {
                    ListEmployeeModel = new List<EmployeeModel>(),
                    ListInvestFund = new List<CategoryModel>(),
                    ListGroupCustomer = new List<CategoryModel>(),
                    ListArea = result.ListArea,
                    ListProvinceEntityModel = result.ListProvinceEntityModel,
                    ListWardEntityModel = result.ListWardEntityModel,
                    ListDistrictEntityModel = result.ListDistrictEntityModel,
                    ListCustomer = result.ListCustomer,
                    ListEmployeeTakeCare = result.ListEmployeeTakeCare,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                //result.ListCustomerGroupEntity.ForEach(customerGroup => response.ListCustomerGroup.Add(new CategoryModel(customerGroup)));
                //result.ListEnterPriseTypeEntity.ForEach(enterprise => response.ListEnterPriseType.Add(new CategoryModel(enterprise)));
                //result.ListBusinessScaleEntity.ForEach(scale => response.ListBusinessScale.Add(new CategoryModel(scale)));

                result.ListInvestFund.ForEach(e => response.ListInvestFund.Add(new CategoryModel(e)));
                result.ListGroupCus.ForEach(e => response.ListGroupCustomer.Add(new CategoryModel(e)));
                result.ListEmployeeEntityModel.ForEach(emp => response.ListEmployeeModel.Add(new Models.Employee.EmployeeModel(emp)));

                return response;
            }
            catch (Exception e)
            {
                return new GetDataCreatePotentialCustomerResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetDataDetailPotentialCustomerResponse GetDataDetailPotentialCustomer(GetDataDetailPotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Get Data Detail Potential Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataDetailPotentialCustomer(parameter);
                var response = new GetDataDetailPotentialCustomerResponse()
                {
                    ListPersonalInChange = new List<EmployeeModel>(),
                    ListInvestFund = new List<CategoryModel>(),
                    PotentialCustomerModel = new CustomerModel(result.PotentialCustomerModel),
                    PotentialCustomerContactModel = new ContactModel(result.PotentialCustomerContactModel),
                    ListContact = new List<ContactModel>(),
                    ListFileByPotentialCustomer = result.ListFileByPotentialCustomer,
                    ListProduct = result.ListProduct,
                    ListLinkOfDocument = result.ListLinkOfDocument,
                    ListPotentialCustomerProduct = result.ListPotentialCustomerProduct,
                    ListQuoteByPotentialCustomer = result.ListQuoteByPotentialCustomer,
                    ListLeadByPotentialCustomer = result.ListLeadByPotentialCustomer,
                    ListCustomerCareInfor = result.ListCustomerCareInfor,
                    CustomerMeetingInfor = result.CustomerMeetingInfor,
                    ListParticipants = result.ListParticipants,
                    ListNote = result.ListNote ?? new List<DataAccess.Models.Note.NoteEntityModel>(),
                    ListArea = result.ListArea,
                    ListCusGroup = result.ListCusGroup,
                    ListStatusSupport = result.ListStatusSupport,
                    StatusSupportId = result.StatusSupportId,
                    StatusCustomerCode = result.StatusCustomerCode,
                    ListEmpTakeCare = result.ListEmpTakeCare,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                result.ListContact.ForEach(e => response.ListContact.Add(new ContactModel(e)));
                result.ListInvestFund.ForEach(e => response.ListInvestFund.Add(new CategoryModel(e)));
                result.ListPersonalInChange.ForEach(emp => response.ListPersonalInChange.Add(new Models.Employee.EmployeeModel(emp)));

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetDataDetailPotentialCustomerResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public UpdatePotentialCustomerResponse UpdatePotentialCustomer(UpdatePotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("UpdatePotentialCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.UpdatePotentialCustomer(parameter);
                var response = new UpdatePotentialCustomerResponse()
                {
                    ListNote = result.ListNote,
                    ListFileByPotentialCustomer = result.ListFileByPotentialCustomer,
                    ListLinkOfDocument = result.ListLinkOfDocument,
                    ListEmpTakeCare = result.ListEmpTakeCare,
                    ListPersonalInChange = result.ListPersonalInChange,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new UpdatePotentialCustomerResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetDataSearchPotentialCustomerResponse GetDataSearchPotentialCustomer(GetDataSearchPotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Get Data Create Potential Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataSearchPotentialCustomer(parameter);
                var response = new GetDataSearchPotentialCustomerResponse()
                {
                    ListEmployeeModel = new List<EmployeeModel>(),
                    ListInvestFund = new List<CategoryModel>(),
                    ListCareState = result.ListCareState,
                    ListArea = result.ListArea,
                    ListCusType = result.ListCusType,
                    ListCusGroup = result.ListCusGroup,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                result.ListInvestFund.ForEach(e => response.ListInvestFund.Add(new CategoryModel(e)));
                result.ListEmployeeEntityModel.ForEach(emp => response.ListEmployeeModel.Add(new Models.Employee.EmployeeModel(emp)));

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetDataSearchPotentialCustomerResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public SearchPotentialCustomerResponse SearchPotentialCustomer(SearchPotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Search Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.SearchPotentialCustomer(parameter);
                var response = new SearchPotentialCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListPotentialCustomer = result.ListPotentialCustomer ?? new List<DataAccess.Models.Customer.CustomerEntityModel>()
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchPotentialCustomerResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetDataDashboardPotentialCustomerResponse GetDataDashboardPotentialCustomer(GetDataDashboardPotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("Search Customer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataDashboardPotentialCustomer(parameter);
                var response = new GetDataDashboardPotentialCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListInvestmentFundDasboard = result.ListInvestmentFundDasboard ?? new List<DataAccess.Models.Customer.ItemInvestmentChartPotentialCustomerDashboard>(),
                    TopNewestCustomer = result.TopNewestCustomer ?? new List<DataAccess.Models.Customer.CustomerEntityModel>(),
                    TopNewestCustomerConverted = result.TopNewestCustomerConverted ?? new List<DataAccess.Models.Customer.CustomerEntityModel>(),
                    FunnelChartPotentialDasboardModel = result.FunnelChartPotentialDasboardModel
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetDataDashboardPotentialCustomerResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ConvertPotentialCustomerResponse ConvertPotentialCustomer(ConvertPotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("ConvertPotentialCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.ConvertPotentialCustomer(parameter);
                var response = new ConvertPotentialCustomerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ConvertPotentialCustomerResponse()
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public DownloadTemplatePotentialCustomerResponse DownloadTemplatePotentialCustomer(DownloadTemplatePotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("DownloadTemplatePotentialCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.DownloadTemplatePotentialCustomer(parameter);
                var response = new DownloadTemplatePotentialCustomerResponse()
                {
                    ExcelFile = result.ExcelFile,
                    NameFile = result.NameFile,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DownloadTemplatePotentialCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public GetDataImportPotentialCustomerResponse GetDataImportPotentialCustomer(GetDataImportPotentialCustomerRequest request)
        {
            try
            {
                logger.LogInformation("GetDataImportPotentialCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.GetDataImportPotentialCustomer(parameter);
                var response = new GetDataImportPotentialCustomerResponse()
                {
                    ListPersonalInChange = result.ListPersonalInChange ?? new List<DataAccess.Models.Employee.EmployeeEntityModel>(),
                    ListInvestFund = result.ListInvestFund,
                    ListEmail = result.ListEmail,
                    ListPhone = result.ListPhone,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetDataImportPotentialCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "Có lỗi xảy ra"
                };
            }
        }

        public DownloadTemplateImportCustomerResponse DownloadTemplateImportCustomer(DownloadTemplateImportCustomerRequest request)
        {
            try
            {
                this.logger.LogInformation("DownloadTemplateImportCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.DownloadTemplateImportCustomer(parameter);

                var response = new DownloadTemplateImportCustomerResponse()
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
                return new DownloadTemplateImportCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchContactCustomerResponse SearchContactCustomer(SearchContactCustomerRequest request)
        {
            try
            {
                this.logger.LogInformation("DownloadTemplateImportCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.SearchContactCustomer(parameter);

                var response = new SearchContactCustomerResponse
                {
                    MessageCode = result.Message,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    ListContact = result.ListContactCustomer
                };

                return response;
            }
            catch (Exception ex)
            {
                return new SearchContactCustomerResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public CheckDuplicateInforCustomerResponse CheckDuplicateInforCustomer(CheckDuplicateInforCustomerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.CheckDuplicateInforCustomer(parameter);

                var response = new CheckDuplicateInforCustomerResponse
                {
                    MessageCode = result.Message,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    Valid = result.Valid
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckDuplicateInforCustomerResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public ChangeStatusSupportResponse ChangeStatusSupport(ChangeStatusSupportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerDataAccess.ChangeStatusSupport(parameter);

                var response = new ChangeStatusSupportResponse
                {
                    MessageCode = result.Message,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeStatusSupportResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }
    }
}
