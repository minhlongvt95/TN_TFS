using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TN.TNM.Common;
using TN.TNM.Common.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.RequestPayment;
using TN.TNM.DataAccess.Messages.Results.RequestPayment;
using TN.TNM.DataAccess.Models.Document;
using TN.TNM.DataAccess.Models.RequestPayment;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class RequestPaymentDAO : BaseDAO, IRequestPaymentDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public RequestPaymentDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }

        public CreateRequestPaymentResult CreateRequestPayment(CreateRequestPaymentParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.RequestPayment, "Create Request Payment", parameter.UserId);
            try
            {
                #region Save RequestPayment to database
                int currentYear = DateTime.Now.Year % 100;
                var lstRequestPayment = context.RequestPayment.Where(w => w.YearCode == currentYear).Select(s => s.NumberCode.Value).ToList();
                int MaxNumberCode = 0;
                if (lstRequestPayment.Count > 0)
                {
                    MaxNumberCode = lstRequestPayment.Max();
                }
                parameter.RequestPayment.RequestPaymentCode = string.Format("RPP-{0}{1}", currentYear, (MaxNumberCode + 1).ToString("D4"));
                var draft = context.Category.FirstOrDefault(c => c.CategoryCode == "DR").CategoryId;
                parameter.RequestPayment.StatusId = draft;
                parameter.RequestPayment.CreateDate = DateTime.Now;
                parameter.RequestPayment.YearCode = currentYear;
                parameter.RequestPayment.NumberCode = MaxNumberCode + 1;
                parameter.RequestPayment.CreateById = parameter.UserId;
                context.RequestPayment.Add(parameter.RequestPayment);
                context.SaveChanges();
                #endregion

                #region Save file attach to disk
                string rootFolder = _hostingEnvironment.WebRootPath + string.Format("\\RequestPaymentUpload\\{0}", parameter.RequestPayment.RequestPaymentCode);
                if (!Directory.Exists(rootFolder))
                {
                    Directory.CreateDirectory(rootFolder);
                }
                List<Entities.Document> lstDocument = new List<Entities.Document>();
                if(parameter.FileList != null)
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
                            itemDocument.ObjectId = parameter.RequestPayment.RequestPaymentId;
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

                return new CreateRequestPaymentResult
                {
                    RequestPaymentId = parameter.RequestPayment.RequestPaymentId,
                    Message = "Tạo yêu cầu thanh toán thành công",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new CreateRequestPaymentResult
                {
                    RequestPaymentId = Guid.Empty,
                    Message = "Có lỗi xảy ra trong quá trình tạo",
                    Status = false
                };
            }
        }

        public EditRequestPaymentResult EditRequestPayment(EditRequestPaymentParameter parameter)
        {
            try
            {
                #region Update RequestPayment
                parameter.RequestPayment.UpdateDate = DateTime.Now;
                parameter.RequestPayment.UpdateById = parameter.UserId;
                context.RequestPayment.Update(parameter.RequestPayment);
                context.SaveChanges();
                #endregion

                #region Update document attach
                //delete document old
                var OldDocument = context.Document.Where(w => !parameter.lstDocument.Contains(w.DocumentId.ToString())).ToList();
                OldDocument.ForEach(item =>
                {
                    if (File.Exists(item.DocumentUrl))
                    {
                        File.Delete(item.DocumentUrl);
                    }
                });
                context.Document.RemoveRange(OldDocument);
                context.SaveChanges();
                ///
                ///Add new File to directory
                string rootFolder = _hostingEnvironment.WebRootPath + string.Format("\\RequestPaymentUpload\\{0}", parameter.RequestPayment.RequestPaymentCode);
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
                            itemDocument.ObjectId = parameter.RequestPayment.RequestPaymentId;
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
                //
                #endregion
                return new EditRequestPaymentResult
                {
                    RequestPaymentId = parameter.UserId,
                    Message = "Cập nhật yêu cầu thanh toán thành công",
                    Status = true
                };

            }
            catch (Exception ex)
            {
                return new EditRequestPaymentResult
                {
                    RequestPaymentId = parameter.UserId,
                    Message = "Có lỗi xảy ra trong quá trình cập nhật",
                    Status = false
                };
            }
        }

        public FindRequestPaymentResult FindRequestPayment(FindRequestPaymentParameter parameter)
        {
            var status = context.Category.FirstOrDefault(e => e.CategoryCode == "DR");
            var lst = context.RequestPayment.Where(rp => rp.RequestPaymentCode.Contains(parameter.RequestCode.Trim()) &&
                                                        (parameter.OrganizationId == null || rp.RequestBranch == parameter.OrganizationId) &&
                                                        (parameter.StatusList == null || parameter.StatusList.Count == 0 || parameter.StatusList.Contains(rp.StatusId.Value)) &&
                                                        (parameter.StartDate == null || parameter.StartDate <= rp.CreateDate) &&
                                                        (parameter.EndDate == null || parameter.EndDate >= rp.CreateDate) &&
                                                        (parameter.EmployeeId == null || rp.RequestEmployee == parameter.EmployeeId) &&
                                                        (parameter.PaymentId == null || parameter.PaymentId == rp.RequestPaymentId) &&
                                                        (rp.CreateById == parameter.UserId || rp.StatusId != status.CategoryId )
                                                        
            ).Select(rp => new RequestPaymentEntityModel(rp)).ToList();

            lst.ForEach(item =>
            {
                item.RequestEmployeeName = context.Employee.FirstOrDefault(e => e.EmployeeId == item.RequestEmployee).EmployeeName;
                item.StatusName = context.Category.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryName;
                item.PaymentName = context.Category.FirstOrDefault(c => c.CategoryId == item.PaymentType).CategoryName;
                item.BranchName = context.Organization.FirstOrDefault(o => o.OrganizationId == item.RequestBranch).OrganizationName;
            });

            return new FindRequestPaymentResult()
            {
                RequestList = lst,
                Status = true
            };
        }

        public GetRequestPaymentByIdResult GetRequestPaymentById(GetRequestPaymentByIdParameter parameter)
        {
            try
            {
                var requestPaymentObject = (from rp in context.RequestPayment
                                            join org in context.Organization on rp.RequestBranch equals org.OrganizationId
                                            join emp1 in context.Employee on rp.RequestEmployee equals emp1.EmployeeId
                                            join emp2 in context.Employee on rp.ApproverId equals emp2.EmployeeId
                                            join stt in context.Category on rp.StatusId equals stt.CategoryId
                                            where rp.RequestPaymentId == parameter.RequestPaymentId
                                            select new RequestPaymentEntityModel
                                            {
                                                RequestPaymentId = rp.RequestPaymentId,
                                                RequestPaymentCode = rp.RequestPaymentCode,
                                                RequestPaymentNote = rp.RequestPaymentNote,
                                                RequestPaymentCreateDate = rp.RequestPaymentCreateDate,
                                                RequestEmployee = rp.RequestEmployee,
                                                RequestEmployeePhone = rp.RequestEmployeePhone,
                                                RequestBranch = rp.RequestBranch,
                                                BranchName = org.OrganizationName,
                                                RequestEmployeeName = emp1.EmployeeCode + " - " + emp1.EmployeeName,
                                                StatusName = rp.StatusId == null ? "" : stt.CategoryName,
                                                ApproverId = rp.ApproverId,
                                                PostionApproverId = rp.PostionApproverId,
                                                TotalAmount = rp.TotalAmount,
                                                PaymentType = rp.PaymentType,
                                                NumberCode = rp.NumberCode,
                                                YearCode = rp.YearCode,
                                                StatusId = rp.StatusId,
                                                Description = rp.Description,
                                                CreateById = rp.CreateById,
                                                CreateDate = rp.CreateDate,
                                                UpdateById = rp.UpdateById,
                                                UpdateDate = rp.UpdateDate
                                            }).FirstOrDefault();

                var listDocument = context.Document.Where(w => w.ObjectId == parameter.RequestPaymentId).Select(s => new DocumentEntityModel(s)).ToList();

                var waitingforApproveId = context.Category.FirstOrDefault(c => c.CategoryCode == "WaitForAp").CategoryId;
                var approvedId = context.Category.FirstOrDefault(c => c.CategoryCode == "Approved").CategoryId;
                var rejectedId = context.Category.FirstOrDefault(c => c.CategoryCode == "Rejected").CategoryId;

                bool IsSendingApprove = requestPaymentObject.StatusId == waitingforApproveId;
                bool IsApprove = requestPaymentObject.StatusId == approvedId;
                bool IsReject = requestPaymentObject.StatusId == rejectedId;

                //Note
                var note = string.Empty;
                if (requestPaymentObject != null)
                {
                    note = context.FeatureNote.FirstOrDefault(f => f.FeatureId == requestPaymentObject.RequestPaymentId)?.Note;
                }

                return new GetRequestPaymentByIdResult
                {
                    requestPaymentEntityModel = requestPaymentObject,
                    lstDocument = listDocument,
                    Message = "Ok",
                    Status = true,
                    Notes = StringHelper.ConvertNoteToObject(note),
                };
            }
            catch (Exception ex)
            {
                return new GetRequestPaymentByIdResult
                {
                    requestPaymentEntityModel = null,
                    Message = "Đã có lỗi xảy ra",
                    Status = false
                };
            }

        }
    }
}
