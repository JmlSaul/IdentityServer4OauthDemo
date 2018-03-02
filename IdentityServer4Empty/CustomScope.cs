using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer4Empty
{
    public class CustomScope : IdentityResource
    {
        public CustomScope()
        {
            this.Enabled = true;
            this.Name = "customscope";
            this.DisplayName = "Custom Scope";
            this.Description = "The custom scope test.";
            this.Emphasize = true;
            this.UserClaims = new[] { "custom_scope" };
        }
    }
}
