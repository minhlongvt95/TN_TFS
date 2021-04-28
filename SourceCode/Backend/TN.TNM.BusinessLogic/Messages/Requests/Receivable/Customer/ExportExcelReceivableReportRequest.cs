using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Receivable;
using TN.TNM.DataAccess.Messages.Parameters.Receivable.Customer;
using TN.TNM.DataAccess.Models.Receivable;

namespace TN.TNM.BusinessLogic.Messages.Requests.Receivable.Customer
{
    public class ExportExcelReceivableReportRequest : BaseRequest<ExportExcelReceivableReportParameter>
    {
        public List<ReceivableCustomerModel> ReceivableCustomerDetail { get; set; }
        public List<ReceivableCustomerModel> ReceiptsList { get; set; }
        public Guid CustomerId { get; set; }
        public decimal? TotalReceivableBefore { get; set; }
        public decimal? TotalReceivableInPeriod { get; set; }
        public decimal? TotalReceivable { get; set; }
        public decimal? TotalPurchaseProduct { get; set; }
        public decimal? TotalReceipt { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public override ExportExcelReceivableReportParameter ToParameter()
        {
            var lst = new List<ReceivableCustomerEntityModel>();
            var lst2 = new List<ReceivableCustomerEntityModel>();

            if(ReceivableCustomerDetail != null && ReceivableCustomerDetail.Count > 0)
            {
                ReceivableCustomerDetail.ForEach(item => {
                    lst.Add(item.ToModel());
                });
            }

            if(ReceiptsList != null && ReceiptsList.Count > 0)
            {
                ReceiptsList.ForEach(item => {
                    lst2.Add(item.ToModel());
                });
            }

            return new ExportExcelReceivableReportParameter() {
                UserId = UserId,
                ReceivableCustomerDetail = lst,
                ReceiptsList = lst2,
                CustomerId = CustomerId,
                TotalReceivableBefore = TotalReceivableBefore,
                TotalReceivableInPeriod = TotalReceivableInPeriod,
                TotalReceivable = TotalReceivable,
                TotalPurchaseProduct = TotalPurchaseProduct,
                TotalReceipt = TotalReceipt,
                FromDate = FromDate,
                ToDate = ToDate
            };
        }
    }
}
