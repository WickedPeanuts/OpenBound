using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avatar_API.Data.Models;
using Avatar_API.Data.Services;
using Avatar_API.Filter;
using Avatar_API.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Session;
using Stripe;

namespace Avatar_API.Controllers
{
    [Route("donation/[controller]/[action]"), ApiExplorerSettings(IgnoreApi = true)]
    public class StripeController : Controller
    {
        private readonly StripeService _stripeService;
        private readonly PackageService _packageService;
        private readonly SessionUser _sessionUser;
        private readonly DonationTransactionService _donationTransactionService;

        public StripeController(StripeService stripeService, PackageService cashService,
            SessionHandler sessionHandler, DonationTransactionService cashTransactionService)
        {
            _stripeService = stripeService;
            _packageService = cashService;
            _donationTransactionService = cashTransactionService;
            _sessionUser = sessionHandler.GetSessionUser();
        }

        [AuthorizeSessionAttribute]
        public IActionResult Donation(int price)
        {
            if (!_packageService.IsPackageValid(price)) return View("Invalid");
            ViewBag.PublishableKey = _stripeService.GetPublishableKey();
            ViewBag.Currency = _packageService.GetCurrency();
            DonationCheckout recharge = new DonationCheckout
            {
                Price = price,
                Description = $"Donation for {_packageService.GetCashValue(price)} Cash!",
            };

            return View(recharge);
        }

        [HttpPost, ValidateAntiForgeryToken, AuthorizeSessionAttribute]
        public IActionResult Checkout(string stripeToken, DonationCheckout productCheckout)
        {
            if (!_packageService.IsPackageValid(productCheckout.Price))
                return View("Invalid");

            productCheckout.DefineCashAmount(_packageService);
            var chargeOptions = new ChargeCreateOptions()
            {
                Amount = (long)(Convert.ToDouble(productCheckout.Price) * 100),
                Currency = _packageService.GetCurrency(),
                Source = stripeToken,
                Description = $"{_packageService.GetCashValue(productCheckout.Price)} Cash / Nickname: {_sessionUser.Nickname}",
                Metadata = new Dictionary<string, string>()
                {
                    { "CashAmount", productCheckout.Cash.ToString() },
                    { "Price", productCheckout.Price.ToString() },
                    { "Email", _sessionUser.Email },
                    { "Nickname", _sessionUser.Nickname }
                }
            };

            ChargeService service = new ChargeService();
            Charge charge = service.Create(chargeOptions);

            if (charge.Status == "succeeded")            
                return View("Success");
            
            return View("Failure");
        }

        [HttpPost]
        public async Task<IActionResult> Check()
        {
            string endpointSecret = _stripeService.GetWebhookSecret();
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                  Request.Headers["Stripe-Signature"], endpointSecret);

                if (stripeEvent.Type == Events.ChargeSucceeded)
                {
                    Charge charge = stripeEvent.Data.Object as Charge;
                    
                    DonationTransaction dt = new DonationTransaction()
                    {
                        CashAmount = Convert.ToInt32(charge.Metadata["CashAmount"]),
                        Price = Convert.ToInt32(charge.Metadata["Price"]),
                        PaymentMethod = "Stripe",
                        GatewayTransactionId = charge.Id
                    };

                    _donationTransactionService.CreateAndDeliver(dt, 
                                                                            charge.Metadata["Email"], 
                                                                            charge.Metadata["Nickname"]);
                }
                else
                {
                    return BadRequest();
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.Message);
                return BadRequest();
            }
        }
    }


}

