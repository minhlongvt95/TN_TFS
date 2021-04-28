using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Email;
using TN.TNM.DataAccess.Messages.Results.Admin.Email;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmailConfigDAO : BaseDAO, IEmailConfigurationDataAccess
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
        public static string Domain;
        public static string BannerUrl;
        public static bool? Ssl;
        public static string Company;

        public EmailConfigDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
            this.Configuration = iconfiguration;
            this.GetConfiguration();
        }

        public void GetConfiguration()
        {
            var configEntity = context.SystemParameter.ToList();

            PrimaryDomain = configEntity.FirstOrDefault(w => w.SystemKey == "PrimaryDomain")?.SystemValueString;
            SecondayDomain = configEntity.FirstOrDefault(w => w.SystemKey == "SecondayDomain")?.SystemValueString;
            PrimaryPort = int.Parse(configEntity.FirstOrDefault(w => w.SystemKey == "PrimaryPort").SystemValueString);
            SecondaryPort = int.Parse(configEntity.FirstOrDefault(w => w.SystemKey == "SecondaryPort").SystemValueString);
            Email = configEntity.FirstOrDefault(w => w.SystemKey == "Email").SystemValueString;
            Password = configEntity.FirstOrDefault(w => w.SystemKey == "Password").SystemValueString;
            Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
            Ssl = configEntity.FirstOrDefault(x => x.SystemKey == "Ssl").SystemValue;
            Company = Configuration["Company"];
            BannerUrl = Configuration["BannerUrl"];
            WEB_ENDPOINT = Configuration["WEB_ENDPOINT"];
        }

        public CreateUpdateEmailTemplateMasterdataResult CreateUpdateEmailTemplateMasterdata(CreateUpdateEmailTemplateMasterdataParameter parameter)
        {
            try
            {
                Entities.EmailTemplate emailTemplateEntityModel = null;
                List<string> listCCEmail = null;

                string TEMPLATE_CODE = "TMPE"; //Mẫu Email Code
                var templateCodeTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == TEMPLATE_CODE)?
                    .CategoryTypeId;
                var listTemplateType = context.Category.Where(w => w.CategoryTypeId == templateCodeTypeId).ToList();

                string STATUS_CODE = "TTE"; //Trạng thái Email
                var templateStatusTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == STATUS_CODE)?
                    .CategoryTypeId;
                var listTemplateStatus = context.Category.Where(w => w.CategoryTypeId == templateStatusTypeId).ToList();
                
                //lấy tất cả email template token 
                var tokenEntity = context.EmailTemplateToken.ToList();
                var emailTemplateToken = new List<Entities.EmailTemplateToken>();

                if (parameter.EmailTemplateId != null)
                {
                    emailTemplateEntityModel =
                        context.EmailTemplate.FirstOrDefault(f => f.EmailTemplateId == parameter.EmailTemplateId);
                    listCCEmail = context.EmailTemplateCcvalue
                        .Where(w => w.EmailTemplateId == parameter.EmailTemplateId).Select(w => w.Ccto).ToList();
                    //chỉ lấy token theo id email tempalte
                    emailTemplateToken = tokenEntity
                        .Where(w => w.EmailTemplateTypeId == emailTemplateEntityModel.EmailTemplateTypeId).ToList();
                }
                else
                {
                    //Mỗi email template chỉ được tạo 1 lần
                    var existTemplateId = context.EmailTemplate.Where(w => w.Active == true)
                        .Select(w => w.EmailTemplateTypeId).ToList();
                    listTemplateType = listTemplateType.Where(w => !existTemplateId.Contains(w.CategoryId)).ToList();

                    #region Giang comment

                    ////lấy tất cả token ( lọc bỏ trùng nhau )
                    //emailTemplateToken = tokenEntity.GroupBy(p => p.TokenCode)
                    //    .Select(g => g.First())
                    //    .ToList();

                    #endregion
                }

                listTemplateType = listTemplateType.OrderBy(w => w.CategoryName).ToList();
                listTemplateStatus = listTemplateStatus.OrderBy(w => w.CategoryName).ToList();

                return new CreateUpdateEmailTemplateMasterdataResult
                {
                    ListEmailType = listTemplateType.OrderBy(w => w.CategoryName).ToList(),
                    ListEmailStatus = listTemplateStatus.OrderBy(w => w.CategoryName).ToList(),
                    EmailTemplateModel = emailTemplateEntityModel ?? new Entities.EmailTemplate(),
                    ListEmailTemplateToken = emailTemplateToken.OrderBy(w => w.TokenLabel).ToList(),
                    ListEmailToCC = listCCEmail ?? new List<string>(),
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new CreateUpdateEmailTemplateMasterdataResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public CreateUpdateEmailTemplateResult CreateUpdateEmailTemplate(CreateUpdateEmailTemplateParameter parameter)
        {
            try
            {
                if (parameter.EmailTemplateEntityModel.EmailTemplateId == Guid.Empty)
                {
                    //tao moi
                    var emailTemplate = new DataAccess.Databases.Entities.EmailTemplate();
                    emailTemplate.EmailTemplateId = Guid.NewGuid();
                    emailTemplate.EmailTemplateName = parameter.EmailTemplateEntityModel.EmailTemplateName.Trim();
                    emailTemplate.EmailTemplateTitle = parameter.EmailTemplateEntityModel.EmailTemplateTitle.Trim();
                    emailTemplate.EmailTemplateContent = parameter.EmailTemplateEntityModel.EmailTemplateContent.Trim();
                    emailTemplate.EmailTemplateTypeId = parameter.EmailTemplateEntityModel.EmailTemplateTypeId;
                    emailTemplate.EmailTemplateStatusId = parameter.EmailTemplateEntityModel.EmailTemplateStatusId;
                    //default value
                    emailTemplate.Active = true;
                    emailTemplate.CreatedById = parameter.UserId;
                    emailTemplate.CreatedDate = DateTime.Now;
                    emailTemplate.UpdatedById = null;
                    emailTemplate.UpdatedDate = null;

                    context.EmailTemplate.Add(emailTemplate);

                    //add CC Email
                    if (parameter.ListEmailToCC.Count > 0)
                    {
                        var listEmailCC = new List<DataAccess.Databases.Entities.EmailTemplateCcvalue>();
                        parameter.ListEmailToCC.ForEach(emailCC =>
                        {
                            listEmailCC.Add(new Entities.EmailTemplateCcvalue
                            {
                                EmailTemplateCcvalueId = Guid.NewGuid(),
                                EmailTemplateId = emailTemplate.EmailTemplateId,
                                Ccto = emailCC.Trim(),
                                //default value 
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = null,
                                UpdatedDate = null
                            });
                        });
                        context.EmailTemplateCcvalue.AddRange(listEmailCC);
                    }

                }
                else
                {

                    //update
                    var emailTemplate = context.EmailTemplate.FirstOrDefault(w => w.EmailTemplateId == parameter.EmailTemplateEntityModel.EmailTemplateId);
                    emailTemplate.EmailTemplateName = parameter.EmailTemplateEntityModel.EmailTemplateName.Trim();
                    emailTemplate.EmailTemplateTitle = parameter.EmailTemplateEntityModel.EmailTemplateTitle.Trim();
                    emailTemplate.EmailTemplateContent = parameter.EmailTemplateEntityModel.EmailTemplateContent.Trim();
                    emailTemplate.EmailTemplateTypeId = parameter.EmailTemplateEntityModel.EmailTemplateTypeId;
                    emailTemplate.EmailTemplateStatusId = parameter.EmailTemplateEntityModel.EmailTemplateStatusId;
                    //default value
                    emailTemplate.UpdatedById = parameter.UserId;
                    emailTemplate.UpdatedDate = DateTime.Now;

                    //xóa những CC email cũ
                    var listOldCC = context.EmailTemplateCcvalue.Where(w => w.EmailTemplateId == parameter.EmailTemplateEntityModel.EmailTemplateId).ToList();
                    context.EmailTemplateCcvalue.RemoveRange(listOldCC);
                    //add CC Email
                    if (parameter.ListEmailToCC.Count > 0)
                    {
                        var listEmailCC = new List<DataAccess.Databases.Entities.EmailTemplateCcvalue>();
                        parameter.ListEmailToCC.ForEach(emailCC =>
                        {
                            listEmailCC.Add(new Entities.EmailTemplateCcvalue
                            {
                                EmailTemplateCcvalueId = Guid.NewGuid(),
                                EmailTemplateId = emailTemplate.EmailTemplateId,
                                Ccto = emailCC.Trim(),
                                //default value 
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                UpdatedById = null,
                                UpdatedDate = null
                            });
                        });
                        context.EmailTemplateCcvalue.AddRange(listEmailCC);
                    }

                }

                context.SaveChanges();

                return new CreateUpdateEmailTemplateResult
                {
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new CreateUpdateEmailTemplateResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public SearchEmailConfigMasterdataResult SearchEmailConfigMasterdata(SearchEmailConfigMasterdataParameter parameter)
        {
            try
            {
                var EMAIL_TYPE_CODE = "TMPE";
                var emailTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == EMAIL_TYPE_CODE).CategoryTypeId;
                var listEmailType = context.Category.Where(w => w.CategoryTypeId == emailTypeId).ToList();

                var EMAIL_STATUS_CODE = "TTE";
                var statusId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == EMAIL_STATUS_CODE).CategoryTypeId;
                var listEmailStatus = context.Category.Where(w => w.CategoryTypeId == statusId).ToList();

                return new SearchEmailConfigMasterdataResult
                {
                    ListEmailStatus = listEmailStatus,
                    ListEmailType = listEmailType,
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new SearchEmailConfigMasterdataResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public SearchEmailTemplateResult SearchEmailTemplate(SearchEmailTemplateParameter parameter)
        {
            try
            {

                var EMAIL_TYPE_CODE = "TMPE";
                var EMAIL_STATUS_CODE = "TTE";

                var emailTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == EMAIL_TYPE_CODE).CategoryTypeId;
                var emailStatusId = context.CategoryType.First(f => f.CategoryTypeCode == EMAIL_STATUS_CODE).CategoryTypeId;

                var listEmailTypeId = context.Category.Where(w => w.CategoryTypeId == emailTypeId).Select(w => w.CategoryId).ToList();
                var listEmailTypeIdToSearch = listEmailTypeId.Where(w => parameter.ListEmailTemplateTypeId.Contains(w)).ToList();
                //nếu danh sách trạng thái cần gửi  rỗng thì search hết
                if (parameter.ListEmailTemplateTypeId.Count == 0)
                {
                    listEmailTypeIdToSearch = listEmailTypeId;
                }

                var listEmailStatusId = context.Category.Where(w => w.CategoryTypeId == emailStatusId).Select(w => w.CategoryId).ToList();
                var listEmailStatusIdToSearch = listEmailStatusId.Where(w => parameter.ListEmailTemplateStatusId.Contains(w)).ToList();
                //nếu danh sách trạng thái rỗng thì search hết
                if (parameter.ListEmailTemplateStatusId.Count == 0)
                {
                    listEmailStatusIdToSearch = listEmailStatusId;
                }

                var nameToSearch = parameter.EmailTemplateName.Trim().ToLower();
                var titleToSearch = parameter.EmailTemplateTitle.Trim().ToLower();

                var listEmailTemplate = context.EmailTemplate.Where(w => w.Active == true)
                                                             .Where(w => w.EmailTemplateName.ToLower().Contains(nameToSearch))
                                                             .Where(w => w.EmailTemplateTitle.ToLower().Contains(titleToSearch))
                                                             .Where(w => listEmailTypeIdToSearch.Contains(w.EmailTemplateTypeId))
                                                             .Where(w => listEmailStatusIdToSearch.Contains(w.EmailTemplateStatusId))
                                                             .ToList();

                listEmailTemplate = listEmailTemplate.OrderBy(w => w.EmailTemplateName).ToList();
             
                return new SearchEmailTemplateResult
                {
                    ListEmailTemplate = listEmailTemplate ?? new List<Entities.EmailTemplate>(),
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new SearchEmailTemplateResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public SendEmailResult SendEmail(SendEmailParameter parameter)
        {
            try
            {
                var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE")
                    .CategoryTypeId;
                var listEmailTempType =
                    context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();
                
                switch (parameter.SendType)
                {
                    case 1:

                        #region Gửi Email sau khi tạo lead

                        var EMAIL_LEAD_CODE = "CREL"; //code gửi email khi tạo lead
                        var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == EMAIL_LEAD_CODE)
                            .CategoryId;

                        var emailTemplate = context.EmailTemplate
                            .Where(w => w.Active == true && w.EmailTemplateTypeId == emailCategoryId).FirstOrDefault();
                        
                        //kiem tra dieu kien o system parameter
                        var isSendCreateLead = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterCreateLead").SystemValue;
                        if (emailTemplate != null && isSendCreateLead == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token for content
                            emailTemplate.EmailTemplateContent =
                                ReplaceTokenForContent(emailTemplate.EmailTemplateContent,
                                    parameter.SendEmailEntityModel);
                            //replace to header
                            emailTemplate.EmailTemplateTitle = ReplaceTokenForContent(emailTemplate.EmailTemplateTitle,
                                parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView = ContentToAlternateView(emailTemplate.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplate.EmailTemplateId).Select(w => w.Ccto)
                                .ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplate.EmailTemplateTitle;
                            mail.IsBodyHtml = true;
                            
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 2:

                        #region Send Email After Create Quote

                        var EMAIL_CRE_QUOTE_CODE = "ECQ";
                        var emailCreateQuoteCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_CRE_QUOTE_CODE).CategoryId;

                        var emailTemplateCreateQuote = context.EmailTemplate.Where(w => w.Active == true
                                                                                        && w.EmailTemplateTypeId ==
                                                                                        emailCreateQuoteCategoryId)
                            .FirstOrDefault();

                        //kiem tra dieu kien o system parameter
                        var isSendCreateQuote = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterCreateQuote").SystemValue;

                        if (emailTemplateCreateQuote != null && isSendCreateQuote == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token
                            emailTemplateCreateQuote.EmailTemplateContent = ReplaceTokenForContent(
                                emailTemplateCreateQuote.EmailTemplateContent, parameter.SendEmailEntityModel);
                            //replace to header
                            emailTemplateCreateQuote.EmailTemplateTitle =
                                ReplaceTokenForContent(emailTemplateCreateQuote.EmailTemplateTitle,
                                    parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView =
                                ContentToAlternateView(emailTemplateCreateQuote.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplateCreateQuote.EmailTemplateId)
                                .Select(w => w.Ccto).ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplateCreateQuote.EmailTemplateTitle;
                            mail.IsBodyHtml = true;
                            
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 3:

                        #region Send Email After Edit Quote

                        var EMAIL_EDIT_QUOTE_CODE = "EEQ";
                        var emailEditQuoteCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_EDIT_QUOTE_CODE).CategoryId;

                        var emailTemplateEditQuote = context.EmailTemplate.Where(w => w.Active == true
                                                                                      && w.EmailTemplateTypeId ==
                                                                                      emailEditQuoteCategoryId)
                            .FirstOrDefault();

                        //kiem tra dieu kien o system parameter
                        var isSendEditQuote = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterEditQuote").SystemValue;

                        if (emailTemplateEditQuote != null && isSendEditQuote == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token
                            emailTemplateEditQuote.EmailTemplateContent =
                                ReplaceTokenForContent(emailTemplateEditQuote.EmailTemplateContent,
                                    parameter.SendEmailEntityModel);
                            //replace to header
                            emailTemplateEditQuote.EmailTemplateTitle =
                                ReplaceTokenForContent(emailTemplateEditQuote.EmailTemplateTitle,
                                    parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView =
                                ContentToAlternateView(emailTemplateEditQuote.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplateEditQuote.EmailTemplateId)
                                .Select(w => w.Ccto).ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplateEditQuote.EmailTemplateTitle;
                            mail.IsBodyHtml = true;
                            
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 4:

                        #region Gửi Email sau khi edit lead

                        var EMAIL_EDIT_LEAD_CODE = "EREL";
                        var emailEditLeadCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_EDIT_LEAD_CODE).CategoryId;

                        var emailTemplateEditLead = context.EmailTemplate.Where(w => w.Active == true
                                                                                     && w.EmailTemplateTypeId ==
                                                                                     emailEditLeadCategoryId)
                            .FirstOrDefault();
                        //kiem tra dieu kien o system parameter
                        var isSendEditLead = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterEditLead").SystemValue;

                        if (emailTemplateEditLead != null && isSendEditLead == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token
                            emailTemplateEditLead.EmailTemplateContent =
                                ReplaceTokenForContent(emailTemplateEditLead.EmailTemplateContent,
                                    parameter.SendEmailEntityModel);
                            //replace to header
                            emailTemplateEditLead.EmailTemplateTitle =
                                ReplaceTokenForContent(emailTemplateEditLead.EmailTemplateTitle,
                                    parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView =
                                ContentToAlternateView(emailTemplateEditLead.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplateEditLead.EmailTemplateId)
                                .Select(w => w.Ccto).ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplateEditLead.EmailTemplateTitle;
                            mail.IsBodyHtml = true;
                            
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 5:

                        #region Gửi Email sau khi tạo khach hang

                        var EMAIL_CREATE_CUS_CODE = "CREC"; //code gửi email khi tạo lead
                        var emailCreateCusCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_CREATE_CUS_CODE).CategoryId;

                        var emailTemplateCreateCus = context.EmailTemplate.Where(w => w.Active == true
                                                                                      && w.EmailTemplateTypeId ==
                                                                                      emailCreateCusCategoryId)
                            .FirstOrDefault();

                        //kiem tra dieu kien o system parameter
                        var isSendCreatCustomer = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterCreateCustomer").SystemValue;

                        if (emailTemplateCreateCus != null && isSendCreatCustomer == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token
                            emailTemplateCreateCus.EmailTemplateContent =
                                ReplaceTokenForContent(emailTemplateCreateCus.EmailTemplateContent,
                                    parameter.SendEmailEntityModel);
                            //replace token
                            emailTemplateCreateCus.EmailTemplateTitle =
                                ReplaceTokenForContent(emailTemplateCreateCus.EmailTemplateTitle,
                                    parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView =
                                ContentToAlternateView(emailTemplateCreateCus.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplateCreateCus.EmailTemplateId)
                                .Select(w => w.Ccto).ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplateCreateCus.EmailTemplateTitle;
                            mail.IsBodyHtml = true;
                            
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 6:

                        #region Gửi Email sau khi chinh sua khach hang

                        var EMAIL_EDIT_CUS_CODE = "EREC";
                        var emailEditCusCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_EDIT_CUS_CODE).CategoryId;

                        //kiem tra dieu kien o system parameter
                        var isSendEditCustomer = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterCreateCustomer").SystemValue;

                        var emailTemplateEditCus = context.EmailTemplate.Where(w => w.Active == true
                                                                                    && w.EmailTemplateTypeId ==
                                                                                    emailEditCusCategoryId)
                            .FirstOrDefault();
                        if (emailTemplateEditCus != null && isSendEditCustomer == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token
                            emailTemplateEditCus.EmailTemplateContent =
                                ReplaceTokenForContent(emailTemplateEditCus.EmailTemplateContent,
                                    parameter.SendEmailEntityModel);
                            //replace token
                            emailTemplateEditCus.EmailTemplateTitle =
                                ReplaceTokenForContent(emailTemplateEditCus.EmailTemplateTitle,
                                    parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView = ContentToAlternateView(emailTemplateEditCus.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplateEditCus.EmailTemplateId)
                                .Select(w => w.Ccto).ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplateEditCus.EmailTemplateTitle;
                            mail.IsBodyHtml = true;
                            
                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 7:

                        #region Gửi Email sau khi tạo nhan vien

                        var EMAIL_CREATE_USER_CODE = "LGI";
                        var emailCreateUserCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_CREATE_USER_CODE).CategoryId;

                        //kiem tra dieu kien o system parameter
                        var isSendInforAccount = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailInforAccount").SystemValue;

                        var emailTemplateCreateUser = context.EmailTemplate.Where(w => w.Active == true
                                                                                       && w.EmailTemplateTypeId ==
                                                                                       emailCreateUserCategoryId)
                            .FirstOrDefault();

                        if (emailTemplateCreateUser != null && isSendInforAccount == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token
                            emailTemplateCreateUser.EmailTemplateContent = ReplaceTokenForContent(
                                emailTemplateCreateUser.EmailTemplateContent, parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView =
                                ContentToAlternateView(emailTemplateCreateUser.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplateCreateUser.EmailTemplateId)
                                .Select(w => w.Ccto).ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplateCreateUser.EmailTemplateTitle;
                            mail.IsBodyHtml = true;

                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 8:

                        #region Gửi Email sau khi tạo đơn hàng

                        var EMAIL_CREATE_ORDER_CODE = "CORD";
                        var emailCreateOrderCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_CREATE_ORDER_CODE).CategoryId;

                        //kiem tra dieu kien o system parameter
                        var isSendCreateOrder = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterOrder").SystemValue;

                        var emailTemplateCreateOrder = context.EmailTemplate.Where(w => w.Active == true
                                                                                        && w.EmailTemplateTypeId ==
                                                                                        emailCreateOrderCategoryId)
                            .FirstOrDefault();
                        if (emailTemplateCreateOrder != null && isSendCreateOrder == true)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token
                            emailTemplateCreateOrder.EmailTemplateContent = ReplaceTokenForContent(
                                emailTemplateCreateOrder.EmailTemplateContent, parameter.SendEmailEntityModel);
                            //replace token
                            emailTemplateCreateOrder.EmailTemplateTitle =
                                ReplaceTokenForContent(emailTemplateCreateOrder.EmailTemplateTitle,
                                    parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView =
                                ContentToAlternateView(emailTemplateCreateOrder.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);
                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailTemplateCreateOrder.EmailTemplateId)
                                .Select(w => w.Ccto).ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailTemplateCreateOrder.EmailTemplateTitle;
                            mail.IsBodyHtml = true;

                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    case 9:

                        #region Gửi Email sau khi tạo đề xuất xin nghỉ(gửi 2 template email: tạo đề xuất xin nghỉ cho người cần thông báo và cho người phê duyệt
                        
                        //1. gửi cho người cần thông báo
                        var EMAIL_CDX_CODE = "CDX"; //mã email Tạo đề xuất xin nghỉ (dành cho người cần thông báo)
                        var emailCDXCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == EMAIL_CDX_CODE)
                            .CategoryId;

                        var emailTemplateCDX = context.EmailTemplate.Where(w => w.Active == true
                                                                                && w.EmailTemplateTypeId ==
                                                                                emailCDXCategoryId).FirstOrDefault();
                     
                        //kiem tra dieu kien o system parameter
                        var isSendrequestNoti = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterRequestLeaving_Notify").SystemValue;

                        if (emailTemplateCDX != null && isSendrequestNoti == true)
                        {
                            for (var i = 0; i < parameter.SendEmailEntityModel.ListEmployeeSendEmail_1.Count; i++)
                            {
                                try
                                {
                                    MailMessage mail = new MailMessage();
                                    SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                                    mail.From = new MailAddress(Email, Email);
                                    //replace token
                                    var _contentForToken = ReplaceTokenForContent(emailTemplateCDX.EmailTemplateContent,
                                        parameter.SendEmailEntityModel);
                                    emailTemplateCDX.EmailTemplateTitle =
                                        ReplaceTokenForContent(emailTemplateCDX.EmailTemplateTitle,
                                            parameter.SendEmailEntityModel);
                                    //replace tên người nhận email
                                    var _lastContent = ReplaceTokenForReceiverName(_contentForToken,
                                        parameter.SendEmailEntityModel.ListEmployeeName_1[i]);
                                    //convert string to stream
                                    AlternateView alterView = ContentToAlternateView(_lastContent);
                                    mail.AlternateViews.Add(alterView);
                                    mail.To.Add(parameter.SendEmailEntityModel.ListEmployeeSendEmail_1[i]);

                                    var listCCEmail = new List<string>();

                                    var listEmailCCEnitty = context.EmailTemplateCcvalue
                                        .Where(w => w.EmailTemplateId == emailTemplateCDX.EmailTemplateId)
                                        .Select(w => w.Ccto).ToList();
                                    listEmailCCEnitty?.ForEach(ccEmail =>
                                    {
                                        //mail.CC.Add(ccEmail);
                                        listCCEmail.Add(ccEmail);
                                    });
                                    parameter.SendEmailEntityModel.ListEmployeeCCEmail?.ForEach(ccEmail =>
                                    {
                                        // mail.CC.Add(ccEmail);
                                        listCCEmail.Add(ccEmail);
                                    });
                                    if (listCCEmail.Count > 0)
                                    {
                                        mail.CC.Add(string.Join(",", listCCEmail));
                                    }
                                    mail.Subject = emailTemplateCDX.EmailTemplateTitle;
                                    mail.IsBodyHtml = true;
                                    SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                                    SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                                    SmtpServer.Send(mail);
                                }
                                catch (Exception e)
                                {

                                }                 
                            }
                        }

                        //2. gửi cho người cần phê duyệt
                        var EMAIL_CDXP_CODE = "CDXP"; //mã email Tạo đề xuất xin nghỉ (dành cho người phê duyệt)
                        var emailCDXPCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == EMAIL_CDXP_CODE)
                            .CategoryId;

                        var emailTemplateCDXP = context.EmailTemplate.Where(w => w.Active == true
                                                                                 && w.EmailTemplateTypeId ==
                                                                                 emailCDXPCategoryId).FirstOrDefault();

                        //kiem tra dieu kien o system parameter
                        var isSendrequestApprove = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterRequestLeaving_Approve").SystemValue;

                        if (emailTemplateCDXP != null && isSendrequestApprove == true)
                        {
                            for (var i = 0; i < parameter.SendEmailEntityModel.ListEmployeeSendEmail_2.Count; i++)
                            {
                                try
                                {
                                    MailMessage mail = new MailMessage();
                                    SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                                    mail.From = new MailAddress(Email, Email);
                                    //replace token
                                    var _contentForToken = ReplaceTokenForContent(
                                        emailTemplateCDXP.EmailTemplateContent, parameter.SendEmailEntityModel);
                                    emailTemplateCDXP.EmailTemplateTitle =
                                        ReplaceTokenForContent(emailTemplateCDXP.EmailTemplateTitle,
                                            parameter.SendEmailEntityModel);
                                    //replace tên người nhận email
                                    var _lastContent = ReplaceTokenForReceiverName(_contentForToken,
                                        parameter.SendEmailEntityModel.ListEmployeeName_2[i]);
                                    //convert string to stream
                                    AlternateView alterView = ContentToAlternateView(_lastContent);
                                    mail.AlternateViews.Add(alterView);
                                    mail.To.Add(parameter.SendEmailEntityModel.ListEmployeeSendEmail_2[i]);

                                    var listCCEmail = new List<string>();

                                    var listEmailCCEnitty = context.EmailTemplateCcvalue
                                        .Where(w => w.EmailTemplateId == emailTemplateCDX.EmailTemplateId)
                                        .Select(w => w.Ccto).ToList();
                                    listEmailCCEnitty?.ForEach(ccEmail =>
                                    {
                                        //mail.CC.Add(ccEmail);
                                        listCCEmail.Add(ccEmail);
                                    });
                                    parameter.SendEmailEntityModel.ListEmployeeCCEmail?.ForEach(ccEmail =>
                                    {
                                        // mail.CC.Add(ccEmail);
                                        listCCEmail.Add(ccEmail);
                                    });

                                    if (listCCEmail.Count > 0)
                                    {
                                        mail.CC.Add(string.Join(",", listCCEmail));
                                    }
                                    mail.Subject = emailTemplateCDXP.EmailTemplateTitle;
                                    mail.IsBodyHtml = true;
                                    SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                                    SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                                    SmtpServer.Send(mail);
                                }
                                catch
                                {

                                }                             
                            }
                        }

                        #endregion

                        break;
                    case 10:

                        #region Gửi Email sau khi gửi phê duyệt phiếu đề xuất mua hàng

                        var EMAIL_SEND_APPRO_PRO_RQ_CODE = "DXM"; //code gửi email khi tạo lead
                        var emailSendApproProRqCategoryId = listEmailTempType
                            .FirstOrDefault(w => w.CategoryCode == EMAIL_SEND_APPRO_PRO_RQ_CODE)
                            .CategoryId;

                        var emailSendApproProRqTemplate = context.EmailTemplate
                            .FirstOrDefault(w =>
                                w.Active == true && w.EmailTemplateTypeId == emailSendApproProRqCategoryId);

                        //kiem tra dieu kien o system parameter
                        var isSendEmailSendApproProRq = context.SystemParameter
                            .FirstOrDefault(f => f.SystemKey == "SendEmailAfterSendApproProcReq").SystemValue;
                        if (emailSendApproProRqTemplate != null && 
                            isSendEmailSendApproProRq == true && 
                            parameter.SendEmailEntityModel.ListSendToEmail.Count > 0)
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                            mail.From = new MailAddress(Email, Email);
                            //replace token for content
                            emailSendApproProRqTemplate.EmailTemplateContent =
                                ReplaceTokenForContent(emailSendApproProRqTemplate.EmailTemplateContent,
                                    parameter.SendEmailEntityModel);
                            //replace to header
                            emailSendApproProRqTemplate.EmailTemplateTitle = ReplaceTokenForContent(
                                emailSendApproProRqTemplate.EmailTemplateTitle,
                                parameter.SendEmailEntityModel);
                            //convert string to stream
                            AlternateView alterView =
                                ContentToAlternateView(emailSendApproProRqTemplate.EmailTemplateContent);
                            mail.AlternateViews.Add(alterView);

                            parameter.SendEmailEntityModel.ListSendToEmail.ForEach(email =>
                            {
                                mail.To.Add(email);
                            });

                            var listEmailCC = context.EmailTemplateCcvalue
                                .Where(w => w.EmailTemplateId == emailSendApproProRqTemplate.EmailTemplateId)
                                .Select(w => w.Ccto)
                                .ToList();

                            listEmailCC?.ForEach(ccEmail =>
                            {
                                mail.CC.Add(ccEmail);
                            });
                            mail.Subject = emailSendApproProRqTemplate.EmailTemplateTitle;
                            mail.IsBodyHtml = true;

                            SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                            SmtpServer.EnableSsl = Ssl != null ? Ssl.Value : false;
                            SmtpServer.Send(mail);
                        }

                        #endregion

                        break;
                    default:
                        break;
                }

                return new SendEmailResult
                {
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new SendEmailResult
                {
                    Status = false,
                    Message = ex.ToString()
                };
            }
        }

        public GetTokenForEmailTypeIdResult GetTokenForEmailTypeId(GetTokenForEmailTypeIdParameter parameter)
        {
            try
            {
                var listEmailTemplateToken = new List<EmailTemplateToken>();

                listEmailTemplateToken = context.EmailTemplateToken
                    .Where(x => x.EmailTemplateTypeId == parameter.EmailTemplateTypeId).ToList();

                return new GetTokenForEmailTypeIdResult()
                {
                    Status = true,
                    Message = "Success",
                    ListEmailTemplateToken = listEmailTemplateToken
                };
            }
            catch (Exception e)
            {
                return new GetTokenForEmailTypeIdResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        private static AlternateView ContentToAlternateView(string content)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
                string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
                if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(base64))
                {
                    //ignore replacement when match normal <img> tag
                    continue;
                }
                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64), new ContentType(type))
                {
                    ContentId = imgCount.ToString()
                };
                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }

        public static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
        }

        public string ReplaceTokenForContent(string currentContent, DataAccess.Models.Email.SendEmailEntityModel sendEmailModel)
        {
            var _result = currentContent;

            var dd = "[dd]";
            var MM = "[MM]";
            var yyyy = "[yyyy]";

            if (_result.Contains(dd))
            {
                _result = _result.Replace(dd, DateTime.Now.Day.ToString("00"));
            }

            if (_result.Contains(MM))
            {
                _result = _result.Replace(MM, DateTime.Now.Month.ToString("00"));
            }

            if (_result.Contains(yyyy))
            {
                _result = _result.Replace(yyyy, DateTime.Now.Year.ToString("0000"));
            }

            //create lead
            var LeadName = "[LEAD_NAME]";
            var LeadPhone = "[LEAD_PHONE]";
            var LeadEmail = "[LEAD_EMAIL]";
            var LeadAddress = "[LEAD_ADDRESS]";
            var LeadInterested = "[LEAD_INTERESTED]";
            var LeadPotential = "[LEAD_POTENTIAL]";
            var EmployeeCode = "[EMP_CODE]";
            var EmployeeName = "[EMP_NAME]";
            var LeadPicCode = "[PIC_CODE]";
            var LeadPicName = "[PIC_NAME]";
            var CompanyName = "[COMPANY_NAME]";
            var CompanyAddress = "[COMPANY_ADDRESS]";
            var AccessSystem = "[ACCESS_SYSTEM]";

            //create quote
            var QuoteCode = "[QUOTE_CODE]";
            var QuoteStatus = "[QUOTE_STT]";
            var CustomerType = "[CUS_TYPE]";
            var CustomerName = "[CUS_NAME]";
            var CustomerEmail = "[CUS_EMAIL]";
            var CustomerPhone = "[CUS_PHONE]";
            var Seller = "[EMP_SALE]";
            var SendQuoteDate = "[INTENDED_DATE]";
            var EffectiveQuoteDate = "[EFF_DAY]";
            var CreatedEmployeeName = "[EMP_NAME]";
            var CreatedEmployeeCode = "[EMP_CODE]";
            var DetailProduct = "[DETAIL_PROD]";

            //create customer
            var CustomerGroup = "[CUS_GROUP]";
            var CustomerCode = "[CUS_CODE]";
            var CustomerAddress = "[CUS_EMAIL]";
            var CustomerSeller = "[CUS_SELLER]";

            //create user
            var UserName = "[USER_NAME]";
            var UserPassword = "[USER_PASS]";

            //create order
            var OrderCode = "[ORDER_CODE]";
            var ReceivedDateHour = "[RECEI_TIME]";
            var OrderStatus = "[ORDER_STATUS]";
            var RecipientPhone = "[RECEI_PHONE]";
            var PlaceOfDelivery = "[RECEI_PLACE]";
            var OrderDate = "[ORDER_DATE]";
            var RecipientName = "[RECEI_NAME]";
            var DetailOrder = "[DETAIL_ORDER]";
            var CompanyEmail = "[COMPANY_EMAIL]";
            var CompanyPhone = "[COMPANY_PHONE]";

            //gửi email khi tạo đề xuất xin nghỉ
            var EmployeeRequestCode = "[REQUEST_CODE]";
            var OfferEmployeeName = "[OFFER_EMP_NAME]";
            var CreateEmployeeCode = "[CREATE_CUS_CODE]";
            var CreateEmployeeName = "[CREATE_CUS_NAME]";
            var CreatedDate = "[CREATE_DATE]";
            var TypeRequestName = "[REQUEST_NAME]";
            var DurationTime = "[DURATION_TIME]";
            var Detail = "[DETAIL]";
            var ApproverCode = "[APPRO_CODE]";
            var ApproverName = "[APPRO_NAME]";
            var NotifyList = "[NOTIFY]";

            //gửi email khi gửi phê duyệt phiếu đề xuất mua hàng
            var OrganizationName = "[ORG_NAME]";
            var ProcurementCode = "[PROCUR_CODE]";
            var ProcurementDetailLink = "[LINK]";

            #region replace for create lead
            if (_result.Contains(LeadName) && sendEmailModel.LeadName != null)
            {
                _result = _result.Replace(LeadName, sendEmailModel.LeadName);
            }

            if (_result.Contains(LeadPhone) && sendEmailModel.LeadPhone != null)
            {
                _result = _result.Replace(LeadPhone, sendEmailModel.LeadPhone);
            }

            if (_result.Contains(LeadEmail) && sendEmailModel.LeadEmail != null)
            {
                _result = _result.Replace(LeadEmail, sendEmailModel.LeadEmail);
            }

            if (_result.Contains(LeadAddress) && sendEmailModel.LeadAddress != null)
            {
                _result = _result.Replace(LeadAddress, sendEmailModel.LeadAddress);
            }

            if (_result.Contains(LeadInterested) && sendEmailModel.LeadInterested != null)
            {
                _result = _result.Replace(LeadInterested, sendEmailModel.LeadInterested);
            }

            if (_result.Contains(LeadPotential) && sendEmailModel.LeadPotential != null)
            {
                _result = _result.Replace(LeadPotential, sendEmailModel.LeadPotential);
            }

            if (_result.Contains(EmployeeCode) && sendEmailModel.EmployeeCode != null)
            {
                _result = _result.Replace(EmployeeCode, sendEmailModel.EmployeeCode);
            }

            if (_result.Contains(EmployeeName) && sendEmailModel.EmployeeCode != null)
            {
                _result = _result.Replace(EmployeeName, sendEmailModel.EmployeeName);
            }

            if (_result.Contains(LeadPicCode) && sendEmailModel.LeadPicCode != null)
            {
                _result = _result.Replace(LeadPicCode, sendEmailModel.LeadPicCode);
            }

            if (_result.Contains(LeadPicName) && sendEmailModel.LeadPicName != null)
            {
                _result = _result.Replace(LeadPicName, sendEmailModel.LeadPicName);
            }

            if (_result.Contains(CompanyName) && sendEmailModel.CompanyName != null)
            {
                _result = _result.Replace(CompanyName, sendEmailModel.CompanyName);
            }
            if (_result.Contains(CompanyAddress) && sendEmailModel.CompanyAddress != null)
            {
                _result = _result.Replace(CompanyAddress, sendEmailModel.CompanyAddress);
            }
            #endregion

            #region replace for create quote
            if (_result.Contains(QuoteCode) && sendEmailModel.QuoteCode != null)
            {
                _result = _result.Replace(QuoteCode, sendEmailModel.QuoteCode);
            }
            if (_result.Contains(QuoteStatus) && sendEmailModel.QuoteStatus != null)
            {
                _result = _result.Replace(QuoteStatus, sendEmailModel.QuoteStatus);
            }
            if (_result.Contains(CustomerType) && sendEmailModel.CustomerType != null)
            {
                _result = _result.Replace(CustomerType, sendEmailModel.CustomerType);
            }
            if (_result.Contains(CustomerName) && sendEmailModel.CustomerName != null)
            {
                _result = _result.Replace(CustomerName, sendEmailModel.CustomerName);
            }
            if (_result.Contains(CustomerEmail) && sendEmailModel.CustomerEmail != null)
            {
                _result = _result.Replace(CustomerEmail, sendEmailModel.CustomerEmail);
            }
            if (_result.Contains(CustomerPhone) && sendEmailModel.CustomerPhone != null)
            {
                _result = _result.Replace(CustomerPhone, sendEmailModel.CustomerPhone);
            }
            if (_result.Contains(Seller) && sendEmailModel.Seller != null)
            {
                _result = _result.Replace(Seller, sendEmailModel.Seller);
            }
            if (_result.Contains(SendQuoteDate) && sendEmailModel.SendQuoteDate != null)
            {
                _result = _result.Replace(SendQuoteDate, sendEmailModel.SendQuoteDate);
            }
            if (_result.Contains(EffectiveQuoteDate) && sendEmailModel.EffectiveQuoteDate != null)
            {
                _result = _result.Replace(EffectiveQuoteDate, sendEmailModel.EffectiveQuoteDate);
            }
            if (_result.Contains(CreatedEmployeeName) && sendEmailModel.CreatedEmployeeName != null)
            {
                _result = _result.Replace(CreatedEmployeeName, sendEmailModel.CreatedEmployeeName);
            }
            if (_result.Contains(CreatedEmployeeCode) && sendEmailModel.CreatedEmployeeCode != null)
            {
                _result = _result.Replace(CreatedEmployeeCode, sendEmailModel.CreatedEmployeeCode);
            }
            if (_result.Contains(DetailProduct) && sendEmailModel.SendDetailProduct == true)
            {
                _result = _result.Replace(DetailProduct, ReplaceDetailProduct(_result, sendEmailModel));
            }
            #endregion

            #region replace for create customer
            if (_result.Contains(CustomerGroup) && sendEmailModel.CustomerGroup != null)
            {
                _result = _result.Replace(CustomerGroup, sendEmailModel.CustomerGroup);
            }
            if (_result.Contains(CustomerCode) && sendEmailModel.CustomerCode != null)
            {
                _result = _result.Replace(CustomerCode, sendEmailModel.CustomerCode);
            }
            if (_result.Contains(CustomerAddress) && sendEmailModel.CustomerAddress != null)
            {
                _result = _result.Replace(CustomerAddress, sendEmailModel.CustomerAddress);
            }
            if (_result.Contains(CustomerSeller) && sendEmailModel.CustomerSeller != null)
            {
                _result = _result.Replace(CustomerSeller, sendEmailModel.CustomerSeller);
            }
            if (_result.Contains(CustomerSeller) && sendEmailModel.CustomerSeller != null)
            {
                _result = _result.Replace(CustomerSeller, sendEmailModel.CustomerSeller);
            }
            #endregion

            #region replace for create user
            if (_result.Contains(UserName) && sendEmailModel.UserName != null)
            {
                _result = _result.Replace(UserName, sendEmailModel.UserName);
            }
            if (_result.Contains(UserPassword) && sendEmailModel.UserPassword != null)
            {
                _result = _result.Replace(UserPassword, sendEmailModel.UserPassword);
            }
            #endregion

            #region replace for create order
            if (_result.Contains(OrderCode) && sendEmailModel.OrderCode != null)
            {
                _result = _result.Replace(OrderCode, sendEmailModel.OrderCode);
            }
            if (_result.Contains(ReceivedDateHour) && sendEmailModel.ReceivedDateHour != null)
            {
                _result = _result.Replace(ReceivedDateHour, sendEmailModel.ReceivedDateHour);
            }
            if (_result.Contains(OrderStatus) && sendEmailModel.OrderStatus != null)
            {
                _result = _result.Replace(OrderStatus, sendEmailModel.OrderStatus);
            }
            if (_result.Contains(RecipientPhone) && sendEmailModel.RecipientPhone != null)
            {
                _result = _result.Replace(RecipientPhone, sendEmailModel.RecipientPhone);
            }
            if (_result.Contains(PlaceOfDelivery) && sendEmailModel.PlaceOfDelivery != null)
            {
                _result = _result.Replace(PlaceOfDelivery, sendEmailModel.PlaceOfDelivery);
            }
            if (_result.Contains(OrderDate) && sendEmailModel.OrderDate != null)
            {
                _result = _result.Replace(OrderDate, sendEmailModel.OrderDate);
            }
            if (_result.Contains(RecipientName) && sendEmailModel.RecipientName != null)
            {
                _result = _result.Replace(RecipientName, sendEmailModel.RecipientName);
            }
            if (_result.Contains(CompanyEmail) && sendEmailModel.CompanyEmail != null)
            {
                _result = _result.Replace(CompanyEmail, sendEmailModel.CompanyEmail);
            }
            if (_result.Contains(CompanyPhone) && sendEmailModel.CompanyPhone != null)
            {
                _result = _result.Replace(CompanyPhone, sendEmailModel.CompanyPhone);
            }
            //replace bang detail order
            if (_result.Contains(DetailOrder) && sendEmailModel.SendDetailProductInOrder == true)
            {
                _result = _result.Replace(DetailOrder, ReplaceDetailProductInOrder(_result, sendEmailModel));
            }

            #endregion

            #region replace for create employee request
            if (_result.Contains(EmployeeRequestCode) && sendEmailModel.EmployeeRequestCode != null)
            {
                _result = _result.Replace(EmployeeRequestCode, sendEmailModel.EmployeeRequestCode);
            }
            if (_result.Contains(OfferEmployeeName) && sendEmailModel.OfferEmployeeName != null)
            {
                _result = _result.Replace(OfferEmployeeName, sendEmailModel.OfferEmployeeName);
            }
            if (_result.Contains(CreateEmployeeCode) && sendEmailModel.CreateEmployeeCode != null)
            {
                _result = _result.Replace(CreateEmployeeCode, sendEmailModel.CreateEmployeeCode);
            }
            if (_result.Contains(CreateEmployeeName) && sendEmailModel.CreateEmployeeName != null)
            {
                _result = _result.Replace(CreateEmployeeName, sendEmailModel.CreateEmployeeName);
            }
            if (_result.Contains(CreatedDate) && sendEmailModel.CreatedDate != null)
            {
                _result = _result.Replace(CreatedDate, sendEmailModel.CreatedDate);
            }
            if (_result.Contains(TypeRequestName) && sendEmailModel.TypeRequestName != null)
            {
                _result = _result.Replace(TypeRequestName, sendEmailModel.TypeRequestName);
            }
            if (_result.Contains(DurationTime) && sendEmailModel.DurationTime != null)
            {
                _result = _result.Replace(DurationTime, sendEmailModel.DurationTime);
            }
            if (_result.Contains(Detail) && sendEmailModel.Detail != null)
            {
                _result = _result.Replace(Detail, sendEmailModel.Detail);
            }
            if (_result.Contains(ApproverCode) && sendEmailModel.ApproverCode != null)
            {
                _result = _result.Replace(ApproverCode, sendEmailModel.ApproverCode);
            }
            if (_result.Contains(ApproverName) && sendEmailModel.ApproverName != null)
            {
                _result = _result.Replace(ApproverName, sendEmailModel.ApproverName);
            }
            if (_result.Contains(NotifyList) && sendEmailModel.NotifyList != null)
            {
                _result = _result.Replace(NotifyList, sendEmailModel.NotifyList);
            }
            #endregion

            #region Đổ data thay thế token: Gửi email sau khi gửi phê duyệt Phiếu đề xuất mua hàng

            if (_result.Contains(OrganizationName) && sendEmailModel.OrganizationName != null)
            {
                _result = _result.Replace(OrganizationName, sendEmailModel.OrganizationName);
            }

            if (_result.Contains(ProcurementCode) && sendEmailModel.ProcurementCode != null)
            {
                _result = _result.Replace(ProcurementCode, sendEmailModel.ProcurementCode);
            }

            if (_result.Contains(ProcurementDetailLink))
            {
                var loginLink = "http://" + Domain + @"/login?returnUrl=%2Fhome";
                _result = _result.Replace(ProcurementDetailLink, loginLink);
            }

            #endregion

            var oldHref = "http://" + AccessSystem.ToLower() + "/";

            if (_result.Contains(oldHref))
            {
                //var detailLeadLink = "http://" + Domain + "/lead" + "/detail?" + "leadId=" + sendEmailModel.LeadId + "&contactId=" + sendEmailModel.LeadContactId;
                var detailLeadLink = "http://" + Domain + @"/login?returnUrl=%2Fhome";  //link tam thoi 

                _result = _result.Replace(oldHref, detailLeadLink);
            }

            return _result;
        }

        public string ReplaceTokenForReceiverName(string currentContent, string receiverName)
        {
            var result = currentContent;
            var _receiverName = "[RECEI_NAME]";
            if (result.Contains(_receiverName) && receiverName != null)
            {
                result = result.Replace(_receiverName, receiverName);
            }
            return result;
        }

        private string ReplaceDetailProduct(string currentContent, DataAccess.Models.Email.SendEmailEntityModel sendEmailModel)
        {
            string result = "";
            string beginTable = @"<table class='MsoTable15Plain1' border='1' cellspacing='0' cellpadding='0' width='610' style='width: 457.4pt; border: none;'>
 <tbody>";
            string endTable = @"</tbody></table><div><div><div id='_com_2' class='msocomtxt' language='JavaScript'>

<!--[if !supportAnnotations]--></div>

<!--[endif]--></div>

</div>";
            string sumarySection = @"<tr style='mso-yfti-irow:2;mso-yfti-lastrow:yes;height:33.05pt'>
  <td width='610' colspan='7' style='width:457.4pt;border:solid #BFBFBF 1.0pt;
  mso-border-themecolor:background1;mso-border-themeshade:191;border-top:none;
  mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
  mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:33.05pt' class=''>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
  mso-color-alt:windowtext'>Tổng chiết khấu: [SumDiscount]</span></b><span class='MsoCommentReference'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  mso-fareast-font-family:Calibri;mso-fareast-theme-font:minor-latin;
  color:black;mso-color-alt:windowtext'>&nbsp;</span></b></span><b><span style='font-size:11.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>Tổng số tiền:&nbsp;[SumAmount]</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;mso-fareast-font-family:
  Calibri;mso-fareast-theme-font:minor-latin;color:black;mso-color-alt:windowtext'>&nbsp;</span></b><b><span style='font-size:11.0pt;font-family:
  &quot;Arial&quot;,sans-serif;color:black;mso-color-alt:windowtext'>VND</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>Chiết khấu theo báo giá: [AmountDiscountByQuote]</span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>Thành tiền:&nbsp; </span></b><b><span style='font-size:11.0pt;
  font-family:&quot;Arial&quot;,sans-serif;color:red'>[SumAmountByQuote]</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;mso-fareast-font-family:
  Calibri;mso-fareast-theme-font:minor-latin;color:red'>&nbsp;</span></b><b><span style='font-size:11.0pt;font-family:
  &quot;Arial&quot;,sans-serif;color:black;mso-themecolor:text1'>VND</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;color:red'> &nbsp;</span></b><b><span style='font-size:11.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><span style='font-family: Arial, sans-serif; font-size: 10pt;'>&nbsp;</span><span style='font-family: Arial, sans-serif; font-size: 10pt;'>&nbsp;</span></p>
  <p style='margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:68'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
  mso-color-alt:windowtext'>Thành tiền bằng chữ</span></b><i><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
  mso-color-alt:windowtext'>: [SumAmountTransform]</span></i><i><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></i></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:#222222'><o:p>&nbsp;</o:p></span></b></p>
  </td>
 </tr>";

            sumarySection = sumarySection.Replace("[SumDiscount]", sendEmailModel.SumAmountDiscount); ////tổng tiền chiết khấu theo sản phẩm
            sumarySection = sumarySection.Replace("[SumAmount]", sendEmailModel.SumAmount); //tổng tiền theo sản phẩm
            sumarySection = sumarySection.Replace("[AmountDiscountByQuote]", sendEmailModel.AmountDiscountByQuote); //tổng tiền theo sản phẩm
            sumarySection = sumarySection.Replace("[SumAmountByQuote]", sendEmailModel.SumAmountByQuote); //tổng tiền theo sản phẩm
            sumarySection = sumarySection.Replace("[SumAmountTransform]", sendEmailModel.SumAmountTransform);

            string _header = @"<tr style='mso-yfti-irow:-1;mso-yfti-firstrow:yes;mso-yfti-lastfirstrow:yes;
  height:36.15pt'>
  <td width='142' style='width:106.25pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;
  mso-border-themecolor:background1;mso-border-themeshade:191;background:#6D98E7;
  padding:0in 5.4pt 0in 5.4pt;height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:5'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>SẢN PHẨM/DỊCH VỤ</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
  <td width='108' style='width:81.0pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>ĐƠN VỊ TÍNH<o:p></o:p></span></b></p>
  </td>
  <td width='74' style='width:55.85pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>ĐƠN GIÁ (VND)</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
  <td width='53' style='width:39.7pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>SL<o:p></o:p></span></b></p>
  </td>
  <td width='65' style='width:49.05pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>CHIẾT KHẤU</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
    <td width='64' style='width:48.15pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>VAT (%)</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
  <td width='103' style='width:77.4pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'><o:p>&nbsp;</o:p></span></p>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>THÀNH TIỀN (VND)</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
 </tr>";

            string row = @"<tr style='mso-yfti-irow:0;height:35.25pt'>
  <td width='142' style='width:106.25pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-top:none;mso-border-top-alt:
  solid #BFBFBF .5pt;mso-border-top-themecolor:background1;mso-border-top-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:background1;
  mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;height:35.25pt'>
  <p style='margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:68'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;mso-color-alt:
  windowtext'>[ProductName]</span><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='108' style='width:81.0pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p style='margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:64'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;mso-color-alt:
  windowtext'>[ProductNameUnit]</span><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='74' style='width:55.85pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:64'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[UnitPrice]</span><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='53' style='width:39.7pt;border-top:none;border-left:none;border-bottom:
  solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;mso-border-bottom-themeshade:
  191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:background1;
  mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[Quantity]</span><span style='font-size:9.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='65' style='width:49.05pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[DiscountValue]</span><span style='font-size:10.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='64' style='width:48.15pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[Vat]</span><span style='font-size:10.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='103' style='width:77.4pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[SumAmount]</span><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
 </tr>";
            string bodyTable = "";

            sendEmailModel.ListQuoteDetailToSendEmail.ForEach(_detail =>
            {
                string _temp = row;
                _temp = _temp.Replace("[ProductName]", _detail.ProductName);
                _temp = _temp.Replace("[ProductNameUnit]", _detail.ProductNameUnit);
                _temp = _temp.Replace("[UnitPrice]", _detail.UnitPrice);
                _temp = _temp.Replace("[Quantity]", _detail.Quantity);
                _temp = _temp.Replace("[Vat]", _detail.Vat);
                _temp = _temp.Replace("[DiscountValue]", _detail.DiscountValue);
                _temp = _temp.Replace("[SumAmount]", _detail.SumAmount);

                bodyTable += _temp;
            });

            result = beginTable + _header + bodyTable + sumarySection + endTable;
            return result;
        }

        private string ReplaceDetailProductInOrder(string currentContent, DataAccess.Models.Email.SendEmailEntityModel sendEmailModel)
        {
            string result = "";
            string beginTable = @"<table class='MsoTable15Plain1' border='1' cellspacing='0' cellpadding='0' width='610' style='width: 457.4pt; border: none;'>
 <tbody>";
            string endTable = @"</tbody></table><div><div><div id='_com_2' class='msocomtxt' language='JavaScript'>

<!--[if !supportAnnotations]--></div>

<!--[endif]--></div>

</div>";
            string sumarySection = @"<tr style='mso-yfti-irow:2;mso-yfti-lastrow:yes;height:33.05pt'>
  <td width='610' colspan='7' style='width:457.4pt;border:solid #BFBFBF 1.0pt;
  mso-border-themecolor:background1;mso-border-themeshade:191;border-top:none;
  mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
  mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:33.05pt' class=''>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
  mso-color-alt:windowtext'>Tổng chiết khấu: [SumDiscount]</span></b><span class='MsoCommentReference'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  mso-fareast-font-family:Calibri;mso-fareast-theme-font:minor-latin;
  color:black;mso-color-alt:windowtext'>&nbsp;</span></b></span><b><span style='font-size:11.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>Tổng số tiền:&nbsp;[SumAmount]</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;mso-fareast-font-family:
  Calibri;mso-fareast-theme-font:minor-latin;color:black;mso-color-alt:windowtext'>&nbsp;</span></b><b><span style='font-size:11.0pt;font-family:
  &quot;Arial&quot;,sans-serif;color:black;mso-color-alt:windowtext'>VND</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>Chiết khấu theo báo giá: [AmountDiscountByQuote]</span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>Thành tiền:&nbsp; </span></b><b><span style='font-size:11.0pt;
  font-family:&quot;Arial&quot;,sans-serif;color:red'>[SumAmountByQuote]</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;mso-fareast-font-family:
  Calibri;mso-fareast-theme-font:minor-latin;color:red'>&nbsp;</span></b><b><span style='font-size:11.0pt;font-family:
  &quot;Arial&quot;,sans-serif;color:black;mso-themecolor:text1'>VND</span></b><b><span style='font-size:11.0pt;font-family:&quot;Arial&quot;,sans-serif;color:red'> &nbsp;</span></b><b><span style='font-size:11.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></b></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><span style='font-family: Arial, sans-serif; font-size: 10pt;'>&nbsp;</span><span style='font-family: Arial, sans-serif; font-size: 10pt;'>&nbsp;</span></p>
  <p style='margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:68'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
  mso-color-alt:windowtext'>Thành tiền bằng chữ</span></b><i><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
  mso-color-alt:windowtext'>: [SumAmountTransform]</span></i><i><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></i></p>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:68'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:#222222'><o:p>&nbsp;</o:p></span></b></p>
  </td>
 </tr>";

            sumarySection = sumarySection.Replace("[SumDiscount]", sendEmailModel.SumAmountDiscountByProductInOder); ////tổng tiền chiết khấu theo sản phẩm
            sumarySection = sumarySection.Replace("[SumAmount]", sendEmailModel.SumAmountBeforeDiscount); //tổng tiền theo sản phẩm
            sumarySection = sumarySection.Replace("[AmountDiscountByQuote]", sendEmailModel.SumAmountDiscountByOrder); // chiết khấu theo tổng đơn hàng
            sumarySection = sumarySection.Replace("[SumAmountByQuote]", sendEmailModel.SumAmountAfterDiscount); //tổng tiền theo sản phẩm
            sumarySection = sumarySection.Replace("[SumAmountTransform]", sendEmailModel.SumAmountTransformInOrder);

            string _header = @"<tr style='mso-yfti-irow:-1;mso-yfti-firstrow:yes;mso-yfti-lastfirstrow:yes;
  height:36.15pt'>
  <td width='142' style='width:106.25pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;
  mso-border-themecolor:background1;mso-border-themeshade:191;background:#6D98E7;
  padding:0in 5.4pt 0in 5.4pt;height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:5'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>SẢN PHẨM/DỊCH VỤ</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
  <td width='108' style='width:81.0pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>ĐƠN VỊ TÍNH<o:p></o:p></span></b></p>
  </td>
  <td width='74' style='width:55.85pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>ĐƠN GIÁ (VND)</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
  <td width='53' style='width:39.7pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>SL<o:p></o:p></span></b></p>
  </td>
  <td width='65' style='width:49.05pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>CHIẾT KHẤU</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
    <td width='64' style='width:48.15pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>VAT (%)</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
  <td width='103' style='width:77.4pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-left:none;mso-border-left-alt:
  solid #BFBFBF .5pt;mso-border-left-themecolor:background1;mso-border-left-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#6D98E7;padding:0in 5.4pt 0in 5.4pt;
  height:36.15pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'><o:p>&nbsp;</o:p></span></p>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:1'><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:white;mso-themecolor:background1'>THÀNH TIỀN (VND)</span></b><b><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222'><o:p></o:p></span></b></p>
  </td>
 </tr>";

            string row = @"<tr style='mso-yfti-irow:0;height:35.25pt'>
  <td width='142' style='width:106.25pt;border:solid #BFBFBF 1.0pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;border-top:none;mso-border-top-alt:
  solid #BFBFBF .5pt;mso-border-top-themecolor:background1;mso-border-top-themeshade:
  191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
  mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:background1;
  mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;height:35.25pt'>
  <p style='margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:68'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;mso-color-alt:
  windowtext'>[ProductName]</span><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='108' style='width:81.0pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p style='margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:64'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;mso-color-alt:
  windowtext'>[ProductNameUnit]</span><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='74' style='width:55.85pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:64'><span style='font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[UnitPrice]</span><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='53' style='width:39.7pt;border-top:none;border-left:none;border-bottom:
  solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;mso-border-bottom-themeshade:
  191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:background1;
  mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[Quantity]</span><span style='font-size:9.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='65' style='width:49.05pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[DiscountValue]</span><span style='font-size:10.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='64' style='width:48.15pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='center' style='margin:0in;margin-bottom:.0001pt;text-align:center;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[Vat]</span><span style='font-size:10.0pt;
  font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
  <td width='103' style='width:77.4pt;border-top:none;border-left:none;
  border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
  mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;mso-border-right-themecolor:
  background1;mso-border-right-themeshade:191;mso-border-top-alt:solid #BFBFBF .5pt;
  mso-border-top-themecolor:background1;mso-border-top-themeshade:191;
  mso-border-left-alt:solid #BFBFBF .5pt;mso-border-left-themecolor:background1;
  mso-border-left-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
  background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
  background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
  height:35.25pt'>
  <p align='right' style='margin:0in;margin-bottom:.0001pt;text-align:right;
  mso-yfti-cnfc:64'><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif;
  color:black;mso-color-alt:windowtext'>[SumAmount]</span><span style='font-size:10.0pt;font-family:&quot;Arial&quot;,sans-serif'><o:p></o:p></span></p>
  </td>
 </tr>";
            string bodyTable = "";

            sendEmailModel.ListDetailToSendEmailInOrder.ForEach(_detail =>
            {
                string _temp = row;
                _temp = _temp.Replace("[ProductName]", _detail.ProductName);
                _temp = _temp.Replace("[ProductNameUnit]", _detail.ProductNameUnit);
                _temp = _temp.Replace("[UnitPrice]", _detail.UnitPrice);
                _temp = _temp.Replace("[Quantity]", _detail.Quantity);
                _temp = _temp.Replace("[Vat]", _detail.Vat);
                _temp = _temp.Replace("[DiscountValue]", _detail.DiscountValue);
                _temp = _temp.Replace("[SumAmount]", _detail.SumAmount);

                bodyTable += _temp;
            });

            result = beginTable + _header + bodyTable + sumarySection + endTable;
            return result;
        }
    }
}
