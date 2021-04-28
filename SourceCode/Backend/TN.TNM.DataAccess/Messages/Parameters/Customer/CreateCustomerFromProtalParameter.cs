using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CreateCustomerFromProtalParameter : BaseParameter
    {
        public string CustomerName { get; set; }
        public string TaxCode { get; set; }
        public DateTime? BusinessRegistrationDate { get; set; } //dd/MM/YYYY
        public string EnterpriseType { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FieldId { get; set; } //Lĩnh vực sản xuất, kinh doanh chính
        public int? TotalEmployeeParticipateSocialInsurance { get; set; }//Tổng số lao động tham gia bảo hiểm xã hội
        public decimal? TotalCapital { get; set; }
        public decimal? TotalRevenueLastYear { get; set; }
        public string BusinessScale { get; set; }//Quy mô doanh nghiệp(nhỏ,vừa)
        public int PortalID { get; set; }
        public string TokenId { get; set; }

    }
}
