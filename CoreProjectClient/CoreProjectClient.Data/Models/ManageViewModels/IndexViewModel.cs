using CoreProjectClient.Data.Models.AccountViewModels;
using System.Collections.Generic;

namespace CoreProjectClient.Data.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        public IList<LoginViewModel> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        public string AuthenticatorKey { get; set; }
    }
}
