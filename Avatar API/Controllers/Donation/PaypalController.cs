using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Avatar_API.Data.Json;
using Avatar_API.Data.Json.Paypal;
using Avatar_API.Data.Models;
using Avatar_API.Data.Services;
using Avatar_API.Filter;
using Avatar_API.Session;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Session;
using PayPal.Api;
using SharpDX.MediaFoundation;

namespace Avatar_API.Controllers.Donation
{
    [Route("donation/[controller]/[action]"), ApiExplorerSettings(IgnoreApi = true)]
    public class PaypalController : Controller
    {
        private readonly PaypalService _paypalService;
        private readonly PackageService _packageService;
        private readonly SessionUser _sessionUser;
        private readonly DonationTransactionService _donationTransactionService;

        public PaypalController(PaypalService paypalService, PackageService packageService,
            SessionHandler sessionHandler, DonationTransactionService cashTransactionService)
        {
            _paypalService = paypalService;
            _packageService = packageService;
            _donationTransactionService = cashTransactionService;
            _sessionUser = sessionHandler.GetSessionUser();
        }

        [AuthorizeSessionAttribute]
        public IActionResult Donation(int price)
        {
            Guid paymentIdGuid = Guid.NewGuid();
            string paymentId = $"PAYID-{paymentIdGuid}";

            if (!_packageService.IsPackageValid(price)) return View("Invalid");
            ViewBag.ClientId = _paypalService.GetClientId();
            ViewBag.Price = price;
            ViewBag.Currency = _packageService.GetCurrency();
            ViewBag.PaymentId = paymentId.ToString();
            ViewBag.Description = $"Donation for {_packageService.GetCashValue(price)} Cash!";

            
            DonationCheckout donationCheckout = new DonationCheckout()
            {
                Price = price,
                Description = paymentId
            };

            donationCheckout.DefineCashAmount(_packageService);

            DonationTransaction dt = new DonationTransaction()
            {
                CashAmount = donationCheckout.Cash,
                PaymentMethod = "PayPal",
                Price = donationCheckout.Price,
                GatewayTransactionId = donationCheckout.Description
            };


            _donationTransactionService.Create(dt, _sessionUser.Email, _sessionUser.Nickname);
             

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Check()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                WebhookEvent webhookEvent = JsonConvert.DeserializeObject<WebhookEvent>(json);

                if (webhookEvent.event_type == "PAYMENT.CAPTURE.COMPLETED")
                {
                    var captureWebhook = JsonConvert.DeserializeObject<PaypalCapture>(json);
                    var captureIntent = await _paypalService.GetCaptureIntent(captureWebhook.Resource.Links[2].Href);

                    if (captureIntent.Status == "COMPLETED")
                    {
                        _donationTransactionService.DeliverByPaypalId(captureIntent.PurchaseUnits[0].Description);
                        return Ok();
                    }
                } else
                {
                    return Ok();
                }

                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}
