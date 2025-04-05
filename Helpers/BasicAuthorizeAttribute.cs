using IrisPayments.Helpers.Services;
using Microsoft.AspNetCore.Mvc;

namespace Payments.Helpers {
    public class BasicAuthorizeAttribute : ServiceFilterAttribute {
        public BasicAuthorizeAttribute() : base(typeof(AuthorizationFilterService)) { }
    }
}
