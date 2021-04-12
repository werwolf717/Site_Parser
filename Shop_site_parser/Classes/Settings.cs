using Shop_site_parser.Interfaces;
using Shop_site_parser.Model;
using System;
using System.Text.Json;
using System.IO;

namespace Shop_site_parser.Classes
{
    class Settings : ISettings
    {
        private SettingsModel mSettings;

        public Settings()
        {
            try
            {
                mSettings = JsonSerializer.Deserialize<SettingsModel>(File.ReadAllText(@"settings.json"));
            }
            catch
            {
                throw new Exception("Error: problem with settings parsing");
            }
        }

        public string getBotToken()
        {
            return mSettings.Bot.token;
        }

        public ShopModel getShopSettings(string _shopName)
        {
            try
            {
                return mSettings.Shop[_shopName];
            }
            catch
            {
                throw new Exception("Can't fint section " + _shopName + " in settings");
            }
        }
    }
}
