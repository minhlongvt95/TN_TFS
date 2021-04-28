using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.BillSale;
using TN.TNM.BusinessLogic.Messages.Requests.BillSale;
using TN.TNM.BusinessLogic.Messages.Responses.BillSale;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.BillSale;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.WareHouse;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.BillSale
{
    public class BillSaleFactory : BaseFactory, IBillSale
    {
        private IBillSaleDataAccess _iBillSaleDataAccess;

        public BillSaleFactory(IBillSaleDataAccess iBillSaleDataAccess, ILogger<BillSaleFactory> _logger)
        {
            _iBillSaleDataAccess = iBillSaleDataAccess;
            this.logger = _logger;
        }

        public AddOrEditBillSaleResponse AddOrEditBillSale(AddOrEditBillSaleRequest request)
        {
            try
            {
                logger.LogInformation("Create bill of sale");
                var parameter = request.ToParameter();
                var result = _iBillSaleDataAccess.AddOrEditBillSale(parameter);
                var response = new AddOrEditBillSaleResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    BillSaleId = result.BillSaleId
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new AddOrEditBillSaleResponse()
                {
                    MessageCode = CommonMessage.BillSale.CREATE_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterBillOfSaleResponse GetMasterBillOfSale(GetMasterBillOfSaleRequest request)
        {
            try
            {
                logger.LogInformation("Get master bill of sale");
                var parameter = request.ToParameter();
                var result = _iBillSaleDataAccess.GetMasterBillOfSale(parameter);
                var response = new GetMasterBillOfSaleResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProduct = new List<ProductModel>(),
                    ListStatus = new List<CategoryModel>()
                };
                result.ListProduct.ForEach(item =>
                {
                    ProductModel obj = new ProductModel(item);
                    response.ListProduct.Add(obj);
                });
                result.ListStatus.ForEach(item =>
                {
                    CategoryModel obj = new CategoryModel(item);
                    response.ListStatus.Add(obj);
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetMasterBillOfSaleResponse()
                {
                    MessageCode = CommonMessage.BillSale.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataBillSaleCreateEditResponse GetMasterDataBillSaleCreateEdit(GetMasterDataBillSaleCreateEditRequest request)
        {
            try
            {
                logger.LogInformation("Get bill of sale");
                var parameter = request.ToParameter();
                var result = _iBillSaleDataAccess.GetMasterDataBillSaleCreateEdit(parameter);
                var response = new GetMasterDataBillSaleCreateEditResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    BillSale = result.BillSale == null ? null : new BillSaleModel(result.BillSale),
                    ListBanking = new List<CategoryModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListNote = new List<NoteModel>(),
                    Order = result.Order == null ? null : new OrderBillModel(result.Order),
                    ListOrder = new List<CustomerOrderModel>(),
                    ListInventoryDeliveryVoucher = new List<Models.WareHouse.InventoryDeliveryVoucherModel>(),
                    ListMoney = new List<CategoryModel>()
                };
                if (response.BillSale != null)
                {
                    response.BillSale.ListBillSaleDetail = new List<BillSaleDetailModel>();
                    response.BillSale.ListCost = new List<BillSaleCostModel>();

                    result.BillSale?.ListCost?.ForEach(item =>
                    {
                        response.BillSale.ListCost.Add(new BillSaleCostModel(item));
                    });

                    result.BillSale?.ListBillSaleDetail?.ForEach(item =>
                    {
                        var temp = new BillSaleDetailModel(item);
                        temp.ListBillSaleDetailProductAttribute = new List<BillSaleDetailProductAttributeModel>();
                        item.ListBillSaleDetailProductAttribute?.ForEach(tem =>
                        {
                            temp.ListBillSaleDetailProductAttribute.Add(new BillSaleDetailProductAttributeModel(tem));
                        });

                        response.BillSale?.ListBillSaleDetail.Add(temp);
                    });
                }

                result.ListNote?.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                result.ListBanking?.ForEach(item =>
                {
                    response.ListBanking.Add(new CategoryModel(item));
                });

                result.ListMoney?.ForEach(item =>
                {
                    response.ListMoney.Add(new CategoryModel(item));
                });

                result.ListCustomer?.ForEach(item =>
                {
                    var customer = new CustomerModel(item);
                    customer.ListBankAccount = new List<BankAccountModel>();
                    item.ListBankAccount?.ForEach(acc =>
                    {
                        customer.ListBankAccount.Add(new BankAccountModel(acc));
                    });
                    response.ListCustomer.Add(customer);
                });

                result.ListEmployee?.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListStatus?.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.ListOrder?.ForEach(item =>
                {
                    response.ListOrder.Add(new CustomerOrderModel(item));
                });

                result.ListInventoryDeliveryVoucher?.ForEach(item =>
                {
                    response.ListInventoryDeliveryVoucher.Add(new InventoryDeliveryVoucherModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetMasterDataBillSaleCreateEditResponse()
                {
                    MessageCode = CommonMessage.BillSale.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchBillOfSaleResponse SearchBillOfSale(SearchBillOfSaleRequest request)
        {
            try
            {
                logger.LogInformation("Search bill of sale");
                var parameter = request.ToParameter();
                var result = _iBillSaleDataAccess.SearchBillOfSale(parameter);
                var response = new SearchBillOfSaleResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListBillOfSale = new List<BillSaleModel>()
                };
                result.ListBillOfSale.ForEach(item =>
                {
                    BillSaleModel obj = new BillSaleModel(item);
                    response.ListBillOfSale.Add(obj);
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new SearchBillOfSaleResponse()
                {
                    MessageCode = CommonMessage.BillSale.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetOrderByOrderIdResponse GetOrderByOrderId(GetOrderByOrderIdRequest request)
        {
            try
            {
                logger.LogInformation("Get bill of sale order");
                var parameter = request.ToParameter();
                var result = _iBillSaleDataAccess.GetOrderByOrderId(parameter);
                var response = new GetOrderByOrderIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Order = result.Order == null?null:new OrderBillModel(result.Order),
                    ListBillSaleDetail = new List<BillSaleDetailModel>(),
                    ListCost = new List<BillSaleCostModel>(),
                    ListInventoryDeliveryVoucher = new List<InventoryDeliveryVoucherModel>()
                };

                result.ListInventoryDeliveryVoucher?.ForEach(item =>
                {
                    response.ListInventoryDeliveryVoucher.Add(new InventoryDeliveryVoucherModel(item));
                });

                result.ListCost?.ForEach(item =>
                {
                    response.ListCost.Add(new BillSaleCostModel(item));
                });

                result.ListBillSaleDetail?.ForEach(item =>
                {
                    response.ListBillSaleDetail.Add(new BillSaleDetailModel(item));
                });


                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetOrderByOrderIdResponse()
                {
                    MessageCode = CommonMessage.BillSale.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateStatusResponse UpdateStatus(UpdateStatusRequest request)
        {
            try
            {
                logger.LogInformation("Get bill of sale order");
                var parameter = request.ToParameter();
                var result = _iBillSaleDataAccess.UpdateStatus(parameter);
                var response = new UpdateStatusResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Note = result.Note == null?null: new NoteModel(result.Note)
                };
                
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new UpdateStatusResponse()
                {
                    MessageCode = CommonMessage.BillSale.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public DeleteBillSaleResponse DeleteBillSale(DeleteBillSaleRequest request)
        {
            try
            {
                logger.LogInformation("Delete bill of sale order");
                var parameter = request.ToParameter();
                var result = _iBillSaleDataAccess.DeleteBillSale(parameter);
                var response = new DeleteBillSaleResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DeleteBillSaleResponse()
                {
                    MessageCode = CommonMessage.BillSale.GET_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
