using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TN.TNM.DataAccess.Databases;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.DataAccess.WebTenantProvider
{
    public class WebTenantProvider : ITenantProvider
    {
        private Guid _tenantId;

        public WebTenantProvider(IHttpContextAccessor accessor, TenantContext _content)
        {
            _tenantId = Guid.Empty;
            StringValues listOrigin;
            var domain = "";
            var headerOrigin = accessor.HttpContext.Request.Headers.TryGetValue("Origin", out listOrigin);
            if (listOrigin.Count > 0)
            {
                var domainFull = listOrigin.FirstOrDefault();
                domain = domainFull.Substring(domainFull.IndexOf("//") + 2);
            }

            var tenant = _content.Tenants.FirstOrDefault(t => t.TenantHost == domain);
            if (tenant != null)
            {
                _tenantId = tenant.TenantId;
            }
        }

        public Guid GetTenantId()
        {
            return _tenantId;
        }
    }
}
