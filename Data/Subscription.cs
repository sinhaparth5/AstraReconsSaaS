using System;

namespace AstraReconsSaas.Data {
    public class UserSubscription {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string StripeSubscriptionId { get; set; }
        public string StripeCustomerId { get; set; }
        public string BillingPeriod { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CancelledAt { get; set; }
        public DateTime? LastBillingDate { get; set; }
        public DateTime? NextBillingDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }
}