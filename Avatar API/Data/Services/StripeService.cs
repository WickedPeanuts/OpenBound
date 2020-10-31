using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace Avatar_API.Data.Services
{
    public class StripeService
    {
        private readonly IConfiguration _config;
        public StripeService(IConfiguration config)
        {
            _config = config;
        }

        public string GetPublishableKey()
        {
            return _config.GetValue<string>("Stripe:PublishableKey");
        }

        public string GetWebhookSecret()
        {
            return _config.GetValue<string>("Stripe:WebhookSecret");
        }

    }
}
