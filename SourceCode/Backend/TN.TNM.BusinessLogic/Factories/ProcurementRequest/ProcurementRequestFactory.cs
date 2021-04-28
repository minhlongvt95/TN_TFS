using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.ProcurementRequest;
using TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest;
using TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.ProcurementRequest;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.Common;
using TN.TNM.Common.CommonObject;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.ProcurementRequest
{
    public class ProcurementRequestFactory : BaseFactory, IProcurementRequest
    {
        private IProcurementRequestDataAccess iProcurementRequestDataAccess;

        public ProcurementRequestFactory(IProcurementRequestDataAccess _iProcurementRequestDataAccess, ILogger<ProcurementRequestFactory> _logger)
        {
            this.iProcurementRequestDataAccess = _iProcurementRequestDataAccess;
            this.logger = _logger;
        }

        /// <summary>
        /// Tạo hóa đơn đặt hàng
        /// </summary>
        /// <param name="ProcurementRequest">Thong tin hoa don dat hang</param>
        /// <param name="ProcurementRequestItemList">Danh sach san pham trong hoa don dat hang</param>
        /// <returns></returns>
        public CreateProcurementRequestResponse CreateProcurementRequest(CreateProcurementRequestRequest request)
        {
            try
            {
                logger.LogInformation("Create ProcurementRequest");
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.CreateProcurementRequest(parameter);
                var response = new CreateProcurementRequestResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.Accepted : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProcurementRequestId = result.ProcurementRequestId
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateProcurementRequestResponse() {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.ProcurementRequest.ADD_FAIL
                };
            }
        }
        public SearchProcurementRequestResponse SearchProcurementRequest(SearchProcurementRequestRequest request)
        {
            try
            {
                logger.LogInformation("search ProcurementRequest");
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.SearchProcurementRequest(parameter);
                var response = new SearchProcurementRequestResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
                response.ListProcurementRequest = new List<Models.ProcurementRequest.ProcurementRequestModel>();
                if (result.ListProcurementRequest != null)
                {
                    result.ListProcurementRequest.ForEach(pr =>
                    {
                        response.ListProcurementRequest.Add(new Models.ProcurementRequest.ProcurementRequestModel(pr));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchProcurementRequestResponse() { };
            }
        }

        public SearchVendorProductPriceResponse SearchVendorProductPrice(SearchVendorProductPriceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.SearchVendorProductPrice(parameter);
                var response = new SearchVendorProductPriceResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    VendorProductPrice = result.VendorProductPrice,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new SearchVendorProductPriceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetAllProcurementPlanResponse GetAllProcurementPlan(GetAllProcurementPlanRequest request)
        {
            try
            {
                logger.LogInformation("GetAllProcurementPlan");
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.GetAllProcurementPlan(parameter);
                var response = new GetAllProcurementPlanResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message,
                    PRPlanList = new List<ProcurementPlanModel>()
                };

                result.PRPlanList.ForEach(item => {
                    response.PRPlanList.Add(new ProcurementPlanModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllProcurementPlanResponse() { };
            }
        }
        public GetProcurementRequestByIdResponse GetProcurementRequestById(GetProcurementRequestByIdRequest request)
        {
            try
            {
                logger.LogInformation("GetProcurementRequestById");
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.GetProcurementRequestById(parameter);
                var response = new GetProcurementRequestByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message,
                    ProcurementRequest = result.ProcurementRequest == null ? new ProcurementRequestModel() : new ProcurementRequestModel(result.ProcurementRequest),
                    ListProcurementItem = new List<ProcurementRequestItemModel>(),
                    IsApprove = result.IsApprove,
                    IsReject = result.IsReject,
                    IsSendingApprove = result.IsSendingApprove,
                    ListDocument = new List<Models.Document.DocumentModel>(),
                    Notes= new List<NoteObject>(),
                };
                if (result.ListProcurementItem.Count > 0)
                {
                    result.ListProcurementItem.ForEach(item =>
                    {
                        response.ListProcurementItem.Add(new ProcurementRequestItemModel(item));
                    });
                }
                if (result.listDocument.Count > 0)
                {
                    result.listDocument.ForEach(doc =>
                    {
                        response.ListDocument.Add(new Models.Document.DocumentModel(doc));
                    });
                }
                response.Notes = result.Notes;
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetProcurementRequestByIdResponse() { };
            }
        }
        public EditProcurementRequestResponse EditProcurementRequest(EditProcurementRequestRequest request)
        {
            try
            {
                logger.LogInformation("EditProcurementRequest");
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.EditProcurementRequest(parameter);
                var response = new EditProcurementRequestResponse()
                {
                    ListDocumentEntityModel = result.ListDocumentEntityModel,
                    StatusCode = System.Net.HttpStatusCode.Accepted,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new EditProcurementRequestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCreateProcurementRequestResponse GetDataCreateProcurementRequest(GetDataCreateProcurementRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.GetDataCreateProcurementRequest(parameter);
                var response = new GetDataCreateProcurementRequestResponse
                {
                    IsWorkFlowInActive = result.IsWorkFlowInActive,
                    ListApproverEmployeeId = result.ListApproverEmployeeId,
                    ListOrder = new List<CustomerOrderModel>(),
                    ListOrderDetail = new List<CustomerOrderDetailModel>(),
                    CurrentEmployeeModel = result.CurrentEmployeeModel,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListOrder.ForEach(e => response.ListOrder.Add(new CustomerOrderModel(e)));
                result.ListOrderDetail.ForEach(e => {
                    var obj = new CustomerOrderDetailModel(e);
                    obj.OrderProductDetailProductAttributeValue = new List<OrderProductDetailProductAttributeValueModel>();
                    response.ListOrderDetail.Add(obj);
                });
                return response;
            }
            catch (Exception e)
            {
                return new GetDataCreateProcurementRequestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCreateProcurementRequestItemResponse GetDataCreateProcurementRequestItem(GetDataCreateProcurementRequestItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.GetDataCreateProcurementRequestItem(parameter);
                var response = new GetDataCreateProcurementRequestItemResponse
                {
                    ListVendor = result.ListVendor,
                    ListProduct = result.ListProduct,
                    ListMoneyUnit = result.ListMoneyUnit,
                    ListProcurementPlan = new List<Models.ProcurementPlan.ProcurementPlanModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };
                
                result.ListProcurementPlan.ForEach(e => response.ListProcurementPlan.Add(new Models.ProcurementPlan.ProcurementPlanModel(e)));

                return response;
            }
            catch (Exception e)
            {
                return new GetDataCreateProcurementRequestItemResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataEditProcurementRequestResponse GetDataEditProcurementRequest(GetDataEditProcurementRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.GetDataEditProcurementRequest(parameter);
                var response = new GetDataEditProcurementRequestResponse
                {
                    IsWorkFlowInActive = result.IsWorkFlowInActive,
                    ListApproverEmployeeId = new List<Models.Employee.EmployeeModel>(),
                    ListProcurementRequestItemEntityModel = new List<ProcurementRequestItemModel>(),
                    ListOrder = new List<CustomerOrderModel>(),
                    ListOrderDetail = new List<CustomerOrderDetailModel>(),
                    ProcurementRequestEntityModel = new ProcurementRequestModel(result.ProcurementRequestEntityModel),
                    ListDocumentModel = result.ListDocumentModel,
                    ListNote = new List<NoteModel>(),
                    ListEmailSendTo = result.ListEmailSendTo,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListApproverEmployeeId.ForEach(e => response.ListApproverEmployeeId.Add(new Models.Employee.EmployeeModel(e)));
                result.ListOrder.ForEach(e => response.ListOrder.Add(new CustomerOrderModel(e)));
                result.ListOrderDetail.ForEach(e => {
                    var obj = new CustomerOrderDetailModel(e);
                    obj.OrderProductDetailProductAttributeValue = new List<OrderProductDetailProductAttributeValueModel>();
                    response.ListOrderDetail.Add(obj);
                });
                result.ListProcurementRequestItemEntityModel.ForEach(e => response.ListProcurementRequestItemEntityModel.Add(new ProcurementRequestItemModel(e)));

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataEditProcurementRequestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchProcurementRequestResponse GetMasterDataSearchProcurementRequest(GetMasterDataSearchProcurementRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.GetMasterDataSearchProcurementRequest(parameter);

                var response = new GetMasterDataSearchProcurementRequestResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListProduct = new List<ProductModel>(),
                    ListVendor = new List<VendorModel>(),
                    ListEmployee = new List<Models.Employee.EmployeeModel>(),
                    ListBudget = new List<Models.ProcurementPlan.ProcurementPlanModel>(),
                    ListStatus = new List<Models.Category.CategoryModel>()
                };

                result.Employees.ForEach(item =>
                {
                    response.ListEmployee.Add(new Models.Employee.EmployeeModel(item));
                });

                result.ListBudget.ForEach(item =>
                {
                    response.ListBudget.Add(new Models.ProcurementPlan.ProcurementPlanModel(item));
                });

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new Models.Category.CategoryModel(item));
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });

                result.ListVendor.ForEach(item =>
                {
                    response.ListVendor.Add(new VendorModel(item));
                });

                return response;
            }
            catch(Exception ex)
            {
                return new GetMasterDataSearchProcurementRequestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public CreateProcurementRequestResponse ApprovalOrReject(GetDataEditProcurementRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.ApprovalOrReject(parameter);

                var response = new CreateProcurementRequestResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new CreateProcurementRequestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public CreateProcurementRequestResponse ChangeStatus(GetDataEditProcurementRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.ChangeStatus(parameter);

                var response = new CreateProcurementRequestResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new CreateProcurementRequestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public SearchProcurementRequestResponse SearchProcurementRequestReport(SearchProcurementRequestRequest request)
        {
            try
            {
                logger.LogInformation("search ProcurementRequest");
                var parameter = request.ToParameter();
                var result = iProcurementRequestDataAccess.SearchProcurementRequestReport(parameter);
                var response = new SearchProcurementRequestResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };
                response.ListProcurementRequest = new List<Models.ProcurementRequest.ProcurementRequestModel>();
                if (result.ListProcurementRequest != null)
                {
                    result.ListProcurementRequest.ForEach(pr =>
                    {
                        response.ListProcurementRequest.Add(new Models.ProcurementRequest.ProcurementRequestModel(pr));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchProcurementRequestResponse() { };
            }
        }
    }
}
