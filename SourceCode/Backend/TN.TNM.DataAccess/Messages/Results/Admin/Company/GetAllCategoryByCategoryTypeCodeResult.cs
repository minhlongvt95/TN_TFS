using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Company
{
    public class GetAllCategoryByCategoryTypeCodeResult : BaseResult
    {
        public List<CategoryEntityModel> Category { get; set; }
        public List<CategoryEntityModel> CategoryPTOList { get; set; }
        public List<CategoryEntityModel> CategoryNHAList { get; set; }
        public List<CategoryEntityModel> CategoryTHAList { get; set; }
        public List<CategoryEntityModel> CategoryTNHList { get; set; }
        public List<CategoryEntityModel> CategoryLDOList { get; set; }
        public List<CategoryEntityModel> CategoryQNGList { get; set; }
        public List<CategoryEntityModel> CategoryGENDERList { get; set; }
        public List<CategoryEntityModel> CategoryLHIList { get; set; }
        public List<CategoryEntityModel> CategoryCVUList { get; set; }
        public List<CategoryEntityModel> CategoryLNGList { get; set; }
        public List<CategoryEntityModel> CategoryNCHList { get; set; }
        public List<CategoryEntityModel> CategoryPMList { get; set; }
        public List<CategoryEntityModel> CategoryDVIList { get; set; }
        public List<CategoryEntityModel> CategoryLabourContractList { get; set; }
        public List<CategoryEntityModel> CategoryNCAList { get; set; }
    }
}
