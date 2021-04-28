using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.DashboardRequest;
using TN.TNM.DataAccess.Messages.Results.DashboardRequest;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class DashboardRequestDAO : BaseDAO, IDashboardRequestDataAccess
    {
        public DashboardRequestDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllRequestResult GetAllRequest(GetAllRequestParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.DASHBOARDREQUEST, "Get all Request", parameter.UserId);
            //lay ra currentEmployee
            var currentEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentEmp = context.Employee.FirstOrDefault(e => e.EmployeeId == currentEmpId);

            //lay bang de xuat xin nghi cho nhan vien
            var empRequestList = (from er in context.EmployeeRequest
                                  join e in context.User on er.CreateById equals e.UserId
                                  join c in context.Category on er.TypeRequest equals c.CategoryId
                                  where  (currentEmp.IsManager || er.CreateById == parameter.UserId
                                  || er.CreateEmployeeId == currentEmpId || er.OfferEmployeeId==currentEmpId
                                  ||er.ApproverId == currentEmpId) &&
                                  er.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                  //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                  select new RequestDetail()
                                  {
                                      RequestId = er.EmployeeRequestId,
                                      CreateEmployeeId = er.CreateEmployeeId,
                                      CreateEmployeeName = e.UserName,
                                      RequestCode = er.EmployeeRequestCode,
                                      RequestContent = er.Detail,
                                      RequestTypeId = er.TypeRequest,
                                      RequestTypeName = "Đề xuất nghỉ phép"

                                  }).Distinct().ToList();
            //lay bang de xuat du toan
            var  procurementRequestList = (from p in context.ProcurementRequest
                                           join e in context.User on p.CreatedById equals e.UserId
                                           where (currentEmp.IsManager||p.RequestEmployeeId == currentEmpId
                                           || p.CreatedById == parameter.UserId || p.ApproverId == currentEmpId) &&
                                           p.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                           //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                           select new RequestDetail()
                                          {
                                              RequestId = p.ProcurementRequestId,
                                              CreateEmployeeId = p.CreatedById,
                                              CreateEmployeeName = e.UserName,
                                              RequestCode = p.ProcurementCode,
                                              RequestContent = p.ProcurementContent,
                                             RequestTypeName = "Đề xuất mua hàng"
                                         }).ToList();
            //lay bang de xuat phe duyet luong
            var salaryRequestList = (from emp in context.EmployeeMonthySalary
                                          join e in context.User on emp.CreateById equals e.UserId
                                          join p in context.Position on emp.EmployeePostionId equals p.PositionId
                                          where(currentEmp.IsManager || emp.CreateById == parameter.UserId 
                                          || emp.EmployeeId == currentEmpId)&& 
                                          emp.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                          //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                          select new RequestDetail()
                                          {
                                              RequestId = emp.EmployeeMonthySalaryId,
                                              CreateEmployeeId = emp.CreateById,
                                              CreateEmployeeName = e.UserName,
                                              Month = emp.Month,
                                              RequestContent = "Phê duyệt bảng lương tháng " + emp.Month,
                                              RequestTypeName = "Lương " + p.PositionName
                                          }).ToList();
            //lay de xuat phe duyet thanh toan
            var paymentRequestList = (from p in context.RequestPayment
                                     join e in context.User on p.CreateById equals e.UserId
                                     
                                     where (currentEmp.IsManager || p.CreateById == parameter.UserId
                                     || p.RequestEmployee == currentEmpId || p.ApproverId == currentEmpId)&&
                                     p.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                     //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                     select new RequestDetail()
                                     {
                                         RequestId = p.RequestPaymentId,
                                         CreateEmployeeId = p.CreateById,
                                         CreateEmployeeName = e.UserName,
                                         RequestCode = p.RequestPaymentCode,
                                         RequestContent =p.RequestPaymentNote,
                                         RequestTypeName = "Đề xuất thanh toán" 
                                     }).ToList();

            List<RequestDetail> listALL = new List<RequestDetail>();
            listALL.AddRange(empRequestList);
            listALL.AddRange(procurementRequestList);
            listALL.AddRange(salaryRequestList);
            listALL.AddRange(paymentRequestList);
            return new GetAllRequestResult
            {
                RequestList = listALL,
                Status = true,
            };
        }

        public SearchAllRequestResult SearchAllRequest(SearchAllRequestParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.DASHBOARDREQUEST, "Search all Request", parameter.UserId);
           
            var currentEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentEmp = context.Employee.FirstOrDefault(e => e.EmployeeId == currentEmpId);

            //lay bang de xuat xin nghi cho nhan vien
            var empRequestList = (from er in context.EmployeeRequest
                                  join e in context.User on er.CreateById equals e.UserId
                                  join c in context.Category on er.TypeRequest equals c.CategoryId
                                  where (currentEmp.IsManager || er.CreateById == parameter.UserId
                                  || er.CreateEmployeeId == currentEmpId || er.OfferEmployeeId == currentEmpId
                                  || er.ApproverId == currentEmpId) &&
                                  er.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                  //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                  select new RequestDetail()
                                  {
                                      RequestId = er.EmployeeRequestId,
                                      CreateEmployeeId = er.CreateEmployeeId,
                                      CreateEmployeeName = e.UserName,
                                      RequestCode = er.EmployeeRequestCode,
                                      RequestContent = er.Detail,
                                      RequestTypeId = er.TypeRequest,
                                      RequestTypeName = "Đề xuất nghỉ phép"

                                  }).Distinct().ToList();
            //lay bang de xuat du toan
            var procurementRequestList = (from p in context.ProcurementRequest
                                          join e in context.User on p.CreatedById equals e.UserId
                                          where (currentEmp.IsManager || p.RequestEmployeeId == currentEmpId
                                          || p.CreatedById == parameter.UserId || p.ApproverId == currentEmpId) &&
                                          p.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                          //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                          select new RequestDetail()
                                          {
                                              RequestId = p.ProcurementRequestId,
                                              CreateEmployeeId = p.CreatedById,
                                              CreateEmployeeName = e.UserName,
                                              RequestCode = p.ProcurementCode,
                                              RequestContent = p.ProcurementContent,
                                              RequestTypeName = "Đề xuất mua hàng"
                                          }).ToList();
          
            //lay de xuat phe duyet thanh toan
            var paymentRequestList = (from p in context.RequestPayment
                                      join e in context.User on p.CreateById equals e.UserId

                                      where (currentEmp.IsManager || p.CreateById == parameter.UserId
                                      || p.RequestEmployee == currentEmpId || p.ApproverId == currentEmpId) &&
                                      p.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                      //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                      select new RequestDetail()
                                      {
                                          RequestId = p.RequestPaymentId,
                                          CreateEmployeeId = p.CreateById,
                                          CreateEmployeeName = e.UserName,
                                          RequestCode = p.RequestPaymentCode,
                                          RequestContent = p.RequestPaymentNote,
                                          RequestTypeName = "Đề xuất thanh toán"
                                      }).ToList();

            List<RequestDetail> listALL = new List<RequestDetail>();

            if (parameter.ListSearchTypeRequest.Contains("RQP"))
            {
                listALL.AddRange(procurementRequestList);
            }
            if (parameter.ListSearchTypeRequest.Contains("RQ"))
            {
                listALL.AddRange(empRequestList);
            }
            if (parameter.ListSearchTypeRequest.Contains("RPP"))
            {
                listALL.AddRange(paymentRequestList);
            }
            if (parameter.ListSearchTypeRequest.Contains("RQS-NV"))
            {
                var empSalaryRequestList = (from emp in context.EmployeeMonthySalary
                                         join e in context.User on emp.CreateById equals e.UserId
                                         join p in context.Position on emp.EmployeePostionId equals p.PositionId
                                         where
                                         (currentEmp.IsManager || emp.CreateById == parameter.UserId
                                          || emp.EmployeeId == currentEmpId) && emp.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                         && emp.Type == 0 //chi lay luong cua nhan vien
                                         //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                         select new RequestDetail()
                                         {
                                             RequestId = emp.EmployeeMonthySalaryId,
                                             CreateEmployeeId = emp.CreateById,
                                             CreateEmployeeName = e.UserName,
                                             Month = emp.Month,
                                             RequestContent = "Phê duyệt bảng lương tháng " + emp.Month,
                                             RequestTypeName = "Lương " + p.PositionName
                                         }).ToList();
                listALL.AddRange(empSalaryRequestList);
            }
            if (parameter.ListSearchTypeRequest.Contains("RQS-GV"))
            {
                var lecturerSalaryRequestList = (from emp in context.EmployeeMonthySalary
                                            join e in context.User on emp.CreateById equals e.UserId
                                            join p in context.Position on emp.EmployeePostionId equals p.PositionId
                                            where
                                            (currentEmp.IsManager || emp.CreateById == parameter.UserId
                                          || emp.EmployeeId == currentEmpId) && emp.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                            && emp.Type == 1 //chi lay luong cua giang vien
                                                             //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                            select new RequestDetail()
                                            {
                                                RequestId = emp.EmployeeMonthySalaryId,
                                                CreateEmployeeId = emp.CreateById,
                                                CreateEmployeeName = e.UserName,
                                                Month = emp.Month,
                                                RequestContent = "Phê duyệt bảng lương tháng " + emp.Month,
                                                RequestTypeName = "Lương " + p.PositionName
                                            }).ToList();
         
                listALL.AddRange(lecturerSalaryRequestList);
            }
            if (parameter.ListSearchTypeRequest.Contains("RQS-TG"))
            {
                var supporterSalaryRequestList = (from emp in context.EmployeeMonthySalary
                                                 join e in context.User on emp.CreateById equals e.UserId
                                                 join p in context.Position on emp.EmployeePostionId equals p.PositionId
                                                 where
                                                 (currentEmp.IsManager || emp.CreateById == parameter.UserId
                                                 || emp.EmployeeId == currentEmpId)
                                                 && emp.StatusId.ToString() == "c648f48a-49b2-4019-b0b6-3479a3acd7d8"
                                                 && emp.Type == 2 //chi lay luong cua giang vien
                                                                  //chỉ lấy đề xuất có trạng thái đang chờ phê duyệt
                                                 select new RequestDetail()
                                                 {
                                                     RequestId = emp.EmployeeMonthySalaryId,
                                                     CreateEmployeeId = emp.CreateById,
                                                     CreateEmployeeName = e.UserName,
                                                     Month = emp.Month,
                                                     RequestContent = "Phê duyệt bảng lương tháng " + emp.Month,
                                                     RequestTypeName = "Lương " + p.PositionName
                                                 }).ToList();
                listALL.AddRange(supporterSalaryRequestList);
            }
            return new SearchAllRequestResult
            {
                RequestList = listALL,
                Status = true,
            };

        }
    }
}