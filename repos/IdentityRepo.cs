using System;
using System.Collections.Generic;
using webapp.interfaces;
using webapp.models;

namespace webapp.repos
{
    public class IdentityRepo  : IIdentityRepo
    {
        public List<Identity> All(){
            return new List<Identity> {
                new Identity { Id = new Guid("99908ca3-b1b8-459e-b8b3-efdd0badcc16"), Name = "Test user 1" },
                new Identity { Id = new Guid("e0d9f221-c61f-4649-8c91-334411046e1e"), Name = "Test user 2" },
            };
        }
    }
}
