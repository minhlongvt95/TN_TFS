using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Category
{
    public class GetAllCategoryByCategoryTypeCodeResponse : BaseResponse
    {
        public List<CategoryModel> Category { get; set; }
        public List<CategoryModel> CategoryPTOList { get; set; }
        public List<CategoryModel> CategoryNHAList { get; set; }
        public List<CategoryModel> CategoryTHAList { get; set; }
        public List<CategoryModel> CategoryTNHList { get; set; }
        public List<CategoryModel> CategoryLDOList { get; set; }
        public List<CategoryModel> CategoryQNGList { get; set; }
        public List<CategoryModel> CategoryGENDERList { get; set; }
        public List<CategoryModel> CategoryLHIList { get; set; }
        public List<CategoryModel> CategoryCVUList { get; set; }
        public List<CategoryModel> CategoryLNGList { get; set; }
        public List<CategoryModel> CategoryNCHList { get; set; }
        public List<CategoryModel> CategoryPMList { get; set; }
        public List<CategoryModel> CategoryDVIList { get; set; }
        public List<CategoryModel> CategoryLabourContractList { get; set; }
        public List<CategoryModel> CategoryNCAList { get; set; }
    }
}
