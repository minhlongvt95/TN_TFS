using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.WareHouse;
using TN.TNM.BusinessLogic.Messages.Requests.WareHouse;
using TN.TNM.BusinessLogic.Messages.Responses.WareHouse;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.BusinessLogic.Models.WareHouse;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Factories.WareHouse
{
    public class WareHouseFactory : BaseFactory, IWareHouse
    {
        private IWareHouseDataAccess iWareHouseDataAccess;
        public WareHouseFactory(IWareHouseDataAccess _iWareHouseDataAccess, ILogger<WareHouseFactory> _logger)
        {
            this.iWareHouseDataAccess = _iWareHouseDataAccess;
            this.logger = _logger;
        }

        public CreateOrUpdateInventoryVoucherResponse CreateOrUpdateInventoryVoucher(CreateOrUpdateInventoryVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("Create/Update CreateOrUpdateInventoryVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CreateOrUpdateInventoryVoucher(parameter);
                var response = new CreateOrUpdateInventoryVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    InventoryReceivingVoucherId = result.InventoryReceivingVoucherId
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError("CreateOrUpdateInventoryVoucher:"+e.ToString());
                return new CreateOrUpdateInventoryVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
        public CreateUpdateWareHouseResponse CreateUpdateWareHouse(CreateUpdateWareHouseRequest request)
        {
            try
            {
                this.logger.LogInformation("Create/Update WareHouse");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CreateUpdateWareHouse(parameter);
                var response = new CreateUpdateWareHouseResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    WarehouseId = result.Status ? result.WarehouseId : Guid.Empty
                };
                return response;
            }
            catch (Exception e)
            {
                return new CreateUpdateWareHouseResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DownloadTemplateSerialResponse DownloadTemplateSerial(DownloadTemplateSerialRequest request)
        {
            try
            {
                this.logger.LogInformation("DownloadTemplateSerial");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.DownloadTemplateSerial(parameter);
                var response = new DownloadTemplateSerialResponse()
                {
                    ExcelFile = result.ExcelFile,
                    NameFile = result.NameFile,
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new DownloadTemplateSerialResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetVendorOrderByVendorIdResponse GetVendorOrderByVendorId(GetVendorOrderByVendorIdRequest request)
        {
            try
            {
                this.logger.LogInformation("GetVendorOrderByVendorId");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetVendorOrderByVendorId(parameter);
                var response = new GetVendorOrderByVendorIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListVendorOrder = result.ListVendorOrder
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetVendorOrderByVendorIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetVendorOrderDetailByVenderOrderIdResponse GetVendorOrderDetailByVenderOrderId(GetVendorOrderDetailByVenderOrderIdRequest request)
        {
            try
            {
                this.logger.LogInformation("GetVendorOrderDetailByVenderOrderId");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetVendorOrderDetailByVenderOrderId(parameter);
                var response = new GetVendorOrderDetailByVenderOrderIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListOrderProduct = new List<GetVendorOrderDetailByVenderOrderIdModel>()
                };

                result.ListOrderProduct.ForEach(item =>
                {
                    response.ListOrderProduct.Add(new GetVendorOrderDetailByVenderOrderIdModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetVendorOrderDetailByVenderOrderIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetWareHouseChaResponse GetWareHouseCha(GetWareHouseChaRequest request)
        {
            try
            {
                this.logger.LogInformation("GetWareHouseCha");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetWareHouseCha(parameter);
                var response = new GetWareHouseChaResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    listWareHouse = new List<WareHouseModel>()
                };

                result.listWareHouse.ForEach(item =>
                {
                    response.listWareHouse.Add(new WareHouseModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetWareHouseChaResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchWareHouseResponse SearchWareHouse(SearchWareHouseRequest request)
        {
            try
            {
                this.logger.LogInformation("Search WareHouse");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.SearchWareHouse(parameter);
                var response = new SearchWareHouseResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    listWareHouse = new List<WareHouseModel>()
                };

                result.listWareHouse.ForEach(item =>
                {
                    response.listWareHouse.Add(new WareHouseModel(item));
                });
           
                return response;
            }
            catch (Exception e)
            {
                return new SearchWareHouseResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public RemoveWareHouseResponse RemoveWareHouse(RemoveWareHouseRequest request)
        {
            try
            {
                this.logger.LogInformation("Remove WareHouse");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.RemoveWareHouse(parameter);
                var response = new RemoveWareHouseResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    WarehouseId = result.Status ? result.WareHouseId : Guid.Empty
                };
                return response;
            }
            catch (Exception e)
            {
                return new RemoveWareHouseResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetInventoryReceivingVoucherByIdResponse GetInventoryReceivingVoucherById(GetInventoryReceivingVoucherByIdRequest request)
        {
            try
            {

                this.logger.LogInformation("Remove WareHouse");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetInventoryReceivingVoucherById(parameter);
                var response = new GetInventoryReceivingVoucherByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    inventoryReceivingVoucher=new InventoryReceivingVoucherSearchModel(result.inventoryReceivingVoucher),
                    inventoryReceivingVoucherMapping=new List<GetVendorOrderDetailByVenderOrderIdModel>(),
                    listVendorOrder=new List<VendorOrderModel>(),
                    listCustomerOrder=new List<CustomerOrderModel>(),
                    SelectVendor=new VendorModel(result.SelectVendor),
                    SelectCustomer=new Models.Customer.CustomerModel(result.SelectCustomer),
                };
                result.inventoryReceivingVoucherMapping.ForEach(item => {
                    var itemPush = new GetVendorOrderDetailByVenderOrderIdModel(item);
                    itemPush.ListSerial = new List<DataAccess.Databases.Entities.Serial>();
                    if (item.ListSerial.Count > 0)
                    {
                        itemPush.ListSerial.AddRange(item.ListSerial);
                    }
                    response.inventoryReceivingVoucherMapping.Add(itemPush);
                });
                if (result.listVendorOrder.Count > 0)
                {
                    result.listVendorOrder.ForEach(item =>
                    {
                        response.listVendorOrder.Add(new VendorOrderModel(item));
                    });
                }
                if (result.listCustomerOrder.Count > 0)
                {
                    result.listCustomerOrder.ForEach(item =>
                    {
                        response.listCustomerOrder.Add(new CustomerOrderModel(item));
                    });
                }

                return response;
            }
            catch (Exception e)
            {
                return new GetInventoryReceivingVoucherByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListInventoryReceivingVoucherResponse GetListInventoryReceivingVoucher(GetListInventoryReceivingVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("GetListInventoryReceivingVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetListInventoryReceivingVoucher(parameter);
                var response = new GetListInventoryReceivingVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    lstResult=new List<InventoryReceivingVoucherSearchModel>()
                };

                result.lstResult.ForEach(item => {
                    var InventoryReceivingVoucherSearchModel = new InventoryReceivingVoucherSearchModel(item);
                    InventoryReceivingVoucherSearchModel.ListVendorOrder = new List<VendorOrderModel>();
                    if (item.ListVendorOrder != null)
                    {
                        item.ListVendorOrder.ForEach(itemvendor =>
                        {
                            InventoryReceivingVoucherSearchModel.ListVendorOrder.Add(new VendorOrderModel(itemvendor));
                        });
                    }
                    InventoryReceivingVoucherSearchModel.ListCustomerOrder = new List<CustomerOrderModel>();
                    if (item.ListCustomerOrder != null)
                    {
                        item.ListCustomerOrder.ForEach(itemcustomer =>
                        {
                            InventoryReceivingVoucherSearchModel.ListCustomerOrder.Add(new CustomerOrderModel(itemcustomer));
                        });
                    }

                    response.lstResult.Add(InventoryReceivingVoucherSearchModel);
                });
                
                return response;
            }
            catch (Exception e)
            {

                return new GetListInventoryReceivingVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListCustomerOrderByIdCustomerIdResponse GetListCustomerOrderByIdCustomerId(GetListCustomerOrderByIdCustomerIdRequest request)
        {
            try
            {
                this.logger.LogInformation("GetListCustomerOrderByIdCustomerId");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetListCustomerOrderByIdCustomerId(parameter);
                var response = new GetListCustomerOrderByIdCustomerIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    listCustomerOrder = new List<CustomerOrderModel>()
                };

                result.listCustomerOrder.ForEach(item => {
                    response.listCustomerOrder.Add(new CustomerOrderModel(item));
                });

                return response;

            }
            catch (Exception e)
            {
                return new GetListCustomerOrderByIdCustomerIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetCustomerOrderDetailByCustomerOrderIdResponse GetCustomerOrderDetailByCustomerOrderId(GetCustomerOrderDetailByCustomerOrderIdRequest request)
        {
            try
            {
                this.logger.LogInformation("GetCustomerOrderDetailByCustomerOrderId");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetCustomerOrderDetailByCustomerOrderId(parameter);
                var response = new GetCustomerOrderDetailByCustomerOrderIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListOrderProduct = new List<GetVendorOrderDetailByVenderOrderIdModel>()
                };

                result.ListOrderProduct.ForEach(item => {
                    response.ListOrderProduct.Add(new GetVendorOrderDetailByVenderOrderIdModel(item));
                });

                return response;

            }
            catch (Exception e)
            {
                return new GetCustomerOrderDetailByCustomerOrderIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CheckQuantityActualReceivingVoucherResponse CheckQuantityActualReceivingVoucher(CheckQuantityActualReceivingVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("CheckQuantityActualReceivingVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CheckQuantityActualReceivingVoucher(parameter);
                var response = new CheckQuantityActualReceivingVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    IsEnough=result.IsEnough,
                    SumTotalQuantityActual=result.SumTotalQuantityActual
                };

                return response;


            }
            catch (Exception e)
            {
                return new CheckQuantityActualReceivingVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public FilterVendorResponse FilterVendor(FilterVendorRequest request)
        {
            try
            {
                this.logger.LogInformation("FilterVendor");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.FilterVendor(parameter);
                var response = new FilterVendorResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    VendorList=new List<VendorModel>()
                };
                if (result.VendorList.Count > 0)
                {
                    result.VendorList.ForEach(item => {
                        response.VendorList.Add(new VendorModel(item));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new FilterVendorResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public FilterCustomerResponse FilterCustomer(FilterCustomerRequest request)
        {
            try
            {
                this.logger.LogInformation("FilterCustomer");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.FilterCustomer(parameter);
                var response = new FilterCustomerResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    Customer = new List<CustomerModel>()
                };
                if (result.Customer.Count > 0)
                {
                    result.Customer.ForEach(item => {
                        response.Customer.Add(new CustomerModel(item));
                    });
                }
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new FilterCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public ChangeStatusInventoryReceivingVoucherResponse ChangeStatusInventoryReceivingVoucher(ChangeStatusInventoryReceivingVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("ChangeStatusInventoryReceivingVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.ChangeStatusInventoryReceivingVoucher(parameter);
                var response = new ChangeStatusInventoryReceivingVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new ChangeStatusInventoryReceivingVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteInventoryReceivingVoucherResponse DeleteInventoryReceivingVoucher(DeleteInventoryReceivingVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("DeleteInventoryReceivingVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.DeleteInventoryReceivingVoucher(parameter);

                var response = new DeleteInventoryReceivingVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new DeleteInventoryReceivingVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public InventoryDeliveryVoucherFilterVendorOrderResponse InventoryDeliveryVoucherFilterVendorOrder(InventoryDeliveryVoucherFilterVendorOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("InventoryDeliveryVoucherFilterVendorOrder");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.InventoryDeliveryVoucherFilterVendorOrder(parameter);

                var response = new InventoryDeliveryVoucherFilterVendorOrderResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    listVendorOrder=new List<VendorOrderModel>()
                };
                if (result.listVendorOrder.Count > 0)
                {
                    result.listVendorOrder.ForEach(item => {
                        response.listVendorOrder.Add(new VendorOrderModel(item));
                    });
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new InventoryDeliveryVoucherFilterVendorOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public InventoryDeliveryVoucherFilterCustomerOrderResponse InventoryDeliveryVoucherFilterCustomerOrder(InventoryDeliveryVoucherFilterCustomerOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("DeleteInventoryReceivingVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.InventoryDeliveryVoucherFilterCustomerOrder(parameter);

                var response = new InventoryDeliveryVoucherFilterCustomerOrderResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    listCustomerOrder=new List<CustomerOrderModel>()
                };
                if (result.listCustomerOrder.Count > 0)
                {
                    result.listCustomerOrder.ForEach(item => {
                        response.listCustomerOrder.Add(new CustomerOrderModel(item));
                    });
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new InventoryDeliveryVoucherFilterCustomerOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetTop10WarehouseFromReceivingVoucherResponse GetTop10WarehouseFromReceivingVoucher(GetTop10WarehouseFromReceivingVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("GetTop10WarehouseFromReceivingVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetTop10WarehouseFromReceivingVoucher(parameter);

                var response = new GetTop10WarehouseFromReceivingVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    lstInventoryReceivingVoucherMapping = new List<InventoryReceivingVoucherMappingModel>()
                };
                if (result.lstInventoryReceivingVoucherMapping.Count > 0)
                {
                    result.lstInventoryReceivingVoucherMapping.ForEach(item => {
                        response.lstInventoryReceivingVoucherMapping.Add(new InventoryReceivingVoucherMappingModel(item));
                    });
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new GetTop10WarehouseFromReceivingVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetSerialResponse GetSerial(GetSerialRequest request)
        {
            try
            {
                this.logger.LogInformation("GetSerial");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetSerial(parameter);

                var response = new GetSerialResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    lstSerial = new List<SerialModel>()
                };
                if (result.lstSerial.Count > 0)
                {
                    result.lstSerial.ForEach(item => {
                        response.lstSerial.Add(new SerialModel(item));
                    });
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new GetSerialResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateUpdateInventoryDeliveryVoucherResponse CreateUpdateInventoryDeliveryVoucher(CreateUpdateInventoryDeliveryVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("CreateUpdateInventoryDeliveryVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CreateUpdateInventoryDeliveryVoucher(parameter);

                var response = new CreateUpdateInventoryDeliveryVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    InventoryDeliveryVoucherId =result.InventoryDeliveryVoucherId
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new CreateUpdateInventoryDeliveryVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetInventoryDeliveryVoucherByIdResponse GetInventoryDeliveryVoucherById(GetInventoryDeliveryVoucherByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("GetInventoryDeliveryVoucherById");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetInventoryDeliveryVoucherById(parameter);

                var response = new GetInventoryDeliveryVoucherByIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    inventoryDeliveryVoucher=new InventoryDeliveryVoucherModel(result.inventoryDeliveryVoucher),
                    inventoryDeliveryVoucherMappingModel=new List<InventoryDeliveryVoucherMappingModel>()
                };

                if (result.inventoryDeliveryVoucherMappingEntityModel.Count > 0)
                {
                    result.inventoryDeliveryVoucherMappingEntityModel.ForEach(item => {
                        response.inventoryDeliveryVoucherMappingModel.Add(new InventoryDeliveryVoucherMappingModel(item));
                    });
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new GetInventoryDeliveryVoucherByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }

        }

        public DeleteInventoryDeliveryVoucherResponse DeleteInventoryDeliveryVoucher(DeleteInventoryDeliveryVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("DeleteInventoryDeliveryVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.DeleteInventoryDeliveryVoucher(parameter);

                var response = new DeleteInventoryDeliveryVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new DeleteInventoryDeliveryVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }

        }

        public ChangeStatusInventoryDeliveryVoucherResponse ChangeStatusInventoryDeliveryVoucher(ChangeStatusInventoryDeliveryVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("ChangeStatusInventoryDeliveryVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.ChangeStatusInventoryDeliveryVoucher(parameter);

                var response = new ChangeStatusInventoryDeliveryVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new ChangeStatusInventoryDeliveryVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }

        }

        public FilterCustomerInInventoryDeliveryVoucherResponse FilterCustomerInInventoryDeliveryVoucher(FilterCustomerInInventoryDeliveryVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("FilterCustomerInInventoryDeliveryVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.FilterCustomerInInventoryDeliveryVoucher(parameter);

                var response = new FilterCustomerInInventoryDeliveryVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    LstCustomer=new List<CustomerModel>()
                };
                if (result.LstCustomer.Count > 0)
                {
                    result.LstCustomer.ForEach(item => {
                        response.LstCustomer.Add(new CustomerModel(item));
                    });
                };

                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new FilterCustomerInInventoryDeliveryVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchInventoryDeliveryVoucherResponse SearchInventoryDeliveryVoucher(SearchInventoryDeliveryVoucherRequest request)
        {
            try
            {
                this.logger.LogInformation("SearchInventoryDeliveryVoucher");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.SearchInventoryDeliveryVoucher(parameter);

                var response = new SearchInventoryDeliveryVoucherResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    lstResult=new List<InventoryDeliveryVoucherModel>()
                };
                if (result.lstResult.Count > 0)
                {
                    result.lstResult.ForEach(item => {
                        response.lstResult.Add(new InventoryDeliveryVoucherModel(item));
                    });
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new SearchInventoryDeliveryVoucherResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public FilterProductResponse FilterProduct(FilterProductRequest request)
        {
            try
            {
                this.logger.LogInformation("FilterProduct");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.FilterProduct(parameter);

                var response = new FilterProductResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ProductList = new List<ProductEntityModel>()
                };
                if (result.ProductList.Count > 0)
                {
                    result.ProductList.ForEach(item => {
                        response.ProductList.Add(item);
                    });
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new FilterProductResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetProductNameAndProductCodeResponse GetProductNameAndProductCode(GetProductNameAndProductCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("GetProductNameAndProductCode");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetProductNameAndProductCode(parameter);

                var response = new GetProductNameAndProductCodeResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ProductList= new List<ProductEntityModel>()
                };
                if (result.ProductList.Count > 0)
                {
                    result.ProductList.ForEach(item => {
                        response.ProductList.Add(item);
                    });
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new GetProductNameAndProductCodeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetVendorInvenoryReceivingResponse GetVendorInvenoryReceiving(GetVendorInvenoryReceivingRequest request)
        {
            try
            {
                this.logger.LogInformation("GetVendorInvenoryReceiving");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetVendorInvenoryReceiving(parameter);

                var response = new GetVendorInvenoryReceivingResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    VendorList = new List<VendorModel>()
                };
                if (result.VendorList.Count > 0)
                {
                    result.VendorList.ForEach(item => {
                        response.VendorList.Add(new VendorModel(item));
                    });
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new GetVendorInvenoryReceivingResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetCustomerDeliveryResponse GetCustomerDelivery(GetCustomerDeliveryRequest request)
        {
            try
            {
                this.logger.LogInformation("GetVendorInvenoryReceiving");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetCustomerDelivery(parameter);

                var response = new GetCustomerDeliveryResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    Customer = new List<CustomerSearchModel>()
                };
                if (result.LstCustomer.Count > 0)
                {
                    result.LstCustomer.ForEach(item => {
                        response.Customer.Add(new CustomerSearchModel(item));
                    });
                };
                return response;

            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new GetCustomerDeliveryResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public InStockReportResponse InStockReport(InStockReportRequest request)
        {
            try
            {
                this.logger.LogInformation("InStockReport");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.InStockReport(parameter);

                var response = new InStockReportResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    lstResult=new List<InStockModel>()
                };
                if (result.lstResult.Count > 0)
                {
                    result.lstResult.ForEach(item => {
                        response.lstResult.Add(new InStockModel(item));
                    });
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return new InStockReportResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateUpdateWarehouseMasterdataResponse CreateUpdateWarehouseMasterdata(CreateUpdateWarehouseMasterdataRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Update Warehouse Masterdata");
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CreateUpdateWarehouseMasterdata(parameter);
                var response = new CreateUpdateWarehouseMasterdataResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListEmployeeEntityModel = new List<DataAccess.Models.Employee.EmployeeEntityModel>(),
                    ListWarehouseCode = new List<string>(),
                    WarehouseEntityModel = new DataAccess.Databases.Entities.Warehouse()
                };
                response.WarehouseEntityModel = result.WarehouseEntityModel;
                result.ListEmployeeEntityModel.ForEach(e => response.ListEmployeeEntityModel.Add(e));
                result.ListWarehouseCode.ForEach(e => response.ListWarehouseCode.Add(e));
                return response;
            }
            catch (Exception e)
            {
                return new CreateUpdateWarehouseMasterdataResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchInStockReportResponse GetMasterDataSearchInStockReport(
            GetMasterDataSearchInStockReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetMasterDataSearchInStockReport(parameter);
                var response = new GetMasterDataSearchInStockReportResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListProductCategory = result.ListProductCategory,
                    ListWareHouse = result.ListWareHouse
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchInStockReportResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchInStockReportResponse SearchInStockReport(SearchInStockReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.SearchInStockReport(parameter);
                var response = new SearchInStockReportResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListResult = result.ListResult
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchInStockReportResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataPhieuNhapKhoResponse GetMasterDataPhieuNhapKho(GetMasterDataPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetMasterDataPhieuNhapKho(parameter);
                var response = new GetMasterDataPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListVendor = result.ListVendor,
                    ListWarehouse = result.ListWarehouse,
                    ListCustomer = result.ListCustomer,
                    EmployeeCodeName = result.EmployeeCodeName
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDanhSachSanPhamCuaPhieuResponse GetDanhSachSanPhamCuaPhieu(GetDanhSachSanPhamCuaPhieuRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetDanhSachSanPhamCuaPhieu(parameter);
                var response = new GetDanhSachSanPhamCuaPhieuResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListItemDetail = result.ListItemDetail
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDanhSachSanPhamCuaPhieuResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDanhSachKhoConResponse GetDanhSachKhoCon(GetDanhSachKhoConRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetDanhSachKhoCon(parameter);
                var response = new GetDanhSachKhoConResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListWarehouse = result.ListWarehouse
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDanhSachKhoConResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateItemInventoryReportResponse CreateItemInventoryReport(CreateItemInventoryReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CreateItemInventoryReport(parameter);
                var response = new CreateItemInventoryReportResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    InventoryReportId = result.InventoryReportId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateItemInventoryReportResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateItemInventoryReportResponse UpdateItemInventoryReport(UpdateItemInventoryReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.UpdateItemInventoryReport(parameter);
                var response = new UpdateItemInventoryReportResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateItemInventoryReportResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateUpdateSerialResponse CreateUpdateSerial(CreateUpdateSerialRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CreateUpdateSerial(parameter);
                var response = new CreateUpdateSerialResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListSerial = result.ListSerial
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateUpdateSerialResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteItemInventoryReportResponse DeleteItemInventoryReport(DeleteItemInventoryReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.DeleteItemInventoryReport(parameter);
                var response = new DeleteItemInventoryReportResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteItemInventoryReportResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetSoGTCuaSanPhamTheoKhoResponse GetSoGTCuaSanPhamTheoKho(GetSoGTCuaSanPhamTheoKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetSoGTCuaSanPhamTheoKho(parameter);
                var response = new GetSoGTCuaSanPhamTheoKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    QuantityReservation = result.QuantityReservation
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetSoGTCuaSanPhamTheoKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreatePhieuNhapKhoResponse CreatePhieuNhapKho(CreatePhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.CreatePhieuNhapKho(parameter);
                var response = new CreatePhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    InventoryReceivingVoucherId = result.InventoryReceivingVoucherId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreatePhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDetailPhieuNhapKhoResponse GetDetailPhieuNhapKho(GetDetailPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetDetailPhieuNhapKho(parameter);
                var response = new GetDetailPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListVendor = result.ListVendor,
                    ListItemDetail = result.ListItemDetail,
                    ListCustomer = result.ListCustomer,
                    ListAllWarehouse = result.ListAllWarehouse,
                    ListWarehouse = result.ListWarehouse,
                    PhieuNhapKho = result.PhieuNhapKho,
                    ListVendorOrder = result.ListVendorOrder,
                    ListProduct = result.ListProduct,
                    ListSelectedVendorOrderId = result.ListSelectedVendorOrderId,
                    ListFileUpload = result.ListFileUpload,
                    NoteHistory = result.NoteHistory
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDetailPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SuaPhieuNhapKhoResponse SuaPhieuNhapKho(SuaPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.SuaPhieuNhapKho(parameter);
                var response = new SuaPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SuaPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public KiemTraKhaDungPhieuNhapKhoResponse KiemTraKhaDungPhieuNhapKho(KiemTraKhaDungPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.KiemTraKhaDungPhieuNhapKho(parameter);
                var response = new KiemTraKhaDungPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListSanPhamPhieuNhapKho = result.ListSanPhamPhieuNhapKho
                };

                return response;
            }
            catch (Exception e)
            {
                return new KiemTraKhaDungPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DanhDauCanLamPhieuNhapKhoResponse DanhDauCanLamPhieuNhapKho(DanhDauCanLamPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.DanhDauCanLamPhieuNhapKho(parameter);
                var response = new DanhDauCanLamPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DanhDauCanLamPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public NhanBanPhieuNhapKhoResponse NhanBanPhieuNhapKho(NhanBanPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.NhanBanPhieuNhapKho(parameter);
                var response = new NhanBanPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    InventoryReceivingVoucherId = result.InventoryReceivingVoucherId
                };

                return response;
            }
            catch (Exception e)
            {
                return new NhanBanPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public XoaPhieuNhapKhoResponse XoaPhieuNhapKho(XoaPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.XoaPhieuNhapKho(parameter);
                var response = new XoaPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new XoaPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public HuyPhieuNhapKhoResponse HuyPhieuNhapKho(HuyPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.HuyPhieuNhapKho(parameter);
                var response = new HuyPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new HuyPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public KhongGiuPhanPhieuNhapKhoResponse KhongGiuPhanPhieuNhapKho(KhongGiuPhanPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.KhongGiuPhanPhieuNhapKho(parameter);
                var response = new KhongGiuPhanPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new KhongGiuPhanPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public KiemTraNhapKhoResponse KiemTraNhapKho(KiemTraNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.KiemTraNhapKho(parameter);
                var response = new KiemTraNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    Mode = result.Mode,
                    ListMaSanPhamKhongHopLe = result.ListMaSanPhamKhongHopLe
                };

                return response;
            }
            catch (Exception e)
            {
                return new KiemTraNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DatVeNhapPhieuNhapKhoResponse DatVeNhapPhieuNhapKho(DatVeNhapPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.DatVeNhapPhieuNhapKho(parameter);
                var response = new DatVeNhapPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DatVeNhapPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListProductPhieuNhapKhoResponse GetListProductPhieuNhapKho(GetListProductPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetListProductPhieuNhapKho(parameter);
                var response = new GetListProductPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListProduct = result.ListProduct
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetListProductPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataListPhieuNhapKhoResponse GetMasterDataListPhieuNhapKho(GetMasterDataListPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.GetMasterDataListPhieuNhapKho(parameter);
                var response = new GetMasterDataListPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListProduct = result.ListProduct,
                    ListStatus = result.ListStatus,
                    ListEmployee = result.ListEmployee,
                    ListWarehouse = result.ListWarehouse
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataListPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchListPhieuNhapKhoResponse SearchListPhieuNhapKho(SearchListPhieuNhapKhoRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iWareHouseDataAccess.SearchListPhieuNhapKho(parameter);
                var response = new SearchListPhieuNhapKhoResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListPhieuNhapKho = result.ListPhieuNhapKho,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchListPhieuNhapKhoResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
    }
}
