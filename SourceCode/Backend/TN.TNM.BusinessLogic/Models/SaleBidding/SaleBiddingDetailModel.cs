using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.SaleBidding;

namespace TN.TNM.BusinessLogic.Models.SaleBidding
{
    public class SaleBiddingDetailModel : BaseModel<SaleBiddingDetail>
    {
        public Guid? SaleBiddingDetailId { get; set; }
        public Guid? SaleBiddingId { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<FileInFolderModel> ListFile { get; set; }

        public SaleBiddingDetailModel()
        {

        }
        public SaleBiddingDetailModel(SaleBiddingDetailModel entity)
        {
            Mapper(entity, this);
        }

        public SaleBiddingDetailModel(SaleBiddingDetailEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.SaleBiddingDetail ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.SaleBiddingDetail();
            Mapper(this, entity);
            return entity;
        }

        public SaleBiddingDetailEntityModel ToEntityModel()
        {
            var entity = new SaleBiddingDetailEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
