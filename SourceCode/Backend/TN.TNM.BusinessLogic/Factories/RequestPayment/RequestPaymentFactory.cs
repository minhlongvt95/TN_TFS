using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.RequestPayment;
using TN.TNM.BusinessLogic.Messages.Requests.RequestPayment;
using TN.TNM.BusinessLogic.Messages.Responses.RequestPayment;
using TN.TNM.BusinessLogic.Models.Document;
using TN.TNM.BusinessLogic.Models.RequestPayment;
using TN.TNM.Common;
using TN.TNM.Common.CommonObject;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.RequestPayment
{
    public class RequestPaymentFactory : BaseFactory, IRequestPayment
    {
        private IRequestPaymentDataAccess iRequestPaymentDataAccess;

        public RequestPaymentFactory(IRequestPaymentDataAccess _iRequestPaymentDataAccess, ILogger<RequestPaymentFactory> _logger)
        {
            this.iRequestPaymentDataAccess = _iRequestPaymentDataAccess;
            this.logger = _logger;
        }

        public CreateRequestPaymentResponse CreateRequestPayment(CreateRequestPaymentRequest request)
        {
            try
            {
                logger.LogInformation("Create Request Payment");
                var parameter = request.ToParameter();
                var result = iRequestPaymentDataAccess.CreateRequestPayment(parameter);
                return new CreateRequestPaymentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message,
                    RequestPaymentId = result.RequestPaymentId
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateRequestPaymentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.RequestPayment.ADD_FAIL
                };
            }
        }

        public EditRequestPaymentResponse EditRequestPayment(EditRequestPaymentRequest request)
        {
            try
            {
                logger.LogInformation("Create Request Payment");
                var parameter = request.ToParameter();
                var result = iRequestPaymentDataAccess.EditRequestPayment(parameter);
                return new EditRequestPaymentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message,
                    RequestPaymentId = result.RequestPaymentId
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditRequestPaymentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.RequestPayment.EDIT_FAIL
                };
            }
        }

        public FindRequestPaymentResponse FindRequestPayment(FindRequestPaymentRequest request)
        {
            try
            {
                logger.LogInformation("Find Request Payment");
                var parameter = request.ToParameter();
                var result = iRequestPaymentDataAccess.FindRequestPayment(parameter);
                return new FindRequestPaymentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message,
                    RequestList = result.RequestList?.Select(rq => new RequestPaymentModel(rq)).ToList()
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new FindRequestPaymentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.RequestPayment.GET_FAIL
                };
            }
        }

        public GetRequestPaymentByIdResponse GetRequestPaymentById(GetRequestPaymentByIdRequest request)
        {
            try
            {
                logger.LogInformation("Create Request Payment");
                var parameter = request.ToParameter();
                var result = iRequestPaymentDataAccess.GetRequestPaymentById(parameter);
                var response = new GetRequestPaymentByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message,
                    requestPaymentEntityModel = new RequestPaymentModel(result.requestPaymentEntityModel),
                    lstDocument = new List<DocumentModel>(),
                    lstDoc = new List<IFormFile>(),
                    IsApprove = result.IsApprove,
                    IsReject = result.IsReject,
                    IsSendingApprove = result.IsSendingApprove,
                    Notes= new List<NoteObject>(),
                };

                if (result.lstDocument.Count > 0)
                {
                    result.lstDocument.ForEach(item =>
                    {
                        response.lstDocument.Add(new DocumentModel(item));
                    });
                }
                response.Notes = result.Notes;
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetRequestPaymentByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.RequestPayment.EDIT_FAIL
                };
            }

        }
    }
}
