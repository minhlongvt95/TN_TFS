using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EditEmployeeAssessmentRequest : BaseRequest<EditEmployeeAssessmentParameter>
    {
        public EmployeeAssessmentModel ListEmployeeAssessment { get; set; }
        public override EditEmployeeAssessmentParameter ToParameter()
        {
            //var _listEmpAssment = new List<EmployeeAssessment>();
            //ListEmployeeAssessment.ForEach(_empAss =>
            //{
            //    _listEmpAssment.Add(_empAss.ToEntity());
            //});
            return new EditEmployeeAssessmentParameter
            {
                //ListEmployeeAssessment = _listEmpAssment,
                UserId = UserId
            };
        }
    }
}
