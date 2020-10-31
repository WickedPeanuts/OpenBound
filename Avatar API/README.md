<h2>Avatar API:</h2>

<h3>Purpose</h3>

The API allows players to make donations to the server owner and supports the following payment gateways. To use the services you have to create an account. The API uses a game account to authenticate.
- Stripe
- Paypal

<h3>Configuration </h3>

The values for this component are in the file "appsettings.json".

<h4>Paypal</h4>

Create a paypal account and insert the ClientId and Secret in the file appsettings.json.
To use paypal productively you must change "PaypalMode" to LIVE.

<h4>Stripe</h4>

In the stripe portal get your PublishableKey and SecretKey insert in the file appsettings.json. Create a webhook for your server endpoint (https://yourwebsite.com/stripe/check) and define a WebhookSecret. This must also be inserted in configuration file.

<h3>Example:</h3>

```json
 "CashPackages": {
    "5": 500,
}
```

Donation value: 5 USD /
In-game currency: 500 Cash

<h4>Donation Packages</h4>

In the configuration file, under the "CashPackages" key you can define the amount of the donation and the amount of game currency received.


<h4>Currency</h4>

Define the type of currency by the acronym of 3 digits in capital letters. This value is defined in the "AppCurrency" key

<h3>Fees</h3>

Please check the fees that the services charge in your country.

<h3>Reference links:</h3>

- [Stripe](https://stripe.com/)
- [Paypal](https://www.paypal.com/)

<h3>Testing</h3>

- Paypal uses mode. SANDBOX is for testing purposes and LIVE is for production.
- Stripe has different Publishable and Secret keys for testing.


For testing purposes please use the documentation. (Webhooks)
- [Paypal webhook simulator](https://developer.paypal.com/docs/api-basics/notifications/webhooks/simulator/)
- [Stripe test a webhook endpoint](https://stripe.com/docs/webhooks/test)
