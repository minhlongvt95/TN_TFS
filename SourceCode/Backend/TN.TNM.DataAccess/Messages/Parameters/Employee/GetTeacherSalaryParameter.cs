namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetTeacherSalaryParameter:BaseParameter
    {
        //public DateTime fTime { get; set; }
        //public DateTime eTime { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

    }
}
