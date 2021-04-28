using TN.TNM.DataAccess.Messages.Parameters.Receivable.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Receivable.Vendor;
using TN.TNM.DataAccess.Messages.Parameters.SalesReport;
using TN.TNM.DataAccess.Messages.Results.Receivable.Customer;
using TN.TNM.DataAccess.Messages.Results.Receivable.Vendor;
using TN.TNM.DataAccess.Messages.Results.SalesReport;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IReceivableDataAccess
    {
        GetReceivableVendorReportResults GetReceivableVendorReport(GetReceivableVendorReportParameter parameter);
        GetReceivableVendorDetailResults GetReceivableVendorDetail(GetReceivableVendorDetailParameter parameter);
        GetReceivableCustomerReportResults GetReceivableCustomerReport(GetReceivableCustomerReportParameter parameter);
        GetReceivableCustomerDetailResults GetReceivableCustomerDetail(GetReceivableCustomerDetailParameter parameter);
        ExportExcelReceivableReportResult ExportExcelReceivableReport(ExportExcelReceivableReportParameter parameter);
        SalesReportResult SearchSalesReport(SalesReportParameter parameter);
        GetDataSearchReceivableVendorResult GetDataSearchReceivableVendor(GetDataSearchReceivableVendorParameter parameter);
    }
}
