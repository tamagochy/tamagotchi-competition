using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tamagotchi.Competition.Consts;
using Tamagotchi.Competition.Helpers.API;

namespace Tamagotchi.Competition.Accessor
{

    public class UnAuthResult : JsonResult
    {
        public UnAuthResult(string message) :
            base(new ApiResult<List<Error>>() { Errors = new List<Error> { new Error { Message = message } } })
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }

    }

    public class AuthAccessor : IAsyncAuthorizationFilter
    {
        public AuthorizationPolicy Policy { get; }

        public AuthAccessor(AuthorizationPolicy policy)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateResult = await policyEvaluator.AuthenticateAsync(Policy, context.HttpContext);
            var authorizeResult = await policyEvaluator.AuthorizeAsync(Policy, authenticateResult, context.HttpContext, context);

            if (authorizeResult.Challenged)
            {
                // Return custom 401 result
                context.Result = new UnAuthResult(ErrorCodes.UNAUTHORIZED);
            }
            else if (authorizeResult.Forbidden)
            {
                // Return default 403 result
                context.Result = new ForbidResult(Policy.AuthenticationSchemes.ToArray());
            }
        }

    }
}
