using TN.TNM.BusinessLogic.Messages.Requests.Receivable.Customer;
using TN.TNM.BusinessLogic.Messages.Requests.Receivable.Vendor;
using TN.TNM.BusinessLogic.Messages.Requests.SalesReport;
using TN.TNM.BusinessLogic.Messages.Responses.Receivable.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Receivable.Vendor;
using TN.TNM.BusinessLogic.Messages.Responses.SalesReport;

namespace TN.TNM.BusinessLogic.Interfaces.Receivable
{
    public interface IReceivable
    {
        GetReceivableVendorDetailResponse GetReceivableVendorDetail(GetReceivableVendorDetailRequest request);
        GetReceivableVendorReportResponse GetReceivableVendorReport(GetReceivableVendorReportRequest request);
        GetReceivableCustomerReportResponse GetReceivableCustomerReport(GetReceivableCustomerReportRequest request);
        GetReceivableCustomerDetailResponse GetReceivableCustomerDetail(GetReceivableCustomerDetailRequest request);
        ExportExcelReceivableReportResponse ExportExcelReceivableReport(ExportExcelReceivableReportRequest request);
        SalesReportResponse SearchSalesReport(SalesReportRequest request);
        GetDataSearchReceivableVendorResponse GetDataSearchReceivableVendor(GetDataSearchReceivableVendorRequest request);
    }
}
