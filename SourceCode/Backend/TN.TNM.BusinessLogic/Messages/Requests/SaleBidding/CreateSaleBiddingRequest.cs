using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.SaleBidding;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class CreateSaleBiddingRequest:BaseRequest<CreateSaleBiddingParameter>
    {
        public SaleBiddingModel SaleBidding { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public List<LeadDetailModel> ListCost { get; set; }
        public List<LeadDetailModel> ListQuocte { get; set; }
        public List<SaleBiddingDetailModel> ListSaleBiddingDetail { get; set; }

        public override CreateSaleBiddingParameter ToParameter()
        {
            var parameter = new CreateSaleBiddingParameter()
            {
                SaleBidding = SaleBidding.ToEntityModel(),
                ListCost = new List<LeadDetailModel>(),
                ListEmployee = ListEmployee==null?new List<Guid>(): ListEmployee,
                UserId = UserId,
                ListQuocte = new List<LeadDetailModel>(),
                ListSaleBiddingDetail = new List<SaleBiddingDetailEntityModel>()
            };
            ListCost = ListCost == null ? new List<LeadDetailModel>() : ListCost;
            ListCost.ForEach(item =>
            {
                parameter.ListCost.Add(item);
            });
            ListQuocte = ListQuocte == null ? new List<LeadDetailModel>() : ListQuocte;
            ListQuocte.ForEach(item =>
            {
                parameter.ListQuocte.Add(item);
            });

            ListSaleBiddingDetail = ListSaleBiddingDetail == null ? new List<SaleBiddingDetailModel>() : ListSaleBiddingDetail;
            ListSaleBiddingDetail.ForEach(item =>
            {
                var temp = item.ToEntityModel();
                temp.ListFormFile = item.ListFormFile;
                parameter.ListSaleBiddingDetail.Add(temp);
            });

            return parameter;
        }
    }
}
