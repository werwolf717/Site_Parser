using System.Collections.Generic;

namespace Shop_site_parser.Model
{
    class SettingsModel
    {
        public BotModel Bot { get; set; }
        public Dictionary<string, ShopModel> Shop { get; set; }
    }
}
