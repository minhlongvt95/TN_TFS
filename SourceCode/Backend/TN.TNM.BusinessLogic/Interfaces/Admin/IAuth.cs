using TN.TNM.BusinessLogic.Messages.Requests.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Requests.Users;
using TN.TNM.BusinessLogic.Messages.Responses.Admin;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Responses.Users;

namespace TN.TNM.BusinessLogic.Interfaces.Admin
{
    /// <summary>
    /// TODO: using to authenticate and authorize user
    /// </summary>
    public interface IAuth
    {
        LoginResponse Login(LoginRequest request, string secretKey, string issuer, string audience);
        GetMenuByModuleCodeResponse GetMenuByModuleCode(GetMenuByModuleCodeRequest request);
        GetUserPermissionResponse GetUserPermission(GetUserPermissionRequest request);
        CreatePermissionResponse CreatePermission(CreatePermissionRequest request);
        EditPermissionByIdResponse EditPermissionById(EditPermissionByIdRequest request);
        DeletePermissionByIdResponse DeletePermissionById(DeletePermissionByIdRequest request);
        GetPermissionByIdResponse GetPermissionById(GetPermissionByIdRequest request);
        GetAllPermissionResponse GetAllPermission(GetAllPermissionRequest request);
        GetPermissionByCodeResponse GetPermissionByCode(GetPermissionByCodeRequest request);
        ChangePasswordResponse ChangePassword(ChangePasswordRequest request);
        GetUserProfileResponse GetUserProfile(GetUserProfileRequest request);
        GetUserProfileByEmailResponse GetUserProfileByEmail(GetUserProfileByEmailRequest request);
        EditUserProfileResponse EditUserProfile(EditUserProfileRequest request);
        GetModuleByPermissionSetIdResponse GetModuleByPermissionSetId(GetModuleByPermissionSetIdRequest request);
        GetAllPermissionSetNameAndCodeResponse GetAllPermissionSetNameAndCode(GetAllPermissionSetNameAndCodeRequest request);
        GetAllUserResponse GetAllUser(GetAllUserRequest request);
        GetCheckUserNameResponse GetCheckUserName(GetCheckUserNameRequest request);
        GetCheckResetCodeUserResponse GetCheckResetCodeUser(GetCheckResetCodeUserRequest request);
        ResetPasswordResponse ResetPassword(ResetPasswordRequest request);
        GetPositionCodeByPositionIdResponse GetPositionCodeByPositionId(GetPositionCodeByPositionIdRequest request);
        GetAllRoleResponse GetAllRole(GetAllRoleRequest request);
        GetCreatePermissionResponse GetCreatePermission(GetCreatePermissionRequest request);
        CreateRoleAndPermissionResponse CreateRoleAndPermission(CreateRoleAndPermissionRequest request);
        GetDetailPermissionResponse GetDetailPermission(GetDetailPermissionRequest request);
        EditRoleAndPermissionResponse EditRoleAndPermission(EditRoleAndPermissionRequest request);
        AddUserRoleResponse AddUserRole(AddUserRoleRequest request);
        DeleteRoleResponse DeleteRole(DeleteRoleRequest request);
    }
}
