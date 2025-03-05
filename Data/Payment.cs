using System;

namespace AstraReconsSaas.Data {
    public class Payment {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int UserSubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string StripePaymentIntentId { get; set; }
        public string StripeInvoiceId { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsSuccessful { get; set; }
        public string Status { get; set; }
        public string FailureReason { get; set; }
        
        public virtual ApplicationUser User { get; set; }
        public virtual UserSubscription UserSubscription { get; set; }
    }
}