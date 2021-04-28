namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class GetListCustomeSaleToprForDashboardParameter: BaseParameter
    {
        public string KeyName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
