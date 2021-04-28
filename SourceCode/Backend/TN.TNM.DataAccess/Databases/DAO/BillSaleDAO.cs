using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;
using TN.TNM.DataAccess.Messages.Results.BillSale;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class BillSaleDAO : BaseDAO, IBillSaleDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public BillSaleDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }

        public AddOrEditBillSaleResult AddOrEditBillSale(AddOrEditBillSaleParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new AddOrEditBillSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_USER
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new AddOrEditBillSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_EMPLOYEE
                    };
                }

                if (parameter.BillSale == null)
                {
                    return new AddOrEditBillSaleResult()
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.EDIT_FAIL
                    };
                }

                Guid billSaleId;
                var _bill = new BillOfSale();
                if (parameter.IsCreate == true)
                {
                    #region Lấy danh sách trạng thái của hóa đơn

                    var categoryTypeBillId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "BILL").CategoryTypeId;
                    var statusId = context.Category.FirstOrDefault(x => x.CategoryCode == "NEW" && x.CategoryTypeId == categoryTypeBillId).CategoryId;

                    #endregion

                    var billSale = new BillOfSale()
                    {
                        Active = true,
                        AccountBankId = parameter.BillSale.AccountBankId,
                        BillDate = parameter.BillSale.BillDate,
                        BillOfSaLeCode = GenerateBillSaleCode(),
                        BillOfSaLeId = Guid.NewGuid(),
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        CustomerId = parameter.BillSale.CustomerId,
                        CustomerName = parameter.BillSale.CustomerName,
                        DebtAccountId = parameter.BillSale.DebtAccountId,
                        Description = parameter.BillSale.Description,
                        EmployeeId = parameter.BillSale.EmployeeId,
                        EndDate = parameter.BillSale.EndDate,
                        InvoiceSymbol = parameter.BillSale.InvoiceSymbol,
                        Mst = parameter.BillSale.Mst,
                        Note = parameter.BillSale.Note,
                        OrderId = parameter.BillSale.OrderId,
                        PaymentMethodId = parameter.BillSale.PaymentMethodId,
                        StatusId = statusId,
                        CustomerAddress = parameter.BillSale.CustomerAddress,
                        TermsOfPaymentId = parameter.BillSale.TermsOfPaymentId,
                        DiscountType = parameter.BillSale.DiscountType,
                        DiscountValue = parameter.BillSale.DiscountValue
                    };

                    context.BillOfSale.Add(billSale);
                    _bill = billSale;
                    billSaleId = billSale.BillOfSaLeId;
                }
                else
                {
                    var billSale = context.BillOfSale.FirstOrDefault(x => parameter.BillSale.BillOfSaLeId == x.BillOfSaLeId && x.Active == true);
                    if (billSale == null)
                    {
                        return new AddOrEditBillSaleResult()
                        {
                            Status = false,
                            Message = CommonMessage.BillSale.EDIT_FAIL
                        };
                    }

                    billSale.AccountBankId = parameter.BillSale.AccountBankId;
                    billSale.BillDate = parameter.BillSale.BillDate;
                    billSale.BillOfSaLeCode = parameter.BillSale.BillOfSaLeCode;
                    billSale.UpdatedById = parameter.UserId;
                    billSale.UpdatedDate = DateTime.Now;
                    billSale.CustomerId = parameter.BillSale.CustomerId;
                    billSale.CustomerName = parameter.BillSale.CustomerName;
                    billSale.DebtAccountId = parameter.BillSale.DebtAccountId;
                    billSale.Description = parameter.BillSale.Description;
                    billSale.EmployeeId = parameter.BillSale.EmployeeId;
                    billSale.EndDate = parameter.BillSale.EndDate;
                    billSale.InvoiceSymbol = parameter.BillSale.InvoiceSymbol;
                    billSale.Mst = parameter.BillSale.Mst;
                    billSale.Note = parameter.BillSale.Note;
                    billSale.OrderId = parameter.BillSale.OrderId;
                    billSale.PaymentMethodId = parameter.BillSale.PaymentMethodId;
                    billSale.StatusId = parameter.BillSale.StatusId;
                    billSale.TermsOfPaymentId = parameter.BillSale.TermsOfPaymentId;
                    billSale.DiscountType = parameter.BillSale.DiscountType;
                    billSale.DiscountValue = parameter.BillSale.DiscountValue;
                    billSale.CustomerAddress = parameter.BillSale.CustomerAddress;

                    context.BillOfSale.Update(billSale);
                    _bill = billSale;
                    billSaleId = billSale.BillOfSaLeId;

                    var listDetail = context.BillOfSaleDetail.Where(x => x.BillOfSaleId == billSale.BillOfSaLeId && x.Active == true).ToList();
                    if (listDetail.Count > 0)
                    {
                        context.RemoveRange(listDetail);
                        var listAttrId = listDetail.Select(x => x.BillOfSaleDetailId).ToList();
                        var listAttr = context.BillOfSaleCostProductAttribute.Where(x => listAttrId.Contains(x.BillOfSaleDetailId.Value)).ToList();
                        context.BillOfSaleCostProductAttribute.RemoveRange(listAttr);
                    }

                    var listCost = context.BillOfSaleCost.Where(x => x.Active == true && x.BillOfSaleId == billSale.BillOfSaLeId).ToList();
                    if (listCost.Count > 0)
                    {
                        context.RemoveRange(listCost);
                    }
                }

                parameter.BillSale.ListBillSaleDetail?.ForEach(item =>
                {
                    var billDetail = new BillOfSaleDetail()
                    {
                        AccountDiscountId = item.AccountDiscountId,
                        AccountId = item.AccountId,
                        Active = true,
                        ActualInventory = item.ActualInventory,
                        BillOfSaleDetailId = Guid.NewGuid(),
                        BillOfSaleId = billSaleId,
                        BusinessInventory = item.BusinessInventory,
                        CostsQuoteType = item.CostsQuoteType,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        CurrencyUnit = item.CurrencyUnit,
                        Description = item.Description,
                        DiscountType = item.DiscountType,
                        DiscountValue = item.DiscountValue,
                        ExchangeRate = item.ExchangeRate,
                        IncurredUnit = item.IncurredUnit,
                        MoneyForGoods = SumAmount(item.Quantity, item.UnitPrice, item.ExchangeRate, item.Vat, item.DiscountValue,
                                                    item.DiscountType),
                        OrderDetailType = item.OrderDetailType,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitId = item.UnitId,
                        UnitPrice = item.UnitPrice,
                        Vat = item.Vat,
                        VendorId = item.VendorId,
                        WarehouseId = item.WarehouseId,
                        OrderNumber = item.OrderNumber
                    };
                    context.Add(billDetail);

                    item.ListBillSaleDetailProductAttribute?.ForEach(attr =>
                    {
                        var billDetailAttr = new BillOfSaleCostProductAttribute()
                        {
                            BillOfSaleCostProductAttributeId = Guid.NewGuid(),
                            BillOfSaleDetailId = billDetail.BillOfSaleDetailId,
                            ProductAttributeCategoryId = attr.ProductAttributeCategoryId,
                            ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId,
                            ProductId = attr.ProductId
                        };

                        context.BillOfSaleCostProductAttribute.Add(billDetailAttr);
                    });
                });

                parameter.BillSale.ListCost?.ForEach(item =>
                {
                    var cost = new BillOfSaleCost()
                    {
                        Active = true,
                        BillOfSaleCostId = Guid.NewGuid(),
                        BillOfSaleId = billSaleId,
                        CostId = item.CostId,
                        CostName = item.CostName,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        CostCode = item.CostCode
                    };

                    context.BillOfSaleCost.Add(cost);
                });

                context.SaveChanges();

                #region Gửi thông báo
                
                if (parameter.IsCreate == true)
                {
                    NotificationHelper.AccessNotification(context, TypeModel.BillSale, "CRE", new BillOfSale(), _bill, true);
                }
                else
                {
                    NotificationHelper.AccessNotification(context, TypeModel.BillSaleDetail, "UPD", new BillOfSale(), _bill, true);
                }

                #endregion

                return new AddOrEditBillSaleResult()
                {
                    Status = true,
                    BillSaleId = billSaleId,
                    Message = CommonMessage.BillSale.EDIT_SUCCESS
                };
            }
            catch (Exception)
            {
                return new AddOrEditBillSaleResult()
                {
                    Status = false,
                    Message = CommonMessage.BillSale.EDIT_FAIL
                };
            }
        }
        public string GenerateBillSaleCode()
        {

            var billSaleTemp = context.BillOfSale.Where(z => z.CreatedDate.Date == DateTime.Now.Date).OrderByDescending(x => x.CreatedDate).ToList().FirstOrDefault();
            var billSaleCode = "";
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            if (billSaleTemp == null)
            {
                billSaleCode = "INV-" + year + month + day + "0001";
            }
            else
            {
                var code = billSaleTemp.BillOfSaLeCode.Substring(billSaleTemp.BillOfSaLeCode.Length - 4);
                int temp = Convert.ToInt32(code);
                temp++;
                string identity = "";
                if (temp < 10)
                {
                    identity = "000" + temp;
                }
                else if (temp < 100)
                {
                    identity = "00" + temp;
                }
                else if (temp < 1000)
                {
                    identity = "0" + temp;
                }
                else if (temp < 10000)
                {
                    identity = temp.ToString();
                }

                billSaleCode = "INV-" + year + month + day + identity;
            }

            return billSaleCode;
        }

        public GetMasterDataBillSaleCreateEditResult GetMasterDataBillSaleCreateEdit(GetMasterDataBillSaleCreateEditParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                // Lấy thông tin người đăng nhập
                if (user == null)
                {
                    return new GetMasterDataBillSaleCreateEditResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_USER
                    };
                }
                // Kiểm tra coi có phải nhân viên không
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetMasterDataBillSaleCreateEditResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_EMPLOYEE
                    };
                }

                BillSaleEntityModel billSaleResult = null;
                var statusBill = new List<CategoryEntityModel>();
                var listTypeBank = new List<CategoryEntityModel>();
                var listCategoryCommon = context.Category.ToList();
                var listCategoryTypeCommon = context.CategoryType.ToList();
                var listEmployeeCommn = context.Employee.ToList();
                var listBillDetail = new List<BillSaleDetailEntityModel>();
                var listCost = new List<BillSaleCostEntityModel>();
                Guid? employeeBillId = null;
                Guid? customerBillId = null;
                Guid? orderId = null;
                Guid? customerId = null;
                var listOrderResult = new List<CustomerOrderEntityModel>();
                var listInventoryDeliveryVoucher = new List<InventoryDeliveryVoucherEntityModel>();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();
                var eployeeUser = new List<EmployeeModel>();
                var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listAllUser = context.User.Where(x => x.Active == true).ToList();
                // Lấy trạng thái đơn hàng là đơn hàng bán
                var statusOrder = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DLV")?.OrderStatusId;

                eployeeUser = listAllEmployee.Select(item => new EmployeeModel
                {
                    EmployeeId = item.EmployeeId,
                    EmployeeName = item.EmployeeName,
                    UserId = listAllUser.FirstOrDefault(u => u.EmployeeId == item.EmployeeId).UserId
                }).ToList();
                var listNote = new List<NoteEntityModel>();

                if (parameter.IsCreate)
                {
                    if (parameter.ObjectId != null && parameter.ObjectId != Guid.Empty)
                    {
                        orderId = parameter.ObjectId;
                        billSaleResult = new BillSaleEntityModel();
                        billSaleResult.ListCost = new List<BillSaleCostEntityModel>();
                        billSaleResult.ListBillSaleDetail = new List<BillSaleDetailEntityModel>();

                        var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == orderId);
                        customerId = order?.CustomerId;

                        #region Lấy thông tin tab chi phí

                        listCost = context.OrderCostDetail.Where(x => x.OrderId == parameter.ObjectId && x.Active == true)
                        .Select(y => new BillSaleCostEntityModel()
                        {
                            OrderId = y.OrderId,
                            OrderCostId = y.OrderCostDetailId,
                            CostId = y.CostId,
                            CostName = y.CostName,
                            Quantity = y.Quantity,
                            UnitPrice = y.UnitPrice
                        }).ToList();

                        #endregion

                        #region Lấy thông tin sản phẩm dịch vụ

                        listBillDetail = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.ObjectId && x.Active == true)
                        .Select(y => new BillSaleDetailEntityModel()
                        {
                            ActualInventory = y.ActualInventory,
                            BusinessInventory = y.BusinessInventory,
                            CurrencyUnit = y.CurrencyUnit,
                            Description = y.Description,
                            DiscountType = y.DiscountType,
                            DiscountValue = y.DiscountValue,
                            ExchangeRate = y.ExchangeRate,
                            IncurredUnit = y.IncurredUnit,
                            MoneyForGoods = SumAmount(y.Quantity, y.UnitPrice, y.ExchangeRate, y.Vat, y.DiscountValue,
                                                    y.DiscountType),
                            OrderDetailType = y.OrderDetailType,
                            ProductName = y.ProductName,
                            ProductId = y.ProductId,
                            Quantity = y.Quantity,
                            UnitId = y.UnitId,
                            UnitPrice = y.UnitPrice,
                            Vat = y.Vat,
                            VendorId = y.VendorId,
                            WarehouseId = y.WarehouseId,
                            OrderId = y.OrderId,
                            OrderDetailId = y.OrderDetailId,
                            ListBillSaleDetailProductAttribute = new List<BillSaleDetailProductAttributeEntityModel>()
                        }).ToList();

                        #endregion

                    }

                }
                else
                {
                    if (parameter.ObjectId == null || parameter.ObjectId == Guid.Empty)
                    {
                        return new GetMasterDataBillSaleCreateEditResult()
                        {
                            Status = false,
                            Message = CommonMessage.BillSale.GET_FAIL
                        };
                    }

                    var billSale = context.BillOfSale.FirstOrDefault(x => x.BillOfSaLeId == parameter.ObjectId && x.Active == true);
                    if (billSale == null)
                    {
                        return new GetMasterDataBillSaleCreateEditResult()
                        {
                            Status = false,
                            Message = CommonMessage.BillSale.GET_FAIL
                        };
                    }

                    orderId = billSale.OrderId;

                    #region Thông tin hóa đơn

                    billSaleResult = new BillSaleEntityModel();
                    billSaleResult.ListBillSaleDetail = new List<BillSaleDetailEntityModel>();
                    billSaleResult.ListCost = new List<BillSaleCostEntityModel>();
                    billSaleResult.AccountBankId = billSale.AccountBankId;
                    billSaleResult.BillOfSaLeCode = billSale.BillOfSaLeCode;
                    billSaleResult.BillOfSaLeId = billSale.BillOfSaLeId;
                    billSaleResult.CustomerName = billSale.CustomerName;
                    billSaleResult.CustomerId = billSale.CustomerId;
                    billSaleResult.DebtAccountId = billSale.DebtAccountId;
                    billSaleResult.Description = billSale.Description;
                    billSaleResult.EmployeeId = billSale.EmployeeId;
                    billSaleResult.EndDate = billSale.EndDate;
                    billSaleResult.Mst = billSale.Mst;
                    billSaleResult.Note = billSale.Note;
                    billSaleResult.BillDate = billSale.BillDate;
                    billSaleResult.OrderId = billSale.OrderId;
                    billSaleResult.PaymentMethodId = billSale.PaymentMethodId;
                    billSaleResult.StatusId = billSale.StatusId;
                    billSaleResult.TermsOfPaymentId = billSale.TermsOfPaymentId;
                    billSaleResult.InvoiceSymbol = billSale.InvoiceSymbol;
                    billSaleResult.CustomerAddress = billSale.CustomerAddress;
                    billSaleResult.DiscountValue = billSale.DiscountValue;
                    billSaleResult.DiscountType = billSale.DiscountType;

                    employeeBillId = billSale.EmployeeId;
                    customerBillId = billSale.CustomerId;

                    customerId = billSale.CustomerId;

                    #endregion

                    #region Lấy thông tin sản phẩm dịch vụ

                    listBillDetail = context.BillOfSaleDetail.Where(x => x.BillOfSaleId == billSale.BillOfSaLeId && x.Active == true)
                    .Select(y => new BillSaleDetailEntityModel()
                    {
                        AccountDiscountId = y.AccountDiscountId,
                        ActualInventory = y.ActualInventory,
                        BillOfSaleDetailId = y.BillOfSaleDetailId,
                        BillOfSaleId = y.BillOfSaleId,
                        BusinessInventory = y.BusinessInventory,
                        CostsQuoteType = y.CostsQuoteType,
                        CurrencyUnit = y.CurrencyUnit,
                        Description = y.Description,
                        DiscountType = y.DiscountType,
                        DiscountValue = y.DiscountValue,
                        ExchangeRate = y.ExchangeRate,
                        IncurredUnit = y.IncurredUnit,
                        MoneyForGoods = SumAmount(y.Quantity, y.UnitPrice, y.ExchangeRate, y.Vat, y.DiscountValue,
                                                    y.DiscountType),
                        OrderDetailType = y.OrderDetailType,
                        ProductName = y.ProductName,
                        ProductId = y.ProductId,
                        Quantity = y.Quantity,
                        UnitId = y.UnitId,
                        UnitPrice = y.UnitPrice,
                        Vat = y.Vat,
                        VendorId = y.VendorId,
                        WarehouseId = y.WarehouseId,
                        OrderNumber = y.OrderNumber,
                        ListBillSaleDetailProductAttribute = new List<BillSaleDetailProductAttributeEntityModel>()
                    }).ToList();


                    #endregion

                    #region Lấy thông tin tab chi phí

                    listCost = context.BillOfSaleCost.Where(x => x.BillOfSaleId == billSale.BillOfSaLeId && x.Active == true)
                    .Select(y => new BillSaleCostEntityModel()
                    {
                        BillOfSaleCostId = y.BillOfSaleCostId,
                        BillOfSaleId = y.BillOfSaleId,
                        CostId = y.CostId,
                        CostName = y.CostName,
                        Quantity = y.Quantity,
                        UnitPrice = y.UnitPrice,
                        CostCode = y.CostCode
                    }).ToList();

                    #endregion

                    #region Lấy list ghi chú



                    listNote = context.Note
                        .Where(x => x.ObjectId == parameter.ObjectId && x.ObjectType == "BILL" && x.Active == true).Select(
                            y => new NoteEntityModel
                            {
                                NoteId = y.NoteId,
                                Description = y.Description,
                                Type = y.Type,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                NoteTitle = y.NoteTitle,
                                Active = y.Active,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                ResponsibleName = "",
                                ResponsibleAvatar = "",
                                NoteDocList = new List<NoteDocumentEntityModel>()
                            }).ToList();

                    if (listNote.Count > 0)
                    {
                        var listNoteId = listNote.Select(x => x.NoteId).ToList();
                        var listUser = context.User.ToList();
                        var _listAllEmployee = context.Employee.ToList();
                        var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                            y => new NoteDocumentEntityModel
                            {
                                DocumentName = y.DocumentName,
                                DocumentSize = y.DocumentSize,
                                DocumentUrl = y.DocumentUrl,
                                CreatedById = y.CreatedById,
                                CreatedDate = y.CreatedDate,
                                UpdatedById = y.UpdatedById,
                                UpdatedDate = y.UpdatedDate,
                                NoteDocumentId = y.NoteDocumentId,
                                NoteId = y.NoteId
                            }).ToList();

                        listNote.ForEach(item =>
                        {
                            var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderByDescending(z => z.UpdatedDate).ToList();
                        });

                        //Sắp xếp lại listNote
                        listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    }

                    #endregion
                }

                // Lấy danh sách đơn vị tính tiền
                var listUnitId = listBillDetail.Select(x => x.UnitId).ToList();
                var listUnit = listCategoryCommon.Where(x => listUnitId.Contains(x.CategoryId)).ToList();
                // Lấy danh sách nhà cung cấp
                var listVendorId = listBillDetail.Select(x => x.VendorId).ToList();
                var listVendor = context.Vendor.Where(x => listVendorId.Contains(x.VendorId)).ToList();
                // Lấy danh sản phẩm
                var listProductId = listBillDetail.Select(x => x.ProductId).ToList();
                var listProduct = context.Product.Where(x => listProductId.Contains(x.ProductId)).ToList();
                // Lấy danh sách kho
                var listWarehouseId = listBillDetail.Select(x => x.WarehouseId).ToList();
                var listWarehouse = context.Warehouse.Where(x => listWarehouseId.Contains(x.WarehouseId)).ToList();

                var listAttr = new List<BillSaleDetailProductAttributeEntityModel>();
                if (!parameter.IsCreate)
                {
                    // Lấy danh sách thuộc tính của sản phẩm
                    var listBillDetailId = listBillDetail.Select(x => x.BillOfSaleDetailId).ToList();
                    listAttr = context.BillOfSaleCostProductAttribute.Where(x => listBillDetailId.Contains(x.BillOfSaleDetailId.Value))
                      .Select(y => new BillSaleDetailProductAttributeEntityModel()
                      {
                          BillOfSaleCostProductAttributeId = y.BillOfSaleCostProductAttributeId,
                          BillOfSaleDetailId = y.BillOfSaleDetailId,
                          ProductAttributeCategoryId = y.ProductAttributeCategoryId,
                          ProductAttributeCategoryValueId = y.ProductAttributeCategoryValueId,
                          ProductId = y.ProductId
                      }).ToList();
                }
                else
                {
                    if (parameter.ObjectId != null && parameter.ObjectId != Guid.Empty)
                    {
                        // Lấy danh sách thuộc tính của sản phẩm
                        var listOrderDetaillId = listBillDetail.Select(x => x.OrderDetailId).ToList();
                        listAttr = context.OrderProductDetailProductAttributeValue.Where(x => listOrderDetaillId.Contains(x.OrderDetailId))
                      .Select(y => new BillSaleDetailProductAttributeEntityModel()
                      {
                          OrderProductDetailProductAttributeValueId = y.OrderProductDetailProductAttributeValueId,
                          OrderDetailId = y.OrderDetailId,
                          ProductAttributeCategoryId = y.ProductAttributeCategoryId,
                          ProductAttributeCategoryValueId = y.ProductAttributeCategoryValueId,
                          ProductId = y.ProductId
                      }).ToList();
                    }
                }


                if (billSaleResult != null)
                {
                    listBillDetail?.ForEach(item =>
                    {
                        var listAttrDetail = listAttr.Where(x => x.BillOfSaleDetailId == item.BillOfSaleDetailId).ToList();
                        item.ListBillSaleDetailProductAttribute = new List<BillSaleDetailProductAttributeEntityModel>();
                        listAttrDetail.AddRange(listAttrDetail);

                        var unit = listUnit.FirstOrDefault(x => x.CategoryId == item.UnitId);
                        item.UnitName = unit?.CategoryName;

                        var vendor = listVendor.FirstOrDefault(x => x.VendorId == item.VendorId);
                        item.VendorName = vendor?.VendorName;

                        var product = listProduct.FirstOrDefault(x => x.ProductId == item.ProductId);
                        item.ProductCode = product?.ProductCode;

                        var warehouse = listWarehouse.FirstOrDefault(x => x.WarehouseId == item.WarehouseId);
                        item.WarehouseCode = warehouse?.WarehouseName;
                    });
                    billSaleResult.ListBillSaleDetail = listBillDetail;
                    billSaleResult.ListCost = listCost;
                }

                #region Lấy danh sách trạng thái của hóa đơn

                var categoryTypeBillId = listCategoryTypeCommon.FirstOrDefault(x => x.CategoryTypeCode == "BILL").CategoryTypeId;
                statusBill = listCategoryCommon.Where(x => x.CategoryTypeId == categoryTypeBillId).Select(y => new CategoryEntityModel()
                {
                    CategoryId = y.CategoryId,
                    CategoryName = y.CategoryName,
                    CategoryCode = y.CategoryCode,
                }).ToList();

                #endregion

                #region Lấy danh sách loại hình thanh toán

                var categoryTypeBankId = listCategoryTypeCommon.FirstOrDefault(x => x.CategoryTypeCode == "PTO").CategoryTypeId;
                listTypeBank = listCategoryCommon.Where(x => x.CategoryTypeId == categoryTypeBankId).Select(y => new CategoryEntityModel()
                {
                    CategoryId = y.CategoryId,
                    CategoryName = y.CategoryName,
                    CategoryCode = y.CategoryCode,
                }).ToList();

                #endregion

                #region Lấy danh sách nhân viên 

                var listEmployee = listEmployeeCommn.Where(x => x.Active == true || x.EmployeeId == employeeBillId).
                    Select(y => new EmployeeEntityModel()
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeName = y.EmployeeName,
                        EmployeeCode = y.EmployeeCode
                    }).ToList();

                #endregion

                #region Lấy danh sách khách hàng định danh 

                var categoryTypeCustomerId = listCategoryTypeCommon.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                var categoryCustomerId = listCategoryCommon.FirstOrDefault(x => x.CategoryTypeId == categoryTypeCustomerId && x.CategoryCode == "HDO").CategoryId;
                var listCustomerCommon = context.Customer.ToList();
                var customerBill = listCustomerCommon.FirstOrDefault(x => x.CustomerId == customerBillId);
                var listCustomer = listCustomerCommon.Where(x => x.Active == true && categoryCustomerId == x.StatusId)
                    .Select(y => new CustomerEntityModel()
                    {
                        CustomerId = y.CustomerId,
                        CustomerName = y.CustomerName,
                        CustomerCode = y.CustomerCode,
                        CustomerGroupId = y.CustomerGroupId,
                        PersonInChargeId = y.PersonInChargeId
                    }).OrderByDescending(z => z.CustomerCode).ToList();
                if (customerBill != null)
                {
                    listCustomer.Add(new CustomerEntityModel()
                    {
                        CustomerId = customerBill.CustomerId,
                        CustomerName = customerBill.CustomerName,
                        CustomerCode = customerBill.CustomerCode,
                        CustomerGroupId = customerBill.CustomerGroupId,
                        PersonInChargeId = customerBill.PersonInChargeId
                    });
                    listCustomer = listCustomer.Distinct().ToList();
                }
                var listEmployeeTempId = listCustomer.Select(x => x.PersonInChargeId).Distinct().ToList();

                var listEmployeeCustomer = listEmployeeCommn.Where(x => listEmployeeTempId.Contains(x.EmployeeId)).ToList();

                var listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                var commonContact = context.Contact.Where(x => listCustomerId.Contains(x.ObjectId) || x.ObjectId == customerBillId).ToList();
                var listCustomerGroupId = listCustomer.Select(x => x.CustomerGroupId).ToList();
                var listCustomerGroup = listCategoryCommon.Where(x => listCustomerGroupId.Contains(x.CategoryId)).ToList();
                // Lấy danh sách xã
                var listWardId = commonContact.Select(x => x.WardId).ToList();
                var listWardCommon = context.Ward.Where(x => listWardId.Contains(x.WardId)).ToList();
                // Lấy danh sách phường
                var listDistrictId = commonContact.Select(x => x.DistrictId).ToList();
                var listDistrictCommon = context.District.Where(x => listDistrictId.Contains(x.DistrictId)).ToList();
                // Lấy danh sách tỉnh
                var listProvinceId = commonContact.Select(x => x.ProvinceId).ToList();
                var listProvinceCommon = context.Province.Where(x => listProvinceId.Contains(x.ProvinceId)).ToList();
                #region  Lấy danh sách tài khoản thanh toán

                var listAccountBank = context.BankAccount.Where(x => x.Active == true && x.ObjectType == "CUS").Select(y => new BankAccountEntityModel()
                {
                    AccountName = y.AccountName,
                    AccountNumber = y.AccountNumber,
                    BankAccountId = y.BankAccountId,
                    BankDetail = y.BankDetail,
                    BankName = y.BankName,
                    BranchName = y.BranchName,
                    ObjectId = y.ObjectId,
                    ObjectType = y.ObjectType
                }).ToList();

                #endregion

                listCustomer.ForEach(item =>
                {
                    var temp = commonContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS");
                    var ward = listWardCommon.FirstOrDefault(x => x.WardId == temp?.WardId);
                    var district = listDistrictCommon.FirstOrDefault(x => x.DistrictId == temp?.DistrictId);
                    var province = listProvinceCommon.FirstOrDefault(x => x.ProvinceId == temp?.ProvinceId);
                    item.CustomerPhone = temp?.Phone;
                    if (temp != null)
                    {
                        item.FullAddress = temp.Address;
                    }

                    if (ward != null)
                    {
                        item.FullAddress = item.FullAddress + "," + ward.WardName;
                    }

                    if (district != null)
                    {
                        item.FullAddress = item.FullAddress + "," + district.DistrictName;
                    }

                    if (province != null)
                    {
                        item.FullAddress = item.FullAddress + "," + province.ProvinceName;
                    }

                    item.TaxCode = temp?.TaxCode;
                    item.CustomerGroup = listCustomerGroup.FirstOrDefault(x => x.CategoryId == item.CustomerGroupId)?.CategoryName;
                    item.PersonInCharge = listEmployeeCustomer.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId)?.EmployeeName;

                    item.ListBankAccount = listAccountBank.Where(x => x.ObjectId == item.CustomerId).ToList();
                });

                #endregion

                #region Lấy thông tin đơn hàng

                OrderBillEntityModel orderResult = null;
                if (orderId != null)
                {
                    orderResult = GetOrder(orderId);

                    #region Lấy thông tin giao hàng

                    listInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.Where(l => l.ObjectId == orderId)
                        .Select(l => new InventoryDeliveryVoucherEntityModel
                        {
                            InventoryDeliveryVoucherId = l.InventoryDeliveryVoucherId,
                            InventoryDeliveryVoucherCode = l.InventoryDeliveryVoucherCode,
                            StatusId = l.StatusId,
                            InventoryDeliveryVoucherType = l.InventoryDeliveryVoucherType,
                            WarehouseId = l.WarehouseId,
                            ObjectId = l.ObjectId,
                            Receiver = l.Receiver,
                            Reason = l.Reason,
                            InventoryDeliveryVoucherDate = l.InventoryDeliveryVoucherDate,
                            InventoryDeliveryVoucherTime = l.InventoryDeliveryVoucherTime,
                            LicenseNumber = l.LicenseNumber,
                            Active = l.Active,
                            CreatedDate = l.CreatedDate,
                            CreatedById = l.CreatedById,
                            UpdatedDate = l.UpdatedDate,
                            UpdatedById = l.UpdatedById,
                            NameCreate = eployeeUser.FirstOrDefault(em => em.UserId == l.CreatedById).EmployeeName,
                            NameStatus = listCategory.FirstOrDefault(ca => ca.CategoryId == l.StatusId).CategoryName,
                        }).ToList();

                    #endregion
                }

                #endregion

                #region Lấy danh sách đơn hàng

                listOrderResult = context.CustomerOrder.Where(x => x.Active == true && x.StatusId == statusOrder).Select(y => new CustomerOrderEntityModel()
                {
                    Amount = y.Amount,
                    BankAccountId = y.BankAccountId,
                    CustomerAddress = y.CustomerAddress,
                    CustomerContactId = y.CustomerContactId,
                    CustomerId = y.CustomerId,
                    CustomerName = y.CustomerName,
                    DaysAreOwed = y.DaysAreOwed,
                    Description = y.Description,
                    DiscountType = y.DiscountType,
                    DiscountValue = y.DiscountValue,
                    IsAutoGenReceiveInfor = y.IsAutoGenReceiveInfor,
                    LocationOfShipment = y.LocationOfShipment,
                    MaxDebt = y.MaxDebt,
                    Note = y.Note,
                    OrderCode = y.OrderCode,
                    OrderContractId = y.OrderContractId,
                    OrderId = y.OrderId,
                    OrderDate = y.OrderDate,
                    PaymentMethod = y.PaymentMethod,
                    PlaceOfDelivery = y.PlaceOfDelivery,
                    QuoteId = y.QuoteId,
                    ReasonCancel = y.ReasonCancel,
                    ReceiptInvoiceAmount = y.ReceiptInvoiceAmount,
                    ReceivedDate = y.ReceivedDate,
                    ReceivedHour = y.ReceivedHour,
                    RecipientEmail = y.RecipientEmail,
                    RecipientPhone = y.RecipientPhone,
                    RecipientName = y.RecipientName,
                    Seller = y.Seller,
                    StatusId = y.StatusId,
                }).ToList();

                #endregion

                #region Lấy loại tiền

                // Lấy loại tiền
                var leadMoneyTypeId = listCategoryTypeCommon.FirstOrDefault(x => x.CategoryTypeCode == "DTI").CategoryTypeId;

                var moneyList = listCategoryCommon.Where(w => w.CategoryTypeId == leadMoneyTypeId).Select(w => new CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                #endregion

                return new GetMasterDataBillSaleCreateEditResult()
                {
                    Status = true,
                    BillSale = billSaleResult,
                    ListStatus = statusBill,
                    ListBanking = listTypeBank,
                    ListCustomer = listCustomer,
                    ListEmployee = listEmployee,
                    Order = orderResult,
                    ListOrder = listOrderResult,
                    ListNote = listNote,
                    ListInventoryDeliveryVoucher = listInventoryDeliveryVoucher,
                    ListMoney = moneyList
                };
            }
            catch (Exception)
            {
                return new GetMasterDataBillSaleCreateEditResult()
                {
                    Status = false,
                    Message = CommonMessage.BillSale.GET_FAIL
                };
            }
        }

        public GetOrderByOrderIdResult GetOrderByOrderId(GetOrderByOrderIdParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetOrderByOrderIdResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_USER
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetOrderByOrderIdResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_EMPLOYEE
                    };
                }

                if (parameter.OrderId == null || parameter.OrderId == Guid.Empty)
                {
                    return new GetOrderByOrderIdResult()
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.GET_FAIL
                    };

                }

                var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                if (order == null)
                {
                    return new GetOrderByOrderIdResult()
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.GET_FAIL
                    };
                }

                // Lấy thông tin tab chi tiết sản phẩm dịch vụ
                var listProduct = context.CustomerOrderDetail.Where(x => x.OrderId == order.OrderId && x.Active == true).ToList();
                var listProductId = listProduct.Select(x => x.OrderDetailId).ToList();

                var listProductDetailAttr = context.OrderProductDetailProductAttributeValue.Where(x => listProductId.Contains(x.OrderDetailId)).ToList();
                var listProductResult = new List<BillSaleDetailEntityModel>();
                listProduct.ForEach(item =>
                {
                    var product = new BillSaleDetailEntityModel();
                    product.OrderDetailType = item.OrderDetailType;
                    product.ProductName = item.ProductName;
                    product.ProductId = item.ProductId;
                    product.Quantity = item.Quantity;
                    product.UnitId = item.UnitId;
                    product.UnitPrice = item.UnitPrice;
                    product.Vat = item.Vat;
                    product.VendorId = item.VendorId;
                    product.WarehouseId = item.WarehouseId;
                    product.OrderId = item.OrderId;
                    product.OrderDetailId = item.OrderDetailId;
                    product.OrderId = item.OrderId;
                    product.MoneyForGoods = SumAmount(item.Quantity, item.UnitPrice, item.ExchangeRate, item.Vat, item.DiscountValue,
                                                    item.DiscountType);
                    product.ActualInventory = item.ActualInventory;
                    product.BusinessInventory = item.BusinessInventory;
                    product.CurrencyUnit = item.CurrencyUnit;
                    product.Description = item.Description;
                    product.DiscountType = item.DiscountType;
                    product.DiscountValue = item.DiscountValue;
                    product.ExchangeRate = item.ExchangeRate;
                    product.IncurredUnit = item.IncurredUnit;
                    product.ListBillSaleDetailProductAttribute = new List<BillSaleDetailProductAttributeEntityModel>();
                    product.UnitId = item.UnitId;
                    product.UnitPrice = item.UnitPrice;
                    product.Vat = item.Vat;
                    product.VendorId = item.VendorId;
                    product.WarehouseId = item.WarehouseId;
                    var listAttrProduct = listProductDetailAttr.Where(x => x.OrderDetailId == item.OrderDetailId).ToList();
                    listAttrProduct.ForEach(attr =>
                    {
                        var tempAttr = new BillSaleDetailProductAttributeEntityModel();
                        tempAttr.OrderDetailId = item.OrderDetailId;
                        tempAttr.ProductId = attr.ProductId;
                        tempAttr.ProductAttributeCategoryId = attr.ProductAttributeCategoryId;
                        tempAttr.ProductAttributeCategoryValueId = attr.ProductAttributeCategoryValueId;
                        product.ListBillSaleDetailProductAttribute.Add(tempAttr);
                    });

                    listProductResult.Add(product);
                });

                var listOrderDetaillId = listProductResult.Select(x => x.OrderDetailId).ToList();
                var listAttr = context.OrderProductDetailProductAttributeValue.Where(x => listOrderDetaillId.Contains(x.OrderDetailId))
                    .Select(y => new BillSaleDetailProductAttributeEntityModel()
                    {
                        OrderProductDetailProductAttributeValueId = y.OrderProductDetailProductAttributeValueId,
                        OrderDetailId = y.OrderDetailId,
                        ProductAttributeCategoryId = y.ProductAttributeCategoryId,
                        ProductAttributeCategoryValueId = y.ProductAttributeCategoryValueId,
                        ProductId = y.ProductId
                    }).ToList();


                var listUnitId = listProductResult.Select(x => x.UnitId).ToList();
                var listUnit = context.Category.Where(x => listUnitId.Contains(x.CategoryId)).ToList();
                // Lấy danh sách nhà cung cấp
                var listVendorId = listProductResult.Select(x => x.VendorId).ToList();
                var listVendor = context.Vendor.Where(x => listVendorId.Contains(x.VendorId)).ToList();
                // Lấy danh sản phẩm
                var listProductTempId = listProductResult.Select(x => x.ProductId).ToList();
                var listProductTemp = context.Product.Where(x => listProductTempId.Contains(x.ProductId)).ToList();
                // Lấy danh sách kho
                var listWarehouseId = listProductResult.Select(x => x.WarehouseId).ToList();
                var listWarehouse = context.Warehouse.Where(x => listWarehouseId.Contains(x.WarehouseId)).ToList();

                listProductResult?.ForEach(item =>
                {
                    var listAttrDetail = listAttr.Where(x => x.BillOfSaleDetailId == item.BillOfSaleDetailId).ToList();
                    item.ListBillSaleDetailProductAttribute = new List<BillSaleDetailProductAttributeEntityModel>();
                    listAttrDetail.AddRange(listAttrDetail);

                    var unit = listUnit.FirstOrDefault(x => x.CategoryId == item.UnitId);
                    item.UnitName = unit?.CategoryName;

                    var vendor = listVendor.FirstOrDefault(x => x.VendorId == item.VendorId);
                    item.VendorName = vendor?.VendorName;

                    var product = listProductTemp.FirstOrDefault(x => x.ProductId == item.ProductId);
                    item.ProductCode = product?.ProductCode;

                    var warehouse = listWarehouse.FirstOrDefault(x => x.WarehouseId == item.WarehouseId);
                    item.WarehouseCode = warehouse?.WarehouseName;
                });

                var listCostResult = new List<BillSaleCostEntityModel>();
                var costCommon = context.Cost.ToList();
                // Lấy thông tin tab chi phí
                var listCost = context.OrderCostDetail.Where(x => x.OrderId == parameter.OrderId && x.Active == true).ToList();
                listCost.ForEach(item =>
                {
                    var tempCost = new BillSaleCostEntityModel();
                    var cost = costCommon.FirstOrDefault(x => x.CostId == item.CostId);
                    tempCost.OrderId = item.OrderId;
                    tempCost.CostCode = cost?.CostCode;
                    tempCost.CostName = cost?.CostName;
                    tempCost.Quantity = item.Quantity;
                    tempCost.OrderId = item.OrderId;
                    tempCost.UnitPrice = item.UnitPrice;

                    listCostResult.Add(tempCost);
                });

                var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listAllUser = context.User.Where(x => x.Active == true).ToList();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();
                var eployeeUser = new List<EmployeeModel>();
                eployeeUser = listAllEmployee.Select(item => new EmployeeModel
                {
                    EmployeeId = item.EmployeeId,
                    EmployeeName = item.EmployeeName,
                    UserId = listAllUser.FirstOrDefault(u => u.EmployeeId == item.EmployeeId).UserId
                }).ToList();

                #region Lấy thông tin giao hàng

                var listInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.Where(l => l.ObjectId == parameter.OrderId)
                    .Select(l => new InventoryDeliveryVoucherEntityModel
                    {
                        InventoryDeliveryVoucherId = l.InventoryDeliveryVoucherId,
                        InventoryDeliveryVoucherCode = l.InventoryDeliveryVoucherCode,
                        StatusId = l.StatusId,
                        InventoryDeliveryVoucherType = l.InventoryDeliveryVoucherType,
                        WarehouseId = l.WarehouseId,
                        ObjectId = l.ObjectId,
                        Receiver = l.Receiver,
                        Reason = l.Reason,
                        InventoryDeliveryVoucherDate = l.InventoryDeliveryVoucherDate,
                        InventoryDeliveryVoucherTime = l.InventoryDeliveryVoucherTime,
                        LicenseNumber = l.LicenseNumber,
                        Active = l.Active,
                        CreatedDate = l.CreatedDate,
                        CreatedById = l.CreatedById,
                        UpdatedDate = l.UpdatedDate,
                        UpdatedById = l.UpdatedById,
                        NameCreate = eployeeUser.FirstOrDefault(em => em.UserId == l.CreatedById).EmployeeName,
                        NameStatus = listCategory.FirstOrDefault(ca => ca.CategoryId == l.StatusId).CategoryName,
                    }).ToList();

                #endregion

                return new GetOrderByOrderIdResult()
                {
                    Status = true,
                    Order = GetOrder(parameter.OrderId),
                    ListInventoryDeliveryVoucher = listInventoryDeliveryVoucher,
                    ListCost = listCostResult,
                    ListBillSaleDetail = listProductResult
                };
            }
            catch (Exception)
            {
                return new GetOrderByOrderIdResult()
                {
                    Status = false,
                    Message = CommonMessage.BillSale.GET_FAIL
                };
            }
        }

        public OrderBillEntityModel GetOrder(Guid? oderId)
        {
            var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == oderId);
            if (order == null) return null;

            var orderResult = new OrderBillEntityModel();
            orderResult.OrderId = order.OrderId;
            orderResult.OrderCode = order.OrderCode;
            orderResult.OrderDate = order.OrderDate;
            orderResult.CustomerId = order.CustomerId;
            var customer = context.Customer.FirstOrDefault(x => x.CustomerId == orderResult.CustomerId);
            orderResult.CustomerCode = customer?.CustomerCode;
            orderResult.CustomerName = customer?.CustomerName;

            var listOrderDetail = context.CustomerOrderDetail.Where(x => x.OrderId == order.OrderId).ToList();
            orderResult.TotalQuantity = listOrderDetail.Sum(x => x.Quantity);
            orderResult.TotalOrder = 0;
            listOrderDetail.ForEach(item =>
            {
                orderResult.TotalOrder = orderResult.TotalOrder + SumAmount(item.Quantity, item.UnitPrice, item.Vat, item.DiscountValue, item.DiscountType, item.ExchangeRate);
            });

            return orderResult;
        }



        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? Vat, decimal? DiscountValue, bool? DiscountType, decimal? ExChangeRate)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;
            decimal SumAmount = Quantity.Value * UnitPrice.Value * ExChangeRate.Value;

            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = ((SumAmount * DiscountValue.Value) / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }
            result = SumAmount - CacuDiscount;
            if (Vat != null)
            {
                CaculateVAT = (result * Vat.Value) / 100;
            }
            result = result + CaculateVAT;
            return result;
        }


        public GetMasterBillOfSaleResult GetMasterBillOfSale(GetMasterBillOfSaleParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetMasterBillOfSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_USER
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetMasterBillOfSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_EMPLOYEE
                    };
                }

                var listProduct = context.Product.Where(x => x.Active == true).ToList();
                var categoryTypeBill = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "BILL" && ct.Active == true);
                var ListStatus = context.Category.Where(c => c.CategoryTypeId == categoryTypeBill.CategoryTypeId && c.Active == true).ToList();

                return new GetMasterBillOfSaleResult()
                {
                    ListProduct = listProduct,
                    ListStatus = ListStatus,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new GetMasterBillOfSaleResult()
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }


        public SearchBillOfSaleResult SearchBillOfSale(SearchBillOfSaleParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new SearchBillOfSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_USER
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchBillOfSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_EMPLOYEE
                    };
                }
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var billDetail = context.BillOfSaleDetail.Where(d => d.Active == true).ToList();

                //check isManager
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                var orderCode = parameter.OrderCode == null ? "" : parameter.OrderCode.Trim();
                var billOfSaleCode = parameter.BillOfSaleCode == null ? "" : parameter.BillOfSaleCode.Trim();
                var customerName = parameter.CustomerName == null ? "" : parameter.CustomerName.Trim();
                var fromDate = parameter.FromDate;
                var toDate = parameter.ToDate;
                var listStatusId = parameter.ListStatusId;
                var categoryTypeBill = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "BILL" && ct.Active == true);
                var listStatus = context.Category.Where(c => c.CategoryTypeId == categoryTypeBill.CategoryTypeId && c.Active == true).ToList();

                // Lấy tất cả đơn hàng theo orderCode
                var listOrderId = context.CustomerOrder.Where(o => o.Active == true && (orderCode == "" || o.OrderCode.Contains(orderCode))).Select(o => o.OrderId).ToList();

                // Lấy tất cả các hóa đơn có trong list sản phẩm Id
                var listBillOfSaleId = billDetail.Where(p => p.Active == true &&
                            (parameter.ListProductId == null || parameter.ListProductId.Count == 0 || parameter.ListProductId.Contains(p.ProductId.Value)))
                    .Select(p => p.BillOfSaleId).ToList();

                var lstOrderBy = context.BillOfSale.Where(x => x.Active == true &&
                            (customerName == "" || x.CustomerName.ToLower().Contains(customerName.ToLower())) &&
                            (billOfSaleCode == "" || x.BillOfSaLeCode.ToLower().Contains(billOfSaleCode.ToLower())) &&
                            (parameter.ListProductId == null || parameter.ListProductId.Count == 0 || listBillOfSaleId.Contains(x.BillOfSaLeId)) &&
                            (listStatusId == null || listStatusId.Count == 0 || listStatusId.Contains(x.StatusId.Value)) &&
                            (listOrderId == null || listOrderId.Count == 0 || listOrderId.Contains(x.OrderId)) &&
                            (fromDate == null || fromDate == DateTime.MinValue || fromDate.Value.Date <= x.BillDate.Value.Date) &&
                            (toDate == null || toDate == DateTime.MinValue || toDate.Value.Date >= x.BillDate.Value.Date))
                    .Select(x => new BillSaleEntityModel
                    {
                        BillOfSaLeId = x.BillOfSaLeId,
                        BillOfSaLeCode = x.BillOfSaLeCode,
                        OrderId = x.OrderId,
                        BillDate = x.BillDate,
                        EndDate = x.EndDate,
                        StatusId = x.StatusId,
                        TermsOfPaymentId = x.TermsOfPaymentId,
                        CustomerId = x.CustomerId,
                        CustomerName = x.CustomerName,
                        DebtAccountId = x.DebtAccountId,
                        Mst = x.Mst,
                        PaymentMethodId = x.PaymentMethodId,
                        EmployeeId = x.EmployeeId,
                        Description = x.Description,
                        Note = x.Note,
                        AccountBankId = x.AccountBankId,
                        Active = x.Active,
                        CreatedById = x.CreatedById,
                        CreatedDate = x.CreatedDate,
                        UpdatedById = x.UpdatedById,
                        UpdatedDate = x.UpdatedDate,
                        InvoiceSymbol = x.InvoiceSymbol,
                        Amount = 0,
                        StatusName = (x.StatusId == null || x.StatusId == Guid.Empty) ? "" : listStatus.FirstOrDefault(c => c.CategoryId == x.StatusId.Value) == null ? "" : listStatus.FirstOrDefault(c => c.CategoryId == x.StatusId.Value).CategoryName,
                        Seller = (x.EmployeeId == null || x.EmployeeId == Guid.Empty) ? "" : listAllEmployee.FirstOrDefault(c => c.EmployeeId == x.EmployeeId.Value) == null ? "" : listAllEmployee.FirstOrDefault(c => c.EmployeeId == x.EmployeeId.Value).EmployeeName,
                    }).OrderByDescending(x => x.BillDate).ToList();

                lstOrderBy.ForEach(item =>
                {
                    var detail = billDetail.Where(d => d.Active == true && d.BillOfSaleId == item.BillOfSaLeId).ToList();
                    item.Amount = 0;
                    detail.ForEach(de =>
                    {
                        var quantity = de.Quantity == null ? 0 : de.Quantity.Value;
                        var unitPrice = de.UnitPrice == null ? 0 : de.UnitPrice.Value;
                        var exchangeRate = de.ExchangeRate == null ? 0 : de.ExchangeRate.Value;
                        var discountValue = de.DiscountValue == null ? 0 : de.DiscountValue.Value;
                        var vat = de.Vat == null ? 0 : de.Vat.Value;
                        var moneyRecord = quantity * unitPrice * exchangeRate;
                        var discountMoney = de.DiscountType == true ? (moneyRecord * discountValue / 100) : discountValue;
                        var vatMoney = (moneyRecord - discountMoney) * vat / 100;
                        item.Amount = item.Amount + (moneyRecord - discountMoney + vatMoney);
                    });
                });

                return new SearchBillOfSaleResult()
                {
                    ListBillOfSale = lstOrderBy,
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new SearchBillOfSaleResult()
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }

        public UpdateStatusResult UpdateStatus(UpdateStatusParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new UpdateStatusResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_USER
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new UpdateStatusResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_EMPLOYEE
                    };
                }

                var billSaleUpdate = context.BillOfSale.FirstOrDefault(x => x.Active == true && x.BillOfSaLeId == parameter.BillSaleId);
                if (billSaleUpdate == null)
                {
                    return new UpdateStatusResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_FOUND_BILL
                    };
                }

                var categoryTypeBill = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "BILL" && ct.Active == true);
                var listStatus = context.Category.Where(c => c.CategoryTypeId == categoryTypeBill.CategoryTypeId && c.Active == true).ToList();
                var statusUpdate = listStatus.FirstOrDefault(x => x.CategoryId == parameter.StatusId);
                if (statusUpdate == null)
                {
                    return new UpdateStatusResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_FOUND_STATUS_BILL
                    };
                }
                var noteResult = new NoteEntityModel();
                if (statusUpdate.CategoryCode == "NEW")
                {
                    var isUpdate = listStatus.FirstOrDefault(x => x.CategoryCode == "CANC" && billSaleUpdate.StatusId == x.CategoryId) != null;
                    if (isUpdate)
                    {
                        billSaleUpdate.StatusId = parameter.StatusId;
                        var note = new Note()
                        {
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            ObjectId = billSaleUpdate.BillOfSaLeId,
                            Description = parameter.Note,
                            NoteId = Guid.NewGuid(),
                            NoteTitle = "dã từ đặt hóa đơn về nháp",
                            ObjectType = "BILL",
                            Type = "ADD"
                        };

                        context.Note.Add(note);
                        noteResult.NoteId = note.NoteId;
                        noteResult.Description = note.Description;
                        noteResult.Type = note.Type;
                        noteResult.ObjectId = note.ObjectId;
                        noteResult.ObjectType = note.ObjectType;
                        noteResult.NoteTitle = note.NoteTitle;
                        noteResult.Active = note.Active;
                        noteResult.CreatedById = note.CreatedById;
                        noteResult.CreatedDate = note.CreatedDate;
                        noteResult.UpdatedById = note.UpdatedById;
                        noteResult.UpdatedDate = note.UpdatedDate;
                        noteResult.ResponsibleName = "";
                        noteResult.ResponsibleAvatar = "";
                        noteResult.NoteDocList = new List<NoteDocumentEntityModel>();
                    }
                    else
                    {
                        return new UpdateStatusResult
                        {
                            Status = false,
                            Message = CommonMessage.BillSale.EDIT_FAIL
                        };
                    }
                }
                else if (statusUpdate.CategoryCode == "CANC")
                {
                    var isUpdate = listStatus.FirstOrDefault(x => x.CategoryCode == "NEW" && billSaleUpdate.StatusId == x.CategoryId) != null;
                    if (isUpdate)
                    {
                        billSaleUpdate.StatusId = parameter.StatusId;
                        var note = new Note()
                        {
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            ObjectId = billSaleUpdate.BillOfSaLeId,
                            Description = parameter.Note,
                            NoteId = Guid.NewGuid(),
                            NoteTitle = "dã hủy hóa đơn",
                            ObjectType = "BILL",
                            Type = "ADD"
                        };

                        context.Note.Add(note);
                        noteResult.NoteId = note.NoteId;
                        noteResult.Description = note.Description;
                        noteResult.Type = note.Type;
                        noteResult.ObjectId = note.ObjectId;
                        noteResult.ObjectType = note.ObjectType;
                        noteResult.NoteTitle = note.NoteTitle;
                        noteResult.Active = note.Active;
                        noteResult.CreatedById = note.CreatedById;
                        noteResult.CreatedDate = note.CreatedDate;
                        noteResult.UpdatedById = note.UpdatedById;
                        noteResult.UpdatedDate = note.UpdatedDate;
                        noteResult.ResponsibleName = "";
                        noteResult.ResponsibleAvatar = "";
                        noteResult.NoteDocList = new List<NoteDocumentEntityModel>();
                    }
                    else
                    {
                        return new UpdateStatusResult
                        {
                            Status = false,
                            Message = CommonMessage.BillSale.EDIT_FAIL
                        };
                    }
                }
                else if (statusUpdate.CategoryCode == "CONFIRM")
                {
                    var isUpdate = listStatus.FirstOrDefault(x => x.CategoryCode == "NEW" && billSaleUpdate.StatusId == x.CategoryId) != null;
                    if (isUpdate)
                    {
                        billSaleUpdate.StatusId = parameter.StatusId;
                        var note = new Note()
                        {
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            ObjectId = billSaleUpdate.BillOfSaLeId,
                            Description = parameter.Note,
                            NoteId = Guid.NewGuid(),
                            NoteTitle = "dã xác nhận hóa đơn",
                            ObjectType = "BILL",
                            Type = "ADD"
                        };

                        context.Note.Add(note);
                        noteResult.NoteId = note.NoteId;
                        noteResult.Description = note.Description;
                        noteResult.Type = note.Type;
                        noteResult.ObjectId = note.ObjectId;
                        noteResult.ObjectType = note.ObjectType;
                        noteResult.NoteTitle = note.NoteTitle;
                        noteResult.Active = note.Active;
                        noteResult.CreatedById = note.CreatedById;
                        noteResult.CreatedDate = note.CreatedDate;
                        noteResult.UpdatedById = note.UpdatedById;
                        noteResult.UpdatedDate = note.UpdatedDate;
                        noteResult.ResponsibleName = "";
                        noteResult.ResponsibleAvatar = "";
                        noteResult.NoteDocList = new List<NoteDocumentEntityModel>();
                    }
                    else
                    {
                        return new UpdateStatusResult
                        {
                            Status = false,
                            Message = CommonMessage.BillSale.EDIT_FAIL
                        };
                    }
                }

                context.SaveChanges();
                return new UpdateStatusResult()
                {
                    Note = noteResult,
                    Message = CommonMessage.BillSale.EDIT_SUCCESS,
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new UpdateStatusResult()
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }

        public DeleteBillSaleResult DeleteBillSale(DeleteBillSaleParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new DeleteBillSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_USER
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new DeleteBillSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_EMPLOYEE
                    };
                }

                var billSaleUpdate = context.BillOfSale.FirstOrDefault(x => x.Active == true && x.BillOfSaLeId == parameter.BillSaleId);
                if (billSaleUpdate == null)
                {
                    return new DeleteBillSaleResult
                    {
                        Status = false,
                        Message = CommonMessage.BillSale.NOT_FOUND_BILL
                    };
                }

                billSaleUpdate.Active = false;
                context.BillOfSale.Update(billSaleUpdate);
                context.SaveChanges();

                return new DeleteBillSaleResult()
                {
                    Message = CommonMessage.BillSale.DELETE_SUCCESS,
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new DeleteBillSaleResult()
                {
                    Message = e.Message,
                    Status = false
                };
            }
        }

        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? ExchangeRate, decimal? Vat, decimal? DiscountValue, bool? DiscountType)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;

            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value * DiscountValue.Value) / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }
            if (Vat != null)
            {
                CaculateVAT = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value - CacuDiscount) * Vat.Value) / 100;
            }
            result = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value) + CaculateVAT - CacuDiscount;
            return result;
        }
    }
}
