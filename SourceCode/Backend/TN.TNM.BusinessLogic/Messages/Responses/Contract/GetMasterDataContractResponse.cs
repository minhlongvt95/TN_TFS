using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contract
{
    public class GetMasterDataContractResponse : BaseResponse
    {
        public List<CustomerModel> ListCustomer { get; set; }
        public List<CategoryModel> ListPaymentMethod { get; set; }
        public List<CategoryModel> ListTypeContract { get; set; }
        public List<QuoteModel> ListQuote { get; set; }
        public List<EmployeeModel> ListEmployeeSeller { get; set; }
        public List<CategoryModel> ListContractStatus { get; set; }
        public List<BankAccountModel> ListBankAccount { get; set; }
        //public List<ContractModel> ListContract { get; set; }
        public List<QuoteDetailModel> ListQuoteDetail { get; set; }
        public List<QuoteCostDetailModel> ListQuoteCostDetail { get; set; }

        // Get Data view
        public ContractModel Contract { get; set; }
        public List<ContractDetailModel> ListContractDetail { get; set; }
        public List<ContractDetailProductAttributeModel> ListContractDetailProductAttribute { get; set; }
        //public EmployeeEntityModel EmployeeSeller { get; set; } //Người bán hàng
        public List<AdditionalInformationModel> ListAdditionalInformation { get; set; }
        public List<NoteModel> ListNote { get; set; }
        public List<ContractCostDetailModel> ListContractCost { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<FileInFolderModel> ListFile { get; set; }
        public List<DataAccess.Databases.Entities.Contract> ListContract { get; set; }
        public Boolean IsOutOfDate { get; set; }
    }
}
