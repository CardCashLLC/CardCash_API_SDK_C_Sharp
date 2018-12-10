using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace CardCash_API
{
    public class API
    {
        private readonly string _appID;
        private readonly bool _debug;
        private readonly Uri _uri;
        private readonly HttpClient _Client;

        private CookieContainer _CardCashCookieJar;
        private HttpClientHandler _handler;

        private HttpResponseMessage httpResponse = null;

        public API(string appID, Boolean isProduction = false, Boolean debug = false)
        {
            _debug = debug;
            _appID = appID;
            _uri = isProduction ?
              new Uri("https://production-api.cardcash.com/v3/") :
              new Uri("https://sandbox-api.cardcash.com/v3/");

            _CardCashCookieJar = new CookieContainer();

            _handler = new HttpClientHandler
            {
                CookieContainer = _CardCashCookieJar
            };

            _Client = new HttpClient(_handler, false)
            {
                BaseAddress = _uri
            };
        }

        private async Task<T> Execute<T>(HttpMethod method, string path, dynamic jsonObject = null)
        {

            var foundCCCookie = false;

            IEnumerable<Cookie> responseCookies = _CardCashCookieJar.GetCookies(_uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                if (cookie.Name == _appID)
                {
                    foundCCCookie = true;
                    if (_debug)
                    {
                        Console.WriteLine("APPID Cookie: " + cookie.Value);
                    }
                }
            }

            if (foundCCCookie == false && path != "session")
            {
                await Execute<object>(HttpMethod.Post, "session");
            }

            HttpRequestMessage request = new HttpRequestMessage(method, path);

            request.Headers.Add("x-cc-app", _appID);

            if (jsonObject != null && HttpMethod.Post == method)
            {
                if (_debug)
                {
                    Console.WriteLine("Request Details: " + request);
                    Console.WriteLine("Post Data " + jsonObject.ToString());
                }

                request.Content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            }

            httpResponse = await _Client.SendAsync(request);

            var result = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                var jObject = JObject.Parse(result);
                dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jObject.ToString());

                return jsonResponse;
            }

            if (result != null && result.Contains("{"))
            {
                var errorObj = JObject.Parse(result);
                dynamic errorJson = JsonConvert.DeserializeObject<dynamic>(errorObj.ToString());

                return errorJson;
            }

            var error = (dynamic)new JObject();
            error.errorCode = httpResponse.StatusCode;
            error.msg = result;

            return error;
        }

        public async Task<dynamic> CustomerLogin(string email, string password)
        {
            var customerLogin = (dynamic)new JObject();
            customerLogin.customer = (dynamic)new JObject();
            customerLogin.customer.email = email;
            customerLogin.customer.password = password;

            var loginResponse = await Execute<dynamic>(HttpMethod.Post, "customers/login", customerLogin);

            return loginResponse;
        }

        public async Task<dynamic> CustomerLogin(string firstName, string lastName, string email, string password)
        {
            var createCustomer = (dynamic)new JObject();
            createCustomer.customer = (dynamic)new JObject();
            createCustomer.customer.firstName = firstName;
            createCustomer.customer.lastName = lastName;
            createCustomer.customer.email = email;
            createCustomer.customer.password = password;

            var createCustomerResponse = await Execute<dynamic>(HttpMethod.Post, "customers", createCustomer);

            return createCustomerResponse;
        }

        public async Task<dynamic> GetMerchants()
        {
            var merchantResponse = await Execute<dynamic>(HttpMethod.Get, "merchants/sell");

            return merchantResponse;
        }

        public async Task<dynamic> RetrieveCart()
        {
            var getCartResponse = await Execute<dynamic>(HttpMethod.Get, "carts");

            return getCartResponse;
        }

        public async Task<dynamic> CreateCart()
        {
            var createCartObj = (dynamic)new JObject();
            createCartObj.action = "sell";

            var createCartResponse = await Execute<dynamic>(HttpMethod.Post, "carts", createCartObj);

            return createCartResponse;
        }

        public async Task<dynamic> DeleteCart(string cartID)
        {
            var deleleteCartResponse = await Execute<dynamic>(HttpMethod.Delete, "carts/" + cartID);

            return deleleteCartResponse;
        }

        public async Task<dynamic> AddCardToCart(string cartID, int merchantID, double cardValue, string cardNum = null, string cardPin = null, string refID = null)
        {
            var addCardObj = (dynamic)new JObject();
            addCardObj.card = (dynamic)new JObject();
            addCardObj.card.merchantId = merchantID;
            addCardObj.card.enterValue = cardValue;

            if (cardNum != null)
            {
                addCardObj.card.number = cardNum;
            }

            if (cardPin != null)
            {
                addCardObj.card.pin = cardPin;
            }

            if (refID != null)
            {
                addCardObj.card.refId = refID;
            }

            var addCardResponse = await Execute<dynamic>(HttpMethod.Post, "carts/" + cartID + "/cards", addCardObj);

            return addCardResponse;
        }

        public async Task<dynamic> UpdateCardInCart(string cartID, string cardID, double cardValue = 0, string cardNum = null, string cardPin = null, string refID = null)
        {
            var updateCardObj = (dynamic)new JObject();
            updateCardObj.card = (dynamic)new JObject();

            if (refID != null)
            {
                updateCardObj.card.refId = refID;
            }

            if (refID != null)
            {
                updateCardObj.card.refId = refID;
            }

            if (cardValue != 0)
            {
                updateCardObj.card.enterValue = cardValue;
            }

            if (cardNum != null)
            {
                updateCardObj.card.number = cardNum;
            }

            if (cardPin != null)
            {
                updateCardObj.card.pin = cardPin;
            }

            var updateCardResponse = await Execute<dynamic>(HttpMethod.Post, "carts/" + cartID + "/cards/" + cardID, updateCardObj);

            return updateCardResponse;
        }

        public async Task<dynamic> DeleteCardInCart(string cartID, string cardID)
        {
            var deleteCardResponse = await Execute<dynamic>(HttpMethod.Delete, "carts/" + cartID + "/cards/" + cardID);

            return deleteCardResponse;
        }

        public async Task<dynamic> PlaceOrder(string cartID, int paymentDetailID, string firstName, string lastName, string street, string city, string state, string postcode, string street2 = null)
        {
            var placeOrderObj = (dynamic)new JObject();
            placeOrderObj.billingDetails = (dynamic)new JObject();
            placeOrderObj.paymentDetails = (dynamic)new JObject();
            placeOrderObj.autoSplit = true;
            placeOrderObj.cartId = cartID;
            placeOrderObj.paymentMethod = "ACH_BULK";
            placeOrderObj.paymentDetails.id = paymentDetailID;
            placeOrderObj.billingDetails.firstname = firstName;
            placeOrderObj.billingDetails.lastname = lastName;
            placeOrderObj.billingDetails.street = street;
            placeOrderObj.billingDetails.city = city;
            placeOrderObj.billingDetails.state = state;
            placeOrderObj.billingDetails.postcode = postcode;

            if (street2 != null)
            {
                placeOrderObj.billingDetails.street2 = street2;
            }

            var orderReponse = await Execute<dynamic>(HttpMethod.Post, "orders", placeOrderObj);

            return orderReponse;
        }

        public async Task<dynamic> GetOrder(string orderID)
        {
            var getOrderResponse = await Execute<dynamic>(HttpMethod.Get, "orders/" + orderID);

            return getOrderResponse;
        }

        public async Task<dynamic> GetAllOrders()
        {
            var getOrdersResponse = await Execute<dynamic>(HttpMethod.Get, "orders/sell");

            return getOrdersResponse;
        }

        public async Task<dynamic> GetOrderCards(string orderID)
        {
            var getOrdersCardsResponse = await Execute<dynamic>(HttpMethod.Get, "cards/sell?orderId=" + orderID);

            return getOrdersCardsResponse;
        }

        public async Task<dynamic> GetAllCards()
        {
            var getAllCardsResponse = await Execute<dynamic>(HttpMethod.Get, "cards/sell");

            return getAllCardsResponse;
        }

        public async Task<dynamic> GetAllPayments()
        {
            var getAllPaymentsResponse = await Execute<dynamic>(HttpMethod.Get, "payments/sell");

            return getAllPaymentsResponse;
        }

        public async Task<dynamic> GetPayment(string paymentID)
        {
            var getPaymentResponse = await Execute<dynamic>(HttpMethod.Get, "payments/sell/" + paymentID);

            return getPaymentResponse;
        }
    }
}
