using Microsoft.AspNetCore.Authentication;

namespace ApiKey4REST.Authentication {
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions {

        public const string DefaultScheme = "API Key 4 REST";
        public bool UseCaseSensitiveKeyValue = false;
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }
}