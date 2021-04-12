namespace Shop_site_parser.Interfaces
{
    interface ISettings
    {
        public string getBotToken();
        public Model.ShopModel getShopSettings(string _shopName);
    }
}
