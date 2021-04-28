using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetTeacherSalaryRequest : BaseRequest<GetTeacherSalaryParameter>
    {
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override GetTeacherSalaryParameter ToParameter()
        {
            return new GetTeacherSalaryParameter
            {
                Month = this.Month,
                Year = this.Year,
                UserId = this.UserId
            };
        }
    }
}
