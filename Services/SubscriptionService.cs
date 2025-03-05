using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstraReconsSaas.Data;

namespace AstraReconsSaas.Services
{
    public class SubscriptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly StripeService _stripeService;

        public SubscriptionService(ApplicationDbContext context, StripeService stripeService)
        {
            _context = context;
            _stripeService = stripeService;
        }

        public async Task<List<SubscriptionPlan>> GetAllPlansAsync()
        {
            return await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<SubscriptionPlan> GetPlanByIdAsync(int planId)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive);
        }

        public async Task<UserSubscription> GetActiveSubscriptionAsync(string userId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.SubscriptionPlan)
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<UserSubscription> CreateSubscriptionAsync(string userId, int planId, string billingPeriod)
        {
            var user = await _context.Users.FindAsync(userId);
            var plan = await _context.SubscriptionPlans.FindAsync(planId);

            if (user == null || plan == null)
                return null;

            // Check if user already has an active subscription
            var existingSubscription = await GetActiveSubscriptionAsync(userId);
            if (existingSubscription != null)
            {
                // Cancel existing subscription in Stripe
                if (!string.IsNullOrEmpty(existingSubscription.StripeSubscriptionId))
                {
                    await _stripeService.CancelSubscriptionAsync(existingSubscription.StripeSubscriptionId);
                }

                // Mark as inactive in our database
                existingSubscription.IsActive = false;
                existingSubscription.CancelledAt = DateTime.UtcNow;
                _context.UserSubscriptions.Update(existingSubscription);
            }

            // Create Stripe customer if not exists
            string stripeCustomerId = user.StripeCustomerId;
            if (string.IsNullOrEmpty(stripeCustomerId))
            {
                stripeCustomerId = await _stripeService.CreateCustomerAsync(user.Email, $"{user.FirstName} {user.LastName}");
                user.StripeCustomerId = stripeCustomerId;
                _context.Users.Update(user);
            }

            // Determine the price based on billing period
            string stripePriceId = billingPeriod.ToLower() == "yearly" 
                ? plan.StripePriceIdYearly 
                : plan.StripePriceIdMonthly;

            // Calculate end date
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = billingPeriod.ToLower() == "yearly" 
                ? startDate.AddYears(1) 
                : startDate.AddMonths(1);

            // Create subscription in Stripe
            var stripeSubscription = await _stripeService.CreateSubscriptionAsync(
                stripeCustomerId,
                stripePriceId);

            // Create subscription in our database
            var subscription = new UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = planId,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                StripeSubscriptionId = stripeSubscription.Id,
                StripeCustomerId = stripeCustomerId,
                BillingPeriod = billingPeriod,
                CreatedAt = DateTime.UtcNow,
                LastBillingDate = DateTime.UtcNow,
                NextBillingDate = endDate
            };

            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }

        public async Task<bool> CancelSubscriptionAsync(string userId, int subscriptionId)
        {
            var subscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.UserId == userId && s.IsActive);

            if (subscription == null)
                return false;

            // Cancel in Stripe
            if (!string.IsNullOrEmpty(subscription.StripeSubscriptionId))
            {
                await _stripeService.CancelSubscriptionAsync(subscription.StripeSubscriptionId);
            }

            // Update in our database
            subscription.IsActive = false;
            subscription.CancelledAt = DateTime.UtcNow;
            _context.UserSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserSubscription>> GetUserSubscriptionHistoryAsync(string userId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.SubscriptionPlan)
                .Where(us => us.UserId == userId)
                .OrderByDescending(us => us.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsSubscriptionActiveAsync(string userId)
        {
            var activeSubscription = await GetActiveSubscriptionAsync(userId);
            return activeSubscription != null;
        }
    }
}