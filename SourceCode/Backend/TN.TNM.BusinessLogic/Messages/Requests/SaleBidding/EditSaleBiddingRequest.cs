using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.SaleBidding;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class EditSaleBiddingRequest:BaseRequest<EditSaleBiddingParameter>
    {
        public SaleBiddingModel SaleBidding { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public List<CostQuoteModel> ListCost { get; set; }
        public List<CostQuoteModel> ListQuocte { get; set; }
        public List<SaleBiddingDetailModel> ListSaleBiddingDetail { get; set; }
        public override EditSaleBiddingParameter ToParameter()
        {
            var parameter = new EditSaleBiddingParameter()
            {
                SaleBidding = SaleBidding.ToEntityModel(),
                ListCost = new List<CostQuoteModel>(),
                ListEmployee = ListEmployee,
                UserId = UserId,
                ListQuocte = new List<CostQuoteModel>(),
                ListSaleBiddingDetail = new List<SaleBiddingDetailEntityModel>()
            };
            ListCost = ListCost == null ? new List<CostQuoteModel>() : ListCost;
            ListCost.ForEach(item =>
            {
                parameter.ListCost.Add(item);
            });

            ListQuocte = ListQuocte == null ? new List<CostQuoteModel>() : ListQuocte;
            ListQuocte.ForEach(item =>
            {
                parameter.ListQuocte.Add(item);
            });

            ListSaleBiddingDetail = ListSaleBiddingDetail == null ? new List<SaleBiddingDetailModel>() : ListSaleBiddingDetail;
            ListSaleBiddingDetail.ForEach(item =>
            {
                var temp = item.ToEntityModel();
                temp.ListFormFile = item.ListFormFile;
                temp.ListFile = new List<FileInFolderEntityModel>();
                item.ListFile = item.ListFile == null ? new List<FileInFolderModel>() : item.ListFile;
                item.ListFile.ForEach(file =>
                {
                    temp.ListFile.Add(file.ToEntityModel());
                });
                parameter.ListSaleBiddingDetail.Add(temp);
            });

            return parameter;
        }
    }
}
