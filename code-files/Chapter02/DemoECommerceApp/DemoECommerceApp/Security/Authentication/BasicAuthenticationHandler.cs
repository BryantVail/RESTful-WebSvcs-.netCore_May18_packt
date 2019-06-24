using DemoECommerceApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;
//using Microsoft.Health.Rest;

namespace DemoECommerceApp.Security.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private FlixOneStoreContext _context;
        
        //constructor
        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock, 
            FlixOneStoreContext context)
            :base(options, logger, encoder, clock)
        {
            //constructor body
            _context = context;
        }

        //locals
        //Header name, http header
        private const string AuthorizationHeaderName = "Authorization";
        //Scheme Text that will be inserted as plain text in: 
            //
        private const string BasicSchemeName = "Basic";

        //cannot find correct library for interface
        //private readonly IBasicAuthenticationService _authenticationService;
        
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            //1. Verify AuthorizationHeaderName is present
                    //if(Request.Headers does not contain "Authorization"
                        //return no result;
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //header not found
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            //2. Verify Header is valid
            //parses AuthenticationHeaderValue, using the constant "AuthorizationHeaderName" to return the value of the list/array
            //'out' AuthenticationHeaderValue
            if(!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                //auth header not valid
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            //3. Verify scheme name is "Basic
            //  //"BasicSchemeName is a literal constant string with the value "Basic"
            if(!BasicSchemeName.Equals(headerValue.Scheme, 
                //non-case sensitive check on the equality 
                    //between the literal string "Basic" in BasicSchemeName, and
                    //headerValue.Scheme's value to ensure it's "basic"
                StringComparison.OrdinalIgnoreCase))//end if conditional
            {
                //Authorization header is not basic 
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            //4. Fetch email&pass from header
            //if length is not >2 then auth fails
            byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
            string emailPassword = Encoding.UTF8.GetString(headerValueBytes);
            string[] parts = emailPassword.Split(":");

            //if AuthHeader does not have 2 values separated by ":"
            if (parts.Length != 2)
            {
                return Task.FromResult(AuthenticateResult
                    .Fail("Invalid Basic Authentication Header"));
            }

            string email = parts[0];
            string password = parts[1];

            //5. Validate email & pass are correct
            var customer = _context.Customers
                .SingleOrDefault(
                //foreach(customer in _context.Customers){
                //  if(customer.Email == requester.email
                //    && customer.password == requester.password)
                //  { then assign that Customer Entity info to 'customer'}
                    x => 
                        x.Email == email 
                        && 
                        x.Password == password);

            //Null; if the search for a user returned null
                //then return "invalid email and password"
            if(customer == null)
            {
                return Task
                    .FromResult(AuthenticateResult
                        .Fail("Invalid email and password"));
            }

            //.6 Create Claims with email & id
            var claims = new[]
            {
                //variables 'email' and 'password' defined after using Split(":");
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString())
            };

            //7. ClaimsIdentity creation with claims
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));

            throw new NotImplementedException();
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"http://localhost:57571\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);  
        }

    }
}
