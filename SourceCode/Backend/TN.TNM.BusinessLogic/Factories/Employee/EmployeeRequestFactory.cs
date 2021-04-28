using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Employee;
using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.Common;
using TN.TNM.Common.CommonObject;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.BusinessLogic.Factories.Employee
{
    public class EmployeeRequestFactory : BaseFactory, IEmployeeRequest
    {
        private IEmployeeRequestDataAccess iEmployeeRequestDataAccess;
        public EmployeeRequestFactory(IEmployeeRequestDataAccess _iEmployeeRequestDataAccess, ILogger<EmployeeRequestFactory> _logger)
        {
            iEmployeeRequestDataAccess = _iEmployeeRequestDataAccess;
            logger = _logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateEmployeeRequestResponse CreateEmployeeRequest(CreateEmployeeRequestRequest request)
        {
            try
            {
                logger.LogInformation("Create Employee Request");
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.CreateEmployeeRequest(parameter);
                var response = new CreateEmployeeRequestResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? result.Message : CommonMessage.EmployeeRequest.CREATE_FAIL,
                    EmployeeRequestId = result.EmployeeRequestId,
                    EmployeeRequestCode = result.EmployeeRequestCode,
                    SendEmaiModel = result.SendEmaiModel            
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateEmployeeRequestResponse()
                {
                    MessageCode = CommonMessage.EmployeeRequest.CREATE_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EditEmployeeRequestByIdResponse EditEmployeeRequestById(EditEmployeeRequestByIdRequest request)
        {
            try
            {
                logger.LogInformation("Edit Employee Request by Id");
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.EditEmployeeRequestById(parameter);
                var response = new EditEmployeeRequestByIdResponse();
                if (!result.Status)
                {
                    response.StatusCode = HttpStatusCode.ExpectationFailed;
                    response.MessageCode = CommonMessage.Employee.EDIT_FAIL;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.MessageCode = result.Message;
                }

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditEmployeeRequestByIdResponse
                {
                    MessageCode = CommonMessage.Employee.EDIT_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllEmployeeRequestResponse GetAllEmployeeRequest(GetAllEmployeeRequestRequest request)
        {
            try
            {
                logger.LogInformation("Get All Employee Request");
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.GetAllEmployeeRequest(parameter);
                var response = new GetAllEmployeeRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    EmployeeRequestList = new List<EmployeeRequestModel>(),
                    OrganizationList = new List<OrganizationDetail>()
                };
                result.EmployeeRequestList.ForEach(employeeRequestEntity =>
                {
                    response.EmployeeRequestList.Add(new EmployeeRequestModel(employeeRequestEntity));
                });

                result.OrganizationList.ForEach(organizationDetail =>
                {
                    response.OrganizationList.Add(organizationDetail);
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllEmployeeRequestResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetEmployeeRequestByIdResponse GetEmployeeRequestById(GetEmployeeRequestByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Employee Request by Id");
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.GetEmployeeRequestById(parameter);

                var response = new GetEmployeeRequestByIdResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    EmployeeRequest = new EmployeeRequestModel(result.EmployeeRequest),
                    ApproverId = result.ApproverId,
                    IsApproved = result.IsApproved,
                    IsInApprovalProgress = result.IsInApprovalProgress,
                    IsRejected = result.IsRejected,
                    MessageCode = result.Message,
                    PositionId = result.PositionId,
                    StatusName = result.StatusName,
                    Notes= new List<NoteObject>(),
                };
                response.Notes = result.Notes;
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeRequestByIdResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchEmployeeRequestResponse SearchEmployeeRequest(SearchEmployeeRequestRequest request)
        {
            try
            {
                logger.LogInformation("Search EmployeeRequest");
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.SearchEmployeeRequest(parameter);
                var response = new SearchEmployeeRequestResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    EmployeeRequestList = new List<EmployeeRequestModel>(),
                    AmountAbsentWithoutPermission = result.AmountAbsentWithoutPermission,
                    AmountAbsentWithPermission = result.AmountAbsentWithPermission
                };
                result.EmployeeRequestList.ForEach(employeeRequestEntity =>
                {
                    response.EmployeeRequestList.Add(new EmployeeRequestModel(employeeRequestEntity));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchEmployeeRequestResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
        public GetEmployeeRequestByEmpIdResponse GetEmployeeRequestByEmpId(GetEmployeeRequestByEmpIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Employee Request by Emp Id");
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.GetEmployeeRequestByEmpId(parameter);
                var response = new GetEmployeeRequestByEmpIdResponse()
                {
                    MessageCode = result.Message,
                    StatusCode = HttpStatusCode.OK,
                    ListEmployeeRequest = new List<EmployeeRequestModel>(),
                    amountAbsentWithoutPermission = result.amountAbsentWithoutPermission,
                    amountAbsentWithPermission = result.amountAbsentWithPermission
                };
                result.ListEmployeeRequest.ForEach(empR =>
                {
                    response.ListEmployeeRequest.Add(new EmployeeRequestModel(empR));
                });
                
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetEmployeeRequestByEmpIdResponse
                {
                    MessageCode = CommonMessage.Employee.EDIT_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
        public CheckEmployeeCreateRequestResponse CheckEmployeeCreateRequest(CheckEmployeeCreateRequest request)
        {
            try
            {
                logger.LogInformation("CheckEmployeeCreateRequest");
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.CheckEmployeeCreateRequest(parameter);
                return new CheckEmployeeCreateRequestResponse()
                {
                    MessageCode = result.Message,
                    StatusCode = HttpStatusCode.OK,
                    IsEmpCreateRequest = result.IsEmpCreateRequest
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CheckEmployeeCreateRequestResponse
                {
                    MessageCode = CommonMessage.Employee.GET_FAIL,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetDataSearchEmployeeRequestResponse GetDataSearchEmployeeRequest(GetDataSearchEmployeeRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iEmployeeRequestDataAccess.GetDataSearchEmployeeRequest(parameter);

                var response = new GetDataSearchEmployeeRequestResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListStatus = new List<Models.Category.CategoryModel>(),
                    ListTypeRequest = new List<Models.Category.CategoryModel>(),
                };

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new Models.Category.CategoryModel(item));
                });

                result.ListTypeRequest.ForEach(item =>
                {
                    response.ListTypeRequest.Add(new Models.Category.CategoryModel(item));
                });

                return response;

            }catch(Exception ex)
            {
                return new GetDataSearchEmployeeRequestResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }
    }
}
