using System;
using System.Linq;
using TN.TNM.Common.Constant;
using TN.TNM.Common.Resources;
using TN.TNM.DataAccess.Databases;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Helper
{
    public static class WorkFlowHepler
    {


        public static void SaveNote(TNTN8Context context,Guid recordId, string note, Guid actorId, string worflowAction)
        {
            var actor = context.Employee.FirstOrDefault(e => e.EmployeeId ==context.User.Where(w=>w.UserId==actorId).Select(s=>s.EmployeeId).FirstOrDefault());
            var position = context.Position.FirstOrDefault(c => c.PositionId == actor.PositionId);

            var featureNote = context.FeatureNote.FirstOrDefault(f => f.FeatureId == recordId);
            if (featureNote != null)
            {
                switch (worflowAction)
                {
                    case WorkflowActionCode.Edit:
                        featureNote.Note += string.Format(CommonResource.NoteTemplateString, "chỉnh sửa", position?.PositionName,
                            actor?.EmployeeName, note);
                        break;
                    case WorkflowActionCode.Reject:
                        featureNote.Note += string.Format(CommonResource.NoteTemplateString, "từ chối phê duyệt", position?.PositionName,
                            actor?.EmployeeName, note);
                        break;
                    case WorkflowActionCode.CancelSubmit:
                        featureNote.Note += string.Format(CommonResource.NoteTemplateString, "hủy gửi phê duyệt", position?.PositionName,
                            actor?.EmployeeName, note);
                        break;
                }
                context.SaveChanges();
            }
            else
            {
                FeatureNote newFeatureNote = new FeatureNote();
                newFeatureNote.Id = Guid.NewGuid();
                newFeatureNote.FeatureId = recordId;
                switch (worflowAction)
                {
                    case WorkflowActionCode.Edit:
                        newFeatureNote.Note = string.Format(CommonResource.NoteTemplateString, "chỉnh sửa", position?.PositionName,
                            actor?.EmployeeName, note);
                        break;
                    case WorkflowActionCode.Reject:
                        newFeatureNote.Note = string.Format(CommonResource.NoteTemplateString, "từ chối phê duyệt", position?.PositionName,
                            actor?.EmployeeName, note);
                        break;
                    case WorkflowActionCode.CancelSubmit:
                        newFeatureNote.Note = string.Format(CommonResource.NoteTemplateString, "hủy gửi phê duyệt", position?.PositionName,
                            actor?.EmployeeName, note);
                        break;
                }
                context.FeatureNote.Add(newFeatureNote);
                context.SaveChanges();
            }
        }

    }
}
