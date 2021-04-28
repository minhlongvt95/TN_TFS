using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Lead;
using TN.TNM.BusinessLogic.Messages.Requests.Lead;
using TN.TNM.BusinessLogic.Messages.Responses.Leads;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Company;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Lead
{
    /// <summary>
    /// TODO: 
    /// </summary>
    public class LeadSearchFactory : BaseFactory, ILeadSearch
    {
        private ILeadSearchDataAccess iLeadDataAccess;
        public LeadSearchFactory(ILeadSearchDataAccess _iLeadDataAccess, ILogger<LeadSearchFactory> _logger)
        {
            this.iLeadDataAccess = _iLeadDataAccess;
            this.logger = _logger;
        }
        /// <summary>
        /// GetAllLead
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetAllLeadResponse GetAllLead(GetAllLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Lead by parameter");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetAllLead(parameter);
                var response = new GetAllLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListLead = new List<GetAllLeadModel>(),
                };
                if (result.ListLead != null)
                {
                    result.ListLead.ForEach(leadEntity =>
                    {
                        response.ListLead.Add(new GetAllLeadModel(leadEntity));
                    });
                }

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllLeadResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        /// <summary>
        /// CreateLead
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateLeadResponse CreateLead(CreateLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Lead by parameter");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.CreateLead(parameter);
                var response = new CreateLeadResponse();
                if (result.Status)
                {
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.LeadId = parameter.Lead.LeadId;
                    response.ContactId = parameter.Contact.ContactId;
                    response.PicName = result.PicName;
                    response.StatusName = result.StatusName;
                    response.Potential = result.Potential;
                    response.SendEmailEntityModel = result.SendEmailEntityModel;
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.MessageCode = CommonMessage.Lead.CREATE_FAIL;
                    response.PicName = "";
                    response.StatusName = "";
                    response.Potential = "";
                }
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateLeadResponse()
                {
                    MessageCode = CommonMessage.Lead.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        /// <summary>
        /// GetLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetLeadByIdResponse GetLeadById(GetLeadByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Lead by Id");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetLeadById(parameter);
                var response = new GetLeadByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    Lead = new LeadModel(result.Lead),
                    Contact = new ContactModel(result.Contact),
                    ListCompany = new List<CompanyModel>(),
                    Company = new CompanyModel(result.Company),
                    InterestedGroupName = result.InterestedGroupName,
                    PotentialName = result.PotentialName,
                    Status_Name = result.Status_Name,
                    PIC_Name = result.PIC_Name,
                    ResponsibleName = result.ResponsibleName,
                    Status_Code = result.Status_Code,
                    Employee = new List<EmployeeModel>(),
                    StatusCategory = new List<CategoryModel>(),
                    Potential = new List<CategoryModel>(),
                    PositionName = result.PositionName,
                    PaymentMethod = new List<CategoryModel>(),
                    InterestedList = new List<CategoryModel>(),
                    InterestedName = result.InterestedName,
                    PaymentMethodName = result.PaymentMethodName,
                    Genders = result.Genders.Select(g => new CategoryModel(g)).ToList(),
                    FullAddress = result.FullAddress,
                    CountLead = result.CountLead,
                    StatusSaleBiddingAndQuote = result.StatusSaleBiddingAndQuote,
                };
                result.PaymentMethod.ForEach(paymentEntity =>
                {
                    response.PaymentMethod.Add(new CategoryModel(paymentEntity));
                });
                result.InterestedList.ForEach(interestedEntity =>
                {
                    response.InterestedList.Add(new CategoryModel(interestedEntity));
                });
                result.ListCompany.ForEach(companyEntity =>
                {
                    response.ListCompany.Add(new CompanyModel(companyEntity));
                });
                result.Employee.ForEach(employeeEntity =>
                {
                    response.Employee.Add(new EmployeeModel(employeeEntity));
                });

                result.StatusCategory.ForEach(statusEntity =>
                {
                    response.StatusCategory.Add(new CategoryModel(statusEntity));
                });

                result.Potential.ForEach(potentialEntity =>
                {
                    response.Potential.Add(new CategoryModel(potentialEntity));
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetLeadByIdResponse
                {
                    MessageCode = CommonMessage.Lead.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        /// <summary>
        /// EditLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EditLeadByIdResponse EditLeadById(EditLeadByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Lead by parameter");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.EditLeadById(parameter);
                var response = new EditLeadByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    IsChangePotential = result.IsChangePotential,
                    IsChangeStatus = result.IsChangeStatus,
                    IsChangePic = result.IsChangePic,
                    StatusName = result.StatusName,
                    Potential = result.Potential,
                    PicName = result.PicName,
                    SendEmailEntityModel = result.SendEmailEntityModel
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new EditLeadByIdResponse
                {
                    MessageCode = CommonMessage.Lead.EDIT_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        /// <summary>
        /// GetNoteHistory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetNoteHistoryResponse GetNoteHistory(GetNoteHistoryRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Note history by parameter");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetNoteHistory(parameter);
                var response = new GetNoteHistoryResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListNote = new List<NoteModel>()
                };

                foreach (var noteEntityModel in result.ListNode.OrderByDescending(n => n.CreatedDate).ToList())
                {
                    response.ListNote.Add(new NoteModel(noteEntityModel));
                }

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetNoteHistoryResponse
                {
                    MessageCode = "Lay note that bai",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        /// <summary>
        /// GetAllLead
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetLeadByStatusResponse GetLeadByStatus(GetLeadByStatusRequest request)
        {
            try
            {
                this.logger.LogInformation("Get unfollow Lead by parameter");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetLeadByStatus(parameter);
                var response = new GetLeadByStatusResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListLead = new List<LeadModel>()
                };
                result.ListLead.ForEach(leadEntity =>
                {
                    response.ListLead.Add(new LeadModel(leadEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetLeadByStatusResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        /// <summary>
        /// Get Lead by Name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetLeadByNameResponse GetLeadByName(GetLeadByNameRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Lead by Name");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetLeadByName(parameter);
                var response = new GetLeadByNameResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListLead = new List<LeadModel>()
                };
                result.ListLead.ForEach(leadEntity =>
                {
                    response.ListLead.Add(new LeadModel(leadEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetLeadByNameResponse
                {
                    MessageCode = "Lay lead theo ten that bai",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeWithNotificationPermisisonResponse GetEmployeeWithNotificationPermisison(GetEmployeeWithNotificationPermisisonRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Lead by Name");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetEmployeeWithNotificationPermisison(parameter);
                var response = new GetEmployeeWithNotificationPermisisonResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    EmployeeList = new List<EmployeeModel>()
                };
                result.EmployeeList.ForEach(empEntity =>
                {
                    response.EmployeeList.Add(new EmployeeModel(empEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetEmployeeWithNotificationPermisisonResponse()
                {
                    MessageCode = "Khong lay dc nhan vien cung quyen",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeLeadStatusToUnfollowResponse ChangeLeadStatusToUnfollow(ChangeLeadStatusToUnfollowRequest request)
        {
            try
            {
                this.logger.LogInformation("Change Lead status to Unfollow");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ChangeLeadStatusToUnfollow(parameter);
                var response = new ChangeLeadStatusToUnfollowResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new ChangeLeadStatusToUnfollowResponse()
                {
                    MessageCode = CommonMessage.Workflow.CHANGE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ApproveRejectUnfollowLeadResponse ApproveRejectUnfollowLead(ApproveRejectUnfollowLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Approve or Reject change Lead status to Unfollow request");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ApproveRejectUnfollowLead(parameter);
                var response = new ApproveRejectUnfollowLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new ApproveRejectUnfollowLeadResponse()
                {
                    MessageCode = CommonMessage.Workflow.CHANGE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeManagerResponse GetEmployeeManager(GetEmployeeManagerRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Employee's manager");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetEmployeeManager(parameter);
                var response = new GetEmployeeManagerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ManagerList = new List<EmployeeModel>()
                };

                result.ManagerList.ForEach(manager =>
                {
                    response.ManagerList.Add(new EmployeeModel(manager));
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetEmployeeManagerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailLeadResponse SendEmailLead(SendEmailLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email Lead");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.SendEmailLead(parameter);
                var response = new SendEmailLeadResponse
                {
                    ListCustomerEmailIgnored = new List<Models.Contact.ContactModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
                result.ListCustomerEmailIgnored.ForEach(customer => response.ListCustomerEmailIgnored.Add(new Models.Contact.ContactModel(customer)));
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SendSMSLeadResponse SendSMSLead(SendSMSLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Send SMS Lead");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.SendSMSLead(parameter);
                var response = new SendSMSLeadResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendSMSLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public ImportLeadResponse ImportLead(ImportLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Import Lead");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ImportLead(parameter);
                var response = new ImportLeadResponse
                {
                    lstcontactContactDuplicate = new List<ContactModel>(),
                    lstcontactLeadDuplicate = new List<LeadModel>(),
                    lstcontactCustomerDuplicate = new List<ContactModel>(),
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
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
                if (result.lstcontactLeadDuplicate != null)
                {
                    if (result.lstcontactLeadDuplicate.Count > 0)
                    {
                        result.lstcontactLeadDuplicate.ForEach(item =>
                        {
                            response.lstcontactLeadDuplicate.Add(new LeadModel(item));
                        });
                    }
                }
                if (result.lstcontactCustomerDuplicate != null)
                {
                    if (result.lstcontactCustomerDuplicate.Count > 0)
                    {
                        result.lstcontactCustomerDuplicate.ForEach(item =>
                        {
                            response.lstcontactCustomerDuplicate.Add(new ContactModel(item));
                        });
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new ImportLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateLeadDuplicateResponse UpdateLeadDuplicate(UpdateLeadDuplicateRequest request)
        {
            try
            {
                this.logger.LogInformation("UpdateLeadDuplicate");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.UpdateLeadDuplicate(parameter);
                var response = new UpdateLeadDuplicateResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new UpdateLeadDuplicateResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public DownloadTemplateLeadResponse DownloadTemplateLead(DownloadTemplateLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("DownloadTemplateLead");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.DownloadTemplateLead(parameter);
                var response = new DownloadTemplateLeadResponse
                {
                    ExcelFile = result.ExcelFile,
                    NameFile = result.NameFile,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new DownloadTemplateLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeLeadStatusToDeleteResponse ChangeLeadStatusToDelete(ChangeLeadStatusToDeleteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ChangeLeadStatusToDelete(parameter);

                var response = new ChangeLeadStatusToDeleteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeLeadStatusToDeleteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Lead.DELETE_FAIL
                };
            }
        }

        public CheckPhoneLeadResponse CheckPhoneLead(CheckPhoneLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("CheckPhoneLead");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.CheckPhoneLead(parameter);
                var response = new CheckPhoneLeadResponse
                {
                    CheckPhone = result.CheckPhone,
                    ObjectType = result.ObjectType,
                    ContactId = result.ContactId,
                    ObjectId = result.ObjectId,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CheckPhoneLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Lead.GET_FAIL
                };
            }
        }

        public CheckEmailLeadResponse CheckEmailLead(CheckEmailLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("CheckEmailLead");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.CheckEmailLead(parameter);
                var response = new CheckEmailLeadResponse
                {
                    CheckEmail = result.CheckEmail,
                    ObjectType = result.ObjectType,
                    ContactId = result.ContactId,
                    ObjectId = result.ObjectId,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CheckEmailLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Lead.GET_FAIL
                };
            }
        }

        public GetPersonInChargeResponse GetPersonInCharge(GetPersonInChargeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Person In Charge");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetPersonInCharge(parameter);
                var response = new GetPersonInChargeResponse
                {
                    ListPersonInCharge = result.ListPersonInCharge,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetPersonInChargeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public EditPersonInChargeResponse EditPersonInCharge(EditPersonInChargeRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Person In Charge");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.EditPersonInCharge(parameter);
                var response = new EditPersonInChargeResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new EditPersonInChargeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public EditLeadStatusByIdResponse EditLeadStatusById(EditLeadStatusByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Lead Status By Id");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.EditLeadStatusById(parameter);
                var response = new EditLeadStatusByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    LeadId = result.LeadId
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new EditLeadStatusByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListCustomerByTypeResponse GetListCustomerByType(GetListCustomerByTypeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetListCustomerByType(parameter);
                var response = new GetListCustomerByTypeResponse
                {
                    ListCustomerByType = result.ListCustomerByType,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message

                };

                return response;
            }
            catch (Exception e)
            {

                return new GetListCustomerByTypeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCreateLeadResponse GetDataCreateLead(GetDataCreateLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetDataCreateLead(parameter);
                var response = new GetDataCreateLeadResponse
                {
                    ListEmailLead = result.ListEmailLead,
                    ListPhoneLead = result.ListPhoneLead,
                    ListCustomerContact = result.ListCustomerContact,
                    ListInterestedGroup = new List<CategoryModel>(),
                    ListGender = new List<CategoryModel>(),
                    ListPotential = new List<CategoryModel>(),
                    ListLeadType = new List<CategoryModel>(),
                    ListLeadGroup = new List<CategoryModel>(),
                    ListPersonalInChange = result.ListPersonalInChange,
                    ListProvince = result.ListProvince,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    //NEW
                    ListBusinessType = result.ListBusinessType,
                    ListInvestFund = result.ListInvestFund,
                    ListLeadReferenceCustomer = result.ListLeadReferenceCustomer,
                    ListProbability = result.ListProbability,
                    ListCareState = result.ListCareState,
                    ListArea = result.ListArea,
                    MessageCode = result.Message
                };

                result.ListInterestedGroup.ForEach(e => response.ListInterestedGroup.Add(new CategoryModel(e)));
                result.ListGender.ForEach(e => response.ListGender.Add(new CategoryModel(e)));
                result.ListPotential.ForEach(e => response.ListPotential.Add(new CategoryModel(e)));
                result.ListLeadType.ForEach(e => response.ListLeadType.Add(new CategoryModel(e)));
                result.ListLeadGroup.ForEach(e => response.ListLeadGroup.Add(new CategoryModel(e)));

                return response;
            }
            catch (Exception e)
            {

                return new GetDataCreateLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataEditLeadResponse GetDataEditLead(GetDataEditLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetDataEditLead(parameter);
                var response = new GetDataEditLeadResponse
                {
                    LeadModel = new LeadModel(result.LeadModel),
                    LeadContactModel = new ContactModel(result.LeadContactModel),
                    ListEmailLead = result.ListEmailLead,
                    ListPhoneLead = result.ListPhoneLead,
                    ListCustomerContact = result.ListCustomerContact,
                    ListInterestedGroup = new List<CategoryModel>(),
                    ListGender = new List<CategoryModel>(),
                    ListPaymentMethod = new List<CategoryModel>(),
                    ListPotential = new List<CategoryModel>(),
                    ListLeadType = new List<CategoryModel>(),
                    ListLeadGroup = new List<CategoryModel>(),
                    ListLeadStatus = new List<CategoryModel>(),
                    ListLeadInterestedGroupMappingId = result.ListLeadInterestedGroupMappingId,
                    ListNote = new List<NoteModel>(),
                    ListPersonalInChange = result.ListPersonalInChange,
                    //NEW
                    ListBusinessType = result.ListBusinessType,
                    ListInvestFund = result.ListInvestFund,
                    ListLeadReferenceCustomer = result.ListLeadReferenceCustomer,
                    ListProbability = result.ListProbability,
                    ListLeadDetail = result.ListLeadDetail,
                    ListLeadContact = new List<ContactModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    CanDelete = result.CanDelete,
                    CanCreateSaleBidding = result.CanCreateSaleBidding,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    StatusSaleBiddingAndQuote = result.StatusSaleBiddingAndQuote,
                    ListQuoteById = result.ListQuoteById,
                    ListOrder = result.ListOrder,
                    ListSaleBiddingById = result.ListSaleBiddingById,
                    ListLinkOfDocument = result.ListLinkOfDocument,
                    ListFile = result.ListFile,
                    ListProvince = result.ListProvince,
                    ListArea = result.ListArea,
                    ListStatusSupport = result.ListStatusSupport,
                    StatusSupportId = result.StatusSupportId
                };

                result.ListLeadContact.ForEach(e => response.ListLeadContact.Add(new ContactModel(e)));
                result.ListInterestedGroup.ForEach(e => response.ListInterestedGroup.Add(new CategoryModel(e)));
                result.ListGender.ForEach(e => response.ListGender.Add(new CategoryModel(e)));
                result.ListPaymentMethod.ForEach(e => response.ListPaymentMethod.Add(new CategoryModel(e)));
                result.ListPotential.ForEach(e => response.ListPotential.Add(new CategoryModel(e)));
                result.ListLeadType.ForEach(e => response.ListLeadType.Add(new CategoryModel(e)));
                result.ListLeadGroup.ForEach(e => response.ListLeadGroup.Add(new CategoryModel(e)));
                result.ListLeadStatus.ForEach(e => response.ListLeadStatus.Add(new CategoryModel(e)));
                result.ListNote.ForEach(e => response.ListNote.Add(new NoteModel(e)));
                result.ListEmployee.ForEach(item => response.ListEmployee.Add(new EmployeeModel(item)));

                return response;
            }
            catch (Exception e)
            {

                return new GetDataEditLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataSearchLeadResponse GetDataSearchLead(GetDataSearchLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetDataSearchLead(parameter);
                var response = new GetDataSearchLeadResponse
                {
                    ListInterestedGroup = new List<CategoryModel>(),
                    ListLeadType = new List<CategoryModel>(),
                    ListPotential = new List<CategoryModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListPersonalInchange = new List<Models.Employee.EmployeeModel>(),
                    ListCusGroup = result.ListCusGroup,
                    ListArea = result.ListArea,
                    ListSource = result.ListSource,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListInterestedGroup.ForEach(e => response.ListInterestedGroup.Add(new CategoryModel(e)));
                result.ListLeadType.ForEach(e => response.ListLeadType.Add(new CategoryModel(e)));
                result.ListPotential.ForEach(e => response.ListPotential.Add(new CategoryModel(e)));
                result.ListStatus.ForEach(e => response.ListStatus.Add(new CategoryModel(e)));
                result.ListPersonalInchange.ForEach(e => response.ListPersonalInchange.Add(new Models.Employee.EmployeeModel(e)));
                return response;
            }
            catch (Exception e)
            {
                return new GetDataSearchLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public ApproveOrRejectLeadUnfollowResponse ApproveOrRejectLeadUnfollow(ApproveOrRejectLeadUnfollowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ApproveOrRejectLeadUnfollow(parameter);
                var response = new ApproveOrRejectLeadUnfollowResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {

                return new ApproveOrRejectLeadUnfollowResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public SetPersonalInChangeResponse SetPersonalInChange(SetPersonalInChangeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.SetPersonalInChange(parameter);
                var response = new SetPersonalInChangeResponse
                {
                    ListPersonalInChange = new List<EmployeeModel>(),
                    StatusCode = result.Status
                        ? System.Net.HttpStatusCode.OK
                        : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };
                result.ListPersonalInChange.ForEach(e => response.ListPersonalInChange.Add(new EmployeeModel(e)));
                return response;
            }
            catch (Exception e)
            {

                return new SetPersonalInChangeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public UnfollowListLeadResponse UnfollowListLead(UnfollowListLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.UnfollowListLead(parameter);
                var response = new UnfollowListLeadResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new UnfollowListLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public ImportLeadDetailResponse ImportLeadDetail(ImportLeadDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ImportLeadDetail(parameter);
                var response = new ImportLeadDetailResponse
                {
                    ListProvince = new List<Models.Admin.ProvinceModel>(),
                    ListDistrict = new List<Models.Admin.DistrictModel>(),
                    ListWard = new List<Models.Admin.WardModel>(),
                    ListGender = new List<CategoryModel>(),
                    ListInterestedGroup = new List<CategoryModel>(),
                    ListPotential = new List<CategoryModel>(),
                    ListPaymentMethod = new List<CategoryModel>(),
                    ListEmailLead = result.ListEmailLead,
                    ListEmailCustomer = result.ListEmailCustomer,
                    ListPhoneLead = result.ListPhoneLead,
                    ListPhoneCustomer = result.ListPhoneCustomer,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListProvince.ForEach(e => response.ListProvince.Add(new Models.Admin.ProvinceModel(e)));
                result.ListDistrict.ForEach(e => response.ListDistrict.Add(new Models.Admin.DistrictModel(e)));
                result.ListWard.ForEach(e => response.ListWard.Add(new Models.Admin.WardModel(e)));

                result.ListGender.ForEach(e => response.ListGender.Add(new CategoryModel(e)));
                result.ListInterestedGroup.ForEach(e => response.ListInterestedGroup.Add(new CategoryModel(e)));
                result.ListPotential.ForEach(e => response.ListPotential.Add(new CategoryModel(e)));
                result.ListPaymentMethod.ForEach(e => response.ListPaymentMethod.Add(new CategoryModel(e)));

                return response;
            }
            catch (Exception e)
            {
                return new ImportLeadDetailResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public ImportListLeadResponse ImportListLead(ImportListLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ImportListLead(parameter);
                var response = new ImportListLeadResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new ImportListLeadResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataLeadProductDialogResponse GetDataLeadProductDialog(GetDataLeadProductDialogRequest request)
        {
            try
            {
                this.logger.LogInformation("GetDataLeadProductDialog");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetDataLeadProductDialog(parameter);

                var response = new GetDataLeadProductDialogResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListUnitMoney = new List<CategoryModel>(),
                    ListUnitProduct = new List<CategoryModel>(),
                    ListVendor = new List<Models.Vendor.VendorModel>(),
                    ListProduct = new List<Models.Product.ProductModel>(),
                    ListPriceProduct = new List<Models.Product.PriceProductModel>()
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
                    response.ListVendor.Add(new Models.Vendor.VendorModel(vendor));
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new Models.Product.ProductModel(item));
                });

                result.ListPriceProduct.ForEach(item =>
                {
                    response.ListPriceProduct.Add(new Models.Product.PriceProductModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataLeadProductDialogResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SendEmailSupportLeadResponse SendEmailSupportLead(SendEmailSupportLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.SendEmailSupportLead(parameter);
                var response = new SendEmailSupportLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,                    //QueueId = result.QueueId,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new SendEmailSupportLeadResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendSMSSupportLeadResponse SendSMSSupportLead(SendSMSSupportLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.SendSMSSupportLead(parameter);
                var response = new SendSMSSupportLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    //QueueId = result.QueueId,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new SendSMSSupportLeadResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendGiftSupportLeadResponse SendGiftSupportLead(SendGiftSupportLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.SendGiftSupportLead(parameter);
                var response = new SendGiftSupportLeadResponse()
                {
                    LeadCareId = result.LeadCareId,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new SendGiftSupportLeadResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public CreateLeadMeetingResponse CreateLeadMeeting(CreateLeadMeetingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.CreateLeadMeeting(parameter);
                var response = new CreateLeadMeetingResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new CreateLeadMeetingResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeLeadStatusResponse ChangeLeadStatus(ChangeLeadStatusRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ChangeLeadStatus(parameter);
                var response = new ChangeLeadStatusResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ChangeLeadStatusResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetHistoryLeadCareResponse GetHistoryLeadCare(GetHistoryLeadCareRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetHistoryLeadCare(parameter);
                var response = new GetHistoryLeadCareResponse()
                {
                    ListCustomerCareInfor = result.ListCustomerCareInfor,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetHistoryLeadCareResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }

        }

        public GetDataPreviewLeadCareResponse GetDataPreviewLeadCare(GetDataPreviewLeadCareRequest request)
        {
            try
            {
                logger.LogInformation("Get Data Preview Lead Care");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetDataPreviewLeadCare(parameter);
                var response = new GetDataPreviewLeadCareResponse()
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
                return new GetDataPreviewLeadCareResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataLeadCareFeedBackResponse GetDataLeadCareFeedBack(GetDataLeadCareFeedBackRequest request)
        {
            try
            {
                logger.LogInformation("Get Data LeadCareFeedBack");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetDataLeadCareFeedBack(parameter);
                var response = new GetDataLeadCareFeedBackResponse()
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
                return new GetDataLeadCareFeedBackResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SaveLeadCareFeedBackResponse SaveLeadCareFeedBack(SaveLeadCareFeedBackRequest request)
        {
            try
            {
                logger.LogInformation("Save LeadCareFeedBack");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.SaveLeadCareFeedBack(parameter);
                var response = new SaveLeadCareFeedBackResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveLeadCareFeedBackResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetHistoryLeadMeetingResponse GetHistoryLeadMeeting(GetHistoryLeadMeetingRequest request)
        {
            try
            {
                logger.LogInformation("Get History Lead Meeting");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetHistoryLeadMeeting(parameter);
                var response = new GetHistoryLeadMeetingResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    LeadMeetingInfor = new LeadMeetingInforBusinessModel()
                };

                response.LeadMeetingInfor.EmployeeId = result.LeadMeetingInfor.EmployeeId;
                response.LeadMeetingInfor.EmployeeName = result.LeadMeetingInfor.EmployeeName;
                response.LeadMeetingInfor.EmployeePosition = result.LeadMeetingInfor.EmployeePosition;

                response.LeadMeetingInfor.Week1 = new List<LeadMeetingForWeekBusinessModel>();
                response.LeadMeetingInfor.Week2 = new List<LeadMeetingForWeekBusinessModel>();
                response.LeadMeetingInfor.Week3 = new List<LeadMeetingForWeekBusinessModel>();
                response.LeadMeetingInfor.Week4 = new List<LeadMeetingForWeekBusinessModel>();
                response.LeadMeetingInfor.Week5 = new List<LeadMeetingForWeekBusinessModel>();

                result.LeadMeetingInfor.Week1.ForEach(item =>
                {
                    var temp = new LeadMeetingForWeekBusinessModel();
                    temp.LeadMeetingId = item.LeadMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.LeadMeetingInfor.Week1.Add(temp);
                });

                result.LeadMeetingInfor.Week2.ForEach(item =>
                {
                    var temp = new LeadMeetingForWeekBusinessModel();
                    temp.LeadMeetingId = item.LeadMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.LeadMeetingInfor.Week2.Add(temp);
                });

                result.LeadMeetingInfor.Week3.ForEach(item =>
                {
                    var temp = new LeadMeetingForWeekBusinessModel();
                    temp.LeadMeetingId = item.LeadMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.LeadMeetingInfor.Week3.Add(temp);
                });

                result.LeadMeetingInfor.Week4.ForEach(item =>
                {
                    var temp = new LeadMeetingForWeekBusinessModel();
                    temp.LeadMeetingId = item.LeadMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.LeadMeetingInfor.Week4.Add(temp);
                });

                result.LeadMeetingInfor.Week5.ForEach(item =>
                {
                    var temp = new LeadMeetingForWeekBusinessModel();
                    temp.LeadMeetingId = item.LeadMeetingId;
                    temp.EmployeeId = item.EmployeeId;
                    temp.Title = item.Title;
                    temp.Subtitle = item.Subtitle;
                    temp.StartDate = item.StartDate;
                    temp.StartHours = item.StartHours;
                    temp.Background = item.Background;
                    response.LeadMeetingInfor.Week5.Add(temp);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetHistoryLeadMeetingResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataLeadMeetingByIdResponse getDataLeadMeetingById(GetDataLeadMeetingByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Data Lead Meeting By Id");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetDataLeadMeetingById(parameter);
                var response = new GetDataLeadMeetingByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    LeadMeeting = new LeadMeetingModel()
                };

                response.LeadMeeting.LeadMeetingId = result.LeadMeeting.LeadMeetingId;
                response.LeadMeeting.LeadId = result.LeadMeeting.LeadId;
                response.LeadMeeting.EmployeeId = result.LeadMeeting.EmployeeId.Value;
                response.LeadMeeting.Title = result.LeadMeeting.Title;
                response.LeadMeeting.LocationMeeting = result.LeadMeeting.LocationMeeting;
                response.LeadMeeting.StartDate = result.LeadMeeting.StartDate;
                response.LeadMeeting.EndDate = result.LeadMeeting.EndDate;
                response.LeadMeeting.Content = result.LeadMeeting.Content;
                response.LeadMeeting.Participant = result.LeadMeeting.Participant;

                return response;
            }
            catch (Exception e)
            {
                return new GetDataLeadMeetingByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CloneLeadResponse CloneLead(CloneLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Reply Lead by parameter");
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.CloneLead(parameter);
                var response = new CloneLeadResponse();

                if (result.Status)
                {
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.LeadId = result.LeadId;
                    response.ContactId = result.ContactId;
                    response.PicName = result.PicName;
                    response.Potential = result.Potential;
                    response.StatusName = result.StatusName;
                    response.SendEmailEntityModel = result.SendEmailEntityModel;
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.MessageCode = "Nhân bản thất bại";
                }
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CloneLeadResponse()
                {
                    MessageCode = "Nhân bản thất bại",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeSellerResponse GetEmployeeSeller(GetEmployeeSellerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetEmployeeSeller(parameter);

                var response = new GetEmployeeSellerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ListEmployee = new List<EmployeeModel>(),
                };

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetEmployeeSellerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetVendorByProductIdResponse GetVendorByProductId(GetVendorByProductIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetVendorByProductId(parameter);

                var response = new GetVendorByProductIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListVendor = new List<VendorModel>(),
                    ListObjectAttributeNameProduct = new List<ObjectAttributeNameProductModel>(),
                    ListObjectAttributeValueProduct = new List<ObjectAttributeValueProductModel>(),
                    PriceProduct = result.PriceProduct
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
                return new GetVendorByProductIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetEmployeeByPersonInChargeResponse GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetEmployeeByPersonInCharge(parameter);

                var response = new GetEmployeeByPersonInChargeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListEmployee = result.ListEmployee
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetEmployeeByPersonInChargeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ReportLeadResponse ReportLead(ReportLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ReportLead(parameter);

                var response = new ReportLeadResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListReportLeadFollowAge = result.ListReportLeadFollowAge,
                    ListReportLeadFollowMonth = result.ListReportLeadFollowMonth,
                    ListReportLeadFollowPic = result.ListReportLeadFollowPic,
                    ListReportLeadFollowSource = result.ListReportLeadFollowSource,
                    ListReportLeadFollowProvincial = result.ListReportLeadFollowProvincial
                };

                return response;
            }
            catch (Exception ex)
            {
                return new ReportLeadResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public GetMasterDataReportLeadReponse GetMasterDataReportLead(GetMasterDataReportLeadRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.GetMasterDataReportLead(parameter);

                var response = new GetMasterDataReportLeadReponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListEmployee = result.ListEmployee,
                    ListArea = result.ListArea,
                    ListSource = result.ListSource
                };

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataReportLeadReponse
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    MessageCode = ex.Message,
                };
            }
        }

        public ChangeLeadStatusSupportResponse ChangeLeadStatusSupport(ChangeLeadStatusSupportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iLeadDataAccess.ChangeLeadStatusSupport(parameter);

                var response = new ChangeLeadStatusSupportResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeLeadStatusSupportResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    MessageCode = e.Message,
                };
            }
        }
    }
}
