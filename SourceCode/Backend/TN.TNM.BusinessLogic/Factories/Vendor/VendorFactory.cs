using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Vendor;
using TN.TNM.BusinessLogic.Messages.Requests.Vendor;
using TN.TNM.BusinessLogic.Messages.Responses.Vendor;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Cost;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.OrderStatus;
using TN.TNM.BusinessLogic.Models.PurchaseOrderStatus;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Factories.Vendor
{
    public class VendorFactory : BaseFactory, IVendor
    {
        private IVendorDataAccess iVendorDataAccess;
        public VendorFactory(IVendorDataAccess _iVendorDataAccess, ILogger<VendorFactory> _logger)
        {
            this.iVendorDataAccess = _iVendorDataAccess;
            this.logger = _logger;
        }

        public CreateVendorResponse CreateVendor(CreateVendorRequest request)
        {
            try
            {
                this.logger.LogInformation("Create new Vendor");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.CreateVendor(parameter);
                var response = new CreateVendorResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ContactId = result.Status ? result.ContactId : Guid.Empty,
                    VendorId = result.Status ? result.VendorId : Guid.Empty
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new CreateVendorResponse
                {
                    MessageCode = CommonMessage.Vendor.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchVendorResponse SearchVendor(SearchVendorRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Vendor");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.SearchVendor(parameter);
                var response = new SearchVendorResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    VendorList = new List<VendorModel>()
                };
                result.VendorList.ForEach(vendor =>
                {
                    response.VendorList.Add(new VendorModel(vendor));
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SearchVendorResponse
                {
                    MessageCode = CommonMessage.Vendor.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetVendorByIdResponse GetVendorById(GetVendorByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Vendor by Id");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetVendorById(parameter);
                var response = new GetVendorByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Vendor = new VendorModel(result.Vendor),
                    Contact = new ContactModel(result.Contact),
                    VendorBankAccountList = new List<BankAccountModel>(),
                    VendorContactList = new List<ContactModel>(),
                    FullAddress = result.FullAddress,
                    CountVendorInformation = result.CountVendorInformation,
                    ListExchangeByVendor = result.ListExchangeByVendor
                };
                if (result.VendorBankAccountList.Count > 0)
                {
                    result.VendorBankAccountList.ForEach(bank =>
                    {
                        response.VendorBankAccountList.Add(new BankAccountModel(bank));
                    });
                }
                if (result.VendorContactList.Count > 0)
                {
                    result.VendorContactList.ForEach(contact =>
                    {
                        response.VendorContactList.Add(new ContactModel(contact));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetVendorByIdResponse
                {
                    MessageCode = CommonMessage.Vendor.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllVendorCodeResponse GetAllVendorCode(GetAllVendorCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Vendor code");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetAllVendorCode(parameter);
                var reponse = new GetAllVendorCodeResponse()
                {
                    StatusCode = HttpStatusCode.Accepted,
                    VendorCodeList = result.VendorCodeList,
                    //                    VendorList = result.VendorList
                };
                return reponse;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllVendorCodeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateVendorByIdResponse UpdateVendorById(UpdateVendorByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Vendor");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.UpdateVendorById(parameter);
                var reponse = new UpdateVendorByIdResponse()
                {
                    StatusCode = HttpStatusCode.Accepted,
                    MessageCode = result.Message
                };
                return reponse;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new UpdateVendorByIdResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Vendor.EDIT_FAIL
                };
            }
        }

        public QuickCreateVendorResponse QuickCreateVendor(QuickCreateVendorRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.QuickCreateVendor(parameter);
                var reponse = new QuickCreateVendorResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    VendorId = result.Status ? result.VendorId : Guid.Empty,
                    ListVendor = result.ListVendor
                };
                return reponse;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new QuickCreateVendorResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = ex.ToString()
                };
            }
        }

        public CreateVendorOrderResponse CreateVendorOrder(CreateVendorOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Vendor Order");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.CreateVendorOrder(parameter);
                var response = new CreateVendorOrderResponse()
                {
                    VendorOrderId = result.VendorOrderId,
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Status ? CommonMessage.Vendor.CREATE_ORDER_SUCCESS : result.Message
                };
                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new CreateVendorOrderResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Vendor.CREATE_ORDER_FAIL
                };
            }
        }

        public SearchVendorOrderResponse SearchVendorOrder(SearchVendorOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Vendor Order");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.SearchVendorOrder(parameter);
                var response = new SearchVendorOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    VendorOrderList = new List<VendorOrderModel>(),
                    MessageCode = result.Message
                };
                result.VendorOrderList.ForEach(vendorOrder =>
                {
                    response.VendorOrderList.Add(new VendorOrderModel(vendorOrder));
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SearchVendorOrderResponse
                {
                    MessageCode = CommonMessage.Vendor.SEARCH_ORDER_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetAllVendorResponse GetAllVendor(GetAllVendorRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Vendor");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetAllVendor(parameter);
                var response = new GetAllVendorResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    VendorList = new List<VendorModel>(),
                    MessageCode = result.Message
                };
                result.VendorList.ForEach(vendor =>
                {
                    response.VendorList.Add(new VendorModel(vendor));
                });

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetAllVendorResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetVendorOrderByIdResponse GetVendorOrderById(GetVendorOrderByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Vendor order by id");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetVendorOrderById(parameter);
                var response = new GetVendorOrderByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    VendorOrder = result.VendorOrder != null ? new VendorOrderModel(result.VendorOrder) : null,
                    VendorOrderDetailList = new List<VendorOrderDetailModel>(),
                    ContactId = result.ContactId
                };

                if (result.VendorOrderDetailList != null)
                {
                    result.VendorOrderDetailList.ForEach(item =>
                    {
                        VendorOrderDetailModel a = new VendorOrderDetailModel(item);

                        List<VendorOrderProductDetailProductAttributeValueModel> lstOrderProductDetailProductAttributeValueModel = new List<VendorOrderProductDetailProductAttributeValueModel>();
                        if (item.VendorOrderProductDetailProductAttributeValue != null)
                        {
                            item.VendorOrderProductDetailProductAttributeValue.ForEach(X1 =>
                            {
                                lstOrderProductDetailProductAttributeValueModel.Add(new VendorOrderProductDetailProductAttributeValueModel(X1));
                            });
                            a.VendorOrderProductDetailProductAttributeValue = lstOrderProductDetailProductAttributeValueModel;
                        }

                        response.VendorOrderDetailList.Add(a);
                    });
                }

                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetVendorOrderByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Vendor.GET_ORDER_FAIL
                };
            }
        }

        public UpdateVendorOrderByIdResponse UpdateVendorOrderById(UpdateVendorOrderByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Vendor order");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.UpdateVendorOrderById(parameter);
                var reponse = new UpdateVendorOrderByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListItemInvalidModel = result.ListItemInvalidModel
                };
                return reponse;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new UpdateVendorOrderByIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Vendor.EDIT_ORDER_FAIL
                };
            }
        }

        public UpdateActiveVendorResponse UpdateActiveVendor(UpdateActiveVendorRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Vendor order");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.UpdateActiveVendor(parameter);
                var reponse = new UpdateActiveVendorResponse()
                {
                    StatusCode = HttpStatusCode.Accepted,
                    MessageCode = result.Message
                };
                return reponse;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new UpdateActiveVendorResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = CommonMessage.Vendor.EDIT_ORDER_FAIL
                };
            }
        }

        public QuickCreateVendorMasterdataResponse QuickCreateVendorMasterdata(QuickCreateVendorMasterdataRequest request)
        {
            try
            {
                this.logger.LogInformation("Quick Create Vendor Masterdata");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.QuickCreateVendorMasterdata(parameter);
                var reponse = new QuickCreateVendorMasterdataResponse()
                {
                    ListVendorCode = result.ListVendorCode,
                    ListVendorCategory = result.ListVendorCategory,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return reponse;
            }
            catch (Exception e)
            {

                return new QuickCreateVendorMasterdataResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.ToString()
                };
            }
        }

        public GetDataCreateVendorResponse GetDataCreateVendor(GetDataCreateVendorRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataCreateVendor(parameter);
                var response = new GetDataCreateVendorResponse
                {
                    ListVendorGroup = new List<Models.Category.CategoryModel>(),
                    ListVendorCode = result.ListVendorCode,
                    ListProvince = new List<Models.Admin.ProvinceModel>(),
                    ListDistrict = new List<Models.Admin.DistrictModel>(),
                    ListWard = new List<Models.Admin.WardModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListVendorGroup.ForEach(e => response.ListVendorGroup.Add(new Models.Category.CategoryModel(e)));
                result.ListProvince.ForEach(e => response.ListProvince.Add(new Models.Admin.ProvinceModel(e)));
                result.ListDistrict.ForEach(e => response.ListDistrict.Add(new Models.Admin.DistrictModel(e)));
                result.ListWard.ForEach(e => response.ListWard.Add(new Models.Admin.WardModel(e)));

                return response;
            }
            catch (Exception e)
            {
                return new GetDataCreateVendorResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataSearchVendorResponse GetDataSearchVendor(GetDataSearchVendorRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataSearchVendor(parameter);
                var response = new GetDataSearchVendorResponse
                {
                    ListVendorGroup = new List<Models.Category.CategoryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };
                result.ListVendorGroup.ForEach(e => response.ListVendorGroup.Add(new Models.Category.CategoryModel(e)));
                return response;
            }
            catch (Exception e)
            {
                return new GetDataSearchVendorResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataEditVendorResponse GetDataEditVendor(GetDataEditVendorRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataEditVendor(parameter);
                var response = new GetDataEditVendorResponse
                {
                    ListVendorGroup = new List<Models.Category.CategoryModel>(),
                    ListPaymentMethod = new List<Models.Category.CategoryModel>(),
                    ListVendorCode = result.ListVendorCode,
                    ListProvince = new List<Models.Admin.ProvinceModel>(),
                    ListDistrict = new List<Models.Admin.DistrictModel>(),
                    ListWard = new List<Models.Admin.WardModel>(),
                    ListReceivableByMonth = result.ListReceivableByMonth,
                    ListVendorOrderByMonth = result.ListVendorOrderByMonth,
                    ListVendorOrderInProcessByMonth = result.ListVendorOrderInProcessByMonth,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListVendorGroup.ForEach(e => response.ListVendorGroup.Add(new Models.Category.CategoryModel(e)));
                result.ListPaymentMethod.ForEach(e => response.ListPaymentMethod.Add(new Models.Category.CategoryModel(e)));
                result.ListProvince.ForEach(e => response.ListProvince.Add(new Models.Admin.ProvinceModel(e)));
                result.ListDistrict.ForEach(e => response.ListDistrict.Add(new Models.Admin.DistrictModel(e)));
                result.ListWard.ForEach(e => response.ListWard.Add(new Models.Admin.WardModel(e)));

                return response;
            }
            catch (Exception e)
            {
                return new GetDataEditVendorResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public CreateVendorContactResponse CreateVendorContact(CreateVendorContactRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.CreateVendorContact(parameter);
                var response = new CreateVendorContactResponse
                {
                    ContactId = result.ContactId,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateVendorContactResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCreateVendorOrderResponse GetDataCreateVendorOrder(GetDataCreateVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataCreateVendorOrder(parameter);
                var response = new GetDataCreateVendorOrderResponse
                {
                    ListPaymentMethod = new List<Models.Category.CategoryModel>(),
                    ListOrderStatus = new List<PurchaseOrderStatusModel>(),
                    ListEmployeeModel = result.ListEmployeeModel,
                    VendorCreateOrderModel = result.VendorCreateOrderModel,
                    ListBankAccount = new List<Models.BankAccount.BankAccountModel>(),
                    ListProcurementRequest = result.ListProcurementRequest,
                    //ListContract = result.ListContract,
                    ListWareHouse = result.ListWareHouse,
                    ListVendorGroup = result.ListVendorGroup,
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListBankAccount.ForEach(e =>
                    response.ListBankAccount.Add(new Models.BankAccount.BankAccountModel(e)));
                result.ListPaymentMethod.ForEach(e =>
                    response.ListPaymentMethod.Add(new Models.Category.CategoryModel(e)));
                result.ListOrderStatus.ForEach(e =>
                    response.ListOrderStatus.Add(new PurchaseOrderStatusModel(e)));

                return response;
            }
            catch (Exception e)
            {
                return new GetDataCreateVendorOrderResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataAddVendorOrderDetailResponse GetDataAddVendorOrderDetail(GetDataAddVendorOrderDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataAddVendorOrderDetail(parameter);
                var response = new GetDataAddVendorOrderDetailResponse
                {
                    ListMoneyUnit = result.ListMoneyUnit,
                    ListProductByVendorId = result.ListProductByVendorId,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataAddVendorOrderDetailResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchVendorOrderResponse GetMasterDataSearchVendorOrder(GetMasterDataSearchVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetGetMasterDataSearchVendorOrder(parameter);
                var response = new GetMasterDataSearchVendorOrderResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListVendor = new List<VendorModel>(),
                    ListOrderStatus = new List<PurchaseOrderStatusModel>(),
                    ListEmployee = new List<Models.Employee.EmployeeModel>(),
                    ListProcurementRequest = result.ListProcurementRequest,
                    ListProduct = result.ListProduct,
                    CompanyConfig = result.CompanyConfig
                };

                result.Vendors.ForEach(item =>
                {
                    response.ListVendor.Add(new VendorModel(item));
                });

                result.OrderStatuses.ForEach(item =>
                {
                    response.ListOrderStatus.Add(new PurchaseOrderStatusModel(item));
                });

                result.Employees.ForEach(item =>
                {
                    response.ListEmployee.Add(new Models.Employee.EmployeeModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchVendorOrderResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataEditVendorOrderResponse GetDataEditVendorOrder(GetDataEditVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataEditVendorOrder(parameter);
                var response = new GetDataEditVendorOrderResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    VendorOrderById = result.VendorOrderById,
                    ListVendorOrderDetailById = result.ListVendorOrderDetailById,
                    ListPaymentMethod = new List<Models.Category.CategoryModel>(),
                    ListOrderStatus = new List<PurchaseOrderStatusModel>(),
                    ListEmployeeModel = result.ListEmployeeModel,
                    VendorCreateOrderModel = result.VendorCreateOrderModel,
                    ListBankAccount = new List<Models.BankAccount.BankAccountModel>(),
                    ListProcurementRequest = result.ListProcurementRequest,
                    //ListContract = result.ListContract,
                    ListWareHouse = result.ListWareHouse,
                    ListProcurementRequestId = result.ListProcurementRequestId,
                    ListVendorOrderCostDetail = result.ListVendorOrderCostDetail,
                    ListNote = new List<NoteModel>()
                };

                result.ListBankAccount.ForEach(e =>
                    response.ListBankAccount.Add(new Models.BankAccount.BankAccountModel(e)));
                ;
                result.ListPaymentMethod.ForEach(e =>
                    response.ListPaymentMethod.Add(new Models.Category.CategoryModel(e)));
                result.ListOrderStatus.ForEach(e => response.ListOrderStatus.Add(new PurchaseOrderStatusModel(e)));

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataEditVendorOrderResponse
                {
                    StatusCode = HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataSearchVendorQuoteResponse GetDataSearchVendorQuote(GetDataSearchVendorQuoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataSearchVendorQuote(parameter);
                var response = new GetDataSearchVendorQuoteResponse
                {
                    ListVendorQuote = new List<SuggestedSupplierQuotesModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                result.ListVendorQuote.ForEach(item =>
                {
                    var vendorQuote = new SuggestedSupplierQuotesModel(item);
                    var listVendorQuoteDetail = new List<SuggestedSupplierQuotesDetailModel>();
                    item.ListVendorQuoteDetail.ForEach(detail =>
                    {
                        listVendorQuoteDetail.Add(new SuggestedSupplierQuotesDetailModel(detail));
                    });
                    vendorQuote.ListVendorQuoteDetail = listVendorQuoteDetail;

                    response.ListVendorQuote.Add(vendorQuote);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataSearchVendorQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public CreateVendorQuoteResponse CreateVendorQuote(ListVendorQuoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.CreateVendorQuote(parameter);
                var response = new CreateVendorQuoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateVendorQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.FailedDependency,
                    MessageCode = e.Message
                };
            }
        }

        public SearchVendorProductPriceResponse SearchVendorProductPrice(SearchVendorProductPriceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.SearchVendorProductPrice(parameter);
                var response = new SearchVendorProductPriceResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListVendorProductPrice = new List<Models.Product.ProductVendorMappingModel>()
                };

                result.ListVendorProductPrice.ForEach(item =>
                {
                    response.ListVendorProductPrice.Add(new Models.Product.ProductVendorMappingModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new SearchVendorProductPriceResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public CreateVendorProductPriceResponse CreateVendorProductPrice(CreateVendorProductPriceRequest request)
        {
            try
            {
                this.logger.LogInformation("Tạo bảng giá nhà cung cấp");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.CreateVendorProductPrice(parameter);
                var response = new CreateVendorProductPriceResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                return new CreateVendorProductPriceResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DeleteProductVendorPriceResponse DeleteProductVendorPrice(DeleteVendorProductPriceRequest request)
        {
            try
            {
                this.logger.LogInformation("Xóa bảng giá nhà cung cấp");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.DeleteVendorProductPrice(parameter);
                var response = new DeleteProductVendorPriceResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                return new DeleteProductVendorPriceResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DownloadTemplateVendorProductPriceResponse DownloadTemplateVendorProductPrice(DownloadTemplateVendorProductPriceRequest request)
        {
            try
            {
                this.logger.LogInformation("Download Template Import Vendor Product Price");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.DownloadTemplateVendorProductPrice(parameter);

                var response = new DownloadTemplateVendorProductPriceResponse()
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
                return new DownloadTemplateVendorProductPriceResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportVendorProductPriceResponse ImportVendorProductPrice(ImportVendorProductPriceRequest request)
        {
            try
            {
                this.logger.LogInformation("Import Vendor Product Price");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.ImportProductVendorPrice(parameter);

                var response = new ImportVendorProductPriceResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                return new ImportVendorProductPriceResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataCreateSuggestedSupplierQuoteResponse GetMasterDataCreateSuggestedSupplierQuote(GetMasterDataCreateSuggestedSupplierQuoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetMasterDataCreateSuggestedSupplierQuote(parameter);

                var response = new GetMasterDataCreateSuggestedSupplierQuoteResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListVendor = new List<VendorModel>(),
                    ListProduct = new List<Models.Product.ProductModel>(),
                    ListProcurementRequest = new List<Models.ProcurementRequest.ProcurementRequestModel>(),
                    ListProcurementRequestItem = new List<Models.ProcurementRequest.ProcurementRequestItemModel>(),
                    ListEmployee = new List<Models.Employee.EmployeeModel>(),
                    ListStatus = new List<Models.Category.CategoryModel>(),
                    InforExportExcel = result.InforExportExcel,
                    ListProductVendorMapping = new List<Models.Product.ProductVendorMappingModel>(),

                    SuggestedSupplierQuotes = new SuggestedSupplierQuotesModel()
                };

                result.ListVendor.ForEach(item => response.ListVendor.Add(new VendorModel(item)));
                result.ListProduct.ForEach(item => response.ListProduct.Add(new Models.Product.ProductModel(item)));
                result.ListProcurementRequest.ForEach(item => response.ListProcurementRequest.Add(new Models.ProcurementRequest.ProcurementRequestModel(item)));
                result.ListProcurementRequestItem.ForEach(item => response.ListProcurementRequestItem.Add(new Models.ProcurementRequest.ProcurementRequestItemModel(item)));
                result.ListEmployee.ForEach(item => response.ListEmployee.Add(new Models.Employee.EmployeeModel(item)));
                result.ListStatus.ForEach(item => response.ListStatus.Add(new Models.Category.CategoryModel(item)));
                result.ListProductVendorMapping.ForEach(item => response.ListProductVendorMapping.Add(new Models.Product.ProductVendorMappingModel(item)));

                // get suggested supplier quote request
                var suggestedSupplierQuote = new SuggestedSupplierQuotesModel(result.SuggestedSupplierQuotes);
                var listSuggestedSupplierQuoteDetail = new List<SuggestedSupplierQuotesDetailModel>();
                result.SuggestedSupplierQuotes.ListVendorQuoteDetail.ForEach(item =>
                {
                    listSuggestedSupplierQuoteDetail.Add(new SuggestedSupplierQuotesDetailModel(item));
                });
                suggestedSupplierQuote.ListVendorQuoteDetail = listSuggestedSupplierQuoteDetail;

                response.SuggestedSupplierQuotes = suggestedSupplierQuote;

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateSuggestedSupplierQuoteResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public CreateOrUpdateSuggestedSupplierQuoteResponse CreateOrUpdateSuggestedSupplierQuote(CreateOrUpdateSuggestedSupplierQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Create or update suggested supplier quote request");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.CreateOrUpdateSuggestedSupplierQuote(parameter);

                var response = new CreateOrUpdateSuggestedSupplierQuoteResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    SuggestedSupplierQuoteId = result.SuggestedSupplierQuoteId
                };

                return response;
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateSuggestedSupplierQuoteResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DeleteSuggestedSupplierQuoteRequestResponse DeleteSuggestedSupplierQuoteRequest(DeleteSuggestedSupplierQuoteRequestRequest request)
        {
            try
            {
                this.logger.LogInformation("Create or update suggested supplier quote request");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.DeleteSuggestedSupplierQuoteRequest(parameter);

                var response = new DeleteSuggestedSupplierQuoteRequestResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new DeleteSuggestedSupplierQuoteRequestResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataAddEditCostVendorOrderResponse GetDataAddEditCostVendorOrder(GetDataAddEditCostVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetDataAddEditCostVendorOrder(parameter);

                var response = new GetDataAddEditCostVendorOrderResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    ListCost = new List<CostModel>(),
                    MessageCode = result.Message,
                };

                result.ListCost.ForEach(item =>
                {
                    response.ListCost.Add(new CostModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataAddEditCostVendorOrderResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public ChangeStatusVendorQuoteResponse ChangeStatusVendorQuote(ChangeStatusVendorQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Change status suggested supplier quote request");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.ChangeStatusVendorQuote(parameter);

                var response = new ChangeStatusVendorQuoteResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                return new ChangeStatusVendorQuoteResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public SendEmailVendorQuoteResponse SendEmailVendorQuote(SendEmailVendorQuoteRequest request)
        {
            try
            {
                logger.LogInformation("SendEmailVendorQuote");
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.SendEmailVendorQuote(parameter);
                var response = new SendEmailVendorQuoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new SendEmailVendorQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message,
                };
            }
        }

        public RemoveVendorOrderResponse RemoveVendorOrder(RemoveVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.RemoveVendorOrder(parameter);
                var response = new RemoveVendorOrderResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new RemoveVendorOrderResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CancelVendorOrderResponse CancelVendorOrder(CancelVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.CancelVendorOrder(parameter);
                var response = new CancelVendorOrderResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new CancelVendorOrderResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DraftVendorOrderResponse DraftVendorOrder(DraftVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.DraftVendorOrder(parameter);
                var response = new DraftVendorOrderResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new DraftVendorOrderResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataVendorOrderReportResponse GetMasterDataVendorOrderReport(GetMasterDataVendorOrderReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetMasterDataVendorOrderReport(parameter);
                var response = new GetMasterDataVendorOrderReportResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListVendor = result.ListVendor,
                    ListStatus = new List<PurchaseOrderStatusModel>(),
                    ListProcurementRequest = result.ListProcurementRequest,
                    ListEmployee = result.ListEmployee
                };

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new PurchaseOrderStatusModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataVendorOrderReportResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchVendorOrderReportResponse SearchVendorOrderReport(SearchVendorOrderReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.SearchVendorOrderReport(parameter);
                var response = new SearchVendorOrderReportResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListVendorOrderReport = result.ListVendorOrderReport
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchVendorOrderReportResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public ApprovalOrRejectVendorOrderResponse ApprovalOrRejectVendorOrder(ApprovalOrRejectVendorOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.ApprovalOrRejectVendorOrder(parameter);
                var response = new ApprovalOrRejectVendorOrderResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListItemInvalidModel = result.ListItemInvalidModel
                };

                return response;
            }
            catch (Exception e)
            {
                return new ApprovalOrRejectVendorOrderResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetQuantityApprovalResponse GetQuantityApproval(GetQuantityApprovalRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iVendorDataAccess.GetQuantityApproval(parameter);
                var response = new GetQuantityApprovalResponse
                {
                    MessageCode = result.Message,
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    QuantityApproval = result.QuantityApproval
                };

                return response;
            }
            catch (Exception ex)
            {
                return new GetQuantityApprovalResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
        }
    }
}
