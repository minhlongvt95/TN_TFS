using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Order;
using TN.TNM.BusinessLogic.Messages.Requests.Order;
using TN.TNM.BusinessLogic.Messages.Responses.Order;
using TN.TNM.BusinessLogic.Models.BillSale;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.OrderStatus;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.BusinessLogic.Models.WareHouse;
using TN.TNM.DataAccess.Interfaces;
using BankAccountModel = TN.TNM.BusinessLogic.Models.BankAccount.BankAccountModel;

namespace TN.TNM.BusinessLogic.Factories.Order
{
    public class CustomerOrderFactory : BaseFactory, ICustomerOrder
    {
        private ICustomerOrderDataAccess iCustomerOrderDataAccess;

        public CustomerOrderFactory(ICustomerOrderDataAccess _iCustomerOrderDataAccess, ILogger<CustomerOrderFactory> _logger)
        {
            this.iCustomerOrderDataAccess = _iCustomerOrderDataAccess;
            this.logger = _logger;
        }

        public CreateCustomerOrderResponse CreateCustomerOrder(CreateCustomerOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Customer Order");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.CreateCustomerOrder(parameter);
                var response = new CreateCustomerOrderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    CustomerOrderID = result.Status ? result.CustomerOrderID : Guid.Empty,
                    SendEmailEntityModel = result.SendEmailEntityModel
                };              
                return response;
            }
            catch (Exception ex)
            {
                return new CreateCustomerOrderResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public GetAllCustomerOrderResponse GetAllCustomerOrder(GetAllCustomerOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Customer Order");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetAllCustomerOrder(parameter);
                var response = new GetAllCustomerOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                    OrderList = new List<CustomerOrderModel>(),
                    OrderTop5List = new List<CustomerOrderModel>()
                };
                if (result.Status)
                {
                    if (result.OrderList != null)
                    {
                        result.OrderList.ForEach(or =>
                        {
                            var tmp = new CustomerOrderModel(or);
                            response.OrderList.Add(tmp);
                        });
                    }
                    if (result.OrderTop5List != null)
                    {
                        result.OrderTop5List.ForEach(or =>
                        {
                            response.OrderTop5List.Add(new CustomerOrderModel(or));
                        });
                    }
                }
                
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetAllCustomerOrderResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetCustomerOrderByIDResponse GetCustomerOrderByID(GetCustomerOrderByIDRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetCustomerOrderByID(parameter);
                var response = new GetCustomerOrderByIDResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    CustomerOrderObject = new CustomerOrderModel(result.CustomerOrderObject),
                    ListCustomerOrderDetail = new List<CustomerOrderDetailModel>(),
                    ListNote = new List<NoteModel>()
                };

                result.ListCustomerOrderDetail.ForEach(cod =>
                {
                    CustomerOrderDetailModel a = new CustomerOrderDetailModel(cod);

                    List<OrderProductDetailProductAttributeValueModel> lstOrderProductDetailProductAttributeValueModel = new List<OrderProductDetailProductAttributeValueModel>();
                    if (cod.OrderProductDetailProductAttributeValue != null)
                    {
                        cod.OrderProductDetailProductAttributeValue.ForEach(X1 =>
                        {
                            lstOrderProductDetailProductAttributeValueModel.Add(new OrderProductDetailProductAttributeValueModel(X1));
                        });
                        a.OrderProductDetailProductAttributeValue = lstOrderProductDetailProductAttributeValueModel;
                    }
                    response.ListCustomerOrderDetail.Add(a);
                });

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                return response;

            }
            catch (Exception e)
            {
                return new GetCustomerOrderByIDResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateCustomerOrderResponse UpdateCustomerOrder(UpdateCustomerOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Customer Order");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.UpdateCustomerOrder(parameter);
                var response = new UpdateCustomerOrderResponse
                {
                    CustomerOrderID = result.Status ? result.CustomerOrderID : Guid.Empty,
                    VendorOrderID = result.Status ? result.VendorOrderID : Guid.Empty,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception ex)
            {
                return new UpdateCustomerOrderResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }
        public ExportCustomerOrderPDFResponse ExportPdfCustomerOrder(ExportCustomerOrderPDFRequest request)
        {
            try
            {
                this.logger.LogInformation("Export Pdf Customer Order");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.ExportPdfCustomerOrder(parameter);

                PDFOrderModel PDFOrder = new PDFOrderModel();
                PDFOrder.OrderDate = result.PDFOrder.OrderDate;
                PDFOrder.CompanyName = result.PDFOrder.CompanyName;
                PDFOrder.Website = result.PDFOrder.Website;
                PDFOrder.TaxCode = result.PDFOrder.TaxCode;
                PDFOrder.CompanyPhone = result.PDFOrder.CompanyPhone;
                PDFOrder.CompanyEmail = result.PDFOrder.CompanyEmail;
                PDFOrder.CompanyAddress = result.PDFOrder.CompanyAddress;
                PDFOrder.OrderCode = result.PDFOrder.OrderCode;
                PDFOrder.Seller = result.PDFOrder.Seller;
                PDFOrder.LocationOfShipment = result.PDFOrder.LocationOfShipment;
                PDFOrder.Description = result.PDFOrder.Description;
                PDFOrder.CustomerName = result.PDFOrder.CustomerName;
                PDFOrder.CustomerCode = result.PDFOrder.CustomerCode;
                PDFOrder.CustomerPhone = result.PDFOrder.CustomerPhone;
                PDFOrder.CustomerAddress = result.PDFOrder.CustomerAddress;
                PDFOrder.CustomerTaxCode = result.PDFOrder.CustomerTaxCode;
                PDFOrder.CustomerPaymentMethod = result.PDFOrder.CustomerPaymentMethod;
                PDFOrder.RecipientName = result.PDFOrder.RecipientName;
                PDFOrder.PlaceOfDelivery = result.PDFOrder.PlaceOfDelivery;
                PDFOrder.ReceivedDate = result.PDFOrder.ReceivedDate;
                PDFOrder.ShippingNote = result.PDFOrder.ShippingNote;
                PDFOrder.TotalAmount = result.PDFOrder.TotalAmount;
                PDFOrder.TotalBeforVat = result.PDFOrder.TotalBeforVat;
                PDFOrder.TotalVat = result.PDFOrder.TotalVat;
                PDFOrder.DiscountValue = result.PDFOrder.DiscountValue;
                PDFOrder.TotalDiscountValue = result.PDFOrder.TotalDiscountValue;
                PDFOrder.TotalAmountAfter = result.PDFOrder.TotalAmountAfter;
                PDFOrder.TotalAmountAfterText = result.PDFOrder.TotalAmountAfterText;
                PDFOrder.ListPDFOrderAttribute = new List<PDFOrderAttributeModel>();
                PDFOrder.ListPDFOrderAttributeOther = new List<PDFOrderAttributeModel>();
                result.PDFOrder.ListPDFOrderAttribute.ForEach(item =>
                {
                    PDFOrderAttributeModel PDFOrderAttribute = new PDFOrderAttributeModel();
                    PDFOrderAttribute.Stt = item.Stt;
                    PDFOrderAttribute.ProductName = item.ProductName;
                    PDFOrderAttribute.ProductCode = item.ProductCode;
                    PDFOrderAttribute.UnitName = item.UnitName;
                    PDFOrderAttribute.Quantity = item.Quantity;
                    PDFOrderAttribute.UnitPrice = item.UnitPrice;
                    PDFOrderAttribute.ExchangeRate = item.ExchangeRate;
                    PDFOrderAttribute.VAT = item.VAT;
                    PDFOrderAttribute.DiscountValue = item.DiscountValue;
                    PDFOrderAttribute.Amount = item.Amount;
                    PDFOrder.ListPDFOrderAttribute.Add(PDFOrderAttribute);
                });

                result.PDFOrder.ListPDFOrderAttributeOther.ForEach(item =>
                {
                    PDFOrderAttributeModel PDFOrderAttribute = new PDFOrderAttributeModel();
                    PDFOrderAttribute.Stt = item.Stt;
                    PDFOrderAttribute.ProductCode = item.ProductCode;
                    PDFOrderAttribute.ProductName = item.ProductName;
                    PDFOrderAttribute.UnitName = item.UnitName;
                    PDFOrderAttribute.Quantity = item.Quantity;
                    PDFOrderAttribute.UnitPrice = item.UnitPrice;
                    PDFOrderAttribute.ExchangeRate = item.ExchangeRate;
                    PDFOrderAttribute.VAT = item.VAT;
                    PDFOrderAttribute.DiscountValue = item.DiscountValue;
                    PDFOrderAttribute.Amount = item.Amount;
                    PDFOrder.ListPDFOrderAttributeOther.Add(PDFOrderAttribute);
                });

                var response = new ExportCustomerOrderPDFResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Code = result.Code,
                    PDFOrder = PDFOrder
                };
                return response;
            }
            catch (Exception ex)
            {
                return new ExportCustomerOrderPDFResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetCustomerOrderBySellerResponse GetCustomerOrderBySeller(GetCustomerOrderBySellerRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Customer Order By Seller");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetCustomerOrderBySeller(parameter);
                var response = new GetCustomerOrderBySellerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    lstResult = result.lstResult,
                    totalProduct = result.totalProduct,
                    OrderList = new List<CustomerOrderModel>(),
                    levelMaxProductCategory = result.levelMaxProductCategory
                };
                result.OrderList.ForEach(or =>
                {
                    response.OrderList.Add(new CustomerOrderModel(or));
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetCustomerOrderBySellerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetEmployeeListByOrganizationIdResponse GetEmployeeListByOrganizationId(GetEmployeeListByOrganizationIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Employee List By OrganizationId");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetEmployeeListByOrganizationId(parameter);
                var response = new GetEmployeeListByOrganizationIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    employeeList = result.employeeList,
                    lstResult = result.lstResult,
                    statusOrderList = result.statusOrderList,
                    monthOrderList = result.monthOrderList,
                    lstOrderInventoryDelivery = new List<CustomerOrderModel>(),
                    lstOrderBill = new List<CustomerOrderModel>(),
                    levelMaxProductCategory = result.levelMaxProductCategory
                };
                result.lstOrderBill.ForEach(item =>
                {
                    CustomerOrderModel obj = new CustomerOrderModel(item);
                    response.lstOrderBill.Add(obj);
                });
                result.lstOrderInventoryDelivery.ForEach(item =>
                {
                    CustomerOrderModel obj = new CustomerOrderModel(item);
                    response.lstOrderInventoryDelivery.Add(obj);
                });
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetEmployeeListByOrganizationIdResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetMonthsListResponse GetMonthsList(GetMonthsListRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetMonthsList(parameter);
                var response = new GetMonthsListResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    monthOrderList = result.monthOrderList
                };
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                return new GetMonthsListResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetProductCategoryGroupByLevelResponse GetProductCategoryGroupByLevel(GetProductCategoryGroupByLevelRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Product Category Group By Level");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetProductCategoryGroupByLevel(parameter);
                var response = new GetProductCategoryGroupByLevelResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    lstResult = result.lstResult
                };
                
                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetProductCategoryGroupByLevelResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetProductCategoryGroupByManagerResponse GetProductCategoryGroupByManager(GetProductCategoryGroupByManagerRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Product Category Group By Manager");
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetProductCategoryGroupByManager(parameter);
                var response = new GetProductCategoryGroupByManagerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    lstResult = result.lstResult
                };

                response.MessageCode = result.Message;
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return new GetProductCategoryGroupByManagerResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetMasterDataOrderSearchResponse GetMasterDataOrderSearch(GetMasterDataOrderSearchRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetMasterDataOrderSearch(parameter);
                var response = new GetMasterDataOrderSearchResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrderStatus = new List<OrderStatusModel>(),
                    ListQuote = new List<QuoteModel>(),
                    ListProduct = new List<ProductModel>(),
                    ListContract = new List<ContractModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    PDFOrder = result.PDFOrder
                };

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListContract.ForEach(item =>
                {
                    response.ListContract.Add(new ContractModel(item));
                });

                result.ListOrderStatus.ForEach(item =>
                {
                    response.ListOrderStatus.Add(new OrderStatusModel(item));
                });

                result.ListQuote.ForEach(item =>
                {
                    QuoteModel obj = new QuoteModel(item);
                    obj.ListCostDetail = item.ListCostDetail;
                    obj.ListDetail = item.ListDetail;
                    response.ListQuote.Add(obj);
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderSearchResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchOrderResponse SearchOrder(SearchOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.SearchOrder(parameter);
                var response = new SearchOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrder = new List<CustomerOrderModel>()
                };

                result.ListOrder.ForEach(item =>
                {
                    response.ListOrder.Add(new CustomerOrderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new SearchOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataOrderCreateResponse GetMasterDataOrderCreate(GetMasterDataOrderCreateRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetMasterDataOrderCreate(parameter);
                var response = new GetMasterDataOrderCreateResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrderStatus = new List<OrderStatusModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    ListPaymentMethod = new List<CategoryModel>(),
                    ListCustomerGroup = new List<CategoryModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListCustomerBankAccount = new List<BankAccountModel>(),
                    ListQuote = new List<QuoteModel>(),
                    ListWare = new List<WareHouseModel>(),
                    ListProduct = new List<ProductModel>(),
                    ListContract = new List<ContractModel>(),
                    ListCustomerCode = result.ListCustomerCode
                };

                result.ListOrderStatus.ForEach(item =>
                {
                    response.ListOrderStatus.Add(new OrderStatusModel(item));
                });

                result.ListContract.ForEach(item =>
                {
                    var obj = new ContractModel(item);
                    obj.ListDetail = item.ListDetail;
                    response.ListContract.Add(obj);
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListPaymentMethod.ForEach(item =>
                {
                    response.ListPaymentMethod.Add(new CategoryModel(item));
                });

                result.ListCustomerGroup.ForEach(item =>
                {
                    response.ListCustomerGroup.Add(new CategoryModel(item));
                });

                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                result.ListCustomerBankAccount.ForEach(item =>
                {
                    response.ListCustomerBankAccount.Add(new BankAccountModel(item));
                });

                result.ListWare.ForEach(item =>
                {
                    response.ListWare.Add(new WareHouseModel(item));
                });

                result.ListQuote.ForEach(item =>
                {
                    QuoteModel obj = new QuoteModel(item);
                    obj.ListCostDetail = item.ListCostDetail;
                    obj.ListDetail = item.ListDetail;
                    response.ListQuote.Add(obj);
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderCreateResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataOrderDetailDialogResponse GetMasterDataOrderDetailDialog(GetMasterDataOrderDetailDialogRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetMasterDataOrderDetailDialog(parameter);
                var response = new GetMasterDataOrderDetailDialogResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListUnitMoney = new List<CategoryModel>(),
                    ListProduct = new List<ProductModel>(),
                    ListUnitProduct = new List<CategoryModel>(),
                    ListWareHouse = new List<WareHouseModel>()
                };

                result.ListUnitMoney.ForEach(item =>
                {
                    response.ListUnitMoney.Add(new CategoryModel(item));
                });

                result.ListUnitProduct.ForEach(item =>
                {
                    response.ListUnitProduct.Add(new CategoryModel(item));
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });

                result.ListWareHouse.ForEach(item =>
                {
                    response.ListWareHouse.Add(new WareHouseModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderDetailDialogResponse()
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
                var result = iCustomerOrderDataAccess.GetVendorByProductId(parameter);
                var response = new GetVendorByProductIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    PriceProduct = result.PriceProduct,
                    ListVendor = new List<VendorModel>(),
                    ListObjectAttributeValueProduct = new List<ObjectAttributeValueProductModel>(),
                    ListObjectAttributeNameProduct = new List<ObjectAttributeNameProductModel>()
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

        public GetMasterDataOrderDetailResponse GetMasterDataOrderDetail(GetMasterDataOrderDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetMasterDataOrderDetail(parameter);
                var response = new GetMasterDataOrderDetailResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrderStatus = new List<OrderStatusModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                    ListPaymentMethod = new List<CategoryModel>(),
                    ListCustomerGroup = new List<CategoryModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListCustomerBankAccount = new List<BankAccountModel>(),
                    ListCustomerCode = result.ListCustomerCode,
                    CustomerOrderObject = new CustomerOrderModel(result.CustomerOrderObject),
                    ListCustomerOrderDetail = new List<CustomerOrderDetailModel>(),
                    ListNote = new List<NoteModel>(),
                    ListQuote = new List<QuoteModel>(),
                    ListWare = new List<WareHouseModel>(),
                    ListProduct = new List<ProductModel>(),
                    ListContract = new List<ContractModel>(),
                    ListInventoryDeliveryVoucher = new List<InventoryDeliveryVoucherModel>(),
                    ListCustomerOrderCostDetail = new List<OrderCostDetailModel>(),
                    IsManager = result.IsManager,
                    ListBillSale = new List<BillSaleModel>()
                };

                result.ListBillSale.ForEach(item =>
                {
                    BillSaleModel obj = new BillSaleModel(item);
                    response.ListBillSale.Add(obj);
                });

                result.ListOrderStatus.ForEach(item =>
                {
                    response.ListOrderStatus.Add(new OrderStatusModel(item));
                });

                result.ListContract.ForEach(item =>
                {
                    response.ListContract.Add(new ContractModel(item));
                });

                result.ListInventoryDeliveryVoucher.ForEach(item =>
                {
                    response.ListInventoryDeliveryVoucher.Add(new InventoryDeliveryVoucherModel(item));
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListPaymentMethod.ForEach(item =>
                {
                    response.ListPaymentMethod.Add(new CategoryModel(item));
                });

                result.ListCustomerGroup.ForEach(item =>
                {
                    response.ListCustomerGroup.Add(new CategoryModel(item));
                });

                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                result.ListCustomerBankAccount.ForEach(item =>
                {
                    response.ListCustomerBankAccount.Add(new BankAccountModel(item));
                });

                result.ListCustomerOrderDetail.ForEach(cod =>
                {
                    CustomerOrderDetailModel a = new CustomerOrderDetailModel(cod);

                    List<OrderProductDetailProductAttributeValueModel> lstOrderProductDetailProductAttributeValueModel = new List<OrderProductDetailProductAttributeValueModel>();
                    if (cod.OrderProductDetailProductAttributeValue != null)
                    {
                        cod.OrderProductDetailProductAttributeValue.ForEach(X1 =>
                        {
                            lstOrderProductDetailProductAttributeValueModel.Add(new OrderProductDetailProductAttributeValueModel(X1));
                        });
                        a.OrderProductDetailProductAttributeValue = lstOrderProductDetailProductAttributeValueModel;
                    }
                    response.ListCustomerOrderDetail.Add(a);
                });

                result.ListCustomerOrderCostDetail.ForEach(cod =>
                {
                    OrderCostDetailModel a = new OrderCostDetailModel(cod);

                    response.ListCustomerOrderCostDetail.Add(a);
                });

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                result.ListWare.ForEach(item =>
                {
                    response.ListWare.Add(new WareHouseModel(item));
                });

                result.ListQuote.ForEach(item =>
                {
                    QuoteModel obj = new QuoteModel(item);
                    obj.ListCostDetail = item.ListCostDetail;
                    obj.ListDetail = item.ListDetail;
                    response.ListQuote.Add(obj);
                });

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteOrderResponse DeleteOrder(DeleteOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.DeleteOrder(parameter);
                var response = new DeleteOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataDashboardHomeResponse GetDataDashboardHome(GetDataDashboardHomeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetDataDashboardHome(parameter);
                var response = new GetDataDashboardHomeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    TotalSalesOfWeek = result.TotalSalesOfWeek,
                    TotalSalesOfMonth = result.TotalSalesOfMonth,
                    TotalSalesOfQuarter = result.TotalSalesOfQuarter,
                    TotalSalesOfWeekPress = result.TotalSalesOfWeekPress,
                    TotalSalesOfMonthPress = result.TotalSalesOfMonthPress,
                    TotalSalesOfQuarterPress = result.TotalSalesOfQuarterPress,
                    ListQuote = new List<QuoteModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListOrderNew = new List<CustomerOrderModel>(),
                    ListCustomerMeeting = new List<CustomerMeetingModel>(),
                    ListEmployeeBirthDayOfWeek = new List<EmployeeModel>(),
                    ListCusBirthdayOfWeek = new List<CustomerModel>(),
                    ListLeadMeeting = new List<Models.Lead.LeadMeetingModel>(),
                    ListParticipants = result.ListParticipants,
                    ListEmployee = result.ListEmployee
                };

                result.ListQuote.ForEach(item =>
                {
                    response.ListQuote.Add(new QuoteModel(item));
                });

                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });

                result.ListOrderNew.ForEach(item =>
                {
                    response.ListOrderNew.Add(new CustomerOrderModel(item));
                });

                result.ListCustomerMeeting.ForEach(item =>
                {
                    response.ListCustomerMeeting.Add(new CustomerMeetingModel(item));
                });

                result.ListEmployeeBirthDayOfWeek.ForEach(item =>
                {
                    response.ListEmployeeBirthDayOfWeek.Add(new EmployeeModel(item));
                });

                result.ListCusBirthdayOfWeek.ForEach(item =>
                {
                    response.ListCusBirthdayOfWeek.Add(new CustomerModel(item));
                });

                result.ListLeadMeeting.ForEach(item =>
                {
                    response.ListLeadMeeting.Add(new Models.Lead.LeadMeetingModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataDashboardHomeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckReceiptOrderHistoryResponse CheckReceiptOrderHistory(CheckReceiptOrderHistoryRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.CheckReceiptOrderHistory(parameter);
                var response = new CheckReceiptOrderHistoryResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    CheckReceiptOrderHistory = result.CheckReceiptOrderHistory
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckReceiptOrderHistoryResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckBeforCreateOrUpdateOrderResponse CheckBeforCreateOrUpdateOrder(CheckBeforCreateOrUpdateOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.CheckBeforCreateOrUpdateOrder(parameter);
                var response = new CheckBeforCreateOrUpdateOrderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    isCheckMaxDebt = result.isCheckMaxDebt
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckBeforCreateOrUpdateOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateCustomerOrderResponse UpdateStatusOrder(UpdateStatusOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.UpdateStatusOrder(parameter);
                var response = new UpdateCustomerOrderResponse()
                {
                    ProcurementRequestId = result.ProcurementRequestId,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateCustomerOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ProfitAccordingCustomersResponse SearchProfitAccordingCustomers(ProfitAccordingCustomersRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.SearchProfitAccordingCustomers(parameter);
                var response = new ProfitAccordingCustomersResponse()
                {
                    ListProfitAccordingCustomers = new List<DataAccess.Models.Order.ProfitAccordingCustomersModel>(),
                    SumCapitalMoney = result.SumCapitalMoney,
                    SumGrossProfit = result.SumGrossProfit,
                    SumProfitMoney = result.SumProfitMoney,
                    SumGrossCapitalMoney = result.SumGrossCapitalMoney,
                    SumGrossProfitMoney = result.SumGrossProfitMoney,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };
                response.ListProfitAccordingCustomers = result.ListProfitAccordingCustomers;

                return response;
            }
            catch (Exception e)
            {
                return new ProfitAccordingCustomersResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataOrderServiceCreateResponse GetMasterDataOrderServiceCreate(GetMasterDataOrderServiceCreateRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetMasterDataOrderServiceCreate(parameter);
                var response = new GetMasterDataOrderServiceCreateResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrderStatus = result.ListOrderStatus,
                    ListMoneyUnit = result.ListMoneyUnit,
                    ListProduct = result.ListProduct,
                    ListProductCategory = result.ListProductCategory,
                    ListLocalAddress = result.ListLocalAddress
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderServiceCreateResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrderServiceResponse CreateOrderService(CreateOrderServiceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.CreateOrderService(parameter);
                var response = new CreateOrderServiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateOrderServiceResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataPayOrderServiceResponse GetMasterDataPayOrderService(GetMasterDataPayOrderServiceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetMasterDataPayOrderService(parameter);
                var response = new GetMasterDataPayOrderServiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListLocalAddress = result.ListLocalAddress,
                    ListLocalPoint = result.ListLocalPoint,
                    PointRate = result.PointRate,
                    MoneyRate = result.MoneyRate
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataPayOrderServiceResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListOrderByLocalPointResponse GetListOrderByLocalPoint(GetListOrderByLocalPointRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetListOrderByLocalPoint(parameter);
                var response = new GetListOrderByLocalPointResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrder = result.ListOrder
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetListOrderByLocalPointResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public PayOrderByLocalPointResponse PayOrderByLocalPoint(PayOrderByLocalPointRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.PayOrderByLocalPoint(parameter);
                var response = new PayOrderByLocalPointResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListLocalPointId = result.ListLocalPointId,
                    OrderDate = result.OrderDate,
                    CustomerName = result.CustomerName,
                    CustomerPhone = result.CustomerPhone,
                    CashierName = result.CashierName
                };

                return response;
            }
            catch (Exception e)
            {
                return new PayOrderByLocalPointResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckExistsCustomerByPhoneResponse CheckExistsCustomerByPhone(CheckExistsCustomerByPhoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.CheckExistsCustomerByPhone(parameter);
                var response = new CheckExistsCustomerByPhoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Customer = result.Customer,
                    PointRate = result.PointRate
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckExistsCustomerByPhoneResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public RefreshLocalPointResponse RefreshLocalPoint(RefreshLocalPointRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.RefreshLocalPoint(parameter);
                var response = new RefreshLocalPointResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListLocalAddress = result.ListLocalAddress
                };

                return response;
            }
            catch (Exception e)
            {
                return new RefreshLocalPointResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetLocalPointByLocalAddressResponse GetLocalPointByLocalAddress(GetLocalPointByLocalAddressRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetLocalPointByLocalAddress(parameter);
                var response = new GetLocalPointByLocalAddressResponse()
                {
                    StatusCode = result.Status
                        ? System.Net.HttpStatusCode.OK
                        : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListLocalPoint = result.ListLocalPoint
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetLocalPointByLocalAddressResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataSearchTopReVenueResponse GetDataSearchTopReVenue(GetDataSearchTopReVenueRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetDataSearchTopReVenue(parameter);
                var response = new GetDataSearchTopReVenueResponse()
                {
                    ListEmployee = result.ListEmployee,
                    CurrentOrganization = result.CurrentOrganization,
                    ListCustomer = result.ListCustomer,
                    ListCustomerGroupCategory = result.ListCustomerGroupCategory,
                    ListProductCategory = result.ListProductCategory,
                    InforExportExcel = result.InforExportExcel,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataSearchTopReVenueResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchTopReVenueResponse SearchTopReVenue(SearchTopReVenueRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.SearchTopReVenue(parameter);
                var response = new SearchTopReVenueResponse()
                {
                   ListTopEmployeeRevenue= result.ListTopEmployeeRevenue,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchTopReVenueResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataSearchRevenueProductResponse GetDataSearchRevenueProduct(GetDataSearchRevenueProductRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetDataSearchRevenueProduct(parameter);
                var response = new GetDataSearchRevenueProductResponse()
                {
                    ListEmployee = result.ListEmployee,
                    ListProductCategory = result.ListProductCategory,
                    ListWarehouse = result.ListWarehouse,
                    ListCustomerGroupCategory = result.ListCustomerGroupCategory,
                    ListCustomer = result.ListCustomer,
                    InforExportExcel = result.InforExportExcel,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataSearchRevenueProductResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchRevenueProductResponse SearchRevenueProduct(SearchRevenueProductRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.SearchRevenueProduct(parameter);
                var response = new SearchRevenueProductResponse()
                {
                    ListProductRevenue = result.ListProductRevenue,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchRevenueProductResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListOrderDetailByOrderResponse GetListOrderDetailByOrder(GetListOrderDetailByOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetListOrderDetailByOrder(parameter);
                var response = new GetListOrderDetailByOrderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListOrderDetail = result.ListOrderDetail
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetListOrderDetailByOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListProductWasOrderResponse GetListProductWasOrder(GetListProductWasOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetListProductWasOrder(parameter);
                var response = new GetListProductWasOrderResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProductWasOrder = result.ListProductWasOrder,
                    OrderId = result.OrderId
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetListProductWasOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateCustomerServiceResponse UpdateCustomerService(UpdateCustomerServiceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.UpdateCustomerService(parameter);
                var response = new UpdateCustomerServiceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateCustomerServiceResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataProfitByCustomerResponse GetDataProfitByCustomer(GetDataProfitByCustomerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.GetDataProfitByCustomer(parameter);
                var response = new GetDataProfitByCustomerResponse()
                {
                    InforExportExcel = result.InforExportExcel,
                    ListContract = result.ListContract,
                    ListCustomer = result.ListCustomer,
                    ListCustomerGroupCategory = result.ListCustomerGroupCategory,
                    ListProductCategory = result.ListProductCategory,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataProfitByCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchProfitCustomerResponse SearchProfitCustomer(SearchProfitCustomerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iCustomerOrderDataAccess.SearchProfitCustomer(parameter);
                var response = new SearchProfitCustomerResponse()
                {
                    ListSearchProfitCustomer = result.ListSearchProfitCustomer,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchProfitCustomerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
