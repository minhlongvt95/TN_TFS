using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.Common.Constant;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Workflow;
using TN.TNM.DataAccess.Messages.Results.Workflow;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Workflow;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class WorkflowDAO : BaseDAO, IWorkflowDataAccess
    {
        public WorkflowDAO(TNTN8Context _context, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _context;
            this.iAuditTrace = _iAuditTrace;
        }
        public CreateWorkflowResult CreateWorkflow(CreateWorkflowParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.WORKFLOW, "Create Workflow", parameter.UserId);
            parameter.Workflow.CreatedBy = parameter.UserId;
            var lst = parameter.WorkflowStepList;

            context.WorkFlows.Add(parameter.Workflow);
            context.SaveChanges();
            if (lst != null)
            {
                var currentWFId = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == parameter.Workflow.WorkflowCode).WorkFlowId;
                lst.ForEach(step =>
                {
                    step.WorkflowId = currentWFId;
                    step.RecordStatus = "Level";
                    context.WorkFlowSteps.Add(step);
                });
                context.SaveChanges();
            }

            return new CreateWorkflowResult()
            {
                Status = true,
                Message = CommonMessage.Workflow.CREATE_SUCCESS
            };
        }

        public SearchWorkflowResult SearchWorkflow(SearchWorkflowParameter parameter)
        {
            var lst = context.WorkFlows.Where(w => (string.IsNullOrEmpty(parameter.WorkflowName) || w.Name.Contains(parameter.WorkflowName.Trim()))
                && (parameter.SystemFeatureIdList == null || parameter.SystemFeatureIdList.Count == 0 || parameter.SystemFeatureIdList.Contains(w.SystemFeatureId))
            ).Select(w => new WorkflowEntityModel
            {
                WorkFlowId = w.WorkFlowId,
                CreatedDate = w.CreatedDate,
                StatusId = w.StatusId,
                SystemFeatureId = w.SystemFeatureId,
                Name = w.Name
            }).OrderByDescending(w => w.CreatedDate).ToList();

            #region Add by Hung
            var commonSystemFeature = context.SystemFeature.ToList();
            var commonCategory = context.Category.ToList();
            #endregion

            lst.ForEach(item =>
            {
                item.SystemFeatureName = commonSystemFeature.FirstOrDefault(sf => sf.SystemFeatureId == item.SystemFeatureId).SystemFeatureName;
                item.StatusName = commonCategory.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryName;
            });

            return new SearchWorkflowResult()
            {
                WorkflowList = lst,
                Status = true
            };
        }

        public GetWorkflowByIdResult GetWorkflowById(GetWorkflowByIdParameter parameter)
        {
            var workflow = context.WorkFlows.FirstOrDefault(w => w.WorkFlowId == parameter.WorkflowId);
            var workflowModel = new WorkflowEntityModel()
            {
                WorkFlowId = workflow.WorkFlowId,
                CreatedBy = workflow.CreatedBy,
                CreatedDate = workflow.CreatedDate,
                Description = workflow.Description,
                Name = workflow.Name,
                StatusId = workflow.StatusId,
                SystemFeatureId = workflow.SystemFeatureId,
                UpdatedBy = workflow.UpdatedBy,
                UpdatedDate = workflow.UpdatedDate,
                WorkflowCode = workflow.WorkflowCode
            };

            var workflowStep = context.WorkFlowSteps.Where(ws => ws.WorkflowId == parameter.WorkflowId)
                .Select(ws => new WorkflowStepEntityModel()
                {
                    WorkflowStepId = ws.WorkflowStepId,
                    ApprovebyPosition = ws.ApprovebyPosition,
                    ApprovedText = ws.ApprovedText,
                    ApproverId = ws.ApproverId,
                    ApproverPositionId = ws.ApproverPositionId,
                    BackStepNumber = ws.BackStepNumber,
                    NextStepNumber = ws.NextStepNumber,
                    NotApprovedText = ws.NotApprovedText,
                    RecordStatus = ws.RecordStatus,
                    StepNumber = ws.StepNumber,
                    WorkflowId = ws.WorkflowId
                }).ToList();

            workflowModel.WorkflowStepList = workflowStep.OrderBy(w=>w.StepNumber).ToList();

            return new GetWorkflowByIdResult()
            {
                Workflow = workflowModel,
                Status = true
            };
        }

        public UpdateWorkflowByIdResult UpdateWorkflowById(UpdateWorkflowByIdParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.UPDATE, ObjectName.WORKFLOW, "Update Workflow", parameter.UserId);

            var workflow = parameter.Workflow;
            context.WorkFlows.Update(workflow);
            var workflowStepList = parameter.WorkflowStepList;

            var currentWSList = context.WorkFlowSteps.Where(ws => ws.WorkflowId == parameter.Workflow.WorkFlowId).ToList();
            currentWSList.ForEach(ws =>
            {
                context.WorkFlowSteps.Remove(ws);
            });
            context.SaveChanges();
            workflowStepList.ForEach(step =>
            {
                step.WorkflowId = parameter.Workflow.WorkFlowId;
                context.WorkFlowSteps.Add(step);
            });
            context.SaveChanges();
            return new UpdateWorkflowByIdResult()
            {
                Status = true,
                Message = CommonMessage.Workflow.EDIT_SUCCESS
            };
        }

        public GetAllWorkflowCodeResult GetAllWorkflowCode(GetAllWorkflowCodeParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETCODE, ObjectName.WORKFLOW, "Get all Workflow code", parameter.UserId);
            var lst = context.WorkFlows.Where(w => w.WorkFlowId != null).Select(w => w.WorkflowCode).ToList();
            return new GetAllWorkflowCodeResult()
            {
                CodeList = lst,
                Status = true
            };
        }

        public GetAllSystemFeatureResult GetAllSystemFeature(GetAllSystemFeatureParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETCODE, ObjectName.WORKFLOW, "Get all SystemFeature", parameter.UserId);
            var lst = context.SystemFeature.ToList();
            return new GetAllSystemFeatureResult()
            {
                SystemFeatureList = lst,
                Status = true
            };
        }

        private void SaveNotification(Guid objectId, string objectType, string notiType, Guid receiverId, string content)
        {
            try
            {
                var notification = new Entities.Notifications()
                {
                    ObjectId = objectId,
                    ObjectType = objectType,
                    NotificationType = notiType,
                    ReceiverId = receiverId,
                    Content = content,
                    CreatedDate = DateTime.Now,
                    Viewed = false
                };

                context.Notifications.Add(notification);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public NextWorkflowStepResult NextWorkflowStep(NextWorkflowStepParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.UPDATE, ObjectName.WORKFLOW, "Next Workflow Step", parameter.UserId);
            var commonCategory = context.Category.ToList();
            var commonWorkFlowSteps = context.WorkFlowSteps.ToList();

            var isFinalApprove = false;
            var message = "";
            var waitingforApproveId = commonCategory.FirstOrDefault(c => c.CategoryCode == "WaitForAp").CategoryId;
            var approvedId = commonCategory.FirstOrDefault(c => c.CategoryCode == "Approved").CategoryId;
            var rejectedId = commonCategory.FirstOrDefault(c => c.CategoryCode == "Rejected").CategoryId;
            var record = context.FeatureWorkFlowProgress.FirstOrDefault(r => r.ApprovalObjectId == parameter.FeatureId);
            var systemfeatureid = context.SystemFeature.FirstOrDefault(sf => sf.SystemFeatureCode == parameter.FeatureCode).SystemFeatureId;
            var workflow = context.WorkFlows.FirstOrDefault(w => w.SystemFeatureId == systemfeatureid);
            if (record != null)
            {
                var wfcurrentStep = commonWorkFlowSteps.FirstOrDefault(s => s.WorkflowId == workflow.WorkFlowId && s.StepNumber == record.CurrentStep);
                record.ProgressStatus = parameter.IsReject
                    ? wfcurrentStep?.NotApprovedText
                    : wfcurrentStep?.ApprovedText;

                var workflowNextStep = commonWorkFlowSteps.FirstOrDefault(s => s.WorkflowId == workflow.WorkFlowId && s.StepNumber == (parameter.IsReject ? 1 : record.NextStepNumber));
                if (workflowNextStep != null)
                {
                    record.CurrentStep = workflowNextStep.StepNumber;
                    record.NextStepNumber = workflowNextStep.NextStepNumber;
                    record.BackStepNumber = workflowNextStep.BackStepNumber;
                    record.ApproverPersonId = workflowNextStep.ApproverId;
                    record.ApproverPositionId = workflowNextStep.ApproverPositionId;
                    record.RecordStatus = workflowNextStep.RecordStatus;
                    record.ActorId = parameter.UserId;
                }
                else
                {
                    isFinalApprove = true;

                    var firstStep = commonWorkFlowSteps.FirstOrDefault(s => s.WorkflowId == workflow.WorkFlowId && s.StepNumber == 1);
                    if (firstStep != null)
                    {
                        record.CurrentStep = firstStep.StepNumber;
                        record.NextStepNumber = firstStep.NextStepNumber;
                        record.BackStepNumber = firstStep.BackStepNumber;
                        record.ApproverPersonId = firstStep.ApproverId;
                        record.ApproverPositionId = firstStep.ApproverPositionId;
                        record.RecordStatus = firstStep.RecordStatus;
                        record.ActorId = parameter.UserId;
                        record.ChangeStepDate = DateTime.Now;
                        record.IsFinalApproved = isFinalApprove;
                    }
                }
            }
            else
            {
                var newRecord = new Entities.FeatureWorkFlowProgress
                {
                    FeatureWorkflowProgressId = Guid.NewGuid(),
                    ApprovalObjectId = parameter.FeatureId.Value,
                    SystemFeatureCode = parameter.FeatureCode
                };
                var workflowCurrentStep = commonWorkFlowSteps.FirstOrDefault(s => s.WorkflowId == workflow.WorkFlowId && s.StepNumber == 1);
                if (workflowCurrentStep != null)
                {
                    newRecord.CurrentStep = workflowCurrentStep.StepNumber;
                    newRecord.NextStepNumber = workflowCurrentStep.NextStepNumber;
                    newRecord.BackStepNumber = workflowCurrentStep.BackStepNumber;
                    newRecord.ApproverPersonId = workflowCurrentStep.ApproverId;
                    newRecord.ApproverPositionId = workflowCurrentStep.ApproverPositionId;
                    newRecord.ProgressStatus = workflowCurrentStep.ApprovedText;
                    newRecord.RecordStatus = workflowCurrentStep.RecordStatus;
                    newRecord.ChangeStepDate = DateTime.Now;

                    record = newRecord;
                }

                context.FeatureWorkFlowProgress.Add(newRecord);
                //NextWorkflowStep(parameter);
            }

            switch (parameter.FeatureCode)
            {
                case SystemFeatureCode.PROCUREMENTREQUEST:

                    var request = context.ProcurementRequest.FirstOrDefault(pr => pr.ProcurementRequestId == parameter.FeatureId);
                                       
                    //if (parameter.IsSendingApprove || (parameter.IsApprove && !isFinalApprove))
                    if (parameter.IsSendingApprove && !parameter.IsApprove)
                        {
                        request.StatusId = waitingforApproveId;
                        message = CommonMessage.Workflow.CHANGE_SUCCESS;
                        SaveNotification(request.ProcurementRequestId, ObjectType.PROCUREMENTREQUEST, NotificationType.APPROVE,
                            request.ApproverId.Value, "Đề xuất mua hàng " + request.ProcurementCode + " " + record.ProgressStatus);
                    }

                    if (parameter.IsApprove && isFinalApprove)
                    {
                        request.StatusId = approvedId;
                        message = CommonMessage.Workflow.APPROVE;
                        SaveNotification(request.ProcurementRequestId, ObjectType.PROCUREMENTREQUEST, NotificationType.APPROVE,
                            request.ApproverId.Value, "Đề xuất mua hàng " + request.ProcurementCode + " " + record.ProgressStatus);
                    }
                    else if (parameter.IsReject)
                    {
                        request.StatusId = rejectedId;
                        message = CommonMessage.Workflow.REJECT;
                        var currentEmpId = context.User.FirstOrDefault(u => u.UserId == request.CreatedById).EmployeeId;
                        SaveNotification(request.ProcurementRequestId, ObjectType.PROCUREMENTREQUEST, NotificationType.APPROVE,
                            currentEmpId.Value, "Đề xuất mua hàng " + request.ProcurementCode + " " + record.ProgressStatus);

                        WorkFlowHepler.SaveNote(context, parameter.FeatureId.Value, parameter.RecordName, parameter.UserId, WorkflowActionCode.Reject);
                    }
                    break;
                //0: nhân viên, 1: giảng viên, 2: trợ giảng
                case SystemFeatureCode.EMPLOYEESALARY:
                    var salaryItemList = context.EmployeeMonthySalary.Where(es => es.Type == 0 && es.CommonId == parameter.FeatureId).ToList();

                    if (parameter.IsReject)
                    {
                        WorkFlowHepler.SaveNote(context, parameter.FeatureId.Value, parameter.RecordName, parameter.UserId, WorkflowActionCode.Reject);
                    }
                    int i = 0;

                    salaryItemList.ForEach(item =>
                    {
                        //if (parameter.IsSendingApprove || (parameter.IsApprove && !isFinalApprove))
                        if (parameter.IsSendingApprove && !parameter.IsApprove)
                        {
                            item.StatusId = waitingforApproveId;
                            message = CommonMessage.Workflow.CHANGE_SUCCESS;
                            if (i == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.EMPLOYEESALARY, NotificationType.APPROVE,
                                record.ApproverPersonId.Value, "Bảng lương nhân viên tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }

                        if (parameter.IsApprove && isFinalApprove)
                        {
                            item.StatusId = approvedId;
                            message = CommonMessage.Workflow.APPROVE;
                            if (i == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.EMPLOYEESALARY, NotificationType.APPROVE,
                                record.ApproverPersonId.Value, "Bảng lương nhân viên tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }
                        else if (parameter.IsReject)
                        {
                            item.StatusId = rejectedId;
                            message = CommonMessage.Workflow.REJECT;
                            if (i == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.EMPLOYEESALARY, NotificationType.REJECT,
                                record.ApproverPersonId.Value, "Bảng lương nhân viên tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }

                        i++;
                    });

                    break;
                case SystemFeatureCode.TEACHERSALARY:
                    var teacherSalaryItemList = context.EmployeeMonthySalary.Where(es => es.Type == 1 && es.CommonId == parameter.FeatureId).ToList();

                    if (parameter.IsReject)
                    {
                        WorkFlowHepler.SaveNote(context, parameter.FeatureId.Value, parameter.RecordName, parameter.UserId, WorkflowActionCode.Reject);
                    }

                    int j = 0;
                    teacherSalaryItemList.ForEach(item =>
                    {
                        //if (parameter.IsSendingApprove || (parameter.IsApprove && !isFinalApprove))
                        if (parameter.IsSendingApprove && !parameter.IsApprove)
                        {
                            item.StatusId = waitingforApproveId;
                            message = CommonMessage.Workflow.CHANGE_SUCCESS;

                            if (j == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.TEACHERSALARY, NotificationType.APPROVE,
                                record.ApproverPersonId.Value, "Bảng lương giảng viên tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }

                        if (parameter.IsApprove && isFinalApprove)
                        {
                            item.StatusId = approvedId;
                            message = CommonMessage.Workflow.APPROVE;

                            if (j == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.TEACHERSALARY, NotificationType.APPROVE,
                                record.ApproverPersonId.Value, "Bảng lương giảng viên tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }
                        else if (parameter.IsReject)
                        {
                            item.StatusId = rejectedId;
                            message = CommonMessage.Workflow.REJECT;

                            if (j == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.TEACHERSALARY, NotificationType.REJECT,
                                record.ApproverPersonId.Value, "Bảng lương giảng viên tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }

                        j++;
                    });

                    break;
                case SystemFeatureCode.ASSISTANTSALARY:
                    var assistantSalaryItemList = context.EmployeeMonthySalary.Where(es => es.Type == 2 && es.CommonId == parameter.FeatureId).ToList();

                    if (parameter.IsReject)
                    {
                        WorkFlowHepler.SaveNote(context, parameter.FeatureId.Value, parameter.RecordName, parameter.UserId, WorkflowActionCode.Reject);
                    }
                    int k = 0;
                    assistantSalaryItemList.ForEach(item =>
                    {
                        //if (parameter.IsSendingApprove || (parameter.IsApprove && !isFinalApprove))
                        if (parameter.IsSendingApprove && !parameter.IsApprove)
                        {
                            item.StatusId = waitingforApproveId;
                            message = CommonMessage.Workflow.CHANGE_SUCCESS;

                            if (k == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.ASSISTANTSALARY, NotificationType.APPROVE,
                                record.ApproverPersonId.Value, "Bảng lương trợ giảng tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }

                        if (parameter.IsApprove && isFinalApprove)
                        {
                            item.StatusId = approvedId;
                            message = CommonMessage.Workflow.APPROVE;

                            if (k == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.ASSISTANTSALARY, NotificationType.APPROVE,
                                record.ApproverPersonId.Value, "Bảng lương trợ giảng tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }
                        else if (parameter.IsReject)
                        {
                            item.StatusId = rejectedId;
                            message = CommonMessage.Workflow.REJECT;

                            if (k == 0)
                            {
                                SaveNotification(item.CommonId, ObjectType.ASSISTANTSALARY, NotificationType.REJECT,
                                record.ApproverPersonId.Value, "Bảng lương trợ giảng tháng " + item.Month + " " + record.ProgressStatus);
                            }
                        }

                        k++;
                    });

                    break;
                case SystemFeatureCode.LEAVEREQUEST:
                    var leaveRequest = context.EmployeeRequest.FirstOrDefault(er => er.EmployeeRequestId == parameter.FeatureId);


                    //if (parameter.IsSendingApprove || (parameter.IsApprove && !isFinalApprove))
                    if (parameter.IsSendingApprove && !parameter.IsApprove)
                    {
                        leaveRequest.StatusId = waitingforApproveId;
                        message = CommonMessage.Workflow.CHANGE_SUCCESS;
                        SaveNotification(leaveRequest.EmployeeRequestId, ObjectType.EMPLOYEEREQUEST, NotificationType.APPROVE,
                            leaveRequest.ApproverId.Value, "Đề xuất xin nghỉ " + leaveRequest.EmployeeRequestCode + " " + record.ProgressStatus);
                    }

                    if (parameter.IsApprove && isFinalApprove)
                    {
                        leaveRequest.StatusId = approvedId;
                        message = CommonMessage.Workflow.APPROVE;

                        var allowance = context.EmployeeAllowance.Where(ea => ea.EmployeeId == leaveRequest.OfferEmployeeId).OrderByDescending(ea => ea.EffectiveDate).FirstOrDefault();
                        if (allowance != null)
                        {
                            var paidLeaveId = commonCategory.FirstOrDefault(c => c.CategoryCode == "NP").CategoryId;
                            var numberOfPaidLeave = allowance.MaternityAllowance;
                            var currentPaidLeaveNumber = Convert.ToDecimal((leaveRequest.EnDate.Value - leaveRequest.StartDate.Value).TotalDays) + 1;
                            //if (paidLeaveId == leaveRequest.TypeRequest && numberOfPaidLeave != null)
                            //{
                            //    allowance.MaternityAllowance = numberOfPaidLeave - currentPaidLeaveNumber;
                            //}
                        }

                        SaveNotification(leaveRequest.EmployeeRequestId, ObjectType.EMPLOYEEREQUEST, NotificationType.APPROVE,
                            leaveRequest.ApproverId.Value, "Đề xuất xin nghỉ " + leaveRequest.EmployeeRequestCode + " " + record.ProgressStatus);
                    }
                    else if (parameter.IsReject)
                    {
                        leaveRequest.StatusId = rejectedId;
                        message = CommonMessage.Workflow.REJECT;
                        var currentEmpId = context.User.FirstOrDefault(u => u.UserId == leaveRequest.CreateById).EmployeeId;
                        SaveNotification(leaveRequest.EmployeeRequestId, ObjectType.EMPLOYEEREQUEST, NotificationType.REJECT,
                            currentEmpId.Value, "Đề xuất xin nghỉ " + leaveRequest.EmployeeRequestCode + " " + record.ProgressStatus);

                        WorkFlowHepler.SaveNote(context, parameter.FeatureId.Value, parameter.RecordName, parameter.UserId, WorkflowActionCode.Reject);
                    }

                    break;
                case SystemFeatureCode.PAYMENTREQUEST:
                    var paymentRequest = context.RequestPayment.FirstOrDefault(pr => pr.RequestPaymentId == parameter.FeatureId);

                    //if (parameter.IsSendingApprove || (parameter.IsApprove && !isFinalApprove))
                    if (parameter.IsSendingApprove && !parameter.IsApprove)
                    {
                        paymentRequest.StatusId = waitingforApproveId;
                        message = CommonMessage.Workflow.CHANGE_SUCCESS;
                        SaveNotification(paymentRequest.RequestPaymentId, ObjectType.PAYMENTREQUEST, NotificationType.APPROVE,
                            paymentRequest.ApproverId.Value, "Đề xuất thanh toán " + paymentRequest.RequestPaymentCode + " " + record.ProgressStatus);

                    }

                    if (parameter.IsApprove && isFinalApprove)
                    {
                        paymentRequest.StatusId = approvedId;
                        message = CommonMessage.Workflow.APPROVE;
                        SaveNotification(paymentRequest.RequestPaymentId, ObjectType.PAYMENTREQUEST, NotificationType.APPROVE,
                            paymentRequest.ApproverId.Value, "Đề xuất thanh toán " + paymentRequest.RequestPaymentCode + " " + record.ProgressStatus);
                    }
                    else if (parameter.IsReject)
                    {
                        paymentRequest.StatusId = rejectedId;
                        message = CommonMessage.Workflow.REJECT;
                        var currentEmpId = context.User.FirstOrDefault(u => u.UserId == paymentRequest.CreateById).EmployeeId;
                        SaveNotification(paymentRequest.RequestPaymentId, ObjectType.PAYMENTREQUEST, NotificationType.REJECT,
                            currentEmpId.Value, "Đề xuất thanh toán " + paymentRequest.RequestPaymentCode + " " + record.ProgressStatus);

                        WorkFlowHepler.SaveNote(context, parameter.FeatureId.Value, parameter.RecordName, parameter.UserId, WorkflowActionCode.Reject);

                    }
                    break;
                case "": break;
            }

            context.SaveChanges();

            return new NextWorkflowStepResult()
            {
                Status = true,
                Message = message
            };
        }

        public GetMasterDataCreateWorkflowResult GetMasterDataCreateWorkflow(GetMasterDataCreateWorkflowParameter parameter)
        {
            try
            {
                var listSystemFeature = new List<SystemFeature>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listStatus = new List<CategoryEntityModel>();
                var listPosition = new List<Position>();

                listSystemFeature = context.SystemFeature.ToList();
                
                listEmployee = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeName = y.EmployeeName
                }).ToList();

                var statusType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TWO");
                listStatus = context.Category
                    .Where(x => x.CategoryTypeId == statusType.CategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        }).ToList();

                listPosition = context.Position.Where(x => x.Active == true).ToList();

                return new GetMasterDataCreateWorkflowResult()
                {
                    Status = true,
                    Message = "Success",
                    ListEmployee = listEmployee,
                    ListStatus = listStatus,
                    ListPosition = listPosition,
                    ListSystemFeature = listSystemFeature
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateWorkflowResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }
    }
}
