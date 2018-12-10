using System;
using System.Threading.Tasks;
using CardCash_API;


namespace PlaceOrder
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            String appID = "";
            String emailAddr = "";
            String pwd = "";

            var CC_API = new API(appID, false, false);

            var login = await CC_API.CustomerLogin(emailAddr, pwd);
            Console.WriteLine("CustomerLogin resp" + login);

            var CreateCart = await CC_API.CreateCart();
            Console.WriteLine("CreateCart resp" + CreateCart);

            var AddCardToCart = await CC_API.AddCardToCart(CreateCart.cartId.ToString(), 99, 50.00, "5555555122235584", "1234");
            Console.WriteLine("AddCardToCart resp" + AddCardToCart);

            var PlaceOrder = await CC_API.PlaceOrder(CreateCart.cartId.ToString(), 1, "Card", "Cash", "990 Cedar Bridge Avenue", "Brick", "NJ", "08540");
            Console.WriteLine("PlaceOrder resp " + PlaceOrder);

        }
    }
}
