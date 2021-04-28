using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Email;
using TN.TNM.DataAccess.Messages.Results.Email;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmailDAO : BaseDAO, IEmailDataAccess
    {
        private IHostingEnvironment hostingEnvironment;
        public IConfiguration Configuration { get; }
        public static string WEB_ENDPOINT;
        public static string PrimaryDomain;
        public static int PrimaryPort;
        public static string SecondayDomain;
        public static int SecondaryPort;
        public static string Email;
        public static string Password;
        public static string BannerUrl;
        public static string Ssl;
        public static string Company;

        public EmailDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
            this.Configuration = iconfiguration;
        }

        public void GetConfiguration()
        {
            PrimaryDomain = Configuration["PrimaryDomain"];
            PrimaryPort = int.Parse(Configuration["PrimaryPort"]);
            SecondayDomain = Configuration["SecondayDomain"];
            SecondaryPort = int.Parse(Configuration["SecondaryPort"]);
            Email = Configuration["Email"];
            Password = Configuration["Password"];
            Ssl = Configuration["Ssl"];
            Company = Configuration["Company"];
            BannerUrl = Configuration["BannerUrl"];
            WEB_ENDPOINT = Configuration["WEB_ENDPOINT"];
        }

        public SendEmailResult SendEmail(SendEmailParameter parameter)
        {
            GetConfiguration();
            string webRootPath = hostingEnvironment.WebRootPath;
            var file = Path.Combine(webRootPath, "emailTemplateCreateEmp.html");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }
            //Thay doi cac thuoc tinh can thiet trong htmltemplate
            body = body.Replace("[UserName]", parameter.UserName);
            body = body.Replace("[FullName]", parameter.FullName);
            body = body.Replace("[Url]", WEB_ENDPOINT);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
            mail.From = new MailAddress(Email, "N8");
            mail.To.Add(parameter.EmailAddress);
            mail.Subject = string.Format("Chào mừng {0} đến với N8", parameter.FullName);
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
            SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
            SmtpServer.Send(mail);

            return new SendEmailResult
            {
                Status = true
            };
        }

        public SendEmailAfterEditPicResult SendEmailAfterEditPic(SendEmailAfterEditPicParameter parameter)
        {
            GetConfiguration();
            //Lay ra cac thong tin can thiet
            string webRootPath = hostingEnvironment.WebRootPath;
            var file = Path.Combine(webRootPath, "emailTemplate.html");
            string body = string.Empty;
            var currentUserEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var currentUserEmp = context.Contact.FirstOrDefault(c => c.ObjectId == currentUserEmpId);
            var currentUserEmpName = currentUserEmp.FirstName + " " + currentUserEmp.LastName;
            var pic = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.PicId);
            var picEmail = pic?.Email;
            var lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.LeadId);
            var leadInfo = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.LeadId);
            var leadInfoName = leadInfo.FirstName + " " + leadInfo.LastName;
            var leadInfoPhone = leadInfo.Phone;
            var leadInfoRequirement = context.Category.FirstOrDefault(c => c.CategoryId == lead.InterestedGroupId).CategoryName;

            //Su dung StreamReader de doc htmltemplate 
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }
            //Thay doi cac thuoc tinh can thiet trong htmltemplate
            body = body.Replace("{bannerUrl}", BannerUrl);
            body = body.Replace("{currentUserEmpName}", currentUserEmpName);
            body = body.Replace("{leadName}", leadInfoName);
            body = body.Replace("{picName}", parameter.PicName);
            body = body.Replace("{statusName}", parameter.StatusName);
            body = body.Replace("{poName}", parameter.PotentialName);
            body = body.Replace("{leadUrl}", parameter.CurrentUrl);
            body = body.Replace("{leadPhone}", leadInfoPhone);
            body = body.Replace("{requirement}", leadInfoRequirement);
            body = body.Replace("{requirementDetail}", lead.RequirementDetail);

            var emailCC = context.Contact.Where(c => parameter.EmpCCIdList.Contains(c.ObjectId)).Select(c => c.Email).ToList();

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
            mail.From = new MailAddress(Email, "N8");
            mail.To.Add(picEmail);
            emailCC.ForEach(email =>
            {
                mail.CC.Add(email);
            });
            mail.Subject = "Chỉnh sửa khách hàng tiềm năng";
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
            SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
            SmtpServer.Send(mail);

            return new SendEmailAfterEditPicResult()
            {
                Status = true
            };
        }

        public SendEmailAfterCreatedLeadResult SendEmailAfterCreatedLead(SendEmailAfterCreatedLeadParameter parameter)
        {
            GetConfiguration();
            string webRootPath = hostingEnvironment.WebRootPath;
            var file = Path.Combine(webRootPath, "emailTemplateCreate.html");
            string body = string.Empty;

            var lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.LeadId);
            var leadInfo = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.LeadId);
            var leadInfoName = leadInfo.FirstName + " " + leadInfo.LastName;
            var leadInfoPhone = leadInfo.Phone;
            var leadInfoRequirement = context.Category.FirstOrDefault(c => c.CategoryId == lead.InterestedGroupId).CategoryName;
            var currentUserEmpId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
            var orgId = context.Employee.FirstOrDefault(e => e.EmployeeId == currentUserEmpId).OrganizationId;
            var managerEmp = context.Employee.Where(e => (e.OrganizationId == orgId) && e.IsManager).ToList();

            List<string> emailList = new List<string>();
            if (managerEmp.Count > 0)
            {
                managerEmp.ForEach(emp =>
                {
                    var email = context.Contact.FirstOrDefault(c => c.ObjectId == emp.EmployeeId).Email;
                    emailList.Add(email);
                });
            }

            //using streamreader for reading my htmltemplate   
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{currentUserEmpName}", parameter.CurrentUsername); //replacing the required things  
            body = body.Replace("{leadUrl}", parameter.CurrentUrl);
            body = body.Replace("{leadName}", leadInfoName);
            body = body.Replace("{leadPhone}", leadInfoPhone);
            body = body.Replace("{requirement}", leadInfoRequirement);
            body = body.Replace("{requirementDetail}", lead.RequirementDetail);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
            mail.From = new MailAddress(Email, "N8");
            emailList.ForEach(email =>
            {
                mail.To.Add(email);
            });
            mail.CC.Add(parameter.CurrentUserEmail);
            mail.Subject = "Khách hàng tiềm năng mới";
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
            SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
            SmtpServer.Send(mail);
            return new SendEmailAfterCreatedLeadResult()
            {
                Status = true
            };
        }

        public SendEmailAfterCreateNoteResult SendEmailAfterCreateNote(SendEmailAfterCreateNoteParameter parameter)
        {
            GetConfiguration();
            string webRootPath = hostingEnvironment.WebRootPath;
            var file = Path.Combine(webRootPath, "emailTemplateCreateNote.html");
            string body = string.Empty;

            var lead = context.Lead.FirstOrDefault(l => l.LeadId == parameter.LeadId);
            var leadInfo = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.LeadId);
            var leadInfoName = leadInfo.FirstName + " " + leadInfo.LastName;
            var leadInfoPhone = leadInfo.Phone;
            var leadInfoRequirement = context.Category.FirstOrDefault(c => c.CategoryId == lead.InterestedGroupId).CategoryName;

            List<string> emailList = new List<string>();
            if (parameter.EmployeeIdList.Count > 0)
            {
                parameter.EmployeeIdList.ForEach(empId =>
                {
                    var email = context.Contact.FirstOrDefault(c => c.ObjectId.ToString() == empId).Email;
                    emailList.Add(email);
                });
            }

            if (emailList.Count > 0)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{currentUserEmpName}", parameter.CurrentUsername); //replacing the required things  
                body = body.Replace("{leadUrl}", parameter.CurrentUrl);
                body = body.Replace("{leadName}", leadInfoName);
                body = body.Replace("{leadPhone}", leadInfoPhone);
                body = body.Replace("{requirement}", leadInfoRequirement);
                body = body.Replace("{requirementDetail}", lead.RequirementDetail);
                body = body.Replace("{noteContent}", parameter.NoteContent);
                body = body.Replace("{numberOfFile}", parameter.FileNameArray.Count.ToString());
                string fileName = "";
                parameter.FileNameArray.ForEach(name =>
                {
                    fileName += "- " + name + "<br />";
                });
                body = body.Replace("{fileArray}", fileName);

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                mail.From = new MailAddress(Email, "N8");
                emailList.ForEach(email =>
                {
                    mail.To.Add(email);
                });
                mail.Subject = "Ghi chú mới";
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                SmtpServer.Send(mail);
            }

            return new SendEmailAfterCreateNoteResult()
            {
                Status = true
            };
        }

        private Guid? GetParentOrgId(Guid orgId)
        {
            var org = context.Organization.FirstOrDefault(o => o.OrganizationId == orgId);

            if (org != null && org.ParentId != Guid.Empty)
            {
                return org.ParentId;
            }

            return Guid.Empty;
        }

        public SendEmailEmployeePayslipResult SendEmailEmployeePayslip(SendEmailEmployeePayslipParameter parameter)
        {
            GetConfiguration();
            string webRootPath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
            var file = Path.Combine(webRootPath, "SendEmailEmployeePayslip.html");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }

            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);

            var listEmployeeInsurance = context.EmployeeInsurance.GroupBy(g => g.EmployeeId).SelectMany(s => s.OrderByDescending(g => g.EffectiveDate).Take(1)).ToList();
            var lstEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => parameter.lstEmpMonthySalary.Contains(w.EmployeeMonthySalaryId)).ToList();
            var listContact = context.Contact.Where(w => w.ObjectType == "EMP").Select(s => new { s.ObjectId, s.Email }).ToList();

            lstEmployeeMonthySalary.ForEach(item =>
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Email, "N8");
                var email = listContact.Where(w => w.ObjectId == item.EmployeeId).Select(s => s.Email).FirstOrDefault();
                if (email != null)
                {
                    //email emai "minhpv@tringhiatech.vn"
                    mail.To.Add(email);
                    mail.Subject = string.Format("Phiếu lương tháng {0} năm {1}", item.Month, item.Year);
                    var itemBody = body;

                    itemBody = itemBody.Replace("[Month]", item.Month.ToString());
                    itemBody = itemBody.Replace("[Year]", item.Year.ToString());
                    itemBody = itemBody.Replace("[EmployeeName]", item.EmployeeName);
                    itemBody = itemBody.Replace("[PostionName]", item.PostionName);
                    itemBody = itemBody.Replace("[EmployeeCode]", item.EmployeeCode);
                    itemBody = itemBody.Replace("[MonthlyWorkingDay]", item.MonthlyWorkingDay.HasValue ? item.MonthlyWorkingDay.Value.ToString() : "");
                    itemBody = itemBody.Replace("[ActualWorkingDay]", item.ActualWorkingDay.HasValue ? item.ActualWorkingDay.Value.ToString() : "");
                    itemBody = itemBody.Replace("[UnPaidLeaveDay]", item.UnPaidLeaveDay.HasValue ? item.UnPaidLeaveDay.Value.ToString() : "");
                    itemBody = itemBody.Replace("[Basedsalary]", item.BasedSalary.HasValue ? item.BasedSalary.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[SocialInsuranceSalary]", item.SocialInsuranceSalary.HasValue ? item.SocialInsuranceSalary.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[ActualOfSalary]", item.ActualOfSalary.HasValue ? item.ActualOfSalary.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[OvertimeOfSalary]", item.OvertimeOfSalary.HasValue ? item.OvertimeOfSalary.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[FuelAllowance]", item.FuelAllowance.HasValue ? item.FuelAllowance.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[LunchAllowance]", item.LunchAllowance.HasValue ? item.LunchAllowance.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[PhoneAllowance]", item.PhoneAllowance.HasValue ? item.PhoneAllowance.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[SocialInsuranceCompanyPaid]", item.SocialInsuranceCompanyPaid.HasValue ? item.SocialInsuranceCompanyPaid.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[OtherAllowance]", item.OtherAllowance.HasValue ? item.OtherAllowance.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[EnrollmentSalary]", item.EnrollmentSalary.HasValue ? item.EnrollmentSalary.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[RetentionSalary]", item.RetentionSalary.HasValue ? item.RetentionSalary.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[TotalIncome]", item.TotalIncome.HasValue ? item.TotalIncome.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[AdditionalAmount]", item.AdditionalAmount.HasValue ? item.AdditionalAmount.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[TotalInsuranceEmployeePaid]", item.TotalInsuranceEmployeePaid.HasValue ? item.TotalInsuranceEmployeePaid.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[ReductionAmount]", item.ReductionAmount.HasValue ? item.ReductionAmount.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[SocialInsuranceEmployeePaid]", item.SocialInsuranceEmployeePaid.HasValue ? item.SocialInsuranceEmployeePaid.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[DesciplineAmount]", item.DesciplineAmount.HasValue ? item.DesciplineAmount.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[ActualPaid]", item.ActualPaid.HasValue ? item.ActualPaid.Value.ToString("#,#.") : "");
                    var empInsurance = listEmployeeInsurance.Where(w => w.EmployeeId == item.EmployeeId).FirstOrDefault();
                    if (empInsurance != null)
                    {
                        itemBody = itemBody.Replace("[SocialInsuranceCompanyPaidPer]", empInsurance.SocialInsuranceSupportPercent.HasValue ? empInsurance.SocialInsuranceSupportPercent.Value.ToString() : "");
                        itemBody = itemBody.Replace("[SocialInsuranceEmployeePaidPer]", empInsurance.SocialInsurancePercent.HasValue ? empInsurance.SocialInsurancePercent.Value.ToString() : "");
                    }

                    //set imageLogo for email
                    string pathLogo = hostingEnvironment.WebRootPath + @"\logo.png";
                    LinkedResource Img = new LinkedResource(pathLogo, MediaTypeNames.Image.Jpeg);
                    Img.ContentId = "MyImage";
                    string img = "cid:MyImage";
                    itemBody = itemBody.Replace("[pathlogo]", img);
                    AlternateView AV =
                    AlternateView.CreateAlternateViewFromString(itemBody, null, MediaTypeNames.Text.Html);
                    AV.LinkedResources.Add(Img);
                    mail.AlternateViews.Add(AV);

                    mail.IsBodyHtml = true;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                    SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                    SmtpServer.Send(mail);
                }
            });

            return new SendEmailEmployeePayslipResult()
            {
                Status = true
            };
        }

        public SendEmailTeacherPayslipResult SendEmailTeacherPayslip(SendEmailTeacherPayslipParameter parameter)
        {
            GetConfiguration();
            string webRootPath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
            var file = Path.Combine(webRootPath, "SendEmailTeacherPayslip.html");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }

            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);

            var listContact = context.Contact.Where(w => w.ObjectType == "EMP").Select(s => new { s.ObjectId, s.Email }).ToList();
            var listEmployeeMonthySalary = (from item in context.EmployeeMonthySalary
                                            where parameter.lstEmpMonthySalary.Contains(item.EmployeeMonthySalaryId)
                                            select new EmployeeMonthySalaryEntityModel(item)).ToList();

            var lstMonthAndYear = listEmployeeMonthySalary.Select(s => new { month = s.Month, year = s.Year }).ToList().Distinct();
            //Lấy ra danh sách các trung tâm có thể tồn tại trong tháng đó chi cho giảng viên
            var lstCenter = context.EmployeeTimesheet.Where(w => lstMonthAndYear.Select(s => s.month).Contains(w.Month.Value) && lstMonthAndYear.Select(s => s.year).Contains(w.Year.Value) && !string.IsNullOrEmpty(w.Center))
                .Select(s => new { center = s.Center }).ToList().Distinct();

            List<dynamic> lstResult = new List<dynamic>();

            listEmployeeMonthySalary.ForEach(empMonthySalary =>
            {

                var lstTimeSheetTeacher = context.EmployeeTimesheet.Where(w => w.Month == empMonthySalary.Month && w.Year == empMonthySalary.Year && w.EmployeeId == empMonthySalary.EmployeeId).ToList();
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Email, "N8");
                var email = listContact.Where(w => w.ObjectId == empMonthySalary.EmployeeId).Select(s => s.Email).FirstOrDefault();
                if (email != null)
                {
                    decimal SumGioDay = 0;
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    var empInf = empMonthySalary;
                    if (empInf != null)
                    {
                        //email "minhpv@tringhiatech.vn"
                        mail.To.Add(email);
                        mail.Subject = string.Format("Phiếu lương tháng {0} năm {1}", empMonthySalary.Month, empMonthySalary.Year);
                        var itemBody = body;

                        itemBody = itemBody.Replace("[Employeename]", empInf.EmployeeName.ToString());
                        itemBody = itemBody.Replace("[EmployeeCode]", empInf.EmployeeCode.ToString());
                        itemBody = itemBody.Replace("[PostionName]", empInf.PostionName);
                        itemBody = itemBody.Replace("[Basedsalary]", empInf.BasedSalary.HasValue ? empInf.BasedSalary.Value.ToString("#,#.") : "");
                        itemBody = itemBody.Replace("[Month]", empInf.Month.ToString());
                        itemBody = itemBody.Replace("[Year]", empInf.Year.ToString());
                        string row = string.Empty;
                        int STT = 1;
                        lstCenter.ToList().ForEach(itemC =>
                        {
                            var GIODAY = lstTimeSheetTeacher.Where(w => w.Center == itemC.center).Select(s => new { GIODAY = (decimal?)s.ActualWorkingDay }).DefaultIfEmpty().FirstOrDefault();
                            SumGioDay = SumGioDay + (GIODAY != null ? GIODAY.GIODAY.Value : 0);
                            row = row + ReturnRowcenter(STT, itemC.center, (GIODAY != null ? GIODAY.GIODAY.Value : 0), empInf.BasedSalary.HasValue ? empInf.BasedSalary.Value : 0);
                            STT = STT + 1;
                        });
                        itemBody = itemBody.Replace("[rowcenter]", row);
                        var TotalIncome = SumGioDay * (empInf.BasedSalary.HasValue ? empInf.BasedSalary.Value : 0);
                        itemBody = itemBody.Replace("[ActualWorkingDay]", SumGioDay.ToString("#,#."));
                        itemBody = itemBody.Replace("[TotalIncome]", TotalIncome.ToString("#,#."));
                        itemBody = itemBody.Replace("[ReductionAmount]", empInf.ReductionAmount.HasValue ? empInf.ReductionAmount.Value.ToString("#,#.") : "");
                        itemBody = itemBody.Replace("[AdditionalAmount]", empInf.AdditionalAmount.HasValue ? empInf.AdditionalAmount.Value.ToString("#,#.") : "");
                        itemBody = itemBody.Replace("[ActualPaid]", empInf.ActualPaid.HasValue ? empInf.ActualPaid.Value.ToString("#,#.") : "");

                        //set imageLogo for email
                        string pathLogo = hostingEnvironment.WebRootPath + @"\logo.png";
                        LinkedResource Img = new LinkedResource(pathLogo, MediaTypeNames.Image.Jpeg);
                        Img.ContentId = "MyImage";
                        string img = "cid:MyImage";
                        itemBody = itemBody.Replace("[pathlogo]", img);
                        AlternateView AV =
                        AlternateView.CreateAlternateViewFromString(itemBody, null, MediaTypeNames.Text.Html);
                        AV.LinkedResources.Add(Img);
                        mail.AlternateViews.Add(AV);

                        mail.IsBodyHtml = true;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                        SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                        SmtpServer.Send(mail);
                    }
                }
            });

            return new SendEmailTeacherPayslipResult()
            {
                Message = "Success",
                Status = true
            };

        }

        public SendEmailAssistantPayslipResult SendEmailAssistantPayslip(SendEmailAssistantPayslipParameter parameter)
        {
            GetConfiguration();
            string webRootPath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
            var file = Path.Combine(webRootPath, "SendEmailAssistantPayslip.html");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }

            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);

            var listEmployeeInsurance = context.EmployeeInsurance.GroupBy(g => g.EmployeeId).SelectMany(s => s.OrderByDescending(g => g.EffectiveDate).Take(1)).ToList();
            var lstEmployeeMonthySalary = context.EmployeeMonthySalary.Where(w => parameter.lstEmpMonthySalary.Contains(w.EmployeeMonthySalaryId)).ToList();
            var listContact = context.Contact.Where(w => w.ObjectType == "EMP").Select(s => new { s.ObjectId, s.Email }).ToList();
            lstEmployeeMonthySalary.ForEach(item =>
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Email, "N8");
                var email = listContact.Where(w => w.ObjectId == item.EmployeeId).Select(s => s.Email).FirstOrDefault();
                if (email != null)
                {
                    //email emai
                    mail.To.Add(email);
                    mail.Subject = string.Format("Phiếu lương tháng {0} năm {1}", item.Month, item.Year);
                    var itemBody = body;

                    itemBody = itemBody.Replace("[Month]", item.Month.ToString());
                    itemBody = itemBody.Replace("[Year]", item.Year.ToString());
                    itemBody = itemBody.Replace("[Employeename]", item.EmployeeName);
                    itemBody = itemBody.Replace("[PostionName]", item.PostionName);
                    itemBody = itemBody.Replace("[EmployeeCode]", item.EmployeeCode);
                    itemBody = itemBody.Replace("[ActualWorkingDay]", item.ActualWorkingDay.HasValue ? item.ActualWorkingDay.Value.ToString() : "");
                    itemBody = itemBody.Replace("[Basedsalary]", item.BasedSalary.HasValue ? item.BasedSalary.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[AdditionalAmount]", item.AdditionalAmount.HasValue ? item.AdditionalAmount.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[ReductionAmount]", item.ReductionAmount.HasValue ? item.ReductionAmount.Value.ToString("#,#.") : "");
                    itemBody = itemBody.Replace("[ActualPaid]", item.ActualPaid.HasValue ? item.ActualPaid.Value.ToString("#,#.") : "");
                    mail.Body = itemBody;

                    //set imageLogo for email
                    string pathLogo = hostingEnvironment.WebRootPath + @"\logo.png";
                    LinkedResource Img = new LinkedResource(pathLogo, MediaTypeNames.Image.Jpeg);
                    Img.ContentId = "MyImage";
                    string img = "cid:MyImage";
                    itemBody = itemBody.Replace("[pathlogo]", img);
                    AlternateView AV =
                    AlternateView.CreateAlternateViewFromString(itemBody, null, MediaTypeNames.Text.Html);
                    AV.LinkedResources.Add(Img);
                    mail.AlternateViews.Add(AV);



                    mail.IsBodyHtml = true;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                    SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                    SmtpServer.Send(mail);
                }
            });

            return new SendEmailAssistantPayslipResult()
            {
                Status = true
            };

        }
        public SendEmailVendorOrderResult SendEmailVendorOrder(SendEmailVendorOrderParameter parameter)
        {
            GetConfiguration();
            string webrootpath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
            var file = Path.Combine(webrootpath, "IspeakingOrderTemplatePDF.html");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }

            SmtpClient smtpserver = new SmtpClient(PrimaryDomain, PrimaryPort);

            var vendoroder = (from v in context.VendorOrder
                              where v.VendorOrderId == parameter.VendorOrderId
                              select v).FirstOrDefault();
            var contact = (from c in context.Contact
                           where c.ContactId == vendoroder.VendorContactId
                           select c).FirstOrDefault();

            var vendor = (from v in context.Vendor
                          where v.VendorId == vendoroder.VendorId
                          select v).FirstOrDefault();
            var listvendoroderdetail = (from d in context.VendorOrderDetail
                                        join v in context.VendorOrder on d.VendorOrderId equals v.VendorOrderId
                                        join p in context.Product on d.ProductId equals p.ProductId
                                        join c in context.Category on p.ProductUnitId equals c.CategoryId
                                        where d.VendorOrderId == vendoroder.VendorOrderId
                                        select new
                                        {
                                            ProductName = p.ProductName,
                                            ProductUnitName = c.CategoryName,
                                            ProductAmount = d.Quantity,
                                            ProductUnitPrice = d.UnitPrice,
                                            IntoMoney = d.Quantity * d.UnitPrice
                                        }).ToList();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(Email, "N8");
            var email = contact.Email;
            var itembody = body;
            if (email != null)
            {
                var index = 1;

                mail.To.Add(email);
                mail.Subject = string.Format("ispeaking đặt hàng_ đơn số : {0}", vendoroder.VendorOrderCode);


                itembody = itembody.Replace("[vendor]", vendor.VendorName);
                itembody = itembody.Replace("[ordercode]", vendoroder.VendorOrderCode);
                itembody = itembody.Replace("[receiver]", vendoroder.RecipientName);
                itembody = itembody.Replace("[phoneNumber]", vendoroder.RecipientPhone);
                itembody = itembody.Replace("[Address]", vendoroder.PlaceOfDelivery);
                string[] receivedDate = vendoroder.ReceivedDate.ToString().Split(" ");
                itembody = itembody.Replace("[receiveDate]", receivedDate[0] + "-" + vendoroder.ReceivedHour);
                itembody = itembody.Replace("[Ammount]", String.Format("{0:0,0.00}", vendoroder.Amount));
                listvendoroderdetail.ForEach(item =>
                {
                    if (index == listvendoroderdetail.Count())
                    {
                        string row = string.Format("<tr>"
                                                + "<td style = \"text-align: center\"> {0}" + "</td>"
                                                + "<td style = \"text-align: center\"> {1}" + "</td>"
                                                + "<td style = \"text-align: center\"> {2}" + "</td>"
                                                + "<td style = \"text-align: center\"> {3}" + "</td>"
                                                + "<td style = \"text-align: center\"> {4}" + "</td>"
                                                + "<td style = \"text-align: center\"> {5}" + "</td>"
                                            + "</tr></br>", index, item.ProductName, item.ProductUnitName, String.Format("{0:0,0.00}", item.ProductAmount)
                                            , String.Format("{0:0,0.00}", item.ProductUnitPrice), String.Format("{0:0,0.00}", item.IntoMoney));
                        itembody = itembody.Replace("[TableContent]", row);
                    }
                    else
                    {
                        string row = string.Format("<tr>"
                                                + "<td style = \"text-align: center\"> {0}" + "</td>"
                                                + "<td style = \"text-align: center\"> {1}" + "</td>"
                                                + "<td style = \"text-align: center\"> {2}" + "</td>"
                                                + "<td style = \"text-align: center\"> {3}" + "</td>"
                                                + "<td style = \"text-align: center\"> {4}" + "</td>"
                                                + "<td style = \"text-align: center\"> {5}" + "</td>"
                                            + "</tr></br>[TableContent]", index, item.ProductName, item.ProductUnitName, String.Format("{0:0,0.00}", item.ProductAmount)
                                            , String.Format("{0:0,0.00}", item.ProductUnitPrice), String.Format("{0:0,0.00}", item.IntoMoney));
                        itembody = itembody.Replace("[TableContent]", row);
                    }
                    index++;
                });

                //set imageLogo for email
                string pathLogo = hostingEnvironment.WebRootPath + @"\logo.png";
                LinkedResource Img = new LinkedResource(pathLogo, MediaTypeNames.Image.Jpeg);
                Img.ContentId = "MyImage";
                string img = "cid:MyImage";
                itembody = itembody.Replace("[pathlogo]", img);
                AlternateView AV =
                AlternateView.CreateAlternateViewFromString(itembody, null, MediaTypeNames.Text.Html);
                AV.LinkedResources.Add(Img);
                mail.AlternateViews.Add(AV);




            }


            mail.IsBodyHtml = true;
            smtpserver.Credentials = new System.Net.NetworkCredential(Email, Password);
            smtpserver.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
            smtpserver.Send(mail);

            return new SendEmailVendorOrderResult()
            {
                Status = true,
                Message = "Đã gửi email cho nhà cung cấp"
            };

        }

        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity)
        {
            var result = emailContent;

            #region Common Token

            const string Logo = "[LOGO]";
            const string UserName = "[USER_NAME]";
            const string EmployeeName = "[EMP_NAME]";
            const string Url = "[ACCESS_SYSTEM]";

            #endregion

            var _model = model as User;

            if (result.Contains(Logo))
            {
                var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                if (!String.IsNullOrEmpty(logo))
                {
                    var temp_logo = "<img src=\"" + logo +
                                    "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
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

            if (result.Contains(Url))
            {
                var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                var loginLink = Domain + "/forgot-pass/change/" + _model.ResetCode;

                if (!String.IsNullOrEmpty(loginLink))
                {
                    result = result.Replace(Url, loginLink);
                }
            }

            return result;
        }

        public SendEmailForgotPassResult SendEmailForgotPass(SendEmailForgotPassParameter parameter)
        {
            var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
            
            user.ResetCode = Guid.NewGuid().ToString().ToUpper();
            user.ResetCodeDate = DateTime.Now;
            user.UpdatedById = parameter.UserId;
            user.UpdatedDate = DateTime.Now;
            context.User.Update(user);
            context.SaveChanges();

            #region Get Employee Infor to send email

            var configEntity = context.SystemParameter.ToList();

            var emailTempCategoryTypeId =
                context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE").CategoryTypeId;
            var listEmailTempType =
                context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

            var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "QMK")
                .CategoryId;

            var emailTemplate =
                context.EmailTemplate.FirstOrDefault(w => w.Active && w.EmailTemplateTypeId == emailCategoryId);

            var subject = ReplaceTokenForContent(context, user, emailTemplate.EmailTemplateTitle,
                configEntity);
            var content = ReplaceTokenForContent(context, user, emailTemplate.EmailTemplateContent,
                configEntity);

            Emailer.SendEmail(context, new List<string> {parameter.EmailAddress}, new List<string>(), subject, content);

            #endregion

            return new SendEmailForgotPassResult
            {
                Status = true
            };
        }
        private string ReturnRowcenter(int stt, string NameCenter, decimal GIODAY, decimal baseSalary)
        {
            decimal totalMoney = baseSalary * GIODAY;
            string result = string.Format("<tr><td class='auto-style3'>{0}</td><td colspan='2' class='auto-style3' style='text-align:left;'>{1}</td><td class='auto-style3'>{2}</td><td class='auto-style3'>{3}</td></tr>",
                stt, NameCenter, GIODAY, totalMoney.ToString("#,#."));
            return result;
        }

        public SendEmailCustomerOrderResult SendEmailCustomerOrder(SendEmailCustomerOrderParameter parameter)
        {
            try
            {
                var sendEmailParam = context.SystemParameter.FirstOrDefault(e => e.SystemKey == "SendEmailCreateOrder");

                if (sendEmailParam == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = ""
                    };
                }
                var isSendEmail = sendEmailParam.SystemValue;
                if (isSendEmail == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = ""
                    };
                }
                else if (isSendEmail == false)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = ""
                    };
                }

                GetConfiguration();
                string webrootpath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
                var file = Path.Combine(webrootpath, "SendEmailCustomerOrder.html");
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    body = reader.ReadToEnd();
                }
                var itembody = body;

                SmtpClient smtpserver = new SmtpClient(PrimaryDomain, PrimaryPort);

                var companyConfig = context.CompanyConfiguration.FirstOrDefault();
                if (companyConfig == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Không tồn tại thông tin công ty trong hệ thống"
                    };
                }
                var companyName = companyConfig.CompanyName != null ? companyConfig.CompanyName.Trim() : "";
                var companyPhone = companyConfig.Phone != null ? companyConfig.Phone.Trim() : "";
                var companyEmail = companyConfig.Email != null ? companyConfig.Email.Trim() : "";
                var companyAddress = companyConfig.CompanyAddress != null ? companyConfig.CompanyAddress.Trim() : "";

                var customerOrder = context.CustomerOrder.FirstOrDefault(order => order.OrderId == parameter.OrderId);
                if (customerOrder == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Không tồn tại đơn hàng trên hệ thống"
                    };
                }
                var orderCode = customerOrder.OrderCode != null ? customerOrder.OrderCode.Trim() : "";
                var orderCreatedDate = customerOrder.CreatedDate != null ? customerOrder.CreatedDate.ToString("dd/MM/yyyy") : "";
                var orderStatus = context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == customerOrder.StatusId);
                var orderStatusName = orderStatus != null ? orderStatus.Description : "";

                var contactCustomer = context.Contact.FirstOrDefault(cont => cont.ObjectId == customerOrder.CustomerId && cont.ObjectType == ObjectType.CUSTOMER);
                if (contactCustomer == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Không tồn tại liên hệ của khách hàng trên hệ thống"
                    };
                }
                if (contactCustomer.Email == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Khách hàng này chưa có email"
                    };
                }
                //Cần thêm email của người liên hệ

                var customerEmail = contactCustomer.Email.Trim();
                var customerPhone = contactCustomer.Phone != null ? contactCustomer.Phone.Trim() : "";

                //Thông tin người nhận
                var receiverName = customerOrder.RecipientName != null ? customerOrder.RecipientName.Trim() : "";
                var receiverAddress = customerOrder.PlaceOfDelivery != null ? customerOrder.PlaceOfDelivery.Trim() : "";
                var receiverTime = (customerOrder.ReceivedDate != null ? customerOrder.ReceivedDate.Value.ToString("dd/MM/yyyy") : "") + " " + (customerOrder.ReceivedHour != null ? customerOrder.ReceivedHour.ToString() : "");
                var receiverPhone = customerOrder.RecipientPhone != null ? customerOrder.RecipientPhone.Trim() : "";

                var customer = context.Customer.FirstOrDefault(cus => cus.CustomerId == customerOrder.CustomerId);
                if (customer == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Không tồn tại khách hàng trên hệ thống"
                    };
                }
                var customerName = customer.CustomerName != null ? customer.CustomerName.Trim() : "";

                var customerOrderDetail = context.CustomerOrderDetail.Where(orderDetail => orderDetail.OrderId == customerOrder.OrderId).ToList();
                if (customerOrderDetail.Count <= 0)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Đơn hàng không có sản phẩm nào"
                    };
                }

                var listProduct = context.Product.ToList();
                var categoryTypeProductUnit = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH");
                if (categoryTypeProductUnit == null)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Đơn vị tính của sản phẩm không tồn tại trên hệ thống"
                    };
                }
                var listProductUnit = context.Category.Where(x => x.CategoryTypeId == categoryTypeProductUnit.CategoryTypeId).ToList();
                if (listProductUnit.Count <= 0)
                {
                    return new SendEmailCustomerOrderResult
                    {
                        Status = true,
                        Message = "Đơn vị tính của sản phẩm không tồn tại trên hệ thống"
                    };
                }

                string row = "";

                customerOrderDetail.ForEach(item =>
                {
                    var currentProduct = listProduct.FirstOrDefault(x => x.ProductId == item.ProductId);
                    var currentProductName = currentProduct != null ? currentProduct.ProductName.Trim() : "";

                    var currentProductUnit = listProductUnit.FirstOrDefault(x => x.CategoryId == currentProduct.ProductUnitId);
                    var currentProductUnitName = currentProductUnit != null ? currentProductUnit.CategoryName.Trim() : "";

                    var currentUnitMoney = (item.UnitPrice * item.ExchangeRate).Value.ToString("#,#.");
                    var currentQuantity = item.Quantity;
                    var currentVAT = item.Vat != null ? Math.Round(item.Vat.Value, 2) : 0;
                    var currentDiscountType = item.DiscountType;
                    var currentDiscount = item.DiscountValue != null ? item.DiscountValue : 0;
                    if (currentDiscountType != true)
                    {
                        currentDiscount = Math.Round((currentDiscount.Value / (item.Quantity.Value * item.UnitPrice.Value * item.ExchangeRate.Value)) * 100, 0);
                    }
                    var currentTotalMoney = (item.UnitPrice * item.ExchangeRate * item.Quantity) +
                        ((currentVAT * (item.UnitPrice * item.ExchangeRate * item.Quantity)) / 100) -
                        (currentDiscountType == true ?
                            ((currentDiscount * (item.UnitPrice * item.ExchangeRate * item.Quantity)) / 100) :
                            (currentDiscount));
                    row += string.Format("<tr>"
                                                + "<td style = \"border: solid 1px black;padding: 15px;\"> {0}" + "</td>"
                                                + "<td style = \"border: solid 1px black;padding: 15px;\"> {1}" + "</td>"
                                                + "<td style = \"border: solid 1px black;padding: 15px;\"> {2}" + "</td>"
                                                + "<td style = \"border: solid 1px black;padding: 15px;\"> {3}" + "</td>"
                                                + "<td style = \"border: solid 1px black;padding: 15px;\"> {4}" + "</td>"
                                                + "<td style = \"border: solid 1px black;padding: 15px;\"> {5}" + "</td>"
                                                + "<td style = \"border: solid 1px black;padding: 15px;\"> {6}" + "</td>"
                                            + "</tr></br>", currentProductName, currentProductUnitName, currentUnitMoney, currentQuantity.Value.ToString("#,#.")
                                            , currentVAT, currentDiscount, currentTotalMoney.Value.ToString("#,#."));
                });

                itembody = itembody.Replace("[TableContent]", row);
                itembody = itembody.Replace("[CompanyName]", companyName);
                itembody = itembody.Replace("[CustomerName]", customerName);
                itembody = itembody.Replace("[CompanyPhone]", companyPhone);
                itembody = itembody.Replace("[OrderCode]", orderCode);
                itembody = itembody.Replace("[OrderCreatedDate]", orderCreatedDate);
                itembody = itembody.Replace("[OrderStatusName]", orderStatusName);
                itembody = itembody.Replace("[ReceiverName]", receiverName);
                itembody = itembody.Replace("[CustomerEmail]", customerEmail);
                itembody = itembody.Replace("[ReceiverAddress]", receiverAddress);
                itembody = itembody.Replace("[CustomerPhone]", customerPhone);
                itembody = itembody.Replace("[ReceiverTime]", receiverTime);
                itembody = itembody.Replace("[ReceiverPhone]", receiverPhone);

                var totalDiscountType = customerOrder.DiscountType;
                var totalDiscountValue = customerOrder.DiscountValue;
                if (totalDiscountType == true)
                {
                    totalDiscountValue = Math.Round(((customerOrder.Amount * totalDiscountValue.Value) / 100), 0);
                }
                itembody = itembody.Replace("[TotalDiscount]", (totalDiscountValue.Value != 0 ? totalDiscountValue.Value.ToString("#,#.") : "0"));
                var totalPrice = customerOrder.Amount - totalDiscountValue;
                itembody = itembody.Replace("[TotalPrice]", totalPrice.Value.ToString("#,#."));
                //itembody = itembody.Replace("[MoneyText]", "");
                itembody = itembody.Replace("[CompanyEmail]", companyEmail);
                itembody = itembody.Replace("[CompanyAddress]", companyAddress);

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);

                mail.From = new MailAddress(Email, "");
                mail.To.Add(customerEmail);
                var company = Company != null ? ("[" + Company.Trim() + "] - ") : "";
                mail.Subject = company + "Xác nhận đơn hàng " + orderCode + " - " + orderStatusName;
                mail.Body = itembody;
                mail.IsBodyHtml = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                SmtpServer.Send(mail);

                return new SendEmailCustomerOrderResult
                {
                    Status = true,
                    Message = "Gửi email thành công"
                };
            }
            catch (Exception)
            {
                return new SendEmailCustomerOrderResult
                {
                    Status = false,
                    Message = "Đã có lỗi xảy ra"
                };
            }
        }

        public SendEmailPersonCreateResult SendEmailPersonCreate(SendEmailPersonCreateParameter parameter)
        {
            GetConfiguration();
            string webRootPath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
            var file = Path.Combine(webRootPath, "SendEmailPersonCreate.html");
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(file))
            {
                body = reader.ReadToEnd();
            }
            string emailPerson = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.CreateId && c.ObjectType == "EMP").Email;

            //Thay doi cac thuoc tinh can thiet trong htmltemplate
            body = body.Replace("[FullNameCreate]", parameter.FullNameCreate);
            body = body.Replace("[RequestId]", parameter.RequestId);
            body = body.Replace("[FullNameRequest]", parameter.FullNameRequest);
            body = body.Replace("[ActiveRequest]", parameter.ActiveRequest);
            body = body.Replace("[AccountApprove]", parameter.AccountApprove);
            body = body.Replace("[FullNameApprove]", parameter.FullNameApprove);
            body = body.Replace("[DateCreate]", parameter.DateCreate);
            body = body.Replace("[RequestType]", parameter.RequestType);
            body = body.Replace("[DateStart]", parameter.DateStart);
            body = body.Replace("[CaStart]", parameter.CaStart);
            body = body.Replace("[DateEnd]", parameter.DateEnd);
            body = body.Replace("[CaEnd]", parameter.CaEnd);
            body = body.Replace("[Note]", parameter.Note);
            body = body.Replace("[ListFullNameNotify]", parameter.ListFullNameNotify);
            body = body.Replace("{forgotUrl}", WEB_ENDPOINT + "/employee/request/detail?requestId=" + parameter.RequestEmployeeId);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
            mail.From = new MailAddress(Email, "N8");
            mail.To.Add(emailPerson);
            mail.Subject = string.Format("[TNM] – {0} đề xuất xin nghỉ [{1} | {2}]", parameter.ActiveRequest, parameter.RequestId, parameter.FullNameRequest);
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
            SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
            SmtpServer.Send(mail);

            return new SendEmailPersonCreateResult
            {
                Status = true
            };
        }

        public SendEmailPersonApproveResult SendEmailPersonApprove(SendEmailPersonApproveParameter parameter)
        {
            var employeeRequest = context.EmployeeRequest.FirstOrDefault(er => er.EmployeeRequestCode == parameter.RequestId);
            List<Guid?> listIdApprove = new List<Guid?>();
            if (employeeRequest.StepNumber == null)
            {
                listIdApprove.Add(parameter.ApproveId);
            }
            else
            {
                var employeeRequestOffer = context.Employee.FirstOrDefault(ef => ef.EmployeeId == employeeRequest.OfferEmployeeId);
                var workFolow = context.WorkFlows.FirstOrDefault(w => w.WorkflowCode == "QTPDDXXN");
                var workFlowStep = context.WorkFlowSteps.FirstOrDefault(ws => ws.WorkflowId == workFolow.WorkFlowId && ws.StepNumber == employeeRequest.StepNumber);

                if (workFlowStep.ApprovebyPosition)
                {
                    var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == employeeRequestOffer.OrganizationId);
                    var employeePermission = context.Employee.Where(em => em.PositionId == workFlowStep.ApproverPositionId && em.OrganizationId == employeeRequestOffer.OrganizationId).ToList();
                    if (employeePermission == null && organization.Level > 0)
                    {
                        var parrenId = organization.ParentId;
                        for (int i = organization.Level; i > 0; i--)
                        {
                            employeePermission = context.Employee.Where(em => em.PositionId == workFlowStep.ApproverPositionId && em.OrganizationId == parrenId).ToList();
                            if (employeePermission == null)
                            {
                                parrenId = context.Organization.FirstOrDefault(o => o.OrganizationId == parrenId).ParentId;
                            }
                            else
                            {
                                i = -1;
                            }
                        }
                    }
                    employeePermission.ForEach(item =>
                    {
                        listIdApprove.Add(item.EmployeeId);
                    });
                }
                else
                {
                    listIdApprove.Add(workFlowStep.ApproverId);
                }
            }
            listIdApprove.ForEach(item =>
            {
                var empApprove = context.Contact.FirstOrDefault(c => c.ObjectId == item && c.ObjectType == "EMP");
                string emailPerson = empApprove.Email;
                if (emailPerson != "")
                {
                    GetConfiguration();
                    string webRootPath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
                    var file = Path.Combine(webRootPath, "SendEmailPersonApprove.html");
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(file))
                    {
                        body = reader.ReadToEnd();
                    }

                    //Thay doi cac thuoc tinh can thiet trong htmltemplate
                    body = body.Replace("[RequestId]", parameter.RequestId);
                    body = body.Replace("[NameApprove]", empApprove.FirstName + " " + empApprove.LastName);
                    body = body.Replace("[FullNameRequest]", parameter.FullNameRequest);
                    body = body.Replace("[ActiveRequest]", parameter.ActiveRequest);
                    body = body.Replace("[AccountApprove]", parameter.AccountApprove);
                    body = body.Replace("[FullNameApprove]", parameter.FullNameApprove);
                    body = body.Replace("[DateCreate]", parameter.DateCreate);
                    body = body.Replace("[RequestType]", parameter.RequestType);
                    body = body.Replace("[DateStart]", parameter.DateStart);
                    body = body.Replace("[CaStart]", parameter.CaStart);
                    body = body.Replace("[DateEnd]", parameter.DateEnd);
                    body = body.Replace("[CaEnd]", parameter.CaEnd);
                    body = body.Replace("[Note]", parameter.Note);
                    body = body.Replace("[ListFullNameNotify]", parameter.ListFullNameNotify);
                    body = body.Replace("{forgotUrl}", WEB_ENDPOINT + "/employee/request/detail?requestId=" + parameter.RequestEmployeeId);

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                    mail.From = new MailAddress(Email, "N8");
                    mail.To.Add(emailPerson);
                    mail.Subject = string.Format("[TNM] – Tạo đề xuất xin nghỉ [{0} | {1}]", parameter.RequestId, parameter.FullNameRequest);
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                    SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                    SmtpServer.Send(mail);
                }
            });
            return new SendEmailPersonApproveResult
            {
                Status = true
            };
        }

        public SendEmailPersonNotifyResult SendEmailPersonNotify(SendEmailPersonNotifyParameter parameter)
        {
            parameter.NotifyId.ForEach(item =>
            {
                GetConfiguration();
                string webRootPath = hostingEnvironment.WebRootPath + "\\SendEmailTemplate";
                var file = Path.Combine(webRootPath, "SendEmailPersonNotify.html");
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    body = reader.ReadToEnd();
                }

                var notifyPersonEmail = context.Contact.FirstOrDefault(c => c.ObjectType == "EMP" && c.ObjectId == item).Email;
                var notifyPersonName = context.Employee.FirstOrDefault(e => e.EmployeeId == item).EmployeeName;
                //Thay doi cac thuoc tinh can thiet trong htmltemplate
                body = body.Replace("[FullNameNotify]", notifyPersonName);
                body = body.Replace("[AccountCreate]", parameter.AccountCreate);
                body = body.Replace("[FullNameCreate]", parameter.FullNameCreate);
                body = body.Replace("[RequestId]", parameter.RequestId);
                body = body.Replace("[FullNameRequest]", parameter.FullNameRequest);
                body = body.Replace("[AccountApprove]", parameter.AccountApprove);
                body = body.Replace("[FullNameApprove]", parameter.FullNameApprove);
                body = body.Replace("[DateCreate]", parameter.DateCreate);
                body = body.Replace("[RequestType]", parameter.RequestType);
                body = body.Replace("[DateStart]", parameter.DateStart);
                body = body.Replace("[CaStart]", parameter.CaStart);
                body = body.Replace("[DateEnd]", parameter.DateEnd);
                body = body.Replace("[CaEnd]", parameter.CaEnd);
                body = body.Replace("[Note]", parameter.Note);
                body = body.Replace("[ListFullNameNotify]", parameter.ListFullNameNotify);
                body = body.Replace("{forgotUrl}", WEB_ENDPOINT + "/employee/request/detail?requestId=" + parameter.RequestEmployeeId);

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                mail.From = new MailAddress(Email, "N8");
                mail.To.Add(notifyPersonEmail);
                mail.Subject = string.Format("[TNM] – Tạo đề xuất xin nghỉ [{0} | {1}]", parameter.RequestId, parameter.FullNameRequest);
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                SmtpServer.Send(mail);

            });

            return new SendEmailPersonNotifyResult
            {
                Status = true
            };
        }

    }
}

//var parents = new List<Guid?>();
//Guid? currentParent = parameter.OrganizationId;
//while (currentParent != Guid.Empty || currentParent != null)
//{
//    var currentOrg = context.Organization.FirstOrDefault(o => o.OrganizationId == currentParent);

//    if (currentOrg != null)
//        if (currentOrg.ParentId != null)
//            parents.Add(currentOrg.ParentId);

//    if (currentParent == null) break;
//    currentParent = GetParentOrgId(currentParent.Value);
//}