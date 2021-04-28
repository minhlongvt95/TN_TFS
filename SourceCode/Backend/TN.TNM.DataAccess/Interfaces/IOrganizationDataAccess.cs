using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;
using TN.TNM.DataAccess.Messages.Results.Admin.Organization;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IOrganizationDataAccess
    {
        GetAllOrganizationResult GetAllOrganization(GetAllOrganizationParameter parameter);
        CreateOrganizationResult CreateOrganization(CreateOrganizationParameter parameter);
        GetOrganizationByIdResult GetOrganizationById(GetOrganizationByIdParameter parameter);
        EditOrganizationByIdResult EditOrganizationById(EditOrganizationByIdParameter parameter);
        DeleteOrganizationByIdResult DeleteOrganizationById(DeleteOrganizationByIdParameter parameter);
        GetAllOrganizationCodeResult GetAllOrganizationCode(GetAllOrganizationCodeParameter parameter);
        GetFinancialindependenceOrgResult GetFinancialindependenceOrg(GetFinancialindependenceOrgParameter parameter);
        GetChildrenOrganizationByIdResult GetChildrenOrganizationById(GetChildrenOrganizationByIdParameter parameter);
        GetOrganizationByEmployeeIdResult GetOrganizationByEmployeeId(GetOrganizationByEmployeeIdParameter parameter);
        GetChildrenByOrganizationIdResult GetChildrenByOrganizationId(GetChildrenByOrganizationIdParameter parameter);
        UpdateOrganizationByIdResult UpdateOrganizationById(UpdateOrganizationByIdParameter parameter);
        GetOrganizationByUserResult GetOrganizationByUser(GetOrganizationByUserParameter parameter);
    }
}
