using Microsoft.AspNetCore.Authentication;

namespace ApiKey4REST.Authentication {
    public class ApiKey4RESTAuthenticationOptions : AuthenticationSchemeOptions {

        public const string DefaultScheme = "APIKey4REST";
        public bool UseCaseSensitiveKeyValue = false;
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }

}