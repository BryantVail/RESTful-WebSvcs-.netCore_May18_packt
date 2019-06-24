using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace DemoECommerceApp.Security.Authentication
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        //need this class to derive from parent for Authentication options
    }
}
