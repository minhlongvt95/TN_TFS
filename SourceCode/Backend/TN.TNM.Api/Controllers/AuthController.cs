using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Requests.Users;
using TN.TNM.BusinessLogic.Messages.Responses.Admin;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Responses.Users;

/// <summary>
/// Controller for authentication and authorization
/// 
/// Author: thanhhh@tringhiatech.vn
/// Date: 16/06/2018
/// </summary>
namespace TN.TNM.Api.Controllers
{
    public class AuthController : Controller
    {
        private IAuth Auth;        
        private IConfiguration Configuration;        

        public AuthController(IConfiguration configuration, IAuth iAuth)
        {
            this.Configuration = configuration;
            this.Auth = iAuth;
        }

        /// <summary>
        /// Get Auth token
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth")]
        [HttpPost]
        [AllowAnonymous]
        public LoginResponse GetAuthToken([FromBody]LoginRequest request)
        {
            var response = this.Auth.Login(request,
                this.Configuration["secret-key-name"],
                this.Configuration["token-valid-issuer"],
                this.Configuration["token-valid-audience"]);

            return response;
        }

        /// <summary>
        /// Get Menu by module code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getmenubymodulecode")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetMenuByModuleCodeResponse GetMenuByModuleCode([FromBody] GetMenuByModuleCodeRequest request)
        {
            var response = this.Auth.GetMenuByModuleCode(request);
            return response;
        }

        /// <summary>
        /// Get current user's permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getUserPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserPermissionResponse GetUserPermission([FromBody]GetUserPermissionRequest request)
        {
            var response = this.Auth.GetUserPermission(request);
            return response;
        }

        /// <summary>
        /// Create new permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/createPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public CreatePermissionResponse CreatePermission([FromBody]CreatePermissionRequest request)
        {
            var response = this.Auth.CreatePermission(request);
            return response;
        }

        /// <summary>
        /// Edit a permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/editPermissionById")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditPermissionByIdResponse EditPermissionById([FromBody]EditPermissionByIdRequest request)
        {
            var response = this.Auth.EditPermissionById(request);
            return response;
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/deletePermissionById")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public DeletePermissionByIdResponse DeletePermissionById([FromBody]DeletePermissionByIdRequest request)
        {
            var response = this.Auth.DeletePermissionById(request);
            return response;
        }

        /// <summary>
        /// Get a permission's info
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getPermissionById")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetPermissionByIdResponse GetPermissionById([FromBody]GetPermissionByIdRequest request)
        {
            var response = this.Auth.GetPermissionById(request);
            return response;
        }

        /// <summary>
        /// Get all Permission in DB
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllPermissionResponse GetAllPermission([FromBody]GetAllPermissionRequest request)
        {
            var response = this.Auth.GetAllPermission(request);
            return response;
        }

        /// <summary>
        /// Get Permission by PermissionCode
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getPermissionByCode")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetPermissionByCodeResponse GetPermissionByCode([FromBody]GetPermissionByCodeRequest request)
        {
            var response = this.Auth.GetPermissionByCode(request);
            return response;
        }

        /// <summary>
        /// Change user's password
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/changePassword")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public ChangePasswordResponse ChangePasswordResponse([FromBody]ChangePasswordRequest request)
        {
            var response = this.Auth.ChangePassword(request);
            return response;
        }

        /// <summary>
        /// Get User profile
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getUserProfile")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserProfileResponse GetUserProfile([FromBody]GetUserProfileRequest request)
        {
            var response = this.Auth.GetUserProfile(request);
            return response;
        }

        /// <summary>
        /// Get User profile
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getUserProfileByEmail")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetUserProfileByEmailResponse GetUserProfileByEmail([FromBody]GetUserProfileByEmailRequest request)
        {
            var response = this.Auth.GetUserProfileByEmail(request);
            return response;
        }

        /// <summary>
        /// Edit User profile
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/editUserProfile")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditUserProfileResponse EditUserProfile([FromBody]EditUserProfileRequest request)
        {
            var response = this.Auth.EditUserProfile(request);
            return response;
        }

        /// <summary>
        /// GetModuleByPermissionSetId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getModuleByPermissionSetId")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetModuleByPermissionSetIdResponse GetModuleByPermissionSetId([FromBody]GetModuleByPermissionSetIdRequest request)
        {
            var response = this.Auth.GetModuleByPermissionSetId(request);
            return response;
        }

        /// <summary>
        /// GetAllPermissionSetNameAndCode
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllPermissionSetNameAndCode")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllPermissionSetNameAndCodeResponse GetAllPermissionSetNameAndCode(
            [FromBody]GetAllPermissionSetNameAndCodeRequest request)
        {
            var response = this.Auth.GetAllPermissionSetNameAndCode(request);
            return response;
        }
        /// <summary>
        /// Get All User
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllUser")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllUserResponse GetAllUser(
            [FromBody]GetAllUserRequest request)
        {
            var response = this.Auth.GetAllUser(request);
            return response;
        }

        /// <summary>
        /// Get Check User Name
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getCheckUserName")]
        [HttpPost]
        [AllowAnonymous]
        public GetCheckUserNameResponse GetCheckUserName(
            [FromBody]GetCheckUserNameRequest request)
        {
            var response = this.Auth.GetCheckUserName(request);
            return response;
        }

        /// <summary>
        /// Get Check Reset Code User
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getCheckResetCodeUser")]
        [HttpPost]
        [AllowAnonymous]
        public GetCheckResetCodeUserResponse GetCheckResetCodeUser(
            [FromBody]GetCheckResetCodeUserRequest request)
        {
            var response = this.Auth.GetCheckResetCodeUser(request);
            return response;
        }

        /// <summary>
        /// Reset Password for User
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/resetPassword")]
        [HttpPost]
        [AllowAnonymous]
        public ResetPasswordResponse ResetPassword(
            [FromBody]ResetPasswordRequest request)
        {
            var response = this.Auth.ResetPassword(request);
            return response;
        }

        /// <summary>
        /// Get PositionCode By PositionId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getPositionCodeByPositionId")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetPositionCodeByPositionIdResponse GetPositionCodeByPositionId(
            [FromBody]GetPositionCodeByPositionIdRequest request)
        {
            var response = this.Auth.GetPositionCodeByPositionId(request);
            return response;
        }

        /// <summary>
        /// Get All Role
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getAllRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetAllRoleResponse GetAllRole([FromBody]GetAllRoleRequest request)
        {
            var response = this.Auth.GetAllRole(request);
            return response;
        }

        /// <summary>
        /// Get Create Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getCreatePermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetCreatePermissionResponse GetCreatePermission([FromBody]GetCreatePermissionRequest request)
        {
            var response = this.Auth.GetCreatePermission(request);
            return response;
        }

        /// <summary>
        /// Create Role And Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/createRoleAndPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public CreateRoleAndPermissionResponse CreateRoleAndPermission([FromBody]CreateRoleAndPermissionRequest request)
        {
            var response = this.Auth.CreateRoleAndPermission(request);
            return response;
        }

        /// <summary>
        /// Get Detail Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/getDetailPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public GetDetailPermissionResponse GetDetailPermission([FromBody]GetDetailPermissionRequest request)
        {
            var response = this.Auth.GetDetailPermission(request);
            return response;
        }

        /// <summary>
        /// Edit Role And Permission
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/editRoleAndPermission")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public EditRoleAndPermissionResponse EditRoleAndPermission([FromBody]EditRoleAndPermissionRequest request)
        {
            var response = this.Auth.EditRoleAndPermission(request);
            return response;
        }

        /// <summary>
        /// Add User Role
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/addUserRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public AddUserRoleResponse AddUserRole([FromBody]AddUserRoleRequest request)
        {
            var response = this.Auth.AddUserRole(request);
            return response;
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/auth/deleteRole")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public DeleteRoleResponse DeleteRole([FromBody]DeleteRoleRequest request)
        {
            var response = this.Auth.DeleteRole(request);
            return response;
        }

    }
}
