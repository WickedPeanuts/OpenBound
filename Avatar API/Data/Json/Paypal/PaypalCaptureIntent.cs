using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avatar_API.Data.Json
{
    public class ItemTotal
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Shipping
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Handling
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Insurance
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class ShippingDiscount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Discount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Breakdown
    {
        [JsonProperty("item_total")]
        public ItemTotal ItemTotal { get; set; }

        [JsonProperty("shipping")]
        public Shipping Shipping { get; set; }

        [JsonProperty("handling")]
        public Handling Handling { get; set; }

        [JsonProperty("insurance")]
        public Insurance Insurance { get; set; }

        [JsonProperty("shipping_discount")]
        public ShippingDiscount ShippingDiscount { get; set; }

        [JsonProperty("discount")]
        public Discount Discount { get; set; }
    }

    public class Amount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("breakdown")]
        public Breakdown Breakdown { get; set; }
    }

    public class Payee
    {
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }
    }

    public class Name
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }
    }

    public class Address
    {
        [JsonProperty("address_line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("address_line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("admin_area_2")]
        public string AdminArea2 { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }

    public class Shipping2
    {
        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
    }

    public class Amount2
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class SellerProtection
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("dispute_categories")]
        public List<string> DisputeCategories { get; set; }
    }

    public class GrossAmount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class PaypalFee
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class NetAmount
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class SellerReceivableBreakdown
    {
        [JsonProperty("gross_amount")]
        public GrossAmount GrossAmount { get; set; }

        [JsonProperty("paypal_fee")]
        public PaypalFee PaypalFee { get; set; }

        [JsonProperty("net_amount")]
        public NetAmount NetAmount { get; set; }
    }

    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public class Capture
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public Amount2 Amount { get; set; }

        [JsonProperty("final_capture")]
        public bool FinalCapture { get; set; }

        [JsonProperty("seller_protection")]
        public SellerProtection SellerProtection { get; set; }

        [JsonProperty("seller_receivable_breakdown")]
        public SellerReceivableBreakdown SellerReceivableBreakdown { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }

        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }
    }

    public class Payments
    {
        [JsonProperty("captures")]
        public List<Capture> Captures { get; set; }
    }

    public class PurchaseUnit
    {
        [JsonProperty("reference_id")]
        public string ReferenceId { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("payee")]
        public Payee Payee { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("shipping")]
        public Shipping2 Shipping { get; set; }

        [JsonProperty("payments")]
        public Payments Payments { get; set; }
    }

    public class Name2
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }
    }

    public class PhoneNumber
    {
        [JsonProperty("national_number")]
        public string NationalNumber { get; set; }
    }

    public class Phone
    {
        [JsonProperty("phone_number")]
        public PhoneNumber PhoneNumber { get; set; }
    }

    public class Address2
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }

    public class Payer
    {
        [JsonProperty("name")]
        public Name2 Name { get; set; }

        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("payer_id")]
        public string PayerId { get; set; }

        [JsonProperty("phone")]
        public Phone Phone { get; set; }

        [JsonProperty("address")]
        public Address2 Address { get; set; }
    }

    public class Link2
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public class PaypalCaptureIntent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("purchase_units")]
        public List<PurchaseUnit> PurchaseUnits { get; set; }

        [JsonProperty("payer")]
        public Payer Payer { get; set; }

        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty("links")]
        public List<Link2> Links { get; set; }
    }

}
