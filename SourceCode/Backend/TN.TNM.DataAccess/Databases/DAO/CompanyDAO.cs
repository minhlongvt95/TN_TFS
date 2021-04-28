using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Company;
using TN.TNM.DataAccess.Messages.Parameters.CompanyConfig;
using TN.TNM.DataAccess.Messages.Results.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Company;
using TN.TNM.DataAccess.Messages.Results.CompanyConfig;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CompanyDAO : BaseDAO, ICompanyDataAccess
    {
        public CompanyDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllCompanyResult GetAllCompany(GetAllCompanyParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.COMPANY, "GetAllCompany", parameter.UserId);
            var company = context.Company.ToList();
            return new GetAllCompanyResult
            {
                Company = company
            };
        }

        public GetCompanyConfigResults GetCompanyConfig(GetCompanyConfigParameter parameter)
        {
            var companyConfig = context.CompanyConfiguration.FirstOrDefault();
            var listBankAccount = context.BankAccount.Where(item=>item.ObjectType=="COM").Select(c=>new BankAccountEntityModel {
                BankAccountId=c.BankAccountId,
                ObjectId=c.ObjectId,
                ObjectType=c.ObjectType,
                AccountNumber=c.AccountNumber,
                BankName=c.BankName,
                BankDetail=c.BankDetail,
                BranchName=c.BankName,
                AccountName=c.AccountName,
                Active=c.Active,
                CreatedDate=c.CreatedDate
            }).OrderBy(z => z.CreatedDate).ToList();

            return new GetCompanyConfigResults
            {
                CompanyConfig = new CompanyConfigEntityModel(companyConfig),
                ListBankAccount=listBankAccount
            };
        }
        public EditCompanyConfigResults EditCompanyConfig(EditCompanyConfigParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.UPDATE, ObjectName.COMPANY, "Edit Company Config", parameter.UserId);
            parameter.CompanyConfigurationObject.CompanyName=parameter.CompanyConfigurationObject.CompanyName.Trim();
            parameter.CompanyConfigurationObject.TaxCode=parameter.CompanyConfigurationObject.TaxCode.Trim();
            parameter.CompanyConfigurationObject.Email= parameter.CompanyConfigurationObject.Email.Trim();
            parameter.CompanyConfigurationObject.ContactName=parameter.CompanyConfigurationObject.ContactName.Trim();
            parameter.CompanyConfigurationObject.ContactRole=parameter.CompanyConfigurationObject.ContactRole.Trim();
            parameter.CompanyConfigurationObject.CompanyAddress=parameter.CompanyConfigurationObject.CompanyAddress.Trim();
            context.CompanyConfiguration.Update(parameter.CompanyConfigurationObject);
            context.SaveChanges();
            return new EditCompanyConfigResults
            {
                Status = true,
                CompanyID = parameter.CompanyConfigurationObject.CompanyId
            };
        }

        public GetAllSystemParameterResult GetAllSystemParameter(GetAllSystemParameterParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.UPDATE, "System Parameter", "Get all system parameter", parameter.UserId);
                var systemParameterList = context.SystemParameter.OrderBy(w => w.SystemGroupCode).ToList();

                return new GetAllSystemParameterResult
                {
                    systemParameterList = systemParameterList,
                    Status = true,
                    Message = "Lưu thành công"
                };
            }
            catch (Exception)
            {
                return new GetAllSystemParameterResult
                {
                    Status = false,
                    Message = "Có lỗi xảy ra khi lưu"
                };
            }
        }

        public ChangeSystemParameterResult ChangeSystemParameter(ChangeSystemParameterParameter parameter)
        {
            try
            {
                var systemParameter = context.SystemParameter.FirstOrDefault(e => e.SystemKey == parameter.SystemKey);
                if (systemParameter == null)
                {
                    return new ChangeSystemParameterResult
                    {
                        Status = false,
                        Message = "Không tồn tại tham số này trên hệ thống"
                    };
                }
                systemParameter.SystemValue = parameter.SystemValue;
                systemParameter.SystemValueString = parameter.SystemValueString;
                systemParameter.Description = parameter.Description;
                context.SystemParameter.Update(systemParameter);
                context.SaveChanges();

                List<SystemParameter> systemParameterList = new List<SystemParameter>();
                systemParameterList = context.SystemParameter.ToList();

                return new ChangeSystemParameterResult
                {
                    Status = true,
                    Message = "Lưu thành công",
                    SystemParameterList = systemParameterList
                };
            }
            catch (Exception)
            {
                return new ChangeSystemParameterResult
                {
                    Status = false,
                    Message = "Có lỗi xảy ra khi lưu"
                };
            }
        }
    }
}
