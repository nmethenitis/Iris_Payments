using IrisPayments.Helpers.Services;
using Microsoft.AspNetCore.Mvc;

namespace Payments.Helpers {
    public class ApiKeyAttribute : ServiceFilterAttribute {
        public ApiKeyAttribute() : base(typeof(AuthorizationFilterService)) { }
    }
}
