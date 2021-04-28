namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CheckDuplicatePersonalCustomerParameter : BaseParameter
    {
        public Databases.Entities.Customer Customer { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
    }
}
