using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppTest01.Attributes {
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method )]

    public class ApiKeyAttribute : ActionFilterAttribute {

        private const string APIKEYNAME = "ApiKey";
        public IConfiguration appSettings { get; }
        public ApiKeyAttribute( IConfiguration config ) {
            appSettings = config ?? throw new ArgumentNullException( nameof( config ) );
        }


        public override async Task OnActionExecutionAsync( ActionExecutingContext context , ActionExecutionDelegate next ) {
        

                if ( context.HttpContext.Request.Headers.TryGetValue( APIKEYNAME , out var extractedApiKey ) ) {
                //Old method, preferred to get injection object
                //var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>(); 
                //or install Microsoft.Extensions.Configuration.Binder to get extensionm method
                //var apiKey = appSettings.GetValue<string>( APIKEYNAME );      
                var apiKey = appSettings[ApiKey4REST.Authentication.Constant.APIKEY_VALUE];

                if ( !apiKey.Equals( extractedApiKey ) ) {

                    context.Result = new UnauthorizedObjectResult( "Api Key is not valid" );

                    return;
                }


            } else {

                context.Result = new UnauthorizedObjectResult( "Api Key was not provided" );
                return;

            }


            await next();
        }
    }
}
