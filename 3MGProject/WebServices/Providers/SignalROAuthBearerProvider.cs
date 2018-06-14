using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace WebServices.Providers
{
    internal class SignalROAuthBearerProvider : OAuthBearerAuthenticationProvider
    {

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query.Get("access_token");

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }

            return Task.FromResult<object>(null);
        }

        
    }
}