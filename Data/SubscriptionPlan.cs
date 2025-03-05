using System;
using System.Collections.Generic;

namespace AstraReconsSaas.Data {
    public class SubscriptionPlan {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public string StripePriceIdMonthly { get; set; }
        public string StripePriceIdYearly { get; set; }
        public string Features { get; set; }
        public bool IsActive { get; set; } = true;
        public int MaxUsers { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<UserSubscription> Subscriptions { get; set; }
    }
}