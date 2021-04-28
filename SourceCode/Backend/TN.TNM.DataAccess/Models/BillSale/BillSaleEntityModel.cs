using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.BillSale
{
    public class BillSaleEntityModel
    {
        public Guid BillOfSaLeId { get; set; }
        public string BillOfSaLeCode { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? BillDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? TermsOfPaymentId { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? DebtAccountId { get; set; }
        public string Mst { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string CustomerAddress { get; set; }
        public Guid? EmployeeId { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid? AccountBankId { get; set; }
        public bool Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string InvoiceSymbol { get; set; }
        public decimal? Amount { get; set; }
        public string StatusName { get; set; }
        public string Seller { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }

        public List<BillSaleCostEntityModel> ListCost { get; set; }
        public List<BillSaleDetailEntityModel> ListBillSaleDetail { get; set; }
    }
}
