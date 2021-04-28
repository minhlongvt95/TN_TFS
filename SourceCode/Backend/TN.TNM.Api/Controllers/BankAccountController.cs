using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.BankAccount;
using TN.TNM.BusinessLogic.Messages.Requests.BankAccount;
using TN.TNM.BusinessLogic.Messages.Responses.BankAccount;

namespace TN.TNM.Api.Controllers
{
    public class BankAccountController : Controller
    {
        private readonly IBankAccount _iBankAccount;
        public BankAccountController(IBankAccount iBankAccount)
        {
            this._iBankAccount = iBankAccount;
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/createBankAccount")]
        [Authorize(Policy = "Member")]
        public CreateBankAccountResponse CreateBankAccount([FromBody]CreateBankAccountRequest request)
        {
            return this._iBankAccount.CreateBankAccount(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/getBankAccountById")]
        [Authorize(Policy = "Member")]
        public GetBankAccountByIdResponse GetBankAccountById([FromBody]GetBankAccountByIdRequest request)
        {
            return this._iBankAccount.GetBankAccountById(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/getAllBankAccountByObject")]
        [Authorize(Policy = "Member")]
        public GetAllBankAccountByObjectResponse GetAllBankAccountByObject([FromBody]GetAllBankAccountByObjectRequest request)
        {
            return this._iBankAccount.GetAllBankAccountByObject(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/editBankAccount")]
        [Authorize(Policy = "Member")]
        public EditBankAccountResponse EditBankAccount([FromBody]EditBankAccountRequest request)
        {
            return this._iBankAccount.EditBankAccount(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/deleteBankAccount")]
        [Authorize(Policy = "Member")]
        public DeleteBankAccountByIdResponse DeleteBankAccount([FromBody]DeleteBankAccountByIdRequest request)
        {
            return this._iBankAccount.DeleteBankAccount(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankAccount/getCompanyBankAccount")]
        [Authorize(Policy = "Member")]
        public GetCompanyBankAccountResponse GetCompanyBankAccount(GetCompanyBankAccountRequest request)
        {
            return this._iBankAccount.GetCompanyBankAccount(request);
        }
    }
}