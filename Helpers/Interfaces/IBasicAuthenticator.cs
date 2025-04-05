namespace IrisPayments.Helpers.Interfaces;
public interface IBasicAuthenticator {
    bool IsValid(string decodedUsernamePassword);
}