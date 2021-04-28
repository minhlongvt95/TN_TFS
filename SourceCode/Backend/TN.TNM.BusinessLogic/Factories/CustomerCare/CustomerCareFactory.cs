using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.CustomerCare;
using TN.TNM.BusinessLogic.Messages.Requests.CustomerCare;
using TN.TNM.BusinessLogic.Messages.Responses.CustomerCare;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.CustomerCare
{
    public class CustomerCareFactory : BaseFactory, ICustomerCare
    {
        private ICustomerCareDataAccess iCustomerCareDataAccess;

        public CustomerCareFactory(ICustomerCareDataAccess _iCustomerCareDataAccess, ILogger<CustomerCareFactory> _logger)
        {
            iCustomerCareDataAccess = _iCustomerCareDataAccess;
            logger = _logger;
        }
        public CreateCustomerCareResponse CreateCustomerCare(CreateCustomerCareRequest request)
        {
            try
            {
                logger.LogInformation("Create Customer Care");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.CreateCustomerCare(parameter);
                var response = new CreateCustomerCareResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCareCustomer = result.CustomerCareCustomer,
                    CustomerCareId = result.CustomerCareId,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new CreateCustomerCareResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }

        }

        public CreateCustomerCareFeedBackResponse CreateCustomerCareFeedBack(CreateCustomerCareFeedBackRequest request)
        {
            try
            {
                logger.LogInformation("Create CustomerCare FeedBack");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.CreateCustomerCareFeedBack(parameter);
                var response = new CreateCustomerCareFeedBackResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCareFeedBackId = result.CustomerCareFeedBackId,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new CreateCustomerCareFeedBackResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public FilterCustomerResponse FilterCustomer(FilterCustomerRequest request)
        {
            try
            {
                logger.LogInformation("FilterCustomer");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.FilterCustomer(parameter);
                var response = new FilterCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCustomer = new List<FilterCustomerModel>(),
                    MessageCode = result.Message
                };
                if (result.ListCustomer.Count > 0)
                {
                    result.ListCustomer.ForEach(item =>
                    {
                        response.ListCustomer.Add(new FilterCustomerModel(item));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new FilterCustomerResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerBirthDayResponse GetCustomerBirthDay(GetCustomerBirthDayRequest request)
        {
            try
            {
                logger.LogInformation("GetCustomerBirthDay");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetCustomerBirthDay(parameter);
                var response = new GetCustomerBirthDayResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListBirthDay = new List<GetCustomerBirthDayModel>(),
                    MessageCode = result.Message
                };
                if (result.ListBirthDay.Count > 0)
                {
                    result.ListBirthDay.ForEach(item =>
                    {
                        var birthday = new GetCustomerBirthDayModel()
                        {
                            ContactId = item.ContactId,
                            ObjectId = item.ObjectId,
                            CustomerName = item.CustomerName,
                            Phone = item.Phone,
                            Email = item.Email,
                            BirthDay = item.BirthDay,
                            EmployeeID = item.EmployeeID,
                            EmployeeName = item.EmployeeName,
                            AvataUrl = item.AvataUrl
                        };
                        response.ListBirthDay.Add(birthday);
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetCustomerBirthDayResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCharCustomerCSResponse GetCharCustomerCS(GetCharCustomerCSRequest request)
        {
            try
            {
                logger.LogInformation("GetCharCustomerCS");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetCharCustomerCS(parameter);
                var response = new GetCharCustomerCSResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListChar = new List<GetCharCustomerCSModel>(),
                    MessageCode = result.Message
                };
                if (result.ListChar.Count > 0)
                {
                    result.ListChar.ForEach(item =>
                    {
                        var birthday = new GetCharCustomerCSModel()
                        {
                            Month = item.Month,
                            TotalCustomerProgram = item.TotalCustomerProgram,
                            TotalCustomerCSKH = item.TotalCustomerCSKH
                        };
                        response.ListChar.Add(birthday);
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetCharCustomerCSResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerCareByIdResponse GetCustomerCareById(GetCustomerCareByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Customer CareById");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetCustomerCareById(parameter);
                var response = new GetCustomerCareByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCare = new CustomerCareModel(result.CustomerCare),
                    CustomerCareFeedBack = new List<CustomerCareFeedBackModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    QueryFilter = result.QueryFilter,
                    TypeCustomer = result.TypeCutomer,
                    MessageCode = result.Message
                };
                result.CustomerCareFeedBack.ForEach(item =>
                {
                    response.CustomerCareFeedBack.Add(new CustomerCareFeedBackModel(item));
                });
                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });
                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new GetCustomerCareByIdResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerCareFeedBackByCusIdAndCusCareIdResponse GetCustomerCareFeedBackByCusIdAndCusCareId(GetCustomerCareFeedBackByCusIdAndCusCareIdRequest request)
        {
            try
            {
                logger.LogInformation("Get CustomerCareFeedBack By Id");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetCustomerCareFeedBackByCusIdAndCusCareId(parameter);
                var response = new GetCustomerCareFeedBackByCusIdAndCusCareIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCareFeedBack = result.CustomerCareFeedBack,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new GetCustomerCareFeedBackByCusIdAndCusCareIdResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerNewCSResponse GetCustomerNewCS(GetCustomerNewCSRequest request)
        {
            try
            {
                logger.LogInformation("GetCustomerNewCS");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetCustomerNewCS(parameter);
                var response = new GetCustomerNewCSResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCustomerNewOrder = new List<GetCustomerNewCSModel>(),
                    MessageCode = result.Message
                };

                if (result.ListCustomerNewOrder.Count > 0)
                {
                    result.ListCustomerNewOrder.ForEach(item =>
                    {
                        var total = new GetCustomerNewCSModel()
                        {
                            ContactId = item.ContactId,
                            ObjectId = item.ObjectId,
                            CustomerName = item.CustomerName,
                            Phone = item.Phone,
                            Email = item.Email,
                            BirthDay = item.BirthDay,
                            EmployeeID = item.EmployeeID,
                            EmployeeName = item.EmployeeName,
                            AvataUrl = item.AvataUrl
                        };
                        response.ListCustomerNewOrder.Add(total);
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetCustomerNewCSResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetTimeLineCustomerCareByCustomerIdResponse GetTimeLineCustomerCareByCustomerId(GetTimeLineCustomerCareByCustomerIdRequest request)
        {
            try
            {
                logger.LogInformation("Get TimeLine Customer Care By CustomerId");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetTimeLineCustomerCareByCustomerId(parameter);
                var response = new GetTimeLineCustomerCareByCustomerIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCustomerCare = result.ListCustomerCare,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetTimeLineCustomerCareByCustomerIdResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetTotalInteractiveResponse GetTotalInteractive(GetTotalInteractiveRequest request)
        {
            try
            {
                logger.LogInformation("GetTotalInteractive");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetTotalInteractive(parameter);
                var response = new GetTotalInteractiveResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCate = new List<GetTotalInteractiveModel>(),
                    TotalCare = result.TotalCare,
                    MessageCode = result.Message
                };

                if (result.ListCate.Count > 0)
                {
                    result.ListCate.ForEach(item =>
                    {
                        var total = new GetTotalInteractiveModel()
                        {
                            CategoryName = item.CategoryName,
                            Total = item.Total
                        };
                        response.ListCate.Add(total);
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetTotalInteractiveResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerCareActiveResponse GetCustomerCareActive(GetCustomerCareActiveRequest request)
        {
            try
            {
                logger.LogInformation("GetCustomerCareActive");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetCustomerCareActive(parameter);
                var response = new GetCustomerCareActiveResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCategoryCare = new List<GetCustomerCareActiveModel>(),
                    MessageCode = result.Message
                };

                if (result.ListCategoryCare.Count > 0)
                {
                    result.ListCategoryCare.ForEach(item =>
                    {
                        var total = new GetCustomerCareActiveModel()
                        {
                            CustomerCareId = item.CustomerCareId,
                            CustomerCareTitle = item.CustomerCareTitle,
                            CustomerTotal = item.CustomerTotal,
                            Status = item.Status,
                            CategoryCare = item.CategoryCare,
                            DateCreate = item.DateCreate
                        };
                        response.ListCategoryCare.Add(total);
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetCustomerCareActiveResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchCustomerCareResponse SearchCustomerCare(SearchCustomerCareRequest request)
        {
            try
            {
                logger.LogInformation("Search Customer Care");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.SearchCustomerCare(parameter);
                var response = new SearchCustomerCareResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    LstCustomerCare = new List<CustomerCareModel>(),
                    MessageCode = result.Message
                };
                if (result.LstCustomerCare.Count > 0)
                {
                    result.LstCustomerCare.ForEach(item =>
                    {
                        response.LstCustomerCare.Add(new CustomerCareModel(item));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchCustomerCareResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendQuickEmailResponse SendQuickEmail(SendQuickEmailRequest request)
        {
            try
            {
                logger.LogInformation("Send Quick Email");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.SendQuickEmail(parameter);
                var response = new SendQuickEmailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QueueId = result.QueueId,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new SendQuickEmailResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendQuickGiftResponse SendQuickGift(SendQuickGiftRequest request)
        {
            try
            {
                logger.LogInformation("Send Quick Gift");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.SendQuickGift(parameter);
                var response = new SendQuickGiftResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCareId = result.CustomerCareId,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new SendQuickGiftResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendQuickSMSResponse SendQuickSMS(SendQuickSMSRequest request)
        {
            try
            {
                logger.LogInformation("Send Quick SMS");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.SendQuickSMS(parameter);
                var response = new SendQuickSMSResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QueueId = result.QueueId,
                    MessageCode = result.Message
                };

                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new SendQuickSMSResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateCustomerCareResponse UpdateCustomerCare(UpdateCustomerCareRequest request)
        {
            try
            {
                logger.LogInformation("Update Customer Care");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.UpdateCustomerCare(parameter);
                var response = new UpdateCustomerCareResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    CustomerCareCustomer = result.CustomerCareCustomer,
                    CustomerCareId = result.CustomerCareId,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new UpdateCustomerCareResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateCustomerCareFeedBackResponse UpdateCustomerCareFeedBack(UpdateCustomerCareFeedBackRequest request)
        {
            try
            {
                logger.LogInformation("UpdateCustomerCareFeedBack");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.UpdateCustomerCareFeedBack(parameter);
                var response = new UpdateCustomerCareFeedBackResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCareFeedBackId = result.CustomerCareFeedBackId,
                    MessageCode = result.Message
                };
                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new UpdateCustomerCareFeedBackResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateStatusCustomerCareResponse UpdateStatusCustomerCare(UpdateStatusCustomerCareRequest request)
        {
            try
            {
                logger.LogInformation("Update Status Customer Care");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.UpdateStatusCustomerCare(parameter);
                var response = new UpdateStatusCustomerCareResponse()
                {
                    StatusCode = result.Status == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Found,
                    MessageCode = result.Message
                };
                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new UpdateStatusCustomerCareResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateStatusCustomerCareCustomerByIdResponse UpdateStatusCustomerCareCustomerById(UpdateStatusCustomerCareCustomerByIdRequest request)
        {
            try
            {
                logger.LogInformation("Update Status Customer Care Customer");
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.UpdateStatusCustomerCareCustomerById(parameter);
                var response = new UpdateStatusCustomerCareCustomerByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCareCustomerId = result.CustomerCareCustomerId,
                    MessageCode = result.Message
                };
                return response;

            }
            catch (Exception e)
            {

                logger.LogError(e.Message);
                return new UpdateStatusCustomerCareCustomerByIdResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataCustomerCareListResponse GetMasterDataCustomerCareList(GetMasterDataCustomerCareListRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.GetMasterDataCustomerCareList(parameter);
                var resonse = new GetMasterDataCustomerCareListResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmployee = result.ListEmployee,
                    ListStatus = result.ListStatus,
                    ListFormCusCare = result.ListFormCusCare
                };

                return resonse;
            }
            catch (Exception ex)
            {
                return new GetMasterDataCustomerCareListResponse
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    MessageCode = ex.Message
                };
            }
        }

        public UpdateStatusCusCareResponse UpdateStatusCusCare(UpdateStatusCusCareRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerCareDataAccess.UpdateStatusCusCare(parameter);

                var respone = new UpdateStatusCusCareResponse
                {
                    MessageCode = result.Message,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden
                };

                return respone;
            }
            catch (Exception ex)
            {
                return new UpdateStatusCusCareResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public UpdateCustomerMeetingResponse UpdateCustomerMeeting(UpdateCustomerMeetingRequest request)
        {
            try
            {
                var paramter = request.ToParameter();
                var result = iCustomerCareDataAccess.UpdateCustomerMeeting(paramter);

                var respone = new UpdateCustomerMeetingResponse
                {
                    MessageCode = result.Message,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden
                };

                return respone;
            }
            catch (Exception ex)
            {
                return new UpdateCustomerMeetingResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public RemoveCustomerMeetingResponse RemoveCustomerMeeting(RemoveCustomerMeetingRequest request)
        {
            try
            {
                var paramter = request.ToParameter();
                var result = iCustomerCareDataAccess.RemoveCustomerMeeting(paramter);

                var respone = new RemoveCustomerMeetingResponse
                {
                    MessageCode = result.Message,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden
                };

                return respone;
            }
            catch (Exception ex)
            {
                return new RemoveCustomerMeetingResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }
    }
}
