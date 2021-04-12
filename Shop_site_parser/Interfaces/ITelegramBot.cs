namespace Shop_site_parser.Interfaces
{
    interface ITelegramBot
    {
        void setMessage(string _message);
        void setChatId(long _chat_id);
    }
}
