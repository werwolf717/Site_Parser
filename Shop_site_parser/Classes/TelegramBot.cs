using System;
using Shop_site_parser.Interfaces;
using Telegram.Bot;

namespace Shop_site_parser.Classes
{
    class TelegramBot : ITelegramBot
    {

        private string token;
        private long? chatid = null;
        private TelegramBotClient Bot; 

        public  TelegramBot(string _token)
        {
            try
            {
                token = _token;
                Bot = new TelegramBotClient(token);
                Bot.SetWebhookAsync("");
            }
            catch
            {
                throw new Exception("Can't bot initializating");
            }
        }

        public void setChatId(long _chat_id)
        {
            chatid = _chat_id;
        }

        public async void setMessage(string _message)
        {
            try
            {
                Console.WriteLine("Sending message to " + chatid);   
                await Bot.SendTextMessageAsync(chatid, _message);
            }
            catch
            {
                throw new Exception("Error: Problem with sending message");
            }
        }
    }
}
