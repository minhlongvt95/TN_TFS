using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Receivable;
using TN.TNM.BusinessLogic.Messages.Requests.Receivable.Customer;
using TN.TNM.BusinessLogic.Messages.Requests.Receivable.Vendor;
using TN.TNM.BusinessLogic.Messages.Requests.SalesReport;
using TN.TNM.BusinessLogic.Messages.Responses.Receivable.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Receivable.Vendor;
using TN.TNM.BusinessLogic.Messages.Responses.SalesReport;
using TN.TNM.BusinessLogic.Models.Receivable;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Receivable
{
    public class ReceivableFactory : BaseFactory, IReceivable
    {
        private IReceivableDataAccess iReceivableDataAccess;

        public ReceivableFactory(IReceivableDataAccess _iReceivableDataAccess, ILogger<ReceivableFactory> _logger)
        {
            iReceivableDataAccess = _iReceivableDataAccess;
            logger = _logger;
        }

        public GetReceivableVendorDetailResponse GetReceivableVendorDetail(GetReceivableVendorDetailRequest request)
        {
            try
            {
                logger.LogInformation("Get Receipt Invoice Id");
                var parameter = request.ToParameter();
                var result = iReceivableDataAccess.GetReceivableVendorDetail(parameter);
                var response = new GetReceivableVendorDetailResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ReceivableVendorDetail = new List<ReceivableVendorReportModel>(),
                    ReceiptsList = new List<ReceivableVendorReportModel>(),
                    VendorName = result.VendorName,
                    TotalReceivableBefore = result.TotalReceivableBefore,
                    TotalReceivable = result.TotalReceivable,
                    TotalReceivableInPeriod = result.TotalReceivableInPeriod,
                    TotalValueReceipt = result.TotalValueReceipt,
                    TotalValueOrder = result.TotalValueOrder
                };

                result.ReceivableVendorDetail.ForEach(item => {
                    response.ReceivableVendorDetail.Add(new ReceivableVendorReportModel(item));
                });
                result.ReceiptsList.ForEach(item => {
                    response.ReceiptsList.Add(new ReceivableVendorReportModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetReceivableVendorDetailResponse
                {
                    MessageCode = CommonMessage.Vendor.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetReceivableVendorReportResponse GetReceivableVendorReport(GetReceivableVendorReportRequest request)
        {
            try
            {
                logger.LogInformation("Get Receivable Vendor Report");
                var parameter = request.ToParameter();
                var result = iReceivableDataAccess.GetReceivableVendorReport(parameter);
                var response = new GetReceivableVendorReportResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ReceivableVendorReport = new List<ReceivableVendorReportModel>(),
                    TotalPurchase = result.TotalPurchase,
                    TotalPaid = result.TotalPaid
                };
                result.ReceivableVendorReport.ForEach(item => {
                    response.ReceivableVendorReport.Add(new ReceivableVendorReportModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetReceivableVendorReportResponse
                {
                    MessageCode = CommonMessage.Vendor.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetReceivableCustomerReportResponse GetReceivableCustomerReport(GetReceivableCustomerReportRequest request)
        {
            try
            {
                logger.LogInformation("Get Receivable Customer Report");
                var parameter = request.ToParameter();
                var result = iReceivableDataAccess.GetReceivableCustomerReport(parameter);
                var response = new GetReceivableCustomerReportResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
                    ReceivableCustomerReport = new List<ReceivableCustomerModel>(),
                    TotalPurchase = result.TotalPurchase,
                    TotalPaid = result.TotalPaid,
                    TotalReceipt = result.TotalReceipt,
                    MessageCode = result.Status ? "" : result.Message
                };
                result.ReceivableCustomerReport.ForEach(item => {
                    response.ReceivableCustomerReport.Add(new ReceivableCustomerModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new GetReceivableCustomerReportResponse
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetReceivableCustomerDetailResponse GetReceivableCustomerDetail(GetReceivableCustomerDetailRequest request)
        {
            try
            {
                logger.LogInformation("Get Receivable Customer Detail");
                var parameter = request.ToParameter();
                var result = iReceivableDataAccess.GetReceivableCustomerDetail(parameter);
                var response = new GetReceivableCustomerDetailResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ReceivableCustomerDetail = new List<ReceivableCustomerModel>(),
                    ReceiptsList = new List<ReceivableCustomerModel>(),
                    CustomerName = result.CustomerName,
                    CustomerContactId = result.CustomerContactId,
                    TotalReceivableBefore = result.TotalReceivableBefore,
                    TotalReceivable = result.TotalReceivable,
                    TotalReceivableInPeriod = result.TotalReceivableInPeriod,
                    TotalPurchaseProduct = result.TotalPurchaseProduct,
                    TotalReceipt = result.TotalReceipt
                };

                result.ReceivableCustomerDetail.ForEach(item => {
                    response.ReceivableCustomerDetail.Add(new ReceivableCustomerModel(item));
                });
                result.ReceiptsList.ForEach(item => {
                    response.ReceiptsList.Add(new ReceivableCustomerModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetReceivableCustomerDetailResponse
                {
                    MessageCode = CommonMessage.Customer.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public ExportExcelReceivableReportResponse ExportExcelReceivableReport(ExportExcelReceivableReportRequest request)
        {
            try
            {
                logger.LogInformation("Get Receivable Customer Report");
                var parameter = request.ToParameter();
                var result = iReceivableDataAccess.ExportExcelReceivableReport(parameter);
                var response = new ExportExcelReceivableReportResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ExcelFile = result.ExcelFile,
                    CustomerName = result.CustomerName
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new ExportExcelReceivableReportResponse
                {
                    MessageCode = CommonMessage.ExportExcelReceiveable.FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SalesReportResponse SearchSalesReport(SalesReportRequest request)
        {
            try
            {
                logger.LogInformation("Search Sale Report");
                var parameter = request.ToParameter();
                var result = iReceivableDataAccess.SearchSalesReport(parameter);
                var response = new SalesReportResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    TotalSale = result.TotalSale,
                    TotalCost = result.TotalCost,
                    SalesReportList = new List<SalesReportModel>()
                };
                result.SalesReportList.ForEach(item => {
                    response.SalesReportList.Add(new SalesReportModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SalesReportResponse
                {
                    MessageCode = CommonMessage.SalesReport.SEARCH_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetDataSearchReceivableVendorResponse GetDataSearchReceivableVendor(GetDataSearchReceivableVendorRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iReceivableDataAccess.GetDataSearchReceivableVendor(parameter);
                var respone = new GetDataSearchReceivableVendorResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListVendor = new List<Models.Vendor.VendorModel>()
                };

                result.ListVendor.ForEach(item => 
                {
                    respone.ListVendor.Add(new Models.Vendor.VendorModel(item));
                });

                return respone;
            }catch(Exception ex)
            {
                return new GetDataSearchReceivableVendorResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }
    }
}
