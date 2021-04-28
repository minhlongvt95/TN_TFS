using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class SearchProductCategoryRequest:BaseRequest<SearchProductCategoryParameter>
    {
        public string Keyword_name { get; set; }
        public string Keyword_code { get; set; }
        public override SearchProductCategoryParameter ToParameter() => new SearchProductCategoryParameter()
        {
            Keyword_name = Keyword_name,
            Keyword_code = Keyword_code,
            UserId = this.UserId
        };
    }

}
