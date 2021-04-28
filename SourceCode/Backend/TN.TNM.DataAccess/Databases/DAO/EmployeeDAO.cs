using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeDAO : BaseDAO, IEmployeeDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public EmployeeDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
        }

        public CreateEmployeeResult CreateEmployee(CreateEmployeeParameter parameter)
        {

            #region Thêm Chi nhánh cho nhân viên (nếu có Chi nhánh)

            var branch = context.Branch.FirstOrDefault();
            parameter.Employee.BranchId = branch?.BranchId;

            #endregion

            parameter.Employee.EmployeeId = Guid.NewGuid();
            parameter.Employee.EmployeeName = parameter.Contact.FirstName + " " + parameter.Contact.LastName;
            parameter.Employee.CreatedDate = DateTime.Now;
            parameter.Employee.CreatedById = parameter.UserId;
            parameter.Employee.IsManager = false;

            if (String.IsNullOrWhiteSpace(parameter.Employee.EmployeeCode) || parameter.Employee.EmployeeCode == "")
            {
                /*do code của nhân viên sẽ tự động gen theo thứ tự 1,2,3 nhưng trong db còn chứa những dữ liệu test nên cần phải lấy 
                EmployeeCode của nhân viên được tạo gần nhất cộng thêm 1 để ra code*/
                var lastestEmpCreated = context.Employee.Max(e => e.CreatedDate);
                var lastestEmpCode = context.Employee.FirstOrDefault(e => e.CreatedDate == lastestEmpCreated).EmployeeCode;
                var newCode = (Convert.ToInt32(lastestEmpCode) + 1).ToString();
                parameter.Employee.EmployeeCode = newCode == null ? "0" : newCode;
                //set EmployeeCode done
            }

            var employeeDupblicase = context.Employee.FirstOrDefault(x => x.EmployeeCode == parameter.Employee.EmployeeCode);
            if (employeeDupblicase != null)
            {
                return new CreateEmployeeResult
                {
                    Status = false,
                    Message = "Mã nhân viên đã tồn tại"
                };
            }

            // check trung ten tai khoan
            var duplicateUser = context.User.FirstOrDefault(x => x.UserName == parameter.User.UserName);
            if (duplicateUser != null)
            {
                return new CreateEmployeeResult
                {
                    Status = false,
                    Message = "Tên tài khoản đã tồn tại"
                };
            }

            parameter.Contact.ContactId = Guid.NewGuid();
            parameter.Contact.FirstName = parameter.Contact.FirstName.Trim();
            parameter.Contact.LastName = parameter.Contact.LastName.Trim();
            parameter.Contact.IdentityId = parameter.Contact.IdentityId != null ? parameter.Contact.IdentityId.Trim() : null;
            parameter.Contact.Email = parameter.Contact.Email.Trim();
            parameter.Contact.ObjectId = parameter.Employee.EmployeeId;
            parameter.Contact.ObjectType = ObjectType.EMPLOYEE;
            parameter.Contact.CreatedDate = DateTime.Now;
            parameter.Contact.CreatedById = parameter.UserId;

            parameter.User.EmployeeId = parameter.Employee.EmployeeId;
            parameter.User.UserName = parameter.User.UserName.Trim();
            parameter.User.CreatedDate = DateTime.Now;
            parameter.User.Active = parameter.IsAccessable;
            parameter.User.CreatedById = parameter.UserId;

            #region Get Employee Infor to send email       
            var SendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();
            if (parameter.IsAccessable == true)
            {
                SendEmailEntityModel.EmployeeName = parameter.Employee.EmployeeName;
                SendEmailEntityModel.UserName = parameter.User.UserName;
                SendEmailEntityModel.UserPassword = context.SystemParameter.FirstOrDefault(w => w.SystemKey == "DefaultUserPassword").SystemValueString;
                SendEmailEntityModel.ListSendToEmail.Add(parameter.Contact.Email);
            }
            else
            {
                SendEmailEntityModel = null;
            }
            
            var configEntity = context.SystemParameter.ToList();

            var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE")
                .CategoryTypeId;
            var listEmailTempType =
                context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

            var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "TNV")
                .CategoryId;

            var emailTemplate =
                context.EmailTemplate.FirstOrDefault(w =>
                    w.Active && w.EmailTemplateTypeId == emailCategoryId);

            var subject = ReplaceTokenForContent(context, parameter.User, emailTemplate.EmailTemplateTitle,
                configEntity);
            var content = ReplaceTokenForContent(context, parameter.User, emailTemplate.EmailTemplateContent,
                configEntity);

            Emailer.SendEmail(context, SendEmailEntityModel.ListSendToEmail, new List<string>(), subject, content);

            #endregion

            context.Employee.Add(parameter.Employee);
            context.Contact.Add(parameter.Contact);
            context.User.Add(parameter.User);
            context.SaveChanges();
            return new CreateEmployeeResult
            {
                Status = true,
                Message = CommonMessage.Employee.CREATE_SUCCESS,
                EmployeeId = parameter.Employee.EmployeeId,
                ContactId = parameter.Contact.ContactId,
                SendEmailEntityModel = SendEmailEntityModel
            };
        }

        public SearchEmployeeResult SearchEmployee(SearchEmployeeParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.EMPLOYEE, "Search employee", parameter.UserId);
            var currentUserEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentUserOrgId = context.Employee.FirstOrDefault(u => u.EmployeeId == currentUserEmpId).OrganizationId;
            var listGetAllChild = new List<Guid>();
            var organizations = context.Organization.ToList();
            if (parameter.OrganizationId != null)
            {
                listGetAllChild = ListChildOfParamToSearch(parameter.OrganizationId.Value, organizations);
                listGetAllChild.Add(parameter.OrganizationId.Value);
            }
            //var orgIdList = context.Organization.Where(o => o.ParentId == currentUserOrgId).Select(o => o.OrganizationId).ToList();

            var employeeList = (from c in context.Contact
                                join e in context.Employee on c.ObjectId equals e.EmployeeId
                                join u in context.User on c.ObjectId equals u.EmployeeId into cu
                                from x in cu.DefaultIfEmpty()
                                where (e.Active == true) &&
                                      (c.FirstName.ToLower().Contains(parameter.FirstName.ToLower().Trim()) || parameter.FirstName == null || parameter.FirstName.Trim() == "") &&
                                            (c.LastName.ToLower().Contains(parameter.LastName.ToLower().Trim()) || parameter.LastName == null || parameter.LastName.Trim() == "") &&
                                            (e.EmployeeCode.ToLower().Contains(parameter.IdentityId.ToLower().Trim()) || parameter.IdentityId == null || parameter.IdentityId.Trim() == "") &&
                                            (x.UserName.ToLower().Contains(parameter.UserName.ToLower().Trim()) || parameter.UserName == null || parameter.UserName.Trim() == "") &&
                                      //e.EmployeeId != currentUserEmpId &&
                                      //e.CreatedById == parameter.UserId &&
                                      (parameter.OrganizationId == null || listGetAllChild.Contains(e.OrganizationId.Value)) &&
                                      (parameter.ListPosition.Count == 0 || parameter.ListPosition.Contains(e.PositionId.Value) || e.PositionId == null)
                                select new EmployeeEntityModel
                                {
                                    EmployeeId = e.EmployeeId,
                                    ContactId = c.ContactId,
                                    EmployeeName = e.EmployeeName,
                                    Username = x == null ? "" : x.UserName,
                                    OrganizationId = e.OrganizationId,
                                    OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId) == null ? "" : organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName,
                                    Identity = c.IdentityId,
                                    EmployeeCode = e.EmployeeCode,
                                    AvatarUrl = c.AvatarUrl,
                                    PositionId = e.PositionId,
                                    CreatedById = e.CreatedById,
                                    CreatedDate = e.CreatedDate,
                                    LastName = c.LastName
                                }).OrderBy(x => x.LastName).ToList();

            //var isManager = context.Employee.FirstOrDefault(e => e.EmployeeId == currentUserEmpId).IsManager;
            //employeeList = !isManager
            //    ? employeeList.Where(emp => emp.OrganizationId == currentUserOrgId || emp.CreatedById == parameter.UserId).ToList()
            //    : employeeList.Where(emp =>
            //        orgIdList.Any(id => id == emp.OrganizationId.Value) || emp.OrganizationId == currentUserOrgId || emp.CreatedById == parameter.UserId).ToList();


            return new SearchEmployeeResult
            {
                Status = true,
                EmployeeList = employeeList
            };
        }
        private List<Guid> ListChildOfParamToSearch(Guid orgId, List<Organization> organizations)
        {
            //var orgParam = context.Organization.FirstOrDefault(org => org.OrganizationId == orgId);
            var _listOrgIdChild = organizations.Where(o => o.ParentId == orgId).Select(id => id.OrganizationId).ToList();
            var _tmpOrgId = new List<Guid>();
            _listOrgIdChild.ForEach(_orgId =>
            {
                _tmpOrgId.Add(_orgId);
                ListChildOfParamToSearch(_orgId, organizations).ForEach(child =>
                {
                    _tmpOrgId.Add(child);
                });
            });
            return _tmpOrgId;
        }
        public GetAllEmployeeResult GetAllEmployee(GetAllEmployeeParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.EMPLOYEE, "Get all Employee", parameter.UserId);
            var empList = context.Employee.Where(e => e.EmployeeCode != "PortalUser" && e.Active == true).ToList();
            var permissionSetId = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == parameter.UserId) == null ? Guid.Empty : context.PermissionMapping.FirstOrDefault(pm => pm.UserId == parameter.UserId).PermissionSetId;
            //var listIdentityMapEmpId = (from c in context.Contact
            //                            join e in context.Employee on c.ObjectId equals e.EmployeeId
            var listIdentityMapEmpId = (from e in context.Employee
                                        where e.EmployeeCode != "PortalUser" && e.Active == true
                                        select new EmployeeEntityModel
                                        {
                                            EmployeeId = e.EmployeeId,
                                            ContactId = Guid.Empty,//c.ContactId,                                            
                                            EmployeeName = e.EmployeeName,
                                            Username = "",
                                            OrganizationId = e.OrganizationId,
                                            OrganizationName = "",// context.Organization.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName, Edit By Hung
                                            EmployeeCode = e.EmployeeCode,
                                            AvatarUrl = "",//c.AvatarUrl,
                                            CreatedById = e.CreatedById,
                                            CreatedDate = e.CreatedDate,
                                            LastName = "",//c.LastName,
                                            Active = e.Active,
                                            IsManager = e.IsManager,
                                            PositionId = e.PositionId,
                                            PermissionSetId = permissionSetId,
                                            StartedDate = e.StartedDate
                                        }).OrderByDescending(e => e.EmployeeCode).ToList();

            #region Add by Hung
            List<Guid> organizationIdList = new List<Guid>();
            List<Guid> employeeIdList = new List<Guid>();
            listIdentityMapEmpId.ForEach(item =>
            {
                if (!organizationIdList.Contains(item.OrganizationId.Value))
                    organizationIdList.Add(item.OrganizationId.Value);
                employeeIdList.Add(item.EmployeeId.Value);
            });
            var contactList = context.Contact.Where(w => employeeIdList.Contains(w.ObjectId) && w.ObjectType == "EMP").ToList();
            var organizationList = context.Organization.Where(w => organizationIdList.Contains(w.OrganizationId)).ToList();
            listIdentityMapEmpId.ForEach(item =>
            {
                var contact = contactList.FirstOrDefault(f => f.ObjectId == item.EmployeeId);
                item.OrganizationName = organizationList.FirstOrDefault(f => f.OrganizationId == item.OrganizationId.Value).OrganizationName;
                item.ContactId = contact.ContactId;
                item.AvatarUrl = contact.AvatarUrl;
                item.LastName = contact.LastName;
            });
            #endregion

            return new GetAllEmployeeResult
            {
                EmployeeList = empList,
                Status = true,
                listIdentityMapEmpId = listIdentityMapEmpId
            };
        }


        public GetEmployeeByIdResult GetEmployeeById(GetEmployeeByIdParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.EMPLOYEE, "Get Employee by Id", parameter.UserId);
            var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
            var userDisable = context.User.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId).Disabled;
            Contact contact;
            if (parameter.ContactId == Guid.Empty)
                contact = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.EmployeeId);
            else
                contact = context.Contact.FirstOrDefault(c => c.ContactId == parameter.ContactId);
            var user = context.User.FirstOrDefault(u => u.EmployeeId == parameter.EmployeeId);
            var permissionMapping = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == user.UserId);
            Guid permissionSetId;
            if (permissionMapping == null)
                permissionSetId = Guid.Empty;
            else
                permissionSetId = permissionMapping.PermissionSetId;
            string employeeName = employee?.EmployeeName;
            string phone = contact?.Phone;
            string gender = contact?.Gender;
            string email = contact?.Email;
            DateTime? dob = contact?.DateOfBirth;
            string identity = contact?.IdentityId;
            string positionname = context.Position.FirstOrDefault(p => p.PositionId == employee.PositionId)?.PositionName;
            string organizationName = context.Organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId)?.OrganizationName;
            string username = user?.UserName;
            DateTime? startDate = employee?.StartedDate;
            string avatarUrl = contact?.AvatarUrl;

            var role = context.UserRole.FirstOrDefault(e => e.UserId == user.UserId);

            EmployeeEntityModel emp = new EmployeeEntityModel()
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                EmployeeName = employeeName,
                Username = username,
                PositionId = employee.PositionId,
                PositionName = positionname,
                OrganizationId = employee.OrganizationId,
                OrganizationName = organizationName,
                StartedDate = startDate,
                CreatedDate = employee.CreatedDate,
                CreatedById = employee.CreatedById,
                Active = employee.Active,
                IsManager = employee.IsManager,
                PermissionSetId = permissionSetId,
                ProbationEndDate = employee.ProbationEndDate,
                ProbationStartDate = employee.ProbationStartDate,
                TrainingStartDate = employee.TrainingStartDate,
                AvatarUrl = contact.AvatarUrl,
                ContractType = employee.ContractType,
                ContractEndDate = employee.ContractEndDate,
                ActiveUser = userDisable,
                RoleId = role == null ? null : role.RoleId,
                IsTakeCare = employee.IsTakeCare
            };

            ContactEntityModel con = new ContactEntityModel()
            {
                ContactId = contact.ContactId,
                ObjectId = contact.ObjectId,
                ObjectType = contact.ObjectType,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                IdentityId = identity,
                Phone = phone,
                Gender = gender,
                Email = email,
                DateOfBirth = dob,
                AvatarUrl = avatarUrl,
                CreatedDate = contact.CreatedDate,
                CreatedById = contact.CreatedById,
                Active = contact.Active,
                Note = contact.Note,
                Address = contact.Address,
                CountryId = contact.CountryId,
                HealthInsuranceDateOfIssue = contact.HealthInsuranceDateOfIssue,
                HealthInsuranceDateOfParticipation = contact.HealthInsuranceDateOfParticipation,
                HealthInsuranceNumber = contact.HealthInsuranceNumber,
                IdentityIddateOfIssue = contact.IdentityIddateOfIssue,
                IdentityIddateOfParticipation = contact.IdentityIddateOfParticipation,
                IdentityIdplaceOfIssue = contact.IdentityIdplaceOfIssue,
                SocialInsuranceDateOfIssue = contact.SocialInsuranceDateOfIssue,
                SocialInsuranceDateOfParticipation = contact.SocialInsuranceDateOfParticipation,
                SocialInsuranceNumber = contact.SocialInsuranceNumber,
                VisaDateOfIssue = contact.VisaDateOfIssue,
                WorkPermitNumber = contact.WorkPermitNumber,
                VisaExpirationDate = contact.VisaExpirationDate,
                VisaNumber = contact.VisaNumber,
                TypePaid = contact.TypePaid,
                WorkHourOfEnd = contact.WorkHourOfEnd,
                WorkHourOfStart = contact.WorkHourOfStart
            };

            UserEntityModel us = new UserEntityModel()
            {
                UserId = user.UserId,
                UserName = username,
                EmployeeId = user.EmployeeId,
                Active = user.Active,
                Password = user.Password,
                CreatedDate = user.CreatedDate,
                CreatedById = user.CreatedById
            };

            return new GetEmployeeByIdResult()
            {
                Employee = emp,
                Contact = con,
                User = us,
                Status = true
            };
        }

        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity)
        {
            var result = emailContent;
            var defaultPass = context.SystemParameter.FirstOrDefault(w => w.SystemKey == "DefaultUserPassword").SystemValueString;

            #region Common Token

            const string Logo = "[LOGO]";
            const string UserName = "[USER_NAME]";
            const string UserPass = "[USER_PASS]";
            const string EmployeeName = "[EMP_NAME]";
            const string Url_Login = "[ACCESS_SYSTEM]";

            #endregion

            var _model = model as User;

            if (result.Contains(Logo))
            {
                var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                if (!String.IsNullOrEmpty(logo))
                {
                    var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                    result = result.Replace(Logo, temp_logo);
                }
                else
                {
                    result = result.Replace(Logo, "");
                }
            }

            if (result.Contains(UserName) && _model.UserName != null)
            {
                result = result.Replace(UserName, _model.UserName);
            }

            if (result.Contains(UserPass) && _model.Password != null)
            {
                result = result.Replace(UserPass, defaultPass);
            }
            
            if (result.Contains(EmployeeName))
            {
                var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UserId)?.EmployeeId;
                var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                if (!String.IsNullOrEmpty(employeeName))
                {
                    result = result.Replace(EmployeeName, employeeName);
                }
                else
                {
                    result = result.Replace(EmployeeName, "");
                }
            }

            if (result.Contains(Url_Login))
            {
                var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                if (!String.IsNullOrEmpty(loginLink))
                {
                    result = result.Replace(Url_Login, loginLink);
                }
            }


            return result;
        }

        public EditEmployeeByIdResult EditEmployeeById(EditEmployeeByIdParameter parameter)
        {
            var SendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();

            if (parameter.IsResetPass)
            {
                var defaultPass = context.SystemParameter.FirstOrDefault(w => w.SystemKey == "DefaultUserPassword")
                    .SystemValueString;
                parameter.User.Password = AuthUtil.GetHashingPassword(defaultPass);
                context.User.Update(parameter.User);
                context.SaveChanges();

                #region Get Employee Infor to send email

                var configEntity = context.SystemParameter.ToList();

                var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE")
                    .CategoryTypeId;
                var listEmailTempType =
                    context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

                var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "REPASS")
                    .CategoryId;

                var emailTemplate =
                    context.EmailTemplate.FirstOrDefault(w =>
                        w.Active && w.EmailTemplateTypeId == emailCategoryId);

                var subject = ReplaceTokenForContent(context, parameter.User, emailTemplate.EmailTemplateTitle,
                    configEntity);
                var content = ReplaceTokenForContent(context, parameter.User, emailTemplate.EmailTemplateContent,
                    configEntity);
                //SendEmailEntityModel.EmployeeName = parameter.Employee.EmployeeName;
                //SendEmailEntityModel.UserName = parameter.User.UserName;
                //SendEmailEntityModel.UserPassword = parameter.User.Password;
                SendEmailEntityModel.ListSendToEmail.Add(parameter.Contact.Email);

                Emailer.SendEmail(context, SendEmailEntityModel.ListSendToEmail, new List<string>(), subject, content);

                #endregion
            }
            else
            {
                #region Thêm Chi nhánh cho nhân viên (nếu có Chi nhánh)

                var branch = context.Branch.FirstOrDefault();
                parameter.Employee.BranchId = branch?.BranchId;

                #endregion

                // Update ngay edit, nguoi edit
                parameter.Employee.EmployeeName =
                    parameter.Contact.FirstName.Trim() + " " + parameter.Contact.LastName.Trim();
                parameter.Employee.UpdatedById = parameter.UserId;
                parameter.Employee.UpdatedDate = DateTime.Now;
                parameter.Contact.UpdatedById = parameter.UserId;
                parameter.Contact.UpdatedDate = DateTime.Now;
                parameter.User.UpdatedById = parameter.UserId;
                parameter.User.UpdatedDate = DateTime.Now;


                //Update db
                context.Employee.Update(parameter.Employee);
                context.Contact.Update(parameter.Contact);
                context.User.Update(parameter.User);
                context.SaveChanges();
            }


            return new EditEmployeeByIdResult()
            {
                Status = true,
                Message = CommonMessage.Employee.EDIT_SUCCESS
            };
        }

        public GetAllEmpAccountResult GetAllEmpAccount(GetAllEmpAccountParameter parameter)
        {
            var empAccountlist = context.User.Select(u => u.UserName.ToLower()).ToList();
            return new GetAllEmpAccountResult()
            {
                Status = true,
                EmpAccountList = empAccountlist
            };
        }

        public GetAllEmployeeAccountResult GetAllEmployeeAccount()
        {
            var result = (from e in context.Employee
                          join u in context.User on e.EmployeeId equals u.EmployeeId
                          where e.Active == true
                          select new EmployeeEntityModel
                          {
                              IsManager = e.IsManager,
                              EmployeeId = e.EmployeeId,
                              EmployeeName = e.EmployeeName,
                              Username = u.UserName,
                              EmployeeCode = e.EmployeeCode,
                              OrganizationId = e.OrganizationId,
                              PositionId = e.PositionId
                          });

            return new GetAllEmployeeAccountResult
            {
                Status = true,
                EmployeeAcounts = result
            };
        }

        public GetAllEmpIdentityResult GetAllEmpIdentity(GetAllEmpIdentityParameter parameter)
        {
            List<string> empIdentityList;
            if (parameter.CurrentEmpId == Guid.Empty)
                empIdentityList = context.Employee.Select(c => c.EmployeeCode.ToLower()).ToList();
            else
                empIdentityList = context.Employee.Where(c => c.EmployeeId != parameter.CurrentEmpId).Select(c => c.EmployeeCode.ToLower()).ToList();

            return new GetAllEmpIdentityResult()
            {
                Status = true,
                EmpIdentityList = empIdentityList
            };
        }


        public EditEmployeeDataPermissionResult EditEmployeeDataPermission(EditEmployeeDataPermissionParameter parameter)
        {
            var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
            emp.IsManager = parameter.IsManager;
            emp.UpdatedById = parameter.UserId;
            emp.UpdatedDate = DateTime.Now;
            context.Update(emp);
            context.SaveChanges();

            return new EditEmployeeDataPermissionResult()
            {
                Status = true,
                Message = CommonMessage.Employee.EDIT_SUCCESS
            };
        }

        public EmployeePermissionMappingResult EmployeePermissionMapping(EmployeePermissionMappingParameter parameter)
        {

            var userId = context.User.FirstOrDefault(u => u.EmployeeId == parameter.EmployeeId).UserId;
            var existedPermission = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == userId);
            if (existedPermission == null)
            {
                PermissionMapping pm = new PermissionMapping()
                {
                    PermissionMappingId = Guid.NewGuid(),
                    UserId = userId,
                    PermissionSetId = parameter.PermissionSetId,
                    UseFor = "U",
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    Active = true
                };

                context.PermissionMapping.Add(pm);
            }
            else
            {
                existedPermission.PermissionSetId = parameter.PermissionSetId;
                existedPermission.UpdatedById = parameter.UserId;
                existedPermission.UpdatedDate = DateTime.Now;
                context.PermissionMapping.Update(existedPermission);
            }

            context.SaveChanges();

            return new EmployeePermissionMappingResult()
            {
                Status = true,
                Message = CommonMessage.Employee.GRAND_PERMISSION_SUCCESS
            };
        }

        public GetEmployeeByPositionCodeResult GetEmployeeByPositionCode(GetEmployeeByPositionCodeParameter parameter)
        {
            var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == employeeId);
            var lst = new List<Entities.Employee>();
            if (employee.IsManager)
            {
                var organization = context.Organization.ToList();
                var organizationGD = organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId);
                var maxLever = organization.Max(o => o.Level);
                List<Guid?> organizationId = new List<Guid?>();
                organizationId.Add(organizationGD.OrganizationId);
                if (organizationGD.Level == 0)
                {
                    lst = context.Employee.Where(e => e.IsManager == false && e.Active == true).OrderBy(e => e.EmployeeName).ToList();
                }
                else
                {
                    var organizationPB1 = organization.Where(o => o.ParentId == employee.OrganizationId).ToList();
                    organizationPB1.ForEach(addId =>
                    {
                        organizationId.Add(addId.OrganizationId);
                    });
                    for (int i = organizationGD.Level + 1; i <= maxLever; i++)
                    {
                        var organizationPB = organization.Where(o => o.Level == i && organizationId.Contains(o.ParentId)).ToList();
                        organizationPB.ForEach(addId =>
                        {
                            organizationId.Add(addId.OrganizationId);
                        });
                    }
                    //var positionId = context.Position.FirstOrDefault(p => p.PositionCode == parameter.PositionCode).PositionId;
                    lst = context.Employee.Where(e => e.IsManager == false && organizationId.Contains(e.OrganizationId) && e.Active == true).OrderBy(e => e.EmployeeName).ToList();
                }

                var empLikeLevel = context.Employee.Where(e => e.IsManager == true && e.OrganizationId == employee.OrganizationId && e.Active == true).ToList();
                empLikeLevel.ForEach(ef =>
                {
                    lst.Add(ef);
                });
            }
            else
            {
                lst = context.Employee.Where(e => e.EmployeeId == employeeId).ToList();
            }
            return new GetEmployeeByPositionCodeResult()
            {
                EmployeeList = lst,
                Status = true
            };
        }
        public GetEmployeeHighLevelByEmpIdResult GetEmployeeHighLevelByEmpId(GetEmployeeHighLevelByEmpIdParameter parameter)
        {

            var listEmpToApprove = new List<Employee>();
            var listEmpToNotify = new List<Employee>();
            var empList = context.Employee.Where(emac => emac.Active == true).ToList();
            var workFlows = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "QTPDDXXN");
            var getWorkFlows = context.WorkFlowSteps.Where(ws => ws.WorkflowId == workFlows.WorkFlowId);
            var getMaxStep = getWorkFlows.Max(m => m.StepNumber);
            var getWorkFlowsEndStep = getWorkFlows.FirstOrDefault(wn => wn.StepNumber == getMaxStep);
            var empRequest = empList.FirstOrDefault(em => em.EmployeeId == parameter.EmployeeId);
            var organizationList = context.Organization.ToList();
            var getOrganizationReq = organizationList.FirstOrDefault(p => p.OrganizationId == empRequest.OrganizationId);
            var parentId = getOrganizationReq.ParentId;
            var listOrgId = new List<Guid?>();
            listOrgId.Add(getOrganizationReq.OrganizationId);

            // get list nhân viên thông báo cho
            var maxLevel = organizationList.Max(maxl => maxl.Level);
            if (maxLevel != getOrganizationReq.Level)
            {
                for (int i = getOrganizationReq.Level; i < maxLevel; i++)
                {
                    organizationList.ForEach(item =>
                    {
                        if (listOrgId.Contains(item.ParentId) && !listOrgId.Contains(item.OrganizationId))
                        {
                            listOrgId.Add(item.OrganizationId);
                        }
                    });
                }
            }
            listEmpToNotify = empList.Where(emno => listOrgId.Contains(emno.OrganizationId) && emno.Active == true).ToList();

            // get list nhân viên approve
            if (getWorkFlowsEndStep.ApprovebyPosition)
            {
                listEmpToApprove = empList.Where(emap => emap.OrganizationId == empRequest.OrganizationId && emap.PositionId == getWorkFlowsEndStep.ApproverPositionId).ToList();
                if (listEmpToApprove.Count == 0 && getOrganizationReq.Level != 0)
                {
                    for (int i = getOrganizationReq.Level; i > 0; i--)
                    {
                        listEmpToApprove = empList.Where(emap => emap.OrganizationId == parentId && emap.PositionId == getWorkFlowsEndStep.ApproverPositionId).ToList();
                        if (listEmpToApprove.Count == 0)
                        {
                            var getLevelParent = organizationList.FirstOrDefault(p => p.OrganizationId == parentId);
                            parentId = getLevelParent.ParentId;
                        }
                        else
                        {
                            i = -1;
                        }
                    }
                }
            }
            else
            {
                listEmpToApprove = empList.Where(e => e.EmployeeId == getWorkFlowsEndStep.ApproverId).ToList();
            }
            return new GetEmployeeHighLevelByEmpIdResult()
            {
                ListEmployeeToApprove = listEmpToApprove,
                ListEmployeeToNotify = listEmpToNotify,
                Message = "Success",
                Status = true
            };
        }
        private List<string> GetPositionWithHigherLevel(string targetPosition)
        {
            var currentEmpCodeListLowLevel = new List<string>() { "GV", "CTV", "NV", "TG" };
            var currentEmpCodeListHightLevel = new List<string>() { "TGD", "GD", "TP", "PP", "QL" };
            if (currentEmpCodeListLowLevel.Contains(targetPosition))
            {
                return currentEmpCodeListHightLevel;
            }
            var result = new List<string>();
            currentEmpCodeListHightLevel.ForEach(code =>
            {
                if (code != targetPosition)
                {
                    result.Add(code);
                }
            });
            return result;
        }

        public GetEmployeeByOrganizationIdResult GetEmployeeByOrganizationId(GetEmployeeByOrganizationIdParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.EMPLOYEE, "Get Employee By OrganizationId", parameter.UserId);
            var listEmployee = (from employee in context.Employee
                                where parameter.OrganizationId == employee.OrganizationId && employee.Active == true
                                select new
                                {
                                    employee.EmployeeId,
                                    employee.EmployeeName
                                }
                                ).ToList();

            List<dynamic> lstResult = new List<dynamic>();
            listEmployee.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("EmployeeId", item.EmployeeId);
                sampleObject.Add("EmployeeName", item.EmployeeName);
                lstResult.Add(sampleObject);
            });

            return new GetEmployeeByOrganizationIdResult()
            {
                listEmployee = lstResult
            };
        }

        public GetEmployeeByTopRevenueResult GetEmployeeByTopRevenue(GetEmployeeByTopRevenueParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.EMPLOYEE, "Get Employee By Top Revenue", parameter.UserId);
            var listEmployee = (from customerOrder in context.CustomerOrder
                                join orderStatus in context.OrderStatus on customerOrder.StatusId equals orderStatus.OrderStatusId
                                join employee in context.Employee on customerOrder.Seller equals employee.EmployeeId
                                join organization in context.Organization on employee.OrganizationId equals organization.OrganizationId
                                join contact in context.Contact on employee.EmployeeId equals contact.ObjectId
                                where (parameter.EmployeeId == null || parameter.EmployeeId == employee.EmployeeId)
                                && (parameter.ListOrganizationId.Contains(organization.OrganizationId) || parameter.ListOrganizationId.Count == 0)
                                && (orderStatus.OrderStatusCode == "IP" || orderStatus.OrderStatusCode == "DLV" || orderStatus.OrderStatusCode == "PD" || orderStatus.OrderStatusCode == "COMP")
                                && (parameter.StartDate == null || parameter.StartDate == DateTime.MinValue || parameter.StartDate.Value.Date <= customerOrder.OrderDate.Date)
                                && (parameter.EndDate == null || parameter.EndDate == DateTime.MinValue || parameter.EndDate.Value.Date >= customerOrder.OrderDate.Date)
                                select new
                                {
                                    employee.EmployeeId,
                                    employee.EmployeeCode,
                                    employee.EmployeeName,
                                    contact.ContactId,
                                    contact.AvatarUrl,
                                    organization.OrganizationName,
                                    Amount = (decimal)((customerOrder.DiscountType == true) ? (customerOrder.Amount * (1 - customerOrder.DiscountValue / 100)) : (customerOrder.Amount - customerOrder.DiscountValue))
                                }
                                ).ToList();
            var newList = listEmployee.GroupBy(x => new { x.EmployeeId, x.EmployeeCode, x.EmployeeName, x.ContactId, x.AvatarUrl, x.OrganizationName }).Select(y => new
            {
                Id = y.Key,
                y.Key.EmployeeId,
                y.Key.EmployeeCode,
                y.Key.EmployeeName,
                y.Key.ContactId,
                y.Key.AvatarUrl,
                y.Key.OrganizationName,
                Total = y.Sum(s => s.Amount)
            }).OrderByDescending(x => x.Total).ToList();

            List<dynamic> lstResultEmployee = new List<dynamic>();
            newList.ForEach(item =>
            {
                var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                sampleObject.Add("EmployeeId", item.EmployeeId);
                sampleObject.Add("EmployeeCode", item.EmployeeCode);
                sampleObject.Add("EmployeeName", item.EmployeeName);
                sampleObject.Add("ContactId", item.ContactId);
                sampleObject.Add("AvatarUrl", item.AvatarUrl);
                sampleObject.Add("OrganizationName", item.OrganizationName);
                sampleObject.Add("Total", item.Total);
                lstResultEmployee.Add(sampleObject);
            });

            return new GetEmployeeByTopRevenueResult()
            {
                listEmployee = lstResultEmployee
            };
        }

        public ExportEmployeeRevenueResult ExportEmployeeRevenue(ExportEmployeeRevenueParameter parameter)
        {
            GetEmployeeByTopRevenueParameter ttt = new GetEmployeeByTopRevenueParameter()
            {
                EmployeeId = parameter.EmployeeId,
                StartDate = parameter.StartDate,
                EndDate = parameter.EndDate,
                ListOrganizationId = parameter.ListOrganizationId,
                UserId = parameter.UserId
            };
            var listEmployee = this.GetEmployeeByTopRevenue(ttt).listEmployee;
            byte[] excelOutput = null;
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.EMPLOYEE, "Export Employee Revenue", parameter.UserId);
            excelOutput = this.ExportEmployeeRevenue(listEmployee);

            return new ExportEmployeeRevenueResult()
            {
                ExcelFile = excelOutput
            };
        }
        public GetStatisticForEmpDashBoardResult GetStatisticForEmpDashBoard(GetStatisticForEmpDashBoardParameter parameter)
        {
            #region Phan quyen User va lay ra tat ca cac nhan vien co quyen
            var currentEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentEmp = context.Employee.FirstOrDefault(emp => emp.EmployeeId == currentEmpId);
            var contacts = context.Contact.Where(w => w.ObjectType == ObjectType.EMPLOYEE).ToList();
            // Lay ra to chuc cua nguoi hien tai => orgRoot
            var organizations = context.Organization.ToList();
            var orgRoot = organizations.FirstOrDefault(o => o.OrganizationId == currentEmp.OrganizationId);
            var listOrg = ListChildOfParamToSearch(orgRoot.OrganizationId, organizations);
            var listEmpDashboard = new List<EmployeeEntityModel>();
            listEmpDashboard.Add(new EmployeeEntityModel()
            {
                EmployeeName = "",
                EmployeeCode = "ORG",
                OrganizationName = orgRoot.OrganizationName,
                OrganizationId = orgRoot.OrganizationId,
                PositionId = null,
                PositionName = orgRoot.Level.ToString()
            });
            if (!currentEmp.IsManager)
            {
                listEmpDashboard.Add(new EmployeeEntityModel()
                {
                    EmployeeId = currentEmpId,
                    ContactId = contacts.FirstOrDefault(c => c.ObjectId == currentEmpId).ContactId,
                    EmployeeName = currentEmp.EmployeeName,
                    EmployeeCode = currentEmp.EmployeeCode,
                    OrganizationId = currentEmp.OrganizationId,
                    AvatarUrl = contacts.FirstOrDefault(c => c.ObjectId == currentEmpId).AvatarUrl
                });
            }
            else
            {
                (from emp in context.Employee
                     //join con in context.Contact on emp.EmployeeId equals con.ObjectId
                 where emp.OrganizationId == orgRoot.OrganizationId && emp.EmployeeName.ToLower().Contains(parameter.KeyName.ToLower().Trim())
                 select new EmployeeEntityModel()
                 {
                     EmployeeId = emp.EmployeeId,
                     ContactId = contacts.FirstOrDefault(f => f.ObjectId == emp.EmployeeId).ContactId,
                     EmployeeName = emp.EmployeeName,
                     EmployeeCode = emp.EmployeeCode,
                     OrganizationId = emp.OrganizationId,
                     AvatarUrl = contacts.FirstOrDefault(f => f.ObjectId == emp.EmployeeId).AvatarUrl,
                     LastName = contacts.FirstOrDefault(f => f.ObjectId == emp.EmployeeId).LastName
                 }).OrderBy(e => e.LastName).ToList().ForEach(l => listEmpDashboard.Add(l));

                listOrg.ForEach(org =>
                {
                    listEmpDashboard.Add(new EmployeeEntityModel()
                    {
                        EmployeeName = "",
                        EmployeeCode = "ORG",
                        OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == org).OrganizationName,
                        OrganizationId = organizations.FirstOrDefault(o => o.OrganizationId == org).OrganizationId,
                        PositionId = organizations.FirstOrDefault(o => o.OrganizationId == org).ParentId,
                        PositionName = organizations.FirstOrDefault(o => o.OrganizationId == org).Level.ToString()
                    });
                    (from emp in context.Employee
                         //join con in context.Contact on emp.EmployeeId equals con.ObjectId
                     where emp.Active == true && emp.OrganizationId == org && emp.EmployeeName.ToLower().Contains(parameter.KeyName.ToLower().Trim())
                     select new EmployeeEntityModel()
                     {
                         EmployeeId = emp.EmployeeId,
                         ContactId = contacts.FirstOrDefault(f => f.ObjectId == emp.EmployeeId).ContactId,
                         EmployeeName = emp.EmployeeName,
                         EmployeeCode = emp.EmployeeCode,
                         OrganizationId = emp.OrganizationId,
                         AvatarUrl = contacts.FirstOrDefault(f => f.ObjectId == emp.EmployeeId).AvatarUrl,
                         LastName = contacts.FirstOrDefault(f => f.ObjectId == emp.EmployeeId).LastName
                     }).OrderBy(e => e.LastName).ToList().ForEach(l => listEmpDashboard.Add(l));
                });
            }
            #endregion

            #region Tim kiem cac EmpRequest lien quan den tuan hien tai          

            var typeRequestList = new List<string>() { "NKL", "NP" };
            var listRequest = (from rq in context.EmployeeRequest
                               join stt in context.Category on rq.StatusId equals stt.CategoryId
                               join typestt in context.Category on rq.TypeRequest equals typestt.CategoryId
                               join start in context.Category on rq.StartTypeTime equals start.CategoryId
                               join end in context.Category on rq.EndTypeTime equals end.CategoryId
                               where stt.CategoryCode == "Approved" &&
                               typeRequestList.Contains(typestt.CategoryCode) &&
                                     (
                                        (rq.StartDate.Value.Date >= parameter.FirstOfWeek.Date && rq.StartDate.Value.Date <= parameter.LastOfWeek.Date) ||
                                        (rq.EnDate.Value.Date >= parameter.FirstOfWeek.Date && rq.EnDate.Value.Date <= parameter.LastOfWeek.Date)
                                     )
                               select new EmployeeRequestEntityModel()
                               {
                                   EmployeeRequestId = rq.EmployeeRequestId,
                                   EmployeeRequestCode = rq.EmployeeRequestCode,
                                   OfferEmployeeCode = rq.OfferEmployeeCode,
                                   OfferEmployeeId = rq.OfferEmployeeId,
                                   StartDate = rq.StartDate,
                                   EnDate = rq.EnDate,
                                   ShiftName = start.CategoryCode + "," + end.CategoryCode
                               }).OrderBy(r => r.StartDate.Value.Date).ToList();
            #endregion

            #region Tim kiem cac nhan vien co ngay sinh nhat gan nhat trong 2 tuan do lai
            var nextTwoWeek = DateTime.Now.AddDays(14);
            var listEmpNearestBirthday = (from emp in context.Employee
                                          join con in context.Contact on emp.EmployeeId equals con.ObjectId
                                          join org in context.Organization on emp.OrganizationId equals org.OrganizationId
                                          where emp.Active == true && con.DateOfBirth != null &&
                                                (
                                                    (con.DateOfBirth.Value.Month > DateTime.Now.Month) ||
                                                    (con.DateOfBirth.Value.Month == DateTime.Now.Month && con.DateOfBirth.Value.Day >= DateTime.Now.Day)
                                                ) &&
                                                (
                                                    (con.DateOfBirth.Value.Month < nextTwoWeek.Month) ||
                                                    (con.DateOfBirth.Value.Month == nextTwoWeek.Month && con.DateOfBirth.Value.Day <= nextTwoWeek.Day)
                                                ) &&
                                                (
                                                    (emp.OrganizationId == orgRoot.OrganizationId) ||
                                                    (listOrg.Contains(emp.OrganizationId.Value))
                                                ) && currentEmp.IsManager && ((emp.OrganizationId == orgRoot.OrganizationId || listOrg.Contains(emp.OrganizationId.Value)))
                                          select new EmployeeEntityModel()
                                          {
                                              EmployeeId = emp.EmployeeId,
                                              EmployeeName = emp.EmployeeName,
                                              EmployeeCode = emp.EmployeeCode,
                                              OrganizationName = org.OrganizationName,
                                              StartedDate = con.DateOfBirth,
                                              ContactId = con.ContactId
                                          }).OrderBy(r => r.StartedDate.Value.Day).OrderBy(r => r.StartedDate.Value.Month).ToList();
            #endregion
            #region Tim kiem cac nhan vien sap het han hop dong trong 1 thang do lai
            var nowofnextMonth = DateTime.Now.AddDays(30);
            var listEmpEndContract = (from emp in context.Employee
                                      join con in context.Contact on emp.EmployeeId equals con.ObjectId
                                      where emp.ContractEndDate != null && emp.Active == true && emp.ContractEndDate.Value.Date >= DateTime.Now.Date &&
                                            (
                                                (currentEmp.IsManager && (emp.OrganizationId == orgRoot.OrganizationId || listOrg.Contains(emp.OrganizationId.Value))) ||
                                                (!currentEmp.IsManager && emp.EmployeeId == currentEmp.EmployeeId)
                                            ) && emp.ContractEndDate.Value.Date <= nowofnextMonth.Date
                                      select new EmployeeEntityModel()
                                      {
                                          EmployeeId = emp.EmployeeId,
                                          ContactId = con.ContactId,
                                          ContractEndDate = emp.ContractEndDate,
                                          EmployeeName = emp.EmployeeName,
                                          EmployeeCode = emp.EmployeeCode
                                      }).OrderBy(d => d.ContractEndDate.Value.Date).ToList();
            listEmpEndContract.ForEach(item =>
            {
                if (item.ContractType != null)
                {
                    item.ContractName = context.Category.FirstOrDefault(l => l.CategoryId == item.ContractType)?.CategoryName;
                }
            });
            #endregion
            return new GetStatisticForEmpDashBoardResult()
            {
                Message = "Success",
                Status = true,
                IsManager = currentEmp.IsManager,
                ListEmployee = listEmpDashboard,
                ListRequestInsideWeek = listRequest,
                ListEmpNearestBirthday = listEmpNearestBirthday,
                ListEmpEndContract = listEmpEndContract
            };
        }
        private byte[] ExportEmployeeRevenue(List<dynamic> lstEmployee)
        {
            string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
            string fileName = @"ExcelEmployeeRevenue.xlsx";
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            //MemoryStream output = new MemoryStream();
            byte[] data = null;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                //worksheet.Name =string.Format("Bảng lương {0}/{1}",month,year);
                worksheet.InsertRow(8, lstEmployee.Count);
                if (lstEmployee.Count > 0)
                {
                    int rowIndex = 6;
                    int i = 0;
                    while (i < lstEmployee.Count)
                    {
                        worksheet.Cells[rowIndex, 1].Value = lstEmployee[i].OrganizationName;
                        worksheet.Cells[rowIndex, 2].Value = lstEmployee[i].EmployeeCode;
                        worksheet.Cells[rowIndex, 3].Value = lstEmployee[i].EmployeeName;
                        worksheet.Cells[rowIndex, 4].Value = string.Format(new CultureInfo("vi-VN"), "{0:#,#}", lstEmployee[i].Total);
                        rowIndex++;
                        i++;
                    }
                    string newFilePath = Path.Combine(rootFolder, @"ExportedExcel.xlsx");
                    package.SaveAs(new FileInfo(newFilePath));
                    data = File.ReadAllBytes(newFilePath);
                    File.Delete(newFilePath);
                    //package.SaveAs(output);
                }
                return data;
            }
        }

        public GetEmployeeCareStaffResult GetEmployeeCareStaff(GetEmployeeCareStaffParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.EMPLOYEE, "Get Employee Care Staff", parameter.UserId);
            var IsManager = parameter.IsManager;
            List<dynamic> listResult = new List<dynamic>();

            if (IsManager)
            {
                //Nếu là quản lý:
                //Lấy danh sách tất cả nhân viên cấp dưới thuộc phòng ban quản lý
                //Bước 1: Lấy danh sách tất cả phòng ban cấp dưới nếu có
                var empl = (from employee in context.Employee
                            where employee.EmployeeId == parameter.EmployeeId
                            select new
                            {
                                employee.OrganizationId
                            }).FirstOrDefault();

                List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                listOrganizationChildrenId.Add(empl.OrganizationId);

                #region Hung Edit this Function
                var organizationList = context.Organization.ToList();
                getOrganizationChildrenId(organizationList, empl.OrganizationId, listOrganizationChildrenId);
                #endregion 
                //Bước 2: Lấy danh sách tất cả nhân viên thuộc danh sách phòng ban vừa lấy được
                var listEmployee = (from employee in context.Employee
                                        //where (listOrganizationChildrenId.Contains(employee.OrganizationId) || listOrganizationChildrenId.Count == 0) && (employee.Active == true)
                                    where (listOrganizationChildrenId.Contains(employee.OrganizationId) || listOrganizationChildrenId.Count == 0)
                                    select new
                                    {
                                        employee.EmployeeId,
                                        employee.EmployeeName,
                                        employee.EmployeeCode,
                                        employee.Active //thêm trạng thái nhân viên
                                    }).ToList();

                listEmployee.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("employeeId", item.EmployeeId);
                    sampleObject.Add("employeeName", item.EmployeeName);
                    sampleObject.Add("employeeCode", item.EmployeeCode);
                    sampleObject.Add("active", item.Active); //thêm trạng thái nhân viên
                    listResult.Add(sampleObject);
                });
            }
            else
            {
                //Nếu là nhân viên:
                //Lấy danh sách các nhân viên trong cùng phòng ban với nhân viên đang đăng nhập
                //var organizationId = context.Employee.Where(e => e.EmployeeId == parameter.EmployeeId).Select(e => e.OrganizationId).FirstOrDefault();
                //var listEmployee = context.Employee.Where(e => e.OrganizationId == organizationId && e.Active == true).ToList();

                //Update SRS: nếu là nhân viên, chỉ lấy nhân viên đó - DUNGPT
                var listEmployee = context.Employee.Where(e => e.EmployeeId == parameter.EmployeeId).ToList();
                listEmployee.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("employeeId", item.EmployeeId);
                    sampleObject.Add("employeeName", item.EmployeeName);
                    sampleObject.Add("employeeCode", item.EmployeeCode);
                    sampleObject.Add("active", item.Active); //thêm trạng thái nhân viên
                    listResult.Add(sampleObject);
                });

            }

            return new GetEmployeeCareStaffResult()
            {
                employeeList = listResult
            };
        }
        //Hung Edit this Function
        private List<Guid?> getOrganizationChildrenId(List<Organization> organizationList, Guid? id, List<Guid?> list)
        {
            var organizations = organizationList.Where(o => o.ParentId == id).ToList();
            organizations.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(organizationList, item.OrganizationId, list);
            });

            return list;
        }

        public SearchEmployeeFromListResult SearchEmployeeFromList(SearchEmployeeFromListParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.EMPLOYEE, "Search employee", parameter.UserId);
            var currentUserEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == currentUserEmpId);
            Guid currentUserOrgId = Guid.Empty;
            if (employee != null)
                currentUserOrgId = employee.OrganizationId.Value;
            var employeeList = new List<EmployeeEntityModel>();
            List<Organization> organizations = null;
            List<Guid?> listGetAllChild = new List<Guid?>();
            if (parameter.IsManager)
            {
                if (currentUserOrgId != null && currentUserOrgId != Guid.Empty)
                {
                    var organizationList = context.Organization.ToList();
                    if (parameter.OrganizationId == null)
                    {
                        listGetAllChild.Add(currentUserOrgId);
                        listGetAllChild = getOrganizationChildrenId(organizationList, currentUserOrgId, listGetAllChild);
                    }

                    else
                    {
                        listGetAllChild.Add(parameter.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(organizationList, parameter.OrganizationId.Value, listGetAllChild);
                    }
                    organizations = organizationList.Where(w => listGetAllChild.Contains(w.OrganizationId)).ToList();

                }
                #region Comment By Hung
                //employeeList = (from c in context.Contact
                //                join e in context.Employee on c.ObjectId equals e.EmployeeId
                //                join u in context.User on c.ObjectId equals u.EmployeeId into cu
                //                from x in cu.DefaultIfEmpty()
                //                where
                //                        (c.FirstName.ToLower().Contains(parameter.FirstName.ToLower().Trim()) || parameter.FirstName == null || parameter.FirstName.Trim() == "") &&
                //                        (c.LastName.ToLower().Contains(parameter.LastName.ToLower().Trim()) || parameter.LastName == null || parameter.LastName.Trim() == "") &&
                //                        (e.EmployeeCode.ToLower().Contains(parameter.IdentityId.ToLower().Trim()) || parameter.IdentityId == null || parameter.IdentityId.Trim() == "") &&
                //                        (parameter.FromContractExpiryDate == null || parameter.FromContractExpiryDate == DateTime.MinValue || parameter.FromContractExpiryDate.Value.Date <= e.ContractEndDate.Value.Date) &&
                //                        (parameter.ToContractExpiryDate == null || parameter.ToContractExpiryDate == DateTime.MinValue || parameter.ToContractExpiryDate.Value.Date >= e.ContractEndDate.Value.Date) &&
                //                        (
                //                        (parameter.FromBirthDay == null) ||
                //                        (c.DateOfBirth.Value.Month > parameter.FromBirthDay.Value.Month) ||
                //                        (c.DateOfBirth.Value.Month == parameter.FromBirthDay.Value.Month && c.DateOfBirth.Value.Day >= parameter.FromBirthDay.Value.Day)
                //                        ) &&
                //                        (
                //                        (parameter.ToBirthDay == null) ||
                //                        (c.DateOfBirth.Value.Month < parameter.ToBirthDay.Value.Month) ||
                //                        (c.DateOfBirth.Value.Month == parameter.ToBirthDay.Value.Month && c.DateOfBirth.Value.Day <= parameter.ToBirthDay.Value.Day)
                //                        ) &&
                //                        (x.UserName.ToLower().Contains(parameter.UserName.ToLower().Trim()) || parameter.UserName == null || parameter.UserName.Trim() == "") &&
                //                        (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId.Value)) &&
                //                        (parameter.ListPosition.Count == 0 || parameter.ListPosition.Contains(e.PositionId.Value) || e.PositionId == null)
                //                select new EmployeeEntityModel
                //                {
                //                    EmployeeId = e.EmployeeId,
                //                    ContactId = c.ContactId,
                //                    EmployeeName = e.EmployeeName,
                //                    Username = x == null ? "" : x.UserName,
                //                    OrganizationId = e.OrganizationId,
                //                    OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId) == null ? "" : organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName,
                //                    Identity = c.IdentityId,
                //                    EmployeeCode = e.EmployeeCode,
                //                    AvatarUrl = c.AvatarUrl,
                //                    PositionId = e.PositionId,
                //                    CreatedById = e.CreatedById,
                //                    CreatedDate = e.CreatedDate,
                //                    LastName = c.LastName
                //                }).OrderBy(x => x.LastName).ToList();
                #endregion
            }
            else
            {
                var organizationList = context.Organization.ToList();
                if (parameter.OrganizationId == null)
                {
                    listGetAllChild.Add(currentUserOrgId);
                    listGetAllChild = getOrganizationChildrenId(organizationList, currentUserOrgId, listGetAllChild);
                }

                else
                {
                    listGetAllChild.Add(parameter.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(organizationList, parameter.OrganizationId.Value, listGetAllChild);
                }
                organizations = organizationList.Where(w => listGetAllChild.Contains(w.OrganizationId)).ToList();
                #region Comment By Hung
                //employeeList = (from c in context.Contact
                //                join e in context.Employee on c.ObjectId equals e.EmployeeId
                //                join u in context.User on c.ObjectId equals u.EmployeeId into cu
                //                from x in cu.DefaultIfEmpty()
                //                where
                //                        (e.CreatedById == parameter.UserId) &&
                //                        (c.FirstName.ToLower().Contains(parameter.FirstName.ToLower().Trim()) || parameter.FirstName == null || parameter.FirstName.Trim() == "") &&
                //                        (c.LastName.ToLower().Contains(parameter.LastName.ToLower().Trim()) || parameter.LastName == null || parameter.LastName.Trim() == "") &&
                //                        (e.EmployeeCode.ToLower().Contains(parameter.IdentityId.ToLower().Trim()) || parameter.IdentityId == null || parameter.IdentityId.Trim() == "") &&
                //                        (parameter.FromContractExpiryDate == null || parameter.FromContractExpiryDate == DateTime.MinValue || parameter.FromContractExpiryDate.Value.Date <= e.ContractEndDate.Value.Date) &&
                //                        (parameter.ToContractExpiryDate == null || parameter.ToContractExpiryDate == DateTime.MinValue || parameter.ToContractExpiryDate.Value.Date >= e.ContractEndDate.Value.Date) &&
                //                        (
                //                        (parameter.FromBirthDay == null) ||
                //                        (c.DateOfBirth.Value.Month > parameter.FromBirthDay.Value.Month) ||
                //                        (c.DateOfBirth.Value.Month == parameter.FromBirthDay.Value.Month && c.DateOfBirth.Value.Day >= parameter.FromBirthDay.Value.Day)
                //                        ) &&
                //                        (
                //                        (parameter.ToBirthDay == null) ||
                //                        (c.DateOfBirth.Value.Month < parameter.ToBirthDay.Value.Month) ||
                //                        (c.DateOfBirth.Value.Month == parameter.ToBirthDay.Value.Month && c.DateOfBirth.Value.Day <= parameter.ToBirthDay.Value.Day)
                //                        ) &&
                //                        (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId.Value)) &&
                //                        (x.UserName.ToLower().Contains(parameter.UserName.ToLower().Trim()) || parameter.UserName == null || parameter.UserName.Trim() == "") &&
                //                        (parameter.ListPosition.Count == 0 || parameter.ListPosition.Contains(e.PositionId.Value) || e.PositionId == null)
                //                select new EmployeeEntityModel
                //                {
                //                    EmployeeId = e.EmployeeId,
                //                    ContactId = c.ContactId,
                //                    EmployeeName = e.EmployeeName,
                //                    Username = x == null ? "" : x.UserName,
                //                    OrganizationId = e.OrganizationId,
                //                    OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId) == null ? "" : organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName,
                //                    Identity = c.IdentityId,
                //                    EmployeeCode = e.EmployeeCode,
                //                    AvatarUrl = c.AvatarUrl,
                //                    PositionId = e.PositionId,
                //                    CreatedById = e.CreatedById,
                //                    CreatedDate = e.CreatedDate,
                //                    LastName = c.LastName
                //                }).OrderBy(x => x.LastName).ToList();
                #endregion
            }

            #region Add By Hung
            if (organizations == null)
                organizations = context.Organization.Where(w => listGetAllChild.Contains(w.OrganizationId)).ToList();
            if (parameter.IsQuitWork)
            {
                var userDisabledIds = context.User.Where(c => c.Disabled == false).Select(c => c.EmployeeId).ToList();
                employeeList = context.Employee.Where(e => (e.Active == false) &&
                                                            userDisabledIds.Contains(e.EmployeeId) &&
                                                            (e.EmployeeCode.ToLower().Contains(parameter.IdentityId.ToLower().Trim()) || parameter.IdentityId == null || parameter.IdentityId.Trim() == "") &&
                                                            (parameter.FromContractExpiryDate == null || parameter.FromContractExpiryDate == DateTime.MinValue || parameter.FromContractExpiryDate.Value.Date <= e.ContractEndDate.Value.Date) &&
                                                            (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId.Value)) &&
                                                            (parameter.ListPosition.Count == 0 || parameter.ListPosition.Contains(e.PositionId.Value) || e.PositionId == null))
                                                            .Select(e => new EmployeeEntityModel
                                                            {
                                                                EmployeeId = e.EmployeeId,
                                                                EmployeeName = e.EmployeeName,
                                                                OrganizationId = e.OrganizationId,
                                                                OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId) == null ? "" : organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName,
                                                                EmployeeCode = e.EmployeeCode,
                                                                PositionId = e.PositionId,
                                                                CreatedById = e.CreatedById,
                                                                CreatedDate = e.CreatedDate,
                                                                Active = e.Active,
                                                                //ActiveUser = u.Active
                                                            }).ToList();
                //employeeList = (from e in context.Employee
                //                join u in context.User on e.EmployeeId equals u.EmployeeId
                //                where (u.Disabled == false) && (e.Active == false) &&
                //                        (e.EmployeeCode.ToLower().Contains(parameter.IdentityId.ToLower().Trim()) || parameter.IdentityId == null || parameter.IdentityId.Trim() == "") &&
                //                        (parameter.FromContractExpiryDate == null || parameter.FromContractExpiryDate == DateTime.MinValue || parameter.FromContractExpiryDate.Value.Date <= e.ContractEndDate.Value.Date) &&
                //                        (parameter.ToContractExpiryDate == null || parameter.ToContractExpiryDate == DateTime.MinValue || parameter.ToContractExpiryDate.Value.Date >= e.ContractEndDate.Value.Date) &&
                //                        (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId.Value)) &&
                //                        (parameter.ListPosition.Count == 0 || parameter.ListPosition.Contains(e.PositionId.Value) || e.PositionId == null)
                //                select new EmployeeEntityModel
                //                {
                //                    EmployeeId = e.EmployeeId,
                //                    //ContactId = c.ContactId,
                //                    EmployeeName = e.EmployeeName,
                //                    //Username = x == null ? "" : x.UserName,
                //                    OrganizationId = e.OrganizationId,
                //                    OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId) == null ? "" : organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName,
                //                    //Identity = c.IdentityId,
                //                    EmployeeCode = e.EmployeeCode,
                //                    //AvatarUrl = c.AvatarUrl,
                //                    PositionId = e.PositionId,
                //                    CreatedById = e.CreatedById,
                //                    CreatedDate = e.CreatedDate,
                //                    Active = e.Active,
                //                    ActiveUser = u.Active
                //                    //LastName = c.LastName
                //                }).ToList();
            }
            else
            {
                var userEmployeeIds = context.User.Where(c => c.Disabled == false).Select(c => c.EmployeeId).ToList();
                employeeList = context.Employee.Where(e => e.Active == true &&
                                                        userEmployeeIds.Contains(e.EmployeeId) &&
                                                        (e.EmployeeCode.ToLower().Contains(parameter.IdentityId.ToLower().Trim()) || parameter.IdentityId == null || parameter.IdentityId.Trim() == "") &&
                                                        (parameter.FromContractExpiryDate == null || parameter.FromContractExpiryDate == DateTime.MinValue || parameter.FromContractExpiryDate.Value.Date <= e.ContractEndDate.Value.Date) &&
                                                        (parameter.ToContractExpiryDate == null || parameter.ToContractExpiryDate == DateTime.MinValue || parameter.ToContractExpiryDate.Value.Date >= e.ContractEndDate.Value.Date) &&
                                                        (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId.Value)) &&
                                                        (parameter.ListPosition.Count == 0 || parameter.ListPosition.Contains(e.PositionId.Value) || e.PositionId == null))
                                                        .Select(e => new EmployeeEntityModel
                                                        {
                                                            EmployeeId = e.EmployeeId,
                                                            EmployeeName = e.EmployeeName,
                                                            OrganizationId = e.OrganizationId,
                                                            OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId) == null ? "" : organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName,
                                                            EmployeeCode = e.EmployeeCode,
                                                            PositionId = e.PositionId,
                                                            CreatedById = e.CreatedById,
                                                            CreatedDate = e.CreatedDate,
                                                            Active = e.Active,
                                                            //ActiveUser = u.Active
                                                        }).ToList();

                //employeeList = (from e in context.Employee
                //                join u in context.User on e.EmployeeId equals u.EmployeeId
                //                where (u.Disabled == false) && (e.Active == true) &&
                //                        (e.EmployeeCode.ToLower().Contains(parameter.IdentityId.ToLower().Trim()) || parameter.IdentityId == null || parameter.IdentityId.Trim() == "") &&
                //                        (parameter.FromContractExpiryDate == null || parameter.FromContractExpiryDate == DateTime.MinValue || parameter.FromContractExpiryDate.Value.Date <= e.ContractEndDate.Value.Date) &&
                //                        (parameter.ToContractExpiryDate == null || parameter.ToContractExpiryDate == DateTime.MinValue || parameter.ToContractExpiryDate.Value.Date >= e.ContractEndDate.Value.Date) &&
                //                        (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(e.OrganizationId.Value)) &&
                //                        (parameter.ListPosition.Count == 0 || parameter.ListPosition.Contains(e.PositionId.Value) || e.PositionId == null)
                //                select new EmployeeEntityModel
                //                {
                //                    EmployeeId = e.EmployeeId,
                //                    //ContactId = c.ContactId,
                //                    EmployeeName = e.EmployeeName,
                //                    //Username = x == null ? "" : x.UserName,
                //                    OrganizationId = e.OrganizationId,
                //                    OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId) == null ? "" : organizations.FirstOrDefault(o => o.OrganizationId == e.OrganizationId).OrganizationName,
                //                    //Identity = c.IdentityId,
                //                    EmployeeCode = e.EmployeeCode,
                //                    //AvatarUrl = c.AvatarUrl,
                //                    PositionId = e.PositionId,
                //                    CreatedById = e.CreatedById,
                //                    CreatedDate = e.CreatedDate,
                //                    Active = e.Active,
                //                    ActiveUser = u.Active
                //                    //LastName = c.LastName
                //                }).ToList();
            }
            List<Guid> listEmployeeId = new List<Guid>();
            employeeList.ForEach(item =>
            {
                listEmployeeId.Add(item.EmployeeId.Value);
            });

            var contacts = context.Contact.Where(w => listEmployeeId.Contains(w.ObjectId) && w.ObjectType == ObjectType.EMPLOYEE &&
                                                (parameter.FirstName == null || parameter.FirstName.Trim() == "" || w.FirstName.ToLower().Contains(parameter.FirstName.ToLower().Trim())) &&
                                                (parameter.LastName == null || parameter.LastName.Trim() == "" || w.LastName.ToLower().Contains(parameter.LastName.ToLower().Trim())) &&
                                                ((parameter.FromBirthDay == null) || (w.DateOfBirth.Value.Month > parameter.FromBirthDay.Value.Month) || (w.DateOfBirth.Value.Month == parameter.FromBirthDay.Value.Month && w.DateOfBirth.Value.Day >= parameter.FromBirthDay.Value.Day)) &&
                                                ((parameter.ToBirthDay == null) || (w.DateOfBirth.Value.Month < parameter.ToBirthDay.Value.Month) || (w.DateOfBirth.Value.Month == parameter.ToBirthDay.Value.Month && w.DateOfBirth.Value.Day <= parameter.ToBirthDay.Value.Day))).ToList();
            var users = context.User.Where(w => listEmployeeId.Contains(w.EmployeeId.Value) &&
                                          (parameter.UserName == null || parameter.UserName.Trim() == "" || w.UserName.ToLower().Contains(parameter.UserName.ToLower().Trim()))).ToList();

            employeeList.ForEach(item =>
            {
                var contact = contacts.FirstOrDefault(f => f.ObjectId == item.EmployeeId);
                if (contact != null)
                {
                    item.ContactId = contact.ContactId;
                    item.Identity = contact.IdentityId;
                    item.AvatarUrl = contact.AvatarUrl;
                    item.LastName = contact.LastName;
                }
                var user = users.FirstOrDefault(f => f.EmployeeId == item.EmployeeId);
                if (user != null)
                {
                    item.Username = user.UserName;
                }
            });
            employeeList = employeeList.Where(w => w.Username != null && w.ContactId != null).OrderBy(x => x.LastName).ToList();
            #endregion

            return new SearchEmployeeFromListResult
            {
                Status = true,
                CurrentOrganizationId = currentUserOrgId,
                EmployeeList = employeeList
            };
        }

        public GetAllEmpAccIdentityResult GetAllEmpAccIdentity(GetAllEmpAccIdentityParameter parameter)
        {
            List<string> listAccountEmployee;
            if (parameter.EmployeeId != Guid.Empty)
                listAccountEmployee = context.User.Where(u => u.EmployeeId != parameter.EmployeeId).Select(x => x.UserName.ToLower()).ToList();
            else
                listAccountEmployee = context.User.Select(x => x.UserName.ToLower()).ToList();
            return new GetAllEmpAccIdentityResult
            {
                Status = true,
                ListAccEmployee = listAccountEmployee
            };
        }

        public DisableEmployeeResult DisableEmployee(DisableEmployeeParameter parameter)
        {
            try
            {
                var userDisable = context.User.FirstOrDefault(u => u.EmployeeId == parameter.EmployeeId);
                userDisable.Disabled = true;
                userDisable.UpdatedById = parameter.UserId;
                userDisable.UpdatedDate = DateTime.Now;
                context.User.Update(userDisable);
                context.SaveChanges();

                return new DisableEmployeeResult
                {
                    Status = true,
                    Message = CommonMessage.Employee.DELETE_SUCCESS,
                };
            }
            catch (Exception ex)
            {
                return new DisableEmployeeResult
                {
                    Message = CommonMessage.Employee.DELETE_FAIL,
                    Status = false,
                };
            }
        }

        public CheckAdminLoginResult CheckAdminLogin(CheckAdminLoginParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);

                return new CheckAdminLoginResult
                {
                    IsAdmin = user.IsAdmin,
                    Message = "OK",
                    Status = false,
                };
            }
            catch (Exception ex)
            {
                return new CheckAdminLoginResult
                {
                    Message = ex.Message,
                    Status = false,
                };
            }
        }
    }
}
