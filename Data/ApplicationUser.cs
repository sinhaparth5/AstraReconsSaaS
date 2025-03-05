using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AstraReconsSaas.Data {
    public class ApplicationUser: IdentityUser {
        public string FristName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string CompanyName { get; set; }
        public virtual ICollection<UserSubscription> Subscriptions { get; set; }
    }
}