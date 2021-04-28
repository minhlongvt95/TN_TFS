using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.Common.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeRequestDAO : BaseDAO, IEmployeeRequestDataAccess
    {
        public EmployeeRequestDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public CreateEmployeeRequestResult CreateEmployeeRequest(CreateEmplyeeRequestParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.EMPLOYEEREQUEST, "Create employee request", parameter.UserId);
            var allowanceById = context.EmployeeAllowance.Where(e => e.EmployeeId == parameter.EmployeeRequest.OfferEmployeeId && DateTime.Parse(e.EffectiveDate.ToString()).Year == DateTime.Now.Year).OrderByDescending(e => e.CreateDate).Select(e => e.EmployeeAllowanceId).Take(1).ToList();
            var allowance = context.EmployeeAllowance.FirstOrDefault(e => allowanceById.Contains(e.EmployeeAllowanceId));
            if (allowance != null)
            {
                var paidLeaveId = context.Category.FirstOrDefault(c => c.CategoryCode == "NP").CategoryId;
                var numberOfPaidLeave = allowance.MaternityAllowance;
                var currentPaidLeaveNumber = Convert.ToDecimal((parameter.EmployeeRequest.EnDate.Value - parameter.EmployeeRequest.StartDate.Value).TotalDays) + 1;
                if (currentPaidLeaveNumber > numberOfPaidLeave && paidLeaveId == parameter.EmployeeRequest.TypeRequest)
                {
                    return new CreateEmployeeRequestResult
                    {
                        Status = false,
                        Message = CommonMessage.EmployeeRequest.REACHED_MAX_LEAVE_DAY
                    };
                }
            }

            var workFolow = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "QTPDDXXN");

            parameter.EmployeeRequest.EmployeeRequestId = Guid.NewGuid();
            parameter.EmployeeRequest.RequestDate = DateTime.Now;
            parameter.EmployeeRequest.StepNumber = context.WorkFlowSteps.FirstOrDefault(ws => ws.StepNumber == 1 && ws.WorkflowId == workFolow.WorkFlowId).NextStepNumber;
            parameter.EmployeeRequest.EmployeeRequestCode = $"RQ-{DateTime.Now.Year.ToString().Substring(2, 2)}{string.Format("{0:0000}", context.EmployeeRequest.Count() + 1)}";
            var category = context.Category.FirstOrDefault(c => c.CategoryCode == "DR");
            parameter.EmployeeRequest.StatusId = category?.CategoryId ?? Guid.Empty;

            context.EmployeeRequest.Add(parameter.EmployeeRequest);
            context.SaveChanges();

            #region Get Infor to Send email
            var sendEmaiModel = new DataAccess.Models.Email.SendEmailEntityModel();

            //danh sach nguoi nhan email
            var createEmployee = context.Employee.FirstOrDefault(f => f.EmployeeId == parameter.EmployeeRequest.CreateEmployeeId);
            var createEmployeeContact = context.Contact.FirstOrDefault(f => f.ObjectId == createEmployee.EmployeeId);
            var offerEmployee = context.Employee.FirstOrDefault(f => f.EmployeeId == parameter.EmployeeRequest.OfferEmployeeId);
            var offerEmployeeContact = context.Contact.FirstOrDefault(f => f.ObjectId == offerEmployee.EmployeeId);
            sendEmaiModel.ListEmployeeName_1.Add(createEmployee.EmployeeName);
            sendEmaiModel.ListEmployeeName_1.Add(offerEmployee.EmployeeName);
            sendEmaiModel.ListEmployeeSendEmail_1.Add(createEmployeeContact.Email);
            if (!sendEmaiModel.ListEmployeeSendEmail_1.Contains(offerEmployeeContact.Email))
            {
                sendEmaiModel.ListEmployeeSendEmail_1.Add(offerEmployeeContact.Email);
            }
            var approverEmployee = context.Employee.FirstOrDefault(f => f.EmployeeId == parameter.EmployeeRequest.ApproverId);
            var approverEmployeeContact = context.Contact.FirstOrDefault(f => f.ObjectId == approverEmployee.EmployeeId);
            sendEmaiModel.ListEmployeeName_2.Add(approverEmployee.EmployeeName);
            sendEmaiModel.ListEmployeeSendEmail_2.Add(approverEmployeeContact.Email);
            //infor
            sendEmaiModel.EmployeeRequestCode = parameter.EmployeeRequest.EmployeeRequestCode;
            sendEmaiModel.OfferEmployeeName = offerEmployee.EmployeeName;
            sendEmaiModel.CreateEmployeeCode = createEmployee.EmployeeCode;
            sendEmaiModel.CreateEmployeeName = createEmployee.EmployeeName;
            var createdDate = parameter.EmployeeRequest.CreateDate ?? DateTime.Now;
            sendEmaiModel.CreatedDate = createdDate.Day.ToString("00") + "/" + createdDate.Month.ToString("00") + "/" + createdDate.Year.ToString("0000");
            sendEmaiModel.TypeRequestName = context.Category.FirstOrDefault(f => f.CategoryId == parameter.EmployeeRequest.TypeRequest).CategoryName;
            var _startDate = parameter.EmployeeRequest.StartDate;
            var _startTypeTime = context.Category.FirstOrDefault(f => f.CategoryId == parameter.EmployeeRequest.StartTypeTime).CategoryName ?? "";
            var _endDate = parameter.EmployeeRequest.EnDate;
            var _endTypeTime = context.Category.FirstOrDefault(f => f.CategoryId == parameter.EmployeeRequest.EndTypeTime).CategoryName ?? "";
            var _startDateString = _startDate.Value.Day.ToString("00") + "/" + _startDate.Value.Month.ToString("00") + "/" + _startDate.Value.Year.ToString("0000");
            var _endDateString = _endDate.Value.Day.ToString("00") + "/" + _endDate.Value.Month.ToString("00") + "/" + _endDate.Value.Year.ToString("0000");
            sendEmaiModel.DurationTime = _startDateString + " (" + _startTypeTime + ") " + "đến " + _endDateString + " (" + _endTypeTime + ") ";
            sendEmaiModel.Detail = parameter.EmployeeRequest.Detail ?? "";
            //sendEmaiModel.ApproverDetail = approverEmployee.EmployeeCode + "-" + approverEmployee.EmployeeName;
            sendEmaiModel.ApproverCode = approverEmployee.EmployeeCode;
            sendEmaiModel.ApproverName = approverEmployee.EmployeeName;
            //list thong bao (CC)
            var listNotifyId = parameter.EmployeeRequest?.NotifyList.Split(',').ToList();
            var _listNotify = new List<string>();
            listNotifyId?.ForEach(emp =>
            {
                if (!string.IsNullOrWhiteSpace(emp))
                {
                    var option = context.Employee.FirstOrDefault(f => f.EmployeeId == new Guid(emp));
                    _listNotify.Add(option.EmployeeCode + "-" + option.EmployeeName);
                    var email = context.Contact.FirstOrDefault(f => f.ObjectId == new Guid(emp))?.Email;
                    if (email != null && !sendEmaiModel.ListEmployeeCCEmail.Contains(email))
                    {
                        sendEmaiModel.ListEmployeeCCEmail.Add(email);
                    }
                }             
            });
            sendEmaiModel.NotifyList = string.Join(",", _listNotify);

            #endregion

            return new CreateEmployeeRequestResult
            {
                Status = true,
                Message = CommonMessage.EmployeeRequest.CREATE_SUCCESS,
                EmployeeRequestId = parameter.EmployeeRequest.EmployeeRequestId,
                EmployeeRequestCode = parameter.EmployeeRequest.EmployeeRequestCode,
                SendEmaiModel = sendEmaiModel
            };
        }

        public EditEmployeeRequestByIdResult EditEmployeeRequestById(EditEmployeeRequestByIdParameter parameter)
        {
            iAuditTrace.Trace(ActionName.UPDATE, ObjectName.EMPLOYEEREQUEST, "Edit Employee Request by Id", parameter.UserId);
            parameter.EmployeeRequest.UpdateById = parameter.UserId;
            parameter.EmployeeRequest.UpdateDate = DateTime.Now;

            context.EmployeeRequest.Update(parameter.EmployeeRequest);
            context.SaveChanges();

            return new EditEmployeeRequestByIdResult()
            {
                Status = true,
                Message = CommonMessage.EmployeeRequest.EDIT_SUCCESS
            };
        }

        public GetAllEmployeeRequestResult GetAllEmployeeRequest(GetAllEmployeeRequestParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.EMPLOYEEREQUEST, "Get all Employee Request", parameter.UserId);
            var parentOrgList = (from chil in context.Organization
                                 join par in context.Organization on chil.ParentId equals par.OrganizationId into orgLeft
                                 from par in orgLeft.DefaultIfEmpty()
                                 select new { orgId = chil.OrganizationId, orgName = chil.OrganizationName, orgParentName = par.OrganizationName, orgParentId = chil.ParentId }).ToList();
            var empRequestList = (from er in context.EmployeeRequest
                                  join c in context.Category on er.TypeRequest equals c.CategoryId
                                  join e in context.Employee on er.OfferEmployeeId equals e.EmployeeId
                                  join o in context.Organization on e.OrganizationId equals o.OrganizationId
                                  join e1 in context.Employee on er.ApproverId equals e1.EmployeeId
                                  join s in context.Category on er.StatusId equals s.CategoryId
                                  join parentOrg in parentOrgList on e.OrganizationId equals parentOrg.orgId
                                  select new EmployeeRequestEntityModel
                                  {
                                      EmployeeRequestId = er.EmployeeRequestId,
                                      EmployeeRequestCode = er.EmployeeRequestCode,
                                      CreateEmployeeId = er.CreateEmployeeId,
                                      CreateEmployeeCode = er.CreateEmployeeCode,
                                      OfferEmployeeId = er.OfferEmployeeId,
                                      OfferEmployeeCode = e.EmployeeCode,
                                      TypeRequest = er.TypeRequest,
                                      StatusId = er.StatusId,
                                      StartDate = er.StartDate,
                                      EnDate = er.EnDate,
                                      StartTypeTime = er.StartTypeTime,
                                      EndTypeTime = er.EndTypeTime,
                                      RequestDate = er.RequestDate,
                                      TypeReason = er.TypeReason,
                                      Detail = er.Detail,
                                      ManagerId = er.ManagerId,
                                      ApproverId = er.ApproverId,
                                      NotifyList = er.NotifyList,
                                      CreateById = er.CreateById,
                                      CreateDate = er.CreateDate,
                                      UpdateDate = er.UpdateDate,
                                      UpdateById = er.UpdateById,
                                      OfferEmployeeName = e.EmployeeName,
                                      Organization = o.OrganizationName,
                                      OrganizationId = o.OrganizationId,

                                      ApproverName = e1.EmployeeName,
                                      TypeRequestName = c.CategoryName,
                                      StatusName = s.CategoryName,
                                      ParentId = parentOrg.orgParentId,
                                      ParentName = parentOrg.orgParentName
                                  }).OrderByDescending(er => er.CreateDate).ToList();
            var organizationList = (from er in context.EmployeeRequest
                                    join e in context.Employee on er.OfferEmployeeId equals e.EmployeeId
                                    join o in context.Organization on e.OrganizationId equals o.OrganizationId
                                    select new OrganizationDetail()
                                    {
                                        OrganizationCode = o.OrganizationCode,
                                        OrganizationId = o.OrganizationId,
                                        OrganizationName = o.OrganizationName
                                    }).Distinct().ToList();

            var currentEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentEmp = context.Employee.FirstOrDefault(e => e.EmployeeId == currentEmpId);
            //bool isManager = context.Employee.Any(e => e.EmployeeId == currentEmpId && e.IsManager);
            empRequestList = empRequestList.Where(er => er.CreateById == parameter.UserId || (currentEmp.IsManager && currentEmp.OrganizationId == er.OrganizationId)).ToList();

            return new GetAllEmployeeRequestResult
            {
                EmployeeRequestList = empRequestList,
                OrganizationList = organizationList,
                Status = true,
            };
        }

        public GetEmployeeRequestByIdResult GetEmployeeRequestById(GetEmployeeRequestByIdParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.EMPLOYEEREQUEST, "Get Employee Request by Id", parameter.UserId);
            var employeeRequest = context.EmployeeRequest.FirstOrDefault(e => e.EmployeeRequestId == parameter.EmployeeRequestId);

            EmployeeRequestEntityModel employeeRequestModel = new EmployeeRequestEntityModel(employeeRequest);
            if (employeeRequestModel.StartTypeTime == null)
            {
                employeeRequestModel.StartTypeTime = context.Category.FirstOrDefault(c => c.CategoryCode == "SAN").CategoryId;
            }
            if (employeeRequestModel.EndTypeTime == null)
            {
                employeeRequestModel.EndTypeTime = context.Category.FirstOrDefault(c => c.CategoryCode == "CHI").CategoryId;
            }

            employeeRequestModel.ShiftName = context.Category.FirstOrDefault(ct => ct.CategoryId == employeeRequestModel.StartTypeTime).CategoryCode + "," + context.Category.FirstOrDefault(ct => ct.CategoryId == employeeRequestModel.EndTypeTime).CategoryCode;

            var waitingforApprove = context.Category.FirstOrDefault(c => c.CategoryCode == "WaitForAp");
            var approved = context.Category.FirstOrDefault(c => c.CategoryCode == "Approved");
            var rejected = context.Category.FirstOrDefault(c => c.CategoryCode == "Rejected");
            var featureprogress = context.FeatureWorkFlowProgress.FirstOrDefault(fwp => fwp.ApprovalObjectId == employeeRequestModel.EmployeeRequestId);
            //Note
            var note = string.Empty;
            if (employeeRequestModel != null)
            {
                note = context.FeatureNote.FirstOrDefault(f => f.FeatureId == employeeRequestModel.EmployeeRequestId)?.Note;
                employeeRequestModel.CreateEmployeeName = context.Employee.FirstOrDefault(e => e.EmployeeId == employeeRequestModel.CreateEmployeeId).EmployeeName;
                employeeRequestModel.OfferEmployeeName = context.Employee.FirstOrDefault(e => e.EmployeeId == employeeRequestModel.OfferEmployeeId).EmployeeName;
                employeeRequestModel.ApproverName = context.Employee.FirstOrDefault(e => e.EmployeeId == employeeRequestModel.ApproverId).EmployeeName;
            }

            var result = new GetEmployeeRequestByIdResult()
            {
                Status = true,
                EmployeeRequest = employeeRequestModel,
                IsApproved = employeeRequestModel.StatusId == approved.CategoryId,
                IsInApprovalProgress = employeeRequestModel.StatusId == waitingforApprove.CategoryId,
                IsRejected = employeeRequestModel.StatusId == rejected.CategoryId,
                ApproverId = featureprogress != null ? featureprogress.ApproverPersonId : Guid.Empty,
                PositionId = featureprogress != null ? featureprogress.ApproverPositionId : Guid.Empty,
                Notes = StringHelper.ConvertNoteToObject(note),
            };

            //result.StatusName = result.IsInApprovalProgress ? waitingforApprove.CategoryName :
            //    (result.IsApproved ? approved.CategoryName :
            //    (result.IsRejected ? rejected.CategoryName : "Chưa gửi phê duyệt"));
            result.StatusName = context.Category.FirstOrDefault(c => c.CategoryId == employeeRequest.StatusId).CategoryName;
            return result;
        }

        public SearchEmployeeRequestResult SearchEmployeeRequest(SearchEmployeeRequestParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.EMPLOYEEREQUEST, "Search employee request", parameter.UserId);
            var employeeList = context.Employee.ToList();
            var currentEmp = employeeList.FirstOrDefault(emp => emp.EmployeeId == context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId);
            var currentPosition = context.Position.FirstOrDefault(p => p.PositionId == currentEmp.PositionId);
            var currentEmpOrg = context.Organization.FirstOrDefault(org => org.OrganizationId == currentEmp.OrganizationId);
            var listChildOfOrg = ListChildOfParamToSearch(currentEmp.OrganizationId.Value);
            listChildOfOrg.Add(currentEmp.OrganizationId.Value);
            var workFolow = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "QTPDDXXN");
            var workFlowStep = context.WorkFlowSteps.Where(ws => ws.WorkflowId == workFolow.WorkFlowId).ToList();
            var maxStep = workFlowStep.Max(m => m.StepNumber);

            var contact = context.Contact.ToList();
            // changing follow by each customer
            var currentEmpCodeListLowLevel = new List<string>() { "GV", "CTV", "NV", "TG" };
            var currentEmpCodeListHightLevel = new List<string>() { "TP", "TGD", "PP", "QL" };

            #region ngoc comment
            var employeeRequestList = (from er in context.EmployeeRequest
                                       join offer in context.Employee on er.OfferEmployeeId equals offer.EmployeeId
                                       join appr in context.Employee on er.ApproverId equals appr.EmployeeId
                                       join org in context.Organization on offer.OrganizationId equals org.OrganizationId
                                       join stt in context.Category on er.StatusId equals stt.CategoryId
                                       where
                                             (parameter.OfferEmployeeCode == "" || parameter.OfferEmployeeCode == null || offer.EmployeeCode.ToLower().Contains(parameter.OfferEmployeeCode.ToLower())) &&
                                             (parameter.OfferEmployeeName == "" || parameter.OfferEmployeeName == null || offer.EmployeeName.ToLower().Contains(parameter.OfferEmployeeName.ToLower())) &&
                                             (parameter.EmployeeRequestCode == "" || parameter.EmployeeRequestCode == null || er.EmployeeRequestCode.ToLower().Contains(parameter.EmployeeRequestCode.ToLower())) &&
                                             (parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(er.StatusId)) &&
                                             (parameter.ListTypeRequestId.Count == 0 || parameter.ListTypeRequestId.Contains(er.TypeRequest)) &&
                                             (parameter.StartDate == null || parameter.StartDate.Value.Date <= er.StartDate.Value.Date) &&
                                             (parameter.EndDate == null || parameter.EndDate.Value.Date >= er.EnDate.Value.Date) &&
                                             (parameter.OfferOrganizationId == null || ListChildOfParamToSearch(parameter.OfferOrganizationId.Value).Contains(offer.OrganizationId.Value) || parameter.OfferOrganizationId == offer.OrganizationId) &&
                                             (
                                                // neu no la nguoi tao
                                                er.CreateEmployeeId == currentEmp.EmployeeId ||
                                                // nếu nó được toàn quyền ~ ở phòng ban cao nhất
                                                (currentEmpOrg.Level == 0 && stt.CategoryCode != "DR") ||
                                                // Nếu CurrentUser(CR) là Nhân viên, trợ giảng , ... thì chỉ nhìn chính nó và không được nhìn đề xuất ở trạng thái nháp ko phải của nó
                                                ((er.OfferEmployeeId == currentEmp.EmployeeId || (er.ApproverId == currentEmp.EmployeeId && er.StepNumber == maxStep) || er.NotifyList.Contains(currentEmp.EmployeeId.ToString())) && currentEmpCodeListLowLevel.Contains(currentPosition.PositionCode) && stt.CategoryCode != "DR") ||
                                                // nếu CR là trưởng phòng, phó phòng nhìn được tất cả các phiếu của nhân viên dưới nó
                                                ((er.OfferEmployeeId == currentEmp.EmployeeId || (er.ApproverId == currentEmp.EmployeeId && er.StepNumber == maxStep) || er.NotifyList.Contains(currentEmp.EmployeeId.ToString()) || appr.OrganizationId == currentEmp.OrganizationId || ListChildOfParamToSearch(currentEmp.OrganizationId.Value).Contains(offer.OrganizationId.Value) || offer.OrganizationId == currentEmp.OrganizationId || ListChildOfParamToSearch(currentEmp.OrganizationId.Value).Contains(appr.OrganizationId.Value))
                                                && currentEmpCodeListHightLevel.Contains(currentPosition.PositionCode) && stt.CategoryCode != "DR")
                                            )
                                       select new EmployeeRequestEntityModel()
                                       {
                                           ApproverName = appr.EmployeeName,
                                           ApproverId = appr.EmployeeId,
                                           CreateById = er.CreateById,
                                           CreateDate = er.CreateDate,
                                           CreateEmployeeCode = er.CreateEmployeeCode,
                                           CreateEmployeeId = er.CreateEmployeeId,
                                           Detail = er.Detail,
                                           EmployeeRequestCode = er.EmployeeRequestCode,
                                           EmployeeRequestId = er.EmployeeRequestId,
                                           EnDate = er.EnDate,
                                           EndTypeTime = er.EndTypeTime,
                                           ManagerId = er.ManagerId,
                                           NotifyList = er.NotifyList,
                                           OfferEmployeeCode = offer.EmployeeCode,
                                           OfferEmployeeId = offer.EmployeeId,
                                           OfferEmployeeName = offer.EmployeeName,
                                           Organization = org.OrganizationName,
                                           OrganizationCode = org.OrganizationCode,
                                           OrganizationId = org.OrganizationId,
                                           StatusId = stt.CategoryId,
                                           StatusCode = stt.CategoryCode,
                                           StatusName = stt.CategoryName,
                                           ParentId = context.Organization.FirstOrDefault(_org => _org.OrganizationId == org.OrganizationId).ParentId,
                                           ParentName = context.Organization.FirstOrDefault(_org => _org.OrganizationId == org.OrganizationId).OrganizationId == null ? "" : context.Organization.FirstOrDefault(o => o.OrganizationId == (context.Organization.FirstOrDefault(_org => _org.OrganizationId == org.OrganizationId).ParentId)).OrganizationName,
                                           RequestDate = er.RequestDate,
                                           StartDate = er.StartDate,
                                           StartTypeTime = er.StartTypeTime,
                                           TypeReason = er.TypeReason,
                                           TypeRequest = er.TypeRequest,
                                           TypeRequestName = context.Category.FirstOrDefault(tr => tr.CategoryId == er.TypeRequest).CategoryName,
                                           UpdateById = er.UpdateById,
                                           UpdateDate = er.UpdateDate,
                                           ShiftName = ",",
                                           BackgroupStatusColor = "",
                                       }).OrderByDescending(o => o.CreateDate).ToList();
            #endregion

            employeeRequestList.ForEach(item =>
            {
                item.ContactId = contact.FirstOrDefault(c => c.ObjectId == item.OfferEmployeeId)?.ContactId ?? Guid.Empty;
                switch (item.StatusCode)
                {
                    case "Approved":
                        item.BackgroupStatusColor = "#007aff";
                        break;
                    case "WaitForAp":
                        item.BackgroupStatusColor = "#ffcc00";
                        break;
                    case "Rejected":
                        item.BackgroupStatusColor = "#CC3C00";
                        break;
                    case "DR":
                        item.BackgroupStatusColor = "#AEA4A0";
                        break;
                }
            });
            #region ngoc comment
            //var employeeRequestList = (from er in context.EmployeeRequest
            //                           join offer in employeeList on er.OfferEmployeeId equals offer.EmployeeId
            //                           //join appr in context.Employee on er.ApproverId equals appr.EmployeeId
            //                           join org in context.Organization on offer.OrganizationId equals org.OrganizationId
            //                           join stt in context.Category on er.StatusId equals stt.CategoryId
            //                           where
            //                                 (parameter.OfferEmployeeCode == "" || parameter.OfferEmployeeCode == null || offer.EmployeeCode.ToLower().Contains(parameter.OfferEmployeeCode.ToLower())) &&
            //                                 (parameter.OfferEmployeeName == "" || parameter.OfferEmployeeName == null || offer.EmployeeName.ToLower().Contains(parameter.OfferEmployeeName.ToLower())) &&
            //                                 (parameter.EmployeeRequestCode == "" || parameter.EmployeeRequestCode == null || er.EmployeeRequestCode.ToLower().Contains(parameter.EmployeeRequestCode.ToLower())) &&
            //                                 (parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(er.StatusId)) &&
            //                                 (parameter.ListTypeRequestId.Count == 0 || parameter.ListTypeRequestId.Contains(er.TypeRequest)) &&
            //                                 (parameter.StartDate == null || parameter.StartDate.Value.Date <= er.StartDate.Value.Date) &&
            //                                 (parameter.EndDate == null || parameter.EndDate.Value.Date >= er.EnDate.Value.Date) &&
            //                                 (parameter.OfferOrganizationId == null || ListChildOfParamToSearch(parameter.OfferOrganizationId.Value).Contains(offer.OrganizationId.Value) || parameter.OfferOrganizationId == offer.OrganizationId) &&
            //                                 (
            //                                     //neu no la nguoi tao
            //                                    er.CreateEmployeeId == currentEmp.EmployeeId ||
            //                                     //nếu nó được toàn quyền ~ở phòng ban cao nhất
            //                                   (currentEmpOrg.Level == 0 && stt.CategoryCode != "DR") ||
            //                                    //Nếu CurrentUser(CR) là Nhân viên, trợ giảng, ... thì chỉ nhìn chính nó và không được nhìn đề xuất ở trạng thái nháp ko phải của nó
            //                                  ((er.OfferEmployeeId == currentEmp.EmployeeId ||
            //                                  (er.ApproverId == currentEmp.EmployeeId && er.StepNumber == maxStep) ||
            //                                  er.NotifyList.Contains(currentEmp.EmployeeId.ToString())) && currentEmpCodeListLowLevel.Contains(currentPosition.PositionCode) && stt.CategoryCode != "DR") ||

            //                                   //nếu CR là trưởng phòng, phó phòng nhìn được tất cả các phiếu của nhân viên dưới nó
            //                                  ((er.OfferEmployeeId == currentEmp.EmployeeId ||
            //                                  (er.ApproverId == currentEmp.EmployeeId && er.StepNumber == maxStep) ||
            //                                  er.NotifyList.Contains(currentEmp.EmployeeId.ToString()) ||
            //                                  ListChildOfParamToSearch(currentEmp.OrganizationId.Value).Contains(offer.OrganizationId.Value) ||
            //                                  offer.OrganizationId == currentEmp.OrganizationId) &&
            //                                  currentEmpCodeListHightLevel.Contains(currentPosition.PositionCode) && stt.CategoryCode != "DR")
            //                                )
            //                           select new EmployeeRequestEntityModel()
            //                           {
            //                               ApproverName = employeeList.FirstOrDefault(emp => emp.EmployeeId == er.ApproverId).EmployeeName,
            //                               ApproverId = er.ApproverId,
            //                               CreateById = er.CreateById,
            //                               CreateDate = er.CreateDate,
            //                               CreateEmployeeCode = er.CreateEmployeeCode,
            //                               CreateEmployeeId = er.CreateEmployeeId,
            //                               Detail = er.Detail,
            //                               EmployeeRequestCode = er.EmployeeRequestCode,
            //                               EmployeeRequestId = er.EmployeeRequestId,
            //                               EnDate = er.EnDate,
            //                               EndTypeTime = er.EndTypeTime,
            //                               ManagerId = er.ManagerId,
            //                               NotifyList = er.NotifyList,
            //                               OfferEmployeeCode = offer.EmployeeCode,
            //                               OfferEmployeeId = offer.EmployeeId,
            //                               OfferEmployeeName = offer.EmployeeName,
            //                               Organization = org.OrganizationName,
            //                               OrganizationCode = org.OrganizationCode,
            //                               OrganizationId = org.OrganizationId,
            //                               StatusId = stt.CategoryId,
            //                               StatusName = stt.CategoryName,
            //                               //ParentId = context.Organization.FirstOrDefault(_org => _org.OrganizationId == org.OrganizationId) == null ? Guid.Empty : context.Organization.FirstOrDefault(_org => _org.OrganizationId == org.OrganizationId).ParentId,
            //                               //ParentName = context.Organization.FirstOrDefault(_org => _org.OrganizationId == org.OrganizationId) == null ? "" : context.Organization.FirstOrDefault(o => o.OrganizationId == (context.Organization.FirstOrDefault(_org => _org.OrganizationId == org.OrganizationId).ParentId)).OrganizationName,
            //                               RequestDate = er.RequestDate,
            //                               StartDate = er.StartDate,
            //                               StartTypeTime = er.StartTypeTime,
            //                               TypeReason = er.TypeReason,
            //                               TypeRequest = er.TypeRequest,
            //                               TypeRequestName = context.Category.FirstOrDefault(tr => tr.CategoryId == er.TypeRequest).CategoryName,
            //                               UpdateById = er.UpdateById,
            //                               UpdateDate = er.UpdateDate,
            //                               ShiftName = ",",
            //                           }).OrderByDescending(o => o.CreateDate).ToList();

            //var empRequests = context.EmployeeRequest.ToList();
            //empRequests.ForEach(item =>
            //{
            //    if (item.StepNumber != null)
            //    {

            //    }
            //    else
            //    {
            //        var offer = employeeList.FirstOrDefault(of => of.EmployeeId == item.OfferEmployeeId);
            //        var orga = context.Organization.FirstOrDefault(org => org.OrganizationId == offer.OrganizationId);
            //        var empR = employeeList.FirstOrDefault(emp => emp.EmployeeId == item.ApproverId);
            //        var category = context.Category.FirstOrDefault(tr => tr.CategoryId == item.TypeRequest);
            //        var orgs = context.Organization.FirstOrDefault(_org => _org.OrganizationId == orga.OrganizationId);
            //        var requests = new EmployeeRequestEntityModel()
            //        {
            //            ApproverName = empR.EmployeeName,
            //            ApproverId = item.ApproverId,
            //            CreateById = item.CreateById,
            //            CreateDate = item.CreateDate,
            //            CreateEmployeeCode = item.CreateEmployeeCode,
            //            CreateEmployeeId = item.CreateEmployeeId,
            //            Detail = item.Detail,
            //            EmployeeRequestCode = item.EmployeeRequestCode,
            //            EmployeeRequestId = item.EmployeeRequestId,
            //            EnDate = item.EnDate,
            //            EndTypeTime = item.EndTypeTime,
            //            ManagerId = item.ManagerId,
            //            NotifyList = item.NotifyList,
            //            OfferEmployeeCode = offer.EmployeeCode,
            //            OfferEmployeeId = offer.EmployeeId,
            //            OfferEmployeeName = offer.EmployeeName,
            //            Organization = orga.OrganizationName,
            //            OrganizationCode = orga.OrganizationCode,
            //            OrganizationId = orga.OrganizationId,
            //            StatusId = item.StatusId,
            //            StatusName = context.Category.FirstOrDefault(st => st.CategoryId == item.StatusId).CategoryName,
            //            ParentId = orgs == null ? Guid.Empty : orgs.ParentId,
            //            ParentName = orgs == null ? "" : context.Organization.FirstOrDefault(o => o.OrganizationId == orgs.ParentId).OrganizationName,
            //            RequestDate = item.RequestDate,
            //            StartDate = item.StartDate,
            //            StartTypeTime = item.StartTypeTime,
            //            TypeReason = item.TypeReason,
            //            TypeRequest = item.TypeRequest,
            //            TypeRequestName = category.CategoryName,
            //            UpdateById = item.UpdateById,
            //            UpdateDate = item.UpdateDate,
            //            ShiftName = ",",
            //        };
            //    }
            //});
            #endregion
            return new SearchEmployeeRequestResult
            {
                Message = "Success",
                EmployeeRequestList = employeeRequestList,
                AmountAbsentWithoutPermission = 0,
                AmountAbsentWithPermission = 0,
                Status = true
            };
        }
        private List<Guid> ListChildOfParamToSearch(Guid orgId)
        {
            //var orgParam = context.Organization.FirstOrDefault(org => org.OrganizationId == orgId);
            var _listOrgIdChild = context.Organization.Where(o => o.ParentId == orgId).Select(id => id.OrganizationId).ToList();
            var _tmpOrgId = new List<Guid>();
            _listOrgIdChild.ForEach(_orgId =>
            {
                _tmpOrgId.Add(_orgId);
                ListChildOfParamToSearch(_orgId).ForEach(child =>
                {
                    _tmpOrgId.Add(child);
                });
            });
            return _tmpOrgId;
        }
        public GetEmployeeRequestByEmpIdResult GetEmployeeRequestByEmpId(GetEmployeeRequestByEmpIdParameter parameter)
        {
            // lay ra id cua category nghi phep
            var absentPermissionId = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NP")?.CategoryId;
            var absentWithoutPermissionId = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NKL")?.CategoryId;
            var _empRequest = (from empR in context.EmployeeRequest
                               join stt in context.Category on empR.StatusId equals stt.CategoryId
                               where empR.OfferEmployeeId == parameter.EmployeeId && empR.RequestDate.Value.Year == DateTime.Now.Year && stt.CategoryCode.Trim() == "Approved"
                               select empR).OrderByDescending(o => o.RequestDate).ToList();
            double amountAbsentWithPermission = 0;
            double amountAbsentWithoutPermission = 0;
            _empRequest.ForEach(empR =>
            {
                if (empR.TypeRequest == absentPermissionId)
                {
                    if (empR.StartTypeTime == empR.EndTypeTime)
                    {
                        amountAbsentWithPermission += (empR.EnDate.Value.Date - empR.StartDate.Value.Date).Days + 0.5;
                    }
                    else
                    {
                        amountAbsentWithPermission += (empR.EnDate.Value.Date - empR.StartDate.Value.Date).Days + 1;
                    }
                }
                if (empR.TypeRequest == absentWithoutPermissionId)
                {
                    if (empR.StartTypeTime == empR.EndTypeTime)
                    {
                        amountAbsentWithoutPermission += (empR.EnDate.Value.Date - empR.StartDate.Value.Date).Days + 1;
                    }
                    else
                    {
                        amountAbsentWithoutPermission += (empR.EnDate.Value.Date - empR.StartDate.Value.Date).Days + 1;
                    }
                }
            });
            return new GetEmployeeRequestByEmpIdResult()
            {
                ListEmployeeRequest = _empRequest,
                amountAbsentWithoutPermission = amountAbsentWithoutPermission,
                amountAbsentWithPermission = amountAbsentWithPermission,
                Message = "Success",
                Status = true
            };
        }

        public CheckEmployeeCreateRequestResult CheckEmployeeCreateRequest(CheckEmployeeCreateRequestParameter parameter)
        {
            bool IsEmpCreateRequest = false;
            var workFollowRequest = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "QTPDDXXN").WorkFlowId;
            var stepNumberStart = context.WorkFlowSteps.Where(wsm => wsm.WorkflowId == workFollowRequest).Min(min => min.StepNumber);

            var workFlowStepRequest = context.WorkFlowSteps.FirstOrDefault(ws => ws.WorkflowId == workFollowRequest && ws.StepNumber == stepNumberStart);

            if (workFlowStepRequest.ApprovebyPosition)
            {
                var empPosition = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId).PositionId;
                IsEmpCreateRequest = empPosition == workFlowStepRequest.ApproverPositionId ? true : false;
            }
            else
            {
                IsEmpCreateRequest = parameter.EmployeeId == workFlowStepRequest.ApproverId ? true : false;
            }

            return new CheckEmployeeCreateRequestResult()
            {
                IsEmpCreateRequest = IsEmpCreateRequest,
                Message = "Success",
                Status = true
            };
        }

        public GetDataSearchEmployeeRequestResult GetDataSearchEmployeeRequest(GetDataSearchEmployeeRequestParameter parameter)
        {
            try
            {
                var typeRequestId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LXU").CategoryTypeId;
                var listTypeRequest = context.Category.Where(c => c.CategoryTypeId == typeRequestId).ToList();

                var statusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DDU").CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == statusId).ToList();

                return new GetDataSearchEmployeeRequestResult
                {
                    Status = true,
                    ListStatus = listStatus,
                    ListTypeRequest = listTypeRequest,
                    Message = "Success"
                };
            }catch(Exception ex)
            {
                return new GetDataSearchEmployeeRequestResult
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
    }
}
