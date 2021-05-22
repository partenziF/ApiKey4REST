using ApiKey4REST.JSON;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiKey4REST.Authentication {

    public static class Constant {
        public const string APIKEY_VALUE = "ApiKey4REST:Header-Key";
    }

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions> {

        private const string ProblemDetailsContentType = "application/problem+json";
        
        private readonly IApiKeyRepository _apikeyRepository;
        private readonly IConfiguration _configuration;
        public ApiKeyAuthenticationHandler( IOptionsMonitor<ApiKeyAuthenticationOptions> options , ILoggerFactory logger , UrlEncoder encoder , ISystemClock clock , IConfiguration configuration, IApiKeyRepository repository ) : base( options , logger , encoder , clock ) {
            _apikeyRepository = repository ?? throw new ArgumentNullException( nameof( repository ) );
            _apikeyRepository.Read();
            this._configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {

            var ApiKeyHeaderName = this._configuration[Constant.APIKEY_VALUE];
            if ( String.IsNullOrWhiteSpace( ApiKeyHeaderName ) ) {
                return AuthenticateResult.NoResult();
            }

            if ( !Request.Headers.TryGetValue( ApiKeyHeaderName , out var apiKeyHeaderValues ) ) {

                return  AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if ( apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace( providedApiKey ) ) {
                return AuthenticateResult.NoResult();
            }

            ApiKey validApiKey = await _apikeyRepository.Find( providedApiKey );

            if ( validApiKey != null ) {

                var claims = new List<Claim>();
                claims.Add( new Claim( ClaimTypes.Name,validApiKey.UserName) );
                claims.Add( new Claim( ClaimTypes.NameIdentifier, validApiKey.Token ) );
                claims.AddRange( validApiKey.Roles.Select( role => new Claim( ClaimTypes.Role , role ) ) );

                var identity = new ClaimsIdentity( claims , Options.AuthenticationType );
                var identities = new List<ClaimsIdentity> { identity };
                var principal = new ClaimsPrincipal( identities );
                var ticket = new AuthenticationTicket( principal , Options.Scheme );

                return AuthenticateResult.Success( ticket );

            }

            return AuthenticateResult.NoResult();

        }

        protected override async Task HandleChallengeAsync( AuthenticationProperties properties ) {
            Response.StatusCode = 401;
            Response.ContentType = ProblemDetailsContentType;
            var problemDetails = new UnauthorizedProblemDetails();
            await Response.WriteAsync( JsonSerializer.Serialize( problemDetails , DefaultJsonSerializerOptions.Options ) );
        }

        protected override async Task HandleForbiddenAsync( AuthenticationProperties properties ) {
            Response.StatusCode = 403;
            Response.ContentType = ProblemDetailsContentType;
            var problemDetails = new ForbiddenProblemDetails();
            await Response.WriteAsync( JsonSerializer.Serialize( problemDetails , DefaultJsonSerializerOptions.Options ) );
        }
    }

    public class UnauthorizedProblemDetails : ProblemDetails {
        public UnauthorizedProblemDetails( string text = null ) {
            Title = "Unauthorized";
            Detail = text;
            Status = 401;
            Type = "https://httpstatuses.com/401";

        }
    }

    public class ForbiddenProblemDetails : ProblemDetails {
        public ForbiddenProblemDetails( string text = null ) {
            Title = "Forbidden";
            Detail = text;
            Status = 403;
            Type = "https://httpstatuses.com/403";
        }
    }

}