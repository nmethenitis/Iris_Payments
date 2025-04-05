namespace IrisPayments.Helpers.Interfaces;
public interface IApiKeyValidator {
    bool IsValid(string apiKey);
}