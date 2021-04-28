using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class OrganizationModel : BaseModel<Organization>
    {
        public Guid? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
        public List<OrganizationModel> OrgChildList { get; set; }
        public int Level { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public bool? IsFinancialIndependence { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? SatelliteId { get; set; }
        public string OrganizationOtherCode { get; set; }

        public OrganizationModel(OrganizationEntityModel model)
        {
            Mapper(model, this);
            if (model.OrgChildList != null)
            {
                var cList = new List<OrganizationModel>();
                model.OrgChildList.ForEach(child =>
                {
                    cList.Add(new OrganizationModel(child));
                });
                this.OrgChildList = cList;
            }
        }

        public OrganizationModel(Organization entity) : base(entity)
        {

        }

        public override Organization ToEntity()
        {
            var entity = new Organization();
            Mapper(this, entity);
            return entity;
        }
    }
}
