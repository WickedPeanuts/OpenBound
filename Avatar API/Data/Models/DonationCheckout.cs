using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avatar_API.Data.Services;

namespace Avatar_API.Data.Models
{
    public class DonationCheckout
    {
        public int Price { get; set; }
        public string Description { get; set; }
        public int Cash { get; private set; }

        public void DefineCashAmount(PackageService cashPackageService)
        {
            Cash = cashPackageService.GetCashValue(Price);
        }

    }
}
