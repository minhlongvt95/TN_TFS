namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class UpdateVendorByIdParameter : BaseParameter
    {
        public Databases.Entities.Vendor Vendor { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
    }
}
