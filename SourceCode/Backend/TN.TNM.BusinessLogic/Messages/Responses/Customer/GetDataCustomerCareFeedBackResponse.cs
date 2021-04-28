using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetDataCustomerCareFeedBackResponse : BaseResponse
    {
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TypeName { get; set; }
        public Guid? FeedBackCode { get; set; }
        public string FeedBackContent { get; set; }
        public List<CategoryModel> ListFeedBackCode { get; set; }
    }
}
