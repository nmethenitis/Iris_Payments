using IrisPayments.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IrisPayments.Helpers.Services;
public class AuthorizationFilterService : IAuthorizationFilter {

    private readonly IApiKeyValidator _apiKeyValidator;
    private readonly IBasicAuthenticator _basicAuthenticator;
    private readonly IConfiguration _config;

    public AuthorizationFilterService(IApiKeyValidator apiKeyValidator, IBasicAuthenticator basicAuthenticator, IConfiguration config) {
        _apiKeyValidator = apiKeyValidator;
        _basicAuthenticator = basicAuthenticator;
        _config = config;
    }

    public void OnAuthorization(AuthorizationFilterContext context) {
        string apiKey = context.HttpContext.Request.Headers[_config.GetValue<string>("AppSettings:AppApiKeyHeaderName")];
        string authHeader = context.HttpContext.Request.Headers["Authorization"];
        bool authorized = false;
        if(apiKey != null) {
            authorized = _apiKeyValidator.IsValid(apiKey);
        } else if(authHeader != null) {
            if(authHeader.StartsWith("Basic ")) {
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                authorized = _basicAuthenticator.IsValid(encodedUsernamePassword);
            }
        }
        if(!authorized) {
            context.Result = new UnauthorizedResult();
        }
    }
}