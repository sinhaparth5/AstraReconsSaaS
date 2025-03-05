using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AstraReconsSaas.Services
{
    public class StripeService
    {
        private readonly string _apiKey;
        private readonly string _webhookSecret;

        public StripeService(IConfiguration configuration)
        {
            _apiKey = configuration["Stripe:SecretKey"];
            _webhookSecret = configuration["Stripe:WebhookSecret"];
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task<Customer> CreateCustomerAsync(string email, string name)
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
            };

            var service = new CustomerService();
            return await service.CreateAsync(options);
        }

        public async Task<Subscription> CreateSubscriptionAsync(string customerId, string priceId)
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                    },
                },
            };

            var service = new SubscriptionService();
            return await service.CreateAsync(options);
        }

        public async Task<Subscription> CancelSubscriptionAsync(string subscriptionId)
        {
            var service = new SubscriptionService();
            return await service.CancelAsync(subscriptionId);
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(string customerId, long amount, string currency = "usd")
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                Customer = customerId,
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public Stripe.Event ConstructWebhookEvent(string json, string signatureHeader)
        {
            try
            {
                return EventUtility.ConstructEvent(json, signatureHeader, _webhookSecret);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error constructing webhook event: {ex.Message}");
            }
        }
    }
}