using System;
using System.Collections.Generic;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;
using TN.TNM.DataAccess.Messages.Results.BankAccount;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class BankAccountDAO : BaseDAO, IBankAccountDataAccess
    {
        public BankAccountDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public CreateBankAccountResult CreateBankAccount(CreateBankAccountParameter parameter)
        {
            try
            {
                if (parameter.BankAccount.BankAccountId != null)
                {
                    var bankAccount =
                        context.BankAccount.FirstOrDefault(x => x.BankAccountId == parameter.BankAccount.BankAccountId);

                    bankAccount.ObjectId = parameter.BankAccount.ObjectId;
                    bankAccount.ObjectType = parameter.BankAccount.ObjectType;
                    bankAccount.AccountNumber = parameter.BankAccount.AccountNumber;
                    bankAccount.BankName = parameter.BankAccount.BankName;
                    bankAccount.BankDetail = parameter.BankAccount.BankDetail;
                    bankAccount.BranchName = parameter.BankAccount.BranchName;
                    bankAccount.AccountName = parameter.BankAccount.AccountName;
                    bankAccount.UpdatedDate = DateTime.Now;
                    bankAccount.UpdatedById = parameter.UserId;

                    context.BankAccount.Update(bankAccount);
                    context.SaveChanges();
                }
                else
                {
                    var newBankAccount = new BankAccount();
                    newBankAccount.BankAccountId = Guid.NewGuid();
                    newBankAccount.ObjectId = parameter.BankAccount.ObjectId;
                    newBankAccount.ObjectType = parameter.BankAccount.ObjectType;
                    newBankAccount.AccountNumber = parameter.BankAccount.AccountNumber;
                    newBankAccount.BankName = parameter.BankAccount.BankName;
                    newBankAccount.BankDetail = parameter.BankAccount.BankDetail;
                    newBankAccount.BranchName = parameter.BankAccount.BranchName;
                    newBankAccount.AccountName = parameter.BankAccount.AccountName;
                    newBankAccount.Active = true;
                    newBankAccount.CreatedById = parameter.UserId;
                    newBankAccount.CreatedDate = DateTime.Now;
                    newBankAccount.UpdatedById = null;
                    newBankAccount.UpdatedDate = null;

                    context.BankAccount.Add(newBankAccount);
                    context.SaveChanges();
                }

                #region Lấy thông tin thanh toán theo đối tượng

                var listBankAccount = new List<BankAccount>();
                listBankAccount = context.BankAccount
                    .Where(b => b.ObjectId == parameter.BankAccount.ObjectId &&
                                b.ObjectType == parameter.BankAccount.ObjectType).OrderByDescending(z => z.CreatedDate)
                    .ToList();

                #endregion

                return new CreateBankAccountResult()
                {
                    Status = true,
                    Message = "Success",
                    ListBankAccount = listBankAccount
                };
            }
            catch (Exception e)
            {
                return new CreateBankAccountResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetBankAccountByIdResult GetBankAccountById(GetBankAccountByIdParameter parameter)
        {
            var bankAccount = context.BankAccount.FirstOrDefault(b => b.BankAccountId == parameter.BankAccountId);
            return new GetBankAccountByIdResult() {
                Status = true,
                BankAccount = bankAccount
            };
        }

        public EditBankAccountResult EditBankAccount(EditBankAccountParameter parameter)
        {
            context.BankAccount.Update(parameter.BankAccount);
            context.SaveChanges();
            return new EditBankAccountResult() {
                Status = true,
                Message = CommonMessage.BankAccount.EDIT_BANK_SUCCESS
            };
        }

        public DeleteBankAccountByIdResult DeleteBankAccountById(DeleteBankAccountByIdParameter parameter)
        {
            var bankAccount = context.BankAccount.FirstOrDefault(b => b.BankAccountId == parameter.BankAccountId);
            var bankPayableInvoice = context.BankPayableInvoice.FirstOrDefault(b => b.BankPayableInvoiceBankAccountId == bankAccount.BankAccountId);
            var bankReceiptInvoice = context.BankReceiptInvoice.FirstOrDefault(b => b.BankReceiptInvoiceBankAccountId == bankAccount.BankAccountId);
            if (bankPayableInvoice != null)
            {
                return new DeleteBankAccountByIdResult()
                {
                    Status = false,
                    Message = "Tài khoản này đang có phiếu Ủy nhiệm chi gắn cùng"
                };
            } else if (bankReceiptInvoice != null)
            {
                return new DeleteBankAccountByIdResult()
                {
                    Status = false,
                    Message = "Tài khoản này đang có Báo có gắn cùng"
                };
            }
            
            context.BankAccount.Remove(bankAccount);
            context.SaveChanges();

            #region Lấy thông tin thanh toán theo đối tượng

            var listBankAccount = new List<BankAccount>();
            listBankAccount = context.BankAccount
                .Where(b => b.ObjectId == parameter.ObjectId &&
                            b.ObjectType == parameter.ObjectType).OrderByDescending(z => z.CreatedDate).ToList();

            #endregion

            return new DeleteBankAccountByIdResult() {
                Status = true,
                Message = CommonMessage.BankAccount.DELETE_BANK_SUCCESS,
                ListBankAccount = listBankAccount
            };
        }

        public GetAllBankAccountByObjectResult GetAllBankAccountByObject(GetAllBankAccountByObjectParameter parameter)
        {
            var bankList = context.BankAccount
                .Where(b => b.ObjectId == parameter.ObjectId && b.ObjectType == parameter.ObjectType).Select(y =>
                    new BankAccountEntityModel
                    {
                        BankAccountId = y.BankAccountId,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        AccountNumber = y.AccountNumber,
                        BankName = y.BankName,
                        BankDetail = y.BankDetail,
                        BranchName = y.BranchName,
                        AccountName = y.AccountName,
                        LabelShow = y.AccountNumber + " - " + y.AccountName + " - " + y.BankName,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        Active = y.Active
                    }).ToList();
            return new GetAllBankAccountByObjectResult() {
                Status = true,
                BankAccountList = bankList
            };
        }

        public GetCompanyBankAccountResult GetCompanyBankAccount(GetCompanyBankAccountParameter parameter)
        {
            var bankList = context.BankAccount.Where(b => b.ObjectType == ObjectType.COMPANY).ToList();
            return new GetCompanyBankAccountResult()
            {
                Status = true,
                BankList = bankList
            };
        }
    }
}
