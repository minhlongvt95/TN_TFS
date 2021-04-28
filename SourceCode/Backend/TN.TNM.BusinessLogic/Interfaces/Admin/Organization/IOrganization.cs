using TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.Organization
{
    public interface IOrganization
    {
        GetAllOrganizationResponse GetAllOrganization(GetAllOrganizationRequest request);
        CreateOrganizationResponse CreateOrganization(CreateOrganizationRequest request);
        GetOrganizationByIdResponse GetOrganizationById(GetOrganizationByIdRequest request);
        EditOrganizationByIdResponse EditOrganizationById(EditOrganizationByIdRequest request);
        DeleteOrganizationByIdResponse DeleteOrganizationById(DeleteOrganizationByIdRequest request);
        GetAllOrganizationCodeResponse GetAllOrganizationCode(GetAllOrganizationCodeRequest request);
        GetFinancialindependenceOrgResponse GetFinancialindependenceOrg(GetFinancialindependenceOrgRequest request);
        GetChildrenOrganizationByIdResponse GetChildrenOrganizationById(GetChildrenOrganizationByIdRequest request);
        GetOrganizationByEmployeeIdResponse GetOrganizationByEmployeeId(GetOrganizationByEmployeeIdRequest request);
        GetChildrenByOrganizationIdResponse GetChildrenByOrganizationId(GetChildrenByOrganizationIdRequest request);
        UpdateOrganizationByIdResponse UpdateOrganizationById(UpdateOrganizationByIdRequest request);
        GetOrganizationByUserResponse GetOrganizationByUser(GetOrganizationByUserRequest request);
    }
}
