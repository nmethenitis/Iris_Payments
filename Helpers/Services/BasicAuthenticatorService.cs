using IrisPayments.Helpers.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace IrisPayments.Helpers.Services;
public class BasicAuthenticatorService : IBasicAuthenticator {


    private readonly IConfiguration _config;
    public BasicAuthenticatorService(IConfiguration config) {
        _config = config;
    }

    public bool IsValid(string encodedUsernamePassword) {
        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
        var username = decodedUsernamePassword.Split(':', 2)[0];
        var password = this.Encode256(decodedUsernamePassword.Split(':', 2)[1]);
        if(username == null || password == null) {
            if(username == _config.GetValue<string>("AppSettings:Dias:Username") && password == _config.GetValue<string>("AppSettings:Dias:Password")) {
                return true;
            }
        }
        return false;
    }


    public string Encode256(string password) {
        using(SHA256 sha256Hash = SHA256.Create()) {
            // ComputeHash - returns byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for(int i = 0;i < bytes.Length;i++) {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}