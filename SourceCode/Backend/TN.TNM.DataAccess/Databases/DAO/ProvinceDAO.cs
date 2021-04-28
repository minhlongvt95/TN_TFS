using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Province;
using TN.TNM.DataAccess.Messages.Results.Admin.Province;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ProvinceDAO : BaseDAO, IProvinceDataAccess
    {
        public ProvinceDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllProvinceResult GetAllProvince(GetAllProvinceParameter parameter)
        {
            var listProvince = context.Province.Select(p => new ProvinceEntityModel()
            {
                ProvinceId = p.ProvinceId,
                ProvinceName = p.ProvinceName,
                ProvinceCode = p.ProvinceCode,
                ProvinceType = p.ProvinceType
            }).OrderBy(p => p.ProvinceName).ToList();

            listProvince.ForEach(p =>
            {
                var districtList = context.District.Where(d => d.ProvinceId == p.ProvinceId)
                    .Select(d => new DistrictEntityModel()
                    {
                        DistrictId = d.DistrictId,
                        DistrictName = d.DistrictName,
                        DistrictCode = d.DistrictCode,
                        DistrictType = d.DistrictType,
                        ProvinceId = d.ProvinceId
                    }).OrderBy(d => d.DistrictName).ToList();

                districtList.ForEach(d =>
                {
                    var wardList = context.Ward.Where(w => w.DistrictId == d.DistrictId).Select(w =>
                        new WardEntityModel()
                        {
                            WardId = w.WardId,
                            WardName = w.WardName,
                            WardCode = w.WardCode,
                            WardType = w.WardType,
                            DistrictId = w.DistrictId
                        }).OrderBy(w => w.WardName).ToList();
                    d.WardList = wardList;
                });

                p.DistrictList = districtList;
            });

            return new GetAllProvinceResult() {
                ListProvince = listProvince,
                Status = true
            };
        }
    }
}
