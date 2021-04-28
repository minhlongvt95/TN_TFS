using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CreateCustomerFromProtalRequest : BaseRequest<CreateCustomerFromProtalParameter>
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

        public override CreateCustomerFromProtalParameter ToParameter()
        {
            return new CreateCustomerFromProtalParameter
            {
                CustomerName = this.CustomerName,
                TaxCode = this.TaxCode,
                BusinessRegistrationDate = this.BusinessRegistrationDate,
                EnterpriseType = this.EnterpriseType,
                Address = this.Address,
                Phone = this.Phone,
                Email = this.Email,
                FieldId = this.FieldId,
                TotalEmployeeParticipateSocialInsurance = this.TotalEmployeeParticipateSocialInsurance,
                TotalCapital = this.TotalCapital,
                TotalRevenueLastYear = this.TotalRevenueLastYear,
                BusinessScale = this.BusinessScale,
                PortalID = PortalID,
                TokenId=TokenId
            };
        }
    }
}
