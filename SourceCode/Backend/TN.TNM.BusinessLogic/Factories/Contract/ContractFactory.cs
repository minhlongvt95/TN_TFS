using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Interfaces.Contract;
using TN.TNM.BusinessLogic.Messages.Requests.Contract;
using TN.TNM.BusinessLogic.Messages.Responses.Contract;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.BusinessLogic.Factories.Contract
{
    public class ContractFactory : BaseFactory, IContract
    {
        private IContractDataAccess iContractDataAccess;

        public ContractFactory(IContractDataAccess _iContractDataAccess, ILogger<ContractFactory> _logger)
        {
            this.iContractDataAccess = _iContractDataAccess;
            this.logger = _logger;
        }

        public ChangeContractStatusResponse ChangeContractStatus(ChangeContractStatusRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.ChangeContractStatus(parameter);
                var response = new ChangeContractStatusResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception ex)
            {
                return new ChangeContractStatusResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public UploadFileResponse UploadFile(UploadFileRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.UploadFile(parameter);

                var response = new UploadFileResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListFileInFolder = new List<FileInFolderModel>()
                };

                result.ListFileInFolder.ForEach(item =>
                {
                    response.ListFileInFolder.Add(new FileInFolderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new UploadFileResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteFileResponse DeleteFile(DeleteFileRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.DeleteFile(parameter);

                var response = new DeleteFileResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteFileResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateContractRespone CreateOrUpdateContract(CreateOrUpdateContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.CreateOrUpdateContract(parameter);
                var response = new CreateOrUpdateContractRespone
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ContractId = result.ContractId
                };

                return response;
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateContractRespone
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public CreateCloneContractResponse CreateCloneContract(CreateCloneContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.CreateCloneContract(parameter);
                var response = new CreateCloneContractResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ContractId = result.ContractId,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new CreateCloneContractResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }


        public GetMasterDataContractResponse GetMasterDataContract(GetMasterDataContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.GetMasterDataContract(parameter);
                var response = new GetMasterDataContractResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListCustomer = new List<Models.Customer.CustomerModel>(),
                    ListPaymentMethod = new List<Models.Category.CategoryModel>(),
                    ListTypeContract = new List<Models.Category.CategoryModel>(),
                    ListQuote = new List<Models.Quote.QuoteModel>(),
                    ListQuoteDetail = new List<Models.Quote.QuoteDetailModel>(),
                    ListQuoteCostDetail = new List<QuoteCostDetailModel>(),

                    ListEmployeeSeller = new List<Models.Employee.EmployeeModel>(),
                    ListContractStatus = new List<Models.Category.CategoryModel>(),
                    ListBankAccount = new List<Models.BankAccount.BankAccountModel>(),
                    //ListContract = new List<Models.Contract.ContractModel>(),
                    Contract = new Models.Contract.ContractModel(),
                    ListContractDetail = new List<Models.Contract.ContractDetailModel>(),
                    //ListContractDetailProductAttribute = new List<Models.Contract.ContractDetailProductAttributeModel>(),
                    ListAdditionalInformation = new List<Models.Quote.AdditionalInformationModel>(),
                    ListNote = new List<Models.Note.NoteModel>(),
                    ListContractCost = new List<ContractCostDetailModel>(),
                    ListFile = new List<Models.Folder.FileInFolderModel>(),
                    ListFormFile = result.ListFormFile,
                    ListContract = result.ListContract,
                    IsOutOfDate = result.IsOutOfDate,
                };

                result.ListBankAccount.ForEach(item =>
                {
                    response.ListBankAccount.Add(new Models.BankAccount.BankAccountModel(item));
                });
                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new Models.Customer.CustomerModel(item));
                });
                result.ListPaymentMethod.ForEach(item =>
                {
                    response.ListPaymentMethod.Add(new Models.Category.CategoryModel(item));
                });
                result.ListTypeContract.ForEach(item =>
                {
                    response.ListTypeContract.Add(new Models.Category.CategoryModel(item));
                });
                result.ListQuote.ForEach(item =>
                {
                    response.ListQuote.Add(new Models.Quote.QuoteModel(item));
                });
                result.ListEmployeeSeller.ForEach(item =>
                {
                    response.ListEmployeeSeller.Add(new Models.Employee.EmployeeModel(item));
                });
                result.ListContractStatus.ForEach(item =>
                {
                    response.ListContractStatus.Add(new Models.Category.CategoryModel(item));
                });
                response.Contract = new Models.Contract.ContractModel(result.Contract);
                result.ListContractDetail.ForEach(item =>
                {
                    var contractDetailModel = new Models.Contract.ContractDetailModel(item);
                    var listContractDetailAttribute = new List<ContractDetailProductAttributeModel>();

                    if (item.ContractProductDetailProductAttributeValue != null)
                    {
                        item.ContractProductDetailProductAttributeValue.ForEach(itemAttribute =>
                        {
                            var contractDetailAttributeModel = new ContractDetailProductAttributeModel(itemAttribute);
                            listContractDetailAttribute.Add(contractDetailAttributeModel);
                        });
                    }
                    
                    contractDetailModel.ContractProductDetailProductAttributeValue = listContractDetailAttribute;
                    response.ListContractDetail.Add(contractDetailModel);

                });
                result.ListAdditionalInformation.ForEach(item =>
                {
                    response.ListAdditionalInformation.Add(new Models.Quote.AdditionalInformationModel(item));
                });
                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new Models.Note.NoteModel(item));
                });
                //result.ListContract.ForEach(item =>
                //{
                //    response.ListContract.Add(new Models.Contract.ContractModel(item));
                //});

                result.ListContractCost.ForEach(item =>
                {
                    response.ListContractCost.Add(new ContractCostDetailModel(item));
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

                result.ListFile.ForEach(item =>
                {
                    response.ListFile.Add(new Models.Folder.FileInFolderModel(item));
                });
                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataContractResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetListMainContractResponses GetListMainContract(GetListMainContractRequests request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.GetListMainContract(parameter);
                var response = new GetListMainContractResponses
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListContract = new List<Models.Contract.ContractModel>(),
                };
                result.ListContract.ForEach(item =>
                {
                    response.ListContract.Add(new Models.Contract.ContractModel(item));
                });
                return response;
            }
            catch (Exception ex)
            {
                return new GetListMainContractResponses
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataDashboardContractResponse GetMasterDataDashboardContract(GetMasterDataDashboardContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.GetMasterDataDashBoard(parameter);
                var response = new GetMasterDataDashboardContractResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListContractNew = new List<ContractModel>(),
                    ListContractWorking = new List<ContractModel>(),
                    ListContractExpiredDate = new List<ContractModel>(),
                    ListValueOfStatus = result.ListValueOfStatus,
                    ListValueOfMonth = result.ListValueOfMonth,
                };
                result.ListContractNew.ForEach(item =>
                {
                    response.ListContractNew.Add(new ContractModel(item));
                });
                result.ListContractWorking.ForEach(item =>
                {
                    response.ListContractWorking.Add(new ContractModel(item));
                });
                result.ListContractExpiredDate.ForEach(item =>
                {
                    response.ListContractExpiredDate.Add(new ContractModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataDashboardContractResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataSearchContractResponse GetMasterDataSearchContract(GetMasterDataSearchContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.GetMasterDataSearchContract(parameter);
                var response = new GetMasterDataSearchContractResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListCustomer = new List<Models.Customer.CustomerModel>(),
                    ListEmployee = new List<Models.Employee.EmployeeModel>(),
                    ListProduct = new List<Models.Product.ProductModel>(),
                    ListStatus = new List<Models.Category.CategoryModel>(),
                    ListTypeContract = new List<Models.Category.CategoryModel>()
                };

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new Models.Category.CategoryModel(item));
                });
                result.ListCutomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new Models.Customer.CustomerModel(item));
                });
                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new Models.Employee.EmployeeModel(item));
                });
                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new Models.Product.ProductModel(item));
                });
                result.ListTypeContract.ForEach(item =>
                {
                    response.ListTypeContract.Add(new Models.Category.CategoryModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchContractResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
        public SearchContractResponse SearchContract(SearchContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.SearchContract(parameter);
                var response = new SearchContractResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListContract = new List<ContractModel>(),
                };
                result.ListContract.ForEach(item =>
                {
                    response.ListContract.Add(new ContractModel(item));
                });

                return response;
            }
            catch (Exception ex)
            {
                return new SearchContractResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

        public DeleteContractResponse DeleteContract(DeleteContractRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iContractDataAccess.DeleteContract(parameter);
                var response = new DeleteContractResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                return new DeleteContractResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
    }
}
