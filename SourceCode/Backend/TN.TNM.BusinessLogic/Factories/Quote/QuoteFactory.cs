using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Interfaces.Quote;
using TN.TNM.BusinessLogic.Messages.Requests.Quote;
using TN.TNM.BusinessLogic.Messages.Responses.Quote;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Lead;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.SaleBidding;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Quote
{
    public class QuoteFactory : BaseFactory, IQuote
    {
        private IQuoteDataAccess iQuoteDataAccess;

        public QuoteFactory(IQuoteDataAccess _iQuoteDataAccess, ILogger<QuoteFactory> _logger)
        {
            this.iQuoteDataAccess = _iQuoteDataAccess;
            this.logger = _logger;
        }

        public CreateQuoteResponse CreateQuote(CreateQuoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.CreateQuote(parameter);
                var response = new CreateQuoteResponse
                {
                    QuoteID = result.QuoteID,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListQuoteDocument = new List<QuoteDocumentModel>()
                };
                result.ListQuoteDocument.ForEach(item =>
                {
                    response.ListQuoteDocument.Add(new QuoteDocumentModel(item));
                });
                return response;
            }
            catch (Exception ex)
            {
                return new CreateQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public ExportPdfQuotePDFResponse ExportPdfQuote(ExportPdfQuoteRequest request)
        {
            throw new NotImplementedException();
        }

        public GetTop3QuotesOverdueResponse GetTop3QuotesOverdue(GetTop3QuotesOverdueRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Top 3 Quotes Overdue");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetTop3QuotesOverdue(parameter);

                var response = new GetTop3QuotesOverdueResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QuoteList = new List<GetTop3QuotesOverdueeModel>()
                };
                result.QuotesOverdueList.ForEach(item =>
                {
                    response.QuoteList.Add(new GetTop3QuotesOverdueeModel(item));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception ex)
            {
                return new GetTop3QuotesOverdueResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetTop3WeekQuotesOverdueResponse GetTop3WeekQuotesOverdue(GetTop3WeekQuotesOverdueRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Top 3 Week Quotes Overdue");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetTop3WeekQuotesOverdue(parameter);

                var response = new GetTop3WeekQuotesOverdueResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QuoteList = new List<GetTop3WeekQuotesOverdueeModel>()
                };
                result.QuotesOverdueList.ForEach(item =>
                {
                    response.QuoteList.Add(new GetTop3WeekQuotesOverdueeModel(item));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception ex)
            {
                return new GetTop3WeekQuotesOverdueResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetAllQuoteResponse GetAllQuote(GetAllQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetAllQuote(parameter);

                var response = new GetAllQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QuoteList = new List<QuoteModel>()
                };
                result.QuoteList.ForEach(or =>
                {
                    response.QuoteList.Add(new QuoteModel(or));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception ex)
            {
                return new GetAllQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetQuoteByIDResponse GetQuoteByID(GetQuoteByIDRequest request)
        {
            try
            {
                this.logger.LogInformation("GetQuoteByID");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetQuoteByID(parameter);
                var response = new GetQuoteByIDResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QuoteEntityObject = new Models.Quote.QuoteModel(result.QuoteEntityObject),
                    ListQuoteDetail = new List<Models.Quote.QuoteDetailModel>(),
                    ListQuoteDocument =new List<QuoteDocumentModel>(),
                };
                result.ListQuoteDetail.ForEach(cod =>
                {
                    QuoteDetailModel a = new QuoteDetailModel(cod);

                    List<QuoteProductDetailProductAttributeValueModel> lstQuoteProductDetailProductAttributeValueModel = new List<QuoteProductDetailProductAttributeValueModel>();
                    if (cod.QuoteProductDetailProductAttributeValue != null)
                    {
                        cod.QuoteProductDetailProductAttributeValue.ForEach(X1 =>
                        {
                            lstQuoteProductDetailProductAttributeValueModel.Add(new QuoteProductDetailProductAttributeValueModel(X1));
                        });
                        a.QuoteProductDetailProductAttributeValue = lstQuoteProductDetailProductAttributeValueModel;
                    }
                    response.ListQuoteDetail.Add(a);
                });
                result.ListQuoteDocument.ForEach(doc =>
                {
                    QuoteDocumentModel a = new QuoteDocumentModel(doc);
                    response.ListQuoteDocument.Add(a);
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception ex)
            {
                return new GetQuoteByIDResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public UpdateQuoteResponse UpdateQuote(UpdateQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.UpdateQuote(parameter);
                var response = new UpdateQuoteResponse
                {
                    QuoteID = result.QuoteID,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;

            }
            catch (Exception ex)
            {
                return new UpdateQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };

            }
        }

        public GetTop3PotentialCustomersResponse GetTop3PotentialCustomers(GetTop3PotentialCustomersRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Top3 Potential Customers");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetTop3PotentialCustomers(parameter);

                var response = new GetTop3PotentialCustomersResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QuoteList = new List<GetTop3PotentialCustomerModel>()
                };
                result.PotentialCustomersList.ForEach(item =>
                {
                    response.QuoteList.Add(new GetTop3PotentialCustomerModel(item));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception ex)
            {
                return new GetTop3PotentialCustomersResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetTotalAmountQuoteResponse GetTotalAmountQuote(GetTotalAmountQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Total Amount Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetTotalAmountQuote(parameter);

                var response = new GetTotalAmountQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ToTalAmount = new GetTotalAmountQuoteeModel()
                };
                
                response.ToTalAmount = new GetTotalAmountQuoteeModel(result.ToTalAmount);

                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception ex)
            {
                return new GetTotalAmountQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetDashBoardQuoteResponse GetDashBoardQuote(GetDashBoardQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Dash Board Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetDashBoardQuote(parameter);

                var response = new GetDashBoardQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    DashBoardQuote = new GetDashBoardQuoteeModel()
                };

                response.DashBoardQuote = new GetDashBoardQuoteeModel(result.DashBoardQuote);

                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception ex)
            {
                return new GetDashBoardQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public UpdateActiveQuoteResponse UpdateActiveQuote(UpdateActiveQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Update Active Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.UpdateActiveQuote(parameter);

                var response = new UpdateActiveQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                
                return response;
            }
            catch (Exception ex)
            {
                return new UpdateActiveQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Quote.DELETE_QUOTE_FAIL
                };
            }
        }

        public GetDataQuoteToPieChartResponse GetDataQuoteToPieChart(GetDataQuoteToPieChartRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Data Quote To Pie Chart");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetDataQuoteToPieChart(parameter);
                var response = new GetDataQuoteToPieChartResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    CategoriesPieChart = result.Status ? result.CategoriesPieChart : new List<string>(),
                    DataPieChart = result.Status ? result.DataPieChart : new List<decimal>()
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataQuoteToPieChartResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchQuoteResponse SearchQuote(SearchQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.SearchQuote(parameter);

                var response = new SearchQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ListQuote = new List<QuoteModel>(),
                    ListStatus = new List<CategoryModel>()
                };
                result.ListQuote.ForEach(or =>
                {
                    response.ListQuote.Add(new QuoteModel(or));
                });
                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new SearchQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCreateUpdateQuoteResponse GetDataCreateUpdateQuote(GetDataCreateUpdateQuoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetDataCreateUpdateQuote(parameter);

                var response = new GetDataCreateUpdateQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ListCustomerAll = new List<CustomerModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListCustomerNew = new List<CustomerModel>(),
                    ListLead = new List<LeadModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    ListPaymentMethod = new List<CategoryModel>(),
                    ListQuoteStatus = new List<CategoryModel>(),
                    Quote = new QuoteModel(result.Quote),
                    ListQuoteDetail = new List<QuoteDetailModel>(),
                    ListQuoteCostDetail = new List<QuoteCostDetailModel>(),
                    ListQuoteDocument = new List<QuoteDocumentModel>(),
                    ListAdditionalInformation = new List<AdditionalInformationModel>(),
                    ListAdditionalInformationTemplates = new List<CategoryModel>(),
                    ListNote = new List<NoteModel>(),
                    IsAprovalQuote = result.IsAprovalQuote,
                    ListInvestFund = result.ListInvestFund,
                    ListSaleBidding = new List<SaleBiddingModel>(),
                    ListProductVendorMapping = result.ListProductVendorMapping,
                    ListParticipant = result.ListParticipant,
                    ListParticipantId = result.ListParticipantId
                };

                result.ListCustomerAll.ForEach(item =>
                {
                    response.ListCustomerAll.Add(new CustomerModel(item));
                });

                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                result.ListCustomerNew.ForEach(item =>
                {
                    response.ListCustomerNew.Add(new CustomerModel(item));
                });

                result.ListLead.ForEach(item =>
                {
                    var obj = new LeadModel(item);
                    obj.ListLeadDetail = item.ListLeadDetail;
                    response.ListLead.Add(obj);
                });

                result.ListSaleBidding.ForEach(item =>
                {
                    var obj = new SaleBiddingModel(item);
                    obj.SaleBiddingDetail = item.SaleBiddingDetail;
                    response.ListSaleBidding.Add(obj);
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListPaymentMethod.ForEach(item =>
                {
                    response.ListPaymentMethod.Add(new CategoryModel(item));
                });

                result.ListQuoteStatus.ForEach(item =>
                {
                    response.ListQuoteStatus.Add(new CategoryModel(item));
                });

                result.ListAdditionalInformationTemplates.ForEach(item =>
                {
                    response.ListAdditionalInformationTemplates.Add(new CategoryModel(item));
                });

                result.ListQuoteCostDetail.ForEach(item =>
                {
                    response.ListQuoteCostDetail.Add(new QuoteCostDetailModel(item));
                });

                result.ListQuoteDetail.ForEach(cod =>
                {
                    QuoteDetailModel a = new QuoteDetailModel(cod);

                    List<QuoteProductDetailProductAttributeValueModel> lstQuoteProductDetailProductAttributeValueModel = new List<QuoteProductDetailProductAttributeValueModel>();
                    if (cod.QuoteProductDetailProductAttributeValue != null)
                    {
                        cod.QuoteProductDetailProductAttributeValue.ForEach(X1 =>
                        {
                            lstQuoteProductDetailProductAttributeValueModel.Add(new QuoteProductDetailProductAttributeValueModel(X1));
                        });
                        a.QuoteProductDetailProductAttributeValue = lstQuoteProductDetailProductAttributeValueModel;
                    }
                    response.ListQuoteDetail.Add(a);
                });

                result.ListQuoteDocument.ForEach(doc =>
                {
                    QuoteDocumentModel a = new QuoteDocumentModel(doc);
                    response.ListQuoteDocument.Add(a);
                });

                result.ListAdditionalInformation.ForEach(item =>
                {
                    response.ListAdditionalInformation.Add(new AdditionalInformationModel(item));
                });

                result.ListNote.ForEach(item =>
                {
                    NoteModel temp = new NoteModel(item);
                    response.ListNote.Add(temp);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataCreateUpdateQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataQuoteAddEditProductDialogResponse GetDataQuoteAddEditProductDialog(
            GetDataQuoteAddEditProductDialogRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Data Quote Add Edit Product Dialog");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetDataQuoteAddEditProductDialog(parameter);

                var response = new GetDataQuoteAddEditProductDialogResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListUnitMoney = new List<CategoryModel>(),
                    ListUnitProduct = new List<CategoryModel>(),
                    ListVendor = new List<VendorModel>(),
                    ListProduct = new List<ProductModel>(),
                    ListPriceProduct = new List<PriceProductModel>()
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
                    response.ListVendor.Add(new VendorModel(vendor));
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });
                result.ListPriceProduct.ForEach(item =>
                {
                    response.ListPriceProduct.Add(new PriceProductModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataQuoteAddEditProductDialogResponse()
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
                var result = iQuoteDataAccess.GetVendorByProductId(parameter);

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
        public GetDataExportExcelQuoteResponse GetDataExportExcelQuote(GetDataExportExcelQuoteRequest request)
        {
            try
            {
                //this.logger.LogInformation("Export Excel Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetDataExportExcelQuote(parameter);

                var response = new GetDataExportExcelQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    InforExportExcel = new InforExportExcelFactoryModel(),
                    Quote = new Models.Quote.QuoteModel(result.Quote),
                    ListQuoteDetail = new List<Models.Quote.QuoteDetailModel>(),
                    ListAdditionalInformation = new List<AdditionalInformationModel>()
                };

                result.ListQuoteDetail.ForEach(cod =>
                {
                    QuoteDetailModel a = new QuoteDetailModel(cod);

                    List<QuoteProductDetailProductAttributeValueModel> lstQuoteProductDetailProductAttributeValueModel = new List<QuoteProductDetailProductAttributeValueModel>();
                    if (cod.QuoteProductDetailProductAttributeValue != null)
                    {
                        cod.QuoteProductDetailProductAttributeValue.ForEach(X1 =>
                        {
                            lstQuoteProductDetailProductAttributeValueModel.Add(new QuoteProductDetailProductAttributeValueModel(X1));
                        });
                        a.QuoteProductDetailProductAttributeValue = lstQuoteProductDetailProductAttributeValueModel;
                    }
                    response.ListQuoteDetail.Add(a);
                });

                result.ListAdditionalInformation.ForEach(item =>
                {
                    response.ListAdditionalInformation.Add(new AdditionalInformationModel(item));
                });
                response.InforExportExcel.CompanyName = result.InforExportExcel.CompanyName;
                response.InforExportExcel.Address = result.InforExportExcel.Address;
                response.InforExportExcel.Phone = result.InforExportExcel.Phone;
                response.InforExportExcel.Website = result.InforExportExcel.Website;
                response.InforExportExcel.Email = result.InforExportExcel.Email;
                response.InforExportExcel.TextTotalMoney = result.InforExportExcel.TextTotalMoney;

                return response;
            }
            catch (Exception e)
            {
                return new GetDataExportExcelQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetEmployeeSaleResponse GetEmployeeSale(GetEmployeeSaleRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetEmployeeSale(parameter);

                var response = new GetEmployeeSaleResponse()
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
                return new GetEmployeeSaleResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DownloadTemplateProductResponse DownloadTemplateProduct(DownloadTemplateProductRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Vendor By ProductId");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.DownloadTemplateProduct(parameter);

                var response = new DownloadTemplateProductResponse()
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
                return new DownloadTemplateProductResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        public CreateCostResponse CreateCost(CreateCostRequest request)
        {
            try
            {
                logger.LogInformation("Create Cost");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.CreateCost(parameter);

                var response = new CreateCostResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCost = new List<Models.Cost.CostModel>(),
                };

                result.ListCost.ForEach(item =>
                {
                    response.ListCost.Add(new Models.Cost.CostModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new CreateCostResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataCreateCostResponse GetMasterDataCreateCost(GetMasterDataCreateCostRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetMasterDataCreateCost(parameter);

                var response = new GetMasterDataCreateCostResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListStatus = new List<Models.Category.CategoryModel>(),
                    ListCost = new List<Models.Cost.CostModel>()
                };

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new Models.Category.CategoryModel(item));
                });

                result.ListCost.ForEach(item =>
                {
                    response.ListCost.Add(new Models.Cost.CostModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateCostResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message
                };
            }
        }

        public UpdateCostResponse UpdateCost(UpdateCostRequest request)
        {
            try
            {
                logger.LogInformation("Update Cost");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.UpdateCost(parameter);
                var response = new UpdateCostResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCost = new List<Models.Cost.CostModel>()
                };

                result.ListCost.ForEach(item =>
                {
                    response.ListCost.Add(new Models.Cost.CostModel(item));
                });

                return response;
            }catch(Exception ex)
            {
                return new UpdateCostResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message,
                };
            }
        }

        public UpdateQuoteResponse UpdateStatusQuote(GetQuoteByIDRequest request)
        {
            try
            {
                logger.LogInformation("Update Status Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.UpdateStatusQuote(parameter);
                var response = new UpdateQuoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };
                
                return response;
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message,
                };
            }
        }

        public SearchQuoteResponse SearchQuoteAprroval(SearchQuoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Quote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.SearchQuoteAprroval(parameter);

                var response = new SearchQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ListQuote = new List<QuoteModel>(),
                    ListStatus = new List<CategoryModel>()
                };
                result.ListQuote.ForEach(or =>
                {
                    response.ListQuote.Add(new QuoteModel(or));
                });
                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new SearchQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateQuoteResponse ApprovalOrRejectQuote(ApprovalOrRejectQuoteRequest request)
        {
            try
            {
                logger.LogInformation("ApprovalOrRejectQuote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.ApprovalOrRejectQuote(parameter);
                var response = new UpdateQuoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message,
                };
            }
        }

        public UpdateQuoteResponse SendEmailCustomerQuote(SendEmailCustomerQuoteRequest request)
        {
            try
            {
                logger.LogInformation("SendEmailCustomerQuote");
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.SendEmailCustomerQuote(parameter);
                var response = new UpdateQuoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new UpdateQuoteResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = ex.Message,
                };
            }
        }

        public GetMasterDataCreateQuoteResponse GetMasterDataCreateQuote(GetMasterDataCreateQuoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetMasterDataCreateQuote(parameter);
                var response = new GetMasterDataCreateQuoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListInvestFund = result.ListInvestFund,
                    ListAdditionalInformationTemplates = result.ListAdditionalInformationTemplates,
                    ListPaymentMethod = result.ListPaymentMethod,
                    ListQuoteStatus = result.ListQuoteStatus,
                    ListEmployee = result.ListEmployee,
                    ListCustomer = result.ListCustomer,
                    ListCustomerNew = result.ListCustomerNew,
                    ListAllLead = result.ListAllLead,
                    ListAllSaleBidding = result.ListAllSaleBidding,
                    ListParticipant = result.ListParticipant
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message,
                };
            }
        }

        public GetEmployeeByPersonInChargeResponse GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetEmployeeByPersonInCharge(parameter);
                var response = new GetEmployeeByPersonInChargeResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmployee = result.ListEmployee,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetEmployeeByPersonInChargeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message,
                };
            }
        }

        public GetMasterDataUpdateQuoteResponse GetMasterDataUpdateQuote(GetMasterDataUpdateQuoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iQuoteDataAccess.GetMasterDataUpdateQuote(parameter);
                var response = new GetMasterDataUpdateQuoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListInvestFund = result.ListInvestFund,
                    ListPaymentMethod = result.ListPaymentMethod,
                    ListQuoteStatus = result.ListQuoteStatus,
                    ListEmployee = result.ListEmployee,
                    ListCustomer = result.ListCustomer,
                    ListCustomerNew = result.ListCustomerNew,
                    ListAllLead = result.ListAllLead,
                    ListAllSaleBidding = result.ListAllSaleBidding,
                    ListParticipant = result.ListParticipant,
                    Quote = result.Quote,
                    ListQuoteDetail = result.ListQuoteDetail,
                    ListQuoteDocument = result.ListQuoteDocument,
                    ListAdditionalInformation = result.ListAdditionalInformation,
                    ListNote = result.ListNote,
                    ListQuoteCostDetail = result.ListQuoteCostDetail,
                    IsApproval = result.IsApproval,
                    ListParticipantId = result.ListParticipantId,
                    IsParticipant = result.IsParticipant,
                    ListPromotionObjectApply = result.ListPromotionObjectApply
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataUpdateQuoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message,
                };
            }
        }
    }
}
