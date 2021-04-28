namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Company
{
    public class ChangeSystemParameterParameter : BaseParameter
    {
        public string SystemKey { get; set; }
        public bool? SystemValue { get; set; }
        public string SystemValueString { get; set; }
        public string Description { get; set; }
    }
}
