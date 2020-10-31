using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using Avatar_API.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OpenBound_Network_Object_Library.Database.Controller;

namespace Avatar_API.Data.Services
{
    public class PackageService
    {
        private readonly IConfiguration _config;

        public PackageService(IConfiguration config)
        {
            _config = config;

        }

        public List<DonationPackage> GetPackages()
        {
            List<DonationPackage> products = new List<DonationPackage>();
            var cashPackages = _config.GetSection("CashPackages").GetChildren().ToList();

            foreach (var item in cashPackages)
            {
                products.Add(new DonationPackage() {Amount = Convert.ToInt32(item.Value), 
                                            Price = Convert.ToInt32(item.Key) });
            }

            return products;
        }

        public string GetCurrency()
        {
            return _config.GetValue<string>("AppCurrency:Currency").ToUpper();
        }

        public int GetCashValue(int price)
        {
            var cashPackages = _config.GetSection("CashPackages").GetChildren().ToList();

            foreach (var item in cashPackages)
            {
                if (item.Key == price.ToString())
                {
                    return Convert.ToInt32(item.Value);
                }
            }

            return 0;
        }

        public bool IsPackageValid(int price)
        {
            var cashPackages = _config.GetSection("CashPackages").GetChildren().ToList();

            foreach (var item in cashPackages)
            {
                if (item.Key == price.ToString())
                {
                    return true;
                }
            }

            return false;
        }

    }
}
