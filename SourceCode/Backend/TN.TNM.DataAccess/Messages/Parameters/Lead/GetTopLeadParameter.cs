namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetTopLeadParameter : BaseParameter
    {
        public int Count { get; set; }
        public string StatusCode { get; set; }
    }
}
