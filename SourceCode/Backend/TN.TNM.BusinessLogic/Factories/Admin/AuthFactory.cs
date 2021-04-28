using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Factories.Jwt;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Requests.Users;
using TN.TNM.BusinessLogic.Messages.Responses.Admin;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission;
using TN.TNM.BusinessLogic.Messages.Responses.Users;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Jwt;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

/// <summary>
/// Authentication factory
/// Using to authenticate user and get permission for them
/// 
/// Author: thanhhh@tringhiatech.vn
/// Date: 14/06/2018
/// </summary>
namespace TN.TNM.BusinessLogic.Factories.Admin
{
    public class AuthFactory : BaseFactory, IAuth
    {
        private IAuthDataAccess iAuthDataAccess;

        public AuthFactory(IAuthDataAccess _iAuthDataAccess, ILogger<AuthFactory> _logger)
        {
            this.iAuthDataAccess = _iAuthDataAccess;
            this.logger = _logger;
        }

        public GetMenuByModuleCodeResponse GetMenuByModuleCode(GetMenuByModuleCodeRequest request)
        {
            try
            {
                this.logger.LogInformation($"Try to get permission with module code {request.ModuleCode}");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetMenuByModuleCode(parameter);

                return new GetMenuByModuleCodeResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Permissions = result.Permissions.Select(p => new PermissionModel(p)).ToList()
                };
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetMenuByModuleCodeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetUserPermissionResponse GetUserPermission(GetUserPermissionRequest request)
        {
            try
            {
                this.logger.LogInformation("Get User Permisison");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetUserPermission(parameter);

                return new GetUserPermissionResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    PermissionList = result.PermissionList
                };
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new GetUserPermissionResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public LoginResponse Login(LoginRequest request, string secretKey, string issuer, string audience)
        {
            try
            {
                //Request to database
                this.logger.LogInformation($"{request.User.UserName} try to login at {DateTime.Now}");
                var parameter = request.ToParameter();
                var loginResult = this.iAuthDataAccess.Login(parameter);
                if (loginResult.User != null)
                {
                    //Neu login thanh cong thi tao JWT token va tra ve response gom token, userid,
                    //thong bao thanh cong va trang thai dang nhap thanh cong
                    var token = new JwtTokenBuilder()
                                    .AddSecurityKey(JwtSecurityKey.Create(secretKey))
                                    .AddSubject(request.User.UserName)
                                    .AddIssuer(issuer)
                                    .AddAudience(audience)
                                    .AddClaim("MembershipId", request.User.UserId.ToString())
                                    .AddExpiry(360)
                                    .Build();

                    var response = new LoginResponse
                    {
                        StatusCode = loginResult.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                        CurrentUser = loginResult.Status ? new AuthModel
                        {
                            UserId = loginResult.User.UserId,
                            UserName = loginResult.User.UserName,
                            EmployeeId = loginResult.User.EmployeeId.Value,
                            Token = token.Value,
                            LoginTime = new DateTime(),
                            PositionId = loginResult.PositionId
                        } : null,
                        UserAvatar = loginResult.Status ? loginResult.UserAvatar : "",
                        UserFullName = loginResult.Status ? loginResult.UserFullName : "",
                        UserEmail = loginResult.Status ? loginResult.UserEmail : "",
                        IsManager = loginResult.IsManager,
                        MessageCode = loginResult.Message,
                        PermissionList = loginResult.PermissionList,
                        ListPermissionResource = loginResult.ListPermissionResource,
                        IsAdmin = loginResult.IsAdmin,
                        SystemParameterList = loginResult.SystemParameterList,
                        IsOrder = loginResult.IsOrder,
                        IsCashier = loginResult.IsCashier,
                        ListMenuBuild = loginResult.ListMenuBuild
                    };
                    return response;
                }
                else
                {
                    return new LoginResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.NotAcceptable,
                        MessageCode = CommonMessage.Login.WRONG_USER_PASSWORD
                    };
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new LoginResponse
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public CreatePermissionResponse CreatePermission(CreatePermissionRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Permisison");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.CreatePermission(parameter);

                return new CreatePermissionResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = result.Message
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new CreatePermissionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public EditPermissionByIdResponse EditPermissionById(EditPermissionByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Permisison by Id");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.EditPermissionById(parameter);

                return new EditPermissionByIdResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = result.Message
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new EditPermissionByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public DeletePermissionByIdResponse DeletePermissionById(DeletePermissionByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Delete Permisison by Id");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.DeletePermissionById(parameter);

                return new DeletePermissionByIdResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new DeletePermissionByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetPermissionByIdResponse GetPermissionById(GetPermissionByIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Permisison by Id");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetPermissionById(parameter);

                var response = new GetPermissionByIdResponse {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = result.Message,
                    Permission = new PermissionSetModel(result.Permission)
                };

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetPermissionByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetAllPermissionResponse GetAllPermission(GetAllPermissionRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Permisison");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetAllPermission(parameter);

                var response = new GetAllPermissionResponse() {
                    PermissionList = new List<PermissionSetModel>(),
                    StatusCode = HttpStatusCode.Accepted
                };

                result.PermissionList.ForEach(p => {
                    response.PermissionList.Add(new PermissionSetModel(p));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetAllPermissionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetPermissionByCodeResponse GetPermissionByCode(GetPermissionByCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Permisison by code");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetPermissionByCode(parameter);

                var response = new GetPermissionByCodeResponse()
                {
                    PermissionList = new List<PermissionModel>(),
                    StatusCode = HttpStatusCode.Accepted
                };

                result.PermissionList.ForEach(p => {
                    response.PermissionList.Add(new PermissionModel(p));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetPermissionByCodeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                this.logger.LogInformation("Change user Password");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.ChangePassword(parameter);

                var response = new ChangePasswordResponse() {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new ChangePasswordResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetUserProfileResponse GetUserProfile(GetUserProfileRequest request)
        {
            try
            {
                this.logger.LogInformation("Get user profile");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetUserProfile(parameter);

                var response = new GetUserProfileResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Email = result.Email,
                    Username = result.Username,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    AvatarUrl = result.AvatarUrl
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GetUserProfileResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetUserProfileByEmailResponse GetUserProfileByEmail(GetUserProfileByEmailRequest request)
        {
            try
            {
                this.logger.LogInformation("Get user profile");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetUserProfileByEmail(parameter);

                var response = new GetUserProfileByEmailResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    UserId = result.UserId,
                    Email = result.Email,
                    UserName = result.UserName,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    FullName = result.FullName,
                    AvatarUrl = result.AvatarUrl,
                    PermissionList = result.PermissionList,
                    PositionId=result.PositionId,
                    IsManager = result.IsManager,
                    EmployeeId = result.EmployeeId
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GetUserProfileByEmailResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public EditUserProfileResponse EditUserProfile(EditUserProfileRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit user profile");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.EditUserProfile(parameter);

                var response = new EditUserProfileResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new EditUserProfileResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetModuleByPermissionSetIdResponse GetModuleByPermissionSetId(GetModuleByPermissionSetIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all Permisison");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetModuleByPermissionSetId(parameter);

                var response = new GetModuleByPermissionSetIdResponse()
                {
                    PermissionListAsModule = new List<PermissionModel>(),
                    StatusCode = HttpStatusCode.Accepted
                };

                result.PermissionListAsModule.ForEach(p => {
                    response.PermissionListAsModule.Add(new PermissionModel(p));
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetModuleByPermissionSetIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetAllPermissionSetNameAndCodeResponse GetAllPermissionSetNameAndCode(
            GetAllPermissionSetNameAndCodeRequest request)
        {
            try
            {
                this.logger.LogInformation("Get all PermisisonSet Name and ");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetAllPermissionSetNameAndCode(parameter);

                var response = new GetAllPermissionSetNameAndCodeResponse()
                {
                    PermissionSetCodeList = new List<string>(),
                    PermissionSetNameList = new List<string>(),
                    StatusCode = HttpStatusCode.Accepted
                };

                result.PermissionSetNameList.ForEach(p => {
                    response.PermissionSetNameList.Add(p);
                });
                result.PermissionSetCodeList.ForEach(p => {
                    response.PermissionSetCodeList.Add(p);
                });

                return response;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                return new GetAllPermissionSetNameAndCodeResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetAllUserResponse GetAllUser(GetAllUserRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All User");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetAllUser(parameter);

                var response = new GetAllUserResponse()
                {
                    MessageCode = result.Message,
                    lstUserEntityModel = new List<UserModel>(),
                    StatusCode = HttpStatusCode.Accepted
                };
                result.lstUserEntityModel.ForEach(item => { response.lstUserEntityModel.Add(new UserModel(item)); });

                return response;

            }
            catch (Exception ex)
            {
                return new GetAllUserResponse
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetCheckUserNameResponse GetCheckUserName(GetCheckUserNameRequest request)
        {
            try
            {
                this.logger.LogInformation("Get check user name");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetCheckUserName(parameter);

                var response = new GetCheckUserNameResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    UserId = result.UserId,
                    UserName = result.UserName,
                    FullName = result.FullName,
                    EmailAddress = result.EmailAddress
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GetCheckUserNameResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetCheckResetCodeUserResponse GetCheckResetCodeUser (GetCheckResetCodeUserRequest request)
        {
            try
            {
                this.logger.LogInformation("Get check reset code user");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetCheckResetCodeUser(parameter); 

                var response = new GetCheckResetCodeUserResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    UserId = result.UserId,
                    UserName = result.UserName
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GetCheckResetCodeUserResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public ResetPasswordResponse ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                this.logger.LogInformation("Get check reset code user");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.ResetPassword(parameter);

                var response = new ResetPasswordResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.Accepted : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new ResetPasswordResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetPositionCodeByPositionIdResponse GetPositionCodeByPositionId(GetPositionCodeByPositionIdRequest request)
        {
            try
            {
                this.logger.LogInformation("Get PositionCode By PositionId");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetPositionCodeByPositionId(parameter);

                var response = new GetPositionCodeByPositionIdResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    PositionCode = result.PositionCode,
                    OrganizationId = result.OrganizationId,
                    OrganizationName = result.OrganizationName,
                    lstResult = result.lstResult
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new GetPositionCodeByPositionIdResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetAllRoleResponse GetAllRole(GetAllRoleRequest request)
        {
            try
            {
                this.logger.LogInformation("Get All Role");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetAllRole(parameter);

                var response = new GetAllRoleResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListRole = new List<RoleModel>()
                };
                result.ListRole.ForEach(item =>
                {
                    response.ListRole.Add(new RoleModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new GetAllRoleResponse()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = "common.messages.exception"
                };
            }
        }

        public GetCreatePermissionResponse GetCreatePermission(GetCreatePermissionRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Create Permission");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetCreatePermission(parameter);

                var response = new GetCreatePermissionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListActionResource = new List<ActionResourceModel>(),
                    ListMenuBuild = result.ListMenuBuild
                };
                result.ListActionResource.ForEach(item =>
                {
                    response.ListActionResource.Add(new ActionResourceModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetCreatePermissionResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateRoleAndPermissionResponse CreateRoleAndPermission(CreateRoleAndPermissionRequest request)
        {
            try
            {
                this.logger.LogInformation("Create Role And Permission");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.CreateRoleAndPermission(parameter);

                var response = new CreateRoleAndPermissionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateRoleAndPermissionResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDetailPermissionResponse GetDetailPermission(GetDetailPermissionRequest request)
        {
            try
            {
                this.logger.LogInformation("Get Detail Permission");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.GetDetailPermission(parameter);

                var response = new GetDetailPermissionResponse()
                {
                    Role = result.Role,
                    ListActionResource = new List<ActionResourceModel>(),
                    ListCurrentActionResource = new List<ActionResourceModel>(),
                    ListMenuBuild = result.ListMenuBuild,
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };

                result.ListActionResource.ForEach(item =>
                {
                    response.ListActionResource.Add(new ActionResourceModel(item));
                });

                result.ListCurrentActionResource.ForEach(item =>
                {
                    response.ListCurrentActionResource.Add(new ActionResourceModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDetailPermissionResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public EditRoleAndPermissionResponse EditRoleAndPermission(EditRoleAndPermissionRequest request)
        {
            try
            {
                this.logger.LogInformation("Edit Role And Permission");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.EditRoleAndPermission(parameter);

                var response = new EditRoleAndPermissionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new EditRoleAndPermissionResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public AddUserRoleResponse AddUserRole(AddUserRoleRequest request)
        {
            try
            {
                this.logger.LogInformation("Add User Role");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.AddUserRole(parameter);

                var response = new AddUserRoleResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new AddUserRoleResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteRoleResponse DeleteRole(DeleteRoleRequest request)
        {
            try
            {
                this.logger.LogInformation("Delete Role");
                var parameter = request.ToParameter();
                var result = this.iAuthDataAccess.DeleteRole(parameter);

                var response = new DeleteRoleResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                return new DeleteRoleResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
    }
}
