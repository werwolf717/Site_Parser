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
        private ShopModel cModel;
        private List<AvitoDBModel> db_dataList;
        private AvitoDBworker dbWorker;
        private TelegramBot cBot;

        public AvitoShop(ShopModel _cModel, string _token)
        {
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
                db_dataList = dbWorker.GetDataList();

                var document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(cModel.link);
                var result_count = (document.QuerySelector("span[data-marker='page-title/count']") != null)
                                    ? Convert.ToInt32(document.QuerySelector("span[data-marker='page-title/count']").TextContent) : 0;

                int parce_counter = 0;
                int page_counter = 1;

                while (parce_counter < result_count)
                {
                    Console.WriteLine("Page: " + page_counter + "...");
                    var page_documet = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(cModel.link + "&p=" + page_counter);

                    var item_list = (page_documet.QuerySelector("div[data-marker='catalog-serp']") != null) ? page_documet.QuerySelector("div[data-marker='catalog-serp']").Children
                    .Where(s => s.GetAttribute("data-marker") == "item").ToList() : null;

                    if (item_list == null)
                    {
                        Console.WriteLine("Can't parse page: " + page_counter);
                        continue;
                    }

                    foreach (var item in item_list)
                    {
                        int? item_id = (item.GetAttribute("data-item-id") != null) ? Convert.ToInt32(item.GetAttribute("data-item-id")) : null;
                        string link = item.QuerySelector("a[itemprop='url']").GetAttribute("href") ?? null;

                        if (item_id == null || link == null)
                        {
                            Console.WriteLine("Can't parse item on: " + page_counter + " " + item.TagName);
                            continue;
                        }

                        if (!db_dataList.Any(it => it.product_id == item_id))
                        {
                            AvitoDBModel newItem = new AvitoDBModel();
                            newItem.link = link;
                            newItem.product_id = (int)item_id;
                            int res = dbWorker.WriteItem(newItem);
                            cBot.setChatId(cModel.chatId);
                            cBot.setMessage("https://www.avito.ru" + link);
                            Console.WriteLine("Add new item " + link);
                        }
                        else
                        {
                            db_dataList.RemoveAll(it => it.product_id == item_id);
                        }
                        parce_counter++;
                    }
                    page_counter++;
                }

                foreach (var item in db_dataList)
                {
                    dbWorker.RemoveItem(item.id);
                    Console.WriteLine("Remove item " + item.link);
                }

                Console.WriteLine("Success!");
            }
            catch
            {
                throw new Exception("Cant parse site");
            }
        }
    }
}
