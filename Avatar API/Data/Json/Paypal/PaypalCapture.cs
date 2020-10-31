using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Amount
{
    [JsonProperty("value")]
    public string Value;

    [JsonProperty("currency_code")]
    public string CurrencyCode;
}

public class SellerProtection
{
    [JsonProperty("dispute_categories")]
    public List<string> DisputeCategories;

    [JsonProperty("status")]
    public string Status;
}

public class PaypalFee
{
    [JsonProperty("value")]
    public string Value;

    [JsonProperty("currency_code")]
    public string CurrencyCode;
}

public class GrossAmount
{
    [JsonProperty("value")]
    public string Value;

    [JsonProperty("currency_code")]
    public string CurrencyCode;
}

public class NetAmount
{
    [JsonProperty("value")]
    public string Value;

    [JsonProperty("currency_code")]
    public string CurrencyCode;
}

public class SellerReceivableBreakdown
{
    [JsonProperty("paypal_fee")]
    public PaypalFee PaypalFee;

    [JsonProperty("gross_amount")]
    public GrossAmount GrossAmount;

    [JsonProperty("net_amount")]
    public NetAmount NetAmount;
}

public class Link
{
    [JsonProperty("method")]
    public string Method;

    [JsonProperty("rel")]
    public string Rel;

    [JsonProperty("href")]
    public string Href;
}

public class Resource
{
    [JsonProperty("amount")]
    public Amount Amount;

    [JsonProperty("seller_protection")]
    public SellerProtection SellerProtection;

    [JsonProperty("update_time")]
    public DateTime UpdateTime;

    [JsonProperty("create_time")]
    public DateTime CreateTime;

    [JsonProperty("final_capture")]
    public bool FinalCapture;

    [JsonProperty("seller_receivable_breakdown")]
    public SellerReceivableBreakdown SellerReceivableBreakdown;

    [JsonProperty("links")]
    public List<Link> Links;

    [JsonProperty("id")]
    public string Id;

    [JsonProperty("status")]
    public string Status;
}

public class Link2
{
    [JsonProperty("href")]
    public string Href;

    [JsonProperty("rel")]
    public string Rel;

    [JsonProperty("method")]
    public string Method;
}

public class PaypalCapture
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("event_version")]
    public string EventVersion;

    [JsonProperty("create_time")]
    public DateTime CreateTime;

    [JsonProperty("resource_type")]
    public string ResourceType;

    [JsonProperty("resource_version")]
    public string ResourceVersion;

    [JsonProperty("event_type")]
    public string EventType;

    [JsonProperty("summary")]
    public string Summary;

    [JsonProperty("resource")]
    public Resource Resource;

    [JsonProperty("links")]
    public List<Link2> Links;
}