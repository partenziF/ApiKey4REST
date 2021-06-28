using ApiKey4REST.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiKey4REST.Middleware {
    public class ApiKey4RESTMiddleware {

        private readonly RequestDelegate next;
        private readonly ApiKey4RESTSettings settings;

        public ApiKey4RESTMiddleware( RequestDelegate next , IOptions<ApiKey4RESTSettings> settings ) {
            this.next = next;
            this.settings = settings.Value;
        }

        public async Task Invoke( HttpContext context , IApiKeyRepository apiKeyRepository ) {

            if ( context.Request.Path.StartsWithSegments( new PathString( "/api" ) ) ) {

                //if ( context.User.Identity.IsAuthenticated ) {

                if ( !context.Request.Headers.ContainsKey( settings.HeaderKey ) ) {
                    context.Response.StatusCode = ( int ) HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync( "Api Key was not provided." );
                    return;
                }
                //var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
                //var apiKey = appSettings.GetValue<string>( APIKEYNAME );

                if ( !context.Request.Headers.TryGetValue( settings.HeaderKey , out var apiKeyHeaderValues ) ) {
                    context.Response.StatusCode = ( int ) HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync( "Api Key was not provided." );
                    return;
                }

                if ( String.IsNullOrWhiteSpace( apiKeyHeaderValues ) ) {
                    context.Response.StatusCode = ( int ) HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync( "Invalid Api Key." );
                    return;
                }

                apiKeyRepository?.Read();
                ApiKey validApiKey = await apiKeyRepository?.Find( apiKeyHeaderValues );

                if ( validApiKey is null ) {
                    //            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync( "Invalid Api Key." );
                    return;
                }

            }

            await next( context );

        }

    }



    #region ExtensionMethod
    public static class ApiKey4RESTMiddlewareExtension {
        public static IApplicationBuilder ApplyUserKeyValidation( this IApplicationBuilder app ) {
            app.UseMiddleware<ApiKey4RESTMiddleware>();
            return app;
        }
    }
    #endregion


}
