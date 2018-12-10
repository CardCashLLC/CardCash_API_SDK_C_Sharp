# CardCash API SDK C Sharp

This is currently a simple CardCash API SDK written in C Sharp to assist bulk seller partners in communicating with CardCash Client API.


## Dependencies
* .NETStandard 2.0
* Microsoft.CSharp (>= 4.5.0)
* Newtonsoft.Json (>= 12.0.1)

## Install

Package Manager

```
PM > Install-Package cardcard_api_sdk -Version 0.0.2
```

.NETCLI

```
dotnet add package cardcard_api_sdk --version 0.0.2
```

Visual Studio

```
1. In Solution Explorer, right-click References and choose Manage NuGet Packages.
2. Choose "nuget.org" as the Package source, select the Browse tab, search for cardcard_api_sdk, select that package in the list, and select Install:
3. Accept any license prompts.
```
## Requirements

1. CardCash Bulk Seller Agreement
2. CardCash X-CC-APP-ID
3. Bulk Seller Customer Account ( Sandbox can be created via this SDK )
4. Default Bank Account w/ Default Payment Method for Bulk Seller Customer Account

## Methods

* `CustomerLogin(string email, string password)` - Logins a customer into their account.
* `CreateCustomer(string firstName, string lastName, string email, string password)` - Will Only Create a brand new customer account in sandbox.
* `GetCustomer` - Get your customer profile, this is used to get your paymentDetailID to place a order.
* `GetMerchants` - Get a list of currently accepted merchants with merchant sell percentages agreed upon with your Account Manager, you must be LOGGED IN to see your sell percentages.
* `RetrieveCart` - Will return a cart if one exists in the system.
* `CreateCart` - Will return a cart if one exists in the system with it's cards or create a new empty cart.
* `DeleteCart(string cartID)` - Delete an entire cart.
* `AddCardToCart(string cartID, int merchantID, double cardValue, string cardNum = null, string cardPin = null, string refID = null)` - Add Cart to a Cart, Cards must be added one-by-one there is no bulk method.
* `UpdateCardInCart(string cartID, string cardID, double cardValue = 0, string cardNum = null, string cardPin = null, string refID = null)` - Update a Card in a cart. 
* `DeleteCardInCart(string cartID, string cardID)` - Delete a given card in a cart.
* `PlaceOrder(string cartID, int paymentDetailID, string firstName, string lastName, string street, string city, string state, string postcode, string street2 = null)` - Place your order. 
* `GetOrder(string orderID)` - Get a single order details using the orders id
* `GetAllOrders` - Get a list of orders you've placed to CardCash.
* `GetOrderCards(string orderID)` - Get a list of cards associated with a given order
* `GetAllCards` - Get a list of all the cards that you've sold to CardCash.
* `GetAllPayments` - Get a summary of all the Payments paid to you.
* `GetPayment(string paymentID)` - Get more details for single payment.

Note: All Optional Parameters are set to null or 0. 

## Order Status
* Pending - Your order is pending and is waiting for the payment to be processed.
* Review2 - Cards are going through the various checks required for sale.
* Review3 - Possible balance issues
* Review - This is where we enter it into our inventory system/ or order has an issue and customer will be contacted
* Processing - Your order is being prepared for shipment.
* On Payment Queue - Payment is being processed
* Payment Initiated - Payment sent to customer
* Paid - Customer has been successfully paid

Note: Pending is the only available order status in Sandbox.

## Usage

You need to import the `CardCash_API` libary

You need to initialize a new API class `API(string appID, Boolean isProduction = false, Boolean debug = false)`

To Create new connection to sandbox:
```var CC_API = new API(appID, false, false);```

To Create new connection to production:
```var CC_API = new API(appID, true, false);```

To debug CardCash Cookies with CardCash help:
```var CC_API = new API(appID, false, true);```

IT IS HIGHLY RECOMMENDED TO ALWAYS START WITH A `CUSTOMER LOGIN` CALL - YOU CAN'T GET YOUR MERCHANT PRICING, SAVE YOUR CART, OR PLACE A ORDER WITHOUT BEING LOGGED IN!

`await CC_API.CustomerLogin(emailAddr, pwd);`


## Examples

[Get Orders & Get Cards](https://github.com/CardCashLLC/CardCash_API_SDK_C_Sharp/blob/master/CardCash_API/Examples/GetOrders/GetOrders/Program.cs)

[Place a Order](https://github.com/CardCashLLC/CardCash_API_SDK_C_Sharp/blob/master/CardCash_API/Examples/PlaceOrder/PlaceOrder/Program.cs)