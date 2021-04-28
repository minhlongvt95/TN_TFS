using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.CompanyConfig
{
    public class EditCompanyConfigParameter : BaseParameter
    {
        public CompanyConfiguration CompanyConfigurationObject { get; set; }
    }
}
