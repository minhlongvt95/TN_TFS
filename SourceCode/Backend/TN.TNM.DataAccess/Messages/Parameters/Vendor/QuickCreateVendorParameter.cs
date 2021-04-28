namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class QuickCreateVendorParameter : BaseParameter
    {
        public Databases.Entities.Vendor Vendor { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
