using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.ReceiptInvoice;
using TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.ReceiptInvoice;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.ReceiptInvoice
{
    public class ReceiptInvoiceFactory : BaseFactory, IReceiptInvoice
    {
        private IReceiptInvoiceDataAccess iReceiptInvoiceDataAccess;

        public ReceiptInvoiceFactory(IReceiptInvoiceDataAccess _iReceiptInvoiceDataAccess, ILogger<ReceiptInvoiceFactory> _logger)
        {
            iReceiptInvoiceDataAccess = _iReceiptInvoiceDataAccess;
            logger = _logger;
        }

        public CreateReceiptInvoiceResponse CreateReceiptInvoice(CreateReceiptInvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Create ReceiptInvoice");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.CreateReceiptInvoice(parameter);
                return new CreateReceiptInvoiceResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateReceiptInvoiceResponse()
                {
                    MessageCode = CommonMessage.ReceiptInvoice.ADD_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public EditReceiptInvoiceResponse EditReceiptInvoice(EditReceiptInvoiceRequest request)
        {
            throw new NotImplementedException();
        }

        public GetReceiptInvoiceByIdResponse GetReceiptInvoiceById(GetReceiptInvoiceByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Receipt Invoice Id");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.GetReceiptInvoiceById(parameter);
                return new GetReceiptInvoiceByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    ReceiptInvoice = new ReceiptInvoiceModel(result.ReceiptInvoice),
                    //ReceiptName = result.ReceiptName,
                    //CreatedByName = result.CreatedByName
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetReceiptInvoiceByIdResponse
                {
                    MessageCode = CommonMessage.ReceiptInvoice.NO_INVOICE,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public GetBankReceiptInvoiceByIdResponse GetBankReceiptInvoiceById(GetBankReceiptInvoiceByIdRequest request)
        {
            try
            {
                logger.LogInformation("Get Bank Receipt Invoice Id");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.GetBankReceiptInvoiceById(parameter);
                return new GetBankReceiptInvoiceByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    BankReceiptInvoice = new BankReceiptInvoiceModel(result.BankReceiptInvoice),
                    BankReceiptInvoiceReasonText = result.BankReceiptInvoiceReasonText,
                    BankReceiptTypeText = result.BankReceiptTypeText,
                    OrganizationText = result.OrganizationText,
                    PriceCurrencyText = result.PriceCurrencyText,
                    StatusText = result.StatusText,
                    CreateName = result.CreateName,
                    ObjectName = result.ObjectName

                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetBankReceiptInvoiceByIdResponse
                {
                    MessageCode = CommonMessage.ReceiptInvoice.NO_INVOICE,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ExportReceiptinvoiceResponse ExportPdfReceiptInvoice(ExportReceiptinvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Export Receipt Invoice to Pdf");
                var param = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.ExportPdfReceiptInvoice(param);
                return new ExportReceiptinvoiceResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xuất Pdf thành công",
                    ReceiptInvoicePdf = result.ReceiptInvoicePdf,
                    Code = result.Code
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportReceiptinvoiceResponse
                {
                    MessageCode = "Xuất phiếu thu thất bại!",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ExportBankReceiptInvoiceResponse ExportBankReceiptInvoice(ExportBankReceiptInvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Export Receipt Invoice to Pdf");
                var param = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.ExportBankReceiptInvoice(param);
                return new ExportBankReceiptInvoiceResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xuất Pdf thành công",
                    BankReceiptInvoicePdf = result.BankReceiptInvoicePdf,
                    Code = result.Code
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportBankReceiptInvoiceResponse
                {
                    MessageCode = "Xuất báo có thất bại!",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchReceiptInvoiceResponse SearchReceiptInvoice(SearchReceiptInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Receipt Invoice");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.SearchReceiptInvoice(parameter);
                var response = new SearchReceiptInvoiceResponse
                {
                    lstReceiptInvoiceEntity = new List<ReceiptInvoiceModel>(),
                    MessageCode = result.Message,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                result.lstReceiptInvoiceEntity.ForEach(item =>
                {
                    response.lstReceiptInvoiceEntity.Add(new ReceiptInvoiceModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new SearchReceiptInvoiceResponse
                {
                    MessageCode = e.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }
        public SearchCashBookReceiptInvoiceResponse SearchCashBookReceiptInvoice(SearchCashBookReceiptInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Receipt Invoice");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.SearchCashBookReceiptInvoice(parameter);
                var response = new SearchCashBookReceiptInvoiceResponse
                {
                    lstReceiptInvoiceEntity = new List<ReceiptInvoiceModel>(),
                    MessageCode = result.Message,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                result.lstReceiptInvoiceEntity.ForEach(item =>
                {
                    response.lstReceiptInvoiceEntity.Add(new ReceiptInvoiceModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new SearchCashBookReceiptInvoiceResponse
                {
                    MessageCode = e.ToString(),
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }
        public CreateBankReceiptInvoiceResponse CreateBankReceiptInvoice(CreateBankReceiptInvoiceRequest request)
        {
            try
            {
                logger.LogInformation("Create BankReceiptInvoice");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.CreateBankReceiptInvoice(parameter);
                return new CreateBankReceiptInvoiceResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new CreateBankReceiptInvoiceResponse()
                {
                    MessageCode = CommonMessage.BankReceiptInvoice.ADD_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
        public SearchBankReceiptInvoiceResponse SearchBankReceiptInvoice(SearchBankReceiptInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Bank ReceiptInvoice Order");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.SearchBankReceiptInvoice(parameter);
                var response = new SearchBankReceiptInvoiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    BankReceiptInvoiceList = new List<BankReceiptInvoiceModel>(),
                    MessageCode = result.Message
                };

                result.BankReceiptInvoiceList.ForEach(item =>
                {
                    response.BankReceiptInvoiceList.Add(new BankReceiptInvoiceModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new SearchBankReceiptInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.ReceiptInvoice.NO_INVOICE
                };
            }
        }

        public SearchBankBookReceiptResponse SearchBankBookReceipt(SearchBankBookReceiptRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Bank Book Receipt");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.SearchBankBookReceipt(parameter);
                var response = new SearchBankBookReceiptResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    BankReceiptInvoiceList = new List<BankReceiptInvoiceModel>(),
                    MessageCode = result.Message
                };

                result.BankReceiptInvoiceList.ForEach(item =>
                {
                    response.BankReceiptInvoiceList.Add(new BankReceiptInvoiceModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new SearchBankBookReceiptResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.ReceiptInvoice.NO_INVOICE
                };
            }
        }

        public GetOrderByCustomerIdResponse GetOrderByCustomerId(GetOrderByCustomerIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Order By Customer Id");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.GetOrderByCustomerId(parameter);
                var response = new GetOrderByCustomerIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    listOrder = new List<ReceiptInvoiceOrderModel>(),
                    totalAmountReceivable = result.totalAmountReceivable,
                    MessageCode = result.Message
                };

                result.listOrder.ForEach(item =>
                {
                    ReceiptInvoiceOrderModel tmp = new ReceiptInvoiceOrderModel();
                    tmp.OrderId = item.OrderId;
                    tmp.OrderCode = item.OrderCode;
                    tmp.AmountCollected = item.AmountCollected;
                    tmp.AmountReceivable = item.AmountReceivable;
                    tmp.Total = item.Total;
                    tmp.OrderDate = item.OrderDate;

                    response.listOrder.Add(tmp);
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex.Message);
                return new GetOrderByCustomerIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.ReceiptInvoice.NO_INVOICE
                };
            }
        }

        public GetMaterDataSearchBankReceiptInvoiceResponse GetMaterDataSearchBankReceiptInvoice(GetMasterDataSearchBankReceiptInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Master Data Search Bank Receipt Invoice");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.GetMasterDateSearchBankReceiptInvoice(parameter);
                var response = new GetMaterDataSearchBankReceiptInvoiceResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ReasonOfPaymentList = new List<CategoryModel>(),
                    StatusOfPaymentList = new List<CategoryModel>(),
                    EmployeeList = new List<Models.Employee.EmployeeModel>()
                };

                result.ReasonOfReceiptList.ForEach(item =>
                {
                    response.ReasonOfPaymentList.Add(new CategoryModel(item));
                });

                result.StatusOfReceiptList.ForEach(item =>
                {
                    response.StatusOfPaymentList.Add(new CategoryModel(item));
                });

                result.Employees.ForEach(item =>
                {
                    response.EmployeeList.Add(new Models.Employee.EmployeeModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMaterDataSearchBankReceiptInvoiceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataReceiptInvoiceResponse GetMasterDataReceiptInvoice(GetMasterDataReceiptInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Master Data Receipt Invoice");
                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.GetMasterDataReceiptInvoice(parameter);
                var response = new GetMasterDataReceiptInvoiceResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    CustomerList = new List<Models.Customer.CustomerModel>(),
                    ReasonOfReceiptList = new List<CategoryModel>(),
                    TypesOfReceiptList = new List<CategoryModel>(),
                    OrganizationList = new List<Models.Admin.OrganizationModel>(),
                    StatusOfReceiptList = new List<CategoryModel>(),
                    UnitMoneyList = new List<CategoryModel>()
                };

                result.CustomerList.ForEach(item =>
                {
                    response.CustomerList.Add(new Models.Customer.CustomerModel(item));
                });

                result.OrganizationList.ForEach(item =>
                {
                    response.OrganizationList.Add(new Models.Admin.OrganizationModel(item));
                });

                result.ReasonOfReceiptList.ForEach(item =>
                {
                    response.ReasonOfReceiptList.Add(new CategoryModel(item));
                });

                result.TypesOfReceiptList.ForEach(item =>
                {
                    response.TypesOfReceiptList.Add(new CategoryModel(item));
                });

                result.StatusOfReceiptList.ForEach(item =>
                {
                    response.StatusOfReceiptList.Add(new CategoryModel(item));
                });

                result.UnitMoneyList.ForEach(item =>
                {
                    response.UnitMoneyList.Add(new CategoryModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataReceiptInvoiceResponse
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataSearchReceiptInvoiceResponse GetGetMasterDataSearchReceiptInvoice(GetMasterDataSearchReceiptInvoiceRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Master Data Search Receipt Invoice");

                var parameter = request.ToParameter();
                var result = iReceiptInvoiceDataAccess.GetGetMasterDataSearchReceiptInvoice(parameter);

                var response = new GetMasterDataSearchReceiptInvoiceResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListReason = new List<CategoryModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListEmployee = new List<Models.Employee.EmployeeModel>()
                };

                result.Employees.ForEach(item =>
                {
                    response.ListEmployee.Add(new Models.Employee.EmployeeModel(item));
                });

                result.ReasonOfReceiptList.ForEach(item =>
                {
                    response.ListReason.Add(new CategoryModel(item));
                });

                result.StatusOfReceiptList.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                return response;
            }catch(Exception ex)
            {
                return new GetMasterDataSearchReceiptInvoiceResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = ex.Message.ToString(),
                };
            }

        }
    }
}
