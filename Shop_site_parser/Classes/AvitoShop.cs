using Shop_site_parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using Shop_site_parser.Model;

namespace Shop_site_parser.Classes
{
    class AvitoShop : IShop
    {
        private ShopModel cModel;                   //Настройки магазина
        private AvitoDBworker dbWorker;             //Класс для работы с БД
        private TelegramBot cBot;                   //Бот

        public AvitoShop(ShopModel _cModel, string _token)
        {
            //Инициализация
            try
            {
                cModel = _cModel;
                dbWorker = new AvitoDBworker(_cModel.dbName);
                cBot = new TelegramBot(_token);
            }
            catch
            {
                throw new Exception("Can't Avito parser initializating");
            }
        }

        public async void ParseSiteAsync()
        {
            try
            {
                dbWorker.ResetActualState(); //Сбрасываем значения актуальности

                //Открываем первую страницу по ссылке с параметрами поиска. На Авито суммарный результат найденного хранится в span[data-marker='page-title/count']
                var document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(cModel.link);
                var result_count = (document.QuerySelector("span[data-marker='page-title/count']") != null)
                                    ? Convert.ToInt32(document.QuerySelector("span[data-marker='page-title/count']").TextContent) : 0;

                int parse_counter = 0;  //Счетчик найденных элементов
                int page_counter = 1;   //Счетчик страниц

                while (parse_counter < result_count)
                {
                    Console.WriteLine("Page: " + page_counter + "...");
                    //Переход к конкретной странице
                    var page_documet = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(cModel.link + "&p=" + page_counter);

                    //Проверяем если количество изменилось
                    result_count = page_documet.QuerySelector("span[data-marker='page-title/count']") == null
                                    ? 0 : Convert.ToInt32(document.QuerySelector("span[data-marker='page-title/count']").TextContent);

                    //Список нужных элементов
                    var item_list = (page_documet.QuerySelector("div[data-marker='catalog-serp']") != null) ? page_documet.QuerySelector("div[data-marker='catalog-serp']").Children
                    .Where(s => s.GetAttribute("data-marker") == "item").ToList() : null;

                    if (item_list == null)
                    {
                        Console.WriteLine("Can't parse page: " + page_counter);
                        continue;
                    }

                    foreach (var item in item_list)
                    {
                        //Ищем id и ссылку каждого элемента
                        int? item_id = (item.GetAttribute("data-item-id") != null) ? Convert.ToInt32(item.GetAttribute("data-item-id")) : null;
                        string link = item.QuerySelector("a[itemprop='url']").GetAttribute("href") ?? null;

                        if (item_id == null || link == null)
                        {
                            Console.WriteLine("Can't parse item on: " + page_counter + " " + item.TagName);
                            continue;
                        }

                        var find_item = dbWorker.GetByItemID((int)item_id);
                        if (find_item == null)
                        {
                            AvitoDBModel newItem = new AvitoDBModel();
                            newItem.link = link;
                            newItem.product_id = (int)item_id;
                            newItem.actual = true;
                            int res = dbWorker.WriteItem(newItem);
                            cBot.setChatId(cModel.chatId);
                            cBot.setMessage("https://www.avito.ru" + link);
                            Console.WriteLine("Add new item " + link);
                        }
                        else
                        {
                            find_item.actual = true;
                            dbWorker.UpdateItem(find_item);
                        }
                        parse_counter++;
                    }
                    page_counter++;
                }
                dbWorker.ClearNonActual();

                Console.WriteLine("Success!");
            }
            catch
            {
                throw new Exception("Cant parse site");
            }
        }
    }
}
