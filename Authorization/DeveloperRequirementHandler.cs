using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ApiKey4REST.Authorization {
    public class DeveloperRequirementHandler : AuthorizationHandler<DeveloperRequirement> {

        protected override Task HandleRequirementAsync( AuthorizationHandlerContext context , DeveloperRequirement requirement ) {

            
            //if ( user.Identity.IsAuthenticated ) {
            if ( context.User.Identity.IsAuthenticated ) {
                if ( context.User.IsInRole( ApiKeyRoles.Developer ) ) {
                    context.Succeed( requirement );
                }
            }
            

            return Task.CompletedTask;
        }
    }
}

