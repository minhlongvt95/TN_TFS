using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;
using TN.TNM.DataAccess.Messages.Parameters.Users;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Permission;
using TN.TNM.DataAccess.Messages.Results.Users;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.MenuBuild;
using TN.TNM.DataAccess.Models.Permission;
using TN.TNM.DataAccess.Models.User;

/// <summary>
/// Authentication Data Access Object
/// Use to authenticate and authorize user
/// 
/// Author: thanhhh@tringhiatech.vn
/// Date: 14/06/2018
/// </summary>
namespace TN.TNM.DataAccess.Databases.DAO
{
    public class AuthDAO : BaseDAO, IAuthDataAccess
    {
        public AuthDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetMenuByModuleCodeResult GetMenuByModuleCode(GetMenuByModuleCodeParameter parameter)
        {
            ////Get list menu directly by User Id
            //var perByUser = from p in this.context.Permission
            //                join pM in this.context.PermissionMapping on p.PermissionId equals pM.PermissionId
            //                join pR in this.context.Permission on p.ParentId equals pR.PermissionId
            //                where pR.PermissionCode == parameter.ModuleCode && pM.UserId == parameter.UserId && "S".Equals(p.Type)
            //                orderby p.Sort ascending
            //                select p;

            ////Get list menu by group where user in
            //var perByGroup = from p in this.context.Permission
            //                 join pR in this.context.Permission on p.ParentId equals pR.PermissionId
            //                 join pM in this.context.PermissionMapping on p.PermissionId equals pM.PermissionId
            //                 join uG in this.context.GroupUser on pM.GroupId equals uG.GroupId
            //                 where pR.PermissionCode == parameter.ModuleCode && uG.UserId == parameter.UserId && "S".Equals(p.Type)
            //                 orderby p.Sort ascending
            //                 select p;

            //var permissions = perByUser.Union(perByGroup);
            //return new GetMenuByModuleCodeResult
            //{
            //    Permissions = permissions.ToList()
            //};

            //Lay ra permission cha theo module code truyen vao

            var parentPermissionId = context.Permission.FirstOrDefault(p => p.PermissionCode == parameter.ModuleCode) != null ? context.Permission.FirstOrDefault(p => p.PermissionCode == parameter.ModuleCode).PermissionId : Guid.Empty;

            //Lay ra cac permission con cua permission cha
            List<Permission> perChildList = new List<Permission>();
            if (parentPermissionId != Guid.Empty)
            {
                perChildList = context.Permission.Where(p => p.ParentId == parentPermissionId && p.Type.Equals("S")).ToList();
            }

            //Lay nhom quyen cua user theo user ID
            var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == parameter.UserId);
            if (permissionSetOfUser != null)
            {
                var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                var permissionIdList = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";").ToList();

                //Kiem tra neu nhom quyen con khong ton tai trong nhom quyen cua user => remove khoi danh sach hien thi
                if (perChildList.Count > 0)
                {
                    List<Permission> newList = new List<Permission>();
                    List<Permission> listRemove = new List<Permission>();
                    perChildList.ForEach(perChild => {
                        if (permissionIdList.IndexOf(perChild.PermissionId.ToString()) == -1)
                        {
                            listRemove.Add(perChild);
                        } else
                        {
                            newList.Add(perChild);
                        }
                    });

                    perChildList = newList;
                }
            }

            return new GetMenuByModuleCodeResult
            {
                Permissions = perChildList
            };
        }

        public LoginResult Login(LoginParameter paramater)
        {
            var user = this.context.User.FirstOrDefault(u =>
                u.UserName == paramater.User.UserName && u.Password == paramater.User.Password);

            if (user == null)
            {
                return new LoginResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.WRONG_USER_PASSWORD
                };
            }

            if (user.Active == false)
            {
                return new LoginResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.INACTIVE_USER,
                    User = user
                };
            }

            bool isAdmin = user.IsAdmin == null ? false : user.IsAdmin.Value;
            var empId = user.EmployeeId;
            string userFullName = "";
            string userAvatar = "";
            string userEmail = "";
            bool isManager = false;
            bool isOrder = false;
            bool isCashier = false;
            Guid? positionId = Guid.Empty;
            List<string> perCodeList = new List<string>();
            List<SystemParameter> systemParameterList = new List<SystemParameter>();
            List<string> listTextActionResource = new List<string>();
            var ListMenuBuild = new List<MenuBuildEntityModel>();

            if (empId != null)
            {
                userFullName = context.Employee.FirstOrDefault(e => e.EmployeeId == empId)?.EmployeeName;
                userAvatar = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "EMP")?
                    .AvatarUrl;
                userEmail = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "EMP")?.Email;
                var manager = context.Employee.FirstOrDefault(e => e.EmployeeId == empId);
                if (manager != null)
                {
                    isManager = manager.IsManager;
                    positionId = manager.PositionId;
                    isOrder = manager.IsOrder ?? false;
                    isCashier = manager.IsCashier ?? false;
                }
                var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == user.UserId);
                if (permissionSetOfUser != null)
                {
                    var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                    var permissionIdList = context.PermissionSet
                        .FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";")
                        .ToList();
                    permissionIdList.ForEach(perId =>
                    {
                        if (!string.IsNullOrEmpty(perId))
                        {
                            var perCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId))
                                .PermissionCode;
                            perCodeList.Add(perCode);
                        }
                    });
                }

                systemParameterList = context.SystemParameter.ToList();

                //Lấy list User Role
                var listUserRole = context.UserRole.Where(e => e.UserId == user.UserId).ToList();
                List<Guid> listRoleId = new List<Guid>();
                if (listUserRole.Count > 0)
                {
                    listUserRole.ForEach(item =>
                    {
                        listRoleId.Add(item.RoleId.Value);
                    });
                }

                //Lấy list Action Resource Id
                var listActionResource =
                    context.RoleAndPermission.Where(e => listRoleId.Contains(e.RoleId.Value)).ToList();
                List<Guid> listActionResourceId = new List<Guid>();
                if (listActionResource.Count > 0)
                {
                    listActionResource.ForEach(item =>
                    {
                        listActionResourceId.Add(item.ActionResourceId.Value);
                    });
                }

                //Lấy list text action resource
                listTextActionResource = context.ActionResource
                    .Where(e => listActionResourceId.Contains(e.ActionResourceId)).Select(x => x.ActionResource1)
                    .ToList();

                #region Lấy list MenuBuid

                ListMenuBuild = context.MenuBuild.Select(y => new MenuBuildEntityModel
                {
                    MenuBuildId = y.MenuBuildId,
                    ParentId = y.ParentId,
                    Name = y.Name,
                    Code = y.Code,
                    CodeParent = y.CodeParent,
                    Path = y.Path,
                    NameIcon = y.NameIcon,
                    Level = y.Level,
                    IndexOrder = y.IndexOrder,
                    IsPageDetail = y.IsPageDetail
                }).OrderBy(z => z.IndexOrder).ToList();

                //Lấy list đường dẫn mặc định gắn với nhóm quyền
                //Nếu user được phân duy nhất 1 quyền
                if (listRoleId.Count == 1)
                {
                    var roleId = listRoleId.FirstOrDefault();

                    var listRoleAndMenuBuild = context.RoleAndMenuBuild.Where(x => x.RoleId == roleId).ToList();
                    ListMenuBuild.ForEach(item =>
                    {
                        //Lấy ra các sub menu module
                        if (item.Level == 1)
                        {
                            var existsDefaultPath = listRoleAndMenuBuild.FirstOrDefault(x => x.Code == item.Code);

                            //Nếu tồn tại đường dẫn mặc định đã được cấu hình
                            if (existsDefaultPath != null)
                            {
                                item.Path = existsDefaultPath.Path;
                            }
                        }
                    });
                }

                #endregion

            }
            
            return new LoginResult
            {
                Status = true,
                User = user,
                UserFullName = userFullName,
                UserAvatar = userAvatar,
                UserEmail = userEmail,
                IsManager = isManager,
                PermissionList = perCodeList,
                PositionId = positionId,
                ListPermissionResource = listTextActionResource,
                IsAdmin = isAdmin,
                SystemParameterList = systemParameterList,
                IsOrder = isOrder,
                IsCashier = isCashier,
                ListMenuBuild = ListMenuBuild
            };
        }

        public GetUserPermissionResult GetUserPermission(GetUserPermissionParameter parameter)
        {
            //var perByUser = from p in context.Permission
            //                join pM in context.PermissionMapping on p.PermissionId equals pM.PermissionId
            //                join pR in context.Permission on p.ParentId equals pR.PermissionId
            //                where pM.UserId == parameter.UserId
            //                orderby p.Sort ascending
            //                select p;
            //return new GetUserPermissionResult()
            //{
            //    Status = true,
            //    PermissionList = perByUser.Select(p => p.PermissionCode).ToList()
            //};
            List<string> perCodeList = new List<string>();
            var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == parameter.UserId);
            if(permissionSetOfUser != null)
            {
                var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                var permissionIdList = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";").ToList();
                permissionIdList.ForEach(perId => {
                    var perCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId)).PermissionCode;
                    perCodeList.Add(perCode);
                });
            }
            return new GetUserPermissionResult()
            {
                Status = true,
                PermissionList = perCodeList
            };
        }

        public CreatePermissionResult CreatePermission(CreatePermissionParameter parameter)
        {
            string permissionList = string.Join(";", parameter.PermissionIdList);

            parameter.PermissionIdList.ToList().ForEach(permissionId => {
                var permission = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(permissionId) && p.ParentId == null);
                if (permission != null)
                {
                    var permissionAsMenu = context.Permission.Where(p => p.ParentId == Guid.Parse(permissionId) && p.Type == "S").Select(p => p.PermissionId).ToList();
                    string permissionAsMenuString = string.Join(";", permissionAsMenu);
                    permissionList += ";" + permissionAsMenuString;
                }
            });

            PermissionSet ps = new PermissionSet() {
                PermissionSetId = Guid.NewGuid(),
                PermissionSetName = parameter.PermissionSetName.Trim(),
                PermissionSetCode = parameter.PermissionSetCode.Trim(),
                PermissionSetDescription = parameter.PermissionSetDescription.Trim(),
                PermissionId = permissionList,
                CreatedById = parameter.UserId,
                CreatedDate = DateTime.Now,
                Active = true
            };

            context.PermissionSet.Add(ps);
            context.SaveChanges();
            return new CreatePermissionResult()
            {
                Status = true,
                Message = CommonMessage.Permission.CREATE_SUCCESS
            };
        }

        public EditPermissionByIdResult EditPermissionById(EditPermissionByIdParameter parameter)
        {
            var permission = context.PermissionSet.FirstOrDefault(p => p.PermissionSetId == parameter.PermissionSetId);
            permission.PermissionSetName = parameter.PermissionSetName.Trim();
            permission.PermissionSetDescription = parameter.PermissionSetDescription.Trim();
            permission.PermissionSetCode = parameter.PermissionSetCode.Trim();
            permission.PermissionId = string.Join(";", parameter.PermissionIdList.ToList());

            context.PermissionSet.Update(permission);
            context.SaveChanges();
            return new EditPermissionByIdResult()
            {
                Status = true,
                Message = CommonMessage.Permission.EDIT_SUCCESS
            };
        }

        public GetAllPermissionResult GetAllPermission(GetAllPermissionParameter parameter)
        {
            var perLst = context.PermissionSet.Select(p => new PermissionSetEntityModel() {
                PermissionSetId = p.PermissionSetId,
                PermissionSetName = p.PermissionSetName,
                PermissionSetCode = p.PermissionSetCode,
                PermissionSetDescription = p.PermissionSetDescription, 
                NumberOfUserHasPermission = context.PermissionMapping.Where(pm => pm.PermissionSetId == p.PermissionSetId).Count(),
                CreatedById = p.CreatedById,
                CreatedDate = p.CreatedDate
            }).OrderByDescending(ps => ps.CreatedDate).ToList();

            return new GetAllPermissionResult
            {
                PermissionList = perLst,
                Status = true
            };
        }

        public GetPermissionByIdResult GetPermissionById(GetPermissionByIdParameter parameter)
        {
            var permissionSet = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == parameter.PermissionSetId);
            var permissionIdList = permissionSet.PermissionId.Split(";").ToList();
            List<PermissionEntityModel> permissionList = new List<PermissionEntityModel>();
            permissionIdList.ForEach(permissionId => {
                var permission = context.Permission.FirstOrDefault(p => p.PermissionId.ToString() == permissionId);

                if(permission != null)
                {
                    PermissionEntityModel pem = new PermissionEntityModel()
                    {
                        PermissionId = permission.PermissionId,
                        PermissionName = permission.PermissionName,
                        PermissionCode = permission.PermissionCode,
                        ParentId = permission.ParentId
                    };

                    permissionList.Add(pem);
                }
            });

            PermissionSetEntityModel psem = new PermissionSetEntityModel()
            {
                PermissionSetId = permissionSet.PermissionSetId,
                PermissionSetName = permissionSet.PermissionSetName,
                PermissionSetDescription = permissionSet.PermissionSetDescription,
                PermissionSetCode = permissionSet.PermissionSetCode,
                NumberOfUserHasPermission = context.PermissionMapping.Where(pm => pm.PermissionSetId == permissionSet.PermissionSetId).Count(),
                PermissionList = permissionList
            };

            return new GetPermissionByIdResult()
            {
                Status = true,
                Permission = psem
            };
        }

        public DeletePermissionByIdResult DeletePermissionById(DeletePermissionByIdParameter parameter)
        {
            var permissionMapping = context.PermissionMapping.Where(pm => pm.PermissionSetId == parameter.PermissionSetId).ToList();
            if(permissionMapping.Count > 0)
            {
                return new DeletePermissionByIdResult()
                {
                    Status = false,
                    Message = CommonMessage.Permission.HAS_USER
                };
            }

            var permission = context.PermissionSet.FirstOrDefault(p => p.PermissionSetId == parameter.PermissionSetId);
            if (permission == null)
            {
                return new DeletePermissionByIdResult()
                {
                    Status = false,
                    Message = CommonMessage.Permission.NOT_EXIST
                };
            }

            context.PermissionSet.Remove(permission);
            context.SaveChanges();
            return new DeletePermissionByIdResult()
            {
                Status = true,
                Message = CommonMessage.Permission.DELETE_SUCCESS
            };
        }

        public ChangePasswordResult ChangePassword(ChangePasswordParameter parameter)
        {
            var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
            var currentPass = user.Password;
            var oldPass = AuthUtil.GetHashingPassword(parameter.OldPassword);

            if (oldPass != currentPass)
            {
                return new ChangePasswordResult() {
                    Status = false,
                    Message = CommonMessage.Password.NOT_CORRECT
                };
            }

            var newPass = AuthUtil.GetHashingPassword(parameter.NewPassword);
            if (newPass == currentPass)
            {
                return new ChangePasswordResult()
                {
                    Status = false,
                    Message = CommonMessage.Password.DUPLICATE
                };
            }

            user.Password = newPass;

            context.User.Update(user);
            context.SaveChanges();

            return new ChangePasswordResult()
            {
                Status = true,
                Message = CommonMessage.Password.CHANGE_FAIL
            };
        }

        public GetPermissionByCodeResult GetPermissionByCode(GetPermissionByCodeParameter parameter)
        {
            var parentPermission = context.Permission.Where(p => parameter.PerCode.IndexOf(p.PermissionCode) > -1).Select(parent => new PermissionEntityModel(){
                PermissionId = parent.PermissionId,
                PermissionName = parent.PermissionName,
                PermissionChildList = context.Permission.Where(p => p.ParentId == parent.PermissionId && p.Type == "P").Select(p => new PermissionEntityModel()
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    PermissionCode = p.PermissionCode,
                    ParentId = p.ParentId,
                    PermissionDescription = p.PermissionDescription,
                    Sort = p.Sort
                }).OrderBy(p => p.Sort).ToList()
            }).ToList();
            return new GetPermissionByCodeResult() {
                PermissionList = parentPermission,
                Status = true
            };
        }

        private List<PermissionEntityModel> GetChildren(Guid? id, List<PermissionEntityModel> list)
        {
            return list.Where(p => p.ParentId == id)
                .Select(p => new PermissionEntityModel()
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    PermissionCode = p.PermissionCode,
                    ParentId = p.ParentId,
                    PermissionDescription = p.PermissionDescription,
                    PermissionChildList = GetChildren(p.PermissionId, list)
                }).ToList();
        }

        public EditUserProfileResult EditUserProfile(EditUserProfileParameter parameter)
        {
            var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
            var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
            var contact = context.Contact.FirstOrDefault(c => c.ObjectId == employee.EmployeeId);

            user.UserName = parameter.Username;
            contact.FirstName = parameter.FirstName;
            contact.LastName = parameter.LastName;
            contact.Email = parameter.Email;
            contact.AvatarUrl = parameter.AvatarUrl;

            context.User.Update(user);
            context.Contact.Update(contact);
            context.SaveChanges();
            return new EditUserProfileResult()
            {
                Status = true,
                Message = CommonMessage.User.CHANGE_SUCCESS
            };
        }

        public GetUserProfileResult GetUserProfile(GetUserProfileParameter parameter)
        {
            var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
            var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
            var contact = context.Contact.FirstOrDefault(c => c.ObjectId == employee.EmployeeId);
            return new GetUserProfileResult()
            {
                Status = true,
                AvatarUrl = contact.AvatarUrl,
                Email = contact.Email,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Username = user.UserName
            };
        }

        public GetUserProfileByEmailResult GetUserProfileByEmail(GetUserProfileByEmailParameter parameter)
        {            
            var contact = context.Contact.FirstOrDefault(c => c.Email == parameter.UserEmail 
            || c.OtherEmail == parameter.UserEmail
            || c.WorkEmail == parameter.UserEmail);            
            var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == contact.ObjectId);
            var user = context.User.FirstOrDefault(u => u.EmployeeId == employee.EmployeeId);

            if (user.Active == false)
            {
                return new GetUserProfileByEmailResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.INACTIVE_USER,
                    AvatarUrl = contact.AvatarUrl,
                    Email = contact.Email,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    UserName = user.UserName,
                    FullName = employee.EmployeeName,
                    UserId = user.UserId,
                    EmployeeId = employee.EmployeeId,
                    PermissionList = new List<string>(),
                    IsManager = false
                };
            }

            var empId = user.EmployeeId;            
            bool isManager = false;
            Guid? positionId = Guid.Empty;
            List<string> perCodeList = new List<string>();
            if (empId != null)
            {
                isManager=employee.IsManager;
                positionId = employee.PositionId;
                
                var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == user.UserId);
                if (permissionSetOfUser != null)
                {
                    var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                    var permissionIdList = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";").ToList();
                    permissionIdList.ForEach(perId => {
                        if (!string.IsNullOrEmpty(perId))
                        {
                            var perCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId)).PermissionCode;
                            var parentId = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId)).ParentId;
                            
                            if (parentId != null)
                            {
                                var parentCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(parentId.ToString())).PermissionCode;
                                if (perCodeList.IndexOf(parentCode.ToString()) == -1)
                                {
                                    perCodeList.Add(parentCode.ToString());
                                }
                            }
                            perCodeList.Add(perCode);
                        }
                    });
                }
            }
            this.iAuditTrace.Trace(ActionName.LOGIN, ObjectName.USER, $"User {user.UserName} login", user.UserId);

            return new GetUserProfileByEmailResult()
            {
                Status = true,
                AvatarUrl = contact.AvatarUrl,
                Email = contact.Email,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                UserName = user.UserName,
                FullName = employee.EmployeeName,
                UserId = user.UserId,
                PermissionList = perCodeList,
                IsManager = isManager,
                PositionId= positionId,
                EmployeeId = employee.EmployeeId,
            };
        }

        public GetModuleByPermissionSetIdResult GetModuleByPermissionSetId(GetModuleByPermissionSetIdParameter parameter)
        {
            var permissionSet = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == parameter.PermissionSetId);
            List<PermissionEntityModel> permissionList = new List<PermissionEntityModel>();
            if (permissionSet != null)
            {
                var permissionIdList = permissionSet.PermissionId.Split(";").ToList();

                if (permissionIdList.Count > 0)
                {
                    permissionIdList.ForEach(perId => {
                        var parent = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId) && p.ParentId == null);
                        if (parent != null)
                        {
                            PermissionEntityModel pm = new PermissionEntityModel()
                            {
                                PermissionId = parent.PermissionId,
                                PermissionName = parent.PermissionName
                            };
                            permissionList.Add(pm);
                        }
                    });
                }
            }

            return new GetModuleByPermissionSetIdResult() {
                Status = true,
                PermissionListAsModule = permissionList
            };
        }

        public GetAllPermissionSetNameAndCodeResult GetAllPermissionSetNameAndCode(
            GetAllPermissionSetNameAndCodeParameter parameter)
        {
            List<string> nameList = context.PermissionSet.Select(ps => ps.PermissionSetName.ToLower()).Distinct().ToList();
            List<string> codeList = context.PermissionSet.Select(ps => ps.PermissionSetCode.ToLower()).Distinct().ToList();

            return new GetAllPermissionSetNameAndCodeResult()
            {
                Status = true,
                PermissionSetNameList = nameList,
                PermissionSetCodeList = codeList
            };
        }

        public GetAllUserResult GetAllUser(GetAllUserParameter parameter)
        {
            try
            {
                var ListUserObject = (from u in context.User
                                 join e in context.Employee on u.EmployeeId equals e.EmployeeId
                                 where e.Active == true
                                 select new UserEntityModel
                                 {
                                     UserId = u.UserId,
                                     UserName = e.EmployeeName,
                                     EmployeeId = e.EmployeeId
                                 }).ToList();

                return new GetAllUserResult
                {
                    Message = "Sucess",
                    Status = true,
                    lstUserEntityModel = ListUserObject
                };
            }
            catch (Exception ex)
            {

                return new GetAllUserResult
                {
                    Message = ex.ToString(),
                    Status = false
                };
            }
        }

        public GetCheckUserNameResult GetCheckUserName(GetCheckUserNameParameter parameter)
        {
            parameter.UserName = parameter.UserName.Trim();
            var user = context.User.FirstOrDefault(u => u.UserName == parameter.UserName);

            if (user == null)
            {
                return new GetCheckUserNameResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.USER_NOT_EXIST
                };
            }

            if (user.Active == false)
            {
                return new GetCheckUserNameResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.INACTIVE_USER
                };
            }

            var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
            var contact = context.Contact.FirstOrDefault(c => c.ObjectId == emp.EmployeeId && c.ObjectType == "EMP");
            var Email = contact.Email;
            if (Email != null)
            {
                Email = Email.Trim();
            }
            //if (string.IsNullOrEmpty(Email))
            //{
            //    Email = contact.WorkEmail.Trim();
            //    if (string.IsNullOrEmpty(Email))
            //    {
            //        Email = contact.OtherEmail.Trim();
            //    }
            //}
            //if (Email == null || Email.Trim() == "")
            //{

            //}
            if (string.IsNullOrEmpty(Email))
            {
                return new GetCheckUserNameResult()
                {
                    Status = false,
                    Message = CommonMessage.Contact.EMAIL_DOES_NOT_EXIST
                };
            }

            return new GetCheckUserNameResult()
            {
                Status = true,
                UserId = user.UserId,
                UserName = user.UserName,
                FullName = emp.EmployeeName,
                EmailAddress = Email
            };
        }

        public GetCheckResetCodeUserResult GetCheckResetCodeUser(GetCheckResetCodeUserParameter parameter)
        {
            var user = context.User.FirstOrDefault(u => u.ResetCode == parameter.Code);

            if (user == null)
            {
                return new GetCheckResetCodeUserResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.RESET_CODE_ERR
                };
            } else if (user.Active == false) {
                return new GetCheckResetCodeUserResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.INACTIVE_USER
                };
            }
            var curr_time = DateTime.Now;
            TimeSpan range_time = Convert.ToDateTime(curr_time) - Convert.ToDateTime(user.ResetCodeDate);
            int sum_day = range_time.Days;
            if (sum_day > 2)
            {
                return new GetCheckResetCodeUserResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.DATELINE_RESET_PASS
                };
            }

            return new GetCheckResetCodeUserResult()
            {
                Status = true,
                UserId = user.UserId,
                UserName = user.UserName
            };
        }

        public ResetPasswordResult ResetPassword(ResetPasswordParameter parameter)
        {
            var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);

            if (user == null)
            {
                return new ResetPasswordResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.USER_NOT_EXIST
                };
            } else if (user.Active == false)
            {
                return new ResetPasswordResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.INACTIVE_USER
                };
            }

            var currentPass = user.Password;
            var newPass = AuthUtil.GetHashingPassword(parameter.Password);

            if (currentPass == newPass)
            {
                return new ResetPasswordResult()
                {
                    Status = false,
                    Message = CommonMessage.Password.DUPLICATE
                };
            }

            user.ResetCode = null;
            user.ResetCodeDate = null;
            user.UpdatedById = parameter.UserId;
            user.UpdatedDate = DateTime.Now;
            user.Password = newPass;
            context.User.Update(user);
            context.SaveChanges();

            return new ResetPasswordResult()
            {
                Status = true
            };
        }

        public GetPositionCodeByPositionIdResult GetPositionCodeByPositionId(GetPositionCodeByPositionIdParameter parameter)
        {
            var position = context.Position.FirstOrDefault(p => p.PositionId == parameter.PositionId);
            var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
            var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId);
            var OrganizationId = organization.OrganizationId;
            var OrganizationName = organization.OrganizationName;
            var organizationIdList = (from or in context.Organization
                                      select new
                                      {
                                          or.OrganizationId,
                                          or.OrganizationCode,
                                          or.OrganizationName
                                      }
                                      ).ToList();

            List<dynamic> lstResult = new List<dynamic>();
            organizationIdList.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("OrganizationId", item.OrganizationId);
                sampleObject.Add("OrganizationCode", item.OrganizationCode);
                sampleObject.Add("OrganizationName", item.OrganizationName);
                lstResult.Add(sampleObject);
            });

            if (position == null)
            {
                return new GetPositionCodeByPositionIdResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.POSITION_NOT_EXIST
                };
            }

            var PositionCode = position.PositionCode;
            if (string.IsNullOrEmpty(PositionCode))
            {
                return new GetPositionCodeByPositionIdResult()
                {
                    Status = false,
                    Message = CommonMessage.Login.POSITION_NOT_EXIST
                };
            }

            return new GetPositionCodeByPositionIdResult()
            {
                Status = true,
                PositionCode = PositionCode,
                OrganizationId = OrganizationId,
                OrganizationName = OrganizationName,
                lstResult = lstResult
            };
        }

        public GetAllRoleResult GetAllRole(GetAllRoleParameter parameter)
        {
            try
            {
                List<RoleModel> listRoleResult = new List<RoleModel>();
                var listRole = context.Role.ToList();
                if (listRole.Count > 0)
                {
                    listRole.ForEach(item =>
                    {
                        RoleModel role = new RoleModel();
                        role.RoleId = item.RoleId;
                        role.RoleValue = item.RoleValue;
                        role.Description = item.Description;
                        var userNumber = context.UserRole.Where(e => e.RoleId == item.RoleId).ToList().Count();
                        role.UserNumber = userNumber;
                        listRoleResult.Add(role);
                    });
                }

                return new GetAllRoleResult
                {
                    ListRole = listRoleResult,
                    Status = true,
                    Message = "Lấy dữ liệu thành công"
                };
            }
            catch (Exception e)
            {
                return new GetAllRoleResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetCreatePermissionResult GetCreatePermission(GetCreatePermissionParameter parameter)
        {
            try
            {
                List<ActionResource> listActionResource = new List<ActionResource>();

                listActionResource = context.ActionResource.ToList();

                var ListMenuBuild = new List<MenuBuildEntityModel>();
                ListMenuBuild = context.MenuBuild.Select(y => new MenuBuildEntityModel
                {
                    MenuBuildId = y.MenuBuildId,
                    ParentId = y.ParentId,
                    Name = y.Name,
                    Code = y.Code,
                    CodeParent = y.CodeParent,
                    Path = y.Path,
                    NameIcon = y.NameIcon,
                    Level = y.Level,
                    IndexOrder = y.IndexOrder,
                    IsPageDetail = y.IsPageDetail
                }).OrderBy(z => z.IndexOrder).ToList();

                return new GetCreatePermissionResult
                {
                    ListActionResource = listActionResource,
                    ListMenuBuild = ListMenuBuild,
                    Status = true,
                    Message = "Lấy dữ liệu thành công"
                };
            }
            catch (Exception e)
            {
                return new GetCreatePermissionResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateRoleAndPermissionResult CreateRoleAndPermission(CreateRoleAndPermissionParameter parameter)
        {
            try
            {
                //Tạo Role
                Role role = new Role();
                role.RoleId = Guid.NewGuid();
                role.RoleValue = parameter.RoleValue;
                role.Description = parameter.Description;
                context.Role.Add(role);

                //Lấy list ActionResourceId từ mảng
                var listActionResourceId = context.ActionResource
                    .Where(e => parameter.ListActionResource.Contains(e.ActionResource1)).ToList();
                
                if (listActionResourceId.Count > 0)
                {
                    List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                    listActionResourceId.ForEach(item =>
                    {
                        RoleAndPermission roleAndPermission = new RoleAndPermission();
                        roleAndPermission.RoleAndPermissionId = Guid.NewGuid();
                        roleAndPermission.ActionResourceId = item.ActionResourceId;
                        roleAndPermission.RoleId = role.RoleId;
                        listRoleAndPermission.Add(roleAndPermission);
                    });
                    context.RoleAndPermission.AddRange(listRoleAndPermission);
                }

                #region Lưu đường dẫn mặc định của Sub menu module tương ứng với Role

                if (parameter.ListMenuBuild.Count > 0)
                {
                    var listAllMenuBuild = context.MenuBuild.ToList();

                    var ListRoleAndMenuBuild = new List<RoleAndMenuBuild>();
                    parameter.ListMenuBuild.ForEach(item =>
                    {
                        var RoleAndMenuBuild = new RoleAndMenuBuild();
                        RoleAndMenuBuild.RoleAndMenuBuildId = Guid.NewGuid();

                        var subMenu = listAllMenuBuild.FirstOrDefault(x => x.Code == item.Code);
                        RoleAndMenuBuild.MenuBuildId = subMenu.MenuBuildId;
                        RoleAndMenuBuild.RoleId = role.RoleId;
                        RoleAndMenuBuild.Code = item.Code;
                        RoleAndMenuBuild.Path = item.Path;

                        ListRoleAndMenuBuild.Add(RoleAndMenuBuild);
                    });

                    context.RoleAndMenuBuild.AddRange(ListRoleAndMenuBuild);
                }

                #endregion

                context.SaveChanges();

                return new CreateRoleAndPermissionResult
                {
                    Status = true,
                    Message = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateRoleAndPermissionResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetDetailPermissionResult GetDetailPermission(GetDetailPermissionParameter parameter)
        {
            try
            {
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (role == null)
                {
                    return new GetDetailPermissionResult
                    {
                        Status = false,
                        Message = "Không tồn tại nhóm quyền này"
                    };
                }

                List<ActionResource> listActionResource = new List<ActionResource>();
                List<ActionResource> listCurrentActionResource = new List<ActionResource>();
                listActionResource = context.ActionResource.ToList();

                List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                listRoleAndPermission = context.RoleAndPermission.Where(e => e.RoleId == role.RoleId).ToList();
                
                if (listRoleAndPermission.Count > 0)
                {
                    List<Guid> listActionResourceId = new List<Guid>();
                    listRoleAndPermission.ForEach(item =>
                    {
                        if (item.ActionResourceId != Guid.Empty && item.ActionResourceId != null)
                        {
                            listActionResourceId.Add(item.ActionResourceId.Value);
                        }
                    });

                    if (listActionResourceId.Count > 0)
                    {
                        listCurrentActionResource = context.ActionResource.Where(e => listActionResourceId.Contains(e.ActionResourceId)).ToList();
                    }
                }

                #region Lấy list MenuBuid

                var ListMenuBuild = new List<MenuBuildEntityModel>();
                ListMenuBuild = context.MenuBuild.Select(y => new MenuBuildEntityModel
                {
                    MenuBuildId = y.MenuBuildId,
                    ParentId = y.ParentId,
                    Name = y.Name,
                    Code = y.Code,
                    CodeParent = y.CodeParent,
                    Path = y.Path,
                    NameIcon = y.NameIcon,
                    Level = y.Level,
                    IndexOrder = y.IndexOrder,
                    IsPageDetail = y.IsPageDetail
                }).OrderBy(z => z.IndexOrder).ToList();

                //Lấy list đường dẫn mặc định gắn với nhóm quyền
                var listRoleAndMenuBuild = context.RoleAndMenuBuild.Where(x => x.RoleId == parameter.RoleId).ToList();
                ListMenuBuild.ForEach(item =>
                {
                    //Lấy ra các sub menu module
                    if (item.Level == 1)
                    {
                        var existsDefaultPath = listRoleAndMenuBuild.FirstOrDefault(x => x.Code == item.Code);

                        //Nếu tồn tại đường dẫn mặc định đã được cấu hình
                        if (existsDefaultPath != null)
                        {
                            item.Path = existsDefaultPath.Path;
                        }
                    }
                });

                #endregion

                return new GetDetailPermissionResult
                {
                    Role = role,
                    ListActionResource = listActionResource,
                    ListCurrentActionResource = listCurrentActionResource,
                    ListMenuBuild = ListMenuBuild,
                    Status = true,
                    Message = "Lấy dữ liệu thành công"
                };
            }
            catch (Exception e)
            {
                return new GetDetailPermissionResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public EditRoleAndPermissionResult EditRoleAndPermission(EditRoleAndPermissionParameter parameter)
        {
            try
            {
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (role == null)
                {
                    return new EditRoleAndPermissionResult
                    {
                        Status = false,
                        Message = "Không tồn tại nhóm quyền"
                    };
                }
                role.RoleValue = parameter.RoleValue;
                role.Description = parameter.Description;
                context.Update(role);

                var listActionResource = context.ActionResource.Where(e => parameter.ListActionResource.Contains(e.ActionResource1)).ToList();
                List<Guid> listActionResourceId = new List<Guid>();
                if (listActionResource.Count > 0)
                {
                    listActionResource.ForEach(item =>
                    {
                        listActionResourceId.Add(item.ActionResourceId);
                    });
                }
                //Xóa trong bảng RoleAndPermission
                var listRoleAndPermissionOld = context.RoleAndPermission.Where(e => e.RoleId == role.RoleId).ToList();
                context.RoleAndPermission.RemoveRange(listRoleAndPermissionOld);

                //Thêm mới trong bảng RoleAndPermission
                List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                if (listActionResourceId.Count > 0)
                {
                    listActionResourceId.ForEach(item =>
                    {
                        RoleAndPermission roleAndPermission = new RoleAndPermission();
                        roleAndPermission.RoleAndPermissionId = Guid.NewGuid();
                        roleAndPermission.ActionResourceId = item;
                        roleAndPermission.RoleId = role.RoleId;
                        listRoleAndPermission.Add(roleAndPermission);
                    });
                    context.RoleAndPermission.AddRange(listRoleAndPermission);
                }

                #region Lưu đường dẫn mặc định của Sub menu module tương ứng với Role

                //Xóa dữ liệu mặc định cũ
                var listOldRoleAndMenuBuild =
                    context.RoleAndMenuBuild.Where(x => x.RoleId == parameter.RoleId).ToList();
                context.RoleAndMenuBuild.RemoveRange(listOldRoleAndMenuBuild);

                if (parameter.ListMenuBuild.Count > 0)
                {
                    var listAllMenuBuild = context.MenuBuild.ToList();

                    var ListRoleAndMenuBuild = new List<RoleAndMenuBuild>();
                    parameter.ListMenuBuild.ForEach(item =>
                    {
                        var RoleAndMenuBuild = new RoleAndMenuBuild();
                        RoleAndMenuBuild.RoleAndMenuBuildId = Guid.NewGuid();

                        var subMenu = listAllMenuBuild.FirstOrDefault(x => x.Code == item.Code);
                        RoleAndMenuBuild.MenuBuildId = subMenu.MenuBuildId;
                        RoleAndMenuBuild.RoleId = role.RoleId;
                        RoleAndMenuBuild.Code = item.Code;
                        RoleAndMenuBuild.Path = item.Path;

                        ListRoleAndMenuBuild.Add(RoleAndMenuBuild);
                    });

                    context.RoleAndMenuBuild.AddRange(ListRoleAndMenuBuild);
                }

                #endregion

                context.SaveChanges();

                return new EditRoleAndPermissionResult
                {
                    Status = true,
                    Message = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new EditRoleAndPermissionResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public AddUserRoleResult AddUserRole(AddUserRoleParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (user == null)
                {
                    return new AddUserRoleResult
                    {
                        Status = false,
                        Message = "Không tồn tại User này trên hệ thống"
                    };
                }
                if (role == null)
                {
                    return new AddUserRoleResult
                    {
                        Status = false,
                        Message = "Không tồn tại Nhóm quyền này trên hệ thống"
                    };
                }
                var listUserRoleOld = context.UserRole.Where(e => e.UserId == user.UserId).ToList();
                if (listUserRoleOld.Count > 0)
                {
                    context.UserRole.RemoveRange(listUserRoleOld);
                }

                //Add lại role cho user
                //Hiện tại chỉ là 1:1
                UserRole userRole = new UserRole();
                userRole.UserRoleId = Guid.NewGuid();
                userRole.UserId = user.UserId;
                userRole.RoleId = role.RoleId;
                context.UserRole.Add(userRole);
                context.SaveChanges();

                return new AddUserRoleResult
                {
                    Status = true,
                    Message = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new AddUserRoleResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public DeleteRoleResult DeleteRole(DeleteRoleParameter parameter)
        {
            try
            {
                //Delete Role
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (role == null)
                {
                    return new DeleteRoleResult
                    {
                        Status = false,
                        Message = "Nhóm quyền này không tồn tại"
                    };
                }
                context.Role.Remove(role);

                //Delete RoleAndPermission
                List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                listRoleAndPermission = context.RoleAndPermission.Where(e => e.RoleId == parameter.RoleId).ToList();
                if (listRoleAndPermission.Count > 0)
                {
                    context.RoleAndPermission.RemoveRange(listRoleAndPermission);
                }

                //Delete User Role
                List<UserRole> listUserRole = new List<UserRole>();
                listUserRole = context.UserRole.Where(e => e.RoleId == parameter.RoleId).ToList();
                if(listUserRole.Count > 0)
                {
                    context.UserRole.RemoveRange(listUserRole);
                }

                context.SaveChanges();
                return new DeleteRoleResult
                {
                    Status = true,
                    Message = "Xóa nhóm quyền thành công"
                };
            }
            catch (Exception e)
            {
                return new DeleteRoleResult
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }
    }
}
