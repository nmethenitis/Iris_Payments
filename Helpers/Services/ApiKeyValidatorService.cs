using IrisPayments.Helpers.Interfaces;

namespace IrisPayments.Helpers.Services;
public class ApiKeyValidatorService : IApiKeyValidator {

    private readonly IConfiguration _config;

    public ApiKeyValidatorService(IConfiguration config) {
        _config = config;
    }

    public bool IsValid(string apiKey) {
        return (apiKey == _config.GetValue<string>("AppSettings:AppApiKey"));
    }
}