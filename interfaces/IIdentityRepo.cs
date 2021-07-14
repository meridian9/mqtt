using System.Collections.Generic;
using webapp.models;

namespace webapp.interfaces
{
    public interface IIdentityRepo {
        List<Identity> All();
    }
}
