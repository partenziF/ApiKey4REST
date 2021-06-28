using ApiKey4REST.Helpers;
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

    //public static class Constant {
    //    public const string APIKEY_VALUE = "ApiKey4REST:Header-Key";
    //}

    public class ApiKey4RESTAuthenticationHandler : AuthenticationHandler<ApiKey4RESTAuthenticationOptions> {

        private const string ProblemDetailsContentType = "application/problem+json";
        
        private readonly IApiKeyRepository apikeyRepository;
        private readonly IConfiguration configuration;

        public ApiKey4RESTSettings Settings { get; }

        public ApiKey4RESTAuthenticationHandler( IOptionsMonitor<ApiKey4RESTAuthenticationOptions> options , ILoggerFactory logger , UrlEncoder encoder , ISystemClock clock , IOptions<ApiKey4RESTSettings> settings , IApiKeyRepository repository ) : base( options , logger , encoder , clock ) {
            apikeyRepository = repository ?? throw new ArgumentNullException( nameof( repository ) );
            apikeyRepository.Read();
            this.Settings = settings.Value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {


            if ( !Request.Headers.ContainsKey( Settings.HeaderKey ) ) {
                return AuthenticateResult.Fail( "No ApiKey found." );
            }

            //var ApiKeyHeaderName = this.configuration[Constant.APIKEY_VALUE];
            //if ( String.IsNullOrWhiteSpace( ApiKeyHeaderName ) ) {
            //    return AuthenticateResult.Fail( "No ApiKey found." );
            //}

            if ( !Request.Headers.TryGetValue( Settings.HeaderKey , out var apiKeyHeaderValues ) ) {

                return AuthenticateResult.Fail( "Invalid ApiKey value." );
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if ( apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace( providedApiKey ) ) {
                return AuthenticateResult.Fail( "Invalid ApiKey value." );
            }

            ApiKey validApiKey = await apikeyRepository.Find( providedApiKey );

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

            } else {
                return AuthenticateResult.Fail( "Invalid ApiKey value." );
            }

            

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