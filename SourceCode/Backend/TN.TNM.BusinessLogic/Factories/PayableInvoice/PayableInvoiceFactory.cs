using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.PayableInvoice;
using TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.PayableInvoice;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.PayableInvoice
{
    public class PayableInvoiceFactory : BaseFactory, IPayableInvoice
    {
        private IPayableInvoiceDataAccess iPayableInvoiceDataAccess;

        public PayableInvoiceFactory(IPayableInvoiceDataAccess _iPayableInvoiceDataAccess, ILogger<PayableInvoiceFactory> _logger)
        {
            this.iPayableInvoiceDataAccess = _iPayableInvoiceDataAccess;
            this.logger = _logger;
        }

        public CreatePayableInvoiceRespone CreatePayableInvoice(CreatePayableInvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Create PayableInvoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.CreatePayableInvoice(parameter);
                return new CreatePayableInvoiceRespone
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreatePayableInvoiceRespone()
                {
                    MessageCode = CommonMessage.PayableInvoice.ADD_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public EditPayableInvoiceRespone EditPayableInvoice(EditPayableInvoiceRequest request)
        {
            throw new NotImplementedException();
        }

        public GetPayableInvoiceByIdResponse GetPayableInvoiceById(GetPayableInvoiceByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Payment invoice by Id");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.GetPayableInvoiceById(parameter);
                var response = new GetPayableInvoiceByIdResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    PayableInvoice = new PayableInvoiceModel(result.PayableInvoice),
                    //PayerName = result.PayerName,
                    //CreatedByName = result.CreatedByName
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetPayableInvoiceByIdResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchPayableInvoiceResponse SearchPayableInvoice(SearchPayableInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Payable Order");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.SearchPayableInvoice(parameter);
                var response = new SearchPayableInvoiceResponse() {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    PayableInvList = new List<PayableInvoiceModel>(),
                    MessageCode = result.Message
                };

                result.PayableInvList.ForEach(item => {
                    response.PayableInvList.Add(new PayableInvoiceModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new SearchPayableInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.PayableInvoice.NO_INVOICE
                };
            }
        }
        public SearchCashBookPayableInvoiceResponse SearchCashBookPayableInvoice(SearchCashBookPayableInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Cash Book Payable Invoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.SearchCashBookPayableInvoice(parameter);
                var response = new SearchCashBookPayableInvoiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    PayableInvList = new List<PayableInvoiceModel>(),
                    MessageCode = result.Message
                };

                result.PayableInvList.ForEach(item => {
                    response.PayableInvList.Add(new PayableInvoiceModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new SearchCashBookPayableInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.PayableInvoice.NO_INVOICE
                };
            }
        }

        public CreateBankPayableInvoiceResponse CreateBankPayableInvoice(CreateBankPayableInvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Create BankPayableInvoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.CreateBankPayableInvoice(parameter);
                return new CreateBankPayableInvoiceResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateBankPayableInvoiceResponse()
                {
                    MessageCode = CommonMessage.BankPayableInvoice.ADD_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchBankPayableInvoiceResponse SearchBankPayableInvoice(SearchBankPayableInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Bank Payable Order");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.SearchBankPayableInvoice(parameter);
                var response = new SearchBankPayableInvoiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    BankPayableInvoiceList = new List<BankPayableInvoiceModel>(),
                    MessageCode = result.Message
                };

                result.BankPayableInvoiceList.ForEach(item => {
                    response.BankPayableInvoiceList.Add(new BankPayableInvoiceModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new SearchBankPayableInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.PayableInvoice.NO_INVOICE
                };
            }
        }

        public ExportPayableInvoiceResponse ExportPayableInvoice(ExportPayableInvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Export Receipt Invoice to Pdf");
                var param = request.ToParameter();
                var result = iPayableInvoiceDataAccess.ExportPayableInvoice(param);
                return new ExportPayableInvoiceResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xuất Pdf thành công",
                    Code = result.Code,
                    PayableInvoicePdf = result.PayableInvoicePdf
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportPayableInvoiceResponse
                {
                    MessageCode = "Xuất phiếu chi thất bại!",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ExportBankPayableInvoiceResponse ExportBankPayableInvoice(ExportBankPayableInvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Export Receipt Invoice to Pdf");
                var param = request.ToParameter();
                var result = iPayableInvoiceDataAccess.ExportBankPayableInvoice(param);
                return new ExportBankPayableInvoiceResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xuất Pdf thành công",
                    BankPayableInvoicePdf = result.BankPayableInvoicePdf,
                    Code = result.Code
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportBankPayableInvoiceResponse
                {
                    MessageCode = "Xuất phiếu thu thất bại!",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetBankPayableInvoiceByIdResponse GetBankPayableInvoiceById(GetBankPayableInvoiceByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Bank Payment by Id");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.GetBankPayableInvoiceById(parameter);
                var response = new GetBankPayableInvoiceByIdResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    BankPayableInvoice = new BankPayableInvoiceModel(result.BankPayableInvoice),
                    //CreatedName = result.CreatedName,
                    //ObjectName = result.ObjectName
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetBankPayableInvoiceByIdResponse
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchBankBookPayableInvoiceResponse SearchBankBookPayableInvoice(SearchBankBookPayableInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Bank Book Payable Invoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.SearchBankBookPayableInvoice(parameter);
                var response = new SearchBankBookPayableInvoiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    BankPayableInvoiceList = new List<BankPayableInvoiceModel>(),
                    MessageCode = result.Message
                };

                result.BankPayableInvoiceList.ForEach(item => {
                    response.BankPayableInvoiceList.Add(new BankPayableInvoiceModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new SearchBankBookPayableInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.PayableInvoice.NO_INVOICE
                };
            }
        }

        // LongNH
        public GetMasterDataPayableInvoiceResponse GetMasterDataPayableInvoice(GetMasterDataPayableInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Master Data Payable Invoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.GetMasterDataPayableInvoice(parameter);
                var response = new GetMasterDataPayableInvoiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ReasonOfPaymentList = new List<CategoryModel>(),
                    StatusOfPaymentList = new List<CategoryModel>(),
                    TypesOfPaymentList = new List<CategoryModel>(),
                    UnitMoneyList = new List<CategoryModel>(),
                    OrganizationList = new List<OrganizationModel>(),
                    CustomerList = new List<CustomerModel>()
                };

                result.ReasonOfPaymentList.ForEach(item =>
                {
                    response.ReasonOfPaymentList.Add(new CategoryModel(item));
                });

                result.StatusOfPaymentList.ForEach(item =>
                {
                    response.StatusOfPaymentList.Add(new CategoryModel(item));
                });

                result.TypesOfPaymentList.ForEach(item =>
                {
                    response.TypesOfPaymentList.Add(new CategoryModel(item));
                });

                result.UnitMoneyList.ForEach(item =>
                {
                    response.UnitMoneyList.Add(new CategoryModel(item));
                });

                result.OrganizationList.ForEach(item =>
                {
                    response.OrganizationList.Add(new OrganizationModel(item));
                });

                result.CustomerList.ForEach(item =>
                {
                    response.CustomerList.Add(new CustomerModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new GetMasterDataPayableInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.PayableInvoice.NO_INVOICE
                };
            }
        }

        public GetMasterDataBankPayableInvoiceResponse GetMasterDataBankPayableInvoice(GetMasterDataBankPayableInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Master Data Bank Payable Invoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.GetMaterDataBankPayableInvoice(parameter);

                var response = new GetMasterDataBankPayableInvoiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ReasonOfPaymentList = new List<CategoryModel>(),
                    StatusOfPaymentList = new List<CategoryModel>(),
                    TypesOfPaymentList = new List<BankAccountModel>(),
                    UnitMoneyList = new List<CategoryModel>(),
                    OrganizationList = new List<OrganizationModel>(),
                    VendorList = new List<Models.Vendor.VendorModel>()
                };

                result.ReasonOfPaymentList.ForEach(item =>
                {
                    response.ReasonOfPaymentList.Add(new CategoryModel(item));
                });

                result.StatusOfPaymentList.ForEach(item =>
                {
                    response.StatusOfPaymentList.Add(new CategoryModel(item));
                });

                result.TypesOfPaymentList.ForEach(item =>
                {
                    response.TypesOfPaymentList.Add(new BankAccountModel(item));
                });

                result.UnitMoneyList.ForEach(item =>
                {
                    response.UnitMoneyList.Add(new CategoryModel(item));
                });

                result.OrganizationList.ForEach(item =>
                {
                    response.OrganizationList.Add(new OrganizationModel(item));
                });

                result.VendorList.ForEach(item =>
                {
                    response.VendorList.Add(new Models.Vendor.VendorModel(item));
                });

                return response;
            }
            catch(Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new GetMasterDataBankPayableInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.PayableInvoice.NO_INVOICE
                };
            }

        }

        public GetMasterDataPayableInvoiceSearchResponse GetMasterDataPayableInvoiceSearch(GetMasterDataPayableInvoiceSearchRequest request)
        {

            try
            {
                this.logger.LogInformation("Get Master Data Payable Invoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.GetMasterDataPayableInvoiceSearch(parameter);
                var response = new GetMasterDataPayableInvoiceSearchResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ReasonOfPaymentList = new List<CategoryModel>(),
                    StatusOfPaymentList = new List<CategoryModel>(),
                    lstUserEntityModel = new List<EmployeeModel>()
                };

                result.ReasonOfPaymentList.ForEach(item =>
                {
                    response.ReasonOfPaymentList.Add(new CategoryModel(item));
                });

                result.StatusOfPaymentList.ForEach(item =>
                {
                    response.StatusOfPaymentList.Add(new CategoryModel(item));
                });

                result.lstUserEntityModel.ForEach(item =>
                {
                    response.lstUserEntityModel.Add(new EmployeeModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new GetMasterDataPayableInvoiceSearchResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.PayableInvoice.NO_INVOICE
                };
            }
        }

        public GetMasterDataBankSearchPayableInvoiceResponse GetMasterDataBankSearchPayableInvoice(GetMasterDataSearchBankPayableInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Master Data Search Bank Payable Invoice");
                var parameter = request.ToParameter();
                var result = iPayableInvoiceDataAccess.GetMasterDateSearchBankPayableInvoice(parameter);
                var response = new GetMasterDataBankSearchPayableInvoiceResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ReasonOfPaymentList = new List<CategoryModel>(),
                    StatusOfPaymentList = new List<CategoryModel>(),
                    EmployeeList = new List<Models.Employee.EmployeeModel>()
                };

                result.ReasonOfPaymentList.ForEach(item =>
                {
                    response.ReasonOfPaymentList.Add(new CategoryModel(item));
                });

                result.StatusOfPaymentList.ForEach(item =>
                {
                    response.StatusOfPaymentList.Add(new CategoryModel(item));
                });

                result.Employees.ForEach(item =>
                {
                    response.EmployeeList.Add(new Models.Employee.EmployeeModel(item));
                });

                return response;
            }
            catch(Exception ex)
            {
                return new GetMasterDataBankSearchPayableInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
    }
}
