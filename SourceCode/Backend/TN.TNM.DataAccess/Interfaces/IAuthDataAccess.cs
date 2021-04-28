using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;
using TN.TNM.DataAccess.Messages.Parameters.Users;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Permission;
using TN.TNM.DataAccess.Messages.Results.Users;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IAuthDataAccess
    {
        LoginResult Login(LoginParameter paramater);
        GetMenuByModuleCodeResult GetMenuByModuleCode(GetMenuByModuleCodeParameter parameter);
        GetUserPermissionResult GetUserPermission(GetUserPermissionParameter parameter);
        CreatePermissionResult CreatePermission(CreatePermissionParameter parameter);
        EditPermissionByIdResult EditPermissionById(EditPermissionByIdParameter parameter);
        DeletePermissionByIdResult DeletePermissionById(DeletePermissionByIdParameter parameter);
        GetPermissionByIdResult GetPermissionById(GetPermissionByIdParameter parameter);
        GetAllPermissionResult GetAllPermission(GetAllPermissionParameter parameter);
        GetPermissionByCodeResult GetPermissionByCode(GetPermissionByCodeParameter parameter);
        ChangePasswordResult ChangePassword(ChangePasswordParameter parameter);
        GetUserProfileResult GetUserProfile(GetUserProfileParameter parameter);
        GetUserProfileByEmailResult GetUserProfileByEmail(GetUserProfileByEmailParameter parameter);
        EditUserProfileResult EditUserProfile(EditUserProfileParameter parameter);
        GetModuleByPermissionSetIdResult GetModuleByPermissionSetId(GetModuleByPermissionSetIdParameter parameter);
        GetAllPermissionSetNameAndCodeResult GetAllPermissionSetNameAndCode(GetAllPermissionSetNameAndCodeParameter parameter);
        GetAllUserResult GetAllUser(GetAllUserParameter parameter);
        GetCheckUserNameResult GetCheckUserName(GetCheckUserNameParameter parameter);
        GetCheckResetCodeUserResult GetCheckResetCodeUser(GetCheckResetCodeUserParameter parameter);
        ResetPasswordResult ResetPassword(ResetPasswordParameter parameter);
        GetPositionCodeByPositionIdResult GetPositionCodeByPositionId(GetPositionCodeByPositionIdParameter parameter);
        GetAllRoleResult GetAllRole(GetAllRoleParameter parameter);
        GetCreatePermissionResult GetCreatePermission(GetCreatePermissionParameter parameter);
        CreateRoleAndPermissionResult CreateRoleAndPermission(CreateRoleAndPermissionParameter parameter);
        GetDetailPermissionResult GetDetailPermission(GetDetailPermissionParameter parameter);
        EditRoleAndPermissionResult EditRoleAndPermission(EditRoleAndPermissionParameter parameter);
        AddUserRoleResult AddUserRole(AddUserRoleParameter parameter);
        DeleteRoleResult DeleteRole(DeleteRoleParameter parameter);
    }
}
