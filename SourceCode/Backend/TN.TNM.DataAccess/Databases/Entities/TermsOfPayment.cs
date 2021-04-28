using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TermsOfPayment
    {
        public Guid TermsOfPaymentId { get; set; }
        public Guid TermsOfPaymentUnitId { get; set; }
        public Guid? TermsOfPaymentCode { get; set; }
        public Guid? TermsOfPaymentName { get; set; }
        public Guid? Time { get; set; }
        public bool Active { get; set; }
        public Guid? TenantId { get; set; }
    }
}
