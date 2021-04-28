using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEntityTypeProvider
    {
        IEnumerable<Type> GetEntityTypes();
    }
}
