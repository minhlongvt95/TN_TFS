using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TN.TNM.Common;
using TN.TNM.Common.Helper;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;
using TN.TNM.DataAccess.Messages.Results.ProcurementRequest;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Document;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ProcurementRequestDAO : BaseDAO, IProcurementRequestDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ProcurementRequestDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// Tao hoa don dat hang
        /// </summary>
        /// <param name="ProcurementRequest">Thong tin hoa don dat hang</param>
        /// <param name="ProcurementRequestItemList">Danh sach san pham trong hoa don dat hang</param>
        /// <returns></returns>
        public CreateProcurementRequestResult CreateProcurementRequest(CreateProcurementRequestParameter parameter)
        {
            bool isValidParameterNumber = true;

            parameter.ProcurementRequestItemList.ForEach(item =>
            {
                if (item?.Quantity <= 0 || item?.UnitPrice < 0)
                {
                    isValidParameterNumber = false;
                }
            });

            if (!isValidParameterNumber)
            {
                return new CreateProcurementRequestResult()
                {
                    Status = false,
                    Message = CommonMessage.ProcurementRequest.ADD_FAIL
                };
            }

            #region Add Procurement Request
            var draftTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DDU").CategoryTypeId;
            var draft = context.Category.FirstOrDefault(f => f.CategoryTypeId == draftTypeId && f.CategoryCode == "DR").CategoryId;

            var numberOfPr = context.ProcurementRequest.Count();
            var code = "RQP" + DateTime.Now.ToString("yy") + (numberOfPr + 1).ToString("D5");

            var requestEmployeeId = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;

            var newProcurementRequst = new ProcurementRequest
            {
                ProcurementRequestId = Guid.NewGuid(),
                ProcurementCode = code,
                ProcurementContent = parameter.ProcurementRequest.ProcurementContent?.Trim() ?? "",
                RequestEmployeeId = requestEmployeeId,
                EmployeePhone = parameter.ProcurementRequest.EmployeePhone,
                Unit = parameter.ProcurementRequest.Unit,
                OrderId = parameter.ProcurementRequest.OrderId,
                ApproverId = parameter.ProcurementRequest.ApproverId,
                ApproverPostion = parameter.ProcurementRequest.ApproverPostion,
                Explain = parameter.ProcurementRequest.Explain,
                StatusId = draft,
                Active = true,
                CreatedById = parameter.UserId,
                CreatedDate = DateTime.Now,
                UpdatedById = null,
                UpdatedDate = null
            };
            #endregion

            context.ProcurementRequest.Add(newProcurementRequst);


            parameter.ProcurementRequestItemList.ForEach(item =>
            {
                item.ProcurementRequestItemId = Guid.NewGuid();
                item.ProcurementRequestId = newProcurementRequst.ProcurementRequestId;
                item.CreatedById = parameter.UserId;
                item.CreatedDate = DateTime.Now;
                item.UpdatedById = null;
                item.UpdatedDate = null;
                context.ProcurementRequestItem.Add(item);
            });

            context.SaveChanges();

            #region Save file attach to disk
            string rootFolder = _hostingEnvironment.WebRootPath + string.Format("\\ProcurementRequestUpload\\{0}", newProcurementRequst.ProcurementCode);
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
            List<Entities.Document> lstDocument = new List<Entities.Document>();
            if (parameter.FileList != null)
            {
                parameter.FileList.ForEach(item =>
                {
                    if (item.Length > 0)
                    {

                        var filePath = Path.Combine(rootFolder, item.FileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            item.CopyTo(fileStream);
                        }
                        var itemDocument = new Entities.Document();
                        itemDocument.Name = item.FileName;
                        itemDocument.ObjectId = newProcurementRequst.ProcurementRequestId;
                        itemDocument.Extension = Path.GetExtension(filePath);
                        itemDocument.Size = item.Length;
                        itemDocument.DocumentUrl = filePath;
                        itemDocument.Header = item.Headers.ToString();
                        itemDocument.ContentType = item.ContentType;
                        itemDocument.CreatedById = parameter.UserId;
                        itemDocument.CreatedDate = DateTime.Now;
                        itemDocument.Active = true;
                        lstDocument.Add(itemDocument);
                    }
                });
                context.Document.AddRange(lstDocument);
                context.SaveChanges();
            }
            #endregion

            #region Gửi thông báo

            NotificationHelper.AccessNotification(context, TypeModel.ProcurementRequest, "CRE", new ProcurementRequest(),
                newProcurementRequst, true);

            #endregion

            return new CreateProcurementRequestResult()
            {
                Status = true,
                Message = CommonMessage.ProcurementRequest.ADD_SUCCESS,
                ProcurementRequestId = newProcurementRequst.ProcurementRequestId
            };
        }

        public SearchProcurementRequestResult SearchProcurementRequest(SearchProcurementRequestParameter parameter)
        {
            var commonProcurementRequest = context.ProcurementRequest.ToList();
            List<ProcurementRequestEntityModel> listProcurementRequest = new List<ProcurementRequestEntityModel>();

            if (commonProcurementRequest.Count > 0)
            {
                var commonOrganization = context.Organization.ToList();
                var commonContact = context.Contact.ToList();

                #region Lấy list trạng thái của Phiếu đề xuất mua hàng

                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DDU").CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == categoryTypeId).ToList();

                #endregion

                #region Lấy current Employee

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var _employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                #endregion

                listProcurementRequest = context.ProcurementRequest
                    .Where(c => c.Active != false && (parameter.ProcurementRequestCode == "" || c.ProcurementCode.Contains(parameter.ProcurementRequestCode)) &&
                     (parameter.ListRequester == null || parameter.ListRequester.Count == 0 || parameter.ListRequester.Contains(c.RequestEmployeeId.Value)) &&
                     (parameter.OrganizationId == null || parameter.OrganizationId == Guid.Empty || c.Unit == parameter.OrganizationId) &&
                     (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue || parameter.FromDate <= c.CreatedDate) &&
                     (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue || parameter.ToDate >= c.CreatedDate) &&
                     (parameter.ListStatus == null || parameter.ListStatus.Count == 0 || parameter.ListStatus.Contains(c.StatusId)))
                     .Select(m => new ProcurementRequestEntityModel
                     {
                         ProcurementRequestId = m.ProcurementRequestId,
                         ProcurementCode = m.ProcurementCode,
                         ProcurementContent = m.ProcurementContent,
                         ApproverId = m.ApproverId,
                         ApproverName = "",
                         ApproverPostion = m.ApproverPostion,
                         CreatedById = m.CreatedById,
                         CreatedDate = m.CreatedDate,
                         EmployeePhone = m.EmployeePhone,
                         Explain = m.Explain,
                         Unit = m.Unit,
                         RequestEmployeeId = m.RequestEmployeeId,
                         UpdatedById = m.UpdatedById,
                         UpdatedDate = m.UpdatedDate,
                         StatusId = m.StatusId
                     })
                     .ToList();
                if (parameter.ListProduct.Count > 0)
                {
                    var listProReqId = context.ProcurementRequestItem.Where(i => i.ProductId != null && parameter.ListProduct.Contains(i.ProductId.Value)).Select(i => i.ProcurementRequestId).Distinct().ToList();
                    listProcurementRequest = listProcurementRequest.Where(p => listProReqId.Contains(p.ProcurementRequestId)).ToList();
                }

                listProcurementRequest.ForEach(item =>
                {
                    var approver = commonContact.FirstOrDefault(c => c.ObjectId == item.ApproverId);
                    item.ApproverName = approver == null ? "" : approver.FirstName + " " + approver.LastName;

                    var org = commonOrganization.FirstOrDefault(c => c.OrganizationId == item.Unit);
                    item.OrganizationName = org == null ? "" : org.OrganizationName;

                    var employee = commonContact.FirstOrDefault(c => c.ObjectId == item.RequestEmployeeId);
                    item.RequestEmployeeName = employee == null ? "" : employee.FirstName + " " + employee.LastName;

                    var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                    item.StatusName = status == null ? "" : status.CategoryName;

                    item.ProcurementContent = item.ProcurementContent ?? "";
                    item.ProcurementCode = item.ProcurementCode ?? "";

                });

                listProcurementRequest.ForEach(item =>
                {
                    if (item.StatusId != null && item.StatusId != Guid.Empty)
                    {
                        var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                        switch (status.CategoryCode)
                        {
                            case "Approved":
                                item.BackgroundColorForStatus = "#007aff";
                                break;
                            case "WaitForAp":
                                item.BackgroundColorForStatus = "#ffcc00";
                                break;
                            case "Rejected":
                                item.BackgroundColorForStatus = "#C9CAC2";
                                break;
                            case "Cancel":
                                item.BackgroundColorForStatus = "#BB0000";
                                break;
                            case "DR":
                                item.BackgroundColorForStatus = "#8dcdff";
                                break;
                            case "Close":
                                item.BackgroundColorForStatus = "#25a02f";
                                break;
                        }
                    }
                });

                if (listProcurementRequest.Count > 0)
                {
                    var commonProcurementRequestItem = context.ProcurementRequestItem.ToList();
                    var commonVendor = context.Vendor.ToList();

                    listProcurementRequest.ForEach(lpr =>
                    {
                        lpr.TotalMoney = GetTotalMoneyOfProcurementRequest(lpr.ProcurementRequestId, commonProcurementRequestItem);
                        var listItem = commonProcurementRequestItem.Where(i => i.ProcurementRequestId == lpr.ProcurementRequestId).ToList();
                        lpr.SumQuantity = listItem.Where(i => i.Quantity != null).Sum(i => i.Quantity.Value);
                        lpr.SumQuantityApproval = listItem.Where(i => i.QuantityApproval != null).Sum(i => i.QuantityApproval.Value);
                        var tmpItem = commonProcurementRequestItem.FirstOrDefault(item => item.ProcurementRequestId == lpr.ProcurementRequestId);
                        if (tmpItem != null && (tmpItem.VendorId != null && tmpItem.VendorId != Guid.Empty))
                        {
                            lpr.VendorName = commonVendor.FirstOrDefault(vendor => vendor.VendorId == tmpItem.VendorId).VendorName;
                        }
                        else
                        {
                            lpr.VendorName = "";
                        }
                    });
                }

                #region Comment by longhdh

                ////Nếu là quản lý
                //if (_employee.IsManager)
                //{
                //    //Lấy list phòng ban con của user
                //    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                //    if (_employee.OrganizationId != null)
                //    {
                //        listGetAllChild.Add(_employee.OrganizationId.Value);
                //        listGetAllChild = getOrganizationChildrenId(_employee.OrganizationId.Value, listGetAllChild);
                //    }
                //    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                //    var listEmployeeInChargeByManager = context.Employee.Where(x =>
                //                                            listGetAllChild.Contains(x.OrganizationId)).Select(y => y.EmployeeId).ToList();

                //    var listUserId = context.User.Where(x => listEmployeeInChargeByManager.Contains(x.EmployeeId.Value)).Select(y => y.UserId).ToList();

                //    listProcurementRequest = listProcurementRequest.Where(x => listUserId.Contains(x.CreatedById.Value)).ToList();
                //}
                ////Nếu là nhân viên thường
                //else
                //{
                //    listProcurementRequest = listProcurementRequest.Where(x => x.CreatedById == user.UserId).ToList();
                //}

                #endregion

                // lấy userRole của user hiện tại
                var userRole = context.UserRole.FirstOrDefault(x => x.UserId == parameter.UserId);
                // lấy list các permission thuộc role của user hiện tại
                var roleAndPermission = context.RoleAndPermission.Where(x => x.RoleId == userRole.RoleId).ToList();
                // list tất cả các permission
                var actionResource = context.ActionResource.ToList();
                var count = 0;
                List<ActionResource> actionResources = new List<ActionResource>();
                // lấy các hành động thuộc nhóm 'Đặt hàng nhà cung cấp'
                actionResource.ForEach(x =>
                {
                    if (x.ActionResource1 == "buy/vendor/create-order/add" || x.ActionResource1 == "buy/vendor/create-order/view")
                    {
                        actionResources.Add(x);
                    }
                });

                roleAndPermission.ForEach(item =>
                {
                    actionResources.ForEach(x =>
                    {
                        if (item.ActionResourceId == x.ActionResourceId)
                        {
                            count++;
                        }
                    });
                });

                // nếu user hiện tại có tất cả các quyền thuộc nhóm 'Đặt hàng nhà cung cấp'
                if (count == actionResources.Count)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    if (_employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(_employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(_employee.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    var listEmployeeInChargeByManager = context.Employee.Where(x =>
                                                            listGetAllChild.Contains(x.OrganizationId)).Select(y => y.EmployeeId).ToList();

                    var listUserId = context.User.Where(x => listEmployeeInChargeByManager.Contains(x.EmployeeId.Value)).Select(y => y.UserId).ToList();

                    listProcurementRequest = listProcurementRequest.Where(x => listUserId.Contains(x.CreatedById.Value)).ToList();
                }
                // nếu không có tất cả các quyền thuộc nhóm 'Đặt hàng nhà cung cấp'
                else
                {
                    listProcurementRequest = listProcurementRequest.Where(x => x.CreatedById == user.UserId).ToList();
                }

                listProcurementRequest = listProcurementRequest.OrderByDescending(c => c.CreatedDate).ToList();
            }
            return new SearchProcurementRequestResult()
            {
                ListProcurementRequest = listProcurementRequest ?? new List<ProcurementRequestEntityModel>(),
                Message = "Success",
                Status = true
            };
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

        public SearchProcurementRequestResult SearchProcurementRequestReport(SearchProcurementRequestParameter parameter)
        {
            var commonProcurementRequest = context.ProcurementRequest.ToList();
            List<ProcurementRequestEntityModel> listProcurementRequest = new List<ProcurementRequestEntityModel>();
            List<ProcurementRequestEntityModel> listProcurementRequestReport = new List<ProcurementRequestEntityModel>();

            if (commonProcurementRequest.Count > 0)
            {
                var commonUser = context.User.ToList();
                var commonEmployee = context.Employee.ToList();
                var commonPosition = context.Position.ToList();
                var commonOrganization = context.Organization.ToList();
                var commonCategory = context.Category.ToList();
                var commonContact = context.Contact.ToList();
                var commonProduct = context.Product.ToList();
                var commonVendorOrderProcurementRequestMapping = context.VendorOrderProcurementRequestMapping.ToList();
                var commonVendorOrder = context.VendorOrder.ToList();
                var commonVendorOrderDetail = context.VendorOrderDetail.ToList();

                var categoryTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DDU").CategoryTypeId;
                var listAllStatus = commonCategory.Where(c => c.CategoryTypeId == categoryTypeId).ToList();

                var statusDrId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;

                var currentEmpId = commonUser.FirstOrDefault(us => us.UserId == parameter.UserId).EmployeeId;
                var currentEmp = commonEmployee.FirstOrDefault(emp => emp.EmployeeId == currentEmpId);
                var currentEmpPosition = commonPosition.FirstOrDefault(p => p.PositionId == currentEmp.PositionId);
                var currentEmpOrg = commonOrganization.FirstOrDefault(org => org.OrganizationId == currentEmp.OrganizationId);

                var currentEmpCodeListLowLevel = new List<string>() { "GV", "CTV", "QL", "NV", "TG" };
                var currentEmpCodeListHightLevel = new List<string>() { "TP", "TGD", "PP" };

                var listProReqId = context.ProcurementRequestItem.Where(i =>
                    (parameter.ListProduct == null || parameter.ListProduct.Count == 0 || (i.ProductId != null && parameter.ListProduct.Contains(i.ProductId.Value))) &&
                    (parameter.ListVendor == null || parameter.ListVendor.Count == 0 || (i.VendorId != null && parameter.ListVendor.Contains(i.VendorId.Value))) &&
                    (parameter.ListBudget == null || parameter.ListBudget.Count == 0 || (i.ProcurementPlanId != null && parameter.ListBudget.Contains(i.ProcurementPlanId.Value))))
                    .Select(i => i.ProcurementRequestId).Distinct().ToList();

                listProcurementRequest = context.ProcurementRequest
                    .Where(c => c.Active != false && (parameter.ProcurementRequestCode == "" || c.ProcurementCode.Contains(parameter.ProcurementRequestCode)) &&
                     (parameter.ProcurementRequestContent == "" || c.ProcurementContent.Contains(parameter.ProcurementRequestContent)) &&
                     (listProReqId == null || listProReqId.Count == 0 || listProReqId.Contains(c.ProcurementRequestId)) &&
                     (parameter.ListRequester == null || parameter.ListRequester.Count == 0 || parameter.ListRequester.Contains(c.RequestEmployeeId.Value)) &&
                     (parameter.ListApproval == null || parameter.ListApproval.Count == 0 || (c.ApproverId != null && parameter.ListApproval.Contains(c.ApproverId.Value))) &&
                     (parameter.OrganizationId == null || parameter.OrganizationId == Guid.Empty || c.Unit == parameter.OrganizationId) &&
                     (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue || parameter.FromDate <= c.CreatedDate) &&
                     (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue || parameter.ToDate >= c.CreatedDate) &&
                     (parameter.ListStatus == null || parameter.ListStatus.Count == 0 || parameter.ListStatus.Contains(c.StatusId)) &&
                     (
                        //nếu nó được toàn quyền ~ ở phòng ban cao nhất
                        currentEmpOrg.Level == 0 ||
                       // Nếu CurrentUser(CR) là Nhân viên, trợ giảng , ... thì chỉ nhìn chính nó
                       ((c.RequestEmployeeId == currentEmpId || c.ApproverId == currentEmpId) && currentEmpCodeListLowLevel.Contains(currentEmpPosition.PositionCode)) ||
                       // nếu CR là trưởng phòng, phó phòng nhìn được tất cả các phiếu của nhân viên dưới nó
                       ((c.RequestEmployeeId == currentEmpId || c.ApproverId == currentEmpId || c.Unit == currentEmp.OrganizationId || ListChildOfOrg(currentEmp.OrganizationId.Value).Contains(c.Unit.Value) || c.Unit == currentEmp.OrganizationId || ListChildOfOrg(currentEmp.OrganizationId.Value).Contains(c.Unit.Value)) && currentEmpCodeListHightLevel.Contains(currentEmpPosition.PositionCode)))
                       &&
                       (c.CreatedById == parameter.UserId || c.StatusId != statusDrId))
                     .Select(m => new ProcurementRequestEntityModel
                     {
                         ProcurementRequestId = m.ProcurementRequestId,
                         ProcurementCode = m.ProcurementCode,
                         ProcurementContent = m.ProcurementContent,
                         ApproverId = m.ApproverId,
                         ApproverName = "",
                         ApproverPostion = m.ApproverPostion,
                         CreatedById = m.CreatedById,
                         CreatedDate = m.CreatedDate,
                         EmployeePhone = m.EmployeePhone,
                         Explain = m.Explain,
                         Unit = m.Unit,
                         RequestEmployeeId = m.RequestEmployeeId,
                         UpdatedById = m.UpdatedById,
                         UpdatedDate = m.UpdatedDate,
                         StatusId = m.StatusId
                     })
                     .ToList();

                listProcurementRequest.ForEach(item =>
                {
                    var approver = commonContact.FirstOrDefault(c => c.ObjectId == item.ApproverId);
                    item.ApproverName = approver == null ? "" : approver.FirstName + " " + approver.LastName;

                    var org = commonOrganization.FirstOrDefault(c => c.OrganizationId == item.Unit);
                    item.OrganizationName = org == null ? "" : org.OrganizationName;

                    var employee = commonContact.FirstOrDefault(c => c.ObjectId == item.RequestEmployeeId);
                    item.RequestEmployeeName = employee == null ? "" : employee.FirstName + " " + employee.LastName;

                    var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                    item.StatusName = status == null ? "" : status.CategoryName;

                    item.ProcurementContent = item.ProcurementContent ?? "";
                    item.ProcurementCode = item.ProcurementCode ?? "";

                });

                listProcurementRequest.ForEach(item =>
                {
                    if (item.StatusId != null && item.StatusId != Guid.Empty)
                    {
                        var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                        switch (status.CategoryCode)
                        {
                            case "Approved":
                                item.BackgroundColorForStatus = "#007aff";
                                break;
                            case "WaitForAp":
                                item.BackgroundColorForStatus = "#ffcc00";
                                break;
                            case "Rejected":
                                item.BackgroundColorForStatus = "#272909";
                                break;
                            case "Cancel":
                                item.BackgroundColorForStatus = "#BB0000";
                                break;
                            case "DR":
                                item.BackgroundColorForStatus = "#C9CAC2";
                                break;
                        }
                    }

                });

                listProcurementRequest = listProcurementRequest.OrderByDescending(c => c.CreatedDate).ToList();

                if (listProcurementRequest.Count > 0)
                {
                    var commonProcurementRequestItem = context.ProcurementRequestItem.ToList();
                    listProcurementRequest.ForEach(lpr =>
                    {
                        var listItem = commonProcurementRequestItem.Where(i => i.ProcurementRequestId == lpr.ProcurementRequestId).ToList();

                        var orderVendorIdList = commonVendorOrderProcurementRequestMapping.Where(m => m.ProcurementRequestId == lpr.ProcurementRequestId).Select(m => m.VendorOrderId).ToList();
                        var vendorOrderDetailObj = commonVendorOrderDetail.Where(d => orderVendorIdList.Contains(d.VendorOrderId)).ToList();

                        listItem.ForEach(item =>
                        {
                            ProcurementRequestEntityModel reportLine = new ProcurementRequestEntityModel();
                            var productObj = commonProduct.FirstOrDefault(c => c.ProductId == item.ProductId);
                            reportLine.ProcurementRequestId = lpr.ProcurementRequestId;
                            reportLine.ProcurementCode = lpr.ProcurementCode;
                            reportLine.ProcurementContent = lpr.ProcurementContent;
                            reportLine.Description = item.Description;
                            reportLine.ApproverId = lpr.ApproverId;
                            reportLine.ApproverName = lpr.ApproverName;
                            reportLine.ApproverPostion = lpr.ApproverPostion;
                            reportLine.CreatedById = lpr.CreatedById;
                            reportLine.CreatedDate = lpr.CreatedDate;
                            reportLine.EmployeePhone = lpr.EmployeePhone;
                            reportLine.Explain = lpr.Explain;
                            reportLine.Unit = lpr.Unit;
                            reportLine.RequestEmployeeId = lpr.RequestEmployeeId;
                            reportLine.UpdatedById = lpr.UpdatedById;
                            reportLine.UpdatedDate = lpr.UpdatedDate;
                            reportLine.StatusId = lpr.StatusId;
                            reportLine.OrganizationName = lpr.OrganizationName;
                            reportLine.RequestEmployeeName = lpr.RequestEmployeeName;
                            reportLine.StatusName = lpr.StatusName;
                            reportLine.BackgroundColorForStatus = lpr.BackgroundColorForStatus;

                            reportLine.ProcurementRequestItemId = item.ProcurementRequestItemId;
                            reportLine.STT = listProcurementRequestReport.Count + 1;
                            reportLine.ProductId = item.ProductId;
                            reportLine.VendorId = item.VendorId;
                            reportLine.BudgetId = item.ProcurementPlanId;
                            reportLine.ProductCode = productObj?.ProductCode;
                            reportLine.ProductName = productObj?.ProductName;
                            reportLine.UnitName = commonCategory.FirstOrDefault(c => c.CategoryId == productObj?.ProductUnitId)?.CategoryName;
                            reportLine.SumQuantity = item.Quantity;
                            reportLine.SumQuantityApproval = item.QuantityApproval;
                            reportLine.SumQuantityPO = vendorOrderDetailObj.Where(d => d.ProductId == item.ProductId).Sum(d => d.Quantity);
                            listProcurementRequestReport.Add(reportLine);
                        });
                    });
                    listProcurementRequestReport = listProcurementRequestReport.Where(c =>
                     (parameter.ListProduct == null || parameter.ListProduct.Count == 0 || (c.ProductId != null && parameter.ListProduct.Contains(c.ProductId.Value))) &&
                     (parameter.ListVendor == null || parameter.ListVendor.Count == 0 || (c.VendorId != null && parameter.ListVendor.Contains(c.VendorId.Value))) &&
                     (parameter.ListBudget == null || parameter.ListBudget.Count == 0 || (c.BudgetId != null && parameter.ListBudget.Contains(c.BudgetId.Value)))).ToList();
                }
            }
            return new SearchProcurementRequestResult()
            {
                ListProcurementRequest = listProcurementRequestReport ?? new List<ProcurementRequestEntityModel>(),
                Message = "Success",
                Status = true
            };
        }

        public SearchVendorProductPriceResult SearchVendorProductPrice(SearchVendorProductPriceParameter parameter)
        {
            var commonCategoryType = context.CategoryType.ToList();
            var commonCategory = context.Category.ToList();

            // Đơn vị tính
            var productUnitTypeId =
                commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DNH")?.CategoryTypeId ?? Guid.Empty;
            var listAllProductUnit = commonCategory.Where(c => c.CategoryTypeId == productUnitTypeId).ToList() ??
                                     new List<Category>();
            // Đơn vị tiền
            var moneyUnitTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DTI")?.CategoryTypeId ??
                                  Guid.Empty;
            var listAllMoneyUnit = commonCategory.Where(c => c.CategoryTypeId == moneyUnitTypeId).ToList() ??
                                   new List<Category>();

            var commonProduct = context.Product.Where(c => c.Active == true).ToList();
            var commonVendor = context.Vendor.Where(c => c.Active == true).ToList();
            var productId = commonProduct.FirstOrDefault(c => c.ProductId == parameter.ProductId)?.ProductId;

            var vendorId = commonVendor.FirstOrDefault(c => c.VendorId == parameter.VendorId)?.VendorId;

            var commonPriceSuggestedSupplierQuotesMapping = context.PriceSuggestedSupplierQuotesMapping.ToList();

            var listVendorProductPrice = new List<ProductVendorMappingEntityModel>();

            if (vendorId != null)
            {
                listVendorProductPrice = context.ProductVendorMapping.Where(c =>
                        c.Active == true && productId == c.ProductId &&
                        vendorId == c.VendorId)
                    .Select(m => new ProductVendorMappingEntityModel
                    {
                        ProductVendorMappingId = m.ProductVendorMappingId,
                        ProductId = m.ProductId,
                        VendorId = m.VendorId,
                        MoneyUnitId = m.UnitPriceId.Value,
                        MoneyUnitName = "",
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        Active = m.Active,
                        VendorProductName = m.VendorProductName,
                        VendorProductCode = m.VendorProductCode,
                        MiniumQuantity = m.MiniumQuantity,
                        Price = m.Price,
                        FromDate = m.FromDate,
                        ToDate = m.ToDate,
                        ProductName = "",
                        VendorName = commonVendor.FirstOrDefault(c => c.VendorId == m.VendorId).VendorName ?? "",
                        ProductCode = "",
                        ProductUnitName = "",
                        ListSuggestedSupplierQuoteId = new List<Guid?>()
                    }).OrderBy(c => c.Price).ToList();
            }
            else
            {
                listVendorProductPrice = context.ProductVendorMapping.Where(c =>
                        c.Active == true && productId == c.ProductId)
                    .Select(m => new ProductVendorMappingEntityModel
                    {
                        ProductVendorMappingId = m.ProductVendorMappingId,
                        ProductId = m.ProductId,
                        VendorId = m.VendorId,
                        MoneyUnitId = m.UnitPriceId.Value,
                        MoneyUnitName = "",
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        Active = m.Active,
                        VendorProductName = m.VendorProductName,
                        VendorProductCode = m.VendorProductCode,
                        MiniumQuantity = m.MiniumQuantity,
                        Price = m.Price,
                        FromDate = m.FromDate,
                        ToDate = m.ToDate,
                        ProductName = "",
                        VendorName = commonVendor.FirstOrDefault(c => c.VendorId == m.VendorId).VendorName ?? "",
                        ProductCode = "",
                        ProductUnitName = "",
                        ListSuggestedSupplierQuoteId = new List<Guid?>()
                    }).OrderBy(c => c.Price).ToList();
            }

            listVendorProductPrice.ForEach(item =>
            {
                var product = commonProduct.FirstOrDefault(c => c.ProductId == item.ProductId) ?? new Product();
                item.ProductName = product.ProductName;
                item.ProductCode = product.ProductCode;
                item.ProductUnitName = listAllProductUnit.FirstOrDefault(c => c.CategoryId == product.ProductUnitId)
                                           ?.CategoryName ?? "";
                item.MoneyUnitName =
                    listAllMoneyUnit.FirstOrDefault(c => c.CategoryId == item.MoneyUnitId)?.CategoryName ?? "";

                item.ListSuggestedSupplierQuoteId = commonPriceSuggestedSupplierQuotesMapping
                    .Where(x => x.ProductVendorMappingId == item.ProductVendorMappingId)
                    .Select(y => y.SuggestedSupplierQuoteId).ToList();
            });

            var today = DateTime.Now;
            var vendorProductPrice = listVendorProductPrice.FirstOrDefault(c => (c.MiniumQuantity <= parameter.Quantity) &&
                                                                                ((c.ToDate != null && c.FromDate <= today && today <= c.ToDate) ||
                                                                                 (c.ToDate == null && c.FromDate <= today)));

            return new SearchVendorProductPriceResult
            {
                Status = true,
                VendorProductPrice = vendorProductPrice,
            };
        }

        private List<Guid> ListChildOfOrg(Guid orgId)
        {
            //var orgParam = context.Organization.FirstOrDefault(org => org.OrganizationId == orgId);
            var _listOrgIdChild = context.Organization.Where(o => o.ParentId == orgId).Select(id => id.OrganizationId).ToList();
            var _tmpOrgId = new List<Guid>();
            _listOrgIdChild.ForEach(_orgId =>
            {
                _tmpOrgId.Add(_orgId);
                ListChildOfOrg(_orgId).ForEach(child =>
                {
                    _tmpOrgId.Add(child);
                });
            });
            return _tmpOrgId;
        }

        private decimal GetTotalMoneyOfProcurementRequest(Guid ProcurementRequestId, List<ProcurementRequestItem> commonProcurementRequestItem)
        {
            decimal totaltMoney = 0;
            var listItem = commonProcurementRequestItem.Where(it => it.ProcurementRequestId == ProcurementRequestId).ToList();
            if (listItem != null)
            {
                listItem.ForEach(item =>
                {
                    totaltMoney += (item.UnitPrice.HasValue ? item.UnitPrice.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0);
                });
            }
            return totaltMoney;
        }
        /// <summary>
        /// Lấy tất cả dự toán
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public GetAllProcurementPlanResult GetAllProcurementPlan(GetAllProcurementPlanParameter parameter)
        {
            var prPlanList = context.ProcurementPlan.ToList();
            return new GetAllProcurementPlanResult()
            {
                PRPlanList = prPlanList,
                Status = true
            };
        }
        /// Laasy ra du toan theo id de hien thi
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public GetProcurementRequestByIdResult GetProcurementRequestById(GetProcurementRequestByIdParameter parameter)
        {
            var commonProcurementRequestItem = context.ProcurementRequestItem.ToList();

            var procurementRequest = (from pr in context.ProcurementRequest
                                      join emp in context.Employee on pr.RequestEmployeeId equals emp.EmployeeId
                                      join ct in context.Contact on pr.RequestEmployeeId equals ct.ObjectId
                                      join org in context.Organization on pr.Unit equals org.OrganizationId
                                      join stt in context.Category on pr.StatusId equals stt.CategoryId
                                      join empap in context.Employee on pr.ApproverId equals empap.EmployeeId
                                      join ctap in context.Contact on pr.ApproverId equals ctap.ObjectId
                                      where pr.ProcurementRequestId == parameter.ProcurementRequestId
                                      select new ProcurementRequestEntityModel
                                      {
                                          ProcurementRequestId = pr.ProcurementRequestId,
                                          ProcurementCode = pr.ProcurementCode,
                                          ProcurementContent = pr.ProcurementContent,
                                          ApproverId = pr.ApproverId,
                                          ApproverName = ctap == null ? "" : empap.EmployeeCode + " - " + ctap.FirstName + " " + ctap.LastName,
                                          ApproverPostion = pr.ApproverPostion,
                                          CreatedById = pr.CreatedById,
                                          CreatedDate = pr.CreatedDate,
                                          EmployeePhone = pr.EmployeePhone,
                                          Explain = pr.Explain,
                                          Unit = pr.Unit,
                                          OrganizationName = org == null ? "" : org.OrganizationName,
                                          RequestEmployeeId = pr.RequestEmployeeId,
                                          RequestEmployeeName = ct == null ? "" : emp.EmployeeCode + " - " + ct.FirstName + " " + ct.LastName,
                                          StatusId = pr.StatusId,
                                          StatusName = stt == null ? "" : stt.CategoryName,
                                          UpdatedById = pr.UpdatedById,
                                          UpdatedDate = pr.UpdatedDate,
                                      }).FirstOrDefault();
            var listProcurementItem = new List<ProcurementRequestItemEntityModel>();
            if (procurementRequest != null)
            {
                listProcurementItem = (from item in commonProcurementRequestItem
                                       join plan in context.ProcurementPlan on item.ProcurementPlanId equals plan.ProcurementPlanId
                                       into item_plan
                                       from ip in item_plan.DefaultIfEmpty()
                                       join prd in context.Product on item.ProductId equals prd.ProductId
                                       join ven in context.Vendor on item.VendorId equals ven.VendorId
                                       join cate in context.Category on prd.ProductUnitId equals cate.CategoryId

                                       where item.ProcurementRequestId == procurementRequest.ProcurementRequestId
                                       select new ProcurementRequestItemEntityModel
                                       {
                                           ProcurementRequestItemId = item.ProcurementRequestItemId,
                                           ProcurementRequestId = item.ProcurementRequestId,
                                           ProcurementPlanId = item.ProcurementPlanId,
                                           ProcurementPlanCode = ip.ProcurementPlanCode != null ? ip.ProcurementPlanCode : "",
                                           CreatedById = item.CreatedById,
                                           CreatedDate = item.CreatedDate,
                                           ProductCode = prd != null ? prd.ProductCode : "",
                                           ProductId = item.ProductId,
                                           ProductName = prd != null ? prd.ProductName : "",
                                           Quantity = item.Quantity,
                                           UnitName = cate != null ? cate.CategoryName : "",
                                           Unit = cate != null ? cate.CategoryId : Guid.Empty,
                                           UnitPrice = item.UnitPrice,
                                           UpdatedById = item.UpdatedById,
                                           UpdatedDate = item.UpdatedDate,
                                           VendorName = ven != null ? ven.VendorName : "",
                                           VendorId = item.VendorId
                                       }).OrderByDescending(or => or.UnitPrice).ToList();

                procurementRequest.TotalMoney = GetTotalMoneyOfProcurementRequest(procurementRequest.ProcurementRequestId, commonProcurementRequestItem);
            }
            var listDocument = context.Document.Where(w => w.ObjectId == parameter.ProcurementRequestId).Select(s => new DocumentEntityModel(s)).ToList();

            var waitingforApproveId = context.Category.FirstOrDefault(c => c.CategoryCode == "WaitForAp").CategoryId;
            var approvedId = context.Category.FirstOrDefault(c => c.CategoryCode == "Approved").CategoryId;
            var rejectedId = context.Category.FirstOrDefault(c => c.CategoryCode == "Rejected").CategoryId;

            bool IsSendingApprove = procurementRequest.StatusId == waitingforApproveId;
            bool IsApprove = procurementRequest.StatusId == approvedId;
            bool IsReject = procurementRequest.StatusId == rejectedId;

            //Note
            var note = string.Empty;
            if (procurementRequest != null)
            {
                note = context.FeatureNote.FirstOrDefault(f => f.FeatureId == procurementRequest.ProcurementRequestId)?.Note;
            }

            return new GetProcurementRequestByIdResult()
            {
                Message = "Success",
                Status = true,
                ProcurementRequest = procurementRequest,
                ListProcurementItem = listProcurementItem,
                IsApprove = IsApprove,
                IsReject = IsReject,
                IsSendingApprove = IsSendingApprove,
                listDocument = listDocument,
                Notes = StringHelper.ConvertNoteToObject(note),
            };
        }
        public EditProcurementRequestResult EditProcurementRequest(EditProcurementRequestParameter parameter)
        {
            var procurementRequest = context.ProcurementRequest.FirstOrDefault(pr => pr.ProcurementRequestId == parameter.ProcurementRequest.ProcurementRequestId);
            procurementRequest.ProcurementCode = parameter.ProcurementRequest.ProcurementCode;
            procurementRequest.ProcurementContent = parameter.ProcurementRequest.ProcurementContent?.Trim();
            procurementRequest.RequestEmployeeId = parameter.ProcurementRequest.RequestEmployeeId;
            procurementRequest.Active = true;
            procurementRequest.Unit = parameter.ProcurementRequest.Unit;
            procurementRequest.UpdatedById = parameter.UserId;
            procurementRequest.UpdatedDate = DateTime.Now;
            procurementRequest.ApproverId = parameter.ProcurementRequest.ApproverId;
            procurementRequest.ApproverPostion = parameter.ProcurementRequest.ApproverPostion;
            procurementRequest.EmployeePhone = parameter.ProcurementRequest.EmployeePhone == null ? "" : parameter.ProcurementRequest.EmployeePhone?.Trim();
            procurementRequest.Explain = parameter.ProcurementRequest.Explain == null ? "" : parameter.ProcurementRequest.Explain?.Trim();
            //procurementRequest.StatusId = parameter.ProcurementRequest.StatusId;
            context.ProcurementRequest.Update(procurementRequest);

            #region Comment by Dung
            //var listItemToDelete = context.ProcurementRequestItem.Where(item => parameter.ListItemToDelete.Contains(item.ProcurementRequestItemId)).ToList();
            //listItemToDelete.ForEach(item =>
            //{
            //    context.ProcurementRequestItem.Remove(item);
            //});

            //var procurementItem = new ProcurementRequestItem();
            //parameter.ListProcurementRequestItem.ForEach(item =>
            //{
            //    if (item.ProcurementRequestItemId == null || item.ProcurementRequestItemId == Guid.Empty)
            //    {
            //        item.ProcurementRequestId = parameter.ProcurementRequest.ProcurementRequestId;
            //        context.ProcurementRequestItem.Add(item);
            //    }
            //    else
            //    {
            //        procurementItem = context.ProcurementRequestItem.FirstOrDefault(it => it.ProcurementRequestItemId == item.ProcurementRequestItemId);
            //        procurementItem.ProcurementPlanId = item.ProcurementPlanId;
            //        procurementItem.ProductId = item.ProductId;
            //        procurementItem.Quantity = item.Quantity;
            //        procurementItem.UnitPrice = item.UnitPrice;
            //        procurementItem.UpdatedById = parameter.UserId;
            //        procurementItem.UpdatedDate = DateTime.Now;
            //        procurementItem.VendorId = item.VendorId;

            //        context.ProcurementRequestItem.Update(procurementItem);
            //    }
            //});
            #endregion

            #region Add by Dung

            var listProcurementRequestItem = context.ProcurementRequestItem.Where(w => w.ProcurementRequestId == parameter.ProcurementRequest.ProcurementRequestId).ToList();

            //xóa procurement request cũ cần xóa
            var listItemId = parameter.ListProcurementRequestItem.Where(w => w.ProcurementRequestItemId != Guid.Empty).Select(w => w.ProcurementRequestItemId).ToList();
            var listDeleteItem = listProcurementRequestItem.Where(w => !listItemId.Contains(w.ProcurementRequestItemId)).ToList();
            context.ProcurementRequestItem.RemoveRange(listDeleteItem);

            parameter.ListProcurementRequestItem?.ForEach(item =>
            {
                if (item.ProcurementRequestItemId != Guid.Empty)
                {
                    //update item cũ
                    var oldItem = listProcurementRequestItem.FirstOrDefault(f => f.ProcurementRequestItemId == item.ProcurementRequestItemId);
                    if (oldItem != null)
                    {
                        oldItem.Description = item.Description;
                        oldItem.DiscountType = item.DiscountType;
                        oldItem.DiscountValue = item.DiscountValue;
                        oldItem.IncurredUnit = item.IncurredUnit;
                        oldItem.OrderDetailType = item.OrderDetailType;
                        oldItem.ProductId = item.ProductId;
                        oldItem.VendorId = item.VendorId;
                        oldItem.Quantity = item.Quantity;
                        oldItem.QuantityApproval = item.QuantityApproval;
                        oldItem.UnitPrice = item.UnitPrice;
                        oldItem.ProcurementRequestId = parameter.ProcurementRequest.ProcurementRequestId;
                        oldItem.ProcurementPlanId = item.ProcurementPlanId;
                        oldItem.OrderNumber = item.OrderNumber;
                        oldItem.CurrencyUnit = item.CurrencyUnit;
                        oldItem.ExchangeRate = item.ExchangeRate;
                        oldItem.UpdatedById = parameter.UserId;
                        oldItem.UpdatedDate = DateTime.Now;
                        context.ProcurementRequestItem.Update(oldItem);
                    }
                }
                else
                {
                    //tạo mới item

                    var newItem = new ProcurementRequestItem
                    {
                        ProcurementRequestItemId = Guid.NewGuid(),
                        Description = item.Description,
                        DiscountType = item.DiscountType,
                        DiscountValue = item.DiscountValue,
                        IncurredUnit = item.IncurredUnit,
                        OrderDetailType = item.OrderDetailType,
                        ProductId = item.ProductId,
                        VendorId = item.VendorId,
                        Quantity = item.Quantity,
                        QuantityApproval = item.QuantityApproval,
                        UnitPrice = item.UnitPrice,
                        ProcurementRequestId = parameter.ProcurementRequest.ProcurementRequestId,
                        ProcurementPlanId = item.ProcurementPlanId,
                        OrderNumber = item.OrderNumber,
                        CurrencyUnit = item.CurrencyUnit,
                        ExchangeRate = item.ExchangeRate,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                    };
                    context.ProcurementRequestItem.Add(newItem);
                }
            });
            #endregion

            #region Update Document
            var listDocument = context.Document.Where(w => w.ObjectId == parameter.ProcurementRequest.ProcurementRequestId).ToList();
            if (listDocument != null)
            {
                var listDeleteDocument = listDocument.Where(w => !parameter.ListDocumentId.Contains(w.DocumentId)).ToList();
                listDeleteDocument?.ForEach(document =>
                {
                    if (File.Exists(document.DocumentUrl))
                    {
                        File.Delete(document.DocumentUrl);
                    }
                });
                context.Document.RemoveRange(listDeleteDocument);
            }

            #endregion

            #region Update document attach
            // Add new File to directory
            string rootFolder = _hostingEnvironment.WebRootPath + string.Format("\\ProcurementRequestUpload\\{0}", parameter.ProcurementRequest.ProcurementCode);

            List<Entities.Document> lstDocument = new List<Entities.Document>();
            if (parameter.FileList != null)
            {
                parameter.FileList.ForEach(item =>
                {
                    if (item.Length > 0)
                    {
                        var filePath = Path.Combine(rootFolder, item.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            item.CopyTo(fileStream);
                        }
                        var itemDocument = new Entities.Document();
                        itemDocument.Name = item.FileName;
                        itemDocument.ObjectId = parameter.ProcurementRequest.ProcurementRequestId;
                        itemDocument.Extension = Path.GetExtension(filePath);
                        itemDocument.Size = item.Length;
                        itemDocument.DocumentUrl = filePath;
                        itemDocument.Header = item.Headers.ToString();
                        itemDocument.ContentType = item.ContentType;
                        itemDocument.CreatedById = parameter.UserId;
                        itemDocument.CreatedDate = DateTime.Now;
                        itemDocument.Active = true;
                        lstDocument.Add(itemDocument);
                    }
                });
                context.Document.AddRange(lstDocument);
            }
            context.SaveChanges();

            #endregion

            #region Lấy thông tin tài liệu đính kèm
            var listDocumentModel = new List<Models.Document.DocumentEntityModel>();
            var listDocumentEntity = context.Document.Where(w => w.Active == true && w.ObjectId == parameter.ProcurementRequest.ProcurementRequestId).ToList();
            listDocumentEntity.ForEach(document =>
            {
                listDocumentModel.Add(new DocumentEntityModel(document));
            });
            #endregion

            #region Gửi thông báo

            NotificationHelper.AccessNotification(context, TypeModel.ProcurementRequestDetail, "UPD", new ProcurementRequest(),
                procurementRequest, true);

            #endregion

            return new EditProcurementRequestResult()
            {
                ListDocumentEntityModel = listDocumentModel,
                Message = "Sửa đề xuất đặt hàng thành công",
                Status = true
            };
        }

        public GetDataCreateProcurementRequestResult GetDataCreateProcurementRequest(GetDataCreateProcurementRequestParameter parameter)
        {
            try
            {
                #region Kiểm tra quy trình hoạt động/không hoạt động
                var active = false;
                var prWorkFlow = context.WorkFlows.FirstOrDefault(f => f.WorkflowCode == "RQP");
                var status = context.Category.FirstOrDefault(f => f.CategoryId == prWorkFlow.StatusId);
                if (status.CategoryCode == "Enable")
                {
                    active = true;
                }

                if (active == false)
                {
                    return new GetDataCreateProcurementRequestResult()
                    {
                        Message = "Quy trình phê đặt hàng sản phẩm/dịch vụ ngừng hoạt động",
                        Status = false
                    };
                }
                #endregion

                #region Lấy thông tin nhân viên hiện tại đang đăng nhập
                var currentEmpId = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;
                var empEntity = context.Employee.FirstOrDefault(f => f.EmployeeId == currentEmpId);
                var empCusrent = new Models.Employee.EmployeeEntityModel(empEntity);
                empCusrent.PositionName = context.Position.FirstOrDefault(f => f.PositionId == empCusrent.PositionId)?.PositionName ?? "";
                empCusrent.OrganizationName = context.Organization.FirstOrDefault(f => f.OrganizationId == empCusrent.OrganizationId)?.OrganizationName ?? "";
                #endregion

                #region Lấy danh sách người phê duyệt theo quy trình
                var listApproverEmployeeId = new List<Models.Employee.EmployeeEntityModel>();
                //chỉ lấy danh sách người phê duyệt ở bước 2 của quy trình
                var workFlowStep_2 = context.WorkFlowSteps.Where(w => w.StepNumber == 2 && w.WorkflowId == prWorkFlow.WorkFlowId).FirstOrDefault();
                if (workFlowStep_2.ApprovebyPosition == false)
                {
                    //theo nhân viên
                    var employee = context.Employee.FirstOrDefault(f => f.Active == true && f.EmployeeId == workFlowStep_2.ApproverId);
                    if (employee != null)
                    {
                        var newEmp = new Models.Employee.EmployeeEntityModel();
                        newEmp.EmployeeId = employee.EmployeeId;
                        newEmp.EmployeeCode = employee.EmployeeCode;
                        newEmp.EmployeeName = empEntity.EmployeeName;
                        newEmp.PositionId = employee.PositionId;
                        newEmp.PositionName = context.Position.FirstOrDefault(f => f.PositionId == newEmp.PositionId)?.PositionName ?? "";
                        listApproverEmployeeId.Add(newEmp);
                    }
                }
                else
                {
                    //theo chức vụ
                    var listEmpByPosition = context.Employee.Where(w => w.Active == true && w.PositionId == workFlowStep_2.ApproverPositionId).ToList();
                    var listPostion = context.Position.ToList();
                    listEmpByPosition?.ForEach(emp =>
                    {
                        var newEmp = new Models.Employee.EmployeeEntityModel();
                        newEmp.EmployeeId = emp.EmployeeId;
                        newEmp.EmployeeCode = emp.EmployeeCode;
                        newEmp.EmployeeName = emp.EmployeeName;
                        newEmp.PositionId = emp.PositionId;
                        newEmp.PositionName = listPostion.FirstOrDefault(f => f.PositionId == newEmp.PositionId)?.PositionName ?? "";

                        listApproverEmployeeId.Add(newEmp);
                    });
                }
                #endregion

                #region Lấy thông tin đơn hàng bán
                var statusOrder = context.OrderStatus.FirstOrDefault(st => st.Active == true && st.OrderStatusCode == "DLV");
                var orderObj = context.CustomerOrder.Where(f => f.StatusId == statusOrder.OrderStatusId).ToList();

                var listAllVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();

                List<Guid> LstOrderId = orderObj.Select(c => c.OrderId).ToList();
                var orderDetailList = context.CustomerOrderDetail.Where(d => LstOrderId.Contains(d.OrderId))
                    .Select(c => new CustomerOrderDetailEntityModel
                    {
                        OrderDetailId = c.OrderDetailId,
                        VendorId = c.VendorId,
                        OrderId = c.OrderId,
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        UnitPrice = c.UnitPrice,
                        CurrencyUnit = c.CurrencyUnit,
                        ExchangeRate = c.ExchangeRate,
                        Vat = c.Vat,
                        DiscountType = c.DiscountType,
                        DiscountValue = c.DiscountValue,
                        Description = c.Description,
                        OrderDetailType = c.OrderDetailType,
                        UnitId = c.UnitId,
                        IncurredUnit = c.IncurredUnit,
                        Guarantee = c.Guarantee,
                        ExpirationDate = c.ExpirationDate,
                        Active = c.Active,
                        CreatedById = c.CreatedById,
                        CreatedDate = c.CreatedDate,
                        UpdatedById = c.UpdatedById,
                        UpdatedDate = c.UpdatedDate,
                        GuaranteeTime = c.GuaranteeTime,
                        GuaranteeDatetime = c.GuaranteeDatetime,
                        NameVendor = listAllVendor.FirstOrDefault(f => f.VendorId == c.VendorId) == null ? null : listAllVendor.FirstOrDefault(f => f.VendorId == c.VendorId).VendorName,
                        NameMoneyUnit = listCategory.FirstOrDefault(f => f.CategoryId == c.CurrencyUnit) == null ? null : listCategory.FirstOrDefault(f => f.CategoryId == c.CurrencyUnit).CategoryName,
                        //NameGene = c.NameGene,
                        NameProductUnit = listCategory.FirstOrDefault(f => f.CategoryId == c.UnitId) == null ? null : listCategory.FirstOrDefault(f => f.CategoryId == c.UnitId).CategoryName,
                        NameProduct = listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId) == null ? null : listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId).ProductName,
                        //SumAmount = c.SumAmount,
                        WarehouseId = c.WarehouseId,
                        PriceInitial = c.PriceInitial,
                        IsPriceInitial = c.IsPriceInitial,
                        WarrantyPeriod = c.WarrantyPeriod,
                        ActualInventory = c.ActualInventory,
                        BusinessInventory = c.BusinessInventory,
                        ProductName = c.ProductName,
                        ProductCode = listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId) == null ? null : listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId).ProductCode,
                    }).ToList();
                #endregion

                return new GetDataCreateProcurementRequestResult()
                {
                    IsWorkFlowInActive = active,
                    CurrentEmployeeModel = empCusrent,
                    ListApproverEmployeeId = listApproverEmployeeId.OrderBy(w => w.EmployeeName).ToList(),
                    ListOrder = orderObj,
                    ListOrderDetail = orderDetailList,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetDataCreateProcurementRequestResult()
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public GetDataCreateProcurementRequestItemResult GetDataCreateProcurementRequestItem(GetDataCreateProcurementRequestItemParameter parameter)
        {
            try
            {
                #region Lấy danh sách Product và category

                var unitTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var moneyTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DTI").CategoryTypeId;
                var listUnit = context.Category.Where(w => w.CategoryTypeId == unitTypeId).ToList();
                var listMoneyUnit = context.Category.Where(w => w.CategoryTypeId == moneyTypeId).Select(y => new CategoryEntityModel
                {
                    CategoryId = y.CategoryId,
                    CategoryCode = y.CategoryCode,
                    CategoryName = y.CategoryName,
                    IsDefault = y.IsDefauld
                }).ToList();
                var listProductEntity = context.Product.Where(w => w.Active == true).ToList() ?? new List<Product>();
                var listProduct = new List<Models.Product.ProductEntityModel>();
                listProductEntity.ForEach(_product =>
                {
                    var product = new Models.Product.ProductEntityModel();
                    product.ProductId = _product.ProductId;
                    product.ProductName = _product.ProductName;
                    product.ProductCode = _product.ProductCode;
                    product.Price1 = _product.Price1;
                    product.ProductMoneyUnitId = _product.ProductMoneyUnitId;
                    product.ProductUnitId = _product.ProductUnitId;
                    product.ProductUnitName = listUnit.FirstOrDefault(f => f.CategoryId == _product.ProductUnitId).CategoryName;
                    product.ProductCategoryId = _product.ProductCategoryId;

                    listProduct.Add(product);
                });

                #endregion

                #region Lấy danh sách Vendor
                var listProductId = listProduct.Select(w => w.ProductId).ToList();
                var listVendor = new List<Models.Vendor.VendorEntityModel>();
                var listProductVendorMapping = context.ProductVendorMapping.Where(w => listProductId.Contains(w.ProductId)).ToList();

                var lisVendorEntity = context.Vendor.Where(w => w.Active == true).ToList();

                lisVendorEntity.ForEach(_vendor =>
                {
                    var vendor = new Models.Vendor.VendorEntityModel();
                    vendor.VendorId = _vendor.VendorId;
                    vendor.VendorName = _vendor.VendorName;
                    vendor.VendorCode = _vendor.VendorCode;
                    vendor.ListProductId = listProductVendorMapping.Where(w => w.VendorId == _vendor.VendorId).Select(w => w.ProductId).ToList() ?? new List<Guid>();
                    listVendor.Add(vendor);
                });
                #endregion

                #region Lấy mã dự toán
                var listProcurementPlan = context.ProcurementPlan.ToList() ?? new List<ProcurementPlan>();
                #endregion

                return new GetDataCreateProcurementRequestItemResult()
                {
                    ListProduct = listProduct.OrderBy(w => w.ProductName).ToList(),
                    ListVendor = listVendor.OrderBy(w => w.VendorName).ToList(),
                    ListProcurementPlan = listProcurementPlan.OrderBy(w => w.ProcurementPlanCode).ToList(),
                    ListMoneyUnit = listMoneyUnit,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetDataCreateProcurementRequestItemResult()
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public GetDataEditProcurementRequestResult GetDataEditProcurementRequest(GetDataEditProcurementRequestParameter parameter)
        {
            try
            {
                var procurementRequestEntity =
                    context.ProcurementRequest.FirstOrDefault(f =>
                        f.ProcurementRequestId == parameter.ProcurementRequestId);

                #region Kiểm tra quy trình hoạt động/không hoạt động

                var active = false;
                var prWorkFlow = context.WorkFlows.FirstOrDefault(f => f.WorkflowCode == "RQP");
                var status = context.Category.FirstOrDefault(f => f.CategoryId == prWorkFlow.StatusId);
                if (status.CategoryCode == "Enable")
                {
                    active = true;
                }

                if (active == false)
                {
                    return new GetDataEditProcurementRequestResult()
                    {
                        Message = "Quy trình phê đặt hàng sản phẩm/dịch vụ ngừng hoạt động",
                        Status = false
                    };
                }

                #endregion

                var listPostion = context.Position.ToList();

                #region Lấy thông tin nhân viên hiện tại đang đăng nhập

                var currentEmpId = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;
                var empEntity = context.Employee.FirstOrDefault(f => f.EmployeeId == currentEmpId);

                #endregion

                #region Lấy danh sách người phê duyệt theo quy trình

                var listApproverEmployeeId = new List<Models.Employee.EmployeeEntityModel>();

                //chỉ lấy danh sách người phê duyệt ở bước 2 của quy trình
                var workFlowStep_2 = context.WorkFlowSteps
                    .Where(w => w.StepNumber == 2 && w.WorkflowId == prWorkFlow.WorkFlowId).FirstOrDefault();
                if (workFlowStep_2.ApprovebyPosition == false)
                {
                    //theo nhân viên
                    var employee = context.Employee.FirstOrDefault(f =>
                        f.Active == true && f.EmployeeId == workFlowStep_2.ApproverId);
                    if (employee != null)
                    {
                        var newEmp = new Models.Employee.EmployeeEntityModel();
                        newEmp.EmployeeId = employee.EmployeeId;
                        newEmp.EmployeeCode = employee.EmployeeCode;
                        newEmp.EmployeeName = empEntity.EmployeeName;
                        newEmp.PositionId = employee.PositionId;
                        newEmp.PositionName = context.Position.FirstOrDefault(f => f.PositionId == newEmp.PositionId)
                                                  ?.PositionName ?? "";
                        listApproverEmployeeId.Add(newEmp);
                    }
                }
                else
                {
                    //theo chức vụ: Lấy tất cả người có chức vụ đã cấu hình trong phòng ban của user

                    var organizationRequest = context.Employee
                        .FirstOrDefault(x => x.EmployeeId == procurementRequestEntity.RequestEmployeeId).OrganizationId;

                    var listEmpByPosition = context.Employee
                        .Where(w => w.Active == true && w.PositionId == workFlowStep_2.ApproverPositionId &&
                                    w.OrganizationId == organizationRequest).ToList();

                    listEmpByPosition?.ForEach(emp =>
                    {
                        var newEmp = new Models.Employee.EmployeeEntityModel();
                        newEmp.EmployeeId = emp.EmployeeId;
                        newEmp.EmployeeCode = emp.EmployeeCode;
                        newEmp.EmployeeName = emp.EmployeeName;
                        newEmp.PositionId = emp.PositionId;
                        newEmp.PositionName =
                            listPostion.FirstOrDefault(f => f.PositionId == newEmp.PositionId)?.PositionName ?? "";

                        listApproverEmployeeId.Add(newEmp);
                    });
                }

                var listEmailSendTo = new List<string>();
                var listContactEmp = context.Contact.Where(x => x.ObjectType == "EMP").ToList();
                listApproverEmployeeId.ForEach(emp =>
                {
                    var emp_contact = listContactEmp.FirstOrDefault(x => x.ObjectId == emp.EmployeeId);
                    if (emp_contact != null)
                    {
                        var email = emp_contact.Email;

                        if (!string.IsNullOrEmpty(email))
                        {
                            listEmailSendTo.Add(email.Trim());
                        }
                    }
                });

                #endregion

                #region Lấy thông tin đề xuất mua hàng và SP/DV đi kèm

                var statusName = context.Category.FirstOrDefault(f => f.CategoryId == procurementRequestEntity.StatusId)?.CategoryName ?? "";
                var statusCode = context.Category.FirstOrDefault(f => f.CategoryId == procurementRequestEntity.StatusId)?.CategoryCode ?? "";
                var requestEmp = context.Employee.FirstOrDefault(f => f.EmployeeId == procurementRequestEntity.RequestEmployeeId);
                var requestName = "";
                Guid? requestUnit = null;
                Guid? requestPosition = null;
                if (requestEmp != null)
                {
                    requestName = requestEmp?.EmployeeCode + " - " + requestEmp?.EmployeeName;
                    requestUnit = requestEmp.OrganizationId;
                    requestPosition = requestEmp.PositionId;
                }
                var approverPositionName = context.Position.FirstOrDefault(f => f.PositionId == procurementRequestEntity.ApproverPostion)?.PositionName ?? "";
                var organizationName = context.Organization.FirstOrDefault(f => f.OrganizationId == procurementRequestEntity.Unit)?.OrganizationName ?? "";

                var procurementRequest = new ProcurementRequestEntityModel
                {
                    ProcurementRequestId = procurementRequestEntity.ProcurementRequestId,
                    ProcurementCode = procurementRequestEntity.ProcurementCode,
                    ProcurementContent = procurementRequestEntity.ProcurementContent,
                    RequestEmployeeId = procurementRequestEntity.RequestEmployeeId,
                    RequestEmployeeName = requestName,
                    EmployeePhone = procurementRequestEntity.EmployeePhone,
                    Unit = procurementRequestEntity.Unit,
                    OrganizationName = organizationName,
                    ApproverId = procurementRequestEntity.ApproverId,
                    ApproverPostion = procurementRequestEntity.ApproverPostion,
                    ApproverPostionName = approverPositionName,
                    Explain = procurementRequestEntity.Explain,
                    StatusId = procurementRequestEntity.StatusId,
                    StatusName = statusName,
                    StatusCode = statusCode,
                    CreatedDate = procurementRequestEntity.CreatedDate,
                    OrderId = procurementRequestEntity.OrderId,
                    UpdatedById = procurementRequestEntity.UpdatedById,
                    UpdatedDate = procurementRequestEntity.UpdatedDate
                };

                if (procurementRequest.Unit == null || procurementRequest.Unit == Guid.Empty)
                {
                    procurementRequest.Unit = requestUnit;
                    procurementRequest.OrganizationName = context.Organization.FirstOrDefault(f => f.OrganizationId == procurementRequest.Unit)?.OrganizationName ?? "";
                }
                if (procurementRequest.ApproverPostion == null || procurementRequest.ApproverPostion == Guid.Empty)
                {
                    procurementRequest.ApproverPostion = requestPosition;
                    procurementRequest.ApproverPostionName = context.Position.FirstOrDefault(f => f.PositionId == procurementRequest.ApproverPostion)?.PositionName ?? "";
                }

                var listRequestItem = new List<Models.ProcurementRequest.ProcurementRequestItemEntityModel>();
                var listRequestItemEntity = context.ProcurementRequestItem.Where(w => w.ProcurementRequestId == parameter.ProcurementRequestId).OrderBy(x => x.OrderNumber).ToList();

                //lấy danh sách sản phẩm, danh sách nhà cung cấp, danh sách mã dự toán
                var listProductId = listRequestItemEntity.Select(w => w.ProductId).ToList();
                var listVendorId = listRequestItemEntity.Select(w => w.VendorId).ToList();
                var listProcurementPlanId = listRequestItemEntity.Select(w => w.ProcurementPlanId).ToList();

                var listProductVendor = context.ProductVendorMapping.OrderByDescending(x => x.CreatedDate).ToList();
                var listProduct = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();
                var listVendor = context.Vendor.Where(w => listVendorId.Contains(w.VendorId)).ToList();
                var listProcurementPlan = context.ProcurementPlan.Where(w => listProcurementPlanId.Contains(w.ProcurementPlanId)).ToList();

                var categoryTypeCodeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listCategory = context.Category.Where(w => w.CategoryTypeId == categoryTypeCodeId).ToList();

                var today = DateTime.Now;

                listRequestItemEntity?.ForEach(item =>
                {
                    var productVendor = listProductVendor.FirstOrDefault(f => (f.MiniumQuantity <= item.Quantity) &&
                                                                              (f.ProductId == item.ProductId) &&
                                                                              (f.VendorId == item.VendorId) &&
                                                                              ((f.ToDate != null && f.FromDate <= today && today <= f.ToDate) ||
                                                                               (f.ToDate == null && f.FromDate <= today)));
                    var productName = listProduct.FirstOrDefault(f => f.ProductId == item.ProductId)?.ProductName ?? "";
                    var productCode = listProduct.FirstOrDefault(f => f.ProductId == item.ProductId)?.ProductCode ?? "";
                    var vendorName = listVendor.FirstOrDefault(f => f.VendorId == item.VendorId)?.VendorName ?? "";
                    var productUnitId = listProduct.FirstOrDefault(f => f.ProductId == item.ProductId)?.ProductUnitId;
                    var unit = productUnitId ?? Guid.Parse("00000000-0000-0000-0000-000000000000");
                    var unitName = listCategory.FirstOrDefault(f => f.CategoryId == unit)?.CategoryName ?? "";
                    var procurementPlanCode = listProcurementPlan.FirstOrDefault(f => f.ProcurementPlanId == item.ProcurementPlanId)?.ProcurementPlanCode ?? "";

                    listRequestItem.Add(new ProcurementRequestItemEntityModel
                    {
                        ProcurementRequestItemId = item.ProcurementRequestItemId,
                        ProductId = item.ProductId,
                        ProductName = productName,
                        ProductCode = productCode,
                        ProductUnitId = unit,
                        ProductUnit = unitName,
                        VendorId = item.VendorId,
                        VendorName = vendorName,
                        Quantity = item.Quantity,
                        QuantityApproval = item.QuantityApproval,
                        UnitPrice = productVendor != null ? productVendor.Price : item.UnitPrice,
                        Unit = unit,
                        UnitName = unitName,
                        ProcurementRequestId = item.ProcurementRequestId,
                        ProcurementPlanId = item.ProcurementPlanId,
                        ProcurementPlanCode = procurementPlanCode,
                        CurrencyUnit = item.CurrencyUnit,
                        ExchangeRate = item.ExchangeRate,
                        Description = item.Description,
                        DiscountType = item.DiscountType,
                        DiscountValue = item.DiscountValue,
                        Vat = item.Vat,
                        OrderDetailType = item.OrderDetailType,
                        IncurredUnit = item.IncurredUnit,
                        OrderNumber = item.OrderNumber
                    });
                });

                #endregion

                #region Lấy thông tin tài liệu đính kèm
                var listDocumentModel = new List<Models.Document.DocumentEntityModel>();
                var listDocument = context.Document.Where(w => w.Active == true && w.ObjectId == parameter.ProcurementRequestId).ToList();
                listDocument.ForEach(document =>
                {
                    listDocumentModel.Add(new DocumentEntityModel(document));
                });
                #endregion

                #region Lấy thông tin đơn hàng bán
                var statusOrder = context.OrderStatus.FirstOrDefault(st => st.Active == true && st.OrderStatusCode == "DLV");
                var orderObj = context.CustomerOrder.Where(f => f.StatusId == statusOrder.OrderStatusId).ToList();
                var listAllVendor = context.Vendor.Where(x => x.Active == true).ToList();
                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();
                var listAllCategory = context.Category.Where(x => x.Active == true).ToList();

                List<Guid> LstOrderId = orderObj.Select(c => c.OrderId).ToList();
                var orderDetailList = context.CustomerOrderDetail.Where(d => LstOrderId.Contains(d.OrderId))
                    .Select(c => new CustomerOrderDetailEntityModel
                    {
                        OrderDetailId = c.OrderDetailId,
                        VendorId = c.VendorId,
                        OrderId = c.OrderId,
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        UnitPrice = c.UnitPrice,
                        CurrencyUnit = c.CurrencyUnit,
                        ExchangeRate = c.ExchangeRate,
                        Vat = c.Vat,
                        DiscountType = c.DiscountType,
                        DiscountValue = c.DiscountValue,
                        Description = c.Description,
                        OrderDetailType = c.OrderDetailType,
                        UnitId = c.UnitId,
                        IncurredUnit = c.IncurredUnit,
                        Guarantee = c.Guarantee,
                        ExpirationDate = c.ExpirationDate,
                        Active = c.Active,
                        CreatedById = c.CreatedById,
                        CreatedDate = c.CreatedDate,
                        UpdatedById = c.UpdatedById,
                        UpdatedDate = c.UpdatedDate,
                        GuaranteeTime = c.GuaranteeTime,
                        GuaranteeDatetime = c.GuaranteeDatetime,
                        NameVendor = listAllVendor.FirstOrDefault(f => f.VendorId == c.VendorId) == null ? null : listAllVendor.FirstOrDefault(f => f.VendorId == c.VendorId).VendorName,
                        NameMoneyUnit = listAllCategory.FirstOrDefault(f => f.CategoryId == c.CurrencyUnit) == null ? null : listAllCategory.FirstOrDefault(f => f.CategoryId == c.CurrencyUnit).CategoryName,
                        //NameGene = c.NameGene,
                        NameProductUnit = listAllCategory.FirstOrDefault(f => f.CategoryId == c.UnitId) == null ? null : listAllCategory.FirstOrDefault(f => f.CategoryId == c.UnitId).CategoryName,
                        NameProduct = listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId) == null ? null : listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId).ProductName,
                        //SumAmount = c.SumAmount,
                        WarehouseId = c.WarehouseId,
                        PriceInitial = c.PriceInitial,
                        IsPriceInitial = c.IsPriceInitial,
                        WarrantyPeriod = c.WarrantyPeriod,
                        ActualInventory = c.ActualInventory,
                        BusinessInventory = c.BusinessInventory,
                        ProductName = c.ProductName,
                        ProductCode = listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId) == null ? null : listAllProduct.FirstOrDefault(f => f.ProductId == c.ProductId).ProductCode,
                    }).ToList();
                #endregion

                #region Lấy list ghi chú
                var listNote = new List<NoteEntityModel>();

                listNote = context.Note
                    .Where(x => x.ObjectId == parameter.ProcurementRequestId && x.ObjectType == "PROCREQ" && x.Active == true).Select(
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

                return new GetDataEditProcurementRequestResult()
                {
                    IsWorkFlowInActive = active,
                    ListApproverEmployeeId = listApproverEmployeeId.OrderBy(w => w.EmployeeName).ToList(),
                    ProcurementRequestEntityModel = procurementRequest,
                    ListProcurementRequestItemEntityModel = listRequestItem,
                    ListDocumentModel = listDocumentModel,
                    ListOrder = orderObj,
                    ListOrderDetail = orderDetailList,
                    ListNote = listNote,
                    ListEmailSendTo = listEmailSendTo,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new GetDataEditProcurementRequestResult()
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }

        }

        public CreateProcurementRequestResult ApprovalOrReject(GetDataEditProcurementRequestParameter parameter)
        {
            try
            {
                // Lấy các trạng thái đề xuất mua hàng
                var categoryTypeObj =
                    context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "DDU");
                var categoryObj = context.Category
                    .Where(c => c.Active == true && c.CategoryTypeId == categoryTypeObj.CategoryTypeId).ToList();

                var objProcurementRequest =
                    context.ProcurementRequest.FirstOrDefault(p =>
                        p.ProcurementRequestId == parameter.ProcurementRequestId);
                var listProItem = context.ProcurementRequestItem.ToList();
                var userObj = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var employeeObj = context.Employee.FirstOrDefault(u => u.EmployeeId == userObj.EmployeeId);

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "PROCREQ";
                note.ObjectId = parameter.ProcurementRequestId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = CommonMessage.Note.NOTE_TITLE;

                if (parameter.IsAprroval == null)
                {
                    objProcurementRequest.StatusId =
                        categoryObj.FirstOrDefault(c => c.CategoryCode == "WaitForAp").CategoryId;
                    note.Description = CommonMessage.Note.NOTE_CONTENT_SEND_APPROVAL;

                    //Gửi email đề xuất phê duyệt
                    #region Gửi thông báo

                    var procurementRequst =
                        context.ProcurementRequest.FirstOrDefault(x =>
                            x.ProcurementRequestId == parameter.ProcurementRequestId);
                    procurementRequst.UpdatedDate = DateTime.Now;
                    procurementRequst.UpdatedById = parameter.UserId;
                    NotificationHelper.AccessNotification(context, TypeModel.ProcurementRequestDetail, "SEND_APPROVAL", new ProcurementRequest(),
                        procurementRequst, true);

                    #endregion
                }
                else
                {
                    if (parameter.IsAprroval == true)
                    {
                        objProcurementRequest.StatusId =
                            categoryObj.FirstOrDefault(c => c.CategoryCode == "Approved").CategoryId;
                        objProcurementRequest.ApproverId = employeeObj.EmployeeId;
                        objProcurementRequest.ApproverPostion = employeeObj.PositionId;
                        parameter.ListProcurementRequestItem.ForEach(item =>
                        {
                            var proReqItem = listProItem.FirstOrDefault(i =>
                                i.ProcurementRequestItemId == item.ProcurementRequestItemId);
                            proReqItem.QuantityApproval = item.QuantityApproval;
                            context.ProcurementRequestItem.Update(proReqItem);
                        });
                        if (parameter.Description == null || parameter.Description.Trim() == "")
                        {
                            note.Description = CommonMessage.Note.NOTE_CONTENT_APPROVAL_SUCCESS;
                        }
                        else
                        {
                            note.Description = CommonMessage.Note.NOTE_CONTENT_APPROVAL_SUCCESS + parameter.Description;
                        }
                    }
                    else
                    {
                        objProcurementRequest.StatusId =
                            categoryObj.FirstOrDefault(c => c.CategoryCode == "Rejected").CategoryId;
                        note.Description = employeeObj.EmployeeName + CommonMessage.Note.NOTE_CONTENT_REJECT +
                                           parameter.Description;
                    }
                }

                context.ProcurementRequest.Update(objProcurementRequest);
                context.Note.Add(note);
                context.SaveChanges();

                return new CreateProcurementRequestResult
                {
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new CreateProcurementRequestResult
                {
                    Message = e.ToString(),
                    Status = false
                };
            }
        }

        public CreateProcurementRequestResult ChangeStatus(GetDataEditProcurementRequestParameter parameter)
        {
            try
            {
                // Lấy các trạng thái đề xuất mua hàng
                var categoryTypeObj = context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "DDU");
                var categoryObj = context.Category.Where(c => c.Active == true && c.CategoryTypeId == categoryTypeObj.CategoryTypeId).ToList();

                var objProcurementRequest = context.ProcurementRequest.FirstOrDefault(p => p.ProcurementRequestId == parameter.ProcurementRequestId);

                var userObj = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var employeeObj = context.Employee.FirstOrDefault(u => u.EmployeeId == userObj.EmployeeId);

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "PROCREQ";
                note.ObjectId = parameter.ProcurementRequestId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = CommonMessage.Note.NOTE_TITLE;

                if (parameter.Description == "NEW")
                {
                    objProcurementRequest.StatusId = categoryObj.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;
                    note.Description = CommonMessage.Note.NOTE_CONTENT_EDIT_NEW;
                    context.Note.Add(note);
                }
                if (parameter.Description == "CANCEL")
                {
                    objProcurementRequest.StatusId = categoryObj.FirstOrDefault(c => c.CategoryCode == "Cancel").CategoryId;
                    note.Description = employeeObj.EmployeeCode + " - " + employeeObj.EmployeeName + CommonMessage.Note.NOTE_CONTENT_CANCEL;
                    context.Note.Add(note);
                }
                if (parameter.Description == "DELETE")
                {
                    objProcurementRequest.Active = false;
                    note.Description = CommonMessage.Note.NOTE_CONTENT_DELETE;
                }

                if (parameter.Description == "CANCEL_APPROVAL")
                {
                    objProcurementRequest.StatusId = categoryObj.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;
                    objProcurementRequest.UpdatedById = parameter.UserId;
                    objProcurementRequest.UpdatedDate = DateTime.Now;

                    note.Description = "Đề xuất đã được hủy yêu cầu phê duyệt bởi nhân viên: " + employeeObj.EmployeeCode + " - " + employeeObj.EmployeeName;
                    context.Note.Add(note);
                }

                context.ProcurementRequest.Update(objProcurementRequest);
                context.SaveChanges();

                #region gửi mail thông báo

                switch (parameter.Description)
                {
                    case "CANCEL_APPROVAL":
                        //var configEntity = context.SystemParameter.ToList();

                        //var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE").CategoryTypeId;

                        //var listEmailTempType =
                        //    context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

                        //var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "PRCA") // PROCUREMENT_REQUEST_CANCEL_APPROVAL
                        //    .CategoryId;

                        //var emailTemplate = context.EmailTemplate.FirstOrDefault(w => w.Active && w.EmailTemplateTypeId == emailCategoryId);

                        //#region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                        //var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                        //var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                        //#endregion

                        //#region Lấy danh sách email cần gửi thông báo

                        //var listEmailSendTo = new List<string>();

                        //#region Lấy email người phê duyệt

                        ////Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                        //var listManager = listAllEmployee.Where(x => x.IsManager)
                        //    .Select(y => y.EmployeeId).ToList();
                        //var listEmailManager = listAllContact
                        //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                        //    .Select(y => y.Email).ToList();

                        //listEmailManager.ForEach(emailManager =>
                        //{
                        //    if (!String.IsNullOrEmpty(emailManager))
                        //    {
                        //        listEmailSendTo.Add(emailManager.Trim());
                        //    }
                        //});

                        //#endregion

                        //#region Lấy email người tạo

                        //var employeeId =
                        //    context.User.FirstOrDefault(x => x.UserId == objProcurementRequest.CreatedById)
                        //        ?.EmployeeId;

                        //var email_created = "";

                        //if (employeeId != null)
                        //{
                        //    email_created = listAllContact.FirstOrDefault(x =>
                        //        x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                        //    if (!String.IsNullOrEmpty(email_created))
                        //    {
                        //        listEmailSendTo.Add(email_created.Trim());
                        //    }
                        //}

                        //#endregion

                        //#region Lấy email người phụ trách (Nhân viên bán hàng)

                        //var email_seller = listAllContact.FirstOrDefault(x =>
                        //    x.ObjectId == objProcurementRequest.RequestEmployeeId && x.ObjectType == "EMP")?.Email;

                        //if (!String.IsNullOrEmpty(email_seller))
                        //{
                        //    listEmailSendTo.Add(email_seller.Trim());
                        //}

                        //#endregion

                        //#region Lấy email người hủy phê duyệt

                        //var empId =
                        //    context.User.FirstOrDefault(x => x.UserId == objProcurementRequest.UpdatedById)
                        //        ?.EmployeeId;

                        //var email_cancel = "";

                        //if (employeeId != null)
                        //{
                        //    email_cancel = listAllContact.FirstOrDefault(x =>
                        //        x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                        //    if (!String.IsNullOrEmpty(email_cancel))
                        //    {
                        //        listEmailSendTo.Add(email_cancel.Trim());
                        //    }
                        //}

                        //#endregion

                        //listEmailSendTo = listEmailSendTo.Distinct().ToList();

                        //#endregion

                        //var subject = ReplaceTokenForContent(context, objProcurementRequest, emailTemplate.EmailTemplateTitle,
                        //    configEntity);
                        //var content = ReplaceTokenForContent(context, objProcurementRequest, emailTemplate.EmailTemplateContent,
                        //    configEntity);

                        //Emailer.SendEmail(context, listEmailSendTo, new List<string>(), subject, content);
                        NotificationHelper.AccessNotification(context, TypeModel.ProcurementRequestDetail, "CANCEL_APPROVAL", new ProcurementRequest(),
                            objProcurementRequest, true);
                        break;
                }

                #endregion

                return new CreateProcurementRequestResult
                {
                    Message = "Success",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new CreateProcurementRequestResult
                {
                    Message = e.ToString(),
                    Status = false
                };
            }
        }

        #region Phần hỗ trợ gửi mail

        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity)
        {
            var result = emailContent;

            #region Common Token

            const string logo = "[LOGO]";
            const string procurementCode = "[PROCUREMENT_CODE]";
            const string employeeName = "[EMPLOYEE_NAME]";
            const string employeeCode = "[EMPLOYEE_CODE]";
            const string updatedDate = "[UPDATED_DATE]";
            const string urlLogin = "[URL]";

            #endregion

            var _model = model as ProcurementRequest;

            #region Replace token

            #region replace logo

            if (result.Contains(logo))
            {
                var logoImg = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                if (!String.IsNullOrEmpty(logoImg))
                {
                    var temp_logo = "<img src=\"" + logoImg + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                    result = result.Replace(logoImg, temp_logo);
                }
                else
                {
                    result = result.Replace(logoImg, "");
                }
            }

            #endregion

            #region replace order code

            if (result.Contains(procurementCode) && _model.ProcurementCode != null)
            {
                result = result.Replace(procurementCode, _model.ProcurementCode.Trim());
            }

            #endregion

            #region replaca change employee code

            if (result.Contains(employeeCode))
            {
                var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                if (!String.IsNullOrEmpty(employeeCode))
                {
                    result = result.Replace(employeeCode, empCode);
                }
                else
                {
                    result = result.Replace(employeeCode, "");
                }
            }

            #endregion

            #region replace change employee name

            if (result.Contains(employeeName))
            {
                var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                if (!String.IsNullOrEmpty(employeeName))
                {
                    result = result.Replace(employeeName, empName);
                }
                else
                {
                    result = result.Replace(employeeName, "");
                }
            }

            #endregion

            #region replace updated date

            if (result.Contains(updatedDate))
            {
                result = result.Replace(updatedDate, FormatDateToString(_model.UpdatedDate));
            }

            #endregion

            #region replace url 

            if (result.Contains(urlLogin))
            {
                var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                if (!String.IsNullOrEmpty(loginLink))
                {
                    result = result.Replace(urlLogin, loginLink);
                }
            }

            #endregion

            #endregion

            return result;
        }

        private static string FormatDateToString(DateTime? date)
        {
            var result = "";

            if (date != null)
            {
                result = date.Value.Day.ToString("00") + "/" +
                         date.Value.Month.ToString("00") + "/" +
                         date.Value.Year.ToString("0000") + " " +
                         date.Value.Hour.ToString("00") + ":" +
                         date.Value.Minute.ToString("00");
            }

            return result;
        }

        #endregion

        private string returnNumberOrder(string number)
        {
            switch (number.Length)
            {
                case 1: { return "000" + number; }
                case 2: { return "00" + number; }
                case 3: { return "0" + number; }
                default:
                    return number;
            }
        }

        private string ReGenerateorderCode(List<string> listVendorCode, int totalVendorOrder)
        {
            string currentYear = DateTime.Now.Year.ToString();
            string result = "DH-" + currentYear.Substring(currentYear.Length - 2) + returnNumberOrder(totalVendorOrder.ToString());
            var checkDuplidate = listVendorCode.FirstOrDefault(f => f == result);
            if (checkDuplidate != null) return ReGenerateorderCode(listVendorCode, totalVendorOrder + 1);
            return result;
        }

        public GetMasterDataSearchProcurementRequestResult GetMasterDataSearchProcurementRequest(GetMasterDataSearchProcurementRequestParameter parameter)
        {
            var listCategory = context.Category.ToList();
            var categoryType = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DDU");

            var listStatus = new List<CategoryEntityModel>();
            if (categoryType != null)
            {
                listStatus = listCategory.Where(c => c.CategoryTypeId == categoryType.CategoryTypeId).Select(m => new CategoryEntityModel()
                {
                    CategoryTypeId = m.CategoryTypeId,
                    CategoryId = m.CategoryId,
                    CategoryName = m.CategoryName,
                    CategoryCode = m.CategoryCode,
                    IsDefault = m.IsDefauld
                }).ToList();
            }
            var employee = context.Employee.Where(c => c.Active == true).OrderBy(c => c.EmployeeName).ToList();

            var listProduct = context.Product.Where(p => p.Active == true).Select(x => new ProductEntityModel
            {
                ProductId = x.ProductId,
                ProductCategoryId = x.ProductCategoryId,
                ProductName = x.ProductName,
                ProductCode = x.ProductCode,
                Price1 = x.Price1,
                Price2 = x.Price2,
                CreatedById = x.CreatedById,
                CreatedDate = x.CreatedDate,
                UpdatedById = x.UpdatedById,
                UpdatedDate = x.UpdatedDate,
                Active = x.Active,
                Quantity = x.Quantity,
                ProductUnitId = x.ProductUnitId,
                //ProductUnitName = x.ProductUnitName,
                ProductDescription = x.ProductDescription,
                Vat = x.Vat,
                MinimumInventoryQuantity = x.MinimumInventoryQuantity,
                ProductMoneyUnitId = x.ProductMoneyUnitId,
                //ProductCategoryName = x.ProductCategoryName,
                //ListVendorName = x.ListVendorName,
                Guarantee = x.Guarantee,
                GuaranteeTime = x.GuaranteeTime,
                //CountProductInformation = x.GuaranteeTime,
                ExWarehousePrice = x.ExWarehousePrice,
                CalculateInventoryPricesId = x.CalculateInventoryPricesId,
                PropertyId = x.PropertyId,
                WarehouseAccountId = x.WarehouseAccountId,
                RevenueAccountId = x.RevenueAccountId,
                PayableAccountId = x.PayableAccountId,
                ImportTax = x.ImportTax,
                CostPriceAccountId = x.CostPriceAccountId,
                AccountReturnsId = x.AccountReturnsId,
                FolowInventory = x.FolowInventory,
                ManagerSerialNumber = x.ManagerSerialNumber,
                ProductCodeName = x.ProductCode + " - " + x.ProductName
            }).ToList();

            var listVendor = context.Vendor.Where(v => v.Active == true).Select(v => new VendorEntityModel
            {
                VendorId = v.VendorId,
                VendorName = v.VendorName,
                VendorGroupId = v.VendorGroupId,
                VendorCode = v.VendorCode,
                VendorCodeName = v.VendorCode + " - " + v.VendorName,
                Active = v.Active
            }).ToList();

            var listBudget = context.ProcurementPlan.Where(v => v.Active != false).ToList();

            return new GetMasterDataSearchProcurementRequestResult
            {
                Status = true,
                Message = "Success",
                Employees = employee,
                ListProduct = listProduct,
                ListStatus = listStatus,
                ListVendor = listVendor,
                ListBudget = listBudget
            };
        }
    }
}
