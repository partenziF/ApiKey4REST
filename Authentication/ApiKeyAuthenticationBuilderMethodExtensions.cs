using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Method Extension
namespace ApiKey4REST.Authentication {
    public static class ApiKeyAuthenticationBuilderMethodExtensions {

        public static AuthenticationBuilder AddApiKey4RESTSupport( this AuthenticationBuilder authenticationBuilder , Action<ApiKeyAuthenticationOptions> options ) {
            return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions , ApiKeyAuthenticationHandler>( ApiKeyAuthenticationOptions.DefaultScheme , options );
        }

    }
}
