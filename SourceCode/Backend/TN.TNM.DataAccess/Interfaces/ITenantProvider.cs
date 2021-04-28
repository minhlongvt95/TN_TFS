using System;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ITenantProvider
    {
        Guid GetTenantId();
    }
}
