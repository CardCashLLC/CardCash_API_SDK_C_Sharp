using System;
using System.Threading.Tasks;
using CardCash_API;


namespace GetOrders
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

            var GetAllOrders = await CC_API.GetAllOrders();
            Console.WriteLine("GetAllOrders resp " + GetAllOrders);

            var GetAllCards = await CC_API.GetAllCards();
            Console.WriteLine("GetAllCards resp " + GetAllCards);

        }
    }


}
