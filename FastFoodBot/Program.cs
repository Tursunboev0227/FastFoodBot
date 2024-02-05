namespace FastFoodBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string token = "6735145596:AAHRt_l7ubGfccokpwgV5pH5e0_d9FHpJmc";
            BotHandler handler = new BotHandler(token);

            try
            {
                await handler.BotHandle();
            }
            catch (Exception ex)
            {
                throw new Exception("Something wrong");
            }
        }
    }
}