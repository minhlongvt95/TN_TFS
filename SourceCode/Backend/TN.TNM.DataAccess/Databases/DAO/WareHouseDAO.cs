using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Messages.Results.WareHouse;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class WareHouseDAO : BaseDAO, IWareHouseDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public WareHouseDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment, ILogger<WareHouseDAO> _logger)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
            this.logger = _logger;
        }

        public CreateUpdateWareHouseResult CreateUpdateWareHouse(CreateUpdateWareHouseParameter parameter)
        {
            try
            {
                parameter.Warehouse.WarehouseCode = parameter.Warehouse.WarehouseCode.Trim();
                parameter.Warehouse.WarehouseName = parameter.Warehouse.WarehouseName.Trim();
                parameter.Warehouse.WarehouseAddress = parameter.Warehouse.WarehouseAddress?.Trim();
                parameter.Warehouse.WarehousePhone = parameter.Warehouse.WarehousePhone?.Trim();
                parameter.Warehouse.WarehouseDescription = parameter.Warehouse.WarehouseDescription?.Trim();

                if (parameter.Warehouse.WarehouseId == Guid.Empty)
                {
                    //Tạo mới
                    parameter.Warehouse.WarehouseId = Guid.NewGuid();
                    parameter.Warehouse.CreatedById = parameter.Warehouse.CreatedById;
                    parameter.Warehouse.CreatedDate = DateTime.Now;
                    parameter.Warehouse.UpdatedById = null;
                    parameter.Warehouse.UpdatedDate = null;
                    context.Warehouse.Add(parameter.Warehouse);
                    context.SaveChanges();
                }
                else
                {
                    //Sửa               
                    parameter.Warehouse.UpdatedById = parameter.Warehouse.UpdatedById;
                    parameter.Warehouse.UpdatedDate = DateTime.Now;
                    context.Warehouse.Update(parameter.Warehouse);
                    context.SaveChanges();
                }
                return new CreateUpdateWareHouseResult()
                {
                    Status = true,
                    Message = "Tạo mới/Cập nhật thành công",
                    WarehouseId = parameter.Warehouse.WarehouseId
                };
            }
            catch (Exception e)
            {
                return new CreateUpdateWareHouseResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchWareHouseResult SearchWareHouse(SearchWareHouseParameter parameter)
        {
            try
            {
                var listWareHouseEntity = context.Warehouse.Where(w => w.Active == true).ToList();
                var listWarehouseId = listWareHouseEntity.Select(w => w.WarehouseId).ToList(); // danh sách kho ID
                var listStorageKeeperId = listWareHouseEntity.Where(w => w.Storagekeeper != null).Select(w => w.Storagekeeper).ToList();//danh sách thủ kho ID
                var listEmployeeEntity = context.Employee.Where(w => listStorageKeeperId.Contains(w.EmployeeId)).ToList();
                var listInventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.Where(w => listWarehouseId.Contains(w.WarehouseId)).ToList(); //nhập kho theo từng sản phẩm
                var listInventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping.Where(w => listWarehouseId.Contains(w.WarehouseId)).ToList(); //xuất kho theo sản phẩm
                var listInventoryReportEntity = context.InventoryReport.Where(w => listWarehouseId.Contains(w.WarehouseId)).ToList();//Tồn kho                                                                                                                               

                #region Kiểm tra phiếu nhập kho có trạng thái Nháp
                var statusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TPH").CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(f => f.CategoryTypeId == statusTypeId && f.CategoryCode == "NHA").CategoryId; // Id trạng thái nháp
                var listReceivingVoucher = context.InventoryReceivingVoucher.Where(w => w.StatusId == statusId).Select(w => w.InventoryReceivingVoucherId);
                var listReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.Where(w => listReceivingVoucher.Contains(w.InventoryReceivingVoucherId)).ToList(); //danh sách chi tiết phiếu nhập kho có trạng thái (của phiếu tổng) là nháp  
                #endregion

                var listWareHouse = new List<WareHouseEntityModel>();

                listWareHouseEntity.ForEach(item =>
                {
                    //thêm tên thủ kho
                    var storagekeeperName = "";
                    var storagekeeper = listEmployeeEntity.Where(w => w.EmployeeId == item.Storagekeeper).FirstOrDefault();
                    if (storagekeeper != null)
                    {
                        storagekeeperName = storagekeeper.EmployeeName;
                    }
                    //kiểm tra có kho con hay là không
                    var hasChild = false;
                    var childWarehouse = listWareHouseEntity.Where(w => w != item && w.WarehouseParent == item.WarehouseId).FirstOrDefault();
                    if (childWarehouse != null)
                    {
                        hasChild = true;
                    }
                    //kiểm tra điều kiện thêm kho con
                    var canAddChild = true;
                    var canRemove = true;

                    var isHasReceivingVoucher = listInventoryReceivingVoucherMapping.Where(w => w.WarehouseId == item.WarehouseId).FirstOrDefault();
                    var isHasDeliveryVoucher = listInventoryDeliveryVoucherMapping.Where(w => w.WarehouseId == item.WarehouseId).FirstOrDefault();
                    var isHasInventoryReport = listInventoryReportEntity.Where(w => w.WarehouseId == item.WarehouseId).FirstOrDefault();

                    var isHasReceivingVoucherMapping = listReceivingVoucherMapping.FirstOrDefault(w => w.WarehouseId == item.WarehouseId);

                    var inventoryReportByWarehouse = listInventoryReportEntity.Where(w => w.WarehouseId == item.WarehouseId)
                                                                              .GroupBy(w => w.WarehouseId)
                                                                              .Select(s =>
                                                                               new
                                                                               {
                                                                                   SumQuanty = s.Sum(sum => sum.Quantity),
                                                                                   SumStartQuantity = s.Sum(sum => sum.StartQuantity),
                                                                               }).FirstOrDefault();
                    //tồn kho = số lượng + tồn kho đầu kỳ
                    decimal? _inventory = 0;
                    //= inventoryReportByWarehouse.SumQuanty + inventoryReportByWarehouse.SumStartQuantity;

                    if (inventoryReportByWarehouse != null)
                    {
                        _inventory = inventoryReportByWarehouse.SumQuanty + inventoryReportByWarehouse.SumStartQuantity ?? 0;
                    }
                    //điều kiện không thể thêm kho: số lượng tồn kho > 0 hoặc tồn tại phiếu nhập kho ở trạng thái nháp
                    if (isHasReceivingVoucherMapping != null || _inventory > 0)
                    {
                        canAddChild = false;
                    }
                    //điều kiện không thể xóa kho: có con hoặc tồn tại phiếu nhập hoăc tồn tại phiếu xuất hoặc tồn tại tồn kho
                    if (hasChild == true || isHasReceivingVoucher != null || isHasDeliveryVoucher != null || isHasInventoryReport != null)
                    {
                        canRemove = false;
                    }

                    var temp = new WareHouseEntityModel()
                    {
                        WarehouseId = item.WarehouseId,
                        WarehouseCode = item.WarehouseCode,
                        WarehouseName = item.WarehouseName,
                        WarehouseParent = item.WarehouseParent,
                        WarehouseParentName = null,
                        WarehouseAddress = item.WarehouseAddress,
                        WarehousePhone = item.WarehousePhone,
                        Storagekeeper = item.Storagekeeper,
                        StoragekeeperName = storagekeeperName,
                        HasChild = hasChild,
                        WarehouseDescription = item.WarehouseDescription,
                        Active = item.Active,
                        CreatedDate = item.CreatedDate,
                        CreatedById = item.CreatedById,
                        UpdatedDate = item.UpdatedDate,
                        UpdatedById = item.UpdatedById,
                        TenantId = item.TenantId,
                        CanAddChild = canAddChild,
                        CanRemove = canRemove,
                    };
                    listWareHouse.Add(temp);

                });

                listWareHouse = listWareHouse.OrderBy(w => w.WarehouseName).ToList();

                return new SearchWareHouseResult()
                {
                    Status = true,
                    Message = "",
                    listWareHouse = listWareHouse
                };
            }
            catch (Exception e)
            {
                return new SearchWareHouseResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetWareHouseChaResult GetWareHouseCha(GetWareHouseChaParameter parameter)
        {
            try
            {
                /*Do something..*/
                var listWareHouse = context.Warehouse.Where(x => x.Active)
                    .OrderBy(x => x.WarehouseName).ToList();
                /*End*/

                return new GetWareHouseChaResult()
                {
                    Status = true,
                    Message = "",
                    listWareHouse = listWareHouse
                };
            }
            catch (Exception e)
            {
                return new GetWareHouseChaResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetVendorOrderByVendorIdResult GetVendorOrderByVendorId(GetVendorOrderByVendorIdParameter parameter)
        {
            try
            {
                //Lấy Id các trạng thái đơn hàng: Đơn hàng mua
                var listStatusCode = new List<string>() { "PURC" };
                var listStatusId = context.PurchaseOrderStatus
                    .Where(ct => listStatusCode.Contains(ct.PurchaseOrderStatusCode) && ct.Active)
                    .Select(ct => ct.PurchaseOrderStatusId).ToList();

                #region Lấy list Đơn hàng mua

                var listVendorOrder = context.VendorOrder
                    .Where(x => x.Active == true && listStatusId.Contains(x.StatusId) &&
                                x.VendorId == parameter.VendorId)
                    .Select(y => new VendorOrderEntityModel
                    {
                        VendorOrderId = y.VendorOrderId,
                        VendorOrderCode = y.VendorOrderCode,
                        Amount = y.Amount,
                        Description = y.Description,
                        VendorDescripton = "",
                        VendorId = y.VendorId
                    }).ToList();

                var listAllVendor = context.Vendor.ToList();
                listVendorOrder.ForEach(item =>
                {
                    var vendor = listAllVendor.FirstOrDefault(x => x.VendorId == item.VendorId);
                    var vendorName = "";

                    if (vendor != null)
                    {
                        vendorName = vendor.VendorName;
                    }

                    item.VendorDescripton = item.VendorOrderCode + " - " + vendorName + " - " + item.Description +
                                            " - " + item.Amount.ToString("#,#");
                });

                #endregion

                return new GetVendorOrderByVendorIdResult()
                {
                    Status = true,
                    Message = "",
                    ListVendorOrder = listVendorOrder
                };
            }
            catch (Exception e)
            {
                return new GetVendorOrderByVendorIdResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetVendorOrderDetailByVenderOrderIdResult GetVendorOrderDetailByVenderOrderId(GetVendorOrderDetailByVenderOrderIdParameter parameter)
        {
            try
            {
                var vendorOrders = context.VendorOrder.Where(vo => vo.Active == true).ToList();
                var vendorOrderDetails = context.VendorOrderDetail.Where(vo => vo.Active == true).ToList();
                var categoryTypeIdUnit = context.CategoryType.FirstOrDefault(cty => cty.Active == true && cty.CategoryTypeCode == "DNH").CategoryTypeId;
                var categories = context.Category.Where(ct => ct.Active == true && ct.CategoryTypeId == categoryTypeIdUnit).ToList();
                var product = context.Product.Where(p => p.Active == true).ToList();
                var warehouseSerial = context.Serial.Where(p => p.Active == true).ToList();
                var result = new List<GetVendorOrderDetailByVenderOrderIdEntityModel>();
                //TypeWarehouseVocher =1:phieu nhap kho
                if (parameter.TypeWarehouseVocher == 1)
                {
                    var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                    var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;

                    //Lọc những đơn hàng đã nhập kho hết
                    var listInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(wh => wh.StatusId == categoryIdNHK).Select(s => s.InventoryReceivingVoucherId).ToList();
                    var inventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.Where(wh => listInventoryReceivingVoucher.Contains(wh.InventoryReceivingVoucherId)).ToList();

                    parameter.ListVendorOrderId.ForEach(item =>
                    {
                        var vendorOrderDetail = vendorOrderDetails.Where(vod => vod.VendorOrderId == item).ToList();
                        vendorOrderDetail.ForEach(detail =>
                        {
                            if (detail.ProductId != null)
                            {
                                decimal quantityInventoryReceiving = inventoryReceivingVoucherMapping.Where(i => i.ProductId == detail.ProductId && i.ObjectId == detail.VendorOrderDetailId).Sum(i => i.QuantityActual);
                                if (quantityInventoryReceiving < detail.Quantity)
                                {
                                    GetVendorOrderDetailByVenderOrderIdEntityModel obj = new GetVendorOrderDetailByVenderOrderIdEntityModel();
                                    obj.VendorOrderId = item;
                                    obj.VendorOrderDetailId = detail.VendorOrderDetailId;
                                    obj.VendorOrderCode = vendorOrders.FirstOrDefault(vo => vo.VendorOrderId == item).VendorOrderCode;
                                    obj.ProductId = detail.ProductId;
                                    obj.ProductName = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductName;
                                    obj.ProductCode = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductCode;
                                    obj.UnitId = detail.UnitId;
                                    obj.UnitName = detail.UnitId == null ? "" : categories.FirstOrDefault(c => c.CategoryId == detail.UnitId).CategoryName;
                                    obj.QuantityRequire = detail.Quantity - quantityInventoryReceiving;
                                    obj.Quantity = detail.Quantity - quantityInventoryReceiving;
                                    obj.Note = "";
                                    obj.TotalSerial = 0;
                                    obj.Price = detail.UnitPrice;
                                    obj.ListSerial = new List<Serial>();
                                    obj.WareHouseId = Guid.Empty;
                                    obj.WareHouseName = "";
                                    obj.CurrencyUnit = detail.CurrencyUnit;
                                    obj.ExchangeRate = detail.ExchangeRate;
                                    obj.Vat = detail.Vat;
                                    obj.DiscountType = detail.DiscountType;
                                    obj.DiscountValue = detail.DiscountValue;
                                    obj.SumAmount = SumAmount(detail.Quantity, detail.UnitPrice, detail.ExchangeRate, detail.Vat, detail.DiscountValue, detail.DiscountType);
                                    obj.NameMoneyUnit = detail.CurrencyUnit != null ? context.Category.FirstOrDefault(c => c.CategoryId == detail.CurrencyUnit).CategoryName : "";
                                    result.Add(obj);
                                }
                            }
                        });

                    });
                }
                else
                {
                    var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX" && ct.Active == true).CategoryTypeId;
                    var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;
                    //Lọc những đơn hàng đã nhập kho hết
                    var listInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.Where(wh => wh.StatusId == categoryIdNHK).ToList();
                    var listInventoryDeliveryVoucherId = listInventoryDeliveryVoucher.Select(s => s.InventoryDeliveryVoucherId).ToList();
                    var inventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping.Where(wh => listInventoryDeliveryVoucherId.Contains(wh.InventoryDeliveryVoucherId)).ToList();

                    //var inventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.ToList();

                    parameter.ListVendorOrderId.ForEach(item =>
                    {
                        var vendorOrderDetail = vendorOrderDetails.Where(vod => vod.VendorOrderId == item).ToList();
                        vendorOrderDetail.ForEach(detail =>
                        {
                            if (detail.ProductId != null)
                            {

                                var lstInventoryDeliveryVoucherId = listInventoryDeliveryVoucher.Where(wh => wh.ObjectId == item).Select(s => s.InventoryDeliveryVoucherId).ToList();
                                decimal quantityInventoryReceiving = inventoryDeliveryVoucherMapping.Where(i => i.ProductId == detail.ProductId && lstInventoryDeliveryVoucherId.Contains(i.InventoryDeliveryVoucherId)).Sum(i => i.QuantityActual);

                                if (quantityInventoryReceiving < detail.Quantity)
                                {
                                    GetVendorOrderDetailByVenderOrderIdEntityModel obj = new GetVendorOrderDetailByVenderOrderIdEntityModel();
                                    obj.VendorOrderId = item;
                                    obj.VendorOrderDetailId = detail.VendorOrderDetailId;
                                    obj.VendorOrderCode = vendorOrders.FirstOrDefault(vo => vo.VendorOrderId == item).VendorOrderCode;
                                    obj.ProductId = detail.ProductId;
                                    obj.ProductName = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductName;
                                    obj.ProductCode = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductCode;
                                    obj.UnitId = detail.UnitId;
                                    obj.UnitName = detail.UnitId == null ? "" : categories.FirstOrDefault(c => c.CategoryId == detail.UnitId).CategoryName;
                                    obj.QuantityRequire = detail.Quantity - quantityInventoryReceiving;
                                    obj.Quantity = detail.Quantity - quantityInventoryReceiving;
                                    obj.Note = "";
                                    obj.TotalSerial = 0;
                                    obj.Price = detail.UnitPrice;
                                    obj.ListSerial = new List<Serial>();
                                    obj.WareHouseId = Guid.Empty;
                                    obj.WareHouseName = "";
                                    obj.CurrencyUnit = detail.CurrencyUnit;
                                    obj.ExchangeRate = detail.ExchangeRate;
                                    obj.Vat = detail.Vat;
                                    obj.DiscountType = detail.DiscountType;
                                    obj.DiscountValue = detail.DiscountValue;
                                    obj.SumAmount = SumAmount(detail.Quantity, detail.UnitPrice, detail.ExchangeRate, detail.Vat, detail.DiscountValue, detail.DiscountType);
                                    obj.NameMoneyUnit = detail.CurrencyUnit != null ? context.Category.FirstOrDefault(c => c.CategoryId == detail.CurrencyUnit).CategoryName : "";
                                    result.Add(obj);
                                }
                            }
                        });

                    });
                }

                /*End*/

                return new GetVendorOrderDetailByVenderOrderIdResult()
                {
                    Status = true,
                    Message = "",
                    ListOrderProduct = result
                };
            }
            catch (Exception e)
            {
                return new GetVendorOrderDetailByVenderOrderIdResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DownloadTemplateSerialResult DownloadTemplateSerial(DownloadTemplateSerialParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"4.Inv_ImportSerial.xlsx";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                string token = string.Empty;
                return new DownloadTemplateSerialResult
                {
                    ExcelFile = data,
                    Message = string.Format("Đã dowload file 4.Inv_ImportSerial"),
                    NameFile = token, //"4.Inv_ImportSerial",
                    Status = true
                };

            }
            catch (Exception)
            {
                return new DownloadTemplateSerialResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình download",
                    Status = false
                };
            }
        }

        public string ConverCreateId(int totalRecordCreate)
        {
            var datenow = DateTime.Now;
            string year = datenow.Year.ToString().Substring(datenow.Year.ToString().Length - 2, 2);
            string month = datenow.Month < 10 ? "0" + datenow.Month.ToString() : datenow.Month.ToString();
            string day = datenow.Day < 10 ? "0" + datenow.Day.ToString() : datenow.Day.ToString();
            string total = "";
            if (totalRecordCreate > 999)
            {
                total = totalRecordCreate.ToString();
            }
            else if (totalRecordCreate > 99 && totalRecordCreate < 1000)
            {
                total = "0" + totalRecordCreate.ToString();
            }
            else if (totalRecordCreate > 9 && totalRecordCreate < 100)
            {
                total = "00" + totalRecordCreate.ToString();
            }
            else
            {
                total = "000" + totalRecordCreate.ToString();
            }
            var result = year + month + day + total;

            return result;
        }

        public CreateOrUpdateInventoryVoucherResult CreateOrUpdateInventoryVoucher(CreateOrUpdateInventoryVoucherParameter parameter)
        {
            try
            {
                if (parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId == Guid.Empty)
                {

                    var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                    var categoryId = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHA" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;

                    var categoryTypeIdSerial = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TSE" && ct.Active == true).CategoryTypeId;
                    var categoryListSerial = context.Category.Where(ct => ct.CategoryTypeId == categoryTypeIdSerial && ct.Active == true).ToList();

                    //var inventoryReports = context.InventoryReport.Where(w => w.Active == true).ToList();

                    var datenow = DateTime.Now;
                    var totalInvertoryCreate = context.InventoryReceivingVoucher.Where(c => Convert.ToDateTime(c.CreatedDate).Day == datenow.Day && Convert.ToDateTime(c.CreatedDate).Month == datenow.Month && Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Count();
                    parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId = Guid.NewGuid();
                    parameter.inventoryReceivingVoucher.InventoryReceivingVoucherCode = "PN-" + ConverCreateId(totalInvertoryCreate + 1);
                    parameter.inventoryReceivingVoucher.StatusId = categoryId;
                    parameter.inventoryReceivingVoucher.ShiperName = !string.IsNullOrEmpty(parameter.inventoryReceivingVoucher.ShiperName) ? parameter.inventoryReceivingVoucher.ShiperName.Trim() : string.Empty;
                    parameter.inventoryReceivingVoucher.Active = true;
                    parameter.inventoryReceivingVoucher.CreatedDate = DateTime.Now;
                    parameter.inventoryReceivingVoucher.CreatedById = parameter.UserId;
                    context.InventoryReceivingVoucher.Add(parameter.inventoryReceivingVoucher);

                    var note = new Note();
                    note.ObjectId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId;
                    note.NoteId = Guid.NewGuid();
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.ObjectType = "WH";
                    note.Description = string.IsNullOrEmpty(parameter.noteContent) ? string.Empty : parameter.noteContent;
                    note.NoteTitle = "đã tạo";
                    note.Type = "ADD";
                    context.Note.Add(note);
                    if (parameter.fileList != null)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = hostingEnvironment.WebRootPath;

                        //upload file to Server
                        if (parameter.fileList != null && parameter.fileList.Count > 0)
                        {

                            string checkexistPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(checkexistPath))
                            {
                                Directory.CreateDirectory(checkexistPath);
                            }
                            foreach (IFormFile item in parameter.fileList)
                            {
                                if (item.Length > 0)
                                {
                                    string fileName = item.FileName.Trim();
                                    string fullPath = Path.Combine(checkexistPath, fileName);
                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        item.CopyTo(stream);
                                    }
                                }
                            }
                        }
                        // Add note
                        var noteAttach = new Note();
                        noteAttach.ObjectId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId;
                        noteAttach.NoteId = Guid.NewGuid();
                        noteAttach.Active = true;
                        noteAttach.CreatedById = parameter.UserId;
                        noteAttach.CreatedDate = DateTime.Now;
                        noteAttach.ObjectType = "WH";
                        noteAttach.NoteTitle = "đã thêm tài liệu";
                        noteAttach.Type = "ADD";

                        // add noteDocument
                        List<NoteDocument> docList = new List<NoteDocument>();
                        string newPath = Path.Combine(webRootPath, folderName);
                        foreach (var file in parameter.fileList)
                        {
                            NoteDocument noteDoc = new NoteDocument()
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = noteAttach.NoteId,
                                DocumentName = file.FileName,
                                DocumentSize = file.Length.ToString(),
                                DocumentUrl = Path.Combine(newPath, file.FileName),
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            docList.Add(noteDoc);
                        }

                        if (docList.Count > 0)
                        {
                            docList.ForEach(dl => { context.NoteDocument.Add(dl); });
                        }
                        context.Note.Add(noteAttach);
                    }

                    parameter.inventoryReceivingVoucherMapping.ForEach(item =>
                    {
                        if (item.ProductId != null)
                        {
                            InventoryReceivingVoucherMapping voucherMapping = new InventoryReceivingVoucherMapping();
                            voucherMapping.InventoryReceivingVoucherMappingId = Guid.NewGuid();
                            voucherMapping.InventoryReceivingVoucherId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId;
                            voucherMapping.ObjectId = item.VendorOrderDetailId;
                            voucherMapping.ProductId = (Guid)item.ProductId;
                            voucherMapping.QuantityRequest = (decimal)item.QuantityRequire;
                            voucherMapping.QuantityActual = (decimal)item.Quantity;
                            voucherMapping.PriceProduct = (decimal)item.Price;
                            voucherMapping.UnitId = item.UnitId;
                            voucherMapping.CurrencyUnit = item.CurrencyUnit;
                            voucherMapping.ExchangeRate = item.ExchangeRate;
                            voucherMapping.DiscountType = item.DiscountType;
                            voucherMapping.DiscountValue = item.DiscountValue;
                            voucherMapping.Vat = item.Vat;
                            voucherMapping.WarehouseId = item.WareHouseId;
                            voucherMapping.Description = item.Note;
                            voucherMapping.Active = true;
                            voucherMapping.CreatedDate = DateTime.Now;
                            voucherMapping.CreatedById = parameter.UserId;

                            if (item.ListSerial != null)
                            {
                                item.ListSerial.ForEach(itemSerial =>
                                {
                                    Serial serial = new Serial();
                                    serial.SerialId = Guid.NewGuid();
                                    serial.SerialCode = itemSerial.SerialCode;
                                    serial.WarehouseId = item.WareHouseId;
                                    serial.ProductId = (Guid)item.ProductId;
                                    serial.Active = true;
                                    serial.CreatedDate = DateTime.Now;
                                    serial.StatusId = categoryListSerial.FirstOrDefault(sr => sr.CategoryCode == "CXU").CategoryId;
                                    context.Serial.Add(serial);

                                    InventoryReceivingVoucherSerialMapping mapserial = new InventoryReceivingVoucherSerialMapping();
                                    mapserial.InventoryReceivingVoucherSerialMappingId = Guid.NewGuid();
                                    mapserial.InventoryReceivingVoucherMappingId = voucherMapping.InventoryReceivingVoucherMappingId;
                                    mapserial.SerialId = serial.SerialId;
                                    mapserial.Active = true;
                                    mapserial.CreatedDate = DateTime.Now;
                                    context.InventoryReceivingVoucherSerialMapping.Add(mapserial);
                                });

                            }

                            //var inventoryReportByProduct = inventoryReports.FirstOrDefault(wh => wh.ProductId == voucherMapping.ProductId && wh.WarehouseId == voucherMapping.WarehouseId);
                            //if (inventoryReportByProduct == null)
                            //{
                            //    InventoryReport inventoryReport = new InventoryReport();
                            //    inventoryReport.InventoryReportId = Guid.NewGuid();
                            //    inventoryReport.WarehouseId = item.WareHouseId;
                            //    inventoryReport.ProductId = voucherMapping.ProductId;
                            //    inventoryReport.Quantity = voucherMapping.QuantityActual;
                            //    inventoryReport.QuantityMinimum = 0;
                            //    inventoryReport.Active = true;
                            //    inventoryReport.CreatedDate = DateTime.Now;
                            //    context.InventoryReport.Add(inventoryReport);
                            //}
                            //else
                            //{
                            //    inventoryReportByProduct.Quantity += voucherMapping.QuantityActual;
                            //    context.InventoryReport.Update(inventoryReportByProduct);
                            //}

                            context.InventoryReceivingVoucherMapping.Add(voucherMapping);

                        }
                    });

                    context.SaveChanges();
                    return new CreateOrUpdateInventoryVoucherResult
                    {
                        InventoryReceivingVoucherId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId,
                        Message = "Tạo thành công",
                        Status = true
                    };
                }
                else
                {
                    var categoryTypeIdSerial = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TSE" && ct.Active == true).CategoryTypeId;
                    var categoryListSerial = context.Category.Where(ct => ct.CategoryTypeId == categoryTypeIdSerial && ct.Active == true).ToList();

                    //var inventoryReports = context.InventoryReport.Where(w => w.Active == true).ToList();
                    parameter.inventoryReceivingVoucher.ShiperName = parameter.inventoryReceivingVoucher.ShiperName.Trim();
                    parameter.inventoryReceivingVoucher.UpdatedDate = DateTime.Now;
                    parameter.inventoryReceivingVoucher.UpdatedById = parameter.UserId;
                    context.InventoryReceivingVoucher.Update(parameter.inventoryReceivingVoucher);

                    var note = new Note();
                    note.ObjectId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId;
                    note.NoteId = Guid.NewGuid();
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.ObjectType = "WH";
                    note.Description = string.IsNullOrEmpty(parameter.noteContent) ? string.Empty : parameter.noteContent;
                    note.NoteTitle = "Đã chỉnh sửa phiếu nhập kho này";
                    note.Type = "EDT";
                    context.Note.Add(note);
                    if (parameter.fileList != null)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = hostingEnvironment.WebRootPath;

                        //upload file to Server
                        if (parameter.fileList != null && parameter.fileList.Count > 0)
                        {

                            string checkexistPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(checkexistPath))
                            {
                                Directory.CreateDirectory(checkexistPath);
                            }
                            foreach (IFormFile item in parameter.fileList)
                            {
                                if (item.Length > 0)
                                {
                                    string fileName = item.FileName.Trim();
                                    string fullPath = Path.Combine(checkexistPath, fileName);
                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        item.CopyTo(stream);
                                    }
                                }
                            }
                        }
                        // Add note
                        var noteAttach = new Note();
                        noteAttach.ObjectId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId;
                        noteAttach.NoteId = Guid.NewGuid();
                        noteAttach.Active = true;
                        noteAttach.CreatedById = parameter.UserId;
                        noteAttach.CreatedDate = DateTime.Now;
                        noteAttach.ObjectType = "WH";
                        noteAttach.NoteTitle = "đã thêm tài liệu";
                        noteAttach.Type = "ADD";

                        // add noteDocument
                        List<NoteDocument> docList = new List<NoteDocument>();
                        string newPath = Path.Combine(webRootPath, folderName);
                        foreach (var file in parameter.fileList)
                        {
                            NoteDocument noteDoc = new NoteDocument()
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = noteAttach.NoteId,
                                DocumentName = file.FileName,
                                DocumentSize = file.Length.ToString(),
                                DocumentUrl = Path.Combine(newPath, file.FileName),
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            docList.Add(noteDoc);
                        }

                        if (docList.Count > 0)
                        {
                            docList.ForEach(dl => { context.NoteDocument.Add(dl); });
                        }
                        context.Note.Add(noteAttach);
                    }
                    context.SaveChanges();

                    //delete item relationship
                    var InventoryReceivingVoucherMappingObject = context.InventoryReceivingVoucherMapping.Where(wh => wh.InventoryReceivingVoucherId == parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId).ToList();
                    var lstInventoryReceivingVoucherMappingId = InventoryReceivingVoucherMappingObject.Select(s => s.InventoryReceivingVoucherMappingId).ToList().Distinct();
                    var InventoryReceivingVoucherSerialMappingObject = context.InventoryReceivingVoucherSerialMapping
                                                                    .Where(wh => lstInventoryReceivingVoucherMappingId
                                                                    .Contains(wh.InventoryReceivingVoucherSerialMappingId)).ToList();
                    //update lai Quantity trong inventoryReports
                    InventoryReceivingVoucherMappingObject.ForEach(item =>
                    {
                        var inventoryReportByProduct = context.InventoryReport.FirstOrDefault(wh => wh.ProductId == item.ProductId && wh.WarehouseId == item.WarehouseId);
                        if (inventoryReportByProduct != null)
                        {
                            inventoryReportByProduct.Quantity = inventoryReportByProduct.Quantity - item.QuantityActual;
                            context.InventoryReport.Update(inventoryReportByProduct);
                            context.SaveChanges();
                        }

                    });

                    context.InventoryReceivingVoucherSerialMapping.RemoveRange(InventoryReceivingVoucherSerialMappingObject);
                    context.InventoryReceivingVoucherMapping.RemoveRange(InventoryReceivingVoucherMappingObject);
                    context.SaveChanges();

                    //tao lai tu dau

                    parameter.inventoryReceivingVoucherMapping.ForEach(item =>
                    {
                        if (item.ProductId != null)
                        {
                            InventoryReceivingVoucherMapping voucherMapping = new InventoryReceivingVoucherMapping();
                            voucherMapping.InventoryReceivingVoucherMappingId = Guid.NewGuid();
                            voucherMapping.InventoryReceivingVoucherId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId;
                            voucherMapping.ObjectId = item.VendorOrderDetailId;
                            voucherMapping.ProductId = (Guid)item.ProductId;
                            voucherMapping.QuantityRequest = (decimal)item.QuantityRequire;
                            voucherMapping.QuantityActual = (decimal)item.Quantity;
                            voucherMapping.PriceProduct = (decimal)item.Price;
                            voucherMapping.UnitId = item.UnitId;
                            voucherMapping.CurrencyUnit = item.CurrencyUnit;
                            voucherMapping.ExchangeRate = item.ExchangeRate;
                            voucherMapping.DiscountType = item.DiscountType;
                            voucherMapping.DiscountValue = item.DiscountValue;
                            voucherMapping.Vat = item.Vat;
                            voucherMapping.WarehouseId = item.WareHouseId;
                            voucherMapping.Description = item.Note;
                            voucherMapping.Active = true;
                            voucherMapping.CreatedDate = DateTime.Now;
                            voucherMapping.CreatedById = parameter.UserId;

                            if (item.ListSerial != null)
                            {
                                item.ListSerial.ForEach(itemSerial =>
                                {
                                    Serial serial = new Serial();
                                    serial.SerialId = Guid.NewGuid();
                                    serial.SerialCode = itemSerial.SerialCode;
                                    serial.WarehouseId = item.WareHouseId;
                                    serial.ProductId = (Guid)item.ProductId;
                                    serial.Active = true;
                                    serial.CreatedById = parameter.UserId;
                                    serial.CreatedDate = DateTime.Now;
                                    serial.StatusId = categoryListSerial.FirstOrDefault(sr => sr.CategoryCode == "CXU").CategoryId;
                                    context.Serial.Add(serial);

                                    InventoryReceivingVoucherSerialMapping mapserial = new InventoryReceivingVoucherSerialMapping();
                                    mapserial.InventoryReceivingVoucherSerialMappingId = Guid.NewGuid();
                                    mapserial.InventoryReceivingVoucherMappingId = voucherMapping.InventoryReceivingVoucherMappingId;
                                    mapserial.SerialId = serial.SerialId;
                                    mapserial.Active = true;
                                    mapserial.CreatedDate = DateTime.Now;
                                    context.InventoryReceivingVoucherSerialMapping.Add(mapserial);
                                });

                                //var inventoryReportByProduct = inventoryReports.FirstOrDefault(wh => wh.ProductId == voucherMapping.ProductId && wh.WarehouseId == voucherMapping.WarehouseId);
                                //if (inventoryReportByProduct == null)
                                //{
                                //    InventoryReport inventoryReport = new InventoryReport();
                                //    inventoryReport.InventoryReportId = Guid.NewGuid();
                                //    inventoryReport.WarehouseId = item.WareHouseId;
                                //    inventoryReport.ProductId = voucherMapping.ProductId;
                                //    inventoryReport.Quantity = voucherMapping.QuantityActual;
                                //    inventoryReport.QuantityMinimum = 0;
                                //    inventoryReport.Active = true;
                                //    inventoryReport.CreatedDate = DateTime.Now;
                                //    context.InventoryReport.Add(inventoryReport);
                                //}
                                //else
                                //{
                                //    inventoryReportByProduct.Quantity += voucherMapping.QuantityActual;
                                //    context.InventoryReport.Update(inventoryReportByProduct);
                                //}

                            }
                            context.InventoryReceivingVoucherMapping.Add(voucherMapping);

                        }
                    });

                    context.SaveChanges();
                    return new CreateOrUpdateInventoryVoucherResult
                    {
                        Message = "Cập nhập thành công",
                        Status = true,
                        InventoryReceivingVoucherId = parameter.inventoryReceivingVoucher.InventoryReceivingVoucherId
                    };
                }
            }
            catch (Exception e)
            {
                this.logger.LogError("CreateOrUpdateInventoryVoucherDAO:" + e.ToString());

                return new CreateOrUpdateInventoryVoucherResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình cập nhập",
                    Status = false
                };
            }
        }

        public RemoveWareHouseResult RemoveWareHouse(RemoveWareHouseParameter parameter)
        {
            try
            {
                var RemoveWareHouse = context.Warehouse.Where(wh => wh.WarehouseId == parameter.WareHouseId).FirstOrDefault();
                if (RemoveWareHouse != null)
                {
                    context.Warehouse.Remove(RemoveWareHouse);
                    context.SaveChanges();
                }

                return new RemoveWareHouseResult()
                {
                    Status = true,
                    Message = "Tạo mới/Cập nhật thành công",
                    WareHouseId = RemoveWareHouse.WarehouseId
                };
            }
            catch (Exception e)
            {
                return new RemoveWareHouseResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetInventoryReceivingVoucherByIdResult GetInventoryReceivingVoucherById(GetInventoryReceivingVoucherByIdParameter parameter)
        {
            try
            {
                var InventoryReceivingVoucherEntity = context.InventoryReceivingVoucher.FirstOrDefault(f => f.InventoryReceivingVoucherId == parameter.Id);
                Employee defaultEmpty = new Employee();
                InventoryReceivingVoucherModel itemModel = new InventoryReceivingVoucherModel();
                itemModel.InventoryReceivingVoucherId = InventoryReceivingVoucherEntity.InventoryReceivingVoucherId;
                itemModel.InventoryReceivingVoucherCode = InventoryReceivingVoucherEntity.InventoryReceivingVoucherCode;
                itemModel.StatusId = InventoryReceivingVoucherEntity.StatusId;
                itemModel.InventoryReceivingVoucherType = InventoryReceivingVoucherEntity.InventoryReceivingVoucherType;
                itemModel.WarehouseId = InventoryReceivingVoucherEntity.WarehouseId;
                itemModel.ShiperName = InventoryReceivingVoucherEntity.ShiperName;
                itemModel.Storekeeper = InventoryReceivingVoucherEntity.Storekeeper;
                itemModel.InventoryReceivingVoucherDate = InventoryReceivingVoucherEntity.InventoryReceivingVoucherDate;
                itemModel.InventoryReceivingVoucherTime = InventoryReceivingVoucherEntity.InventoryReceivingVoucherTime;
                itemModel.LicenseNumber = InventoryReceivingVoucherEntity.LicenseNumber;
                itemModel.NameStorekeeper = InventoryReceivingVoucherEntity.Storekeeper != null ? context.Employee.FirstOrDefault(f => f.EmployeeId == InventoryReceivingVoucherEntity.Storekeeper.Value).EmployeeName : "";
                var userCreate = context.User.FirstOrDefault(f => f.UserId == InventoryReceivingVoucherEntity.CreatedById);
                itemModel.NameCreate = (userCreate != null) ? context.Employee.Where(f => f.EmployeeId == userCreate.EmployeeId).DefaultIfEmpty(defaultEmpty).FirstOrDefault().EmployeeName : "";
                itemModel.NameStatus = context.Category.FirstOrDefault(f => f.CategoryId == InventoryReceivingVoucherEntity.StatusId).CategoryName;
                itemModel.Active = InventoryReceivingVoucherEntity.Active;
                itemModel.CreatedDate = InventoryReceivingVoucherEntity.CreatedDate;
                itemModel.CreatedById = InventoryReceivingVoucherEntity.CreatedById;

                var inventoryReceivingVoucherMappingEntity = context.InventoryReceivingVoucherMapping.Where(wh => wh.InventoryReceivingVoucherId == parameter.Id).ToList();
                var lstnventoryReceivingVoucherMappingID = inventoryReceivingVoucherMappingEntity.Select(s => s.InventoryReceivingVoucherMappingId).ToList();
                var InventoryReceivingVoucherSerialMapping = context.InventoryReceivingVoucherSerialMapping
                                                             .Where(wh => lstnventoryReceivingVoucherMappingID
                                                             .Contains(wh.InventoryReceivingVoucherMappingId)).ToList();

                var lstSerialId = InventoryReceivingVoucherSerialMapping.Select(s => s.SerialId).ToList();
                var Serial = context.Serial.Where(wh => lstSerialId.Contains(wh.SerialId));
                var vendorOrderDetails = new List<VendorOrderDetail>();
                var vendorOrder = new List<VendorOrder>();
                var customerOrderDetails = new List<CustomerOrderDetail>();
                var customerOrder = new List<CustomerOrder>();

                if (InventoryReceivingVoucherEntity.InventoryReceivingVoucherType == 1)
                {
                    var lstinventoryReceivingVoucherMappingEntityId = inventoryReceivingVoucherMappingEntity.Select(s => s.ObjectId).ToList();
                    vendorOrderDetails = context.VendorOrderDetail.Where(vo => vo.Active == true && lstinventoryReceivingVoucherMappingEntityId.Contains(vo.VendorOrderDetailId)).ToList();

                    var lstVendorOderId = vendorOrderDetails.Select(s => s.VendorOrderId).ToList();
                    vendorOrder = context.VendorOrder.Where(wh => lstVendorOderId.Contains(wh.VendorOrderId)).ToList();
                }
                else if (InventoryReceivingVoucherEntity.InventoryReceivingVoucherType == 2)
                {
                    var lstinventoryReceivingVoucherMappingEntityId = inventoryReceivingVoucherMappingEntity.Select(s => s.ObjectId).ToList();
                    customerOrderDetails = context.CustomerOrderDetail.Where(vo => vo.Active == true && lstinventoryReceivingVoucherMappingEntityId.Contains(vo.OrderDetailId)).ToList();
                    var lstCustomerOderId = customerOrderDetails.Select(s => s.OrderId).ToList();
                    customerOrder = context.CustomerOrder.Where(wh => lstCustomerOderId.Contains(wh.OrderId)).ToList();
                }
                //var categoryTypeIdUnit = context.CategoryType.FirstOrDefault(cty => cty.Active == true && cty.CategoryTypeCode == "DNH").CategoryTypeId;
                var categories = context.Category.Where(ct => ct.Active == true).ToList();
                var lstinventoryReceivingVoucherMappingEntityProductId = inventoryReceivingVoucherMappingEntity.Select(s => s.ProductId).ToList();
                var product = context.Product.Where(p => p.Active == true && lstinventoryReceivingVoucherMappingEntityProductId.Contains(p.ProductId)).ToList();

                List<GetVendorOrderDetailByVenderOrderIdEntityModel> lstinventoryReceivingVoucherMappingModel = new List<GetVendorOrderDetailByVenderOrderIdEntityModel>();

                inventoryReceivingVoucherMappingEntity.ForEach(detail =>
                {
                    GetVendorOrderDetailByVenderOrderIdEntityModel obj = new GetVendorOrderDetailByVenderOrderIdEntityModel();

                    if (InventoryReceivingVoucherEntity.InventoryReceivingVoucherType == 1)
                    {
                        var vendorOrderDetailsQuery = vendorOrderDetails.FirstOrDefault(wh => wh.VendorOrderDetailId == detail.ObjectId);
                        var vendororderQuery = vendorOrder.FirstOrDefault(wh => wh.VendorOrderId == vendorOrderDetailsQuery.VendorOrderId);
                        obj.VendorOrderId = vendororderQuery.VendorOrderId;
                        obj.VendorOrderCode = vendororderQuery.VendorOrderCode;

                    }
                    else if (InventoryReceivingVoucherEntity.InventoryReceivingVoucherType == 2)
                    {
                        var customerOrderDetailsQuery = customerOrderDetails.FirstOrDefault(wh => wh.OrderDetailId == detail.ObjectId);
                        var cusotmerOrderQuery = customerOrder.FirstOrDefault(wh => wh.OrderId == customerOrderDetailsQuery.OrderId);
                        obj.VendorOrderId = cusotmerOrderQuery.OrderId;
                        obj.VendorOrderCode = cusotmerOrderQuery.OrderCode;

                    }
                    obj.InventoryReceivingVoucherMappingId = detail.InventoryReceivingVoucherMappingId;
                    obj.VendorOrderDetailId = detail.ObjectId.Value;
                    obj.ProductId = detail.ProductId;
                    obj.ProductName = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductName;
                    obj.ProductCode = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductCode;
                    obj.UnitId = detail.UnitId;
                    obj.UnitName = detail.UnitId == null ? "" : categories.FirstOrDefault(c => c.CategoryId == detail.UnitId).CategoryName;
                    obj.CurrencyUnit = detail.CurrencyUnit;
                    obj.ExchangeRate = detail.ExchangeRate;
                    obj.Vat = detail.Vat;
                    obj.DiscountType = detail.DiscountType;
                    obj.DiscountValue = detail.DiscountValue;
                    obj.SumAmount = SumAmount(detail.QuantityActual, detail.PriceProduct, detail.ExchangeRate, detail.Vat, detail.DiscountValue, detail.DiscountType);
                    obj.NameMoneyUnit = detail.CurrencyUnit != null ? categories.FirstOrDefault(c => c.CategoryId == detail.CurrencyUnit).CategoryName : "";
                    obj.QuantityRequire = detail.QuantityRequest;
                    obj.Quantity = detail.QuantityActual;
                    obj.Note = detail.Description;
                    obj.TotalSerial = 0;
                    obj.Price = detail.PriceProduct;
                    obj.ListSerial = new List<Serial>();
                    obj.WareHouseId = detail.WarehouseId;
                    obj.WareHouseName = (detail.WarehouseId != Guid.Empty) ? context.Warehouse.FirstOrDefault(f => f.WarehouseId == detail.WarehouseId).WarehouseName : "";
                    lstinventoryReceivingVoucherMappingModel.Add(obj);
                });
                //Get list Serial
                lstinventoryReceivingVoucherMappingModel.ForEach(item =>
                {
                    var InventoryReceivingVoucherSerialMappingQuery = InventoryReceivingVoucherSerialMapping
                                                                     .Where(wh => wh.InventoryReceivingVoucherMappingId == item.InventoryReceivingVoucherMappingId).ToList();
                    var lstSerial = InventoryReceivingVoucherSerialMappingQuery.Select(s => s.SerialId).ToList();
                    var SerialQuery = Serial.Where(wh => lstSerial.Contains(wh.SerialId)).ToList();
                    item.ListSerial.AddRange(SerialQuery);
                    item.TotalSerial = SerialQuery.Count();

                });
                var selectVendor = new VendorEntityModel();
                if (vendorOrder.Count > 0)
                {
                    var vendororderQueryX = vendorOrder.First();
                    selectVendor = context.Vendor.Where(wh => wh.VendorId == vendororderQueryX.VendorId)
                       .Select(v => new VendorEntityModel
                       {
                           VendorId = v.VendorId,
                           VendorName = v.VendorName,
                           VendorGroupId = v.VendorGroupId,
                           VendorCode = v.VendorCode,
                           TotalPurchaseValue = v.TotalPurchaseValue,
                           TotalPayableValue = v.TotalPayableValue,
                           NearestDateTransaction = v.NearestDateTransaction,
                           PaymentId = v.PaymentId,
                           CreatedById = v.CreatedById,
                           CreatedDate = v.CreatedDate,
                           UpdatedById = v.UpdatedById,
                           UpdatedDate = v.UpdatedDate,
                           Active = v.Active,
                       }).First();
                }
                var selectCustomer = new CustomerEntityModel();
                if (customerOrder.Count > 0)
                {
                    var customerOrderQueryX = customerOrder.First();
                    selectCustomer = context.Customer.Where(wh => wh.CustomerId == customerOrderQueryX.CustomerId)
                       .Select(v => new CustomerEntityModel
                       {
                           CustomerId = v.CustomerId,
                           CustomerName = v.CustomerName,
                           CustomerGroupId = v.CustomerGroupId,
                           CustomerCode = v.CustomerCode,
                           TotalSaleValue = v.TotalSaleValue,
                           TotalReceivable = v.TotalReceivable,
                           NearestDateTransaction = v.NearestDateTransaction,
                           PaymentId = v.PaymentId,
                           CreatedById = v.CreatedById,
                           CreatedDate = v.CreatedDate,
                           UpdatedById = v.UpdatedById,
                           UpdatedDate = v.UpdatedDate,
                           Active = v.Active,
                       }).First();
                }


                return new GetInventoryReceivingVoucherByIdResult
                {
                    inventoryReceivingVoucher = itemModel,
                    inventoryReceivingVoucherMapping = lstinventoryReceivingVoucherMappingModel,
                    SelectVendor = selectVendor,
                    SelectCustomer = selectCustomer,
                    listVendorOrder = vendorOrder,
                    listCustomerOrder = customerOrder,
                    Status = true
                };

            }
            catch (Exception e)
            {
                return new GetInventoryReceivingVoucherByIdResult
                {
                    Message = "Có lỗi xảy ra",
                    Status = false
                };
            }

        }

        public GetListInventoryReceivingVoucherResult GetListInventoryReceivingVoucher(GetListInventoryReceivingVoucherParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllContact = context.Contact.ToList();

                //set default cac Object
                Serial emptyX = new Serial();
                InventoryReceivingVoucherMapping emptyIRVM = new InventoryReceivingVoucherMapping();
                InventoryReceivingVoucherSerialMapping emptyIRSM = new InventoryReceivingVoucherSerialMapping();
                VendorOrderDetail emptyVOD = new VendorOrderDetail();
                CustomerOrderDetail emptyCOD = new CustomerOrderDetail();
                Employee emptyEMP = new Employee();
                User emptyUser = new User();

                if (parameter.listCreateDate != null)
                {
                    if (parameter.listCreateDate.Count == 1)
                    {
                        parameter.listCreateDate.Add(parameter.listCreateDate[0]);
                    }
                }
                if (parameter.listInventoryReceivingDate != null)
                {
                    if (parameter.listInventoryReceivingDate.Count == 1)
                    {
                        parameter.listInventoryReceivingDate.Add(parameter.listInventoryReceivingDate[0]);
                    }
                }
                //check isManager
                var userCurrent = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (userCurrent == null)
                {
                    return new GetListInventoryReceivingVoucherResult
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (userCurrent.EmployeeId == null || userCurrent.EmployeeId == Guid.Empty)
                {
                    return new GetListInventoryReceivingVoucherResult
                    {
                        Status = false,
                        Message = "Lỗi dữ liệu"
                    };
                }
                var employeeId = userCurrent.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;
                //////////////////////////////////////
                var lstInventoryReceivingVoucherEntity = context.InventoryReceivingVoucher.Where(inventoryrv =>
                (parameter.VoucherCode == null || parameter.VoucherCode == "" || inventoryrv.InventoryReceivingVoucherCode.ToLower().Contains(parameter.VoucherCode.Trim().ToLower()))
                && (parameter.listCreateDate == null || parameter.listCreateDate.Count == 0 || (parameter.listCreateDate[0].Date <= inventoryrv.CreatedDate.Date && parameter.listCreateDate[1].Date >= inventoryrv.CreatedDate.Date))
                && (parameter.listInventoryReceivingDate == null || parameter.listInventoryReceivingDate.Count == 0 ||
                (parameter.listInventoryReceivingDate[0].Date <= inventoryrv.InventoryReceivingVoucherDate.Date && parameter.listInventoryReceivingDate[1].Date >= inventoryrv.InventoryReceivingVoucherDate.Date))
                && (parameter.listStatusSelectedId == null || parameter.listStatusSelectedId.Count == 0 || parameter.listStatusSelectedId.Contains(inventoryrv.StatusId))
                ).ToList();

                var lstInventoryReceivingVoucherMappingEntity = context.InventoryReceivingVoucherMapping.Where(inventoryrvm =>
                 (parameter.listWarehouseSelectedId == null || parameter.listWarehouseSelectedId.Count == 0 || parameter.listWarehouseSelectedId.Contains(inventoryrvm.WarehouseId))
                 && (parameter.listProductSelectedId == null || parameter.listProductSelectedId.Count == 0 || parameter.listProductSelectedId.Contains(inventoryrvm.ProductId))
                ).ToList();

                var lstMappingID = lstInventoryReceivingVoucherMappingEntity.Select(s => s.InventoryReceivingVoucherMappingId).ToList();

                var lstInventoryReceivingVoucherSerialMapping = context.InventoryReceivingVoucherSerialMapping.Where(wh => lstMappingID.Contains(wh.InventoryReceivingVoucherMappingId)).ToList();

                var lstSerialEntity = context.Serial.Where(serial => (parameter.serial == null || parameter.serial == "" || serial.SerialCode.Contains(parameter.serial.Trim()))).ToList();

                var lstvendorOrderDetails = context.VendorOrderDetail.Where(vendorOrderDetail =>
                   (parameter.listVendorSelectedId == null || parameter.listVendorSelectedId.Count == 0 || parameter.listVendorSelectedId.Contains(vendorOrderDetail.VendorId))
                ).ToList();

                var listStorekeeper = context.Employee.Where(emp =>
                           (parameter.listStorekeeperSelectedId == null || parameter.listStorekeeperSelectedId.Count == 0 || parameter.listStorekeeperSelectedId.Contains(emp.EmployeeId))
                    ).ToList();

                var listCreateVoucher = context.Employee.Where(emp2 =>
                            (parameter.listCreateVoucherSelectedId == null || parameter.listCreateVoucherSelectedId.Count == 0 || parameter.listCreateVoucherSelectedId.Contains(emp2.EmployeeId))
                    ).ToList();
                ///////////////////////////////////////
                var lstInventoryReceivingVoucher = new List<InventoryReceivingVoucherModel>();
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                    List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                    List<Guid> listUserByManagerId = new List<Guid>();

                    listEmployeeInChargeByManager.ForEach(item =>
                    {
                        if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                            listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                    });

                    //Lấy danh sách nhân viên UserId mà user phụ trách
                    listEmployeeInChargeByManagerId.ForEach(item =>
                    {
                        var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                        if (user_employee != null)
                            listUserByManagerId.Add(user_employee.UserId);
                    });
                    var lstInventoryReceivingVoucherX = new List<Guid>();
                    if (parameter.serial == null || parameter.serial == "")
                    {
                        lstInventoryReceivingVoucherX = (from inventoryrv in lstInventoryReceivingVoucherEntity
                                                         join inventoryrvm in lstInventoryReceivingVoucherMappingEntity on inventoryrv.InventoryReceivingVoucherId equals inventoryrvm.InventoryReceivingVoucherId into inventoryrvmLeft
                                                         from inventoryrvmX in inventoryrvmLeft.DefaultIfEmpty(emptyIRVM)
                                                         join inventoryrvsm in lstInventoryReceivingVoucherSerialMapping on inventoryrvmX.InventoryReceivingVoucherMappingId equals inventoryrvsm.InventoryReceivingVoucherMappingId into inventoryrvsmLeft
                                                         from inventoryrvsmX in inventoryrvsmLeft.DefaultIfEmpty(emptyIRSM)
                                                         join serial in lstSerialEntity on inventoryrvsmX.SerialId equals serial.SerialId into serialLeft
                                                         from serialX in serialLeft.DefaultIfEmpty(emptyX)
                                                         join vendorOrderDetail in lstvendorOrderDetails on inventoryrvmX.ObjectId equals vendorOrderDetail.VendorOrderDetailId into vendorOrderDetailLeft
                                                         //from vendorOrderDetail in vendorOrderDetailLeft.DefaultIfEmpty(emptyVOD)
                                                         join emp in listStorekeeper on inventoryrv.Storekeeper equals emp.EmployeeId into empLeft
                                                         from emp in empLeft.DefaultIfEmpty(emptyEMP)
                                                         join user in context.User on inventoryrv.CreatedById equals user.UserId into userLeft
                                                         from user in userLeft.DefaultIfEmpty(emptyUser)
                                                         join emp2 in listCreateVoucher on user.EmployeeId equals emp2.EmployeeId into emp2Left
                                                         from emp2 in emp2Left.DefaultIfEmpty(emptyEMP)
                                                         where listUserByManagerId.Contains(inventoryrv.CreatedById)
                                                         orderby inventoryrv.CreatedDate descending
                                                         select inventoryrv.InventoryReceivingVoucherId
                                                        ).Distinct().ToList();
                    }
                    else
                    {
                        lstInventoryReceivingVoucherX = (from inventoryrv in lstInventoryReceivingVoucherEntity
                                                         join inventoryrvm in lstInventoryReceivingVoucherMappingEntity on inventoryrv.InventoryReceivingVoucherId equals inventoryrvm.InventoryReceivingVoucherId into inventoryrvmLeft
                                                         from inventoryrvmX in inventoryrvmLeft.DefaultIfEmpty(emptyIRVM)
                                                         join inventoryrvsm in lstInventoryReceivingVoucherSerialMapping on inventoryrvmX.InventoryReceivingVoucherMappingId equals inventoryrvsm.InventoryReceivingVoucherMappingId into inventoryrvsmLeft
                                                         from inventoryrvsmX in inventoryrvsmLeft.DefaultIfEmpty(emptyIRSM)
                                                         join serial in lstSerialEntity on inventoryrvsmX.SerialId equals serial.SerialId
                                                         //from serialX in serialLeft.DefaultIfEmpty(emptyX)
                                                         join vendorOrderDetail in lstvendorOrderDetails on inventoryrvmX.ObjectId equals vendorOrderDetail.VendorOrderDetailId into vendorOrderDetailLeft
                                                         //from vendorOrderDetail in vendorOrderDetailLeft.DefaultIfEmpty(emptyVOD)
                                                         join emp in listStorekeeper on inventoryrv.Storekeeper equals emp.EmployeeId into empLeft
                                                         from emp in empLeft.DefaultIfEmpty(emptyEMP)
                                                         join user in context.User on inventoryrv.CreatedById equals user.UserId into userLeft
                                                         from user in userLeft.DefaultIfEmpty(emptyUser)
                                                         join emp2 in listCreateVoucher on user.EmployeeId equals emp2.EmployeeId into emp2Left
                                                         from emp2 in emp2Left.DefaultIfEmpty(emptyEMP)
                                                         where listUserByManagerId.Contains(inventoryrv.CreatedById)
                                                         orderby inventoryrv.CreatedDate descending
                                                         select inventoryrv.InventoryReceivingVoucherId
                                ).Distinct().ToList();

                    }

                    lstInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(wh => lstInventoryReceivingVoucherX.Contains(wh.InventoryReceivingVoucherId))
                                                   .Select(inventoryrv => new InventoryReceivingVoucherModel
                                                   {
                                                       InventoryReceivingVoucherId = inventoryrv.InventoryReceivingVoucherId,
                                                       InventoryReceivingVoucherCode = inventoryrv.InventoryReceivingVoucherCode,
                                                       StatusId = inventoryrv.StatusId,
                                                       InventoryReceivingVoucherType = inventoryrv.InventoryReceivingVoucherType,
                                                       WarehouseId = inventoryrv.WarehouseId,
                                                       ShiperName = inventoryrv.ShiperName,
                                                       Storekeeper = inventoryrv.Storekeeper,
                                                       InventoryReceivingVoucherDate = inventoryrv.InventoryReceivingVoucherDate,
                                                       InventoryReceivingVoucherTime = inventoryrv.InventoryReceivingVoucherTime,
                                                       LicenseNumber = inventoryrv.LicenseNumber,
                                                       Active = inventoryrv.Active,
                                                       CreatedDate = inventoryrv.CreatedDate,
                                                       CreatedById = inventoryrv.CreatedById,
                                                   }).OrderByDescending(or => or.CreatedDate).ToList();

                }
                else
                {

                    var lstInventoryReceivingVoucherX = new List<Guid>();
                    if (parameter.serial == null || parameter.serial == "")
                    {

                        lstInventoryReceivingVoucherX = (from inventoryrv in lstInventoryReceivingVoucherEntity
                                                         join inventoryrvm in lstInventoryReceivingVoucherMappingEntity on inventoryrv.InventoryReceivingVoucherId equals inventoryrvm.InventoryReceivingVoucherId into inventoryrvmLeft
                                                         from inventoryrvmX in inventoryrvmLeft.DefaultIfEmpty(emptyIRVM)
                                                         join inventoryrvsm in lstInventoryReceivingVoucherSerialMapping on inventoryrvmX.InventoryReceivingVoucherMappingId equals inventoryrvsm.InventoryReceivingVoucherMappingId into inventoryrvsmLeft
                                                         from inventoryrvsmX in inventoryrvsmLeft.DefaultIfEmpty(emptyIRSM)
                                                         join serial in lstSerialEntity on inventoryrvsmX.SerialId equals serial.SerialId into serialLeft
                                                         from serialX in serialLeft.DefaultIfEmpty(emptyX)
                                                         join vendorOrderDetail in lstvendorOrderDetails on inventoryrvmX.ObjectId equals vendorOrderDetail.VendorOrderDetailId into vendorOrderDetailLeft
                                                         from vendorOrderDetail in vendorOrderDetailLeft.DefaultIfEmpty(emptyVOD)
                                                         join emp in listStorekeeper on inventoryrv.Storekeeper equals emp.EmployeeId into empLeft
                                                         from emp in empLeft.DefaultIfEmpty(emptyEMP)
                                                         join user in context.User on inventoryrv.CreatedById equals user.UserId into userLeft
                                                         from user in userLeft.DefaultIfEmpty(emptyUser)
                                                         join emp2 in listCreateVoucher on user.EmployeeId equals emp2.EmployeeId into emp2Left
                                                         from emp2 in emp2Left.DefaultIfEmpty(emptyEMP)
                                                         where inventoryrv.CreatedById == parameter.UserId
                                                         orderby inventoryrv.CreatedDate descending
                                                         select inventoryrv.InventoryReceivingVoucherId
                                                        ).Distinct().ToList();
                    }
                    else
                    {
                        lstInventoryReceivingVoucherX = (from inventoryrv in lstInventoryReceivingVoucherEntity
                                                         join inventoryrvm in lstInventoryReceivingVoucherMappingEntity on inventoryrv.InventoryReceivingVoucherId equals inventoryrvm.InventoryReceivingVoucherId into inventoryrvmLeft
                                                         from inventoryrvmX in inventoryrvmLeft.DefaultIfEmpty(emptyIRVM)
                                                         join inventoryrvsm in lstInventoryReceivingVoucherSerialMapping on inventoryrvmX.InventoryReceivingVoucherMappingId equals inventoryrvsm.InventoryReceivingVoucherMappingId into inventoryrvsmLeft
                                                         from inventoryrvsmX in inventoryrvsmLeft.DefaultIfEmpty(emptyIRSM)
                                                         join serial in lstSerialEntity on inventoryrvsmX.SerialId equals serial.SerialId
                                                         join vendorOrderDetail in lstvendorOrderDetails on inventoryrvmX.ObjectId equals vendorOrderDetail.VendorOrderDetailId into vendorOrderDetailLeft
                                                         from vendorOrderDetail in vendorOrderDetailLeft.DefaultIfEmpty(emptyVOD)
                                                         join emp in listStorekeeper on inventoryrv.Storekeeper equals emp.EmployeeId into empLeft
                                                         from emp in empLeft.DefaultIfEmpty(emptyEMP)
                                                         join user in context.User on inventoryrv.CreatedById equals user.UserId into userLeft
                                                         from user in userLeft.DefaultIfEmpty(emptyUser)
                                                         join emp2 in listCreateVoucher on user.EmployeeId equals emp2.EmployeeId into emp2Left
                                                         from emp2 in emp2Left.DefaultIfEmpty(emptyEMP)
                                                         where inventoryrv.CreatedById == parameter.UserId
                                                         orderby inventoryrv.CreatedDate descending
                                                         select inventoryrv.InventoryReceivingVoucherId
                                ).Distinct().ToList();

                    }
                    lstInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(wh => lstInventoryReceivingVoucherX.Contains(wh.InventoryReceivingVoucherId))
                                                 .Select(inventoryrv => new InventoryReceivingVoucherModel
                                                 {
                                                     InventoryReceivingVoucherId = inventoryrv.InventoryReceivingVoucherId,
                                                     InventoryReceivingVoucherCode = inventoryrv.InventoryReceivingVoucherCode,
                                                     StatusId = inventoryrv.StatusId,
                                                     InventoryReceivingVoucherType = inventoryrv.InventoryReceivingVoucherType,
                                                     WarehouseId = inventoryrv.WarehouseId,
                                                     ShiperName = inventoryrv.ShiperName,
                                                     Storekeeper = inventoryrv.Storekeeper,
                                                     InventoryReceivingVoucherDate = inventoryrv.InventoryReceivingVoucherDate,
                                                     InventoryReceivingVoucherTime = inventoryrv.InventoryReceivingVoucherTime,
                                                     LicenseNumber = inventoryrv.LicenseNumber,
                                                     Active = inventoryrv.Active,
                                                     CreatedDate = inventoryrv.CreatedDate,
                                                     CreatedById = inventoryrv.CreatedById,
                                                 }).OrderByDescending(or => or.CreatedDate).ToList();

                }


                List<InventoryReceivingVoucherModel> lstReuslt = new List<InventoryReceivingVoucherModel>();
                if (lstInventoryReceivingVoucher.Count > 0)
                {
                    lstInventoryReceivingVoucher.ForEach(item =>
                    {
                        var InventoryReceivingVoucherMappingQuery = lstInventoryReceivingVoucherMappingEntity.Where(wh => wh.InventoryReceivingVoucherId == item.InventoryReceivingVoucherId).ToList();

                        Employee defaultEmpty = new Employee();

                        var lstVendorOrderDetailsId = InventoryReceivingVoucherMappingQuery.Select(s => s.ObjectId).ToList();
                        var lstVendorOrderDetails = context.VendorOrderDetail.Where(wh => lstVendorOrderDetailsId.Contains(wh.VendorOrderDetailId)).ToList();
                        var lstVendorOrderId = lstVendorOrderDetails.Select(s => s.VendorOrderId).ToList().Distinct().ToList();

                        var lstCustomerOrderDetails = context.CustomerOrderDetail.Where(wh => lstVendorOrderDetailsId.Contains(wh.OrderDetailId)).ToList();
                        var lstCustomerOrderId = lstCustomerOrderDetails.Select(s => s.OrderId).ToList().Distinct().ToList();


                        //InventoryReceivingVoucherModel itemModel = new InventoryReceivingVoucherModel();
                        if (item.InventoryReceivingVoucherType == 1)
                        {
                            var lstVendorOrder = context.VendorOrder.Where(wh => lstVendorOrderId.Contains(wh.VendorOrderId)).ToList();
                            if (lstVendorOrder.Count == 0) return;
                            var VendorId = lstVendorOrder.FirstOrDefault().VendorId;
                            item.ListVendorOrder = new List<VendorOrder>();
                            item.ListVendorOrder.AddRange(lstVendorOrder);
                            item.VendorId = VendorId;
                            item.NameVendor = lstVendorOrder.Count > 0 ? context.Vendor.FirstOrDefault(f => f.VendorId == VendorId).VendorName : string.Empty;
                        }
                        else if (item.InventoryReceivingVoucherType == 2)
                        {
                            var lstCustomerOrder = context.CustomerOrder.Where(wh => lstCustomerOrderId.Contains(wh.OrderId)).ToList();
                            if (lstCustomerOrder.Count == 0) return;
                            var CustomerId = lstCustomerOrder.FirstOrDefault().CustomerId;
                            item.ListCustomerOrder = new List<CustomerOrder>();
                            item.ListCustomerOrder.AddRange(lstCustomerOrder);
                            item.CustomerId = CustomerId;
                            item.CustomerName = lstCustomerOrder.Count > 0 ? context.Customer.FirstOrDefault(f => f.CustomerId == CustomerId).CustomerName : string.Empty;

                        }

                        //itemModel.InventoryReceivingVoucherId = item.InventoryReceivingVoucherId;
                        //itemModel.InventoryReceivingVoucherCode = item.InventoryReceivingVoucherCode;
                        //itemModel.StatusId = item.StatusId;
                        //itemModel.InventoryReceivingVoucherType = item.InventoryReceivingVoucherType;
                        //itemModel.WarehouseId = item.WarehouseId;
                        //itemModel.ShiperName = item.ShiperName;
                        //itemModel.Storekeeper = item.Storekeeper;
                        //itemModel.InventoryReceivingVoucherDate = item.InventoryReceivingVoucherDate;
                        //itemModel.InventoryReceivingVoucherTime = item.InventoryReceivingVoucherTime;
                        //itemModel.LicenseNumber = item.LicenseNumber;
                        item.NameStorekeeper = item.Storekeeper != null ? context.Employee.FirstOrDefault(f => f.EmployeeId == item.Storekeeper.Value).EmployeeName : "";
                        var userCreate = context.User.FirstOrDefault(f => f.UserId == item.CreatedById);
                        item.NameCreate = (userCreate != null) ? context.Employee.Where(f => f.EmployeeId == userCreate.EmployeeId).DefaultIfEmpty(defaultEmpty).FirstOrDefault().EmployeeName : "";
                        item.NameStatus = context.Category.FirstOrDefault(f => f.CategoryId == item.StatusId).CategoryName;
                        //itemModel.Active = item.Active;
                        //itemModel.CreatedDate = item.CreatedDate;
                        //itemModel.CreatedById = item.CreatedById;
                        lstReuslt.Add(item);
                    });
                }
                return new GetListInventoryReceivingVoucherResult
                {
                    lstResult = lstReuslt,
                    Message = "Đã lọc xong danh sách phiếu nhập",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new GetListInventoryReceivingVoucherResult
                {
                    Message = "Có lỗi xảy ra trong quá trình lọc",
                    Status = false
                };
            }

        }

        public GetListCustomerOrderByIdCustomerIdResult GetListCustomerOrderByIdCustomerId(GetListCustomerOrderByIdCustomerIdParameter parameter)
        {
            try
            {
                //Lấy Id các trạng thái đơn hàng: Đang xử lý, Đã thanh toán, Đã giao hàng và đã đóng
                var listStatusCode = new List<string>() { "RTN" };
                var listStatusId = context.OrderStatus.Where(ct => listStatusCode.Contains(ct.OrderStatusCode) && ct.Active == true).Select(ct => ct.OrderStatusId).ToList();
                //Lọc những đơn hàng đã nhập kho hết
                var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;
                var listInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(wh => wh.StatusId == categoryIdNHK).Select(s => s.InventoryReceivingVoucherId).ToList();
                var inventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.Where(wh => listInventoryReceivingVoucher.Contains(wh.InventoryReceivingVoucherId)).ToList();
                //var inventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.ToList();
                var customerOrderDetail = context.CustomerOrderDetail.ToList();

                var result = new List<CustomerOrder>();


                //var result = new List<CustomerOrder>();
                //Lấy các đơn hàng có các trạng thái trên
                //Thiếu điều kiện thêm trường vào VendorOderDetail: số lượng đã nhập, Trạng thái nhậps
                var listCustomerOrder = context.CustomerOrder.Where(x => x.Active == true && listStatusId.Contains(x.StatusId.Value) && x.CustomerId == parameter.CustomerId).OrderBy(x => x.OrderDate).ToList();

                listCustomerOrder.ForEach(item =>
                {
                    var productCustomerOrder = customerOrderDetail.Where(v => v.OrderId == item.OrderId).ToList();
                    productCustomerOrder.ForEach(proitem =>
                    {
                        if (proitem.ProductId != null)
                        {
                            decimal quantityInventoryReceiving = inventoryReceivingVoucherMapping.Where(i => i.ProductId == proitem.ProductId && i.ObjectId == proitem.OrderDetailId).Sum(i => i.QuantityActual);
                            if (quantityInventoryReceiving < proitem.Quantity)
                            {
                                result.Add(item);
                            }
                        }
                    });
                });

                result = result.Distinct().ToList();
                return new GetListCustomerOrderByIdCustomerIdResult()
                {
                    Status = true,
                    Message = "",
                    listCustomerOrder = result
                };


            }
            catch (Exception e)
            {

                return new GetListCustomerOrderByIdCustomerIdResult()
                {
                    Status = false,
                    Message = "Có lỗi xảy ra",
                };

            }
        }

        public CheckQuantityActualReceivingVoucherResult CheckQuantityActualReceivingVoucher(CheckQuantityActualReceivingVoucherParameter parameter)
        {
            try
            {
                decimal SumTotalQuantityActual = context.InventoryReceivingVoucherMapping.Where(wh => wh.ObjectId == parameter.ObjectId).Sum(s => s.QuantityActual);
                decimal? Quantity = 0;
                bool IsEnoughX = false;
                if (parameter.Type == 1)
                {
                    Quantity = context.VendorOrderDetail.FirstOrDefault(f => f.VendorOrderDetailId == parameter.ObjectId).Quantity;
                }
                else
                {
                    Quantity = context.CustomerOrderDetail.FirstOrDefault(f => f.OrderDetailId == parameter.ObjectId).Quantity;
                }

                if (Quantity.HasValue)
                {
                    if (SumTotalQuantityActual == Quantity)
                    {
                        IsEnoughX = true;
                    }
                    else
                    {
                        IsEnoughX = false;
                    }
                }

                return new CheckQuantityActualReceivingVoucherResult
                {
                    IsEnough = IsEnoughX,
                    SumTotalQuantityActual = SumTotalQuantityActual,
                    Status = true
                };
            }
            catch (Exception)
            {
                return new CheckQuantityActualReceivingVoucherResult
                {
                    Message = "Loi",
                    Status = false
                };

            }

        }
        //Method Prive
        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? ExchangeRate, decimal? Vat, decimal? DiscountValue, bool? DiscountType)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;

            if (Vat != null)
            {
                CaculateVAT = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value * Vat.Value) / 100;
            }
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
            result = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value) + CaculateVAT - CacuDiscount;
            return result;
        }

        public GetCustomerOrderDetailByCustomerOrderIdResult GetCustomerOrderDetailByCustomerOrderId(GetCustomerOrderDetailByCustomerOrderIdParameter parameter)
        {
            try
            {
                var customerOrders = context.CustomerOrder.Where(co => co.Active == true).ToList();
                var customerOrderDetails = context.CustomerOrderDetail.Where(vo => vo.Active == true).ToList();
                var categoryTypeIdUnit = context.CategoryType.FirstOrDefault(cty => cty.Active == true && cty.CategoryTypeCode == "DNH").CategoryTypeId;
                var categories = context.Category.Where(ct => ct.Active == true && ct.CategoryTypeId == categoryTypeIdUnit).ToList();
                var product = context.Product.Where(p => p.Active == true).ToList();
                var warehouseSerial = context.Serial.Where(p => p.Active == true).ToList();
                var result = new List<GetVendorOrderDetailByVenderOrderIdEntityModel>();
                //TypeWarehouseVocher:1 phiếu nhập

                if (parameter.TypeWarehouseVocher == 1)
                {
                    var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                    var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;
                    //Lọc những đơn hàng đã nhập kho hết
                    var listInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(wh => wh.StatusId == categoryIdNHK).Select(s => s.InventoryReceivingVoucherId).ToList();
                    var inventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.Where(wh => listInventoryReceivingVoucher.Contains(wh.InventoryReceivingVoucherId)).ToList();

                    //var inventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.ToList();

                    parameter.ListCustomerOrderId.ForEach(item =>
                    {
                        var customerOrderDetail = customerOrderDetails.Where(vod => vod.OrderId == item).ToList();
                        customerOrderDetail.ForEach(detail =>
                        {
                            if (detail.ProductId != null)
                            {
                                decimal quantityInventoryReceiving = inventoryReceivingVoucherMapping.Where(i => i.ProductId == detail.ProductId && i.ObjectId == detail.OrderDetailId).Sum(i => i.QuantityActual);
                                if (quantityInventoryReceiving < detail.Quantity)
                                {

                                    GetVendorOrderDetailByVenderOrderIdEntityModel obj = new GetVendorOrderDetailByVenderOrderIdEntityModel();
                                    obj.VendorOrderId = item;
                                    obj.VendorOrderDetailId = detail.OrderDetailId;
                                    obj.VendorOrderCode = customerOrders.FirstOrDefault(vo => vo.OrderId == item).OrderCode;
                                    obj.ProductId = detail.ProductId;
                                    obj.ProductName = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductName;
                                    obj.ProductCode = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductCode;
                                    obj.UnitId = detail.UnitId;
                                    obj.UnitName = detail.UnitId == null ? "" : categories.FirstOrDefault(c => c.CategoryId == detail.UnitId).CategoryName;
                                    obj.QuantityRequire = detail.Quantity - quantityInventoryReceiving;
                                    obj.Quantity = detail.Quantity - quantityInventoryReceiving;
                                    obj.Note = "";
                                    obj.TotalSerial = 0;
                                    obj.Price = detail.UnitPrice;
                                    obj.ListSerial = new List<Serial>();
                                    obj.WareHouseId = Guid.Empty;
                                    obj.WareHouseName = "";
                                    obj.CurrencyUnit = detail.CurrencyUnit;
                                    obj.ExchangeRate = detail.ExchangeRate;
                                    obj.Vat = detail.Vat;
                                    obj.DiscountType = detail.DiscountType;
                                    obj.DiscountValue = detail.DiscountValue;
                                    obj.SumAmount = SumAmount(detail.Quantity, detail.UnitPrice, detail.ExchangeRate, detail.Vat, detail.DiscountValue, detail.DiscountType);
                                    obj.NameMoneyUnit = detail.CurrencyUnit != null ? context.Category.FirstOrDefault(c => c.CategoryId == detail.CurrencyUnit).CategoryName : "";
                                    result.Add(obj);
                                }
                            }
                        });

                    });
                }
                else
                {
                    var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX" && ct.Active == true).CategoryTypeId;
                    var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;
                    //Lọc những đơn hàng đã nhập kho hết
                    var listInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.Where(wh => wh.StatusId == categoryIdNHK).ToList();
                    var listInventoryDeliveryVoucherId = listInventoryDeliveryVoucher.Select(s => s.InventoryDeliveryVoucherId).ToList();
                    var inventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping.Where(wh => listInventoryDeliveryVoucherId.Contains(wh.InventoryDeliveryVoucherId)).ToList();

                    //var inventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.ToList();

                    parameter.ListCustomerOrderId.ForEach(item =>
                    {
                        var lstInventoryDeliveryVoucherId = listInventoryDeliveryVoucher.Where(wh => wh.ObjectId == item).Select(s => s.InventoryDeliveryVoucherId).ToList();

                        var customerOrderDetail = customerOrderDetails.Where(vod => vod.OrderId == item).ToList();
                        customerOrderDetail.ForEach(detail =>
                        {
                            if (detail.ProductId != null)
                            {
                                decimal quantityInventoryReceiving = inventoryDeliveryVoucherMapping.Where(i => i.ProductId == detail.ProductId && lstInventoryDeliveryVoucherId.Contains(i.InventoryDeliveryVoucherId)).Sum(i => i.QuantityActual);
                                if (quantityInventoryReceiving < detail.Quantity)
                                {

                                    GetVendorOrderDetailByVenderOrderIdEntityModel obj = new GetVendorOrderDetailByVenderOrderIdEntityModel();
                                    obj.VendorOrderId = item;
                                    obj.VendorOrderDetailId = detail.OrderDetailId;
                                    obj.VendorOrderCode = customerOrders.FirstOrDefault(vo => vo.OrderId == item).OrderCode;
                                    obj.ProductId = detail.ProductId;
                                    obj.ProductName = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductName;
                                    obj.ProductCode = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductCode;
                                    obj.UnitId = detail.UnitId;
                                    obj.UnitName = detail.UnitId == null ? "" : categories.FirstOrDefault(c => c.CategoryId == detail.UnitId).CategoryName;
                                    obj.QuantityRequire = detail.Quantity - quantityInventoryReceiving;
                                    obj.Quantity = detail.Quantity - quantityInventoryReceiving;
                                    obj.Note = "";
                                    obj.TotalSerial = 0;
                                    obj.Price = detail.UnitPrice;
                                    obj.ListSerial = new List<Serial>();
                                    obj.WareHouseId = Guid.Empty;
                                    obj.WareHouseName = "";
                                    obj.CurrencyUnit = detail.CurrencyUnit;
                                    obj.ExchangeRate = detail.ExchangeRate;
                                    obj.Vat = detail.Vat;
                                    obj.DiscountType = detail.DiscountType;
                                    obj.DiscountValue = detail.DiscountValue;
                                    obj.SumAmount = SumAmount(detail.Quantity, detail.UnitPrice, detail.ExchangeRate, detail.Vat, detail.DiscountValue, detail.DiscountType);
                                    obj.NameMoneyUnit = detail.CurrencyUnit != null ? context.Category.FirstOrDefault(c => c.CategoryId == detail.CurrencyUnit).CategoryName : "";
                                    result.Add(obj);
                                }
                            }
                        });

                    });
                }
                /*End*/

                return new GetCustomerOrderDetailByCustomerOrderIdResult()
                {
                    Status = true,
                    Message = "",
                    ListOrderProduct = result
                };
            }
            catch (Exception e)
            {
                return new GetCustomerOrderDetailByCustomerOrderIdResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }

        }
        /// <summary>
        /// Loc nha cung cap
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public FilterVendorResult FilterVendor(FilterVendorParameter parameter)
        {
            try
            {
                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct =>
                    ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;

                //Lọc những đơn hàng chưa nhập kho hết
                var listInventoryReceivingVoucher = context.InventoryReceivingVoucher
                    .Where(wh => wh.StatusId == categoryIdNHK).Select(s => s.InventoryReceivingVoucherId).ToList();
                var listInventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping
                    .Where(wh => listInventoryReceivingVoucher.Contains(wh.InventoryReceivingVoucherId)).ToList();

                //Lấy Id các trạng thái đơn hàng: Đang xử lý, Đã thanh toán, Đã giao hàng và đã đóng
                var listStatusCode = new List<string>() { "PURC" };
                var listStatusId = context.PurchaseOrderStatus
                    .Where(ct => listStatusCode.Contains(ct.PurchaseOrderStatusCode) && ct.Active)
                    .Select(ct => ct.PurchaseOrderStatusId).ToList();
                var listVendorOrder = context.VendorOrder
                    .Where(x => x.Active == true && listStatusId.Contains(x.StatusId)).OrderBy(x => x.VendorOrderDate)
                    .ToList();
                var listVendorOrderId = listVendorOrder.Select(s => s.VendorOrderId).ToList();
                var listVendorOrderDetails = context.VendorOrderDetail
                    .Where(wh => listVendorOrderId.Contains(wh.VendorOrderId)).ToList();

                var listResultValidate = new List<VendorOrder>();
                listVendorOrder.ForEach(vendororder =>
                {
                    bool IsEnough = true;
                    var lstOrderDetails = listVendorOrderDetails
                        .Where(wh => wh.VendorOrderId == vendororder.VendorOrderId).ToList();
                    if (lstOrderDetails.Count > 0)
                    {
                        lstOrderDetails.ForEach(item =>
                        {
                            var SumQualityActual = listInventoryReceivingVoucherMapping
                                .Where(wh => wh.ObjectId == item.VendorOrderDetailId).Sum(s => s.QuantityActual);
                            if (SumQualityActual < item.Quantity)
                            {
                                IsEnough = false;
                            };
                        });
                    }
                    if (!IsEnough)
                    {
                        listResultValidate.Add(vendororder);
                    }
                });
                ///DS vendor co don
                var listVendorID = listResultValidate.Select(s => s.VendorId).Distinct().ToList();
                var VendorList = context.Vendor.Where(wh => listVendorID.Contains(wh.VendorId))
                    .Select(v => new VendorEntityModel
                    {
                        VendorId = v.VendorId,
                        VendorName = v.VendorName,
                        VendorGroupId = v.VendorGroupId,
                        VendorCode = v.VendorCode,
                        TotalPurchaseValue = v.TotalPurchaseValue,
                        TotalPayableValue = v.TotalPayableValue,
                        NearestDateTransaction = v.NearestDateTransaction,
                        PaymentId = v.PaymentId,
                        CreatedById = v.CreatedById,
                        CreatedDate = v.CreatedDate,
                        UpdatedById = v.UpdatedById,
                        UpdatedDate = v.UpdatedDate,
                        Active = v.Active,
                    }).OrderByDescending(v => v.CreatedDate).ToList();

                return new FilterVendorResult
                {
                    VendorList = VendorList,
                    Status = true
                };
            }
            catch (Exception)
            {
                return new FilterVendorResult
                {
                    VendorList = new List<VendorEntityModel>(),
                    Status = false
                };
            };
        }
        /// <summary>
        /// Loc khach hang
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public FilterCustomerResult FilterCustomer(FilterCustomerParameter parameter)
        {
            try
            {
                var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;

                //Lọc những đơn hàng chưa nhập kho hết
                var listInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(wh => wh.StatusId == categoryIdNHK).Select(s => s.InventoryReceivingVoucherId).ToList();
                var listInventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.Where(wh => listInventoryReceivingVoucher.Contains(wh.InventoryReceivingVoucherId)).ToList();
                //Lấy Id các trạng thái đơn hàng: Đang xử lý, Đã thanh toán, Đã giao hàng và đã đóng
                var listStatusCode = new List<string>() { "RTN" };
                var listStatusId = context.OrderStatus.Where(ct => listStatusCode.Contains(ct.OrderStatusCode) && ct.Active == true).Select(ct => ct.OrderStatusId).ToList();
                var listCustomerOrder = context.CustomerOrder.Where(x => x.Active == true && listStatusId.Contains(x.StatusId.Value)).OrderBy(x => x.OrderDate).ToList();
                var listCustomerOrderId = listCustomerOrder.Select(s => s.OrderId).ToList();
                var listCustomerorderDetail = context.CustomerOrderDetail.Where(wh => listCustomerOrderId.Contains(wh.OrderId)).ToList();
                var listResultValidate = new List<CustomerOrder>();

                listCustomerOrder.ForEach(customerorder =>
                {
                    bool IsEnough = true;
                    var lstOrderDetails = listCustomerorderDetail.Where(wh => wh.OrderId == customerorder.OrderId).ToList();
                    if (lstOrderDetails.Count > 0)
                    {
                        lstOrderDetails.ForEach(item =>
                        {
                            var SumQualityActual = listInventoryReceivingVoucherMapping.Where(wh => wh.ObjectId == item.OrderDetailId).Sum(s => s.QuantityActual);
                            if (SumQualityActual < item.Quantity)
                            {
                                IsEnough = false;
                                return;
                            };
                        });

                    }
                    if (!IsEnough)
                    {
                        listResultValidate.Add(customerorder);
                    }
                });


                var listCustomerID = listResultValidate.Select(s => s.CustomerId).Distinct().ToList();

                var CustomerList = context.Customer.Where(wh => listCustomerID.Contains(wh.CustomerId))
                                  .Select(c => new CustomerEntityModel
                                  {
                                      CustomerId = c.CustomerId,
                                      CustomerCode = c.CustomerCode,
                                      CustomerName = c.CustomerName,
                                  }).OrderByDescending(date => date.CreatedDate).ToList();

                return new FilterCustomerResult
                {
                    Customer = CustomerList,
                    Status = true
                };
            }
            catch (Exception)
            {
                return new FilterCustomerResult
                {
                    Customer = new List<CustomerEntityModel>(),
                    Status = false
                };
            };
        }

        public ChangeStatusInventoryReceivingVoucherResult ChangeStatusInventoryReceivingVoucher(ChangeStatusInventoryReceivingVoucherParameter parameter)
        {
            try
            {
                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct =>
                    ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;
                //var OrderStatusTypeId =
                //    context.OrderStatus.FirstOrDefault(ct => ct.OrderStatusCode == "DNK").OrderStatusId;
                var InventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(f =>
                    f.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);
                var inventoryReceivingVoucherMappingEntity = context.InventoryReceivingVoucherMapping
                    .Where(wh =>
                        wh.InventoryReceivingVoucherId == InventoryReceivingVoucher.InventoryReceivingVoucherId)
                    .ToList();

                InventoryReceivingVoucher.StatusId = categoryIdNHK;
                //InventoryReceivingVoucher.InventoryReceivingVoucherDate = DateTime.Now;
                TimeSpan today = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                InventoryReceivingVoucher.InventoryReceivingVoucherTime = today;
                var userNK = context.User.FirstOrDefault(f => f.UserId == parameter.UserId);
                if (userNK != null)
                {
                    Employee defaultEmpty = new Employee();
                    InventoryReceivingVoucher.Storekeeper = context.Employee
                        .Where(f => f.EmployeeId == userNK.EmployeeId).DefaultIfEmpty(defaultEmpty).FirstOrDefault()
                        .EmployeeId;
                }
                context.InventoryReceivingVoucher.Update(InventoryReceivingVoucher);
                context.SaveChanges();

                //update vao bang Ton kho
                inventoryReceivingVoucherMappingEntity.ForEach(voucherMapping =>
                {
                    var inventoryReportByProduct = context.InventoryReport.FirstOrDefault(wh =>
                        wh.ProductId == voucherMapping.ProductId && wh.WarehouseId == voucherMapping.WarehouseId);
                    if (inventoryReportByProduct == null)
                    {
                        InventoryReport inventoryReport = new InventoryReport();
                        inventoryReport.InventoryReportId = Guid.NewGuid();
                        inventoryReport.WarehouseId = voucherMapping.WarehouseId;
                        inventoryReport.ProductId = voucherMapping.ProductId;
                        inventoryReport.Quantity = voucherMapping.QuantityActual;
                        inventoryReport.QuantityMinimum = 0;
                        inventoryReport.Active = true;
                        inventoryReport.CreatedDate = DateTime.Now;
                        context.InventoryReport.Add(inventoryReport);
                    }
                    else
                    {
                        inventoryReportByProduct.Quantity += voucherMapping.QuantityActual;
                        context.InventoryReport.Update(inventoryReportByProduct);
                    }
                });
                context.SaveChanges();

                ///Update status cho order
                var vendorOrderDetails = new List<VendorOrderDetail>();
                var listVendorOrder = new List<VendorOrder>();
                var customerOrderDetails = new List<CustomerOrderDetail>();
                var lstcustomerOrder = new List<CustomerOrder>();

                if (InventoryReceivingVoucher.InventoryReceivingVoucherType == 1)
                {
                    var lstinventoryReceivingVoucherMappingEntityId =
                        inventoryReceivingVoucherMappingEntity.Select(s => s.ObjectId).ToList();
                    vendorOrderDetails = context.VendorOrderDetail.Where(vo =>
                        vo.Active == true &&
                        lstinventoryReceivingVoucherMappingEntityId.Contains(vo.VendorOrderDetailId)).ToList();
                    var lstVendorOderId = vendorOrderDetails.Select(s => s.VendorOrderId).Distinct().ToList();
                    listVendorOrder = context.VendorOrder.Where(wh => lstVendorOderId.Contains(wh.VendorOrderId))
                        .ToList();

                    var listVendorOrderDetails = context.VendorOrderDetail
                        .Where(wh => lstVendorOderId.Contains(wh.VendorOrderId)).ToList();
                    if (listVendorOrder.Count > 0)
                    {
                        listVendorOrder.ForEach(vendororder =>
                        {
                            bool IsEnough = true;
                            var lstOrderDetails = listVendorOrderDetails
                                .Where(wh => wh.VendorOrderId == vendororder.VendorOrderId).ToList();
                            if (lstOrderDetails.Count > 0)
                            {
                                lstOrderDetails.ForEach(item =>
                                {
                                    var SumQualityActual = inventoryReceivingVoucherMappingEntity
                                        .Where(wh => wh.ObjectId == item.VendorOrderDetailId)
                                        .Sum(s => s.QuantityActual);
                                    if (SumQualityActual < item.Quantity)
                                    {
                                        IsEnough = false;
                                    };
                                });
                            }
                            else
                            {
                                IsEnough = false;
                            }

                            if (IsEnough)
                            {
                                #region Đổi trạng thái đơn hàng mua sang đã nhập kho: Giang comment

                                //var OrderEntity = context.VendorOrder
                                //    .Where(wh => wh.VendorOrderId == vendororder.VendorOrderId).First();
                                //OrderEntity.StatusId = OrderStatusTypeId;
                                //context.VendorOrder.Update(OrderEntity);
                                //context.SaveChanges();

                                #endregion
                            }
                        });
                    }
                }
                else if (InventoryReceivingVoucher.InventoryReceivingVoucherType == 2)
                {
                    var lstinventoryReceivingVoucherMappingEntityId =
                        inventoryReceivingVoucherMappingEntity.Select(s => s.ObjectId).ToList();
                    customerOrderDetails = context.CustomerOrderDetail.Where(vo =>
                            vo.Active == true && lstinventoryReceivingVoucherMappingEntityId.Contains(vo.OrderDetailId))
                        .ToList();
                    var lstCustomerOderId = customerOrderDetails.Select(s => s.OrderId).Distinct().ToList();
                    lstcustomerOrder = context.CustomerOrder.Where(wh => lstCustomerOderId.Contains(wh.OrderId)).ToList();

                    var listCustomerOrderDetails = context.CustomerOrderDetail
                        .Where(wh => lstCustomerOderId.Contains(wh.OrderId)).ToList();

                    if (lstcustomerOrder.Count > 0)
                    {
                        lstcustomerOrder.ForEach(customerorder =>
                        {
                            bool IsEnough = true;
                            var lstOrderDetails = listCustomerOrderDetails
                                .Where(wh => wh.OrderId == customerorder.OrderId).ToList();
                            if (lstOrderDetails.Count > 0)
                            {
                                lstOrderDetails.ForEach(item =>
                                {
                                    var SumQualityActual = inventoryReceivingVoucherMappingEntity
                                        .Where(wh => wh.ObjectId == item.OrderDetailId).Sum(s => s.QuantityActual);
                                    if (SumQualityActual < item.Quantity)
                                    {
                                        IsEnough = false;
                                    };
                                });
                            }
                            else
                            {
                                IsEnough = false;
                            }

                            if (IsEnough)
                            {
                                #region Đổi trạng thái đơn hàng sang đã nhập kho: Giang comment

                                //var OrderEntity = context.CustomerOrder.Where(wh => wh.OrderId == customerorder.OrderId)
                                //    .First();
                                //OrderEntity.StatusId = OrderStatusTypeId;
                                //context.CustomerOrder.Update(OrderEntity);
                                //context.SaveChanges();

                                #endregion
                            }
                        });
                    }
                }

                return new ChangeStatusInventoryReceivingVoucherResult
                {
                    Status = true,
                    Message = "Đã nhập kho thành công"
                };
            }
            catch (Exception)
            {
                return new ChangeStatusInventoryReceivingVoucherResult
                {
                    Status = false,
                    Message = "Có lỗi khi tiến hành nhập kho"
                };
            }
        }

        public InventoryDeliveryVoucherFilterCustomerOrderResult InventoryDeliveryVoucherFilterCustomerOrder(InventoryDeliveryVoucherFilterCustomerOrderParameter parameter)
        {
            try
            {
                var listStatusCode = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "IDO")
                    ?.SystemValueString.Split(';').ToList();
                var lstCustomerOrder = context.CustomerOrder.ToList();
                var listStatusId = context.OrderStatus
                    .Where(ct => listStatusCode.Contains(ct.OrderStatusCode) && ct.Active == true)
                    .Select(ct => ct.OrderStatusId).ToList();
                var listCustomerOrder = lstCustomerOrder
                    .Where(x => x.Active == true && listStatusId.Contains(x.StatusId.Value)).OrderBy(x => x.OrderDate)
                    .ToList();

                //check cac don hang da nhap kho het chua
                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct =>
                    ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;

                //Lọc những đơn hàng đã nhập kho hết
                var result = new List<CustomerOrder>();
                listCustomerOrder.ForEach(cusOrder =>
                {
                    if (!checkCustomerOrderIsEnough(cusOrder, categoryIdNHK))
                    {
                        result.Add(cusOrder);
                    }
                });

                return new InventoryDeliveryVoucherFilterCustomerOrderResult
                {
                    listCustomerOrder = result,
                    Status = true
                };
            }
            catch (Exception)
            {
                return new InventoryDeliveryVoucherFilterCustomerOrderResult
                {
                    Message = "Có lỗi xảy ra",
                    listCustomerOrder = new List<CustomerOrder>(),
                    Status = false
                };
            }
        }

        public InventoryDeliveryVoucherFilterVendorOrderResult InventoryDeliveryVoucherFilterVendorOrder(InventoryDeliveryVoucherFilterVendorOrderParameter parameter)
        {
            try
            {
                var listStatusCode = new List<string>() { "RTN" };
                var listStatusId = context.OrderStatus.Where(ct => listStatusCode.Contains(ct.OrderStatusCode) && ct.Active == true).Select(ct => ct.OrderStatusId).ToList();
                var listVendorOrder = context.VendorOrder.Where(x => x.Active == true && listStatusId.Contains(x.StatusId)).OrderBy(x => x.VendorOrderDate).ToList();
                var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;
                var result = new List<VendorOrder>();
                listVendorOrder.ForEach(venOrder =>
                {
                    if (!checkVendorOrderIsEnough(venOrder, categoryIdNHK))
                    {
                        result.Add(venOrder);
                    }
                });

                return new InventoryDeliveryVoucherFilterVendorOrderResult
                {
                    listVendorOrder = result,
                    Status = true
                };

            }
            catch (Exception)
            {

                return new InventoryDeliveryVoucherFilterVendorOrderResult
                {
                    Message = "Có lỗi xảy ra",
                    listVendorOrder = new List<VendorOrder>(),
                    Status = false
                };
            }

        }
        public DeleteInventoryReceivingVoucherResult DeleteInventoryReceivingVoucher(DeleteInventoryReceivingVoucherParameter parameter)
        {
            try
            {
                var InventoryReceivingVoucherEntity = context.InventoryReceivingVoucher.FirstOrDefault(f => f.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);
                var InventoryReceivingVoucherMappingObject = context.InventoryReceivingVoucherMapping.Where(wh => wh.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).ToList();
                var lstInventoryReceivingVoucherMappingId = InventoryReceivingVoucherMappingObject.Select(s => s.InventoryReceivingVoucherMappingId).ToList().Distinct();
                var InventoryReceivingVoucherSerialMappingObject = context.InventoryReceivingVoucherSerialMapping
                                                                .Where(wh => lstInventoryReceivingVoucherMappingId
                                                                .Contains(wh.InventoryReceivingVoucherSerialMappingId)).ToList();
                //update lai Quantity trong inventoryReports
                InventoryReceivingVoucherMappingObject.ForEach(item =>
                {
                    var inventoryReportByProduct = context.InventoryReport.FirstOrDefault(wh => wh.ProductId == item.ProductId && wh.WarehouseId == item.WarehouseId);
                    if (inventoryReportByProduct != null)
                    {
                        inventoryReportByProduct.Quantity = inventoryReportByProduct.Quantity - item.QuantityActual;
                        context.InventoryReport.Update(inventoryReportByProduct);
                        context.SaveChanges();
                    }

                });

                context.InventoryReceivingVoucherSerialMapping.RemoveRange(InventoryReceivingVoucherSerialMappingObject);
                context.InventoryReceivingVoucherMapping.RemoveRange(InventoryReceivingVoucherMappingObject);
                context.InventoryReceivingVoucher.Remove(InventoryReceivingVoucherEntity);
                context.SaveChanges();
                return new DeleteInventoryReceivingVoucherResult
                {
                    Message = "Đã xóa phiếu nhập kho",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new DeleteInventoryReceivingVoucherResult
                {
                    Message = "Có lỗi trong quá trình xóa phiếu nhập kho",
                    Status = false
                };

            }
            //delete item relationship

        }

        public GetTop10WarehouseFromReceivingVoucherResult GetTop10WarehouseFromReceivingVoucher(GetTop10WarehouseFromReceivingVoucherParameter parameter)
        {
            try
            {
                var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPH" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;

                var lstTop10InventoryReceivingVoucher = context.InventoryReceivingVoucher
                                                        .Where(wh => wh.StatusId == categoryIdNHK)
                                                        .OrderByDescending(or => or.InventoryReceivingVoucherDate)
                                                        .Select(s => s.InventoryReceivingVoucherId)
                                                        .Take(10).ToList();
                var listInventoryReceivingVoucherMap = context.InventoryReceivingVoucherMapping
                                                       .Where(wh => lstTop10InventoryReceivingVoucher.Contains(wh.InventoryReceivingVoucherId)).ToList();

                return new GetTop10WarehouseFromReceivingVoucherResult
                {
                    lstInventoryReceivingVoucherMapping = listInventoryReceivingVoucherMap,
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new GetTop10WarehouseFromReceivingVoucherResult
                {
                    lstInventoryReceivingVoucherMapping = new List<InventoryReceivingVoucherMapping>(),
                    Status = true
                };
            }
        }

        public GetSerialResult GetSerial(GetSerialParameter parameter)
        {
            try
            {
                var categoryTypeStatusSerialId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TSE" && ct.Active == true).CategoryTypeId;
                var categoryIdDXU = context.Category.FirstOrDefault(ct => ct.CategoryCode == "CXU" && ct.CategoryTypeId == categoryTypeStatusSerialId && ct.Active == true).CategoryId;

                var lstInventoryReceivingVoucherSerialMappingSerialId = context.InventoryReceivingVoucherSerialMapping.Select(s => s.SerialId).ToList();
                var lstSerialReceivingVoucher = context.Serial.Where(wh => lstInventoryReceivingVoucherSerialMappingSerialId.Contains(wh.SerialId)
                                                && wh.StatusId == categoryIdDXU).ToList();
                var lstResultSerial = lstSerialReceivingVoucher
                                      .Where(wh => (parameter.ProductId == null || wh.ProductId == parameter.ProductId)
                                            && (parameter.WarehouseId == null || wh.WarehouseId == parameter.WarehouseId))
                                      .OrderBy(or => or.SerialCode)
                                      .ToList();
                return new GetSerialResult
                {
                    lstSerial = lstResultSerial,
                    Status = true
                };
            }
            catch (Exception)
            {
                return new GetSerialResult
                {
                    lstSerial = new List<Serial>(),
                    Status = false
                };
            }

        }

        public CreateUpdateInventoryDeliveryVoucherResult CreateUpdateInventoryDeliveryVoucher(CreateUpdateInventoryDeliveryVoucherParameter parameter)
        {
            try
            {
                if (parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId == Guid.Empty)
                {

                    var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX" && ct.Active == true).CategoryTypeId;
                    var categoryId = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHA" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;

                    var categoryTypeIdSerial = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TSE" && ct.Active == true).CategoryTypeId;
                    var categoryListSerial = context.Category.Where(ct => ct.CategoryTypeId == categoryTypeIdSerial && ct.Active == true).ToList();

                    var inventoryReports = context.InventoryReport.Where(w => w.Active == true).ToList();

                    var datenow = DateTime.Now;
                    var totalInvertoryCreate = context.InventoryDeliveryVoucher.Where(c => Convert.ToDateTime(c.CreatedDate).Day == datenow.Day && Convert.ToDateTime(c.CreatedDate).Month == datenow.Month && Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Count();
                    parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId = Guid.NewGuid();
                    parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherCode = "PX-" + ConverCreateId(totalInvertoryCreate + 1);
                    if (parameter.inventoryDeliveryVoucher.Receiver != string.Empty && parameter.inventoryDeliveryVoucher.Receiver != null)
                    {
                        parameter.inventoryDeliveryVoucher.Receiver = parameter.inventoryDeliveryVoucher.Receiver.Trim();
                    }
                    if (parameter.inventoryDeliveryVoucher.Reason != string.Empty && parameter.inventoryDeliveryVoucher.Reason != null)
                    {
                        parameter.inventoryDeliveryVoucher.Reason = parameter.inventoryDeliveryVoucher.Reason.Trim();
                    }
                    parameter.inventoryDeliveryVoucher.StatusId = categoryId;
                    parameter.inventoryDeliveryVoucher.Active = true;
                    parameter.inventoryDeliveryVoucher.CreatedDate = DateTime.Now;
                    parameter.inventoryDeliveryVoucher.CreatedById = parameter.UserId;
                    context.InventoryDeliveryVoucher.Add(parameter.inventoryDeliveryVoucher);

                    var note = new Note();
                    note.ObjectId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId;
                    note.NoteId = Guid.NewGuid();
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.ObjectType = "WH";
                    note.Description = string.IsNullOrEmpty(parameter.noteContent) ? string.Empty : parameter.noteContent;
                    note.NoteTitle = "đã tạo";
                    note.Type = "ADD";
                    context.Note.Add(note);
                    if (parameter.fileList != null)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = hostingEnvironment.WebRootPath;

                        //upload file to Server
                        if (parameter.fileList != null && parameter.fileList.Count > 0)
                        {

                            string checkexistPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(checkexistPath))
                            {
                                Directory.CreateDirectory(checkexistPath);
                            }
                            foreach (IFormFile item in parameter.fileList)
                            {
                                if (item.Length > 0)
                                {
                                    string fileName = item.FileName.Trim();
                                    string fullPath = Path.Combine(checkexistPath, fileName);
                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        item.CopyTo(stream);
                                    }
                                }
                            }
                        }
                        // Add note
                        var noteAttach = new Note();
                        noteAttach.ObjectId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId;
                        noteAttach.NoteId = Guid.NewGuid();
                        noteAttach.Active = true;
                        noteAttach.CreatedById = parameter.UserId;
                        noteAttach.CreatedDate = DateTime.Now;
                        noteAttach.ObjectType = "WH";
                        noteAttach.NoteTitle = "đã thêm tài liệu";
                        noteAttach.Type = "ADD";

                        // add noteDocument
                        List<NoteDocument> docList = new List<NoteDocument>();
                        string newPath = Path.Combine(webRootPath, folderName);
                        foreach (var file in parameter.fileList)
                        {
                            NoteDocument noteDoc = new NoteDocument()
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = noteAttach.NoteId,
                                DocumentName = file.FileName,
                                DocumentSize = file.Length.ToString(),
                                DocumentUrl = Path.Combine(newPath, file.FileName),
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            docList.Add(noteDoc);
                        }

                        if (docList.Count > 0)
                        {
                            docList.ForEach(dl => { context.NoteDocument.Add(dl); });
                        }
                        context.Note.Add(noteAttach);
                    }

                    parameter.inventoryyDeliveryVoucherMapping.ForEach(item =>
                    {
                        if (item.ProductId != null)
                        {
                            InventoryDeliveryVoucherMapping voucherMapping = new InventoryDeliveryVoucherMapping();
                            voucherMapping.InventoryDeliveryVoucherMappingId = Guid.NewGuid();
                            voucherMapping.InventoryDeliveryVoucherId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId;
                            voucherMapping.ProductId = (Guid)item.ProductId;
                            voucherMapping.QuantityRequest = (decimal)item.QuantityRequire;
                            voucherMapping.QuantityActual = (decimal)item.Quantity;
                            voucherMapping.PriceProduct = (decimal)item.Price;
                            if (item.QuantityInventory != null)
                            {
                                voucherMapping.QuantityInventory = (decimal)item.QuantityInventory;
                            }
                            voucherMapping.UnitId = item.UnitId;
                            voucherMapping.CurrencyUnit = item.CurrencyUnit;
                            voucherMapping.ExchangeRate = item.ExchangeRate;
                            voucherMapping.DiscountType = item.DiscountType;
                            voucherMapping.DiscountValue = item.DiscountValue;
                            voucherMapping.Vat = item.Vat;
                            voucherMapping.WarehouseId = item.WarehouseId;
                            voucherMapping.Description = item.Note;
                            voucherMapping.Active = true;
                            voucherMapping.CreatedDate = DateTime.Now;
                            voucherMapping.CreatedById = parameter.UserId;

                            if (item.ListSerial != null)
                            {
                                item.ListSerial.ForEach(itemSerial =>
                                {
                                    //Serial serial = new Serial();
                                    //serial.SerialId = Guid.NewGuid();
                                    //serial.SerialCode = itemSerial.SerialCode;
                                    //serial.WarehouseId = item.WarehouseId;
                                    //serial.ProductId = (Guid)item.ProductId;
                                    //serial.Active = true;
                                    //serial.CreatedDate = DateTime.Now;
                                    //serial.StatusId = categoryListSerial.FirstOrDefault(sr => sr.CategoryCode == "DXU").CategoryId;
                                    //context.Serial.Add(serial);

                                    InventoryDeliveryVoucherSerialMapping mapserial = new InventoryDeliveryVoucherSerialMapping();
                                    mapserial.InventoryDeliveryVoucherSerialMappingId = Guid.NewGuid();
                                    mapserial.InventoryDeliveryVoucherMappingId = voucherMapping.InventoryDeliveryVoucherMappingId;
                                    mapserial.SerialId = itemSerial.SerialId;
                                    mapserial.Active = true;
                                    mapserial.CreatedDate = DateTime.Now;
                                    context.InventoryDeliveryVoucherSerialMapping.Add(mapserial);
                                });

                            }
                            context.InventoryDeliveryVoucherMapping.Add(voucherMapping);

                        }
                    });

                    context.SaveChanges();
                    return new CreateUpdateInventoryDeliveryVoucherResult
                    {
                        InventoryDeliveryVoucherId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId,
                        Message = "Tạo thành công",
                        Status = true
                    };
                }
                else
                {
                    var categoryTypeIdSerial = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TSE" && ct.Active == true).CategoryTypeId;
                    var categoryListSerial = context.Category.Where(ct => ct.CategoryTypeId == categoryTypeIdSerial && ct.Active == true).ToList();

                    var inventoryReports = context.InventoryReport.Where(w => w.Active == true).ToList();

                    //parameter.inventoryReceivingVoucher.CreatedDate = DateTime.Now;
                    //parameter.inventoryReceivingVoucher.CreatedById = parameter.UserId;
                    if (parameter.inventoryDeliveryVoucher.Receiver != string.Empty && parameter.inventoryDeliveryVoucher.Receiver != null)
                    {
                        parameter.inventoryDeliveryVoucher.Receiver = parameter.inventoryDeliveryVoucher.Receiver.Trim();
                    }
                    if (parameter.inventoryDeliveryVoucher.Reason != string.Empty && parameter.inventoryDeliveryVoucher.Reason != null)
                    {
                        parameter.inventoryDeliveryVoucher.Reason = parameter.inventoryDeliveryVoucher.Reason.Trim();
                    }
                    parameter.inventoryDeliveryVoucher.UpdatedDate = DateTime.Now;
                    parameter.inventoryDeliveryVoucher.UpdatedById = parameter.UserId;
                    context.InventoryDeliveryVoucher.Update(parameter.inventoryDeliveryVoucher);

                    var note = new Note();
                    note.ObjectId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId;
                    note.NoteId = Guid.NewGuid();
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.ObjectType = "WH";
                    note.Description = string.IsNullOrEmpty(parameter.noteContent) ? string.Empty : parameter.noteContent;
                    note.NoteTitle = "Đã chỉnh sửa phiếu xuất kho này";
                    note.Type = "EDT";
                    context.Note.Add(note);
                    if (parameter.fileList != null)
                    {
                        string folderName = "FileUpload";
                        string webRootPath = hostingEnvironment.WebRootPath;

                        //upload file to Server
                        if (parameter.fileList != null && parameter.fileList.Count > 0)
                        {

                            string checkexistPath = Path.Combine(webRootPath, folderName);
                            if (!Directory.Exists(checkexistPath))
                            {
                                Directory.CreateDirectory(checkexistPath);
                            }
                            foreach (IFormFile item in parameter.fileList)
                            {
                                if (item.Length > 0)
                                {
                                    string fileName = item.FileName.Trim();
                                    string fullPath = Path.Combine(checkexistPath, fileName);
                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        item.CopyTo(stream);
                                    }
                                }
                            }
                        }
                        // Add note
                        var noteAttach = new Note();
                        noteAttach.ObjectId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId;
                        noteAttach.NoteId = Guid.NewGuid();
                        noteAttach.Active = true;
                        noteAttach.CreatedById = parameter.UserId;
                        noteAttach.CreatedDate = DateTime.Now;
                        noteAttach.ObjectType = "WH";
                        noteAttach.NoteTitle = "đã thêm tài liệu";
                        noteAttach.Type = "ADD";

                        // add noteDocument
                        List<NoteDocument> docList = new List<NoteDocument>();
                        string newPath = Path.Combine(webRootPath, folderName);
                        foreach (var file in parameter.fileList)
                        {
                            NoteDocument noteDoc = new NoteDocument()
                            {
                                NoteDocumentId = Guid.NewGuid(),
                                NoteId = noteAttach.NoteId,
                                DocumentName = file.FileName,
                                DocumentSize = file.Length.ToString(),
                                DocumentUrl = Path.Combine(newPath, file.FileName),
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                Active = true
                            };
                            docList.Add(noteDoc);
                        }

                        if (docList.Count > 0)
                        {
                            docList.ForEach(dl => { context.NoteDocument.Add(dl); });
                        }
                        context.Note.Add(noteAttach);
                    }
                    context.SaveChanges();

                    //delete item relationship
                    var InventoryDeliveryVoucherMappingObject = context.InventoryDeliveryVoucherMapping.Where(wh => wh.InventoryDeliveryVoucherId == parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId).ToList();
                    var lstInventoryDeliveryVoucherMappingId = InventoryDeliveryVoucherMappingObject.Select(s => s.InventoryDeliveryVoucherMappingId).ToList().Distinct();
                    var InventorDeliveryVoucherSerialMappingObject = context.InventoryDeliveryVoucherSerialMapping
                                                                    .Where(wh => lstInventoryDeliveryVoucherMappingId
                                                                    .Contains(wh.InventoryDeliveryVoucherSerialMappingId)).ToList();
                    //update lai Quantity trong inventoryReports
                    //InventorDeliveryVoucherSerialMappingObject.ForEach(item =>
                    //{
                    //    var inventoryReportByProduct = context.InventoryReport.FirstOrDefault(wh => wh.ProductId == item.ProductId && wh.WarehouseId == item.WarehouseId);
                    //    if (inventoryReportByProduct != null)
                    //    {
                    //        inventoryReportByProduct.Quantity = inventoryReportByProduct.Quantity - item.QuantityActual;
                    //        context.InventoryReport.Update(inventoryReportByProduct);
                    //        context.SaveChanges();
                    //    }

                    //});

                    context.InventoryDeliveryVoucherSerialMapping.RemoveRange(InventorDeliveryVoucherSerialMappingObject);
                    context.InventoryDeliveryVoucherMapping.RemoveRange(InventoryDeliveryVoucherMappingObject);
                    context.SaveChanges();

                    //tao lai tu dau
                    parameter.inventoryyDeliveryVoucherMapping.ForEach(item =>
                    {
                        if (item.ProductId != null)
                        {
                            InventoryDeliveryVoucherMapping voucherMapping = new InventoryDeliveryVoucherMapping();
                            voucherMapping.InventoryDeliveryVoucherMappingId = Guid.NewGuid();
                            voucherMapping.InventoryDeliveryVoucherId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId;
                            voucherMapping.ProductId = (Guid)item.ProductId;
                            voucherMapping.QuantityRequest = (decimal)item.QuantityRequire;
                            voucherMapping.QuantityActual = (decimal)item.Quantity;
                            voucherMapping.PriceProduct = (decimal)item.Price;
                            if (item.QuantityInventory != null)
                            {
                                voucherMapping.QuantityInventory = (decimal)item.QuantityInventory;
                            }
                            voucherMapping.UnitId = item.UnitId;
                            voucherMapping.CurrencyUnit = item.CurrencyUnit;
                            voucherMapping.ExchangeRate = item.ExchangeRate;
                            voucherMapping.DiscountType = item.DiscountType;
                            voucherMapping.DiscountValue = item.DiscountValue;
                            voucherMapping.Vat = item.Vat;
                            voucherMapping.WarehouseId = item.WarehouseId;
                            voucherMapping.Description = item.Note;
                            voucherMapping.Active = true;
                            voucherMapping.CreatedDate = DateTime.Now;
                            voucherMapping.CreatedById = parameter.UserId;

                            if (item.ListSerial != null)
                            {
                                item.ListSerial.ForEach(itemSerial =>
                                {
                                    //Serial serial = new Serial();
                                    //serial.SerialId = Guid.NewGuid();
                                    //serial.SerialCode = itemSerial.SerialCode;
                                    //serial.WarehouseId = item.WarehouseId;
                                    //serial.ProductId = (Guid)item.ProductId;
                                    //serial.Active = true;
                                    //serial.CreatedDate = DateTime.Now;
                                    //serial.StatusId = categoryListSerial.FirstOrDefault(sr => sr.CategoryCode == "DXU").CategoryId;
                                    //context.Serial.Add(serial);

                                    InventoryDeliveryVoucherSerialMapping mapserial = new InventoryDeliveryVoucherSerialMapping();
                                    mapserial.InventoryDeliveryVoucherSerialMappingId = Guid.NewGuid();
                                    mapserial.InventoryDeliveryVoucherMappingId = voucherMapping.InventoryDeliveryVoucherMappingId;
                                    mapserial.SerialId = itemSerial.SerialId;
                                    mapserial.Active = true;
                                    mapserial.CreatedDate = DateTime.Now;
                                    context.InventoryDeliveryVoucherSerialMapping.Add(mapserial);
                                });


                            }
                            context.InventoryDeliveryVoucherMapping.Add(voucherMapping);

                        }
                    });

                    context.SaveChanges();

                    return new CreateUpdateInventoryDeliveryVoucherResult
                    {
                        InventoryDeliveryVoucherId = parameter.inventoryDeliveryVoucher.InventoryDeliveryVoucherId,
                        Message = "Cập nhật thành công",
                        Status = true
                    };
                }
            }
            catch (Exception)
            {

                return new CreateUpdateInventoryDeliveryVoucherResult
                {
                    Message = "Có lỗi xảy ra",
                    Status = false
                };

            }
        }

        public GetInventoryDeliveryVoucherByIdResult GetInventoryDeliveryVoucherById(GetInventoryDeliveryVoucherByIdParameter parameter)
        {
            try
            {
                Employee defaultEmpty = new Employee();

                var InventoryDeliveryVoucherEntity = context.InventoryDeliveryVoucher.Where(wh => wh.InventoryDeliveryVoucherId == parameter.Id)
                                                     .Select(s => new InventoryDeliveryVoucherEntityModel
                                                     {
                                                         InventoryDeliveryVoucherId = s.InventoryDeliveryVoucherId,
                                                         InventoryDeliveryVoucherCode = s.InventoryDeliveryVoucherCode,
                                                         StatusId = s.StatusId,
                                                         InventoryDeliveryVoucherType = s.InventoryDeliveryVoucherType,
                                                         WarehouseId = s.WarehouseId,
                                                         ObjectId = s.ObjectId,
                                                         Receiver = s.Receiver,
                                                         Reason = s.Reason,
                                                         InventoryDeliveryVoucherDate = s.InventoryDeliveryVoucherDate,
                                                         InventoryDeliveryVoucherTime = s.InventoryDeliveryVoucherTime,
                                                         LicenseNumber = s.LicenseNumber,
                                                         Active = s.Active,
                                                         CreatedDate = s.CreatedDate,
                                                         CreatedById = s.CreatedById,
                                                         UpdatedById = s.UpdatedById,
                                                         UpdatedDate = s.UpdatedDate,
                                                     }).FirstOrDefault();
                if (InventoryDeliveryVoucherEntity.InventoryDeliveryVoucherType == 1)
                {
                    InventoryDeliveryVoucherEntity.NameObject = context.CustomerOrder.FirstOrDefault(f => f.OrderId == InventoryDeliveryVoucherEntity.ObjectId).OrderCode;
                }
                else if (InventoryDeliveryVoucherEntity.InventoryDeliveryVoucherType == 2)
                {
                    InventoryDeliveryVoucherEntity.NameObject = context.VendorOrder.FirstOrDefault(f => f.VendorOrderId == InventoryDeliveryVoucherEntity.ObjectId).VendorOrderCode;
                }
                else
                {
                    InventoryDeliveryVoucherEntity.NameObject = string.Empty;
                }
                var userCreate = context.User.FirstOrDefault(f => f.UserId == InventoryDeliveryVoucherEntity.CreatedById);
                var employeeDefault = (userCreate != null) ? context.Employee.Where(f => f.EmployeeId == userCreate.EmployeeId).DefaultIfEmpty(defaultEmpty).FirstOrDefault() : null;
                InventoryDeliveryVoucherEntity.NameCreate = (employeeDefault != null) ? employeeDefault.EmployeeCode + "-" + employeeDefault.EmployeeName : "";
                //nguoi xuat kho
                var userUpdate = context.User.FirstOrDefault(f => f.UserId == InventoryDeliveryVoucherEntity.UpdatedById);
                var employeeDefault2 = (userUpdate != null) ? context.Employee.Where(f => f.EmployeeId == userUpdate.EmployeeId).DefaultIfEmpty(defaultEmpty).FirstOrDefault() : null;
                InventoryDeliveryVoucherEntity.NameOutOfStock = (employeeDefault2 != null) ? employeeDefault2.EmployeeCode + "-" + employeeDefault2.EmployeeName : "";

                InventoryDeliveryVoucherEntity.NameStatus = context.Category.FirstOrDefault(f => f.CategoryId == InventoryDeliveryVoucherEntity.StatusId).CategoryName;
                ///

                var LstInventoryDeliveryVoucherMappingEntity = context.InventoryDeliveryVoucherMapping.Where(wh => wh.InventoryDeliveryVoucherId == parameter.Id).ToList();
                var categories = context.Category.Where(ct => ct.Active == true).ToList();
                var lstInventoryDeliveryVoucherMappingEntityId = LstInventoryDeliveryVoucherMappingEntity.Select(s => s.InventoryDeliveryVoucherMappingId).ToList();
                var lstinventoryDeliveryMappingEntityProductId = LstInventoryDeliveryVoucherMappingEntity.Select(s => s.ProductId).ToList();
                var product = context.Product.Where(p => p.Active == true && lstinventoryDeliveryMappingEntityProductId.Contains(p.ProductId)).ToList();
                var lstInventoryDeliveryVoucherSerialMapping = context.InventoryDeliveryVoucherSerialMapping.Where(wh => lstInventoryDeliveryVoucherMappingEntityId.Contains(wh.InventoryDeliveryVoucherMappingId)).ToList();
                List<InventoryDeliveryVoucherMappingEntityModel> lstInventoryDeliveryVoucherMappingEntityModel = new List<InventoryDeliveryVoucherMappingEntityModel>();
                LstInventoryDeliveryVoucherMappingEntity.ForEach(detail =>
                {
                    InventoryDeliveryVoucherMappingEntityModel obj = new InventoryDeliveryVoucherMappingEntityModel();


                    obj.InventoryDeliveryVoucherMappingId = detail.InventoryDeliveryVoucherMappingId;
                    obj.ProductId = detail.ProductId;
                    obj.ProductName = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductName;
                    obj.ProductCode = detail.ProductId == null ? "" : product.FirstOrDefault(p => p.ProductId == detail.ProductId).ProductCode;
                    obj.UnitId = detail.UnitId;
                    obj.UnitName = detail.UnitId == null ? "" : categories.FirstOrDefault(c => c.CategoryId == detail.UnitId).CategoryName;
                    obj.CurrencyUnit = detail.CurrencyUnit;
                    obj.ExchangeRate = detail.ExchangeRate;
                    obj.Vat = detail.Vat;
                    obj.DiscountType = detail.DiscountType;
                    obj.DiscountValue = detail.DiscountValue;
                    obj.SumAmount = SumAmount(detail.QuantityActual, detail.PriceProduct, detail.ExchangeRate, detail.Vat, detail.DiscountValue, detail.DiscountType);
                    obj.NameMoneyUnit = detail.CurrencyUnit != null ? categories.FirstOrDefault(c => c.CategoryId == detail.CurrencyUnit).CategoryName : "";
                    obj.QuantityRequire = detail.QuantityRequest;
                    obj.QuantityInventory = detail.QuantityInventory;
                    obj.Quantity = detail.QuantityActual;
                    obj.Note = detail.Description;
                    obj.TotalSerial = lstInventoryDeliveryVoucherSerialMapping.Where(wh => wh.InventoryDeliveryVoucherMappingId == detail.InventoryDeliveryVoucherMappingId).Count();
                    obj.Price = detail.PriceProduct;
                    obj.ListSerial = new List<Serial>();
                    obj.WarehouseId = detail.WarehouseId;
                    obj.WareHouseName = (detail.WarehouseId != Guid.Empty) ? context.Warehouse.FirstOrDefault(f => f.WarehouseId == detail.WarehouseId).WarehouseName : "";
                    lstInventoryDeliveryVoucherMappingEntityModel.Add(obj);
                });

                return new GetInventoryDeliveryVoucherByIdResult
                {
                    inventoryDeliveryVoucher = InventoryDeliveryVoucherEntity,
                    inventoryDeliveryVoucherMappingEntityModel = lstInventoryDeliveryVoucherMappingEntityModel,
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new GetInventoryDeliveryVoucherByIdResult
                {
                    Message = "Có lỗi xảy ra",
                    Status = false
                };
            }
        }

        public DeleteInventoryDeliveryVoucherResult DeleteInventoryDeliveryVoucher(DeleteInventoryDeliveryVoucherParameter parameter)
        {
            try
            {
                var InventoryDeliveryVoucherEntity = context.InventoryDeliveryVoucher.FirstOrDefault(f => f.InventoryDeliveryVoucherId == parameter.InventoryDeliveryVoucherId);
                var InventoryDeliveryVoucherMappingObject = context.InventoryDeliveryVoucherMapping.Where(wh => wh.InventoryDeliveryVoucherId == parameter.InventoryDeliveryVoucherId).ToList();

                var lstInventoryDeliveryVoucherMappingId = InventoryDeliveryVoucherMappingObject.Select(s => s.InventoryDeliveryVoucherMappingId).ToList().Distinct();
                var InventoryDeliveryVoucherSerialMappingObject = context.InventoryDeliveryVoucherSerialMapping
                                                                .Where(wh => lstInventoryDeliveryVoucherMappingId
                                                                .Contains(wh.InventoryDeliveryVoucherSerialMappingId)).ToList();
                //update lai Quantity trong inventoryReports
                //InventoryDeliveryVoucherSerialMappingObject.ForEach(item =>
                //{
                //    var inventoryReportByProduct = context.InventoryReport.FirstOrDefault(wh => wh.ProductId == item.ProductId && wh.WarehouseId == item.WarehouseId);
                //    if (inventoryReportByProduct != null)
                //    {
                //        inventoryReportByProduct.Quantity = inventoryReportByProduct.Quantity - item.QuantityActual;
                //        context.InventoryReport.Update(inventoryReportByProduct);
                //        context.SaveChanges();
                //    }

                //});

                context.InventoryDeliveryVoucherSerialMapping.RemoveRange(InventoryDeliveryVoucherSerialMappingObject);
                context.InventoryDeliveryVoucherMapping.RemoveRange(InventoryDeliveryVoucherMappingObject);
                context.InventoryDeliveryVoucher.Remove(InventoryDeliveryVoucherEntity);
                context.SaveChanges();
                return new DeleteInventoryDeliveryVoucherResult
                {
                    Message = "Đã xóa phiếu xuất kho",
                    Status = true
                };
            }
            catch (Exception)
            {
                return new DeleteInventoryDeliveryVoucherResult
                {
                    Message = "Có lỗi trong quá trình xóa phiếu xuất kho",
                    Status = false
                };

            }
        }

        public ChangeStatusInventoryDeliveryVoucherResult ChangeStatusInventoryDeliveryVoucher(ChangeStatusInventoryDeliveryVoucherParameter parameter)
        {
            try
            {
                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category.FirstOrDefault(ct =>
                    ct.CategoryCode == "NHK" && ct.CategoryTypeId == categoryTypeId && ct.Active == true).CategoryId;
                //var OrderStatusTypeId =
                //    context.OrderStatus.FirstOrDefault(ct => ct.OrderStatusCode == "DXK").OrderStatusId;

                var InventoryDeliveryVoucher = context.InventoryDeliveryVoucher.FirstOrDefault(f =>
                    f.InventoryDeliveryVoucherId == parameter.InventoryDeliveryVoucherId);
                InventoryDeliveryVoucher.StatusId = categoryIdNHK;
                InventoryDeliveryVoucher.InventoryDeliveryVoucherDate = DateTime.Now;
                InventoryDeliveryVoucher.UpdatedById = parameter.UserId;
                InventoryDeliveryVoucher.UpdatedDate = DateTime.Now;
                TimeSpan today = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                InventoryDeliveryVoucher.InventoryDeliveryVoucherTime = today;
                context.InventoryDeliveryVoucher.Update(InventoryDeliveryVoucher);
                context.SaveChanges();

                //update lai ton kho
                var inventoryDeliveryMappingVoucherEntity = context.InventoryDeliveryVoucherMapping
                    .Where(wh => wh.InventoryDeliveryVoucherId == InventoryDeliveryVoucher.InventoryDeliveryVoucherId)
                    .ToList();

                inventoryDeliveryMappingVoucherEntity.ForEach(voucherMapping =>
                {
                    var inventoryReportByProduct = context.InventoryReport.FirstOrDefault(wh =>
                        wh.ProductId == voucherMapping.ProductId && wh.WarehouseId == voucherMapping.WarehouseId);
                    if (inventoryReportByProduct == null)
                    {
                        //InventoryReport inventoryReport = new InventoryReport();
                        //inventoryReport.InventoryReportId = Guid.NewGuid();
                        //inventoryReport.WarehouseId = voucherMapping.WarehouseId;
                        //inventoryReport.ProductId = voucherMapping.ProductId;
                        //inventoryReport.Quantity =-voucherMapping.QuantityActual;
                        //inventoryReport.QuantityMinimum = 0;
                        //inventoryReport.Active = true;
                        //inventoryReport.CreatedDate = DateTime.Now;
                        //context.InventoryReport.Add(inventoryReport);
                    }
                    else
                    {
                        inventoryReportByProduct.Quantity -= voucherMapping.QuantityActual;
                        context.InventoryReport.Update(inventoryReportByProduct);
                    }
                });
                context.SaveChanges();

                //update lai status so serial
                var categoryTypeStatusSerialId = context.CategoryType
                    .FirstOrDefault(ct => ct.CategoryTypeCode == "TSE" && ct.Active == true).CategoryTypeId;
                var categoryIdDXU = context.Category.FirstOrDefault(ct =>
                        ct.CategoryCode == "DXU" && ct.CategoryTypeId == categoryTypeStatusSerialId &&
                        ct.Active == true)
                    .CategoryId;

                var lstinventoryDeliveryMappingVoucherId = inventoryDeliveryMappingVoucherEntity
                    .Select(s => s.InventoryDeliveryVoucherMappingId).ToList();
                var inventoryDeliveryVoucherSerialMappingEntity = context.InventoryDeliveryVoucherSerialMapping
                    .Where(wh => lstinventoryDeliveryMappingVoucherId.Contains(wh.InventoryDeliveryVoucherMappingId))
                    .Select(s => s.SerialId).ToList();
                var lstSerial = context.Serial
                    .Where(wh => inventoryDeliveryVoucherSerialMappingEntity.Contains(wh.SerialId)).ToList();

                lstSerial.ForEach(item =>
                {
                    item.StatusId = categoryIdDXU;
                    item.UpdatedDate = DateTime.Now;
                });
                context.Serial.UpdateRange(lstSerial);
                context.SaveChanges();

                //update lại status don hang
                if (InventoryDeliveryVoucher.InventoryDeliveryVoucherType == 1)
                {
                    var customerOrderEntity =
                        context.CustomerOrder.FirstOrDefault(f => f.OrderId == InventoryDeliveryVoucher.ObjectId);
                    if (customerOrderEntity != null)
                    {
                        if (checkCustomerOrderIsEnough(customerOrderEntity, categoryIdNHK))
                        {
                            #region Cập nhật trạng thái đơn hàng thành Đã xuất kho: Giang comment

                            //customerOrderEntity.StatusId = OrderStatusTypeId;
                            //context.CustomerOrder.Update(customerOrderEntity);
                            //context.SaveChanges();

                            #endregion
                        }
                    }
                }
                else if (InventoryDeliveryVoucher.InventoryDeliveryVoucherType == 2)
                {
                    var vendorOrderEntity =
                        context.VendorOrder.FirstOrDefault(f => f.VendorOrderId == InventoryDeliveryVoucher.ObjectId);
                    if (vendorOrderEntity != null)
                    {
                        if (checkVendorOrderIsEnough(vendorOrderEntity, categoryIdNHK))
                        {
                            #region Cập nhật trạng thái đơn hàng mua thành Đã xuât kho: Giang comment

                            //vendorOrderEntity.StatusId = OrderStatusTypeId;
                            //context.VendorOrder.Update(vendorOrderEntity);
                            //context.SaveChanges();

                            #endregion
                        }
                    }
                }

                return new ChangeStatusInventoryDeliveryVoucherResult
                {
                    Status = true,
                    Message = "Đã xuất kho thành công"
                };
            }
            catch (Exception)
            {
                return new ChangeStatusInventoryDeliveryVoucherResult
                {
                    Status = false,
                    Message = "Có lỗi khi tiến hành xuất kho"
                };
            }
        }

        public FilterCustomerInInventoryDeliveryVoucherResult FilterCustomerInInventoryDeliveryVoucher(FilterCustomerInInventoryDeliveryVoucherParameter parameter)
        {
            try
            {
                var listObjectId = context.InventoryDeliveryVoucher.Where(wh => wh.InventoryDeliveryVoucherType == 1).Select(s => s.ObjectId).Distinct().ToList();
                var ListCustomerId = context.CustomerOrder.Where(wh => listObjectId.Contains(wh.OrderId)).Select(s => s.CustomerId).Distinct().ToList();
                var listCustomer = context.Customer.Where(wh => ListCustomerId.Contains(wh.CustomerId))
                                                      .Select(c => new CustomerEntityModel
                                                      {
                                                          CustomerId = c.CustomerId,
                                                          CustomerCode = c.CustomerCode,
                                                          CustomerName = c.CustomerName,
                                                      }).OrderByDescending(date => date.CreatedDate).ToList();
                return new FilterCustomerInInventoryDeliveryVoucherResult
                {
                    LstCustomer = listCustomer,
                    Status = true
                };
            }
            catch (Exception)
            {
                return new FilterCustomerInInventoryDeliveryVoucherResult
                {
                    LstCustomer = new List<CustomerEntityModel>(),
                    Status = false
                };
            }
        }

        public SearchInventoryDeliveryVoucherResult SearchInventoryDeliveryVoucher(SearchInventoryDeliveryVoucherParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllContact = context.Contact.ToList();


                Serial emptyX = new Serial();
                InventoryDeliveryVoucherMapping emptyIRVM = new InventoryDeliveryVoucherMapping();
                InventoryDeliveryVoucherSerialMapping emptyIRSM = new InventoryDeliveryVoucherSerialMapping();
                VendorOrder emptyVOD = new VendorOrder();
                CustomerOrder emptyCOD = new CustomerOrder();
                Employee emptyEMP = new Employee();
                User emptyUser = new User();

                if (parameter.listCreateDate != null)
                {
                    if (parameter.listCreateDate.Count == 1)
                    {
                        parameter.listCreateDate.Add(parameter.listCreateDate[0]);
                    }
                }
                if (parameter.listInventoryReceivingDate != null)
                {
                    if (parameter.listInventoryReceivingDate.Count == 1)
                    {
                        parameter.listInventoryReceivingDate.Add(parameter.listInventoryReceivingDate[0]);
                    }
                }
                //check isManager
                var userCurrent = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (userCurrent == null)
                {
                    return new SearchInventoryDeliveryVoucherResult
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (userCurrent.EmployeeId == null || userCurrent.EmployeeId == Guid.Empty)
                {
                    return new SearchInventoryDeliveryVoucherResult
                    {
                        Status = false,
                        Message = "Lỗi dữ liệu"
                    };
                }

                var employeeId = userCurrent.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;
                /////////////////////
                var lstInventoryDeliveryVoucherEntity = context.InventoryDeliveryVoucher.Where(inventoryrv =>
                                                        (parameter.VoucherCode == null || parameter.VoucherCode == "" || inventoryrv.InventoryDeliveryVoucherCode.ToLower().Contains(parameter.VoucherCode.ToLower().Trim()))
                                                        && (parameter.listCreateDate == null || parameter.listCreateDate.Count == 0 || (parameter.listCreateDate[0].Date <= inventoryrv.CreatedDate.Date && parameter.listCreateDate[1].Date >= inventoryrv.CreatedDate.Date))
                                                        && (parameter.listInventoryReceivingDate == null || parameter.listInventoryReceivingDate.Count == 0 ||
                                                           (parameter.listInventoryReceivingDate[0].Date <= inventoryrv.InventoryDeliveryVoucherDate.Value.Date
                                                           && parameter.listInventoryReceivingDate[1].Date >= inventoryrv.InventoryDeliveryVoucherDate.Value.Date))
                                                        && (parameter.listStatusSelectedId == null || parameter.listStatusSelectedId.Count == 0 || parameter.listStatusSelectedId.Contains(inventoryrv.StatusId))
                                                        && (parameter.listCreateVoucherSelectedId == null || parameter.listCreateVoucherSelectedId.Count == 0 || parameter.listCreateVoucherSelectedId.Contains(inventoryrv.CreatedById))
                                                       ).ToList();

                var lstInventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping.Where(inventoryrvm =>
                                                            (parameter.listWarehouseSelectedId == null || parameter.listWarehouseSelectedId.Count == 0 || parameter.listWarehouseSelectedId.Contains(inventoryrvm.WarehouseId))
                                                            && (parameter.listProductSelectedId == null || parameter.listProductSelectedId.Count == 0 || parameter.listProductSelectedId.Contains(inventoryrvm.ProductId))
                                                        ).ToList();

                var lstInventoryDeliveryVoucherSerialMapping = context.InventoryDeliveryVoucherSerialMapping.ToList();
                var lstSerial = context.Serial.Where(wh =>
                                (parameter.serial == null || parameter.serial == "" || wh.SerialCode.Contains(parameter.serial.Trim()))
                                ).ToList();
                var lstUser = context.User.ToList();
                var lstCustomerOrder = context.CustomerOrder.Where(customerOrder =>
                                        (parameter.listCustomerSelectedId == null || parameter.listCustomerSelectedId.Count == 0 || parameter.listCustomerSelectedId.Contains(customerOrder.CustomerId))
                                        ).ToList();

                var listEmployee = context.Employee.Where(emp2 =>
                                   (parameter.listCreateVoucherSelectedId == null || parameter.listCreateVoucherSelectedId.Count == 0 || parameter.listCreateVoucherSelectedId.Contains(emp2.EmployeeId))
                                  ).ToList();

                var lstVendorOrder = context.VendorOrder.ToList();
                var lstCustomer = context.Customer.ToList();
                var lstVendor = context.Vendor.ToList();

                var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX" && ct.Active == true).CategoryTypeId;
                var lstCategory = context.Category.Where(ct => ct.CategoryTypeId == categoryTypeId && ct.Active == true).ToList();

                var lstInventoryDeliveryVoucherVoucher = new List<InventoryDeliveryVoucherEntityModel>();
                ////////////////////////////
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                    List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                    List<Guid> listUserByManagerId = new List<Guid>();

                    listEmployeeInChargeByManager.ForEach(item =>
                    {
                        if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                            listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                    });

                    //Lấy danh sách nhân viên UserId mà user phụ trách
                    listEmployeeInChargeByManagerId.ForEach(item =>
                    {
                        var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                        if (user_employee != null)
                            listUserByManagerId.Add(user_employee.UserId);
                    });

                    var lstInventoryDeliveryVoucherVoucherX = (from inventoryrv in lstInventoryDeliveryVoucherEntity
                                                               join inventoryrvm in lstInventoryDeliveryVoucherMapping on inventoryrv.InventoryDeliveryVoucherId equals inventoryrvm.InventoryDeliveryVoucherId into inventoryrvmLeft
                                                               from inventoryrvmX in inventoryrvmLeft.DefaultIfEmpty(emptyIRVM)
                                                               join inventoryrvsm in lstInventoryDeliveryVoucherSerialMapping on inventoryrvmX.InventoryDeliveryVoucherMappingId equals inventoryrvsm.InventoryDeliveryVoucherMappingId into inventoryrvsmLeft
                                                               from inventoryrvsmX in inventoryrvsmLeft.DefaultIfEmpty(emptyIRSM)
                                                               join serial in lstSerial on inventoryrvsmX.SerialId equals serial.SerialId into serialLeft
                                                               from serialX in serialLeft.DefaultIfEmpty(emptyX)
                                                                   //join vendorOrder in lstVendorOrder on inventoryrv.ObjectId equals vendorOrder.VendorOrderId into vendorOrderLeft
                                                                   //from vendorOrder in vendorOrderLeft.DefaultIfEmpty(emptyVOD)
                                                               join customerOrder in lstCustomerOrder on inventoryrv.ObjectId equals customerOrder.OrderId into customerOrderLeft
                                                               from customerOrder in customerOrderLeft.DefaultIfEmpty(emptyCOD)
                                                               join user in lstUser on inventoryrv.CreatedById equals user.UserId into userLeft
                                                               from user in userLeft.DefaultIfEmpty(emptyUser)
                                                               join emp2 in listEmployee on user.EmployeeId equals emp2.EmployeeId into emp2Left
                                                               from emp2 in emp2Left.DefaultIfEmpty(emptyEMP)
                                                               where listUserByManagerId.Contains(inventoryrv.CreatedById)
                                                               //(parameter.VoucherCode == null || parameter.VoucherCode == "" || inventoryrv.InventoryDeliveryVoucherCode.Contains(parameter.VoucherCode.Trim()))
                                                               //&& (parameter.listCreateDate == null || parameter.listCreateDate.Count == 0 || (parameter.listCreateDate[0].Date <= inventoryrv.CreatedDate.Date && parameter.listCreateDate[1].Date >= inventoryrv.CreatedDate.Date))
                                                               //&& (parameter.listInventoryReceivingDate == null || parameter.listInventoryReceivingDate.Count == 0 ||
                                                               //(parameter.listInventoryReceivingDate[0].Date <= inventoryrv.InventoryDeliveryVoucherDate.Value.Date
                                                               //&& parameter.listInventoryReceivingDate[1].Date >= inventoryrv.InventoryDeliveryVoucherDate.Value.Date))
                                                               //&& (parameter.listStatusSelectedId == null || parameter.listStatusSelectedId.Count == 0 || parameter.listStatusSelectedId.Contains(inventoryrv.StatusId))
                                                               //&& (parameter.listWarehouseSelectedId == null || parameter.listWarehouseSelectedId.Count == 0 || parameter.listWarehouseSelectedId.Contains(inventoryrvmX.WarehouseId))
                                                               //&& (parameter.listProductSelectedId == null || parameter.listProductSelectedId.Count == 0 || parameter.listProductSelectedId.Contains(inventoryrvmX.ProductId))
                                                               //&& (parameter.serial == null || parameter.serial == "" || serialX.SerialCode.Contains(parameter.serial.Trim()))
                                                               //&& (parameter.listCustomerSelectedId == null || parameter.listCustomerSelectedId.Count == 0 || parameter.listCustomerSelectedId.Contains(customerOrder.CustomerId))
                                                               //&& (parameter.listStorekeeperSelectedId == null || parameter.listStorekeeperSelectedId.Count == 0 || parameter.listStorekeeperSelectedId.Contains(emp.EmployeeId))
                                                               //&& (parameter.listCreateVoucherSelectedId == null || parameter.listCreateVoucherSelectedId.Count == 0 || parameter.listCreateVoucherSelectedId.Contains(emp2.EmployeeId))
                                                               //orderby inventoryrv.CreatedDate descending
                                                               select inventoryrv.InventoryDeliveryVoucherId).Distinct().ToList();

                    lstInventoryDeliveryVoucherVoucher = context.InventoryDeliveryVoucher.Where(wh => lstInventoryDeliveryVoucherVoucherX.Contains(wh.InventoryDeliveryVoucherId))
                                                 .Select(inventoryrv => new InventoryDeliveryVoucherEntityModel
                                                 {
                                                     InventoryDeliveryVoucherId = inventoryrv.InventoryDeliveryVoucherId,
                                                     InventoryDeliveryVoucherCode = inventoryrv.InventoryDeliveryVoucherCode,
                                                     StatusId = inventoryrv.StatusId,
                                                     ObjectId = inventoryrv.ObjectId,
                                                     InventoryDeliveryVoucherType = inventoryrv.InventoryDeliveryVoucherType,
                                                     WarehouseId = inventoryrv.WarehouseId,
                                                     InventoryDeliveryVoucherDate = inventoryrv.InventoryDeliveryVoucherDate,
                                                     InventoryDeliveryVoucherTime = inventoryrv.InventoryDeliveryVoucherTime,
                                                     LicenseNumber = inventoryrv.LicenseNumber,
                                                     Active = inventoryrv.Active,
                                                     CreatedDate = inventoryrv.CreatedDate,
                                                     CreatedById = inventoryrv.CreatedById,
                                                 }).OrderByDescending(or => or.CreatedDate).ToList();
                }
                else
                {
                    var lstInventoryDeliveryVoucherVoucherX = (from inventoryrv in lstInventoryDeliveryVoucherEntity
                                                               join inventoryrvm in lstInventoryDeliveryVoucherMapping on inventoryrv.InventoryDeliveryVoucherId equals inventoryrvm.InventoryDeliveryVoucherId into inventoryrvmLeft
                                                               from inventoryrvmX in inventoryrvmLeft.DefaultIfEmpty(emptyIRVM)
                                                               join inventoryrvsm in lstInventoryDeliveryVoucherSerialMapping on inventoryrvmX.InventoryDeliveryVoucherMappingId equals inventoryrvsm.InventoryDeliveryVoucherMappingId into inventoryrvsmLeft
                                                               from inventoryrvsmX in inventoryrvsmLeft.DefaultIfEmpty(emptyIRSM)
                                                               join serial in lstSerial on inventoryrvsmX.SerialId equals serial.SerialId into serialLeft
                                                               from serialX in serialLeft.DefaultIfEmpty(emptyX)
                                                                   //join vendorOrder in lstVendorOrder on inventoryrv.ObjectId equals vendorOrder.VendorOrderId into vendorOrderLeft
                                                                   //from vendorOrder in vendorOrderLeft.DefaultIfEmpty(emptyVOD)
                                                               join customerOrder in lstCustomerOrder on inventoryrv.ObjectId equals customerOrder.OrderId into customerOrderLeft
                                                               from customerOrder in customerOrderLeft.DefaultIfEmpty(emptyCOD)
                                                               join user in lstUser on inventoryrv.CreatedById equals user.UserId into userLeft
                                                               from user in userLeft.DefaultIfEmpty(emptyUser)
                                                               join emp2 in listEmployee on user.EmployeeId equals emp2.EmployeeId into emp2Left
                                                               from emp2 in emp2Left.DefaultIfEmpty(emptyEMP)
                                                               where inventoryrv.CreatedById == parameter.UserId
                                                               //(parameter.VoucherCode == null || parameter.VoucherCode == "" || inventoryrv.InventoryDeliveryVoucherCode.Contains(parameter.VoucherCode.Trim()))
                                                               //&& (parameter.listCreateDate == null || parameter.listCreateDate.Count == 0 || (parameter.listCreateDate[0].Date <= inventoryrv.CreatedDate.Date && parameter.listCreateDate[1].Date >= inventoryrv.CreatedDate.Date))
                                                               //&& (parameter.listInventoryReceivingDate == null || parameter.listInventoryReceivingDate.Count == 0 ||
                                                               //(parameter.listInventoryReceivingDate[0].Date <= inventoryrv.InventoryDeliveryVoucherDate.Value.Date
                                                               //&& parameter.listInventoryReceivingDate[1].Date >= inventoryrv.InventoryDeliveryVoucherDate.Value.Date))
                                                               //&& (parameter.listStatusSelectedId == null || parameter.listStatusSelectedId.Count == 0 || parameter.listStatusSelectedId.Contains(inventoryrv.StatusId))
                                                               //&& (parameter.listWarehouseSelectedId == null || parameter.listWarehouseSelectedId.Count == 0 || parameter.listWarehouseSelectedId.Contains(inventoryrvmX.WarehouseId))
                                                               //&& (parameter.listProductSelectedId == null || parameter.listProductSelectedId.Count == 0 || parameter.listProductSelectedId.Contains(inventoryrvmX.ProductId))
                                                               //&& (parameter.serial == null || parameter.serial == "" || serialX.SerialCode.Contains(parameter.serial.Trim()))
                                                               //&& (parameter.listCustomerSelectedId == null || parameter.listCustomerSelectedId.Count == 0 || parameter.listCustomerSelectedId.Contains(customerOrder.CustomerId))
                                                               //&& (parameter.listStorekeeperSelectedId == null || parameter.listStorekeeperSelectedId.Count == 0 || parameter.listStorekeeperSelectedId.Contains(emp.EmployeeId))
                                                               //&& (parameter.listCreateVoucherSelectedId == null || parameter.listCreateVoucherSelectedId.Count == 0 || parameter.listCreateVoucherSelectedId.Contains(emp2.EmployeeId))
                                                               //orderby inventoryrv.CreatedDate descending
                                                               select inventoryrv.InventoryDeliveryVoucherId).Distinct().ToList();

                    lstInventoryDeliveryVoucherVoucher = context.InventoryDeliveryVoucher.Where(wh => lstInventoryDeliveryVoucherVoucherX.Contains(wh.InventoryDeliveryVoucherId))
                             .Select(inventoryrv => new InventoryDeliveryVoucherEntityModel
                             {
                                 InventoryDeliveryVoucherId = inventoryrv.InventoryDeliveryVoucherId,
                                 InventoryDeliveryVoucherCode = inventoryrv.InventoryDeliveryVoucherCode,
                                 StatusId = inventoryrv.StatusId,
                                 ObjectId = inventoryrv.ObjectId,
                                 InventoryDeliveryVoucherType = inventoryrv.InventoryDeliveryVoucherType,
                                 WarehouseId = inventoryrv.WarehouseId,
                                 InventoryDeliveryVoucherDate = inventoryrv.InventoryDeliveryVoucherDate,
                                 InventoryDeliveryVoucherTime = inventoryrv.InventoryDeliveryVoucherTime,
                                 LicenseNumber = inventoryrv.LicenseNumber,
                                 Active = inventoryrv.Active,
                                 CreatedDate = inventoryrv.CreatedDate,
                                 CreatedById = inventoryrv.CreatedById,
                             }).OrderByDescending(or => or.CreatedDate).ToList();

                }


                List<InventoryDeliveryVoucherEntityModel> lstReuslt = new List<InventoryDeliveryVoucherEntityModel>();
                lstInventoryDeliveryVoucherVoucher.ForEach(item =>
                {


                    Employee defaultEmpty = new Employee();

                    //InventoryDeliveryVoucherEntityModel itemModel = new InventoryDeliveryVoucherEntityModel();
                    if (item.InventoryDeliveryVoucherType == 1)
                    {
                        var CustomerOrderEntity = lstCustomerOrder.FirstOrDefault(f => f.OrderId == item.ObjectId);
                        var CustomerEntity = lstCustomer.FirstOrDefault(f => f.CustomerId == CustomerOrderEntity.CustomerId);
                        item.NameObject = CustomerOrderEntity.OrderCode;
                        item.VendorId = CustomerOrderEntity.CustomerId;
                        item.VedorName = CustomerEntity.CustomerName;
                    }
                    else if (item.InventoryDeliveryVoucherType == 2)
                    {
                        var VendorOrderEntity = lstVendorOrder.FirstOrDefault(f => f.VendorOrderId == item.ObjectId);
                        var VendorEntity = lstVendor.FirstOrDefault(f => f.VendorId == VendorOrderEntity.VendorId);
                        item.NameObject = VendorOrderEntity.VendorOrderCode;
                        item.VendorId = VendorOrderEntity.VendorId;
                        item.VedorName = VendorEntity.VendorName;

                    }
                    //itemModel.InventoryDeliveryVoucherId = item.InventoryDeliveryVoucherId;
                    //itemModel.InventoryDeliveryVoucherCode = item.InventoryDeliveryVoucherCode;
                    //itemModel.StatusId = item.StatusId;
                    //itemModel.ObjectId = item.ObjectId;
                    //itemModel.InventoryDeliveryVoucherType = item.InventoryDeliveryVoucherType;
                    //itemModel.WarehouseId = item.WarehouseId;
                    //itemModel.InventoryDeliveryVoucherDate = item.InventoryDeliveryVoucherDate;
                    //itemModel.InventoryDeliveryVoucherTime = item.InventoryDeliveryVoucherTime;
                    //itemModel.LicenseNumber = item.LicenseNumber;
                    var userCreate = context.User.FirstOrDefault(f => f.UserId == item.CreatedById);
                    item.NameCreate = (userCreate != null) ? listEmployee.Where(f => f.EmployeeId == userCreate.EmployeeId).DefaultIfEmpty(defaultEmpty).FirstOrDefault().EmployeeName : "";
                    item.NameStatus = lstCategory.FirstOrDefault(f => f.CategoryId == item.StatusId).CategoryName;
                    //itemModel.Active = item.Active;
                    //itemModel.CreatedDate = item.CreatedDate;
                    //itemModel.CreatedById = item.CreatedById;
                    lstReuslt.Add(item);

                });
                return new SearchInventoryDeliveryVoucherResult
                {
                    lstResult = lstReuslt,
                    Message = "Đã lọc xong danh sách phiếu nhập",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new SearchInventoryDeliveryVoucherResult
                {
                    Message = "Có lỗi xảy ra trong quá trình lọc",
                    Status = false
                };
            }


        }

        public FilterProductResult FilterProduct(FilterProductParameter parameter)
        {
            try
            {
                var commonProductCategory = context.ProductCategory.ToList();
                var commonProduct = context.Product.ToList();
                var commonProductVendorMapping = context.ProductVendorMapping.ToList();

                var vendorOrderDetails = context.VendorOrderDetail.ToList();
                //var vendorOrderProductDetailProductAttributeValues = context.VendorOrderProductDetailProductAttributeValue.ToList();
                var customerOrderDetails = context.CustomerOrderDetail.ToList();
                //var orderProductDetailProductAttributeValues = context.OrderProductDetailProductAttributeValue.ToList();
                var quoteDetails = context.QuoteDetail.ToList();
                //var quoteProductDetailProductAttributeValues = context.QuoteProductDetailProductAttributeValue.ToList();
                var procurementRequestItems = context.ProcurementRequestItem.ToList();
                //var productAttributes = context.ProductAttribute.ToList();
                var productVendorMappings = context.ProductVendorMapping.ToList();


                if (parameter.ListProductCategory.Count > 0)
                {
                    List<Guid> listGuidTemp = parameter.ListProductCategory;
                    for (int i = 0; i < listGuidTemp.Count; ++i)
                    {
                        ListChildProductCategory(listGuidTemp[i], parameter.ListProductCategory, commonProductCategory);
                    }
                }
                var productList = (from itemproduct in commonProduct
                                   join itemProductCategory in commonProductCategory on itemproduct.ProductCategoryId equals itemProductCategory.ProductCategoryId
                                   join itemproductvendormap in commonProductVendorMapping on itemproduct.ProductId equals itemproductvendormap.ProductId
                                   where (itemproduct.Active == true) &&
                                   (parameter.ListProductCategory.Count == 0 || parameter.ListProductCategory.Contains(itemproduct.ProductCategoryId)) &&
                                   (parameter.ListProductId.Count == 0 || parameter.ListProductId.Contains(itemproduct.ProductId))
                                   select new ProductEntityModel
                                   {
                                       ProductId = itemproduct.ProductId,
                                       ProductCategoryId = itemproduct.ProductCategoryId,
                                       ProductName = itemproduct.ProductName,
                                       ProductCode = itemproduct.ProductCode,
                                       ProductDescription = itemproduct.ProductDescription,
                                       ProductUnitId = itemproduct.ProductUnitId,
                                       Quantity = itemproduct.Quantity,
                                       Price1 = itemproduct.Price1,
                                       Price2 = itemproduct.Price2,
                                       Active = itemproduct.Active,
                                       ProductMoneyUnitId = itemproduct.ProductMoneyUnitId,
                                       CreatedById = itemproduct.CreatedById,
                                       CreatedDate = itemproduct.CreatedDate,
                                       UpdatedById = itemproduct.UpdatedById,
                                       UpdatedDate = itemproduct.UpdatedDate,
                                       ProductCategoryName = itemProductCategory.ProductCategoryName,
                                       MinimumInventoryQuantity = itemproduct.MinimumInventoryQuantity,
                                       GuaranteeTime = itemproduct.GuaranteeTime,
                                   }
                                ).ToList();
                var resultGroup = productList.GroupBy(x => x.ProductId).Select(y => y.First()).ToList();
                return new FilterProductResult
                {
                    Status = true,
                    ProductList = resultGroup
                };

            }
            catch (Exception e)
            {
                return new FilterProductResult
                {
                    Status = false,
                    ProductList = new List<ProductEntityModel>()
                };

            }
        }

        public GetProductNameAndProductCodeResult GetProductNameAndProductCode(GetProductNameAndProductCodeParameter parameter)
        {
            try
            {
                var productList = context.Product.Where(wh =>
                   parameter.Query == null || parameter.Query == string.Empty
                   || wh.ProductCode.Contains(parameter.Query) || wh.ProductName.Contains(parameter.Query)
                ).Select(s => new ProductEntityModel
                {
                    ProductId = s.ProductId,
                    ProductName = string.Format("{0}-{1}", s.ProductCode.Trim(), s.ProductName.Trim())
                }).ToList();
                return new GetProductNameAndProductCodeResult
                {
                    Status = true,
                    ProductList = productList
                };

            }
            catch (Exception e)
            {
                return new GetProductNameAndProductCodeResult
                {
                    Status = false,
                    ProductList = new List<ProductEntityModel>()
                };
            }
        }

        public GetVendorInvenoryReceivingResult GetVendorInvenoryReceiving(GetVendorInvenoryReceivingParameter parameter)
        {
            try
            {
                var commonVendor = context.Vendor.Where(v => v.Active == true).ToList();

                var listIRVID = context.InventoryReceivingVoucher.Where(wh => wh.InventoryReceivingVoucherType == 1).Select(s => s.InventoryReceivingVoucherId).ToList();
                var listObjectId = context.InventoryReceivingVoucherMapping.Where(wh => listIRVID.Contains(wh.InventoryReceivingVoucherId)).Select(s => s.ObjectId).ToList();
                var listVendorOrderId = context.VendorOrderDetail.Where(wh => listObjectId.Contains(wh.VendorOrderDetailId)).Select(s => s.VendorOrderId).Distinct().ToList();
                var listVendorId = context.VendorOrder.Where(wh => listVendorOrderId.Contains(wh.VendorOrderId)).Select(s => s.VendorId).Distinct().ToList();
                var vendorList = commonVendor.Where(wh => listVendorId.Contains(wh.VendorId))
                .Select(v => new VendorEntityModel
                {
                    VendorId = v.VendorId,
                    VendorName = string.Format("{0}-{1}", v.VendorCode, v.VendorName),
                    VendorCode = v.VendorCode,
                }).OrderByDescending(v => v.CreatedDate).ToList();

                return new GetVendorInvenoryReceivingResult
                {
                    Status = true,
                    VendorList = vendorList
                };
            }
            catch (Exception e)
            {
                return new GetVendorInvenoryReceivingResult
                {
                    Status = false,
                    VendorList = new List<VendorEntityModel>()
                };
            }
        }

        public GetCustomerDeliveryResult GetCustomerDelivery(GetCustomerDeliveryParameter parameter)
        {
            try
            {
                var commonCustomer = context.Customer.Where(v => v.Active == true).ToList();

                var listCustomerOrder = context.InventoryDeliveryVoucher.Where(wh => wh.InventoryDeliveryVoucherType == 1).Select(s => s.ObjectId).Distinct().ToList();
                var listCustomerId = context.CustomerOrder.Where(wh => listCustomerOrder.Contains(wh.OrderId)).Select(s => s.CustomerId).Distinct().ToList();
                var customerList = commonCustomer.Where(wh => listCustomerId.Contains(wh.CustomerId))
                                    .Select(v => new CustomerEntityModel
                                    {
                                        CustomerId = v.CustomerId,
                                        CustomerName = string.Format("{0}-{1}", v.CustomerCode, v.CustomerName),
                                        CustomerCode = v.CustomerCode,
                                    }).OrderByDescending(v => v.CreatedDate).ToList();
                return new GetCustomerDeliveryResult
                {
                    Status = true,
                    LstCustomer = customerList
                };

            }
            catch (Exception e)
            {
                return new GetCustomerDeliveryResult
                {
                    Status = false,
                    LstCustomer = new List<CustomerEntityModel>()
                };
            }
        }
        public void ListChildProductCategory(Guid ProductCategoryID, List<Guid> listResult, List<ProductCategory> commonProductCategory)
        {
            var listProductCategoryChil = commonProductCategory.Where(item => item.ParentId == ProductCategoryID).ToList();
            if (listProductCategoryChil.Count > 0)
            {
                for (int i = 0; i < listProductCategoryChil.Count; ++i)
                {
                    listResult.Add(listProductCategoryChil[i].ProductCategoryId);
                    ListChildProductCategory(listProductCategoryChil[i].ProductCategoryId, listResult, commonProductCategory);
                }
            }
        }
        private List<Guid?> getOrganizationChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Organization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(item.OrganizationId, list);
            });

            return list;
        }
        //kiem tra da nhap kho het chua tai 1 customerOrder
        private bool checkCustomerOrderIsEnough(CustomerOrder CustomerOrder, Guid statusId)
        {
            bool result = true;
            var customerOrderDetails =
                context.CustomerOrderDetail.Where(wh => wh.OrderId == CustomerOrder.OrderId).ToList();
            var InventoryDeliveryVoucherEntity = context.InventoryDeliveryVoucher
                .Where(wh => wh.ObjectId == CustomerOrder.OrderId && wh.StatusId == statusId).ToList();
            var lstInventoryDeliveryVoucherId =
                InventoryDeliveryVoucherEntity.Select(s => s.InventoryDeliveryVoucherId).ToList();
            if (InventoryDeliveryVoucherEntity.Count > 0)
            {
                var lstInventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping
                    .Where(wh => lstInventoryDeliveryVoucherId.Contains(wh.InventoryDeliveryVoucherId)).ToList();

                customerOrderDetails.ForEach(item =>
                {
                    if (item.ProductId != null)
                    {
                        decimal quantityInventoryReceiving = lstInventoryDeliveryVoucherMapping.Where(i =>
                                i.ProductId == item.ProductId
                                && lstInventoryDeliveryVoucherId.Contains(i.InventoryDeliveryVoucherId))
                            .Sum(i => i.QuantityActual);
                        if (quantityInventoryReceiving < item.Quantity)
                        {
                            result = false;
                        }
                    }
                });
            }
            else
            {
                result = false;
            }
            return result;
        }
        private bool checkVendorOrderIsEnough(VendorOrder vendorOrder, Guid statusId)
        {
            bool result = true;
            var vendorOrderDetails = context.VendorOrderDetail.Where(wh => wh.VendorOrderId == vendorOrder.VendorOrderId).ToList();
            var InventoryDeliveryVoucherEntity = context.InventoryDeliveryVoucher.Where(wh => wh.ObjectId == vendorOrder.VendorOrderId && wh.StatusId == statusId).ToList();
            var lstInventoryDeliveryVoucherId = InventoryDeliveryVoucherEntity.Select(s => s.InventoryDeliveryVoucherId).ToList();
            if (InventoryDeliveryVoucherEntity.Count > 0)
            {
                var lstInventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping
                                                        .Where(wh => lstInventoryDeliveryVoucherId.Contains(wh.InventoryDeliveryVoucherId)).ToList();

                vendorOrderDetails.ForEach(item =>
                {
                    if (item.ProductId != null)
                    {
                        decimal quantityInventoryReceiving = lstInventoryDeliveryVoucherMapping.Where(i => i.ProductId == item.ProductId && lstInventoryDeliveryVoucherId.Contains(i.InventoryDeliveryVoucherId)).Sum(i => i.QuantityActual);
                        if (quantityInventoryReceiving < item.Quantity)
                        {
                            result = false;
                            return;
                        }
                    }
                });
            }
            else
            {
                result = false;
            }
            return result;
        }

        public InStockReportResult InStockReport(InStockReportParameter parameter)
        {
            try
            {
                var categoryTypeId = context.CategoryType
                    .FirstOrDefault(ct => ct.CategoryTypeCode == "DNH" && ct.Active == true).CategoryTypeId;
                var categoryIdNHK = context.Category
                    .Where(ct => ct.CategoryTypeId == categoryTypeId && ct.Active == true).ToList();

                //
                var lstInventoryReport = context.InventoryReport.Where(wh =>
                    (parameter.lstProduct == null || parameter.lstProduct.Count == 0 ||
                     parameter.lstProduct.Contains(wh.ProductId))
                    && (parameter.lstWarehouse == null || parameter.lstWarehouse.Count == 0 ||
                        parameter.lstWarehouse.Contains(wh.WarehouseId))
                    && ((parameter.FromQuantity == null || parameter.FromQuantity <= wh.Quantity) &&
                        (parameter.FromQuantity == null || parameter.ToQuantity >= wh.Quantity))
                ).ToList();

                var lstProductId = lstInventoryReport.Select(s => s.ProductId).Distinct().ToList();
                var lstWareHouseId = lstInventoryReport.Select(s => s.WarehouseId).Distinct().ToList();

                var lstProduct = context.Product.Where(wh => lstProductId.Contains(wh.ProductId)).ToList();
                var lstWarehouse = context.Warehouse.Where(wh => lstWareHouseId.Contains(wh.WarehouseId)).ToList();
                var lstProductCategory = context.ProductCategory.ToList();

                var lstResult = (from inventoryR in lstInventoryReport
                                 join product in lstProduct on inventoryR.ProductId equals product.ProductId
                                 join categoryUnint in categoryIdNHK on product.ProductUnitId equals categoryUnint.CategoryId
                                 join warehouse in lstWarehouse on inventoryR.WarehouseId equals warehouse.WarehouseId
                                 join productCategory in lstProductCategory on product.ProductCategoryId equals productCategory
                                     .ProductCategoryId
                                 select new InStockEntityModel
                                 {
                                     ProductId = inventoryR.ProductId,
                                     ProductCode = product.ProductCode,
                                     ProductName = product.ProductName,
                                     ProductGroup = productCategory.ProductCategoryName,
                                     ProductUnitName = categoryUnint.CategoryName,
                                     QuantityInStock = inventoryR.Quantity,
                                     WareHouseId = warehouse.WarehouseId,
                                     WareHouseName = warehouse.WarehouseName,
                                     ProductPrice = product.Price1,
                                     lstSerial = new List<Serial>()
                                 }).ToList();

                //Lấy danh sách Serial
                var categoryTypeStatusSerialId = context.CategoryType
                    .FirstOrDefault(ct => ct.CategoryTypeCode == "TSE" && ct.Active == true).CategoryTypeId;
                var categoryIdDXU = context.Category.FirstOrDefault(ct =>
                        ct.CategoryCode == "CXU" && ct.CategoryTypeId == categoryTypeStatusSerialId &&
                        ct.Active == true)
                    .CategoryId;

                var lstInventoryReceivingVoucherSerialMappingSerialId = context.InventoryReceivingVoucherSerialMapping
                    .Select(s => s.SerialId).ToList();
                var lstSerialReceivingVoucher = context.Serial.Where(wh =>
                    lstInventoryReceivingVoucherSerialMappingSerialId.Contains(wh.SerialId)
                    && wh.StatusId == categoryIdDXU
                    && (parameter.SerialCode == null || wh.SerialCode.Contains(parameter.SerialCode.Trim()))
                ).ToList();

                var listRmove = new List<InStockEntityModel>();
                lstResult.ForEach(item =>
                {
                    var lstResultSerial = lstSerialReceivingVoucher
                        .Where(wh => (item.ProductId == null || wh.ProductId == item.ProductId)
                                     && (item.WareHouseId == null || wh.WarehouseId == item.WareHouseId)).ToList();
                    if (parameter.SerialCode != null && parameter.SerialCode != "" && parameter.SerialCode != string.Empty)
                    {
                        if (lstResultSerial.Count == 0)
                        {
                            listRmove.Add(item);
                        }
                        else
                        {
                            item.lstSerial = lstResultSerial;
                        }
                    }
                    else
                    {
                        item.lstSerial = lstResultSerial;
                    }
                });

                if (listRmove.Count > 0)
                {
                    listRmove.ForEach(item =>
                    {
                        lstResult.Remove(item);
                    });
                }

                return new InStockReportResult
                {
                    lstResult = lstResult,
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new InStockReportResult
                {
                    Message = "Có lỗi xảy ra",
                    Status = false
                };
            }
        }

        public CreateUpdateWarehouseMasterdataResult CreateUpdateWarehouseMasterdata(CreateUpdateWarehouseMasterdataParameter parameter)
        {
            try
            {
                #region Get Employee Care Staff List
                var portalUserCode = "PortalUser"; //loại portalUser
                var ListEmployeeEntityModel = new List<DataAccess.Models.Employee.EmployeeEntityModel>();
                var listEmployeeEntity = context.Employee.Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user
                var userById = context.User.FirstOrDefault(f => f.UserId == parameter.UserId);
                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == userById.EmployeeId).FirstOrDefault();
                //check Is Manager
                var isManage = employeeById.IsManager;
                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employeeById.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                    getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                    var listEmployeeFiltered = listEmployeeEntity.Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
                    {
                        EmployeeId = w.EmployeeId,
                        EmployeeName = w.EmployeeName,
                        EmployeeCode = w.EmployeeCode,
                    }).ToList();

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == userById.EmployeeId).FirstOrDefault();
                    ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode
                    });
                }
                #endregion

                #region Get List Warehouse Code
                var ListWarehouseCode = new List<string>();
                var warehouseEntity = context.Warehouse.Where(w => w.Active == true).Select(w => w.WarehouseCode).ToList();
                warehouseEntity?.ForEach(warehouse =>
                {
                    ListWarehouseCode.Add(warehouse);
                });
                #endregion

                #region Get Warehouse by Id
                var WarehouseEntityModel = new DataAccess.Databases.Entities.Warehouse();
                if (parameter.WarehouseId != null)
                {
                    var warehouseById = context.Warehouse.Where(w => w.WarehouseId == parameter.WarehouseId).FirstOrDefault();
                    if (warehouseById != null)
                    {
                        WarehouseEntityModel.WarehouseId = warehouseById.WarehouseId;
                        WarehouseEntityModel.WarehouseCode = warehouseById.WarehouseCode;
                        WarehouseEntityModel.WarehouseName = warehouseById.WarehouseName;
                        WarehouseEntityModel.WarehouseParent = warehouseById.WarehouseParent;
                        WarehouseEntityModel.WarehouseAddress = warehouseById.WarehouseAddress;
                        WarehouseEntityModel.Storagekeeper = warehouseById.Storagekeeper;
                        WarehouseEntityModel.WarehouseDescription = warehouseById.WarehouseDescription;
                    }
                }
                #endregion

                return new CreateUpdateWarehouseMasterdataResult
                {
                    WarehouseEntityModel = WarehouseEntityModel,
                    ListEmployeeEntityModel = ListEmployeeEntityModel,
                    ListWarehouseCode = ListWarehouseCode,
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new CreateUpdateWarehouseMasterdataResult
                {
                    Message = e.ToString(),
                    Status = false
                };
            }
        }

        public GetMasterDataSearchInStockReportResult GetMasterDataSearchInStockReport(
            GetMasterDataSearchInStockReportParameter parameter)
        {
            try
            {
                var listAllProductCategory = context.ProductCategory.Where(x => x.Active == true).Select(y =>
                    new ProductCategoryEntityModel
                    {
                        ProductCategoryId = y.ProductCategoryId,
                        ParentId = y.ParentId,
                        ProductCategoryCode = y.ProductCategoryCode,
                        ProductCategoryName = y.ProductCategoryName,
                        ProductCategoryCodeName = y.ProductCategoryCode.Trim() + " - " + y.ProductCategoryName.Trim(),
                        ProductCategoryLevel = y.ProductCategoryLevel
                    }).OrderBy(z => z.ProductCategoryName).ToList();

                var listWareHouse = new List<WareHouseEntityModel>();
                listWareHouse = context.Warehouse.Where(x => x.Active).Select(y =>
                    new WareHouseEntityModel
                    {
                        WarehouseId = y.WarehouseId,
                        WarehouseCode = y.WarehouseCode,
                        WarehouseName = y.WarehouseName,
                        WarehouseCodeName = y.WarehouseCode.Trim() + " - " + y.WarehouseName.Trim()
                    }).OrderBy(z => z.WarehouseName).ToList();

                return new GetMasterDataSearchInStockReportResult()
                {
                    Status = true,
                    Message = "Success",
                    ListProductCategory = listAllProductCategory,
                    ListWareHouse = listWareHouse
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchInStockReportResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchInStockReportResult SearchInStockReport(SearchInStockReportParameter parameter)
        {
            try
            {
                var listResult = new List<InStockEntityModel>();
                var listAllInventoryReport = context.InventoryReport.Where(x => x.Active).ToList();
                var listAllWarehouse = context.Warehouse.Where(x => x.Active).ToList();
                var listAllProductCategory = context.ProductCategory.Where(x => x.Active == true).ToList();
                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();

                #region Lấy list Đơn vị tính của sản phẩm

                var statusTypeDVT = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH")
                    ?.CategoryTypeId;
                var listDVT = context.Category.Where(x => x.Active == true && x.CategoryTypeId == statusTypeDVT)
                    .ToList();

                #endregion

                var currentWarehouse = listAllWarehouse.FirstOrDefault(x => x.WarehouseId == parameter.WarehouseId);

                if (currentWarehouse != null)
                {
                    //Kiểm tra xem kho là kho cha hay không?
                    var hasParent =
                        listAllWarehouse.FirstOrDefault(x => x.WarehouseParent == currentWarehouse.WarehouseId);

                    //Nếu không là kho cha thì
                    if (hasParent == null)
                    {
                        var listWarehouseId = new List<Guid>();
                        listWarehouseId.Add(currentWarehouse.WarehouseId);

                        listResult = GetListResult(listWarehouseId, parameter.FromDate, listAllProduct,
                            parameter.ProductCategoryId,
                            parameter.ProductNameCode, listDVT, listAllInventoryReport);
                    }
                    //Nếu là kho cha thì
                    else
                    {
                        #region Lấy tất cả kho con cấp cuối cùng của nó

                        var listWarehouseId = GetListWarehouseChild(listAllWarehouse, currentWarehouse.WarehouseId,
                            new List<Guid>());

                        listResult = GetListResult(listWarehouseId, parameter.FromDate, listAllProduct,
                            parameter.ProductCategoryId,
                            parameter.ProductNameCode, listDVT, listAllInventoryReport);

                        #endregion
                    }
                }

                return new SearchInStockReportResult()
                {
                    Status = true,
                    Message = "Success",
                    ListResult = listResult
                };
            }
            catch (Exception e)
            {
                return new SearchInStockReportResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataPhieuNhapKhoResult GetMasterDataPhieuNhapKho(GetMasterDataPhieuNhapKhoParameter parameter)
        {
            try
            {
                var listVendor = new List<VendorEntityModel>();
                var listWarehouse = new List<WareHouseEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employeeCodeName = context.Employee.Where(x => x.EmployeeId == user.EmployeeId).Select(y =>
                    new EmployeeEntityModel
                    {
                        EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                    }).FirstOrDefault()?.EmployeeCodeName;

                listVendor = context.Vendor.Where(x => x.Active == true).Select(y => new VendorEntityModel
                {
                    VendorId = y.VendorId,
                    VendorCode = y.VendorCode,
                    VendorName = y.VendorName,
                    VendorCodeName = y.VendorCode.Trim() + " - " + y.VendorName.Trim()
                }).OrderBy(z => z.VendorName).ToList();

                listWarehouse = context.Warehouse.Where(x => x.Active && x.WarehouseParent == null).Select(y =>
                    new WareHouseEntityModel
                    {
                        WarehouseId = y.WarehouseId,
                        WarehouseParent = y.WarehouseParent,
                        HasChild = false,
                        WarehouseCode = y.WarehouseCode,
                        WarehouseName = y.WarehouseName,
                        WarehouseCodeName = y.WarehouseCode.Trim() + " - " + y.WarehouseName.Trim()
                    }).OrderBy(z => z.WarehouseName).ToList();

                var listAllWarehouse = context.Warehouse.Where(x => x.Active).ToList();

                listWarehouse.ForEach(item =>
                {
                    var hasChild = listAllWarehouse.FirstOrDefault(x => x.WarehouseParent == item.WarehouseId);

                    //Nếu Kho có kho con
                    if (hasChild != null)
                    {
                        item.HasChild = true;
                    }
                    //Nếu kho không có kho con
                    else
                    {
                        item.HasChild = false;
                    }
                });

                //Lấy list khách hàng có trạng thái Định danh
                var statusCustomerType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                var statusCustomerDd = context.Category.FirstOrDefault(x =>
                    x.CategoryTypeId == statusCustomerType.CategoryTypeId && x.CategoryCode == "HDO");
                listCustomer = context.Customer
                    .Where(x => x.Active == true && x.StatusId == statusCustomerDd.CategoryId).Select(y =>
                        new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName.Trim()
                        }).OrderBy(z => z.CustomerName).ToList();

                return new GetMasterDataPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    ListVendor = listVendor,
                    ListWarehouse = listWarehouse,
                    ListCustomer = listCustomer,
                    EmployeeCodeName = employeeCodeName
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDanhSachSanPhamCuaPhieuResult GetDanhSachSanPhamCuaPhieu(GetDanhSachSanPhamCuaPhieuParameter parameter)
        {
            try
            {
                var listItemDetail = new List<SanPhamPhieuNhapKhoModel>();

                //Lấy Id các trạng thái đơn hàng: Đơn hàng mua
                var listStatusCode = new List<string>() { "PURC" };
                var listStatusId = context.PurchaseOrderStatus
                    .Where(ct => listStatusCode.Contains(ct.PurchaseOrderStatusCode) && ct.Active)
                    .Select(ct => ct.PurchaseOrderStatusId).ToList();

                //Nếu là phiếu mua hàng
                if (parameter.ObjectType == 1)
                {
                    //Lấy đơn hàng mua theo Id
                    var listVendorOrder = context.VendorOrder
                        .Where(x => parameter.ListObjectId.Contains(x.VendorOrderId) &&
                                    listStatusId.Contains(x.StatusId)).ToList();
                    var listVendorOrderId = listVendorOrder.Select(y => y.VendorOrderId).ToList();

                    //Lấy list sản phẩm của đơn hàng mua
                    var listVendorOrderDetail = context.VendorOrderDetail
                        .Where(x => listVendorOrderId.Contains(x.VendorOrderId)).OrderBy(z => z.VendorOrderId).ToList();

                    #region Lấy số lượng cần nhập hiện tại

                    //Trạng thái của Phiếu nhập kho
                    var statusPhieuNhapKhoType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH");
                    var statusNhapKho = context.Category.FirstOrDefault(x =>
                        x.CategoryTypeId == statusPhieuNhapKhoType.CategoryTypeId && x.CategoryCode == "NHK");

                    //Lấy list Id phiếu nhập kho có trạng thái Nhập kho và loại phiếu nhập kho là Phiếu mua hàng
                    var listPhieuNhapKhoId = context.InventoryReceivingVoucher
                        .Where(x => x.StatusId == statusNhapKho.CategoryId && x.InventoryReceivingVoucherType == 1)
                        .Select(y => y.InventoryReceivingVoucherId)
                        .ToList();

                    //Lấy list sản phẩm đã nhập kho
                    var listSanPhamDaNhapKho = context.InventoryReceivingVoucherMapping
                        .Where(x => listPhieuNhapKhoId.Contains(x.InventoryReceivingVoucherId) &&
                                    x.ObjectId != null &&
                                    listVendorOrderId.Contains(x.ObjectId.Value)).ToList();

                    //Lấy list sản phẩm của phiếu nhập kho (nếu là màn hình chi tiết)
                    var listSanPhamCuaPhieuNhap = context.InventoryReceivingVoucherMapping
                        .Where(x => x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).ToList();

                    listVendorOrderDetail.ForEach(item =>
                    {
                        var tongSoLuongDaNhap = listSanPhamDaNhapKho.Where(x =>
                                x.ObjectDetailId == item.VendorOrderDetailId) //x.ProductId == item.ProductId && x.ObjectId == item.VendorOrderId
                            .Sum(s => s.QuantityActual);
                        var soLuongCanNhap = item.Quantity - tongSoLuongDaNhap;

                        //Nếu sản phẩm trong Đơn hàng mua chưa được nhập kho hết
                        if (soLuongCanNhap > 0)
                        {
                            //Lấy lại số thực nhập nếu có
                            var _item = listSanPhamCuaPhieuNhap.FirstOrDefault(x =>
                                x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId &&
                                x.ObjectDetailId == item.VendorOrderDetailId && x.WarehouseId == parameter.WarehouseId);
                            decimal soThucNhap = _item == null ? 0 : _item.QuantityActual;

                            var sanPhamNhapKho = new SanPhamPhieuNhapKhoModel();
                            sanPhamNhapKho.ObjectId = item.VendorOrderId;
                            sanPhamNhapKho.ObjectDetailId = item.VendorOrderDetailId;
                            sanPhamNhapKho.ProductId = item.ProductId;
                            sanPhamNhapKho.QuantityRequest = soLuongCanNhap.Value;
                            sanPhamNhapKho.QuantityReservation = 0;
                            sanPhamNhapKho.QuantityActual = soThucNhap;
                            sanPhamNhapKho.PriceAverage = false;
                            sanPhamNhapKho.PriceProduct = item.PriceWarehouse ?? 0;
                            sanPhamNhapKho.WarehouseId = parameter.WarehouseId;
                            sanPhamNhapKho.OrderCode = "";
                            sanPhamNhapKho.ProductCode = "";
                            sanPhamNhapKho.Description = "";
                            sanPhamNhapKho.UnitName = "";
                            sanPhamNhapKho.WarehouseName = "";
                            sanPhamNhapKho.WarehouseCodeName = "";

                            listItemDetail.Add(sanPhamNhapKho);
                        }
                    });

                    //Lấy thêm thông tin cho list sản phẩm
                    if (listItemDetail.Count > 0)
                    {
                        var _listVendorOrderId = listItemDetail.Select(y => y.ObjectId).ToList();
                        var listAllVendorOrder = context.VendorOrder
                            .Where(x => _listVendorOrderId.Contains(x.VendorOrderId)).Select(y => new VendorOrder
                            {
                                VendorOrderId = y.VendorOrderId,
                                VendorOrderCode = y.VendorOrderCode
                            }).ToList();

                        var _listProductId = listItemDetail.Select(y => y.ProductId).ToList();
                        var listAllProduct = context.Product.Where(x => _listProductId.Contains(x.ProductId)).Select(
                            y => new Product
                            {
                                ProductId = y.ProductId,
                                ProductCode = y.ProductCode,
                                ProductName = y.ProductName,
                                ProductUnitId = y.ProductUnitId
                            }).ToList();

                        //Đơn vị tính
                        var unitProductType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH");
                        var listUnitProduct =
                            context.Category.Where(x => x.CategoryTypeId == unitProductType.CategoryTypeId).ToList();

                        listItemDetail.ForEach(item =>
                        {
                            var vendor = listAllVendorOrder.FirstOrDefault(x => x.VendorOrderId == item.ObjectId);

                            if (vendor != null)
                            {
                                item.OrderCode = vendor.VendorOrderCode;
                            }

                            var product = listAllProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                            if (product != null)
                            {
                                item.ProductCode = product.ProductCode;
                                item.Description = product.ProductName;

                                var unit = listUnitProduct.FirstOrDefault(x => x.CategoryId == product.ProductUnitId);

                                if (unit != null)
                                {
                                    item.UnitName = unit.CategoryName;
                                }
                            }
                        });

                        #region Tính số giữ trước

                        var systemType = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "HIM")
                            ?.SystemValueString;

                        if (!String.IsNullOrEmpty(systemType))
                        {
                            if (systemType.Trim() == "1" || systemType.Trim() == "2")
                            {
                                listItemDetail.ForEach(item =>
                                {
                                    item.QuantityReservation = item.QuantityRequest;
                                });
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
                //Nếu là phiếu xuất kho
                else if (parameter.ObjectType == 2)
                {

                }
                //Nếu là phiếu kiểm kê
                else if (parameter.ObjectType == 3)
                {
                    
                }
                //Nếu là điều chuyển
                else if (parameter.ObjectType == 4)
                {
                    
                }
                else
                {
                    return new GetDanhSachSanPhamCuaPhieuResult()
                    {
                        Status = false,
                        Message = "Không tồn tại loại phiếu",
                        ListItemDetail = listItemDetail
                    };
                }

                return new GetDanhSachSanPhamCuaPhieuResult()
                {
                    Status = true,
                    Message = "Success",
                    ListItemDetail = listItemDetail
                };
            }
            catch (Exception e)
            {
                return new GetDanhSachSanPhamCuaPhieuResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDanhSachKhoConResult GetDanhSachKhoCon(GetDanhSachKhoConParameter parameter)
        {
            try
            {
                var listAllWarehouse = context.Warehouse.Where(x => x.Active).ToList();

                var listWarehouse = new List<WareHouseEntityModel>();

                var listResultId = getListWarehouseChildrenId(listAllWarehouse, parameter.WarehouseId, new List<Guid?>());
                listResultId.Add(parameter.WarehouseId);

                listWarehouse = listAllWarehouse.Where(x => listResultId.Contains(x.WarehouseId)).Select(y =>
                    new WareHouseEntityModel
                    {
                        WarehouseId = y.WarehouseId,
                        WarehouseParent = y.WarehouseParent,
                        WarehouseCode = y.WarehouseCode,
                        WarehouseName = y.WarehouseName,
                        WarehouseCodeName = y.WarehouseCode + " - " + y.WarehouseName
                    }).ToList();

                //Phân loại kho: Có kho con (hasChild = true) và Không có kho con (hasChild = false)
                listWarehouse.ForEach(item =>
                {
                    var hasChild = listAllWarehouse.FirstOrDefault(x => x.WarehouseParent == item.WarehouseId);
                    
                    if (hasChild != null)
                    {
                        item.HasChild = true;
                    }
                    else
                    {
                        item.HasChild = false;
                    }
                });

                return new GetDanhSachKhoConResult()
                {
                    Status = true,
                    Message = "Success",
                    ListWarehouse = listWarehouse
                };
            }
            catch (Exception e)
            {
                return new GetDanhSachKhoConResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateItemInventoryReportResult CreateItemInventoryReport(CreateItemInventoryReportParameter parameter)
        {
            try
            {
                var InventoryReportId = Guid.Empty;

                var inventoryReport = new InventoryReport();

                inventoryReport.InventoryReportId = Guid.NewGuid();
                inventoryReport.WarehouseId = parameter.InventoryReport.WarehouseId;
                inventoryReport.ProductId = parameter.InventoryReport.ProductId;
                inventoryReport.Quantity = parameter.InventoryReport.Quantity;
                inventoryReport.QuantityMinimum = parameter.InventoryReport.QuantityMinimum;
                inventoryReport.QuantityMaximum = parameter.InventoryReport.QuantityMaximum;
                inventoryReport.StartQuantity = parameter.InventoryReport.StartQuantity;
                inventoryReport.OpeningBalance = parameter.InventoryReport.OpeningBalance;
                inventoryReport.Note = parameter.InventoryReport.Note;
                inventoryReport.Active = true;
                inventoryReport.CreatedById = parameter.UserId;
                inventoryReport.CreatedDate = DateTime.Now;

                context.InventoryReport.Add(inventoryReport);
                context.SaveChanges();

                InventoryReportId = inventoryReport.InventoryReportId;

                return new CreateItemInventoryReportResult()
                {
                    Status = true,
                    Message = "Success",
                    InventoryReportId = InventoryReportId
                };
            }
            catch (Exception e)
            {
                return new CreateItemInventoryReportResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public UpdateItemInventoryReportResult UpdateItemInventoryReport(UpdateItemInventoryReportParameter parameter)
        {
            try
            {
                var inventoryReport = context.InventoryReport.FirstOrDefault(x =>
                    x.InventoryReportId == parameter.InventoryReport.InventoryReportId);

                if (inventoryReport == null)
                {
                    return new UpdateItemInventoryReportResult()
                    {
                        Status = false,
                        Message = "Dữ liệu không tồn tại trên hệ thống"
                    };
                }

                inventoryReport.QuantityMinimum = parameter.InventoryReport.QuantityMinimum;
                inventoryReport.QuantityMaximum = parameter.InventoryReport.QuantityMaximum;
                inventoryReport.StartQuantity = parameter.InventoryReport.StartQuantity;
                inventoryReport.OpeningBalance = parameter.InventoryReport.OpeningBalance;

                context.InventoryReport.Update(inventoryReport);
                context.SaveChanges();

                return new UpdateItemInventoryReportResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateItemInventoryReportResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateUpdateSerialResult CreateUpdateSerial(CreateUpdateSerialParameter parameter)
        {
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var listSerial = new List<SerialEntityModel>();

                    #region Thêm serial

                    var SERIAL_STATUS_CODE = "TSE";
                    var serialStatusId = context.CategoryType
                        .FirstOrDefault(w => w.CategoryTypeCode == SERIAL_STATUS_CODE)?.CategoryTypeId;
                    var NEW_SERIAL_STATUS_CODE = "CXU"; //Mặc định trạng thái mới của serial: Chưa xuất;
                    var statusId = context.Category.FirstOrDefault(w =>
                            w.CategoryTypeId == serialStatusId && w.CategoryCode == NEW_SERIAL_STATUS_CODE)?
                        .CategoryId;

                    var listOldSerial = context.Serial.Where(x =>
                        x.ProductId == parameter.ProductId &&
                        x.WarehouseId == parameter.WarehouseId).ToList();

                    context.Serial.RemoveRange(listOldSerial);
                    context.SaveChanges();

                    var listNewSerial = new List<Serial>();
                    parameter.ListSerial.ForEach(item =>
                    {
                        var newSerial = new Serial();
                        newSerial.SerialId = item.SerialId.Value == Guid.Empty ? Guid.NewGuid() : item.SerialId.Value;
                        newSerial.SerialCode = item.SerialCode;
                        newSerial.ProductId = item.ProductId.Value;
                        newSerial.WarehouseId = item.WarehouseId;
                        newSerial.StatusId = item.SerialId.Value == Guid.Empty ? statusId.Value : item.StatusId.Value;
                        newSerial.CreatedDate = DateTime.Now;
                        newSerial.CreatedById = parameter.UserId;
                        newSerial.Active = true;

                        listNewSerial.Add(newSerial);
                    });

                    context.Serial.AddRange(listNewSerial);
                    context.SaveChanges();

                    transaction.Commit();

                    listSerial = context.Serial
                        .Where(x => x.WarehouseId == parameter.WarehouseId && x.ProductId == parameter.ProductId)
                        .Select(y => new SerialEntityModel
                        {
                            SerialId = y.SerialId,
                            WarehouseId = y.WarehouseId,
                            ProductId = y.ProductId,
                            SerialCode = y.SerialCode,
                            StatusId = y.StatusId,
                            CreatedDate = y.CreatedDate
                        }).OrderBy(z => z.SerialCode).ToList();

                    #endregion

                    return new CreateUpdateSerialResult()
                    {
                        Status = true,
                        Message = "Success",
                        ListSerial = listSerial
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateUpdateSerialResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DeleteItemInventoryReportResult DeleteItemInventoryReport(DeleteItemInventoryReportParameter parameter)
        {
            try
            {
                var inventoryReport =
                    context.InventoryReport.FirstOrDefault(x => x.InventoryReportId == parameter.InventoryReportId);

                if (inventoryReport == null)
                {
                    return new DeleteItemInventoryReportResult()
                    {
                        Status = false,
                        Message = "Dữ liệu không tồn tại trên hệ thống"
                    };
                }

                context.InventoryReport.Remove(inventoryReport);

                var listSerial = context.Serial.Where(x =>
                    x.WarehouseId == inventoryReport.WarehouseId && x.ProductId == inventoryReport.ProductId).ToList();

                context.Serial.RemoveRange(listSerial);

                context.SaveChanges();

                return new DeleteItemInventoryReportResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DeleteItemInventoryReportResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetSoGTCuaSanPhamTheoKhoResult GetSoGTCuaSanPhamTheoKho(GetSoGTCuaSanPhamTheoKhoParameter parameter)
        {
            try
            {
                decimal quantityReservation = parameter.QuantityRequest;
                var systemType = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "HIM")
                    ?.SystemValueString;

                if (!String.IsNullOrEmpty(systemType))
                {
                    if (systemType.Trim() == "3")
                    {
                        var warehouse =
                                    context.Warehouse.FirstOrDefault(x => x.WarehouseParent == parameter.WarehouseId);

                        var listAllInventoryReceivingVoucher = context.InventoryReceivingVoucher.ToList();
                        var listAllInventoryReceivingVoucherMapping =
                            context.InventoryReceivingVoucherMapping.ToList();
                        var listAllInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.ToList();
                        var listAllInventoryDeliveryVoucherMapping =
                            context.InventoryDeliveryVoucherMapping.ToList();
                        var listAllInventoryReport = context.InventoryReport.ToList();

                        var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                            ?.CategoryTypeId;
                        var statusId_PNK = context.Category
                            .FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PNK)
                            ?.CategoryId;
                        var statusId_SanSang_PNK = context.Category
                            .FirstOrDefault(x => x.CategoryCode == "SAS" && x.CategoryTypeId == statusTypeId_PNK)
                            ?.CategoryId;

                        var statusTypeId_PXK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPHX")
                            ?.CategoryTypeId;
                        var statusId_PXK = context.Category
                            .FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PXK)
                            ?.CategoryId;
                        var statusId_SanSang_PXK = context.Category
                            .FirstOrDefault(x => x.CategoryCode == "SAS" && x.CategoryTypeId == statusTypeId_PXK)
                            ?.CategoryId;

                        //Nếu kho được chọn có Kho con thì Số giữ trước = 0
                        if (warehouse != null)
                        {
                            quantityReservation = 0;
                        }
                        else
                        {
                            //Số tồn kho tối đa của sản phẩm
                            var inventoryReport = listAllInventoryReport.FirstOrDefault(x =>
                                x.WarehouseId == parameter.WarehouseId && x.ProductId == parameter.ProductId);

                            var quantityMaximum = inventoryReport != null
                                ? inventoryReport.QuantityMaximum
                                : null;

                            //Số tồn kho thực tế của sản phẩm
                            var quantityInStock = GetSoTonKhoThucTe(parameter.WarehouseId,
                                parameter.ProductId,
                                statusId_PNK.Value, statusId_PXK.Value,
                                listAllInventoryReceivingVoucher,
                                listAllInventoryReceivingVoucherMapping,
                                listAllInventoryDeliveryVoucher,
                                listAllInventoryDeliveryVoucherMapping,
                                listAllInventoryReport);

                            #region Số giữ trước của sản phẩm trên phiếu nhập kho

                            var listIdPhieuNK = listAllInventoryReceivingVoucher
                                .Where(x => x.StatusId == statusId_SanSang_PNK)
                                .Select(y => y.InventoryReceivingVoucherId).ToList();

                            var quantityReservation_NK = listAllInventoryReceivingVoucherMapping
                                .Where(x => listIdPhieuNK.Contains(x.InventoryReceivingVoucherId) &&
                                            x.ProductId == parameter.ProductId).Sum(s => s.QuantityReservation);

                            #endregion


                            #region Số giữ trước của sản phẩm trên phiếu xuất kho

                            var listIdPhieuXK = listAllInventoryDeliveryVoucher
                                .Where(x => x.StatusId == statusId_SanSang_PXK)
                                .Select(y => y.InventoryDeliveryVoucherId).ToList();

                            var quantityReservation_XK = listAllInventoryDeliveryVoucherMapping
                                .Where(x => listIdPhieuXK.Contains(x.InventoryDeliveryVoucherId) &&
                                            x.ProductId == parameter.ProductId).Sum(s => s.QuantityReservation);

                            #endregion

                            /*
                             * Số tồn kho kinh doanh của sản phẩm = Số tồn kho thực tế +
                             *                                      Số giữ trước của sản phẩm trên phiếu nhập kho -
                             *                                      Số giữ trước của sản phẩm trên phiếu xuất kho
                             */
                            var so_ton_kho_kinh_doanh =
                                quantityInStock + quantityReservation_NK - quantityReservation_XK;

                            #region Số giữ trước

                            decimal so_B = 0;

                            /*
                             * Nếu số lượng tồn kho tối đa có giá trị null hoặc có giá trị bằng 0
                             * thì Số B = Số tồn kho kinh doanh của sản phẩm
                             */
                            if (quantityMaximum == null || quantityMaximum == 0)
                            {
                                so_B = so_ton_kho_kinh_doanh;
                            }
                            /*
                             * Số B = Số lượng tồn kho tối đa - Số tồn kho kinh doanh của sản phẩm
                             * 
                             */
                            else
                            {
                                so_B = quantityMaximum.Value - so_ton_kho_kinh_doanh;
                            }

                            /*
                             * A là Số cần nhập
                             * Nếu B = 0
                             */
                            if (so_B == 0)
                            {
                                var product_exists = listAllInventoryReceivingVoucherMapping
                                    .FirstOrDefault(x =>
                                        listIdPhieuNK.Contains(x.InventoryReceivingVoucherId) &&
                                        x.ProductId == parameter.ProductId);

                                //Nếu sản phẩm chưa nhập kho bao giờ thì Số giữ trước = A
                                if (product_exists == null)
                                {
                                    quantityReservation = parameter.QuantityRequest;
                                }
                                //Ngược lại Số giữ trước = 0
                                else
                                {
                                    quantityReservation = 0;
                                }
                            }
                            else
                            {
                                /*
                                 * Nếu B != 0
                                 * Nếu B >= A: Số giữ trước = A
                                 * Nếu B < A: Số giữ trước = B
                                 */
                                if (so_B >= parameter.QuantityRequest)
                                {
                                    quantityReservation = parameter.QuantityRequest;
                                }
                                else
                                {
                                    quantityReservation = so_B;
                                }
                            }

                            #endregion
                        }
                    }
                }

                return new GetSoGTCuaSanPhamTheoKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    QuantityReservation = quantityReservation
                };
            }
            catch (Exception e)
            {
                return new GetSoGTCuaSanPhamTheoKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreatePhieuNhapKhoResult CreatePhieuNhapKho(CreatePhieuNhapKhoParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var statusId_Nhap_PNK = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "NHA" && x.CategoryTypeId == statusTypeId_PNK)
                    ?.CategoryId;

                var Code = "";
                var datenow = DateTime.Now;
                string year = datenow.Year.ToString().Substring(datenow.Year.ToString().Length - 2, 2);
                string month = datenow.Month < 10 ? "0" + datenow.Month.ToString() : datenow.Month.ToString();
                string day = datenow.Day < 10 ? "0" + datenow.Day.ToString() : datenow.Day.ToString();

                var listCodeToDay = context.InventoryReceivingVoucher.Where(c =>
                    Convert.ToDateTime(c.CreatedDate).Day == datenow.Day &&
                    Convert.ToDateTime(c.CreatedDate).Month == datenow.Month &&
                    Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Select(y => new
                {
                    InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode
                }).ToList();

                if (listCodeToDay.Count == 0)
                {

                    Code = "PN-" + year + month + day + "0001";
                }
                else
                {
                    var listNumber = new List<int>();
                    listCodeToDay.ForEach(item =>
                    {
                        var stringNumber = item.InventoryReceivingVoucherCode.Substring(9);
                        var number = Int32.Parse(stringNumber);
                        listNumber.Add(number);
                    });

                    var maxNumber = listNumber.OrderByDescending(x => x).FirstOrDefault();
                    var newNumber = maxNumber + 1;

                    if (newNumber > 9999)
                    {
                        Code = "PN-" + year + month + day + newNumber;
                    }
                    else
                    {
                        Code = "PN-" + year + month + day + newNumber.ToString("D4");
                    }
                }

                parameter.InventoryReceivingVoucher.InventoryReceivingVoucherCode = Code;

                var existsCode = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherCode == Code);

                if (existsCode != null)
                {
                    return new CreatePhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Mã phiếu nhập kho đã tồn tại"
                    };
                }

                parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId = Guid.NewGuid();
                parameter.InventoryReceivingVoucher.StatusId = statusId_Nhap_PNK.Value;
                parameter.InventoryReceivingVoucher.Active = true;
                parameter.InventoryReceivingVoucher.CreatedById = parameter.UserId;
                parameter.InventoryReceivingVoucher.CreatedDate = DateTime.Now;

                context.InventoryReceivingVoucher.Add(parameter.InventoryReceivingVoucher);

                parameter.ListInventoryReceivingVoucherMapping.ForEach(item =>
                {
                    item.InventoryReceivingVoucherMappingId = Guid.NewGuid();
                    item.InventoryReceivingVoucherId = parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId;
                    item.Active = true;
                    item.CreatedById = parameter.UserId;
                    item.CreatedDate = DateTime.Now;
                });

                context.InventoryReceivingVoucherMapping.AddRange(parameter.ListInventoryReceivingVoucherMapping);

                #region Lưu list file

                var folder = context.Folder.FirstOrDefault(x => x.Active && x.FolderType == "QLNK");
                if (folder == null)
                {
                    return new CreatePhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Chưa có thư mục để lưu. Bạn phải cấu hình thư mục để lưu"
                    };
                }

                if (parameter.ListFile != null && parameter.ListFile.Count > 0)
                {
                    var folderName = folder.Url + "\\";
                    string webRootPath = hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);

                    if (!Directory.Exists(newPath))
                    {
                        return new CreatePhieuNhapKhoResult()
                        {
                            Status = false,
                            Message = "Thư mục vật lý không tồn tại"
                        };
                    }

                    foreach (IFormFile file in parameter.ListFile)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = file.FileName.Trim();

                            var fileInForder = new FileInFolder();
                            fileInForder.Active = true;
                            fileInForder.CreatedById = parameter.UserId;
                            fileInForder.CreatedDate = DateTime.Now;
                            fileInForder.FileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                            fileInForder.FileInFolderId = Guid.NewGuid();
                            fileInForder.FileName =
                                fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + Guid.NewGuid();
                            fileInForder.FolderId = folder.FolderId;
                            fileInForder.ObjectId = parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId;
                            fileInForder.ObjectType = "QLNK";
                            fileInForder.Size = file.Length.ToString();
                            context.FileInFolder.Add(fileInForder);
                            fileName = fileInForder.FileName + "." + fileInForder.FileExtension;
                            string fullPath = Path.Combine(newPath, fileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                    }
                }

                #endregion

                #region Thêm vào Dòng thời gian

                var note = new Note();
                note.NoteId = Guid.NewGuid();
                note.Description = employee.EmployeeName.Trim() + " đã tạo phiếu nhập kho";
                note.Type = "NEW";
                note.ObjectId = parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId;
                note.ObjectType = "PNK";
                note.Active = true;
                note.NoteTitle = "Đã tạo phiếu nhập kho";
                note.CreatedDate = DateTime.Now;
                note.CreatedById = parameter.UserId;

                context.Note.Add(note);

                #endregion

                context.SaveChanges();

                return new CreatePhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    InventoryReceivingVoucherId = parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId
                };
            }
            catch (Exception e)
            {
                return new CreatePhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDetailPhieuNhapKhoResult GetDetailPhieuNhapKho(GetDetailPhieuNhapKhoParameter parameter)
        {
            try
            {
                var listVendor = new List<VendorEntityModel>();
                var listWarehouse = new List<WareHouseEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listVendorOrder = new List<VendorOrderEntityModel>();
                var listProduct = new List<ProductEntityModel>();
                var listSelectedVendorOrderId = new List<Guid?>();
                var listFileUpload = new List<FileInFolderEntityModel>();

                listVendor = context.Vendor.Where(x => x.Active == true).Select(y => new VendorEntityModel
                {
                    VendorId = y.VendorId,
                    VendorCode = y.VendorCode,
                    VendorName = y.VendorName,
                    VendorCodeName = y.VendorCode.Trim() + " - " + y.VendorName.Trim()
                }).OrderBy(z => z.VendorName).ToList();

                var _listAllWarehouse = context.Warehouse.Where(x => x.Active == true).Select(y =>
                    new WareHouseEntityModel
                    {
                        WarehouseId = y.WarehouseId,
                        WarehouseParent = y.WarehouseParent,
                        HasChild = false,
                        WarehouseCode = y.WarehouseCode,
                        WarehouseName = y.WarehouseName,
                        WarehouseCodeName = y.WarehouseCode.Trim() + " - " + y.WarehouseName.Trim()
                    }).OrderBy(z => z.WarehouseName).ToList();

                listWarehouse = context.Warehouse.Where(x => x.Active && x.WarehouseParent == null).Select(y =>
                    new WareHouseEntityModel
                    {
                        WarehouseId = y.WarehouseId,
                        WarehouseParent = y.WarehouseParent,
                        HasChild = false,
                        WarehouseCode = y.WarehouseCode,
                        WarehouseName = y.WarehouseName,
                        WarehouseCodeName = y.WarehouseCode.Trim() + " - " + y.WarehouseName.Trim()
                    }).OrderBy(z => z.WarehouseName).ToList();

                var listAllWarehouse = context.Warehouse.Where(x => x.Active).ToList();

                listWarehouse.ForEach(item =>
                {
                    var hasChild = listAllWarehouse.FirstOrDefault(x => x.WarehouseParent == item.WarehouseId);

                    //Nếu Kho có kho con
                    if (hasChild != null)
                    {
                        item.HasChild = true;
                    }
                    //Nếu kho không có kho con
                    else
                    {
                        item.HasChild = false;
                    }
                });

                _listAllWarehouse.ForEach(item =>
                {
                    var hasChild = listAllWarehouse.FirstOrDefault(x => x.WarehouseParent == item.WarehouseId);

                    //Nếu Kho có kho con
                    if (hasChild != null)
                    {
                        item.HasChild = true;
                    }
                    //Nếu kho không có kho con
                    else
                    {
                        item.HasChild = false;
                    }
                });

                //Lấy list khách hàng có trạng thái Định danh
                var statusCustomerType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                var statusCustomerDd = context.Category.FirstOrDefault(x =>
                    x.CategoryTypeId == statusCustomerType.CategoryTypeId && x.CategoryCode == "HDO");
                listCustomer = context.Customer
                    .Where(x => x.Active == true && x.StatusId == statusCustomerDd.CategoryId).Select(y =>
                        new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            CustomerCodeName = y.CustomerCode.Trim() + " - " + y.CustomerName.Trim()
                        }).OrderBy(z => z.CustomerName).ToList();

                var phieuNhapKho = context.InventoryReceivingVoucher.Where(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).Select(y =>
                    new PhieuNhapKhoModel
                    {
                        InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                        InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode,
                        StatusId = y.StatusId,
                        InventoryReceivingVoucherType = y.InventoryReceivingVoucherType,
                        WarehouseId = y.WarehouseId,
                        ShiperName = y.ShiperName,
                        Storekeeper = y.Storekeeper,
                        InventoryReceivingVoucherDate = y.InventoryReceivingVoucherDate,
                        InventoryReceivingVoucherTime = y.InventoryReceivingVoucherTime,
                        LicenseNumber = y.LicenseNumber,
                        ExpectedDate = y.ExpectedDate,
                        Description = y.Description,
                        Note = y.Note,
                        PartnersId = y.PartnersId,
                        StatusName = "",
                        StatusCode = "",
                        EmployeeCodeName = "",
                        TotalQuantityActual = 0,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate
                    }).FirstOrDefault();

                if (phieuNhapKho == null)
                {
                    return new GetDetailPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                    };
                }

                var listItemDetail = context.InventoryReceivingVoucherMapping
                    .Where(x => x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).Select(y =>
                        new SanPhamPhieuNhapKhoModel
                        {
                            InventoryReceivingVoucherMappingId = y.InventoryReceivingVoucherMappingId,
                            InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                            ObjectId = y.ObjectId,
                            ObjectDetailId = y.ObjectDetailId,
                            ProductId = y.ProductId,
                            QuantityRequest = y.QuantityRequest,
                            QuantityReservation = y.QuantityReservation,
                            QuantityActual = y.QuantityActual,
                            PriceAverage = y.PriceAverage,
                            PriceProduct = y.PriceProduct,
                            WarehouseId = y.WarehouseId,
                            Amount = Math.Round(y.QuantityActual * y.PriceProduct, 2),
                            OrderCode = "",
                            ProductCode = "",
                            Description = "",
                            UnitName = "",
                            WarehouseName = "",
                            WarehouseCodeName = ""
                        }).OrderBy(z => z.ObjectId).ToList();

                #region Lấy tên trạng thái Phiếu nhập kho

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatus_PNK = context.Category
                    .Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();

                var status = listStatus_PNK.FirstOrDefault(x => x.CategoryId == phieuNhapKho.StatusId);
                phieuNhapKho.StatusName = status != null ? status.CategoryName.Trim() : "";
                phieuNhapKho.StatusCode = status != null ? status.CategoryCode.Trim() : "";

                #endregion

                #region Lấy tên người lập phiếu nhập kho

                var userCreated = context.User.FirstOrDefault(x => x.UserId == phieuNhapKho.CreatedById);
                var employeeCreated = context.Employee.FirstOrDefault(x => x.EmployeeId == userCreated.EmployeeId);
                phieuNhapKho.EmployeeCodeName =
                    employeeCreated.EmployeeCode.Trim() + " - " + employeeCreated.EmployeeName.Trim();

                #endregion

                #region Lấy Tổng số lượng thực nhập

                phieuNhapKho.TotalQuantityActual = listItemDetail.Sum(x => x.QuantityActual);

                #endregion

                #region Lấy list File Upload

                listFileUpload = context.FileInFolder
                    .Where(x => x.ObjectId == phieuNhapKho.InventoryReceivingVoucherId && x.ObjectType == "QLNK")
                    .Select(y => new FileInFolderEntityModel
                    {
                        FileInFolderId = y.FileInFolderId,
                        FileName = y.FileName,
                        FileExtension = y.FileExtension,
                        Size = y.Size,
                        CreatedDate = y.CreatedDate
                    }).OrderBy(z => z.CreatedDate).ToList();

                #endregion

                var listAllProduct = context.Product.ToList();
                //Đơn vị tính
                var unitProductType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH");
                var listUnitProduct =
                    context.Category.Where(x => x.CategoryTypeId == unitProductType.CategoryTypeId).ToList();
                var listAllVendorOrder = context.VendorOrder.ToList();

                listItemDetail.ForEach(item =>
                {
                    var order = listAllVendorOrder.FirstOrDefault(x => x.VendorOrderId == item.ObjectId);
                    var product = listAllProduct.FirstOrDefault(x => x.ProductId == item.ProductId);
                    var unit = listUnitProduct.FirstOrDefault(x => x.CategoryId == product?.ProductUnitId);
                    var warehouse = listAllWarehouse.FirstOrDefault(x => x.WarehouseId == item.WarehouseId);

                    item.OrderCode = order != null ? order.VendorOrderCode.Trim() : "";
                    item.ProductCode = product != null ? product.ProductCode.Trim() : "";
                    item.Description = product != null ? product.ProductName.Trim() : "";
                    item.UnitName = unit != null ? unit.CategoryName.Trim() : "";
                    item.WarehouseName = warehouse != null ? warehouse.WarehouseName.Trim() : "";
                    item.WarehouseCodeName = warehouse != null
                        ? warehouse.WarehouseCode.Trim() + " - " + warehouse.WarehouseName.Trim()
                        : "";
                });

                var type = phieuNhapKho.InventoryReceivingVoucherType;
                //Nhập theo phiếu mua hàng
                if (type == 1)
                {
                    #region Lấy list Đơn hàng mua

                    //Lấy list Id đơn hàng mua đã lưu
                    listSelectedVendorOrderId = listItemDetail.Select(y => y.ObjectId).Distinct().ToList();

                    //Lấy Id các trạng thái đơn hàng: Đơn hàng mua
                    var listStatusCode = new List<string>() { "PURC" };
                    var listStatusId = context.PurchaseOrderStatus
                        .Where(ct => listStatusCode.Contains(ct.PurchaseOrderStatusCode) && ct.Active)
                        .Select(ct => ct.PurchaseOrderStatusId).ToList();

                    listVendorOrder = listAllVendorOrder
                        .Where(x => x.Active == true &&
                                    (listSelectedVendorOrderId.Contains(x.VendorOrderId) ||
                                     listStatusId.Contains(x.StatusId)) &&
                                    x.VendorId == phieuNhapKho.PartnersId)
                        .Select(y => new VendorOrderEntityModel
                        {
                            VendorOrderId = y.VendorOrderId,
                            VendorOrderCode = y.VendorOrderCode,
                            Amount = y.Amount,
                            Description = y.Description,
                            VendorDescripton = "",
                            VendorId = y.VendorId
                        }).ToList();

                    var listAllVendor = context.Vendor.ToList();
                    listVendorOrder.ForEach(item =>
                    {
                        var vendor = listAllVendor.FirstOrDefault(x => x.VendorId == item.VendorId);
                        var vendorName = "";

                        if (vendor != null)
                        {
                            vendorName = vendor.VendorName;
                        }

                        item.VendorDescripton = item.VendorOrderCode + " - " + vendorName + " - " + item.Description +
                                                " - " + item.Amount.ToString("#,#");
                    });

                    #endregion
                }
                else if (type == 2)
                {

                }
                else if (type == 3)
                {

                }
                else if (type == 4)
                {

                }
                else if (type == 5)
                {
                    #region Lấy listProduct

                    listProduct = context.Product
                        .Where(x => x.Active == true).Select(
                            y => new ProductEntityModel
                            {
                                ProductId = y.ProductId,
                                ProductName = y.ProductName,
                                ProductCode = y.ProductCode,
                                ProductCodeName = y.ProductCode.Trim() + " - " + y.ProductName.Trim(),
                                ProductUnitId = y.ProductUnitId,
                                ProductUnitName = ""
                            }).OrderBy(z => z.ProductName).ToList();

                    listProduct.ForEach(item =>
                    {
                        var unit = listUnitProduct.FirstOrDefault(x => x.CategoryId == item.ProductUnitId);

                        if (unit != null)
                        {
                            item.ProductUnitName = unit.CategoryName ?? "";
                        }
                    });

                    #endregion
                }

                #region Lấy list ghi chú

                var noteHistory = new List<NoteEntityModel>();

                noteHistory = context.Note
                    .Where(x => x.ObjectId == parameter.InventoryReceivingVoucherId && x.ObjectType == "PNK" &&
                                x.Active == true).Select(
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

                if (noteHistory.Count > 0)
                {
                    var listNoteId = noteHistory.Select(x => x.NoteId).ToList();
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

                    noteHistory.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                            .OrderByDescending(z => z.UpdatedDate).ToList();
                    });

                    //Sắp xếp lại listNote
                    noteHistory = noteHistory.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                return new GetDetailPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    ListVendor = listVendor,
                    ListAllWarehouse = _listAllWarehouse,
                    ListWarehouse = listWarehouse,
                    ListCustomer = listCustomer,
                    PhieuNhapKho = phieuNhapKho,
                    ListItemDetail = listItemDetail,
                    ListVendorOrder = listVendorOrder,
                    ListProduct = listProduct,
                    ListSelectedVendorOrderId = listSelectedVendorOrderId,
                    ListFileUpload = listFileUpload,
                    NoteHistory = noteHistory
                };
            }
            catch (Exception e)
            {
                return new GetDetailPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SuaPhieuNhapKhoResult SuaPhieuNhapKho(SuaPhieuNhapKhoParameter parameter)
        {
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId);

                    if (inventoryReceivingVoucher == null)
                    {
                        return new SuaPhieuNhapKhoResult()
                        {
                            Status = false,
                            Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                        };
                    }

                    inventoryReceivingVoucher.WarehouseId = parameter.InventoryReceivingVoucher.WarehouseId;
                    inventoryReceivingVoucher.ShiperName = parameter.InventoryReceivingVoucher.ShiperName;
                    inventoryReceivingVoucher.InventoryReceivingVoucherDate =
                        parameter.InventoryReceivingVoucher.InventoryReceivingVoucherDate;
                    inventoryReceivingVoucher.InventoryReceivingVoucherTime =
                        parameter.InventoryReceivingVoucher.InventoryReceivingVoucherTime;
                    inventoryReceivingVoucher.LicenseNumber = parameter.InventoryReceivingVoucher.LicenseNumber;
                    inventoryReceivingVoucher.ExpectedDate = parameter.InventoryReceivingVoucher.ExpectedDate;
                    inventoryReceivingVoucher.Description = parameter.InventoryReceivingVoucher.Description;
                    inventoryReceivingVoucher.Note = parameter.InventoryReceivingVoucher.Note;
                    inventoryReceivingVoucher.PartnersId = parameter.InventoryReceivingVoucher.PartnersId;

                    context.InventoryReceivingVoucher.Update(inventoryReceivingVoucher);
                    context.SaveChanges();

                    #region Thêm sản phẩm

                    var listOldItemDetail = context.InventoryReceivingVoucherMapping.Where(x =>
                            x.InventoryReceivingVoucherId ==
                            parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId)
                        .ToList();

                    context.InventoryReceivingVoucherMapping.RemoveRange(listOldItemDetail);
                    context.SaveChanges();

                    var listInventoryReceivingVoucherMapping = new List<InventoryReceivingVoucherMapping>();
                    parameter.ListInventoryReceivingVoucherMapping.ForEach(item =>
                    {
                        var inventoryReceivingVoucherMapping = new InventoryReceivingVoucherMapping();
                        inventoryReceivingVoucherMapping.InventoryReceivingVoucherMappingId = Guid.NewGuid();
                        inventoryReceivingVoucherMapping.InventoryReceivingVoucherId =
                            parameter.InventoryReceivingVoucher.InventoryReceivingVoucherId;
                        inventoryReceivingVoucherMapping.ObjectId = item.ObjectId;
                        inventoryReceivingVoucherMapping.ObjectDetailId = item.ObjectDetailId;
                        inventoryReceivingVoucherMapping.ProductId = item.ProductId;
                        inventoryReceivingVoucherMapping.QuantityRequest = item.QuantityRequest;
                        inventoryReceivingVoucherMapping.QuantityActual = item.QuantityActual;
                        inventoryReceivingVoucherMapping.QuantitySerial = item.QuantitySerial;
                        inventoryReceivingVoucherMapping.PriceProduct = item.PriceProduct;
                        inventoryReceivingVoucherMapping.WarehouseId = item.WarehouseId;
                        inventoryReceivingVoucherMapping.Description = item.Description;
                        inventoryReceivingVoucherMapping.Active = true;
                        inventoryReceivingVoucherMapping.CreatedById = parameter.UserId;
                        inventoryReceivingVoucherMapping.CreatedDate = DateTime.Now;
                        inventoryReceivingVoucherMapping.QuantityReservation = item.QuantityReservation;
                        inventoryReceivingVoucherMapping.PriceAverage = item.PriceAverage;

                        listInventoryReceivingVoucherMapping.Add(inventoryReceivingVoucherMapping);
                    });

                    context.InventoryReceivingVoucherMapping.AddRange(listInventoryReceivingVoucherMapping);
                    context.SaveChanges();

                    #endregion

                    transaction.Commit();

                    return new SuaPhieuNhapKhoResult()
                    {
                        Status = true,
                        Message = "Success"
                    };
                }
            }
            catch (Exception e)
            {
                return new SuaPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public KiemTraKhaDungPhieuNhapKhoResult KiemTraKhaDungPhieuNhapKho(KiemTraKhaDungPhieuNhapKhoParameter parameter)
        {
            try
            {
                var systemType = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "HIM")
                    ?.SystemValueString;

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatusPNK = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();
                var statusId_PNK = listStatusPNK.FirstOrDefault(x => x.CategoryCode == "NHK")?.CategoryId;
                var statusId_SanSang_PNK = listStatusPNK.FirstOrDefault(x => x.CategoryCode == "SAS")?.CategoryId;

                var statusTypeId_PXK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPHX")
                    ?.CategoryTypeId;
                var listStatusPXK = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PXK).ToList();
                var statusId_PXK = listStatusPXK.FirstOrDefault(x => x.CategoryCode == "NHK")?.CategoryId;
                var statusId_SanSang_PXK = listStatusPXK.FirstOrDefault(x => x.CategoryCode == "SAS")?.CategoryId;

                if (!String.IsNullOrEmpty(systemType))
                {
                    if (systemType.Trim() == "3")
                    {
                        var listAllWarehouse = context.Warehouse.ToList();
                        var listAllInventoryReceivingVoucher = context.InventoryReceivingVoucher.ToList();
                        var listAllInventoryReceivingVoucherMapping =
                            context.InventoryReceivingVoucherMapping.ToList();
                        var listAllInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.ToList();
                        var listAllInventoryDeliveryVoucherMapping =
                            context.InventoryDeliveryVoucherMapping.ToList();
                        var listAllInventoryReport = context.InventoryReport.ToList();

                        parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                        {
                            //Nếu vị trí của sản phẩm (Kho) không hợp lệ (Có kho con) thì Số giữ trước bằng 0
                            var warehouseChild = listAllWarehouse.FirstOrDefault(x => x.WarehouseParent == item.WarehouseId);

                            if (warehouseChild != null)
                            {
                                item.QuantityReservation = 0;
                            }
                            else
                            {
                                //Số tồn kho tối đa của sản phẩm
                                var inventoryReport = listAllInventoryReport.FirstOrDefault(x =>
                                    x.WarehouseId == item.WarehouseId && x.ProductId == item.ProductId);

                                var quantityMaximum = inventoryReport != null
                                    ? inventoryReport.QuantityMaximum
                                    : null;

                                //Số tồn kho thực tế của sản phẩm
                                var quantityInStock = GetSoTonKhoThucTe(item.WarehouseId.Value,
                                    item.ProductId.Value,
                                    statusId_PNK.Value, statusId_PXK.Value,
                                    listAllInventoryReceivingVoucher,
                                    listAllInventoryReceivingVoucherMapping,
                                    listAllInventoryDeliveryVoucher,
                                    listAllInventoryDeliveryVoucherMapping,
                                    listAllInventoryReport);

                                #region Số giữ trước của sản phẩm trên phiếu nhập kho

                                var listIdPhieuNK = listAllInventoryReceivingVoucher
                                    .Where(x => x.StatusId == statusId_SanSang_PNK && x.InventoryReceivingVoucherId !=
                                                parameter.InventoryReceivingVoucherId)
                                    .Select(y => y.InventoryReceivingVoucherId).ToList();

                                var quantityReservation_NK = listAllInventoryReceivingVoucherMapping
                                    .Where(x => listIdPhieuNK.Contains(x.InventoryReceivingVoucherId) &&
                                                x.ProductId == item.ProductId).Sum(s => s.QuantityReservation);

                                #endregion

                                #region Số giữ trước của sản phẩm trên phiếu xuất kho

                                var listIdPhieuXK = listAllInventoryDeliveryVoucher
                                    .Where(x => x.StatusId == statusId_SanSang_PXK)
                                    .Select(y => y.InventoryDeliveryVoucherId).ToList();

                                var quantityReservation_XK = listAllInventoryDeliveryVoucherMapping
                                    .Where(x => listIdPhieuXK.Contains(x.InventoryDeliveryVoucherId) &&
                                                x.ProductId == item.ProductId).Sum(s => s.QuantityReservation);

                                #endregion

                                /*
                                 * Số tồn kho kinh doanh của sản phẩm = Số tồn kho thực tế +
                                 *                                      Số giữ trước của sản phẩm trên phiếu nhập kho -
                                 *                                      Số giữ trước của sản phẩm trên phiếu xuất kho
                                 */
                                var so_ton_kho_kinh_doanh =
                                    quantityInStock + quantityReservation_NK - quantityReservation_XK;

                                #region Số giữ trước

                                //Nếu không khai báo tồn kho tối đa thì Số giữ trước = Số cần nhập
                                if (quantityMaximum == null)
                                {
                                    item.QuantityReservation = item.QuantityRequest;
                                }
                                //Ngược lại nếu có khai báo số tồn kho tối đa
                                else
                                {
                                    decimal so_B = 0;
                                    so_B = quantityMaximum.Value - so_ton_kho_kinh_doanh;

                                    if (so_B >= item.QuantityRequest)
                                    {
                                        item.QuantityReservation = item.QuantityRequest;
                                    }
                                    else if (so_B >= 0 && so_B < item.QuantityRequest)
                                    {
                                        item.QuantityReservation = so_B;
                                    }
                                    else if (so_B < 0)
                                    {
                                        item.QuantityReservation = 0;
                                    }
                                }

                                #endregion
                            }
                        });
                    }
                    else
                    {
                        parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                        {
                            item.QuantityReservation = item.QuantityRequest;
                        });
                    }
                }

                #region Lưu list sản phẩm

                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                var listOldSanPham = context.InventoryReceivingVoucherMapping.Where(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).ToList();

                context.InventoryReceivingVoucherMapping.RemoveRange(listOldSanPham);

                var status = listStatusPNK.FirstOrDefault(x => x.CategoryId == inventoryReceivingVoucher.StatusId);

                var listNewSanPham = new List<InventoryReceivingVoucherMapping>();
                parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                {
                    var newSanPham = new InventoryReceivingVoucherMapping();
                    newSanPham.InventoryReceivingVoucherMappingId = Guid.NewGuid();
                    newSanPham.InventoryReceivingVoucherId = parameter.InventoryReceivingVoucherId;
                    newSanPham.ObjectId = item.ObjectId;
                    newSanPham.ProductId = item.ProductId.Value;
                    newSanPham.QuantityRequest = item.QuantityRequest;
                    newSanPham.QuantityActual = item.QuantityActual;
                    newSanPham.QuantitySerial = 0;
                    newSanPham.PriceProduct = item.PriceProduct;
                    newSanPham.WarehouseId = item.WarehouseId.Value;
                    newSanPham.Description = item.Description;
                    newSanPham.Active = true;
                    newSanPham.CreatedDate = DateTime.Now;
                    newSanPham.CreatedById = parameter.UserId;
                    newSanPham.QuantityReservation = item.QuantityReservation;
                    newSanPham.PriceAverage = item.PriceAverage;
                    newSanPham.ObjectDetailId = item.ObjectDetailId;

                    listNewSanPham.Add(newSanPham);
                });

                context.InventoryReceivingVoucherMapping.AddRange(listNewSanPham);

                if (status.CategoryCode == "CHO")
                {
                    //Nếu có ít nhất 1 sản phẩm có Số giữ trước > 0 thì chuyển trạng thái phiếu nhập kho -> Sẵn sàng
                    var check = false;
                    parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                    {
                        if (item.QuantityReservation > 0)
                        {
                            check = true;
                        }
                    });

                    if (check == true)
                    {
                        inventoryReceivingVoucher.StatusId = statusId_SanSang_PNK.Value;
                        context.InventoryReceivingVoucher.Update(inventoryReceivingVoucher);
                    }
                }

                context.SaveChanges();

                #endregion

                return new KiemTraKhaDungPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    ListSanPhamPhieuNhapKho = parameter.ListSanPhamPhieuNhapKho
                };
            }
            catch (Exception e)
            {
                return new KiemTraKhaDungPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DanhDauCanLamPhieuNhapKhoResult DanhDauCanLamPhieuNhapKho(DanhDauCanLamPhieuNhapKhoParameter parameter)
        {
            try
            {
                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                if (inventoryReceivingVoucher == null)
                {
                    return new DanhDauCanLamPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                    };
                }

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatus = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();
                var statusId_NHAP_PNK = listStatus.FirstOrDefault(x => x.CategoryCode == "NHA")?.CategoryId;
                var statusId_SS_PNK = listStatus.FirstOrDefault(x => x.CategoryCode == "SAS")?.CategoryId;
                var statusId_CHO_PNK = listStatus.FirstOrDefault(x => x.CategoryCode == "CHO")?.CategoryId;

                var status = listStatus.FirstOrDefault(x => x.CategoryId == inventoryReceivingVoucher.StatusId);

                if (inventoryReceivingVoucher.StatusId != statusId_NHAP_PNK)
                {
                    return new DanhDauCanLamPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Không thể đánh dấu cần làm với phiếu nhập kho có trạng thái " + status.CategoryName
                    };
                }

                if (parameter.Check)
                {
                    inventoryReceivingVoucher.StatusId = statusId_SS_PNK.Value;
                }
                else
                {
                    inventoryReceivingVoucher.StatusId = statusId_CHO_PNK.Value;
                }

                context.InventoryReceivingVoucher.Update(inventoryReceivingVoucher);
                context.SaveChanges();

                return new DanhDauCanLamPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DanhDauCanLamPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public NhanBanPhieuNhapKhoResult NhanBanPhieuNhapKho(NhanBanPhieuNhapKhoParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                if (inventoryReceivingVoucher == null)
                {
                    return new NhanBanPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                    };
                }

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatus = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();
                var statusId_NHAP_PNK = listStatus.FirstOrDefault(x => x.CategoryCode == "NHA")?.CategoryId;
                var statusId_HUY_PNK = listStatus.FirstOrDefault(x => x.CategoryCode == "HUY")?.CategoryId;

                var status = listStatus.FirstOrDefault(x => x.CategoryId == inventoryReceivingVoucher.StatusId);
                
                //Không được nhân bản phiếu nhập kho có trạng thái hủy
                if (inventoryReceivingVoucher.StatusId == statusId_HUY_PNK)
                {
                    return new NhanBanPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Không thể nhân bản phiếu nhập kho có trạng thái " + status.CategoryName
                    };
                }

                #region Lấy mã phiếu nhập kho

                var Code = "";
                var datenow = DateTime.Now;
                string year = datenow.Year.ToString().Substring(datenow.Year.ToString().Length - 2, 2);
                string month = datenow.Month < 10 ? "0" + datenow.Month.ToString() : datenow.Month.ToString();
                string day = datenow.Day < 10 ? "0" + datenow.Day.ToString() : datenow.Day.ToString();

                var listCodeToDay = context.InventoryReceivingVoucher.Where(c =>
                    Convert.ToDateTime(c.CreatedDate).Day == datenow.Day &&
                    Convert.ToDateTime(c.CreatedDate).Month == datenow.Month &&
                    Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Select(y => new
                {
                    InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode
                }).ToList();

                if (listCodeToDay.Count == 0)
                {

                    Code = "PN-" + year + month + day + "0001";
                }
                else
                {
                    var listNumber = new List<int>();
                    listCodeToDay.ForEach(item =>
                    {
                        var stringNumber = item.InventoryReceivingVoucherCode.Substring(9);
                        var number = Int32.Parse(stringNumber);
                        listNumber.Add(number);
                    });

                    var maxNumber = listNumber.OrderByDescending(x => x).FirstOrDefault();
                    var newNumber = maxNumber + 1;

                    if (newNumber > 9999)
                    {
                        Code = "PN-" + year + month + day + newNumber;
                    }
                    else
                    {
                        Code = "PN-" + year + month + day + newNumber.ToString("D4");
                    }
                }

                //Kiểm tra mã nếu trùng thì báo lỗi
                var existCode =
                    context.InventoryReceivingVoucher.FirstOrDefault(x => x.InventoryReceivingVoucherCode == Code);

                if (existCode != null)
                {
                    return new NhanBanPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Nhân bản thất bại, mã phiếu nhập kho mới đã tồn tại"
                    };
                }

                #endregion

                var newPhieuNhapKho = new InventoryReceivingVoucher();
                newPhieuNhapKho.InventoryReceivingVoucherId = Guid.NewGuid();
                newPhieuNhapKho.InventoryReceivingVoucherCode = Code;
                newPhieuNhapKho.StatusId = statusId_NHAP_PNK.Value;
                newPhieuNhapKho.InventoryReceivingVoucherType = inventoryReceivingVoucher.InventoryReceivingVoucherType;
                newPhieuNhapKho.WarehouseId = inventoryReceivingVoucher.WarehouseId;
                newPhieuNhapKho.ShiperName = inventoryReceivingVoucher.ShiperName;
                newPhieuNhapKho.Storekeeper = inventoryReceivingVoucher.Storekeeper;
                newPhieuNhapKho.InventoryReceivingVoucherDate = inventoryReceivingVoucher.InventoryReceivingVoucherDate;
                newPhieuNhapKho.InventoryReceivingVoucherTime = inventoryReceivingVoucher.InventoryReceivingVoucherTime;
                newPhieuNhapKho.LicenseNumber = inventoryReceivingVoucher.LicenseNumber;
                newPhieuNhapKho.Active = inventoryReceivingVoucher.Active;
                newPhieuNhapKho.CreatedDate = DateTime.Now;
                newPhieuNhapKho.CreatedById = parameter.UserId;
                newPhieuNhapKho.ExpectedDate = inventoryReceivingVoucher.ExpectedDate;
                newPhieuNhapKho.Description = inventoryReceivingVoucher.Description;
                newPhieuNhapKho.Note = inventoryReceivingVoucher.Note;
                newPhieuNhapKho.PartnersId = inventoryReceivingVoucher.PartnersId;

                context.InventoryReceivingVoucher.Add(newPhieuNhapKho);

                var listOldSanPham = context.InventoryReceivingVoucherMapping
                    .Where(x => x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).ToList();

                var listNewSanPham = new List<InventoryReceivingVoucherMapping>();
                listOldSanPham.ForEach(item =>
                {
                    var sanPham = new InventoryReceivingVoucherMapping();
                    sanPham.InventoryReceivingVoucherMappingId = Guid.NewGuid();
                    sanPham.InventoryReceivingVoucherId = newPhieuNhapKho.InventoryReceivingVoucherId;
                    sanPham.ObjectId = item.ObjectId;
                    sanPham.ProductId = item.ProductId;
                    sanPham.QuantityRequest = item.QuantityRequest;
                    sanPham.QuantityActual = 0;
                    sanPham.QuantitySerial = item.QuantitySerial;
                    sanPham.PriceProduct = item.PriceProduct;
                    sanPham.WarehouseId = item.WarehouseId;
                    sanPham.Description = item.Description;
                    sanPham.Active = true;
                    sanPham.CreatedDate = DateTime.Now;
                    sanPham.CreatedById = parameter.UserId;
                    sanPham.QuantityReservation = 0;
                    sanPham.PriceAverage = false;
                    sanPham.ObjectDetailId = item.ObjectDetailId;

                    listNewSanPham.Add(sanPham);
                });

                context.InventoryReceivingVoucherMapping.AddRange(listNewSanPham);

                #region Thêm vào Dòng thời gian

                var note = new Note();
                note.NoteId = Guid.NewGuid();
                note.Description = employee.EmployeeName.Trim() + " đã tạo phiếu nhập kho";
                note.Type = "NEW";
                note.ObjectId = newPhieuNhapKho.InventoryReceivingVoucherId;
                note.ObjectType = "PNK";
                note.Active = true;
                note.NoteTitle = "Đã tạo phiếu nhập kho";
                note.CreatedDate = DateTime.Now;
                note.CreatedById = parameter.UserId;

                context.Note.Add(note);

                #endregion

                context.SaveChanges();

                return new NhanBanPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    InventoryReceivingVoucherId = newPhieuNhapKho.InventoryReceivingVoucherId
                };
            }
            catch (Exception e)
            {
                return new NhanBanPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public XoaPhieuNhapKhoResult XoaPhieuNhapKho(XoaPhieuNhapKhoParameter parameter)
        {
            try
            {
                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                if (inventoryReceivingVoucher == null)
                {
                    return new XoaPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                    };
                }

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var status_NHAP_PNK = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "NHA" && x.CategoryTypeId == statusTypeId_PNK);

                if (inventoryReceivingVoucher.StatusId != status_NHAP_PNK?.CategoryId)
                {
                    return new XoaPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Không thể xóa phiếu nhập kho ở trạng thái khác " + status_NHAP_PNK?.CategoryName
                    };
                }

                context.InventoryReceivingVoucher.Remove(inventoryReceivingVoucher);

                var listSanPham = context.InventoryReceivingVoucherMapping
                    .Where(x => x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).ToList();

                context.InventoryReceivingVoucherMapping.RemoveRange(listSanPham);
                context.SaveChanges();

                return new XoaPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new XoaPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public HuyPhieuNhapKhoResult HuyPhieuNhapKho(HuyPhieuNhapKhoParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                if (inventoryReceivingVoucher == null)
                {
                    return new HuyPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                    };
                }

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatus_PNK = context.Category
                    .Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();

                var status_NHAP_PNK = listStatus_PNK
                    .FirstOrDefault(x => x.CategoryCode == "NHA");

                var status_HUY_PNK = listStatus_PNK
                    .FirstOrDefault(x => x.CategoryCode == "HUY");

                var status = listStatus_PNK.FirstOrDefault(x => x.CategoryId == inventoryReceivingVoucher.StatusId);

                //Không được hủy phiếu nhập kho ở trạng thái nháp
                if (inventoryReceivingVoucher.StatusId == status_NHAP_PNK?.CategoryId)
                {
                    return new HuyPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Không thể hủy Phiếu nhập kho ở trạng thái " + status?.CategoryName
                    };
                }

                #region Kiểm tra đã phát sinh phiếu xuất kho chưa?

                //Trạng thái của Phiếu xuất kho
                var statusPhieuXuatKhoType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPHX");
                var status_XuatKho_PXK = context.Category.FirstOrDefault(x =>
                    x.CategoryTypeId == statusPhieuXuatKhoType.CategoryTypeId && x.CategoryCode == "NHK");

                //Lấy list Phiếu xuất kho có trạng thái Đã xuất kho
                var listPhieuXuatKhoId = context.InventoryDeliveryVoucher
                    .Where(x => x.StatusId == status_XuatKho_PXK.CategoryId).Select(y => y.InventoryDeliveryVoucherId)
                    .ToList();

                //Lấy list ProductId của các phiếu xuất kho
                var listProductId = context.InventoryDeliveryVoucherMapping
                    .Where(x => listPhieuXuatKhoId.Contains(x.InventoryDeliveryVoucherId)).Select(y => y.ProductId)
                    .Distinct().ToList();

                //Nếu trong Phiếu xuất kho hiện tại có ít nhất 1 sản phẩm đã xuất kho thì không có Hủy phiếu
                var listCurrentProductId = context.InventoryReceivingVoucherMapping
                    .Where(x => x.InventoryReceivingVoucherId == inventoryReceivingVoucher.InventoryReceivingVoucherId)
                    .Select(y => y.ProductId).Distinct().ToList();

                var productExists = false;
                listCurrentProductId.ForEach(item =>
                {
                    var product = listProductId.FirstOrDefault(x => x == item);

                    if (product != Guid.Empty)
                    {
                        productExists = true;
                    }
                });

                if (productExists)
                {
                    return new HuyPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Không thể hủy phiếu vì tồn tại sản phẩm đã xuất kho"
                    };
                }

                #endregion

                #region Xóa trong Sổ kho

                var listSoKho = context.SoKho.Where(x =>
                    x.LoaiPhieu == 1 && x.PhieuId == inventoryReceivingVoucher.InventoryReceivingVoucherId).ToList();
                context.SoKho.RemoveRange(listSoKho);

                #endregion

                inventoryReceivingVoucher.StatusId = status_HUY_PNK.CategoryId;
                context.InventoryReceivingVoucher.Update(inventoryReceivingVoucher);

                #region Thêm vào Dòng thời gian

                var note = new Note();
                note.NoteId = Guid.NewGuid();
                note.Description = employee.EmployeeName.Trim() + " đã Hủy phiếu nhập kho";
                note.Type = "ADD";
                note.ObjectId = inventoryReceivingVoucher.InventoryReceivingVoucherId;
                note.ObjectType = "PNK";
                note.Active = true;
                note.NoteTitle = "Đã hủy phiếu nhập kho";
                note.CreatedDate = DateTime.Now;
                note.CreatedById = parameter.UserId;

                context.Note.Add(note);

                #endregion

                context.SaveChanges();

                return new HuyPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new HuyPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public KhongGiuPhanPhieuNhapKhoResult KhongGiuPhanPhieuNhapKho(KhongGiuPhanPhieuNhapKhoParameter parameter)
        {
            try
            {
                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatusPNK = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();
                var statusId_SanSang_PNK = listStatusPNK.FirstOrDefault(x => x.CategoryCode == "SAS")?.CategoryId;
                var statusId_Cho_PNK = listStatusPNK.FirstOrDefault(x => x.CategoryCode == "CHO")?.CategoryId;

                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                //Nếu phiếu nhập kho có trạng thái Sẵn sàng thì chuyển sang trạng thái Chờ nhập kho
                if (inventoryReceivingVoucher.StatusId == statusId_SanSang_PNK)
                {
                    inventoryReceivingVoucher.StatusId = statusId_Cho_PNK.Value;
                    context.InventoryReceivingVoucher.Update(inventoryReceivingVoucher);
                }
                else
                {
                    var listOldSanPham = context.InventoryReceivingVoucherMapping.Where(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId).ToList();

                    context.InventoryReceivingVoucherMapping.RemoveRange(listOldSanPham);

                    var listNewSanPham = new List<InventoryReceivingVoucherMapping>();
                    parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                    {
                        var newSanPham = new InventoryReceivingVoucherMapping();
                        newSanPham.InventoryReceivingVoucherMappingId = Guid.NewGuid();
                        newSanPham.InventoryReceivingVoucherId = parameter.InventoryReceivingVoucherId;
                        newSanPham.ObjectId = item.ObjectId;
                        newSanPham.ProductId = item.ProductId.Value;
                        newSanPham.QuantityRequest = item.QuantityRequest;
                        newSanPham.QuantityActual = item.QuantityActual;
                        newSanPham.QuantitySerial = 0;
                        newSanPham.PriceProduct = item.PriceProduct;
                        newSanPham.WarehouseId = item.WarehouseId.Value;
                        newSanPham.Description = item.Description;
                        newSanPham.Active = true;
                        newSanPham.CreatedDate = DateTime.Now;
                        newSanPham.CreatedById = parameter.UserId;
                        newSanPham.QuantityReservation = 0;
                        newSanPham.PriceAverage = item.PriceAverage;
                        newSanPham.ObjectDetailId = item.ObjectDetailId;

                        listNewSanPham.Add(newSanPham);
                    });

                    context.InventoryReceivingVoucherMapping.AddRange(listNewSanPham);
                }

                context.SaveChanges();

                return new KhongGiuPhanPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new KhongGiuPhanPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public KiemTraNhapKhoResult KiemTraNhapKho(KiemTraNhapKhoParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                /*
                 * mode = 1: Nhập kho thành công
                 * mode = 2: Nhập kho thành công và hiển thị popup cảnh báo
                 * mode = 3: Không nhập kho và hiển thị popup cảnh báo
                 */
                int mode = 0;
                var listMaSanPhamKhongHopLe = new List<string>();

                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                if (inventoryReceivingVoucher == null)
                {
                    return new KiemTraNhapKhoResult()
                    {
                        Status = false,
                        Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                    };
                }

                var systemType = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "HIM")
                    ?.SystemValueString;

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatusPNK = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();
                var statusId_DNK_PNK = listStatusPNK.FirstOrDefault(x => x.CategoryCode == "NHK")?.CategoryId;

                var statusTypeId_PXK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPHX")
                    ?.CategoryTypeId;
                var listStatusPXK = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PXK).ToList();
                var statusId_DXK_PXK = listStatusPXK.FirstOrDefault(x => x.CategoryCode == "NHK")?.CategoryId;

                #region Lọc ra các sản phẩm có Số lượng thực nhập = 0

                var listIdSanPhamKhongHopLe = parameter.ListSanPhamPhieuNhapKho.Where(x => x.QuantityActual == 0)
                    .Select(y => y.InventoryReceivingVoucherMappingId).ToList();

                parameter.ListSanPhamPhieuNhapKho =
                    parameter.ListSanPhamPhieuNhapKho.Where(x => x.QuantityActual > 0).ToList();

                #endregion

                #region Kiểm tra tồn kho tối đa của list sản phẩm

                var listAllInventoryReceivingVoucher = context.InventoryReceivingVoucher.ToList();
                var listAllInventoryReceivingVoucherMapping =
                    context.InventoryReceivingVoucherMapping.ToList();
                var listAllInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.ToList();
                var listAllInventoryDeliveryVoucherMapping =
                    context.InventoryDeliveryVoucherMapping.ToList();
                var listAllInventoryReport = context.InventoryReport.ToList();

                var checkTonKhoToiDa = false;
                var listSanPhamCoTonKhoToiDa = new List<Guid?>();
                parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                {
                    var tonKhoToiDa = listAllInventoryReport.FirstOrDefault(x =>
                        x.WarehouseId == item.WarehouseId && x.ProductId == item.ProductId);

                    if (tonKhoToiDa != null)
                    {
                        checkTonKhoToiDa = true;
                        listSanPhamCoTonKhoToiDa.Add(item.InventoryReceivingVoucherMappingId);
                    }
                });

                //Nếu chưa có sản phẩm nào khai báo tồn kho tối đa
                if (!checkTonKhoToiDa)
                {
                    mode = 1;
                }
                else
                {
                    var vuotQuaTonKhoToiDa = false;
                    parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                    {
                        var sanPham =
                            listSanPhamCoTonKhoToiDa.FirstOrDefault(x => x == item.InventoryReceivingVoucherMappingId);
                        
                        if (sanPham != null && sanPham != Guid.Empty)
                        {
                            //Số tồn kho thực tế của sản phẩm
                            var quantityInStock = GetSoTonKhoThucTe(item.WarehouseId.Value,
                                item.ProductId.Value,
                                statusId_DNK_PNK.Value, statusId_DXK_PXK.Value,
                                listAllInventoryReceivingVoucher,
                                listAllInventoryReceivingVoucherMapping,
                                listAllInventoryDeliveryVoucher,
                                listAllInventoryDeliveryVoucherMapping,
                                listAllInventoryReport);

                            var SoTonKhoToiDa = listAllInventoryReport.FirstOrDefault(x =>
                                x.WarehouseId == item.WarehouseId && x.ProductId == item.ProductId).QuantityMaximum;

                            if (quantityInStock + item.QuantityActual > SoTonKhoToiDa)
                            {
                                listMaSanPhamKhongHopLe.Add(item.ProductCode.Trim());
                                vuotQuaTonKhoToiDa = true;
                            }
                        }
                    });

                    //Nếu có sản phẩm vượt quá số lượng tồn kho tối đa
                    if (vuotQuaTonKhoToiDa)
                    {
                        if (systemType.Trim() == "1")
                        {
                            mode = 1;
                        }
                        else if (systemType.Trim() == "2")
                        {
                            mode = 2;
                        }
                        else if (systemType.Trim() == "3")
                        {
                            mode = 3;
                        }
                    }
                    //Nếu tất cả sản phẩm đều hợp lệ
                    else
                    {
                        mode = 1;
                    }
                }

                if (mode == 1 || mode == 2)
                {
                    //Chuyển trạng thái phiếu nhập kho => Đã nhập kho
                    inventoryReceivingVoucher.StatusId = statusId_DNK_PNK.Value;
                    context.InventoryReceivingVoucher.Update(inventoryReceivingVoucher);

                    //Xóa các sản phẩm có Số lượng thực nhập = 0;
                    var listSanPhamKhongHopLe = context.InventoryReceivingVoucherMapping
                        .Where(x => listIdSanPhamKhongHopLe.Contains(x.InventoryReceivingVoucherMappingId)).ToList();
                    context.InventoryReceivingVoucherMapping.RemoveRange(listSanPhamKhongHopLe);

                    #region Thêm vào Sổ kho

                    var listSoKho = new List<SoKho>();
                    parameter.ListSanPhamPhieuNhapKho.ForEach(item =>
                    {
                        var soKho = new SoKho();
                        soKho.SoKhoId = Guid.NewGuid();
                        soKho.LoaiPhieu = 1;
                        soKho.PhieuId = item.InventoryReceivingVoucherId.Value;
                        soKho.ChiTietPhieuId = item.InventoryReceivingVoucherMappingId.Value;
                        soKho.ChiTietLoaiPhieu = inventoryReceivingVoucher.InventoryReceivingVoucherType;
                        soKho.NgayChungTu = inventoryReceivingVoucher.InventoryReceivingVoucherDate;
                        soKho.SoChungTu = inventoryReceivingVoucher.InventoryReceivingVoucherCode;
                        soKho.ProductId = item.ProductId.Value;
                        soKho.SoLuong = item.QuantityActual;
                        soKho.Gia = item.PriceProduct;
                        soKho.ThanhTien = item.QuantityActual * item.PriceProduct;
                        soKho.DoiTac = inventoryReceivingVoucher.PartnersId;
                        soKho.WarehouseId = item.WarehouseId.Value;
                        soKho.CheckGia = item.PriceAverage;
                        soKho.CreatedDate = DateTime.Now;
                        soKho.CreatedById = parameter.UserId;

                        listSoKho.Add(soKho);
                    });

                    context.SoKho.AddRange(listSoKho);

                    #endregion

                    #region Thêm vào Dòng thời gian

                    var note = new Note();
                    note.NoteId = Guid.NewGuid();
                    note.Description = employee.EmployeeName.Trim() + " đã Nhập kho cho phiếu nhập kho";
                    note.Type = "ADD";
                    note.ObjectId = inventoryReceivingVoucher.InventoryReceivingVoucherId;
                    note.ObjectType = "PNK";
                    note.Active = true;
                    note.NoteTitle = "Đã nhập kho";
                    note.CreatedDate = DateTime.Now;
                    note.CreatedById = parameter.UserId;

                    context.Note.Add(note);

                    #endregion

                    context.SaveChanges();
                }

                #endregion

                return new KiemTraNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    Mode = mode,
                    ListMaSanPhamKhongHopLe = listMaSanPhamKhongHopLe
                };
            }
            catch (Exception e)
            {
                return new KiemTraNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DatVeNhapPhieuNhapKhoResult DatVeNhapPhieuNhapKho(DatVeNhapPhieuNhapKhoParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var inventoryReceivingVoucher = context.InventoryReceivingVoucher.FirstOrDefault(x =>
                    x.InventoryReceivingVoucherId == parameter.InventoryReceivingVoucherId);

                if (inventoryReceivingVoucher == null)
                {
                    return new DatVeNhapPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Phiếu nhập kho không tồn tại trên hệ thống"
                    };
                }

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                var listStatusPNK = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();
                var statusId_HUY_PNK = listStatusPNK.FirstOrDefault(x => x.CategoryCode == "HUY")?.CategoryId;
                var statusId_NHAP_PNK = listStatusPNK.FirstOrDefault(x => x.CategoryCode == "NHA")?.CategoryId;

                //Chỉ được đặt về nháp phiếu nhập kho có trạng thái Hủy
                if (inventoryReceivingVoucher.StatusId != statusId_HUY_PNK)
                {
                    var status = listStatusPNK.FirstOrDefault(x => x.CategoryId == inventoryReceivingVoucher.StatusId);

                    return new DatVeNhapPhieuNhapKhoResult()
                    {
                        Status = false,
                        Message = "Không thể đặt về nháp Phiếu nhập kho có trạng thái " + status.CategoryName
                    };
                }

                //Đổi trạng thái phiếu nhập kho -> Nháp
                inventoryReceivingVoucher.StatusId = statusId_NHAP_PNK.Value;
                context.InventoryReceivingVoucher.Update(inventoryReceivingVoucher);

                #region Thêm vào Dòng thời gian

                var note = new Note();
                note.NoteId = Guid.NewGuid();
                note.Description = employee.EmployeeName.Trim() + " đã chuyển trạng thái phiếu nhập kho sang Nháp";
                note.Type = "ADD";
                note.ObjectId = inventoryReceivingVoucher.InventoryReceivingVoucherId;
                note.ObjectType = "PNK";
                note.Active = true;
                note.NoteTitle = "Đã đặt về nháp phiếu nhập kho";
                note.CreatedDate = DateTime.Now;
                note.CreatedById = parameter.UserId;

                context.Note.Add(note);

                #endregion

                context.SaveChanges();

                return new DatVeNhapPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new DatVeNhapPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetListProductPhieuNhapKhoResult GetListProductPhieuNhapKho(GetListProductPhieuNhapKhoParameter parameter)
        {
            try
            {
                var listProduct = new List<ProductEntityModel>();

                listProduct = context.Product.Where(x => x.Active == true).Select(y => new ProductEntityModel
                {
                    ProductId = y.ProductId,
                    ProductName = y.ProductName,
                    ProductCode = y.ProductCode,
                    ProductCodeName = y.ProductCode.Trim() + " - " + y.ProductName.Trim(),
                    ProductUnitId = y.ProductUnitId,
                    ProductUnitName = ""
                }).OrderBy(z => z.ProductName).ToList();

                //Đơn vị tính
                var unitProductType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH");
                var listUnitProduct =
                    context.Category.Where(x => x.CategoryTypeId == unitProductType.CategoryTypeId).ToList();

                listProduct.ForEach(item =>
                {
                    var unit = listUnitProduct.FirstOrDefault(x => x.CategoryId == item.ProductUnitId);

                    if (unit != null)
                    {
                        item.ProductUnitName = unit.CategoryName ?? "";
                    }
                });

                return new GetListProductPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    ListProduct = listProduct
                };
            }
            catch (Exception e)
            {
                return new GetListProductPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMasterDataListPhieuNhapKhoResult GetMasterDataListPhieuNhapKho(GetMasterDataListPhieuNhapKhoParameter parameter)
        {
            try
            {
                var listStatus = new List<CategoryEntityModel>();
                var listWarehouse = new List<WareHouseEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listProduct = new List<ProductEntityModel>();

                #region Lấy list status

                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                    ?.CategoryTypeId;
                listStatus = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).Select(y => new CategoryEntityModel
                {
                    CategoryId = y.CategoryId,
                    CategoryCode = y.CategoryCode,
                    CategoryName = y.CategoryName
                }).ToList();

                #endregion

                #region Lấy list kho 

                var listAllWarehouse = context.Warehouse.Where(x => x.Active).ToList();

                var listWarehouseTemporary = new List<WarehouseTemporaryModel>();
                listAllWarehouse.ForEach(item =>
                {
                    var newItem = new WarehouseTemporaryModel();
                    newItem.WarehouseId = item.WarehouseId;
                    newItem.WarehouseParent = item.WarehouseParent;
                    newItem.WarehouseName = item.WarehouseName;
                    newItem.ListWarehouseParent = new List<Guid>();
                    listWarehouseTemporary.Add(newItem);
                });

                listWarehouseTemporary.ForEach(item =>
                {
                    if (item.WarehouseParent != null)
                    {
                        item.ListWarehouseParent =
                            getWarehouseParentWarehouse(listAllWarehouse, item.WarehouseParent.Value, new List<Guid>());
                    }
                    else
                    {
                        item.FirstParentId = item.WarehouseId;
                    }
                });

                listWarehouseTemporary.ForEach(item =>
                {
                    if (item.ListWarehouseParent.Count > 0)
                    {
                        var count = item.ListWarehouseParent.Count;

                        item.FirstParentId = item.ListWarehouseParent[count - 1];

                        var listName = new List<string>();
                        item.ListWarehouseParent.ForEach(parentId =>
                        {
                            var name = listAllWarehouse.FirstOrDefault(x => x.WarehouseId == parentId).WarehouseName;
                            listName.Add(name);
                        });

                        listName.Reverse();

                        var oldName = item.WarehouseName;
                        var newName = string.Join(" > ", listName) + " > " + oldName;
                        item.WarehouseName = newName;
                    }
                });

                listWarehouseTemporary = listWarehouseTemporary.OrderBy(z => z.FirstParentId).ToList();

                listWarehouseTemporary.ForEach(item =>
                {
                    var newItem = new WareHouseEntityModel();
                    newItem.WarehouseId = item.WarehouseId;
                    newItem.WarehouseName = item.WarehouseName;
                    newItem.WarehouseParent = item.WarehouseParent;

                    listWarehouse.Add(newItem);
                });

                #endregion

                #region Lấy list người lập phiếu

                listEmployee = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeName = y.EmployeeName,
                    EmployeeCodeName = y.EmployeeCode.Trim() + " - " + y.EmployeeName.Trim()
                }).OrderBy(z => z.EmployeeName).ToList();

                #endregion

                #region Lấy list sản phẩm

                listProduct = context.Product.Where(x => x.Active == true).Select(y => new ProductEntityModel
                {
                    ProductId = y.ProductId,
                    ProductCode = y.ProductCode,
                    ProductName = y.ProductName,
                    ProductCodeName = y.ProductCode.Trim() + " - " + y.ProductName.Trim()
                }).OrderBy(z => z.ProductName).ToList();

                #endregion

                return new GetMasterDataListPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    ListStatus = listStatus,
                    ListWarehouse = listWarehouse,
                    ListEmployee = listEmployee,
                    ListProduct = listProduct
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataListPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public SearchListPhieuNhapKhoResult SearchListPhieuNhapKho(SearchListPhieuNhapKhoParameter parameter)
        {
            try
            {
                var listPhieuNhapKho = new List<InventoryReceivingVoucherEntityModel>();

                #region Convert parameter

                var maPhieu = parameter.MaPhieu == null ? null : parameter.MaPhieu.Trim();
                var listUserId = new List<Guid>();

                if (parameter.ListEmployeeId.Count > 0)
                {
                    listUserId = context.User.Where(x => x.EmployeeId != null &&
                                                         parameter.ListEmployeeId.Contains(x.EmployeeId.Value))
                        .Select(y => y.UserId).Distinct().ToList();
                }

                #endregion

                #region Các trường hợp Ngày nhập kho và Ngày lập phiếu

                if (parameter.FromNgayLapPhieu == null && parameter.ToNgayLapPhieu == null &&
                    parameter.FromNgayNhapKho == null && parameter.ToNgayNhapKho == null)
                {
                    listPhieuNhapKho = context.InventoryReceivingVoucher
                        .Where(x =>
                            (maPhieu == "" || maPhieu == null || x.InventoryReceivingVoucherCode.Contains(maPhieu)) &&
                            (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 ||
                             parameter.ListStatusId.Contains(x.StatusId)) &&
                            (parameter.ListWarehouseId == null || parameter.ListWarehouseId.Count == 0 ||
                             parameter.ListWarehouseId.Contains(x.WarehouseId)) &&
                            (listUserId.Count == 0 || listUserId.Contains(x.CreatedById)))
                        .Select(y => new InventoryReceivingVoucherEntityModel
                        {
                            InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                            InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode,
                            StatusId = y.StatusId,
                            InventoryReceivingVoucherType = y.InventoryReceivingVoucherType,
                            WarehouseId = y.WarehouseId,
                            InventoryReceivingVoucherDate = y.InventoryReceivingVoucherDate,
                            PartnersId = y.PartnersId,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate
                        }).ToList();
                }
                else if (parameter.FromNgayLapPhieu != null && parameter.ToNgayLapPhieu != null &&
                         parameter.FromNgayNhapKho != null && parameter.ToNgayNhapKho != null)
                {
                    listPhieuNhapKho = context.InventoryReceivingVoucher
                        .Where(x =>
                            (maPhieu == "" || maPhieu == null || x.InventoryReceivingVoucherCode.Contains(maPhieu)) &&
                            (x.CreatedDate.Date >= parameter.FromNgayLapPhieu.Value.Date &&
                             x.CreatedDate.Date <= parameter.ToNgayLapPhieu.Value.Date) &&
                            (x.InventoryReceivingVoucherDate.Date >= parameter.FromNgayNhapKho.Value.Date &&
                             x.InventoryReceivingVoucherDate.Date <= parameter.ToNgayNhapKho.Value.Date) &&
                            (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 ||
                             parameter.ListStatusId.Contains(x.StatusId)) &&
                            (parameter.ListWarehouseId == null || parameter.ListWarehouseId.Count == 0 ||
                             parameter.ListWarehouseId.Contains(x.WarehouseId)) &&
                            (listUserId.Count == 0 || listUserId.Contains(x.CreatedById)))
                        .Select(y => new InventoryReceivingVoucherEntityModel
                        {
                            InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                            InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode,
                            StatusId = y.StatusId,
                            InventoryReceivingVoucherType = y.InventoryReceivingVoucherType,
                            WarehouseId = y.WarehouseId,
                            InventoryReceivingVoucherDate = y.InventoryReceivingVoucherDate,
                            PartnersId = y.PartnersId,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate
                        }).ToList();
                }
                else if (parameter.FromNgayLapPhieu == null && parameter.ToNgayLapPhieu != null &&
                         parameter.FromNgayNhapKho == null && parameter.ToNgayNhapKho != null)
                {
                    DateTime? NgayLapPhieu = parameter.ToNgayLapPhieu;
                    DateTime? NgayNhapKho = parameter.ToNgayNhapKho;

                    listPhieuNhapKho = context.InventoryReceivingVoucher
                        .Where(x =>
                            (maPhieu == "" || maPhieu == null || x.InventoryReceivingVoucherCode.Contains(maPhieu)) &&
                            (NgayLapPhieu == DateTime.MinValue || x.CreatedDate.Date == NgayLapPhieu.Value.Date) &&
                            (NgayNhapKho == DateTime.MinValue ||
                             x.InventoryReceivingVoucherDate.Date == NgayNhapKho.Value.Date) &&
                            (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 ||
                             parameter.ListStatusId.Contains(x.StatusId)) &&
                            (parameter.ListWarehouseId == null || parameter.ListWarehouseId.Count == 0 ||
                             parameter.ListWarehouseId.Contains(x.WarehouseId)) &&
                            (listUserId.Count == 0 || listUserId.Contains(x.CreatedById)))
                        .Select(y => new InventoryReceivingVoucherEntityModel
                        {
                            InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                            InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode,
                            StatusId = y.StatusId,
                            InventoryReceivingVoucherType = y.InventoryReceivingVoucherType,
                            WarehouseId = y.WarehouseId,
                            InventoryReceivingVoucherDate = y.InventoryReceivingVoucherDate,
                            PartnersId = y.PartnersId,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate
                        }).ToList();
                }
                else if (parameter.FromNgayLapPhieu == null && parameter.ToNgayLapPhieu != null &&
                         parameter.FromNgayNhapKho == null && parameter.ToNgayNhapKho == null)
                {
                    DateTime? NgayLapPhieu = parameter.ToNgayLapPhieu;

                    listPhieuNhapKho = context.InventoryReceivingVoucher
                        .Where(x =>
                            (maPhieu == "" || maPhieu == null || x.InventoryReceivingVoucherCode.Contains(maPhieu)) &&
                            (NgayLapPhieu == DateTime.MinValue || x.CreatedDate.Date == NgayLapPhieu.Value.Date) &&
                            (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 ||
                             parameter.ListStatusId.Contains(x.StatusId)) &&
                            (parameter.ListWarehouseId == null || parameter.ListWarehouseId.Count == 0 ||
                             parameter.ListWarehouseId.Contains(x.WarehouseId)) &&
                            (listUserId.Count == 0 || listUserId.Contains(x.CreatedById)))
                        .Select(y => new InventoryReceivingVoucherEntityModel
                        {
                            InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                            InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode,
                            StatusId = y.StatusId,
                            InventoryReceivingVoucherType = y.InventoryReceivingVoucherType,
                            WarehouseId = y.WarehouseId,
                            InventoryReceivingVoucherDate = y.InventoryReceivingVoucherDate,
                            PartnersId = y.PartnersId,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate
                        }).ToList();
                }
                else if (parameter.FromNgayLapPhieu == null && parameter.ToNgayLapPhieu == null &&
                         parameter.FromNgayNhapKho == null && parameter.ToNgayNhapKho != null)
                {
                    DateTime? NgayNhapKho = parameter.ToNgayNhapKho;

                    listPhieuNhapKho = context.InventoryReceivingVoucher
                        .Where(x =>
                            (maPhieu == "" || maPhieu == null || x.InventoryReceivingVoucherCode.Contains(maPhieu)) &&
                            (NgayNhapKho == DateTime.MinValue ||
                             x.InventoryReceivingVoucherDate.Date == NgayNhapKho.Value.Date) &&
                            (parameter.ListStatusId == null || parameter.ListStatusId.Count == 0 ||
                             parameter.ListStatusId.Contains(x.StatusId)) &&
                            (parameter.ListWarehouseId == null || parameter.ListWarehouseId.Count == 0 ||
                             parameter.ListWarehouseId.Contains(x.WarehouseId)) &&
                            (listUserId.Count == 0 || listUserId.Contains(x.CreatedById)))
                        .Select(y => new InventoryReceivingVoucherEntityModel
                        {
                            InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                            InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode,
                            StatusId = y.StatusId,
                            InventoryReceivingVoucherType = y.InventoryReceivingVoucherType,
                            WarehouseId = y.WarehouseId,
                            InventoryReceivingVoucherDate = y.InventoryReceivingVoucherDate,
                            PartnersId = y.PartnersId,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate
                        }).ToList();
                }

                #endregion

                //Nếu có điều kiện tìm kiếm theo sản phẩm
                if (parameter.ListProductId != null && parameter.ListProductId.Count > 0)
                {
                    var listPhieuNhapKhoId = listPhieuNhapKho.Select(y => y.InventoryReceivingVoucherId).ToList();

                    //Lấy list Id phiếu nhập kho thỏa mãn
                    var listResultId = context.InventoryReceivingVoucherMapping
                        .Where(x => listPhieuNhapKhoId.Contains(x.InventoryReceivingVoucherId) &&
                                    parameter.ListProductId.Contains(x.ProductId))
                        .Select(y => y.InventoryReceivingVoucherId).Distinct().ToList();

                    listPhieuNhapKho = listPhieuNhapKho.Where(x => listResultId.Contains(x.InventoryReceivingVoucherId.Value))
                        .ToList();
                }

                if (listPhieuNhapKho.Count > 0)
                {
                    var listDoiTacId = listPhieuNhapKho.Select(y => y.PartnersId).Distinct().ToList();
                    var listDoiTacNhaCungCap = context.Vendor.Where(x => listDoiTacId.Contains(x.VendorId)).ToList();
                    var listDoiTacKhachHang = context.Customer.Where(x => listDoiTacId.Contains(x.CustomerId)).ToList();

                    var listCreateUserId = listPhieuNhapKho.Select(y => y.CreatedById).Distinct().ToList();
                    var listCreateUser = context.User.Where(x => listCreateUserId.Contains(x.UserId)).Distinct()
                        .ToList();
                    var listCreateEmployeeId = listCreateUser.Where(x => listCreateUserId.Contains(x.UserId))
                        .Select(y => y.EmployeeId).Distinct().ToList();
                    var listCreateEmployee = context.Employee.Where(x => listCreateEmployeeId.Contains(x.EmployeeId))
                        .ToList();

                    var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                        ?.CategoryTypeId;
                    var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusTypeId_PNK).ToList();

                    listPhieuNhapKho.ForEach(item =>
                    {
                        #region Lấy tên Đối tác

                        if (item.InventoryReceivingVoucherType == 2)
                        {
                            var doitac = listDoiTacKhachHang.FirstOrDefault(x => x.CustomerId == item.PartnersId);
                            item.PartnersName = doitac == null ? "" : doitac.CustomerName.Trim();
                        }
                        else
                        {
                            var doitac = listDoiTacNhaCungCap.FirstOrDefault(x => x.VendorId == item.PartnersId);
                            item.PartnersName = doitac == null ? "" : doitac.VendorName.Trim();
                        }

                        #endregion

                        #region Lấy tên loại phiếu

                        if (item.InventoryReceivingVoucherType == 1)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập theo phiếu mua hàng";
                        }
                        else if (item.InventoryReceivingVoucherType == 2)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập hàng bán bị trả lại";
                        }
                        else if (item.InventoryReceivingVoucherType == 3)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập kiểm kê";
                        }
                        else if (item.InventoryReceivingVoucherType == 4)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập điều chuyển";
                        }
                        else if (item.InventoryReceivingVoucherType == 5)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập khác";
                        }

                        #endregion

                        #region Lấy tên người lập phiếu

                        var employeeIdCreate = listCreateUser.FirstOrDefault(x => x.UserId == item.CreatedById)
                            ?.EmployeeId;
                        var employeeCreate = listCreateEmployee.FirstOrDefault(x => x.EmployeeId == employeeIdCreate);

                        item.CreatedName = employeeCreate?.EmployeeName.Trim();

                        #endregion

                        #region Lấy tên trạng thái

                        var status = listAllStatus.FirstOrDefault(x => x.CategoryId == item.StatusId);
                        item.StatusName = status?.CategoryName;

                        #endregion
                    });

                    listPhieuNhapKho = listPhieuNhapKho.OrderByDescending(z => z.CreatedDate).ToList();
                }

                return new SearchListPhieuNhapKhoResult()
                {
                    Status = true,
                    Message = "Success",
                    ListPhieuNhapKho = listPhieuNhapKho
                };
            }
            catch (Exception e)
            {
                return new SearchListPhieuNhapKhoResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private List<Guid> getWarehouseParentWarehouse(List<Warehouse> listAllWarehouse, Guid WarehouseParent, List<Guid> listResult)
        {
            var parent = listAllWarehouse.FirstOrDefault(x => x.WarehouseId == WarehouseParent);

            if (parent != null)
            {
                listResult.Add(parent.WarehouseId);

                if (parent.WarehouseParent != null)
                {
                    getWarehouseParentWarehouse(listAllWarehouse, parent.WarehouseParent.Value, listResult);
                }
            }

            return listResult;
        }

        /*Model để convert tên các kho con*/
        public class WarehouseTemporaryModel
        {
            public Guid WarehouseId { get; set; }
            public Guid? WarehouseParent { get; set; }
            public string WarehouseName { get; set; }
            public List<Guid> ListWarehouseParent { get; set; } //List các id kho cha
            public Guid? FirstParentId { get; set; } //Dùng để group các kho con theo kho cha gốc
        }

        private List<Guid?> getOrganizationChildrenId(List<Organization> organizationList, Guid? id, List<Guid?> list)
        {
            var organizations = organizationList.Where(o => o.ParentId == id).ToList();
            organizations.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(organizationList, item.OrganizationId, list);
            });

            return list;
        }

        private List<Guid?> getListWarehouseChildrenId(List<Warehouse> ListWarehouse, Guid? id, List<Guid?> list)
        {
            var _listWarehouse = ListWarehouse.Where(o => o.WarehouseParent == id).ToList();
            _listWarehouse.ForEach(item =>
            {
                list.Add(item.WarehouseId);
                getListWarehouseChildrenId(ListWarehouse, item.WarehouseId, list);
            });

            return list;
        }

        private List<ProductCategoryEntityModel> GetListChildProductCategory(List<ProductCategoryEntityModel> listAll,
            List<ProductCategoryEntityModel> listParent, List<ProductCategoryEntityModel> result)
        {
            listParent.ForEach(item =>
            {
                var hasValue = result.FirstOrDefault(x => x.ProductCategoryId == item.ProductCategoryId);
                if (hasValue == null)
                {
                    result.Add(item);
                }

                var listChild = listAll.Where(x => x.ParentId == item.ProductCategoryId).ToList();

                listChild.ForEach(child =>
                {
                    var hasChildValue = result.FirstOrDefault(x => x.ProductCategoryId == child.ProductCategoryId);
                    if (hasChildValue == null)
                    {
                        result.Add(child);
                    }
                    GetListChildProductCategory(listAll, listChild, result);
                });
            });

            return result;
        }

        private List<Guid> GetListWarehouseChild(List<Warehouse> listWarehouse, Guid warehouseId, List<Guid> listResult)
        {
            var listWarehouseChild = listWarehouse.Where(x => x.WarehouseParent == warehouseId).ToList();

            listWarehouseChild.ForEach(item =>
            {
                var hasChild = listWarehouse.FirstOrDefault(x => x.WarehouseParent == item.WarehouseId);

                if (hasChild == null)
                {
                    var hasValue = listResult.FirstOrDefault(x => x == item.WarehouseId);

                    if (hasValue == Guid.Empty)
                    {
                        listResult.Add(item.WarehouseId);
                    }
                }
                else
                {
                    GetListWarehouseChild(listWarehouse, item.WarehouseId, listResult);
                }
            });

            return listResult;
        }

        private List<InStockEntityModel> GetListResult(List<Guid> ListWarehouseId, DateTime FromDate,
            List<Product> ListAllProduct, Guid? ProductCategoryId, string ProductNameCode, List<Category> ListDVT,
            List<InventoryReport> ListAllInventoryReport)
        {
            var listResult = new List<InStockEntityModel>();

            #region Lấy tất cả phiếu nhập kho theo điều kiện
            /*
             * - Kho được chọn (WarehouseId)
             * - Có trạng thái Nhập kho
             * - Có thời gian nhập kho bé hơn thời gian được chọn (fromDate)
             */
            var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")
                ?.CategoryTypeId;
            var statusId_PNK = context.Category
                .FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PNK)
                ?.CategoryId;
            var listInventoryReceivingVoucher = context.InventoryReceivingVoucher
                .Where(x => x.Active && x.StatusId == statusId_PNK && x.InventoryReceivingVoucherDate != null &&
                            x.InventoryReceivingVoucherDate.Date <= FromDate.Date).ToList();

            #region Lấy tất cả sản phẩm đã nhập kho theo các phiếu nhập kho bên trên

            var listInventoryReceivingVoucherId = listInventoryReceivingVoucher
                .Select(y => y.InventoryReceivingVoucherId).ToList();

            var listProductReceivingVoucher = context.InventoryReceivingVoucherMapping
                .Where(x => ListWarehouseId.Contains(x.WarehouseId) &&
                            listInventoryReceivingVoucherId.Contains(x.InventoryReceivingVoucherId)).ToList();

            #endregion

            #endregion

            #region Lấy tất cả phiếu xuất kho theo điều kiện
            /*
             * - Kho được chọn (WarehouseId)
             * - Có trạng thái Xuất kho
             * - Có thời gian xuất kho bé hơn thời gian được chọn (fromDate)
             */
            var statusTypeId_PXK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPHX")
                ?.CategoryTypeId;
            var statusId_PXK = context.Category
                .FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PXK)
                ?.CategoryId;
            var listInventoryDeliveryVoucher = context.InventoryDeliveryVoucher
                .Where(x => x.Active && x.StatusId == statusId_PXK &&
                            (x.InventoryDeliveryVoucherDate != null &&
                             x.InventoryDeliveryVoucherDate.Value.Date <= FromDate.Date)).ToList();

            #region Lấy tất cả sản phẩm đã xuất kho theo các phiếu xuất kho bên trên

            var listInventoryDeliveryVoucherId = listInventoryDeliveryVoucher
                .Select(y => y.InventoryDeliveryVoucherId).ToList();

            var listProductDeliveryVoucher = context.InventoryDeliveryVoucherMapping
                .Where(x => ListWarehouseId.Contains(x.WarehouseId) &&
                            listInventoryDeliveryVoucherId.Contains(x.InventoryDeliveryVoucherId)).ToList();

            #endregion

            #endregion

            //Lấy list danh sách sản phẩm dịch vụ
            var listProduct = ListAllProduct.Where(x =>
                    (ProductCategoryId == null ||
                     x.ProductCategoryId == ProductCategoryId) &&
                    (ProductNameCode.Trim() == "" || ProductNameCode == null ||
                     x.ProductName.Contains(ProductNameCode.Trim()) ||
                     x.ProductCode.Contains(ProductNameCode.Trim())))
                .ToList();

            var listProductId = listProduct.Select(y => y.ProductId).ToList();

            #region Lọc ra các sản cần lấy trong list sản phẩm Nhập kho 

            listProductReceivingVoucher = listProductReceivingVoucher
                .Where(x => listProductId.Contains(x.ProductId)).ToList();

            #endregion

            #region Lọc ra các sản cần lấy trong list sản phẩm Xuất kho

            listProductDeliveryVoucher = listProductDeliveryVoucher
                .Where(x => listProductId.Contains(x.ProductId)).ToList();

            #endregion

            if (listProduct.Count > 0)
            {
                listProduct.ForEach(_prod =>
                {
                    var item = new InStockEntityModel();
                    item.ProductId = _prod.ProductId;
                    item.ProductCode = _prod.ProductCode?.Trim();
                    item.ProductName = _prod.ProductName?.Trim();

                    var dvt = ListDVT.FirstOrDefault(x => x.CategoryId == _prod.ProductUnitId)
                        ?.CategoryName;

                    item.ProductUnitName = dvt ?? "";
                    item.QuantityInStock = 0;
                    item.QuantityInStockMaximum = 0;
                    item.ProductPrice = 0;
                    item.WareHouseId = Guid.Empty;
                    item.WareHouseName = "";
                    item.lstSerial = new List<Serial>();

                    #region Số tồn kho ban đầu

                    //Kiểm tra trong bảng InventoryReport
                    var quantityInitialReport = ListAllInventoryReport.Where(x =>
                            ListWarehouseId.Contains(x.WarehouseId) && x.ProductId == _prod.ProductId)
                        .Sum(y => y.StartQuantity);

                    var quantityInitial = quantityInitialReport != null
                        ? (quantityInitialReport ?? 0)
                        : 0;

                    #endregion

                    #region Số tồn kho tối đa

                    var quantityMaximumReport = ListAllInventoryReport.Where(x =>
                            ListWarehouseId.Contains(x.WarehouseId) && x.ProductId == _prod.ProductId)
                        .Sum(y => y.QuantityMaximum);

                    var quantityMaximum = quantityMaximumReport != null ? (quantityMaximumReport ?? 0) : 0;

                    #endregion

                    //Số lượng nhập kho của sản phẩm
                    decimal quantityReceivingInStock = listProductReceivingVoucher
                        .Where(x => x.ProductId == _prod.ProductId)
                        .Sum(y => y.QuantityActual);

                    //Số lượng xuất kho của sản phẩm
                    decimal quantityDeliveryInStock = listProductDeliveryVoucher
                        .Where(x => x.ProductId == _prod.ProductId)
                        .Sum(y => y.QuantityActual);

                    //Số tồn kho = Số tồn kho ban đầu + Số lượng nhập kho - Số lượng xuất kho
                    item.QuantityInStock =
                        quantityInitial + quantityReceivingInStock - quantityDeliveryInStock;

                    // Số tồn kho tối đa
                    item.QuantityInStockMaximum = quantityMaximum;

                    listResult.Add(item);
                });
            }

            return listResult;
        }

        private decimal GetSoTonKhoThucTe(Guid WarehouseId, Guid ProductId,
            Guid StatusPNKId, Guid StatusPXKId,
            List<InventoryReceivingVoucher> ListAllInventoryReceivingVoucher,
            List<InventoryReceivingVoucherMapping> ListAllInventoryReceivingVoucherMapping,
            List<InventoryDeliveryVoucher> ListAllInventoryDeliveryVoucher,
            List<InventoryDeliveryVoucherMapping> ListAllInventoryDeliveryVoucherMapping,
            List<InventoryReport> ListAllInventoryReport)
        {
            decimal result = 0;

            #region Lấy tất cả phiếu nhập kho theo điều kiện
            /*
             * - Kho được chọn (WarehouseId)
             * - Có trạng thái Nhập kho
             * - Có thời gian nhập kho bé hơn thời gian được chọn (fromDate)
             */

            var listInventoryReceivingVoucher = ListAllInventoryReceivingVoucher
                .Where(x => x.Active && x.StatusId == StatusPNKId).ToList();

            #region Lấy tất cả sản phẩm đã nhập kho theo các phiếu nhập kho bên trên

            var listInventoryReceivingVoucherId = listInventoryReceivingVoucher
                .Select(y => y.InventoryReceivingVoucherId).ToList();

            var listProductReceivingVoucher = ListAllInventoryReceivingVoucherMapping
                .Where(x => x.WarehouseId == WarehouseId &&
                            listInventoryReceivingVoucherId.Contains(x.InventoryReceivingVoucherId)).ToList();

            #endregion

            #endregion

            #region Lấy tất cả phiếu xuất kho theo điều kiện
            /*
             * - Kho được chọn (WarehouseId)
             * - Có trạng thái Xuất kho
             * - Có thời gian xuất kho bé hơn thời gian được chọn (fromDate)
             */

            var listInventoryDeliveryVoucher = ListAllInventoryDeliveryVoucher
                .Where(x => x.Active && x.StatusId == StatusPXKId && 
                            x.InventoryDeliveryVoucherDate != null).ToList();

            #region Lấy tất cả sản phẩm đã xuất kho theo các phiếu xuất kho bên trên

            var listInventoryDeliveryVoucherId = listInventoryDeliveryVoucher
                .Select(y => y.InventoryDeliveryVoucherId).ToList();

            var listProductDeliveryVoucher = ListAllInventoryDeliveryVoucherMapping
                .Where(x => x.WarehouseId == WarehouseId &&
                            listInventoryDeliveryVoucherId.Contains(x.InventoryDeliveryVoucherId)).ToList();

            #endregion

            #endregion

            #region Số tồn kho ban đầu

            //Kiểm tra trong bảng InventoryReport
            var quantityInitialReport = ListAllInventoryReport.Where(x =>
                    x.WarehouseId == WarehouseId && x.ProductId == ProductId)
                .Sum(y => y.StartQuantity);

            var quantityInitial = quantityInitialReport != null
                ? (quantityInitialReport ?? 0)
                : 0;

            #endregion

            #region Số tồn kho tối đa

            var quantityMaximumReport = ListAllInventoryReport.Where(x =>
                    x.WarehouseId == WarehouseId && x.ProductId == ProductId)
                .Sum(y => y.QuantityMaximum);

            var quantityMaximum = quantityMaximumReport != null ? (quantityMaximumReport ?? 0) : 0;

            #endregion

            #region Số tồn kho thực tế

            //Số lượng nhập kho của sản phẩm
            decimal quantityReceivingInStock = listProductReceivingVoucher
                .Where(x => x.ProductId == ProductId)
                .Sum(y => y.QuantityActual);

            //Số lượng xuất kho của sản phẩm
            decimal quantityDeliveryInStock = listProductDeliveryVoucher
                .Where(x => x.ProductId == ProductId)
                .Sum(y => y.QuantityActual);

            //Số tồn kho = Số tồn kho ban đầu + Số lượng nhập kho - Số lượng xuất kho
            result = quantityInitial + quantityReceivingInStock - quantityDeliveryInStock;

            #endregion

            return result;
        }
    }
}

