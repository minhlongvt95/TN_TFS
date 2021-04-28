using System;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan;
using TN.TNM.DataAccess.Messages.Results.ProcurementPlan;
using TN.TNM.DataAccess.Models.ProcurementPlan;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ProcurementPlanDAO : BaseDAO, IProcurementPlanDataAccess
    {
        public ProcurementPlanDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }
        public CreateProcurementPlanResult CreateProcurementPlan(CreateProcurementPlanParameter parameter)
        {
            iAuditTrace.Trace(ActionName.Create, ObjectName.PROCUREMENTPLAN, "Create budget", parameter.UserId);
            parameter.ProcurementPlan.ProcurementPlanId = Guid.NewGuid();
            parameter.ProcurementPlan.CreatedDate = DateTime.Now;
            var numberOfPr = context.ProcurementPlan.Count();
            parameter.ProcurementPlan.ProcurementPlanCode = "MDT" + parameter.ProcurementPlan.ProcurementYear.ToString().Substring(2,2)
                + parameter.ProcurementPlan.ProcurementMonth.ToString().PadLeft(2, '0')
            +(numberOfPr + 1).ToString("D4");
            context.ProcurementPlan.Add(parameter.ProcurementPlan);
            context.SaveChanges();
            return new CreateProcurementPlanResult
            {
                Status = true,
                Message = CommonMessage.ProcurementPlan.ADD_SUCCESS,
                ProcurementPlanId = parameter.ProcurementPlan.ProcurementPlanId
            };
        }

        public GetAllProcurementPlanResult GetAllProcurementPlan(GetAllProcurementPlanParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.PROCUREMENTPLAN, "Get all ProcurementPlan", parameter.UserId);
            var current = DateTime.Now;
            var procurementPlanList = (from pro in context.ProcurementPlan
                                       //where pro.ProcurementMonth.Value >= current.Month && pro.ProcurementYear.Value >= current.Year
                                       // hiển thị được hết tất cả các dự toán
                                       select new ProcurementPlanEntityModel
                                       {
                                           ProcurementPlanId = pro.ProcurementPlanId,
                                           ProcurementPlanCode = pro.ProcurementPlanCode,
                                           ProcurementContent = pro.ProcurementContent,
                                           ProcurementAmount = pro.ProcurementAmount,
                                           ProcurementMonth = pro.ProcurementMonth,
                                           ProcurementYear = pro.ProcurementYear,
                                           CreatedById = pro.CreatedById,
                                           CreatedDate = pro.CreatedDate,
                                           UpdatedById = pro.UpdatedById,
                                           UpdatedDate = pro.UpdatedDate
                                       }).OrderByDescending(t => t.ProcurementPlanCode).ToList();
            return new GetAllProcurementPlanResult
            {
                ProcurementPlanList = procurementPlanList

            };
        }

        public GetProcurementPlanByIdResult GetProcurementPlanById(GetProcurementPlanByIdParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.PROCUREMENTPLAN, "Get ProcurementPlan by Id", parameter.UserId);
            var procurementPlan = context.ProcurementPlan.FirstOrDefault(e => e.ProcurementPlanId == parameter.ProcurementPlanId);
            ProcurementPlanEntityModel pro = new ProcurementPlanEntityModel(procurementPlan);
            pro.ProductCategoryName = context.ProductCategory.FirstOrDefault(pc => pc.ProductCategoryId == pro.ProductCategoryId).ProductCategoryName;
            return new GetProcurementPlanByIdResult()
            {
                Status = true,
                ProcurementPlan = pro

            };
        }

        public SearchProcurementPlanResult SearchProcurementPlan(SearchProcurementPlanParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.PROCUREMENTPLAN, "Search ProcurementPlan", parameter.UserId);
            var procurementPlanList = (from pro in context.ProcurementPlan
                                       where (string.IsNullOrWhiteSpace(parameter.ProcurementPlanYear) || 
                                       pro.ProcurementYear.ToString()== parameter.ProcurementPlanYear) &&
                                       (string.IsNullOrWhiteSpace(parameter.ProcurementPlanMonth) ||
                                        pro.ProcurementMonth.ToString()== parameter.ProcurementPlanMonth)
                                       select new ProcurementPlanEntityModel
                                       {
                                           ProcurementPlanId = pro.ProcurementPlanId,
                                           ProcurementPlanCode = pro.ProcurementPlanCode,
                                           ProcurementContent = pro.ProcurementContent,
                                           ProcurementAmount = pro.ProcurementAmount,
                                           ProcurementMonth = pro.ProcurementMonth,
                                           ProcurementYear = pro.ProcurementYear,
                                           CreatedById = pro.CreatedById,
                                           CreatedDate = pro.CreatedDate,
                                           UpdatedById = pro.UpdatedById,
                                           UpdatedDate = pro.UpdatedDate
                                       }).ToList();
            return new SearchProcurementPlanResult
            {
                ProcurementPlanList = procurementPlanList

            };

        }

        public EditProcurementPlanByIdResult EditProcurementPlanById(EditProcurementPlanByIdParameter parameter)
        {
            iAuditTrace.Trace(ActionName.UPDATE, ObjectName.PROCUREMENTPLAN, "Edit Pro by Id", parameter.UserId);
            // Update ngay edit, nguoi edit
           
            parameter.ProcurementPlan.UpdatedById = parameter.UserId;
            parameter.ProcurementPlan.UpdatedDate = DateTime.Now;
            

            //Update db
            context.ProcurementPlan.Update(parameter.ProcurementPlan);
            
            context.SaveChanges();

            return new EditProcurementPlanByIdResult()
            {
                Status = true,
                Message = CommonMessage.ProcurementPlan.EDIT_SUCCESS
            };
        }

    }
}
  
