using System.Collections.Generic;
using CoreProjectClient.Data.Models.AccountViewModels;
using Microsoft.AspNetCore.Authentication;

namespace CoreProjectClient.Data.Models.ManageViewModels
{
    public class ManageLoginsViewModel
    {
        public IList<LoginViewModel> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }
    }
}
